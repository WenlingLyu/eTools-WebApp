using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable disable

namespace PurchasingSystem.ViewModels
{
    public class itemOnInventoryOrOrder
    {
        public int? PurchaseOrderDetailID { get; set; }
        public int stockItemID { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int QOH { get; set; }
        public int ROL { get; set; }
        public int QOO { get; set; }
        public int QTO { get; set; }
        public decimal Total { get; set; }
        public int Buffer { get; set; } 
        
    }
}
