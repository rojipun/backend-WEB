using LiteDB;
using AirbnbProject.Models;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;


namespace AirbnbProject.Data
{
    public class DataBase : IDataContext
    {
        private readonly LiteDatabase _db;
        private readonly ILogger<DataBase> _logger;
        private readonly string _jwtSecret;

        //Constructor to initializes the database and loads the JWT secret from configuration
        public DataBase(ILogger<DataBase> logger, IConfiguration configuration)
        {
            _db = new LiteDatabase("Airbnb.db");
            _logger = logger;
            _jwtSecret = configuration["JwtSecret"]; 
            
            if (string.IsNullOrEmpty(_jwtSecret))
            {
                throw new InvalidOperationException("JwtSecret configuration is missing or empty.");
            }
            
            

            _logger.LogInformation("JwtSecret loaded successfully.");
        }

        // User Methods
        // Register a new user, including password hashing and validation
        public void Register(RegisterRequest model)
        {
            try
            {
                var users = _db.GetCollection<User>("users");

                
                if (model == null)
                    throw new ArgumentNullException(nameof(model), "RegisterRequest model cannot be null");
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password); 
                     var existingUser = users.FindOne(u => u.Username == model.Username);
                    if (existingUser != null)
                        throw new InvalidOperationException("Username already exists");

                // Create a new user object with the hashed password
                var user = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Username = model.Username,
                    Password = hashedPassword,  
                    Roles = "User", // Default role
                   
                };

                // Inserts the new user into the database
                users.Insert(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                throw; 
            }
        }

        // Authenticate a user and generate a JWT token if successful
        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            try
            {
                var users = _db.GetCollection<User>("users");
                var user = users.FindOne(u => u.Username == model.Username);

                if (user == null)
                {
                    _logger.LogWarning("User not found: {Username}", model.Username);
                    return null;
                }

                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    _logger.LogWarning("Invalid password attempt for user: {Username}", model.Username);
                    return null;
                }

