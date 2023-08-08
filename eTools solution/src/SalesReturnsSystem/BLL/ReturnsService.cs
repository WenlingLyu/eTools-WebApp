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
    public class ReturnsService
    {
        #region Constructor and Context Dependency
        private readonly SalesReturnsContext _context;

        internal ReturnsService(SalesReturnsContext context)
        {
            _context = context;
        }
        #endregion

        #region Services
        public SaleTRX ReturnsService_RetrieveSaleBySearch(int saleID)
        {
            int saleRefundID = _context.SaleRefunds.Count() == 0 ? 1
                               : _context.SaleRefunds.OrderBy(x => x.SaleRefundID).Max(x => x.SaleRefundID) + 1;

            SaleTRX results = _context.Sales
                                .Where(x => x.SaleID == saleID)
                                .Select(x => new SaleTRX
                                {
                                    SaleID = x.SaleID,
                                    SaleRefundID = saleRefundID,
                                    EmployeeID = x.EmployeeID,
                                    Date = x.SaleDate,
                                    PaymentType = x.PaymentType,
                                    DiscountPercent = x.CouponID != null ? x.Coupon.CouponDiscount : 0
                                })
                                .FirstOrDefault();

            return results;
        }

        public List<SaleDetailTRX> ReturnsService_RetrieveOrderItems(SaleTRX refundInfo)
        {
            List<SaleDetailTRX> saleRefundExists = _context.SaleRefunds
                                                        .OrderBy(x => x.SaleRefundID)
                                                        .Where(x => x.SaleID == refundInfo.SaleID)
                                                        .Select(x => new SaleDetailTRX
                                                        {
                                                            SaleRefundID = x.SaleRefundID
                                                        })
                                                        .ToList();
            ;

            List<SaleDetailTRX> results = _context.SaleDetails
                                                    .Where(x => x.SaleID == refundInfo.SaleID)
                                                    .Select(x => new SaleDetailTRX
                                                    {
                                                        SaleDetailID = x.SaleDetailID,
                                                        SaleID = x.SaleID,
                                                        StockItemID = x.StockItemID,
                                                        Description = x.StockItem.Description,
                                                        Quantity = x.Quantity,
                                                        SellingPrice = x.SellingPrice
                                                    })
                                                    .ToList();

            foreach (var item in results)
            {
                for (int i = 0; i < saleRefundExists.Count(); i++)
                {
                    var refundedItemExists = _context.SaleRefundDetails
                                            .Where(x => x.SaleRefundID == saleRefundExists[i].SaleRefundID && x.StockItemID == item.StockItemID)
                                            .Select(x => x)
                                            .FirstOrDefault();

                    if (refundedItemExists != null)
                    {
                        var index = results.FindIndex(x => x.StockItemID == item.StockItemID);
                        results[index].QuantityRefunded += refundedItemExists.Quantity;
                    }
                }
            }

            return results;
        }

        public bool ReturnsService_ProcessRefund(SaleTRX refundInfo, List<SaleDetailTRX> refundItems)
        {
            SaleRefund newRefund = null;
            SaleRefundDetail newRefundDetails = null;
            List<Exception> errorList = new List<Exception>();

            if (refundInfo == null)
            {
                throw new ArgumentNullException("Refund order details is missing");
            }

            // add new sale refunds item
            newRefund = new SaleRefund()
            {
                SaleRefundDate = refundInfo.Date,
                SaleID = refundInfo.SaleID,
                EmployeeID = refundInfo.EmployeeID,
                TaxAmount = refundInfo.Tax,
                SubTotal = refundInfo.SubTotal
            };

            if (refundItems.Count() == 0)
            {
                errorList.Add(new Exception("There are no items to be refunded")); ;
            }
            else
            {
                _context.SaleRefunds.Add(newRefund);

                foreach (var item in refundItems)
                {
                    if (item.ReturnQuantity > 0)
                    {
                        newRefundDetails = new SaleRefundDetail()
                        {
                            SaleRefundID = refundInfo.SaleRefundID,
                            StockItemID = item.StockItemID,
                            SellingPrice = item.SellingPrice,
                            Quantity = item.ReturnQuantity
                        };

                        UpdateStockItemQuantities(item);
                        newRefund.SaleRefundDetails.Add(newRefundDetails);
                    }
                }
            }

            if (errorList.Count > 0)
            {
                //  throw the list of business processing error(s)
                throw new AggregateException("Unable to process refund. Check concerns", errorList);
            }
            else
            {
                _context.SaveChanges();
                return true;
            }
        }

        public void UpdateStockItemQuantities(SaleDetailTRX item)
        {
            List<Exception> errorList = new List<Exception>();

            var itemExists = _context.StockItems
                                .Where(x => x.StockItemID == item.StockItemID)
                                .Select(x => x)
                                .FirstOrDefault();

            if (itemExists == null)
            {
                errorList.Add(new Exception("Item to be refunded not found"));
            }
            else
            {
                itemExists.QuantityOnHand += item.ReturnQuantity;
                _context.StockItems.Update(itemExists);
            }

            if (errorList.Count > 0)
            {
                //  throw the list of business processing error(s)
                throw new AggregateException("Errors encountered. Check concerns", errorList);
            }
            else
            {
                _context.SaveChanges();
            }
        }
        #endregion
    }
}
