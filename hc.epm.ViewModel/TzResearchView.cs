using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    /// <summary>
    /// 现场勘探
    /// </summary>
    public class TzResearchView
    {
        ///<summary>
        ///所属项目ID
        ///</summary>
        public long ProjectId { get; set; }

        ///<summary>
		///状态
		///</summary>
		public int State { get; set; }

        ///<summary>
        ///调研开始日期
        ///</summary>
        public DateTime? StartDate { get; set; }

        #region 现场工程方面调研
        public long? ResearchId { get; set; }
        ///<summary>
        ///调研开始日期
        ///</summary>
        public DateTime? ResearchStartDate { get; set; }

        ///<summary>
        ///调研截止日期
        ///</summary>
        public DateTime? ResearchEndDate { get; set; }

        ///<summary>
        ///调研人ID
        ///</summary>
        public long? ResearchUserId { get; set; }

        ///<summary>
        ///调研人姓名
        ///</summary>
        public string ReaearchUserName { get; set; }

        ///<summary>
        ///调研人协同OA编码
        ///</summary>
        public string ReaearchUserXt { get; set; }

        ///<summary>
        ///职务编码(字典)
        ///</summary>
        public string JobCode { get; set; }

        ///<summary>
        ///职务名称
        ///</summary>
        public string JobName { get; set; }

        ///<summary>
        ///形象工程符合行业规划 0 符合，1基本符合，2 不符合
        ///</summary>
        public string IndustryPlanning { get; set; }

        ///<summary>
        ///罩棚面积(平米)
        ///</summary>
        public decimal? Shelter { get; set; }
        
        ///<summary>
        ///加油枪(台)
        ///</summary>
        public int? MachineOfOilStage { get; set; }

        ///<summary>
        ///加油枪(个)
        ///</summary>
        public int? MachineOfOil { get; set; }

        ///<summary>
        ///加气枪(台)
        ///</summary>
        public int? MachineOfGasStage { get; set; }

        ///<summary>
        ///加气枪(个)
        ///</summary>
        public int? MachineOfGas { get; set; }

        ///<summary>
        ///油罐
        ///</summary>
        public decimal? OilTank { get; set; }

        ///<summary>
        ///站房(平米)
        ///</summary>
        public decimal? StationRoon { get; set; }

        ///<summary>
        ///储气井(方)
        ///</summary>
        public decimal? GasWells { get; set; }

        ///<summary>
        ///信息化系统
        ///</summary>
        public string HasInformationSystem { get; set; }

        ///<summary>
        ///改造必要性
        ///</summary>
        public string ReformCode { get; set; }

        ///<summary>
        ///改造必要性
        ///</summary>
        public string ReformName { get; set; }
        #endregion

        #region 信息方面调研
        public long? InfoId { get; set; }
        ///<summary>
        ///调研开始日期
        ///</summary>
        public DateTime? InfoResearchStartDate { get; set; }

        ///<summary>
        ///调研截止日期
        ///</summary>
        public DateTime? InfoResearchEndDate { get; set; }
        ///<summary>
		///调研人ID
		///</summary>
		public long? InfoResearchUserId { get; set; }

        ///<summary>
        ///调研人姓名
        ///</summary>
        public string InfoReaearchUserName { get; set; }

        ///<summary>
        ///调研人协同OA编码
        ///</summary>
        public string InfoReaearchUserXt { get; set; }

        ///<summary>
        ///职务编码(字典)
        ///</summary>
        public string InfoJobCode { get; set; }

        ///<summary>
        ///职务名称
        ///</summary>
        public string InfoJobName { get; set; }

        ///<summary>
        ///改进措施
        ///</summary>
        public string Improvement { get; set; }
        
        #endregion

        #region 现场投资调研
        public long? InvestId { get; set; }
        ///<summary>
        ///调研开始日期
        ///</summary>
        public DateTime? InvestResearchStartDate { get; set; }

        ///<summary>
        ///调研截止日期
        ///</summary>
        public DateTime? InvestResearchEndDate { get; set; }
        ///<summary>
		///调研人ID
		///</summary>
		public long? InvestResearchUserId { get; set; }

        ///<summary>
        ///调研人姓名
        ///</summary>
        public string InvestResearchUserName { get; set; }

        ///<summary>
        ///调研人协同OA编码
        ///</summary>
        public string InvestResearchUserXt { get; set; }

        ///<summary>
        ///职务编码(字典)
        ///</summary>
        public string InvestJobCode { get; set; }

        ///<summary>
        ///职务名称
        ///</summary>
        public string InvestJobName { get; set; }

        ///<summary>
        ///详细地理位置
        ///</summary>
        public string Address { get; set; }

        ///<summary>
        ///周边环境编码
        ///</summary>
        public string EnvironmentTypeCode { get; set; }

        ///<summary>
        ///周边环境名称
        ///</summary>
        public string EnvironmentTypeName { get; set; }

        ///<summary>
        ///土地性质编码
        ///</summary>
        public string LandNatureCode { get; set; }

        ///<summary>
        ///土地性质名称
        ///</summary>
        public string LandNatureName { get; set; }

        ///<summary>
        ///土地用途编码
        ///</summary>
        public string LandUseCode { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string LandUseName { get; set; }

        ///<summary>
        ///土地面积
        ///</summary>
        public decimal? LandArea { get; set; }

        ///<summary>
        ///土地形状(临街长度)米
        ///</summary>
        public decimal? LandShape { get; set; }

        ///<summary>
        ///是否符合地区建站规划
        ///</summary>
        public string IsMeetAreaPlan { get; set; }

        ///<summary>
        ///周边车辆保有量(辆)
        ///</summary>
        public int? AroundCarCount { get; set; }

        ///<summary>
        ///平均日车流辆
        ///</summary>
        public int? DailyTraffic { get; set; }

        ///<summary>
        ///成品油销量测算(吨/日)
        ///</summary>
        public decimal? OilSaleTotal { get; set; }

        ///<summary>
        ///柴汽比
        ///</summary>
        public string DieselGasolineRatio { get; set; }

        ///<summary>
        ///土地价格(万元/亩)
        ///</summary>
        public decimal? LandPrice { get; set; }

        ///<summary>
        ///气销量测算(千方/日)
        ///</summary>
        public decimal? GasSaleTotal { get; set; }
        #endregion

        #region 现场法律调研
        public long? LawId { get; set; }
        ///<summary>
        ///调研开始日期
        ///</summary>
        public DateTime? LawResearchStartDate { get; set; }

        ///<summary>
        ///调研截止日期
        ///</summary>
        public DateTime? LawResearchEndDate { get; set; }
        ///<summary>
		///调研人ID
		///</summary>
		public long? LawResearchUserId { get; set; }

        ///<summary>
        ///调研人姓名
        ///</summary>
        public string LawResearchUserName { get; set; }

        ///<summary>
        ///调研人协同OA 编码
        ///</summary>
        public string LawResearchUserXt { get; set; }

        ///<summary>
        ///职务编码(字典)
        ///</summary>
        public string LawJobCode { get; set; }

        ///<summary>
        ///职务名称
        ///</summary>
        public string LawJobName { get; set; }

        ///<summary>
        ///产权清晰
        ///</summary>
        public string PropertyRights { get; set; }

        ///<summary>
        ///资产主体资格
        ///</summary>
        public string AssetSubject { get; set; }

        ///<summary>
        ///纠纷判断
        ///</summary>
        public string DisputesJudgment { get; set; }

        ///<summary>
        ///证照类型
        ///</summary>
        public string License { get; set; }
        #endregion

        #region 经营方面调研
        public long? ManageId { get; set; }
        ///<summary>
        ///调研开始日期
        ///</summary>
        public DateTime? ManageResearchStartDate { get; set; }

        ///<summary>
        ///调研截止日期
        ///</summary>
        public DateTime? ManageResearchEndDate { get; set; }
        ///<summary>
		///调研人ID
		///</summary>
		public long? ManageResearchUserId { get; set; }

        ///<summary>
        ///调研人姓名
        ///</summary>
        public string ManageReaearchUserName { get; set; }

        ///<summary>
        ///调研人协同OA编码
        ///</summary>
        public string ManageReaearchUserXt { get; set; }

        ///<summary>
        ///职务编码(字典)
        ///</summary>
        public string ManageJobCode { get; set; }

        ///<summary>
        ///职务名称
        ///</summary>
        public string ManageJobName { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string SalesRealizability { get; set; }

        ///<summary>
        ///油运品距(公里)
        ///</summary>
        public decimal? CargoDistance { get; set; }

        ///<summary>
        ///油品来源
        ///</summary>
        public string SourceOfOil { get; set; }

        ///<summary>
        ///当前成品油日销量(顿/日)
        ///</summary>
        public decimal? CurrentSalesVolume { get; set; }

        ///<summary>
        ///特殊销售手段
        ///</summary>
        public string SalesMeans { get; set; }

        ///<summary>
        ///当前气销量（千方/日）
        ///</summary>
        public decimal? GasDailySales { get; set; }
        #endregion

        #region 安全方面调研
        public long? SafeId { get; set; }
        ///<summary>
		///调研开始日期
		///</summary>
		public DateTime? SafeResearchStartDate { get; set; }

        ///<summary>
        ///调研截止日期
        ///</summary>
        public DateTime? SafeResearchEndDate { get; set; }

        ///<summary>
        ///调研人ID
        ///</summary>
        public long? SafeResearchUserId { get; set; }

        ///<summary>
        ///调研人姓名
        ///</summary>
        public string SafeReaearchUserName { get; set; }

        ///<summary>
        ///调研人协同OA编码
        ///</summary>
        public string SafeReaearchUserXt { get; set; }

        ///<summary>
        ///职务编码(字典)
        ///</summary>
        public string SafeJobCode { get; set; }

        ///<summary>
        ///职务名称
        ///</summary>
        public string SafeJobName { get; set; }

        ///<summary>
        ///环保问题
        ///</summary>
        public string Environmental { get; set; }

        ///<summary>
        ///改进措施
        ///</summary>
        public string ImprovementMeasures { get; set; }

        ///<summary>
        ///隐患问题
        ///</summary>
        public string hiddenDanger { get; set; }
        #endregion

        public List<Epm_TzAttachs> TzAttachs { get; set; }
    }
}
