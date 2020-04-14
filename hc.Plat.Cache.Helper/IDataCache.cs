using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.Plat.Cache.Helper
{
    public interface IDataCache
    {
        /// <summary>  
        /// 获取缓存  
        /// </summary>  
        /// <typeparam name="T">类型</typeparam>  
        /// <param name="key">缓存key</param>  
        /// <returns></returns>  
        T Get<T>(string key);
        T Get<T>(string key, string depFile);

        /// <summary>  
        /// 写入缓存  
        /// </summary>  
        /// <typeparam name="T">类型</typeparam>  
        /// <param name="key">缓存key</param>  
        /// <param name="value">缓存值</param>  
        /// <returns>返回值，表示：是否写入成功</returns>  
        bool Set<T>(string key, T value);

        /// <summary>  
        /// 写入缓存，设置过期时间点  
        /// </summary>  
        /// <typeparam name="T">类型</typeparam>  
        /// <param name="key">缓存key</param>  
        /// <param name="value">缓存值</param>  
        /// <param name="expiresAt">过期时间点</param>  
        /// <returns>返回值，表示：是否写入成功</returns>  
        bool Set<T>(string key, T value, DateTime expiresAt);

        /// <summary>  
        /// 写入缓存，设置过期秒数  
        /// </summary>  
        /// <typeparam name="T">类型</typeparam>  
        /// <param name="key">缓存key</param>  
        /// <param name="value">缓存值</param>  
        /// <param name="expiresSecond">过期秒数</param>  
        /// <returns>返回值，表示：是否写入成功</returns>  
        bool Set<T>(string key, T value, int expiresSecond);

        bool Set<T>(string key, T value, string depFile);

        /// <summary>  
        /// 删除缓存  
        /// </summary>  
        /// <param name="key">缓存key</param>  
        /// <returns></returns>  
        int Delete(string key);

        /// <summary>  
        /// 删除多个缓存  
        /// </summary>  
        /// <param name="keys">缓存key数组</param>  
        /// <returns></returns>  
        int Delete(string[] keys);
    }
}
