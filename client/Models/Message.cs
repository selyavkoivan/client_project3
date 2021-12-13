using System;
using System.Text.Json;

namespace client.Models
{
    public class Message
    {
        public int chatId { get; set; }
        public string date { get; set; }
        public bool type { get; set; }
        public Order order { get; set; }
        public string message { get; set; }

        public Message(DateTime date, bool type, Order order, string message)
        {
            this.date = date.ToString("yyyy-MM-dd HH:mm:ss");
            this.type = type;
            this.order = order;
            this.message = message;
        }
        public Message() { }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);  
        }
    }
}