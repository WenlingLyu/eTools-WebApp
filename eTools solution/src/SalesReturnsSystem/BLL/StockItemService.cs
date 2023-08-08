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
    public class StockItemService
    {
		#region Constructor and Context Dependency
		private readonly SalesReturnsContext _context;

		internal StockItemService(SalesReturnsContext context)
		{
			_context = context;
		}
		#endregion

		#region Queries
		public List<SearchItemInfo> StockItemService_ListItemsBySearch(string searchInput, string searchType)
		{
			IEnumerable<SearchItemInfo> results = null;

			if (searchType == "Category")
			{
				results = _context.StockItems
					.Where(x => x.Category.Description.Contains(searchInput) && x.Discontinued == false)
					.Select(x => new SearchItemInfo
					{
						StockItemID = x.StockItemID,
						Description = x.Description,
						Discontinued = x.Discontinued,
						QuantityOnHand = x.QuantityOnHand,
						Price = x.SellingPrice
					});
			}
			else if (searchType == "ItemName")
			{
				results = _context.StockItems
					.Where(x => x.Description.Contains(searchInput) && x.Discontinued == false)
					.Select(x => new SearchItemInfo
					{
						StockItemID = x.StockItemID,
						Description = x.Description,
						Discontinued = x.Discontinued,
						QuantityOnHand = x.QuantityOnHand,
						Price = x.SellingPrice
					});
			}
			else
			{
				throw new ArgumentException("Invalid search type.");
			}

			return results.ToList();
		}

		public List<SearchItemInfo> StockItemService_ListItemsByCategory(string category)
		{
			IEnumerable<SearchItemInfo> results = null;

			if (category == "All")
			{
				results = _context.StockItems
					.Where(x => x.Discontinued == false)
					.Select(x => new SearchItemInfo
					{
						StockItemID = x.StockItemID,
						Description = x.Description,
						Discontinued = x.Discontinued,
						QuantityOnHand = x.QuantityOnHand,
						Price = x.SellingPrice
					});
			}
			else
			{
				results = _context.StockItems
					.Where(x => x.Category.CategoryID == Int32.Parse(category) && x.Discontinued == false)
					.Select(x => new SearchItemInfo
					{
						StockItemID = x.StockItemID,
						Description = x.Description,
						Discontinued = x.Discontinued,
						QuantityOnHand = x.QuantityOnHand,
						Price = x.SellingPrice
					});
			}

			return results.ToList();
		}
		#endregion
	}
}
