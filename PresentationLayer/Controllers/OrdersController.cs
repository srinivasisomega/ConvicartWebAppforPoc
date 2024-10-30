using Microsoft.AspNetCore.Mvc;
using ConvicartWebApp.Filter;
using ConvicartWebApp.BussinessLogicLayer.Interface;
namespace ConvicartWebApp.PresentationLayer.Controllers
{
    [TypeFilter(typeof(CustomerInfoFilter))]
    [SessionAuthorize]
    public class OrdersController : Controller
    {
        private readonly IOrderService OrderService;

        // Inject the order service
        public OrdersController(IOrderService orderService)
        {
            OrderService = orderService;
        }

        [ServiceFilter(typeof(CustomerAuthorizationFilter))]
        public async Task<IActionResult> OrderHistory(int? customerId)
        {
            if (!customerId.HasValue)
            {
                return BadRequest("Customer ID is required");
            }

            var orders = await OrderService.GetOrderHistoryAsync(customerId.Value);
            return View(orders);
        }
        [HttpPost]
        [ServiceFilter(typeof(CustomerAuthorizationFilter))]
        public async Task<IActionResult> CancelOrder(int orderId, int? customerId)
        {
            if (!customerId.HasValue)
            {
                return BadRequest("Customer ID is required");
            }

            // Attempt to cancel the order
            var success = await OrderService.CancelOrderAsync(orderId, customerId.Value);
            if (!success)
            {
                return BadRequest("Order cannot be cancelled. It may already be processed or does not exist.");
            }

            return Ok("Order successfully cancelled, and points refunded.");
        }

    }


}
