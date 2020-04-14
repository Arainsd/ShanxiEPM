using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hc.epm.DataModel.Basic;

namespace hc.epm.DataModel.Business
{
    public class BusinessDataContext : DbContext
    {
        public BusinessDataContext() : base("businessConnectionString")
        {

        }

        public DbSet<Epm_AdPutRecord> Epm_AdPutRecord { get; set; }

        public DbSet<Epm_AdTarget> Epm_AdTarget { get; set; }

        public DbSet<Epm_Approver> Epm_Approver { get; set; }

        public DbSet<Epm_Bim> Epm_Bim { get; set; }

        public DbSet<Epm_Change> Epm_Change { get; set; }

        public DbSet<Epm_ChangeCompany> Epm_ChangeCompany { get; set; }

        public DbSet<Epm_CompletionAcceptance> Epm_CompletionAcceptance { get; set; }

        public DbSet<Epm_Contract> Epm_Contract { get; set; }

        public DbSet<Epm_DangerousWork> Epm_DangerousWork { get; set; }

        public DbSet<Epm_WorkUploadRealScene> Epm_WorkUploadRealScene { get; set; }

        public DbSet<Epm_DataConfig> Epm_DataConfig { get; set; }

        public DbSet<Epm_Draw> Epm_Draw { get; set; }

        public DbSet<Epm_Material> Epm_Material { get; set; }

        public DbSet<Epm_MaterialDetails> Epm_MaterialDetails { get; set; }

        public DbSet<Epm_Materiel> Epm_Materiel { get; set; }

        public DbSet<Epm_MaterielDetails> Epm_MaterielDetails { get; set; }

        public DbSet<Epm_Milepost> Epm_Milepost { get; set; }

        public DbSet<Epm_MilepostData> Epm_MilepostData { get; set; }

        public DbSet<Epm_Monitor> Epm_Monitor { get; set; }

        public DbSet<Epm_MonitorDetailBIM> Epm_MonitorDetailBIM { get; set; }

        public DbSet<Epm_MonitorDetails> Epm_MonitorDetails { get; set; }

        public DbSet<Epm_News> Epm_News { get; set; }

        public DbSet<Epm_NewTarget> Epm_NewTarget { get; set; }

        public DbSet<Epm_OilSales> Epm_OilSales { get; set; }

        public DbSet<Epm_OilStation> Epm_OilStation { get; set; }

        public DbSet<Epm_Plan> Epm_Plan { get; set; }

        public DbSet<Epm_PlanComponent> Epm_PlanComponent { get; set; }

        public DbSet<Epm_Project> Epm_Project { get; set; }

        public DbSet<Epm_ProjectCompany> Epm_ProjectCompany { get; set; }

        public DbSet<Epm_ProjectData> Epm_ProjectData { get; set; }

        public DbSet<Epm_ProjectDataSubmit> Epm_ProjectDataSubmit { get; set; }

        public DbSet<Epm_ProjectMilepost> Epm_ProjectMilepost { get; set; }

        public DbSet<Epm_ProjectStateTrack> Epm_ProjectStateTrack { get; set; }

        public DbSet<Epm_Question> Epm_Question { get; set; }

        public DbSet<Epm_QuestionBIM> Epm_QuestionBIM { get; set; }

        public DbSet<Epm_QuestionTrack> Epm_QuestionTrack { get; set; }

        public DbSet<Epm_QuestionUser> Epm_QuestionUser { get; set; }

        public DbSet<Epm_SpecialAcceptance> Epm_SpecialAcceptance { get; set; }

        public DbSet<Epm_SpecialAcceptanceDetails> Epm_SpecialAcceptanceDetails { get; set; }

        public DbSet<Epm_SupervisorLog> Epm_SupervisorLog { get; set; }

        public DbSet<Epm_SupervisorLogDetails> Epm_SupervisorLogDetails { get; set; }

        public DbSet<Epm_SupervisorLogWork> Epm_SupervisorLogWork { get; set; }

        public DbSet<Epm_Template> Epm_Template { get; set; }

        public DbSet<Epm_TemplateDetails> Epm_TemplateDetails { get; set; }

        public DbSet<Epm_Train> Epm_Train { get; set; }

        public DbSet<Epm_TrainCompany> Epm_TrainCompany { get; set; }

        public DbSet<Epm_TrainDetails> Epm_TrainDetails { get; set; }

