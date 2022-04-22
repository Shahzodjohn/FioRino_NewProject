using FioRino_NewProject.Data;
using FioRino_NewProject.Entities;
using FioRino_NewProject.Repositories;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public class ExcelParsingClass
    {
        private readonly FioRinoBaseContext _context;
        private readonly ICategoryRepository _categoryRepository;
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
            _uniqueProductRepository = uniqueProductRepository;
            _sizeRepository = sizeRepository;
            _skuRepository = skuRepository;
            _pService = pService;
            _environment = environment;
            _statusRepository = statusRepository;
        }
        int CurrentAmount = 1;
        public async Task<string> ExcelParsingFromMojegs(Stream file, string filePath, int TotalAmount)
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
                        if (worksheet.Cells[row, 22].Value != null)
                        {
                            var SKUString = worksheet.Cells[row, 22].Value.ToString();
                            skuCodeId = await _skuRepository.InsertingSkuIFNull(SKUString);
                        }
                        else
                            skuCodeId = 0;
                        int ProductUniqueId = await _uniqueProductRepository.InsertUniqueProductIfNull(MatchingProducts.Trim(), skuCodeId);
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
