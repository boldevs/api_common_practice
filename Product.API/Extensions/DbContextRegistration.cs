using Microsoft.EntityFrameworkCore;
using Product.API.Data;

namespace Product.API.Extensions
{
    public static class DbContextRegistration
    {
        public static void AddConfiguredDbContext(this IServiceCollection services, IConfiguration config, IWebHostEnvironment env)
        {
            if (!env.IsEnvironment("Testing"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlite(config.GetConnectionString("DefaultConnection")));
            }
        }
    }
}
