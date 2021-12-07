using FioRino_NewProject.Data;
using FioRino_NewProject.Entities;
using FioRino_NewProject.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public class ProductService : IProductService
    {
        private readonly FioRinoBaseContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productReposiotory;
        private readonly IStorageRepository _storageReposiotory;

        public ProductService(FioRinoBaseContext context, IUserRepository userReposiotory, IStorageRepository storageReposiotory, IProductRepository productReposiotory)
        {
            _context = context;
            _userRepository = userReposiotory;
            _storageReposiotory = storageReposiotory;
            _productReposiotory = productReposiotory;
        }

        public async Task AddingProductsToStorage(string GTIN, int SkuCodeId, int Amount, int ProductId, int SizeId, int CategoryId)
        {
            var findProductInStorage = await _storageReposiotory.FindFromStorageByGtinAsync(GTIN);
            if(findProductInStorage == null)
            {
                var AddingProductsToStorage = new DmStorage
                {
                    SkuCodeId = SkuCodeId,
                    Amount = Amount,
                    AmountLeft = Amount,
                    Gtin = GTIN,
                    ProductId = ProductId,
                    SizeId = SizeId,
                    CategoryId = CategoryId
                };
                _context.DmStorages.Add(AddingProductsToStorage);
                await _context.SaveChangesAsync();  
            }
            else
            {
                findProductInStorage.Amount = findProductInStorage.Amount + Amount;
                findProductInStorage.AmountLeft = findProductInStorage.AmountLeft + Amount;
            }
            
            await _context.SaveChangesAsync();
        }

        public async Task<DmProduct> FindProductAsync(int ProductId)
        {
            var findProduct = await _context.DmProducts.FirstOrDefaultAsync(x => x.Id == ProductId);
            return findProduct;
        }

        public async Task SettingUpReceiver(DmOrder dmOrder,int CurrentUserId, int SenderId)
        {
            var User = await _userRepository.GetUser(SenderId);
            var currentUser = await _userRepository.GetUser(CurrentUserId);
            if (User.Id != currentUser.Id && dmOrder.IsInMagazyn == true)
            {
                dmOrder.ReceiverId = currentUser.Id;
            }
            dmOrder.OrderStatusId = 2;
            await _context.SaveChangesAsync();
        }
        public async Task<DmProduct> InsertDmProduct(string ProductName, int CategoryId, string Gtin, int UniqueProductId)
        {
            var FindProduct = await _productReposiotory.FindProductByGtinAsync(Gtin);
            if(FindProduct == null)
            {
                var AddProd = new DmProduct
                {
                    ProductName = ProductName,
                    CategoryId = CategoryId,
                    Gtin = Gtin,
                    UniqueProductId = UniqueProductId,

                };
                await _context.DmProducts.AddAsync(AddProd);
                await _context.SaveChangesAsync();
                return AddProd;
            }
            
            return FindProduct;
        }
    }
}
