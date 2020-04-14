using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class XtTzTenderingApplyView
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 承办部门
        /// </summary>
        public string UndertakeDepartment { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string UndertakeContacts { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string UndertakeTel { get; set; }
        /// <summary>
        /// 批复文件或者纪要
        /// </summary>
        public string Minutes { get; set; }
        /// <summary>
        /// 招标名称
        /// </summary>
        public string TenderingName { get; set; }
        /// <summary>
        /// 招标类型
        /// </summary>
        public string TenderingType { get; set; }
        /// <summary>
        /// 招标方式
        /// </summary>
        public string BidName { get; set; }
        /// <summary>
        /// 资金预算及依据
        /// </summary>
        public string CapitalBudget { get; set; }
        /// <summary>
        /// 项目概述
        /// </summary>
        public string ProjectSummary { get; set; }
        /// <summary>
        /// 附件类型
        /// </summary>
        public string Temp_TzAttachs { get; set; }
        // <summary>
        /// 创建人Id 对应表 Base_User 中的 ObjeId
        /// </summary>
        public string hr_sqr { get; set; }
    }
}