        public DbSet<Epm_Visa> Epm_Visa { get; set; }

        public DbSet<Epm_VisaCompany> Epm_VisaCompany { get; set; }
        public DbSet<Epm_Notice> Epm_Notice { get; set; }
        public DbSet<Epm_NoticeCompany> Epm_NoticeCompany { get; set; }
        public DbSet<Epm_NoticeProject> Epm_NoticeProject { get; set; }
        public DbSet<Epm_NoticeUser> Epm_NoticeUser { get; set; }
        public DbSet<Epm_MonitorRectifRecord> Epm_MonitorRectifRecord { get; set; }


        public DbSet<Bp_OilStation> Bp_OilStation { get; set; }
        public DbSet<Bp_Organization> Bp_Organization { get; set; }
        public DbSet<Bp_Project> Bp_Project { get; set; }
        public DbSet<Bp_ProjectNature> Bp_ProjectNature { get; set; }
        public DbSet<Bp_Supplier> Bp_Supplier { get; set; }

        public DbSet<Bp_Log> Bp_Log { get; set; }
        public DbSet<Bp_User> Bp_User { get; set; }


        public DbSet<Epm_SupervisorLogCompany> Epm_SupervisorLogCompany { get; set; }
        public DbSet<Epm_ProjectlLogName> Epm_ProjectlLogName { get; set; }
        public DbSet<Epm_AttendanceList> Epm_AttendanceList { get; set; }

        public DbSet<Epm_PlanDelay> Epm_PlanDelay { get; set; }

        public DbSet<Epm_PlanDelayCompany> Epm_PlanDelayCompany { get; set; }

        public DbSet<Epm_CheckItem> EPM_CheckItem { get; set; }
        public DbSet<Epm_Constitute> Epm_Constitute { get; set; }

        public DbSet<Epm_ConstituteWorkMainPoints> Epm_ConstituteWorkMainPoints { get; set; }

        public DbSet<Epm_WorkMainPoints> Epm_WorkMainPoints { get; set; }

        public DbSet<Epm_ProjectNature> Epm_ProjectNature { get; set; }

        public DbSet<Epm_ProjectConstitute> Epm_ProjectConstitute { get; set; }

        public DbSet<Epm_ProjectConstituteHistory> Epm_ProjectConstituteHistory { get; set; }

        public DbSet<Epm_ProjectWorkMainPoints> Epm_ProjectWorkMainPoints { get; set; }

        public DbSet<Epm_ProjectWorkMainPointsHistory> Epm_ProjectWorkMainPointsHistory { get; set; }
        public DbSet<Epm_ProjectContract> Epm_ProjectContract { get; set; }

        public DbSet<Epm_CompletionRectifyCompany> Epm_CompletionRectifyCompany { get; set; }


        public DbSet<Epm_AppVersion> Epm_AppVersion { get; set; }
        
        public DbSet<Epm_ConstituteCompany> Epm_ConstituteCompany { get; set; }
        public DbSet<Epm_ConstituteCompanyDetails> Epm_ConstituteCompanyDetails { get; set; }
        ///<summary>
        ///模型属性表
        ///</summary>
        public DbSet<EPM_CustomProperty> EPM_CustomProperty { get; set; }

        ///<summary>
        ///消息信息
        ///</summary>
        public DbSet<Epm_Massage> Epm_Massage { get; set; }
        public DbSet<Epm_ProjectKPI> Epm_ProjectKPI { get; set; }
        public DbSet<Bp_SendDate> Bp_SendDate { get; set; }

        public DbSet<Epm_Inspect> Epm_Inspect { get; set; }
        public DbSet<Epm_InspectItem> Epm_InspectItem { get; set; }
        public DbSet<Epm_InspectScore> Epm_InspectScore { get; set; }
        public DbSet<Epm_Rectification> Epm_Rectification { get; set; }
        public DbSet<Epm_RectificationItem> Epm_RectificationItem { get; set; }
        public DbSet<Epm_RectificationRecord> Epm_RectificationRecord { get; set; }

        public DbSet<Epm_TzSecondTakl> Epm_TzSecondTakl { get; set; }
        public DbSet<Epm_TzSecondTalkAudit> Epm_TzSecondTalkAudit { get; set; }
        /// <summary>
        /// 人脸注册表
        /// </summary>
        public DbSet<Epm_SignInformation> Epm_SignInformation { get; set; }
        public DbSet<EPM_AIUserFace> EPM_AIUserFace { get; set; }
        public DbSet<EPM_FaceOperateLog> EPM_FaceOperateLog { get; set; }

