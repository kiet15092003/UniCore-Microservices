namespace EnrollmentService.Entities
{
    public class Exam : BaseEntity
    {
        public int Group { get; set; }
        public int Type { get; set; }
        public DateTime ExamTime { get; set; }
        public int Duration { get; set; }
        public Guid AcademicClassId { get; set; }
        public Guid RoomId { get; set; }
        public List<EnrollmentExam> EnrollmentExams { get; set; } = new List<EnrollmentExam>();
    }
}
