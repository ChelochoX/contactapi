namespace GraciaTech.ContactApi.Services;

public class SmtpSettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public bool UseSsl { get; set; }
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromName { get; set; } = "Gracia Tech";
    public string ToEmail { get; set; } = string.Empty;
}
