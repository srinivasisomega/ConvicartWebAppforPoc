using Microsoft.AspNetCore.Mvc;
using ConvicartWebApp.Models;

namespace ConvicartWebApp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ConvicartWarehouseContext _context;

        public CustomerController(ConvicartWarehouseContext context)
        {
            _context = context;
        }
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp([Bind("Name,Number,Email,Password")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(SignUp)); 
            }
            return View("Subscription"); 
        }
       



    }
}