        /// <summary>
        /// 非常规作业表
        /// </summary>
        public DbSet<EPM_UnconventionalWork> EPM_UnconventionalWork { get; set; }

        /// <summary>
        /// 加油站试运行申请
        /// </summary>
        public DbSet<Epm_TzProjectPolit> Epm_TzProjectPolit { get; set; }


        public DbSet<Epm_ProjectAuditRecord> Epm_ProjectAuditRecord { get; set; }

        public DbSet<Epm_ReformRecord> Epm_ReformRecord { get; set; }

        /// <summary>
        /// 投资相关附件
        /// </summary>
        public DbSet<Epm_TzAttachs> Epm_TzAttachs { get; set; }

        /// <summary>
        /// 投资编码映射
        /// </summary>
        public DbSet<Epm_TzCodeMap> Epm_TzCodeMap { get; set; }

        /// <summary>
        /// 项目提出审核
        /// </summary>
        public DbSet<Epm_TzProSubmissionApprova> Epm_TzProSubmissionApprova { get; set; }

        /// <summary>
        /// 现场调研
        /// </summary>
        public DbSet<Epm_TzSiteSurvey> Epm_TzSiteSurvey { get; set; }

        /// <summary>
        /// 初次谈判
        /// </summary>
        public DbSet<Epm_TzInitialTalk> Epm_TzInitialTalk { get; set; }

        /// <summary>
        /// 土地协议出让谈判信息
        /// </summary>
        public DbSet<Epm_TzLandTalk> Epm_TzLandTalk { get; set; }

        /// <summary>
        /// 评审材料管理员审核
        /// </summary>
        public DbSet<Epm_TzTalkFileAudit> Epm_TzTalkFileAudit { get; set; }

        /// <summary>
        /// 评审材料审核
        /// </summary>
        public DbSet<Epm_TzTalkFileHeadAudit> Epm_TzTalkFileHeadAudit { get; set; }

        /// <summary>
        /// 评审会记录
        /// </summary>
        public DbSet<Epm_TzTalkRecord> Epm_TzTalkRecord { get; set; }

        /// <summary>
        /// 评审会投资部门确认
        /// </summary>
        public DbSet<Epm_TzTalkRecordConfirm> Epm_TzTalkRecordConfirm { get; set; }

        /// <summary>
        /// 评审会签
        /// </summary>
        public DbSet<Epm_TzTalkSign> Epm_TzTalkSign { get; set; }

        /// <summary>
        /// 领导签发
        /// </summary>
        public DbSet<Epm_TzTalkLeaderSign> Epm_TzTalkLeaderSign { get; set; }

        /// <summary>
        /// 项目批复请示
        /// </summary>
        public DbSet<Epm_TzProjectApproval> Epm_TzProjectApproval { get; set; }

        /// <summary>
        /// 工程建设项目开工报告
        /// </summary>
        public DbSet<Epm_TzStartsApply> Epm_TzStartsApply { get; set; }

        /// <summary>
        /// 建设工程设计变更申请
        /// </summary>
        public DbSet<Epm_TzDesiginChangeApply> Epm_TzDesiginChangeApply { get; set; }

        public DbSet<Epm_TzDevResourceReportItem> Epm_TzDevResourceReportItem { get; set; }
        public DbSet<Epm_TzDevResourceReport> Epm_TzDevResourceReport { get; set; }
        public DbSet<Epm_TzPeopleChgApplyItem> Epm_TzPeopleChgApplyItem { get; set; }
        public DbSet<Epm_TzPeopleChgApply> Epm_TzPeopleChgApply { get; set; }
        public DbSet<Epm_TzRivalStationReport> Epm_TzRivalStationReport { get; set; }

        ///<summary>
        ///工程甲供物资订单表
        ///</summary>
        public DbSet<Epm_TzGcGoodsOrdersApply> Epm_TzGcGoodsOrdersApply { get; set; }
        ///<summary>
        ///工程甲供物资订单详细表
        ///</summary>
        public DbSet<Epm_TzGcGoodsOrdersItem> Epm_TzGcGoodsOrdersItem { get; set; }

