using api.Domain.Services;

namespace api.Application.Emails;

public class SendWelcomeEmailUseCase(IEMailService emailService)
{
    public async Task Execute(string email, string accountNumber, string accountDigit, string name)
    {
        var mailRequest = new MailRequest
        {
            ToEmail = email,
            Subject = $"Welcome {name}!",
            Body =  $"Welcome {name}! Your bank account number is {accountNumber}-{accountDigit}"
        };

        await emailService.SendBasicEmail(mailRequest);
    }
}
