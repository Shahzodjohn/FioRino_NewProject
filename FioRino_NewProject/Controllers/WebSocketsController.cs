using FioRino_NewProject.Data;
using FioRino_NewProject.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace FioRino_NewProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebSocketsController : ControllerBase
    {
        private readonly ILogger<WebSocketsController> _logger;
        private readonly FioRinoBaseContext _context;
        public WebSocketsController(ILogger<WebSocketsController> logger, FioRinoBaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("/ws")]
        public async Task Get()
        {

            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                _logger.Log(LogLevel.Information, "WebSocket connection established");
                await Echos(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

        #region
        private async Task ReceiveAsync(WebSocket webSocket)
        {

            var buffer = new ArraySegment<byte>(new byte[2048]);
            var stoppingToken = new CancellationToken();
            WebSocketReceiveResult result;
            while (!stoppingToken.IsCancellationRequested)
            {
                using var memoryStream = new MemoryStream();
                do
                {
                    result = await webSocket.ReceiveAsync(buffer, stoppingToken);
                    memoryStream.Write(buffer.Array, buffer.Offset, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                memoryStream.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(memoryStream, Encoding.UTF8);
                var message = await reader.ReadToEndAsync();
                var encoded = Encoding.UTF8.GetBytes(message);
                var bufferToSend = new ArraySegment<Byte>(encoded, 0, encoded.Length);
                await webSocket.SendAsync(bufferToSend, WebSocketMessageType.Text, true, CancellationToken.None/*, stoppingToken*/);
            };
        }
        #endregion
        //let webSocket = new WebSocket('wss://localhost:44372/ws');
        #region
        private async Task Echo(WebSocket webSocket)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            var stoppingToken = new CancellationToken();
            WebSocketReceiveResult result;
            while (!stoppingToken.IsCancellationRequested)
            {
                using var memoryStream = new MemoryStream();
                do
                {
                    result = await webSocket.ReceiveAsync(buffer, stoppingToken);
                    memoryStream.Write(buffer.Array, buffer.Offset, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                memoryStream.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(memoryStream, Encoding.UTF8);
                var message = await reader.ReadToEndAsync();
                var ms = message;
                //var encoded = Encoding.UTF8.GetBytes(message);
                var NUM = 1;
                for (int i = 0; i < 200; i++)
                {
                    var botResult = Encoding.UTF8.GetBytes($"Server => {message} + {NUM}");
                    NUM++;
                    var bufferToSend = new ArraySegment<Byte>(botResult, 0, botResult.Length);
                    await webSocket.SendAsync(bufferToSend, WebSocketMessageType.Text, true, CancellationToken.None/*, stoppingToken*/);
                }

            }
        }
        #endregion
        private async Task Echos(WebSocket webSocket)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            var stoppingToken = new CancellationToken();
            WebSocketReceiveResult result;
            while (!stoppingToken.IsCancellationRequested)
            {
                using var memoryStream = new MemoryStream();
                do
                {
                    result = await webSocket.ReceiveAsync(buffer, stoppingToken);
                    memoryStream.Write(buffer.Array, buffer.Offset, result.Count);
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                memoryStream.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(memoryStream, Encoding.UTF8);
                var message = await reader.ReadToEndAsync();
                var ms = message;
                //var encoded = Encoding.UTF8.GetBytes(message);
                var NUM = 1;
                var products = await _context.DmProducts.Select(x => x.ProductName).ToListAsync();

                var starting = (from dmpProd in _context.DmProducts
                                join dmSize in _context.DmSizes on dmpProd.SizeId equals dmSize.Id
                                join dmCategory in _context.DmCategories on dmpProd.CategoryId equals dmCategory.Id
                                join dmUniqueProd in _context.DmUniqueProducts on dmpProd.UniqueProductId equals dmUniqueProd.Id
                                select new WSdmProductDTO
                                {
                                    ProductName = dmpProd.ProductName,
                                    CategoryName = dmCategory.CategoryName,
                                    SizeName = dmSize.Number + "(" + dmSize.Title + ")",
                                    UniqueProductId = dmUniqueProd.Id
                                }).ToList();

                foreach (var productName in starting)
                {

                    var botResult = Encoding.UTF8.GetBytes($"Server => {productName.ProductName}, { productName.CategoryName}, {productName.SizeName} + {NUM}");
                    NUM++;
                    var bufferToSend = new ArraySegment<Byte>(botResult, 0, botResult.Length);
                    await webSocket.SendAsync(bufferToSend, WebSocketMessageType.Text, true, CancellationToken.None/*, stoppingToken*/);
                }
                //for (int i = 0; i < 200; i++)
                //{

                //}

            }
        }
    }
}
