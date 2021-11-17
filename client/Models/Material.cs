using System.Linq;
using System.Windows;
using client.Controller;

namespace client.Models
{
    public class Material
    {
        public int id { get; set; }
        public string material { get; set; }
        public string color { get; set; }
        public string pattern { get; set; }


        public Material(string data)
        {
            var dataArray = data.Split(Const.b);
            id = int.Parse(dataArray[0]);
            material = dataArray[1];
            color = dataArray[2];
            pattern = dataArray[3];
        }

        public Material()
        {
            id = 0;
            material = "0";
            color = "0";
            pattern = "0";
        }


        public override string ToString()
        {
            return id + Const.b + material + Const.b + color + Const.b + pattern;
        }
    }
}