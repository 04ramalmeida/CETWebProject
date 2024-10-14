using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace CETWebProject.Helpers
{
    public interface IImageHelper
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string folder);
    }
}
