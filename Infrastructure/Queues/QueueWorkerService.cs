using api.Domain.Services;

namespace api.Infrastructure.Queues
{
    public abstract class QueueWorkerService(IServiceProvider serviceProvider, ILogger<QueueWorkerService> logger) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly ILogger<QueueWorkerService> _logger = logger;

        protected string QueueName { get; set; } = default!;
        protected int MaxMessages { get; set; } = 10;
        protected int WaitDelayWhenNoMessages { get; set; } = 1;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var queueService = scope.ServiceProvider.GetRequiredService<IQueueService>();

            var queueUrl = await queueService.GetQueueUrlAsync(QueueName);

            LogInformation($"Starting polling queue : {QueueName}");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var messages = await queueService.ReceiveMessageAsync(queueUrl, MaxMessages);
                    if (messages.Count != 0)
                    {
                        foreach (var msg in messages)
                        {
                            var request = new QueueMessage
                            {
                                MessageId = msg.MessageId,
                                Message = msg.Body
                            };
                            var result = await ProcessMessageAsync(request);

                            if (result)
                            {
                                LogInformation($"{msg.MessageId} processed with success");
                                await queueService.DeleteMessageAsync(queueUrl, msg.Handle);
                            }
                        }
                    }
                    else
                    {
                        LogInformation($"No messages found. Waiting {WaitDelayWhenNoMessages} seconds");
                        await Task.Delay(WaitDelayWhenNoMessages * 1000, stoppingToken);
                    }

                }
                catch (Exception ex)
                {

                    LogError($"Error processing message: {ex.Message}");
                }

            }

            await Task.CompletedTask;
        }

        protected abstract Task<bool> ProcessMessageAsync(QueueMessage msg);

        protected void LogInformation(string message)
        {
            _logger.LogInformation(message);
        }

        protected virtual void LogError(string message)
        {
            _logger.LogError(message);
        }
    }
}
