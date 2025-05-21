using Confluent.Kafka;
using ECommerce.Shared.Interface;
using ECommerce.Shared.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace InventoryService.Infrastructure
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly IConfiguration _configuration;

        public InventoryRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task DecrementStockAsync(Guid orderId, List<OrderItem> items)
        {
            using var _connection = new SqlConnection(_configuration.GetConnectionString("InventoryDb"));
            await _connection.OpenAsync();

            // Check if the order has already been processed
            var checkCommand = new SqlCommand("SELECT COUNT(1) FROM ProcessedOrders WHERE OrderId = @OrderId", _connection);
            checkCommand.Parameters.AddWithValue("@OrderId", orderId);

            var alreadyProcessed = (int)await checkCommand.ExecuteScalarAsync() > 0;
            if (alreadyProcessed)
                return; // ✅ Skip processing

            // Begin transaction to ensure atomicity
            using var transaction = _connection.BeginTransaction();

            try
            {
                foreach (var item in items)
                {
                    var updateStockCommand = new SqlCommand(
                        "UPDATE Products SET StockCount = StockCount - @Qty WHERE Id = @ProductId",
                        _connection, transaction);
                    updateStockCommand.Parameters.AddWithValue("@Qty", item.Quantity);
                    updateStockCommand.Parameters.AddWithValue("@ProductId", item.ProductId);

                    await updateStockCommand.ExecuteNonQueryAsync();
                }

                // Record order as processed
                var insertProcessedCommand = new SqlCommand(
                    "INSERT INTO ProcessedOrders (OrderId) VALUES (@OrderId)",
                    _connection, transaction);
                insertProcessedCommand.Parameters.AddWithValue("@OrderId", orderId);

                await insertProcessedCommand.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<List<Product>> GetLowStockProductsAsync(int threshold = 10)
        {
            using var _connection = new SqlConnection(_configuration.GetConnectionString("InventoryDb"));

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
