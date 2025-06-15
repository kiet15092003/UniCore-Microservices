namespace CourseService.Entities
{
    public class AcademicClass : BaseEntity
    {
        public string Name { get; set; }
        public int GroupNumber { get; set; }
        public int Capacity { get; set; }          
        public List<int> ListOfWeeks { get; set; }        
        public bool IsRegistrable { get; set; }
        public int MinEnrollmentRequired { get; set; }
        public DateTime? RegistrationOpenTime { get; set; }
        public DateTime? RegistrationCloseTime { get; set; }
        public Guid SemesterId { get; set; }
        public Semester Semester { get; set; }
        public Guid CourseId { get; set; }
        public Course Course { get; set; }
        public Guid? ParentTheoryAcademicClassId { get; set; }
        public AcademicClass? ParentTheoryAcademicClass { get; set; }
        public List<Guid> ChildPracticeAcademicClassIds { get; set; } = new List<Guid>();
        public List<AcademicClass> ChildPracticeAcademicClasses { get; set; } = new List<AcademicClass>();
        public List<ScheduleInDay> ScheduleInDays { get; set; } = new List<ScheduleInDay>();
    }
}
