//------------------------------------------------------------------------------
// <auto-generated>
// 此代码由华春网络代码生成工具生成
// version 1.0
// author hanshiwei 2017.07.24
// auto create time:2019-12-02 14:14:27
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using hc.epm.DataModel.BaseCore;
namespace hc.epm.DataModel.Business
{
    ///<summary>
    ///TEMP_TzProjectApprovalInfo:
    ///</summary>
    public class TEMP_TzProjectApprovalInfo : BaseBusiness
	{ 
		///<summary>
		///总投资
		///</summary>
		public decimal? TotalInvestment { get; set; }

		///<summary>
		///合同款
		///</summary>
		public decimal? ContractPayment { get; set; }

		///<summary>
		///工程费用
		///</summary>
		public decimal? EngineeringCost { get; set; }

		///<summary>
		///土地费用
		///</summary>
		public decimal? LandCosts { get; set; }

		///<summary>
		///其他费用
		///</summary>
		public decimal? OtherExpenses { get; set; }

		///<summary>
		///加油机
		///</summary>
		public int? MachineOfOilStage { get; set; }

		///<summary>
		///加油枪
		///</summary>
		public int? MachineOfOil { get; set; }

		///<summary>
		///加气机
		///</summary>
		public int? MachineOfGas { get; set; }

        ///<summary>
        ///加气枪
        ///</summary>
        public int? MachineOfGasStage { get; set; }

		///<summary>
		///罩棚
		///</summary>
		public decimal? Shelter { get; set; }

		///<summary>
		///油罐
		///</summary>
		public decimal? OilTank { get; set; }

		///<summary>
		///站房
		///</summary>
		public decimal? StationRoom { get; set; }

		///<summary>
		///油罐数量
		///</summary>
		public int? TankNumber { get; set; }

		///<summary>
		///储气井
		///</summary>
		public decimal? GasWells { get; set; }

		///<summary>
		///土地费用支付类型
		///</summary>
		public string LandPayment { get; set; }

		///<summary>
		///资产类型
		///</summary>
		public string AssetTypeName { get; set; }

		///<summary>
		///土地费用备注
		///</summary>
		public string RemarkOnLandCost { get; set; }

		///<summary>
		///土地性质
		///</summary>
		public string LandStatus { get; set; }

		///<summary>
		///土地用途
		///</summary>
		public string LandUse { get; set; }

		///<summary>
		///土地面积
		///</summary>
		public decimal? AreaOfLand { get; set; }

		///<summary>
		///今年预计付款
		///</summary>
		public decimal? ExpectedPaymentThisYear { get; set; }

		///<summary>
		///预计达销时间
		///</summary>
		public int? EstimatedTimeOfSales { get; set; }

		///<summary>
		///限上、限下项目
		///</summary>
		public string LimitName { get; set; }

		///<summary>
		///示范、标准
		///</summary>
		public string StandardName { get; set; }

        ///<summary>
        ///可研编制单位
        ///</summary>
        public string UnitFeasibilityCompilation { get; set; }

        ///<summary>
        ///项目批复文号
        ///</summary>
        public string ApprovalNo { get; set; }

        ///<summary>
        ///股权投资形式
        ///</summary>
        public string FormOfEquityInvestment { get; set; }

        ///<summary>
        ///实施单位
        ///</summary>
        public string ImplementationCcompany { get; set; }

        ///<summary>
        ///注册资本
        ///</summary>
        public decimal? RegisteredCapital { get; set; }

        ///<summary>
        ///控制比例
        ///</summary>
        public string HoldingTheProportion { get; set; }

        ///<summary>
        ///柴油日销量
        ///</summary>
        public decimal? DailyDieselSales { get; set; }

        ///<summary>
        ///汽油日销量
        ///</summary>
        public decimal? DailyGasolineSales { get; set; }

        ///<summary>
        ///柴气比
        ///</summary>
        public string ChaiQibi { get; set; }

        ///<summary>
        ///气日销量
        ///</summary>
        public decimal? CNGYAndLNGQ { get; set; }

        ///<summary>
        ///投资回收期
        ///</summary>
        public string PayBackPeriod { get; set; }

        ///<summary>
        ///内部收益率
        ///</summary>
        public string InternalRateReturn { get; set; }

        ///<summary>
        ///非油年收入
        ///</summary>
        public decimal? AnnualNonOilIncome { get; set; }

        ///<summary>
        ///非油年成本
        ///</summary>
        public decimal? NonOilAnnualCost { get; set; }

        ///<summary>
        ///预计投运时间
        ///</summary>
        public string ScheduledComTime { get; set; }

        ///<summary>
        ///可研批复日期
        ///</summary>
        public DateTime? FeasibleApprovalDate { get; set; }

        ///<summary>
        ///是否有工程信息
        ///</summary>
        public bool? HasEInformation { get; set; }

        ///<summary>
        ///登录账户
        ///</summary>
        public string username { get; set; }

        ///<summary>
        ///分公司名称
        ///</summary>
        public string companys { get; set; }

        ///<summary>
        ///回写标识
        ///</summary>
        public int? WriteMark { get; set; }

        ///<summary>
        /// 回写结果
        ///</summary>
        public int? WriteResult { get; set; }

        ///<summary>
        ///后续操作
        ///</summary>
        public string FollowOperate { get; set; }

        ///<summary>
        ///附件地址
        ///</summary>
        public string FilePath { get; set; }

        /// <summary>
        ///  附件数量
        /// </summary>
        public int FileNumber { get; set; }
        public string ProjectName { get; set; }
        public int status1 { get; set; }
    }
}
