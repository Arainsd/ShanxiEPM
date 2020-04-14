using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    /// <summary>
    /// 评审材料上报流程申请
    /// </summary>
    public class TzFormTalkFileWorkFlowView
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 站库名称
        /// </summary>
        public string StationName { get; set; }
        /// <summary>
        /// 项目性质
        /// </summary>
        public string NatureName { get; set; }
        /// <summary>
        /// 地理位置
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 提出时间
        /// </summary>
        public string ApplyTime { get; set; }
        /// <summary>
        /// 地市公司
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 估计金额
        /// </summary>
        public string PredictMoney { get; set; }
        /// <summary>
        /// 地市项目负责人
        /// </summary>
        public string ProjectLeaderName { get; set; }
        /// <summary>
        /// 地市项目决策人
        /// </summary>
        public string ProjectDecisionerName { get; set; }
        /// <summary>
        /// 编写负责人
        /// </summary>
        public string WriterName { get; set; }
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
