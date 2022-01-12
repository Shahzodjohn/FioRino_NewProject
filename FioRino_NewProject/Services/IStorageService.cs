﻿using FioRino_NewProject.DataTransferObjects;
using FioRino_NewProject.Entities;
using FioRino_NewProject.Responses;
using System.Threading.Tasks;

namespace FioRino_NewProject.Services
{
    public interface IStorageService
    {
        Task<DmStorage> UpdatingAmountStorage(int stanId, InsertingProductsParams parameters, string Gtin);
        Task StorageCheckPlusAmount(DmStorage StorageCheck, InsertingProductsParams parameters);
        Task<Response> MinusingAmountFromStorage(StanAmountUpdateDTO dTO);
        Task<int> DrukujCodes(int OrderId);
    }
}
