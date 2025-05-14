using ECommerce.Shared.Interface;
using ECommerce.Shared.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace InventoryService.Infrastructure
{
    public class InventoryRepository: IInventoryRepository
    {
        private readonly SqlConnection _connection;

        public InventoryRepository(IConfiguration configuration)
        {
            _connection = new SqlConnection(configuration.GetConnectionString("InventoryDb"));
        }

        public async Task DecrementStockAsync(Guid productId, int quantity)
        {
            var query = "UPDATE Products SET StockCount = StockCount - @Quantity WHERE Id = @ProductId";
            using var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Quantity", quantity);
            command.Parameters.AddWithValue("@ProductId", productId);

            await _connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
            await _connection.CloseAsync();
        }

        public async Task<List<Product>> GetLowStockProductsAsync(int threshold = 10)
        {
            var query = "SELECT Id, Name, StockCount, Region FROM Products WHERE StockCount < @Threshold";
            using var command = new SqlCommand(query, _connection);
            command.Parameters.AddWithValue("@Threshold", threshold);

            var products = new List<Product>();

            await _connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                products.Add(new Product
                {
                    Id = reader.GetGuid(0),
                    Name = reader.GetString(1),
                    StockCount = reader.GetInt32(2),
                    Region = reader.GetString(3)
                });
            }

            await _connection.CloseAsync();
            return products;
        }
    }
}
