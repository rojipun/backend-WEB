using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AirbnbProject.Data;
using AirbnbProject.Models;
using Microsoft.Extensions.Logging;

namespace AirbnbProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IDataContext _data;  // Dependency injection for data context to interact with the database.
        private readonly ILogger<UserController> _logger;  // Dependency injection for logging.

        // Constructor to initialize the controller with the injected services.
        public UserController(IDataContext data, ILogger<UserController> logger)
        {
            _data = data;
            _logger = logger;
        }

        // Endpoint for user login. Takes a login request, authenticates the user, and returns a JWT token if successful.
        [HttpPost("login")]
        public IActionResult LogIn([FromBody] AuthenticateRequest login)
        {
            if (login == null)
                return BadRequest("Invalid login request");  // Validate if the login request body is not null.

            try
            {
                var response = _data.Authenticate(login);  // Attempt to authenticate the user.

                if (response != null)
                {
                    return Ok(response);  // Return the JWT token if authentication is successful.
                }
                else
                {
                    return Unauthorized("Invalid username or password");  // Return unauthorized if credentials are incorrect.
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");  // Log any errors that occur during the login process.
                return StatusCode(500, "Internal server error");  // Return a generic server error.
            }
        }

        // Endpoint for user registration. Takes a registration request, creates a new user, and returns a success message.
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest model)
        {
            if (model == null)
                return BadRequest("Invalid registration request");  // Validate if the registration request body is not null.

            // Validate if the password and confirm password fields match.
            if (model.Password != model.ConfirmPassword)
                return BadRequest("Passwords do not match");

            try
            {
                _data.Register(model);  // Register the new user.
                return Ok("Registration successful");  // Return success message.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");  // Log any errors that occur during the registration process.
                return StatusCode(500, "Internal server error");  // Return a generic server error.
            }
        }

        // Endpoint to get information about all users. This method should only be accessible by authorized users.
        [HttpGet("Userinfo")]
        public IActionResult Getuserinfo()
        {
            try
            {
                var users = _data.GetAllUsers();  // Retrieve all users from the database.
                return Ok(users);  // Return the list of users.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");  // Log any errors that occur during the retrieval process.
                return StatusCode(500, "Internal server error");  // Return a generic server error.
            }
        }

        // Endpoint to get all users, restricted to users with the 'admin' role.
        [HttpGet("all")]
        [Authorize(Roles = "admin")]
        public IActionResult GetAll()
        {
            try
            {
                var users = _data.GetAllUsers();  // Retrieve all users from the database.
                return Ok(users);  // Return the list of users.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");  // Log any errors that occur during the retrieval process.
                return StatusCode(500, "Internal server error");  // Return a generic server error.
            }
        }

        // Endpoint to get a specific user by their user ID.
        [HttpGet("getUsId/{userId}")]
        public IActionResult Get(int userId)
        {
            try
            {
                var user = _data.GetUserById(userId);  // Retrieve the user by their ID.

                if (user != null)
                {
                    return Ok(user);  // Return the user if found.
                }
                else
                {
                    return NotFound("User not found");  // Return a 404 error if the user is not found.
                }
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found");  // Log a warning if the user is not found.
                return NotFound("User not found");  // Return a 404 error if the user is not found.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by ID");  // Log any errors that occur during the retrieval process.
                return StatusCode(500, "Internal server error");  // Return a generic server error.
            }
        }

        // Endpoint to update a user's information by their user ID.
        [HttpPut("putUsId/{userId}")]
        public IActionResult Update(int userId, [FromBody] UpdateRequest model)
        {
            if (model == null)
                return BadRequest("Invalid update request");  // Validate if the update request body is not null.

            try
            {
                var existingUser = _data.GetUserById(userId);  // Check if the user exists.

                if (existingUser == null)
                    return NotFound("User not found");  // Return a 404 error if the user is not found.

                _data.UpdateUser(userId, model);  // Update the user's information.
                return Ok("User information updated successfully");  // Return success message.
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found");  // Log a warning if the user is not found.
                return NotFound("User not found");  // Return a 404 error if the user is not found.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");  // Log any errors that occur during the update process.
                return StatusCode(500, "Internal server error");  // Return a generic server error.
            }
        }

        // Endpoint to delete a user by their user ID. Restricted to users with the 'admin' role.
        [HttpDelete("deleteUsId/{userId}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int userId)
        {
            try
            {
                var existingUser = _data.GetUserById(userId);  // Check if the user exists.

                if (existingUser == null)
                    return NotFound("User not found");  // Return a 404 error if the user is not found.

                _data.DeleteUser(userId);  // Delete the user.
                return Ok("User deleted successfully");  // Return success message.
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found");  // Log a warning if the user is not found.
                return NotFound("User not found");  // Return a 404 error if the user is not found.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");  // Log any errors that occur during the deletion process.
                return StatusCode(500, "Internal server error");  // Return a generic server error.
            }
        }
    }
}
