using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using ECommerce.Shared.Interface;

namespace ECommerce.Shared
{
    public class KafkaProducer: IKafkaProducer
    {
        private readonly IProducer<string, string> _producer;

        public KafkaProducer()
        {
            var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task ProduceAsync<T>(string topic, T message, string regionKey)
        {
            var json = JsonSerializer.Serialize(message);
            await _producer.ProduceAsync(topic, new Message<string, string> { Key = regionKey, Value = json });
        }
    }
}
