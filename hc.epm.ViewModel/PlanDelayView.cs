using System;
using System.Collections.Generic;
using hc.epm.DataModel.BaseCore;
using hc.epm.DataModel.Business;
using hc.epm.DataModel.Basic;

namespace hc.epm.ViewModel
{
    public class PlanDelayView:BaseBusiness
    {
        public PlanDelayView()
        {
            PlanDelayCompanys = new List<Epm_PlanDelayCompany>();
        }

        ///<summary>
        /// 所属项目
        ///</summary>
        public long ProjectId { get; set; }

        ///<summary>
        /// 所属项目名称
        ///</summary>
        public string ProjectName { get; set; }

        ///<summary>
        /// 延期计划 ID
        ///</summary>
        public long PlanId { get; set; }

        ///<summary>
        /// 延期计划名称
        ///</summary>
        public string PlanName { get; set; }

        ///<summary>
        /// 原计划开始日期
        ///</summary>
        public DateTime OldStartDate { get; set; }

        ///<summary>
        /// 原计划结束日期
        ///</summary>
        public DateTime OldEndDate { get; set; }

        ///<summary>
        /// 延期天数
        ///</summary>
        public decimal DelayDay { get; set; }

        ///<summary>
        /// 延期原因
        ///</summary>
        public string Reason { get; set; }

        ///<summary>
        /// 备注
        ///</summary>
        public string Remark { get; set; }

        ///<summary>
        /// 状态
        ///</summary>
        public int State { get; set; }

        ///<summary>
        /// 提交日期
        ///</summary>
        public DateTime CreateDate { get; set; }

        ///<summary>
        /// 申请单位 ID
        ///</summary>
        public long ApplyCompanyId { get; set; }

        ///<summary>
        /// 申请单位名称
        ///</summary>
        public string ApplyCompanyName { get; set; }

        ///<summary>
        /// 审核日期
        ///</summary>
        public DateTime? AuditDate { get; set; }

        ///<summary>
        /// 审核单位 ID
        ///</summary>
        public long? OrgId { get; set; }

        ///<summary>
        /// 审核单位名称
        ///</summary>
        public string OrgName { get; set; }

        ///<summary>
        /// 审核人 ID
        ///</summary>
        public long? AuditUserId { get; set; }

        ///<summary>
        /// 审核人名称
        ///</summary>
        public string AuditUserName { get; set; }

        /// <summary>
        /// 责任单位列表
        /// </summary>
        public List<Epm_PlanDelayCompany> PlanDelayCompanys { get; set; }

        public string CompanyIds { get; set; }

        public string CompanyNames { get; set; }
    }
}
