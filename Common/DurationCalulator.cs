namespace E_Commers_Adelia.Common
{
    public static class DurationCalulator
    {
        public static string GetTimeAgo(DateTime createDate)
        {
            var duration = DateTime.UtcNow - createDate;

            if (duration.TotalSeconds < 60)
                return $"{Math.Floor(duration.TotalSeconds)} seconds ago";

            if (duration.TotalMinutes < 60)
                return $"{Math.Floor(duration.TotalMinutes)} minutes ago";

            if (duration.TotalHours < 24)
                return $"{Math.Floor(duration.TotalHours)} hours ago";

            if (duration.TotalDays < 7)
                return $"{Math.Floor(duration.TotalDays)} days ago";

            if (duration.TotalDays < 30)
                return $"{Math.Floor(duration.TotalDays / 7)} weeks ago";

            if (duration.TotalDays < 365)
                return $"{Math.Floor(duration.TotalDays / 30)} months ago";

            return $"{Math.Floor(duration.TotalDays / 365)} years ago";
        }
    }
}
