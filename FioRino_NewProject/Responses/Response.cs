using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;

namespace FioRino_NewProject.Responses
{
    public class Response
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public Logger logger { get; set; }

    }
    public class Logger
    {
        private readonly IWebHostEnvironment _environment;

        public Logger(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public async Task Logging()
        {
            var webHost = _environment.WebRootPath;
            var FilePath = webHost + "\\Logger";
            if (!Directory.Exists(FilePath))
            {
                System.IO.DirectoryInfo di = new DirectoryInfo($"{FilePath}");
                Directory.CreateDirectory(FilePath);
            }

        }
    }

}
