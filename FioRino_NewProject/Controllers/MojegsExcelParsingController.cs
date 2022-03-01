using DocumentFormat.OpenXml.Spreadsheet;
using FioRino_NewProject.Data;
using FioRino_NewProject.Entities;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using FioRino_NewProject.Services;
using MailChimp.Net.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Response = FioRino_NewProject.Responses.Response;

namespace FioRino_NewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MojegsExcelParsing : ControllerBase
    {
        private readonly FioRinoBaseContext _context;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUniqueProductsRepository _uniqueProductRepository;
        private readonly ISizeRepository _sizeRepository;
        private readonly ISkuRepository _skuRepository;
        private readonly IProductService _pService;
        private readonly IWebHostEnvironment _environment;
        private readonly ExcelParsingClass _excelParsingClass;
        private readonly IStatusRepository _statusRepository;
        private readonly ParsingByDownloadingExcel _ParsingByDownloadingExcel;

        public MojegsExcelParsing(FioRinoBaseContext context, ICategoryRepository categoryRepository, IProductRepository productRepository, IUniqueProductsRepository uniqueProductRepository, ISizeRepository sizeRepository, ISkuRepository skuRepository, IProductService pService, IWebHostEnvironment environment, ExcelParsingClass excelParsingClass, IStatusRepository statusRepository, ParsingByDownloadingExcel parsingByDownloadingExcel)
        {
            _context = context;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _uniqueProductRepository = uniqueProductRepository;
            _sizeRepository = sizeRepository;
            _skuRepository = skuRepository;
            _pService = pService;
            _environment = environment;
            _excelParsingClass = excelParsingClass;
            _statusRepository = statusRepository;
            _ParsingByDownloadingExcel = parsingByDownloadingExcel;
        }
       

        #region
        //[HttpPost("GetZipDownload")]
        //public async Task GetZipDownload()
        //{
        //    var rootPath = _environment.WebRootPath;
        //    var zipDirectory = rootPath + "/Zips/";

        //    if (Directory.Exists(zipDirectory))
        //    {
        //        System.IO.DirectoryInfo di = new DirectoryInfo($"{zipDirectory}");
        //        foreach (FileInfo file in di.GetFiles())
        //        {
        //            file.Delete();
        //        }
        //    }
        //    else
        //        Directory.CreateDirectory(zipDirectory);
        //    string[] DownloadCheck = Directory.GetFiles($"{zipDirectory}", "*.zip");
        //    ChromeOptions options = new ChromeOptions();
        //    options.AddArguments("--headless");
        //    options.AddArgument("test-type");
        //    options.AddArgument("no-sandbox");
        //    //if (!Directory.Exists(zipDirectory))
        //    //    Directory.CreateDirectory(zipDirectory);
        //    options.AddUserProfilePreference("download.default_directory", zipDirectory);
        //    options.AddUserProfilePreference("download.prompt_for_download", false);
        //    options.AddUserProfilePreference("disable-popup-blocking", "true");

        //    WebDriver driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));
        //    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120);
        //    driver.Navigate().GoToUrl("https://mojegs1.pl/logowanie");
        //    IWebElement element = driver.FindElement(By.Name("email"));
        //    element.SendKeys("tomek@fiorino.eu");
        //    element = driver.FindElement(By.Name("password"));
        //    element.SendKeys("Epoka1-wsx");
        //    element.Submit();
        //    driver.Navigate().GoToUrl("https://mojegs1.pl/moje-produkty");

        //    element = driver.FindElement(By.Id("productsListExportBtn"));
        //    element.Click();
        //    element = driver.FindElement(By.XPath("//*[@id='exportProductsForm']/div[2]/div/div/div[3]/label"));
        //    await Task.Delay(1000);
        //    element.Click();

        //    element = driver.FindElement(By.Id("productsListExportModalAcceptBtn"));
        //    element.Click();

        //    #region
        //    ////driver.Navigate().GoToUrl("chrome:downloads");
        //    int num = 0;

        //    for (int i = 0; ; i++)
        //    {
        //        string[] DownloadCheckFormat = Directory.GetFiles($"{zipDirectory}", "*.zip");
        //        try
        //        {
        //            if (DownloadCheckFormat[0] == DownloadCheckFormat[num])
        //            {
        //                driver.Close();
        //                driver.Dispose();
        //            }
        //        }
        //        catch (Exception)
        //        {
        //            await Task.Delay(1000);
        //        }
        //    }
        //}
        #endregion
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
                return BadRequest(status.Status);
            }
            else
                return BadRequest(status.Message.ToString());
            
        }

        
    }
    public class ExcelParsingClass
    {
        private readonly FioRinoBaseContext _context;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUniqueProductsRepository _uniqueProductRepository;
        private readonly ISizeRepository _sizeRepository;
        private readonly ISkuRepository _skuRepository;
        private readonly IProductService _pService;
        private readonly IWebHostEnvironment _environment;
        private readonly IStatusRepository _statusRepository;

        public ExcelParsingClass(FioRinoBaseContext context, ICategoryRepository categoryRepository, IProductRepository productRepository, IUniqueProductsRepository uniqueProductRepository, ISizeRepository sizeRepository, ISkuRepository skuRepository, IProductService pService, IWebHostEnvironment environment, IStatusRepository statusRepository)
        {
            _context = context;
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _uniqueProductRepository = uniqueProductRepository;
            _sizeRepository = sizeRepository;
            _skuRepository = skuRepository;
            _pService = pService;
            _environment = environment;
            _statusRepository = statusRepository;
        }
        int CurrentAmount = 1;
        public async Task<string> ExcelParsingFromMojegs(Stream file,string filePath, int TotalAmount)
        {
            var categoryList = await _categoryRepository.CreateCategoryWithListReturn();
            var brak = categoryList[0];
            var classicCategory = categoryList[1];
            var fasterCategory = categoryList[2];
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowcount = worksheet.Dimension.Rows;
                    await _categoryRepository.CreateCategory();

                    var statusForLoading = await _statusRepository.GetFirst();
                    statusForLoading.Status = "LOADING";
                    await _context.SaveChangesAsync();
                    for (int row = 3; row < rowcount - 1; row++)
                    {
                        var originalEntity = await _statusRepository.OriginalValues();
                        var LoadingStatus = await _statusRepository.CreateStatusIfNull(CurrentAmount, TotalAmount);
                        CurrentAmount++;
                        if (originalEntity.Status == "STOPPED")
                        {
                            var updating = await _statusRepository.GetFirst();
                            updating.CurrentAmount = 0;
                            //updating.PocessIsKilled = false;
                            updating.TotalAmount = 0;
                            updating.Status = originalEntity.Status;
                            await _context.SaveChangesAsync();
                            var rootPath = _environment.WebRootPath;
                            //var zipDirectory = rootPath + "\\Zips";
                            var zipDirectory = rootPath + "/Zips/";
                            if (Directory.Exists(zipDirectory))
                            {
                                System.IO.DirectoryInfo di = new DirectoryInfo($"{zipDirectory}");
                                foreach (FileInfo files in di.GetFiles())    
                                {
                                    files.Delete();
                                }
                            }
                            return "STOPPED";
                        }
                        if (CurrentAmount == TotalAmount)
                        {
                            var statusSelect = await _statusRepository.GetFirst();
                            statusSelect.Status = "SUCCESS";
                            statusSelect.CurrentAmount = 0;
                            statusSelect.TotalAmount = 0;
                            statusSelect.SuccessDate = DateTime.Now;
                            await _context.SaveChangesAsync();
                        }
                        var ProductFullName = worksheet.Cells[row, 2].Value;
                        if (ProductFullName == null)
                            return "OK";
                        DmCategory category = new DmCategory();
                        DmSize size = new DmSize();
                        string sizeName = string.Empty;
                        var isClassicCategory = worksheet.Cells[row, 2].Value == null ? false :
                                                worksheet.Cells[row, 2].Value.ToString().ToLower().Contains("classic") ? true : false;
                        var isFasterCategory = worksheet.Cells[row, 2].Value == null ? false :
                                                worksheet.Cells[row, 2].Value.ToString().ToLower().Contains("faster") ? true : false;
                        var Brak = isClassicCategory == false && isFasterCategory == false;

                        var productName = isClassicCategory ? worksheet.Cells[row, 2].Value.ToString().ToLower().Replace("classic", " ").Trim() :
                                      isFasterCategory ? worksheet.Cells[row, 2].Value.ToString().ToLower().Replace("faster", " ").Trim() :
                                      worksheet.Cells[row, 2].Value.ToString().ToLower().Trim();

                        var ProdName = productName.ToString().Contains("rozm.") ?
                                         productName.ToString().Replace("rozm.", " ") :
                                            productName.ToString().Replace("r.", "");
                        var ProdNameWithUpperSlash = ProdName.Split(" ").Last().Contains("-");

                        string output;

                        if (!ProdName.Contains("cm") && !ProdName.Contains("MET") && !ProdNameWithUpperSlash == true)
                        {
                            output = Regex.Replace(ProdName, @"[\0-9]", " ");
                        }
                        else
                        {
                            output = ProdName.ToString().Trim();
                        }

                        var categoryId = (isClassicCategory ? classicCategory.Id :
                            (isFasterCategory ? fasterCategory.Id : brak.Id));
                        var MatchingProducts = output.Replace("    ", "");
                        string FindSizeAlphabet;
                        int skuCodeId;
                        //var containProduct = await _uniqueProductRepository.FindUniqueProductByName(MatchingProducts);
                        if (worksheet.Cells[row, 22].Value != null)
                        {
                            var SKUString = worksheet.Cells[row, 22].Value.ToString();
                            skuCodeId = await _skuRepository.InsertingSkuIFNull(SKUString);
                        }
                        else
                            skuCodeId = 0;
                        int ProductUniqueId = await _uniqueProductRepository.InsertUniqueProductIfNull(MatchingProducts, skuCodeId);
                        var splitLastIndex = ProductFullName.ToString().Split(" ").Last();
                        if (MatchingProducts.Split(" ").Last() == "M" ||
                            MatchingProducts.Split(" ").Last() == "S" ||
                            MatchingProducts.Split(" ").Last() == "L" ||
                           MatchingProducts.Split(" ").Last() == "XL" ||
                           MatchingProducts.Split(" ").Last() == "XS" ||
                           MatchingProducts.Split(" ").Last() == "2XL")
                        {
                            FindSizeAlphabet = MatchingProducts.Split(" ").Last();
                            var SizeAlphabet = MatchingProducts.LastIndexOf(" ");
                            if (SizeAlphabet > 0)
                                MatchingProducts = MatchingProducts.Substring(0, SizeAlphabet);
                        }
                        else
                        {
                            FindSizeAlphabet = null;
                        }
                        int SizeNum = 0;
                        if (!splitLastIndex.Contains("cm") && !splitLastIndex.Contains("-"))
                        {
                            var resultString = Regex.Match(splitLastIndex, @"\d+").Value;
                            Int32.TryParse(resultString, out SizeNum);
                        }
                        var findSize = await _sizeRepository.FindSizeByNumber(SizeNum);
                        var sizeId = await _sizeRepository.CreateSizeIfNull(findSize, SizeNum, FindSizeAlphabet);
                        //if (worksheet.Cells[row, 22].Value != null)
                        //{
                        //    var SKUString = worksheet.Cells[row, 22].Value.ToString();
                        //    var skuCodeId = await _skuRepository.InsertingSkuIFNull(SKUString);
                        //}
                        var GTIN = worksheet.Cells[row, 3].Value.ToString();

                        
                        
                        var AddProd = await _pService.InsertDmProduct(MatchingProducts, categoryId, GTIN, ProductUniqueId, sizeId);
                        System.IO.File.Delete(filePath);
                    }
                        return "ok Finish";
                }
            }
        }
    }
}
