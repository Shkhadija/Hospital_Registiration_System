using HospitalRegistrationSystem.Data;
using HospitalRegistrationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospitalRegistrationSystem.Pages.Doctors
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        public Doctor? Doctor { get; set; }

        // Silmə təsdiq səhifəsini göstərir
        public async Task<IActionResult> OnGetAsync(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return NotFound();

            Doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Slug == slug);

            if (Doctor == null)
                return NotFound();

            return Page();
        }

        // Confirm Delete düyməsi basıldıqda
        public async Task<IActionResult> OnPostAsync(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return NotFound();

            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Slug == slug);

            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Doctors/Index");
        }
    }
}