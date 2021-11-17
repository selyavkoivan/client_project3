using System.Windows;
using client.Controller;

namespace client.Models
{
    public class User
    {
        public int id { get; set; }
        public string login{ get; set; }
        public string name{ get; set; }
        public string password{ get; set; }

        public User()
        {
            id = 0;
            login = "0";
            name = "0";
            password = "0";
        }

        public User(int id, string login, string name, string password) {
            this.id = id;
            this.login = login;
            this.name = name;
            this.password = password;
        }
        
        public User(string data)
        {
            var dataArray = data.Split(Const.b);
            id = int.Parse(dataArray[0]);
            login = dataArray[1];
            name = dataArray[2];
            password = dataArray[3];
          
        }

        public override string ToString()
        {
            return id + Const.b + login + Const.b + name + Const.b + password;
        }
    }
}