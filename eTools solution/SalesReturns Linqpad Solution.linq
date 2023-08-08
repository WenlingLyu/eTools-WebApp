<Query Kind="Program">
  <Connection>
    <ID>b32deab1-0dee-449e-b4be-adacc2d4cdf3</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <Server>.</Server>
    <Database>eTools2021</Database>
    <DisplayName>eTools2021 Entity Framework Core</DisplayName>
    <DriverData>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

void Main()
{	
	string searchInput = "";
	string searchType = "";
	string category = "";
	string trigger = "";
	string saleid = "";
	string couponCode = ""; 
	
	SaleTRX newSale = new SaleTRX
	{
		EmployeeID = 1,
		Date = DateTime.Now,
	};
	CouponInfo coupon = new CouponInfo
	{
		CouponID = 4,
		CouponValue = "Joy23",
		Discount = 5
	};
	List<SaleDetailTRX> itemCart = new List<SaleDetailTRX>();
	
	//Drivers
	//TestCategoryList();

	//searchType = "Category";
	//TestItemListBySearch(searchInput, searchType); //-> No search value submitted

	//searchInput = "Hand Saw";
	//TestItemListBySearch(searchInput, searchType); //-> No search type submitted

	//searchInput = "Hand Saw";
	//searchType = "erick";
	//TestItemListBySearch(searchInput, searchType); //-> Invalid search type

	//searchInput = "Hand Saw";
	//searchType = "Category";
	//TestItemListBySearch(searchInput, searchType); //-> Search by category

	//searchInput = "Dewalt";
	//searchType = "ItemName";
	//TestItemListBySearch(searchInput, searchType); //-> Search by item name
	
	//TestItemListByCategory(category); //-> No category selected
	
	//category = "All";
	//TestItemListByCategory(category); //-> Display all items
	
	//category = "Hand Saw";
	//TestItemListByCategory(category); //-> Display items in selected category
	
	//TestCouponSearch(couponCode); //-> No coupon code submitted
	
	//couponCode = "Joy23";
	//TestCouponSearch(couponCode); 
	
	//trigger="Scenario1";
	//TestItemCartData(itemCart, newSale, coupon, trigger); //-> no items in item cart, "No items in cart found."
	
	//trigger = "Scenario2";
	//TestItemCartData(itemCart, newSale, coupon, trigger); //-> no payment type found
	
	//trigger = "Scenario3";
	//TestItemCartData(itemCart, newSale, coupon, trigger); //-> invalid payment type
	
	//trigger = "Scenario4";
	//TestItemCartData(itemCart, newSale, coupon, trigger); //-> bad quantity to be ordered
	
	//trigger = "Scenario5";
	//TestItemCartData(itemCart, newSale, coupon, trigger); //-> good data
	
	
	//saleid = "erick";
	//newSale = TestRetrieveSaleBySearch(saleid); //-> bad saleid input
	
	//saleid = "6"; //-> input saleid of previously added sale record
	//newSale = TestRetrieveSaleBySearch(saleid); //-> valid saleid
	//itemCart = TestRetrieveOrderItems(newSale);
	//trigger = "Scenario1";
	//TestProcessRefund(newSale, itemCart, trigger); //-> order details not found
	//trigger = "Scenario2";
	//TestProcessRefund(newSale, itemCart, trigger); //-> there are no items to be refunded
	//trigger = "Scenario3";
	//TestProcessRefund(newSale, itemCart, trigger); //-> item to be refunded not found
	//trigger = "Scenario4";
	//TestProcessRefund(newSale, itemCart, trigger); //-> good data
}

#region Driver Methods
void TestCategoryList()
{
	List<CategoryFetch> categoryList = CategoryService_ListCategories();
	categoryList.Dump();
}

void TestItemListBySearch(string searchInput, string searchType)
{
	List<SearchItemInfo> itemList = StockItemService_ListItemsBySearch(searchInput, searchType);
	itemList.Dump();
}

void TestItemListByCategory(string categorySelection)
{
	List<SearchItemInfo> itemList = StockItemService_ListItemsByCategory(categorySelection);
	itemList.Dump();
}

void TestCouponSearch(string couponCode)
{
	CouponInfo newCoupon = new CouponInfo();
	newCoupon = SaleService_RetrieveCouponBySearch(couponCode);
	newCoupon.Dump();
}

