using HospitalRegistrationSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HospitalRegistrationSystem.Pages.Dashboard
{
    [Authorize] // Yalnız login olmuş istifadəçilər daxil ola bilər
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        // Dashboard kartlarında göstəriləcək saylar
        public int TotalPatients { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalUsers { get; set; }

        // Dashboard səhifəsi açılarkən işləyir
        public void OnGet()
        {
            // Verilənlər bazasındakı ümumi sayları hesablayırıq
            TotalPatients = _context.Patients.Count();
            TotalDoctors = _context.Doctors.Count();
            TotalAppointments = _context.Appointments.Count();
            TotalUsers = _context.Users.Count();
        }
    }
}