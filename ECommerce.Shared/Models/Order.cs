using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string? Region { get; set; }
        public List<OrderItem> Items { get; set; }
    }
}
