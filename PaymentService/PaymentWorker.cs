using ECommerce.Shared;
using ECommerce.Shared.Interface;
using ECommerce.Shared.Models;

namespace PaymentService
{
    public class PaymentWorker : BackgroundService
    {
        private readonly ILogger<PaymentWorker> _logger;
        private readonly IKafkaConsumer _consumer = new KafkaConsumer("payment-group");

        public PaymentWorker(ILogger<PaymentWorker> logger)
        {
            _logger = logger;
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
                    Console.WriteLine($"Processing payment for OrderId: {order.Id} from region {order.Region}");
                }, stoppingToken);
            }
        }
    }
}
