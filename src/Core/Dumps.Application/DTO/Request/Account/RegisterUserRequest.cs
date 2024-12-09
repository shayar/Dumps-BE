using System.ComponentModel.DataAnnotations;

namespace Dumps.Application.DTO.Request.RegisterUser
{
    public class RegisterUserRequest
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
