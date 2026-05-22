namespace HospitalRegistrationSystem.Models
{
    public class ApiLog
    {
        public int Id { get; set; }

        public string Method { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;

        public string RequestBody { get; set; } = string.Empty;
        public string ResponseBody { get; set; } = string.Empty;

        public int StatusCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}