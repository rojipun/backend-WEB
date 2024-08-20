
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LiteDB;

namespace AirbnbProject.Models
{
    public class CampingSpot
{
        
    public  int SpotId { get; set; }

    

    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    [Required]
    public string Location { get; set; }

    [Required]
    public decimal Price { get; set; }

    public bool Availability { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Relationships

        //with Nullable Collection
    public Booking? Booking { get; set; }
    public List<CommentRating>  CommentRatings { get; set; } = new List<CommentRating>(); // Empty list initialize
}

}



