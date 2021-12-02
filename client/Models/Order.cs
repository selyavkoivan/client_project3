using System;
using System.Text.Json;

namespace client.Models
{
    public class Order
    {
        public int orderId { get; set; }
        public User user { get; set; }
        public Product product { get; set; }
        public int count { get; set; }
        public DateTime date { get; set; }
        public bool delivery { get; set; }
        public string deliveryAddress { get; set; }
        
        public Order(int orderId, User user, Product product, int count, DateTime date, bool delivery, String deliveryAddress) {
            this.orderId = orderId;
            this.user = user;
            this.product = product;
            this.count = count;
            this.date = date;
            this.delivery = delivery;
            this.deliveryAddress = deliveryAddress;
        }
        public Order(User user, Product product, int count, DateTime date, String deliveryAddress) {
            this.user = user;
            this.product = product;
            this.count = count;
            this.date = date;
            delivery = true;
            this.deliveryAddress = deliveryAddress;
        }
        public Order(User user, Product product, int count, DateTime date) {
            this.user = user;
            this.product = product;
            this.count = count;
            this.date = date;
            delivery = false;
        }
        public Order()
        {
            user = new User();
            product = new Product();
        }
        
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}