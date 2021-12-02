using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Windows;
using client.Controller;

namespace client.Models
{
    public class Product : Material
    {
        public int productId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public double price { get; set; }
        public string type { get; set; }
        public List<Size> sizes { get; set; }

        public Product()
        {
            sizes = new List<Size>();
        }


        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

        public int GetCount()
        {
            return sizes.Sum(s => s.count);
        }

        public bool isFull()
        {
            if (name == string.Empty || price <= 0 || type == string.Empty || !base.isFull()) return false;
            return sizes.Where(s => s.isFull() == false).ToList().Count == 0;
        }
    }
}