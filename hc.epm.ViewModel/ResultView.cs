using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    /// <summary>
    /// web 简易result基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResultView<T>
    {
        /// <summary>
        /// 成功标识
        /// </summary>
        public bool Flag { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 其他附加数据
        /// </summary>
        public string Other { get; set; }
    }
}
