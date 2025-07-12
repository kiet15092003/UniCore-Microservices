using System;

namespace CourseService.Business.Dtos.AcademicClass
{
    public class AcademicClassAnalyticsSummaryResponse
    {
        public int TotalEnrollment { get; set; }
        public int TotalPassed { get; set; }
        public int TotalFailed { get; set; }
        public double TotalAverageScore { get; set; }
    }
} 