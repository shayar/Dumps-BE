using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Dumps.Application.ServicesInterfaces;
using Microsoft.AspNetCore.Http;

namespace Dumps.Infrastructure.Services
{
    public class CloudinaryStorageService : IStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryStorageService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file.Length == 0)
                throw new ArgumentException("File is empty");

            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new RawUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                };

                var result = await _cloudinary.UploadAsync(uploadParams);

                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return result.SecureUrl.AbsoluteUri;  // Return the URL of the uploaded file
                }
                else
                {
                    throw new Exception($"Cloudinary upload failed with status: {result.StatusCode}");
                }

                throw new Exception("File upload failed.");
            }
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            var publicId = GetPublicIdFromUrl(fileUrl);
            var deletionParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deletionParams);

            if (result.Result != "ok")
            {
                throw new Exception("Failed to delete file from Cloudinary.");
            }
        }

        private string GetPublicIdFromUrl(string fileUrl)
        {
            var uri = new Uri(fileUrl);
            var path = uri.AbsolutePath;
            var publicId = Path.GetFileNameWithoutExtension(path);
            return publicId;
        }
    }
}
