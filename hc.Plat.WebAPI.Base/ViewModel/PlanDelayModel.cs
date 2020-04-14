using System.Collections.Generic;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 计划延期
    /// </summary>
    public class PlanDelayModel
    {
        ///<summary>
        /// 所属项目
        ///</summary>
        public long projectId { get; set; }

        ///<summary>
        /// 所属项目名称
        ///</summary>
        public string projectName { get; set; }

        ///<summary>
        /// 延期计划 ID
        ///</summary>
        public long planId { get; set; }

        ///<summary>
        /// 延期计划名称
        ///</summary>
        public string planName { get; set; }

        ///<summary>
        /// 延期天数
        ///</summary>
        public decimal delayDay { get; set; }

        ///<summary>
        /// 延期原因
        ///</summary>
        public string reason { get; set; }

        ///<summary>
        /// 备注
        ///</summary>
        public string remark { get; set; }

        /// <summary>
        /// 整改单位
        /// </summary>
        public string delayCompanies { get; set; }
    }

    /// <summary>
    /// 延期责任单位
    /// </summary>
    public class PlanDelayCompanyModel
    {
        ///<summary>
        /// 责任单位 ID
        ///</summary>
        public long id { get; set; }

        ///<summary>
        /// 责任单位名称
        ///</summary>
        public string name { get; set; }

        ///<summary>
        /// 延期天数
        ///</summary>
        public decimal delayDay { get; set; }

    }
}