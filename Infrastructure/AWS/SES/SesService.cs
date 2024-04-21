using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using api.Domain.Services;
using Microsoft.Extensions.Options;
using Amazon.Runtime;

namespace api.Infrastructure.AWS.SES;

public class SesService : IEMailService
{
    private readonly MailSettings _mailSettings;
    private readonly AwsSettings _awsSettings;
    private readonly IAmazonSimpleEmailService _mailService;

    public SesService(IOptions<MailSettings> mailSettings,
    IOptions<AwsSettings> awsSettings)
    {
        _mailSettings = mailSettings.Value;
        _awsSettings = awsSettings.Value;

        var config = new BasicAWSCredentials(_awsSettings.AwsAccessKeyId, _awsSettings.AwsSecretAccessKey);
        _mailService = new AmazonSimpleEmailServiceClient(config, RegionEndpoint.GetBySystemName(_awsSettings.Region));
    }


    public async Task SendEmailAsync(MailRequest mailRequest)
    {
        var mailBody = new Body(new Content(mailRequest.Body));
        var message = new Message(new Content(mailRequest.Subject), mailBody);
        var destination = new Destination([mailRequest.ToEmail!]);
        var request = new SendEmailRequest(_mailSettings.Mail, destination, message);
        await _mailService.SendEmailAsync(request);
    }
}
