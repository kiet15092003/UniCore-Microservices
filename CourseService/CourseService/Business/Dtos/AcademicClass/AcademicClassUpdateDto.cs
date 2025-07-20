namespace CourseService.Business.Dtos.AcademicClass
{
    public class AcademicClassUpdateDto
    {
        public string? Name { get; set; }
        public int? GroupNumber { get; set; }
        public int? Capacity { get; set; }
        public List<int>? ListOfWeeks { get; set; }
        public bool? IsRegistrable { get; set; }
        public int? MinEnrollmentRequired { get; set; }
        // Note: CourseId and SemesterId are not included in updates to prevent breaking relationships
        // These fields should only be set during creation
        public Guid? ParentTheoryAcademicClassId { get; set; }
        public List<ScheduleInDayCreateForClassDto>? ScheduleInDays { get; set; }
    }
} 