using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.Plat.Common.Extend
{
    /// <summary>
    /// 枚举Text属性
    /// </summary>
    public class EnumTextAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="value"></param>
        public EnumTextAttribute(string value)
        {
            Value = value;
        }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; private set; }
    }

}
