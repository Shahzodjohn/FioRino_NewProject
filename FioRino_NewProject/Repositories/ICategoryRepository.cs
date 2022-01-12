using FioRino_NewProject.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<DmCategory>> CreateCategoryWithListReturn();
        Task CreateCategory();
    }
}
