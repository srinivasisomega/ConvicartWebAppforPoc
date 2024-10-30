using ConvicartWebApp.BussinessLogicLayer.Interface;
using ConvicartWebApp.BussinessLogicLayer.Interface.RepositoryInterface;
using ConvicartWebApp.BussinessLogicLayer.Services;
using ConvicartWebApp.DataAccessLayer.Data;
using ConvicartWebApp.DataAccessLayer.Repositories;
using ConvicartWebApp.Filter;
using ConvicartWebApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
ConfigureServices(builder);

var app = builder.Build();

// Configure the HTTP request pipeline
ConfigurePipeline(app);

app.Run();

void ConfigureServices(WebApplicationBuilder builder)
{
    var services = builder.Services;
    services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        options.CallbackPath = "/signin-google";
    });
    // Register ConvicartWarehouseContext with connection string
    services.AddDbContext<ConvicartWarehouseContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("ConvicartWarehouseContextConnection")));

    services.AddControllersWithViews(options => options.Filters.Add(new CacheImageFilter(3600)));

    // Register DI services and filters
    services.AddScoped<CustomerInfoFilter>()
            .AddScoped<CustomerAuthorizationFilter>()
            .AddScoped<IEmailService, EmailService>()
            .AddScoped<IPasswordResetService, PasswordResetService>()
            .AddScoped<ISubscriptionService, SubscriptionService>()
            .AddScoped<IStoreService, StoreService>()
            .AddScoped<IRecipeService, RecipeService>()
            .AddScoped<IAddressService, AddressService>()
            .AddScoped<IPointsService, PointsService>()
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<IPreferenceService, PreferenceService>()
            .AddScoped<IOrderService, OrderService>()
            .AddScoped<ICartService, CartService>()
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<IAddressRepository, AddressRepository>()
            .AddScoped<IStoreRepository, StoreRepository>();


    services.AddHttpContextAccessor(); // For accessing HttpContext
    services.AddMemoryCache(); // In-memory caching
    services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });
}

void ConfigurePipeline(WebApplication app)
{
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    //app.UseRouting();
    app.UseSession();
    app.UseAuthorization();

    // Define default route
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
}
