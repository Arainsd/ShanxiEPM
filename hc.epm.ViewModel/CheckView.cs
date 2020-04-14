using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class CheckView
    {
        /// <summary>
        /// 检查项Id
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 层级
        /// </summary>
        public string level { get; set; }
        /// <summary>
        /// 是否选择
        /// </summary>
        public string selected { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 整改责任人
        /// </summary>
        public List<Rectification> addRectification { get; set; }
        /// <summary>
        /// 打分
        /// </summary>
        public string addScore { get; set; }
        /// <summary>
        /// 子检查明细
        /// </summary>
        public List<CheckView> children { get; set; }

        public bool isChange { get; set; }
    }

    public class Rectification
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}
