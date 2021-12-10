using FioRino_NewProject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Repositories
{
    public interface ISizeRepository
    {
        Task<DmSize> FindSizeByNumber(int SizeNum);
        Task<int> CreateSizeIfNull(DmSize findSize, int SizeNum, string resultString, string FindSizeAlphabet);
    }
}
