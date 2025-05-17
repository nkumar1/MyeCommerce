using AnalyticsService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<AnalyticsWorker>();

var host = builder.Build();
host.Run();