void TestItemCartData(List<SaleDetailTRX> cartData, SaleTRX saleInfo, CouponInfo coupon, string trigger)
{
	SaleTRX newSaleInfo = null;
	string paymentType = "M";

	switch (trigger)
	{
		case "Scenario1":
			{
				newSaleInfo = CalculateOrderTotals(cartData, coupon);
				SaleService_PlaceOrder(newSaleInfo, paymentType, coupon, cartData);
				break;
			}
		case "Scenario2":
			{
				cartData.Add(new SaleDetailTRX
				{
					StockItemID = 23,
					Description = "Dewalt Multi Speed Drill",
					SellingPrice = 45.6500m,
					Quantity = 1,
					Selected = true
				});
				cartData.Add(new SaleDetailTRX
				{
					StockItemID = 23,
					Description = "Dewalt Multi Speed Sander",
					SellingPrice = 49.6500m,
					Quantity = 2,
					Selected = true
				});
				paymentType = "";

				newSaleInfo = CalculateOrderTotals(cartData, coupon);
				SaleService_PlaceOrder(newSaleInfo, paymentType, coupon, cartData);
				break;
			}
		case "Scenario3":
			{
				cartData.Add(new SaleDetailTRX
				{
					StockItemID = 23,
					Description = "Dewalt Multi Speed Drill",
					SellingPrice = 45.6500m,
					Quantity = 1,
					Selected = true
				});
				cartData.Add(new SaleDetailTRX
				{
					StockItemID = 23,
					Description = "Dewalt Multi Speed Sander",
					SellingPrice = 49.6500m,
					Quantity = 2,
					Selected = true
				});
				paymentType = "F";

				newSaleInfo = CalculateOrderTotals(cartData, coupon);
				SaleService_PlaceOrder(newSaleInfo, paymentType, coupon, cartData);
				break;
			}
		case "Scenario4":
			{
				cartData.Add(new SaleDetailTRX
				{
					StockItemID = 23,
					Description = "Dewalt Multi Speed Drill",
					SellingPrice = 45.6500m,
					Quantity = 66,
					Selected = true
				});
				cartData.Add(new SaleDetailTRX
				{
					StockItemID = 23,
					Description = "Dewalt Multi Speed Sander",
					SellingPrice = 49.6500m,
					Quantity = 2,
					Selected = true
				});

				newSaleInfo = CalculateOrderTotals(cartData, coupon);
				SaleService_PlaceOrder(newSaleInfo, paymentType, coupon, cartData);
				break;
			}
		case "Scenario5":
			{	
				cartData.Add(new SaleDetailTRX
				{
					StockItemID = 23,
					Description = "Dewalt Multi Speed Drill",
					SellingPrice = 45.6500m,
					Quantity = 1,
					Selected = true
				});
				cartData.Add(new SaleDetailTRX
				{
					StockItemID = 34,
					Description = "Dewalt Multi Speed Sander",
					SellingPrice = 49.6500m,
					Quantity = 2,
					Selected = true
				});
				StockItems.Where(x => x.StockItemID == cartData[0].StockItemID).Dump();
				StockItems.Where(x => x.StockItemID == cartData[1].StockItemID).Dump();
				newSaleInfo = CalculateOrderTotals(cartData, coupon);
				saleInfo.Tax = newSaleInfo.Tax;
				saleInfo.SubTotal = newSaleInfo.SubTotal;
				SaleService_PlaceOrder(saleInfo, paymentType, coupon, cartData);
				Sales.Dump();
				StockItems.Where(x => x.StockItemID == cartData[0].StockItemID).Dump();
				StockItems.Where(x => x.StockItemID == cartData[1].StockItemID).Dump();
				break;
			}
		default:
			{
				break;
			}
	}
}

public SaleTRX TestRetrieveSaleBySearch(string saleid)
{	
	SaleTRX retrievedSale = null;
	retrievedSale = SaleRefundService_RetrieveSaleBySearch(saleid);
	retrievedSale.Dump();
	return retrievedSale;
}

