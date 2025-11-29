using GraciaTech.ContactApi.Models;

namespace GraciaTech.ContactApi.Services;

public interface IEmailSender
{
    Task SendContactEmailAsync(ContactRequest request);
}
