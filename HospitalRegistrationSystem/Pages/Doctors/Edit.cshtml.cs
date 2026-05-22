using HospitalRegistrationSystem.Data;
using HospitalRegistrationSystem.Helpers;
using HospitalRegistrationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospitalRegistrationSystem.Pages.Doctors
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Doctor Doctor { get; set; } = new();

        // URL-dən slug alıb doctor-u yükləyirik
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

        // Save düyməsi basıldıqda
        public async Task<IActionResult> OnPostAsync()
        {
            // Slug-i yenidən yaradırıq (ad dəyişibsə URL də dəyişsin)
            Doctor.Slug = SlugHelper.GenerateSlug(Doctor.FullName);
            ModelState.Remove("Doctor.Slug");

            if (!ModelState.IsValid)
                return Page();

            _context.Attach(Doctor).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToPage("/Doctors/Index");
        }
    }
}