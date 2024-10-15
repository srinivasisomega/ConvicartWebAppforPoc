using Microsoft.AspNetCore.Mvc;
using ConvicartWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
namespace ConvicartWebApp.Controllers
{


    
        public class CartController : Controller
        {
            private readonly ConvicartWarehouseContext _context;

            public CartController(ConvicartWarehouseContext context)
            {
                _context = context;
            }

            // Action to add product to the cart
            [HttpPost]
            public IActionResult AddToCart(int productId, int quantity)
            {
                var product = _context.Stores.Find(productId);
                if (product == null || quantity <= 0)
                {
                    return NotFound();
                }

                // Get the customer ID from the session
                var customerId = HttpContext.Session.GetInt32("CustomerId");
                if (customerId == null)
                {
                    return RedirectToAction("SignIn", "Customer"); // Redirect to sign-in if not in session
                }

                var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId.Value); // Find customer by session CustomerId

                // Find or create the cart for the customer
                var cart = _context.Cart.Include(c => c.CartItems)
                                          .FirstOrDefault(c => c.CustomerId == customer.CustomerId)
                             ?? new Cart { CustomerId = customer.CustomerId };

                // Check if item is already in the cart
                var existingItem = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity; // Update quantity
                }
                else
                {
                    cart.CartItems.Add(new CartItem
                    {
                        ProductId = productId, 
                        Quantity = quantity
                    });
                }

                // Save changes to the database
                if (_context.Cart.Any(c => c.CartId == cart.CartId))
                {
                    _context.Update(cart);
                }
                else
                {
                    _context.Cart.Add(cart);
                }

                _context.SaveChanges();
                return RedirectToAction("ViewCart");
            }

        public IActionResult ViewCart()
        {
            // Get the customer ID from the session
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("SignIn", "Customer"); // Redirect to sign-in if not in session
            }

            var cart = _context.Cart.Include(c => c.CartItems)
                                      .FirstOrDefault(c => c.CustomerId == customerId.Value);

            var viewModel = new CartViewModel
            {
                CartItems = cart?.CartItems ?? new List<CartItem>()
            };

            // Populate the product names for each cart item from the Store
            foreach (var item in viewModel.CartItems)
            {
                var product = _context.Stores.FirstOrDefault(p => p.ProductId == item.ProductId);
                if (product != null)
                {
                    viewModel.ProductNames[item.ProductId] = product.ProductName;
                }
            }

            return View(viewModel);
        }



        // Action to handle purchase
        [HttpPost]
            public IActionResult Purchase(CartViewModel cartViewModel)
            {
                // Get the customer ID from the session
                var customerId = HttpContext.Session.GetInt32("CustomerId");
                if (customerId == null)
                {
                    return RedirectToAction("SignIn", "Customer"); // Redirect to sign-in if not in session
                }

                var customer = _context.Customers.Find(customerId.Value); // Fetch customer details

                // Calculate the total cost from the cart items
                decimal totalCost = cartViewModel.TotalAmount;

                // Create a new order
                var order = new Order
                {
                    CustomerId = customer.CustomerId,
                    TotalAmount = totalCost,
                    OrderDate = DateTime.Now
                };

                // Add cart items to the order
                foreach (var item in cartViewModel.CartItems)
                {
                    var orderItem = new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity
                    };
                    order.OrderItems.Add(orderItem); // Add item to the order
                }

                // Deduct points from the customer based on total cost
                if (customer.PointBalance >= totalCost)
                {
                    customer.PointBalance -= (int)totalCost; // Deduct the points
                    _context.Customers.Update(customer); // Update customer points
                }
                else
                {
                    // Handle insufficient points scenario (e.g., return an error message)
                    ModelState.AddModelError("", "Insufficient points for this purchase.");
                    return View(cartViewModel); // Return to cart view with an error
                }

                // Save the order and order items to the database
                _context.Orders.Add(order);
                _context.SaveChanges(); // Commit the transaction

                // Optionally, clear the cart after purchase
                // _context.Carts.Remove(cart); 
                // _context.SaveChanges();

                return RedirectToAction("OrderConfirmation"); // Redirect to a confirmation page
            }
        }
    


}
