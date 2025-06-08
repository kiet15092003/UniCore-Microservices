using System.ComponentModel.DataAnnotations;

namespace UserService.Business.Dtos.Lecturer
{
    public class UpdateUserImageDto
    {
        [Required]
        public Guid Id { get; set; }
        
        [Required]
        public string ImageUrl { get; set; }
    }
} 