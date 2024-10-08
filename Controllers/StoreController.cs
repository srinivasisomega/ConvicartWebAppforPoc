using ConvicartWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConvicartWebApp.Controllers
{

    public class StoreController : Controller
    {
        private readonly ConvicartWarehouseContext _context;

        public StoreController(ConvicartWarehouseContext context)
        {
            _context = context;
        }

        public IActionResult Store(string searchTerm = "", string sortOrder = "New", int page = 1)
        {
            // Fetch items from the database
            var items = _context.Stores.AsQueryable();

            // Search functionality
            if (!string.IsNullOrEmpty(searchTerm))
            {
                items = items.Where(i => i.ProductName.Contains(searchTerm)); // Filter based on product name
            }

            // Sorting functionality
            items = sortOrder switch
            {
                "Price ascending" => items.OrderBy(i => i.Price),
                "Price descending" => items.OrderByDescending(i => i.Price),
                "Rating" => items.OrderByDescending(i => i.Rating),
                _ => items.OrderBy(i => i.ProductId), // Default sorting
            };

            // Pagination logic (assuming 9 items per page)
            int pageSize = 9;
            var totalItems = items.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var pagedItems = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SortOrder = sortOrder;

            return View(pagedItems);
        }


    }

}

