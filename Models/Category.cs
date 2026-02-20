using System.ComponentModel.DataAnnotations;

namespace StudentSpot.Models
{
    public class Category
    {
        [Key]
        public int category_id { get; set; }
        public string category_name { get; set; }
        public List<Product> product { get; set; }
    }
}
