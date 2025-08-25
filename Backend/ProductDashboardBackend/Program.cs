using Microsoft.EntityFrameworkCore;
using ProductDashboard.Services;
using ProductDashboardBackend.Data;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.AspNetCore.SpaServices.Extensions;

// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework with MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 33)),
        mysqlOptions =>
        {
            mysqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }
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

// Add SPA services for React (optional)
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp/build";
});


// Add JSON options for API endpoints
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Apply any pending migrations and create database if it doesn't exist
        await context.Database.MigrateAsync();
        // Seed the database using raw SQL
        await SqlSeedData.InitializeWithSqlAsync(context);
        Console.WriteLine("Database seeded successfully with SQL!");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
        if (app.Environment.IsDevelopment())
        {
            throw;
        }
    }
}

app.UseRouting();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

//app.UseHttpsRedirection();
app.UseStaticFiles();

// Enable SPA static files (for production React build)
if (!app.Environment.IsDevelopment())
{
    app.UseSpaStaticFiles();
    
}

// Use CORS in development for React
if (app.Environment.IsDevelopment())
{   
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSession(); // Enable session middleware
app.UseAuthorization();

// Add a route constraint to distinguish between API and regular requests
app.MapWhen(
    context => context.Request.Path.StartsWithSegments("/api") ||
               context.Request.Query.ContainsKey("react"),
    apiApp =>
    {
        apiApp.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "api",
                pattern: "api/{controller}/{action=Index}/{id?}");
        });
    });

app.Use(async (context, next) =>
{
    // Always add CORS headers
    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
    context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
    context.Response.Headers.Add("Access-Control-Allow-Headers", "*");

    // Handle preflight requests
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 200;
        return;
    }

    await next();
});

// Map default MVC routes for Razor views
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();