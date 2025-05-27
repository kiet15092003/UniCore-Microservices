using Microsoft.AspNetCore.Http;

namespace CourseService.Utils
{
    public interface ICloudinaryService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task<bool> DeleteFileAsync(string publicId);
    }
} 