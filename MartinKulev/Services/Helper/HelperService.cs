using System.Globalization;

namespace MartinKulev.Services.Helper
{
    public static class HelperService
    {
        public static string GetDurationToString(DateTime startDate, DateTime? endDate)
        {
            if(endDate == null)
            {
                endDate = DateTime.Now;
            }

            int totalMonths = ((endDate.Value.Year - startDate.Year) * 12) + endDate.Value.Month - startDate.Month + 1;
            if (endDate.Value.Day < startDate.Day)
            {
                totalMonths--; // Adjust for partial month
            }

            int years = totalMonths / 12;
            int months = totalMonths % 12;

            var parts = new List<string>();
            if (years > 0) parts.Add($"{years}y");
            if (months > 0) parts.Add($"{months}m");

            return $"({string.Join(", ", parts)})";
        }

        public static string GetEndDateToString(DateTime? endDate)
        {
            if (endDate == null)
            {
                return "Present";
            }

            return endDate.Value.ToString("MMM yyyy", CultureInfo.InvariantCulture);
        }

        public static string FormatTimeAgo(TimeSpan ts)
        {
            if (ts.TotalSeconds < 60)
                return $"{(int)ts.TotalSeconds}s ago";
            if (ts.TotalMinutes < 60)
                return $"{(int)ts.TotalMinutes}m ago";
            if (ts.TotalHours < 24)
                return $"{(int)ts.TotalHours}h ago";
            return $"{(int)ts.TotalDays}d ago";
        }
    }
}
