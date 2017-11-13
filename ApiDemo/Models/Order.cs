using System;
using System.Collections.Generic;

namespace ApiDemo.Models
{
    public class Order
    {
        public Guid ID { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ShippingAndHandling { get; set; }
        public decimal Total { get; set; }
    }

    public class OrderItem
    {
        public string ProductID { get; set; }
		
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal LineTotal { get; set; }
    }
}