using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesReturnsSystem.DAL;
using SalesReturnsSystem.BLL;
#endregion

namespace SalesReturnsSystem
{
    public static class SalesReturnsSystemExtensions
    {
        public static void SalesReturnsSystemBackendDependencies(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<SalesReturnsContext>(options);

            services.AddTransient<CategoryService>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<SalesReturnsContext>();
                return new CategoryService(context);
            });

            services.AddTransient<StockItemService>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<SalesReturnsContext>();
                return new StockItemService(context);
            });

            services.AddTransient<SaleService>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<SalesReturnsContext>();
                return new SaleService(context);
            });

            services.AddTransient<ReturnsService>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<SalesReturnsContext>();
                return new ReturnsService(context);
            });
        }
    }
}
