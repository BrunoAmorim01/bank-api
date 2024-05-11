namespace api.Domain.Services;
public class QueueMessage
{
    public required string MessageId { get; set; }
    public required string Body { get; set; }
    public required string Handle { get; set; }
}
public interface IQueueService
{
    Task<string> GetQueueUrlAsync(string queueName, bool isFifo);
    Task<bool> PublishToQueueAsync(string queueUrl, string message, string? messageGroupId);
    Task<bool> PublishDepositAsync(string message, string? messageGroupId);
    Task<bool> PublishTransferAsync(string message, string? messageGroupId);
    Task<List<QueueMessage>> ReceiveMessageAsync(string queueUrl, int maxMessages = 1);
    Task DeleteMessageAsync(string queueUrl, string id);
}
