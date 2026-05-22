using HospitalRegistrationSystem.Data;
using HospitalRegistrationSystem.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospitalRegistrationSystem.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Doctor> FeaturedDoctors { get; set; } = new();

        public async Task OnGetAsync()
        {
            FeaturedDoctors = await _context.Doctors
                .Take(3)
                .ToListAsync();
        }
    }
}