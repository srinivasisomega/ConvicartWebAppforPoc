using Microsoft.AspNetCore.Mvc;
using ConvicartWebApp.Models;
using Microsoft.EntityFrameworkCore;
using ConvicartWebApp.Filter;
namespace ConvicartWebApp.Controllers
{
    /// <summary>
    /// Controller for managing the shopping cart functionality.
    /// </summary>
    [TypeFilter(typeof(CustomerInfoFilter))]
    public class CartController : Controller
    {
        private readonly ConvicartWarehouseContext _context;
        /// Initializes a new instance of the <see cref="CartController"/> class.
        /// <param name="context">The database context for accessing data.</param>
        public CartController(ConvicartWarehouseContext context)
        {
            _context = context;
        }


        /// Adds a specified quantity of a product to the customer's cart.
        /// <param name="productId">The ID of the product to add.</param>
        /// <param name="quantity">The quantity of the product to add.</param>
        /// <returns>A redirect to the view cart page or a NotFound result.</returns>
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

            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId.Value);

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

        // Removes a specified quantity of a product from the customer's cart.
        /// <param name="productId">The ID of the product to remove.</param>
        /// <param name="quantity">The quantity of the product to remove.</param>
        /// <returns>A redirect to the view cart page.</returns>
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
        /// Adds a specified quantity of a product to the customer's cart via a POST request.
        /// <param name="productId">The ID of the product to add.</param>
        /// <param name="quantity">The quantity of the product to add.</param>
        /// <returns>A JSON response indicating success or failure.</returns>
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

        // Removes a specified quantity of a product from the customer's cart via a POST request.
        /// <param name="productId">The ID of the product to remove.</param>
        /// <param name="quantity">The quantity of the product to remove.</param>
        /// <returns>A JSON response indicating success or failure.</returns>
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
        // Displays the customer's cart with all items and total calculations.
        /// <returns>A view displaying the cart and its details.</returns>
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

        
        // Processes the purchase of items in the cart and creates an order.
        /// <param name="cartViewModel">The view model containing cart details.</param>
        /// <returns>A redirect to the order confirmation page or the view with error messages.</returns>
        public IActionResult Purchase(CartViewModel cartViewModel)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("SignIn", "Customer");
            }

            var customer = _context.Customers.Find(customerId.Value);

            // Initialize variables
            decimal totalCost = 0; // Sum of cart item prices
            decimal discount = cartViewModel.Discount; // Percentage discount
            decimal shippingCost = cartViewModel.ShippingCost; // Fixed shipping cost
            decimal taxAmount = cartViewModel.TaxAmount; // Tax amount

            // Create a new order
            var order = new Order
            {
                CustomerId = customer.CustomerId,
                OrderDate = DateTime.Now
            };

            // Add cart items to the order and calculate the total cost
            foreach (var item in cartViewModel.CartItems)
            {
                var product = _context.Stores.Find(item.ProductId);
                if (product == null)
                {
                    ModelState.AddModelError("", $"Product with ID {item.ProductId} not found.");
                    return View(cartViewModel);
                }

                // Add the price for each item (Price * Quantity)
                totalCost += product.Price * item.Quantity;

                // Create the order item
                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    Quantity = item.Quantity,
                    imgUrl = product.imgUrl
                };

                order.OrderItems.Add(orderItem);
            }

            // Apply the discount to the total cost
            decimal discountAmount = totalCost * discount;

            // Final total after applying discount, shipping, and tax
            decimal finalTotal = totalCost - discountAmount + shippingCost + taxAmount;

            // Update the order's total amount
            order.TotalAmount = finalTotal;

            // Deduct points from the customer's balance
            if (customer.PointBalance >= finalTotal)
            {
                customer.PointBalance -= (int)finalTotal; // Deduct points
                _context.Customers.Update(customer); // Update customer in the database
            }
            else
            {
                // Redirect to the profile page if points are insufficient
                TempData["ErrorMessage"] = "Insufficient points for this purchase. Please review your balance on your profile page.";
                return RedirectToAction("Profile", "Customer");
            }

            // Save the order and order items to the database
            _context.Orders.Add(order);
            _context.SaveChanges();

            // Clear the cart after purchase
            var cartItems = _context.Cart.Where(c => c.CustomerId == customerId.Value).ToList();
            _context.Cart.RemoveRange(cartItems);
            _context.SaveChanges();

            // Redirect to confirmation page
            return RedirectToAction("OrderConfirmation");
        }

        /// Displays the order confirmation view after a successful purchase.
        /// <returns>The confirmation view.</returns>
        public IActionResult OrderConfirmation()
        {
            // Return a view to show the purchase confirmation
            return View();
        }
    }
}
