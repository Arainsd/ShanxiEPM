using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class ChangeView
    {
        public long Id { get; set; }
        ///<summary>
        ///项目表Id
        ///</summary>
        public long? ProjectId { get; set; }

        ///<summary>
        ///项目名称
        ///</summary>
        public string ProjectName { get; set; }

        ///<summary>
        ///变更号
        ///</summary>
        public string ChangeNo { get; set; }

        ///<summary>
        ///变更名称
        ///</summary>
        public string ChangeName { get; set; }

        ///<summary>
        ///变更内容
        ///</summary>
        public string ChangeContent { get; set; }

        ///<summary>
        ///变更原因
        ///</summary>
        public string ChangeReason { get; set; }

        ///<summary>
        ///变更金额，单位万元
        ///</summary>
        public decimal? ChangeAmount { get; set; }

        ///<summary>
        ///变更开始时间
        ///</summary>
        public DateTime? ChangeStartTime { get; set; }

        ///<summary>
        ///变更结束时间
        ///</summary>
        public DateTime? ChangeEndTime { get; set; }

        ///<summary>
        ///变更工期，单位天
        ///</summary>
        public int? ChangeDays { get; set; }

        ///<summary>
        ///发起人Id
        ///</summary>
        public long? SubmitUserId { get; set; }

        ///<summary>
        ///发起人Name
        ///</summary>
        public string SubmitUserName { get; set; }

        ///<summary>
        ///状态
        ///</summary>
        public int? State { get; set; }

        ///<summary>
        ///备注
        ///</summary>
        public string Remark { get; set; }


        ///<summary>
        ///涉及单位Id
        ///</summary>
        public string CompanyIds { get; set; }

        ///<summary>
        ///涉及单位Name
        ///</summary>
        public string CompanyNames { get; set; }
        public string VisaNum { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        public long CreateUserId { get; set; }
        

        public string CreateUserName { get; set; }

        /// <summary>
        /// 项目总金额
        /// </summary>
        public decimal? TotalAmount { get; set; }

        /// <summary>
        /// 调增额
        /// </summary>
        public decimal? AddAmount { get; set; }

        /// <summary>
        /// 调减额
        /// </summary>
        public decimal? ReduceAmount { get; set; }
        /// <summary>
        /// 创建单位
        /// </summary>
        public string CrtCompanyName { get; set; }
        public List<Epm_ChangeCompany> Epm_ChangeCompany { get; set; }

    }

    public class VisaView
    {
        public long Id { get; set; }
        ///<summary>
		///项目表Id
		///</summary>
		public long? ProjectId { get; set; }

        ///<summary>
        ///项目名称
        ///</summary>
        public string ProjectName { get; set; }

        ///<summary>
        ///变更表Id
        ///</summary>
        public long? ChangeId { get; set; }

        ///<summary>
        ///变更名称
        ///</summary>
        public string ChangeName { get; set; }

        ///<summary>
        ///签证号
        ///</summary>
        public string VisaNo { get; set; }

        ///<summary>
        ///标题
        ///</summary>
        public string VisaTitle { get; set; }

        ///<summary>
        ///原因
        ///</summary>
        public string VisaResean { get; set; }

        ///<summary>
        ///内容
        ///</summary>
        public string VisaContent { get; set; }

        ///<summary>
        ///金额，单位万元
        ///</summary>
        public decimal? VisaAmount { get; set; }

        ///<summary>
        ///签证开始时间
        ///</summary>
        public DateTime? VisaStartTime { get; set; }

        ///<summary>
        ///签证结束时间
        ///</summary>
        public DateTime? VisaEndTime { get; set; }

        ///<summary>
        ///签证工期，单位天
        ///</summary>
        public decimal? VisaDays { get; set; }

        ///<summary>
        ///发起人Id
        ///</summary>
        public long? SubmitUserId { get; set; }

        ///<summary>
        ///发起人Name
        ///</summary>
        public string SubmitUserName { get; set; }

        ///<summary>
        ///发起时间
        ///</summary>
        public DateTime? SubmitTime { get; set; }

        ///<summary>
        ///状态[10待处理,20审核通过,30已驳回,40已废弃]枚举
        ///</summary>
        public int? State { get; set; }

        ///<summary>
        ///备注
        ///</summary>
        public string Remark { get; set; }

        ///<summary>
        ///创建单位Id
        ///</summary>
        public long? CrtCompanyId { get; set; }

        ///<summary>
        ///创建单位Name
        ///</summary>
        public string CrtCompanyName { get; set; }
        ///<summary>
        ///涉及单位Id
        ///</summary>
        public string CompanyIds { get; set; }

        ///<summary>
        ///涉及单位Name
        ///</summary>
        public string CompanyNames { get; set; }

        public string VisaTypeName { get; set; }

        public List<Epm_VisaCompany> Epm_VisaCompany { get; set; }

        public long CreateUserId { get; set; }

        public string CreateUserName { get; set; }

        public DateTime? CreateTime { get; set; }
    }
}
