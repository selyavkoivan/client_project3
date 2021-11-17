using System;
using client.Controller;

namespace client.Models
{
    public class Product : Material
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public double price { get; set; }
        public int count { get; set; }
        public string size { get; set; }


        public Product(string data) : base(data)
        {
            var dataArray = data.Split(Const.b);
            id = int.Parse(dataArray[4]);
            name = dataArray[5];
            description = dataArray[6];
            price = double.Parse(dataArray[7]);
            count = int.Parse(dataArray[8]);
            size = dataArray[9];
        }

        

        public override string ToString()
        {
            return base.ToString() + Const.b + id + Const.b + name +
                   Const.b + description + Const.b + price + Const.b + count + Const.b + size;
        }
    }
}