using FioRino_NewProject.Entities;
using FioRino_NewProject.Responses;
using RestSharp;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public interface IParsingProductsService
    {
        Task<Response> ParsingProducts();
        Task<Response> Cancel();
        Task Pool(RestClient RstClientNew, string xrf, string laravelsession, HtmlAgilityPack.HtmlDocument web, DmDownloadingStatus originalEntity, IRestResponse PostResponse, int linkCount, RestRequest PostRequest);
    }
}
