using System.Linq;
using System.Windows;
using client.Controller;

namespace client.Models
{
    public class Admin : User
    {

        public int id { get; set; }
        public string position { get; set; }

        public Admin(int id, string position, User user) : base(user.ToString())
        {
            this.id = id;
            this.position = position;
        }

        public Admin(string data) : base(data)
        {
            var dataArray = data.Split(Const.b);
            id = int.Parse(dataArray[4]);
            position = dataArray[5];
        }

        public Admin()
        {
            id = 0;
            position = "0";
        }

        public override string ToString()
        {
            return  base.ToString() + Const.b + id + Const.b + position;
        }
        
    }
}