using System.Globalization;
using api.Domain.Services;

namespace api.Application.Emails;

public class SendCompletedDepositEmailUseCase(IEMailService emailService, ILogger<SendCompletedDepositEmailUseCase> logger)
{
    public async Task Execute(string email, string name, int value)
    {
        var valueInDecimal = value / 100;
        var valueInBRL = valueInDecimal.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));

        var mailRequest = new MailRequest
        {
            ToEmail = email,
            Subject = "Processed deposit",
            Body = $"Hello {name}! Your deposit of {valueInBRL} was processed successfully. "
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