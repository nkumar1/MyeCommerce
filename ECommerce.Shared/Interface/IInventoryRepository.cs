using ECommerce.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Interface
{
    public interface IInventoryRepository
    {
        Task DecrementStockAsync(Guid productId, List<OrderItem> items);
        Task<List<Product>> GetLowStockProductsAsync(int threshold = 10);
    }
}
