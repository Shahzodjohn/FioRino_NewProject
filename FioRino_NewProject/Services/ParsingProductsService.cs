﻿using FioRino_NewProject.Repositories;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
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

        public ParsingProductsService(ICategoryRepository categoryRepository, IUniqueProductsRepository uniqueProductRepository, ISizeRepository sizeRepository, ISkuRepository skuRepository, ISaveRepository save, IProductService pService, IProductRepository productRepository)
        {
            _categoryRepository = categoryRepository;
            _uniqueProductRepository = uniqueProductRepository;
            _sizeRepository = sizeRepository;
            _skuRepository = skuRepository;
            _save = save;
            _pService = pService;
            _productRepository = productRepository;
        }

        public async Task<string> ParsingProducts()
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
            var RstClient = new RestClient("https://mojegs1.pl/moje-produkty/drukuj-etykiete/5904083473223");
            RstClient.Timeout = -1;
            var RestRequest = new RestRequest(Method.GET);
            RestRequest.AddHeader("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
            foreach (var cookie in PostResponse.Cookies)
            {
                PostRequest.AddCookie(cookie.Name, cookie.Value);
            }
            IRestResponse RstResponse = RstClient.Execute(RestRequest);
            web.LoadHtml(RstResponse.Content);
            var headerCookie = RstResponse.Headers[4].Value;
            var linkCount = 1;
            for (int i = 0; ; i++)
            {
                var RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/{linkCount}?searchText=&isPublic=&amountPerPage=1");
                RstClientNew.Timeout = -1;
                var RestRequestNew = new RestRequest(Method.GET);
                RestRequestNew.AddHeader("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
                IRestResponse rsponseCookie = RstClientNew.Execute(RestRequestNew);
                web.LoadHtml(rsponseCookie.Content);
                var skuN = rsponseCookie.Content.Contains("https://mojegs1.pl/moje-produkty/edycja/");
                foreach (var cookie in RstResponse.Cookies)
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
                        var gtin = await _productRepository.FindProductByGtinAsync(items[3].InnerText.Trim());
                        var isClassicCategory = items[1].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "classic");
                        var isFasterCategory = items[1].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "faster");
                        var productName = isClassicCategory ? items[1].InnerText.Trim().Replace("CLASSIC", " ") :
                                          isFasterCategory ? items[1].InnerText.Trim().Replace("FASTER", " ") :
                                          items[1].InnerText.Trim();
                        var PorductFullName = items[1].InnerText.Trim();
                        var ProdName = productName.Replace("rozm.", " ");
                        var output = Regex.Replace(ProdName, @"[\0-9]", " ");
                        var categoryId = (isClassicCategory ? classicCategory.Id :
                                (isFasterCategory ? fasterCategory.Id : nocategory.Id));
                        var nazvaProductu = new List<string>();
                        nazvaProductu.Add(PorductFullName);
                        var gting = new List<string>();
                        gting.Add(items[3].InnerText.Trim());
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
                        var containProduct = await _uniqueProductRepository.FindUniqueProductByName(MatchingProducts);
                        int ProductId = await _uniqueProductRepository.InsertUniqueProductIfNull(containProduct,MatchingProducts);
                        var resultString = Regex.Match(PorductFullName, @"\d+").Value;
                        int SizeNum = 0;
                        Int32.TryParse(resultString, out SizeNum);
                        var findSize = await _sizeRepository.FindSizeByNumber(SizeNum);
                        var sizeId = await _sizeRepository.CreateSizeIfNull(findSize, SizeNum,resultString);
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
                                 var skuCodeId = await _skuRepository.InsertingSkuIFNull(find,regexing);
                            }
                        }

                        var AddProd = await _pService.InsertDmProduct(MatchingProducts, categoryId, gting[0], ProductId);
                        if (sizeId != 0)
                        {
                            AddProd.SizeId = sizeId;
                            await _save.SaveAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    return ex.InnerException.Message.ToString();
                }
                
            }
        }
    }
}
