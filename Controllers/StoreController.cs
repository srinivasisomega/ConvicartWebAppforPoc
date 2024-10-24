using ConvicartWebApp.Filter;
using ConvicartWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ConvicartWebApp.Controllers
{
    /// <summary>
    /// This controller is resposible for displaing all products stored in store table and shows recipie of the product.
    /// </summary>
    [TypeFilter(typeof(CustomerInfoFilter))]
    public class StoreController : Controller
    {
        private readonly ConvicartWarehouseContext _context;

        public StoreController(ConvicartWarehouseContext context)
        {
            _context = context;
        }
        //displays products acoring to pagination, difficulty, points range, duration range, it also diplays products inthe order of acending and decending points.
        public IActionResult Store(string searchTerm = "",string sortOrder = "New",int page = 1,List<string> preferences = null, List<string> difficulty = null, int? cookTimeMin = null,int? cookTimeMax = null,int? minPoints = null, int? maxPoints = null)

        {
            // Fetch items from the database
            var items = _context.Stores.Include(i => i.Preference).AsQueryable(); // Include related data

            // Search functionality
            if (!string.IsNullOrEmpty(searchTerm))
            {
                items = items.Where(i => i.ProductName.Contains(searchTerm)); // Filter based on product name
            }
            // Filter by preferences
            if (preferences != null && preferences.Any()) // Check for any items in the list
            {
                items = items.Where(i => preferences.Contains(i.Preference.PreferenceName));
            }

            // Filter by difficulty
            if (difficulty != null && difficulty.Any()) // Check for any items in the list
            {
                items = items.Where(i => difficulty.Contains(i.Difficulty));
            }

            // Filter by cook time range
            if (cookTimeMin.HasValue)
            {
                // Calculate the minimum cook time as a TimeSpan from the minutes
                var minimumCookTime = TimeSpan.FromMinutes(cookTimeMin.Value);

                // Filter the items based on the calculated TimeSpan
                items = items.Where(i => i.CookTime >= minimumCookTime); // Filter based on minimum cook time
            }

            if (cookTimeMax.HasValue)
            {
                // Create TimeSpan from cookTimeMax
                var maximumCookTime = TimeSpan.FromMinutes(cookTimeMax.Value);

                // Filter the items based on the calculated TimeSpan
                items = items.Where(i => i.CookTime <= maximumCookTime); // Filter based on maximum cook time
            }


            // Filter by points range (Price)
            if (minPoints.HasValue)
            {
                items = items.Where(i => i.Price >= minPoints.Value); // Filter based on minimum points (price)
            }
            if (maxPoints.HasValue)
            {
                items = items.Where(i => i.Price <= maxPoints.Value); // Filter based on maximum points (price)
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
            int pageSize = 12;
            var totalItems = items.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            var pagedItems = items.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Passing filter values back to the view so that the UI can retain the applied filters
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Preferences = preferences ?? new List<string>(); // Ensure it's not null
            ViewBag.Difficulty = difficulty ?? new List<string>(); // Ensure it's not null
            ViewBag.CookTimeMin = cookTimeMin;
            ViewBag.CookTimeMax = cookTimeMax;
            ViewBag.MinPoints = minPoints;
            ViewBag.MaxPoints = maxPoints;

            return View(pagedItems);
        }
        public IActionResult Recipe(int id)
        {
            // Retrieve the product from the database
            var product = _context.Stores.FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound(); // Return 404 if the product is not found
            }

            // Retrieve recipe steps for the product
            var steps = _context.RecipeSteps.Where(s => s.ProductId == id).OrderBy(s => s.StepNumber).ToList();

            // Pass the product and steps to the view
            ViewBag.RecipeSteps = steps; // Use ViewBag to pass the recipe steps or adjust to pass it directly
            ViewBag.ProductId = id;
            return View(product); // Pass the product model to the view
        }
        //displays recipie steps of product
        public IActionResult GetRecipeSteps(int productId)
        {
            // Retrieve the recipe steps from the database
            var steps = _context.RecipeSteps.Where(s => s.ProductId == productId).OrderBy(s => s.StepNumber).ToList();
            return PartialView("_RecipeSteps", steps); // Return the partial view with the steps
        }
        public IActionResult GetProfileImage(int id)
        {
            var customer = _context.Stores.FirstOrDefault(c => c.ProductId == id);
            if (customer?.ProductImage != null)
            {
                return File(customer.ProductImage, "image/jpeg");  // Serve image
            }
            else
            {
                return NotFound();  // No image found
            }
        }
    }
}


