using System.Collections.Generic;

namespace CourseService.Business.Dtos.AcademicClass
{
    public class AcademicClassAnalyticsListResponse
    {
        public List<AcademicClassAnalyticsDto> Data { get; set; }
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
} 