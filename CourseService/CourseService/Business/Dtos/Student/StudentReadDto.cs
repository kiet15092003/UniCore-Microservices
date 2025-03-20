namespace CourseService.Business.Dtos.Student
{
    public class StudentReadDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string StudentCode { get; set; }
        public string FullName { get; set; }
    }
}
