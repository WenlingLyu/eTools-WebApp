using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PurchasingSystem.DAL;
using PurchasingSystem.BLL;

namespace PurchasingSystem
{
    public static class PurchasingSystemExtensions
    {
        public static void PurchasingSystemBackendDependencies(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<PurchasingContext>(options);


            // Add transient for vendor services
            services.AddTransient<VendorService>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<PurchasingContext>();
                //  Create an instance of the service and return the instance
                return new VendorService(context);
            });
            // Add transient for order services
            services.AddTransient<OrderServiceTest>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<PurchasingContext>();
                //  Create an instance of the service and return the instance
                return new OrderServiceTest(context);
            });
        }
    }
}


