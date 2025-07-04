namespace EnrollmentService.Business.Dtos.Exam
{
    public class ExamGrpcRoomData
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int AvailableSeats { get; set; }
        public Guid FloorId { get; set; }
    }

    public class ExamGrpcAcademicClassData
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GroupNumber { get; set; }
        public int Capacity { get; set; }
        public List<int>? ListOfWeeks { get; set; }
        public bool IsRegistrable { get; set; }
        public Guid SemesterId { get; set; }
        public Guid CourseId { get; set; }    }

    public class ExamReadDto
    {
        public Guid Id { get; set; }
        public int Group { get; set; }
        public int Type { get; set; }
        public DateTime ExamTime { get; set; }
        public int Duration { get; set; }
        public Guid AcademicClassId { get; set; }
        public Guid RoomId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Statistics
        public int TotalEnrollment { get; set; }
        public int TotalPassed { get; set; }
        public int TotalFailed { get; set; }
        public double AverageScore { get; set; }

        // Navigation properties from gRPC calls
        public ExamGrpcRoomData? Room { get; set; }
        public ExamGrpcAcademicClassData? AcademicClass { get; set; }
    }
}
