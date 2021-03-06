//------------------------------------------------------------------------------
// <auto-generated>
// 此代码由华春网络代码生成工具生成
// version 1.0
// author hanshiwei 2017.07.24
// auto create time:2019-11-04 18:14:37
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using hc.epm.DataModel.BaseCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace hc.epm.DataModel.Business
{
    ///<summary>
    ///Epm_TzDesignScheme:项目设计方案
    ///</summary>
    public class  Epm_TzDesignScheme:BaseBusiness
	{
        public Epm_TzDesignScheme()
        {
            TzAttachs = new List<Epm_TzAttachs>();
        }

       
        ///<summary>
        ///所属项目ID
        ///</summary>
        public long? ProjectId { get; set; }

		///<summary>
		///项目编码
		///</summary>
		public string ProjectCode { get; set; }

		///<summary>
		///项目名称
		///</summary>
		public string ProjectName { get; set; }

		///<summary>
		///项目批复号
		///</summary>
		public string ApprovalNo { get; set; }

		///<summary>
		///项目性质（数据字典）
		///</summary>
		public string Nature { get; set; }

		///<summary>
		///项目性质名称
		///</summary>
		public string NatureName { get; set; }

		///<summary>
		///项目提出日期（冗余）
		///</summary>
		public DateTime? ApplyTime { get; set; }

		///<summary>
		///库站ID
		///</summary>
		public long? StationId { get; set; }

		///<summary>
		///库站协同编码
		///</summary>
		public string StationCodeXt { get; set; }

		///<summary>
		///库站名称
		///</summary>
		public string StationName { get; set; }

		///<summary>
		///地市公司ID
		///</summary>
		public long? CompanyId { get; set; }

		///<summary>
		///地市公司协同编码
		///</summary>
		public string CompanyCodeXt { get; set; }

		///<summary>
		///地市公司名称
		///</summary>
		public string CompanyName { get; set; }

		///<summary>
		///初步设计单位
		///</summary>
		public string DesignUnit { get; set; }

		///<summary>
		///示范/标注数据字典编码
		///</summary>
		public string StandarCode { get; set; }

		///<summary>
		///示范名称
		///</summary>
		public string StandarName { get; set; }

		///<summary>
		///上报概算
		///</summary>
		public decimal? Estimate { get; set; }

		///<summary>
		///总工程费用（冗余）
		///</summary>
		public decimal? TotalInvestment { get; set; }

		///<summary>
		///其它工程费用（冗余）
		///</summary>
		public decimal? OtheInvestment { get; set; }

		///<summary>
		///设计单位招标日期
		///</summary>
		public DateTime? InviteTime { get; set; }

		///<summary>
		///设计单位负责人
		///</summary>
		public string DesignUnitCharge { get; set; }

		///<summary>
		///设计单位负责人职务
		///</summary>
		public string DesignJob { get; set; }

		///<summary>
		///项目经理（冗余）
		///</summary>
		public string ProjectManager { get; set; }

		///<summary>
		///项目经理职务（冗余）
		///</summary>
		public string ProjectJob { get; set; }

		///<summary>
		///占地面积
		///</summary>
		public decimal? LandArea { get; set; }

		///<summary>
		///加油机（台）
		///</summary>
		public int? MachineofOilStage { get; set; }

		///<summary>
		///加气机（台）
		///</summary>
		public int? MachineofGasStage { get; set; }

		///<summary>
		///储气机
		///</summary>
		public decimal? GasWells { get; set; }

		///<summary>
		///罐容
		///</summary>
		public decimal? OilTank { get; set; }

		///<summary>
		///罩棚面积（冗余）
		///</summary>
		public decimal? Shelter { get; set; }

		///<summary>
		///站房面积（冗余）
		///</summary>
		public decimal? StationRoom { get; set; }

		///<summary>
		///便利店面积
		///</summary>
		public decimal? ConvenienceRoom { get; set; }

		///<summary>
		///批复概算投资（冗余）
		///</summary>
		public decimal? ReleaseInvestmentAmount { get; set; }

		///<summary>
		///其他工程内容
		///</summary>
		public string OtherProject { get; set; }

		///<summary>
		///项目信息是否同步：是、否
		///</summary>
		public string IsSynchro { get; set; }

		///<summary>
		///状态：暂存、待审核、审批通过、不通过
		///</summary>
		public int? State { get; set; }

		///<summary>
		///
		///</summary>
		public string Remark { get; set; }

        /// <summary>
        /// 投资相关附件
        /// </summary>
        [NotMapped]
        public List<Epm_TzAttachs> TzAttachs { get; set; }


        ///<summary>
		///工程费用
		///</summary>
		public decimal? EngineeringCost { get; set; }
        ///<summary>
		///其它费用
		///</summary>
		public decimal? OtherExpenses { get; set; }
        ///<summary>
		///土地费用
		///</summary>
		public decimal? LandCosts { get; set; }
        ///<summary>
		///估算投资
		///</summary>
		public decimal? PredictMoney { get; set; }
        ///<summary>
		///地区公司
		///</summary>
		public string RegionCompany { get; set; }
        ///<summary>
		///项目类型
		///</summary>
		public string ProjectType { get; set; }
        /// <summary>
        /// 流程申请 ID
        /// </summary>
        public string WorkFlowId { get; set; }

        ///<summary>
        ///当前状态
        ///</summary>
        public string StateType { get; set; }

        ///<summary>
        ///状态编码
        ///</summary>
        public string StateName { get; set; }

        /// <summary>
        /// 当前审批人
        /// </summary>
        public string ApprovalName { get; set; }

        /// <summary>
        /// 当前审批人Id
        /// </summary>
        public long? ApprovalNameId { get; set; }

        /// <summary>
        /// 开工报告审批时间（本次流程全部审批完成时间（年月日），若还未完成审批，不显示）
        /// </summary>
        public DateTime? WorkStartApprovalTime { get; set; }
    }
}

