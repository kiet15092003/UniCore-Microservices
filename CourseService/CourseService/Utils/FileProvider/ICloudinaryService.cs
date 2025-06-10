using Microsoft.AspNetCore.Http;
using System.IO;

namespace CourseService.Utils
{
    public interface ICloudinaryService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task<bool> DeleteFileAsync(string publicId);
        Task<byte[]> DownloadFileAsync(string fileUrl);
        Task<Stream> GetFileStreamAsync(string fileUrl);
    }
} 