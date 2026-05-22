using HospitalRegistrationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace HospitalRegistrationSystem.Data
{
    // AppDbContext database ilə C# modelləri arasında körpü rolunu oynayır.
    // Buradakı hər DbSet database-də bir cədvəl (table) kimi yaradılır.
    public class AppDbContext : DbContext
    {
        // Constructor
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // =========================
        // Tables
        // =========================

        // Login/Register üçün istifadəçilər
        public DbSet<User> Users { get; set; }

        // Həkimlər
        public DbSet<Doctor> Doctors { get; set; }

        // Xəstələr
        public DbSet<Patient> Patients { get; set; }

        // Növbələr (appointments)
        public DbSet<Appointment> Appointments { get; set; }

        // Ödənişlər
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<ApiLog> ApiLogs { get; set; }

        // =========================
        // Fluent API
        // =========================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------------------------------
            // User.Username unikal olsun
            // ---------------------------------
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // ---------------------------------
            // Doctor.Slug unikal olsun
            // Məsələn: /Doctors/leyla-aliyeva
            // ---------------------------------
            modelBuilder.Entity<Doctor>()
                .HasIndex(d => d.Slug)
                .IsUnique();

            // ---------------------------------
            // Patient.PatientCode unikal olsun
            // ---------------------------------
            modelBuilder.Entity<Patient>()
                .HasIndex(p => p.PatientCode)
                .IsUnique();

            // ---------------------------------
            // Invoice.InvoiceNumber unikal olsun
            // ---------------------------------
            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.InvoiceNumber)
                .IsUnique();
            
            // ---------------------------------
            // Invoice -> Patient əlaqəsi
            // Patient silinərkən Restrict tətbiq edilir
            // ---------------------------------
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Patient)
                .WithMany(p => p.Invoices)
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------------------------
            // Appointment -> Patient əlaqəsi
            // ---------------------------------
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------------------------
            // Appointment -> Doctor əlaqəsi
            // ---------------------------------
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}