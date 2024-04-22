using Microsoft.AspNetCore.Mvc;
using Syncfusion.EJ2.Base;
using WebMethodAdaptor.Server.Models;

namespace WebMethodAdaptor.Server.Controllers
{
    public class GridController : Controller
    {
        // method to retrieve data
        [HttpGet]
        [Route("api/[controller]")]
        public List<OrdersDetails> GetOrderData()
        {
            // Retrieve all records and convert to list
            var data = OrdersDetails.GetAllRecords().ToList();
            return data;
        }

        // POST method to handle incoming data manager requests
        [HttpPost]
        [Route("api/[controller]")]
        public object Post([FromBody] DataManager DataManagerRequest)
        {
            // Retrieve data source and convert to queryable
            IQueryable<OrdersDetails> DataSource = GetOrderData().AsQueryable();

            // Initialize QueryableOperation
            QueryableOperation queryableOperation = new QueryableOperation();

            // Retrieve data manager value
            DataManagerRequest dataManagerParams = DataManagerRequest.Value;


            // Perform filtering operation if filtering is provided
            if (dataManagerParams.Where != null && dataManagerParams.Where.Count > 0)
            {
                DataSource = queryableOperation.PerformFiltering(DataSource, dataManagerParams.Where, dataManagerParams.Where[0].Operator);
            }

            // Perform search operation if search is provided
            if (dataManagerParams.Search != null && dataManagerParams.Search.Count > 0)
            {
                DataSource = queryableOperation.PerformSearching(DataSource, dataManagerParams.Search);
            }

            // Perform sorting operation if sorting is provided
            if (dataManagerParams.Sorted != null && dataManagerParams.Sorted.Count > 0)
            {
                DataSource = queryableOperation.PerformSorting(DataSource, dataManagerParams.Sorted);
            }

            // Get total record count after applying filters
            int totalRecordsCount = DataSource.Count();

            // Perform skip operation if skip value is provided
            if (dataManagerParams.Skip != 0)
            {
                DataSource = queryableOperation.PerformSkip(DataSource, dataManagerParams.Skip);
            }

            // Perform take operation if take value is provided
            if (dataManagerParams.Take != 0)
            {
                DataSource = queryableOperation.PerformTake(DataSource, dataManagerParams.Take);
            }

            // Return result and total record count
            return new { result = DataSource, count = totalRecordsCount };
        }

        // POST method to handle record insertion
        [HttpPost]
        [Route("api/[controller]/Insert")]
        public void Insert([FromBody] CRUDModel<OrdersDetails> newRecord)
        {
            // Check if new record is not null
            if (newRecord.value != null)
            {
                // Insert new record
                OrdersDetails.GetAllRecords().Insert(0, newRecord.value);
            }
        }

        // POST method to handle record updates
        [HttpPost]
        [Route("api/[controller]/Update")]
        public void Update([FromBody] CRUDModel<OrdersDetails> updatedRecord)
        {
            // Retrieve updated order
            var updatedOrder = updatedRecord.value;
            if (updatedOrder != null)
            {
                // Find existing record
                var data = OrdersDetails.GetAllRecords().FirstOrDefault(or => or.OrderID == updatedOrder.OrderID);
                if (data != null)
                {
                    // Update existing record
                    data.OrderID = updatedOrder.OrderID;
                    data.CustomerID = updatedOrder.CustomerID;
                    data.ShipCity = updatedOrder.ShipCity;
                    data.ShipCountry = updatedOrder.ShipCountry;
                    // Update other properties similarly
                }
            }
        }

        // POST method to handle record removal
        [HttpPost]
        [Route("api/[controller]/Remove")]
        public void Remove([FromBody] int key)
        {
            // Retrieve order ID from deleted record
           // int orderId = int.Parse(deletedRecord.key.ToString());
            // Find record to delete
            var data = OrdersDetails.GetAllRecords().FirstOrDefault(orderData => orderData.OrderID == key);
            if (data != null)
            {
                // Remove record from data collection
                OrdersDetails.GetAllRecords().Remove(data);
            }
        }

        [HttpPost]
        [Route("api/[controller]/CrudUpdate")]
        public void CrudUpdate([FromBody] CRUDModel<OrdersDetails> request)
        {
            // perform update operation
            if (request.action == "update")
            {
                var orderValue = request.value;
                OrdersDetails existingRecord = OrdersDetails.GetAllRecords().Where(or => or.OrderID == orderValue.OrderID).FirstOrDefault();
                existingRecord.OrderID = orderValue.OrderID;
                existingRecord.CustomerID = orderValue.CustomerID;
                existingRecord.ShipCity = orderValue.ShipCity;
            }
            // perform insert operation
            else if (request.action == "insert")
            {
                OrdersDetails.GetAllRecords().Insert(0, request.value);
            }
            // perform remove operation
            else if (request.action == "remove")
            {
                OrdersDetails.GetAllRecords().Remove(OrdersDetails.GetAllRecords().Where(or => or.OrderID == int.Parse(request.key.ToString())).FirstOrDefault());
            }
        }
        [HttpPost]
        [Route("api/[controller]/BatchUpdate")]
        public IActionResult BatchUpdate([FromBody] CRUDModel<OrdersDetails> batchOperation)
        {
            if (batchOperation.added != null)
            {
                foreach (var addedOrder in batchOperation.added)
                {
                    OrdersDetails.GetAllRecords().Insert(0, addedOrder);
                }
            }
            if (batchOperation.changed != null)
            {
                foreach (var changedOrder in batchOperation.changed)
                {
                    var existingOrder = OrdersDetails.GetAllRecords().FirstOrDefault(or => or.OrderID == changedOrder.OrderID);
                    if (existingOrder != null)
                    {
                        existingOrder.CustomerID = changedOrder.CustomerID;
                        existingOrder.ShipCity = changedOrder.ShipCity;
                        // Update other properties as needed
                    }
                }
            }
            if (batchOperation.deleted != null)
            {
                foreach (var deletedOrder in batchOperation.deleted)
                {
                    var orderToDelete = OrdersDetails.GetAllRecords().FirstOrDefault(or => or.OrderID == deletedOrder.OrderID);
                    if (orderToDelete != null)
                    {
                        OrdersDetails.GetAllRecords().Remove(orderToDelete);
                    }
                }
            }
            return Json(batchOperation);
        }
        // Model for handling data manager requests
        public class DataManager
        {
            public required DataManagerRequest Value { get; set; }
        }

        // Model for handling CRUD operations
        public class CRUDModel<T> where T : class
        {
            public string? action { get; set; }
            public string? keyColumn { get; set; }
            public object? key { get; set; }
            public T? value { get; set; }
            public List<T>? added { get; set; }
            public List<T>? changed { get; set; }
            public List<T>? deleted { get; set; }
            public IDictionary<string, object>? @params { get; set; }
        }

    }
}