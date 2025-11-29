using GraciaTech.ContactApi.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace GraciaTech.ContactApi.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpSettings _settings;

    public SmtpEmailSender(IOptions<SmtpSettings> options)
    {
        _settings = options.Value;
    }


    public async Task SendContactEmailAsync(ContactRequest request)
    {
        var mail = new MailMessage
        {
            From = new MailAddress(_settings.User, _settings.FromName),
            Subject = $"Nuevo mensaje: {request.Nombre}",
            Body =
                $"Nombre: {request.Nombre}\n" +
                $"Email: {request.Email}\n\n" +
                $"Mensaje:\n{request.Mensaje}"
        };

        mail.To.Add(_settings.ToEmail);
        mail.ReplyToList.Add(new MailAddress(request.Email));

        using var smtp = new SmtpClient(_settings.Host, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.User, _settings.Password),
            EnableSsl = _settings.UseSsl
        };

        await smtp.SendMailAsync(mail);
    }
}
