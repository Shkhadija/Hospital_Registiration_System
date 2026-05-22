using System.ComponentModel.DataAnnotations;

namespace HospitalRegistrationSystem.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        // Xəstə ID-si
        [Required]
        public int PatientId { get; set; }

        // Navigation property
        public Patient? Patient { get; set; }

        // Həkim ID-si
        [Required]
        public int DoctorId { get; set; }

        // Navigation property
        public Doctor? Doctor { get; set; }

        // Görüş tarixi və saatı
        [Required]
        public DateTime AppointmentDate { get; set; }

        // Status:
        // Pending, Confirmed, Completed, Cancelled
        [StringLength(30)]
        public string Status { get; set; } = "Pending";

        // Xəstənin əlavə qeydləri (optional)
        [StringLength(500)]
        public string? Notes { get; set; }

        // Həmin gün üçün növbə sırası
        public int QueueNumber { get; set; }
     
        // Yaradılma tarixi
        public DateTime CreatedAt { get; set; } = DateTime.Now;
      
    }
}