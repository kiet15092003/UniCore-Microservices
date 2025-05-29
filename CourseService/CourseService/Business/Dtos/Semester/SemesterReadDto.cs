using System;

namespace CourseService.Business.Dtos.Semester
{
    public class SemesterReadDto
    {
        public Guid Id { get; set; }
        public int SemesterNumber { get; set; }
        public int Year { get; set; }
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfWeeks { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
