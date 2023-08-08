#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#region Additional Namespaces
using WebApp.Helpers;
using EToolsSecurity.BLL;
using EToolsSecurity.ViewModel;
using SalesReturnsSystem.DataModels.SalesReturns;
using SalesReturnsSystem.DataModels.Sales;
using SalesReturnsSystem.BLL;

#endregion

namespace eToolsWebFramework.Pages.SalesReturnsPages
{
    public class SalesModel : PageModel
    {
        #region Private variables and DI constructor
        private readonly CategoryService _categoryService;
        private readonly StockItemService _stockItemService;
        private readonly SecurityService _securityService;

        public SalesModel(CategoryService categoryServices, StockItemService stockItemServices, SecurityService securityServices)
        {
            _categoryService = categoryServices; 
            _stockItemService = stockItemServices;
            _securityService = securityServices;
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

        #region Paginator
        private const int PAGE_SIZE = 5;
        public Paginator Pager { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? currentPage { get; set; }
        #endregion

        #region Employee Validation
        [BindProperty]
        public string LoginID { get; set; }
        [BindProperty]
        public string Phone { get; set; }
        [BindProperty]
        public EmployeeInfo Employee { get; set; }
        #endregion

        [BindProperty]
        public int TotalItemCount { get; set; }
        [BindProperty]
        public string SearchInput { get; set; }
        [BindProperty]
        public string SearchType { get; set; }
        [BindProperty]
        public string CategorySelection { get; set; }
        [BindProperty]
        public SaleTRX SaleInfo { get; set; } = new();
        [BindProperty]
        public CouponInfo Coupon { get; set; }
        [BindProperty]
        public List<SaleDetailTRX> SelectedItems { get; set; }
        [BindProperty]
        public List<SearchItemInfo> RetrievedItems { get; set; } = new();
        [BindProperty]
        public List<SearchItemInfo> DisplayItems { get; set; } = new();
        [BindProperty]
        public List<SaleDetailTRX> CartItems { get; set; }
        public List<CategoryFetch> CategoryList { get; set; } = new();
        
        public void OnGet()
        {
            try
            {
                Feedback = "";
                Error = "";
                SaleInfo.Date = DateTime.Now;

                PopulateCategoryList();
            }
            catch (Exception ex)
            {   
                GetInnerException(ex);
            }
        }

        public void OnPost()
        {
            Feedback = "";
            Error = "";

            SelectedPageItems();
            PopulateCategoryList();
        }

        public IActionResult OnPostDisplaySelectedPage()
        {
            Feedback = "";
            Error = "";

            SelectedPageItems();
            PopulateCategoryList();
            return Page();
        }

        public void SelectedPageItems()
        {
            DisplayItems.Clear();
            int pagenumber = currentPage.HasValue ? currentPage.Value : 1;
            PageState current = new(pagenumber, PAGE_SIZE);
            int totalcount = RetrievedItems.Count();
            int rowsskipped = (pagenumber - 1) * PAGE_SIZE;

            foreach (var item in RetrievedItems.Skip(rowsskipped).Take(PAGE_SIZE))
            {
                DisplayItems.Add(item);
            }

            Pager = new(totalcount, current);
        }

        public IActionResult OnPostNewOrder()
        {
            Error = "";
            Feedback = "Cancel Transaction successful. Returned to Shopping screen.";
            FeedbackDetails.Add(Feedback);
            CartItems = new List<SaleDetailTRX>();
            RetrievedItems = new List<SearchItemInfo>();
            Coupon = new();
            SaleInfo = new();
            SearchInput = null;
            SearchType = null;
            CategorySelection = "0";
            return Page();
        }

        public IActionResult OnPostEmployeeValidation()
        {
            try
            {
                Error = "";
                Feedback = "";

                if (string.IsNullOrEmpty(LoginID) || string.IsNullOrEmpty(Phone))
                {
                    throw new ArgumentNullException("Error with employee login: empty login input values.");
                }

                Employee = _securityService.GetEmployeeInfoSales(LoginID, Phone);
                SaleInfo.EmployeeID = Employee.EmployeeID;
                Feedback += $"Employee {Employee.FirstName} {Employee.LastName} validated.";
                FeedbackDetails.Add(Feedback);

                
                PopulateCategoryList();
                return Page();
            }
            catch (AggregateException ex)
            {
                Error = "Error with Employee login:";
                foreach (var error in ex.InnerExceptions)
                {
                    ErrorDetails.Add(error.Message);

                }
                
                Employee = null;
                PopulateCategoryList();
                return Page();
            }
            catch (Exception ex)
            {
                Error = GetInnerException(ex).Message;

                Employee = null;
                PopulateCategoryList();
                return Page();
            }
        }

        public IActionResult OnPostSearchItemsByInput()
        {
            try
            {
                Error = "";
                Feedback = "";
                CategorySelection = "0";

                if (string.IsNullOrWhiteSpace(SearchInput))
                {
                    throw new ArgumentNullException("No search value submitted");
                }
                if (string.IsNullOrWhiteSpace(SearchType))
                {
                    throw new ArgumentNullException("No search type submitted");
                }

                RetrievedItems = _stockItemService.StockItemService_ListItemsBySearch(SearchInput, SearchType);

                if (RetrievedItems.Count() == 0)
                {
                    Feedback += $"Your search query of {SearchInput} did not bring back any results. Try again.";
                    FeedbackDetails.Add(Feedback);
                }

                SelectedPageItems();
                PopulateCategoryList();
                return Page();
            }
            catch (AggregateException ex)
            {
                Error = "Unable to process search";

                foreach (var error in ex.InnerExceptions)
                {
                    ErrorDetails.Add(error.Message);

                }

                SelectedPageItems();
                PopulateCategoryList();
                return Page();
            }
            catch (Exception ex)
            {
                Error = GetInnerException(ex).Message;

                SelectedPageItems();
                PopulateCategoryList();
                return Page();
            }
        }

        public IActionResult OnPostSearchItemsByCategory()
        {
            try
            {
                Error = "";
                Feedback = "";
                SearchInput = "";

                if (CategorySelection == "0")
                {
                    throw new ArgumentNullException("Must select a category to display items.");
                }

                RetrievedItems = _stockItemService.StockItemService_ListItemsByCategory(CategorySelection);

                SelectedPageItems();
                PopulateCategoryList();
                return Page();
            }
            catch (AggregateException ex)
            {
                Error = "Unable to process search";

                foreach (var error in ex.InnerExceptions)
                {
                    ErrorDetails.Add(error.Message);

                }

                SelectedPageItems();
                PopulateCategoryList();
                return Page();
            }
            catch (Exception ex)
            {
                Error = GetInnerException(ex).Message;

                SelectedPageItems();
                PopulateCategoryList();
                return Page();
            }
        }

        public IActionResult OnPostAddToCart()
        {
            try
            {
                Error = "";

                var selected = SelectedItems.Where(x => x.SelectedItem == true).Select(x => x).FirstOrDefault();

                if (selected == null)
                {
                    throw new ArgumentNullException("Error adding item to cart: Must select an item to add to cart.");
                }

                foreach (var item in SelectedItems)
                {
                    if (item.SelectedItem == true)
                    {
                        Feedback = "";

                        if (item.Quantity <= 0)
                        {
                            throw new ArgumentException($"Error adding item to cart: {item.Description}'s quantity to order must be a positive whole number.");
                        }
                        
                        int quantityOnHand = RetrievedItems
                                                .Where(x => x.StockItemID == item.StockItemID)
                                                .Select(x => x.QuantityOnHand)
                                                .FirstOrDefault();

                        if (item.Quantity > quantityOnHand)
                        {
                            throw new ArgumentException($"Error adding item to cart: {item.Description}'s quantity to order must not exceed existing stock quantities.");
                        }

                        SaleDetailTRX itemExists = null;

                        itemExists = CartItems
                            .Where(x => x.StockItemID == item.StockItemID)
                            .Select(x => x)
                            .FirstOrDefault();

                        if (itemExists == null)
                        {
                            item.ItemTotal = item.Quantity * item.SellingPrice;
                            CartItems.Add(item);
                            SaleInfo.SubTotal += item.ItemTotal;
                            Feedback = $"{item.Description} successfully added to cart.";
                            FeedbackDetails.Add(Feedback);
                        }
                        else
                        {
                            SaleInfo.SubTotal -= itemExists.ItemTotal;
                            itemExists.Quantity += item.Quantity;
                            itemExists.ItemTotal = itemExists.Quantity * item.SellingPrice;
                            SaleInfo.SubTotal += itemExists.ItemTotal;

                            var index = CartItems.FindIndex(x => x.StockItemID == item.StockItemID);
                            CartItems[index] = itemExists;

                            Feedback += $"{itemExists.Description} already in cart: updated quantity levels in cart.";
                            FeedbackDetails.Add(Feedback);
                        }
                    }
                }

                SelectedPageItems();
                PopulateCategoryList();
                return Page();
            }
            catch (AggregateException ex)
            {
                Error = "Unable to add item to cart";
                foreach (var error in ex.InnerExceptions)
                {
                    ErrorDetails.Add(error.Message);

                }

                SelectedPageItems();
                PopulateCategoryList();
                return Page();
            }
            catch (Exception ex)
            {
                Error = GetInnerException(ex).Message;

                SelectedPageItems();
                PopulateCategoryList();
                return Page();
            }
        }

        public void PopulateCategoryList()
        {
            CategoryList = _categoryService.CategoryService_ListCategories();
            foreach(var category in CategoryList)
            {
                TotalItemCount += category.ItemCount;
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
