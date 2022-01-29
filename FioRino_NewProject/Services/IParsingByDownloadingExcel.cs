using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public interface IParsingByDownloadingExcel
    {
         Task GetZipDownload();
        Task<Responses.Response> ParsingProducts();
        Task GetProductAmount();
        Task ZipStatusCheck(int TotalAmount);
        Task UnzipZip(string ZipPath, int TotalAmount);
        Task<string> ExcelParsingFromMojegs(Stream file, string filePath, int TotalAmount);
    }
}
