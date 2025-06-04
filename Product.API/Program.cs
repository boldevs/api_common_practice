using Product.API.Caching; // Make sure you have this using for the cache service
using Product.API.Endpoints.V1;
using Product.API.Extensions;
using Product.API.Middlewares;
using Product.API.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// --- CORRECTED SERILOG SETUP ---
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig
    .ReadFrom.Configuration(context.Configuration));

// --- THE REST OF YOUR SETUP IS EXCELLENT ---
builder.Services.AddConfiguredDbContext(builder.Configuration, builder.Environment);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "ProductAPI_";
});

// --- THE CORRECT WAY TO CHECK REDIS HEALTH ---

// First, get the Redis connection string you've already configured
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

// Then, add the health check using the proper Redis method
builder.Services.AddHealthChecks()
    .AddRedis(redisConnectionString, name: "Redis Cache"); // This speaks the correct language
    
// Register our custom, abstract cache service
builder.Services.AddSingleton<ICacheService, RedisCacheService>();

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapProductV1();

// Map health checks endpoint
app.MapHealthChecks("/health");

app.Run();

public partial class Program { }