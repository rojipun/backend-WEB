using AirbnbProject.Models;
using LiteDB;


namespace AirbnbProject.Data
{
    public interface IDataContext
    {
        // User methods
        void Register(RegisterRequest model); // Register a new user
        AuthenticateResponse Authenticate(AuthenticateRequest model); // Authenticate user and return JWT token
        void DeleteUser(int userId); // Delete a user by their username
        User GetUserById(int userId); // Retrieve a user by their ID
        void UpdateUser(int userId, UpdateRequest model); // Update an existing user
        List<User> GetAllUsers();


        // Camping Spot methods
        List<CampingSpot> GetAllCampingSpots();
        CampingSpot GetCampingSpotById(int id);
        void CreateCampingSpot(CampingSpot campingSpot);

        //void UpdateCampingSpot(int id, CampingSpot model);
        

        // Booking methods
        Task AddBooking(Booking booking); // Add a new booking
        List<Booking> GetBookingsByUserId(int userId); // Get bookings for a specific user
        Booking GetBookingById(int id); // Get a booking by its ID
        void UpdateBooking(int id, Booking model); // Update an existing booking
        void DeleteBooking(int id); // Delete a booking by its ID
        IEnumerable<Booking> GetBookingByUser(int userId);


        // CommentRating methods
        void AddCommentRating(CommentRating commentRating); // Add a comment or rating for a camping spot
        List<CommentRating> GetCommentsRatingsBySpotId(int spotId); // Get comments and ratings for a camping spot


        // Availability methods
        void CreateAvailability(Availability availability); // Create availability for a camping spot
        List<Availability> GetAvailabilityBySpotId(int spotId); // Get availability for a camping spot
        void UpdateAvailability(int id, Availability model); // Update availability
        void DeleteAvailability(int id); // Delete availability
    }
}
