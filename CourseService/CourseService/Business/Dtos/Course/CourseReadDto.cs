using CourseService.Entities;

namespace CourseService.Business.Dtos.Course
{
    public class CourseReadDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public bool IsOpening { get; set; }
        public int Credit { get; set; }
        public bool IsHavePracticeClass { get; set; }
        public bool IsUseForCalculateScore { get; set; }
        public int? MinCreditCanApply { get; set; }
        public Guid? MajorId { get; set; }
        public Guid? CompulsoryCourseId { get; set; }
        public Guid? ParallelCourseId { get; set; }

        public List<CourseCertificateReadDto> CourseCertificates { get; set; }
        public List<CourseMaterialReadDto> CourseMaterials { get; set; }
    }
}
