using HospitalRegistrationSystem.Data;
using HospitalRegistrationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospitalRegistrationSystem.Pages.Patients
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        // Səhifədə göstəriləcək xəstələr
        public IList<Patient> Patients { get; set; } = new List<Patient>();

        // Search box üçün property
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync()
        {
            IQueryable<Patient> query = _context.Patients;

            // Search varsa filter et
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(p =>
                  p.FullName.Contains(SearchTerm) ||
                  p.Phone.Contains(SearchTerm));
            }

            // Nəticələri gətir
            Patients = await query
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }
    }
}