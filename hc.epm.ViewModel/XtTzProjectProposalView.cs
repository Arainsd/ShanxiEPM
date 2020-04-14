using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class XtTzProjectProposalView
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 项目性质
        /// </summary>
        public string NatureName { get; set; }
        /// <summary>
        /// 提出时间
        /// </summary>
        public string ApplyTime { get; set; }
        /// <summary>
        /// 站库名称
        /// </summary>
        public string StationName { get; set; }
        /// <summary>
        /// 地市公司
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 推荐人姓名
        /// </summary>
        public string Recommender { get; set; }
        /// <summary>
        /// 推荐人职务
        /// </summary>
        public string RecommenderJob { get; set; }
        /// <summary>
        /// 推荐人单位
        /// </summary>
        public string RecommenderDept { get; set; }
        /// <summary>
        /// 申报人
        /// </summary>
        public string DeclarerUser { get; set; }
        /// <summary>
        /// 地理位置
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 项目地理位置
        /// </summary>
        public string ProjectAddress { get; set; }
        /// <summary>
        /// 加油站类别
        /// </summary>
        public string StationType { get; set; }
        /// <summary>
        /// 估计金额
        /// </summary>
        public string PredictMoney { get; set; }
        /// <summary>
        /// 估计气日销量（CNG）
        /// </summary>
        public string CNGY { get; set; }
        /// <summary>
        /// 估计油日销量
        /// </summary>
        public string OilSalesTotal { get; set; }
        /// <summary>
        /// 估计气日销量（LNG）
        /// </summary>
        public string LNGQ { get; set; }
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
