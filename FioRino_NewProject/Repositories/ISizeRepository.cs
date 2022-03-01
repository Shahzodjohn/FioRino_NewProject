using FioRino_NewProject.Entities;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public interface ISizeRepository
    {
        Task<DmSize> FindSizeByNumber(int SizeNum);
        Task<DmSize> FindSizeById(int? Id);
        //Task<int> CreateSizeByNumberWithchecking(int num);
        Task<int> CreateSizeIfNull(DmSize findSize, int SizeNum, string FindSizeAlphabet);
    }
}
