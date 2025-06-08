using CourseService.Business.Dtos.Course;

namespace CourseService.Business.Dtos.CoursesGroup
{
    public class CoursesGroupReadDto
    {
        public Guid Id { get; set; }
        public required string GroupName { get; set; }
        public Guid? MajorId { get; set; }
        public int Credit { get; set; }
        public List<Guid> CourseIds { get; set; } = new List<Guid>();     
        public List<CourseReadDto>? Courses { get; set; }
    }
}