using System.Linq;
using System.Text.Json;
using System.Windows;
using client.Controller;

namespace client.Models
{
    public class Material
    {
        public int materialId { get; set; }
        public string material { get; set; }
        public string color { get; set; }
        public string pattern { get; set; }

        

        public Material() { }

        public bool isFull()
        {
            if (material == string.Empty || color == string.Empty) return false;
            return true;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}