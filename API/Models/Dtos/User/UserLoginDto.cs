using System.ComponentModel.DataAnnotations;

namespace API.Models.Dtos.User
{
    public class UserLoginDto
    {
        [Required]
        public string UserName { get; set; }
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}