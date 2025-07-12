using System;

namespace CourseService.Business.Dtos.AcademicClass
{
    public class AcademicClassAnalyticsDto : AcademicClassReadDto
    {
        public int TotalPassed { get; set; }
        public int TotalFailed { get; set; }
        public double AverageScore { get; set; }
    }
} 