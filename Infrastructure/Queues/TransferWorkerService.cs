using System.Text.Json;
using api.Application.Transactions;
using Microsoft.Extensions.Options;

namespace api.Infrastructure.Queues;



public class TransferWorkerService : QueueWorkerService
{
    private readonly AwsSettings _awsSettings;
    private readonly ILogger<QueueWorkerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    public TransferWorkerService(
        IServiceProvider serviceProvider,
        ILogger<QueueWorkerService> logger,
        IOptions<AwsSettings> awsSettings
        ) : base(serviceProvider, logger)

    {
        _awsSettings = awsSettings.Value;
        QueueName = _awsSettings.QueueTransferName;
        IsFifo = true;
        _logger = logger;
        _serviceProvider = serviceProvider;

    }
    protected override async Task<bool> ProcessMessageAsync(QueueMessage queue)
    {
        try
        {
            var transaction = JsonSerializer.Deserialize<ResponseDeserialize>(queue.Message) ?? throw new Exception($"Can't process message : {queue.MessageId}");
            using var scope = _serviceProvider.CreateScope();
            var proccessTransferUseCase = scope.ServiceProvider.GetRequiredService<ProccessTransferUseCase>();
            await proccessTransferUseCase.Execute(transaction.TransactionId);
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
