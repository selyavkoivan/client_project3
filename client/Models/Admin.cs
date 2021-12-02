using System.Linq;
using System.Text.Json;
using System.Windows;
using client.Controller;

namespace client.Models
{
    public class Admin : User
    {

        public int adminId { get; set; }
        public string position { get; set; }

        public Admin(int id, string position, User user) : base(user)
        {
            this.adminId = id;
            this.position = position;
        }

        public Admin() { }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
        
    }
}