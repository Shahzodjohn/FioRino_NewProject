using FioRino_NewProject.Data;
using FioRino_NewProject.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FioRinoBaseContext _context;

        public CategoryRepository(FioRinoBaseContext context)
        {
            _context = context;
        }
        public async Task<List<DmCategory>> CreateCategoryWithListReturn()
        {
            var nocategory = await _context.DmCategories.FirstOrDefaultAsync(x => x.CategoryName == "BRAK");
            if (nocategory == null)
            {
                var rt = new DmCategory()
                {
                    CategoryName = "BRAK",
                };

                nocategory = _context.DmCategories.Add(rt).Entity;
                _context.SaveChanges();
            }

            var ClassicCategory = await _context.DmCategories.FirstOrDefaultAsync(x => x.CategoryName == "Classic");
            if (ClassicCategory == null)
            {
                var rt = new DmCategory()
                {
                    CategoryName = "Classic",
                };

                ClassicCategory = rt;
                await _context.SaveChangesAsync();
            }

            var FasterCategory = await _context.DmCategories.FirstOrDefaultAsync(x => x.CategoryName == "Faster");
            if (FasterCategory == null)
            {
                var rt = new DmCategory()
                {
                    CategoryName = "Faster",
                };

                FasterCategory = rt;
                await _context.SaveChangesAsync();
            }
            return new List<DmCategory> { nocategory, ClassicCategory, FasterCategory };
        }

        public async Task CreateCategory()
        {
            var nocategory = await _context.DmCategories.FirstOrDefaultAsync(x => x.CategoryName == "BRAK");
            if (nocategory == null)
            {
                var rt = new DmCategory()
                {
                    CategoryName = "BRAK",
                };

                nocategory = _context.DmCategories.Add(rt).Entity;
                _context.SaveChanges();
            }

            var ClassicCategory = _context.DmCategories.FirstOrDefault(x => x.CategoryName == "Classic");
            if (ClassicCategory == null)
            {
                var rt = new DmCategory()
                {
                    CategoryName = "Classic",
                };

                ClassicCategory = _context.DmCategories.Add(rt).Entity;
                await _context.SaveChangesAsync();
            }
            var FasterCategory = _context.DmCategories.FirstOrDefault(x => x.CategoryName == "Faster");
            if (FasterCategory == null)
            {
                var rt = new DmCategory()
                {
                    CategoryName = "Faster",
                };

                FasterCategory = _context.DmCategories.Add(rt).Entity;
                await _context.SaveChangesAsync();
            }
        }
    }
}
