using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Interface
{
    public interface IKafkaProducer
    {
        Task ProduceAsync<T>(string topic, T message, string regionKey);
    }
}
