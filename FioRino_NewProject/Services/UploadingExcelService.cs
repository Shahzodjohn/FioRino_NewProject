
using FioRino_NewProject.Entities;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public class UploadingExcelService
    {
        private readonly IUserRepository _repository;
        private readonly ISaveRepository _Save;
        private readonly IOrderRepository _OrderRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IOrderProductsRepository _OrderProductRepository;

        public UploadingExcelService(IUserRepository repository, ISaveRepository save, IOrderRepository orderRepository, ICategoryRepository categoryRepository, IOrderProductsRepository orderProductRepository)
        {
            _repository = repository;
            _Save = save;
            _OrderRepository = orderRepository;
            _categoryRepository = categoryRepository;
            _OrderProductRepository = orderProductRepository;
        }
        public async Task<Response> CreateOrder(IFormFile file, int Id)
        {
            try
            {
                var currentUser = await _repository.GetUser(Id);
                var list = new List<DmProduct>();
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                        if (worksheet.Cells[1, 4].Value == null || worksheet.Cells[1, 4].Value.ToString() != "FIORINO Izabela Gądek-Pagacz")
                        {
                            new Response { Status = "Error", Message = "File ERROR! Please choose the correct file" };
                        }
                        var rowcount = worksheet.Dimension.Rows;
                        await _categoryRepository.CreateCategory();
                        var indexColumn = 3;
                        var OrderId = await _OrderRepository.CreateNewOrder(Id);
                        for (int row = 10; row < rowcount - 1; row += 2)
                        {
                            DmCategory category = new DmCategory();
                            DmSize size = new DmSize();
                            string sizeName = string.Empty;
                            int productAmount = 0;
                            var isClassicCategory = worksheet.Cells[row, indexColumn].Value == null ? false :
                                                    worksheet.Cells[row, indexColumn].Value.ToString().ToLower() == "classic" ? true : false;
                            var tempColumn = indexColumn + 1;
                            int cicleRound = 10;
                            var amount = 0;
                            while (true)
                            {
                                var ExAmount = worksheet.Cells[cicleRound, 19].Value.ToString().Trim();
                                Int32.TryParse(ExAmount.ToString(), out amount);

                                if (amount > 1)
                                {
                                    await _OrderRepository.UpdateOrder(amount, OrderId);
                                    break;
                                }
                                cicleRound++;
                            };
                            var indexing = 4;
                            while (true)
                            {
                                sizeName = (worksheet.Cells[9, indexing].Value == null ? "" : worksheet.Cells[9, indexing].Value.ToString());
                                var sizeNumber = (worksheet.Cells[9, indexing].Value == null ? "" : worksheet.Cells[9, indexing].Value.ToString());
                                if (sizeName.Contains(" "))
                                {
                                    var sizeTrim = sizeName.LastIndexOf(" ");
                                    if (sizeTrim > 0)
                                        sizeName = sizeName.Substring(0, sizeTrim);
                                }
                                sizeNumber = sizeNumber.Remove(0, sizeNumber.IndexOf(' ') + 1).Replace(" ", "");
                                if (sizeNumber.Contains("/"))
                                {
                                    var sizeTrim = sizeNumber.LastIndexOf("/");
                                    if (sizeTrim > 0)
                                        sizeNumber = sizeNumber.Substring(0, sizeTrim);
                                }
                                var sizeNum = 0;
                                Int32.TryParse(sizeNumber, out sizeNum);
                                sizeName = sizeName.Replace(" ", "");
                                var findSizes = await _OrderRepository.FindSize(sizeName);
                                var sizeList = worksheet.Cells[9, indexing].Value.ToString();
                                if (sizeList.Contains("RAZEM"))
                                {
                                    break;
                                }
                                indexing++;
                            }
                            while (isClassicCategory && tempColumn < 19)
                            {
                                var cell = worksheet.Cells[row, tempColumn].Value;
                                if (cell != null)
                                {
                                    Int32.TryParse(cell.ToString(), out productAmount);
                                    category = await _OrderRepository.ClassicCategory();
                                    sizeName = (worksheet.Cells[9, tempColumn].Value == null ? "" : worksheet.Cells[9, tempColumn].Value.ToString());
                                    var sizeNumber = (worksheet.Cells[9, tempColumn].Value == null ? "" : worksheet.Cells[9, tempColumn].Value.ToString());
                                    if (sizeName.Contains(" "))
                                    {
                                        var sizeTrim = sizeName.LastIndexOf(" ");
                                        if (sizeTrim > 0)
                                            sizeName = sizeName.Substring(0, sizeTrim);
                                    }
                                    sizeNumber = sizeNumber.Remove(0, sizeNumber.IndexOf(' ') + 1).Replace(" ", "");

                                    if (sizeNumber.Contains("/"))
                                    {
                                        var sizeTrim = sizeNumber.LastIndexOf("/");
                                        if (sizeTrim > 0)
                                            sizeNumber = sizeNumber.Substring(0, sizeTrim);
                                    }
                                    var sizeNum = 0;
                                    Int32.TryParse(sizeNumber, out sizeNum);
                                    sizeName = sizeName.Replace(" ", "");
                                    var findSizes = await _OrderRepository.FindSize(sizeName);
                                    int SizeId = 0;
                                    if (findSizes == null)
                                    {
                                        SizeId = await _OrderRepository.InsertSize(sizeNum, sizeName);
                                    }
                                    else
                                    {
                                        SizeId = findSizes.Id;
                                    }
                                    tempColumn++;
                                    var productName = worksheet.Cells[row, 2].Value.ToString().Trim();
                                    var GtinPaging = worksheet.Cells[row, 20].Value == null ? "" : worksheet.Cells[row, 20].Value.ToString().Trim();
                                    var findStan = await _OrderRepository.FindStorage(GtinPaging);
                                    var findProduct = await _OrderRepository.FindDmProduct(GtinPaging);
                                    if (findProduct == null)
                                    {
                                        await _OrderRepository.RemoveOrder(OrderId);
                                        return new Response { Status = "Error!", Message = $"GTIN {GtinPaging} not found!" };
                                    }
                                    var SkuNumber = worksheet.Cells[row, 1].Value.ToString().Trim();
                                    var SkuId = await _OrderRepository.InsertSKUcodes(SkuNumber);
                                    var CheckingProductValidation = await _OrderRepository.CheckingProductValidation(productName, category.Id, size.Id);

                                    var productNameworksheet = worksheet.Cells[row, 2].Value.ToString().Trim();
                                    var uniqueproductId = await _OrderRepository.CreateUniqueProduct(productNameworksheet);
                                    var ProductId = 0;
                                    if (CheckingProductValidation == null && GtinPaging != "")
                                    {
                                        ProductId = await _OrderRepository.InsertProductsToDmProducts(findProduct.ProductName, uniqueproductId, category.Id, SizeId, GtinPaging);
                                    }
                                    else
                                    {
                                        CheckingProductValidation.Gtin = GtinPaging;
                                        ProductId = CheckingProductValidation.Id;
                                        await _Save.SaveAsync();
                                    }
                                    if (GtinPaging != "")
                                    {
                                        var insert = await _OrderProductRepository.InsertProductsToOrderProducts(OrderId, findProduct.Id, SizeId, SkuId, category.Id, productAmount, GtinPaging);
                                        if (findStan != null && insert.Amount <= findStan.AmountLeft)
                                        {
                                            findStan.AmountLeft = findStan.AmountLeft - insert.Amount;
                                            insert.ProductStatusesId = 2;
                                            await _Save.SaveAsync();
                                        }
                                    }
                                }
                                if (cell == null)
                                    tempColumn++;
                                if (tempColumn == 19 && cell == null)
                                    isClassicCategory = false;
                            }
                            if (!isClassicCategory)
                            {
                                var isFasterCategory = worksheet.Cells[row + 1, indexColumn].Value == null ? false :
                                                   worksheet.Cells[row + 1, indexColumn].Value.ToString().ToLower() == "faster" ? true : false;
                                category = await _OrderRepository.FasterCategory();
                                tempColumn = indexColumn + 1;
                                while (isFasterCategory && tempColumn < 19)
                                {
                                    var cell = worksheet.Cells[row + 1, tempColumn].Value;
                                    if (cell != null)
                                    {
                                        Int32.TryParse(cell.ToString(), out productAmount);
                                        sizeName = (worksheet.Cells[9, tempColumn].Value == null ? "" : worksheet.Cells[9, tempColumn].Value.ToString());
                                        var sizeNumber = (worksheet.Cells[9, tempColumn].Value == null ? "" : worksheet.Cells[9, tempColumn].Value.ToString());
                                        if (sizeName.Contains(" "))
                                        {
                                            var sizeTrim = sizeName.LastIndexOf(" ");
                                            if (sizeTrim > 0)
                                                sizeName = sizeName.Substring(0, sizeTrim);
                                        }
                                        sizeNumber = sizeNumber.Remove(0, sizeNumber.IndexOf(' ') + 1).Replace(" ", "");

                                        if (sizeNumber.Contains("/"))
                                        {
                                            var sizeTrim = sizeNumber.LastIndexOf("/");
                                            if (sizeTrim > 0)
                                                sizeNumber = sizeNumber.Substring(0, sizeTrim);
                                        }
                                        var sizeNum = 0;
                                        Int32.TryParse(sizeNumber, out sizeNum);
                                        sizeName = sizeName.Replace(" ", "");
                                        var findSizes = await _OrderRepository.FindSize(sizeName);
                                        int SizeId = 0;
                                        if (findSizes == null)
                                        {
                                            SizeId = await _OrderRepository.InsertSize(sizeNum, sizeName);
                                        }
                                        else
                                        {
                                            SizeId = findSizes.Id;
                                        }
                                        tempColumn++;
                                        var productName = worksheet.Cells[row, 2].Value.ToString().Trim();
                                        var GtinPage = worksheet.Cells[row + 1, 20].Value == null ? "" : worksheet.Cells[row + 1, 20].Value.ToString().Trim();
                                        var findStan = await _OrderRepository.FindStorage(GtinPage);
                                        var findProduct = await _OrderRepository.FindDmProduct(GtinPage);
                                        if (findProduct == null)
                                        {
                                            await _OrderRepository.RemoveOrder(OrderId);
                                            return new Response { Status = "Error!", Message = $"GTIN {GtinPage} not found!" };
                                        }
                                        var SkuNumber = worksheet.Cells[row, 1].Value.ToString().Trim();
                                        var SkuId = await _OrderRepository.InsertSKUcodes(SkuNumber);
                                        var CheckingProductValidation = await _OrderRepository.CheckingProductValidation(productName, category.Id, size.Id);
                                        var productNameworksheet = worksheet.Cells[row, 2].Value.ToString().Trim();
                                        var uniquproductFind = await _OrderRepository.CreateUniqueProduct(productNameworksheet);
                                        var ProductId = 0;
                                        if (CheckingProductValidation == null && GtinPage != "")
                                        {
                                            ProductId = await _OrderRepository.InsertProductsToDmProducts(findProduct.ProductName, uniquproductFind, category.Id, SizeId, GtinPage);
                                        }
                                        else
                                        {
                                            CheckingProductValidation.Gtin = GtinPage;
                                            ProductId = CheckingProductValidation.Id;
                                            await _Save.SaveAsync();
                                        }
                                        SkuId = await _OrderRepository.InsertSKUcodes(SkuNumber);
                                        if (GtinPage != "")
                                        {
                                            var insert = await _OrderProductRepository.InsertProductsToOrderProducts(OrderId, findProduct.Id, SizeId, SkuId, category.Id, productAmount, GtinPage);
                                            if (findStan != null && insert.Amount <= findStan.AmountLeft)
                                            {
                                                findStan.AmountLeft = findStan.AmountLeft - insert.Amount;
                                                insert.ProductStatusesId = 2;
                                                await _Save.SaveAsync();
                                            }
                                        }
                                    }
                                    if (cell == null)
                                        tempColumn++;
                                    if (tempColumn == 19 && cell == null)
                                        isFasterCategory = false;
                                }
                            }
                            if (category.Id == 0) continue;
                        }
                    }
                }
                return new Response { Status = "Ok", Message = "Success!" };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.ToString());
                return new Response { Status = "Error", Message = "Error while adding file! File ERROR!" };
            }
        }
    }
}
