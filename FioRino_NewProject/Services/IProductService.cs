using FioRino_NewProject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public interface IProductService
    {
        Task<DmProduct> FindProductAsync(int ProductId);
        Task SettingUpReceiver(DmOrder dmOrder, int CurrentUserId, int SenderId);
        Task AddingProductsToStorage(string GTIN, int SkuCodeId, int Amount, int ProductId, int SizeId, int CategoryId);
        Task<DmProduct> InsertDmProduct(string ProductName, int CategoryId, string Gtin, int UniqueProductId);
    }
}