        public DbSet<Epm_TzProjectProposal> Epm_TzProjectProposal { get; set; }

        public DbSet<Epm_TzResearchOfEngineering> Epm_TzResearchOfEngineering { get; set; }
        public DbSet<Epm_TzResearchOfInformation> Epm_TzResearchOfInformation { get; set; }
        public DbSet<Epm_TzResearchOfInvestment> Epm_TzResearchOfInvestment { get; set; }
        public DbSet<Epm_TzResearchOfLaw> Epm_TzResearchOfLaw { get; set; }
        public DbSet<Epm_TzResearchOfManagement> Epm_TzResearchOfManagement { get; set; }
        public DbSet<Epm_TzResearchOfSecurity> Epm_TzResearchOfSecurity { get; set; }

        ///<summary>
        ///初次谈判
        ///</summary>
        public DbSet<Epm_TzFirstNegotiation> Epm_TzFirstNegotiation { get; set; }
        ///<summary>
        ///土地谈判协议
        ///</summary>
        public DbSet<Epm_TzLandNegotiation> Epm_TzLandNegotiation { get; set; }

        ///<summary>
        ///上会材料上报
        ///</summary>
        public DbSet<Epm_MeetingFileReport> Epm_MeetingFileReport { get; set; }
        ///<summary>
        ///项目批复信息
        ///</summary>
        public DbSet<Epm_TzProjectApprovalInfo> Epm_TzProjectApprovalInfo { get; set; }
        ///<summary>
        ///项目评审记录
        ///</summary>
        public DbSet<Epm_TzProjectReveiews> Epm_TzProjectReveiews { get; set; }

        ///<summary>
        ///评审材料上报
        ///</summary>
        public DbSet<Epm_TzFormTalkFile> Epm_TzFormTalkFile { get; set; }

        ///<summary>
        ///招标结果
        ///</summary>
        public DbSet<Epm_TzBidResult> Epm_TzBidResult { get; set; }
        ///<summary>
        ///施工图纸会审
        ///</summary>
        public DbSet<Epm_TzConDrawing> Epm_TzConDrawing { get; set; }
        ///<summary>
        ///项目设计方案
        ///</summary>
        public DbSet<Epm_TzDesignScheme> Epm_TzDesignScheme { get; set; }
        ///<summary>
        ///开工申请表
        ///</summary>
        public DbSet<Epm_TzProjectStartApply> Epm_TzProjectStartApply { get; set; }
        ///<summary>
        ///甲供申请物资详情
        ///</summary>
        public DbSet<Epm_TzSupMatApplyList> Epm_TzSupMatApplyList { get; set; }
        ///<summary>
        ///甲供物资管理
        ///</summary>
        public DbSet<Epm_TzSupMatManagement> Epm_TzSupMatManagement { get; set; }
        ///<summary>
        ///甲供物资申请单
        ///</summary>
        public DbSet<Epm_TzSupplyMaterialApply> Epm_TzSupplyMaterialApply { get; set; }
        ///<summary>
        ///招标申请
        ///</summary>
        public DbSet<Epm_TzTenderingApply> Epm_TzTenderingApply { get; set; }

        ///<summary>
        ///工期管理和外部手续
        ///</summary>
        public DbSet<Epm_TimeLimitAndProcedure> Epm_TimeLimitAndCrossings { get; set; }

        ///<summary>
        ///竣工验收
        ///</summary>
        public DbSet<Epm_CompletionAcceptanceResUpload> Epm_CompletionAcceptanceResUpload { get; set; }

        ///<summary>
        ///项目考勤设置表
        ///</summary>
        public DbSet<Epm_ProjectAttendance> Epm_ProjectAttendance { get; set; }

        #region 机器人回写
        ///<summary>
        ///
        ///</summary>
        public DbSet<RPA_FieldInvestigation> RPA_FieldInvestigation { get; set; }
        public DbSet<OMADS_FieldInvestigation> OMADS_FieldInvestigation { get; set; }
        public DbSet<TEMP_FieldInvestigation> TEMP_FieldInvestigation { get; set; }