public List<SaleDetailTRX> TestRetrieveOrderItems(SaleTRX saleInfo)
{
	List<SaleDetailTRX> orderItems = new();
	orderItems = SaleRefundService_RetrieveOrderItems(saleInfo);
	orderItems.Dump();
	return orderItems;
}

void TestProcessRefund(SaleTRX saleInfo, List<SaleDetailTRX> orderItems, string trigger)
{
	switch(trigger)
	{	
		case "Scenario1":
		{	
			SaleTRX emptyOrder = null;
			SaleRefundService_ProcessRefund(emptyOrder, orderItems);
			break;
		}
		case "Scenario2":
		{
			List<SaleDetailTRX> emptyOrderItems = new();
			SaleRefundService_ProcessRefund(saleInfo, emptyOrderItems);
			break;
		}
		case "Scenario3":
		{	
			orderItems[0].StockItemID = 1;
			SaleRefundService_ProcessRefund(saleInfo, orderItems);
			break;
			}
		case "Scenario4":
			{	
				StockItems.Where(x => x.StockItemID == orderItems[0].StockItemID).Select(x => x.QuantityOnHand).Dump();
				StockItems.Where(x => x.StockItemID == orderItems[1].StockItemID).Select(x => x.QuantityOnHand).Dump();
				SaleRefundService_ProcessRefund(saleInfo, orderItems);
				SaleRefunds.Dump();
				StockItems.Where(x => x.StockItemID == orderItems[0].StockItemID).Select(x => x.QuantityOnHand).Dump();
				StockItems.Where(x => x.StockItemID == orderItems[1].StockItemID).Select(x => x.QuantityOnHand).Dump();
				break;
			}
		default:
		{
			break;
		}
	}
}
#endregion

#region Data Models
#region Query
public class CategoryFetch
{
	public int CategoryID { get; set; }
	public string Description { get; set; }
	public int ItemCount { get; set; }
}

public class SearchItemInfo
{
	public int StockItemID { get; set; }
	public string Description { get; set; }
	public decimal Price { get; set; }
	public int QuantityOnHand { get; set; }
	public bool Discontinued { get; set; }
}

public class CouponInfo
{
	public int CouponID { get; set; }
	public string CouponValue { get; set; }
	public int Discount { get; set; }
}

#endregion
#region Query/Command
public class SaleTRX
{
	public int SaleID { get; set; }
	public int EmployeeID { get; set; }
	public DateTime Date { get; set; }
	public string PaymentType { get; set; }
	public decimal Tax { get; set; }
	public decimal SubTotal { get; set; }
	public int? CouponID { get; set; }
	public int DiscountPercent { get; set; }
	public decimal? Discount { get; set; }
}
public class SaleDetailTRX
{
	public int SaleDetailID { get; set; }
	public int SaleID { get; set; }
	public int StockItemID { get; set; }
	public string Description { get; set; }
	public decimal SellingPrice { get; set; }
	public int Quantity { get; set; }
	public bool Selected { get; set; }
}
#endregion
#endregion

#region Transactional Service Methods
#region StockItemServices BLL
#region Queries
public List<CategoryFetch> CategoryService_ListCategories()
{
	IEnumerable<CategoryFetch> results = Categories
											.OrderBy(x => x.Description)
											.Select(x => new CategoryFetch
											{
												CategoryID = x.CategoryID,
												Description = x.Description,
												ItemCount = x.StockItems.Count()
											})
											;
	return results.ToList();
}

public List<SearchItemInfo> StockItemService_ListItemsBySearch(string searchInput, string searchType)
{
	IEnumerable<SearchItemInfo> results = null;
	if (string.IsNullOrWhiteSpace(searchInput))
	{
		throw new ArgumentNullException("No search value submitted");
	}
	if (string.IsNullOrWhiteSpace(searchType))
	{
		throw new ArgumentNullException("No search type submitted");
	}

	if (searchType == "Category")
	{
		results = StockItems
			.Where(x => x.Category.Description.Contains(searchInput) && x.Discontinued == false)
			.Select(x => new SearchItemInfo
			{
				StockItemID = x.StockItemID,
				Description = x.Description,
				Discontinued = x.Discontinued,
				QuantityOnHand = x.QuantityOnHand,
				Price = x.SellingPrice
			});
	}
	else if (searchType == "ItemName")
	{
		results = StockItems
			.Where(x => x.Description.Contains(searchInput) && x.Discontinued == false)
			.Select(x => new SearchItemInfo
			{
				StockItemID = x.StockItemID,
				Description = x.Description,
				Discontinued = x.Discontinued,
				QuantityOnHand = x.QuantityOnHand,
				Price = x.SellingPrice
			});
	}
	else
	{
		throw new ArgumentException("Invalid search type.");
	}

	return results.ToList();
}

