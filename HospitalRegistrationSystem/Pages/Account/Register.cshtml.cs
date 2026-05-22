using HospitalRegistrationSystem.Data;
using HospitalRegistrationSystem.Models;
using HospitalRegistrationSystem.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HospitalRegistrationSystem.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _context;

        public RegisterModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public RegisterInput Input { get; set; } = new();

        public class RegisterInput
        {
            [Required(ErrorMessage = "Full name is required.")]
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Date of birth is required.")]
            [DataType(DataType.Date)]
            [Display(Name = "Date of Birth")]
            public DateTime BirthDate { get; set; } = DateTime.Today.AddYears(-25);

            [Display(Name = "Gender")]
            public string Gender { get; set; } = string.Empty;

            [Display(Name = "Phone")]
            public string Phone { get; set; } = string.Empty;

            [Required(ErrorMessage = "Username is required.")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required.")]
            [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Please confirm your password.")]
            [Display(Name = "Confirm Password")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public void OnGet()
        {
            Input.BirthDate = DateTime.Today.AddYears(-25);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (Input.Password != Input.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return Page();
            }

            // BirthDate validation - must be in past and realistic
            if (Input.BirthDate >= DateTime.Today)
            {
                ModelState.AddModelError("Input.BirthDate", "Date of birth must be in the past.");
                return Page();
            }

            if (Input.BirthDate.Year < 1900)
            {
                ModelState.AddModelError("Input.BirthDate", "Please enter a valid date of birth.");
                return Page();
            }

            bool usernameExists = await _context.Users
                .AnyAsync(u => u.Username == Input.Username);

            if (usernameExists)
            {
                ModelState.AddModelError("", "Username already exists.");
                return Page();
            }

            // Patient yarat
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.FullName == Input.FullName);

            if (patient == null)
            {
                patient = new Patient
                {
                    FullName = Input.FullName,
                    PatientCode = "PAT-" + DateTime.Now.Ticks,
                    BirthDate = Input.BirthDate,
                    Gender = Input.Gender,
                    Phone = Input.Phone,
                    CreatedAt = DateTime.Now
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Mövcud patientin BirthDate-i yenilə (əgər default-dursa)
                if (patient.BirthDate == DateTime.MinValue || patient.BirthDate.Year == 1)
                {
                    patient.BirthDate = Input.BirthDate;
                    if (string.IsNullOrEmpty(patient.Gender)) patient.Gender = Input.Gender;
                    if (string.IsNullOrEmpty(patient.Phone)) patient.Phone = Input.Phone;
                    _context.Patients.Update(patient);
                    await _context.SaveChangesAsync();
                }
            }

            var user = new User
            {
                FullName = Input.FullName,
                Username = Input.Username,
                PasswordHash = PasswordHelper.HashPassword(Input.Password),
                Role = "Patient",
                PatientId = patient.Id,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Account/Login");
        }
    }
}
