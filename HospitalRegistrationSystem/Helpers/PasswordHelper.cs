using System.Security.Cryptography;
using System.Text;

namespace HospitalRegistrationSystem.Helpers
{
    public static class PasswordHelper
    {
        // Verilən şifrəni SHA256 hash-ə çevirir
        public static string HashPassword(string password)
        {
            // Şifrəni byte massivinə çevir
            var bytes = Encoding.UTF8.GetBytes(password);

            // SHA256 hash hesabla
            var hashBytes = SHA256.HashData(bytes);

            // Hash-i string-ə çevir
            return Convert.ToHexString(hashBytes);
        }
    }
}