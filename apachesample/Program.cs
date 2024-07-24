using Confluent.Kafka;
using MassTransit;
using shared;

// 1: docker-compose up
// 2: http://localhost:9021/ -> Create Topics -> my-task-events


namespace confluentkafkasample;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddMassTransit(mt =>
                {
                    mt.UsingInMemory((context, cfg) =>
                        cfg.ConfigureEndpoints(context, SnakeCaseEndpointNameFormatter.Instance));
                    mt.AddRider(rider =>
                    {
                        rider.AddProducer<Message>("my-task-events");
                        rider.AddConsumer<MessageConsumer>();

                        rider.UsingKafka((ctx, kafka) =>
                        {
                            kafka.Host("localhost:9092");

                            kafka.TopicEndpoint<Message>("my-task-events", Guid.NewGuid().ToString(), cfg =>
                            {
                                cfg.AutoOffsetReset = AutoOffsetReset.Earliest;
                                cfg.ConfigureConsumer<MessageConsumer>(ctx);
                            });
                        });
                    });
                });

                services.AddHostedService<Worker>();
            });
    }
}