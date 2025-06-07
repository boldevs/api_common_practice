using Product.API.Caching;
using Product.API.Endpoints.V1;
using Product.API.Extensions;
using Product.API.Middlewares;
using Product.API.Services;
using Product.API.Data;
using Serilog;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- SERILOG SETUP ---
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig
    .ReadFrom.Configuration(context.Configuration));

// --- DATABASE & CACHE SETUP ---
builder.Services.AddConfiguredDbContext(builder.Configuration, builder.Environment);

// --- ROBUST REDIS CONFIGURATION ---
// This will only add the Redis services if the connection string is present
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnectionString))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnectionString;
        options.InstanceName = "ProductAPI_";
    });
}
// ------------------------------------


// --- HEALTH CHECKS ---
var healthChecksBuilder = builder.Services.AddHealthChecks();
if (!string.IsNullOrEmpty(redisConnectionString))
{
    healthChecksBuilder.AddRedis(redisConnectionString, name: "Redis Cache");
}

// --- SERVICES ---
builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ***************************************************************
// *** IMPORTANT: CONFIGURE APP TO LISTEN ON RENDER'S PORT ***
// This line tells your app to listen on the port Render provides.
// It falls back to port 8080 if the PORT variable isn't set (for local dev).
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");
// ***************************************************************


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!env.IsEnvironment("Testing") && db.Database.IsRelational())
    {
        db.Database.Migrate();
    }
}


// ***************************************************************
// *** IMPORTANT: REMOVED HTTPS REDIRECTION FOR RENDER ***
// app.UseHttpsRedirection(); // This line is removed as Render handles HTTPS.
// ***************************************************************

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapProductV1();

// Map health checks endpoint
app.MapHealthChecks("/health");

app.Run();

// Make the Program class public for testing purposes
public partial class Program { }