public List<SearchItemInfo> StockItemService_ListItemsByCategory(string category)
{
	IEnumerable<SearchItemInfo> results = null;
	if (string.IsNullOrWhiteSpace(category))
	{
		throw new ArgumentNullException("No category selected");
	}

	if (category == "All")
	{
		results = StockItems
			.Where(x => x.Discontinued == false)
			.Select(x => new SearchItemInfo
			{
				StockItemID = x.StockItemID,
				Description = x.Description,
				Discontinued = x.Discontinued,
				QuantityOnHand = x.QuantityOnHand,
				Price = x.SellingPrice
			});
	}
	else
	{
		results = StockItems
			.Where(x => x.Category.Description == category && x.Discontinued == false)
			.Select(x => new SearchItemInfo
			{
				StockItemID = x.StockItemID,
				Description = x.Description,
				Discontinued = x.Discontinued,
				QuantityOnHand = x.QuantityOnHand,
				Price = x.SellingPrice
			});
	}

	return results.ToList();
}
#endregion
#endregion

#region SaleServices BLL
#region Queries
public CouponInfo SaleService_RetrieveCouponBySearch(string couponValue)
{
	if (string.IsNullOrWhiteSpace(couponValue))
	{
		throw new ArgumentNullException("No coupon code submitted");
	}

	CouponInfo result = Coupons
							.Where(x => x.CouponIDValue == couponValue)
							.Select(x => new CouponInfo
							{
								CouponID = x.CouponID,
								CouponValue = x.CouponIDValue,
								Discount = x.CouponDiscount
							})
							.FirstOrDefault()
							;
	return result;
}
#endregion
#region Command Methods
public void SaleService_PlaceOrder(SaleTRX saleInfo, string paymentType, CouponInfo coupon, List<SaleDetailTRX> orderItems)
{
	// Note to add in the future: still not sure how i'm going to retrieve an employee id from the webapp without a login,
	// but im going to assume its been retrieved
	StockItems stockItem = null;
	Sales sale = null;
	SaleDetails saleDetails = null;
	List<Exception> errorList = new List<Exception>();
	int stockQuantity = 0;

	if (string.IsNullOrEmpty(paymentType))
	{
		throw new ArgumentNullException("No payment type found.");
	}

	if (orderItems.Count() == 0)
	{
		throw new ArgumentException("No items in cart found.");
	}

	if (coupon != null)
	{
		saleInfo.CouponID = coupon.CouponID;
	}

	if (paymentType == "M" || paymentType == "C" || paymentType == "D")
	{	
		// create sale item
		sale = new Sales()
		{
			SaleDate = saleInfo.Date,
			PaymentType = paymentType,
			EmployeeID = saleInfo.EmployeeID,
			TaxAmount = saleInfo.Tax,
			SubTotal = saleInfo.SubTotal,
			CouponID = saleInfo.CouponID
		};

		Sales.Add(sale);

		foreach (var item in orderItems)
		{
			stockQuantity = StockItems
							.Where(x => x.StockItemID == item.StockItemID)
							.Select(x => x.QuantityOnHand)
							.FirstOrDefault();

			if (stockQuantity < item.Quantity)
			{
				errorList.Add(new Exception("The quantity ordered must not exceed current in-stock quantities."));
			}
			else
			{
				stockItem = StockItems
								.Where(x => x.StockItemID == item.StockItemID)
								.Select(x => x)
								.FirstOrDefault();

				stockItem.QuantityOnHand -= item.Quantity;
				StockItems.Update(stockItem);

				// create a new sale details items
				saleDetails = new SaleDetails()
				{
					SaleID = sale.SaleID,
					StockItemID = item.StockItemID,
					Quantity = item.Quantity,
					SellingPrice = item.SellingPrice
				};

				sale.SaleDetails.Add(saleDetails);
			}
		}
	}
	else 
	{
		errorList.Add(new Exception("Invalid payment type"));
	}

	if (errorList.Count > 0)
	{
		//  throw the list of business processing error(s)
		throw new AggregateException("Unable to place new order Check concerns", errorList);
	}
	else
	{
		//_context.
		SaveChanges();
	}
}
#endregion
#endregion

