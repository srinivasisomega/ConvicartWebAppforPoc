using ConvicartWebApp.Filter; // Make sure this is the correct namespace for your filters
using ConvicartWebApp.Interface;
using ConvicartWebApp.Models; // Ensure you have the correct namespace for your context
using ConvicartWebApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new CacheImageFilter(3600)); // Your custom cache filter
});

// Register ConvicartWarehouseContext with a connection string
builder.Services.AddDbContext<ConvicartWarehouseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConvicartWarehouseContextConnection")));

// Register the CustomerInfoFilter with dependency injection
builder.Services.AddScoped<CustomerInfoFilter>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IPointsService, PointsService>();
builder.Services.AddScoped<ICustomerService,CustomerService>();
// Register IHttpContextAccessor for accessing HttpContext
builder.Services.AddHttpContextAccessor(); // This is essential for filters to access HttpContext

builder.Services.AddMemoryCache(); // Register in-memory caching
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
app.UseAuthorization();

// Define the default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

