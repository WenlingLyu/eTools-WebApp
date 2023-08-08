using PurchasingSystem.DAL;
using PurchasingSystem.Entities;
using PurchasingSystem.ViewModels;
#nullable disable

namespace PurchasingSystem.BLL
{
    public class VendorService
    {
        #region Constructor and COntext Dependency

        private readonly PurchasingContext _context;
        internal VendorService(PurchasingContext context)
        {
            _context = context;
        }
		#endregion

		#region Service
		public VendorInfo Vendor_DisplayInfo(int vendorID)
		{
            if (vendorID <= 0)
            {
                throw new ArgumentNullException("No vendor submitted");
            }

            // Check if the vendor exists
            VendorInfo? vendorExists = _context.Vendors
											  .Where(v => v.VendorID == vendorID)
											  .Select(v => new VendorInfo
											  {
												  VendorID = v.VendorID,
												  SelectedVendor = v.VendorName,
												  Phone = v.Phone,
												  City = v.City,
											  })
											  .FirstOrDefault();
			if (vendorExists == null)
			{
				throw new ArgumentException($"Could not find Vendor");
			}

			return vendorExists;
		}

        public VendorInfo Vendor_Get(int vendorID)
        {
            VendorInfo vendorInfo = new VendorInfo();
            var CurrentPONumber = _context.PurchaseOrders.Select(p => p.PurchaseOrderID).OrderByDescending(p => p).FirstOrDefault();
            if (vendorID <= 0)
            {
                throw new ArgumentNullException("No vendor submitted");
            }
            bool vendorexist = false;
            vendorexist = _context.Vendors.Where(x => x.VendorID == vendorID).Any();

            bool orderexist = false;
            if (vendorexist == true)
            {

                vendorInfo.SelectedVendor = _context.Vendors.Where(x => x.VendorID == vendorID).Select(x => x.VendorName).FirstOrDefault();
                vendorInfo.Phone = _context.Vendors.Where(x => x.VendorID == vendorID).Select(x => x.Phone).FirstOrDefault();
                vendorInfo.City = _context.Vendors.Where(x => x.VendorID == vendorID).Select(x => x.City).FirstOrDefault();
                orderexist = _context.PurchaseOrders.Where(x => x.VendorID == vendorID && x.OrderDate == null).Any();
                if (orderexist == true)
                {
                    vendorInfo.PO = _context.PurchaseOrders.Where(x => x.VendorID == vendorID && x.OrderDate == null).Select(x => x.PurchaseOrderID).FirstOrDefault();
                }
                else
                {
                    vendorInfo.PO = CurrentPONumber + 1;
                }
            }
            else
            {
                throw new ArgumentNullException("Vendor does not exist");
            }


            return vendorInfo;

        }
        #endregion
    }
}
