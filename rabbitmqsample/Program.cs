using MassTransit;
using rabbitmqsample;

// docker run -p 15672:15672 -p 5672:5672 masstransit/rabbitmq

CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args)
{
    return Host.CreateDefaultBuilder(args)
        .ConfigureServices((hostContext, services) =>
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<MessageConsumer>();

                x.UsingRabbitMq((context, cfg) => { cfg.ConfigureEndpoints(context); });
            });

            services.AddHostedService<Worker>();
        });
}