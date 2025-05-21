using ECommerce.Shared.Interface;
using ECommerce.Shared;
using ECommerce.Shared.Models;
using static Confluent.Kafka.ConfigPropertyNames;

namespace InventoryService
{
    public class InventoryWorker : BackgroundService
    {
        private readonly ILogger<InventoryWorker> _logger;
        private readonly IKafkaConsumer _consumer;
        private readonly IServiceProvider _serviceProvider;

        public InventoryWorker(ILogger<InventoryWorker> logger, IServiceProvider serviceProvider, IKafkaConsumer consumer)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                using var scope = _serviceProvider.CreateScope();
                var inventoryRepo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

                await _consumer.ConsumeAsync<Order>("orders-topic", async (order) =>
                {
                    foreach (var item in order.Items)
                    {
                        await inventoryRepo.DecrementStockAsync(item.ProductId, item.Quantity);
                    }
                }, stoppingToken);
            }
        }
    }
}
