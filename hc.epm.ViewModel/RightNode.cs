using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    /// <summary>
    /// 权限树显示节点构造
    /// </summary>
    public class RightNode
    {
        /// <summary>
        /// 显示的name文本
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 附加data-数据
        /// </summary>
        public object data { get; set; }

        /// <summary>
        /// 复选框的值
        /// </summary>
        public string checkboxValue { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool @checked { get; set; }

        /// <summary>
        /// 是否展开
        /// </summary>
        public bool spread { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        //public string href { get; set; } = "";

        public List<RightNode> children { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        public int? Level { get; set; }
    }
}
