using System.Net.Http.Headers;
using System.Text.Json;
using api;
using api.Domain.Services;
using Microsoft.Extensions.Options;

public class MailerSendService : IEMailService
{
    private readonly MailSettings _mailSettings;
    private readonly ILogger<MailerSendService> _logger;
    public MailerSendService(
        IOptions<MailSettings> _mailSettings,
         ILogger<MailerSendService> _logger
        )
    {
        this._mailSettings = _mailSettings.Value;
        this._logger = _logger;
    }
    public async Task SendBasicEmail(MailRequest mailRequest)
    {
        var body = new
        {
            from = new
            {
                email = _mailSettings.Mail,
                name = _mailSettings.DisplayName
            },
            to = new[]
            {
                new
                {
                    email = mailRequest.ToEmail
                }
            },
            subject = mailRequest.Subject,
            text = mailRequest.Body
        };

        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _mailSettings.MailerSendApiKey);
        var response = await client.PostAsJsonAsync($"{_mailSettings.MailerSendBaseUrl}/email", body);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogError(content);

            throw new Exception("Error sending email");
        }


    }
}