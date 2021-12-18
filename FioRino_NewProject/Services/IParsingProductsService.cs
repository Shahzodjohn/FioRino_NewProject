using FioRino_NewProject.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public interface IParsingProductsService
    {
        Task<Response> ParsingProducts();
        Task<Response> Cancel();
    }
}
