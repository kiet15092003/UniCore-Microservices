namespace EnrollmentService.Business.Dtos.Enrollment
{
    public class EnrollmentReadDto
    {
        public Guid Id { get; set; }
        public int Status { get; set; }
        public Guid StudentId { get; set; }
        public Guid AcademicClassId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Related data from gRPC services
        public GrpcStudentData? Student { get; set; }
        public GrpcAcademicClassData? AcademicClass { get; set; }
    }

    // Mapped from gRPC StudentData
    public class GrpcStudentData
    {
        public Guid Id { get; set; }
        public string? StudentCode { get; set; }
        public int AccumulateCredits { get; set; }
        public double AccumulateScore { get; set; }
        public int AccumulateActivityScore { get; set; }
        public Guid MajorId { get; set; }
        public Guid BatchId { get; set; }
        public Guid ApplicationUserId { get; set; }
        public GrpcUserData? User { get; set; }
    }

    // Mapped from gRPC UserData
    public class GrpcUserData
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName => $"{FirstName} {LastName}";
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PersonId { get; set; }
        public string? ImageUrl { get; set; }
    }

    // Mapped from gRPC AcademicClassData
    public class GrpcAcademicClassData
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int GroupNumber { get; set; }
        public int Capacity { get; set; }
        public List<int>? ListOfWeeks { get; set; }
        public bool IsRegistrable { get; set; }
        public Guid SemesterId { get; set; }
        public GrpcSemesterData? Semester { get; set; }
        public Guid CourseId { get; set; }
        public GrpcCourseData? Course { get; set; }
        public List<GrpcScheduleInDayData>? ScheduleInDays { get; set; }
    }

    // Mapped from gRPC SemesterData
    public class GrpcSemesterData
    {
        public Guid Id { get; set; }
        public int SemesterNumber { get; set; }
        public int Year { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfWeeks { get; set; }
    }

    // Mapped from gRPC CourseData
    public class GrpcCourseData
    {
        public Guid Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int Credit { get; set; }
        public int PracticePeriod { get; set; }
        public bool IsRequired { get; set; }
        public double Cost { get; set; }
    }

    // Mapped from gRPC ScheduleInDayData
    public class GrpcScheduleInDayData
    {
        public Guid Id { get; set; }
        public string? DayOfWeek { get; set; }
        public Guid RoomId { get; set; }
        public GrpcRoomData? Room { get; set; }
        public Guid ShiftId { get; set; }
        public GrpcShiftData? Shift { get; set; }
    }

    // Mapped from gRPC RoomData
    public class GrpcRoomData
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public int Capacity { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    // Mapped from gRPC ShiftData
    public class GrpcShiftData
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
    }
}
