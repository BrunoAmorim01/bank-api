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
    private readonly string _queueTransferName;

    public SqsService(IOptions<AwsSettings> awsSettings)
    {
        _awsSettings = awsSettings.Value;

        var config = new BasicAWSCredentials(_awsSettings.AwsAccessKeyId, _awsSettings.AwsSecretAccessKey);
        _amazonSqs = new AmazonSQSClient(config, RegionEndpoint.GetBySystemName(_awsSettings.Region));
        _queueDepositName = _awsSettings.QueueDepositName;
        _queueTransferName = _awsSettings.QueueTransferName;
    }

    public async Task<string> GetQueueUrlAsync(string queueName, bool isFifo = false)
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
            var createRequest = new CreateQueueRequest
            {
                QueueName = queueName,               
            };

            if (isFifo)
            {
                createRequest.Attributes.Add("FifoQueue", "true");
                createRequest.Attributes.Add("ContentBasedDeduplication", "true");
            }
            
            var response = await _amazonSqs.CreateQueueAsync(createRequest);

            return response.QueueUrl;
        }
        catch (Exception e)
        {
            throw new Exception($"Error getting queue url: {e.Message}");
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
            MaxNumberOfMessages = maxMessages,
            WaitTimeSeconds = 5
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

    public Task<bool> PublishTransferAsync(string message, string? messageGroupId)
    {
        return PublishToQueueAsync(_queueTransferName, message, messageGroupId);
    }
}

