using HospitalRegistrationSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalRegistrationSystem.Pages.Billing
{
    [Authorize(Roles = "Admin")]
    public class MarkAsPaidModel : PageModel
    {
        private readonly AppDbContext _context;

        public MarkAsPaidModel(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
                return NotFound();

            invoice.PaymentStatus = "Paid";

            await _context.SaveChangesAsync();

            return RedirectToPage("Reports");
        }
    }
}