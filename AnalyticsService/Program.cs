using AnalyticsService;
using ECommerce.Shared;
using ECommerce.Shared.Interface;
using InventoryService.Infrastructure;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IKafkaProducer, KafkaProducer>();

        //Register the InventoryRepository for IInventoryRepository
        services.AddScoped<IInventoryRepository, InventoryRepository>();

        services.AddHostedService<AnalyticsWorker>();
    })
    .Build();

await host.RunAsync();
