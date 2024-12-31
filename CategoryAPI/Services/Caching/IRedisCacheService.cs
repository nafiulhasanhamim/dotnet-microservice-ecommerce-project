using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_mvc.Services.Caching
{
    public interface IRedisCacheService
    {
        Task<T?> GetDataAsync<T>(string key);
        void SetData<T>(string key, T data);
        void RemoveData(string key);
    }
}