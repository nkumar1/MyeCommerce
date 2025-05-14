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
    public class KafkaConsumer: IKafkaConsumer
    {
        private readonly IConsumer<string, string> _consumer;

        public KafkaConsumer(string groupId)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _consumer = new ConsumerBuilder<string, string>(config).Build();
        }

        public async Task ConsumeAsync<T>(string topic, Func<T, Task> onMessage, CancellationToken cancellationToken)
        {
            _consumer.Subscribe(topic);
            while (!cancellationToken.IsCancellationRequested)
            {
                var cr = _consumer.Consume(cancellationToken);
                var data = JsonSerializer.Deserialize<T>(cr.Message.Value);
                await onMessage(data);
            }
        }
    }
}
