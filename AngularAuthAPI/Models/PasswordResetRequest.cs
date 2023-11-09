using System.ComponentModel.DataAnnotations;

namespace AngularAuthAPI.Models
{
    public class PasswordResetRequest
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
