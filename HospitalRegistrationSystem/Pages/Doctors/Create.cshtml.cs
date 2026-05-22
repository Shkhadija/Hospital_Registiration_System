using HospitalRegistrationSystem.Data;
using HospitalRegistrationSystem.Helpers;
using HospitalRegistrationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalRegistrationSystem.Pages.Doctors
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        // Formdan gələn doctor məlumatları
        [BindProperty]
        public Doctor Doctor { get; set; } = new();

        // Səhifə açılarkən
        public void OnGet()
        {
        }

        // Save Doctor düyməsinə basıldıqda
        public async Task<IActionResult> OnPostAsync()
        {
            // Slug yarat
            Doctor.Slug = SlugHelper.GenerateSlug(Doctor.FullName);

            // Slug üçün validation xətasını sil
            ModelState.Remove("Doctor.Slug");

            // Slug boş olarsa
            if (string.IsNullOrWhiteSpace(Doctor.Slug))
            {
                Doctor.Slug = Guid.NewGuid().ToString();
            }

            // Status boş olarsa
            if (string.IsNullOrWhiteSpace(Doctor.Status))
            {
                Doctor.Status = "Available";
            }

            // Rating boş və ya 0 olarsa
            if (Doctor.Rating <= 0)
            {
                Doctor.Rating = 5.0m;
            }

            // Şəkil seçilməyibsə
            if (string.IsNullOrWhiteSpace(Doctor.ImageUrl))
            {
                Doctor.ImageUrl = "/images/default-doctor.png";
            }

            // İş saatları boş olarsa
            if (string.IsNullOrWhiteSpace(Doctor.WorkingHours))
            {
                Doctor.WorkingHours = "Mon-Fri, 09:00-17:00";
            }

            // Biography boş olarsa
            if (string.IsNullOrWhiteSpace(Doctor.Biography))
            {
                Doctor.Biography = "Professional doctor at MediCore Hospital.";
            }

            // RoomNumber boş olarsa
            if (string.IsNullOrWhiteSpace(Doctor.RoomNumber))
            {
                Doctor.RoomNumber = "101";
            }

            //<div asp-validation-summary="All" class="text-danger mb-3"></div> Email boş olarsa
            if (string.IsNullOrWhiteSpace(Doctor.Email))
            {
                Doctor.Email = "doctor@medicore.az";
            }

            // Phone boş olarsa
            if (string.IsNullOrWhiteSpace(Doctor.Phone))
            {
                Doctor.Phone = "+994500000000";
            }

            // Validation yoxla
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Bazaya əlavə et
            _context.Doctors.Add(Doctor);
            await _context.SaveChangesAsync();

            // Doctors siyahısına qayıt
            return RedirectToPage("/Doctors/Index");
        }
    }
}