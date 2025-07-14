namespace CourseService.Utils.Constants
{
    public static class AcademicClassStatus
    {
        public const int Pending = 1;
        public const int Approved = 2;
        public const int Started = 3;
        public const int Rejected = 6;
        public const int Ended = 7;

        public static string GetStatusName(int status)
        {
            return status switch
            {
                Pending => "Pending",
                Approved => "Approved",
                Started => "Started",
                Rejected => "Rejected",
                Ended => "Ended",
                _ => "Unknown"
            };
        }
    }
} 