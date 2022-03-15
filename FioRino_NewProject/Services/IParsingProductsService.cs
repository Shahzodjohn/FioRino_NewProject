using FioRino_NewProject.Entities;
using FioRino_NewProject.Responses;
using RestSharp;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public interface IParsingProductsService
    {
        Task<Response> Cancel();
    }
}
