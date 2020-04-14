using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class TzDesignSchemeWorkFlowView
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
        /// 项目类型
        /// </summary>
        public string StationTypeName { get; set; }
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ProjectCode { get; set; }
        /// <summary>
        /// 地区公司
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 地市公司
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// 估算投资
        /// </summary>
        public string PredictMoney { get; set; }
        /// <summary>
        /// 初步设计单位
        /// </summary>
        public string DesignUnit { get; set; }
        /// <summary>
        /// 示范/标注
        /// </summary>
        public string StandarName { get; set; }
        /// <summary>
        /// 上报概算
        /// </summary>
        public string Estimate { get; set; }
        /// <summary>
        /// 工程费用
        /// </summary>
        public string TotalInvestment { get; set; }
        /// <summary>
        /// 工程其他费用
        /// </summary>
        public string OtheInvestment { get; set; }
        /// <summary>
        /// 设计单位招标时间
        /// </summary>
        public string InviteTime { get; set; }
        /// <summary>
        /// 设计单位负责人
        /// </summary>
        public string DesignUnitCharge { get; set; }
        /// <summary>
        /// 职务
        /// </summary>
        public string DesignJob { get; set; }
        /// <summary>
        /// 项目经理
        /// </summary>
        public string ProjectManager { get; set; }

        /// <summary>
        /// 职务(项目经理)
        /// </summary>
        public string ProjectJob { get; set; }
        /// <summary>
        /// 占地面积
        /// </summary>
        public string LandArea { get; set; }
        /// <summary>
        /// 加油机
        /// </summary>
        public string MachineofOilStage { get; set; }

        /// <summary>
        /// 加气机
        /// </summary>
        public string MachineofGasStage { get; set; }
        /// <summary>
        /// 储气机
        /// </summary>
        public string GasWells { get; set; }
        /// <summary>
        /// 罐容
        /// </summary>
        public string OilTank { get; set; }
        /// <summary>
        /// 罩棚面积
        /// </summary>
        public string Shelter { get; set; }
        /// <summary>
        /// 站房面积
        /// </summary>
        public string StationRoom { get; set; }
        /// <summary>
        /// 便利店面积
        /// </summary>
        public string ConvenienceRoom { get; set; }
        /// <summary>
        /// 批复概算投资
        /// </summary>
        public string ReleaseInvestmentAmount { get; set; }
        /// <summary>
        /// 批复文号
        /// </summary>
        public string ApprovalNo { get; set; }
        /// <summary>
        /// 附件类型
        /// </summary>
        public string Temp_TzAttachs { get; set; }
        /// <summary>
        /// 其他工程内容
        /// </summary>
        public string OtherProject { get; set; }
        /// <summary>
        /// 项目信息是否与实际同步
        /// </summary>
        public string IsSynchro { get; set; }
        // <summary>
        /// 创建人Id 对应表 Base_User 中的 ObjeId
        /// </summary>
        public string hr_sqr { get; set; }
        /// <summary>
        /// 土地费用
        /// </summary>
        public string LandCosts { get; set; }
        /// <summary>
        /// 工程费用
        /// </summary>
        public string EngineeringCost { get; set; }
        /// <summary>
        /// 其他费用
        /// </summary>
        public string OtherExpenses { get; set; }
    }
    
}
