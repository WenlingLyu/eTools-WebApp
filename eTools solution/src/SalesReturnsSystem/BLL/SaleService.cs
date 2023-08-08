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
using SalesReturnsSystem.DataModels.SalesReturns;
#endregion

namespace SalesReturnsSystem.BLL
{
    public class SaleService
    {
        #region Constructor and Context Dependency
        private readonly SalesReturnsContext _context;

        internal SaleService(SalesReturnsContext context)
        {
            _context = context;
        }
        #endregion

        #region Services
        public CouponInfo SaleService_RetrieveCouponBySearch(string couponValue)
		{
			if (string.IsNullOrWhiteSpace(couponValue))
			{
				throw new ArgumentNullException("No coupon code submitted");
			}

			CouponInfo result = _context.Coupons
									.Where(x => x.CouponIDValue == couponValue)
									.Select(x => new CouponInfo
									{
										CouponID = x.CouponID,
										CouponValue = x.CouponIDValue,
										Discount = x.CouponDiscount
									})
									.FirstOrDefault()
									;
			return result;
		}

		public int SaleService_PlaceOrder(SaleTRX saleInfo, CouponInfo coupon, List<SaleDetailTRX> orderItems)
		{
			StockItem stockItem = null;
			Sale sale = null;
			SaleDetail saleDetails = null;
			List<Exception> errorList = new List<Exception>();
			int stockQuantity = 0;

			if (orderItems.Count() == 0)
			{
				throw new ArgumentException("No items in cart found.");
			}

			if (coupon.CouponID != 0)
			{
				saleInfo.CouponID = coupon.CouponID;
			}

			if (saleInfo.PaymentType == "M" || saleInfo.PaymentType == "C" || saleInfo.PaymentType == "D")
			{
				// create sale item
				sale = new Sale()
				{
					SaleDate = saleInfo.Date,
					PaymentType = saleInfo.PaymentType,
					EmployeeID = saleInfo.EmployeeID,
					TaxAmount = saleInfo.Tax,
					SubTotal = saleInfo.SubTotal,
					CouponID = saleInfo.CouponID
				};

				_context.Sales.Add(sale);

				foreach (var item in orderItems)
				{
					stockQuantity = _context.StockItems
									.Where(x => x.StockItemID == item.StockItemID)
									.Select(x => x.QuantityOnHand)
									.FirstOrDefault();

					if (stockQuantity < item.Quantity)
					{
						errorList.Add(new Exception("The quantity ordered must not exceed current in-stock quantities."));
					}
					else
					{
						stockItem = _context.StockItems
										.Where(x => x.StockItemID == item.StockItemID)
										.Select(x => x)
										.First();

						stockItem.QuantityOnHand -= item.Quantity;
						_context.StockItems.Update(stockItem);

						int saleID = _context.Sales.Count() == 0 ? 1
							   : _context.Sales.OrderBy(x => x.SaleID).Max(x => x.SaleID) + 1;

						// create a new sale details items
						saleDetails = new SaleDetail()
						{
							SaleID = saleID,
							StockItemID = item.StockItemID,
							Quantity = item.Quantity,
							SellingPrice = item.SellingPrice
						};

						sale.SaleDetails.Add(saleDetails);
					}
				}
			}
			else
			{
				errorList.Add(new Exception("Invalid payment type"));
			}

			if (errorList.Count > 0)
			{
				//  throw the list of business processing error(s)
				throw new AggregateException("Unable to place new order Check concerns", errorList);
			}
			else
			{
				_context.SaveChanges();
				return sale.SaleID;
			}
		}
		#endregion
	}
}
