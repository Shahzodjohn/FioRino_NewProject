using FioRino_NewProject.Data;
using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Entities;
using FioRino_NewProject.Model;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public class StorageService : IStorageService
    {
        private readonly IStorageRepository _storageReposioty;
        private readonly IOrderProductsRepository _OrderProductRepository;
        private readonly FioRinoBaseContext _context;
        private readonly IProductRepository _pRepository; 
        private readonly IWebHostEnvironment _environment;
        private readonly IStorageRepository _storageRepository;

        public StorageService(IStorageRepository storageReposioty, IOrderProductsRepository orderProductRepository, FioRinoBaseContext context, IWebHostEnvironment environment, IProductRepository pPepository, IStorageRepository storageRepository)
        {
            _storageReposioty = storageReposioty;
            _OrderProductRepository = orderProductRepository;
            _context = context;
            _environment = environment;
            _pRepository = pPepository;
            _storageRepository = storageRepository;
        }
        public async Task<Response> MinusingAmountFromStorage(StanAmountUpdateDTO dTO)
        {
            var stanProduct = await _storageReposioty.FindFromStorageByIdAsync(dTO.StanProductId);
            if (dTO.Amount <= stanProduct.Amount)
            {
                stanProduct.AmountLeft = stanProduct.AmountLeft - dTO.Amount; stanProduct.Amount = stanProduct.Amount - dTO.Amount;
                if (stanProduct.Amount == 0)
                {
                    _context.DmStorages.Remove(stanProduct);
                }
                await _context.SaveChangesAsync();
            }
            else
            {
                return new Response { Status = "Error", Message = $"Tylko {stanProduct.Amount} dostępnych produktów!" };
            }
            return new Response { Status = "OK", Message = "Success!" };
        }

        

        public async Task<DmStorage> UpdatingAmountStorage(int stanId, InsertingProductsParams parameters, string Gtin)
        {
            var findFromStan = await _storageReposioty.FindFromStorageByIdAsync(stanId);
            findFromStan.Gtin = Gtin;
            var OrderProductId = await _OrderProductRepository.GetOrderProductListByGtinAsync(Gtin);
            //var findFromStan = await _storageReposioty.FindFromStorageByGtinAsync(Gtin);
            findFromStan.AmountLeft = parameters.Amount;
            await _context.SaveChangesAsync();

            foreach (var item in OrderProductId)
            {
                var Order = await _context.DmOrders.FirstOrDefaultAsync(x => x.Id == item.OrderId);
                if (Order.IsInArchievum != true)
                {
                    if (findFromStan.AmountLeft > item.Amount)
                    {
                        item.ProductStatusesId = 2;
                        findFromStan.AmountLeft = findFromStan.AmountLeft - item.Amount;
                    }
                    if (findFromStan.AmountLeft < item.Amount)
                    {
                        item.ProductStatusesId = 1;
                    }
                }
                await _context.SaveChangesAsync();
            }
            return findFromStan;
        }
        public async Task<int> DrukujCodes(int OrderId)
        {
            using var client = new HttpClient();
            HtmlAgilityPack.HtmlDocument web = new HtmlAgilityPack.HtmlDocument();
            string filePath = Path.GetFullPath($"PdfCodes/");
            var fileName = ($"PdfCodes/{OrderId}" + ".zip");
            if (Directory.Exists(filePath))
            {
                System.IO.DirectoryInfo di = new DirectoryInfo($"{filePath}");

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
                System.IO.File.Delete(fileName);
                Directory.CreateDirectory(filePath);
            }   
            else
            {
                Directory.CreateDirectory(filePath);
            }
            var UrlAddressClient = new RestClient("https://mojegs1.pl/logowanie");
            UrlAddressClient.Timeout = -1;
            var UrlAddressClientRequest = new RestRequest(Method.GET);
            IRestResponse UrlAddressClientResponse = UrlAddressClient.Execute(UrlAddressClientRequest);
            web.LoadHtml(UrlAddressClientResponse.Content);
            var cookieId = UrlAddressClientResponse.Headers[4].Value;

            var cookieName = UrlAddressClientResponse.Cookies[2].Name;
            var CookieValue = UrlAddressClientResponse.Cookies[2].Value;

            var xtokenpath = "//div/form/input[@name='_token' and @type='hidden']";
            var itess = web.DocumentNode.SelectNodes(xtokenpath).ToList();

            var TokenValue = itess[0].OuterHtml;
            var CookieAuthorizingValue = TokenValue.Replace("<input name=\"_token\" type=\"hidden\" value=\"", "");
            CookieAuthorizingValue = CookieAuthorizingValue.Replace("\">", "");

            var PostClient = new RestClient("https://mojegs1.pl/logowanie/zaloguj-uzytkownika");
            PostClient.Timeout = -1;
            var PostRequest = new RestRequest(Method.POST);
            foreach (var cookie in UrlAddressClientResponse.Cookies)
            {
                PostRequest.AddCookie(cookie.Name, cookie.Value);
            }
            PostRequest.AddParameter("_token", CookieAuthorizingValue);
            PostRequest.AddParameter("email", "tomek@fiorino.eu");
            PostRequest.AddParameter("password", "Epoka1-wsx");
            IRestResponse PostResponse = PostClient.Execute(PostRequest);
            web.LoadHtml(PostResponse.Content);
            var ChoiceClient = new RestClient("https://mojegs1.pl/logowanie/wybor-firmy");
            ChoiceClient.Timeout = -1;
            var ChoiceRequest = new RestRequest(Method.POST);
            foreach (var cookie in PostResponse.Cookies)
            {
                ChoiceRequest.AddCookie(cookie.Name, cookie.Value);
            }
            ChoiceRequest.AlwaysMultipartFormData = true;
            ChoiceRequest.AddParameter("company", "62736");
            ChoiceRequest.AddParameter("_token", CookieAuthorizingValue);
            IRestResponse ChoiceResponse = ChoiceClient.Execute(ChoiceRequest);
            Console.WriteLine(ChoiceResponse.Content);

            var findOrderProducts = await _OrderProductRepository.GetOrderProductListByOrderIdAsync(OrderId);
            var Images = new List<MemoryStream>();
            var xrf = ChoiceResponse.Cookies[0].Value;
            var laravelsession = ChoiceResponse.Cookies[1].Value;
            foreach (var gtinAddress in findOrderProducts)
            {
                var restclient = new RestClient($"https://mojegs1.pl/moje-produkty/drukuj-etykiete/{gtinAddress.Gtin}");
                restclient.Timeout = -1;
                var restrequest = new RestRequest(Method.GET);
                restrequest.AddHeader("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
                IRestResponse restresponse = restclient.Execute(restrequest);
                Console.WriteLine(restresponse.Content);
                web.LoadHtml(restresponse.Content);
                var findPath = "//div/input[@type='text']";
                var webload = web.DocumentNode.SelectNodes(findPath).ToList();
                var ProductNameString = webload[0].OuterHtml;
                ProductNameString = ProductNameString.Replace("<input type=\"text\" name=\"label_text\" value=\"", "").Replace("\">", "");
                string url = $"https://mojegs1.pl/moje-produkty/pobierz-etykiete?gtin={gtinAddress.Gtin}&label_text={ProductNameString}&size=1&extension=pdf";
                using (HttpClient cl = new HttpClient())
                {
                    cl.DefaultRequestHeaders.Add("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
                    using (HttpResponseMessage MessageResponse = await cl.GetAsync(url))
                    using (Stream streamToReadFrom = await MessageResponse.Content.ReadAsStreamAsync())
                    {
                        MemoryStream ms = new MemoryStream();
                        streamToReadFrom.CopyTo(ms);
                        Images.Add(ms);
                    }
                }
            }

            
            byte[] archiveFile;
            
            using (var archiveStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
                {
                    var numArray = 0;
                    foreach (var file in Images)
                    {
                        var findOrder = await _OrderProductRepository.GetOrderProductListByOrderIdAsync(OrderId);
                        if (findOrder[numArray].Gtin == null)
                            break;
                        var fileLen = Convert.ToInt32(file.Length);
                        var zipArchiveEntry = archive.CreateEntry(findOrder[numArray].Gtin + ".pdf", System.IO.Compression.CompressionLevel.Fastest);
                        using (var zipStream = zipArchiveEntry.Open())
                            zipStream.Write(file.ToArray(), 0, fileLen);
                        numArray++;
                    }
                    archiveStream.Flush();
                }
                archiveFile = archiveStream.ToArray();
            }
            using (FileStream fs = System.IO.File.Create($"PdfCodes/{OrderId}.zip"))
            { fs.Write(archiveFile, 0, archiveFile.Length); }
            return OrderId;
        }

        public async Task<Response> StorageInsertingProducts(InsertingProductsParams parameters)
        {
            var SelectingcurrentProduct = await _pRepository.FindProductByParams(parameters.UniqueProductId, parameters.CategoryId, parameters.SizeId);
            var productValidation = await _OrderProductRepository.ProductValidationForStanController(parameters);
            if (productValidation.Status == "Error")
            {
                return new Response { Status = "Error", Message = $"{productValidation.Message}" };
            }
            parameters.ProductId = SelectingcurrentProduct.Id;
            var StorageCheck = await _storageRepository.FindFromStorageByGtinAsync(SelectingcurrentProduct.Gtin);
            if (SelectingcurrentProduct != null && StorageCheck == null)
            {
                int? stanId = 0;
                using (SPToCoreContext db = new SPToCoreContext())
                {
                    db.EXPOSE_dm_Storage_Insertingproducts /**/ (parameters.UniqueProductId, parameters.SkuCodeId, parameters.ProductId, parameters.CategoryId, parameters.SizeId, parameters.Amount, ref stanId);
                }
                var findFromStan = await _storageReposioty.FindFromStorageByIdAsync(stanId ?? 0);
                findFromStan.Gtin = SelectingcurrentProduct.Gtin;
                var OrderProductId = await _OrderProductRepository.GetOrderProductListByGtinAsync(SelectingcurrentProduct.Gtin);
                findFromStan.AmountLeft = parameters.Amount;
                await _context.SaveChangesAsync();

                foreach (var item in OrderProductId)
                {
                    var Order = await _context.DmOrders.FirstOrDefaultAsync(x => x.Id == item.OrderId);
                    if (Order.IsInArchievum != true)
                    {
                        if (findFromStan.AmountLeft > item.Amount)
                        {
                            item.ProductStatusesId = 2;
                            findFromStan.AmountLeft = findFromStan.AmountLeft - item.Amount;
                        }
                        else if(findFromStan.AmountLeft < item.Amount)
                        {
                            item.ProductStatusesId = 1;
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                if (findFromStan == null)
                {
                    return new Response { Status = "Error", Message = "Ten produkt jest już w magazynie!" };
                }
            }
            else if (StorageCheck != null)
            {
                StorageCheck.Amount = StorageCheck.Amount + parameters.Amount;
                StorageCheck.AmountLeft = StorageCheck.AmountLeft + parameters.Amount;
                await _context.SaveChangesAsync();
            }
            return new Response { Status = "Ok", Message = "Success" };
        }
    }
    

}
