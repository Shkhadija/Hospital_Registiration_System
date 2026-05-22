using HospitalRegistrationSystem.Data;
using HospitalRegistrationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace HospitalRegistrationSystem.Pages.Appointments
{
    [Authorize(Roles = "Admin,Receptionist")]
    public class ManageModel : PageModel
    {
        private readonly AppDbContext _context;

        public ManageModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = new();

        public List<SelectListItem> StatusOptions { get; set; } = new()
        {
            new SelectListItem("Pending", "Pending"),
            new SelectListItem("Confirmed", "Confirmed"),
            new SelectListItem("Completed", "Completed"),
            new SelectListItem("Cancelled", "Cancelled")
        };

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            Appointment = appointment;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var appointmentInDb = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == Appointment.Id);

            if (appointmentInDb == null)
                return NotFound();

            var oldStatus = appointmentInDb.Status;

            // Dəyişdirilə bilən sahələri yenilə
            appointmentInDb.Status = Appointment.Status;
            appointmentInDb.Notes = Appointment.Notes;

            // Status ilk dəfə "Confirmed" olduqda avtomatik invoice yarat
            if (oldStatus != "Confirmed" && Appointment.Status == "Confirmed")
            {
                bool invoiceExists = await _context.Invoices
                    .AnyAsync(i => i.AppointmentId == appointmentInDb.Id);

                if (!invoiceExists)
                {
                    // Doctor-a görə konsultasiya haqqı
                    decimal fee = 50m;

                    var invoice = new Invoice
                    {
                        InvoiceNumber   = $"INV-{DateTime.Now:yyyyMMddHHmmss}",
                        PatientId       = appointmentInDb.PatientId,
                        AppointmentId   = appointmentInDb.Id,
                        ConsultationFee = fee,
                        ServiceFee      = 0m,
                        InsuranceDiscount = 0m,
                        TotalAmount     = fee,
                        PaymentStatus   = "Unpaid",
                        PaymentMethod   = "Cash",
                        CreatedAt       = DateTime.Now
                    };

                    _context.Invoices.Add(invoice);
                }
            }

            // Status "Completed"-ə keçəndə mövcud invoice-u "Paid" et (əgər hələ Unpaid-dirsə)
            // Bu avtomatik deyil — admin özü Mark as Paid edəcək, bunu saxlayırıq

            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
