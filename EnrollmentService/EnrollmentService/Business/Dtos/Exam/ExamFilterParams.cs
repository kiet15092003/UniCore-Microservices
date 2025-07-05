namespace EnrollmentService.Business.Dtos.Exam
{
    public class ExamListFilterParams
    {
        public Guid? AcademicClassId { get; set; }
        public Guid? RoomId { get; set; }
        public DateTime? MinExamTime { get; set; }
        public DateTime? MaxExamTime { get; set; }
    }
}
