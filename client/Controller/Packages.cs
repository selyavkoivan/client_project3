using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace client.Controller
{
    public static class StringChecker
    {
        public static string Check(this string str)
        {
            if (str.Last() == Const.w || str.Last() == Const.b)
            {
                return str.Substring(0, str.Length - 1);
            }

            return str;
        }
    }
    public static class Packages
    {
      
        public static string Recv(NetworkStream stream)
        {
            
            var sizeBuffer = new byte[4];
            stream.Read(sizeBuffer, 0, 4);
            
            var size = BitConverter.ToInt32(sizeBuffer, 0);
          
            var messageBuffer = new byte[size];
            stream.Read(messageBuffer, 0, size);
           
            return Encoding.UTF8.GetString(messageBuffer).Check();
        }
        
        public static void Send(NetworkStream stream, string message)
        {
            var messageBuffer = Encoding.UTF8.GetBytes(message);
            var sizeBuffer = BitConverter.GetBytes(Encoding.UTF8.GetByteCount(message));
            stream.Write(sizeBuffer, 0, sizeBuffer.Length);
            stream.Write(messageBuffer, 0, messageBuffer.Length);
            
        }
    }
}