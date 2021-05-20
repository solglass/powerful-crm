using Microsoft.Extensions.Caching.Memory;
using powerful_crm.API.Models.InputModels;
using System.Collections.Generic;

namespace powerful_crm.API
{
    public class MemoryCacheSingleton
    {
        public MemoryCache Cache { get; set; }

        private static MemoryCacheSingleton _instance;

        private MemoryCacheSingleton()
        {
            Cache = new MemoryCache(new MemoryCacheOptions());
        }

        public static MemoryCacheSingleton GetCacheInstance()
        {
            if (_instance == null)
            {
                _instance = new MemoryCacheSingleton();
            }
            return _instance;
        }
    }
}
