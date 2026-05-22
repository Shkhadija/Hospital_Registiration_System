using HospitalRegistrationSystem.Data;
using HospitalRegistrationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospitalRegistrationSystem.Pages.Billing
{
    [Authorize(Roles = "Admin,Receptionist")]
    public class ReportsModel : PageModel
    {
        private readonly AppDbContext _context;

        public ReportsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Invoice> Invoices { get; set; } = new();

        public int TotalInvoices { get; set; }
        public int PaidInvoices { get; set; }
        public int PendingInvoices { get; set; }
        public decimal TotalRevenue { get; set; }

        public async Task OnGetAsync()
        {
            Invoices = await _context.Invoices
                .Include(i => i.Patient)
                .Include(i => i.Appointment)
                    .ThenInclude(a => a!.Doctor)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            TotalInvoices   = Invoices.Count;
            PaidInvoices    = Invoices.Count(i => i.PaymentStatus == "Paid");
            PendingInvoices = Invoices.Count(i => i.PaymentStatus == "Unpaid");
            TotalRevenue    = Invoices
                .Where(i => i.PaymentStatus == "Paid")
                .Sum(i => i.TotalAmount);
        }

        // Invoice məbləğini yenilə
        public async Task<IActionResult> OnPostUpdateFeeAsync(int invoiceId, decimal consultationFee, decimal serviceFee, decimal insuranceDiscount, string paymentMethod)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);
            if (invoice == null) return NotFound();

            invoice.ConsultationFee   = consultationFee;
            invoice.ServiceFee        = serviceFee;
            invoice.InsuranceDiscount = insuranceDiscount;
            invoice.TotalAmount       = consultationFee + serviceFee - insuranceDiscount;
            invoice.PaymentMethod     = paymentMethod;

            await _context.SaveChangesAsync();
            TempData["Success"] = $"Invoice #{invoice.InvoiceNumber} updated.";
            return RedirectToPage();
        }

        // Invoice sil
        public async Task<IActionResult> OnPostDeleteAsync(int invoiceId)
        {
            var invoice = await _context.Invoices.FindAsync(invoiceId);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Invoice deleted.";
            }
            return RedirectToPage();
        }
    }
}
