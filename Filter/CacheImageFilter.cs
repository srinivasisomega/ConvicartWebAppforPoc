namespace ConvicartWebApp.Filter
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class CacheImageFilter : IAsyncActionFilter
    {
        private readonly int _duration;

        public CacheImageFilter(int duration)
        {
            _duration = duration; // Duration in seconds
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Check if the response is already cached
            var resultContext = await next();

            // Apply caching only if the result is not null and is successful
            if (resultContext.Result is ObjectResult)
            {
                var response = context.HttpContext.Response;

                // Add cache headers
                response.Headers["Cache-Control"] = $"public,max-age={_duration}";
                response.Headers["Expires"] = DateTime.UtcNow.AddSeconds(_duration).ToString("R"); // Optional: Expires header
            }
        }
    }

}
