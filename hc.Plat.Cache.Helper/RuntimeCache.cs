using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;


namespace hc.Plat.Cache.Helper
{
    public class RuntimeCache : IDataCache
    {

        #region 删除缓存  
        /// <summary>  
        /// 删除缓存  
        /// </summary>  
        /// <param name="cacheKey">键</param>  
        public int Delete(string cacheKey)
        {
            HttpRuntime.Cache.Remove(cacheKey);
            return 1;
        }
        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="cacheKeys"></param>
        /// <returns></returns>
        public int Delete(string[] cacheKeys)
        {
            int i = 0;
            foreach (var key in cacheKeys)
            {
                HttpRuntime.Cache.Remove(key);
                i++;
            }
            return i;
        }
        #endregion

        #region 获取缓存

        /// <summary>  
        /// 获取缓存
        /// </summary>  
        /// <param name="cacheKey">键</param>  
        /// <returns></returns>  
        public T Get<T>(string cacheKey)
        {
            object obj_time = HttpRuntime.Cache[cacheKey + "_time"];
            object obj_cache = HttpRuntime.Cache[cacheKey];
            if (obj_cache != null)
            {
                if (obj_time != null)
                {
                    if (Convert.ToDateTime(obj_time) < DateTime.Now)
                    {
                        Delete(cacheKey);
                        Delete(cacheKey + "_time");
                        return default(T);
                    }
                    return (T)obj_cache;
                }
                return (T)obj_cache;
            }
            Delete(cacheKey);
            Delete(cacheKey + "_time");
            return default(T);

        }


        /// <summary>  
        /// 获取缓存，依赖文件  
        /// </summary>  
        /// <param name="cacheKey">键</param>  
        /// <param name="depFile">依赖的文件</param>  
        /// <returns></returns>  
        public T Get<T>(string cacheKey, string depFile)
        {
            object obj_time = HttpRuntime.Cache[cacheKey + "_time"];
            object obj_cache = HttpRuntime.Cache[cacheKey];
            if (File.Exists(depFile))
            {
                FileInfo fi = new FileInfo(depFile);
                if (obj_time != null && obj_cache != null)
                {
                    if (Convert.ToDateTime(obj_time) != fi.LastWriteTime)
                    {
                        Delete(cacheKey);
                        Delete(cacheKey + "_time");
                        return default(T);
                    }
                    return (T)obj_cache;
                }
                Delete(cacheKey);
                Delete(cacheKey + "_time");
                return default(T);
            }
            else
            {
                throw new Exception("文件(" + depFile + ")不存在！");
            }
        }
        #endregion

        #region 插入缓存  
        /// <summary>  
        /// 简单的插入缓存  
        /// </summary>  
        /// <param name="cacheKey">键</param>  
        /// <param name="objObject">数据</param>  
        public bool Set<T>(string cacheKey, T objObject)
        {
            HttpRuntime.Cache.Insert(cacheKey, objObject);
            return true;
        }


        /// <summary>
        /// 有过期时间的插入缓存数据  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="objObject"></param>
        /// <param name="expiresAt"></param>
        /// <returns></returns>
        public bool Set<T>(string cacheKey, T objObject, DateTime expiresAt)
        {
            HttpRuntime.Cache.Insert(cacheKey, objObject, null, expiresAt, System.Web.Caching.Cache.NoSlidingExpiration);
            HttpRuntime.Cache.Insert(cacheKey + "_time", expiresAt, null, expiresAt, System.Web.Caching.Cache.NoSlidingExpiration);//存储过期时间  
            return true;
        }


        /// <summary>
        /// 插入缓存数据，指定缓存多少秒  
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="objObject"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public bool Set<T>(string cacheKey, T objObject, int seconds)
        {
            DateTime expiresAt = DateTime.Now.AddSeconds(seconds);
            HttpRuntime.Cache.Insert(cacheKey, objObject, null, expiresAt, System.Web.Caching.Cache.NoSlidingExpiration);
            HttpRuntime.Cache.Insert(cacheKey + "_time", expiresAt, null, expiresAt, System.Web.Caching.Cache.NoSlidingExpiration);//存储过期时间  
            return true;
        }
        /// <summary>  
        /// 依赖文件的缓存，文件没改不会过期  
        /// </summary>  
        /// <param name="cacheKey">键</param>  
        /// <param name="objObject">数据</param>  
        /// <param name="depfilename">依赖文件，可调用 DataCache 里的变量</param>  
        public bool Set<T>(string cacheKey, T objObject, string depfilename)
        {
            //缓存依赖对象  
            System.Web.Caching.CacheDependency dep = new System.Web.Caching.CacheDependency(depfilename);
            DateTime absoluteExpiration = System.Web.Caching.Cache.NoAbsoluteExpiration;
            TimeSpan slidingExpiration = System.Web.Caching.Cache.NoSlidingExpiration;
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(
                cacheKey,
                objObject,
                dep,
                absoluteExpiration, //从不过期  
                slidingExpiration, //禁用可调过期  
                System.Web.Caching.CacheItemPriority.Default,
                null);
            if (File.Exists(depfilename))
            {
                FileInfo fi = new FileInfo(depfilename);
                DateTime lastWriteTime = fi.LastWriteTime;
                HttpRuntime.Cache.Insert(cacheKey + "_time", lastWriteTime, null, absoluteExpiration, slidingExpiration);//存储文件最后修改时间  
            }
            return true;
        }
        #endregion
    }
}