        ///<summary>
        ///
        ///</summary>
        public DbSet<RPA_ProjectProposal> RPA_ProjectProposal { get; set; }
        public DbSet<OMADS_ProjectProposal> OMADS_ProjectProposal { get; set; }
        public DbSet<TEMP_ProjectProposal> TEMP_ProjectProposal { get; set; }
        ///<summary>
        ///
        ///</summary>
        public DbSet<RPA_TzConDrawing> RPA_TzConDrawing { get; set; }
        public DbSet<OMADS_TzConDrawing> OMADS_TzConDrawing { get; set; }
        public DbSet<TEMP_TzConDrawing> TEMP_TzConDrawing { get; set; }
        ///<summary>
        ///
        ///</summary>
        public DbSet<RPA_TzFirstNegotiation> RPA_TzFirstNegotiation { get; set; }
        public DbSet<OMADS_TzFirstNegotiation> OMADS_TzFirstNegotiation { get; set; }
        public DbSet<TEMP_TzFirstNegotiation> TEMP_TzFirstNegotiation { get; set; }
        ///<summary>
        ///
        ///</summary>
        public DbSet<RPA_TzFormTalkFileHX> RPA_TzFormTalkFileHX { get; set; }
        public DbSet<OMADS_TzFormTalkFileHX> OMADS_TzFormTalkFileHX { get; set; }
        public DbSet<TEMP_TzFormTalkFileHX> TEMP_TzFormTalkFileHX { get; set; }
        ///<summary>
        ///
        ///</summary>
        public DbSet<RPA_TzLandNegotiation> RPA_TzLandNegotiation { get; set; }
        public DbSet<OMADS_TzLandNegotiation> OMADS_TzLandNegotiation { get; set; }
        public DbSet<TEMP_TzLandNegotiation> TEMP_TzLandNegotiation { get; set; }
        ///<summary>
        ///
        ///</summary>
        public DbSet<RPA_TzProjectApprovalHX> RPA_TzProjectApprovalHX { get; set; }
        public DbSet<OMADS_TzProjectApprovalHX> OMADS_TzProjectApprovalHX { get; set; }
        public DbSet<TEMP_TzProjectApprovalHX> TEMP_TzProjectApprovalHX { get; set; }
        ///<summary>
        ///
        ///</summary>
        public DbSet<RPA_TzProjectApprovalInfo> RPA_TzProjectApprovalInfo { get; set; }
        public DbSet<OMADS_TzProjectApprovalInfo> OMADS_TzProjectApprovalInfo { get; set; }
        public DbSet<TEMP_TzProjectApprovalInfo> TEMP_TzProjectApprovalInfo { get; set; }
        #endregion
        /// <summary>
        /// 板块公司方案审查审核
        /// </summary>
        public DbSet<RPA_BoardCompanyReveiews> RPA_BoardCompanyReveiews { get; set; }
        public DbSet<OMADS_BoardCompanyReveiews> OMADS_BoardCompanyReveiews { get; set; }
        public DbSet<TEMP_BoardCompanyReveiews> TEMP_BoardCompanyReveiews { get; set; }
        /// <summary>
        /// 地区公司方案审核
        /// </summary>
        public DbSet<RPA_CompanyProjectReveiews> RPA_CompanyProjectReveiews { get; set; }
        public DbSet<OMADS_CompanyProjectReveiews> OMADS_CompanyProjectReveiews { get; set; }
        public DbSet<TEMP_CompanyProjectReveiews> TEMP_CompanyProjectReveiews { get; set; }
        /// <summary>
        /// 项目主信息
        /// </summary>
        public DbSet<RPA_ProjectInfo> RPA_ProjectInfo { get; set; }
        public DbSet<OMADS_ProjectInfo> OMADS_ProjectInfo { get; set; }
        public DbSet<TEMP_ProjectInfo> TEMP_ProjectInfo { get; set; }
        /// <summary>
        /// 竣工信息
        /// </summary>
        public DbSet<RPA_CompletionInfo> RPA_CompletionInfo { get; set; }
        public DbSet<OMADS_CompletionInfo> OMADS_CompletionInfo { get; set; }
        public DbSet<TEMP_CompletionInfo> TEMP_CompletionInfo { get; set; }

        public DbSet<RPA_ReformRecordHX> RPA_ReformRecordHX { get; set; }
        public DbSet<OMADS_ReformRecordHX> OMADS_ReformRecordHX { get; set; }
        public DbSet<TEMP_ReformRecordHX> TEMP_ReformRecordHX { get; set; }

