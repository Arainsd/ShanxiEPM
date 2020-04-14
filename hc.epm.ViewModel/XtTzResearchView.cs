using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class XtTzResearchView
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
        /// 起始时间
        /// </summary>
        public string ResearchStartDate { get; set; }
        /// <summary>
        /// 详细地理位置
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 周边环境
        /// </summary>
        public string EnvironmentTypeName { get; set; }
        /// <summary>
        /// 土地性质
        /// </summary>
        public string LandNatureName { get; set; }
        /// <summary>
        /// 土地用途
        /// </summary>
        public string LandUseName { get; set; }
        /// <summary>
        /// 土地面积
        /// </summary>
        public string LandArea { get; set; }
        /// <summary>
        /// 土地形状（临街长度）
        /// </summary>
        public string LandShape { get; set; }
        /// <summary>
        /// 是否符合地区建站规划
        /// </summary>
        public string IsMeetAreaPlan { get; set; }
        /// <summary>
        /// 周边车辆保有量
        /// </summary>
        public string AroundCarCount { get; set; }
        /// <summary>
        /// 平均日车流量
        /// </summary>
        public string DailyTraffic { get; set; }
        /// <summary>
        /// 成品油销量测算
        /// </summary>
        public string OilSaleTotal { get; set; }
        /// <summary>
        /// 柴汽比
        /// </summary>
        public string DieselGasolineRatio { get; set; }
        /// <summary>
        /// 土地价格
        /// </summary>
        public string LandPrice { get; set; }
        /// <summary>
        /// 气销量测算
        /// </summary>
        public string GasSaleTotal { get; set; }
        /// <summary>
        /// 产权清晰
        /// </summary>
        public string PropertyRights { get; set; }
        /// <summary>
        /// 资产主体资格
        /// </summary>
        public string AssetSubject { get; set; }
        /// <summary>
        /// 纠纷判断
        /// </summary>
        public string DisputesJudgment { get; set; }
        /// <summary>
        /// 证照类型
        /// </summary>
        public string License { get; set; }
        /// <summary>
        /// 形象工程是否符合行业规划
        /// </summary>
        public string IndustryPlanning { get; set; }
        /// <summary>
        /// 罩棚
        /// </summary>
        public string Shelter { get; set; }
        /// <summary>
        /// 油罐
        /// </summary>
        public string OilTank { get; set; }
        /// <summary>
        ///  加油枪(台)
        /// </summary>
        public string MachineOfOilStage { get; set; }
        /// <summary>
        /// 加油枪(个)
        /// </summary>
        public string MachineOfOil { get; set; }
        /// <summary>
        /// 加气枪(台)
        /// </summary>
        public string MachineOfGasStage { get; set; }
        /// <summary>
        /// 加气枪(个) 
        /// </summary>
        public string MachineOfGas { get; set; }
        /// <summary>
        /// 站房
        /// </summary>
        public string StationRoon { get; set; }
        /// <summary>
        /// 储气井
        /// </summary>
        public string GasWells { get; set; }
        /// <summary>
        /// 信息化系统
        /// </summary>
        public string HasInformationSystem { get; set; }
        /// <summary>
        /// 当前成品油日销量
        /// </summary>
        public string CurrentSalesVolume { get; set; }
        /// <summary>
        /// 改造必要性
        /// </summary>
        public string ReformName { get; set; }
        /// <summary>
        /// 油运品距
        /// </summary>
        public string CargoDistance { get; set; }
        /// <summary>
        /// 当前气日销量
        /// </summary>
        public string GasDailySales { get; set; }
        /// <summary>
        /// 特殊销售手段
        /// </summary>
        public string SalesMeans { get; set; }
        /// <summary>
        /// 销量可实现性
        /// </summary>
        public string SalesRealizability { get; set; }
        /// <summary>
        /// 油品来源
        /// </summary>
        public string SourceOfOil { get; set; }
        /// <summary>
        /// 环保问题
        /// </summary>
        public string Environmental { get; set; }
        /// <summary>
        /// 隐患问题
        /// </summary>
        public string hiddenDanger { get; set; }
        /// <summary>
        /// 改进措施
        /// </summary>
        public string ImprovementMeasures { get; set; }
        /// <summary>
        /// 改进措施
        /// </summary>
        public string Improvement { get; set; }
        /// <summary>
        /// 附件类型
        /// </summary>
        public string Temp_TzAttachs { get; set; }
        /// <summary>
        /// 投资调研人
        /// </summary>
        public string tzResearchUserName { get; set; }
        /// <summary>
        /// 投资职务
        /// </summary>
        public string tzJobName { get; set; }
        /// <summary>
        /// 法律调研人
        /// </summary>
        public string flResearchUserName { get; set; }
        /// <summary>
        /// 法律职务
        /// </summary>
        public string flJobName { get; set; }
        /// <summary>
        /// 工程调研人
        /// </summary>
        public string gcResearchUserName { get; set; }
        /// <summary>
        /// 工程职务
        /// </summary>
        public string gcJobName { get; set; }
        /// <summary>
        /// 经营调研人
        /// </summary>
        public string jyResearchUserName { get; set; }
        /// <summary>
        /// 经营职务
        /// </summary>
        public string jyJobName { get; set; }
        /// <summary>
        /// 安全调研人
        /// </summary>
        public string aqResearchUserName { get; set; }
        /// <summary>
        /// 安全职务
        /// </summary>
        public string aqJobName { get; set; }
        /// <summary>
        /// 信息调研人
        /// </summary>
        public string xxResearchUserName { get; set; }
        /// <summary>
        /// 信息职务
        /// </summary>
        public string xxJobName { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string hr_sqr { get; set; }
    }
}
