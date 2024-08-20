using System.ComponentModel.DataAnnotations;

namespace AirbnbProject.Models
{
    public class RegisterRequest
    {
       
      
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is  required")]
        [DataType (DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        
         
    }
}