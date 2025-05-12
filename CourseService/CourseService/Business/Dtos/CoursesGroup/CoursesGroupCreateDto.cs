using System.ComponentModel.DataAnnotations;

namespace CourseService.Business.Dtos.CoursesGroup
{
    public class CoursesGroupCreateDto
    {
        [Required]
        public string GroupName { get; set; }
        
        [Required]
        public Guid MajorId { get; set; }
        
        // List of course IDs to associate with this group
        public List<Guid> CourseIds { get; set; } = new List<Guid>();
    }
}