using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using eToolsWebFramework.Data;
using Microsoft.AspNetCore.Identity;
using EToolsSecurity.BLL;
using PurchasingSystem.ViewModels;
using PurchasingSystem.BLL;
using EToolsSecurity.ViewModel;

namespace eToolsWebFramework.Pages.PurchasingPages
{
    public class PurchasingTestModel : PageModel
    {

        #region Private variables and DI constructor
        private readonly VendorService _vendorService;
        private readonly SecurityService _securityService;
        private readonly OrderServiceTest _orderServiceTest;

        public PurchasingTestModel(VendorService vendorService,SecurityService securityService,OrderServiceTest orderServiceTest)
        {
            _vendorService = vendorService;
            _securityService = securityService;
            _orderServiceTest = orderServiceTest;
        }
        #endregion

        #region Messaging and Error Handling
        [TempData]
        public string FeedBackMessage { get; set; }

        public string ErrorMessage { get; set; }

        //a get property that returns the result of the lamda action
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
        public bool HasFeedBack => !string.IsNullOrWhiteSpace(FeedBackMessage);

        //used to display any collection of errors on web page
        //whether the errors are generated locally OR come form the class library
        //      service methods
        public List<string> ErrorDetails { get; set; } = new();

        //PageModel local error list for collection 
        public List<Exception> Errors { get; set; } = new();

        #endregion

        #region Employee Validation
        [BindProperty]
        public int LoginID { get; set; }
        [BindProperty]
        public string Phone { get; set; }
        public EmployeeInfo? Employee { get; set; }
        #endregion

        #region Purchase Order Totals
        public decimal subtotal { get; set; }
        public decimal gst { get; set; }
        public decimal total { get; set; }
        #endregion

        #region Routing Parameters

        [BindProperty(SupportsGet = true)]
        public int VendorID { get; set; }

        [BindProperty]
        public PurchaseOrderInfo? PurchaseOrder { get; set; }

        [BindProperty]
        public List<itemOnInventoryOrOrder> VendorInventory { get; set; }
        [BindProperty]
        public List<itemOnInventoryOrOrder> CurrentOrder { get; set; } = new();

        [BindProperty]
        public int itemToMoveOnOrder { get; set; }

        [BindProperty]
        public int vendorItemToMove { get; set; }
        [BindProperty]
        public int orderOption { get; set; }

        #region refresh Item
        [BindProperty]
        public int itemToRefersh { get; set; }
        [BindProperty]
        public decimal inputPrice { get; set; }
        [BindProperty]
        public int inputQTO { get; set; }
        #endregion

        #endregion
        public VendorInfo? Vendor { get; set; }

        public void OnGet()
        {
            if (VendorID > 0)
            {
                // Get the Vendor by Vendor ID
                GetVendorInfo();

                // Get Purchase Order by VendorID
                GetPurchaseOrder();

                // Get employee 
                if (LoginID > 0 && string.IsNullOrEmpty(Phone))
                {
                    Employee = _securityService.GetEmployeeInfoPurchasing(LoginID, Phone);
                    GetEmployeeInfo();
                }
                // Populate the CurrentOrder and VendoryInventory lists
                if (PurchaseOrder != null)
                {
                    CurrentOrder = _orderServiceTest.ActiveOrderDisplay((int)PurchaseOrder.PurchaseOrderID);
                }
                else
                {
                    CurrentOrder = _orderServiceTest.SuggestedOrderDisplay((int)VendorID);
                }

                if (CurrentOrder != null)
                {
                    GetOrderTotals();
                }

                if (VendorInventory == null && CurrentOrder != null)
                {
                    VendorInventory = _orderServiceTest.VendorInventoryDisplay((int)VendorID, CurrentOrder);
                }
            }
        }

