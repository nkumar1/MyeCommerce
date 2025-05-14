using ECommerce.Shared.Interface;
using ECommerce.Shared;
using ECommerce.Shared.Models;

namespace InventoryService
{
    public class InventoryWorker : BackgroundService
    {
        private readonly ILogger<InventoryWorker> _logger;
        private readonly IKafkaConsumer _consumer = new KafkaConsumer("inventory-group");
        private readonly IInventoryRepository _inventoryRepo;

        public InventoryWorker(ILogger<InventoryWorker> logger, IInventoryRepository inventoryRepo)
        {
            _logger = logger;
            _inventoryRepo = inventoryRepo;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                await _consumer.ConsumeAsync<Order>("orders-topic", async (order) =>
                {
                    foreach (var item in order.Items)
                    {
                        await _inventoryRepo.DecrementStockAsync(item.ProductId, item.Quantity);
                    }
                }, stoppingToken);
            }
        }
    }
}
