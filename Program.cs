using ConvicartWebApp;
using ConvicartWebApp.BussinessLogicLayer.Interface;
using ConvicartWebApp.BussinessLogicLayer.Services;
using ConvicartWebApp.DataAccessLayer.Data;
using ConvicartWebApp.Filter; // Make sure this is the correct namespace for your filters
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new CacheImageFilter(3600));
});

// Register ConvicartWarehouseContext with a connection string
builder.Services.AddDbContext<ConvicartWarehouseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConvicartWarehouseContextConnection")));

// Register the CustomerInfoFilter with dependency injection
builder.Services.AddScoped<CustomerInfoFilter>();
// In Startup.cs or Program.cs (ConfigureServices method)
builder.Services.AddScoped<CustomerAuthorizationFilter>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IStoreService,StoreService>();
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IPointsService, PointsService>();
builder.Services.AddScoped<ICustomerService,CustomerService>();
builder.Services.AddScoped<IPreferenceService, PreferenceService>();

// Register IHttpContextAccessor for accessing HttpContext
builder.Services.AddHttpContextAccessor(); // This is essential for filters to access HttpContext

builder.Services.AddMemoryCache(); // Register in-memory caching
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Customer/SignIn";   // Path to redirect if not authenticated
        options.AccessDeniedPath = "/Customer/AccessDenied";  // Path if access is denied
        options.ExpireTimeSpan = TimeSpan.FromDays(7); // Cookie expiration
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("GoldOnly", policy =>
        policy.Requirements.Add(new SubscriptionRequirement("Gold")));

    options.AddPolicy("SilverOnly", policy =>
        policy.Requirements.Add(new SubscriptionRequirement("Silver")));

    options.AddPolicy("BronzeOnly", policy =>
        policy.Requirements.Add(new SubscriptionRequirement("Bronze")));
});

// Register the authorization handler
builder.Services.AddSingleton<IAuthorizationHandler, SubscriptionHandler>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Optional session timeout
    options.Cookie.HttpOnly = true; // Security for session cookie
    options.Cookie.IsEssential = true; // Required for GDPR compliance
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Enable session management
app.UseAuthentication();
app.UseAuthorization();


// Define the default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