        #region Item service method
        public void OnPostAddItem()
        {
            try
            {
                if (vendorItemToMove <= 0)
                {
                    ErrorMessage = "Unable to process add Item.";
                }
                else
                {
                    _orderServiceTest.OrderService_AddItem(vendorItemToMove, CurrentOrder, VendorInventory);
                    CurrentOrder.Sort((x, y) => x.stockItemID.CompareTo(y.stockItemID));
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = GetInnerException(ex).Message;
            }
            RepopulateFields();
        }

        public void OnPostRemoveOrderItem()
        {
            try
            {
                if (itemToMoveOnOrder <= 0)
                {
                    ErrorMessage = "Unable to process add Item.";
                }
                else
                {
                    _orderServiceTest.OrderService_RemoveItem(itemToMoveOnOrder, CurrentOrder, VendorInventory);
                    VendorInventory.Sort((x, y) => x.stockItemID.CompareTo(y.stockItemID));
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = GetInnerException(ex).Message;
            }
            RepopulateFields();
        }

        public void OnPostRefreshOrderItem()
        {
            if (itemToRefersh <= 0)
            {
                ErrorMessage = "Unable to process your request.";
            }
            if (inputQTO <= 0)
            {
                ErrorMessage = "QTO must be greater than 0";
            }
            if (inputPrice < 0)
            {
                ErrorMessage = "Price must be greater or equal than 0";
            }
            else
            {
                var item = CurrentOrder.Where(x => x.stockItemID == itemToRefersh).SingleOrDefault();
                if (item != null)
                {
                    try
                    {
                        item.Total = inputPrice * inputQTO;
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = GetInnerException(ex).Message;
                    }
                }
            }
            RepopulateFields();
        }
        #endregion

        #region command button

        public IActionResult OnPostUpdateOrder()
        {
            try
            {
                if (VendorID <= 0)
                {
                    Errors.Add(new Exception("You must select a vendor."));
                }
                if (LoginID <= 0)
                {
                    Errors.Add(new Exception("You must be logged in to perform this action."));
                }
                if (CurrentOrder == null || CurrentOrder.Count <= 0)
                {
                    Errors.Add(new Exception("You must select one item to purchase."));
                }
                if (Errors.Any())
                {
                    throw new AggregateException(Errors);
                }
                if (CurrentOrder == null)
                {
                    throw new ArgumentNullException("Order not exist");
                }
                GetPurchaseOrder();
                if (PurchaseOrder!=null)
                {
                    _orderServiceTest.SaveCurrentOrder(LoginID, PurchaseOrder, CurrentOrder);
                }
                else
                {
                    _orderServiceTest.SaveNewOrder(VendorID, LoginID, CurrentOrder);
                }
                FeedBackMessage = $"Order has update.";
                return RedirectToPage(new
                {
                    LoginID = LoginID,
                    Phone = Phone,
                    VendorID = VendorID
                });
            }
            catch (AggregateException ex)
            {
                ErrorMessage = "Unable to process your request.";
                foreach (var error in ex.InnerExceptions)
                {
                    ErrorDetails.Add(error.Message);
                }
                RepopulateFields();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = GetInnerException(ex).Message;
                RepopulateFields();
                return Page();
            }
        }

        public IActionResult OnPostPlaceOrder()
        {
            try
            {
                if (LoginID <= 0)
                {
                    Errors.Add(new Exception("Re-login please."));
                }
                if (PurchaseOrder == null)
                {
                    Errors.Add(new Exception("There was an error with the purchase order. Please try again."));
                }
                if (CurrentOrder.Count == 0)
                {
                    Errors.Add(new Exception("There was an error with the current order list. Please try again."));
                }
                if (Errors.Any())
                {
                    throw new AggregateException(Errors);
                }
                GetPurchaseOrder();
                if(PurchaseOrder != null)
                {
                    _orderServiceTest.PlaceOrder(LoginID,VendorID, PurchaseOrder, CurrentOrder);
                }
                else
                {
                    Errors.Add(new Exception("Update Order First."));
                }

                FeedBackMessage = $"Order placed.";

                return RedirectToPage(new
                {
                    LoginID = LoginID,
                    Phone = Phone,
                    VendorID = VendorID
                });
            }
            catch (AggregateException ex)
            {
                ErrorMessage = "Unable to process your request.";
                foreach (var error in ex.InnerExceptions)
                {
                    ErrorDetails.Add(error.Message);
                }
                RepopulateFields();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = GetInnerException(ex).Message;
                RepopulateFields();
                return Page();
            }
        }

        public IActionResult OnPostDeleteOrder()
        {
            try
            {
                GetPurchaseOrder();
                if (PurchaseOrder == null)
                {
                    throw new ArgumentNullException("No Purchase Order specified. Please try again.");
                }

                _orderServiceTest.DeleteOrder(PurchaseOrder);

                FeedBackMessage = $"Order deleted.";
            }
            catch (Exception ex)
            {
                ErrorMessage = GetInnerException(ex).Message;
                RepopulateFields();
                return Page();
            }

            return RedirectToPage(new
            {
                LoginID = LoginID,
                Phone = Phone,
                VendorID = VendorID
            });
        }

        public IActionResult OnPostClear()
        {
            FeedBackMessage = "";
            ModelState.Clear();
            return RedirectToPage(new { VendorID=0});
        }

        #endregion

        #region vendor and purchase order method
        public void GetPurchaseOrder()
        {
            if (VendorID > 0)
            {
                PurchaseOrder = _orderServiceTest.GetPurchaseOrder((int)VendorID);
            }
        }
        public void GetOrderTotals()
        {
            const decimal GST = 0.05m;
            subtotal = 0;
            gst = 0;
            total = 0;
            foreach (var item in CurrentOrder)
            {
                if (item.QTO > 0 )
                {
                    subtotal += item.Price * (decimal)item.QTO;
                    gst += (item.Price * GST) * (decimal)item.QTO;
                }

                total = subtotal + gst;
            }
        }
        public void GetVendorInfo()
        {
            Vendor = _vendorService.Vendor_DisplayInfo((int)VendorID);
        }

        public IActionResult OnPostFindOrderAndEmployeeValidation()
        {
            try
            {
                if (LoginID <= 0 || string.IsNullOrEmpty(Phone))
                {
                    throw new ArgumentNullException("Error with employee login: empty login input values needed.");
                }
                if (VendorID <= 0)
                {
                    throw new ArgumentOutOfRangeException(("Please Enter vendor"));
                }
                Employee = _securityService.GetEmployeeInfoPurchasing(LoginID, Phone);
                PurchaseOrder = _orderServiceTest.GetPurchaseOrder((int)VendorID);
                if (Employee != null)
                {
                    if (Employee.IsManager == false)
                    {
                        throw new Exception("Only department head can view this page");
                    }
                    FeedBackMessage += $"Employee {Employee.FirstName} {Employee.LastName} validated.";
                }
                return RedirectToPage(new
                {
                    LoginID = LoginID,
                    Phone = Phone,
                    VendorID = VendorID
                });
            }
            catch (AggregateException ex)
            {
                foreach (var error in ex.InnerExceptions)
                {
                    ErrorDetails.Add(error.Message);
                }
                return Page();
            }
            catch (Exception ex)
            {
                FeedBackMessage = GetInnerException(ex).Message;
                Employee = null;
                return Page();
            }
        }
        #endregion

        #region Employee Methods
        public void GetEmployeeInfo()
        {
            if (LoginID > 0 && string.IsNullOrEmpty(Phone))
            {
                Employee = _securityService.GetEmployeeInfoPurchasing((int)LoginID, (string)Phone);
            }
        }
        #endregion

        public void RepopulateFields()
        {
            GetVendorInfo();
            GetPurchaseOrder();
            VendorInventory = _orderServiceTest.VendorInventoryDisplay((int)VendorID, CurrentOrder);
            GetOrderTotals();
            GetEmployeeInfo();
        }
        private Exception GetInnerException(Exception ex)
        {
            while (ex.InnerException != null)
                ex = ex.InnerException;
            return ex;
        }
    }
}
