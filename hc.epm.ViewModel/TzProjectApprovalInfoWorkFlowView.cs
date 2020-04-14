using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    /// <summary>
    /// 项目批复流程申请
    /// </summary>
    public class TzProjectApprovalInfoWorkFlowView
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
        /// 估计油日销量
        /// </summary>
        public string OilSalesTotal { get; set; }
        /// <summary>
        /// 估计气日销量（CNG）
        /// </summary>
        public string CNGY { get; set; }
        /// <summary>
        /// 估计气日销量（LNG）
        /// </summary>
        public string LNGQ { get; set; }
        /// <summary>
        /// 项目类型
        /// </summary>
        public string StationTypeName { get; set; }
        /// <summary>
        /// 地区公司
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 限上/限下
        /// </summary>
        public string LimitName { get; set; }
        /// <summary>
        /// 示范/标准
        /// </summary>
        public string StandardName { get; set; }
        /// <summary>
        /// 项目编号
        /// </summary>
        public string ProjectCode { get; set; }
        /// <summary>
        /// 项目批复文号
        /// </summary>
        public string ApprovalNo { get; set; }
        /// <summary>
        /// 签发人
        /// </summary>
        public string SignerName { get; set; }
        /// <summary>
        /// 拟稿人
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 会签人
        /// </summary>
        public string SignPeopleName { get; set; }
        /// <summary>
        /// 决策负责人
        /// </summary>
        public string DecisionMakerNam { get; set; }
        /// <summary>
        /// 现场负责人
        /// </summary>
        public string FieldManagerName { get; set; }
        /// <summary>
        /// 运营人
        /// </summary>
        public string HeadOperationsName { get; set; }
        /// <summary>
        /// 总投资
        /// </summary>
        public string TotalInvestment { get; set; }
        /// <summary>
        /// 合同款
        /// </summary>
        public string ContractPayment { get; set; }
        /// <summary>
        /// 工程费用
        /// </summary>
        public string EngineeringCost { get; set; }
        /// <summary>
        /// 土地费用
        /// </summary>
        public string LandCosts { get; set; }
        /// <summary>
        /// 其他费用
        /// </summary>
        public string OtherExpenses { get; set; }
        /// <summary>
        /// 首次取得投资计划日期
        /// </summary>
        public string DateFirstScheme { get; set; }
        /// <summary>
        /// 已下达计划
        /// </summary>
        public string IssuedPlan { get; set; }
        /// <summary>
        /// 上传计划明细
        /// </summary>
        public string Temp_TzAttachs { get; set; }
        /// <summary>
        /// 加油机（台）
        /// </summary>
        public string MachineOfOilStage { get; set; }
        /// <summary>
        /// 加油机（枪）
        /// </summary>
        public string MachineOfOil { get; set; }
        /// <summary>
        /// 加气机（枪）
        /// </summary>
        public string MachineOfGasStage { get; set; }
        /// <summary>
        /// 加气机（台）
        /// </summary>
        public string MachineOfGas { get; set; }
        /// <summary>
        /// 罩棚
        /// </summary>
        public string Shelter { get; set; }
        /// <summary>
        /// 油罐
        /// </summary>
        public string OilTank { get; set; }
        /// <summary>
        /// 站房
        /// </summary>
        public string StationRoom { get; set; }
        /// <summary>
        /// 油罐数量
        /// </summary>
        public string TankNumber { get; set; }
        /// <summary>
        /// 储气井（气罐）
        /// </summary>
        public string GasWells { get; set; }
        /// <summary>
        /// 土地费用支付类型
        /// </summary>
        public string LandPaymentType { get; set; }
        /// <summary>
        /// 资产类型
        /// </summary>
        public string AssetTypeName { get; set; }
        /// <summary>
        /// 投资金额
        /// </summary>
        public string InvestmentAmount { get; set; }
        /// <summary>
        /// 土地费用备注
        /// </summary>
        public string RemarkOnLandCost { get; set; }
        /// <summary>
        /// 土地性质
        /// </summary>
        public string LandStatus { get; set; }
        /// <summary>
        /// 土地用途
        /// </summary>
        public string LandUse { get; set; }
        /// <summary>
        /// 土地面积
        /// </summary>
        public string AreaOfLand { get; set; }
        /// <summary>
        /// 今年预计付款
        /// </summary>
        public string ExpectedPaymentThisYear { get; set; }
        /// <summary>
        /// 预计达销时间
        /// </summary>
        public string EstimatedTimeOfSales { get; set; }
        /// <summary>
        /// 可研编制单位
        /// </summary>
        public string UnitFeasibilityCompilation { get; set; }
        /// <summary>
        /// 股权投资形式
        /// </summary>
        public string FormOfEquityInvestment { get; set; }
        /// <summary>
        /// 实施单位
        /// </summary>
        public string ImplementationCcompany { get; set; }
        /// <summary>
        /// 注册资本
        /// </summary>
        public string RegisteredCapital { get; set; }
        /// <summary>
        /// 控股比例
        /// </summary>
        public string HoldingTheProportion { get; set; }
        /// <summary>
        /// 柴油日销量
        /// </summary>
        public string DailyDieselSales { get; set; }
        /// <summary>
        /// 汽油日销量
        /// </summary>
        public string DailyGasolineSales { get; set; }
        /// <summary>
        /// 柴汽比
        /// </summary>
        public string ChaiQibi { get; set; }
        /// <summary>
        /// 气日销量CNG
        /// </summary>
        public string CNG { get; set; }
        /// <summary>
        /// 气日销量LNG
        /// </summary>
        public string LNG { get; set; }
        /// <summary>
        /// 投资回收期
        /// </summary>
        public string PayBackPeriod { get; set; }
        /// <summary>
        /// 内部收益率
        /// </summary>
        public string InternalRateReturn { get; set; }
        /// <summary>
        /// 非油年收入
        /// </summary>
        public string AnnualNonOilIncome { get; set; }
        /// <summary>
        /// 非油年成本
        /// </summary>
        public string NonOilAnnualCost { get; set; }
        /// <summary>
        /// 预计投运时间
        /// </summary>
        public string ScheduledComTime { get; set; }
        /// <summary>
        /// 可研批复日期
        /// </summary>
        public string FeasibleApprovalDate { get; set; }
        /// <summary>
        /// 是否有工程信息
        /// </summary>
        public string HasEInformation { get; set; }

        // <summary>
        /// 创建人Id 对应表 Base_User 中的 ObjeId
        /// </summary>
        public string hr_sqr { get; set; }


    }

}
