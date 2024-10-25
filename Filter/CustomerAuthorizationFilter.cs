using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace ConvicartWebApp.Filter
{
public class CustomerAuthorizationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var session = context.HttpContext.Session;
        int? customerId = session.GetInt32("CustomerId");

        if (customerId == null)
        {
            context.Result = new RedirectToActionResult("SignUp", "Customer", null);
            return;
        }

        // Pass customerId to the action method
        context.ActionArguments["customerId"] = customerId.Value;

        // Proceed to the next filter or action method
        await next();
    }
}

}
