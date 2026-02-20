using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentSpot.Models
{
    public class BayNow
    {
        [Key]
        public int order_i { get; set; }
        public int carts_id { get; set; }
        public int Bay_status_id { get; set; }
      
        public DateTime CreatedAt { get; set; } = DateTime.Now;  // Optional: Add timestamp
        [ForeignKey("carts_id")]
        public Cart Cart { get; set; }

    }
}