using Microsoft.AspNetCore.Mvc;
using AirbnbProject.Data;
using AirbnbProject.Models;
using Microsoft.AspNetCore.Authorization;

namespace AirbnbProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        private readonly IDataContext _dataContext;

        public AvailabilityController(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        // POST: api/Availability
        // Creates a new availability record. Accessible only by users with the "AdminPolicy" policy.
        [HttpPost]
        [Authorize(Policy = "AdminPolicy")]
        public ActionResult CreateAvailability([FromBody] Availability model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            _dataContext.CreateAvailability(model);
            return Ok("Availability created!");
        }

        // GET: api/Availability/spot/{spotId}
        // Retrieves all availability records for a specific camping spot.
        [HttpGet("spot/{spotId}")]
        public ActionResult<List<Availability>> GetAvailabilityBySpotId(int spotId)
        {
            var availabilities = _dataContext.GetAvailabilityBySpotId(spotId);
            if (availabilities == null || availabilities.Count == 0)
            {
                return NotFound("No availability found for the specified spot.");
            }

            return Ok(availabilities);
        }

        // PUT: api/Availability/{id}
        // Updates an existing availability record. Accessible only by users with the "AdminPolicy" policy.
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public ActionResult UpdateAvailability(int id, [FromBody] Availability model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            _dataContext.UpdateAvailability(id, model);
            return Ok("Availability updated!");
        }

        // DELETE: api/Availability/{id}
        // Deletes an existing availability record. Accessible only by users with the "AdminPolicy" policy.
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public ActionResult DeleteAvailability(int id)
        {
            _dataContext.DeleteAvailability(id);
            return Ok("Availability deleted!");
        }
    }
}
