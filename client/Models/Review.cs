using System.Text.Json;

namespace client.Models
{
    public class Review
    {
        public int reviewId { get; set; }
        public int rate { get; set; }
        public double averageRate { get; set; }
        public User user { get; set; }
        public Product product { get; set; }

        public Review(int reviewId, int rate, double averageRate, User user, Product product)
        {
            this.reviewId = reviewId;
            this.rate = rate;
            this.averageRate = averageRate;
            this.user = user;
            this.product = product;
        }
        public Review(int rate, User user, Product product)
        {
            this.rate = rate;
            this.user = user;
            this.product = product;
        }
        public Review()
        {
        }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}