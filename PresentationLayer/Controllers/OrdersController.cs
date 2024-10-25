using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConvicartWebApp.Filter;
using ConvicartWebApp.DataAccessLayer.Data;
namespace ConvicartWebApp.PresentationLayer.Controllers
{


    [TypeFilter(typeof(CustomerInfoFilter))]
    public class OrdersController : Controller
    {
        private readonly ConvicartWarehouseContext _context;

        // Inject the database context
        public OrdersController(ConvicartWarehouseContext context)
        {
            _context = context;
        }
        [ServiceFilter(typeof(CustomerAuthorizationFilter))]
        public IActionResult OrderHistory(int? customerId)
        {
            var orders = _context.Orders
                .Where(o => o.CustomerId == customerId.Value)
                .Include(o => o.OrderItems) // Include related OrderItems
                .ToList();

            return View(orders);
        }

    }

}
