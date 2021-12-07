using FioRino_NewProject.Data;
using FioRino_NewProject.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public class SizeRepository : ISizeRepository
    {
        private readonly FioRinoBaseContext _context;

        public SizeRepository(FioRinoBaseContext context)
        {
            _context = context;
        }

        public async Task<int> CreateSizeIfNull(DmSize findSize, int SizeNum, string resultString)
        {
            int sizeId = 0;
            if (findSize == null)
            {
                if (SizeNum != 0 || resultString == "")
                {
                    var addingSizes = _context.DmSizes.Add(new DmSize
                    {
                        Number = SizeNum
                    });
                    await _context.SaveChangesAsync();
                    sizeId = addingSizes.Entity.Id;
                    //var num = addingSizes.Entity.Id;
                }
                var FindCurrentSize = _context.DmSizes.FirstOrDefault(x => x.Id == sizeId);

                #region Deviding by Sizes
                if (resultString == "16" || resultString == "17")
                {
                    FindCurrentSize.Title = "XS";
                }
                if (resultString == "18" || resultString == "19")
                {
                    FindCurrentSize.Title = "S";
                }
                if (resultString == "20")
                {
                    FindCurrentSize.Title = "M";
                }
                if (resultString == "21" || resultString == "22")
                {
                    FindCurrentSize.Title = "L";
                }
                if (resultString == "23" || resultString == "24")
                {
                    FindCurrentSize.Title = "XL";
                }
                if (resultString == "25" || resultString == "26")
                {
                    FindCurrentSize.Title = "2XL";
                }
                if (resultString == "27")
                {
                    FindCurrentSize.Title = "3XL";
                }
                if (resultString == "28" || resultString == "29")
                {
                    FindCurrentSize.Title = "4XL";
                }
                if (resultString == "30")
                {
                    FindCurrentSize.Title = "5XL";
                }
                if (resultString == "31" || resultString == "32")
                {
                    FindCurrentSize.Title = "6XL";
                }
                if (resultString == "33" || resultString == "34")
                {
                    FindCurrentSize.Title = "7XL";
                }
                if (resultString == "35" || resultString == "36")
                {
                    FindCurrentSize.Title = "8XL";
                }
                if (resultString == "37" || resultString == "38")
                {
                    FindCurrentSize.Title = "9XL";
                }
                if (resultString == "39")
                {
                    FindCurrentSize.Title = "10XL";
                }
                if (resultString == "40" || resultString == "41")
                {
                    FindCurrentSize.Title = "11XL";
                }
                if (resultString == "42" || resultString == "43")
                {
                    FindCurrentSize.Title = "12XL";
                }
                if (resultString == "44")
                {
                    FindCurrentSize.Title = "13XL";
                }
                if (resultString == "0" || resultString == "")
                {
                    FindCurrentSize.Title = "BRAK";
                }
                #endregion
                await _context.SaveChangesAsync();
            }
            else
            {
                sizeId = findSize.Id;
            }
            return sizeId;
        }

        public async Task<DmSize> FindSizeByNumber(int SizeNum)
        {
            var findSize = await _context.DmSizes.FirstOrDefaultAsync(x => x.Number == SizeNum);
            return findSize;
        }
    }
}
