using System.Globalization;
using api.Domain.Services;

namespace api.Application.Emails;

public class SendCompletedTransferOriginEmailUseCase(IEMailService emailService, ILogger<SendCompletedTransferOriginEmailUseCase> logger)
{
    public async Task Execute(string email, string nameDestination, int value)
    {
        var valueInDecimal = value / 100;
        var valueInBRL = valueInDecimal.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));

        var mailRequest = new MailRequest
        {
            ToEmail = email,
            Subject = "Transfer sended",
            Body = $"Hello {nameDestination}! You sended a transfer of {valueInBRL} successfully."
        };
        try
        {
            await emailService.SendBasicEmail(mailRequest);
        }
        catch (Exception e)
        {
            logger.LogError("Error sending email");
            logger.LogError(e.Message);

        }

    }
}