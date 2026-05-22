using System.ComponentModel.DataAnnotations;

namespace HospitalRegistrationSystem.Models
{
    public class Doctor
    {
        // Primary Key
        public int Id { get; set; }

        // Həkimin tam adı
        // Məsələn: "Leyla Əliyeva"
        [Required(ErrorMessage = "Doctor name is required.")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        // URL üçün istifadə olunacaq slug
        // Məsələn: "leyla-aliyeva"
        [Required]
        [StringLength(120)]
        public string Slug { get; set; } = string.Empty;

        // Həkimin ixtisası
        // Məsələn: Cardiologist, Neurologist, Pediatrician
        [Required(ErrorMessage = "Specialty is required.")]
        [StringLength(100)]
        public string Specialty { get; set; } = string.Empty;

        // Təcrübə ili
        // Məsələn: 8, 12, 20
        [Range(0, 60)]
        public int ExperienceYears { get; set; }

        // Qısa bio
        public string? Bio { get; set; }

        // Həkimin şəkli
        // Məsələn: "/images/doctors/leyla.jpg"
        [StringLength(250)]
        public string? ImageUrl { get; set; }

        // Otaq nömrəsi
        // Məsələn: "205A"
        [StringLength(20)]
        public string RoomNumber { get; set; } = string.Empty;

        // Status:
        // Available, Busy, Off-duty
        [StringLength(30)]
        public string Status { get; set; } = "Available";

        // Email ünvanı
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        // Telefon nömrəsi
        [Phone]
        [StringLength(30)]
        public string Phone { get; set; } = string.Empty;

        // Ətraflı məlumat
        public string Biography { get; set; } = string.Empty;

        // İş saatları
        // Məsələn: "Mon-Fri, 09:00-17:00"
        [StringLength(100)]
        public string WorkingHours { get; set; } = string.Empty;

        // Reytinq
        // Məsələn: 4.8
        [Range(0, 5)]
        public decimal Rating { get; set; } = 5.0m;

        // Qeydiyyat tarixi
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation Property
        // Bir həkimin bir neçə appointment-i ola bilər
        public List<Appointment> Appointments { get; set; } = new();
    }
}