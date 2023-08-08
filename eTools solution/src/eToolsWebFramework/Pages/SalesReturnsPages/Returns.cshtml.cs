#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#region Additional Namespaces
using EToolsSecurity.ViewModel;
using SalesReturnsSystem.DataModels.SalesReturns;
using SalesReturnsSystem.DataModels.Sales;
using SalesReturnsSystem.BLL;
#endregion

namespace eToolsWebFramework.Pages.SalesReturnsPages
{
    public class ReturnsModel : PageModel
    {
        #region Private variables and DI constructor
        private readonly ReturnsService _returnsService;

        public ReturnsModel(ReturnsService returnsService)
        {
            _returnsService = returnsService;
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
        public SaleTRX RefundInfo { get; set; }
        [BindProperty]
        public EmployeeInfo Employee { get; set; }
        [BindProperty]
        public string SaleID { get; set; }
        [BindProperty]
        public decimal RefundTotal { get; set; }
        [BindProperty]
        public int SelectedItem { get; set; }
        [BindProperty]
        public List<SaleDetailTRX> RefundItems { get; set; }
        [BindProperty]
        public List<SaleDetailTRX> InitialItems { get; set; }
        [BindProperty]
        public bool SuccessfulTransaction { get; set; } 

        public void OnGet()
        {
        }

        public void OnPost()
        {
            Error = "";
            Feedback = "";
            RefundInfo = null;
            SuccessfulTransaction = false;
        }

        public IActionResult OnPostClear()
        {   
            Error = "";
            SaleID = null;
            RefundTotal = 0.00m;
            RefundInfo = null;
            RefundItems = new List<SaleDetailTRX>();
            
            Feedback = "Page cleared. You may now search for a new order.";
            FeedbackDetails.Add(Feedback);
            return Page();
        }

        public IActionResult OnPostSaleSearch()
        {
            try
            {
                Error = "";
                Feedback = "";
                RefundTotal = 0.00m;
                int saleID = 0;
                RefundItems = new List<SaleDetailTRX>();

                if (string.IsNullOrWhiteSpace(SaleID))
                {
                    throw new ArgumentNullException("A sale ID is required in order to retrieve sales");
                }
                else if (!Int32.TryParse(SaleID, out int parseResult))
                {
                    throw new ArgumentException("A sale ID must be a whole number greater than zero");
                }
                else
                {
                    saleID = Int32.Parse(SaleID);
                }

                RefundInfo = _returnsService.ReturnsService_RetrieveSaleBySearch(saleID);

                if (RefundInfo == null)
                {
                    Feedback += $"Your search input of {SaleID} does not currently match any sales records existing in the database.";
                    FeedbackDetails.Add(Feedback);
                }
                else
                {
                    InitialItems = _returnsService.ReturnsService_RetrieveOrderItems(RefundInfo);

                    Feedback += $"Sale Search Successful: displaying order information of [Sale ID: {RefundInfo.SaleID}] ";
                    FeedbackDetails.Add(Feedback);
                }
                return Page();
            }
            catch (AggregateException ex)
            {
                Error = "Unable to search for sale record:";
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

        public IActionResult OnPostUpdateReturnQuantities()
        {
            return Page();
        }

        public IActionResult OnPostUpdateReturnTotals()
        {
            try
            {
                Error = "";
                Feedback = "";

                decimal discountValue = (decimal)RefundInfo.DiscountPercent / 100;
                var selectedItem = InitialItems.SingleOrDefault(x => x.SaleDetailID == SelectedItem);
                var index = RefundItems.FindIndex(x => x.SaleDetailID == SelectedItem);

                foreach (var item in InitialItems)
                {
                    if (selectedItem.SaleDetailID == item.SaleDetailID)
                    {
                        int totalQty = item.ReturnQuantity + item.QuantityRefunded;

                        if (totalQty > item.Quantity)
                        {
                            item.ReturnQuantity = 0;
                            throw new ArgumentException("Existing return quantities indicate your total return quantities exceed the original order quantities.");
                        }

                        if (item.ReturnQuantity > item.Quantity)
                        {
                            item.ReturnQuantity = 0;
                            throw new ArgumentException("Existing return quantities indicate your total return quantities exceed the original order quantities.");
                        }

                        if (item.ReturnQuantity <= 0)
                        {
                            item.ReturnQuantity = 0;
                            throw new ArgumentException("Your return quantity is invalid (must be a positive whole number greater than zero/can not exceed order quantities.");
                        }

                        var existingItem = RefundItems.SingleOrDefault(x => x.SaleDetailID == SelectedItem);

                        if (existingItem == null)
                        {   
                            RefundItems.Add(item);                           
                        }
                        else
                        {
                            decimal oldSubtotal = existingItem.ReturnQuantity * item.SellingPrice;
                            RefundItems[index].ReturnQuantity = item.ReturnQuantity; 
                            RefundInfo.SubTotal -= oldSubtotal;
                            RefundInfo.Tax -= oldSubtotal * 0.05m;
                            RefundInfo.Discount -= oldSubtotal * discountValue;
                        }

                        decimal itemTotal = item.ReturnQuantity * item.SellingPrice;

                        RefundInfo.SubTotal += itemTotal;
                        RefundInfo.Tax += itemTotal * 0.05m;

                        RefundInfo.Discount += itemTotal * discountValue;
                        RefundTotal = RefundInfo.SubTotal + RefundInfo.Tax - RefundInfo.Discount;

                        Feedback = "Your refund totals have been updated.";
                        FeedbackDetails.Add(Feedback);
                    }
                }
                
                return Page();
            }
            catch (AggregateException ex)
            {
                Error = "Error with processing return quantities:";
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

        public IActionResult OnPostProcessRefund()
        {
            try
            {
                Error = "";
                Feedback = "";

                RefundInfo.Date = DateTime.Now;
                SuccessfulTransaction = _returnsService.ReturnsService_ProcessRefund(RefundInfo, RefundItems);
                RefundTotal = RefundInfo.SubTotal + RefundInfo.Tax - RefundInfo.Discount;

                Feedback = $"Your refund transaction (Refund ID: {RefundInfo.SaleRefundID}) has been successfully processed.";
                FeedbackDetails.Add(Feedback);
                return Page();
            }
            catch (AggregateException ex)
            {
                Error = "Unable to process refund:";
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
