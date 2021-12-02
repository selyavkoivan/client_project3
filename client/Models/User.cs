using System.Text.Json;
using System.Windows;
using client.Controller;

namespace client.Models
{
    public class User
    {
        public int userId { get; set; }
        public string login{ get; set; }
        public string name{ get; set; }
        public string password{ get; set; }

        public User() { }

        public User(int id, string login, string name, string password) {
            this.userId = id;
            this.login = login;
            this.name = name;
            this.password = password;
        }
        
        public User(User user)
        {
            userId = user.userId;
            login = user.login;
            name = user.name;
            password = user.password;
          
        }
     

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}