using FioRino_NewProject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public interface IParsingExcelService
    {
        //Task CreateCategory();
        Task<int> CreateNewOrder(int Id);
        Task UpdateOrder(int amount, int OrderId);
        Task<DmSize> FindSize(string sizeName);
        Task<int> InsertSize(int sizeNum, string sizeName);
        Task<DmCategory> ClassicCategory();
        Task<DmCategory> FasterCategory();
        Task<DmStorage> FindStorage(string GtinPaging);
        Task<DmProduct> FindDmProduct(string GtinPaging);
        Task RemoveOrder(int OrderId);
        Task<int> InsertSKUcodes(string SkuNumber);
        Task<DmProduct> CheckingProductValidation(string productName, int CategoryId, int SizeId);
        Task<int> CreateUniqueProduct(string productNameworksheet);
        Task<int> InsertProductsToDmProducts(string ProductName, int uniqueproductId, int categoryId, int SizeId, string GtinPaging);
        Task<DmOrderProduct> InsertProductsToOrderProducts(int OrderId, int ProductId, int SizeId, int SkuId, int CategoryId,int productAmount, string GtinPaging);
        Task<DmSkucode> FindSkuCode(string SkuNumber);
    }
}
