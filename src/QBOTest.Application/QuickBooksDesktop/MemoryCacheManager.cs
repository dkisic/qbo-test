﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QBOTest.QuickBooksDesktop
{
    public class MemoryCacheManager : ICacheManager
    {
        protected ObjectCache Cache
        {
            get { return MemoryCache.Default; }
        }

        #region ICacheManager Members

        /// <summary>
        /// 	Gets or sets the value associated with the specified key.
        /// </summary>
        /// <typeparam name = "T">Type</typeparam>
        /// <param name = "key">The key of the value to get.</param>
        /// <returns>The value associated with the specified key.</returns>
        public T Get<T>(string key)
        {
            return (T)Cache[key];
        }

        /// <summary>
        /// 	Adds the specified key and object to the cache.
        /// </summary>
        /// <param name = "key">key</param>
        /// <param name = "data">Data</param>
        /// <param name = "cacheTime">Cache time</param>
        public void Set(string key, object data, int cacheTime)
        {
            if (data == null)
                return;

            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime)
            };
            Cache.Add(new CacheItem(key, data), policy);
        }

        /// <summary>
        /// 	Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name = "key">key</param>
        /// <returns>Result</returns>
        public bool IsSet(string key)
        {
            return (Cache.Contains(key));
        }
        /// <summary>
        /// 	Gets a value indicating whether the value associated with the specified key is cached
        /// </summary>
        /// <param name = "key">key</param>
        /// <returns>Result</returns>
        public object GetForCheck(string key)
        {
            return Cache.Get(key);
        }
        /// <summary>
        /// 	Removes the value with the specified key from the cache
        /// </summary>
        /// <param name = "key">/key</param>
        public void Remove(string key)
        {
            // Trace.WriteLine("MemoryCacheManager::::     Removing Items from Cache::: " + key);
            Cache.Remove(key);
        }

        /// <summary>
        /// 	Removes items by pattern
        /// </summary>
        /// <param name = "pattern">pattern</param>
        public void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = new List<String>();

            foreach (var item in Cache)
                if (regex.IsMatch(item.Key))
                    keysToRemove.Add(item.Key);

            foreach (string key in keysToRemove)
            {
                Remove(key);
            }
        }

        /// <summary>
        /// 	Clear all cache data
        /// </summary>
        public void Clear()
        {
            foreach (var item in Cache)
            {
                Type valueType = item.Value.GetType();
                if (valueType.FullName != "QuickbooksSession" && valueType.FullName != "TwillioBuyNo"
                    && !item.Key.Contains("QBDesktopExportProcess") && !item.Key.Contains("QBExportProcess"))//Don't remove Quick book Desktop and Twilio cache.
                    Remove(item.Key);
            }
        }

        #endregion
    }
}