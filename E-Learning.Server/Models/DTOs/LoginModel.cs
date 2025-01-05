using System.ComponentModel.DataAnnotations;

namespace E_Learning.Server.Models.DTOs
{
    public class LoginModel
    {
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
