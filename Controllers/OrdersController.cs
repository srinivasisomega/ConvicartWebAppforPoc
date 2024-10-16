using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConvicartWebApp.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using ConvicartWebApp.Models;

    public class OrdersController : Controller
    {
        private readonly ConvicartWarehouseContext _context;

        // Inject the database context
        public OrdersController(ConvicartWarehouseContext context)
        {
            _context = context;
        }

        // Action method to fetch past orders
        public IActionResult OrderHistory(string timeFilter = "6months")
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId"); ; // Replace with actual logic to get customer ID
            DateTime filterDate = GetFilterDate(timeFilter);

            // Fetch orders for the customer within the filter date range
            var orders = _context.Orders
                .Where(o => o.CustomerId == customerId && o.OrderDate >= filterDate)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            var orderViewModels = orders.Select(order => new OrderViewModel
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                ImageUrl = order.OrderItems.FirstOrDefault()?.imgUrl // Get the first item's image URL
            }).ToList();
            return View("OrderHistory",orderViewModels);
        }

        // Action for 'Order Again' functionality
        //[HttpPost]
        //public IActionResult OrderAgain(int orderId)
        //{
        //    var order = _context.Orders
        //        .Include(o => o.OrderItems)
        //        .FirstOrDefault(o => o.OrderId == orderId);

        //    if (order != null)
        //    {
        //        // Logic to place a new order with the same items
        //        var newOrder = new Order
        //        {
        //            CustomerId = order.CustomerId,
        //            TotalAmount = order.TotalAmount, // You might want to recalculate this
        //            OrderDate = DateTime.Now,
        //            OrderItems = order.OrderItems.Select(oi => new OrderItem
        //            {
        //                ProductId = oi.ProductId,
        //                ProductName = oi.ProductName,
        //                Price = oi.Price,
        //                Quantity = oi.Quantity
        //            }).ToList()
        //        };

        //        _context.Orders.Add(newOrder);
        //        _context.SaveChanges();
        //    }

        //    return RedirectToAction("PastOrders");
        //}

        // Utility to get the customer ID (replace with actual user context logic)
        private int GetLoggedInCustomerId()
        {
            // Simulate getting logged-in customer ID
            return 1; // Replace with actual customer ID
        }

        // Utility to calculate the filter date based on timeFilter
        private DateTime GetFilterDate(string timeFilter)
        {
            DateTime now = DateTime.Now;
            return timeFilter switch
            {
                "15days" => now.AddDays(-15),
                "30days" => now.AddDays(-30),
                "3months" => now.AddMonths(-3),
                "6months" => now.AddMonths(-6),
                _ => now.AddMonths(-6) // Default to 6 months
            };
        }
    }

}
