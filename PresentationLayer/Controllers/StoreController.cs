using ConvicartWebApp.BussinessLogicLayer.Interface;
using ConvicartWebApp.DataAccessLayer.Data;
using ConvicartWebApp.Filter;
using Microsoft.AspNetCore.Mvc;

namespace ConvicartWebApp.PresentationLayer.Controllers
{
    /// <summary>
    /// This controller is resposible for displaing all products stored in store table and shows recipie of the product.
    /// </summary>
    [TypeFilter(typeof(CustomerInfoFilter))]
    public class StoreController : Controller
    {
        private readonly ConvicartWarehouseContext _context;
        private readonly IStoreService StoreService;
        private readonly IRecipeService RecipeService;


        public StoreController(ConvicartWarehouseContext context, IStoreService storeService, IRecipeService recipeService)
        {
            _context = context;
            StoreService = storeService;
            RecipeService = recipeService;
        }
        //displays products acoring to pagination, difficulty, points range, duration range, it also diplays products inthe order of acending and decending points.
        public IActionResult Store(
                string searchTerm = "",
                string sortOrder = "New",
                int page = 1,
                List<string> preferences = null,
                List<string> difficulty = null,
                int? cookTimeMin = null,
                int? cookTimeMax = null,
                int? minPoints = null,
                int? maxPoints = null)
        {
            // Get filtered and sorted stores from the service
            var stores = StoreService.GetStores(searchTerm, preferences, difficulty, cookTimeMin, cookTimeMax, minPoints, maxPoints);
            stores = StoreService.SortStores(stores, sortOrder);

            // Paginate results
            const int pageSize = 12;
            var pagedStores = StoreService.PaginateStores(stores, page, pageSize, out int totalPages);

            // Pass pagination and filter data to the view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SortOrder = sortOrder;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Preferences = preferences ?? new List<string>();
            ViewBag.Difficulty = difficulty ?? new List<string>();
            ViewBag.CookTimeMin = cookTimeMin;
            ViewBag.CookTimeMax = cookTimeMax;
            ViewBag.MinPoints = minPoints;
            ViewBag.MaxPoints = maxPoints;

            return View(pagedStores);
        }
        public async Task<IActionResult> Recipe(int id)
        {
            // Retrieve the product from the database asynchronously
            var product = await StoreService.GetProductByIdAsync(id);

            if (product == null)
            {
                return NotFound(); // Return 404 if the product is not found
            }

            // Retrieve recipe steps for the product asynchronously
            var steps = await RecipeService.GetRecipeStepsByProductIdAsync(id);

            // Pass the product and steps to the view
            ViewBag.RecipeSteps = steps;
            ViewBag.ProductId = id;

            return View(product); // Pass the product model to the view
        }

        //displays recipie steps of product
        public async Task<IActionResult> GetRecipeSteps(int productId)
        {
            // Retrieve the recipe steps from the database asynchronously
            var steps = await RecipeService.GetRecipeStepsByProductIdAsync(productId);

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


