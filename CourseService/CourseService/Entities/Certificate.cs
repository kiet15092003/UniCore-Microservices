namespace CourseService.Entities
{
    public class Certificate : BaseEntity
    {
        public string Name { get; set; }
        public int RequiredScore { get; set; }
        public List<CourseCertificate> CourseCertificates { get; set; }
    }
}
