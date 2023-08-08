#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesReturnsSystem.DataModels.SalesReturns
{
    public class SaleTRX
    {
		public int SaleID { get; set; }
		public int SaleRefundID { get; set; }
		public int EmployeeID { get; set; }
		public DateTime Date { get; set; }
		public string PaymentType { get; set; }
		public decimal Tax { get; set; }
		public decimal SubTotal { get; set; }
		public int? CouponID { get; set; }
		public int DiscountPercent { get; set; }
		public decimal Discount { get; set; }
	}
}
