using HospitalRegistrationSystem.Data;
using HospitalRegistrationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HospitalRegistrationSystem.Helpers;

namespace HospitalRegistrationSystem.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly AppDbContext _context;

        public UsersModel(AppDbContext context)
        {
            _context = context;
        }

        public List<User> Users { get; set; } = new();

        [BindProperty]
        public User NewUser { get; set; } = new();

        public async Task OnGetAsync()
        {
            Users = await _context.Users
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        // Add new user
        public async Task<IActionResult> OnPostAddAsync(string newUserPassword)
        {
            if (string.IsNullOrWhiteSpace(NewUser.FullName) ||
                string.IsNullOrWhiteSpace(NewUser.Username) ||
                string.IsNullOrWhiteSpace(newUserPassword) ||
                string.IsNullOrWhiteSpace(NewUser.Role))
            {
                TempData["Error"] = "All fields are required.";
                await OnGetAsync();
                return Page();
            }

            // Username mövcuddurmu?
            bool usernameExists = await _context.Users.AnyAsync(u => u.Username == NewUser.Username);
            if (usernameExists)
            {
                TempData["Error"] = $"Username '{NewUser.Username}' already exists.";
                await OnGetAsync();
                return Page();
            }

            NewUser.CreatedAt = DateTime.Now;
            NewUser.PasswordHash = PasswordHelper.HashPassword(newUserPassword);

            if (NewUser.Role == "Patient")
            {
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.FullName == NewUser.FullName);

                if (patient == null)
                {
                    patient = new Patient
                    {
                        FullName = NewUser.FullName,
                        PatientCode = "PAT-" + DateTime.Now.Ticks,
                        CreatedAt = DateTime.Now
                    };
                    _context.Patients.Add(patient);
                    await _context.SaveChangesAsync();
                }

                NewUser.PatientId = patient.Id;
            }

            _context.Users.Add(NewUser);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"User '{NewUser.Username}' added successfully.";
            return RedirectToPage();
        }

        // Delete user
        public async Task<IActionResult> OnPostDeleteAsync(int userId)
        {
            var currentUsername = User.Identity?.Name;

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToPage();
            }

            // Özünü silə bilməz
            if (user.Username == currentUsername)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToPage();
            }

            // Son Admin-i silmə
            if (user.Role == "Admin")
            {
                int adminCount = await _context.Users.CountAsync(u => u.Role == "Admin");
                if (adminCount <= 1)
                {
                    TempData["Error"] = "Cannot delete the last admin account.";
                    return RedirectToPage();
                }
            }

            // Bu userin appointment-lərini sil (əgər varsa)
            // Patient record-u saxlayırıq, yalnız user account-u silirik
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"User '{user.Username}' deleted successfully.";
            return RedirectToPage();
        }

        // Reset password
        public async Task<IActionResult> OnPostResetPasswordAsync(int userId, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
            {
                TempData["Error"] = "Password must be at least 6 characters.";
                return RedirectToPage();
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToPage();
            }

            user.PasswordHash = PasswordHelper.HashPassword(newPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Password for '{user.Username}' has been reset.";
            return RedirectToPage();
        }
    }
}
