using System;

namespace hc.Plat.Common.Extend
{
    /// <summary>
    /// 可空类型的扩展方法
    /// </summary>
    public static class NullableExtensionMethod
    {
        /// <summary>
        /// 可空decimal型格式化输出
        /// </summary>
        /// <param name="d"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToString(this decimal? d, string format)
        {
            if (d.HasValue)
                return d.Value.ToString(format);
            return string.Empty;
        }

        /// <summary>
        /// 可空decimal返回指定位数小数
        /// </summary>
        /// <param name="d"></param>
        /// <param name="decimals"></param>
        /// <returns></returns>
        public static decimal? Round(this decimal? d, int decimals)
        {
            if (d.HasValue)
                return decimal.Round(d.Value, decimals);
            return null;
        }

        /// <summary>
        /// 可空double型格式化输出
        /// </summary>
        /// <param name="d"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToString(this double? d, string format)
        {
            if (d.HasValue)
                return d.Value.ToString(format);
            return string.Empty;
        }

        /// <summary>
        /// 可空float型格式化输出
        /// </summary>
        /// <param name="f"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToString(this float? f, string format)
        {
            if (f.HasValue)
                return f.Value.ToString(format);
            return string.Empty;
        }

        /// <summary>
        /// 可空int型格式化输出
        /// </summary>
        /// <param name="i"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToString(this int? i, string format)
        {
            if (i.HasValue)
                return i.Value.ToString(format);
            return string.Empty;
        }

        /// <summary>
        /// 可空long型格式化输出
        /// </summary>
        /// <param name="l"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToString(this long? l, string format)
        {
            if (l.HasValue)
                return l.Value.ToString(format);
            return string.Empty;
        }

        /// <summary>
        /// 可空DateTime型格式化输出
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToString(this DateTime? dt, string format)
        {
            if (dt.HasValue)
                return dt.Value.ToString(format);
            return string.Empty;
        }
    }
}
