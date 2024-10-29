using Microsoft.AspNetCore.Mvc;
using ConvicartWebApp.Filter;
using ConvicartWebApp.PresentationLayer.ViewModels;
using ConvicartWebApp.BussinessLogicLayer.Interface;
namespace ConvicartWebApp.PresentationLayer.Controllers
{
    /// <summary>
    /// Controller for managing the shopping cart functionality.
    /// </summary>
    [TypeFilter(typeof(CustomerInfoFilter))]
    [SessionAuthorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        public IActionResult AddToCartMain(int productId, int quantity)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null) return RedirectToAction("SignIn", "Customer");

            _cartService.AddToCartMain(customerId.Value, productId, quantity);
            return RedirectToAction("ViewCart");
        }

        public IActionResult RemoveFromCartMain(int productId, int quantity)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null) return RedirectToAction("SignIn", "Customer");

            _cartService.RemoveFromCartMain(customerId.Value, productId, quantity);
            return RedirectToAction("ViewCart");
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null) return RedirectToAction("SignUp", "Customer");

            var success = _cartService.AddToCart(customerId.Value, productId, quantity);
            return Json(new { success });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId, int quantity)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null) return Json(new { success = false, redirectUrl = Url.Action("SignIn", "Customer") });

            var success = _cartService.RemoveFromCart(customerId.Value, productId, quantity);
            return Json(new { success });
        }

        public IActionResult ViewCart()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null) return RedirectToAction("SignUp", "Customer");

            var viewModel = _cartService.GetCartDetails(customerId.Value);
            return View(viewModel);
        }

        public IActionResult Purchase(CartViewModel cartViewModel)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null) return RedirectToAction("SignIn", "Customer");

            var success = _orderService.Purchase(customerId.Value, cartViewModel);
            return success ? View("OrderConfirmation") : View("Failure");
        }
    }


}
