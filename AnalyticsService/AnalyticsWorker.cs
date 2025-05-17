using ECommerce.Shared.Interface;

namespace AnalyticsService
{
    public class AnalyticsWorker : BackgroundService
    {
        private readonly ILogger<AnalyticsWorker> _logger;
        private readonly IInventoryRepository _inventoryRepo;
        private readonly IKafkaProducer _producer;

        public AnalyticsWorker(ILogger<AnalyticsWorker> logger, IInventoryRepository inventoryRepo, IKafkaProducer producer)
        {
            _logger = logger;
            _inventoryRepo = inventoryRepo;
            _producer = producer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var lowStockProducts = await _inventoryRepo.GetLowStockProductsAsync();

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
