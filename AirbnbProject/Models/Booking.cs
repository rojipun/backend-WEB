using AirbnbProject.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AirbnbProject.Models
{
    public class Booking
{
    [Key]
    public int BookingId { get; set; }

    [Required]
    [ForeignKey("User")]
    public int UserId { get; set; }

    [Required]
    [ForeignKey("CampingSpot")]
    public int SpotId { get; set; }

    [Required]
    public DateTime CheckInDate { get; set; }

    [Required]
    public DateTime CheckOutDate { get; set; }

    public string Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Relationships
    public User User { get; set; }
    
}   

}


