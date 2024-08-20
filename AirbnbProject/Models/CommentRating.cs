using AirbnbProject.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AirbnbProject.Models
{
    
    public class CommentRating
{
    [Key]
    public int CommentId { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }

    [ForeignKey("CampingSpot")]
    public int SpotId { get; set; }

    public string Comment { get; set; }
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Relationships
    public User User { get; set; }
    
}
}


