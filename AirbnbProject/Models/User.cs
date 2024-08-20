using LiteDB;
using System.Text.Json.Serialization;

namespace AirbnbProject.Models
{
    public class User
    {
        [BsonId]
        public int Id { get; set; } 
        public string Username { get; set; }
        
      
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [JsonIgnore]
        public string? Password { get; set; }

        public string Roles { get; set; } = "User"; //default

  
        

        ////define the reationship between two tables 
        //public ICollection<CampingSpot> CampingSpots { get; set; }
        //public ICollection<Booking> Bookings { get; set; }
        //public ICollection<CommentRating> CommentRatings { get; set; }
        //public string ConfirmPassword { get; internal set; }
    }
}
