using System.Text.Json;
using api.Application.Transactions;
using Microsoft.Extensions.Options;

namespace api.Infrastructure.Queues;

public class QueueMessage
{
    public required string MessageId { get; set; }
    public required string Message { get; set; }
}

public class ResponseDeserialize
{
    public Guid TransactionId { get; set; }

}

public class DepositWorkerService : QueueWorkerService
{
    private readonly AwsSettings _awsSettings;
    private readonly ILogger<QueueWorkerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    public DepositWorkerService(
        IServiceProvider serviceProvider,
        ILogger<QueueWorkerService> logger,
        IOptions<AwsSettings> awsSettings
        ) : base(serviceProvider, logger)

    {
        _awsSettings = awsSettings.Value;
        QueueName = _awsSettings.QueueDepositName;
        IsFifo = true;
        _logger = logger;
        _serviceProvider = serviceProvider;

    }
    protected override async Task<bool> ProcessMessageAsync(QueueMessage queue)
    {
        try
        {
            var transaction = JsonSerializer.Deserialize<ResponseDeserialize>(queue.Message);
            if (transaction is null)
            {
                throw new Exception($"Can't process message : {queue.MessageId}");
            }

            using var scope = _serviceProvider.CreateScope();
            var proccessDepositUseCase = scope.ServiceProvider.GetRequiredService<ProccessDepositUseCase>();
            await proccessDepositUseCase.Execute(transaction.TransactionId);
            scope.Dispose();

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError("Error processing message: {0}", e.Message);
            _logger.LogError("StackTrace: {0}", e.StackTrace);
            _logger.LogError("InnerException: {0}", e.InnerException);
            return false;
        }
    }
}
