using System.Text.RegularExpressions;

namespace HospitalRegistrationSystem.Helpers
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string text)
        {
            // Mətn boşdursa boş string qaytar
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Kiçik hərflərə çevir
            text = text.ToLower();

            // Boşluqları "-" ilə əvəz et
            text = Regex.Replace(text, @"\s+", "-");

            // İngilis əlifbası, rəqəm və "-" xaric hər şeyi sil
            text = Regex.Replace(text, @"[^a-z0-9\-]", "");

            // Birdən çox "-" varsa bir ədədə endir
            text = Regex.Replace(text, @"-+", "-");

            // Baş və sondakı "-" işarələrini sil
            text = text.Trim('-');

            return text;
        }
    }
}