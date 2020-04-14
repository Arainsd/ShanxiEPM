using hc.Plat.Common.Extend;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.Plat.Cache.Helper
{
    /// <summary>  
    /// Redis缓存服务器  
    /// 服务器和客户端下载：  
    ///  https://github.com/MSOpenTech/redis/releases  
    ///  https://github.com/ServiceStack/ServiceStack.Redis  
    /// </summary>  
    public class RedisCache : IDataCache, IDisposable
    {
        private IRedisClient _redis = null;
        private static string redisIP = System.Configuration.ConfigurationManager.AppSettings["RedisIP"];
        private static string redisPort = System.Configuration.ConfigurationManager.AppSettings["RedisPort"];
        private static string redisPassword = System.Configuration.ConfigurationManager.AppSettings["RedisPassword"];
        private static int redisDB = System.Configuration.ConfigurationManager.AppSettings["RedisDB"].ToInt32Req();

        ~RedisCache()
        {
            Dispose();
        }

        public IRedisClient redis
        {
            get
            {
                if (_redis == null)
                {
                    //_redis = new RedisClient(redisIP, redisPort.ToInt32Req(), redisPassword);//要开启服务器才能连接  
                    _redis = CreateManager().GetClient();
                }
                return _redis;

            }
        }
        public RedisCache()
        {
            if (_redis != null)
            {
                _redis.Dispose();
                _redis.Shutdown();
            }
        }


        private PooledRedisClientManager _manager = null;
        /// <summary>
        /// 连接池
        /// </summary>
        public PooledRedisClientManager CreateManager()
        {
            if (_manager == null)
            {
                string connURL = redisPassword + "@" + redisIP + ":" + redisPort;
                string[] readURL = new string[] { connURL };
                _manager = new PooledRedisClientManager(readURL//用于写
                , readURL//用于读
                , new RedisClientManagerConfig
                {
                    MaxWritePoolSize = 50,//“写”链接池链接数
                    MaxReadPoolSize = 50,//“读”链接池链接数
                    AutoStart = true

                });
                _manager.ConnectTimeout = 2000;
            }
            return _manager;
        }


        /// <summary>  
        /// 获取缓存  
        /// </summary>  
        /// <typeparam name="T">类型（对象必须可序列化，否则可以作为object类型取出再类型转换，不然会报错）</typeparam>  
        /// <param name="key">缓存key</param>  
        /// <returns></returns>  
        public T Get<T>(string key)
        {
            return redis.Get<T>(key);
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
            string timeKey = key + "_time";
            if (redis.ContainsKey(timeKey) && redis.ContainsKey(key))
            {
                DateTime obj_time = Get<DateTime>(timeKey);
                T obj_cache = Get<T>(key);
                if (File.Exists(depFile))
                {
                    FileInfo fi = new FileInfo(depFile);
                    if (obj_time != fi.LastWriteTime)
                    {
                        Delete(key);
                        Delete(timeKey);
                        return default(T);
                    }
                    return obj_cache;
                }
                else
                {
                    throw new Exception("文件(" + depFile + ")不存在！");
                }
            }
            return default(T);

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
            return redis.Set<T>(key, value);
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
            return redis.Set<T>(key, value, expiresAt);
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
            return redis.Set<T>(key, value, DateTime.Now.AddSeconds(expiresSecond));
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
            bool ret1 = redis.Set<T>(key, value);
            if (ret1 && File.Exists(depFile))
            {
                FileInfo fi = new FileInfo(depFile);
                DateTime lastWriteTime = fi.LastWriteTime;
                return redis.Set<DateTime>(key + "_time", lastWriteTime);
            }
            return false;
        }
        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int Delete(string key)
        {
            redis.Remove(key);
            return 1;
        }
        /// <summary>
        /// 批量删除缓存
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public int Delete(string[] keys)
        {
            redis.RemoveAll(keys);
            return 1;
        }
        /// <summary>
        /// 释放redis连接
        /// </summary>
        public void Dispose()
        {
            if (_redis != null)
            {
                _redis.Dispose();
                _redis.Shutdown();//释放
            }

        }

    }
}
