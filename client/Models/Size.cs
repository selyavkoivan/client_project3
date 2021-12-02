using System.Windows;
using client.Controller;
using System.Text.Json;

namespace client.Models
{
    public class Size
    {
        public int sizeId { get; set; }
        public string size { get; set; }
        public int count { get; set; }

        public Size() { }
        
        public Size(int id, string size, int count)
        {
            this.sizeId = id;
            this.size = size;
            this.count = count;
        }

        public bool isFull()
        {
            if (size == string.Empty || count <= 0) return false;
            return true;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);;
        }
    }
}