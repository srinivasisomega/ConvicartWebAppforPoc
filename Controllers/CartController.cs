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

        public IActionResult AddToCartMain(int productId, int quantity)
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
        public IActionResult RemoveFromCartMain(int productId, int quantity)
        {
            // Get the customer ID from the session
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("SignIn", "Customer"); // Redirect to sign-in if not in session
            }

            var cart = _context.Cart.Include(c => c.CartItems)
                                      .FirstOrDefault(c => c.CustomerId == customerId.Value);

            if (cart == null)
            {
                return RedirectToAction("ViewCart"); // Cart does not exist, redirect to view cart
            }

            // Find the item in the cart
            var item = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                if (item.Quantity > quantity)
                {
                    item.Quantity -= quantity; // Decrease the quantity
                }
                else
                {
                    // If quantity becomes 0 or less, remove the item from the cart
                    cart.CartItems.Remove(item);
                }

                // Save the changes to the database
                _context.Update(cart);
                _context.SaveChanges();
            }

            return RedirectToAction("ViewCart"); // Redirect back to the cart view
        }
        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var product = _context.Stores.Find(productId);
            if (product == null || quantity <= 0)
            {
                return Json(new { success = false, message = "Product not found or invalid quantity." });
            }

            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("SignUp", "Customer"); // Redirect to sign-in if not in session
            }

            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId.Value);
            var cart = _context.Cart.Include(c => c.CartItems)
                                      .FirstOrDefault(c => c.CustomerId == customer.CustomerId)
                         ?? new Cart { CustomerId = customer.CustomerId };

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

            if (_context.Cart.Any(c => c.CartId == cart.CartId))
            {
                _context.Update(cart);
            }
            else
            {
                _context.Cart.Add(cart);
            }

            _context.SaveChanges();
            return Json(new { success = true, message = "Product added to cart." });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId, int quantity)
        {
            // Get the customer ID from the session
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return Json(new { success = false, redirectUrl = Url.Action("SignIn", "Customer") }); // Redirect to sign-in if not in session
            }

            var cart = _context.Cart.Include(c => c.CartItems)
                                      .FirstOrDefault(c => c.CustomerId == customerId.Value);

            if (cart == null)
            {
                return Json(new { success = false, redirectUrl = Url.Action("ViewCart") }); // Cart does not exist
            }

            // Find the item in the cart
            var item = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                if (item.Quantity > quantity)
                {
                    item.Quantity -= quantity; // Decrease the quantity
                }
                else
                {
                    // If quantity becomes 0 or less, remove the item from the cart
                    cart.CartItems.Remove(item);
                }

                // Save the changes to the database
                _context.Update(cart);
                _context.SaveChanges();
            }

            return Json(new { success = true }); // Return success response
        }



        public IActionResult ViewCart()
        {
            // Get the customer ID from the session
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("SignUp", "Customer"); // Redirect to sign-in if not in session
            }

            // Fetch the cart for the customer
            var cart = _context.Cart.Include(c => c.CartItems)
                                      .ThenInclude(ci => ci.Product) // Include the Product details
                                      .FirstOrDefault(c => c.CustomerId == customerId.Value);

            // Log the cart and its items for debugging
            if (cart == null)
            {
                Console.WriteLine("Cart not found for customer.");
                return View(new CartViewModel()); // Return an empty model if no cart
            }
            else
            {
                Console.WriteLine($"Cart found: {cart.CartId}, Items Count: {cart.CartItems.Count}");
            }

            // Fetch customer details to apply subscription discounts
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId.Value);

            // Default shipping cost and tax rate
            decimal taxRate = 0.05m; // 5% tax
            decimal shippingCost = 10.00m; // Fixed shipping cost

            // Calculate discount based on customer's subscription
            decimal discount = customer?.Subscription switch
            {
                "Gold" => 0.20m,   // 20% discount for Gold subscribers
                "Silver" => 0.10m, // 10% discount for Silver subscribers
                "Bronze" => 0.05m, // 5% discount for Bronze subscribers
                _ => 0m            // No discount for non-subscribers
            };

            // Calculate subtotal (total amount before tax, discount, and shipping)
            decimal subtotal = cart.CartItems.Sum(item => item.TotalPrice); // This should now calculate correctly

            // Log the subtotal
            Console.WriteLine($"Subtotal: {subtotal}");

            // Apply discount to subtotal
            decimal discountAmount = subtotal * discount;  // Amount to discount
            decimal discountedSubtotal = subtotal - discountAmount;  // Subtotal after discount

            // Debugging output
            Console.WriteLine($"Discount Amount: {discountAmount}");
            Console.WriteLine($"Discounted Subtotal: {discountedSubtotal}");

            // Calculate tax based on discounted subtotal
            decimal taxAmount = discountedSubtotal * taxRate;

            // Calculate final total
            decimal finalTotal = discountedSubtotal + taxAmount + shippingCost;

            // Populate the CartViewModel
            var viewModel = new CartViewModel
            {
                CartItems = cart.CartItems ?? new List<CartItem>(),
                TaxRate = taxRate,
                ShippingCost = shippingCost,
                Discount = discount,
                DiscountedSubtotal = discountedSubtotal,
                TaxAmount = taxAmount,
                FinalTotal = finalTotal
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

            return View(viewModel); // Ensure you're passing the viewModel to the view
        }
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
                // Fetch product details from the database using the ProductId
                var product = _context.Stores.Find(item.ProductId);
                if (product == null)
                {
                    // Handle product not found scenario
                    ModelState.AddModelError("", $"Product with ID {item.ProductId} not found.");
                    return View(cartViewModel); // Return to cart view with an error
                }

                // Create the order item
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = product.ProductName, // Set the product name
                    Price = product.Price, // Set the price
                    Quantity = item.Quantity,
                    imgUrl = product.imgUrl
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

            // Clear the cart after purchase (assuming you have a Cart table)
            var cartItems = _context.Cart.Where(c => c.CustomerId == customerId.Value).ToList();
            _context.Cart.RemoveRange(cartItems); // Remove all items from the cart
            _context.SaveChanges(); // Save changes to remove items

            // Redirect to a temporary confirmation page
            return RedirectToAction("OrderConfirmation"); // Redirect to confirmation page
        }



        // Confirmation action method
        public IActionResult OrderConfirmation()
        {
            // Return a view to show the purchase confirmation
            return View();
        }

        // Redirect to store after a delay (in the view for OrderConfirmation)




    }
}