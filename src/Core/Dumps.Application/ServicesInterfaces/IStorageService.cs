using Microsoft.AspNetCore.Http;

namespace Dumps.Application.ServicesInterfaces
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task DeleteFileAsync(string fileUrl);
    }
}
