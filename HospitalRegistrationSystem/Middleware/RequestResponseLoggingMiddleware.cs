using System.Text;
using HospitalRegistrationSystem.Data;
using HospitalRegistrationSystem.Models;

namespace HospitalRegistrationSystem.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext db)
        {
            // Swagger requestlərini loglama
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                await _next(context);
                return;
            }

            // Request body oxumaq
            context.Request.EnableBuffering();

            string requestBody = "";

            using (var reader = new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                leaveOpen: true))
            {
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            // Response body-ni tutmaq
            var originalResponseBody = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            // Controller işləsin
            await _next(context);

            // Response body oxumaq
            responseBody.Position = 0;
            string responseText = await new StreamReader(responseBody).ReadToEndAsync();

            // Database-ə yazmaq
            var log = new ApiLog
            {
                Method = context.Request.Method,
                Path = context.Request.Path,
                RequestBody = requestBody,
                ResponseBody = responseText,
                StatusCode = context.Response.StatusCode,
                CreatedAt = DateTime.UtcNow
            };

            db.ApiLogs.Add(log);
            await db.SaveChangesAsync();

            // Response-u istifadəçiyə geri göndərmək
            responseBody.Position = 0;
            await responseBody.CopyToAsync(originalResponseBody);

            context.Response.Body = originalResponseBody;
        }
    }
}