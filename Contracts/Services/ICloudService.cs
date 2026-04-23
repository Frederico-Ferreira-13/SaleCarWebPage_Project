using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Contracts.Services
{
    public interface ICloudService
    {
        Task<string> UploadImageAsync(IFormFile? imageFile);
    }
}
