#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using SalesReturnsSystem.DAL;
using SalesReturnsSystem.Entities;
using SalesReturnsSystem.DataModels.Sales;
#endregion

namespace SalesReturnsSystem.BLL
{
    public class CategoryService
    {
        #region Constructor and Context Dependency
        private readonly SalesReturnsContext _context;

        internal CategoryService(SalesReturnsContext context)
        {
            _context = context;
        }
        #endregion

        #region Queries
        public List<CategoryFetch> CategoryService_ListCategories()
        {
            IEnumerable<CategoryFetch> results = _context.Categories
                                                    .OrderBy(x => x.Description)
                                                    .Select(x => new CategoryFetch
                                                    {
                                                        CategoryID = x.CategoryID,
                                                        Description = x.Description,
                                                        ItemCount = x.StockItems.Count()
                                                    })
                                                    ;
            return results.ToList();
        }
        #endregion
    }
}
