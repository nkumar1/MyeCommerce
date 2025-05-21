using ECommerce.Shared.Interface;
using ECommerce.Shared;
using ReplenishService;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IKafkaConsumer>(new KafkaConsumer("replenish-group"));
        services.AddSingleton<IKafkaProducer, KafkaProducer>(); 

        services.AddHostedService<OrderReplenishWorker>();
    })
    .Build();

await host.RunAsync();