using PurchasingSystem.DAL;
using PurchasingSystem.Entities;
using PurchasingSystem.ViewModels;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace PurchasingSystem.BLL
{
    public class OrderServiceTest
    {
        #region Constructor and COntext Dependency

        //  rename the context to match your system
        private readonly PurchasingContext _context;
        internal OrderServiceTest(PurchasingContext context)
        {
            _context = context;
        }
        #endregion

        #region Display order and inventory
        public PurchaseOrderInfo GetPurchaseOrder(int vendorID)
        {
            if (vendorID < 0)
            {
                throw new ArgumentNullException("Select a Vendor.");
            }
            PurchaseOrderInfo? purchaseOrderInfo = null;
            bool purchaseOrderExists = _context.PurchaseOrders.Where(x => x.VendorID == vendorID && x.OrderDate == null).Any();

            if (purchaseOrderExists == true)
            {
                purchaseOrderInfo = _context.PurchaseOrders
                                                 .Where(po => po.VendorID == vendorID
                                                           && po.OrderDate == null)
                                                 .Select(po => new PurchaseOrderInfo
                                                 {
                                                     PurchaseOrderID = po.PurchaseOrderID,
                                                 })
                                                 .FirstOrDefault();
            }
            return purchaseOrderInfo;
        }
        public List<itemOnInventoryOrOrder> SuggestedOrderDisplay(int vendorID)
        {
            if (vendorID < 0)
            {
                throw new ArgumentNullException("Select a Vendor.");
            }

            List<itemOnInventoryOrOrder> suggestedOrder = _context.StockItems
                                                             .Where(x => x.VendorID == vendorID
                                                                      && x.ReOrderLevel - (x.QuantityOnHand + x.QuantityOnOrder) > 0)
                                                             .Select(x => new itemOnInventoryOrOrder
                                                             {
                                                                 stockItemID = x.StockItemID,
                                                                 Description = x.Description,
                                                                 QOH = x.QuantityOnHand,
                                                                 ROL = x.ReOrderLevel,
                                                                 QOO = x.QuantityOnOrder,
                                                                 QTO = x.ReOrderLevel - x.QuantityOnHand,
                                                                 Price = Math.Round(x.PurchasePrice, 2),
                                                                 Total = Math.Round(((x.ReOrderLevel - x.QuantityOnHand) * x.PurchasePrice), 2)
                                                             })
                                                             .ToList();
            return suggestedOrder;
        }

        public List<itemOnInventoryOrOrder> ActiveOrderDisplay(int purchaseOrderID)
        {
            if (purchaseOrderID < 0)
            {
                throw new ArgumentNullException("Must have a purchase order id to process");
            }
            List<itemOnInventoryOrOrder> activeOrder = _context.PurchaseOrderDetails
                                                          .Where(x => x.PurchaseOrderID == purchaseOrderID)
                                                          .Select(x => new itemOnInventoryOrOrder
                                                          {
                                                              stockItemID = x.StockItemID,
                                                              Description = x.StockItem.Description,
                                                              QOH = x.StockItem.QuantityOnHand,
                                                              ROL = x.StockItem.ReOrderLevel,
                                                              QOO = x.StockItem.QuantityOnOrder,
                                                              QTO = x.Quantity,
                                                              Price = Math.Round(x.PurchasePrice, 2),
                                                              Total = Math.Round((x.Quantity * x.PurchasePrice), 2)
                                                          })
                                                          .ToList();
            return activeOrder;
        }

        public List<itemOnInventoryOrOrder> VendorInventoryDisplay(int vendorID, List<itemOnInventoryOrOrder> currentOrder)
        {
            if (vendorID < 0)
            {
                throw new ArgumentNullException("No Vendor ID was supplied. Please try again.");
            }
            List<itemOnInventoryOrOrder> inventory = _context.StockItems
                                                        .Where(x => x.VendorID == vendorID)
                                                        .Select(x => new itemOnInventoryOrOrder
                                                        {
                                                            stockItemID = x.StockItemID,
                                                            Description = x.Description,
                                                            QOH = x.QuantityOnHand,
                                                            ROL = x.ReOrderLevel,
                                                            QOO = x.QuantityOnOrder,
                                                            Buffer = x.QuantityOnHand - x.ReOrderLevel,
                                                            Price = Math.Round(x.PurchasePrice, 2)
                                                        })
                                                        .ToList();

            List<itemOnInventoryOrOrder> vendorInventory = inventory.Where(c => !currentOrder.Any(x => x.stockItemID == c.stockItemID)).ToList();

            return vendorInventory;
        }

        #endregion

        #region ADD DELETE AND REFRESH
        public void OrderService_AddItem(int stockItemID, List<itemOnInventoryOrOrder> OrderList, List<itemOnInventoryOrOrder> InventoryList)
        {
            //local variables
            List<Exception> errorList = new List<Exception>();
            itemOnInventoryOrOrder addItemExistOnInventory = null;
            itemOnInventoryOrOrder addItemExistOnOrder = null;

            // check if the item is on current order list
            addItemExistOnOrder = OrderList.Where(x => x.stockItemID == stockItemID).FirstOrDefault();
            // item exist
            if (addItemExistOnOrder != null)
            {
                throw new ArgumentException("Item exist on current order, can not add twice");
            }
            else
            {
                // Item does not exist - add item to current order display
                addItemExistOnOrder = new itemOnInventoryOrOrder()
                {
                    stockItemID = stockItemID,
                    Description = _context.StockItems.Where(x => x.StockItemID == stockItemID).Select(x => x.Description).FirstOrDefault(),
                    QOH = _context.StockItems.Where(x => x.StockItemID == stockItemID).Select(x => x.QuantityOnHand).FirstOrDefault(),
                    ROL = _context.StockItems.Where(x => x.StockItemID == stockItemID).Select(x => x.ReOrderLevel).FirstOrDefault(),
                    QOO = _context.StockItems.Where(x => x.StockItemID == stockItemID).Select(x => x.QuantityOnOrder).FirstOrDefault(),
                    //QTO = _context.StockItems.Where(x => x.StockItemID == stockItemID).Select(x => x.ReOrderLevel - x.QuantityOnHand).FirstOrDefault(),
                    Price = Math.Round((_context.StockItems.Where(x => x.StockItemID == stockItemID).Select(x => x.PurchasePrice).FirstOrDefault()), 2),
                };
                var itemQTO = _context.StockItems.Where(x => x.StockItemID == stockItemID).Select(x => x.ReOrderLevel - x.QuantityOnHand).FirstOrDefault();
                if (itemQTO <= 0)
                {
                    addItemExistOnOrder.QTO = 1;
                    addItemExistOnOrder.Total = 1 * addItemExistOnOrder.Price;
                }
                else
                {
                    addItemExistOnOrder.QTO = _context.StockItems.Where(x => x.StockItemID == stockItemID).Select(x => x.ReOrderLevel - x.QuantityOnHand).FirstOrDefault();
                    addItemExistOnOrder.Total = Math.Round((_context.StockItems.Where(x => x.StockItemID == stockItemID).Select(x => x.PurchasePrice).FirstOrDefault() * (_context.StockItems.Where(x => x.StockItemID == stockItemID).Select(x => x.ReOrderLevel - x.QuantityOnHand).FirstOrDefault())), 2);
                }
            }

            // Check if this item exist
            addItemExistOnInventory = InventoryList.Where(x => x.stockItemID == stockItemID).FirstOrDefault();

            // If item does not exist in Inventory
            if (addItemExistOnInventory == null)
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
                _context.SaveChanges();
            }

        }

        public void OrderService_RemoveItem(int stockItemID, List<itemOnInventoryOrOrder> OrderList, List<itemOnInventoryOrOrder> InventoryList)
        {
            //local variables
            List<Exception> errorList = new List<Exception>();
            itemOnInventoryOrOrder removeItemExistOnInventory = null;
            itemOnInventoryOrOrder removeItemExistOnOrder = null;

            // check if the item on Current Order
            removeItemExistOnOrder = OrderList.Where(x => x.stockItemID == stockItemID).Select(x => x).FirstOrDefault();

            // item does not exist
            if (removeItemExistOnOrder == null)
            {
                throw new ArgumentException("Item does not exist on Current Order.");
            }

            //check if the item does not exist on vendor inventory
            removeItemExistOnInventory = InventoryList.Where(x => x.stockItemID == stockItemID).Select(x => x).FirstOrDefault();

            // item exist on current vendor inventory
            if (removeItemExistOnInventory != null)
            {
                throw new ArgumentException("Item does not belongs to this vendor.");
            }
            else
            {
                removeItemExistOnInventory = new itemOnInventoryOrOrder()
                {
                    stockItemID = stockItemID,
                    Description = _context.StockItems.Where(x => x.StockItemID == stockItemID).Select(x => x.Description).FirstOrDefault(),
                    QOH = _context.StockItems.Where(x => x.StockItemID == stockItemID).Select(x => x.QuantityOnHand).FirstOrDefault(),
                    ROL = _context.StockItems.Where(x => x.StockItemID == stockItemID).Select(x => x.ReOrderLevel).FirstOrDefault(),
                    QOO = _context.StockItems.Where(x => x.StockItemID == stockItemID).Select(x => x.QuantityOnOrder).FirstOrDefault(),
                    Buffer = _context.StockItems.Where(x => x.StockItemID == stockItemID).Select(x => x.QuantityOnHand - x.ReOrderLevel).FirstOrDefault(),
                    Price = _context.StockItems.Where(x => x.StockItemID == stockItemID).Select(x => x.PurchasePrice).FirstOrDefault()
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
                _context.SaveChanges();
            }
        }

        public void OrderService_RefreshItemTotalPrice(int itemID, int QTO, decimal price, List<itemOnInventoryOrOrder> OrderList, List<itemOnInventoryOrOrder> InventoryList)
        {
            //local variables
            List<Exception> errorList = new List<Exception>();
            itemOnInventoryOrOrder refreshItemExistOnOrder = null;
            itemOnInventoryOrOrder refreshItem = null;

            // Validate input

            // Quantity must be greater or equal to 1
            //if (QTO < 1)
            //{
            //    throw new ArgumentException("Order Quantity must be greater or equal to 1");
            //}
            //// Price must be greater than 0
            //if (price <= 0)
            //{
            //    throw new ArgumentException("Price must be greater than 0");
            //}

            // Check if the item exist on Current Order
            // check if the item on Current Order
            refreshItemExistOnOrder = OrderList.Where(x => x.stockItemID == itemID).Select(x => x).FirstOrDefault();

            // item does not exist
            if (refreshItemExistOnOrder == null)
            {
                throw new ArgumentException("Item does not exist on Current Order.");
            }
            else
            {
                refreshItem = new itemOnInventoryOrOrder();
                refreshItem.stockItemID = refreshItemExistOnOrder.stockItemID;
                refreshItem.Description = refreshItemExistOnOrder.Description;
                refreshItem.QOH = refreshItemExistOnOrder.QOH;
                refreshItem.ROL = refreshItemExistOnOrder.ROL;
                refreshItem.QOO = refreshItemExistOnOrder.QOO;
                refreshItemExistOnOrder.Price = (decimal)price;
                refreshItemExistOnOrder.QTO = QTO;
                refreshItemExistOnOrder.Total = (decimal)price * QTO;
            }


            OrderList.Remove(refreshItemExistOnOrder);
            OrderList.Add(refreshItem);

            if (errorList.Count() > 0)
            {
                // throw the list of business porcessing error(s)
                throw new AggregateException("unable to add track, Check concerns", errorList);
            }
            else
            {
                // consider data valid
                // has passed business processing rules

                _context.SaveChanges();
            }
        }
        #endregion

        #region Order update / place / delete
        public void SaveCurrentOrder(int loginID, PurchaseOrderInfo purchaseOrder, List<itemOnInventoryOrOrder> currentOrder)
        {
            #region Global Variables
            const decimal GST = 0.05m;

            decimal subtotal = 0.00m,
                    gst = 0.00m;

            List<Exception> errorList = new();
            List<PurchaseOrderDetail> orderItemsToRemove = new();
            #endregion

            #region Parameter Validation
            if (purchaseOrder == null)
            {
                throw new ArgumentNullException("Purchase order is required. ");
            }
            if (purchaseOrder.PurchaseOrderID == 0)
            {
                throw new ArgumentNullException("Purchase Order ID is missing. ");
            }
            if (loginID <= 0)
            {
                throw new ArgumentNullException("Employee ID is required.");
            }
            if (currentOrder == null)
            { 
                throw new ArgumentNullException("List of purchase order items is required. ");
            }
            #endregion

            PurchaseOrder? purchaseOrderExists = _context.PurchaseOrders
                                                         .Where(p => p.PurchaseOrderID == purchaseOrder.PurchaseOrderID)
                                                         .FirstOrDefault();
            if (purchaseOrderExists == null)
            {
                throw new ArgumentException($"Purchase Order does not exist.");
            }

            Employee? employeeExists = _context.Employees
                                              .Where(p => p.EmployeeID == loginID)
                                              .FirstOrDefault();

            if (employeeExists == null)
            {
                throw new ArgumentException($"Employee does not exist.");
            }
            List<PurchaseOrderDetail> purchaseOrderItems = _context.PurchaseOrderDetails
                                                                     .Where(p => p.PurchaseOrderID == purchaseOrder.PurchaseOrderID)
                                                                     .ToList();

            foreach (var purchaseOrderItem in purchaseOrderItems)
            {
                if (!currentOrder.Any(oi => oi.PurchaseOrderDetailID == purchaseOrderItem.PurchaseOrderDetailID))
                {
                    orderItemsToRemove.Add(purchaseOrderItem);
                }
            }

            if (orderItemsToRemove.Count > 0)
            {
                foreach (var orderItem in orderItemsToRemove)
                {
                    _context.PurchaseOrderDetails.Remove(orderItem);
                }
            }

            foreach (var item in currentOrder)
            {
                subtotal += (decimal)item.QTO * item.Price;
                gst += (item.Price * GST) * (decimal)item.QTO;

                PurchaseOrderDetail? orderDetailExists = _context.PurchaseOrderDetails
                                                                 .Where(pod => pod.PurchaseOrderDetailID == item.PurchaseOrderDetailID)
                                                                 .FirstOrDefault();

                if (orderDetailExists == null)
                {
                    PurchaseOrderDetail newOrderDetail = new PurchaseOrderDetail
                    {
                        PurchaseOrderID = (int)purchaseOrder.PurchaseOrderID,
                        StockItemID = item.stockItemID,
                        Quantity = (int)item.QTO,
                        PurchasePrice = item.Price,
                    };
                    purchaseOrderExists.PurchaseOrderDetails.Add(newOrderDetail);
                }
                else
                {
                    orderDetailExists.Quantity = (int)item.QTO;
                    orderDetailExists.PurchasePrice = item.Price;
                    EntityEntry<PurchaseOrderDetail> updatedOrderDetail = _context.Entry(orderDetailExists);
                    updatedOrderDetail.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
            }

            purchaseOrderExists.SubTotal = subtotal;
            purchaseOrderExists.TaxAmount = gst;
            purchaseOrderExists.EmployeeID = loginID;

            EntityEntry<PurchaseOrder> updatedPurchaseOrder = _context.Entry(purchaseOrderExists);
            updatedPurchaseOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            if (errorList.Count > 0)
                throw new AggregateException("Unable to update the current order.", errorList);
            else
                _context.SaveChanges();
        }
        public void SaveNewOrder(int vendorID, int employeeID, List<itemOnInventoryOrOrder> currentOrder)
        {
            #region Global Variables
            const decimal GST = 0.05m;

            List<Exception> errorList = new();
            decimal subtotal = 0.00m,
                    gst = 0.00m;
            #endregion

            #region Parameter Validation
            if (vendorID < 0)
            {
                throw new ArgumentNullException("No Vendor ID was supplied. Please try again.");
            }
            if (employeeID < 0)
            {
                throw new ArgumentNullException("No EmployeeID was supplied. Please try again.");
            }
            if (currentOrder == null)
            {
                throw new ArgumentNullException("No list of purchase order items was supplied. Please try again.");
            }
            #endregion

            Vendor? vendorExists = _context.Vendors.Where(v => v.VendorID == vendorID).FirstOrDefault();

            if (vendorExists == null)
            {
                errorList.Add(new NullReferenceException($"There is no record of Vendor ({vendorID}) in our system."));
            }

            Employee? employeeExists = _context.Employees.Where(e => e.EmployeeID == employeeID).FirstOrDefault();

            if (employeeExists == null)
            {
                errorList.Add(new NullReferenceException($"There is no record of Employee ({employeeID}) in our system."));
            }

            List<PurchaseOrder> purchaseOrders = _context.PurchaseOrders.ToList();

            foreach (var item in currentOrder)
            {
                StockItem itemExists = _context.StockItems.Where(x => x.StockItemID == item.stockItemID).FirstOrDefault();

                if (itemExists == null)
                {
                    errorList.Add(new NullReferenceException($"Item is null."));
                }

                subtotal += item.Price * (decimal)item.QTO;
                gst += (item.Price * GST) * (decimal)item.QTO;
            }

            PurchaseOrder newOrder = new PurchaseOrder();

            newOrder.VendorID = vendorID;
            newOrder.EmployeeID = employeeID;
            newOrder.SubTotal = subtotal;
            newOrder.TaxAmount = gst;

            vendorExists.PurchaseOrders.Add(newOrder);

            foreach (var item in currentOrder)
            {
                PurchaseOrderDetail newOrderDetail = new PurchaseOrderDetail();

                newOrderDetail.StockItemID = item.stockItemID;
                newOrderDetail.Quantity = (int)item.QTO;
                newOrderDetail.PurchasePrice = item.Price;
                newOrderDetail.PurchaseOrderID = newOrder.PurchaseOrderID;

                newOrder.PurchaseOrderDetails.Add(newOrderDetail);
            }

            if (errorList.Count > 0)
                throw new AggregateException("Unable to process your request.", errorList);
            else
                _context.SaveChanges();
        }
        public void PlaceOrder(int loginID, int vendorID,PurchaseOrderInfo purchaseOrder, List<itemOnInventoryOrOrder> currentOrder)
        {
            List<Exception> errorList = new List<Exception>();
            PurchaseOrderDetail orderDetails = null;
            StockItem itemsToUpdate = null;

            if (loginID < 0 || vendorID < 0)
            {
                throw new Exception("Please login and submit vendor");
            }
            PurchaseOrder order = _context.PurchaseOrders.Where(x => x.PurchaseOrderID == purchaseOrder.PurchaseOrderID).FirstOrDefault();
            if( order == null)
            {
                throw new ArgumentException("Current order doex not exist in database, update first");
            }
            foreach (var item in currentOrder)
            {
                orderDetails = _context.PurchaseOrderDetails.Where(x=>x.PurchaseOrderID == purchaseOrder.PurchaseOrderID && x.StockItemID == item.stockItemID).FirstOrDefault();
                // if item does not exist
                if (orderDetails == null)
                {
                    throw new ArgumentException("item does not match database, update first");
                }
                // check if item details match database
                if (orderDetails.Quantity != item.QTO || orderDetails.PurchasePrice != item.Price)
                {
                    throw new ArgumentException("item details does not match database, update first");
                }
                // update QTO for this stock item
                itemsToUpdate = _context.StockItems.Where(x => x.StockItemID == item.stockItemID && x.Vendor.VendorID == vendorID).FirstOrDefault();
                itemsToUpdate.QuantityOnOrder = item.QTO;
                _context.StockItems.Update(itemsToUpdate);
                // Update order date
                order.OrderDate = DateTime.Now;
                _context.PurchaseOrders.Update(order);
                _context.SaveChanges();
            }
        }
        public void DeleteOrder(PurchaseOrderInfo purchaseOrder)
        {
            #region Global Variables
            List<PurchaseOrderDetail> orderDetails = new();
            #endregion

            if (purchaseOrder == null)
                throw new ArgumentNullException("No Purchase Order specified. Please try again.");
            if (purchaseOrder.PurchaseOrderID <= 0)
                throw new ArgumentNullException("No Purchase Order specified. Please try again.");

            PurchaseOrder? purchaseOrderExists = _context.PurchaseOrders
                                                         .Where(p => p.PurchaseOrderID == purchaseOrder.PurchaseOrderID)
                                                         .FirstOrDefault();
            if (purchaseOrderExists == null)
                throw new ArgumentException("The specified Purchase Order does not exist. Please try again.");

            orderDetails = _context.PurchaseOrderDetails
                                   .Where(p => p.PurchaseOrderID == purchaseOrder.PurchaseOrderID)
                                   .ToList();

            foreach (PurchaseOrderDetail detail in orderDetails)
            {
                _context.PurchaseOrderDetails.Remove(detail);
            }

            _context.PurchaseOrders.Remove(purchaseOrderExists);
            _context.SaveChanges();
        }
        #endregion

    }
} 