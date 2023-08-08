<Query Kind="Program">
  <Connection>
    <ID>7d3f6805-0dfc-45b6-8f81-ed81b0aa50bb</ID>
    <NamingServiceVersion>2</NamingServiceVersion>
    <Persist>true</Persist>
    <Driver Assembly="(internal)" PublicKeyToken="no-strong-name">LINQPad.Drivers.EFCore.DynamicDriver</Driver>
    <Database>eTools2021</Database>
    <Server>JUL</Server>
    <DisplayName>eTools-Entity</DisplayName>
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
		int testPurchaseOrderID = 358;
		
		List<ReceiveOrder> goodReceiveInfo = new List<ReceiveOrder>();
		
		goodReceiveInfo.Add(new ReceiveOrder()
		{
			StockItemID = 5567,
			VendorStockNumber = null,
			QtyReceived = 20,
			QtyReturned = 0,
			ReturnReason = null
		});
		
		List<PurchaseOrder> ActivePOs = PurchaseOrders_Fetch();
		ActivePOs.Dump("Active POs");
		
		List<PurchaseOrderDetail> PODetails = PODetails_FetchByPurchaseOrderID(testPurchaseOrderID);
		PODetails.Dump("Intial PO Details");
		
		List<UnOrderedItem> UnorderedItems = UnOrderedItems_Fetch();
		UnorderedItems.Dump("UnOrdered Returns Cart");
		
		OrderService_ReceiveOrder(testPurchaseOrderID, goodReceiveInfo);
		
		//List<PurchaseOrderDetail> UpdatedPODetails = PODetails_FetchByPurchaseOrderID(testPurchaseOrderID);
		//PODetails.Dump("Updated PO Details");
	}
	catch (AggregateException ex)
	{
		foreach (var error in ex.InnerExceptions)
		{
			error.Message.Dump();
		}
	}
	catch (ArgumentNullException ex)
	{
		GetInnerException(ex).Message.Dump();
	}
	catch (Exception ex)
	{
		GetInnerException(ex).Message.Dump();
	}
}

// You can define other methods, fields, classes and namespaces here

#region Methods
private Exception GetInnerException(Exception ex)
{
	while (ex.InnerException != null)
		ex = ex.InnerException;
	return ex;
}
#endregion


#region Data Models
public class PurchaseOrder
{
  public int PurchaseOrderID { get; set; }
  public DateTime? PODate { get; set; }
  public string VendorName { get; set; }
  public string VendorPhone { get; set; }
}

public class PurchaseOrderDetail
{
    public int StockItemID { get; set; }
    public string StockDescription { get; set; }
    public int QtyOnOrder { get; set; }
    public int QtyOutstanding { get; set; }
    public int QtyReceived { get; set; }
    public int QtyReturned { get; set; }
    public string ReturnReason { get; set; }
}

public class UnOrderedItem
{
	public int CartID { get; set; }
	public string Description { get; set; }
	public string VendorStockNumber { get; set; }
	public int Quantity { get; set; }
}

public class ReceiveOrder
{
    public int StockItemID { get; set; }
    public string VendorStockNumber { get; set; }
    public int QtyReceived { get; set; }
    public int QtyReturned { get; set; }
    public string ReturnReason { get; set; }
}

public class CloseOrder
{
    public string ForceCloseReason { get; set; }
    public bool Closed { get; set; }
}

public class InsertUnorderedItem
{
    public string StockDescription { get; set; }
    public string VendorStockNumber { get; set; }
    public int QtyReturned { get; set; }
}

public class RemoveUnorderedItem
{
    public string VendorStockNumber { get; set; }
}
#endregion


#region query
public List<PurchaseOrder> PurchaseOrders_Fetch()
{
	IEnumerable<PurchaseOrder> ActivePurchaseOrders = PurchaseOrders
			.Where(x => x.OrderDate != null && x.Closed == false)
			.Select(x => new PurchaseOrder
			{
				PurchaseOrderID = x.PurchaseOrderID,
				PODate = x.OrderDate,
				VendorName = x.Vendor.VendorName,
				VendorPhone = x.Vendor.Phone
			});
	return ActivePurchaseOrders.ToList();
}

public List<PurchaseOrderDetail> PODetails_FetchByPurchaseOrderID(int purchaseOrderID)
{
	IEnumerable<PurchaseOrderDetail> PODetails = PurchaseOrderDetails
			.Where(x => x.PurchaseOrderID == purchaseOrderID)
			.Select(x => new PurchaseOrderDetail
			{
				StockItemID = x.StockItemID,
				StockDescription = x.StockItem.Description,
				QtyOnOrder = x.StockItem.QuantityOnOrder,
				QtyOutstanding = x.StockItem.QuantityOnOrder,
				QtyReceived = ReceiveOrderDetails
								.Where(r => r.PurchaseOrderDetailID == x.PurchaseOrderDetailID)
								.Select (r => r.QuantityReceived).Sum(),
				QtyReturned = ReturnedOrderDetails
								.Where(ro => ro.PurchaseOrderDetailID == x.PurchaseOrderDetailID)
								.Select (ro => ro.Quantity).Sum(),
				ReturnReason = ReturnedOrderDetails
								.Where(ro => ro.PurchaseOrderDetailID == x.PurchaseOrderDetailID)
								.Select (ro => ro.Reason).FirstOrDefault()
			});
			
	return PODetails.ToList();
}

public List<UnOrderedItem> UnOrderedItems_Fetch()
{
	IEnumerable<UnOrderedItem> UnOrderedItemDetails = UnOrderedItems
			.Select(x => new UnOrderedItem
			{
				CartID = x.ItemID,
				Description = x.ItemName,
				VendorStockNumber = x.VendorProductID,
				Quantity = x.Quantity
			});
			
	return UnOrderedItemDetails.ToList();
}

#endregion

#region Command Methods
void OrderService_ReceiveOrder(int purchaseOrderID, List<ReceiveOrder> receivedItemInfo)
{
	bool purchaseOrderExist = false;
	//PurchaseOrderDetails receivedQuantities = null;
	ReturnedOrderDetails returnedQuantities = null;

	// parameter validation
	List<Exception> errorList = new List<Exception>();
	
	purchaseOrderExist = PurchaseOrders
							.Where(x => x.PurchaseOrderID == purchaseOrderID)
							.Select(x => x.PurchaseOrderID)
							.Any();
	if (!purchaseOrderExist)
	{
		throw new ArgumentNullException("Purchase Order does not exist.");
	}
	
	foreach (var entry in receivedItemInfo)
	{
		
		
		
	}
							
}
#endregion

