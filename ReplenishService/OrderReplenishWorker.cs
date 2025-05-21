using ECommerce.Shared.Interface;
using ECommerce.Shared;
using ECommerce.Shared.Models;

namespace ReplenishService
{
    public class OrderReplenishWorker : BackgroundService
    {
        private readonly ILogger<OrderReplenishWorker> _logger;
        private readonly IKafkaConsumer _consumer;
        private readonly IKafkaProducer _producer;


        public OrderReplenishWorker(ILogger<OrderReplenishWorker> logger, IKafkaConsumer consumer, IKafkaProducer producer)
        {
            _logger = logger;
            _consumer = consumer;
            _producer = producer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await _consumer.ConsumeAsync<Product>("low-stock-topic", async (product) =>
            {
                var replenishOrder = new Order
                {
                    Region = product.Region,
                    Items = new List<OrderItem> { new OrderItem { ProductId = product.Id, Quantity = 100 } }
                };
                await _producer.ProduceAsync("orders-topic", replenishOrder, product.Region);
            }, stoppingToken);

        }
    }
}
