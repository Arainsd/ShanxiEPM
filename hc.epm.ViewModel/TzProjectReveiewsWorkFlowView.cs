using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    /// <summary>
    /// 项目评审记录流程申请
    /// </summary>
    public class TzProjectReveiewsWorkFlowView
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
        /// 评审时间
        /// </summary>
        public string ReveiewDate { get; set; }
        /// <summary>
        /// 主持人
        /// </summary>
        public string HostUser { get; set; }
        /// <summary>
        /// 评审地点
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 评审结论
        /// </summary>
        public string ConclusionName { get; set; }
        /// <summary>
        /// 其他信息
        /// </summary>
        public string OtherInfo { get; set; }
        /// <summary>
        /// 特邀专家
        /// </summary>
        public string InvitedExperts { get; set; }
        /// <summary>
        /// 参会人员
        /// </summary>
        public string Attendees { get; set; }
        /// <summary>
        /// 项目完善内容
        /// </summary>
        public string PerfectContent { get; set; }
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