                var token = GenerateJwtToken(user);
                return new AuthenticateResponse
                {
                    Token = token,
                    User = user
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication");
                throw; 
            }
        }

        // Generate a JWT token using the user's details
        private string GenerateJwtToken(User user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSecret); 
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.Roles) 
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token");
                throw; 
            }
        }

        // Retrieve all users from the database
        public List<User> GetAllUsers()
        {
            var usersCollection = _db.GetCollection<User>("users");
            return usersCollection.FindAll().ToList();
        }

        // Delete a user by ID
        public void DeleteUser(int userId)
        {
            var usersCollection = _db.GetCollection<User>("users");
            usersCollection.Delete(userId);
        }

        // Retrieve a user by ID
        public User GetUserById(int userId)
        {
            try
            {
                var users = _db.GetCollection<User>("users");
                return users.FindById(userId) ?? throw new KeyNotFoundException("User not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID");
                throw; 
            }
        }

        // Update user details
        public void UpdateUser(int userId, UpdateRequest model)
        {
            try
            {
                var users = _db.GetCollection<User>("users");
                var user = users.FindById(userId);

                if (user != null)
                {
                    if (!string.IsNullOrEmpty(model.Firstname))
                        user.FirstName = model.Firstname;
                    if (!string.IsNullOrEmpty(model.Lastname))
                        user.LastName = model.Lastname;
                    if (!string.IsNullOrEmpty(model.Username))
                        user.Username = model.Username;
                    if (!string.IsNullOrEmpty(model.Password))
                        user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);

                    users.Update(user);
                }
                else
                {
                    throw new KeyNotFoundException("User not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                throw; 
            }
        }

        // Camping Spot Methods
        // Create a new camping spot

        public void CreateCampingSpot(CampingSpot campingSpot)
        {
            try
            {
                if (campingSpot == null)
                    throw new ArgumentNullException(nameof(campingSpot), "CampingSpot cannot be null");

                var collection = _db.GetCollection<CampingSpot>("campingSpots");
                collection.Insert(campingSpot);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating camping spot");
                throw;
            }
        }

        // Retrieve all camping spots
        public List<CampingSpot> GetAllCampingSpots()
        {
            try
            {
                return _db.GetCollection<CampingSpot>("campingSpots").FindAll().ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all camping spots");
                throw;
            }
        }
        // Retrieve a camping spot by ID
        public CampingSpot GetCampingSpotById(int spotId)
        {
            try
            {
                _logger.LogInformation("Attempting to retrieve camping spot with ID: {SpotId}", spotId);

                var collection = _db.GetCollection<CampingSpot>("campingSpots");
                var campingSpot = collection.FindOne(x => x.SpotId == spotId);

                if (campingSpot == null)
                {
                    _logger.LogWarning("Camping spot not found with ID: {SpotId}", spotId);
                    throw new KeyNotFoundException("Camping spot not found");
                }

                return campingSpot;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting camping spot by ID");
                throw;
            }
        }


        //public void UpdateCampingSpot(int spotId, CampingSpot updatedSpot)
        //{
        //    var collection = _db.GetCollection<CampingSpot>("campingSpots");

        //    // Find the existing spot by SpotId
        //    var existingSpot = collection.FindOne(x => x.SpotId == spotId);

        //    if (existingSpot != null)
        //    {
        //        // Update properties
        //        existingSpot.Name = updatedSpot.Name;
        //        existingSpot.Description = updatedSpot.Description;
        //        existingSpot.Location = updatedSpot.Location;
        //        existingSpot.Price = updatedSpot.Price;
        //        existingSpot.Availability = updatedSpot.Availability;
        //        existingSpot.CreatedAt = updatedSpot.CreatedAt; // Optional

        //        // Ensure to handle this properly if it has an issue
        //        if (!collection.Update(existingSpot))
        //        {
        //            throw new Exception("Failed to update camping spot.");
        //        }
        //    }
        //    else
        //    {
        //        throw new KeyNotFoundException($"Camping spot with SpotId {spotId} not found.");
        //    }
        //}




        // Booking Methods

        // Add a new booking and mark the camping spot as unavailable

        public async Task AddBooking(Booking booking)
        {
            try
            {
                if (booking == null)
                    throw new ArgumentNullException(nameof(booking), "Booking cannot be null");

                var spotCollection = _db.GetCollection<CampingSpot>("campingSpots");
                var spot = spotCollection.FindById(booking.SpotId);

                if (spot == null)
                {
                    throw new KeyNotFoundException("Camping spot not found");
                }

                if (!spot.Availability)
                {
                    throw new InvalidOperationException("This camping spot is already booked");
                }

                // Mark the camping spot as unavailable and assign the booking
                spot.Availability = false;
                spot.Booking = booking;

                spotCollection.Update(spot);

                _logger.LogInformation("Booking successfully added for User ID: {UserId}, Spot ID: {SpotId}", booking.UserId, booking.SpotId);
            }
            catch (LiteException ex)
            {
                _logger.LogError(ex, "Database error occurred while adding booking for User ID: {UserId}, Spot ID: {SpotId}", booking.UserId, booking.SpotId);
                throw new InvalidOperationException("A database error occurred while adding the booking. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding booking for User ID: {UserId}, Spot ID: {SpotId}", booking.UserId, booking.SpotId);
                throw;
            }
        }


        // Retrieve all bookings for a specific user
        public List<Booking> GetBookingsByUserId(int userId)
        {
            try
            {
                return _db.GetCollection<Booking>("bookings")
                    .Find(b => b.UserId == userId)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving bookings by user ID.");
                throw;
            }
        }

        // Retrieve a booking by its ID
        public Booking GetBookingById(int id)
        {
            try
            {
                var booking = _db.GetCollection<Booking>("bookings").FindById(id);
                return booking ?? throw new KeyNotFoundException("Booking not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving booking by ID.");
                throw;
            }
        }

        // Update a booking's details
        public void UpdateBooking(int id, Booking model)
        {
            try
            {
                var collection = _db.GetCollection<Booking>("bookings");
                var booking = collection.FindById(id);

                if (booking != null)
                {
                    booking.CheckInDate = model.CheckInDate;
                    booking.CheckOutDate = model.CheckOutDate;
                    booking.Status = model.Status;
                    booking.SpotId = model.SpotId;
                    booking.UserId = model.UserId;
                    collection.Update(booking);
                }
                else
                {
                    throw new KeyNotFoundException("Booking not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating booking.");
                throw;
            }
        }

        // Deletes a booking based on the provided booking ID.
        public void DeleteBooking(int id)
        {
            try
            {
                var collection = _db.GetCollection<Booking>("bookings");
                var deleted = collection.Delete(id);
                if (!deleted)
                {
                    throw new KeyNotFoundException("Booking not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting booking.");
                throw;
            }
        }

        // Retrieves all bookings associated with a specific user ID.
        public IEnumerable<Booking> GetBookingByUser(int userId)
        {
            try
            {
                return _db.GetCollection<Booking>("bookings").Find(e_booking => e_booking.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving bookings for a user.");
                throw;
            }
        }

        // Comment Rating Methods
        // Adds a new comment rating to the database.

        public void AddCommentRating(CommentRating commentRating)
        {
            try
            {
                if (commentRating == null)
                    throw new ArgumentNullException(nameof(commentRating), "CommentRating cannot be null");

                var commentRatingsCollection = _db.GetCollection<CommentRating>("commentRatings");
                commentRatingsCollection.Insert(commentRating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a comment rating.");
                throw;
            }
        }

        // Retrieves all comment ratings associated with a specific spot ID.
        public List<CommentRating> GetCommentsRatingsBySpotId(int spotId)
        {
            try
            {
                return _db.GetCollection<CommentRating>("commentRatings")
                    .Find(c => c.SpotId == spotId)
                    .ToList(); // Find all comment ratings by spot ID and return as a list.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving comment ratings by spot ID.");
                throw;
            }
        }

        // Availability Methods
        // Creates a new availability record in the database.
        public void CreateAvailability(Availability availability)
        {
            try
            {
                if (availability == null)
                    throw new ArgumentNullException(nameof(availability), "Availability cannot be null");

                var availabilitiesCollection = _db.GetCollection<Availability>("availabilities");
                availabilitiesCollection.Insert(availability);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating availability.");
                throw;
            }
        }

        // Retrieves all availability records associated with a specific spot ID.
        public List<Availability> GetAvailabilityBySpotId(int spotId)
        {
            try
            {
                return _db.GetCollection<Availability>("availabilities")
                    .Find(a => a.SpotId == spotId)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving availability by spot ID.");
                throw;
            }
        }

        // Updates an existing availability record by ID with new data from the provided model.
        public void UpdateAvailability(int id, Availability model)
        {
            try
            {
                if (model == null)
                    throw new ArgumentNullException(nameof(model), "Availability model cannot be null");

                var collection = _db.GetCollection<Availability>("availabilities");
                var availability = collection.FindById(id);

                if (availability != null)
                {
                    // Update fields with the provided model values
                    availability.StartDate = model.StartDate;
                    availability.EndDate = model.EndDate;
                    availability.SpotId = model.SpotId;

                    collection.Update(availability);
                }
                else
                {
                    throw new KeyNotFoundException("Availability not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating availability.");
                throw;
            }
        }

        // Deletes an availability record based on the provided availability ID.
        public void DeleteAvailability(int id)
        {
            try
            {
                var collection = _db.GetCollection<Availability>("availabilities");
                var deleted = collection.Delete(id);

                if (!deleted)
                {
                    throw new KeyNotFoundException("Availability not found");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting availability.");
                throw;
            }
        }

    }
}
