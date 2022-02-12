using FioRino_NewProject.Data;
using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Entities;
using FioRino_NewProject.Model;
using FioRino_NewProject.Repositories;
using FioRino_NewProject.Responses;
using FioRino_NewProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace FioRino_NewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoperController : ControllerBase
    {
        private readonly FioRinoBaseContext _context;
        private readonly ISizeRepository _sizeRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISkuRepository _skuRepository;
        private readonly IParsingExcelService _ParsingService;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderProductsRepository _orderProductsRepository;
        private readonly IStorageRepository _storageRepository;
        private readonly ISaveRepository _save;


        public ShoperController(FioRinoBaseContext context, ISizeRepository sizeRepository, IProductRepository productRepository, ICategoryRepository categoryRepository, ISkuRepository skuRepository, IParsingExcelService parsingService, IOrderRepository orderRepository, IOrderProductsRepository orderProductsRepository, IStorageRepository storageRepository, ISaveRepository save)
        {
            _context = context;
            _sizeRepository = sizeRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _skuRepository = skuRepository;
            _ParsingService = parsingService;
            _orderRepository = orderRepository;
            _orderProductsRepository = orderProductsRepository;
            _storageRepository = storageRepository;
            _save = save;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task ShoperAction()
        {
            var client = new RestClient("https://sklep459600.shoparena.pl/webapi/rest/auth");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Authorization", "Basic UFJPR1JBTURFVEFMOlByb2dyYW0xMjM=");
            request.AddHeader("Cookie", "Shop5=llktdfpferarjc884c6gvv0bsp; admin_ip_verify=d41d8cd98f00b204e9800998ecf8427e");
            IRestResponse response = client.Execute(request);
            ShoperAuthorisationDTO restoredToken = JsonConvert.DeserializeObject<ShoperAuthorisationDTO>(response.Content);
            var accessToken = restoredToken.access_token;

            var shoparenaClient = new RestClient("https://sklep459600.shoparena.pl/webapi/rest/orders?order=date&page=1&limit=20&filters={\"status.type\": 1}");
            shoparenaClient.Timeout = -1;
            var shoparenaRequest = new RestRequest(Method.GET);
            shoparenaRequest.AddHeader("Authorization", $"Bearer {accessToken}");
            shoparenaRequest.AddHeader("Cookie", "Shop5=llktdfpferarjc884c6gvv0bsp; admin_ip_verify=d41d8cd98f00b204e9800998ecf8427e");
            var body = @"";
            shoparenaRequest.AddParameter("text/plain", body, ParameterType.RequestBody);
            IRestResponse shoparenaResponse = shoparenaClient.Execute(shoparenaRequest);
            Root shoperOrdersDTO = JsonConvert.DeserializeObject<Root>(shoparenaResponse.Content);
            var IdList = new List<string>();
            var claim = User.Identity as ClaimsIdentity;
            GetByEmailParams param = new GetByEmailParams();
            List<SPToCoreContext.EXPOSE_dm_Users_GetByEmailResult> user;
            using (SPToCoreContext db = new SPToCoreContext())
            {
                user = await db.EXPOSE_dm_Users_GetByEmailAsync /**/ (claim.Name);
            }
            int? orderId = 0;
            foreach (var Ids in shoperOrdersDTO.list)
            {
                CreateOrderParams parameters = new CreateOrderParams();
                parameters.CreatedAt = DateTime.Today;
                //parameters.UpdatedAt = DateTime;
                /*parameters.SenderName = user[0].FirstName + " " + user[0].LastName;*/
                parameters.OrderStatusId = 1; parameters.Is_InMagazyn = false; parameters.SourceOfOrder = "Shoper"; parameters.OrderExecutor = "FIORINO Izabela Gądek-Pagacz";
                using (SPToCoreContext db = new SPToCoreContext())
                {
                    db.EXPOSE_dm_Orders_CreateOrder /**/ (parameters.CreatedAt, parameters.UpdatedAt, parameters.OrderStatusId, parameters.Is_InMagazyn, parameters.SourceOfOrder, parameters.OrderExecutor, parameters.SenderName, ref orderId);
                }
                var RestClients = new RestClient("https://sklep459600.shoparena.pl/webapi/rest/order-products?order-products=date&page=1&filters={\"order_id\":"+ $"{ Ids.order_id}"+ "}");
                RestClients.Timeout = -1;
                var RestRequest = new RestRequest(Method.GET);
                RestRequest.AddHeader("Authorization", $"Bearer {accessToken}");
                RestRequest.AddHeader("Cookie", "admin_ip_verify=d41d8cd98f00b204e9800998ecf8427e");
                var Restbody = @"";
                RestRequest.AddParameter("text/plain", Restbody, ParameterType.RequestBody);
                IRestResponse RestResponse = RestClients.Execute(RestRequest);
                Console.WriteLine(RestResponse.Content);
                var category = await _categoryRepository.CreateCategoryWithListReturn();
                var nocategory = category[0];
                var classicCategory = category[1];
                var fasterCategory = category[2];
                Root shoperOrders = JsonConvert.DeserializeObject<Root>(RestResponse.Content);
                
                int num = 0;
                while (true)
                {
                    try
                    {
                        var ProductName = shoperOrders.list[num].name;
                        var isClassicCategory = shoperOrders.list[num].option.Trim().Split(" ").ToList().Any(x => x.ToLower() == "classic" || x.ToLower() == "classic+");
                        var isFasterCategory = shoperOrders.list[num].option.Trim().Split(" ").ToList().Any(x => x.ToLower() == "faster" || x.ToLower() == "faster+");
                        var categoryId = (isClassicCategory ? classicCategory.Id :
                                    (isFasterCategory ? fasterCategory.Id : nocategory.Id));
                        var SkuCode = shoperOrders.list[num].code.Replace("SKU ", "");
                        var SkuCodeObject = await _skuRepository.FindSkuBySkuCodeName(SkuCode);
                        var SizeOption = shoperOrders.list[num].option;

                        var sizeTrim = SizeOption.LastIndexOf("(");
                        var sizeTrim2 = SizeOption.LastIndexOf(")");
                        if (sizeTrim > 0)
                            SizeOption = SizeOption.Substring(sizeTrim - 1, sizeTrim2 - sizeTrim + 2);

                        var SizeNumberStr = Regex.Match(SizeOption, @"\d+").Value;
                        int SizeNumber;
                        Int32.TryParse(SizeNumberStr, out SizeNumber);
                        int Amount;
                        Int32.TryParse(shoperOrders.list[num].quantity, out Amount);
                        var Size = await _sizeRepository.FindSizeByNumber(SizeNumber);
                        
                        var ProductTrim = ProductName.LastIndexOf("(");
                        if (ProductTrim > 0)
                            ProductName = ProductName.Substring(0, ProductTrim);
                        DmProduct findProductByParameters = new DmProduct();
                        var checkOrderStatus = await _orderProductsRepository.GetOrderProductListByOrderIdAsync((int)orderId);
                        var FindingProduct = await _productRepository.FindProductByName(ProductName);
                        if (FindingProduct != null)
                        {
                            findProductByParameters = await _productRepository.FindProductByParams((int)FindingProduct.UniqueProductId, categoryId, Size.Id);
                        }
                        else
                        {
                            if (checkOrderStatus.Count == 0)
                            {
                                await _orderRepository.DeleteOrder((int)orderId);
                            }
                            break;
                        }
                        //var findProductByParameters = await _productRepository.FindProductByParams((int)FindingProduct.UniqueProductId, categoryId, Size.Id);
                        
                        //if (FindingProduct == null) break;
                        //ekoTuptusie APLIKACJA ŁAPKI NA KREMIE
                        if (findProductByParameters != null)
                        {
                            var insert = await _ParsingService.InsertProductsToOrderProducts((int)orderId, findProductByParameters.Id, Size.Id, SkuCodeObject.Id, categoryId, Amount, findProductByParameters.Gtin);
                            var findStan = await _storageRepository.FindFromStorageByGtinAsync(FindingProduct.Gtin);
                            if (findStan != null && insert.Amount <= findStan.AmountLeft)
                            {
                                findStan.AmountLeft = findStan.AmountLeft - insert.Amount;
                                insert.ProductStatusesId = 2;
                                await _save.SaveAsync();
                            }
                        }
                        else 
                        {
                            if (checkOrderStatus.Count == 0)
                            {
                                await _orderRepository.DeleteOrder((int)orderId);
                            }
                            break;
                        }
                        num++;
                    }
                    catch (Exception)
                    {
                        var findOrder = await _orderRepository.FindOrder((int)orderId);
                        if (findOrder != null)
                        {
                            findOrder.SenderName = user[0].FirstName + " " + user[0].LastName;
                        }
                        break;
                    }
                }
                //else
                //    return BadRequest(new Responses.Response { Status = "404", Message = "Product does not exists!" });

                IdList.Add(Ids.order_id);
            }
                
            


            Console.WriteLine(shoparenaResponse.Content);
            
        }
        //"{\"access_token\":\"918aa2e663c39e47ac3f4dd2aea4cd91b5e2be05\",\"expires_in\":2592000,\"token_type\":\"bearer\"}"
    }

}


