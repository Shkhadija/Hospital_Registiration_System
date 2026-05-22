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
    public class EditStatusModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditStatusModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = new();

        public List<SelectListItem> StatusOptions { get; set; } = new();

        private void LoadStatuses()
        {
            StatusOptions = new List<SelectListItem>
            {
                new("Pending", "Pending"),
                new("Confirmed", "Confirmed"),
                new("Completed", "Completed"),
                new("Cancelled", "Cancelled")
            };
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (Appointment == null)
                return NotFound();

            LoadStatuses();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validation xətası olsa dropdown-u yenidən doldur
            if (!ModelState.IsValid)
            {
                LoadStatuses();
                return Page();
            }

            // Bazadan appointment-i bütün əlaqələri ilə birlikdə tap
            var appointmentInDb = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == Appointment.Id);

            if (appointmentInDb == null)
                return NotFound();

            // Köhnə statusu saxla
            var oldStatus = appointmentInDb.Status;

            // Yeni statusu yaz
            appointmentInDb.Status = Appointment.Status;

            // Status ilk dəfə Confirmed olduqda invoice yarat
            if (oldStatus != "Confirmed" &&
                Appointment.Status == "Confirmed")
            {
                bool invoiceExists = await _context.Invoices
                    .AnyAsync(i => i.AppointmentId == appointmentInDb.Id);

                if (!invoiceExists)
                {
                    var invoice = new Invoice
                    {
                        InvoiceNumber =
                            $"INV-{DateTime.Now:yyyyMMddHHmmss}",

                        PatientId = appointmentInDb.PatientId,
                        AppointmentId = appointmentInDb.Id,

                        ConsultationFee = 50m,
                        ServiceFee = 0m,
                        InsuranceDiscount = 0m,
                        TotalAmount = 50m,

                        PaymentStatus = "Unpaid",
                        PaymentMethod = "Cash",

                        CreatedAt = DateTime.Now
                    };

                    _context.Invoices.Add(invoice);
                }
            }

            // Dəyişiklikləri yadda saxla
            await _context.SaveChangesAsync();

            // Siyahıya qayıt
            return RedirectToPage("/Appointments/Index");
        }
    }
}