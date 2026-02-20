using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentSpot.Models
{
    [Table("tbl_wishlist")]
    public class Wishlist
    {
        [Key]
        public int wishlist_id { get; set; }

        [ForeignKey("Customer")]
        public int customer_id { get; set; }

        [ForeignKey("Product")]
        public int product_id { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Product Product { get; set; }
    }
}
