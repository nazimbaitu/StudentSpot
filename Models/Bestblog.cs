using System.ComponentModel.DataAnnotations;

namespace StudentSpot.Models
{
    public class Bestblog
    {
        [Key]
        public int blog_id { get; set; }

        [Required]
        public string blog_name { get; set; }

        [Required]
        public string blog_description { get; set; }

        public string? blog_image { get; set; }
    }
}
