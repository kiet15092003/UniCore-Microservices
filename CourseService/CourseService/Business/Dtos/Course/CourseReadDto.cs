using CourseService.Entities;

namespace CourseService.Business.Dtos.Course
{
    public class CourseReadDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Cost { get; set; }
        public bool IsRegistrable { get; set; }
        public int Credit { get; set; }
        public int PracticePeriod { get; set; }
        public bool IsRequired { get; set; }
        public int? MinCreditRequired { get; set; }
        public Guid? MajorId { get; set; }
        public Guid[]? PreCourseIds { get; set; }
        public Guid[]? ParallelCourseIds { get; set; }

        public List<CourseCertificateReadDto> CourseCertificates { get; set; }
        public List<CourseMaterialReadDto> CourseMaterials { get; set; }
    }
}
