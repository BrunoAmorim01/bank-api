namespace api.Domain.Services;

public class MailRequest
{
    public string? ToEmail { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
}

public interface IEMailService
{
    Task SendEmailAsync(MailRequest mailRequest);
}