using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using System.Net;

namespace hc.Plat.Cache.Helper
{
    /// <summary>
    /// memcached缓存
    /// </summary>
    public class MemCached : IDataCache
    {
        /// <summary>  
        /// 获取缓存  
        /// </summary>  
        /// <typeparam name="T">类型（对象必须可序列化，否则可以作为object类型取出再类型转换，不然会报错）</typeparam>  
        /// <param name="key">缓存key</param>  
        /// <returns></returns>  
        public T Get<T>(string key)
        {
            using (MemcachedClient mc = new MemcachedClient())
            {
                return mc.Get<T>(key);
            }
        }
        /// <summary>
        /// 获取缓存，依赖文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="depFile"></param>
        /// <returns></returns>
        public T Get<T>(string key, string depFile)
        {
            throw new Exception("未实现此方法");
        }
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value)
        {
            using (MemcachedClient mc = new MemcachedClient())
            {
                var data = mc.Get(key);
                bool res = true;
                if (data == null)
                    res = mc.Store(StoreMode.Add, key, value);
                else
                    res = mc.Store(StoreMode.Replace, key, value);
                return res;
            }

        }
        /// <summary>
        /// 设置缓存，含过期时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, DateTime expiresAt)
        {
            using (MemcachedClient mc = new MemcachedClient())
            {
                var data = mc.Get(key);
                if (data == null)
                    return mc.Store(StoreMode.Add, key, value, expiresAt);
                else
                    return mc.Store(StoreMode.Replace, key, value, expiresAt);
            }
        }
        /// <summary>
        /// 设置缓存，指定多少秒后过期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresSecond"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, int expiresSecond)
        {
            using (MemcachedClient mc = new MemcachedClient())
            {
                var data = mc.Get(key);

                DateTime dateTime = DateTime.Now.AddSeconds(expiresSecond);
                if (data == null)
                    return mc.Store(StoreMode.Add, key, value, dateTime);
                else
                    return mc.Store(StoreMode.Replace, key, value, dateTime);
            }
        }
        /// <summary>
        /// 设置缓存，指定间隔多久过期
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, TimeSpan timeSpan)
        {
            using (MemcachedClient mc = new MemcachedClient())
            {
                var data = mc.Get(key);
                if (data == null)
                    return mc.Store(StoreMode.Add, key, value, timeSpan);
                else
                    return mc.Store(StoreMode.Replace, key, value, timeSpan);
            }
        }
        /// <summary>
        /// 设置缓存，依赖文件更改
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="depFile"></param>
        /// <returns></returns>
        public bool Set<T>(string key, T value, string depFile)
        {
            throw new Exception("未实现此方法");
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int Delete(string key)
        {
            using (MemcachedClient mc = new MemcachedClient())
            {

                return mc.Remove(key) ? 1 : 0;
            }
        }
        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public int Delete(string[] keys)
        {
            using (MemcachedClient mc = new MemcachedClient())
            {
                foreach (var item in keys)
                {
                    mc.Remove(item);
                }
                return 1;
            }
        }
        /// <summary>
        /// 清空缓存服务器上的缓存
        /// </summary>
        public void FlushCache()
        {
            using (MemcachedClient mc = new MemcachedClient())
            {
                mc.FlushAll();
            }
        }
    }
}