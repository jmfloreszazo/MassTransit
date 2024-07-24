using MassTransit;
using shared;

namespace confluentkafkasample;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var date = DateTimeOffset.Now;

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Worker running at: {time}", date);

            using (var scope = _scopeFactory.CreateScope())
            {
                var producer = scope.ServiceProvider.GetRequiredService<ITopicProducer<Message>>();

                await producer.Produce(new Message { Text = $"The time is {date}" }, stoppingToken);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}