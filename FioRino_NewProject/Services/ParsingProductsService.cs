using FioRino_NewProject.Data;
using FioRino_NewProject.Entities;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using RestSharp;
using System;
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
        private readonly IStatusRepository _statusRepository;
        private readonly FioRinoBaseContext _context;
        //private readonly ParseHelper parse;


        public ParsingProductsService(ICategoryRepository categoryRepository, IUniqueProductsRepository uniqueProductRepository, ISizeRepository sizeRepository,
            ISkuRepository skuRepository, ISaveRepository save, IProductService pService, IProductRepository productRepository, IStatusRepository statusRepository,
            FioRinoBaseContext context)
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

        //public async Task<Response> ParsingProducts()
        //{
        //    var loadingStatus = await _statusRepository.GetFirst();
        //    if (loadingStatus.Status == "LOADING")
        //    {
        //        return new Response { Status = "Error", Message = "Double click is not allowed!" };
        //    }
        //    loadingStatus.Status = "LOADING";
        //    await _save.SaveAsync();
        //    await Parser();
        //    return new Response { Status = "Ok", Message = "Successfully stopped!" };
        //}

    //    public async Task Parser()
    //    {
    //        using var client = new HttpClient();
    //        HtmlAgilityPack.HtmlDocument web = new HtmlAgilityPack.HtmlDocument();
    //        var UrlAddressClient = new RestClient("https://mojegs1.pl/logowanie");
    //        UrlAddressClient.Timeout = -1;
    //        var UrlAddressClientRequest = new RestRequest(Method.GET);
    //        IRestResponse UrlAddressClientResponse = UrlAddressClient.Execute(UrlAddressClientRequest);
    //        web.LoadHtml(UrlAddressClientResponse.Content);
    //        //var cookieId = UrlAddressClientResponse.Headers[4].Value;
    //        var cookieName = UrlAddressClientResponse.Cookies[2].Name;
    //        var CookieValue = UrlAddressClientResponse.Cookies[2].Value;
    //        var xtokenpath = "//div/form/input[@name='_token' and @type='hidden']";
    //        var itess = web.DocumentNode.SelectNodes(xtokenpath).ToList();
    //        var TokenValue = itess[0].OuterHtml;
    //        var CookieAuthorizingValue = TokenValue.Replace("<input name=\"_token\" type=\"hidden\" value=\"", "");
    //        CookieAuthorizingValue = CookieAuthorizingValue.Replace("\">", "");
    //        var PostClient = new RestClient("https://mojegs1.pl/logowanie/zaloguj-uzytkownika");
    //        PostClient.Timeout = -1;
    //        var PostRequest = new RestRequest(Method.POST);
    //        foreach (var cookie in UrlAddressClientResponse.Cookies)
    //        {
    //            PostRequest.AddCookie(cookie.Name, cookie.Value);
    //        }
    //        PostRequest.AddParameter("_token", CookieAuthorizingValue);
    //        PostRequest.AddParameter("email", "tomek@fiorino.eu");
    //        PostRequest.AddParameter("password", "Epoka1-wsx");
    //        IRestResponse PostResponse = PostClient.Execute(PostRequest);
    //        var xrf = PostResponse.Cookies[0].Value;
    //        var laravelsession = PostResponse.Cookies[1].Value;
    //        var rs = PostResponse.Headers[4].Value;
    //        web.LoadHtml(PostResponse.Content);

    //        var linkCount = 1; 
    //        for (int i = 0; ; i++)
    //        {
    //            var originalEntity = await _statusRepository.OriginalValues();

    //            var RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/{linkCount}?searchText=&isPublic=&amountPerPage=1");
    //            //var RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/1?amountPerPage=50&searchText=5904083402070");
    //            //var RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/1?amountPerPage=50&searchText=5904422639426#");
    //            #region
    //            RstClientNew.Timeout = -1;
    //            var RestRequestNew = new RestRequest(Method.GET);
    //            RestRequestNew.AddHeader("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //            IRestResponse rsponseCookie = RstClientNew.Execute(RestRequestNew);
    //            web.LoadHtml(rsponseCookie.Content);
    //            var pageFind = "//div[@class='form-group']";
    //            var page = web.DocumentNode.SelectNodes(pageFind).ToList();

    //            string CurrentAmountString = Regex.Match(page[2].InnerHtml, @"\d+").Value;
    //            int CurrentAmount = 0;
    //            Int32.TryParse(CurrentAmountString, out CurrentAmount);
    //            int TotalAmount = 0;
    //            var TotalAmountString = page[2].InnerHtml.Replace($"\n            <br>\n            ({CurrentAmount}-{CurrentAmount}\n            z\n            ", "").Replace(")\n        ", "");
    //            Int32.TryParse(TotalAmountString, out TotalAmount);
    //            var LoadingStatus = await _statusRepository.CreateStatusIfNull(CurrentAmount, TotalAmount);
    //            if (originalEntity.Status == "STOPPED")
    //            {
    //                var updating = await _statusRepository.GetFirst();
    //                updating.CurrentAmount = 0;
    //                //updating.PocessIsKilled = false;
    //                updating.TotalAmount = 0;
    //                updating.Status = originalEntity.Status;
    //                await _context.SaveChangesAsync();
    //                return;
    //            }
    //            if (CurrentAmount == TotalAmount)
    //            {
    //                var statusSelect = await _statusRepository.GetFirst();
    //                statusSelect.Status = "SUCCESS";
    //                statusSelect.SuccessDate = DateTime.Now;
    //                await _save.SaveAsync();
    //            }


    //            var skuN = rsponseCookie.Content.Contains("https://mojegs1.pl/moje-produkty/edycja/");
    //            foreach (var cookie in PostResponse.Cookies)
    //            {
    //                PostRequest.AddCookie(cookie.Name, cookie.Value);
    //            }
    //            web.LoadHtml(rsponseCookie.Content);
    //            linkCount++;
    //            var xpath = "//tr";
    //            var category = await _categoryRepository.CreateCategoryWithListReturn();
    //            var nocategory = category[0];
    //            var classicCategory = category[1];
    //            var fasterCategory = category[2];
    //            try
    //            {

    //                #region
    //                foreach (var item in web.DocumentNode.SelectNodes(xpath).ToList().Skip(1))
    //                {
    //                    var items = item.ChildNodes;
    //                    var gtin = items[5].InnerText.Trim();
    //                    var isClassicCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "classic");
    //                    var isFasterCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "faster");
    //                    var productName = isClassicCategory ? items[3].InnerText.Trim().Replace("CLASSIC", " ") :
    //                                      isFasterCategory ? items[3].InnerText.Trim().Replace("FASTER", " ") :
    //                                      items[3].InnerText.Trim();
    //                    var ProductFullName = items[3].InnerText.Trim();
    //                    string output;

    //                    var ProdName = productName.Contains("rozm.") ? productName.Replace("rozm.", " ") : productName.Replace("r.", "");
    //                    //ProdName = Regex.Replace(productName, @"[\0-9]", " ");
    //                    var ProdNameWithUpperSlash = ProdName.Split(" ").Last().Contains("-");

    //                    if (!ProdName.Contains("cm") && !ProdName.Contains("MET") && !ProdNameWithUpperSlash == true)
    //                    {
    //                        output = Regex.Replace(ProdName, @"[\0-9]", " ");
    //                    }
    //                    else
    //                    {
    //                        output = productName;
    //                    }
    //                    var categoryId = (isClassicCategory ? classicCategory.Id :
    //                            (isFasterCategory ? fasterCategory.Id : nocategory.Id));
    //                    #region EANCodesDonwloading
    //                    //foreach (var gtinImages in gting)
    //                    //{
    //                    //    using (HttpClient cl = new HttpClient())
    //                    //    {
    //                    //        cl.DefaultRequestHeaders.Add("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //                    //        string url = $"https://mojegs1.pl/moje-produkty/pobierz-etykiete?gtin={gtinImages}&label_text={PorductFullName}&size=1&extension=png";
    //                    //        using (HttpResponseMessage MessageResponse = await cl.GetAsync(url))
    //                    //        using (Stream streamToReadFrom = await MessageResponse.Content.ReadAsStreamAsync())
    //                    //        {
    //                    //            string fileToWriteTo = Path.GetFullPath($"PdfCodes/{gtinImages}.png");
    //                    //            using (Stream streamToWriteTo = System.IO.File.Open(fileToWriteTo, FileMode.Create))
    //                    //            {
    //                    //                await streamToReadFrom.CopyToAsync(streamToWriteTo);
    //                    //            }
    //                    //        }
    //                    //    }
    //                    //}
    //                    #endregion
    //                    var MatchingProducts = output.Replace("    ", "");
    //                    string FindSizeAlphabet;
    //                    //var containProduct = await _uniqueProductRepository.FindUniqueProductByName(MatchingProducts);
    //                    int ProductId = await _uniqueProductRepository.InsertUniqueProductIfNull(MatchingProducts);
    //                    var splitLastIndex = ProductFullName.Split(" ").Last();
    //                    if (MatchingProducts.Split(" ").Last() == "M" ||
    //                         MatchingProducts.Split(" ").Last() == "S" ||
    //                         MatchingProducts.Split(" ").Last() == "L" ||
    //                         MatchingProducts.Split(" ").Last() == "XL" ||
    //                         MatchingProducts.Split(" ").Last() == "XS" ||
    //                         MatchingProducts.Split(" ").Last() == "2XL")
    //                    {
    //                        FindSizeAlphabet = MatchingProducts.Split(" ").Last();
    //                        var SizeAlphabet = MatchingProducts.LastIndexOf(" ");
    //                        if (SizeAlphabet > 0)
    //                            MatchingProducts = MatchingProducts.Substring(0, SizeAlphabet);
    //                    }
    //                    else
    //                    {
    //                        FindSizeAlphabet = null;
    //                    }
    //                    int SizeNum = 0;
    //                    if (!splitLastIndex.Contains("cm") && !splitLastIndex.Contains("-"))
    //                    {
    //                        var resultString = Regex.Match(splitLastIndex, @"\d+").Value;
    //                        Int32.TryParse(resultString, out SizeNum);
    //                    }

    //                    var findSize = await _sizeRepository.FindSizeByNumber(SizeNum);
    //                    var sizeId = await _sizeRepository.CreateSizeIfNull(findSize, SizeNum, FindSizeAlphabet);

    //                    var skuTaking = "//div/table/tbody/tr//td/a[@href]";
    //                    var webload = web.DocumentNode.SelectNodes(skuTaking).ToList();
    //                    var address = webload[0].OuterHtml;
    //                    address = address.Replace("<a href=\"", "");
    //                    var addressTrimming = address.LastIndexOf("\"");
    //                    if (addressTrimming > 0)
    //                        address = address.Substring(0, addressTrimming);
    //                    var HOSTclient = new RestClient(address);
    //                    HOSTclient.Timeout = -1;
    //                    var HOSTrequest = new RestRequest(Method.GET);
    //                    foreach (var cookie in rsponseCookie.Cookies)
    //                    {
    //                        HOSTrequest.AddCookie(cookie.Name, cookie.Value);
    //                    }
    //                    IRestResponse HOSTresponse = HOSTclient.Execute(HOSTrequest);
    //                    web.LoadHtml(HOSTresponse.Content);
    //                    var skuNumberPath = "//*[@id='productSku']";

    //                    if (web.DocumentNode.SelectSingleNode(skuNumberPath) != null)
    //                    {
    //                        var skuNumber = web.DocumentNode.SelectNodes(skuNumberPath).ToList();
    //                        var regexing = skuNumber[0].OuterHtml;
    //                        regexing = regexing.Replace("<div class=\"row-fluid text--blue text--bold text--wrap\" id=\"productSku\">\n                ", "").Replace("\n            </div>", "");
    //                        if (regexing.Contains("value=\""))
    //                        {
    //                            regexing = regexing.Replace("value=\"", "").Replace("\" ", "");
    //                        }

    //                        if (regexing != "")
    //                        {
    //                            var find = await _skuRepository.FindSkuBySkuCodeName(regexing);
    //                            var skuCodeId = await _skuRepository.InsertingSkuIFNull(/*find,*/ regexing);
    //                        }
    //                    }
    //                    var AddProd = await _pService.InsertDmProduct(MatchingProducts, categoryId, gtin, ProductId, sizeId);
    //                    if (sizeId != 0)
    //                    {
    //                        AddProd.SizeId = sizeId;
    //                        await _save.SaveAsync();
    //                    }

    //                }
    //                #endregion
    //            }
    //            catch (Exception ex)
    //            {
    //                ex.Message.ToString();
    //                if (LoadingStatus.CurrentAmount != LoadingStatus.TotalAmount)
    //                {
    //                    LoadingStatus.Status = "ERROR";
    //                    LoadingStatus.SuccessDate = DateTime.Now;
    //                    await _save.SaveAsync();
    //                }

    //            }
    //            #endregion

    //            //var HelperInstance = await parse.Pool(RstClientNew, xrf, laravelsession, web, originalEntity, PostResponse, linkCount, PostRequest);
    //            //linkCount++;
    //            //if(HelperInstance == "STOPPED")
    //            //{
    //            //    return new Response { Status = "Ok", Message = "Successfully stopped!" };
    //            //}
    //            #region
    //            //RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/{linkCount}?searchText=&isPublic=&amountPerPage=1");
    //            //var HelperInstance_2 = await parseHelperInstance2.Pool(RstClientNew, xrf, laravelsession, web, originalEntity, PostResponse, linkCount, PostRequest);
    //            //if (HelperInstance_2 == "STOPPED")
    //            //{
    //            //    return new Response { Status = "Ok", Message = "Successfully stopped!" };
    //            //}
    //            //linkCount++;
    //            //RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/{linkCount}?searchText=&isPublic=&amountPerPage=1");
    //            //var HelperInstance_3 = await parseHelperInstance1.Pool(RstClientNew, xrf, laravelsession, web, originalEntity, PostResponse, linkCount, PostRequest);
    //            //if (HelperInstance_3 == "STOPPED")
    //            //{
    //            //    return new Response { Status = "Ok", Message = "Successfully stopped!" };
    //            //}
    //            //linkCount++;
    //            //RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/{linkCount}?searchText=&isPublic=&amountPerPage=1");
    //            //var HelperInstance_4 = await parseHelperInstance3.Pool(RstClientNew, xrf, laravelsession, web, originalEntity, PostResponse, linkCount, PostRequest);
    //            //if (HelperInstance_4 == "STOPPED")
    //            //{
    //            //    return new Response { Status = "Ok", Message = "Successfully stopped!" };
    //            //}
    //            //linkCount++;
    //            //RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/{linkCount}?searchText=&isPublic=&amountPerPage=1");
    //            //var HelperInstance_5 = await parseHelperInstance4.Pool(RstClientNew, xrf, laravelsession, web, originalEntity, PostResponse, linkCount, PostRequest);
    //            //if (HelperInstance_5 == "STOPPED")
    //            //{
    //            //    return new Response { Status = "Ok", Message = "Successfully stopped!" };
    //            //}
    //            //linkCount++;
    //            //RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/{linkCount}?searchText=&isPublic=&amountPerPage=1");
    //            //var HelperInstance_6 = await parseHelperInstance5.Pool(RstClientNew, xrf, laravelsession, web, originalEntity, PostResponse, linkCount, PostRequest);
    //            //if (HelperInstance_6 == "STOPPED")
    //            //{
    //            //    return new Response { Status = "Ok", Message = "Successfully stopped!" };
    //            //}
    //            // linkCount++;
    //            #endregion
    //            #region
    //            //RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/{linkCount}?searchText=&isPublic=&amountPerPage=1");
    //            //var HelperInstance_7 = await parseHelperInstance6.Pool(RstClientNew, xrf, laravelsession, web, originalEntity, PostResponse, linkCount, PostRequest);
    //            //if (HelperInstance_7 == "STOPPED")
    //            //{
    //            //    return new Response { Status = "Ok", Message = "Successfully stopped!" };
    //            //}
    //            //linkCount++;
    //            //RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/{linkCount}?searchText=&isPublic=&amountPerPage=1");
    //            //var HelperInstance_8 = await parseHelperInstance7.Pool(RstClientNew, xrf, laravelsession, web, originalEntity, PostResponse, linkCount, PostRequest);
    //            //if (HelperInstance_8 == "STOPPED")
    //            //{
    //            //    return new Response { Status = "Ok", Message = "Successfully stopped!" };
    //            //}
    //            //linkCount++;
    //            //RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/{linkCount}?searchText=&isPublic=&amountPerPage=1");
    //            //var HelperInstance_9 = await parseHelperInstance8.Pool(RstClientNew, xrf, laravelsession, web, originalEntity, PostResponse, linkCount, PostRequest);
    //            //if (HelperInstance_9 == "STOPPED")
    //            //{
    //            //    return new Response { Status = "Ok", Message = "Successfully stopped!" };
    //            //}
    //            //linkCount++; RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/{linkCount}?searchText=&isPublic=&amountPerPage=1");
    //            //var HelperInstance_10 = await parseHelperInstance9.Pool(RstClientNew, xrf, laravelsession, web, originalEntity, PostResponse, linkCount, PostRequest);
    //            //if (HelperInstance_10 == "STOPPED")
    //            //{
    //            //    return new Response { Status = "Ok", Message = "Successfully stopped!" };
    //            //}
    //            //linkCount++;
    //            //RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/{linkCount}?searchText=&isPublic=&amountPerPage=1");
    //            //var HelperInstance_11 = await parseHelperInstance10.Pool(RstClientNew, xrf, laravelsession, web, originalEntity, PostResponse, linkCount, PostRequest);
    //            //if (HelperInstance_11 == "STOPPED")
    //            //{
    //            //    return new Response { Status = "Ok", Message = "Successfully stopped!" };
    //            //}
    //            //linkCount++;
    //            //RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/{linkCount}?searchText=&isPublic=&amountPerPage=1");
    //            //var HelperInstance_12 = await parseHelperInstance11.Pool(RstClientNew, xrf, laravelsession, web, originalEntity, PostResponse, linkCount, PostRequest);
    //            //if (HelperInstance_12 == "STOPPED")
    //            //{
    //            //    return new Response { Status = "Ok", Message = "Successfully stopped!" };
    //            //}
    //            //linkCount++;
    //            //RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/{linkCount}?searchText=&isPublic=&amountPerPage=1");
    //            //var HelperInstance_13 = await parseHelperInstance12.Pool(RstClientNew, xrf, laravelsession, web, originalEntity, PostResponse, linkCount, PostRequest);
    //            //if (HelperInstance_13 == "STOPPED")
    //            //{
    //            //    return new Response { Status = "Ok", Message = "Successfully stopped!" };
    //            //}
    //            //linkCount++;
    //            //RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/{linkCount}?searchText=&isPublic=&amountPerPage=1");
    //            //var HelperInstance_14 = await parseHelperInstance13.Pool(RstClientNew, xrf, laravelsession, web, originalEntity, PostResponse, linkCount, PostRequest);
    //            //if (HelperInstance_14 == "STOPPED")
    //            //{
    //            //    return new Response { Status = "Ok", Message = "Successfully stopped!" };
    //            //}
    //            //linkCount++;
    //            //RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty/sortowanie/nazwa/kierunek/rosnaco/{linkCount}?searchText=&isPublic=&amountPerPage=1");
    //            //var HelperInstance_15 = await parseHelperInstance14.Pool(RstClientNew, xrf, laravelsession, web, originalEntity, PostResponse, linkCount, PostRequest);
    //            //if (HelperInstance_15 == "STOPPED")
    //            //{
    //            //    return new Response { Status = "Ok", Message = "Successfully stopped!" };
    //            //}
    //            //linkCount++;
    //            //var HelperInstance_16 = await parseHelperInstance15.Pool(RstClientNew, xrf, laravelsession, web, originalEntity, PostResponse, linkCount, PostRequest);
    //            //if (HelperInstance_16 == "STOPPED")
    //            //{
    //            //    return new Response { Status = "Ok", Message = "Successfully stopped!" };
    //            //}
    //            //linkCount++;
    //            #endregion
                

    //            //ParseHelperInstance2 ParseHelperInstance2 = new ParseHelperInstance2();
    //            //await parseHelper.Pool(RstClientNew, xrf, laravelsession, web, originalEntity, PostResponse, linkCount, PostRequest);

    //        }

    //    }
    //    public async Task Pool(RestClient RstClientNew, string xrf, string laravelsession, HtmlAgilityPack.HtmlDocument web, DmDownloadingStatus originalEntity, IRestResponse PostResponse, int linkCount, RestRequest PostRequest)
    //    {
    //        #region
    //        RstClientNew.Timeout = -1;
    //        var RestRequestNew = new RestRequest(Method.GET);
    //        RestRequestNew.AddHeader("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //        IRestResponse rsponseCookie = RstClientNew.Execute(RestRequestNew);
    //        web.LoadHtml(rsponseCookie.Content);
    //        var pageFind = "//div[@class='form-group']";
    //        var page = web.DocumentNode.SelectNodes(pageFind).ToList();

    //        string CurrentAmountString = Regex.Match(page[2].InnerHtml, @"\d+").Value;
    //        int CurrentAmount = 0;
    //        Int32.TryParse(CurrentAmountString, out CurrentAmount);
    //        int TotalAmount = 0;
    //        var TotalAmountString = page[2].InnerHtml.Replace($"\n            <br>\n            ({CurrentAmount}-{CurrentAmount}\n            z\n            ", "").Replace(")\n        ", "");
    //        Int32.TryParse(TotalAmountString, out TotalAmount);
    //        var LoadingStatus = await _statusRepository.CreateStatusIfNull(CurrentAmount, TotalAmount);
    //        if (originalEntity.Status == "STOPPED")
    //        {
    //            var updating = await _statusRepository.GetFirst();
    //            updating.CurrentAmount = 0;
    //            //updating.PocessIsKilled = false;
    //            updating.TotalAmount = 0;
    //            updating.Status = originalEntity.Status;
    //            await _context.SaveChangesAsync();
    //            return;
    //        }
    //        if (CurrentAmount == TotalAmount)
    //        {
    //            var statusSelect = await _statusRepository.GetFirst();
    //            statusSelect.Status = "SUCCESS";
    //            statusSelect.SuccessDate = DateTime.Now;
    //            await _save.SaveAsync();
    //        }


    //        var skuN = rsponseCookie.Content.Contains("https://mojegs1.pl/moje-produkty/edycja/");
    //        foreach (var cookie in PostResponse.Cookies)
    //        {
    //            PostRequest.AddCookie(cookie.Name, cookie.Value);
    //        }
    //        web.LoadHtml(rsponseCookie.Content);
    //        //linkCount++;
    //        var xpath = "//tr";
    //        var category = await _categoryRepository.CreateCategoryWithListReturn();
    //        var nocategory = category[0];
    //        var classicCategory = category[1];
    //        var fasterCategory = category[2];
    //        try
    //        {

    //            #region
    //            foreach (var item in web.DocumentNode.SelectNodes(xpath).ToList().Skip(1))
    //            {
    //                var items = item.ChildNodes;
    //                var gtin = items[5].InnerText.Trim();
    //                var isClassicCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "classic");
    //                var isFasterCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "faster");
    //                var productName = isClassicCategory ? items[3].InnerText.Trim().Replace("CLASSIC", " ") :
    //                                  isFasterCategory ? items[3].InnerText.Trim().Replace("FASTER", " ") :
    //                                  items[3].InnerText.Trim();
    //                var ProductFullName = items[3].InnerText.Trim();
    //                string output;

    //                var ProdName = productName.Contains("rozm.") ? productName.Replace("rozm.", " ") : productName.Replace("r.", "");
    //                //ProdName = Regex.Replace(productName, @"[\0-9]", " ");
    //                var ProdNameWithUpperSlash = ProdName.Split(" ").Last().Contains("-");

    //                if (!ProdName.Contains("cm") && !ProdName.Contains("MET") && !ProdNameWithUpperSlash == true)
    //                {
    //                    output = Regex.Replace(ProdName, @"[\0-9]", " ");
    //                }
    //                else
    //                {
    //                    output = productName;
    //                }
    //                var categoryId = (isClassicCategory ? classicCategory.Id :
    //                        (isFasterCategory ? fasterCategory.Id : nocategory.Id));
    //                #region EANCodesDonwloading
    //                //foreach (var gtinImages in gting)
    //                //{
    //                //    using (HttpClient cl = new HttpClient())
    //                //    {
    //                //        cl.DefaultRequestHeaders.Add("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //                //        string url = $"https://mojegs1.pl/moje-produkty/pobierz-etykiete?gtin={gtinImages}&label_text={PorductFullName}&size=1&extension=png";
    //                //        using (HttpResponseMessage MessageResponse = await cl.GetAsync(url))
    //                //        using (Stream streamToReadFrom = await MessageResponse.Content.ReadAsStreamAsync())
    //                //        {
    //                //            string fileToWriteTo = Path.GetFullPath($"PdfCodes/{gtinImages}.png");
    //                //            using (Stream streamToWriteTo = System.IO.File.Open(fileToWriteTo, FileMode.Create))
    //                //            {
    //                //                await streamToReadFrom.CopyToAsync(streamToWriteTo);
    //                //            }
    //                //        }
    //                //    }
    //                //}
    //                #endregion
    //                var MatchingProducts = output.Replace("    ", "");
    //                string FindSizeAlphabet;
    //                var containProduct = await _uniqueProductRepository.FindUniqueProductByName(MatchingProducts);
    //                int ProductId = await _uniqueProductRepository.InsertUniqueProductIfNull(/*containProduct,*/ MatchingProducts);
    //                var splitLastIndex = ProductFullName.Split(" ").Last();
    //                if (MatchingProducts.Split(" ").Last() == "M" ||
    //                     MatchingProducts.Split(" ").Last() == "S" ||
    //                     MatchingProducts.Split(" ").Last() == "L" ||
    //                     MatchingProducts.Split(" ").Last() == "XL" ||
    //                     MatchingProducts.Split(" ").Last() == "XS" ||
    //                     MatchingProducts.Split(" ").Last() == "2XL")
    //                {
    //                    FindSizeAlphabet = MatchingProducts.Split(" ").Last();
    //                    var SizeAlphabet = MatchingProducts.LastIndexOf(" ");
    //                    if (SizeAlphabet > 0)
    //                        MatchingProducts = MatchingProducts.Substring(0, SizeAlphabet);
    //                }
    //                else
    //                {
    //                    FindSizeAlphabet = null;
    //                }
    //                int SizeNum = 0;
    //                if (!splitLastIndex.Contains("cm") && !splitLastIndex.Contains("-"))
    //                {
    //                    var resultString = Regex.Match(splitLastIndex, @"\d+").Value;
    //                    Int32.TryParse(resultString, out SizeNum);
    //                }

    //                var findSize = await _sizeRepository.FindSizeByNumber(SizeNum);
    //                var sizeId = await _sizeRepository.CreateSizeIfNull(findSize, SizeNum, FindSizeAlphabet);

    //                var skuTaking = "//div/table/tbody/tr//td/a[@href]";
    //                var webload = web.DocumentNode.SelectNodes(skuTaking).ToList();
    //                var address = webload[0].OuterHtml;
    //                address = address.Replace("<a href=\"", "");
    //                var addressTrimming = address.LastIndexOf("\"");
    //                if (addressTrimming > 0)
    //                    address = address.Substring(0, addressTrimming);
    //                var HOSTclient = new RestClient(address);
    //                HOSTclient.Timeout = -1;
    //                var HOSTrequest = new RestRequest(Method.GET);
    //                foreach (var cookie in rsponseCookie.Cookies)
    //                {
    //                    HOSTrequest.AddCookie(cookie.Name, cookie.Value);
    //                }
    //                IRestResponse HOSTresponse = HOSTclient.Execute(HOSTrequest);
    //                web.LoadHtml(HOSTresponse.Content);
    //                var skuNumberPath = "//*[@id='productSku']";

    //                if (web.DocumentNode.SelectSingleNode(skuNumberPath) != null)
    //                {
    //                    var skuNumber = web.DocumentNode.SelectNodes(skuNumberPath).ToList();
    //                    var regexing = skuNumber[0].OuterHtml;
    //                    regexing = regexing.Replace("<div class=\"row-fluid text--blue text--bold text--wrap\" id=\"productSku\">\n                ", "").Replace("\n            </div>", "");
    //                    if (regexing.Contains("value=\""))
    //                    {
    //                        regexing = regexing.Replace("value=\"", "").Replace("\" ", "");
    //                    }

    //                    if (regexing != "")
    //                    {
    //                        var find = await _skuRepository.FindSkuBySkuCodeName(regexing);
    //                        var skuCodeId = await _skuRepository.InsertingSkuIFNull(/*find, */regexing);
    //                    }
    //                }
    //                //var AddProd = await _pService.InsertDmProduct(MatchingProducts, categoryId, gtin, ProductId);
    //                //if (sizeId != 0)
    //                //{
    //                //    AddProd.SizeId = sizeId;
    //                //    await _save.SaveAsync();
    //                //}

    //            }
    //            #endregion
    //        }
    //        catch (Exception ex)
    //        {
    //            ex.Message.ToString();
    //            if (LoadingStatus.CurrentAmount != LoadingStatus.TotalAmount)
    //            {
    //                LoadingStatus.Status = "ERROR";
    //                LoadingStatus.SuccessDate = DateTime.Now;
    //                await _save.SaveAsync();
    //            }

    //        }
    //        #endregion
    //    }
    //}
    //public class ParseHelper
    //{
    //    private readonly ICategoryRepository _categoryRepository;
    //    private readonly IProductRepository _productRepository;
    //    private readonly IProductService _pService;
    //    private readonly IUniqueProductsRepository _uniqueProductRepository;
    //    private readonly ISizeRepository _sizeRepository;
    //    private readonly ISkuRepository _skuRepository;
    //    private readonly ISaveRepository _save;
    //    private readonly IStatusRepository _statusRepository;
    //    private readonly FioRinoBaseContext _context;

    //    public ParseHelper(ICategoryRepository categoryRepository, IUniqueProductsRepository uniqueProductRepository, ISizeRepository sizeRepository, ISkuRepository skuRepository, ISaveRepository save, IProductService pService, IProductRepository productRepository, IStatusRepository statusRepository, FioRinoBaseContext context)
    //    {
    //        _categoryRepository = categoryRepository;
    //        _uniqueProductRepository = uniqueProductRepository;
    //        _sizeRepository = sizeRepository;
    //        _skuRepository = skuRepository;
    //        _save = save;
    //        _pService = pService;
    //        _productRepository = productRepository;
    //        _statusRepository = statusRepository;
    //        _context = context;
    //    }
    //    public async Task<string> Pool(RestClient RstClientNew, string xrf, string laravelsession, HtmlAgilityPack.HtmlDocument web, DmDownloadingStatus originalEntity, IRestResponse PostResponse, int linkCount, RestRequest PostRequest)
    //    {
    //        RstClientNew.Timeout = -1;
    //        var RestRequestNew = new RestRequest(Method.GET);
    //        RestRequestNew.AddHeader("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //        IRestResponse rsponseCookie = RstClientNew.Execute(RestRequestNew);
    //        web.LoadHtml(rsponseCookie.Content);
    //        var pageFind = "//div[@class='form-group']";
    //        var page = web.DocumentNode.SelectNodes(pageFind).ToList();

    //        string CurrentAmountString = Regex.Match(page[2].InnerHtml, @"\d+").Value;
    //        int CurrentAmount = 0;
    //        Int32.TryParse(CurrentAmountString, out CurrentAmount);
    //        int TotalAmount = 0;
    //        var TotalAmountString = page[2].InnerHtml.Replace($"\n            <br>\n            ({CurrentAmount}-{CurrentAmount}\n            z\n            ", "").Replace(")\n        ", "");
    //        Int32.TryParse(TotalAmountString, out TotalAmount);
    //        var LoadingStatus = await _statusRepository.CreateStatusIfNull(CurrentAmount, TotalAmount);
    //        if (originalEntity.Status == "STOPPED")
    //        {
    //            var updating = await _statusRepository.GetFirst();
    //            updating.CurrentAmount = 0;
    //            //updating.PocessIsKilled = false;
    //            updating.TotalAmount = 0;
    //            updating.Status = originalEntity.Status;
    //            await _context.SaveChangesAsync();
    //            return "STOPPED";
    //        }
    //        if (CurrentAmount == TotalAmount)
    //        {
    //            var statusSelect = await _statusRepository.GetFirst();
    //            statusSelect.Status = "SUCCESS";
    //            statusSelect.SuccessDate = DateTime.Now;
    //            await _save.SaveAsync();
    //        }


    //        var skuN = rsponseCookie.Content.Contains("https://mojegs1.pl/moje-produkty/edycja/");
    //        foreach (var cookie in PostResponse.Cookies)
    //        {
    //            PostRequest.AddCookie(cookie.Name, cookie.Value);
    //        }
    //        web.LoadHtml(rsponseCookie.Content);
    //        //linkCount++;
    //        var xpath = "//tr";
    //        var category = await _categoryRepository.CreateCategoryWithListReturn();
    //        var nocategory = category[0];
    //        var classicCategory = category[1];
    //        var fasterCategory = category[2];
    //        try
    //        {

    //            #region
    //            foreach (var item in web.DocumentNode.SelectNodes(xpath).ToList().Skip(1))
    //            {
    //                var items = item.ChildNodes;
    //                var gtin = items[5].InnerText.Trim();
    //                var isClassicCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "classic");
    //                var isFasterCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "faster");
    //                var productName = isClassicCategory ? items[3].InnerText.Trim().Replace("CLASSIC", " ") :
    //                                  isFasterCategory ? items[3].InnerText.Trim().Replace("FASTER", " ") :
    //                                  items[3].InnerText.Trim();
    //                var ProductFullName = items[3].InnerText.Trim();
    //                string output;

    //                var ProdName = productName.Contains("rozm.") ? productName.Replace("rozm.", " ") : productName.Replace("r.", "");
    //                //ProdName = Regex.Replace(productName, @"[\0-9]", " ");
    //                var ProdNameWithUpperSlash = ProdName.Split(" ").Last().Contains("-");

    //                if (!ProdName.Contains("cm") && !ProdName.Contains("MET") && !ProdNameWithUpperSlash == true)
    //                {
    //                    output = Regex.Replace(ProdName, @"[\0-9]", " ");
    //                }
    //                else
    //                {
    //                    output = productName;
    //                }
    //                var categoryId = (isClassicCategory ? classicCategory.Id :
    //                        (isFasterCategory ? fasterCategory.Id : nocategory.Id));
    //                #region EANCodesDonwloading
    //                //foreach (var gtinImages in gting)
    //                //{
    //                //    using (HttpClient cl = new HttpClient())
    //                //    {
    //                //        cl.DefaultRequestHeaders.Add("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //                //        string url = $"https://mojegs1.pl/moje-produkty/pobierz-etykiete?gtin={gtinImages}&label_text={PorductFullName}&size=1&extension=png";
    //                //        using (HttpResponseMessage MessageResponse = await cl.GetAsync(url))
    //                //        using (Stream streamToReadFrom = await MessageResponse.Content.ReadAsStreamAsync())
    //                //        {
    //                //            string fileToWriteTo = Path.GetFullPath($"PdfCodes/{gtinImages}.png");
    //                //            using (Stream streamToWriteTo = System.IO.File.Open(fileToWriteTo, FileMode.Create))
    //                //            {
    //                //                await streamToReadFrom.CopyToAsync(streamToWriteTo);
    //                //            }
    //                //        }
    //                //    }
    //                //}
    //                #endregion
    //                var MatchingProducts = output.Replace("    ", "");
    //                string FindSizeAlphabet;
    //                var containProduct = await _uniqueProductRepository.FindUniqueProductByName(MatchingProducts);
    //                int ProductId = await _uniqueProductRepository.InsertUniqueProductIfNull(/*containProduct,*/ MatchingProducts);
    //                var splitLastIndex = ProductFullName.Split(" ").Last();
    //                if (MatchingProducts.Split(" ").Last() == "M" ||
    //                     MatchingProducts.Split(" ").Last() == "S" ||
    //                     MatchingProducts.Split(" ").Last() == "L" ||
    //                     MatchingProducts.Split(" ").Last() == "XL" ||
    //                     MatchingProducts.Split(" ").Last() == "XS" ||
    //                     MatchingProducts.Split(" ").Last() == "2XL")
    //                {
    //                    FindSizeAlphabet = MatchingProducts.Split(" ").Last();
    //                    var SizeAlphabet = MatchingProducts.LastIndexOf(" ");
    //                    if (SizeAlphabet > 0)
    //                        MatchingProducts = MatchingProducts.Substring(0, SizeAlphabet);
    //                }
    //                else
    //                {
    //                    FindSizeAlphabet = null;
    //                }
    //                int SizeNum = 0;
    //                if (!splitLastIndex.Contains("cm") && !splitLastIndex.Contains("-"))
    //                {
    //                    var resultString = Regex.Match(splitLastIndex, @"\d+").Value;
    //                    Int32.TryParse(resultString, out SizeNum);
    //                }

    //                var findSize = await _sizeRepository.FindSizeByNumber(SizeNum);
    //                var sizeId = await _sizeRepository.CreateSizeIfNull(findSize, SizeNum, FindSizeAlphabet);

    //                var skuTaking = "//div/table/tbody/tr//td/a[@href]";
    //                var webload = web.DocumentNode.SelectNodes(skuTaking).ToList();
    //                var address = webload[0].OuterHtml;
    //                address = address.Replace("<a href=\"", "");
    //                var addressTrimming = address.LastIndexOf("\"");
    //                if (addressTrimming > 0)
    //                    address = address.Substring(0, addressTrimming);
    //                var HOSTclient = new RestClient(address);
    //                HOSTclient.Timeout = -1;
    //                var HOSTrequest = new RestRequest(Method.GET);
    //                foreach (var cookie in rsponseCookie.Cookies)
    //                {
    //                    HOSTrequest.AddCookie(cookie.Name, cookie.Value);
    //                }
    //                IRestResponse HOSTresponse = HOSTclient.Execute(HOSTrequest);
    //                web.LoadHtml(HOSTresponse.Content);
    //                var skuNumberPath = "//*[@id='productSku']";

    //                if (web.DocumentNode.SelectSingleNode(skuNumberPath) != null)
    //                {
    //                    var skuNumber = web.DocumentNode.SelectNodes(skuNumberPath).ToList();
    //                    var regexing = skuNumber[0].OuterHtml;
    //                    regexing = regexing.Replace("<div class=\"row-fluid text--blue text--bold text--wrap\" id=\"productSku\">\n                ", "").Replace("\n            </div>", "");
    //                    if (regexing.Contains("value=\""))
    //                    {
    //                        regexing = regexing.Replace("value=\"", "").Replace("\" ", "");
    //                    }

    //                    if (regexing != "")
    //                    {
    //                        var find = await _skuRepository.FindSkuBySkuCodeName(regexing);
    //                        var skuCodeId = await _skuRepository.InsertingSkuIFNull(/*find,*/ regexing);
    //                    }
    //                }
    //                //var AddProd = await _pService.InsertDmProduct(MatchingProducts, categoryId, gtin, ProductId);
    //                //if (sizeId != 0)
    //                //{
    //                //    AddProd.SizeId = sizeId;
    //                //    await _save.SaveAsync();
    //                //}

    //            }
    //            #endregion

    //        }
    //        catch (Exception ex)
    //        {
    //            ex.Message.ToString();
    //            if (LoadingStatus.CurrentAmount != LoadingStatus.TotalAmount)
    //            {
    //                LoadingStatus.Status = "ERROR";
    //                LoadingStatus.SuccessDate = DateTime.Now;
    //                await _save.SaveAsync();
    //            }

    //        }
    //        return "OK";
    //    }
    }
    
    
    #region

    //public class ParseHelperInstance8
    //{
    //    private readonly ICategoryRepository _categoryRepository;
    //    private readonly IProductRepository _productRepository;
    //    private readonly IProductService _pService;
    //    private readonly IUniqueProductsRepository _uniqueProductRepository;
    //    private readonly ISizeRepository _sizeRepository;
    //    private readonly ISkuRepository _skuRepository;
    //    private readonly ISaveRepository _save;
    //    private readonly IStatusRepository _statusRepository;
    //    private readonly FioRinoBaseContext _context;

    //    public ParseHelperInstance8(ICategoryRepository categoryRepository, IUniqueProductsRepository uniqueProductRepository, ISizeRepository sizeRepository, ISkuRepository skuRepository, ISaveRepository save, IProductService pService, IProductRepository productRepository, IStatusRepository statusRepository, FioRinoBaseContext context)
    //    {
    //        _categoryRepository = categoryRepository;
    //        _uniqueProductRepository = uniqueProductRepository;
    //        _sizeRepository = sizeRepository;
    //        _skuRepository = skuRepository;
    //        _save = save;
    //        _pService = pService;
    //        _productRepository = productRepository;
    //        _statusRepository = statusRepository;
    //        _context = context;
    //    }
    //    public async Task<string> Pool(RestClient RstClientNew, string xrf, string laravelsession, HtmlAgilityPack.HtmlDocument web, DmDownloadingStatus originalEntity, IRestResponse PostResponse, int linkCount, RestRequest PostRequest)
    //    {
    //        RstClientNew.Timeout = -1;
    //        var RestRequestNew = new RestRequest(Method.GET);
    //        RestRequestNew.AddHeader("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //        IRestResponse rsponseCookie = RstClientNew.Execute(RestRequestNew);
    //        web.LoadHtml(rsponseCookie.Content);
    //        var pageFind = "//div[@class='form-group']";
    //        var page = web.DocumentNode.SelectNodes(pageFind).ToList();

    //        string CurrentAmountString = Regex.Match(page[2].InnerHtml, @"\d+").Value;
    //        int CurrentAmount = 0;
    //        Int32.TryParse(CurrentAmountString, out CurrentAmount);
    //        int TotalAmount = 0;
    //        var TotalAmountString = page[2].InnerHtml.Replace($"\n            <br>\n            ({CurrentAmount}-{CurrentAmount}\n            z\n            ", "").Replace(")\n        ", "");
    //        Int32.TryParse(TotalAmountString, out TotalAmount);
    //        var LoadingStatus = await _statusRepository.CreateStatusIfNull(CurrentAmount, TotalAmount);
    //        if (originalEntity.Status == "STOPPED")
    //        {
    //            var updating = await _statusRepository.GetFirst();
    //            updating.CurrentAmount = 0;
    //            //updating.PocessIsKilled = false;
    //            updating.TotalAmount = 0;
    //            updating.Status = originalEntity.Status;
    //            await _context.SaveChangesAsync();
    //            return "STOPPED";
    //        }
    //        if (CurrentAmount == TotalAmount)
    //        {
    //            var statusSelect = await _statusRepository.GetFirst();
    //            statusSelect.Status = "SUCCESS";
    //            statusSelect.SuccessDate = DateTime.Now;
    //            await _save.SaveAsync();
    //        }


    //        var skuN = rsponseCookie.Content.Contains("https://mojegs1.pl/moje-produkty/edycja/");
    //        foreach (var cookie in PostResponse.Cookies)
    //        {
    //            PostRequest.AddCookie(cookie.Name, cookie.Value);
    //        }
    //        web.LoadHtml(rsponseCookie.Content);
    //        //linkCount++;
    //        var xpath = "//tr";
    //        var category = await _categoryRepository.CreateCategoryWithListReturn();
    //        var nocategory = category[0];
    //        var classicCategory = category[1];
    //        var fasterCategory = category[2];
    //        try
    //        {

    //            #region
    //            foreach (var item in web.DocumentNode.SelectNodes(xpath).ToList().Skip(1))
    //            {
    //                var items = item.ChildNodes;
    //                var gtin = items[5].InnerText.Trim();
    //                var isClassicCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "classic");
    //                var isFasterCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "faster");
    //                var productName = isClassicCategory ? items[3].InnerText.Trim().Replace("CLASSIC", " ") :
    //                                  isFasterCategory ? items[3].InnerText.Trim().Replace("FASTER", " ") :
    //                                  items[3].InnerText.Trim();
    //                var ProductFullName = items[3].InnerText.Trim();
    //                string output;

    //                var ProdName = productName.Contains("rozm.") ? productName.Replace("rozm.", " ") : productName.Replace("r.", "");
    //                //ProdName = Regex.Replace(productName, @"[\0-9]", " ");
    //                var ProdNameWithUpperSlash = ProdName.Split(" ").Last().Contains("-");

    //                if (!ProdName.Contains("cm") && !ProdName.Contains("MET") && !ProdNameWithUpperSlash == true)
    //                {
    //                    output = Regex.Replace(ProdName, @"[\0-9]", " ");
    //                }
    //                else
    //                {
    //                    output = productName;
    //                }
    //                var categoryId = (isClassicCategory ? classicCategory.Id :
    //                        (isFasterCategory ? fasterCategory.Id : nocategory.Id));
    //                #region EANCodesDonwloading
    //                //foreach (var gtinImages in gting)
    //                //{
    //                //    using (HttpClient cl = new HttpClient())
    //                //    {
    //                //        cl.DefaultRequestHeaders.Add("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //                //        string url = $"https://mojegs1.pl/moje-produkty/pobierz-etykiete?gtin={gtinImages}&label_text={PorductFullName}&size=1&extension=png";
    //                //        using (HttpResponseMessage MessageResponse = await cl.GetAsync(url))
    //                //        using (Stream streamToReadFrom = await MessageResponse.Content.ReadAsStreamAsync())
    //                //        {
    //                //            string fileToWriteTo = Path.GetFullPath($"PdfCodes/{gtinImages}.png");
    //                //            using (Stream streamToWriteTo = System.IO.File.Open(fileToWriteTo, FileMode.Create))
    //                //            {
    //                //                await streamToReadFrom.CopyToAsync(streamToWriteTo);
    //                //            }
    //                //        }
    //                //    }
    //                //}
    //                #endregion
    //                var MatchingProducts = output.Replace("    ", "");
    //                string FindSizeAlphabet;
    //                var containProduct = await _uniqueProductRepository.FindUniqueProductByName(MatchingProducts);
    //                int ProductId = await _uniqueProductRepository.InsertUniqueProductIfNull(/*containProduct,*/ MatchingProducts);
    //                var splitLastIndex = ProductFullName.Split(" ").Last();
    //                if (MatchingProducts.Split(" ").Last() == "M" ||
    //                     MatchingProducts.Split(" ").Last() == "S" ||
    //                     MatchingProducts.Split(" ").Last() == "L" ||
    //                     MatchingProducts.Split(" ").Last() == "XL" ||
    //                     MatchingProducts.Split(" ").Last() == "XS" ||
    //                     MatchingProducts.Split(" ").Last() == "2XL")
    //                {
    //                    FindSizeAlphabet = MatchingProducts.Split(" ").Last();
    //                    var SizeAlphabet = MatchingProducts.LastIndexOf(" ");
    //                    if (SizeAlphabet > 0)
    //                        MatchingProducts = MatchingProducts.Substring(0, SizeAlphabet);
    //                }
    //                else
    //                {
    //                    FindSizeAlphabet = null;
    //                }
    //                int SizeNum = 0;
    //                if (!splitLastIndex.Contains("cm") && !splitLastIndex.Contains("-"))
    //                {
    //                    var resultString = Regex.Match(splitLastIndex, @"\d+").Value;
    //                    Int32.TryParse(resultString, out SizeNum);
    //                }

    //                var findSize = await _sizeRepository.FindSizeByNumber(SizeNum);
    //                var sizeId = await _sizeRepository.CreateSizeIfNull(findSize, SizeNum, FindSizeAlphabet);

    //                var skuTaking = "//div/table/tbody/tr//td/a[@href]";
    //                var webload = web.DocumentNode.SelectNodes(skuTaking).ToList();
    //                var address = webload[0].OuterHtml;
    //                address = address.Replace("<a href=\"", "");
    //                var addressTrimming = address.LastIndexOf("\"");
    //                if (addressTrimming > 0)
    //                    address = address.Substring(0, addressTrimming);
    //                var HOSTclient = new RestClient(address);
    //                HOSTclient.Timeout = -1;
    //                var HOSTrequest = new RestRequest(Method.GET);
    //                foreach (var cookie in rsponseCookie.Cookies)
    //                {
    //                    HOSTrequest.AddCookie(cookie.Name, cookie.Value);
    //                }
    //                IRestResponse HOSTresponse = HOSTclient.Execute(HOSTrequest);
    //                web.LoadHtml(HOSTresponse.Content);
    //                var skuNumberPath = "//*[@id='productSku']";

    //                if (web.DocumentNode.SelectSingleNode(skuNumberPath) != null)
    //                {
    //                    var skuNumber = web.DocumentNode.SelectNodes(skuNumberPath).ToList();
    //                    var regexing = skuNumber[0].OuterHtml;
    //                    regexing = regexing.Replace("<div class=\"row-fluid text--blue text--bold text--wrap\" id=\"productSku\">\n                ", "").Replace("\n            </div>", "");
    //                    if (regexing.Contains("value=\""))
    //                    {
    //                        regexing = regexing.Replace("value=\"", "").Replace("\" ", "");
    //                    }

    //                    if (regexing != "")
    //                    {
    //                        var find = await _skuRepository.FindSkuBySkuCodeName(regexing);
    //                        var skuCodeId = await _skuRepository.InsertingSkuIFNull(/*find,*/ regexing);
    //                    }
    //                }
    //                //var AddProd = await _pService.InsertDmProduct(MatchingProducts, categoryId, gtin, ProductId);
    //                //if (sizeId != 0)
    //                //{
    //                //    AddProd.SizeId = sizeId;
    //                //    await _save.SaveAsync();
    //                //}

    //            }
    //            #endregion

    //        }
    //        catch (Exception ex)
    //        {
    //            ex.Message.ToString();
    //            if (LoadingStatus.CurrentAmount != LoadingStatus.TotalAmount)
    //            {
    //                LoadingStatus.Status = "ERROR";
    //                LoadingStatus.SuccessDate = DateTime.Now;
    //                await _save.SaveAsync();
    //            }

    //        }
    //        return "OK";
    //    }
    //}
    //public class ParseHelperInstance9
    //{
    //    private readonly ICategoryRepository _categoryRepository;
    //    private readonly IProductRepository _productRepository;
    //    private readonly IProductService _pService;
    //    private readonly IUniqueProductsRepository _uniqueProductRepository;
    //    private readonly ISizeRepository _sizeRepository;
    //    private readonly ISkuRepository _skuRepository;
    //    private readonly ISaveRepository _save;
    //    private readonly IStatusRepository _statusRepository;
    //    private readonly FioRinoBaseContext _context;

    //    public ParseHelperInstance9(ICategoryRepository categoryRepository, IUniqueProductsRepository uniqueProductRepository, ISizeRepository sizeRepository, ISkuRepository skuRepository, ISaveRepository save, IProductService pService, IProductRepository productRepository, IStatusRepository statusRepository, FioRinoBaseContext context)
    //    {
    //        _categoryRepository = categoryRepository;
    //        _uniqueProductRepository = uniqueProductRepository;
    //        _sizeRepository = sizeRepository;
    //        _skuRepository = skuRepository;
    //        _save = save;
    //        _pService = pService;
    //        _productRepository = productRepository;
    //        _statusRepository = statusRepository;
    //        _context = context;
    //    }
    //    public async Task<string> Pool(RestClient RstClientNew, string xrf, string laravelsession, HtmlAgilityPack.HtmlDocument web, DmDownloadingStatus originalEntity, IRestResponse PostResponse, int linkCount, RestRequest PostRequest)
    //    {
    //        RstClientNew.Timeout = -1;
    //        var RestRequestNew = new RestRequest(Method.GET);
    //        RestRequestNew.AddHeader("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //        IRestResponse rsponseCookie = RstClientNew.Execute(RestRequestNew);
    //        web.LoadHtml(rsponseCookie.Content);
    //        var pageFind = "//div[@class='form-group']";
    //        var page = web.DocumentNode.SelectNodes(pageFind).ToList();

    //        string CurrentAmountString = Regex.Match(page[2].InnerHtml, @"\d+").Value;
    //        int CurrentAmount = 0;
    //        Int32.TryParse(CurrentAmountString, out CurrentAmount);
    //        int TotalAmount = 0;
    //        var TotalAmountString = page[2].InnerHtml.Replace($"\n            <br>\n            ({CurrentAmount}-{CurrentAmount}\n            z\n            ", "").Replace(")\n        ", "");
    //        Int32.TryParse(TotalAmountString, out TotalAmount);
    //        var LoadingStatus = await _statusRepository.CreateStatusIfNull(CurrentAmount, TotalAmount);
    //        if (originalEntity.Status == "STOPPED")
    //        {
    //            var updating = await _statusRepository.GetFirst();
    //            updating.CurrentAmount = 0;
    //            //updating.PocessIsKilled = false;
    //            updating.TotalAmount = 0;
    //            updating.Status = originalEntity.Status;
    //            await _context.SaveChangesAsync();
    //            return "STOPPED";
    //        }
    //        if (CurrentAmount == TotalAmount)
    //        {
    //            var statusSelect = await _statusRepository.GetFirst();
    //            statusSelect.Status = "SUCCESS";
    //            statusSelect.SuccessDate = DateTime.Now;
    //            await _save.SaveAsync();
    //        }


    //        var skuN = rsponseCookie.Content.Contains("https://mojegs1.pl/moje-produkty/edycja/");
    //        foreach (var cookie in PostResponse.Cookies)
    //        {
    //            PostRequest.AddCookie(cookie.Name, cookie.Value);
    //        }
    //        web.LoadHtml(rsponseCookie.Content);
    //        //linkCount++;
    //        var xpath = "//tr";
    //        var category = await _categoryRepository.CreateCategoryWithListReturn();
    //        var nocategory = category[0];
    //        var classicCategory = category[1];
    //        var fasterCategory = category[2];
    //        try
    //        {

    //            #region
    //            foreach (var item in web.DocumentNode.SelectNodes(xpath).ToList().Skip(1))
    //            {
    //                var items = item.ChildNodes;
    //                var gtin = items[5].InnerText.Trim();
    //                var isClassicCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "classic");
    //                var isFasterCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "faster");
    //                var productName = isClassicCategory ? items[3].InnerText.Trim().Replace("CLASSIC", " ") :
    //                                  isFasterCategory ? items[3].InnerText.Trim().Replace("FASTER", " ") :
    //                                  items[3].InnerText.Trim();
    //                var ProductFullName = items[3].InnerText.Trim();
    //                string output;

    //                var ProdName = productName.Contains("rozm.") ? productName.Replace("rozm.", " ") : productName.Replace("r.", "");
    //                //ProdName = Regex.Replace(productName, @"[\0-9]", " ");
    //                var ProdNameWithUpperSlash = ProdName.Split(" ").Last().Contains("-");

    //                if (!ProdName.Contains("cm") && !ProdName.Contains("MET") && !ProdNameWithUpperSlash == true)
    //                {
    //                    output = Regex.Replace(ProdName, @"[\0-9]", " ");
    //                }
    //                else
    //                {
    //                    output = productName;
    //                }
    //                var categoryId = (isClassicCategory ? classicCategory.Id :
    //                        (isFasterCategory ? fasterCategory.Id : nocategory.Id));
    //                #region EANCodesDonwloading
    //                //foreach (var gtinImages in gting)
    //                //{
    //                //    using (HttpClient cl = new HttpClient())
    //                //    {
    //                //        cl.DefaultRequestHeaders.Add("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //                //        string url = $"https://mojegs1.pl/moje-produkty/pobierz-etykiete?gtin={gtinImages}&label_text={PorductFullName}&size=1&extension=png";
    //                //        using (HttpResponseMessage MessageResponse = await cl.GetAsync(url))
    //                //        using (Stream streamToReadFrom = await MessageResponse.Content.ReadAsStreamAsync())
    //                //        {
    //                //            string fileToWriteTo = Path.GetFullPath($"PdfCodes/{gtinImages}.png");
    //                //            using (Stream streamToWriteTo = System.IO.File.Open(fileToWriteTo, FileMode.Create))
    //                //            {
    //                //                await streamToReadFrom.CopyToAsync(streamToWriteTo);
    //                //            }
    //                //        }
    //                //    }
    //                //}
    //                #endregion
    //                var MatchingProducts = output.Replace("    ", "");
    //                string FindSizeAlphabet;
    //                var containProduct = await _uniqueProductRepository.FindUniqueProductByName(MatchingProducts);
    //                int ProductId = await _uniqueProductRepository.InsertUniqueProductIfNull(/*containProduct, */MatchingProducts);
    //                var splitLastIndex = ProductFullName.Split(" ").Last();
    //                if (MatchingProducts.Split(" ").Last() == "M" ||
    //                     MatchingProducts.Split(" ").Last() == "S" ||
    //                     MatchingProducts.Split(" ").Last() == "L" ||
    //                     MatchingProducts.Split(" ").Last() == "XL" ||
    //                     MatchingProducts.Split(" ").Last() == "XS" ||
    //                     MatchingProducts.Split(" ").Last() == "2XL")
    //                {
    //                    FindSizeAlphabet = MatchingProducts.Split(" ").Last();
    //                    var SizeAlphabet = MatchingProducts.LastIndexOf(" ");
    //                    if (SizeAlphabet > 0)
    //                        MatchingProducts = MatchingProducts.Substring(0, SizeAlphabet);
    //                }
    //                else
    //                {
    //                    FindSizeAlphabet = null;
    //                }
    //                int SizeNum = 0;
    //                if (!splitLastIndex.Contains("cm") && !splitLastIndex.Contains("-"))
    //                {
    //                    var resultString = Regex.Match(splitLastIndex, @"\d+").Value;
    //                    Int32.TryParse(resultString, out SizeNum);
    //                }

    //                var findSize = await _sizeRepository.FindSizeByNumber(SizeNum);
    //                var sizeId = await _sizeRepository.CreateSizeIfNull(findSize, SizeNum, FindSizeAlphabet);

    //                var skuTaking = "//div/table/tbody/tr//td/a[@href]";
    //                var webload = web.DocumentNode.SelectNodes(skuTaking).ToList();
    //                var address = webload[0].OuterHtml;
    //                address = address.Replace("<a href=\"", "");
    //                var addressTrimming = address.LastIndexOf("\"");
    //                if (addressTrimming > 0)
    //                    address = address.Substring(0, addressTrimming);
    //                var HOSTclient = new RestClient(address);
    //                HOSTclient.Timeout = -1;
    //                var HOSTrequest = new RestRequest(Method.GET);
    //                foreach (var cookie in rsponseCookie.Cookies)
    //                {
    //                    HOSTrequest.AddCookie(cookie.Name, cookie.Value);
    //                }
    //                IRestResponse HOSTresponse = HOSTclient.Execute(HOSTrequest);
    //                web.LoadHtml(HOSTresponse.Content);
    //                var skuNumberPath = "//*[@id='productSku']";

    //                if (web.DocumentNode.SelectSingleNode(skuNumberPath) != null)
    //                {
    //                    var skuNumber = web.DocumentNode.SelectNodes(skuNumberPath).ToList();
    //                    var regexing = skuNumber[0].OuterHtml;
    //                    regexing = regexing.Replace("<div class=\"row-fluid text--blue text--bold text--wrap\" id=\"productSku\">\n                ", "").Replace("\n            </div>", "");
    //                    if (regexing.Contains("value=\""))
    //                    {
    //                        regexing = regexing.Replace("value=\"", "").Replace("\" ", "");
    //                    }

    //                    if (regexing != "")
    //                    {
    //                        var find = await _skuRepository.FindSkuBySkuCodeName(regexing);
    //                        var skuCodeId = await _skuRepository.InsertingSkuIFNull(/*find,*/ regexing);
    //                    }
    //                }
    //                //var AddProd = await _pService.InsertDmProduct(MatchingProducts, categoryId, gtin, ProductId);
    //                //if (sizeId != 0)
    //                //{
    //                //    AddProd.SizeId = sizeId;
    //                //    await _save.SaveAsync();
    //                //}

    //            }
    //            #endregion

    //        }
    //        catch (Exception ex)
    //        {
    //            ex.Message.ToString();
    //            if (LoadingStatus.CurrentAmount != LoadingStatus.TotalAmount)
    //            {
    //                LoadingStatus.Status = "ERROR";
    //                LoadingStatus.SuccessDate = DateTime.Now;
    //                await _save.SaveAsync();
    //            }

    //        }
    //        return "OK";
    //    }
    //}
    //public class ParseHelperInstance10
    //{
    //    private readonly ICategoryRepository _categoryRepository;
    //    private readonly IProductRepository _productRepository;
    //    private readonly IProductService _pService;
    //    private readonly IUniqueProductsRepository _uniqueProductRepository;
    //    private readonly ISizeRepository _sizeRepository;
    //    private readonly ISkuRepository _skuRepository;
    //    private readonly ISaveRepository _save;
    //    private readonly IStatusRepository _statusRepository;
    //    private readonly FioRinoBaseContext _context;

    //    public ParseHelperInstance10(ICategoryRepository categoryRepository, IUniqueProductsRepository uniqueProductRepository, ISizeRepository sizeRepository, ISkuRepository skuRepository, ISaveRepository save, IProductService pService, IProductRepository productRepository, IStatusRepository statusRepository, FioRinoBaseContext context)
    //    {
    //        _categoryRepository = categoryRepository;
    //        _uniqueProductRepository = uniqueProductRepository;
    //        _sizeRepository = sizeRepository;
    //        _skuRepository = skuRepository;
    //        _save = save;
    //        _pService = pService;
    //        _productRepository = productRepository;
    //        _statusRepository = statusRepository;
    //        _context = context;
    //    }
    //    public async Task<string> Pool(RestClient RstClientNew, string xrf, string laravelsession, HtmlAgilityPack.HtmlDocument web, DmDownloadingStatus originalEntity, IRestResponse PostResponse, int linkCount, RestRequest PostRequest)
    //    {
    //        RstClientNew.Timeout = -1;
    //        var RestRequestNew = new RestRequest(Method.GET);
    //        RestRequestNew.AddHeader("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //        IRestResponse rsponseCookie = RstClientNew.Execute(RestRequestNew);
    //        web.LoadHtml(rsponseCookie.Content);
    //        var pageFind = "//div[@class='form-group']";
    //        var page = web.DocumentNode.SelectNodes(pageFind).ToList();

    //        string CurrentAmountString = Regex.Match(page[2].InnerHtml, @"\d+").Value;
    //        int CurrentAmount = 0;
    //        Int32.TryParse(CurrentAmountString, out CurrentAmount);
    //        int TotalAmount = 0;
    //        var TotalAmountString = page[2].InnerHtml.Replace($"\n            <br>\n            ({CurrentAmount}-{CurrentAmount}\n            z\n            ", "").Replace(")\n        ", "");
    //        Int32.TryParse(TotalAmountString, out TotalAmount);
    //        var LoadingStatus = await _statusRepository.CreateStatusIfNull(CurrentAmount, TotalAmount);
    //        if (originalEntity.Status == "STOPPED")
    //        {
    //            var updating = await _statusRepository.GetFirst();
    //            updating.CurrentAmount = 0;
    //            //updating.PocessIsKilled = false;
    //            updating.TotalAmount = 0;
    //            updating.Status = originalEntity.Status;
    //            await _context.SaveChangesAsync();
    //            return "STOPPED";
    //        }
    //        if (CurrentAmount == TotalAmount)
    //        {
    //            var statusSelect = await _statusRepository.GetFirst();
    //            statusSelect.Status = "SUCCESS";
    //            statusSelect.SuccessDate = DateTime.Now;
    //            await _save.SaveAsync();
    //        }


    //        var skuN = rsponseCookie.Content.Contains("https://mojegs1.pl/moje-produkty/edycja/");
    //        foreach (var cookie in PostResponse.Cookies)
    //        {
    //            PostRequest.AddCookie(cookie.Name, cookie.Value);
    //        }
    //        web.LoadHtml(rsponseCookie.Content);
    //        //linkCount++;
    //        var xpath = "//tr";
    //        var category = await _categoryRepository.CreateCategoryWithListReturn();
    //        var nocategory = category[0];
    //        var classicCategory = category[1];
    //        var fasterCategory = category[2];
    //        try
    //        {

    //            #region
    //            foreach (var item in web.DocumentNode.SelectNodes(xpath).ToList().Skip(1))
    //            {
    //                var items = item.ChildNodes;
    //                var gtin = items[5].InnerText.Trim();
    //                var isClassicCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "classic");
    //                var isFasterCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "faster");
    //                var productName = isClassicCategory ? items[3].InnerText.Trim().Replace("CLASSIC", " ") :
    //                                  isFasterCategory ? items[3].InnerText.Trim().Replace("FASTER", " ") :
    //                                  items[3].InnerText.Trim();
    //                var ProductFullName = items[3].InnerText.Trim();
    //                string output;

    //                var ProdName = productName.Contains("rozm.") ? productName.Replace("rozm.", " ") : productName.Replace("r.", "");
    //                //ProdName = Regex.Replace(productName, @"[\0-9]", " ");
    //                var ProdNameWithUpperSlash = ProdName.Split(" ").Last().Contains("-");

    //                if (!ProdName.Contains("cm") && !ProdName.Contains("MET") && !ProdNameWithUpperSlash == true)
    //                {
    //                    output = Regex.Replace(ProdName, @"[\0-9]", " ");
    //                }
    //                else
    //                {
    //                    output = productName;
    //                }
    //                var categoryId = (isClassicCategory ? classicCategory.Id :
    //                        (isFasterCategory ? fasterCategory.Id : nocategory.Id));
    //                #region EANCodesDonwloading
    //                //foreach (var gtinImages in gting)
    //                //{
    //                //    using (HttpClient cl = new HttpClient())
    //                //    {
    //                //        cl.DefaultRequestHeaders.Add("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //                //        string url = $"https://mojegs1.pl/moje-produkty/pobierz-etykiete?gtin={gtinImages}&label_text={PorductFullName}&size=1&extension=png";
    //                //        using (HttpResponseMessage MessageResponse = await cl.GetAsync(url))
    //                //        using (Stream streamToReadFrom = await MessageResponse.Content.ReadAsStreamAsync())
    //                //        {
    //                //            string fileToWriteTo = Path.GetFullPath($"PdfCodes/{gtinImages}.png");
    //                //            using (Stream streamToWriteTo = System.IO.File.Open(fileToWriteTo, FileMode.Create))
    //                //            {
    //                //                await streamToReadFrom.CopyToAsync(streamToWriteTo);
    //                //            }
    //                //        }
    //                //    }
    //                //}
    //                #endregion
    //                var MatchingProducts = output.Replace("    ", "");
    //                string FindSizeAlphabet;
    //                var containProduct = await _uniqueProductRepository.FindUniqueProductByName(MatchingProducts);
    //                int ProductId = await _uniqueProductRepository.InsertUniqueProductIfNull(/*containProduct, */MatchingProducts);
    //                var splitLastIndex = ProductFullName.Split(" ").Last();
    //                if (MatchingProducts.Split(" ").Last() == "M" ||
    //                     MatchingProducts.Split(" ").Last() == "S" ||
    //                     MatchingProducts.Split(" ").Last() == "L" ||
    //                     MatchingProducts.Split(" ").Last() == "XL" ||
    //                     MatchingProducts.Split(" ").Last() == "XS" ||
    //                     MatchingProducts.Split(" ").Last() == "2XL")
    //                {
    //                    FindSizeAlphabet = MatchingProducts.Split(" ").Last();
    //                    var SizeAlphabet = MatchingProducts.LastIndexOf(" ");
    //                    if (SizeAlphabet > 0)
    //                        MatchingProducts = MatchingProducts.Substring(0, SizeAlphabet);
    //                }
    //                else
    //                {
    //                    FindSizeAlphabet = null;
    //                }
    //                int SizeNum = 0;
    //                if (!splitLastIndex.Contains("cm") && !splitLastIndex.Contains("-"))
    //                {
    //                    var resultString = Regex.Match(splitLastIndex, @"\d+").Value;
    //                    Int32.TryParse(resultString, out SizeNum);
    //                }

    //                var findSize = await _sizeRepository.FindSizeByNumber(SizeNum);
    //                var sizeId = await _sizeRepository.CreateSizeIfNull(findSize, SizeNum, FindSizeAlphabet);

    //                var skuTaking = "//div/table/tbody/tr//td/a[@href]";
    //                var webload = web.DocumentNode.SelectNodes(skuTaking).ToList();
    //                var address = webload[0].OuterHtml;
    //                address = address.Replace("<a href=\"", "");
    //                var addressTrimming = address.LastIndexOf("\"");
    //                if (addressTrimming > 0)
    //                    address = address.Substring(0, addressTrimming);
    //                var HOSTclient = new RestClient(address);
    //                HOSTclient.Timeout = -1;
    //                var HOSTrequest = new RestRequest(Method.GET);
    //                foreach (var cookie in rsponseCookie.Cookies)
    //                {
    //                    HOSTrequest.AddCookie(cookie.Name, cookie.Value);
    //                }
    //                IRestResponse HOSTresponse = HOSTclient.Execute(HOSTrequest);
    //                web.LoadHtml(HOSTresponse.Content);
    //                var skuNumberPath = "//*[@id='productSku']";

    //                if (web.DocumentNode.SelectSingleNode(skuNumberPath) != null)
    //                {
    //                    var skuNumber = web.DocumentNode.SelectNodes(skuNumberPath).ToList();
    //                    var regexing = skuNumber[0].OuterHtml;
    //                    regexing = regexing.Replace("<div class=\"row-fluid text--blue text--bold text--wrap\" id=\"productSku\">\n                ", "").Replace("\n            </div>", "");
    //                    if (regexing.Contains("value=\""))
    //                    {
    //                        regexing = regexing.Replace("value=\"", "").Replace("\" ", "");
    //                    }

    //                    if (regexing != "")
    //                    {
    //                        var find = await _skuRepository.FindSkuBySkuCodeName(regexing);
    //                        var skuCodeId = await _skuRepository.InsertingSkuIFNull(/*find,*/ regexing);
    //                    }
    //                }
    //                //var AddProd = await _pService.InsertDmProduct(MatchingProducts, categoryId, gtin, ProductId);
    //                //if (sizeId != 0)
    //                //{
    //                //    AddProd.SizeId = sizeId;
    //                //    await _save.SaveAsync();
    //                //}

    //            }
    //            #endregion

    //        }
    //        catch (Exception ex)
    //        {
    //            ex.Message.ToString();
    //            if (LoadingStatus.CurrentAmount != LoadingStatus.TotalAmount)
    //            {
    //                LoadingStatus.Status = "ERROR";
    //                LoadingStatus.SuccessDate = DateTime.Now;
    //                await _save.SaveAsync();
    //            }

    //        }
    //        return "OK";
    //    }
    //}
    //public class ParseHelperInstance11
    //{
    //    private readonly ICategoryRepository _categoryRepository;
    //    private readonly IProductRepository _productRepository;
    //    private readonly IProductService _pService;
    //    private readonly IUniqueProductsRepository _uniqueProductRepository;
    //    private readonly ISizeRepository _sizeRepository;
    //    private readonly ISkuRepository _skuRepository;
    //    private readonly ISaveRepository _save;
    //    private readonly IStatusRepository _statusRepository;
    //    private readonly FioRinoBaseContext _context;

    //    public ParseHelperInstance11(ICategoryRepository categoryRepository, IUniqueProductsRepository uniqueProductRepository, ISizeRepository sizeRepository, ISkuRepository skuRepository, ISaveRepository save, IProductService pService, IProductRepository productRepository, IStatusRepository statusRepository, FioRinoBaseContext context)
    //    {
    //        _categoryRepository = categoryRepository;
    //        _uniqueProductRepository = uniqueProductRepository;
    //        _sizeRepository = sizeRepository;
    //        _skuRepository = skuRepository;
    //        _save = save;
    //        _pService = pService;
    //        _productRepository = productRepository;
    //        _statusRepository = statusRepository;
    //        _context = context;
    //    }
    //    public async Task<string> Pool(RestClient RstClientNew, string xrf, string laravelsession, HtmlAgilityPack.HtmlDocument web, DmDownloadingStatus originalEntity, IRestResponse PostResponse, int linkCount, RestRequest PostRequest)
    //    {
    //        RstClientNew.Timeout = -1;
    //        var RestRequestNew = new RestRequest(Method.GET);
    //        RestRequestNew.AddHeader("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //        IRestResponse rsponseCookie = RstClientNew.Execute(RestRequestNew);
    //        web.LoadHtml(rsponseCookie.Content);
    //        var pageFind = "//div[@class='form-group']";
    //        var page = web.DocumentNode.SelectNodes(pageFind).ToList();

    //        string CurrentAmountString = Regex.Match(page[2].InnerHtml, @"\d+").Value;
    //        int CurrentAmount = 0;
    //        Int32.TryParse(CurrentAmountString, out CurrentAmount);
    //        int TotalAmount = 0;
    //        var TotalAmountString = page[2].InnerHtml.Replace($"\n            <br>\n            ({CurrentAmount}-{CurrentAmount}\n            z\n            ", "").Replace(")\n        ", "");
    //        Int32.TryParse(TotalAmountString, out TotalAmount);
    //        var LoadingStatus = await _statusRepository.CreateStatusIfNull(CurrentAmount, TotalAmount);
    //        if (originalEntity.Status == "STOPPED")
    //        {
    //            var updating = await _statusRepository.GetFirst();
    //            updating.CurrentAmount = 0;
    //            //updating.PocessIsKilled = false;
    //            updating.TotalAmount = 0;
    //            updating.Status = originalEntity.Status;
    //            await _context.SaveChangesAsync();
    //            return "STOPPED";
    //        }
    //        if (CurrentAmount == TotalAmount)
    //        {
    //            var statusSelect = await _statusRepository.GetFirst();
    //            statusSelect.Status = "SUCCESS";
    //            statusSelect.SuccessDate = DateTime.Now;
    //            await _save.SaveAsync();
    //        }


    //        var skuN = rsponseCookie.Content.Contains("https://mojegs1.pl/moje-produkty/edycja/");
    //        foreach (var cookie in PostResponse.Cookies)
    //        {
    //            PostRequest.AddCookie(cookie.Name, cookie.Value);
    //        }
    //        web.LoadHtml(rsponseCookie.Content);
    //        //linkCount++;
    //        var xpath = "//tr";
    //        var category = await _categoryRepository.CreateCategoryWithListReturn();
    //        var nocategory = category[0];
    //        var classicCategory = category[1];
    //        var fasterCategory = category[2];
    //        try
    //        {

    //            #region
    //            foreach (var item in web.DocumentNode.SelectNodes(xpath).ToList().Skip(1))
    //            {
    //                var items = item.ChildNodes;
    //                var gtin = items[5].InnerText.Trim();
    //                var isClassicCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "classic");
    //                var isFasterCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "faster");
    //                var productName = isClassicCategory ? items[3].InnerText.Trim().Replace("CLASSIC", " ") :
    //                                  isFasterCategory ? items[3].InnerText.Trim().Replace("FASTER", " ") :
    //                                  items[3].InnerText.Trim();
    //                var ProductFullName = items[3].InnerText.Trim();
    //                string output;

    //                var ProdName = productName.Contains("rozm.") ? productName.Replace("rozm.", " ") : productName.Replace("r.", "");
    //                //ProdName = Regex.Replace(productName, @"[\0-9]", " ");
    //                var ProdNameWithUpperSlash = ProdName.Split(" ").Last().Contains("-");

    //                if (!ProdName.Contains("cm") && !ProdName.Contains("MET") && !ProdNameWithUpperSlash == true)
    //                {
    //                    output = Regex.Replace(ProdName, @"[\0-9]", " ");
    //                }
    //                else
    //                {
    //                    output = productName;
    //                }
    //                var categoryId = (isClassicCategory ? classicCategory.Id :
    //                        (isFasterCategory ? fasterCategory.Id : nocategory.Id));
    //                #region EANCodesDonwloading
    //                //foreach (var gtinImages in gting)
    //                //{
    //                //    using (HttpClient cl = new HttpClient())
    //                //    {
    //                //        cl.DefaultRequestHeaders.Add("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //                //        string url = $"https://mojegs1.pl/moje-produkty/pobierz-etykiete?gtin={gtinImages}&label_text={PorductFullName}&size=1&extension=png";
    //                //        using (HttpResponseMessage MessageResponse = await cl.GetAsync(url))
    //                //        using (Stream streamToReadFrom = await MessageResponse.Content.ReadAsStreamAsync())
    //                //        {
    //                //            string fileToWriteTo = Path.GetFullPath($"PdfCodes/{gtinImages}.png");
    //                //            using (Stream streamToWriteTo = System.IO.File.Open(fileToWriteTo, FileMode.Create))
    //                //            {
    //                //                await streamToReadFrom.CopyToAsync(streamToWriteTo);
    //                //            }
    //                //        }
    //                //    }
    //                //}
    //                #endregion
    //                var MatchingProducts = output.Replace("    ", "");
    //                string FindSizeAlphabet;
    //                var containProduct = await _uniqueProductRepository.FindUniqueProductByName(MatchingProducts);
    //                int ProductId = await _uniqueProductRepository.InsertUniqueProductIfNull(/*containProduct,*/ MatchingProducts);
    //                var splitLastIndex = ProductFullName.Split(" ").Last();
    //                if (MatchingProducts.Split(" ").Last() == "M" ||
    //                     MatchingProducts.Split(" ").Last() == "S" ||
    //                     MatchingProducts.Split(" ").Last() == "L" ||
    //                     MatchingProducts.Split(" ").Last() == "XL" ||
    //                     MatchingProducts.Split(" ").Last() == "XS" ||
    //                     MatchingProducts.Split(" ").Last() == "2XL")
    //                {
    //                    FindSizeAlphabet = MatchingProducts.Split(" ").Last();
    //                    var SizeAlphabet = MatchingProducts.LastIndexOf(" ");
    //                    if (SizeAlphabet > 0)
    //                        MatchingProducts = MatchingProducts.Substring(0, SizeAlphabet);
    //                }
    //                else
    //                {
    //                    FindSizeAlphabet = null;
    //                }
    //                int SizeNum = 0;
    //                if (!splitLastIndex.Contains("cm") && !splitLastIndex.Contains("-"))
    //                {
    //                    var resultString = Regex.Match(splitLastIndex, @"\d+").Value;
    //                    Int32.TryParse(resultString, out SizeNum);
    //                }

    //                var findSize = await _sizeRepository.FindSizeByNumber(SizeNum);
    //                var sizeId = await _sizeRepository.CreateSizeIfNull(findSize, SizeNum, FindSizeAlphabet);

    //                var skuTaking = "//div/table/tbody/tr//td/a[@href]";
    //                var webload = web.DocumentNode.SelectNodes(skuTaking).ToList();
    //                var address = webload[0].OuterHtml;
    //                address = address.Replace("<a href=\"", "");
    //                var addressTrimming = address.LastIndexOf("\"");
    //                if (addressTrimming > 0)
    //                    address = address.Substring(0, addressTrimming);
    //                var HOSTclient = new RestClient(address);
    //                HOSTclient.Timeout = -1;
    //                var HOSTrequest = new RestRequest(Method.GET);
    //                foreach (var cookie in rsponseCookie.Cookies)
    //                {
    //                    HOSTrequest.AddCookie(cookie.Name, cookie.Value);
    //                }
    //                IRestResponse HOSTresponse = HOSTclient.Execute(HOSTrequest);
    //                web.LoadHtml(HOSTresponse.Content);
    //                var skuNumberPath = "//*[@id='productSku']";

    //                if (web.DocumentNode.SelectSingleNode(skuNumberPath) != null)
    //                {
    //                    var skuNumber = web.DocumentNode.SelectNodes(skuNumberPath).ToList();
    //                    var regexing = skuNumber[0].OuterHtml;
    //                    regexing = regexing.Replace("<div class=\"row-fluid text--blue text--bold text--wrap\" id=\"productSku\">\n                ", "").Replace("\n            </div>", "");
    //                    if (regexing.Contains("value=\""))
    //                    {
    //                        regexing = regexing.Replace("value=\"", "").Replace("\" ", "");
    //                    }

    //                    if (regexing != "")
    //                    {
    //                        var find = await _skuRepository.FindSkuBySkuCodeName(regexing);
    //                        var skuCodeId = await _skuRepository.InsertingSkuIFNull(/*find,*/ regexing);
    //                    }
    //                }
    //                //var AddProd = await _pService.InsertDmProduct(MatchingProducts, categoryId, gtin, ProductId);
    //                //if (sizeId != 0)
    //                //{
    //                //    AddProd.SizeId = sizeId;
    //                //    await _save.SaveAsync();
    //                //}

    //            }
    //            #endregion

    //        }
    //        catch (Exception ex)
    //        {
    //            ex.Message.ToString();
    //            if (LoadingStatus.CurrentAmount != LoadingStatus.TotalAmount)
    //            {
    //                LoadingStatus.Status = "ERROR";
    //                LoadingStatus.SuccessDate = DateTime.Now;
    //                await _save.SaveAsync();
    //            }

    //        }
    //        return "OK";
    //    }
    //}
    //public class ParseHelperInstance12
    //{
    //    private readonly ICategoryRepository _categoryRepository;
    //    private readonly IProductRepository _productRepository;
    //    private readonly IProductService _pService;
    //    private readonly IUniqueProductsRepository _uniqueProductRepository;
    //    private readonly ISizeRepository _sizeRepository;
    //    private readonly ISkuRepository _skuRepository;
    //    private readonly ISaveRepository _save;
    //    private readonly IStatusRepository _statusRepository;
    //    private readonly FioRinoBaseContext _context;

    //    public ParseHelperInstance12(ICategoryRepository categoryRepository, IUniqueProductsRepository uniqueProductRepository, ISizeRepository sizeRepository, ISkuRepository skuRepository, ISaveRepository save, IProductService pService, IProductRepository productRepository, IStatusRepository statusRepository, FioRinoBaseContext context)
    //    {
    //        _categoryRepository = categoryRepository;
    //        _uniqueProductRepository = uniqueProductRepository;
    //        _sizeRepository = sizeRepository;
    //        _skuRepository = skuRepository;
    //        _save = save;
    //        _pService = pService;
    //        _productRepository = productRepository;
    //        _statusRepository = statusRepository;
    //        _context = context;
    //    }
    //    public async Task<string> Pool(RestClient RstClientNew, string xrf, string laravelsession, HtmlAgilityPack.HtmlDocument web, DmDownloadingStatus originalEntity, IRestResponse PostResponse, int linkCount, RestRequest PostRequest)
    //    {
    //        RstClientNew.Timeout = -1;
    //        var RestRequestNew = new RestRequest(Method.GET);
    //        RestRequestNew.AddHeader("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //        IRestResponse rsponseCookie = RstClientNew.Execute(RestRequestNew);
    //        web.LoadHtml(rsponseCookie.Content);
    //        var pageFind = "//div[@class='form-group']";
    //        var page = web.DocumentNode.SelectNodes(pageFind).ToList();

    //        string CurrentAmountString = Regex.Match(page[2].InnerHtml, @"\d+").Value;
    //        int CurrentAmount = 0;
    //        Int32.TryParse(CurrentAmountString, out CurrentAmount);
    //        int TotalAmount = 0;
    //        var TotalAmountString = page[2].InnerHtml.Replace($"\n            <br>\n            ({CurrentAmount}-{CurrentAmount}\n            z\n            ", "").Replace(")\n        ", "");
    //        Int32.TryParse(TotalAmountString, out TotalAmount);
    //        var LoadingStatus = await _statusRepository.CreateStatusIfNull(CurrentAmount, TotalAmount);
    //        if (originalEntity.Status == "STOPPED")
    //        {
    //            var updating = await _statusRepository.GetFirst();
    //            updating.CurrentAmount = 0;
    //            //updating.PocessIsKilled = false;
    //            updating.TotalAmount = 0;
    //            updating.Status = originalEntity.Status;
    //            await _context.SaveChangesAsync();
    //            return "STOPPED";
    //        }
    //        if (CurrentAmount == TotalAmount)
    //        {
    //            var statusSelect = await _statusRepository.GetFirst();
    //            statusSelect.Status = "SUCCESS";
    //            statusSelect.SuccessDate = DateTime.Now;
    //            await _save.SaveAsync();
    //        }


    //        var skuN = rsponseCookie.Content.Contains("https://mojegs1.pl/moje-produkty/edycja/");
    //        foreach (var cookie in PostResponse.Cookies)
    //        {
    //            PostRequest.AddCookie(cookie.Name, cookie.Value);
    //        }
    //        web.LoadHtml(rsponseCookie.Content);
    //        //linkCount++;
    //        var xpath = "//tr";
    //        var category = await _categoryRepository.CreateCategoryWithListReturn();
    //        var nocategory = category[0];
    //        var classicCategory = category[1];
    //        var fasterCategory = category[2];
    //        try
    //        {

    //            #region
    //            foreach (var item in web.DocumentNode.SelectNodes(xpath).ToList().Skip(1))
    //            {
    //                var items = item.ChildNodes;
    //                var gtin = items[5].InnerText.Trim();
    //                var isClassicCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "classic");
    //                var isFasterCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "faster");
    //                var productName = isClassicCategory ? items[3].InnerText.Trim().Replace("CLASSIC", " ") :
    //                                  isFasterCategory ? items[3].InnerText.Trim().Replace("FASTER", " ") :
    //                                  items[3].InnerText.Trim();
    //                var ProductFullName = items[3].InnerText.Trim();
    //                string output;

    //                var ProdName = productName.Contains("rozm.") ? productName.Replace("rozm.", " ") : productName.Replace("r.", "");
    //                //ProdName = Regex.Replace(productName, @"[\0-9]", " ");
    //                var ProdNameWithUpperSlash = ProdName.Split(" ").Last().Contains("-");

    //                if (!ProdName.Contains("cm") && !ProdName.Contains("MET") && !ProdNameWithUpperSlash == true)
    //                {
    //                    output = Regex.Replace(ProdName, @"[\0-9]", " ");
    //                }
    //                else
    //                {
    //                    output = productName;
    //                }
    //                var categoryId = (isClassicCategory ? classicCategory.Id :
    //                        (isFasterCategory ? fasterCategory.Id : nocategory.Id));
    //                #region EANCodesDonwloading
    //                //foreach (var gtinImages in gting)
    //                //{
    //                //    using (HttpClient cl = new HttpClient())
    //                //    {
    //                //        cl.DefaultRequestHeaders.Add("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //                //        string url = $"https://mojegs1.pl/moje-produkty/pobierz-etykiete?gtin={gtinImages}&label_text={PorductFullName}&size=1&extension=png";
    //                //        using (HttpResponseMessage MessageResponse = await cl.GetAsync(url))
    //                //        using (Stream streamToReadFrom = await MessageResponse.Content.ReadAsStreamAsync())
    //                //        {
    //                //            string fileToWriteTo = Path.GetFullPath($"PdfCodes/{gtinImages}.png");
    //                //            using (Stream streamToWriteTo = System.IO.File.Open(fileToWriteTo, FileMode.Create))
    //                //            {
    //                //                await streamToReadFrom.CopyToAsync(streamToWriteTo);
    //                //            }
    //                //        }
    //                //    }
    //                //}
    //                #endregion
    //                var MatchingProducts = output.Replace("    ", "");
    //                string FindSizeAlphabet;
    //                var containProduct = await _uniqueProductRepository.FindUniqueProductByName(MatchingProducts);
    //                int ProductId = await _uniqueProductRepository.InsertUniqueProductIfNull(/*containProduct,*/ MatchingProducts);
    //                var splitLastIndex = ProductFullName.Split(" ").Last();
    //                if (MatchingProducts.Split(" ").Last() == "M" ||
    //                     MatchingProducts.Split(" ").Last() == "S" ||
    //                     MatchingProducts.Split(" ").Last() == "L" ||
    //                     MatchingProducts.Split(" ").Last() == "XL" ||
    //                     MatchingProducts.Split(" ").Last() == "XS" ||
    //                     MatchingProducts.Split(" ").Last() == "2XL")
    //                {
    //                    FindSizeAlphabet = MatchingProducts.Split(" ").Last();
    //                    var SizeAlphabet = MatchingProducts.LastIndexOf(" ");
    //                    if (SizeAlphabet > 0)
    //                        MatchingProducts = MatchingProducts.Substring(0, SizeAlphabet);
    //                }
    //                else
    //                {
    //                    FindSizeAlphabet = null;
    //                }
    //                int SizeNum = 0;
    //                if (!splitLastIndex.Contains("cm") && !splitLastIndex.Contains("-"))
    //                {
    //                    var resultString = Regex.Match(splitLastIndex, @"\d+").Value;
    //                    Int32.TryParse(resultString, out SizeNum);
    //                }

    //                var findSize = await _sizeRepository.FindSizeByNumber(SizeNum);
    //                var sizeId = await _sizeRepository.CreateSizeIfNull(findSize, SizeNum, FindSizeAlphabet);

    //                var skuTaking = "//div/table/tbody/tr//td/a[@href]";
    //                var webload = web.DocumentNode.SelectNodes(skuTaking).ToList();
    //                var address = webload[0].OuterHtml;
    //                address = address.Replace("<a href=\"", "");
    //                var addressTrimming = address.LastIndexOf("\"");
    //                if (addressTrimming > 0)
    //                    address = address.Substring(0, addressTrimming);
    //                var HOSTclient = new RestClient(address);
    //                HOSTclient.Timeout = -1;
    //                var HOSTrequest = new RestRequest(Method.GET);
    //                foreach (var cookie in rsponseCookie.Cookies)
    //                {
    //                    HOSTrequest.AddCookie(cookie.Name, cookie.Value);
    //                }
    //                IRestResponse HOSTresponse = HOSTclient.Execute(HOSTrequest);
    //                web.LoadHtml(HOSTresponse.Content);
    //                var skuNumberPath = "//*[@id='productSku']";

    //                if (web.DocumentNode.SelectSingleNode(skuNumberPath) != null)
    //                {
    //                    var skuNumber = web.DocumentNode.SelectNodes(skuNumberPath).ToList();
    //                    var regexing = skuNumber[0].OuterHtml;
    //                    regexing = regexing.Replace("<div class=\"row-fluid text--blue text--bold text--wrap\" id=\"productSku\">\n                ", "").Replace("\n            </div>", "");
    //                    if (regexing.Contains("value=\""))
    //                    {
    //                        regexing = regexing.Replace("value=\"", "").Replace("\" ", "");
    //                    }

    //                    if (regexing != "")
    //                    {
    //                        var find = await _skuRepository.FindSkuBySkuCodeName(regexing);
    //                        var skuCodeId = await _skuRepository.InsertingSkuIFNull(/*find,*/ regexing);
    //                    }
    //                }
    //                //var AddProd = await _pService.InsertDmProduct(MatchingProducts, categoryId, gtin, ProductId);
    //                //if (sizeId != 0)
    //                //{
    //                //    AddProd.SizeId = sizeId;
    //                //    await _save.SaveAsync();
    //                //}

    //            }
    //            #endregion

    //        }
    //        catch (Exception ex)
    //        {
    //            ex.Message.ToString();
    //            if (LoadingStatus.CurrentAmount != LoadingStatus.TotalAmount)
    //            {
    //                LoadingStatus.Status = "ERROR";
    //                LoadingStatus.SuccessDate = DateTime.Now;
    //                await _save.SaveAsync();
    //            }

    //        }
    //        return "OK";
    //    }
    //}
    //public class ParseHelperInstance13
    //{
    //    private readonly ICategoryRepository _categoryRepository;
    //    private readonly IProductRepository _productRepository;
    //    private readonly IProductService _pService;
    //    private readonly IUniqueProductsRepository _uniqueProductRepository;
    //    private readonly ISizeRepository _sizeRepository;
    //    private readonly ISkuRepository _skuRepository;
    //    private readonly ISaveRepository _save;
    //    private readonly IStatusRepository _statusRepository;
    //    private readonly FioRinoBaseContext _context;

    //    public ParseHelperInstance13(ICategoryRepository categoryRepository, IUniqueProductsRepository uniqueProductRepository, ISizeRepository sizeRepository, ISkuRepository skuRepository, ISaveRepository save, IProductService pService, IProductRepository productRepository, IStatusRepository statusRepository, FioRinoBaseContext context)
    //    {
    //        _categoryRepository = categoryRepository;
    //        _uniqueProductRepository = uniqueProductRepository;
    //        _sizeRepository = sizeRepository;
    //        _skuRepository = skuRepository;
    //        _save = save;
    //        _pService = pService;
    //        _productRepository = productRepository;
    //        _statusRepository = statusRepository;
    //        _context = context;
    //    }
    //    public async Task<string> Pool(RestClient RstClientNew, string xrf, string laravelsession, HtmlAgilityPack.HtmlDocument web, DmDownloadingStatus originalEntity, IRestResponse PostResponse, int linkCount, RestRequest PostRequest)
    //    {
    //        RstClientNew.Timeout = -1;
    //        var RestRequestNew = new RestRequest(Method.GET);
    //        RestRequestNew.AddHeader("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //        IRestResponse rsponseCookie = RstClientNew.Execute(RestRequestNew);
    //        web.LoadHtml(rsponseCookie.Content);
    //        var pageFind = "//div[@class='form-group']";
    //        var page = web.DocumentNode.SelectNodes(pageFind).ToList();

    //        string CurrentAmountString = Regex.Match(page[2].InnerHtml, @"\d+").Value;
    //        int CurrentAmount = 0;
    //        Int32.TryParse(CurrentAmountString, out CurrentAmount);
    //        int TotalAmount = 0;
    //        var TotalAmountString = page[2].InnerHtml.Replace($"\n            <br>\n            ({CurrentAmount}-{CurrentAmount}\n            z\n            ", "").Replace(")\n        ", "");
    //        Int32.TryParse(TotalAmountString, out TotalAmount);
    //        var LoadingStatus = await _statusRepository.CreateStatusIfNull(CurrentAmount, TotalAmount);
    //        if (originalEntity.Status == "STOPPED")
    //        {
    //            var updating = await _statusRepository.GetFirst();
    //            updating.CurrentAmount = 0;
    //            //updating.PocessIsKilled = false;
    //            updating.TotalAmount = 0;
    //            updating.Status = originalEntity.Status;
    //            await _context.SaveChangesAsync();
    //            return "STOPPED";
    //        }
    //        if (CurrentAmount == TotalAmount)
    //        {
    //            var statusSelect = await _statusRepository.GetFirst();
    //            statusSelect.Status = "SUCCESS";
    //            statusSelect.SuccessDate = DateTime.Now;
    //            await _save.SaveAsync();
    //        }


    //        var skuN = rsponseCookie.Content.Contains("https://mojegs1.pl/moje-produkty/edycja/");
    //        foreach (var cookie in PostResponse.Cookies)
    //        {
    //            PostRequest.AddCookie(cookie.Name, cookie.Value);
    //        }
    //        web.LoadHtml(rsponseCookie.Content);
    //        //linkCount++;
    //        var xpath = "//tr";
    //        var category = await _categoryRepository.CreateCategoryWithListReturn();
    //        var nocategory = category[0];
    //        var classicCategory = category[1];
    //        var fasterCategory = category[2];
    //        try
    //        {

    //            #region
    //            foreach (var item in web.DocumentNode.SelectNodes(xpath).ToList().Skip(1))
    //            {
    //                var items = item.ChildNodes;
    //                var gtin = items[5].InnerText.Trim();
    //                var isClassicCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "classic");
    //                var isFasterCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "faster");
    //                var productName = isClassicCategory ? items[3].InnerText.Trim().Replace("CLASSIC", " ") :
    //                                  isFasterCategory ? items[3].InnerText.Trim().Replace("FASTER", " ") :
    //                                  items[3].InnerText.Trim();
    //                var ProductFullName = items[3].InnerText.Trim();
    //                string output;

    //                var ProdName = productName.Contains("rozm.") ? productName.Replace("rozm.", " ") : productName.Replace("r.", "");
    //                //ProdName = Regex.Replace(productName, @"[\0-9]", " ");
    //                var ProdNameWithUpperSlash = ProdName.Split(" ").Last().Contains("-");

    //                if (!ProdName.Contains("cm") && !ProdName.Contains("MET") && !ProdNameWithUpperSlash == true)
    //                {
    //                    output = Regex.Replace(ProdName, @"[\0-9]", " ");
    //                }
    //                else
    //                {
    //                    output = productName;
    //                }
    //                var categoryId = (isClassicCategory ? classicCategory.Id :
    //                        (isFasterCategory ? fasterCategory.Id : nocategory.Id));
    //                #region EANCodesDonwloading
    //                //foreach (var gtinImages in gting)
    //                //{
    //                //    using (HttpClient cl = new HttpClient())
    //                //    {
    //                //        cl.DefaultRequestHeaders.Add("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //                //        string url = $"https://mojegs1.pl/moje-produkty/pobierz-etykiete?gtin={gtinImages}&label_text={PorductFullName}&size=1&extension=png";
    //                //        using (HttpResponseMessage MessageResponse = await cl.GetAsync(url))
    //                //        using (Stream streamToReadFrom = await MessageResponse.Content.ReadAsStreamAsync())
    //                //        {
    //                //            string fileToWriteTo = Path.GetFullPath($"PdfCodes/{gtinImages}.png");
    //                //            using (Stream streamToWriteTo = System.IO.File.Open(fileToWriteTo, FileMode.Create))
    //                //            {
    //                //                await streamToReadFrom.CopyToAsync(streamToWriteTo);
    //                //            }
    //                //        }
    //                //    }
    //                //}
    //                #endregion
    //                var MatchingProducts = output.Replace("    ", "");
    //                string FindSizeAlphabet;
    //                var containProduct = await _uniqueProductRepository.FindUniqueProductByName(MatchingProducts);
    //                int ProductId = await _uniqueProductRepository.InsertUniqueProductIfNull(/*containProduct, */MatchingProducts);
    //                var splitLastIndex = ProductFullName.Split(" ").Last();
    //                if (MatchingProducts.Split(" ").Last() == "M" ||
    //                     MatchingProducts.Split(" ").Last() == "S" ||
    //                     MatchingProducts.Split(" ").Last() == "L" ||
    //                     MatchingProducts.Split(" ").Last() == "XL" ||
    //                     MatchingProducts.Split(" ").Last() == "XS" ||
    //                     MatchingProducts.Split(" ").Last() == "2XL")
    //                {
    //                    FindSizeAlphabet = MatchingProducts.Split(" ").Last();
    //                    var SizeAlphabet = MatchingProducts.LastIndexOf(" ");
    //                    if (SizeAlphabet > 0)
    //                        MatchingProducts = MatchingProducts.Substring(0, SizeAlphabet);
    //                }
    //                else
    //                {
    //                    FindSizeAlphabet = null;
    //                }
    //                int SizeNum = 0;
    //                if (!splitLastIndex.Contains("cm") && !splitLastIndex.Contains("-"))
    //                {
    //                    var resultString = Regex.Match(splitLastIndex, @"\d+").Value;
    //                    Int32.TryParse(resultString, out SizeNum);
    //                }

    //                var findSize = await _sizeRepository.FindSizeByNumber(SizeNum);
    //                var sizeId = await _sizeRepository.CreateSizeIfNull(findSize, SizeNum, FindSizeAlphabet);

    //                var skuTaking = "//div/table/tbody/tr//td/a[@href]";
    //                var webload = web.DocumentNode.SelectNodes(skuTaking).ToList();
    //                var address = webload[0].OuterHtml;
    //                address = address.Replace("<a href=\"", "");
    //                var addressTrimming = address.LastIndexOf("\"");
    //                if (addressTrimming > 0)
    //                    address = address.Substring(0, addressTrimming);
    //                var HOSTclient = new RestClient(address);
    //                HOSTclient.Timeout = -1;
    //                var HOSTrequest = new RestRequest(Method.GET);
    //                foreach (var cookie in rsponseCookie.Cookies)
    //                {
    //                    HOSTrequest.AddCookie(cookie.Name, cookie.Value);
    //                }
    //                IRestResponse HOSTresponse = HOSTclient.Execute(HOSTrequest);
    //                web.LoadHtml(HOSTresponse.Content);
    //                var skuNumberPath = "//*[@id='productSku']";

    //                if (web.DocumentNode.SelectSingleNode(skuNumberPath) != null)
    //                {
    //                    var skuNumber = web.DocumentNode.SelectNodes(skuNumberPath).ToList();
    //                    var regexing = skuNumber[0].OuterHtml;
    //                    regexing = regexing.Replace("<div class=\"row-fluid text--blue text--bold text--wrap\" id=\"productSku\">\n                ", "").Replace("\n            </div>", "");
    //                    if (regexing.Contains("value=\""))
    //                    {
    //                        regexing = regexing.Replace("value=\"", "").Replace("\" ", "");
    //                    }

    //                    if (regexing != "")
    //                    {
    //                        var find = await _skuRepository.FindSkuBySkuCodeName(regexing);
    //                        var skuCodeId = await _skuRepository.InsertingSkuIFNull(/*find,*/ regexing);
    //                    }
    //                }
    //                //var AddProd = await _pService.InsertDmProduct(MatchingProducts, categoryId, gtin, ProductId);
    //                //if (sizeId != 0)
    //                //{
    //                //    AddProd.SizeId = sizeId;
    //                //    await _save.SaveAsync();
    //                //}

    //            }
    //            #endregion

    //        }
    //        catch (Exception ex)
    //        {
    //            ex.Message.ToString();
    //            if (LoadingStatus.CurrentAmount != LoadingStatus.TotalAmount)
    //            {
    //                LoadingStatus.Status = "ERROR";
    //                LoadingStatus.SuccessDate = DateTime.Now;
    //                await _save.SaveAsync();
    //            }

    //        }
    //        return "OK";
    //    }
    //}
    //public class ParseHelperInstance14
    //{
    //    private readonly ICategoryRepository _categoryRepository;
    //    private readonly IProductRepository _productRepository;
    //    private readonly IProductService _pService;
    //    private readonly IUniqueProductsRepository _uniqueProductRepository;
    //    private readonly ISizeRepository _sizeRepository;
    //    private readonly ISkuRepository _skuRepository;
    //    private readonly ISaveRepository _save;
    //    private readonly IStatusRepository _statusRepository;
    //    private readonly FioRinoBaseContext _context;

    //    public ParseHelperInstance14(ICategoryRepository categoryRepository, IUniqueProductsRepository uniqueProductRepository, ISizeRepository sizeRepository, ISkuRepository skuRepository, ISaveRepository save, IProductService pService, IProductRepository productRepository, IStatusRepository statusRepository, FioRinoBaseContext context)
    //    {
    //        _categoryRepository = categoryRepository;
    //        _uniqueProductRepository = uniqueProductRepository;
    //        _sizeRepository = sizeRepository;
    //        _skuRepository = skuRepository;
    //        _save = save;
    //        _pService = pService;
    //        _productRepository = productRepository;
    //        _statusRepository = statusRepository;
    //        _context = context;
    //    }
    //    public async Task<string> Pool(RestClient RstClientNew, string xrf, string laravelsession, HtmlAgilityPack.HtmlDocument web, DmDownloadingStatus originalEntity, IRestResponse PostResponse, int linkCount, RestRequest PostRequest)
    //    {
    //        RstClientNew.Timeout = -1;
    //        var RestRequestNew = new RestRequest(Method.GET);
    //        RestRequestNew.AddHeader("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //        IRestResponse rsponseCookie = RstClientNew.Execute(RestRequestNew);
    //        web.LoadHtml(rsponseCookie.Content);
    //        var pageFind = "//div[@class='form-group']";
    //        var page = web.DocumentNode.SelectNodes(pageFind).ToList();

    //        string CurrentAmountString = Regex.Match(page[2].InnerHtml, @"\d+").Value;
    //        int CurrentAmount = 0;
    //        Int32.TryParse(CurrentAmountString, out CurrentAmount);
    //        int TotalAmount = 0;
    //        var TotalAmountString = page[2].InnerHtml.Replace($"\n            <br>\n            ({CurrentAmount}-{CurrentAmount}\n            z\n            ", "").Replace(")\n        ", "");
    //        Int32.TryParse(TotalAmountString, out TotalAmount);
    //        var LoadingStatus = await _statusRepository.CreateStatusIfNull(CurrentAmount, TotalAmount);
    //        if (originalEntity.Status == "STOPPED")
    //        {
    //            var updating = await _statusRepository.GetFirst();
    //            updating.CurrentAmount = 0;
    //            //updating.PocessIsKilled = false;
    //            updating.TotalAmount = 0;
    //            updating.Status = originalEntity.Status;
    //            await _context.SaveChangesAsync();
    //            return "STOPPED";
    //        }
    //        if (CurrentAmount == TotalAmount)
    //        {
    //            var statusSelect = await _statusRepository.GetFirst();
    //            statusSelect.Status = "SUCCESS";
    //            statusSelect.SuccessDate = DateTime.Now;
    //            await _save.SaveAsync();
    //        }


    //        var skuN = rsponseCookie.Content.Contains("https://mojegs1.pl/moje-produkty/edycja/");
    //        foreach (var cookie in PostResponse.Cookies)
    //        {
    //            PostRequest.AddCookie(cookie.Name, cookie.Value);
    //        }
    //        web.LoadHtml(rsponseCookie.Content);
    //        //linkCount++;
    //        var xpath = "//tr";
    //        var category = await _categoryRepository.CreateCategoryWithListReturn();
    //        var nocategory = category[0];
    //        var classicCategory = category[1];
    //        var fasterCategory = category[2];
    //        try
    //        {

    //            #region
    //            foreach (var item in web.DocumentNode.SelectNodes(xpath).ToList().Skip(1))
    //            {
    //                var items = item.ChildNodes;
    //                var gtin = items[5].InnerText.Trim();
    //                var isClassicCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "classic");
    //                var isFasterCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "faster");
    //                var productName = isClassicCategory ? items[3].InnerText.Trim().Replace("CLASSIC", " ") :
    //                                  isFasterCategory ? items[3].InnerText.Trim().Replace("FASTER", " ") :
    //                                  items[3].InnerText.Trim();
    //                var ProductFullName = items[3].InnerText.Trim();
    //                string output;

    //                var ProdName = productName.Contains("rozm.") ? productName.Replace("rozm.", " ") : productName.Replace("r.", "");
    //                //ProdName = Regex.Replace(productName, @"[\0-9]", " ");
    //                var ProdNameWithUpperSlash = ProdName.Split(" ").Last().Contains("-");

    //                if (!ProdName.Contains("cm") && !ProdName.Contains("MET") && !ProdNameWithUpperSlash == true)
    //                {
    //                    output = Regex.Replace(ProdName, @"[\0-9]", " ");
    //                }
    //                else
    //                {
    //                    output = productName;
    //                }
    //                var categoryId = (isClassicCategory ? classicCategory.Id :
    //                        (isFasterCategory ? fasterCategory.Id : nocategory.Id));
    //                #region EANCodesDonwloading
    //                //foreach (var gtinImages in gting)
    //                //{
    //                //    using (HttpClient cl = new HttpClient())
    //                //    {
    //                //        cl.DefaultRequestHeaders.Add("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //                //        string url = $"https://mojegs1.pl/moje-produkty/pobierz-etykiete?gtin={gtinImages}&label_text={PorductFullName}&size=1&extension=png";
    //                //        using (HttpResponseMessage MessageResponse = await cl.GetAsync(url))
    //                //        using (Stream streamToReadFrom = await MessageResponse.Content.ReadAsStreamAsync())
    //                //        {
    //                //            string fileToWriteTo = Path.GetFullPath($"PdfCodes/{gtinImages}.png");
    //                //            using (Stream streamToWriteTo = System.IO.File.Open(fileToWriteTo, FileMode.Create))
    //                //            {
    //                //                await streamToReadFrom.CopyToAsync(streamToWriteTo);
    //                //            }
    //                //        }
    //                //    }
    //                //}
    //                #endregion
    //                var MatchingProducts = output.Replace("    ", "");
    //                string FindSizeAlphabet;
    //                var containProduct = await _uniqueProductRepository.FindUniqueProductByName(MatchingProducts);
    //                int ProductId = await _uniqueProductRepository.InsertUniqueProductIfNull(/*containProduct,*/ MatchingProducts);
    //                var splitLastIndex = ProductFullName.Split(" ").Last();
    //                if (MatchingProducts.Split(" ").Last() == "M" ||
    //                     MatchingProducts.Split(" ").Last() == "S" ||
    //                     MatchingProducts.Split(" ").Last() == "L" ||
    //                     MatchingProducts.Split(" ").Last() == "XL" ||
    //                     MatchingProducts.Split(" ").Last() == "XS" ||
    //                     MatchingProducts.Split(" ").Last() == "2XL")
    //                {
    //                    FindSizeAlphabet = MatchingProducts.Split(" ").Last();
    //                    var SizeAlphabet = MatchingProducts.LastIndexOf(" ");
    //                    if (SizeAlphabet > 0)
    //                        MatchingProducts = MatchingProducts.Substring(0, SizeAlphabet);
    //                }
    //                else
    //                {
    //                    FindSizeAlphabet = null;
    //                }
    //                int SizeNum = 0;
    //                if (!splitLastIndex.Contains("cm") && !splitLastIndex.Contains("-"))
    //                {
    //                    var resultString = Regex.Match(splitLastIndex, @"\d+").Value;
    //                    Int32.TryParse(resultString, out SizeNum);
    //                }

    //                var findSize = await _sizeRepository.FindSizeByNumber(SizeNum);
    //                var sizeId = await _sizeRepository.CreateSizeIfNull(findSize, SizeNum, FindSizeAlphabet);

    //                var skuTaking = "//div/table/tbody/tr//td/a[@href]";
    //                var webload = web.DocumentNode.SelectNodes(skuTaking).ToList();
    //                var address = webload[0].OuterHtml;
    //                address = address.Replace("<a href=\"", "");
    //                var addressTrimming = address.LastIndexOf("\"");
    //                if (addressTrimming > 0)
    //                    address = address.Substring(0, addressTrimming);
    //                var HOSTclient = new RestClient(address);
    //                HOSTclient.Timeout = -1;
    //                var HOSTrequest = new RestRequest(Method.GET);
    //                foreach (var cookie in rsponseCookie.Cookies)
    //                {
    //                    HOSTrequest.AddCookie(cookie.Name, cookie.Value);
    //                }
    //                IRestResponse HOSTresponse = HOSTclient.Execute(HOSTrequest);
    //                web.LoadHtml(HOSTresponse.Content);
    //                var skuNumberPath = "//*[@id='productSku']";

    //                if (web.DocumentNode.SelectSingleNode(skuNumberPath) != null)
    //                {
    //                    var skuNumber = web.DocumentNode.SelectNodes(skuNumberPath).ToList();
    //                    var regexing = skuNumber[0].OuterHtml;
    //                    regexing = regexing.Replace("<div class=\"row-fluid text--blue text--bold text--wrap\" id=\"productSku\">\n                ", "").Replace("\n            </div>", "");
    //                    if (regexing.Contains("value=\""))
    //                    {
    //                        regexing = regexing.Replace("value=\"", "").Replace("\" ", "");
    //                    }

    //                    if (regexing != "")
    //                    {
    //                        var find = await _skuRepository.FindSkuBySkuCodeName(regexing);
    //                        var skuCodeId = await _skuRepository.InsertingSkuIFNull(/*find,*/ regexing);
    //                    }
    //                }
    //                //var AddProd = await _pService.InsertDmProduct(MatchingProducts, categoryId, gtin, ProductId);
    //                //if (sizeId != 0)
    //                //{
    //                //    AddProd.SizeId = sizeId;
    //                //    await _save.SaveAsync();
    //                //}

    //            }
    //            #endregion

    //        }
    //        catch (Exception ex)
    //        {
    //            ex.Message.ToString();
    //            if (LoadingStatus.CurrentAmount != LoadingStatus.TotalAmount)
    //            {
    //                LoadingStatus.Status = "ERROR";
    //                LoadingStatus.SuccessDate = DateTime.Now;
    //                await _save.SaveAsync();
    //            }

    //        }
    //        return "OK";
    //    }
    //}
    //public class ParseHelperInstance15
    //{
    //    private readonly ICategoryRepository _categoryRepository;
    //    private readonly IProductRepository _productRepository;
    //    private readonly IProductService _pService;
    //    private readonly IUniqueProductsRepository _uniqueProductRepository;
    //    private readonly ISizeRepository _sizeRepository;
    //    private readonly ISkuRepository _skuRepository;
    //    private readonly ISaveRepository _save;
    //    private readonly IStatusRepository _statusRepository;
    //    private readonly FioRinoBaseContext _context;

    //    public ParseHelperInstance15(ICategoryRepository categoryRepository, IUniqueProductsRepository uniqueProductRepository, ISizeRepository sizeRepository, ISkuRepository skuRepository, ISaveRepository save, IProductService pService, IProductRepository productRepository, IStatusRepository statusRepository, FioRinoBaseContext context)
    //    {
    //        _categoryRepository = categoryRepository;
    //        _uniqueProductRepository = uniqueProductRepository;
    //        _sizeRepository = sizeRepository;
    //        _skuRepository = skuRepository;
    //        _save = save;
    //        _pService = pService;
    //        _productRepository = productRepository;
    //        _statusRepository = statusRepository;
    //        _context = context;
    //    }
    //    public async Task<string> Pool(RestClient RstClientNew, string xrf, string laravelsession, HtmlAgilityPack.HtmlDocument web, DmDownloadingStatus originalEntity, IRestResponse PostResponse, int linkCount, RestRequest PostRequest)
    //    {
    //        RstClientNew.Timeout = -1;
    //        var RestRequestNew = new RestRequest(Method.GET);
    //        RestRequestNew.AddHeader("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //        IRestResponse rsponseCookie = RstClientNew.Execute(RestRequestNew);
    //        web.LoadHtml(rsponseCookie.Content);
    //        var pageFind = "//div[@class='form-group']";
    //        var page = web.DocumentNode.SelectNodes(pageFind).ToList();

    //        string CurrentAmountString = Regex.Match(page[2].InnerHtml, @"\d+").Value;
    //        int CurrentAmount = 0;
    //        Int32.TryParse(CurrentAmountString, out CurrentAmount);
    //        int TotalAmount = 0;
    //        var TotalAmountString = page[2].InnerHtml.Replace($"\n            <br>\n            ({CurrentAmount}-{CurrentAmount}\n            z\n            ", "").Replace(")\n        ", "");
    //        Int32.TryParse(TotalAmountString, out TotalAmount);
    //        var LoadingStatus = await _statusRepository.CreateStatusIfNull(CurrentAmount, TotalAmount);
    //        if (originalEntity.Status == "STOPPED")
    //        {
    //            var updating = await _statusRepository.GetFirst();
    //            updating.CurrentAmount = 0;
    //            //updating.PocessIsKilled = false;
    //            updating.TotalAmount = 0;
    //            updating.Status = originalEntity.Status;
    //            await _context.SaveChangesAsync();
    //            return "STOPPED";
    //        }
    //        if (CurrentAmount == TotalAmount)
    //        {
    //            var statusSelect = await _statusRepository.GetFirst();
    //            statusSelect.Status = "SUCCESS";
    //            statusSelect.SuccessDate = DateTime.Now;
    //            await _save.SaveAsync();
    //        }


    //        var skuN = rsponseCookie.Content.Contains("https://mojegs1.pl/moje-produkty/edycja/");
    //        foreach (var cookie in PostResponse.Cookies)
    //        {
    //            PostRequest.AddCookie(cookie.Name, cookie.Value);
    //        }
    //        web.LoadHtml(rsponseCookie.Content);
    //        //linkCount++;
    //        var xpath = "//tr";
    //        var category = await _categoryRepository.CreateCategoryWithListReturn();
    //        var nocategory = category[0];
    //        var classicCategory = category[1];
    //        var fasterCategory = category[2];
    //        try
    //        {

    //            #region
    //            foreach (var item in web.DocumentNode.SelectNodes(xpath).ToList().Skip(1))
    //            {
    //                var items = item.ChildNodes;
    //                var gtin = items[5].InnerText.Trim();
    //                var isClassicCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "classic");
    //                var isFasterCategory = items[3].InnerText.Trim().Split(" ").ToList().Any(x => x.ToLower() == "faster");
    //                var productName = isClassicCategory ? items[3].InnerText.Trim().Replace("CLASSIC", " ") :
    //                                  isFasterCategory ? items[3].InnerText.Trim().Replace("FASTER", " ") :
    //                                  items[3].InnerText.Trim();
    //                var ProductFullName = items[3].InnerText.Trim();
    //                string output;

    //                var ProdName = productName.Contains("rozm.") ? productName.Replace("rozm.", " ") : productName.Replace("r.", "");
    //                //ProdName = Regex.Replace(productName, @"[\0-9]", " ");
    //                var ProdNameWithUpperSlash = ProdName.Split(" ").Last().Contains("-");

    //                if (!ProdName.Contains("cm") && !ProdName.Contains("MET") && !ProdNameWithUpperSlash == true)
    //                {
    //                    output = Regex.Replace(ProdName, @"[\0-9]", " ");
    //                }
    //                else
    //                {
    //                    output = productName;
    //                }
    //                var categoryId = (isClassicCategory ? classicCategory.Id :
    //                        (isFasterCategory ? fasterCategory.Id : nocategory.Id));
    //                #region EANCodesDonwloading
    //                //foreach (var gtinImages in gting)
    //                //{
    //                //    using (HttpClient cl = new HttpClient())
    //                //    {
    //                //        cl.DefaultRequestHeaders.Add("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
    //                //        string url = $"https://mojegs1.pl/moje-produkty/pobierz-etykiete?gtin={gtinImages}&label_text={PorductFullName}&size=1&extension=png";
    //                //        using (HttpResponseMessage MessageResponse = await cl.GetAsync(url))
    //                //        using (Stream streamToReadFrom = await MessageResponse.Content.ReadAsStreamAsync())
    //                //        {
    //                //            string fileToWriteTo = Path.GetFullPath($"PdfCodes/{gtinImages}.png");
    //                //            using (Stream streamToWriteTo = System.IO.File.Open(fileToWriteTo, FileMode.Create))
    //                //            {
    //                //                await streamToReadFrom.CopyToAsync(streamToWriteTo);
    //                //            }
    //                //        }
    //                //    }
    //                //}
    //                #endregion
    //                var MatchingProducts = output.Replace("    ", "");
    //                string FindSizeAlphabet;
    //                var containProduct = await _uniqueProductRepository.FindUniqueProductByName(MatchingProducts);
    //                int ProductId = await _uniqueProductRepository.InsertUniqueProductIfNull(/*containProduct,*/ MatchingProducts);
    //                var splitLastIndex = ProductFullName.Split(" ").Last();
    //                if (MatchingProducts.Split(" ").Last() == "M" ||
    //                     MatchingProducts.Split(" ").Last() == "S" ||
    //                     MatchingProducts.Split(" ").Last() == "L" ||
    //                     MatchingProducts.Split(" ").Last() == "XL" ||
    //                     MatchingProducts.Split(" ").Last() == "XS" ||
    //                     MatchingProducts.Split(" ").Last() == "2XL")
    //                {
    //                    FindSizeAlphabet = MatchingProducts.Split(" ").Last();
    //                    var SizeAlphabet = MatchingProducts.LastIndexOf(" ");
    //                    if (SizeAlphabet > 0)
    //                        MatchingProducts = MatchingProducts.Substring(0, SizeAlphabet);
    //                }
    //                else
    //                {
    //                    FindSizeAlphabet = null;
    //                }
    //                int SizeNum = 0;
    //                if (!splitLastIndex.Contains("cm") && !splitLastIndex.Contains("-"))
    //                {
    //                    var resultString = Regex.Match(splitLastIndex, @"\d+").Value;
    //                    Int32.TryParse(resultString, out SizeNum);
    //                }

    //                var findSize = await _sizeRepository.FindSizeByNumber(SizeNum);
    //                var sizeId = await _sizeRepository.CreateSizeIfNull(findSize, SizeNum, FindSizeAlphabet);

    //                var skuTaking = "//div/table/tbody/tr//td/a[@href]";
    //                var webload = web.DocumentNode.SelectNodes(skuTaking).ToList();
    //                var address = webload[0].OuterHtml;
    //                address = address.Replace("<a href=\"", "");
    //                var addressTrimming = address.LastIndexOf("\"");
    //                if (addressTrimming > 0)
    //                    address = address.Substring(0, addressTrimming);
    //                var HOSTclient = new RestClient(address);
    //                HOSTclient.Timeout = -1;
    //                var HOSTrequest = new RestRequest(Method.GET);
    //                foreach (var cookie in rsponseCookie.Cookies)
    //                {
    //                    HOSTrequest.AddCookie(cookie.Name, cookie.Value);
    //                }
    //                IRestResponse HOSTresponse = HOSTclient.Execute(HOSTrequest);
    //                web.LoadHtml(HOSTresponse.Content);
    //                var skuNumberPath = "//*[@id='productSku']";

    //                if (web.DocumentNode.SelectSingleNode(skuNumberPath) != null)
    //                {
    //                    var skuNumber = web.DocumentNode.SelectNodes(skuNumberPath).ToList();
    //                    var regexing = skuNumber[0].OuterHtml;
    //                    regexing = regexing.Replace("<div class=\"row-fluid text--blue text--bold text--wrap\" id=\"productSku\">\n                ", "").Replace("\n            </div>", "");
    //                    if (regexing.Contains("value=\""))
    //                    {
    //                        regexing = regexing.Replace("value=\"", "").Replace("\" ", "");
    //                    }

    //                    if (regexing != "")
    //                    {
    //                        var find = await _skuRepository.FindSkuBySkuCodeName(regexing);
    //                        var skuCodeId = await _skuRepository.InsertingSkuIFNull(/*find,*/ regexing);
    //                    }
    //                }
    //                //var AddProd = await _pService.InsertDmProduct(MatchingProducts, categoryId, gtin, ProductId);
    //                //if (sizeId != 0)
    //                //{
    //                //    AddProd.SizeId = sizeId;
    //                //    await _save.SaveAsync();
    //                //}

    //            }
    //            #endregion

    //        }
    //        catch (Exception ex)
    //        {
    //            ex.Message.ToString();
    //            if (LoadingStatus.CurrentAmount != LoadingStatus.TotalAmount)
    //            {
    //                LoadingStatus.Status = "ERROR";
    //                LoadingStatus.SuccessDate = DateTime.Now;
    //                await _save.SaveAsync();
    //            }

    //        }
    //        return "OK";
    //    }
   // }
    #endregion


}
