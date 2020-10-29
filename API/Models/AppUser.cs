using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class AppUser : IId
    {
        public int Id { get; set; }
        
        [Required]
        public string UserName { get; set; }
        public byte[] Password { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}