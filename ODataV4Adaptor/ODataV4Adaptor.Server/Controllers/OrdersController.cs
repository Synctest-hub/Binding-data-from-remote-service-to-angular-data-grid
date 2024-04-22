using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ODataV4Adaptor.Server.Models;

namespace OdataV4Adaptor.Server.Controllers
{

    public class OrdersController : ODataController
    {
        /// <summary>
        /// Retrieves all orders.
        /// </summary>
        /// <returns>The collection of orders.</returns>
        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            var data = OrdersDetails.GetAllRecords().AsQueryable();
            return Ok(data);
        }

        /// <summary>
        /// Creates a new order.
        /// </summary>
        /// <param name="order">The order to be created.</param>
        /// <returns>The newly created order.</returns>
        [HttpPost]
        [EnableQuery]
        public IActionResult Post([FromBody] OrdersDetails order)
        {
            if (order == null)
            {
                return BadRequest("Null order");
            }

            OrdersDetails.GetAllRecords().Insert(0, order);
            return Ok(order);
        }

        /// <summary>
        /// Updates an existing order.
        /// </summary>
        /// <param name="key">The key of the order to be updated.</param>
        /// <param name="updatedOrder">The updated order details.</param>
        /// <returns>The updated order.</returns>
        [HttpPatch("{key}")]
        public IActionResult Patch(int key, [FromBody] OrdersDetails updatedOrder)
        {
            if (updatedOrder == null)
            {
                return BadRequest("No records");
            }
            var existingOrder = OrdersDetails.GetAllRecords().FirstOrDefault(o => o.OrderID == key);
            if (existingOrder != null)
            {
                // If the order exists, update its properties
                existingOrder.CustomerID = updatedOrder.CustomerID ?? existingOrder.CustomerID;
                existingOrder.EmployeeID = updatedOrder.EmployeeID ?? existingOrder.EmployeeID;
                existingOrder.ShipCountry = updatedOrder.ShipCountry ?? existingOrder.ShipCountry;
            }
            return Ok(existingOrder);
        }

        /// <summary>
        /// Deletes an order.
        /// </summary>
        /// <param name="key">The key of the order to be deleted.</param>
        /// <returns>The deleted order.</returns>
        [HttpDelete("{key}")]
        public IActionResult Delete(int key)
        {
            var order = OrdersDetails.GetAllRecords().FirstOrDefault(o => o.OrderID == key);
            if (order != null)
            {
                OrdersDetails.GetAllRecords().Remove(order);
            }
            return Ok(order);
        }
    }
}