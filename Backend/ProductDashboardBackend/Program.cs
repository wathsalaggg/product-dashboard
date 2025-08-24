//using ProductDashboardBackend.Data;
//using Microsoft.EntityFrameworkCore;
//using System.Text.Json.Serialization;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//    {
//        // Fix circular reference issue
//        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//        options.JsonSerializerOptions.WriteIndented = true; 
//    });

//// Add MySQL DbContext
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseMySql(
//        builder.Configuration.GetConnectionString("DefaultConnection"),
//        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
//    )
//);

//// Add CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowAll",
//        policy =>
//        {
//            policy.AllowAnyOrigin()
//                  .AllowAnyMethod()
//                  .AllowAnyHeader();
//        });
//});

//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
//app.UseCors("AllowAll");
//app.UseAuthorization();
//app.MapControllers();

//// Ensure database is created and seed data
//try
//{
//    using (var scope = app.Services.CreateScope())
//    {
//        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

//        // Test DB connection
//        bool canConnect = await db.Database.CanConnectAsync();
//        Console.WriteLine($"Database connection successful? {canConnect}");

//        // Ensure database is created
//        await db.Database.EnsureCreatedAsync();
//        Console.WriteLine("Database ensured created.");

//        // Seed database asynchronously
//        await SeedData.SeedAsync(db);
//        Console.WriteLine("Database seeding completed.");
//    }
//}
//catch (Exception ex)
//{
//    Console.WriteLine($"Database connection or seeding error: {ex.Message}");
//}

//app.Run();













//using Microsoft.EntityFrameworkCore;
//using ProductDashboardBackend.Data;
//using System;

//public class Program
//{
//    public static void Main(string[] args)
//    {
//        var builder = WebApplication.CreateBuilder(args);

//        // Add services
//        builder.Services.AddControllersWithViews(options =>
//        {
//            // Register the custom model binder for comma-separated values
//            options.ModelBinderProviders.Insert(0, new CommaSeparatedModelBinderProvider());
//        });

//        builder.Services.AddDbContext<ApplicationDbContext>(options =>
//            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//        builder.Services.AddScoped<IProductService, ProductService>();

//        var app = builder.Build();

//        // Configure pipeline
//        if (app.Environment.IsDevelopment())
//        {
//            app.UseDeveloperExceptionPage();
//        }

//        app.UseRouting();

//        // Enable static files (for CSS/JS)
//        app.UseStaticFiles();

//        // Map default controller route
//        app.MapControllerRoute(
//            name: "default",
//            pattern: "{controller=Home}/{action=Index}/{id?}"
//        );

//        // Keep API controllers mapped as well
//        app.MapControllers();

//        app.Run();
//    }
//}

using Microsoft.EntityFrameworkCore;
using ProductDashboard.Services;
using ProductDashboardBackend.Data;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Entity Framework with MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 33)) // Replace with your MySQL version
    )
);

// Register ProductService
builder.Services.AddScoped<ProductService>();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); // Enable session middleware
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
