using Microsoft.AspNetCore.Http;

namespace FioRino_NewProject.DataTransferOrigins
{
    public class UploadImageDTO
    {
        public IFormFile File { get; set; }
        public int Id { get; set; }
    }
}
