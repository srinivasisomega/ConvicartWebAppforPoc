using ConvicartWebApp.BussinessLogicLayer.Interface;
using ConvicartWebApp.DataAccessLayer.Data;
using ConvicartWebApp.DataAccessLayer.Models;
using ConvicartWebApp.PresentationLayer.ViewModels;
using Microsoft.EntityFrameworkCore;
namespace ConvicartWebApp.BussinessLogicLayer.Services
{

    public class OrderService : IOrderService
    {
        private readonly ConvicartWarehouseContext _context;

        public OrderService(ConvicartWarehouseContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetOrderHistoryAsync(int customerId)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.OrderItems)
                .ToListAsync();
        }
        public bool Purchase(int customerId, CartViewModel cartViewModel)
        {
            // Find the customer and validate existence
            var customer = _context.Customers.Find(customerId);
            if (customer == null) return false;

            // Validate cart items
            if (cartViewModel?.CartItems == null || !cartViewModel.CartItems.Any())
            {
                return false;
            }

            // Explicitly load products for each cart item to ensure data is available
            foreach (var cartItem in cartViewModel.CartItems)
            {
                _context.Entry(cartItem).Reference(ci => ci.Product).Load();
            }

            // Calculate total cost, discount, and final total
            decimal totalCost = cartViewModel.CartItems.Sum(item => item.TotalPrice);
            decimal discountAmount = totalCost * cartViewModel.Discount;
            decimal finalTotal = totalCost - discountAmount + cartViewModel.ShippingCost + cartViewModel.TaxAmount;

            // Verify customer has sufficient points
            if (customer.PointBalance < finalTotal)
            {
                return false;
            }

            // Deduct final total from customer points balance
            customer.PointBalance -= (int)finalTotal;
            _context.Customers.Update(customer);

            // Create a new order and populate order items from cart items
            var order = new Order
            {
                CustomerId = customer.CustomerId,
                OrderDate = DateTime.Now,
                Status = "OrderPlaced",
                TotalAmount = finalTotal,
                OrderItems = cartViewModel.CartItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product?.ProductName ?? "Unknown Product",
                    Price = item.Product?.Price ?? 0,
                    Quantity = item.Quantity,
                    ProductImage = item.Product?.ProductImage
                }).ToList()
            };

            // Save the order and clear the cart items
            _context.Orders.Add(order);
            _context.SaveChanges();

            // Remove the customer's cart items after purchase
            _context.Cart.RemoveRange(_context.Cart.Where(c => c.CustomerId == customerId));
            _context.SaveChanges();

            return true;
        }
        public async Task<bool> CancelOrderAsync(int orderId, int customerId)
        {
            // Find the order based on orderId and ensure it belongs to the customer
            var order = await _context.Orders
                .Where(o => o.OrderId == orderId && o.CustomerId == customerId && o.Status == "OrderPlaced")
                .FirstOrDefaultAsync();

            if (order == null) return false; // Order not found or status is not "OrderPlaced"

            // Retrieve the customer to update the point balance
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null) return false;

            // Add the order's total amount back to the customer's point balance
            customer.PointBalance += (int)order.TotalAmount;

            // Update order status to "Cancelled"
            order.Status = "Cancelled";

            // Update customer and order in the database
            _context.Customers.Update(customer);
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
