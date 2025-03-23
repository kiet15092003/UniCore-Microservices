namespace CourseService.Business.Dtos.Course
{
    public class CourseCertificateReadDto
    {
        public Guid CertificateId { get; set; }
        public string Name { get; set; }
        public int RequiredScore { get; set; }
    }
}
