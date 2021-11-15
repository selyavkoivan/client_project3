using System.Linq;
using System.Windows;
using client.Controller;

namespace client.Models
{
    public class Admin
    {
       
        public int id { get; set; }
        public string position { get; set; }
        public User user { get; set; }

        public Admin(int id, string position, User user)
        {
            this.id = id;
            this.position = position;
            this.user = user;
        }

        public Admin(string data)
        {
            var dataArray = data.Split(Const.b);
            id = int.Parse(dataArray[0]);
            position = dataArray[1];
            data = "";
            for (int i = 2; i < dataArray.Length; i++)
            {
                data += dataArray[i] + Const.b;
            }

            user = new User(data);

        }

        public Admin()
        {
            id = 0;
            position = "0";
            user = new User();
        }

        public override string ToString()
        {
            return id.ToString() + Const.b + position + Const.b + user;
           
            
        }
        
    }
}