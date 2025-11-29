namespace GraciaTech.ContactApi.Models;

public class ContactRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
}
