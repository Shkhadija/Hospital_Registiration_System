using HospitalRegistrationSystem.Data;
using HospitalRegistrationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HospitalRegistrationSystem.Pages.Doctors
{
    
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        // View-də istifadə olunacaq doctor siyahısı
        public List<Doctor> Doctors { get; set; } = new();

        // URL-dən gələn axtarış sözü (?search=cardio)
        [BindProperty(SupportsGet = true)]
        public string Search { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            // Əgər bazada həkim yoxdursa, bir neçəsini əlavə edək (test üçün)
            if (!await _context.Doctors.AnyAsync())
            {
                var dummyDoctors = new List<Doctor>
                {
                    new Doctor { FullName = "Dr. Hikmət Cavadov", Specialty = "Cardiologist", Slug = "hikmet-cavadov", ExperienceYears = 15, Status = "Available", RoomNumber = "101", Email = "hikmet@hospital.com", ImageUrl = "https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?auto=format&fit=crop&w=500&q=80" },
                    new Doctor { FullName = "Dr. Leyla Əliyeva", Specialty = "Neurologist", Slug = "leyla-aliyeva", ExperienceYears = 10, Status = "Available", RoomNumber = "102", Email = "leyla@hospital.com", ImageUrl = "https://images.unsplash.com/photo-1559839734-2b71a197ec25?auto=format&fit=crop&w=500&q=80" },
                    new Doctor { FullName = "Dr. Elnur Məmmədov", Specialty = "Pediatrician", Slug = "elnur-mammedov", ExperienceYears = 8, Status = "Busy", RoomNumber = "103", Email = "elnur@hospital.com", ImageUrl = "https://images.unsplash.com/photo-1537368910025-700350fe46c7?auto=format&fit=crop&w=500&q=80" }
                };
                _context.Doctors.AddRange(dummyDoctors);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Mövcud dummy həkimlərin şəkillərini yeniləyək (məcburi overwrite)
                var hikmet = await _context.Doctors.FirstOrDefaultAsync(d => d.Slug == "hikmet-cavadov");
                if (hikmet != null) { hikmet.ImageUrl = "https://images.unsplash.com/photo-1612349317150-e413f6a5b16d?auto=format&fit=crop&w=500&q=80"; }

                var leyla = await _context.Doctors.FirstOrDefaultAsync(d => d.Slug == "leyla-aliyeva");
                if (leyla != null) { leyla.ImageUrl = "https://images.unsplash.com/photo-1559839734-2b71a197ec25?auto=format&fit=crop&w=500&q=80"; }

                var elnur = await _context.Doctors.FirstOrDefaultAsync(d => d.Slug == "elnur-mammedov");
                if (elnur != null) { elnur.ImageUrl = "https://images.unsplash.com/photo-1537368910025-700350fe46c7?auto=format&fit=crop&w=500&q=80"; }

                await _context.SaveChangesAsync();
            }

            // Doctors cədvəlindən query başlayırıq
            var query = _context.Doctors.AsQueryable();

            // Search yazılıbsa ad və ixtisasa görə filter et
            if (!string.IsNullOrWhiteSpace(Search))
            {
                query = query.Where(d =>
                    d.FullName.Contains(Search) ||
                    d.Specialty.Contains(Search));
            }

            // Nəticəni ada görə sıralayıb siyahıya çeviririk
            Doctors = await query
                .OrderBy(d => d.FullName)
                .ToListAsync();
        }
    }
}