#region SaleRefundServices BLL
#region Queries
public SaleTRX SaleRefundService_RetrieveSaleBySearch(string saleID)
{
	int saleid = 0;
	if (string.IsNullOrWhiteSpace(saleID))
	{
		throw new ArgumentNullException("A sale ID is required in order to retrieve sales");
	}
	else if (!Int32.TryParse(saleID, out int parseResult))
	{
		throw new ArgumentException("A sale ID must be a whole number greater than zero");
	}
	else
	{
		saleid = Int32.Parse(saleID);
	}

	SaleTRX results = Sales
						.Where(x => x.SaleID == saleid)
						.Select(x => new SaleTRX
						{
							SaleID = x.SaleID,
							EmployeeID = x.EmployeeID,
							Date = x.SaleDate,
							PaymentType = x.PaymentType,
							DiscountPercent = x.Coupon.CouponDiscount
						})
						.FirstOrDefault();

	return results;
}

public List<SaleDetailTRX> SaleRefundService_RetrieveOrderItems(SaleTRX saleInfo)
{
	IEnumerable<SaleDetailTRX> results = SaleDetails
											.Where(x => x.SaleID == saleInfo.SaleID)
											.Select(x => new SaleDetailTRX
											{
												SaleDetailID = x.SaleDetailID,
												SaleID = x.SaleID,
												StockItemID = x.StockItemID,
												Description = x.StockItem.Description,
												Quantity = x.Quantity,
												SellingPrice = x.SellingPrice
											});

	return results.ToList();
}
#endregion
#region Command Methods
public void SaleRefundService_ProcessRefund(SaleTRX refundOrderInfo, List<SaleDetailTRX> refundItems)
{
	StockItems itemExists = null;
	SaleRefunds newRefund = null;
	SaleRefundDetails newRefundDetails = null;
	List<Exception> errorList = new List<Exception>();
	if (refundOrderInfo == null)
	{
		throw new ArgumentNullException("Refund order details is missing");
	}

	// add new sale refunds item
	newRefund = new SaleRefunds
	{
		SaleRefundDate = DateTime.Now,
		SaleID = refundOrderInfo.SaleID,
		EmployeeID = refundOrderInfo.EmployeeID,
		TaxAmount = refundOrderInfo.Tax,
		SubTotal = refundOrderInfo.SubTotal
	};

	if (refundItems.Count() == 0)
	{
		errorList.Add(new Exception("There are no items to be refunded")); ;
	}
	else
	{
		SaleRefunds.Add(newRefund);

		foreach (var item in refundItems)
		{
			newRefundDetails = new SaleRefundDetails
			{
				SaleRefundID = newRefund.SaleRefundID,
				StockItemID = item.StockItemID,
				SellingPrice = item.SellingPrice,
				Quantity = item.Quantity
			};

			itemExists = StockItems
							.Where(x => x.StockItemID == item.StockItemID)
							.Select(x => x)
							.FirstOrDefault();

			if (itemExists == null)
			{
				errorList.Add(new Exception("Item to be refunded not found")); ;
			}
			else
			{
				itemExists.QuantityOnHand += item.Quantity;
				StockItems.Update(itemExists);
				newRefund.SaleRefundDetails.Add(newRefundDetails);
			}
		}
	}

	if (errorList.Count > 0)
	{
		//  throw the list of business processing error(s)
		throw new AggregateException("Unable to process refund. Check concerns", errorList);
	}
	else
	{
		//_context.
		SaveChanges();
	}
}
#endregion
#endregion
#endregion

// This section is for purposes of coding my final solution, not required for this deliverable

