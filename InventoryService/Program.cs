using ECommerce.Shared;
using ECommerce.Shared.Interface;
using InventoryService;
using InventoryService.Infrastructure;

//var builder = Host.CreateApplicationBuilder(args);

////(AddHostedService), which by default has a singleton lifetime. 
//builder.Services.AddHostedService<InventoryWorker>();

//var host = builder.Build();

//host.Run();
//builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IKafkaConsumer>(new KafkaConsumer("inventory-group"));

        // ✅ Register your InventoryRepository
        services.AddScoped<IInventoryRepository, InventoryRepository>();

        services.AddHostedService<InventoryWorker>();
    })
    .Build();

await host.RunAsync();
