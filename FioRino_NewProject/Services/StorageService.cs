using FioRino_NewProject.Data;
using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Entities;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public class StorageService : IStorageService
    {
        private readonly IStorageRepository _storageReposioty;
        private readonly IOrderProductsRepository _OrderProductRepository;
        private readonly FioRinoBaseContext _context;

        public StorageService(IStorageRepository storageReposioty, IOrderProductsRepository orderProductRepository, FioRinoBaseContext context)
        {
            _storageReposioty = storageReposioty;
            _OrderProductRepository = orderProductRepository;
            _context = context;
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

        public async Task StorageCheckPlusAmount(DmStorage StorageCheck, InsertingProductsParams parameters)
        {
            StorageCheck.Amount = StorageCheck.Amount + parameters.Amount;
            StorageCheck.AmountLeft = StorageCheck.AmountLeft + parameters.Amount;
            await _context.SaveChangesAsync();
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
                if (findFromStan.IsBlocked != true)
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
            var xrf = PostResponse.Cookies[0].Value;
            var laravelsession = PostResponse.Cookies[1].Value;
            web.LoadHtml(PostResponse.Content);
            var findOrderProducts = await _OrderProductRepository.GetOrderProductListByOrderIdAsync(OrderId);
            var Images = new List<MemoryStream>();
            foreach (var gtinAddress in findOrderProducts)
            {
                var restclient = new RestClient($"https://mojegs1.pl/moje-produkty/drukuj-etykiete/{gtinAddress.Gtin}");
                restclient.Timeout = -1;
                var restrequest = new RestRequest(Method.GET);
                restrequest.AddHeader("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
                foreach (var cookie in PostResponse.Cookies)
                {
                    PostRequest.AddCookie(cookie.Name, cookie.Value);
                }
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
                        if (findOrder[numArray].Gtin == null) { break; }

                        if (findOrder[numArray].ProductStatusesId == 1)
                        {
                            var fileLen = Convert.ToInt32(file.Length);
                            var zipArchiveEntry = archive.CreateEntry(findOrder[numArray].Gtin + ".pdf", CompressionLevel.Fastest);
                            using (var zipStream = zipArchiveEntry.Open())
                                zipStream.Write(file.ToArray(), 0, fileLen);
                        }
                        numArray++;
                    }
                    archiveStream.Flush();
                }
                archiveFile = archiveStream.ToArray();
            }
            using (FileStream fs = System.IO.File.Create($"PdfCodes/{OrderId}.zip"))
            { fs.Write(archiveFile, 0, archiveFile.Length);}
            return OrderId;
        }
    }
}
