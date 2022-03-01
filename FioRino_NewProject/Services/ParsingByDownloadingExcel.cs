using FioRino_NewProject.Controllers;
using FioRino_NewProject.Data;
using FioRino_NewProject.Repositories;
using Microsoft.AspNetCore.Hosting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public class ParsingByDownloadingExcel
    {
        private readonly IWebHostEnvironment _environment;
        private readonly FioRinoBaseContext _context;
        private readonly IStatusRepository _statusRepository;

        public ParsingByDownloadingExcel(IWebHostEnvironment environment, FioRinoBaseContext context, IStatusRepository statusRepository)
        {
            _environment = environment;
            _context = context;
            _statusRepository = statusRepository;
        }

        public async Task DownloadZip()
        {
            var rootPath = _environment.WebRootPath;
           // var zipDirectory = rootPath + "\\Zips";
           var zipDirectory = rootPath + "/Zips/";
  
            var originalEntity = await _statusRepository.GetFirst();
            originalEntity.Status = "DOWNLOADING";
            await _context.SaveChangesAsync();
            if (Directory.Exists(zipDirectory))
            {
                System.IO.DirectoryInfo di = new DirectoryInfo($"{zipDirectory}");
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }
            }
            else
                Directory.CreateDirectory(zipDirectory);
            string[] DownloadCheck = Directory.GetFiles($"{zipDirectory}", "*.zip");
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--headless");
            options.AddArgument("test-type");
            options.AddArgument("no-sandbox");
            //if (!Directory.Exists(zipDirectory))
            //    Directory.CreateDirectory(zipDirectory);
            options.AddUserProfilePreference("download.default_directory", zipDirectory);
            options.AddUserProfilePreference("download.prompt_for_download", false);
            options.AddUserProfilePreference("disable-popup-blocking", "true");

            WebDriver driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), options, TimeSpan.FromMinutes(3));
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(120);
            driver.Navigate().GoToUrl("https://mojegs1.pl/logowanie");
            IWebElement element = driver.FindElement(By.Name("email"));
            element.SendKeys("tomek@fiorino.eu");
            element = driver.FindElement(By.Name("password"));
            element.SendKeys("Epoka1-wsx");
            element.Submit();

            element = driver.FindElement(By.Id("CybotCookiebotDialogBodyLevelButtonLevelOptinAllowAll"));
            element.Click();
            element = driver.FindElement(By.XPath("//*[@id='CybotCookiebotDialogBodyLevelButtonLevelOptinAllowAll']"));
            //await Task.Delay(1000);
            element.Click();
            driver.Navigate().GoToUrl("https://mojegs1.pl/moje-produkty");


            element = driver.FindElement(By.Id("productsListExportBtn"));
            element.Click();
            element = driver.FindElement(By.XPath("//*[@id='exportProductsForm']/div[2]/div/div/div[3]/label"));
            await Task.Delay(1000);
            element.Click();

            element = driver.FindElement(By.Id("productsListExportModalAcceptBtn"));
            await Task.Delay(1000);
            element.Click();
            await Task.Delay(1000);

            int num = 0;

            for (int i = 0; ; i++)
            {
                string[] DownloadCheckFormat = Directory.GetFiles($"{zipDirectory}", "*.zip");
                try
                {
                    if (DownloadCheckFormat[0] == DownloadCheckFormat[num])
                    {
                        driver.Close();
                        driver.Dispose();
                        return;
                    }
                }
                catch (Exception)
                {
                    await Task.Delay(1000);
                }
            }
        }

        public async Task<Responses.Response> ParsingProducts()
        {
            var loadingStatus = await _statusRepository.GetFirst();
            if (loadingStatus.Status == "LOADING")
            {
                return new Responses.Response { Status = "Error", Message = "Double click is not allowed!" };
            }
            
            return new Responses.Response { Status = "Ok" };
        }
        //[HttpGet("GetProductAmount")]
        public int GetProductAmount()
        {
            using var client = new HttpClient();
            HtmlAgilityPack.HtmlDocument web = new HtmlAgilityPack.HtmlDocument();
            var UrlAddressClient = new RestClient("https://mojegs1.pl/logowanie");
            UrlAddressClient.Timeout = -1;
            var UrlAddressClientRequest = new RestRequest(Method.GET);
            IRestResponse UrlAddressClientResponse = UrlAddressClient.Execute(UrlAddressClientRequest);
            web.LoadHtml(UrlAddressClientResponse.Content);
            //var cookieId = UrlAddressClientResponse.Headers[4].Value;
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
            var RstClientNew = new RestClient($"https://mojegs1.pl/moje-produkty");
            RstClientNew.Timeout = -1;
            var RestRequestNew = new RestRequest(Method.GET);
            RestRequestNew.AddHeader("Cookie", $"XSRF-TOKEN={xrf}; laravel_session={laravelsession}");
            IRestResponse rsponseCookie = RstClientNew.Execute(RestRequestNew);
            web.LoadHtml(rsponseCookie.Content);
            var pageFind = "//div[@class='form-group']";
            var page = web.DocumentNode.SelectNodes(pageFind).ToList();

            string CurrentAmountString = Regex.Match(page[2].InnerHtml, @"\d+").Value;

            int TotalAmount = 0;
            var TotalAmountString = page[2].InnerHtml.Replace($"\n            <br>\n            (1-50\n            z\n            ", "").Replace(")\n        ", "");
            Int32.TryParse(TotalAmountString, out TotalAmount);
            return TotalAmount;
            //await ZipStatusCheck(TotalAmount);
        } // returns totalAmount


        public async Task ZipStatusCheck(int TotalAmount)
        {
            var rootPath = _environment.WebRootPath;
            var ZipPath = rootPath + "/Zips/";
            //var ZipPath = rootPath + "\\Zips";
            int num = 0;
            for (int i = 0 ; ; i++)
            {
                string[] DownloadCheck = Directory.GetFiles($"{ZipPath}", "*.zip");
                try
                {
                    if (DownloadCheck[0] == DownloadCheck[num])
                    {
                        await UnzipZip(ZipPath, TotalAmount);
                    }
                }
                catch (Exception)
                {
                    await Task.Delay(1000);
                }
            }
        }

        public async Task<(string, int)> UnzipZip(string ZipPath, int TotalAmount)
        {
            try
            {
                var originalEntity = await _statusRepository.GetFirst();
                originalEntity.Status = "UNZIPPING";
                await _context.SaveChangesAsync();
                string[] files = Directory.GetFiles($"{ZipPath}", "*.zip");
                var FileName = files[0].Split('/').Last();
                var ZipAddress = ZipPath + "/" + FileName;
                ZipFile.ExtractToDirectory(ZipAddress, ZipPath);
                System.IO.File.Delete(files[0]);
                //await ExcelFileCheck(ZipPath);
                return (ZipPath, TotalAmount);
            }
            catch (Exception)
            {
                return (ZipPath, TotalAmount);
            }
        }
        public void ExcelFileCheck(string ZipPath)
        {
            string[] XlsxFiles = Directory.GetFiles($"{ZipPath}", "*.xlsx");

            for (int fileLength = 0; fileLength < XlsxFiles.Length; fileLength++)
            {
             
                string filePath = XlsxFiles[fileLength];
                byte[] bytes = System.IO.File.ReadAllBytes(filePath);

                MemoryStream ms = new MemoryStream(bytes);
            }
        }
    } 

}
