using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentSpot.Models
{
    public class Product
    {
        [Key] 
        public int product_id { get; set; }
        public string product_name { get; set; }
        public string product_image { get; set; }
        public string product_price { get; set; }
        public string product_description { get; set; }
     
        public int cat_id { get; set; }
        public Category Category { get; set; }
    

    }
}
