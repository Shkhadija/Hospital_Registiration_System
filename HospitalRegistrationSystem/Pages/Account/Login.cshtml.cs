using System.Security.Claims;
using HospitalRegistrationSystem.Data;
using HospitalRegistrationSystem.Helpers;
using HospitalRegistrationSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospitalRegistrationSystem.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _context;

        public LoginModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LoginViewModel Input { get; set; } = new();

        public IActionResult OnGet()
        {
            // İstifadəçi artıq login olubsa Dashboard-a yönləndir
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Dashboard/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Form validation
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Username-də boşluqları təmizlə
            var username = Input.Username.Trim();

            // Daxil edilən şifrəni hash et
            var hashedPassword =
                PasswordHelper.HashPassword(Input.Password);

            // Username üzrə user tap
            var userByUsername = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            // User ümumiyyətlə yoxdur
            if (userByUsername == null)
            {
                ModelState.AddModelError(string.Empty,
                    "Username və ya şifrə yanlışdır.");
                return Page();
            }

            // Şifrə uyğun gəlmir
            if (userByUsername.PasswordHash != hashedPassword)
            {
                ModelState.AddModelError(string.Empty,
                    "Username və ya şifrə yanlışdır.");
                return Page();
            }

            // Cookie üçün claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userByUsername.FullName),
                new Claim(ClaimTypes.NameIdentifier,
                    userByUsername.Id.ToString()),
                new Claim(ClaimTypes.Role, userByUsername.Role),
                new Claim("Username", userByUsername.Username)
            };

            // Identity yarat
            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            // Remember Me
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = Input.RememberMe
            };

            // Login et
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Dashboard-a yönləndir
            return RedirectToPage("/Dashboard/Index");
        }
    }
}