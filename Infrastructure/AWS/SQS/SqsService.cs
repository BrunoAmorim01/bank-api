using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using api.Domain.Services;
using Microsoft.Extensions.Options;

namespace api.Infrastructure.AWS;

public class SqsService : IQueueService
{
    private readonly AmazonSQSClient _amazonSqs;
    private readonly AwsSettings _awsSettings;

    private readonly string _queueDepositName;

    public SqsService(IOptions<AwsSettings> awsSettings)
    {
        _awsSettings = awsSettings.Value;

        var config = new BasicAWSCredentials(_awsSettings.AwsAccessKeyId, _awsSettings.AwsSecretAccessKey);
        _amazonSqs = new AmazonSQSClient(config, RegionEndpoint.GetBySystemName(_awsSettings.Region));
        _queueDepositName = _awsSettings.QueueDepositName;
    }

    public async Task<string> GetQueueUrlAsync(string queueName)
    {
        try
        {
            var response = await _amazonSqs.GetQueueUrlAsync(new GetQueueUrlRequest
            {
                QueueName = queueName
            });

            return response.QueueUrl;
        }
        catch (QueueDoesNotExistException)
        {
            //You might want to add additionale exception handling here because that may fail
            var response = await _amazonSqs.CreateQueueAsync(new CreateQueueRequest
            {
                QueueName = queueName
            });

            return response.QueueUrl;
        }
    }

    public async Task<bool> PublishToQueueAsync(string queueUrl, string message, string? messageGroupId)
    {
        await _amazonSqs.SendMessageAsync(new SendMessageRequest
        {
            MessageBody = message,
            QueueUrl = queueUrl,
            MessageGroupId = messageGroupId
        });

        return true;
    }

    public async Task<List<QueueMessage>> ReceiveMessageAsync(string queueUrl, int maxMessages = 1)
    {
        var request = new ReceiveMessageRequest
        {
            QueueUrl = queueUrl,
            MaxNumberOfMessages = maxMessages
        };

        var messages = await _amazonSqs.ReceiveMessageAsync(request);

        return messages.Messages.Select(m => new QueueMessage
        {
            MessageId = m.MessageId,
            Body = m.Body,
            Handle = m.ReceiptHandle,            
        }).ToList();
    }

    public async Task DeleteMessageAsync(string queueUrl, string id)
    {
        await _amazonSqs.DeleteMessageAsync(new DeleteMessageRequest
        {
            QueueUrl = queueUrl,
            ReceiptHandle = id
        });
    }

    public Task<bool> PublishDepositAsync(string message, string? messageGroupId)
    {
        return PublishToQueueAsync(_queueDepositName, message, messageGroupId);
    }
}

