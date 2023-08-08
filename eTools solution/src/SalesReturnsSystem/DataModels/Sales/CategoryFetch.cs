#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesReturnsSystem.DataModels.Sales
{
    public class CategoryFetch
    {
        public int CategoryID { get; set; }
        public string Description { get; set; }
        public int ItemCount { get; set; }
    }
}
