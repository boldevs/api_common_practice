using Microsoft.EntityFrameworkCore;
using Product.API.Data;
using Npgsql;

namespace Product.API.Extensions
{
    public static class DbContextRegistration
    {
        public static void AddConfiguredDbContext(this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {
            if (!env.IsEnvironment("Testing"))
            {
                var rawConnectionString = config.GetConnectionString("DefaultConnection");

                // Normalize connection string if it is a URL style
                if (!string.IsNullOrEmpty(rawConnectionString) && rawConnectionString.StartsWith("postgres://"))
                {
                    var builder = new NpgsqlConnectionStringBuilder(rawConnectionString);
                    rawConnectionString = builder.ConnectionString;
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(rawConnectionString);
                });
            }
        }

    }
}
