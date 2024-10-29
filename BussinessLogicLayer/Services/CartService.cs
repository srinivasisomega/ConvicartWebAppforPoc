using ConvicartWebApp.DataAccessLayer.Data;
using ConvicartWebApp.DataAccessLayer.Models;
using ConvicartWebApp.PresentationLayer.ViewModels;
using Microsoft.EntityFrameworkCore;
using ConvicartWebApp.BussinessLogicLayer.Interface;
namespace ConvicartWebApp.BussinessLogicLayer.Services
{
    public class CartService : ICartService
    {
        private readonly ConvicartWarehouseContext _context;

        public CartService(ConvicartWarehouseContext context)
        {
            _context = context;
        }

        public bool AddToCartMain(int customerId, int productId, int quantity)
        {
            var product = _context.Stores.Find(productId);
            if (product == null || quantity <= 0) return false;

            var cart = GetOrCreateCart(customerId);

            var existingItem = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            SaveOrUpdateCart(cart);
            return true;
        }

        public bool RemoveFromCartMain(int customerId, int productId, int quantity)
        {
            var cart = GetOrCreateCart(customerId);
            var item = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item == null) return false;

            if (item.Quantity > quantity)
                item.Quantity -= quantity;
            else
                cart.CartItems.Remove(item);

            SaveOrUpdateCart(cart);
            return true;
        }

        public bool AddToCart(int customerId, int productId, int quantity)
        {
            return AddToCartMain(customerId, productId, quantity);
        }

        public bool RemoveFromCart(int customerId, int productId, int quantity)
        {
            return RemoveFromCartMain(customerId, productId, quantity);
        }

        public CartViewModel GetCartDetails(int customerId)
        {
            var cart = _context.Cart.Include(c => c.CartItems)
                                      .ThenInclude(ci => ci.Product)
                                      .FirstOrDefault(c => c.CustomerId == customerId);
            if (cart == null) return new CartViewModel();

            var customer = _context.Customers.Find(customerId);
            var viewModel = CalculateCartTotals(cart, customer);

            foreach (var item in viewModel.CartItems)
            {
                viewModel.ProductNames[item.ProductId] = item.Product?.ProductName;
            }

            return viewModel;
        }

        private Cart GetOrCreateCart(int customerId)
        {
            return _context.Cart.Include(c => c.CartItems)
                                  .FirstOrDefault(c => c.CustomerId == customerId)
                   ?? new Cart { CustomerId = customerId };
        }

        private void SaveOrUpdateCart(Cart cart)
        {
            if (_context.Cart.Any(c => c.CartId == cart.CartId))
                _context.Update(cart);
            else
                _context.Cart.Add(cart);

            _context.SaveChanges();
        }

        private CartViewModel CalculateCartTotals(Cart cart, Customer customer)
        {
            var taxRate = 0.05m;
            var shippingCost = 10.00m;
            var discount = customer?.Subscription switch
            {
                "Gold" => 0.20m,
                "Silver" => 0.10m,
                "Bronze" => 0.05m,
                _ => 0m
            };
            var subtotal = cart.CartItems.Sum(item => item.TotalPrice);
            var discountAmount = subtotal * discount;
            var discountedSubtotal = subtotal - discountAmount;
            var taxAmount = discountedSubtotal * taxRate;
            var finalTotal = discountedSubtotal + taxAmount + shippingCost;

            return new CartViewModel
            {
                CartItems = cart.CartItems,
                TaxRate = taxRate,
                ShippingCost = shippingCost,
                Discount = discount,
                DiscountedSubtotal = discountedSubtotal,
                TaxAmount = taxAmount,
                FinalTotal = finalTotal
            };
        }
    }
}
