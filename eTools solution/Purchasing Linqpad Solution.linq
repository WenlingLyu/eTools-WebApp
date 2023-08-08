<Query Kind="Program">
  <Connection>
    <ID>13e125ce-9591-4214-9ca0-55c34c01ea02</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <Server>LAPTOP-5ERRQBHS</Server>
    <Database>eTools2021</Database>
    <DisplayName>eTools202_Entity</DisplayName>
    <DriverData>
      <PreserveNumeric1>True</PreserveNumeric1>
      <EFProvider>Microsoft.EntityFrameworkCore.SqlServer</EFProvider>
    </DriverData>
  </Connection>
</Query>

void Main()
{
	try
	{			
		//Driver
		//Test_EmpolyeeQuery();
			//Employee_DisplayInfo("Donna","Bookem");
		//Test_VendorInfoQuery();
		VendorInfo TestVendor = new VendorInfo();
		TestVendor.VendorName = "THOMAS Brown & Sons"; //"Hardware International";//"HandTools Wholesale"
			//Vendor_DisplayInfo("Nuts, Bolts and Fastseners");
		//Set up currentOrder and display current order
		EmployeeInfo TestUser = new EmployeeInfo();
		TestUser.FirstName = "Hess";
		TestUser.LastName = "Agonor";
		List<CurrentOrder> CurrentOrderTest = OrderDisplay(TestUser,TestVendor);
		Console.WriteLine("--------------Original List---------------");
		CurrentOrderTest.Dump();
		//Set up currentInventory and display current inventory list;
		List<VendorInventory> VendorInventoryTest = InventoryDisplay(TestUser,TestVendor);
		VendorInventoryTest.Dump();
		//Test_AddItemTRX();
		Console.WriteLine("--------------List after add item---------------");
			//OrderService_AddItem(5606,CurrentOrderTest,VendorInventoryTest);
			//CurrentOrderTest.Dump();
			//VendorInventoryTest.Dump();
		//Test_RemoveItemTRX();
		Console.WriteLine("--------------List after remove item---------------");
			//OrderService_RemoveItem(5595, CurrentOrderTest, VendorInventoryTest);
			//OrderService_RemoveItem(5599, CurrentOrderTest, VendorInventoryTest);
			//OrderService_RemoveItem(5601, CurrentOrderTest, VendorInventoryTest);
			//OrderService_RemoveItem(5606, CurrentOrderTest, VendorInventoryTest);
			//OrderService_RemoveItem(5611, CurrentOrderTest, VendorInventoryTest);
			//CurrentOrderTest.Dump();
			//VendorInventoryTest.Dump();
		//Test_RefreshItemTRX();
		Console.WriteLine("--------------List after refresh item---------------");
			CurrentActiveOrderTRXInput InputTest = new CurrentActiveOrderTRXInput();
			InputTest.StockItemID = 5611;
			InputTest.QuantityToOrder = 10;
			InputTest.Price = 10;
			//OrderService_RefreshItemTotalPrice(InputTest, CurrentOrderTest, VendorInventoryTest);
			//CurrentOrderTest.Dump();
			//VendorInventoryTest.Dump();
		//Test_UPDOrder();
		   ModifyOrderTRXInput modifyInput = new ModifyOrderTRXInput();
		   modifyInput.ModifyOperation = "Delete";
		   OrderService_UPDOrder(modifyInput,CurrentOrderTest,TestVendor,TestUser);
	}
	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	catch (ArgumentException ex)
	{

		GetInnerException(ex).Message.Dump();
	}
	catch (AggregateException ex)
	{
		foreach (var error in ex.InnerExceptions)
		{
			error.Message.Dump();
		}
	}
	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
}

#region Get Inner Exception
//Driver Methods
//general method to drill down into an exception of obtain the InnerException where your
//  actual error is detailed

private Exception GetInnerException(Exception ex)
{
	while (ex.InnerException != null)
		ex = ex.InnerException;
	return ex;
}

#endregion

