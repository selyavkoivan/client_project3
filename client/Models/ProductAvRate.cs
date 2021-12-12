using System;
using System.Collections.Generic;
using System.Linq;

namespace client.Models
{
    public class ProductAvRate
    {
        public CalculatedProduct product { get; set; }
        public List<Review> reviews { get; set; } 
        public  double averageRate { get; set; }

        public ProductAvRate(CalculatedProduct product, List<Review> reviews)
        {
            this.product = product;
            if (reviews != null)
            {
                this.reviews = reviews;
                averageRate = Math.Round(reviews.Average(r => r.averageRate) * 100.0) / 100.0;
            }
            else
            {
                this.reviews = null;
                averageRate = 0;
            }
        }
        public ProductAvRate(Product product, List<Review> reviews)
        {
            this.product = new CalculatedProduct();
            if (reviews != null)
            {
                this.reviews = reviews;
                averageRate = Math.Round(reviews.Average(r => r.averageRate) * 100.0) / 100.0;
            }
            else
            {
                this.reviews = null;
                averageRate = 0;
            }
        }
    }
}