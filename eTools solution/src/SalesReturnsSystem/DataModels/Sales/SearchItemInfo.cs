#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesReturnsSystem.DataModels.Sales
{
    public class SearchItemInfo
    {
        public int StockItemID { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int QuantityOnHand { get; set; }
        public bool Discontinued { get; set; }
    }
}
