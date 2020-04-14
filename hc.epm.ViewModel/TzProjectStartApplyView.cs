using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class TzProjectStartApplyView
    {
        public TzProjectStartApplyView()
        {
            TzAttachs = new List<Epm_TzAttachs>();
            TzAttachsTime = new List<Epm_TzAttachs>();
        }


        ///<summary>
        ///协同
        ///</summary>
        public string WorkFlowId { get; set; }

        ///<summary>
        //ID
        ///</summary>
        public long? Id { get; set; }

        ///<summary>
        ///所属项目ID
        ///</summary>
        public long? ProjectId { get; set; }

        ///<summary>
        ///项目编码（冗余）
        ///</summary>
        public string ProjectCode { get; set; }

        ///<summary>
        ///项目名称（冗余）
        ///</summary>
        public string ProjectName { get; set; }

        ///<summary>
        ///项目批复号
        ///</summary>
        public string ApprovalNo { get; set; }

        ///<summary>
        ///项目性质编码
        ///</summary>
        public string Nature { get; set; }

        ///<summary>
        ///项目性质名称
        ///</summary>
        public string NatureName { get; set; }

        ///<summary>
        ///项目提出日期
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
        ///申请标题
        ///</summary>
        public string ApplyTitle { get; set; }

        ///<summary>
        ///申请人ID
        ///</summary>
        public long? ApplyUserId { get; set; }

        ///<summary>
        ///申请人协同OA编码
        ///</summary>
        public string ApplyUserCodeXt { get; set; }

        ///<summary>
        ///申请人姓名
        ///</summary>
        public string ApplyUserName { get; set; }

        ///<summary>
        ///申请开工日期
        ///</summary>
        public DateTime? StartApplyTime { get; set; }

        ///<summary>
        ///申请部门ID
        ///</summary>
        public long? ApplyDepartmentId { get; set; }

        ///<summary>
        ///申请部门协同编码
        ///</summary>
        public string ApplyDepartmentCodeXt { get; set; }

        ///<summary>
        ///立项批复或初步设计批准文号
        ///</summary>
        public string ReplyNumber { get; set; }

        ///<summary>
        ///申请部门
        ///</summary>
        public string ApplyDepartment { get; set; }

        ///<summary>
        ///申请地市公司ID
        ///</summary>
        public long? ApplyCompanyId { get; set; }

        ///<summary>
        ///申请地市公司协同编码
        ///</summary>
        public string ApplyCompanyCodeXt { get; set; }

        ///<summary>
        ///申请地市公司
        ///</summary>
        public string ApplyCompanyName { get; set; }

        ///<summary>
        ///申请联系电话
        ///</summary>
        public string ApplyTel { get; set; }

        ///<summary>
        ///设计规模
        ///</summary>
        public string DesignScale { get; set; }

        ///<summary>
        ///投资估算额
        ///</summary>
        public decimal? InvestmentEstimateAmount { get; set; }

        ///<summary>
        ///可研报告批准号
        ///</summary>
        public string FeasibilityReport { get; set; }

        ///<summary>
        ///项目建设书批准文号
        ///</summary>
        public string BuildNumber { get; set; }

        ///<summary>
        ///批复投资额
        ///</summary>
        public decimal? ReplyInvestmentAmount { get; set; }

        ///<summary>
        ///资金来源编码
        ///</summary>
        public string FundsSourceType { get; set; }

        ///<summary>
        ///资金来源
        ///</summary>
        public string FundsSource { get; set; }

        ///<summary>
        ///当前投资情况
        ///</summary>
        public string CurrentPlanned { get; set; }

        ///<summary>
        ///计划开工日期
        ///</summary>
        public DateTime? PlanStartTime { get; set; }

        ///<summary>
        ///计划结束日期
        ///</summary>
        public DateTime? PlanEndTime { get; set; }

        public DateTime? CreateTime { get; set; }
        ///<summary>
        ///建设周期
        ///</summary>
        public string BuildCycle { get; set; }

        ///<summary>
        ///工程概况
        ///</summary>
        public string ProjectSummary { get; set; }

        ///<summary>
        ///工程管理机构及人员
        ///</summary>
        public string ProjectManagement { get; set; }

        ///<summary>
        ///建设总体部署
        ///</summary>
        public string BuildDeploy { get; set; }

        ///<summary>
        ///设计单位及图纸落实情况
        ///</summary>
        public string DesignUnits { get; set; }

        ///<summary>
        ///施工单位ID
        ///</summary>
        public long? ConstructionUnitId { get; set; }

        ///<summary>
        ///施工单位
        ///</summary>
        public string ConstructionName { get; set; }

        ///<summary>
        ///项目经理ID
        ///</summary>
        public long? ProjectManagerId { get; set; }

        ///<summary>
        ///项目经理
        ///</summary>
        public string ProjectManager { get; set; }

        ///<summary>
        ///施工安全考试成绩
        ///</summary>
        public string ConstructionScore { get; set; }

        ///<summary>
        ///施工单位落实情况
        ///</summary>
        public string ConstructionSituation { get; set; }

        ///<summary>
        ///监理单位
        ///</summary>
        public string SupervisionUnit { get; set; }

        ///<summary>
        ///监理工程师
        ///</summary>
        public string SupervisionEngineer { get; set; }

        ///<summary>
        ///监理安全考试成绩
        ///</summary>
        public string SupervisionScore { get; set; }

        ///<summary>
        ///监理单位落实情况
        ///</summary>
        public string SupervisionSituation { get; set; }

        ///<summary>
        ///施工前准备工作
        ///</summary>
        public string ConstructionReady { get; set; }

        ///<summary>
        ///主要设备到货情况
        ///</summary>
        public string MainEquipment { get; set; }

        ///<summary>
        ///环境影响
        ///</summary>
        public string Environment { get; set; }

        ///<summary>
        ///工程形象计划
        ///</summary>
        public string ProjectPlan { get; set; }

        ///<summary>
        ///分管领导ID
        ///</summary>
        public long? LeadershipId { get; set; }

        ///<summary>
        ///分管领导协同编码
        ///</summary>
        public string LeadershipCodeXt { get; set; }

        ///<summary>
        ///分管领导名称
        ///</summary>
        public string LeadershipName { get; set; }

        ///<summary>
        ///状态：暂存、待审核、审核通过、审核不通过
        ///</summary>
        public int? State { get; set; }

        ///<summary>
        ///备注
        ///</summary>
        public string Remark { get; set; }

        [NotMapped]
        public List<Epm_TzAttachs> TzAttachs { get; set; }

        //摄像头序列号
        public string CameraSerialNumber { get; set; }
        //验证码
        public string VerificationCode { get; set; }

        /// <summary>
        /// 摄像头地址
        /// </summary>
        public string UrlAddress { get; set; }
        #region 工期管理及外部手续
        ///<summary>
        ///外部手续
        ///</summary>
        public bool? IsCrossings { get; set; }

        ///<summary>
        ///实际停业时间
        ///</summary>
        public DateTime? ShutdownTime { get; set; }

        ///<summary>
        ///计划开工时间
        ///</summary>
        public DateTime? PlanWorkStartTime { get; set; }

        ///<summary>
        ///计划完工时间
        ///</summary>
        public DateTime? PlanWorkEndTime { get; set; }

        ///<summary>
        ///工期
        ///</summary>
        public int? TimeLimit { get; set; }

        ///<summary>
        ///计划开业时间
        ///</summary>
        public DateTime? PlanOpeningTime { get; set; }

        ///<summary>
        ///计划停业时长
        ///</summary>
        public int? PlanShutdowLimit { get; set; }

        [NotMapped]
        public List<Epm_TzAttachs> TzAttachsTime { get; set; }
        #endregion
    }
}
