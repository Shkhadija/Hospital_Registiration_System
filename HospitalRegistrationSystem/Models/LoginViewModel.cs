using System.ComponentModel.DataAnnotations;

namespace HospitalRegistrationSystem.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username daxil edin")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifrə daxil edin")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;
    }
}