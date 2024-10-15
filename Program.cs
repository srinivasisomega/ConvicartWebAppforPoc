using ConvicartWebApp.Filter;
using ConvicartWebApp.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new CacheImageFilter(3600)); // Example cache filter
});

// Register ConvicartWarehouseContext with a connection string
builder.Services.AddDbContext<ConvicartWarehouseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConvicartWarehouseContextConnection")));

// Add session support with custom options (optional configuration for session)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout (optional)
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

// Enable session middleware
app.UseSession(); // Add this line to enable session management

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
