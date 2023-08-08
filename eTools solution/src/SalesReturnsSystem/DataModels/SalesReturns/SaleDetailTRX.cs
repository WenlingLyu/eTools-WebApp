#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesReturnsSystem.DataModels.SalesReturns
{
    public class SaleDetailTRX
    {
        public int SaleRefundID { get; set; }
        public int SaleDetailID { get; set; }
		public int SaleID { get; set; }
		public int StockItemID { get; set; }
		public string Description { get; set; }
		public decimal SellingPrice { get; set; }
		public decimal ItemTotal { get; set; }
		public int Quantity { get; set; }
        public int QuantityRefunded { get; set; }
		public int ReturnQuantity { get; set; }
		public bool SelectedItem { get; set; }
	}
}
