using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace hc.Plat.WebAPI.Base.Models
{
    public class MemoryNETCache
    {
        public static ObjectCache Cache
        {
            get
            {
                return MemoryCache.Default;
            }
        }
        /// <summary>
        /// 是否存在指定key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Contains(string key)
        {
            return Cache.Contains(key);
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(string key)
        {
            object obj_cache = Cache[key];
            if (obj_cache != null)
            {
                return (T)obj_cache;
            }
            return default(T);

        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheTime"></param>
        public static void Set(string key, object data, double secondsTime = 6000)
        {
            if (data == null)
            {
                return;
            }
            var isExits = Contains(key);
            if (isExits)
            {
                Remove(key);
            }
            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now.AddSeconds(secondsTime);
            Cache.Set(new CacheItem(key, data), policy);
        }


        /// <summary>
        ///移除缓存
        /// </summary>
        /// <param name="key">/key</param>
        public static void Remove(string key)
        {
            Cache.Remove(key);
        }

        /// <summary>
        /// 批量移除缓存
        /// </summary>
        /// <param name="pattern">pattern</param>
        public static void RemoveByPattern(string pattern)
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
        /// 清空缓存
        /// </summary>
        public static void Clear()
        {
            foreach (var item in Cache)
                Remove(item.Key);
        }
    }
}