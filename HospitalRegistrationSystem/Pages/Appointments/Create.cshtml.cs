using HospitalRegistrationSystem.Data;
using HospitalRegistrationSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HospitalRegistrationSystem.Pages.Appointments
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Appointment Appointment { get; set; } = new();

        [BindProperty]
        public string SelectedTime { get; set; } = "09:00";

        public Doctor? Doctor { get; set; }

        public List<SelectListItem> TimeSlots { get; set; } = new();

        private void LoadTimeSlots()
        {
            TimeSlots = new List<SelectListItem>
            {
                new SelectListItem("09:00", "09:00"),
                new SelectListItem("10:00", "10:00"),
                new SelectListItem("11:00", "11:00"),
                new SelectListItem("12:00", "12:00"),
                new SelectListItem("14:00", "14:00"),
                new SelectListItem("15:00", "15:00"),
                new SelectListItem("16:00", "16:00"),
                new SelectListItem("17:00", "17:00")
            };
        }

        public async Task<IActionResult> OnGetAsync(string doctorSlug)
        {
            if (string.IsNullOrWhiteSpace(doctorSlug))
                return NotFound();

            Doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Slug == doctorSlug);

            if (Doctor == null)
                return NotFound();

            Appointment.AppointmentDate = DateTime.Today.AddDays(1);
            LoadTimeSlots();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string doctorSlug)
        {
            Doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.Slug == doctorSlug);

            if (Doctor == null)
                return NotFound();

            LoadTimeSlots();

            // ModelState-dən bağlı olmayan field-ları temiziyle
            ModelState.Remove("Appointment.PatientId");
            ModelState.Remove("Appointment.DoctorId");
            ModelState.Remove("Appointment.Status");
            ModelState.Remove("Appointment.Patient");
            ModelState.Remove("Appointment.Doctor");

            // Tarix + saat birleşdir
            var selectedDate = Appointment.AppointmentDate.Date;
            var selectedTime = TimeSpan.Parse(SelectedTime);
            Appointment.AppointmentDate = selectedDate.Add(selectedTime);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Login olan istifadəçinin username-i
            var username = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrWhiteSpace(username))
                return Challenge();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return Challenge();

            // Patient tap
            Patient? patient = null;
            if (user.PatientId.HasValue)
            {
                patient = await _context.Patients.FindAsync(user.PatientId.Value);
            }

            if (patient == null)
            {
                patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.FullName == user.FullName);
            }

            // Patient yoxdursa avtomatik yarat
            if (patient == null)
            {
                patient = new Patient
                {
                    FullName = user.FullName,
                    PatientCode = "PAT-" + DateTime.Now.Ticks,
                    CreatedAt = DateTime.Now
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }

            // User-ə PatientId əlaqəsini qur (əgər yoxdursa)
            if (!user.PatientId.HasValue)
            {
                user.PatientId = patient.Id;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

            // Eyni doctor, eyni vaxt artıq tutulubmu?
            bool isBooked = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == Doctor.Id &&
                a.AppointmentDate == Appointment.AppointmentDate &&
                a.Status != "Cancelled");

            if (isBooked)
            {
                ModelState.AddModelError(string.Empty,
                    "This time slot is already booked. Please choose another time.");
                return Page();
            }

            Appointment.PatientId = patient.Id;
            Appointment.DoctorId = Doctor.Id;
            Appointment.Status = "Pending";
            Appointment.CreatedAt = DateTime.Now;
            Appointment.Notes ??= string.Empty;

            var todayCount = await _context.Appointments.CountAsync(a =>
                a.DoctorId == Doctor.Id &&
                a.AppointmentDate.Date == Appointment.AppointmentDate.Date);

            Appointment.QueueNumber = todayCount + 1;

            _context.Appointments.Add(Appointment);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Appointments/MyAppointments");
        }
    }
}
