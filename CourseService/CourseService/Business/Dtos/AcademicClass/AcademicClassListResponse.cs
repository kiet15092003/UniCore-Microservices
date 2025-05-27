using CourseService.Business.Dtos.Course;

namespace CourseService.Business.Dtos.AcademicClass
{
    public class AcademicClassListResponse
    {
        public List<AcademicClassReadDto> Data { get; set; }
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
    }
}
