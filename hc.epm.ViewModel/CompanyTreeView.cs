using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class CompanyTreeView
    {
        /// <summary>
        /// 企业Id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 企业名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 数据级别
        /// </summary>
        public int level { get; set; }

        /// <summary>
        /// 是否展开
        /// </summary>
        public bool expanded { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public string parent { get; set; }

        /// <summary>
        /// 是否是末节点
        /// </summary>
        public bool isLeaf { get; set; }
        /// <summary>
        /// 法人
        /// </summary>
        public string manager { get; set; }
    }
}
