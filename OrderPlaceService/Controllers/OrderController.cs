using ECommerce.Shared.Interface;
using ECommerce.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OrderPlaceService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IKafkaProducer _producer;

        public OrderController(IKafkaProducer producer)
        {
            _producer = producer;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] Order order)
        {
            order.Id = order.Id == Guid.Empty ? Guid.NewGuid() : order.Id;

            await _producer.ProduceAsync("orders-topic", order, order.Region);
            return Ok("Order placed successfully");
        }
    }
}
