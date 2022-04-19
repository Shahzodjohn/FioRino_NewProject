using FioRino_NewProject.Data;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public class BrakiService : IBrakiService
    {
        private readonly IOrderProductsRepository _opRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly ISkuRepository _skuRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISizeRepository _sizeRepository;
        private readonly IProductRepository _productRepository;

        public BrakiService(IWebHostEnvironment environment, FioRinoBaseContext context, ISkuRepository skuRepository, ICategoryRepository categoryRepository, ISizeRepository sizeRepository, IProductRepository productRepository, IOrderProductsRepository opRepository)
        {
            _environment = environment;

            _skuRepository = skuRepository;
            _categoryRepository = categoryRepository;
            _sizeRepository = sizeRepository;
            _productRepository = productRepository;
            _opRepository = opRepository;
        }
        public async Task<int> BrakiDrukuj(int OrderId)
        {
            var rootPath = _environment.WebRootPath + "/ExcelOrderFiles";
            string FilePath = $@"/home/pizza/FiorinoDefaultFiles/FileOrderNewEx.xlsx";
            //string FilePath = $@"D:\repos\FioRino_NewProject\FioRino_NewProject\wwwroot\OrderKody\FileOrderNewEx.xlsx";
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
            var newFile = $@"{rootPath}/Order + {OrderId}.xlsx";
            Aspose.Cells.Workbook workBook = new Aspose.Cells.Workbook(FilePath);
            Aspose.Cells.Worksheet workSheet = workBook.Worksheets[0];
            var FindOrderProduct = await _opRepository.GetOrderProductListByOrderIdAsync(OrderId);
            //var FindOrderProduct = await _context.DmOrderProducts.Where(x => x.OrderId == OrderId).ToListAsync();
            int column = 9;
            foreach (var Product in FindOrderProduct)
            {
                var S = workSheet.Cells[column, 3];
                var M = workSheet.Cells[column, 4];
                var L = workSheet.Cells[column, 5];
                var XL = workSheet.Cells[column, 6];
                var TwoXL = workSheet.Cells[column, 7];
                var ThreeXl = workSheet.Cells[column, 8];
                var FourXl = workSheet.Cells[column, 9];
                var FiveXl = workSheet.Cells[column, 10];
                var SixXl = workSheet.Cells[column, 11];
                var SevenXl = workSheet.Cells[column, 12];
                var EightXl = workSheet.Cells[column, 13];
                var NineXl = workSheet.Cells[column, 14];
                var TenXl = workSheet.Cells[column, 15];
                var ElevenXl = workSheet.Cells[column, 16];
                var TwelveXl = workSheet.Cells[column, 17];
                var BRAK = workSheet.Cells[column, 18];
                var Gtin = Product.Gtin;
                var InsertGtin = workSheet.Cells[column, 20];
                var SkuCode = await _skuRepository.FindSkuBySkuId(Product.SkucodeId);
                var FindCategory = await _categoryRepository.FindCategoryById(Product.CategoryId);
                var Size = await _sizeRepository.FindSizeById(Product.SizeId);
                var OrderProductName = await _productRepository.FindProductByIdAsync(Product.ProductId);
                var ProductName = OrderProductName.ProductName.Trim();
                var InsertProductName = workSheet.Cells[column, 1];
                InsertProductName.PutValue(ProductName);
                if (Size.Title == "S")
                {
                    if (FindCategory.CategoryName == "CLASSIC")
                    {
                        S = workSheet.Cells[column, 3];
                        S.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "FASTER")
                    {
                        S = workSheet.Cells[column + 1, 3];
                        S.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 1, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "BRAK")
                    {
                        S = workSheet.Cells[column + 2, 3];
                        S.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 1, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                }
                if (Size.Title == "M")
                {
                    if (FindCategory.CategoryName == "CLASSIC")
                    {
                        M = workSheet.Cells[column, 4];
                        M.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "FASTER")
                    {
                        M = workSheet.Cells[column + 1, 4];
                        M.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 1, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "BRAK")
                    {
                        M = workSheet.Cells[column + 2, 4];
                        M.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 2, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    //M.PutValue(Product.Amount);
                }
                if (Size.Title == "L")
                {
                    if (FindCategory.CategoryName == "CLASSIC")
                    {
                        L = workSheet.Cells[column, 5];
                        L.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "FASTER")
                    {
                        L = workSheet.Cells[column + 1, 5];
                        L.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 1, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "BRAK")
                    {
                        L = workSheet.Cells[column + 2, 5];
                        L.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 2, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    //L.PutValue(Product.Amount);
                }
                if (Size.Title == "XL")
                {
                    if (FindCategory.CategoryName == "CLASSIC")
                    {
                        XL = workSheet.Cells[column, 6];
                        XL.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "FASTER")
                    {
                        XL = workSheet.Cells[column + 1, 6];
                        XL.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 1, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "BRAK")
                    {
                        XL = workSheet.Cells[column + 2, 6];
                        XL.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 2, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    //XL.PutValue(Product.Amount);
                }
                if (Size.Title == "2XL")
                {
                    if (FindCategory.CategoryName == "CLASSIC")
                    {
                        TwoXL = workSheet.Cells[column, 7];
                        TwoXL.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "FASTER")
                    {
                        TwoXL = workSheet.Cells[column + 1, 7];
                        TwoXL.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 1, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "BRAK")
                    {
                        TwoXL = workSheet.Cells[column + 2, 7];
                        TwoXL.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 2, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    //TwoXL.PutValue(Product.Amount);
                }
                if (Size.Title == "3XL")
                {
                    if (FindCategory.CategoryName == "CLASSIC")
                    {
                        ThreeXl = workSheet.Cells[column, 8];
                        ThreeXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "FASTER")
                    {
                        ThreeXl = workSheet.Cells[column + 1, 8];
                        ThreeXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 1, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "BRAK")
                    {
                        ThreeXl = workSheet.Cells[column + 2, 8];
                        ThreeXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 2, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    //ThreeXl.PutValue(Product.Amount);
                }
                if (Size.Title == "4XL")
                {
                    if (FindCategory.CategoryName == "CLASSIC")
                    {
                        FourXl = workSheet.Cells[column, 9];
                        FourXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "FASTER")
                    {
                        FourXl = workSheet.Cells[column + 1, 9];
                        FourXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 1, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "BRAK")
                    {
                        FourXl = workSheet.Cells[column + 2, 9];
                        FourXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 2, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    //FourXl.PutValue(Product.Amount);
                }
                if (Size.Title == "5XL")
                {
                    if (FindCategory.CategoryName == "CLASSIC")
                    {
                        FiveXl = workSheet.Cells[column, 10];
                        FiveXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "FASTER")
                    {
                        FiveXl = workSheet.Cells[column + 1, 10];
                        FiveXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 1, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "BRAK")
                    {
                        FiveXl = workSheet.Cells[column + 2, 10];
                        FiveXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 2, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    //FiveXl.PutValue(Product.Amount);
                }
                if (Size.Title == "6XL")
                {
                    if (FindCategory.CategoryName == "CLASSIC")
                    {
                        SixXl = workSheet.Cells[column, 11];
                        SixXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "FASTER")
                    {
                        SixXl = workSheet.Cells[column + 1, 11];
                        SixXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 1, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "BRAK")
                    {
                        SixXl = workSheet.Cells[column + 2, 11];
                        SixXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 2, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    //SixXl.PutValue(Product.Amount);
                }
                if (Size.Title == "7XL")
                {
                    if (FindCategory.CategoryName == "CLASSIC")
                    {
                        SevenXl = workSheet.Cells[column, 12];
                        SevenXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "FASTER")
                    {
                        SevenXl = workSheet.Cells[column + 1, 12];
                        SevenXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 1, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "BRAK")
                    {
                        SevenXl = workSheet.Cells[column + 2, 12];
                        SevenXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 2, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    //SevenXl.PutValue(Product.Amount);
                }
                if (Size.Title == "8XL")
                {
                    if (FindCategory.CategoryName == "CLASSIC")
                    {
                        EightXl = workSheet.Cells[column, 13];
                        EightXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "FASTER")
                    {
                        EightXl = workSheet.Cells[column + 1, 13];
                        EightXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 1, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "BRAK")
                    {
                        EightXl = workSheet.Cells[column + 2, 13];
                        EightXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 2, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    //EightXl.PutValue(Product.Amount);
                }
                if (Size.Title == "9XL")
                {
                    if (FindCategory.CategoryName == "CLASSIC")
                    {
                        NineXl = workSheet.Cells[column, 14];
                        NineXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "FASTER")
                    {
                        NineXl = workSheet.Cells[column + 1, 14];
                        NineXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 1, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "BRAK")
                    {
                        NineXl = workSheet.Cells[column + 2, 14];
                        NineXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 2, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    //NineXl.PutValue(Product.Amount);
                }
                if (Size.Title == "10XL")
                {
                    if (FindCategory.CategoryName == "CLASSIC")
                    {
                        TenXl = workSheet.Cells[column, 15];
                        TenXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "FASTER")
                    {
                        TenXl = workSheet.Cells[column + 1, 15];
                        TenXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 1, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "BRAK")
                    {
                        TenXl = workSheet.Cells[column + 2, 15];
                        TenXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 2, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    //TenXl.PutValue(Product.Amount);
                }
                if (Size.Title == "11XL")
                {
                    if (FindCategory.CategoryName == "CLASSIC")
                    {
                        ElevenXl = workSheet.Cells[column, 16];
                        ElevenXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "FASTER")
                    {
                        ElevenXl = workSheet.Cells[column + 1, 16];
                        ElevenXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 1, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "BRAK")
                    {
                        ElevenXl = workSheet.Cells[column + 2, 16];
                        ElevenXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 2, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    //ElevenXl.PutValue(Product.Amount);
                }
                if (Size.Title == "12XL")
                {
                    if (FindCategory.CategoryName == "CLASSIC")
                    {
                        TwelveXl = workSheet.Cells[column, 17];
                        TwelveXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "FASTER")
                    {
                        TwelveXl = workSheet.Cells[column + 1, 17];
                        TwelveXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 1, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "BRAK")
                    {
                        TwelveXl = workSheet.Cells[column + 2, 17];
                        TwelveXl.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 2, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    // TwelveXl.PutValue(Product.Amount);
                }
                if (Size.Title == "BRAK")
                {
                    if (FindCategory.CategoryName == "CLASSIC")
                    {
                        BRAK = workSheet.Cells[column, 18];
                        BRAK.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "FASTER")
                    {
                        BRAK = workSheet.Cells[column + 1, 18];
                        BRAK.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 1, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    if (FindCategory.CategoryName == "BRAK")
                    {
                        BRAK = workSheet.Cells[column + 2, 18];
                        BRAK.PutValue(Product.Amount);
                        InsertGtin = workSheet.Cells[column + 2, 20];
                        InsertGtin.PutValue(Gtin);
                    }
                    //BRAK.PutValue(Product.Amount);
                }
                var InsertSku = workSheet.Cells[column, 0];
                InsertSku.PutValue(SkuCode.SkucodeName);
                workBook.Save(newFile, Aspose.Cells.SaveFormat.Xlsx);
                foreach (var wb in workBook.Worksheets)
                {
                    if (wb.Name.ToString().Contains("Evaluation Warning"))
                    {
                        DeleteWorkSheet(newFile);
                    }
                }
                column = column + 3;
            }
            return OrderId;
        }
        private void DeleteWorkSheet(string Route)
        {
            ExcelPackage.LicenseContext = LicenseContext.Commercial;
            FileInfo file = new(Route);
            using (ExcelPackage exPackage = new(file))
            {
                ExcelWorksheet excelWorksheet = exPackage.Workbook.Worksheets[1];
                exPackage.Workbook.Worksheets.Delete(excelWorksheet);
                exPackage.Save();
            }
        }
    }
}