        /// <summary>
        /// 设计方案
        /// </summary>
        public DbSet<RPA_TzDesignScheme> RPA_TzDesignScheme { get; set; }
        public DbSet<OMADS_TzDesignScheme> OMADS_TzDesignScheme { get; set; }
        public DbSet<TEMP_TzDesignScheme> TEMP_TzDesignScheme { get; set; }
        public DbSet<RPA_TzProjectReveiews> RPA_TzProjectReveiews { get; set; }
        public DbSet<OMADS_TzProjectReveiews> OMADS_TzProjectReveiews { get; set; }
        public DbSet<TEMP_TzProjectReveiews> TEMP_TzProjectReveiews { get; set; }

        #region 投资系统数据临时表

        public DbSet<Temp_ReformRecord> Temp_ReformRecord { get; set; }
        public DbSet<Temp_TzAttachs> Temp_TzAttachs { get; set; }
        public DbSet<Temp_TzFormTalkFile> Temp_TzFormTalkFile { get; set; }
        public DbSet<Temp_TzProjectProposal> Temp_TzProjectProposal { get; set; }
        public DbSet<Temp_TzInitialTalk> Temp_TzInitialTalk { get; set; }
        public DbSet<Temp_TzLandTalk> Temp_TzLandTalk { get; set; }
        public DbSet<Temp_TzProjectApproval> Temp_TzProjectApproval { get; set; }
        public DbSet<Temp_TzProjectPolit> Temp_TzProjectPolit { get; set; }
        public DbSet<Temp_TzProSubmissionApprova> Temp_TzProSubmissionApprova { get; set; }
        public DbSet<Temp_TzSecondTakl> Temp_TzSecondTakl { get; set; }
        public DbSet<Temp_TzSecondTalkAudit> Temp_TzSecondTalkAudit { get; set; }
        public DbSet<Temp_TzSiteSurvey> Temp_TzSiteSurvey { get; set; }
        public DbSet<Temp_TzTalkFileAudit> Temp_TzTalkFileAudit { get; set; }
        public DbSet<Temp_TzTalkFileHeadAudit> Temp_TzTalkFileHeadAudit { get; set; }
        public DbSet<Temp_TzTalkLeaderSign> Temp_TzTalkLeaderSign { get; set; }
        public DbSet<Temp_TzTalkRecord> Temp_TzTalkRecord { get; set; }
        public DbSet<Temp_TzTalkRecordConfirm> Temp_TzTalkRecordConfirm { get; set; }
        public DbSet<Temp_TzTalkSign> Temp_TzTalkSign { get; set; }

        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //解决DbContext保存decimal类型数据到数据库，默认只会保存小数点后的前2位小数，其余均置0的问题，这样就会保存小数点7位了
            modelBuilder.Entity<Epm_Project>().Property(p => p.Amount).HasPrecision(18, 6);
            modelBuilder.Entity<Epm_Project>().Property(p => p.BalanceAmount).HasPrecision(18, 6);
            modelBuilder.Entity<Epm_Project>().Property(p => p.AProvideAmount).HasPrecision(18, 6);
            modelBuilder.Entity<Epm_ProjectConstitute>().Property(p => p.Amount).HasPrecision(18, 6);
            modelBuilder.Entity<Epm_ProjectConstituteHistory>().Property(p => p.Amount).HasPrecision(18, 6);
            modelBuilder.Entity<Epm_ProjectWorkMainPoints>().Property(p => p.Qty).HasPrecision(18, 6);
            modelBuilder.Entity<Epm_ProjectWorkMainPointsHistory>().Property(p => p.Qty).HasPrecision(18, 6);
            modelBuilder.Entity<Epm_Contract>().Property(p => p.Amount).HasPrecision(18, 6);
            modelBuilder.Entity<Epm_Change>().Property(p => p.TotalAmount).HasPrecision(18, 6);
            modelBuilder.Entity<Epm_Change>().Property(p => p.AddAmount).HasPrecision(18, 6);
            modelBuilder.Entity<Epm_Change>().Property(p => p.ReduceAmount).HasPrecision(18, 6);
            modelBuilder.Entity<Epm_Change>().Property(p => p.ChangeAmount).HasPrecision(18, 6);
            modelBuilder.Entity<Epm_Visa>().Property(p => p.VisaAmount).HasPrecision(18, 6);
        }
    }
}

