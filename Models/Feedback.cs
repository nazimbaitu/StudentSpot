using System.ComponentModel.DataAnnotations;

namespace StudentSpot.Models
{
    public class Feedback
    {
       
            [Key]
            public int id { get; set; }
            public string name { get; set; }
            public string message { get; set; }

        
    }
}
