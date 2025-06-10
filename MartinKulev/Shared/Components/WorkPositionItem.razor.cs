namespace MartinKulev.Shared.Components
{
    public partial class WorkPositionItem
    {
        private string GetDuration(DateTime startDate, DateTime endDate)
        {
            int totalMonths = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month + 1;
            if (endDate.Day < startDate.Day)
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
    }
}
