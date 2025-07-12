using Microsoft.AspNetCore.Http;

namespace UserService.Business.Dtos.Lecturer
{
    public class UpdateUserImageDto
    {
        public Guid Id { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
} 