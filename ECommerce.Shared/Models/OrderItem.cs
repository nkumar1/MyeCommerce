using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Models
{
    public class OrderItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
