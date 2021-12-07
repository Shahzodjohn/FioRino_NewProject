using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.DataTransferOrigins
{
    public class UploadImageDTO
    {
        public IFormFile File { get; set; }
        public int Id { get; set; }
    }
}
