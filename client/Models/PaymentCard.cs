using System;
using System.Text.Json;

namespace client.Models
{
    public class PaymentCard
    {
        public int PaymentCardId { get; set; }
        public string CardNumber { get; set; }
        public int CVV { get; set; }
        public DateTime expityDate { get; set; }

        public PaymentCard(int paymentCardId, string cardNumber, int cvv, DateTime expityDate)
        {
            PaymentCardId = paymentCardId;
            CardNumber = cardNumber;
            CVV = cvv;
            this.expityDate = expityDate;
        }
        public PaymentCard(string cardNumber, int cvv, DateTime expityDate)
        {
          
            CardNumber = cardNumber;
            CVV = cvv;
            this.expityDate = expityDate;
        }

        public PaymentCard()
        {
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
