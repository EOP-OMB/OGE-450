using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Caching;

namespace OMB.SharePoint.Infrastructure
{
    public class MemCache
    {
        private static ObjectCache cache = MemoryCache.Default;
        private static int cacheMinutes = 15;

        public static void Add(string cacheKeyName, object cacheItem)
        {
            CacheItemPolicy policy = new CacheItemPolicy() { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheMinutes) };
            cache.Set(cacheKeyName, cacheItem, policy);
        }

        public static Object Get(string cacheKeyName)
        {
            return cache[cacheKeyName] as Object;
        }

        public static void Clear(string cacheKeyName)
        {
            cache.Remove(cacheKeyName);
        }
    }
}
