using Microsoft.AspNetCore.Mvc;
using AirbnbProject.Data;
using AirbnbProject.Models;
using Microsoft.AspNetCore.Authorization;



namespace AirbnbProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IDataContext _dataContext;

        public BookingController(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        // POST: api/Booking/book
        // Creates a new booking for a camping spot.

        [HttpPost("book")]
        public IActionResult BookCampingSpot([FromBody] Booking booking)
        {
            try
            {
                // Validate the booking request
                if (booking == null)
                {
                    return BadRequest("Booking cannot be null");
                }

                _dataContext.AddBooking(booking);

                return Ok("Booking successful");
            }
            catch (InvalidOperationException ex)
            {
                // Handle the case where the spot is already booked
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "An error occurred while booking the camping spot");
            }
        }

        // GET: api/Booking/user/{userId}
        // Retrieves all bookings made by a specific user.
        [HttpGet("user/{userId}")]
        [Authorize]
        public ActionResult<List<Booking>> GetBookingsByUserId(int userId)
        {
            var bookings = _dataContext.GetBookingsByUserId(userId);
            if (bookings == null || bookings.Count == 0)
            {
                return NotFound("No bookings found for the specified user.");
            }

            return Ok(bookings);
        }

        // GET: api/Booking/{id}
        // Retrieves a specific booking by its ID.
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<Booking> GetBookingById(int id)
        {
            var booking = _dataContext.GetBookingById(id);
            if (booking == null)
            {
                return NotFound("Booking not found.");
            }

            return Ok(booking);
        }

        // PUT: api/Booking/{id}
        // Updates an existing booking. Accessible only by users with the "AdminPolicy" policy.
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public ActionResult UpdateBooking(int id, [FromBody] Booking model)
        {
            if (model == null)
            {
                return BadRequest("Invalid booking data.");
            }

            _dataContext.UpdateBooking(id, model);
            return Ok("Booking successfully updated!");
        }

        // DELETE: api/Booking/{id}
        // Deletes a specific booking by its ID. Accessible only by users with the "AdminPolicy" policy.
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public ActionResult DeleteBooking(int id)
        {
            _dataContext.DeleteBooking(id);
            return Ok("Booking successfully deleted!");
        }
    }
}
