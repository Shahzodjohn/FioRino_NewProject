using FioRino_NewProject.Data;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using FioRino_NewProject.Settings;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public class ParsingProductsService : IParsingProductsService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductService _pService;
        private readonly IUniqueProductsRepository _uniqueProductRepository;
        private readonly ISizeRepository _sizeRepository;
        private readonly ISkuRepository _skuRepository;
        private readonly ISaveRepository _save;
        private readonly IStatusRepository _statusRepository;
        private readonly FioRinoBaseContext _context;


        public ParsingProductsService(ICategoryRepository categoryRepository, IUniqueProductsRepository uniqueProductRepository, ISizeRepository sizeRepository, ISkuRepository skuRepository, ISaveRepository save, IProductService pService, IProductRepository productRepository, IStatusRepository statusRepository, FioRinoBaseContext context)
        {
            _categoryRepository = categoryRepository;
            _uniqueProductRepository = uniqueProductRepository;
            _sizeRepository = sizeRepository;
            _skuRepository = skuRepository;
            _save = save;
            _pService = pService;
            _productRepository = productRepository;
            _statusRepository = statusRepository;
            _context = context;
        }
        public async Task<Response> Cancel()
        {
            var loadingStatus = await _statusRepository.GetFirst();
            loadingStatus.Status = "STOPPED";
            //loadingStatus.PocessIsKilled = true;
            
            await _save.SaveAsync();
            return new Response { Status = "Error", Message = "Double click is not allowed!" };
        }

        public async Task<Response> ParsingProducts()
        {
            var loadingStatus = await _statusRepository.GetFirst();
            if (loadingStatus.Status == "LOADING")
            {
                return new Response { Status = "Error", Message = "Double click is not allowed!" };
            }
            loadingStatus.Status = "LOADING";
            await _save.SaveAsync();
            await Parser();
            return new Response { Status = "Ok", Message = "Successfully stopped!" };
        }

        public async Task Parser()
        {
            using var client = new HttpClient();
            HtmlAgilityPack.HtmlDocument web = new HtmlAgilityPack.HtmlDocument();
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
            var rs = PostResponse.Headers[4].Value;
            web.LoadHtml(PostResponse.Content);

            var linkCount = 1;
            for (int i = 0; ;i++)
            {
                var originalEntity = await _statusRepository.OriginalValues();
                
                var RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/{linkCount}?searchText=&isPublic=&amountPerPage=1");
                //var RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/1?amountPerPage=50&searchText=5904083497274&isPublic=");
                //var RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/1?amountPerPage=1&searchText=-6&isPublic=");
                RstClientNew.Timeout = -1;
                var RestRequestNew = new RestRequest(Method.GET);
                RestRequestNew.AddHeader("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
                IRestResponse rsponseCookie = RstClientNew.Execute(RestRequestNew);
                web.LoadHtml(rsponseCookie.Content);
                var pageFind = "//div[@class='form-group']";
                var page = web.DocumentNode.SelectNodes(pageFind).ToList();

                string CurrentAmountString = Regex.Match(page[0].InnerHtml, @"\d+").Value;
                int CurrentAmount = 0;
                Int32.TryParse(CurrentAmountString, out CurrentAmount);
                int TotalAmount = 0;
                var TotalAmountString = page[0].InnerHtml.Replace(")\n                            ", "").Split(" ").Last();
                Int32.TryParse(TotalAmountString, out TotalAmount);
                var LoadingStatus = await _statusRepository.CreateStatusIfNull(CurrentAmount, TotalAmount);
                if (originalEntity.Status == "STOPPED")
                {
                    var updating = await _statusRepository.GetFirst();
                    updating.CurrentAmount = 0;
                    //updating.PocessIsKilled = false;
                    updating.TotalAmount = 0;
                    updating.Status = originalEntity.Status;
                    await _context.SaveChangesAsync();
                    return;
                }
                if (CurrentAmount == TotalAmount)
                {
                    var statusSelect = await _statusRepository.GetFirst();
                    statusSelect.Status = "SUCCESS";
                    statusSelect.SuccessDate = DateTime.Now;
                    await _save.SaveAsync();
                }


                var skuN = rsponseCookie.Content.Contains("https://mojegs1.pl/moje-produkty/edycja/");
                foreach (var cookie in PostResponse.Cookies)
                {
                    PostRequest.AddCookie(cookie.Name, cookie.Value);
                }
                web.LoadHtml(rsponseCookie.Content);
                linkCount++;
                var xpath = "//tr";
                var category = await _categoryRepository.CreateCategoryWithListReturn();
                var nocategory = category[0];
                var classicCategory = category[1];
                var fasterCategory = category[2];
                try
                {
                    foreach (var item in web.DocumentNode.SelectNodes(xpath).ToList().Skip(1))
                    {
                        var items = item.ChildNodes;
                        var gtin = items[3].InnerText.Trim();
                        var isClassicCategory = items[1].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "classic");
                        var isFasterCategory = items[1].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "faster");
                        var productName = isClassicCategory ? items[1].InnerText.Trim().Replace("CLASSIC", " ") :
                                          isFasterCategory ? items[1].InnerText.Trim().Replace("FASTER", " ") :
                                          items[1].InnerText.Trim();
                        var ProductFullName = items[1].InnerText.Trim();
                        string output;
                        
                        var ProdName = productName.Contains("rozm.") ? productName.Replace("rozm.", " ") : productName.Replace("r.", "");
                        //ProdName = Regex.Replace(productName, @"[\0-9]", " ");
                        var ProdNameWithUpperSlash = ProdName.Split(" ").Last().Contains("-");
                       
                        if (!ProdName.Contains("cm") && !ProdName.Contains("MET") && !ProdNameWithUpperSlash == true)
                        {
                            output = Regex.Replace(ProdName, @"[\0-9]", " ");
                        }
                        else
                        {
                            output = productName;
                        }
                        var categoryId = (isClassicCategory ? classicCategory.Id :
                                (isFasterCategory ? fasterCategory.Id : nocategory.Id));
                        #region EANCodesDonwloading
                        //foreach (var gtinImages in gting)
                        //{
                        //    using (HttpClient cl = new HttpClient())
                        //    {
                        //        cl.DefaultRequestHeaders.Add("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
                        //        string url = $"https://mojegs1.pl/moje-produkty/pobierz-etykiete?gtin={gtinImages}&label_text={PorductFullName}&size=1&extension=png";
                        //        using (HttpResponseMessage MessageResponse = await cl.GetAsync(url))
                        //        using (Stream streamToReadFrom = await MessageResponse.Content.ReadAsStreamAsync())
                        //        {
                        //            string fileToWriteTo = Path.GetFullPath($"PdfCodes/{gtinImages}.png");
                        //            using (Stream streamToWriteTo = System.IO.File.Open(fileToWriteTo, FileMode.Create))
                        //            {
                        //                await streamToReadFrom.CopyToAsync(streamToWriteTo);
                        //            }
                        //        }
                        //    }
                        //}
                        #endregion
                        var MatchingProducts = output.Replace("    ", "");
                        string FindSizeAlphabet;
                        var containProduct = await _uniqueProductRepository.FindUniqueProductByName(MatchingProducts);
                        int ProductId = await _uniqueProductRepository.InsertUniqueProductIfNull(containProduct, MatchingProducts);
                        var splitLastIndex = ProductFullName.Split(" ").Last();
                        if (MatchingProducts.Split(" ").Last() == "M" ||
                             MatchingProducts.Split(" ").Last() == "S" ||
                             MatchingProducts.Split(" ").Last() == "L" ||
                             MatchingProducts.Split(" ").Last() == "XL" ||
                             MatchingProducts.Split(" ").Last() == "XS" ||
                             MatchingProducts.Split(" ").Last() == "2XL")
                        {
                            FindSizeAlphabet = MatchingProducts.Split(" ").Last();
                            var SizeAlphabet = MatchingProducts.LastIndexOf(" ");
                            if (SizeAlphabet > 0)
                                MatchingProducts = MatchingProducts.Substring(0, SizeAlphabet);
                        }
                        else
                        {
                            FindSizeAlphabet = null;
                        }
                        int SizeNum = 0;
                        if (!splitLastIndex.Contains("cm") && !splitLastIndex.Contains("-"))
                        {
                            var resultString = Regex.Match(splitLastIndex, @"\d+").Value;
                            Int32.TryParse(resultString, out SizeNum);
                        }

                        var findSize = await _sizeRepository.FindSizeByNumber(SizeNum);
                        var sizeId = await _sizeRepository.CreateSizeIfNull(findSize, SizeNum, FindSizeAlphabet);

                        var skuTaking = "//div/table/tbody/tr//td/a[@href]";
                        var webload = web.DocumentNode.SelectNodes(skuTaking).ToList();
                        var address = webload[0].OuterHtml;
                        address = address.Replace("<a href=\"", "");
                        var addressTrimming = address.LastIndexOf("\"");
                        if (addressTrimming > 0)
                            address = address.Substring(0, addressTrimming);
                        var HOSTclient = new RestClient(address);
                        HOSTclient.Timeout = -1;
                        var HOSTrequest = new RestRequest(Method.GET);
                        foreach (var cookie in rsponseCookie.Cookies)
                        {
                            HOSTrequest.AddCookie(cookie.Name, cookie.Value);
                        }
                        IRestResponse HOSTresponse = HOSTclient.Execute(HOSTrequest);
                        web.LoadHtml(HOSTresponse.Content);
                        var skuNumberPath = "//input[@name='product_sku']";
                        if (web.DocumentNode.SelectNodes(skuNumberPath) != null)
                        {
                            var skuNumber = web.DocumentNode.SelectNodes(skuNumberPath).ToList();
                            var regexing = skuNumber[0].OuterHtml;
                            regexing = regexing.Replace("<input type=\"text\" name=\"product_sku\" tabindex=\"5\" maxlength=\"255", "");
                            if (regexing.Contains("value=\""))
                            {
                                regexing = regexing.Replace("value=\"", "").Replace("\" ", "");
                            }
                            var regexslash = regexing.LastIndexOf("\"");
                            if (addressTrimming > 0)
                                regexing = regexing.Substring(0, regexslash);
                            if (regexing != "")
                            {
                                var find = await _skuRepository.FindSkuBySkuCodeName(regexing);
                                var skuCodeId = await _skuRepository.InsertingSkuIFNull(find, regexing);
                            }
                        }
                        var AddProd = await _pService.InsertDmProduct(MatchingProducts, categoryId, gtin, ProductId);
                        if (sizeId != 0)
                        {
                            AddProd.SizeId = sizeId;
                            await _save.SaveAsync();
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    
                    if (LoadingStatus.CurrentAmount != LoadingStatus.TotalAmount)
                    {
                        LoadingStatus.Status = "ERROR";
                        LoadingStatus.SuccessDate = DateTime.Now;
                        await _save.SaveAsync();
                    }
                    //return ex.InnerException.Message.ToString();
                }

            }
        }
    }
}
