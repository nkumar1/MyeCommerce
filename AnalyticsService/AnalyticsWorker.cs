using ECommerce.Shared.Interface;

namespace AnalyticsService
{
    public class AnalyticsWorker : BackgroundService
    {
        private readonly ILogger<AnalyticsWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IKafkaProducer _producer;

        public AnalyticsWorker(ILogger<AnalyticsWorker> logger, IServiceProvider serviceProvider, IKafkaProducer producer)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _producer = producer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceProvider.CreateScope();
                var inventoryRepo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

                var lowStockProducts = await inventoryRepo.GetLowStockProductsAsync();

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                foreach (var product in lowStockProducts)
                {
                    await _producer.ProduceAsync("low-stock-topic", product, product.Region);
                }
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }
    }
}
