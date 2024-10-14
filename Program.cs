using ConvicartWebApp.Filter;
using ConvicartWebApp.Models;
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
