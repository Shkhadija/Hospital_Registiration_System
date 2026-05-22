using HospitalRegistrationSystem.Data;
using HospitalRegistrationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HospitalRegistrationSystem.Pages.Appointments
{
    [Authorize]
    public class MyAppointmentsModel : PageModel
    {
        private readonly AppDbContext _context;

        public MyAppointmentsModel(AppDbContext context)
        {
            _context = context;
        }

        // İstifadəçinin appointment-ləri
        public List<Appointment> Appointments { get; set; } = new();

        public int DebugTotalAppointments { get; set; }
        public int? DebugUserPatientId { get; set; }
        public string DebugUserFullName { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            // Login olan istifadəçinin username-i
            var username = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrWhiteSpace(username))
                return;

            // User tapılır
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return;

            DebugTotalAppointments = await _context.Appointments.CountAsync();
            DebugUserPatientId = user.PatientId;
            DebugUserFullName = user.FullName;

            // Appointment-ləri gətir (Həm ID ilə, həm də Ad ilə axtarırıq ki, dəqiq tapsın)
            var patientId = user.PatientId;
            var fullName = user.FullName;

            Appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Where(a => a.PatientId == patientId || (a.Patient != null && a.Patient.FullName == fullName))
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }
    }
}