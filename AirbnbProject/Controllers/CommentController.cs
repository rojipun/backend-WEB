using Microsoft.AspNetCore.Mvc;
using AirbnbProject.Data;
using AirbnbProject.Models;

namespace AirbnbProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentRatingController : ControllerBase
    {
        private readonly IDataContext _dataContext;

        public CommentRatingController(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        // GET: api/CommentRating/spot/5
        // Retrieves all comments and ratings for a specific camping spot.
        [HttpGet("spot/{spotId}")]
        public ActionResult<List<CommentRating>> GetCommentsRatingsBySpotId(int spotId)
        {
            var comments = _dataContext.GetCommentsRatingsBySpotId(spotId);
            if (comments == null || comments.Count == 0)
            {
                return NotFound("No comments found for the specified camping spot.");
            }

            return Ok(comments);
        }

        // POST: api/CommentRating
        // Adds a new comment and rating to the database.
        [HttpPost]
        public ActionResult AddCommentRating([FromBody] CommentRating model)
        {
            if (model == null)
            {
                return BadRequest("Invalid comment/rating data.");
            }

            _dataContext.AddCommentRating(model);
            return Ok("Comment and rating successfully added!");
        }
    }
}
