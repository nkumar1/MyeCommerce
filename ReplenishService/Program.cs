using ReplenishService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<OrderReplenishWorker>();

var host = builder.Build();
host.Run();
