using System;
using System.Collections.Generic;

namespace hc.epm.ViewModel
{
    /// <summary>
    /// 前台站点导航模型
    /// </summary>
    [Serializable]
    public class WebRightNode
    {

        /// <summary>
        /// ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 跳转地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 下级菜单
        /// </summary>
        public List<WebRightNode> ChildNode { get; set; }

        /// <summary>
        /// 链接打开方式
        /// </summary>
        public string Target { get; set; }

        //public int Sort { get; set; }

        public string Remark { get; set; }
    }
}
