using Microsoft.AspNetCore.Mvc.Filters;
using ConvicartWebApp.DataAccessLayer.Data;
namespace ConvicartWebApp.Filter
{
    public class CustomerInfoFilter : IActionFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ConvicartWarehouseContext _context;

        public CustomerInfoFilter(IHttpContextAccessor httpContextAccessor, ConvicartWarehouseContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var customerId = _httpContextAccessor.HttpContext.Session.GetInt32("CustomerId");
            if (customerId != null)
            {
                var customer = _context.Customers.Find(customerId.Value);
                if (customer != null)
                {
                    if (customer.Name == null)
                    { 
                        context.HttpContext.Items["CustomerName"] = "Add Customer Name at Profile";
                        context.HttpContext.Items["PointBalance"] = customer.PointBalance;
                        context.HttpContext.Items["CustomerId"] = customer.CustomerId;
                    }
                    else
                    {
                        context.HttpContext.Items["CustomerName"] = customer.Name;
                        context.HttpContext.Items["PointBalance"] = customer.PointBalance;
                        context.HttpContext.Items["CustomerId"] = customer.CustomerId;
                    }
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }

}
