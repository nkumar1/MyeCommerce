using Confluent.Kafka;
using ECommerce.Shared.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ECommerce.Shared
{
    public class KafkaConsumer: IKafkaConsumer, IDisposable
    {
        private readonly IConsumer<string, string> _consumer;

        public KafkaConsumer(string groupId)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = groupId,
                EnableAutoCommit = false,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _consumer = new ConsumerBuilder<string, string>(config).Build();
        }

        public async Task ConsumeAsync<T>(string topic, Func<T, Task> onMessage, CancellationToken cancellationToken)
        {
            _consumer.Subscribe(topic);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = _consumer.Consume(cancellationToken);
                    if (cr?.Message == null) continue;

                    var data = JsonSerializer.Deserialize<T>(cr.Message.Value);
                    await onMessage(data); //Your processing logic

                    //Manually commit the offset only after successful processing
                    _consumer.Commit(cr);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error consuming Kafka message: {ex.Message}");
                    // No commit here — Kafka will retry the same message
                }
                //var cr = _consumer.Consume(cancellationToken);
                //var data = JsonSerializer.Deserialize<T>(cr.Message.Value);
                //await onMessage(data);
                //_consumer.Commit(cr);  // ✅ Manually commit the offset

            }
        }
        public void Dispose()
        {
            _consumer?.Close();
            _consumer?.Dispose();
        }
    }
}
