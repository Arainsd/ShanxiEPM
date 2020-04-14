using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.Plat.Common.Extend
{
    public static class EnumExtensionMethod
    {
        /// <summary>
        /// 取某个对象的EnumText属性，如果没有设置EnumTextAttribute，则取枚举值的Name。
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetText(this Enum e)
        {
            if (e == null)
                return string.Empty;

            var attributes = (EnumTextAttribute[])e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(EnumTextAttribute), false);
            if (attributes.Count() > 0)
                return attributes.First().Value;

            return e.ToString();
        }

        /// <summary>
        /// 取某个枚举值。
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static object GetValue(this Enum e)
        {
            if (e == null)
                return null;

            //return e.GetType().InvokeMember(e.ToString(), BindingFlags.GetField, null, null, null).ToString();
            return e.GetType().GetField(e.ToString()).GetRawConstantValue();
        }

      
    }
    /// <summary>
    /// 泛型枚举静态帮助类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Enum<T> where T : struct
    {
        /// <summary>
        /// 将枚举转换成IEnumerable
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static IEnumerable<T> AsEnumerable()
        {
            Type enumType = typeof(T);

            if (!enumType.IsEnum)
                throw new NotSupportedException(string.Format("{0}必须为枚举类型。", enumType));

            EnumQuery<T> query = new EnumQuery<T>();
            return query;

            if (enumType == typeof(DateTime))
            {
            }
        }

       
    }

    class EnumQuery<T> : IEnumerable<T>
    {
        private List<T> _list;

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            Array values = Enum.GetValues(typeof(T));
            _list = new List<T>(values.Length);
            foreach (var value in values)
                _list.Add((T)value);

            return _list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
