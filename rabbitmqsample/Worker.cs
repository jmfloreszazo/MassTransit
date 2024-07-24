using MassTransit;
using shared;

namespace rabbitmqsample;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IBus _bus;

    public Worker(ILogger<Worker> logger, IBus bus)
    {
        _logger = logger;
        _bus = bus;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var date = DateTimeOffset.Now;

            if (_logger.IsEnabled(LogLevel.Information))
                _logger.LogInformation("Worker running at: {time}", date);

            //Message Publisher
            await _bus.Publish(new Message { Text = $"The time is {date}" }, stoppingToken);

            await Task.Delay(1000, stoppingToken);
        }
    }
}