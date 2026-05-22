using HospitalRegistrationSystem.Data;
using HospitalRegistrationSystem.Helpers;
using HospitalRegistrationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospitalRegistrationSystem.Pages.Patients
{
    [Authorize(Roles = "Admin,Receptionist")]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Patient Patient { get; set; } = new();

        [BindProperty]
        public bool CreateUserAccount { get; set; }

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public void OnGet()
        {
            // Default BirthDate - formu açanda boş görünsün
            Patient.BirthDate = DateTime.Today.AddYears(-30);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Patient.PatientCode = $"PT-{DateTime.Now:yyyyMMddHHmmss}";
            ModelState.Remove("Patient.PatientCode");

            if (string.IsNullOrWhiteSpace(Patient.Gender))
                Patient.Gender = "Not Specified";

            // BirthDate validasiyası
            if (Patient.BirthDate == DateTime.MinValue || Patient.BirthDate.Year <= 1900)
            {
                ModelState.AddModelError("Patient.BirthDate", "Please enter a valid date of birth.");
            }

            if (!ModelState.IsValid)
                return Page();

            _context.Patients.Add(Patient);
            await _context.SaveChangesAsync(); // Patient ID almaq üçün əvvəl save et

            if (CreateUserAccount)
            {
                if (string.IsNullOrWhiteSpace(Username))
                {
                    ModelState.AddModelError(string.Empty, "Username is required.");
                    return Page();
                }

                if (string.IsNullOrWhiteSpace(Password))
                {
                    ModelState.AddModelError(string.Empty, "Password is required.");
                    return Page();
                }

                bool usernameExists = await _context.Users
                    .AnyAsync(u => u.Username == Username);

                if (usernameExists)
                {
                    ModelState.AddModelError(string.Empty, "Username already exists.");
                    return Page();
                }

                var user = new User
                {
                    FullName = Patient.FullName,
                    Username = Username,
                    PasswordHash = PasswordHelper.HashPassword(Password),
                    Role = "Patient",
                    PatientId = Patient.Id,  // FIX: PatientId düzgün set edilir
                    CreatedAt = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Patients/Index");
        }
    }
}
