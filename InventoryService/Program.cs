using InventoryService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<InventoryWorker>();

var host = builder.Build();
host.Run();
