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
        public PaymentCard card { get; set; }
        public bool status { get; set; }

        public User() { }

        public User(int id, string login, string name, string password, bool status) {
            this.userId = id;
            this.login = login;
            this.name = name;
            this.password = password;
            this.status = status;
        }
        public User(int id, string login, string name, string password, PaymentCard card) {
            this.userId = id;
            this.login = login;
            this.name = name;
            this.password = password;
            this.card = card;
        }
        
        public User(User user)
        {
            userId = user.userId;
            login = user.login;
            name = user.name;
            password = user.password;
            status = user.status;

        }
     

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}