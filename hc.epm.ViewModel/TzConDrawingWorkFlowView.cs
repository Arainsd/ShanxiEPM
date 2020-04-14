using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class TzConDrawingWorkFlowView
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
        /// 项目类型
        /// </summary>
        public string StationTypeName { get; set; }
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectCode { get; set; }
        /// <summary>
        /// 地区公司
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 地市公司
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 估算投资
        /// </summary>
        public string PredictMoney { get; set; }
        /// <summary>
        /// 评审时间
        /// </summary>
        public string ReviewTime { get; set; }
        /// <summary>
        /// 主持人
        /// </summary>
        public string Moderator { get; set; }
        /// <summary>
        /// 评审地点
        /// </summary>
        public string ReviewAddress { get; set; }
        /// <summary>
        /// 特邀专家
        /// </summary>
        public string ReviewExperts { get; set; }
        /// <summary>
        /// 参会人员
        /// </summary>
        public string Participants { get; set; }
        /// <summary>
        /// 评审结论
        /// </summary>
        public string Conclusion { get; set; }
        /// <summary>
        /// 附件类型
        /// </summary>
        public string Temp_TzAttachs { get; set; }
        // <summary>
        /// 创建人Id 对应表 Base_User 中的 ObjeId
        /// </summary>
        public string hr_sqr { get; set; }
        /// <summary>
        /// 土地费用
        /// </summary>
        public string LandCosts { get; set; }
        /// <summary>
        /// 工程费用
        /// </summary>
        public string EngineeringCost { get; set; }
        /// <summary>
        /// 其他费用
        /// </summary>
        public string OtherExpenses { get; set; }
    }
    
}
