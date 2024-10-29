namespace ConvicartWebApp.Filter
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class SessionAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var customerId = context.HttpContext.Session.GetInt32("CustomerId");

            if (!customerId.HasValue) // User is not logged in
            {
                context.Result = new RedirectToActionResult("SignUp", "Customer", null);
            }
        }
    }
}
