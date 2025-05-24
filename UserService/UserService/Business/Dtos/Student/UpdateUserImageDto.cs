using Microsoft.AspNetCore.Http;

namespace UserService.Business.Dtos.Student
{
    public class UpdateUserImageDto
    {
        public Guid StudentId { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
