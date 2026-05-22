using Microsoft.AspNetCore.Authentication; // SignOutAsync üçün
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace HospitalRegistrationSystem.Pages.Account
{
    [Authorize]
    public class LogoutModel : PageModel
    {
        // Səhifə açılan kimi işləyir
        public async Task<IActionResult> OnGetAsync()
        {
            // Authentication cookie-ni silir
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            // Login səhifəsinə yönləndirir
            return RedirectToPage("/Account/Login");
        }
    }
}