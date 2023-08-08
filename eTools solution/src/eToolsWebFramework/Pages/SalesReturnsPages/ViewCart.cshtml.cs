#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#region Additional Namespaces
using EToolsSecurity.ViewModel;
using SalesReturnsSystem.DataModels.SalesReturns;
using SalesReturnsSystem.DataModels.Sales;
#endregion

namespace eToolsWebFramework.Pages.SalesReturnsPages
{
    public class ViewCartModel : PageModel
    {   
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
        public string SearchInput { get; set; }
        [BindProperty]
        public string SearchType { get; set; }
        [BindProperty]
        public string CategorySelection { get; set; }
        [BindProperty]
        public List<SearchItemInfo> RetrievedItems { get; set; } = new();
        [BindProperty]
        public List<SaleDetailTRX> CartItems { get; set; }
        [BindProperty]
        public List<SaleDetailTRX> SelectedItems { get; set; }
        [BindProperty]
        public SaleTRX SaleInfo { get; set; }
        [BindProperty]
        public CouponInfo Coupon { get; set; }

        public void OnGet()
        {   
            
        }

        public void OnPost()
        {
            Feedback = "";
            Error = "";
        }

        public IActionResult OnPostUpdateQuantity()
        {
            try
            {
                Error = "";

                var selected = SelectedItems.Where(x => x.SelectedItem == true).Select(x => x).FirstOrDefault();

                if (selected == null)
                {
                    throw new ArgumentNullException("Error updating item quantity: Must select an item to update quantity.");
                }

                foreach (var item in SelectedItems)
                {
                    // Selected item for updated quantity
                    if (item.SelectedItem == true)
                    {
                        Feedback = "";

                        if (item.Quantity < 0)
                        {
                            throw new ArgumentException($"Error updating item quantity: {item.Description}'s quantity to order must be a positive whole number.");
                        }

                        int quantityOnHand = RetrievedItems
                                                .Where(x => x.StockItemID == item.StockItemID)
                                                .Select(x => x.QuantityOnHand)
                                                .FirstOrDefault();

                        if (item.Quantity > quantityOnHand)
                        {
                            throw new ArgumentException($"Error updating item quantity: {item.Description}'s quantity to order must not exceed existing stock quantities.");
                        }

                        if (item.Quantity == 0)
                        {
                            

                            SaleInfo.SubTotal -= item.ItemTotal;

                            var index = CartItems.FindIndex(x => x.StockItemID == item.StockItemID);

                            CartItems.Remove(CartItems[index]);
                        }
                        else
                        {
                            SaleInfo.SubTotal -= item.ItemTotal;
                            SaleInfo.SubTotal += item.Quantity * item.SellingPrice;
                            item.ItemTotal = item.Quantity * item.SellingPrice;

                            int index = CartItems.FindIndex(x => x.StockItemID == item.StockItemID);
                            CartItems[index] = item;

                            Feedback = $"Item {item.Description} successfully updated quantity.";
                            FeedbackDetails.Add(Feedback);
                        }
                    }
                }

                return Page();
            }
            catch (AggregateException ex)
            {
                Error = "Unable to update quantity";
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

        public IActionResult OnPostRemoveItem()
        {
            try
            {
                Error = "";

                var selected = SelectedItems.Where(x => x.SelectedItem == true).Select(x => x).FirstOrDefault();

                if (selected == null)
                {
                    throw new ArgumentNullException("Error removing item: Must select an item to remove.");
                }

                foreach (var item in SelectedItems)
                {
                    if (item.SelectedItem == true)
                    {
                        Feedback = "";

                        SaleInfo.SubTotal -= item.ItemTotal;

                        var index = CartItems.FindIndex(x => x.StockItemID == item.StockItemID);

                        CartItems.Remove(CartItems[index]);

                        Feedback = $"Item {item.Description} successfully removed.";
                        FeedbackDetails.Add(Feedback);
                    }
                }

                return Page();
            }
            catch (AggregateException ex)
            {
                Error = "Unable to remove item";
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
