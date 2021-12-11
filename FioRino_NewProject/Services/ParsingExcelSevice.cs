using FioRino_NewProject.Data;
using FioRino_NewProject.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public class ParsingExcelSevice : IParsingExcelService
    {
        private readonly FioRinoBaseContext _context;

        public ParsingExcelSevice(FioRinoBaseContext context)
        {
            _context = context;
        }

        public Task<DmCategory> ClassicCategory()
        {
           return _context.DmCategories.FirstOrDefaultAsync(x => x.CategoryName == "Classic");
        }
        public Task<DmCategory> FasterCategory()
        {
            return _context.DmCategories.FirstOrDefaultAsync(x => x.CategoryName == "Faster");
        }

        

        public async Task<int> CreateNewOrder(int Id)
        {
            var currentUser = await _context.DmUsers.FirstOrDefaultAsync(x=>x.Id == Id);
            var CreateOrder = new DmOrder
            {
                CreatedAt = DateTime.Today,
                OrderStatusId = 1,
                Amount = 0,
                SenderId = currentUser.Id,
                SourceOfOrder = "Wgrane do systemu",
                OrderExecutor = "FIORINO Izabela Gądek-Pagacz",
                IsInArchievum = false
            };
            _context.DmOrders.Add(CreateOrder);
            await _context.SaveChangesAsync();
            return CreateOrder.Id;
        }
        public async Task<DmSize> FindSize(string sizeName)
        {
            var findSizes = await _context.DmSizes.FirstOrDefaultAsync(x => x.Title == sizeName);
            return findSizes;
        }
        public async Task<int> InsertSize(int sizeNum, string sizeName)
        {
            var insertSizes = new DmSize
            {
                Number = sizeNum,
                Title = sizeName,
            };
            _context.DmSizes.Add(insertSizes);
            await _context.SaveChangesAsync();
            return insertSizes.Id;
        }
        public async Task UpdateOrder(int amount,int OrderId)
        {
            var update = await _context.DmOrders.FirstOrDefaultAsync(x => x.Id == OrderId);
            update.Amount = amount;
            _context.DmOrders.Update(update);
            await _context.SaveChangesAsync();
        }

        public async Task<DmStorage> FindStorage(string GtinPaging)
        {
            var findStan = await _context.DmStorages.FirstOrDefaultAsync(x => x.Gtin == GtinPaging);
            return findStan;
        }

        public async Task<DmProduct> FindDmProduct(string GtinPaging)
        {
            var findProduct = await _context.DmProducts.FirstOrDefaultAsync(x => x.Gtin == GtinPaging);
            return findProduct;
        }

        public async Task RemoveOrder(int OrderId)
        {
            var OrderFind = await _context.DmOrders.FirstOrDefaultAsync(x => x.Id == OrderId);
            var OrderProducts = await _context.DmOrderProducts.Where(x => x.OrderId == OrderId).ToListAsync();
            _context.DmOrderProducts.RemoveRange(OrderProducts);
            _context.DmOrders.Remove(OrderFind);
            await _context.SaveChangesAsync();
        }

        public async Task<int> InsertSKUcodes(string SkuNumber)
        {
            var SkuId = 0;
            var findSKU = await _context.DmSkucodes.FirstOrDefaultAsync(x => x.SkucodeName == SkuNumber);
            if (findSKU == null)
            {
                var addSKU = new DmSkucode
                {
                    SkucodeName = SkuNumber
                };
                _context.DmSkucodes.Add(addSKU);
                await _context.SaveChangesAsync();
                SkuId = addSKU.Id;
            }
            else
            {
                SkuId = findSKU.Id;
            }
            return SkuId;
        }

        public async Task<DmProduct> CheckingProductValidation(string productName, int CategoryId, int SizeId)
        {
            var CheckingProductValidation = await _context.DmProducts.FirstOrDefaultAsync(x => x.ProductName == productName && x.CategoryId == CategoryId && x.SizeId == SizeId);
            return CheckingProductValidation;
        }

        public async Task<int> CreateUniqueProduct(string productNameworksheet)
        {
            var uniquproductFind = await _context.DmUniqueProducts.FirstOrDefaultAsync(x => x.ProductName == productNameworksheet);
            var uniqueproductId = 0;
            if (uniquproductFind == null)
            {
                var addUniqueProduct = new DmUniqueProduct
                {
                    ProductName = productNameworksheet
                };
                _context.DmUniqueProducts.Add(addUniqueProduct);
                await _context.SaveChangesAsync();
                uniqueproductId = addUniqueProduct.Id;
            }
            else
            {
                uniqueproductId = uniquproductFind.Id;
            }
            return uniqueproductId;
        }

        public async Task<int> InsertProductsToDmProducts(string ProductName, int uniqueproductId, int categoryId, int SizeId, string GtinPaging)
        {
            var findProduct = await _context.DmProducts.FirstOrDefaultAsync(x=>x.Gtin == GtinPaging);
            int ProductId = 0;
            if(findProduct == null)
            {
                var insertProds = new DmProduct
                {
                    ProductName = ProductName,
                    UniqueProductId = uniqueproductId,
                    CategoryId = categoryId,
                    SizeId = SizeId,
                    Gtin = GtinPaging
                };
                _context.DmProducts.Add(insertProds);
                await _context.SaveChangesAsync();
                return ProductId = insertProds.Id;
            }
            else
            {
                return ProductId = findProduct.Id;
            }
            
            
        }

        public async Task<DmOrderProduct> InsertProductsToOrderProducts(int OrderId, int ProductId, int SizeId, int SkuId, int CategoryId, int productAmount, string GtinPaging)
        {
            var insert = new DmOrderProduct
            {
                OrderId = OrderId,
                ProductId = ProductId,
                SizeId = SizeId,
                SkucodeId = SkuId,
                CategoryId = CategoryId,
                Amount = productAmount,
                ProductStatusesId = 1,
                Gtin = GtinPaging
            };
            _context.DmOrderProducts.Add(insert);
            await _context.SaveChangesAsync();

            return insert;
        }

        public async Task<DmSkucode> FindSkuCode(string SkuNumber)
        {
            var findSKU = await _context.DmSkucodes.FirstOrDefaultAsync(x => x.SkucodeName == SkuNumber);
            return findSKU;
        }
    }
}
