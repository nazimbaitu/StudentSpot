using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentSpot.Models
{
    public class Cart
    {
        [Key]
       
        public int cart_id { get; set; }

    
        public int prod_id { get; set; }

        public int customer_id { get; set; }
        public int product_quantity { get; set; }
        //public DateTime order_date { get; set; }
        public int status_id { get; set; }

        [ForeignKey("prod_id")]
        public Product Products { get; set; }

        [ForeignKey("customer_id")]
        public Customer customers { get; set; }

     

    }
}
