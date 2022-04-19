using FioRino_NewProject.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace FioRino_NewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MojegsExcelParsing : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ExcelParsingClass _excelParsingClass;
        private readonly ParsingByDownloadingExcel _ParsingByDownloadingExcel;

        public MojegsExcelParsing(IWebHostEnvironment environment, ExcelParsingClass excelParsingClass, ParsingByDownloadingExcel parsingByDownloadingExcel)
        {
            _environment = environment;
            _excelParsingClass = excelParsingClass;
            _ParsingByDownloadingExcel = parsingByDownloadingExcel;
        }
        
        [HttpGet("OpenParser")]
        public async Task<IActionResult> OpenParser()
        {
            var status = await _ParsingByDownloadingExcel.ParsingProducts();
            if (status.Status == "Ok")
            {
                await _ParsingByDownloadingExcel.DownloadZip();
                var rootPath = _environment.WebRootPath;
                var ZipPath = rootPath + "/Zips/";
                //var ZipPath = rootPath + "\\Zips";
                var TotalAmount = _ParsingByDownloadingExcel.GetProductAmount();
                await _ParsingByDownloadingExcel.UnzipZip(ZipPath, TotalAmount);
                string[] XlsxFiles = Directory.GetFiles($"{ZipPath}", "*.xlsx");

                for (int fileLength = 0; fileLength < XlsxFiles.Length; fileLength++)
                {
                    string filePath = XlsxFiles[fileLength];
                    byte[] bytes = System.IO.File.ReadAllBytes(filePath);
                    MemoryStream ms = new MemoryStream(bytes);
                    await _excelParsingClass.ExcelParsingFromMojegs(ms, filePath, TotalAmount);
                }
                return Ok(status.Status);
            }
            else
                return BadRequest(status.Message.ToString());
        } 
    }
}
