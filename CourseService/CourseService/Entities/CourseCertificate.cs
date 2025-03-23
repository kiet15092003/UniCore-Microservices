namespace CourseService.Entities
{
    public class CourseCertificate : BaseEntity
    {
        public Guid CourseId { get; set; }
        public Course Course { get; set; }

        public Guid CertificateId { get; set; }
        public Certificate Certificate { get; set; }
    }
}
