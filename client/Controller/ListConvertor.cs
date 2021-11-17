using System.Collections.Generic;
using System.Linq;
using System.Windows;
using client.Models;

namespace client.Controller
{
    public class ListConvertor
    {
        public static IEnumerable<User> GetUsers(string users)
        {
            return users.Split(Const.w).Select(u => new User(u)).ToList();
        }
    }
}