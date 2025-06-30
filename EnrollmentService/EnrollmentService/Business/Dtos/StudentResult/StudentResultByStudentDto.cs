namespace EnrollmentService.Business.Dtos.StudentResult
{
    public class StudentResultByStudentDto
    {
        public Guid StudentId { get; set; }
        public string StudentCode { get; set; }
        public string StudentName { get; set; }
        public List<StudentResultDto> Results { get; set; }
    }
} 