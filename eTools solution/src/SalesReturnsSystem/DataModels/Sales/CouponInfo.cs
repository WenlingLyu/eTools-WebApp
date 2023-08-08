#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesReturnsSystem.DataModels.Sales
{
    public class CouponInfo
    {
        public int CouponID { get; set; }
        public string CouponValue { get; set; }
        public int Discount { get; set; }
    }
}
