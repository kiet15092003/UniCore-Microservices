using System.ComponentModel.DataAnnotations;

namespace CourseService.Business.Dtos.Course
{
    public class CourseUpdateDto
    {
        [Required]
        public string Name { get; set; }      
        public string Description { get; set; }      
        public bool IsActive { get; set; }      
        public int Credit { get; set; }        
        public int PracticePeriod { get; set; }       
        public bool IsRequired { get; set; }
        public bool IsOpenForAll { get; set; } = false;
        public int? MinCreditRequired { get; set; }      
        public Guid[]? MajorIds { get; set; }        
        public Guid[]? PreCourseIds { get; set; }        
        public Guid[]? ParallelCourseIds { get; set; }
    }
}