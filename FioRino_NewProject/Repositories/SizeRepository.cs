using FioRino_NewProject.Data;
using FioRino_NewProject.Entities;
using Microsoft.EntityFrameworkCore;
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

        public async Task<int> CreateSizeIfNull(DmSize findSize, int SizeNum, string FindSizeAlphabet)
        {
            int sizeId = 0;
            var size = await _context.DmSizes.FirstOrDefaultAsync(x => x.Title == FindSizeAlphabet);
            if (findSize == null)
            {
                if (FindSizeAlphabet != null)
                {
                    var addingSizes = _context.DmSizes.Add(new DmSize
                    {
                        Title = FindSizeAlphabet
                    });
                    await _context.SaveChangesAsync();
                    return sizeId = addingSizes.Entity.Id;
                }
                var Size = await _context.DmSizes.FirstOrDefaultAsync(x => x.Number == SizeNum);
                if (size == null && SizeNum != 0)
                {
                    var addingSizes = _context.DmSizes.Add(new DmSize
                    {
                        Number = SizeNum
                    });
                    await _context.SaveChangesAsync();
                    sizeId = addingSizes.Entity.Id;
                }
                else
                {
                    var brakSize = await _context.DmSizes.FirstOrDefaultAsync(x => x.Title == "BRAK");
                    if (brakSize == null)
                    {
                        var CreateSize = _context.DmSizes.Add(new DmSize
                        {
                            Title = "BRAK",
                            Number = 0
                        });
                        await _context.SaveChangesAsync();
                        return CreateSize.Entity.Id;
                    }
                    else
                    {
                        return brakSize.Id;
                    }
                }

                var FindCurrentSize = _context.DmSizes.FirstOrDefault(x => x.Id == sizeId);

                #region Deviding by Sizes
                if (FindCurrentSize.Number <= 15)
                {
                    FindCurrentSize.Title = $"rozm.{FindCurrentSize.Number}";
                }
                if (FindCurrentSize.Number == 16 || FindCurrentSize.Number == 17)
                {
                    FindCurrentSize.Title = "XS";
                }
                if (FindCurrentSize.Number == 18 || FindCurrentSize.Number == 19)
                {
                    FindCurrentSize.Title = "S";
                }
                if (FindCurrentSize.Number == 20)
                {
                    FindCurrentSize.Title = "M";
                }
                if (FindCurrentSize.Number == 21 || FindCurrentSize.Number == 22)
                {
                    FindCurrentSize.Title = "L";
                }
                if (FindCurrentSize.Number == 23 || FindCurrentSize.Number == 24)
                {
                    FindCurrentSize.Title = "XL";
                }
                if (FindCurrentSize.Number == 25 || FindCurrentSize.Number == 26)
                {
                    FindCurrentSize.Title = "2XL";
                }
                if (FindCurrentSize.Number == 27)
                {
                    FindCurrentSize.Title = "3XL";
                }
                if (FindCurrentSize.Number == 28 || FindCurrentSize.Number == 29)
                {
                    FindCurrentSize.Title = "4XL";
                }
                if (FindCurrentSize.Number == 30)
                {
                    FindCurrentSize.Title = "5XL";
                }
                if (FindCurrentSize.Number == 31 || FindCurrentSize.Number == 32)
                {
                    FindCurrentSize.Title = "6XL";
                }
                if (FindCurrentSize.Number == 33 || FindCurrentSize.Number == 34)
                {
                    FindCurrentSize.Title = "7XL";
                }
                if (FindCurrentSize.Number == 35 || FindCurrentSize.Number == 36)
                {
                    FindCurrentSize.Title = "8XL";
                }
                if (FindCurrentSize.Number == 37 || FindCurrentSize.Number == 38)
                {
                    FindCurrentSize.Title = "9XL";
                }
                if (FindCurrentSize.Number == 39)
                {
                    FindCurrentSize.Title = "10XL";
                }
                if (FindCurrentSize.Number == 40 || FindCurrentSize.Number == 41)
                {
                    FindCurrentSize.Title = "11XL";
                }
                if (FindCurrentSize.Number == 42 || FindCurrentSize.Number == 43)
                {
                    FindCurrentSize.Title = "12XL";
                }
                if (FindCurrentSize.Number == 44)
                {
                    FindCurrentSize.Title = "13XL";
                }

                if (FindCurrentSize.Number == 0)
                {
                    FindCurrentSize.Title = "BRAK";
                }
                #endregion
                await _context.SaveChangesAsync();
            }
            else
            {
                if (size != null) { return sizeId = size.Id; }
                sizeId = findSize.Id;
            }
            return sizeId;
        }

        public async Task<DmSize> FindSizeById(int? Id)
        {
            var findSize = await _context.DmSizes.FirstOrDefaultAsync(x => x.Id == Id);
            return findSize;
        }

        public async Task<DmSize> FindSizeByNumber(int SizeNum)
        {
            var findSize = await _context.DmSizes.FirstOrDefaultAsync(x => x.Number == SizeNum);
            return findSize;
        }
    }
}
