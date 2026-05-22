using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalRegistrationSystem.Models
{
    public class User
    {
        public int Id { get; set; }

        // İstifadəçinin tam adı
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        // Login üçün istifadə olunan username
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        // Şifrəni açıq formada yox, hash kimi saxlayacağıq
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        // Role-based access üçün: Admin, Receptionist, Doctor
        [Required]
        [StringLength(30)]
        public string Role { get; set; } = string.Empty;

        // Xəstə ilə əlaqə (əgər rolu Patient-dirsə)
        public int? PatientId { get; set; }
        
        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }

        // Account-un yaradılma tarixi
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}