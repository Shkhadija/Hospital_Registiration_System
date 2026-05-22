using System.ComponentModel.DataAnnotations;

namespace HospitalRegistrationSystem.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        // Avtomatik yaradılacaq invoice nömrəsi
        // Məsələn: INV-2026-0001
        [Required]
        [StringLength(30)]
        public string InvoiceNumber { get; set; } = string.Empty;

        // Patient ilə əlaqə
        [Required]
        public int PatientId { get; set; }

        public Patient? Patient { get; set; }

        // Appointment ilə əlaqə
        [Required]
        public int AppointmentId { get; set; }

        public Appointment? Appointment { get; set; }

        // Həkim konsultasiya haqqı
        public decimal ConsultationFee { get; set; } = 0;

        // Əlavə xidmət haqqı
        public decimal ServiceFee { get; set; } = 0;

        // Sığorta endirimi
        public decimal InsuranceDiscount { get; set; } = 0;

        // Yekun məbləğ
        public decimal TotalAmount { get; set; } = 0;

        // Paid, Unpaid, Partially Paid
        [Required]
        [StringLength(30)]
        public string PaymentStatus { get; set; } = "Unpaid";

        // Ödəniş üsulu
        // Cash, Card, Insurance
        [Required]
        [StringLength(30)]
        public string PaymentMethod { get; set; } = "Cash";

        // Invoice yaradılma tarixi
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}