#region Query Models
public class EmployeeInfo
{
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Description {get;set;}
}
public class VendorInfo
{
	public string VendorName { get; set; }
	public int Phone { get; set; }
	public string City { get; set; }
	public int PO { get; set; }
}
public class CurrentOrder
{
	public int StockItemID { get; set; }
	public string Description { get; set;}
	public int QOH { get; set; }
	public int ROL { get; set; }
	public int QOO { get; set; }
	public int QTO { get; set; }
	public decimal Price { get; set; }
	public decimal Total {get;set;}
}
public class VendorInventory
{
	public int StockItemID { get; set; }
	public string Description { get; set;}
	public int QOH { get; set; }
	public int ROL { get; set; }
	public int QOO { get; set; }
	public int Buffer { get; set; }
	public decimal Price { get; set; }
}
public class CurrentActiveOrderTRXInput
{
	public int StockItemID { get; set; }
	public int QuantityToOrder { get; set; }
	public int Price { get; set; }
}
public class ModifyOrderTRXInput
{
	public string ModifyOperation { get; set; }
	//public int PurchaseOrderID { get; set; }
}
#endregion

#region Command models
public void Employee_DisplayInfo(string firstName, string lastName)
{
	string position = Employees.Where(x => x.FirstName == firstName && x.LastName == lastName).Select(x => x.Position.Description).FirstOrDefault();
	if (string.IsNullOrWhiteSpace(firstName))
	{
		throw new ArgumentNullException("No firstName submitted");
	}
	if (string.IsNullOrWhiteSpace(lastName))
	{
		throw new ArgumentNullException("No lastName submitted");
	}
	Employees employeeExist = Employees.Where(x => x.FirstName == firstName && x.LastName == lastName).FirstOrDefault();
	if(employeeExist==null)
	{
		throw new ArgumentException("employee does not exist");
	}
	else
	{
		var result = Employees
		.Where(x => x.FirstName == firstName && x.LastName == lastName)
		.Select(x => new
		{
			FirstName = x.FirstName,
			LastName = x.LastName,
			Position = x.Position.Description
		}).Dump();
	}

}
public void Vendor_DisplayInfo(string vendorName)
{
	if (string.IsNullOrWhiteSpace(vendorName))
	{
		throw new ArgumentNullException("No vendor submitted");
	}
	var CurrentPONumber = PurchaseOrders.Select(p=>p.PurchaseOrderID).OrderByDescending(p=>p).FirstOrDefault();
	var results = Vendors
						.Where(x => x.VendorName == vendorName)
						.Select(x => new {
									SelectedVendor = x.VendorName,
									Phone = x.Phone,
									City = x.City,
									PONumber = x.PurchaseOrders
												.Select(po=>po.OrderDate ==null ? po.PurchaseOrderID : CurrentPONumber+1)
												.FirstOrDefault()
						}).Dump();
}
public List<CurrentOrder> OrderDisplay(EmployeeInfo user, VendorInfo vendor)
{   
	// Check to see input parameter exist
	if (string.IsNullOrWhiteSpace(user.FirstName))
	{
		throw new ArgumentNullException("No user firstName submitted");
	}
	if (string.IsNullOrWhiteSpace(user.LastName))
	{
		throw new ArgumentNullException("No user lastName submitted");
	}
	if (string.IsNullOrWhiteSpace(vendor.VendorName))
	{
		throw new ArgumentNullException("Must select a vendor");
	}

	//Check employee position
	//Employees employeeExist = Employees.Where(x=>x.FirstName == user.FirstName && x.LastName == user.LastName).Select(x=>x).FirstOrDefault();
	//if(employeeExist == null)
	//{
	//	throw new ArgumentException("Emoloyee dose not exist");
	//}
	
	//check if vendor exist
	Vendors vendorExist = Vendors.Where(x=>x.VendorName == vendor.VendorName).Select(x=>x).FirstOrDefault();
	if(vendorExist == null)
	{
		throw new ArgumentException("Vendor does not exist");
	}
	
	// Check to employee position
	string LoggedUserDescription = Employees.Where(x => x.FirstName == user.FirstName && x.LastName == user.LastName).Select(x => x.Position.Description).FirstOrDefault().ToString();

	if (LoggedUserDescription != "Department Head")
	{
		throw new ArgumentException("Only Department Head are allowed to make changes to Order");
	}
	
	IEnumerable<CurrentOrder> OrderDetails = null;
	int CurrentOrderExist = PurchaseOrders
								.Where(x=>x.Vendor.VendorName==vendor.VendorName&&x.OrderDate==null).Count();
	var SuggesttedOrder = StockItems.Where(x => x.Vendor.VendorName == vendor.VendorName && (x.ReOrderLevel - x.QuantityOnHand - x.QuantityOnOrder) > 0).Select(x => x).ToList();
	
	if(CurrentOrderExist>0)
	{
		OrderDetails = PurchaseOrderDetails
				.Where(x=>x.PurchaseOrder.Vendor.VendorName == vendor.VendorName && x.PurchaseOrder.OrderDate == null)
				.Select(x=>new CurrentOrder{
					StockItemID = x.StockItemID,
					Description = x.StockItem.Description,
					QOH = x.StockItem.QuantityOnHand,
					ROL = x.StockItem.ReOrderLevel,
					QOO = x.StockItem.QuantityOnOrder,
					QTO = x.Quantity,
					Price = Math.Round(x.PurchasePrice,2),
					Total =  Math.Round((x.Quantity * x.PurchasePrice),2)
				});
	}
	if(CurrentOrderExist==0)
	{
		OrderDetails = SuggesttedOrder
						.Select(x => new CurrentOrder {
							StockItemID = x.StockItemID,
							Description = x.Description,
							QOH = x.QuantityOnHand,
							ROL = x.ReOrderLevel,
							QOO = x.QuantityOnOrder,
							QTO = x.ReOrderLevel-x.QuantityOnHand-x.QuantityOnOrder,
							Price = Math.Round(x.PurchasePrice,2),
							Total =Math.Round(( (x.ReOrderLevel-x.QuantityOnHand) * x.PurchasePrice),2)
						});
	}
	//SaveChanges();
	return OrderDetails.ToList();																
}
public List<VendorInventory>InventoryDisplay (EmployeeInfo user,VendorInfo vendor)
{
	
	List<VendorInventory> Inventory = null;
	List<CurrentOrder> OrderList = OrderDisplay(user,vendor);
	VendorInventory removeItem = null;
	
	Inventory = StockItems
					.Where(x => x.Vendor.VendorName == vendor.VendorName)
					.Select(x => new VendorInventory{
						StockItemID = x.StockItemID,
						Description = x.Description,
						QOH = x.QuantityOnHand,
						ROL = x.ReOrderLevel,
						QOO = x.QuantityOnOrder,
						Buffer = x.QuantityOnHand - x.ReOrderLevel,
						Price = Math.Round(x.PurchasePrice,2)
					}).ToList();
					

	foreach(var item in OrderList)
	{
		removeItem = Inventory.Where(x=>x.StockItemID == item.StockItemID).FirstOrDefault();
		if(removeItem!=null)
		{
			Inventory.Remove(removeItem);
		}
	}
	
	return Inventory;
}
#endregion

