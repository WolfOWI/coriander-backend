using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CoriCore.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
