#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#region Additional Namespaces
using EToolsSecurity.ViewModel;
using SalesReturnsSystem.BLL;
using SalesReturnsSystem.DataModels.SalesReturns;
using SalesReturnsSystem.DataModels.Sales;
#endregion

namespace eToolsWebFramework.Pages.SalesReturnsPages
{
    public class CheckoutModel : PageModel
    {
        #region Private variables and DI constructor
        private readonly SaleService _saleService;

        public CheckoutModel(SaleService salesService)
        {
            _saleService = salesService;
        }
        #endregion

        #region Messaging and Error Handling
        [TempData]
        public string Feedback { get; set; }
        [TempData]
        public string Error { get; set; }
        public bool HasFeedback => !string.IsNullOrWhiteSpace(Feedback);
        public bool HasError => !string.IsNullOrWhiteSpace(Error);
        public List<string> ErrorDetails { get; set; } = new();
        public List<string> FeedbackDetails { get; set; } = new();
        public List<Exception> Errors { get; set; } = new();
        #endregion

        [BindProperty]
        public EmployeeInfo Employee { get; set; }
        [BindProperty]
        public decimal OrderTotal { get; set; }
        [BindProperty]
        public string SearchInput { get; set; }
        [BindProperty]
        public string SearchType { get; set; }
        [BindProperty]
        public string PaymentType { get; set; }
        [BindProperty]
        public string CategorySelection { get; set; }
        [BindProperty]
        public string CouponInput { get; set; }
        [BindProperty]
        public int SaleID { get; set; }
        [BindProperty]
        public List<SaleDetailTRX> CartItems { get; set; }
        [BindProperty]
        public List<SearchItemInfo> RetrievedItems { get; set; } = new();
        [BindProperty]
        public CouponInfo Coupon { get; set; }
        [BindProperty]
        public SaleTRX SaleInfo { get; set; }

        public void OnGet()
        {
            Error = "";
            Feedback = "";
        }

        public void OnPost()
        {
            Error = "";
            Feedback = "";
            SaleInfo.EmployeeID = Employee.EmployeeID;
            decimal discountValue = (decimal)Coupon.Discount / 100;
            SaleInfo.Discount = SaleInfo.SubTotal * discountValue;
            SaleInfo.Tax = SaleInfo.SubTotal * 0.05m;
            OrderTotal = SaleInfo.SubTotal + SaleInfo.Tax - SaleInfo.Discount;
        }

        public IActionResult OnPostValidateCoupon()
        {
            try
            {
                Error = "";
                Feedback = "";

                if (string.IsNullOrWhiteSpace(CouponInput))
                {
                    throw new ArgumentNullException("Error applying coupon: Coupon search value is empty.");
                }

                if (CouponInput == Coupon.CouponValue)
                {
                    throw new ArgumentException("Error applying coupon: Coupon already applied to Cart");
                }

                Coupon = _saleService.SaleService_RetrieveCouponBySearch(CouponInput);

                if (Coupon == null)
                {
                    Feedback += $"Your coupon search of {CouponInput} was not a valid coupon.";
                }
                else
                {
                    decimal discountValue = (decimal)Coupon.Discount / 100;
                    SaleInfo.Discount = SaleInfo.SubTotal * discountValue;
                    OrderTotal -= SaleInfo.Discount;
                    Feedback += $"The coupon {Coupon.CouponValue} successfully applied to cart.";
                }

                FeedbackDetails.Add(Feedback);
                return Page();
            }
            catch (AggregateException ex)
            {
                Error = "Unable to validate coupon:";
                foreach (var error in ex.InnerExceptions)
                {
                    ErrorDetails.Add(error.Message);

                }
                return Page();
            }
            catch (Exception ex)
            {
                Error = GetInnerException(ex).Message;
                return Page();
            }
        }

        public IActionResult OnPostProcessOrder()
        {
            try
            {
                Error = "";
                Feedback = "";

                if (string.IsNullOrEmpty(PaymentType))
                {
                    throw new ArgumentNullException("Must select a payment type before placing order.");
                }
               
                SaleInfo.PaymentType = PaymentType;

                SaleID = _saleService.SaleService_PlaceOrder(SaleInfo, Coupon, CartItems);

                if (SaleID != 0)
                {
                    Feedback += $"Your order of SaleID: {SaleID} has been placed. Please keep the number for any refunds/returns in the future. You may now begin a new order.";
                    FeedbackDetails.Add(Feedback);
                    CartItems = new List<SaleDetailTRX>();
                    RetrievedItems = new List<SearchItemInfo>();
                    Coupon = new();
                    SaleInfo = new();
                    SearchInput = null;
                    SearchType = null;
                    CategorySelection = "0";
                }
                return Page();
            }
            catch (AggregateException ex)
            {
                Error = "Unable to process order:";
                foreach (var error in ex.InnerExceptions)
                {
                    ErrorDetails.Add(error.Message);

                }
                return Page();
            }
            catch (Exception ex)
            {
                Error = GetInnerException(ex).Message;
                return Page();
            }
        }
        
        public Exception GetInnerException(Exception ex)
        {
            while (ex.InnerException != null)
                ex = ex.InnerException;
            return ex;
        }
    }
}
