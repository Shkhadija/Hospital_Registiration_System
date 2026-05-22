using HospitalRegistrationSystem.Data;
using HospitalRegistrationSystem.Helpers;
using HospitalRegistrationSystem.Middleware;
using HospitalRegistrationSystem.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ==============================
// Razor Pages xidmətlərini əlavə edirik
// ==============================
builder.Services.AddRazorPages();

// ==============================
// SQL Server bağlantısını qururuq
// ==============================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// ==============================
// Cookie Authentication
// ==============================
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Login olmayan istifadəçi bu səhifəyə yönləndiriləcək
        options.LoginPath = "/Account/Login";

        // İcazəsi olmayan istifadəçi bu səhifəyə yönləndiriləcək
        options.AccessDeniedPath = "/Account/AccessDenied";

        // Cookie adı
        options.Cookie.Name = "HospitalAuthCookie";

        // Cookie müddəti
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);

        // Aktivlik olduqca müddəti yenilə
        options.SlidingExpiration = true;
    });

// ==============================
// Authorization
// ==============================
builder.Services.AddAuthorization();

// ==============================
// Application build
// ==============================
var app = builder.Build();

// ==============================
// İlk dəfə admin istifadəçisi yarat
// ==============================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Database və migration-ləri tətbiq et
    db.Database.Migrate();

    // Əgər Users cədvəli boşdursa admin yarat
    if (!db.Users.Any())
    {
        db.Users.Add(new User
        {
            FullName = "Admin",
            Username = "admin",
            PasswordHash = PasswordHelper.HashPassword("123456"),
            Role = "Admin",
            CreatedAt = DateTime.Now
        });

        db.SaveChanges();
    }
}

// ==============================
// Production mühiti üçün Error Handling
// ==============================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// ==============================
// Middleware Pipeline
// ==============================

// HTTPS yönləndirməsi
app.UseHttpsRedirection();

// Static files (CSS, JS, şəkillər)
app.UseStaticFiles();

// Routing
app.UseRouting();

// Authentication
app.UseAuthentication();

// Authorization
app.UseAuthorization();

// Request və Response logging middleware
app.UseMiddleware<RequestResponseLoggingMiddleware>();

// Razor Pages route-ları
app.MapRazorPages();

// ==============================
// Tətbiqi işə sal
// ==============================
app.Run();