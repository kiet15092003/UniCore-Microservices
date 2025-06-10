using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net.Http;

namespace CourseService.Utils
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly HttpClient _httpClient;

        public CloudinaryService(IConfiguration configuration)
        {
            var cloudinarySettings = configuration.GetSection("Cloudinary");
            var account = new Account(
                cloudinarySettings["CloudName"],
                cloudinarySettings["ApiKey"],
                cloudinarySettings["ApiSecret"]
            );
            _cloudinary = new Cloudinary(account);
            _httpClient = new HttpClient();
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            using var stream = file.OpenReadStream();
            
            // Determine file type and set appropriate folder
            string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string resourceType = DetermineResourceType(fileExtension);
            
            // Create appropriate upload parameters based on resource type
            if (resourceType == "image")
            {
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = true,
                    Folder = "assets"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.ToString();
            }
            else
            {
                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = true,
                    Folder = "assets"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                return uploadResult.SecureUrl.ToString();
            }
        }

        public async Task<byte[]> DownloadFileAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return null;

            try
            {
                // Download file from Cloudinary URL
                var response = await _httpClient.GetAsync(fileUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<Stream> GetFileStreamAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
                return null;

            try
            {
                // Get file as a stream
                var response = await _httpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStreamAsync();
                }
                
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string DetermineResourceType(string fileExtension)
        {
            switch (fileExtension)
            {
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                case ".bmp":
                case ".webp":
                    return "image";
                case ".mp4":
                case ".avi":
                case ".mov":
                case ".wmv":
                    return "video";
                default:
                    return "raw";
            }
        }

        public async Task<bool> DeleteFileAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
                return false;

            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok";
        }
    }
} 