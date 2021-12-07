using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public interface IParsingProductsService
    {
        Task<string> ParsingProducts();
    }
}