#region Command TRX methods
public void OrderService_AddItem(int stockItemID, List<CurrentOrder> OrderList, List<VendorInventory>InventoryList)
{
	//local variables
	List<Exception> errorList = new List<Exception>();
	VendorInventory addItemExistOnInventory = null;
	CurrentOrder addItemExistOnOrder = null;
	
	// check if the item is on current order list
	addItemExistOnOrder = OrderList.Where(x=>x.StockItemID == stockItemID).FirstOrDefault();
	// item exist
	if(addItemExistOnOrder!=null)
	{
		throw new ArgumentException("Item exist on current order, can not add twice");
	}
	else
	{
		// Item does not exist - add item to current order display
		addItemExistOnOrder = new CurrentOrder()
		{
			StockItemID = stockItemID,
			Description = StockItems.Where(x=>x.StockItemID == stockItemID).Select(x=>x.Description).FirstOrDefault(),
			QOH = StockItems.Where(x=>x.StockItemID == stockItemID).Select(x=>x.QuantityOnHand).FirstOrDefault(),
			ROL = StockItems.Where(x=>x.StockItemID == stockItemID).Select(x=>x.ReOrderLevel).FirstOrDefault(),
			QOO = StockItems.Where(x=>x.StockItemID == stockItemID).Select(x=>x.QuantityOnOrder).FirstOrDefault(),
			QTO = StockItems.Where(x=>x.StockItemID == stockItemID).Select(x=>x.ReOrderLevel-x.QuantityOnHand).FirstOrDefault(),
			Price = Math.Round((StockItems.Where(x=>x.StockItemID == stockItemID).Select(x=>x.PurchasePrice).FirstOrDefault()),2),
			Total = Math.Round((StockItems.Where(x=>x.StockItemID == stockItemID).Select(x=>x.PurchasePrice).FirstOrDefault()*(StockItems.Where(x=>x.StockItemID == stockItemID).Select(x=>x.ReOrderLevel-x.QuantityOnHand).FirstOrDefault())),2)
		};
	}
	

	// Check if this item exist
	addItemExistOnInventory = InventoryList.Where(x=>x.StockItemID==stockItemID).FirstOrDefault();
	
	// If item does not exist in Inventory
	if(addItemExistOnInventory == null)
	{
		throw new ArgumentNullException("Item doesn't exist in inventory");
	}

	// add item to current order
	OrderList.Add(addItemExistOnOrder);
	// delete item on Inventor 
	InventoryList.Remove(addItemExistOnInventory);
	

	if (errorList.Count() > 0)
	{
		// throw the list of business porcessing error(s)
		throw new AggregateException("unable to add track, Check concerns", errorList);
	}
	else
	{
		// consider data valid
		// has passed business processing rules
		SaveChanges();
	}

}
public void OrderService_RemoveItem(int stockItemID, List<CurrentOrder> OrderList, List<VendorInventory>InventoryList)
{
	//local variables
	List<Exception> errorList = new List<Exception>();
	VendorInventory removeItemExistOnInventory = null;
	CurrentOrder removeItemExistOnOrder = null;
	
	// check if the item on Current Order
	removeItemExistOnOrder = OrderList.Where(x=>x.StockItemID == stockItemID).Select(x=>x).FirstOrDefault();
	
	// item does not exist
	if(removeItemExistOnOrder==null)
	{
		throw new ArgumentException("Item does not exist on Current Order.");
	}
	
	//check if the item does not exist on vendor inventory
	removeItemExistOnInventory = InventoryList.Where(x=>x.StockItemID == stockItemID).Select(x=>x).FirstOrDefault();
	
	// item exist on current vendor inventory
	if(removeItemExistOnInventory != null)
	{
		throw new ArgumentException("Item does not belongs to this vendor.");
	}
	else
	{
		removeItemExistOnInventory = new VendorInventory()
										{
											StockItemID = stockItemID,
											Description = StockItems.Where(x=>x.StockItemID== stockItemID).Select(x=>x.Description).FirstOrDefault(),
											QOH = StockItems.Where(x=>x.StockItemID==stockItemID).Select(x=>x.QuantityOnHand).FirstOrDefault(),
											ROL = StockItems.Where(x=>x.StockItemID==stockItemID).Select(x=>x.ReOrderLevel).FirstOrDefault(),
											QOO = StockItems.Where(x=>x.StockItemID==stockItemID).Select(x=>x.QuantityOnOrder).FirstOrDefault(),
											Buffer = StockItems.Where(x=>x.StockItemID==stockItemID).Select(x=>x.QuantityOnHand - x.ReOrderLevel).FirstOrDefault(),
											Price = StockItems.Where(x=>x.StockItemID==stockItemID).Select(x=>x.PurchasePrice).FirstOrDefault()
										};
	}
	
	// Delete item record on Current Order
	OrderList.Remove(removeItemExistOnOrder);
	// add item to current vendor inventory
	InventoryList.Add(removeItemExistOnInventory);

	if (errorList.Count() > 0)
	{
		// throw the list of business porcessing error(s)
		throw new AggregateException("unable to add track, Check concerns", errorList);
	}
	else
	{
		// consider data valid
		// has passed business processing rules
		SaveChanges();
	}
}
public void OrderService_RefreshItemTotalPrice(CurrentActiveOrderTRXInput Input,  List<CurrentOrder> OrderList, List<VendorInventory>InventoryList)
{
	//local variables
	List<Exception> errorList = new List<Exception>();
	CurrentOrder refreshItemExistOnOrder = null;
	
	// Validate input
	
	// Quantity must be greater or equal to 1
	if(Input.QuantityToOrder<=1)
	{
		throw new ArgumentException("Order Quantity must be greater or equal to 1");
	}
	// Price must be greater than 0
	if(Input.Price<=0)
	{
		throw new ArgumentException("Price must be greater than 0");
	}

	// Check if the item exist on Current Order
	// check if the item on Current Order
	refreshItemExistOnOrder = OrderList.Where(x => x.StockItemID == Input.StockItemID).Select(x => x).FirstOrDefault();

	// item does not exist
	if (refreshItemExistOnOrder == null)
	{
		throw new ArgumentException("Item does not exist on Current Order.");
	}
	else
	{
		refreshItemExistOnOrder.Price = (decimal)Input.Price;
		refreshItemExistOnOrder.QTO = Input.QuantityToOrder;
		refreshItemExistOnOrder.Total = (decimal)Input.Price * Input.QuantityToOrder;
	}


	if (errorList.Count() > 0)
	{
		// throw the list of business porcessing error(s)
		throw new AggregateException("unable to add track, Check concerns", errorList);
	}
	else
	{
		// consider data valid
		// has passed business processing rules
		SaveChanges();
	}
}
public void OrderService_UPDOrder(ModifyOrderTRXInput modifyInput,List<CurrentOrder> OrderList,VendorInfo selectedVendor,EmployeeInfo user)
{
	// Check modify input exist and are one of Update, Delete, and Place
	if(string.IsNullOrEmpty(modifyInput.ModifyOperation))
	{
		throw new ArgumentNullException("Must choose your operation");
	}
	if(!(modifyInput.ModifyOperation=="Update"||modifyInput.ModifyOperation=="Delete"||modifyInput.ModifyOperation=="Place"))
	{
		throw new ArgumentException("Input must be Updata, Delete or Place");
	}
	// Check if the vendor exist
	if(string.IsNullOrEmpty(selectedVendor.VendorName))
	{
		throw new ArgumentNullException("Must provide a vendor");
	}

	// Local variable
	List<Exception> errorList = new List<Exception>();
	Vendors vendorExist = null;
	PurchaseOrders orderExist = null;
	PurchaseOrderDetails orderDetails = null;
	PurchaseOrderDetails orderDetailsToUpdate = null;
	List<PurchaseOrderDetails>removelist = new List<PurchaseOrderDetails>();
	List<PurchaseOrderDetails>keeplist = new List<PurchaseOrderDetails>();
	StockItems itemsToUpdate = null;
	PurchaseOrderDetails removeItem = null;
	
	// check if the vendor exist
	vendorExist = Vendors.Where(x=>x.VendorName==selectedVendor.VendorName).FirstOrDefault();
	
	if (selectedVendor == null)
	{
		throw new ArgumentException("Vendor does not exist");
	}

	// Input type == Update
	switch(modifyInput.ModifyOperation)
	{
		case "Update":
		{
			//check if this vendor has purchase order 
			orderExist = PurchaseOrders.Where(x => x.Vendor.VendorName == selectedVendor.VendorName && x.OrderDate == null).FirstOrDefault();
			// When process pruchase Order details, QTO must be greater than 0 and Price must be greater or euqal than o 
			if (OrderList.Count > 0)
			{
				foreach (var item in OrderList)
				{
					if (item.QTO < 1 || item.Price < 0)
					{
						throw new ArgumentException("QTO must greater or equal than 1 and price must be greater or equal than 0");
					}
				}
			}
			//yes -- save current order to purchase order and save purchase order details.
			if (orderExist != null)
			{
				int numAtOrder = orderExist.PurchaseOrderDetails.Count(); // Order exist but without any details, needs to add the current order details into purchaseOrderDetail
				if(numAtOrder==0)
				{
					foreach(var item in OrderList)
					{
						orderDetailsToUpdate = new PurchaseOrderDetails()
						{
							PurchaseOrderID = PurchaseOrders.Where(x => x.Vendor.VendorName == selectedVendor.VendorName && x.OrderDate == null).Select(x => x.PurchaseOrderID).FirstOrDefault(),
							StockItemID = item.StockItemID,
							PurchasePrice = item.Price,
							Quantity = item.QTO
						};
						PurchaseOrderDetails.Add(orderDetailsToUpdate);
					}
				}
				if(numAtOrder>0) //delete details first then re-add current order details at purchase order details
				{
					foreach(var item in orderExist.PurchaseOrderDetails) // delete details
					{
						PurchaseOrderDetails.Remove(item);
					}
					foreach(var item in OrderList) // add details
					{
						orderDetailsToUpdate = new PurchaseOrderDetails()
						{
							PurchaseOrderID = PurchaseOrders.Where(x => x.Vendor.VendorName == selectedVendor.VendorName && x.OrderDate == null).Select(x => x.PurchaseOrderID).FirstOrDefault(),
							StockItemID = item.StockItemID,
							PurchasePrice = item.Price,
							Quantity = item.QTO
						};
						PurchaseOrderDetails.Add(orderDetailsToUpdate);
					}
				}
			}
			//no -- save current suggestted order to purchase order and save purchase order details.
			else
			{
				orderExist = new PurchaseOrders()
				{
					OrderDate = null,
					VendorID = Vendors.Where(x=>x.VendorName==selectedVendor.VendorName).Select(x=>x.VendorID).FirstOrDefault(),
					EmployeeID = Employees.Where(x=>x.FirstName==user.FirstName&&x.LastName==user.LastName).Select(x=>x.EmployeeID).FirstOrDefault(),
					TaxAmount = Math.Round((OrderList.Sum(x=>x.Total)*(decimal)0.05),2),
					SubTotal = OrderList.Sum(x=>x.Total),
					Closed = false,
					Notes = null
				};
				// add suggestted order to purchaseorder 
				PurchaseOrders.Add(orderExist);
				// add suggestted order detail to purchaseOrderDetails
				foreach (var item in OrderList)
				{
					orderDetailsToUpdate = new PurchaseOrderDetails()
					{
						StockItemID = item.StockItemID,
						PurchasePrice = item.Price,
						Quantity = item.QTO
					};
					orderExist.PurchaseOrderDetails.Add(orderDetailsToUpdate);	
					//PurchaseOrders.Update(orderExist);
				}
			}
			break;
			}
		case "Place":
		{
			// check if this order exist in database and match record
			orderExist = PurchaseOrders.Where(x => x.Vendor.VendorName == selectedVendor.VendorName && x.OrderDate == null).FirstOrDefault();
			// if does not exist
			if(orderExist == null)
			{
				throw new ArgumentException("Current order doex not exist in database, update first");
			}
			// check order details match database
			foreach(var item in OrderList)
			{
				orderDetails = PurchaseOrderDetails.Where(x=>x.StockItemID==item.StockItemID&&x.PurchaseOrder.Vendor.VendorName==selectedVendor.VendorName).FirstOrDefault();
				// if item does not exist
				if(orderDetails==null)
				{
					throw new ArgumentException("item does not match database, update first");
				}
				// check if item details match database
				if(orderDetails.Quantity != item.QTO || orderDetails.PurchasePrice != item.Price)
				{
					throw new ArgumentException("item details does not match database, update first");
				}
					// ---------------- !! ISSUE A CONFIRMATION DIALOG WILL BE IMPLEMENTE AT RAZOR PAGE !! -----------------------------------
				// update QTO for this stock item
				itemsToUpdate = StockItems.Where(x=>x.StockItemID==item.StockItemID&&x.Vendor.VendorName==selectedVendor.VendorName).FirstOrDefault();
				itemsToUpdate.QuantityOnOrder = item.QTO;
				StockItems.Update(itemsToUpdate);
				// Update order date
				orderExist.OrderDate = DateTime.Now;
				PurchaseOrders.Update(orderExist);
			}			
			break;
		}
		case "Delete":
		{
			// Check if this Order exist in the purchase order database
			orderExist = PurchaseOrders.Where(x => x.Vendor.VendorName == selectedVendor.VendorName && x.OrderDate == null).FirstOrDefault();
			// Order does not exist
			if(orderExist == null)
			{
				throw new ArgumentException("No current active order, no need to delete.(Did you already place the order?)");
			}
			// Check if the order has not been placed
			if(orderExist.OrderDate != null)
			{
				throw new ArgumentException("Placed order can not be deleted");
			}
			// if order exisi and not placed - delete record on purchase order details
			int numAtOrder = orderExist.PurchaseOrderDetails.Count(); // Order exist but without any details, needs to add the current order details into purchaseOrderDetail
			if (numAtOrder == 0)
			{
				// 直接删除purchase order
				PurchaseOrders.Remove(orderExist);
			}
			if (numAtOrder > 0) //delete details first then re-add current order details at purchase order details
			{
				foreach (var item in orderExist.PurchaseOrderDetails) // delete details --- 先删除了所有Orderdetails
				{
					PurchaseOrderDetails.Remove(item);
				}
				PurchaseOrders.Remove(orderExist);
			}
			break;
		}
	}
	if (errorList.Count() > 0)
	{
		// throw the list of business porcessing error(s)
		throw new AggregateException("unable to modify order, Check concerns", errorList);
	}
	else
	{
		// consider data valid
		// has passed business processing rules
	 	SaveChanges();		
	}
}
#endregion
