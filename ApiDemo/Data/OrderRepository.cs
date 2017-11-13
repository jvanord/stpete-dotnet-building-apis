using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDemo.Models;

namespace ApiDemo.Data
{
    public class OrderRepository
    {
        private static OrderRepository _singleton;
        private static List<Order> _allOrders = new List<Order>();

        private OrderRepository() { }

        public static OrderRepository Current
        {
            get
            {
                if (_singleton == null) _singleton = new OrderRepository();
                return _singleton;
            }
        }

        public async Task<Order> CreateNew()
        {
            var order = new Order { ID = Guid.NewGuid() };
            _allOrders.Add(order);
            return order;
        }

        public async Task<Order> GetById(Guid id) => _allOrders?.FirstOrDefault(o => o.ID == id);

        public async Task Save(Order order)
        {
            var match = _allOrders?.FirstOrDefault(o => o.ID == order.ID);
            if (match == null)
                _allOrders.Add(order);
            else
                match = order;
        }
    }
}