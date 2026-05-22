using System.ComponentModel.DataAnnotations;

namespace HospitalRegistrationSystem.Models
{
    public class Patient
    {
        public int Id { get; set; }

        // Avtomatik yaradılacaq xəstə kodu.
        [Required]
        [StringLength(30)]
        public string PatientCode { get; set; } = string.Empty;

        // Xəstənin tam adı
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        // FIN və ya şəxsiyyət kodu
        [StringLength(20)]
        public string FinCode { get; set; } = string.Empty;

        // Doğum tarixi
        public DateTime BirthDate { get; set; }
        public string Email { get; set; } = string.Empty;

        // Gender: Male, Female
        [StringLength(20)]
        public string Gender { get; set; } = string.Empty;

        // Əlaqə nömrəsi
        [StringLength(30)]
        public string Phone { get; set; } = string.Empty;

        // Ünvan
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        // Qan qrupu
        [StringLength(10)]
        public string BloodGroup { get; set; } = string.Empty;

        // Allergiyalar
        public string Allergies { get; set; } = string.Empty;

        // Xroniki xəstəliklər
        public string ChronicDiseases { get; set; } = string.Empty;

        // Təcili əlaqə şəxsi
        [StringLength(100)]
        public string EmergencyContact { get; set; } = string.Empty;

        // Insurance: Insured, Not Insured
        [StringLength(30)]
        public string InsuranceStatus { get; set; } = string.Empty;

        // Xəstənin şikayəti
        public string Complaint { get; set; } = string.Empty;

        // Priority: Normal, Urgent, Emergency
        [StringLength(30)]
        public string Priority { get; set; } = "Normal";

        // Qeydiyyat tarixi
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Bir xəstənin bir neçə appointment-i ola bilər
        public List<Appointment> Appointments { get; set; } = new();

        // Bir xəstənin bir neçə invoice-u ola bilər
        public List<Invoice> Invoices { get; set; } = new();
    }
}