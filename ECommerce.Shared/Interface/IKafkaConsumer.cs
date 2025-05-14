using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Interface
{
    public interface IKafkaConsumer
    {
        Task ConsumeAsync<T>(string topic, Func<T, Task> onMessage, CancellationToken cancellationToken);
    }
}
