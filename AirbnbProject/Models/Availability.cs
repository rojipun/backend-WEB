namespace AirbnbProject.Models
{
    public class Availability
    {
        public int AvailabilityId { get; set; }
        public int SpotId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; } =DateTime.Now;  

        //Relationship 
        //public ICollection <CampingSpot> CampingSpot { get; set; }
    }
}
