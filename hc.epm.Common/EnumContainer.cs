using hc.Plat.Common.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Common
{
    #region 业务枚举
    /// <summary>
    /// 业务类型
    /// </summary>
    public enum BusinessType
    {
        [EnumText("审批记录")]
        Approver = 99,
        [EnumText("施工计划")]
        Plan = 1,
        [EnumText("进度跟踪")]
        Schedule = 2,
        [EnumText("安全培训")]//暂未用
        SecurityTrain = 3,
        [EnumText("危险作业")]
        Dangerous = 4,
        [EnumText("现场检查")]//原为安全检查
        SecurityCheck = 5,
        [EnumText("质量培训")]//暂未用
        QualityTrain = 6,
        [EnumText("质量检查")]//暂未用
        QualityCheck = 7,
        [EnumText("监理日志")]
        Log = 8,
        [EnumText("变更")]
        Change = 9,
        [EnumText("签证")]
        Visa = 10,
        [EnumText("图纸")]
        Draw = 11,
        [EnumText("模型")]
        Model = 12,
        [EnumText("工器具机")]//材料验收
        Equipment = 13,
        [EnumText("材料设备")]//物料接收
        Track = 14,
        [EnumText("专项验收")]//暂未用
        Special = 15,
        [EnumText("完工验收")]//原为竣工验收 Level 2 Checklist
        Completed = 16,
        [EnumText("变更计划")]
        DelayApply = 17,
        [EnumText("项目")]
        Project = 18,
        [EnumText("合同")]
        Contract = 19,
        [EnumText("问题沟通")]
        Question = 20,
        [EnumText("整改单")]
        Rectification = 21,
    }
    #endregion

    #region 业务状态
    /// <summary>
    /// 项目状态，枚举 0草稿、5在建、10结项、15终止、20作废、25无效
    /// </summary>
    public enum ProjectState
    {
        [EnumText("未开工")]
        NoStart = 1,
        [EnumText("在建")]
        Construction = 5,
        [EnumText("完工")]
        Success = 10,
        [EnumText("终止")]
        Failure = 15,
        [EnumText("作废")]
        Discard = 20,
        [EnumText("无效")]
        Invalid = 25,
        [EnumText("落地")]
        Landing = 30,
        [EnumText("投运")]
        Commissioning = 35,
    }
    /// <summary>
    /// 审批状态，枚举 0草稿、30待审核、35审核通过、40审核不通过、60已废弃、63施工中、62已完成
    /// </summary>
    public enum ApprovalState
    {
        [EnumText("草稿")]
        Enabled = 3,
        [EnumText("待审核")]
        WaitAppr = 30,
        [EnumText("审核通过")]
        ApprSuccess = 35,
        [EnumText("审核不通过")]
        ApprFailure = 40,

        [EnumText("待验收")]
        WorkPartAppr = 42,
        [EnumText("已完成")]
        WorkFinish = 43,

        [EnumText("已废弃")]
        Discarded = 60,
    }

    ///// <summary>
    ///// 作业实景状态 61待审核、64审核不通过、65审核通过
    ///// </summary>
    //public enum RealSceneState
    //{
    //    [EnumText("待审核")]
    //    WorkWaitAppr = 61,
    //    [EnumText("审核不通过")]
    //    WorkApprFailure = 64,
    //    [EnumText("审核通过")]
    //    WorkApprSuccess = 65,
    //}
    /// <summary>
    /// 确认状态，枚举 0草稿、45待确认、50确认通过、55确认不通过、60已废弃
    /// </summary>
    public enum ConfirmState
    {
        [EnumText("草稿")]
        Enabled = 3,
        [EnumText("待确认")]
        WaitConfirm = 45,
        [EnumText("确认通过")]
        Confirm = 50,
        [EnumText("确认不通过")]
        ConfirmFailure = 55,
        [EnumText("已废弃")]
        Discarded = 60,
    }
    ///// <summary>
    ///// 检查状态，枚举 65待检查、70检查通过、75整改中、80整改后通过
    ///// </summary>
    //public enum CheckState
    //{
    //    [EnumText("待检查")]
    //    WaitCheck = 65,
    //    [EnumText("检查通过")]
    //    CheckSuccess = 70,
    //    [EnumText("整改中")]
    //    Rectification = 75,
    //    [EnumText("整改后通过")]
    //    UpdateOk = 80,
    //}
    /// <summary>
    /// 整改状态，枚举 85待整改、90已整改、95整改通过、99整改不通过
    /// </summary>
    public enum RectificationState
    {
        [EnumText("待整改")]
        WaitRectification = 85,
        [EnumText("已整改")]
        Rectificationed = 90,
        [EnumText("整改通过")]
        RectificationSuccess = 95,
        [EnumText("整改不通过")]
        RectificationOk = 99,
        [EnumText("已完成")]
        WorkFinish = 43,
    }
    /// <summary>
    /// 模型状态
    /// </summary>
    public enum BIMModelState
    {
        [EnumText("未上传")]
        NotBIM = 100,
        [EnumText("模型轻量化")]
        BIMLightWeight = 110,
        [EnumText("轻量化完成")]
        BIMLightWeightSuccess = 120,
    }

    /// <summary>
    /// 项目前期所有状态
    /// </summary>
    public enum PreProjectState
    {
        [EnumText("待提交")]
        WaitSubmitted = 0,
        [EnumText("暂存")]
        TemporaryStorage = 2,
        [EnumText("已提交")]
        Submitted = 5,
        [EnumText("待审批")]
        WaitApproval = 10,
        [EnumText("已批复")]
        ApprovalSuccess = 15,
        [EnumText("已驳回")]
        ApprovalFailure = 20,
        [EnumText("作废")]
        Discarded = 22,
        [EnumText("关闭")]
        Closed = 25,
    }

    /// <summary>
    /// 投资管理审批状态（项目提出、评审材料上报，项目评审记录，会议决策，项目批复）（施工图纸设计方案）
    /// </summary>
    public enum PreProjectApprovalState
    {
        [EnumText("待提交")]
        WaitSubmitted = 0,
        [EnumText("待审批")]
        WaitApproval = 10,
        [EnumText("已批复")]
        ApprovalSuccess = 15,
        [EnumText("已驳回")]
        ApprovalFailure = 20,
        [EnumText("关闭")]
        Closed = 25,
    }

    /// <summary>
    /// 甲供物资申请状态
    /// </summary>
    public enum SupApprovalState
    {
        [EnumText("待提交")]
        WaitSubmitted = 0,
        [EnumText("待审批")]
        WaitApproval = 10,
        [EnumText("已批复")]
        ApprovalSuccess = 15,
        [EnumText("已驳回")]
        ApprovalFailure = 20,
        [EnumText("作废")]
        Discarded = 22,
    }
    
    public enum PreCompletionScceptanceState
    {
        [EnumText("待提交")]
        WaitSubmitted = 0,
        [EnumText("待审批")]
        WaitApproval = 10,
        [EnumText("已批复")]
        ApprovalSuccess = 15,
        [EnumText("已驳回")]
        ApprovalFailure = 20,
    }

    /// <summary>
    /// 投资管理提交状态（现场踏勘，项目谈判/土地出让协议谈判）
    /// </summary>
    public enum PreProjectSubmitState
    {
        [EnumText("待提交")]
        WaitSubmitted = 0,
        [EnumText("已提交")]
        Submitted = 5,
        [EnumText("关闭")]
        Closed = 25,
    }
    /// <summary>
    /// 周报Type 1:全部，2：新增，3：改造
    /// </summary>
    public enum ProjectType
    {
        [EnumText("全部")]
        All = 1,
        [EnumText("新增")]
        NewAdd = 2,
        [EnumText("改造")]
        Modify = 3
    }
    /// <summary>
    /// 周报stateType 1:新增项目汇总，2：未完成设计，3：完工未投运，4：正在施工，5：改造项目汇总，6:在建，7:未开工
    /// </summary>
    public enum ProjectStateType
    {
        [EnumText("新增项目汇总")]
        NewProjectCount = 1,
        [EnumText("未完成设计")]
        UnfinishedDesign = 2,
        [EnumText("完工未投运")]
        CompletedNotOperational = 3,
        [EnumText("正在施工")]
        UnderConstruction = 4,
        [EnumText("改造项目汇总")]
        RetrofitProjectSummary = 5,
        [EnumText("在建")]
        Construction = 6,
        [EnumText("未开工")]
        NotStarted = 7
    }
    #endregion

    #region 数据字典
    /// <summary>
    /// 字典和类型。
    /// </summary>
    public enum DictionaryType
    {
        [EnumText("项目主体")]
        ProjectSubject = 4,
        [EnumText("危险作业分类")]
        WorkType = 8,
        [EnumText("天气")]
        Weather = 12,
        [EnumText("图纸版本")]
        Draw = 13,
        [EnumText("模型版本")]
        Model = 14,
        [EnumText("广告类型")]//广告类型[轮播图]
        AdType = 17,
        [EnumText("新闻分类")]
        NewsCategory = 18,
        [EnumText("方案类型")]
        SchemeType = 19,
        [EnumText("总批复及构成")]
        Constitute = 20,
        [EnumText("甲供设备")]
        AProvide = 21,
        [EnumText("工程内容项")]
        WorkMainPoints = 22,
        [EnumText("签证类型")]
        VisaType = 23,
        [EnumText("变更金额审批线")]
        ChangeRatio = 24,
        [EnumText("问题类型")]
        ProblemType = 25,
        [EnumText("里程碑类型")]
        MilepostType = 26,

        [EnumText("职称")]
        ProfessionalType = 27,
        [EnumText("岗位")]
        PostType = 28,
        [EnumText("职业资格")]
        QualificationType = 29,
        [EnumText("工种")]
        Job_Scopes = 30,
        [EnumText("责任单位")]
        rectificationDw = 31,

        [EnumText("项目性质")]
        ProjectNature = 32,
        [EnumText("加油站类别")]
        GasStationType = 33,
        [EnumText("改造类型")]
        ReformType = 34,
        [EnumText("资金来源")]
        CapitalSource = 35,
        [EnumText("限上/限下")]
        Limit = 36,
        [EnumText("地理位置")]
        GeographicDosition = 38,
        [EnumText("谈判结果")]
        NegotiateResult = 39,
        [EnumText("物资种类")]
        MaterialNumber = 40,
        [EnumText("上会材料类别")]
        UpperConference = 41,
        [EnumText("项目评审记录附件类别")]
        ProjectReviewRecord = 42,

        [EnumText("周边环境")]
        Environment = 43,
        [EnumText("土地性质")]
        NatureLand = 44,
        [EnumText("土地用途")]
        LandUse = 45,
        [EnumText("职务")]
        Job = 46,

        [EnumText("证照类型")]
        PermitType = 47,
        [EnumText("形象工程符合行业规划")]
        VanityProject = 48,
        [EnumText("改造必要性")]
        ReformNeed = 49,

        [EnumText("项目提出附件类型")]
        FileType = 50,
        [EnumText("现场踏勘附件类型")]
        ResearchFileType = 51,
        [EnumText("谈判附件类型")]
        NegotiationFileType = 52,
        [EnumText("上会材料上报附件类型")]
        MeetingFileType = 53,
        [EnumText("项目评审记录附件类型")]
        ReveiewsFileType = 54,
        [EnumText("项目批复信息附件类型")]
        ApprovalInfoFileType = 55,
        [EnumText("评审材料上报附件类型")]
        PsclsbFileType = 56,
        [EnumText("土地费用支付类型")]
        LandPaymentType = 57,
        [EnumText("评审结论")]
        ConclusionCode = 58,
        [EnumText("项目评审其他信息")]
        OtherInfo = 59,


        [EnumText("示范/标准类别")]
        StandardType = 60,


        [EnumText("设计方案附件类型")]
        EnclosureType = 61,
        [EnumText("施工图纸附件类型")]
        DrawingType = 62,
        
        [EnumText("招标方式")]
        BiddingMethod = 63,

        [EnumText("招标附件类型")]
        TenderingFileType = 64,

        [EnumText("项目类型")]
        ProjectType = 65,

        [EnumText("招标类型")]
        TenderingType = 66,

        [EnumText("三商管理附件类型-供应商")]
        SupplierFileType = 67,

        [EnumText("工期管理及外部手续附件类型")]
        TimeAndCrossingsType = 68,

        [EnumText("竣工验收附件类型")]
        AcceptanceCheckType = 69,

        [EnumText("人员管理附件类型")]
        UserFileType = 70,

        [EnumText("试运行申请附件类型")]
        PolitFileType = 71,

        [EnumText("三商管理附件类型-承包商")]
        ContractorFileType = 72,
        [EnumText("三商管理附件类型-服务商")]
        ServiceProviderFileType = 73,

        [EnumText("库站改造改造类型")]
        StationReformType = 74,
        [EnumText("库站改造附件类型")]
        StationReformFileType = 75,

        [EnumText("三商管理级别")]
        LevelType = 76,

        [EnumText("三商管理服务商类别")]
        SSGLType = 77,
    }
    #endregion

    #region 消息
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        [EnumText("站内信")]
        Message = 1,
        [EnumText("邮箱")]
        Email = 2,
        [EnumText("短信")]
        SMS = 3
    }
    #region 消息发送环节
    /// <summary>
    /// 消息发送环节
    /// </summary>
    public enum MessageStep
    {
        [EnumText("新建项目")]
        ProjectAdd = 1,
        [EnumText("更新项目")]
        ProjectUpdate = 2,
        [EnumText("项目终结")]
        ProjectFailure = 3,
        [EnumText("项目归档成功")]
        ProjectAudit = 4,
        [EnumText("项目归档失败")]
        ProjectReject = 5,
        [EnumText("更新服务商经理")]
        UpdateServicePM = 6,
        [EnumText("更新服务商负责人")]
        UpdateServiceLinkMan = 7,
        [EnumText("驳回服务商经理")]
        RejectServicePM = 19,
        [EnumText("驳回服务商负责人")]
        RejectServiceLinkMan = 20,
        [EnumText("总批复构成更新")]
        UpdateConstitute = 9,
        [EnumText("工程要点更新")]
        UpdateMainPoints = 12,
        [EnumText("更新服务商")]
        UpdateService = 13,
        [EnumText("更新工期")]
        UpdateProjectLimit = 14,


        [EnumText("新建合同")]
        ContractAdd = 8,
        [EnumText("合同审批通过")]
        ContractAudit = 10,
        [EnumText("合同审批失败")]
        ContractReject = 11,

        [EnumText("新建变更")]
        ChangeAdd = 15,
        [EnumText("变更审批通过")]
        ChangeAudit = 17,
        [EnumText("变更审批失败")]
        ChangeReject = 18,

        [EnumText("新建签证")]
        VisaAdd = 22,
        [EnumText("签证审批通过")]
        VisaAudit = 24,
        [EnumText("签证审批失败")]
        VisaReject = 25,

        [EnumText("上传图纸")]
        DrawAdd = 29,
        [EnumText("图纸审批通过")]
        DrawAudit = 31,
        [EnumText("图纸审批失败")]
        DrawReject = 32,

        [EnumText("上传模型")]
        ModelAdd = 36,
        [EnumText("模型审批通过")]
        ModelAudit = 38,
        [EnumText("模型审批失败")]
        ModelReject = 39,

        [EnumText("提报施工计划")]
        PlanAdd = 43,
        [EnumText("计划审批通过")]
        PlanAudit = 44,
        [EnumText("计划审批失败")]
        PlanReject = 45,

        [EnumText("提报安全培训")]
        TrainAddAQ = 49,
        [EnumText("安全培训确认通过")]
        TrainAuidtAQ = 51,
        [EnumText("安全培训确认失败")]
        TrainRejectAQ = 52,

        [EnumText("提报质量培训")]
        TrainAddZL = 56,
        [EnumText("质量培训审批通过")]
        TrainAuditZL = 58,
        [EnumText("质量培训审批失败")]
        TrainRejectZL = 59,

        [EnumText("提报安全检查")]
        MonitorAddAQ = 63,
        [EnumText("安全检查审批通过")]
        MonitorAuditAQ = 65,
        [EnumText("提交整改")]
        SubmitRectification = 69,
        [EnumText("安全检查审批失败")]
        MonitorRejectAQ = 66,
        [EnumText("安全检查整改完成")]
        MonitorRectificationAQ = 68,

        [EnumText("提报质量检查")]
        MonitorAddZL = 71,
        [EnumText("质量检查审批通过")]
        MonitorAuditZL = 73,
        [EnumText("质量检查审批失败")]
        MonitorRejectZL = 74,
        [EnumText("质量检查整改完成")]
        MonitorRectificationZL = 76,

        [EnumText("提报危险作业")]
        WorkAdd = 79,
        [EnumText("危险作业审批通过")]
        WorkAudit = 81,
        [EnumText("危险作业审批失败")]
        WorkReject = 82,
        [EnumText("作业已完成")]
        WorkFinish = 83,
        [EnumText("上传实景审核通过")]
        UploadAudit = 84,
        [EnumText("上传实景审核失败")]
        UploadReject = 85,

        [EnumText("提交材料设备验收")]
        MaterialAdd = 86,
        [EnumText("材料设备验收确认通过")]
        MaterialAudit = 88,
        [EnumText("材料设备验收确认失败")]
        MaterialReject = 89,

        [EnumText("提交物料进场")]
        MaterielAdd = 93,
        [EnumText("物料进场确认通过")]
        MaterielAudit = 95,
        [EnumText("物料进场确认失败")]
        MaterielReject = 96,

        [EnumText("新建专项验收")]
        SpecialAdd = 100,
        [EnumText("专项验收审批通过")]
        SpecialAudit = 102,
        [EnumText("专项验收审批失败")]
        SpecialReject = 103,

        [EnumText("新建竣工验收")]
        ComplationAdd = 107,
        [EnumText("竣工验收审批通过")]
        ComplationAudit = 109,
        [EnumText("竣工验收审批失败")]
        ComplationReject = 110,

        [EnumText("提报监理日志")]
        SupervisorLogAdd = 114,
        [EnumText("驳回监理日志")]
        SupervisorLogReject = 115,
        [EnumText("审核监理日志")]
        SupervisorLogAudit = 116,

        [EnumText("提报沟通问题")]
        QuestionAdd = 117,
        [EnumText("问题关闭")]
        QuestionClose = 118,
        [EnumText("回复问题")]
        QuestionReply = 119,

        [EnumText("添加单位信息成功")]
        CompanyAdd = 120,
        [EnumText("修改单位信息成功")]
        CompanyUpdate = 121,

        [EnumText("添加用户信息成功")]
        UserAdd = 130,
        [EnumText("修改用户信息成功")]
        UserUpdate = 131,

        [EnumText("提报延期申请")]
        DelayApplyAdd = 141,
        [EnumText("延期申请审核通过")]
        DelayApplyAudit = 142,
        [EnumText("延期申请审核失败")]
        DelayApplyReject = 143,


        [EnumText("注册激活")]
        RegisterActive = 996,//31
        [EnumText("邮箱绑定")]
        CertificationValid = 997,//32
        [EnumText("找回密码")]
        FindPwd = 998,//33
        [EnumText("默认")]
        Default = 999
    }
    #endregion
    #endregion

    /// <summary>
    /// 系统所有配置项
    /// </summary>
    public enum Settings
    {
        /// <summary>
        /// 消息发送方
        /// </summary>
        [EnumText("消息发送方")]
        MsgSendUser,
        /// <summary>
        /// 邮箱认证有效时间(分钟)
        /// </summary>
        [EnumText("邮箱认证有效时间")]
        CertificationValidTime,
        /// <summary>
        /// 注册激活链接有效期(小时)
        /// </summary>
        [EnumText("注册激活链接有效期")]
        RegisterActiveUrlValidity,
        /// <summary>
        /// 找回密码链接有效期(分钟)
        /// </summary>
        [EnumText("找回密码链接有效期")]
        FindPwdUrlValidity,
        /// <summary>
        /// 网站地址
        /// </summary>
        [EnumText("网站地址")]
        WebUrl,
        /// <summary>
        /// 网站电话
        /// </summary>
        [EnumText("网站电话")]
        WebPhone,
        /// <summary>
        /// 一个手机号，一天最多能接收短信验证码次数
        /// </summary>
        [EnumText("一个手机号一天最多能接收短信验证码次数")]
        MaxMsgCodeNum,
        /// <summary>
        /// 一个Ip，一天最多请求接收短信验证码次数
        /// </summary>
        [EnumText("一个IP一天最多能接收短信验证码次数")]
        MaxIpNum,
        /// <summary>
        /// 是否短信通知注册审核结果
        /// </summary>
        [EnumText("否短信通知注册审核结果")]
        IsSendRegisterAuditResult,
        /// <summary>
        /// 注册短信验证码发送间隔时间（秒）
        /// </summary>
        [EnumText("注册短信验证码发送间隔时间")]
        SendRegisterCodeTime,
        /// <summary>
        /// 短信验证码有效时长（秒）
        /// </summary>
        [EnumText("短信验证码有效时长(秒)")]
        SMSCodeDuration,
        /// <summary>
        /// 邮件验证码有效时长（秒）
        /// </summary>
        [EnumText("邮件验证码有效时长(秒)")]
        EmailCodeDuration,
        /// <summary>
        /// 文件服务器地址
        /// </summary>
        [EnumText("文件服务器地址")]
        FileServerURL,
        /// <summary>
        /// 邮件验证链接入口
        /// </summary>
        [EnumText("邮件验证链接入口")]
        ValidateEmailLink,

        /// <summary>
        /// 视频接口 AppKey
        /// </summary>
        [EnumText("视频接口 AppKey")]
        HkAppKey,

        /// <summary>
        /// 视频接口 Secret 
        /// </summary>
        [EnumText("视频接口 Secret")]
        HkSecret,

        /// <summary>
        /// 视频接口 AccessToken
        /// </summary>
        [EnumText("视频接口 AccessToken")]
        HkAccessToken,

        /// <summary>
        /// 视频接口获取 AccessToken 接口地址
        /// </summary>
        [EnumText("视频接口获取 AccessToken 接口地址")]
        HkGetAccessTokenUrl,

        /// <summary>
        /// 视频接口开通设备直播功能接口地址
        /// </summary>
        [EnumText("视频接口开通设备直播功能接口地址")]
        HkOpenVideoUrl,

        /// <summary>
        /// 视频接口获取指定有效期直播地址的接口地址
        /// </summary>
        [EnumText("视频接口获取指定有效期直播地址的接口地址")]
        HkVideoUrl,

        /// <summary>
        /// 新增设备接口地址
        /// </summary>
        [EnumText("新增设备接口地址")]
        HkAddVideoUrl,

        /// <summary>
        /// 修改设备接口地址
        /// </summary>
        [EnumText("修改设备接口地址")]
        HkEditVideoUrl,

        /// <summary>
        /// 删除设备接口地址
        /// </summary>
        [EnumText("删除设备接口地址")]
        HkDeleteVideoUrl,


        /// <summary>
        /// 关闭设备视频加密接口地址
        /// </summary>
        [EnumText("关闭设备视频加密接口地址")]
        HkColseVidelEncryptUrl
    }
    /// <summary>
    /// 状态
    /// </summary>
    public enum EnumState
    {
        [EnumText("正常")]
        Normal = 1,
        [EnumText("关闭")]
        Close = 2,
        [EnumText("禁用")]
        Disable = 4,
        [EnumText("启用")]
        Enable = 5,
        [EnumText("未确认")]
        NoConfim = 6,
        [EnumText("已确认")]
        Confirmed = 7,
        [EnumText("未锁定")]
        NoLock = 8,
        [EnumText("已锁定")]
        Lock = 9,
        [EnumText("是")]
        Must = 10,
        [EnumText("否")]
        UnMust = 11,
    }
    /// <summary>
    /// 验证码使用状态
    /// </summary>
    public enum ValCodeState
    {
        [EnumText("未使用")]
        UNUse = 1,
        [EnumText("已使用")]
        Used = 2
    }
    /// <summary>
    /// 电子协议类型
    /// </summary>
    public enum ProtocolType
    {
        [EnumText("注册协议")]
        Register = 1,
    }
    /// <summary>
    /// 公告发表途径
    /// </summary>
    public enum WayOfRelease
    {
        [EnumText("PC")]
        PC = 1,
        [EnumText("APP")]
        APP = 2,
        [EnumText("PC和APP")]
        PCAndAPP = 3,
    }
    /// <summary>
    /// 合同类型
    /// </summary>
    public enum ContractType
    {
        [EnumText("单项合同")]
        Contract = 1,
        [EnumText("框架合同")]
        FrameContract = 2,
        [EnumText("委托书/订单")]
        Order = 3,
    }

    public enum PreProjectTypeState
    {
        [EnumText("加油站")]
        加油站,
        [EnumText("油库")]
        油库,
    }

    /// <summary>
    /// 三商类型
    /// </summary>
    public enum CompanyType
    {
        [EnumText("承包商")]
        SSCBS,
        [EnumText("供应商")]
        SSGYS,
        [EnumText("服务商")]
        SSFWS,
    }
    /// <summary>
    /// 甘特图自定义样式
    /// </summary>
    public enum GanttCustomerClass
    {
        /// <summary>
        /// 灰色：计划工期
        /// </summary>
        [EnumText("计划工期")]
        ganttGray,

        /// <summary>
        /// 橙色：变更工期
        /// </summary>
        [EnumText("变更工期")]
        ganttOrange,

        /// <summary>
        /// 绿色：提前完工
        /// </summary>
        [EnumText("提前完工")]
        ganttGreen,

        /// <summary>
        /// 蓝色：正常完工
        /// </summary>
        [EnumText("正常完工")]
        ganttBlue,

        /// <summary>
        /// 红色：延期完工
        /// </summary>
        [EnumText("延期完工")]
        ganttRed,
    }


    public enum EquipmentFileType
    {
        /// <summary>
        /// 外观照片
        /// </summary>
        [EnumText("外观照片")]
        WGZP,

        /// <summary>
        /// 铭牌照片
        /// </summary>
        [EnumText("铭牌照片")]
        MPZP,

        /// <summary>
        /// 线缆照片
        /// </summary>
        [EnumText("线缆照片")]
        XLZP,

        /// <summary>
        /// 接线盒照片
        /// </summary>
        [EnumText("接线盒照片")]
        JXHZP,

        /// <summary>
        /// 其他照片
        /// </summary>
        [EnumText("其他照片")]
        QTZP,
    }
    /// <summary>
    /// 人脸操作
    /// </summary>
    public enum FaceOperate
    {
        Add = 1,

        Update = 2,

        Search = 3,

        Delete = 4
    }

    /// <summary>
    /// 考勤签到结果
    /// </summary>
    public enum SignRes
    {
        [EnumText("未识别到人脸")]
        NoFace = 0,
        [EnumText("没有网络")]
        NoNetwork = 1,
        [EnumText("签到失败")]
        Fail = 2,
        [EnumText("签到成功")]
        Success = 3,
        [EnumText("其他")]
        Other = 4
    }

    /// <summary>
    /// 编码映射类型
    /// </summary>
    public enum CodeMapType
    {
        /// <summary>
        /// 人员账号
        /// </summary>
        [EnumText("人员账号")]
        Account = 0,

        /// <summary>
        /// 组织机构
        /// </summary>
        [EnumText("组织机构")]
        Org = 1,

        /// <summary>
        /// 加油站
        /// </summary>
        [EnumText("加油站")]
        OilStation = 2
    }

    /// <summary>
    /// 系统映射关系
    /// </summary>
    public enum SysMapType
    {
        /// <summary>
        /// 投资和BIM
        /// </summary>
        BimToTz,

        /// <summary>
        /// BIM 和协同
        /// </summary>
        BimToXt,

        /// <summary>
        /// 投资和BIM
        /// </summary>
        TzToBim,

        /// <summary>
        /// 投资和协同
        /// </summary>
        TzToXt,

    }

    /// <summary>
    /// 协同相关业务数据状态(试运行申请、开工申请、工程设计变更)
    /// </summary>
    public enum XtBusinessDataState
    {
        /// <summary>
        /// 暂存
        /// </summary>
        [EnumText("暂存")]
        Staged = 0,

        /// <summary>
        /// 审核中
        /// </summary>
        [EnumText("审核中")]
        Auditing = 10,

        /// <summary>
        /// 审核通过
        /// </summary>
        [EnumText("审核通过")]
        Pass = 20,

        /// <summary>
        /// 审核不通过
        /// </summary>
        [EnumText("审核不通过")]
        NoPass = 30
    }


    /// <summary>
    /// 协同审批工作流编码
    /// </summary>
    //public enum XtWorkFlowCode
    //{
    //    /// <summary>
    //    /// 试运行申请：2927
    //    /// </summary>
    //    WfSyxsq = 3091,

    //    /// <summary>
    //    /// 防渗改造进度填报：2547
    //    /// </summary>
    //    WfFsgzjdtb = 2547,

    //    /// <summary>
    //    /// 工程建设项目开工报告审批流程：2446
    //    /// </summary>
    //    Wfjsxmkgbg = 3044,//2446,

    //    /// <summary>
    //    /// 工程甲供物资订单审批流程：2464
    //    /// </summary>
    //    WfWzddsp = 2464,

    //    /// <summary>
    //    /// 建设工程设计变更申请流程：2504
    //    /// </summary>
    //    WfGcsjbg = 2504,

    //    /// <summary>
    //    /// 建设工程项目管理人员变更申请流程：2505
    //    /// </summary>
    //    WfGcglrybg = 2505,

    //    /// <summary>
    //    /// 陕西省各竞争对手加油（气）站现状上报流程：2624
    //    /// </summary>
    //    Wfjzdsjyz = 2624,

    //    /// <summary>
    //    /// 陕西省意向开发加油（气）站上报流程：2625
    //    /// </summary>
    //    WfYxkfjyz = 2625,
    //    /// <summary>
    //    /// 设计方案提交流程
    //    /// </summary>
    //    WfSjfa = 2689,

    //    /// <summary>
    //    /// 项目提出提交流程
    //    /// </summary>
    //    WfXmtcsq = 3031,//3167,

    //    /// <summary>
    //    /// 项目谈判提交流程
    //    /// </summary>
    //    WfXmtpsq = 2630,

    //    /// <summary>
    //    /// 土地出让协议谈判提交流程
    //    /// </summary>
    //    WfTdcrxitpsq = 3049,

    //    /// <summary>
    //    /// 现场踏勘提交流程
    //    /// </summary>
    //    WfXctksq = 3047,

    //    /// <summary>
    //    /// 招标申请提交流程
    //    /// </summary>
    //    WfZbsq = 3040,//2633,

    //    /// <summary>
    //    /// 招标申请结果提交流程
    //    /// </summary>
    //    WfZbjgsq = 3036,//2634,
    //    /// <summary>
    //    /// 上会材料上报
    //    /// </summary>
    //    WfShclsb = 3045,
    //    /// <summary>
    //    /// 评审材料上报
    //    /// </summary>
    //    WfPsclsb = 3041,
    //    /// <summary>
    //    /// 甲供物资申请
    //    /// </summary>
    //    WfJgwzsq = 3046,
    //    // <summary>
    //    /// 项目评审
    //    /// </summary>
    //    Wfxmps = 3050,
    //    // <summary>
    //    /// 项目批复
    //    /// </summary>
    //    Wfxmpf = 3088,
    //    // <summary>
    //    /// 施工图纸
    //    /// </summary>
    //    Wfsgtu = 3053,
    //    // <summary>
    //    /// 开工申请
    //    /// </summary>
    //    Wfkgsq= 3090,
    //    // <summary>
    //    /// 竣工验收申请
    //    /// </summary>
    //    Wfjgyssq = 3090,
    //}
    // <summary>
    // 协同审批工作流编码正式环境
    // </summary>
    public enum XtWorkFlowCode
    {
        /// <summary>
        /// 试运行申请：2927
        /// </summary>
        WfSyxsq = 2927,

        /// <summary>
        /// 防渗改造进度填报：2547
        /// </summary>
        WfFsgzjdtb = 2547,

        /// <summary>
        /// 工程建设项目开工报告审批流程：2446
        /// </summary>
        Wfjsxmkgbg = 2446,//2446,

        /// <summary>
        /// 工程甲供物资订单审批流程：2464
        /// </summary>
        WfWzddsp = 2464,

        /// <summary>
        /// 建设工程设计变更申请流程：2504
        /// </summary>
        WfGcsjbg = 2504,

        /// <summary>
        /// 建设工程项目管理人员变更申请流程：2505
        /// </summary>
        WfGcglrybg = 2505,

        /// <summary>
        /// 陕西省各竞争对手加油（气）站现状上报流程：2624
        /// </summary>
        Wfjzdsjyz = 2624,

        /// <summary>
        /// 陕西省意向开发加油（气）站上报流程：2625
        /// </summary>
        WfYxkfjyz = 2625,
        /// <summary>
        /// 设计方案提交流程
        /// </summary>
        WfSjfa = 3409,

        /// <summary>
        /// 项目提出提交流程
        /// </summary>
        WfXmtcsq = 3167,

        /// <summary>
        /// 项目谈判提交流程
        /// </summary>
        WfXmtpsq = 2630,

        /// <summary>
        /// 土地出让协议谈判提交流程
        /// </summary>
        WfTdcrxitpsq = 3049,

        /// <summary>
        /// 现场踏勘提交流程
        /// </summary>
        WfXctksq = 3047,

        /// <summary>
        /// 招标申请提交流程
        /// </summary>
        WfZbsq = 3212,//2633,

        /// <summary>
        /// 招标申请结果提交流程
        /// </summary>
        WfZbjgsq = 3241,//2634,
        /// <summary>
        /// 上会材料上报
        /// </summary>
        WfShclsb = 3211,
        /// <summary>
        /// 评审材料上报
        /// </summary>
        WfPsclsb = 3210,
        /// <summary>
        /// 甲供物资申请
        /// </summary>
        WfJgwzsq = 3548,//3229,
        // <summary>
        /// 项目评审
        /// </summary>
        Wfxmps = 3407,
        // <summary>
        /// 项目批复
        /// </summary>
        Wfxmpf = 3408,
        // <summary>
        /// 施工图纸
        /// </summary>
        Wfsgtu = 3410,
        // <summary>
        /// 开工申请
        /// </summary>
        Wfkgsq = 3230,
        // <summary>
        /// 竣工验收申请
        /// </summary>
        Wfjgyssq = 3411,
    }
    /// <summary>
    /// 多页面附件
    /// </summary>
    public enum InvestmentEnclosure
    {
        /// <summary>
        /// 当前表
        /// </summary>
        [EnumText("当前表")]
        itself = 1,
        /// <summary>
        /// 上会材料
        /// </summary>
        [EnumText("上会材料上报")]
        ConferenceMaterials = 2,

    }

    public enum RadioEnum
    {
        /// <summary>
        /// 当前表
        /// </summary>
        [EnumText("当前表")]
        itself = 1,
        /// <summary>
        /// 上会材料
        /// </summary>
        [EnumText("上会材料上报")]
        ConferenceMaterials = 2,

    }
}