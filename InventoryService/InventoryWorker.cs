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
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            using var scope = _serviceProvider.CreateScope();
            var inventoryRepo = scope.ServiceProvider.GetRequiredService<IInventoryRepository>();

            await _consumer.ConsumeAsync<Order>("orders-topic", async (order) =>
            {
                try
                {
                    _logger.LogInformation("Received order {OrderId} from region {Region}", order.Id, order.Region);
                    await inventoryRepo.DecrementStockAsync(order.Id, order.Items);
                    _logger.LogInformation("Stock decremented for order {OrderId}", order.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing order {OrderId}", order.Id);
                    // Optional: implement retry or dead-letter logic
                }
            }, stoppingToken);
        }
    }
}
