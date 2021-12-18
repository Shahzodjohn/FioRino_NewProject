using FioRino_NewProject.Repositories;
using FioRino_NewProject.Settings;
using Microsoft.Extensions.Hosting;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public class LongRunningService : BackgroundService
    {
        private readonly BackgroundWorkerQueue queue;

        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductService _pService;
        private readonly IUniqueProductsRepository _uniqueProductRepository;
        private readonly ISizeRepository _sizeRepository;
        private readonly ISkuRepository _skuRepository;
        private readonly ISaveRepository _save;
        private readonly IStatusRepository _statusRepository;

        public LongRunningService(ICategoryRepository categoryRepository, IProductRepository productRepository, IProductService pService, IUniqueProductsRepository uniqueProductRepository, ISizeRepository sizeRepository, ISkuRepository skuRepository, ISaveRepository save, IStatusRepository statusRepository)
        {
            _categoryRepository = categoryRepository;
            _productRepository = productRepository;
            _pService = pService;
            _uniqueProductRepository = uniqueProductRepository;
            _sizeRepository = sizeRepository;
            _skuRepository = skuRepository;
            _save = save;
            _statusRepository = statusRepository;
        }

        public LongRunningService(BackgroundWorkerQueue queue)
        {
            this.queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await queue.DequeueAsync(stoppingToken);

                await workItem(stoppingToken);
            }
        }

       
    }
}