#region Shopping Page CodeBehind 
public List<SaleDetailTRX> AddSalesLines(List<SaleDetailTRX> itemCart, List<SaleDetailTRX> orderedItems)
{
	SaleDetailTRX itemInfo = null;
	List<Exception> errorList = new List<Exception>();

	List<SaleDetailTRX> keepList = orderedItems
									.Where(x => x.Selected)
									.Select(x => x)
									.ToList();

	if (keepList.Count() == 0)
	{
		errorList.Add(new Exception("Must select an item to be able to add to the cart."));
	}

	foreach (var item in keepList)
	{
		if (item.Quantity <= 0)
		{
			errorList.Add(new Exception("The quantity of items ordered must be a whole number greater than 0"));
		}
		else
		{
			// check if item exists in cart already
			itemInfo = itemCart
						.Where(x => x.StockItemID == item.StockItemID)
						.Select(x => x)
						.FirstOrDefault();

			if (itemInfo != null)
			{
				itemInfo = UpdateQuantity(itemCart, item.StockItemID, item.Quantity);
				itemCart[item.StockItemID] = itemInfo;
			}
			else
			{
				itemInfo = new SaleDetailTRX
				{
					StockItemID = item.StockItemID,
					Description = item.Description,
					SellingPrice = item.SellingPrice,
					Quantity = item.Quantity
				};

				itemCart.Add(itemInfo);
			}
		}
	}

	if (errorList.Count > 0)
	{
		//  throw the list of business processing error(s)
		throw new AggregateException("Unable to register new employee. Check concerns", errorList);
	}
	else
	{
		return itemCart;
	}
}
public List<SaleDetailTRX> RemoveSalesLines(List<SaleDetailTRX> itemCart, int stockItemID)
{
	SaleDetailTRX removeItem = null;

	removeItem = itemCart
					.Where(x => x.StockItemID == stockItemID)
					.Select(x => x)
					.FirstOrDefault();

	if (removeItem == null)
	{
		throw new ArgumentNullException("Must select an item to remove before removing from cart");
	}
	else
	{
		itemCart.Remove(removeItem);
	}

	return itemCart;
}

public SaleDetailTRX UpdateQuantity(List<SaleDetailTRX> itemCart, int stockItemID, int quantity)
{
	List<Exception> errorList = new List<Exception>();
	SaleDetailTRX itemInfo = new SaleDetailTRX();

	itemInfo = itemCart
						.Where(x => x.StockItemID == stockItemID)
						.Select(x => x)
						.FirstOrDefault();

	if (itemInfo == null)
	{
		errorList.Add(new Exception("No items selected to update quantity"));
	}
	else
	{
		if (quantity <= 0)
		{
			errorList.Add(new Exception("The quantity of items ordered must be a whole number greater than 0"));
		}
		else
		{
			itemInfo.Quantity = quantity;
		}
	}

	if (errorList.Count > 0)
	{
		//  throw the list of business processing error(s)
		throw new AggregateException("Unable to update quantity of item. Check concerns", errorList);
	}
	else
	{
		return itemInfo;
	}
}
#endregion

#region View Cart CodeBehind 
public decimal CalcCartSubtotal(List<SaleDetailTRX> cartItems)
{
	decimal subTotal = 0;
	// Triggered by OnGet request
	// Displays order total of initial item cart
	if (cartItems.Count() == 0)
	{
		throw new ArgumentNullException("Item cart is empty, add items to cart");
	}

	foreach (var item in cartItems)
	{
		subTotal += item.SellingPrice * item.Quantity;
	}

	return subTotal;
}

// Updating Quantities in View Cart screen will use the same UpdateQuantity method from the Shopping Screen CodeBehind
#endregion

#region Checkout Page CodeBehind
public SaleTRX CalculateOrderTotals(List<SaleDetailTRX> orderItems, CouponInfo coupon)
{
	SaleTRX newSale = null;

	decimal subTotal = 0.00m;
	decimal tax = 0.00m;
	decimal discount = 0;

	foreach (var item in orderItems)
	{
		subTotal += item.SellingPrice * item.Quantity;
		tax += subTotal * 0.05m;
		discount += subTotal * (coupon.Discount / 100);
	}

	newSale = new SaleTRX
	{
		Date = DateTime.Now,
		SubTotal = subTotal,
		Tax = tax,
		Discount = discount,
		DiscountPercent = coupon.Discount
	};

	return newSale;
}
#endregion

#region Refund Page CodeBehind 

#endregion
