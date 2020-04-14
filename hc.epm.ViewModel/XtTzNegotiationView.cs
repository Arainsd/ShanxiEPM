using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class XtTzNegotiationView
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
        /// 所属地市公司
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 估计金额
        /// </summary>
        public string PredictMoney { get; set; }
        /// <summary>
        /// 谈判时间
        /// </summary>
        public string TalkTime { get; set; }
        /// <summary>
        /// 谈判地点
        /// </summary>
        public string TalkAdress { get; set; }
        /// <summary>
        /// 土地竞拍支付第一笔出让金
        /// </summary>
        public string Fees { get; set; }
        /// <summary>
        /// 第一笔出让金日期
        /// </summary>
        public string FeesTime { get; set; }
        /// <summary>
        /// 我方谈判人
        /// </summary>
        public string OurNegotiators { get; set; }
        /// <summary>
        /// 对方谈判人
        /// </summary>
        public string OtherNegotiators { get; set; }
        /// <summary>
        /// 谈判结果
        /// </summary>
        public string TalkResultName { get; set; }
        /// <summary>
        /// 附件类型
        /// </summary>
        public string Temp_TzAttachs { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string hr_sqr { get; set; }
    }
}
