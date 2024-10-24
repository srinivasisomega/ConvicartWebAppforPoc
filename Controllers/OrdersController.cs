using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConvicartWebApp.Models;
using ConvicartWebApp.Filter;
namespace ConvicartWebApp.Controllers
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
        public IActionResult OrderHistory()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("SignUp", "Customer");
            }

            var orders = _context.Orders
                .Where(o => o.CustomerId == customerId.Value)
                .Include(o => o.OrderItems) // Include related OrderItems
                .ToList();

            return View(orders);
        }

    }

}
