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

                throw new Exception("File upload failed.");
            }
        }
    }
}
