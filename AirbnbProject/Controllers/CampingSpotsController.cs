using Microsoft.AspNetCore.Mvc;
using AirbnbProject.Data;
using AirbnbProject.Models;
using Microsoft.AspNetCore.Authorization;


namespace AirbnbProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampingSpotController : ControllerBase
    {
        private readonly IDataContext _dataContext;
        private readonly ILogger<CampingSpotController> _logger; // Fixed logger type to match the class name

        public CampingSpotController(IDataContext data, ILogger<CampingSpotController> logger)
        {
            _dataContext = data;
            _logger = logger;
        }

        // GET: api/CampingSpot
        // Retrieves all camping spots.
        [HttpGet]
        public ActionResult<List<CampingSpot>> GetAllCampingSpots()
        {
            var spots = _dataContext.GetAllCampingSpots();
            return Ok(spots);
        }

        // GET: api/CampingSpot/5
        // Retrieves a specific camping spot by its ID.
        [HttpGet("{id}")]
        public ActionResult<CampingSpot> GetCampingSpotById(int id)
        {
            var campingSpot = _dataContext.GetCampingSpotById(id);
            if (campingSpot == null)
            {
                return NotFound("Camping spot not found.");
            }
            return Ok(campingSpot);
        }

        // POST: api/CampingSpot
        // Creates a new camping spot. Requires admin role.
        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult CreateCampingSpot([FromBody] CampingSpot model)
        {
            if (model == null)
            {
                return BadRequest("Invalid camping spot data.");
            }

            _dataContext.CreateCampingSpot(model);
            return Ok("Camping spot successfully created!");
        }

        // PUT: api/CampingSpot/5
        // Uncomment to enable updating a camping spot. Requires admin role.
        // [HttpPut("{id}")]
        // [Authorize(Roles = "admin")]
        // public ActionResult UpdateCampingSpot(int id, [FromBody] CampingSpot model)
        // {
        //     if (model == null)
        //     {
        //         return BadRequest("Invalid camping spot data.");
        //     }

        //     _dataContext.UpdateCampingSpot(id, model);
        //     return Ok("Camping spot successfully updated!");
        // }
    }
}
