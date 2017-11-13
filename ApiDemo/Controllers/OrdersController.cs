using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApiDemo.Data;
using ApiDemo.Models;
using Newtonsoft.Json;

namespace ApiDemo.Controllers
{
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        // GET api/orders/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            return Ok(await OrderRepository.Current.GetById(id));
        }


        // GET api/orders/{id}/items/{productId}
        [HttpGet("{id}/items/{productId}")]
        public async Task<IActionResult> GetItem(Guid id, string productId)
        {
            var order = await OrderRepository.Current.GetById(id);
            if (order == null) return NotFound("Order Not Found");
            var item = order.Items.FirstOrDefault(i => i.ProductID == productId);
            if (item == null) return NotFound("Item Not Found on Order");
            return Ok(item);
        }

        // POST api/orders
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var newOrder = await OrderRepository.Current.CreateNew();
            var newUri = Url.Action("Get", new { id = newOrder.ID });
            return Created(newUri, newOrder);
        }

        // POST api/orders/{id}/items
        [HttpPost("{id}/items")]
        public async Task<IActionResult> PostItem(Guid id, OrderItemInput item)
        {
            // Validate Input
            var order = await OrderRepository.Current.GetById(id);
            if (order == null)
                order = await OrderRepository.Current.CreateNew();
            if (item == null || string.IsNullOrWhiteSpace(item.ProductID)) return BadRequest("Item Not Valid");
            if (order.Items.Any(i => i.ProductID == item.ProductID))
                return BadRequest($"Product {item.ProductID} is already on this order. Use PATCH to change quantity.");

            // Add Item, Recalculate, and Save
            order.Items.Add(new OrderItem
            {
                ProductID = item.ProductID,
                Quantity = item.Quantity
            });
            await recalculateOrder(order);
            await OrderRepository.Current.Save(order);

            // Result For Item Created
            var newUri = Url.Action("GetItem", new { id = id, productId = item.ProductID });
            return Created(newUri, order);
        }

        // PATCH api/orders/{id}/items/{productId}
        [HttpPatch("{id}/items/{productId}")]
        public async Task<IActionResult> PatchItem(Guid id, string productId, [FromBody]List<JsonPatch<int>> patches)
        {
            // Validate Input
            var order = await OrderRepository.Current.GetById(id);
            if (order == null)
                return NotFound("Order Not Found");
            var item = order.Items.FirstOrDefault(i => i.ProductID == productId);
            if (item == null) return NotFound("Item Not Found on Order");

            // Patches
            foreach (var patch in patches)
            {
                switch (patch.Path)
                {
                    case "/quantity":
                        item.Quantity = patch.Value;
                        break;
                    default: break; // not recognized - skip
                }
            }

            //  Recalculate and Save
            await recalculateOrder(order);
            await OrderRepository.Current.Save(order);

            // Result For Updated Item
            return Ok(order);
        }

        // DELETE api/orders/{id}/items/{productId}
        [HttpDelete("{id}/items/{productId}")]
        public async Task<IActionResult> DeleteItem(Guid id, string productId)
        {
            // Validate Input
            var order = await OrderRepository.Current.GetById(id);
            if (order == null)
                return NotFound("Order Not Found");
            var item = order.Items.FirstOrDefault(i => i.ProductID == productId);
            if (item == null) return NotFound("Item Not Found on Order");

            // Remove Item, Recalculate and Save
			order.Items.Remove(item);
            await recalculateOrder(order);
            await OrderRepository.Current.Save(order);

            // Result For Removed Item
            return Ok(order);
        }



        private async Task recalculateOrder(Order order)
        {
            order.Subtotal = 0;
            var totalQuantity = 0;
            foreach (var item in order.Items.ToList())
            {
                var product = await ProductRepository.Current.GetById(item.ProductID);
                if (product == null)
                    order.Items.Remove(item);
                else
                {
                    totalQuantity += item.Quantity;
                    item.Description = product.Description;
                    item.UnitCost = product.Price;
                    item.LineTotal = item.Quantity * item.UnitCost;
                    order.Subtotal += item.LineTotal;
                }
            }
            order.Tax = order.Subtotal * 0.07m;
            order.ShippingAndHandling = totalQuantity > 10
                ? 10m
                : totalQuantity > 0
                    ? 5m
                    : 0m;
            order.Total = order.Subtotal + order.Tax + order.ShippingAndHandling;
        }
    }

    public class OrderItemInput
    {
        public string ProductID { get; set; }
        public int Quantity { get; set; }
    }

    public class JsonPatch<T>
    {
        [JsonProperty("op")]
        public string Operation { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("value")]
        public T Value { get; set; }
    }
}
