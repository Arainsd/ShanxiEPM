using hc.Plat.Common.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Common
{
    #region 用户身份
    /// <summary>
    ///用户身份
    /// </summary>
    public enum RoleType
    {
        [EnumText("后台管理员")]
        Admin = 1,
        [EnumText("业主")]
        Owner = 2,
        [EnumText("供应商")]//在改造前服务商就是供应商
        Supplier = 7,

        //[EnumText("服务商")]
        //ServiceProvider= 8,
        //[EnumText("服务商")]
        //Contractor = 9,
    }
    #endregion
    #region 
    public enum RoleTypes
    {
        [EnumText("业主")]
        Owner = 2,
        [EnumText("供应商")]
        Supplier = 7,
    }
    #endregion
    #region 后台枚举
    /// <summary>
    /// 后台管理一级导航菜单
    /// </summary>
    public enum AdminNav
    {
        [EnumText("系统管理")]
        SystemManage = 1,
        [EnumText("消息管理")]
        MsgManage = 2,
        [EnumText("用户权限")]
        UserFuncManage = 3,
    }
    /// <summary>
    /// 后台管理二级模块菜单
    /// </summary>
    public enum AdminCategory
    {
        [EnumText("里程碑管理")]
        MilepostManage = 1,
        [EnumText("项目资料管理")]
        ProjectFileManage = 2,
        [EnumText("加油站管理")]
        OilStationManage = 21,
        [EnumText("新闻资讯管理")]
        NewsManage = 15,
        [EnumText("广告管理")]
        AdManage = 16,
        [EnumText("模板管理")]
        TemplateManage = 17,
        [EnumText("公告管理")]
        NoticeManage = 18,
        [EnumText("数据字典")]
        DataDictionaryManage = 3,
        [EnumText("省市地区")]
        RegionManage = 4,
        [EnumText("系统参数")]
        SystemParameter = 8,
        [EnumText("电子协议")]
        ElectronicAgreementManage = 9,
        [EnumText("检查项管理")]
        CheckItemManage = 88,
        [EnumText("批复构成管理")]
        ReplyConstituteManage = 99,
        [EnumText("工程服务商设置")]
        ISPManage = 19,
        [EnumText("工程内容设置")]
        ContentManage = 20,
        [EnumText("视频监控管理")]
        VideoManage = 100,

        [EnumText("消息设置")]
        ManageSet = 10,
        [EnumText("邮件")]
        Email = 11,
        [EnumText("短信管理")]
        SMSManage = 13,
        [EnumText("站内消息")]
        Message = 14,

        [EnumText("权限管理")]
        FuncManage = 5,
        [EnumText("角色管理")]
        RoleManage = 6,
        [EnumText("组织结构")]
        Organization = 61,
        [EnumText("用户管理")]
        UserManage = 7,

        [EnumText("考勤设置")]
        ProjectAttendance = 200,
    }
    /// <summary>
    /// 后台管理三级功能菜单
    /// </summary>
    public enum AdminModule
    {
        [EnumText("里程碑管理")]
        MilepostConfig = 1,

        [EnumText("检查项管理")]
        CheckItem = 88,

        [EnumText("批复构成管理")]
        ReplyConstitute = 99,

        [EnumText("工程服务商设置")]
        ISPSet = 61,

        [EnumText("工程内容设置")]
        ContentSet = 62,

        [EnumText("项目资料管理")]
        ProjectFileConfig = 2,

        [EnumText("加油站管理")]
        OilStation = 211,

        [EnumText("新闻分类管理")]
        NewsTarget = 31,
        [EnumText("新闻发布记录")]
        News = 32,

        [EnumText("广告位管理")]
        AdTarget = 33,
        [EnumText("广告投放记录")]
        Ad = 34,

        [EnumText("安全检查模板")]
        AQTemplate = 35,
        [EnumText("质量检查模板")]
        ZLTemplate = 36,
        [EnumText("专项验收模板")]
        ZXTemplate = 37,
        [EnumText("安全培训模板")]
        AQTrainTemplate = 38,
        [EnumText("质量培训模板")]
        ZLTrainTemplate = 39,
        [EnumText("检查整改记录")]
        MonitorRectifRecord = 41,

        [EnumText("公告发布记录")]
        NoticeManage = 40,

        [EnumText("数据字典")]
        DataDictionary = 3,

        [EnumText("省市地区")]
        RegionConfig = 4,

        [EnumText("系统配置")]
        SystemSetting = 10,
        [EnumText("系统参数")]
        SystemParameter = 11,
        [EnumText("系统日志")]
        SystemLog = 12,
        [EnumText("视频监控")]
        VideoManager = 102,
        [EnumText("电子协议")]
        ElectronicAgreement = 13,


        [EnumText("消息环节")]
        MessageSection = 14,
        [EnumText("消息发送策略")]
        MessageStrategy = 15,

        [EnumText("邮件模板")]
        EmailTemplete = 16,
        [EnumText("邮件接口设置")]
        EmailSetting = 17,
        [EnumText("邮件发送记录")]
        EmailHistory = 18,
        [EnumText("邮件记录")]
        EmailList = 19,

        [EnumText("短信平台")]
        SMSPlatform = 20,
        [EnumText("短信模板")]
        SMSTemplete = 21,
        [EnumText("短信记录")]
        SMSHistory = 22,
        [EnumText("短信接口设置")]
        SMSSetting = 23,

        [EnumText("站内消息模板")]
        MessageTemplete = 24,
        [EnumText("站内消息记录")]
        MessageList = 25,


        [EnumText("普通模板")]
        DocTemplete = 26,
        [EnumText("数据类型")]
        TypeDictionary = 27,
        [EnumText("部门管理")]
        Dep = 28,
        [EnumText("用户管理")]
        UserManager = 29,
        [EnumText("用户检索")]
        UserRetrieve = 30,


        [EnumText("管理员权限")]
        AdminRight = 50,
        [EnumText("业主权限")]
        OwnerRight = 51,
        [EnumText("角色管理")]
        AdminRole = 60,
        [EnumText("组织结构")]
        AdminOrganization = 66,
        [EnumText("用户管理")]
        AdminUserManager = 70,
        [EnumText("密码管理")]
        PasswordManager = 8,
        [EnumText("修改资料")]
        EditUserInfo = 9,
        [EnumText("项目KPI")]
        ProjectKPI = 97,
        [EnumText("人脸识别")]
        AIUserFace = 98,
        [EnumText("人脸库")]
        AIUserFaceManager = 120,
        [EnumText("考勤管理")]
        SignManager = 121,
        [EnumText("操作日志")]
        LogManager = 122,
        [EnumText("项目试运行申请")]
        ProjectApproval = 123,
        [EnumText("防渗改造")]
        ReformRecord = 124,

        [EnumText("考勤设置")]
        ProjectAttendance = 200,
        

    }
    #endregion

    #region 前台枚举
    /// <summary>
    /// 前台管理一级导航菜单
    /// </summary>
    public enum WebNav
    {
        [EnumText("首页")]
        LoginHome = 1,
        [EnumText("投资管理")]
        InvestManage = 2,
        [EnumText("工程管理")]
        EngineeringManage = 3,
        [EnumText("其他管理")]
        OtherManage = 4,
        [EnumText("统计报表")]
        StatisticalReport = 5,
        [EnumText("个人中心")]
        PersonalCenter = 6,
    }
    /// <summary>
    /// 前台管理二级功能菜单
    /// </summary>
    public enum WebCategory
    {
        #region 投资管理
        [EnumText("项目提出")]
        TzProjectProposal = 1,
        [EnumText("现场踏勘")]
        TzResearch = 2,
        [EnumText("项目谈判")]
        TzFirstNegotiation = 3,
        [EnumText("土地出让协议谈判")]
        TzLandNegotiation = 4,
        [EnumText("评审材料上报")]
        TzFormTalkFile = 5,
        [EnumText("项目评审")]
        TzProjectReveiews = 6,
        [EnumText("会议决策")]
        TzMeetingFileReport = 7,
        [EnumText("项目批复")]
        TzProjectApprovalInfo = 8,
        [EnumText("库站改造")]
        ReformRecord = 26,
        #endregion

        #region 工程管理
        [EnumText("设计方案")]
        DesignScheme = 9,
        [EnumText("施工图纸会审")]
        ConstructionDrawings = 10,
        [EnumText("招标管理")]
        TenderManage = 11,
        [EnumText("甲供物资采购申请")]
        GoodsApply = 12,
        [EnumText("开工申请")]
        TzStartsApply = 13,
        [EnumText("施工管理")]
        ConstructionManage = 14,
        [EnumText("竣工管理")]
        CompletionAcceptanceApply = 15,
        [EnumText("试运行申请")]
        ProjectOperateApply = 16,

        [EnumText("财务决算")]
        FinanceAccount = 29,
        
        #endregion

        #region 其他管理
        [EnumText("安全人才库")]
        UserManage = 17,
        [EnumText("图纸管理")]
        Draw = 18,
        [EnumText("质量培训")]
        QualityCheck = 19,
        [EnumText("三商管理")]
        ThreeQuotientManage = 20,
        [EnumText("甲供物资管理")]
        GoodsManage = 24,
        [EnumText("视频管理")]
        VideoManage = 30,
        #endregion

        #region 统计报表
        [EnumText("监理日志报表")]
        LogStatistic = 21,
        [EnumText("项目完成度统计")]
        ProjectCompletion = 22,
        [EnumText("考勤统计")]
        SignManage = 23,

        [EnumText("项目进度报表")]
        TzProProgressChart = 25,

        [EnumText("周报管理")]
        WeeklyManage = 28,

        [EnumText("甲供物资报表")]
        SupplyMaterialReport = 40,

        [EnumText("考勤报表")]
        Attendancereport = 41,

        #endregion
    }

    /// <summary>
    /// 前台管理三级功能菜单
    /// </summary>
    public enum WebModule
    {
        [EnumText("供应商管理")]
        SupplierManage = 1,
        [EnumText("承包商管理")]
        ContractorManage = 2,
        [EnumText("服务商管理")]
        ServiceManage = 3,
        [EnumText("项目信息")]
        ProjectInfoManage = 4,
        [EnumText("施工计划")]
        Plan = 5,
        [EnumText("进度跟踪")]
        ProgressManage = 6,
        [EnumText("工器具机械验收")]
        Material = 7,
        [EnumText("材料设备验收")]
        Materiel = 8,
        [EnumText("危险作业报备")]
        DangerousWork = 9,
        [EnumText("监理日志")]
        LogManage = 10,
        [EnumText("现场检查")]
        InspectionManage = 11,
        [EnumText("问题沟通")]
        ProblemManage = 12,
        [EnumText("变更计划")]
        DelayApply = 13,
        [EnumText("人员变更申请")]
        PersonnelChangeApply = 14,
        [EnumText("设计方案变更申请")]
        DesignSchemeChangeApply = 15,
        [EnumText("工程变更")]
        EngineeringChange = 16,
        [EnumText("签证管理")]
        Visa = 17,
        [EnumText("招标申请")]
        TenderingApply = 18,
        [EnumText("招标结果")]
        TenderResult = 19,
        [EnumText("招标统计")]
        TenderingCount = 20,

        [EnumText("竣工验收申请")]
        CompletionApply = 21,
        [EnumText("竣工验收结果上传")]
        CompletionAcceptance = 22,


        [EnumText("分公司统计")]
        CompanyAttendanceCount = 23,
        [EnumText("项目统计")]
        ProjectAttendanceCount = 24,
        [EnumText("人员统计")]
        UserAttendanceCount = 25,
    }
    #endregion

    #region 操作权限
    /// <summary>
    /// 系统权限，关系为一个模块对应若干权限
    /// </summary>
    public enum SystemRight
    {
        [EnumText("添加")]
        Add = 1,
        [EnumText("修改")]
        Modify = 2,
        [EnumText("删除")]
        Delete = 3,
        [EnumText("审核")]
        Check = 4,
        [EnumText("驳回")]
        UnCheck = 5,
        [EnumText("导出")]
        Export = 6,
        [EnumText("导入")]
        Import = 7,
        [EnumText("浏览")]
        Browse = 10,
        [EnumText("详情")]
        Info = 11,
        [EnumText("解除锁定")]
        UnLock = 12,
        [EnumText("作废")]
        Invalid = 14,
        [EnumText("设置权限")]
        SetRight = 16,
        [EnumText("启用/禁用")]
        Enable = 17,
        [EnumText("下载")]
        DownLoadFiles = 20,
        [EnumText("分配角色")]
        SetRole = 21,
        [EnumText("锁定")]
        Lock = 22,
        [EnumText("整改")]
        Rectif = 25,
        [EnumText("关闭")]
        Close = 26,
        [EnumText("项目结项")]
        Finish = 27,
        [EnumText("项目终结")]
        EndUp = 28,
        [EnumText("设置服务商")]
        SetCustomer = 29,
        [EnumText("设置服务商用户")]
        SetCustomerUser = 30,
        [EnumText("审核服务商用户")]
        AuditCustomerUser = 34,
        [EnumText("驳回服务商用户")]
        RejectCustomerUser = 35,
        [EnumText("生成里程碑计划")]
        AddPlan = 32,
        [EnumText("审核里程碑计划")]
        AuditPlan = 31,
        [EnumText("驳回里程碑计划")]
        RejectPlan = 36,
        [EnumText("问题回复")]
        Reply = 33,
        [EnumText("确认整改")]
        AuditRectif = 37,
        [EnumText("驳回整改")]
        RejectRectif = 38,
        [EnumText("计划关联模型")]
        BindComponent = 39,
        [EnumText("设置总批复构成")]
        SetConstitute = 40,
        [EnumText("设置工程要点")]
        SetMainPoints = 41,
        [EnumText("设置工期")]
        SetProjectLimit = 42,
        [EnumText("设置项目基本信息")]
        SaveBaseProject = 43,
        [EnumText("上传作业实景")]
        UploadWork = 44,
        [EnumText("提报整改结果")]
        UploadSecurityCheck = 45,
        [EnumText("保存项目资料")]
        SaveProjectData = 46,
        [EnumText("签到")]
        FaceAI = 47,
    }
    #endregion

    #region 导航类型
    /// <summary>
    /// 功能类型
    /// </summary>
    public enum FunctionType
    {
        [EnumText("导航")]
        Nav = 1,
        [EnumText("分类")]
        Category = 2,
        [EnumText("模块")]
        Module = 3,
        [EnumText("操作")]
        Action = 4
    }
    /// <summary>
    /// 前端功能类型
    /// </summary>
    public enum WebFunctionType
    {
        [EnumText("导航")]
        Nav = 1,
        [EnumText("分类")]
        Category = 2,
        [EnumText("模块")]
        Module = 3,
        [EnumText("操作")]
        Action = 4
    }
    #endregion

    #region 权限枚举
    public enum Role
    {
        [EnumText("后台管理员")]
        Admin = 1,

        //省公司分：
        [EnumText("省公司领导")]
        LDBZ = 2,
        [EnumText("工程处项目经理")]
        GCCXMJL = 4,

        //分公司分：
        [EnumText("分公司领导")]
        FGJL = 5,
        [EnumText("分公司部门主任")]
        FGBMZR = 7,
        [EnumText("分公司现场经理")]
        FGXMFZR = 8,

        //服务商
        [EnumText("供应商")]
        SGDW = 9,

        /// <summary>
        /// 站经理
        /// </summary>
        [EnumText("站经理")]
        ZJL = 10,
    }

    public enum RoleTypeEnum
    {
        [EnumText("省公司")]
        SGS = 1,
        [EnumText("分公司")]
        FGS = 2,
        [EnumText("站经理")]
        ZJL = 3,
        [EnumText("监理")]
        JL = 4
    }

    public enum RectificationPeople
    {
        [EnumText("分公司现场经理")]
        FGSXMFZR = 1,

        [EnumText("设计负责人")]
        SJFZR = 2,

        [EnumText("总监")]
        ZJ = 3,
        [EnumText("现场监理")]
        XCJL = 4,

        [EnumText("土建单位负责人")]
        TJZFZR = 5,
        [EnumText("土建本省负责人")]
        TJAQFZR = 6,
        [EnumText("土建现场负责人")]
        TJXCFZR = 8,

        [EnumText("安装单位负责人")]
        AZZFZR = 9,
        [EnumText("安装本省负责人")]
        AZAQFZR = 10,
        [EnumText("安装现场负责人")]
        AZXCFZR = 12,

        [EnumText("包装单位负责人")]
        BZZFZR = 13,
        [EnumText("包装本省负责人")]
        BZAQFZR = 14,
        [EnumText("包装现场负责人")]
        BZXCFZR = 16,

        [EnumText("加固单位负责人")]
        JGZFZR = 17,
        [EnumText("加固本省负责人")]
        JGAQFZR = 18,
        [EnumText("加固现场负责人")]
        JGXCFZR = 20,

        [EnumText("内衬单位负责人")]
        NCZFZR = 21,
        [EnumText("内衬本省负责人")]
        NCAQFZR = 22,
        [EnumText("内衬现场负责人")]
        NCXCFZR = 24,

        [EnumText("清罐单位负责人人")]
        QGZFZR = 25,
        [EnumText("清罐本省负责人")]
        QGAQFZR = 26,
        [EnumText("清罐现场负责人")]
        QGXCFZR = 28,

        [EnumText("网架单位负责人")]
        WJZFZR = 51,
        [EnumText("网架本省负责人")]
        WJAQFZR = 52,
        [EnumText("网架现场负责人")]
        WJXCFZR = 53,

        [EnumText("油罐单位负责人")]
        YGZFZR = 55,
        [EnumText("油罐本省负责人")]
        YGAQFZR = 56,
        [EnumText("油罐现场负责人")]
        YGXCFZR = 57,

        [EnumText("管线单位负责人")]
        GXZFZR = 59,
        [EnumText("管线本省负责人")]
        GXAQFZR = 60,
        [EnumText("管线现场负责人")]
        GXXCFZR = 61,

        [EnumText("发电机单位负责人")]
        FDJZFZR = 63,
        [EnumText("发电机本省负责人")]
        FDJAQFZR = 64,
        [EnumText("发电机现场负责人")]
        FDJXCFZR = 65,

        [EnumText("液位仪单位负责人")]
        YWYZFZR = 67,
        [EnumText("液位仪本省负责人")]
        YWYAQFZR = 68,
        [EnumText("液位仪现场负责人")]
        YWYXCFZR = 69,

        [EnumText("加油机单位负责人")]
        JYJZFZR = 37,
        [EnumText("加油机本省负责人")]
        JYJAQFZR = 38,
        [EnumText("加油机现场负责人")]
        JYJXCFZR = 39,

        [EnumText("配电柜单位负责人")]
        PDGZFZR = 71,
        [EnumText("配电柜本省负责人")]
        PDGAQFZR = 72,
        [EnumText("配电柜现场负责人")]
        PGDXCFZR = 73,

        [EnumText("成品井单位负责人")]
        CPJZFZR = 40,
        [EnumText("成品井本省负责人")]
        CPJAQFZR = 41,
        [EnumText("成品井现场负责人")]
        CPJXCFZR = 42,

        [EnumText("站经理")]
        ZJL = 99,
    }

    public enum RectificationCompany
    {
        [EnumText("分公司")]
        FGS = 1,
        [EnumText("监理单位")]
        JLDW = 2,
        [EnumText("设计单位")]
        SJDW = 3,
        [EnumText("土建单位")]
        TJDW = 4,
        [EnumText("安装单位")]
        AZDW = 5,
        [EnumText("包装单位")]
        BZDW = 6,
        [EnumText("加固单位")]
        JGDW = 7,
        [EnumText("内衬单位")]
        NCDW = 8,
        [EnumText("油罐清洗单位")]
        QGDW = 9,

        [EnumText("网架单位")]
        WJDW = 15,
        [EnumText("油罐单位")]
        YGDW = 16,
        [EnumText("管线单位")]
        GXDW = 17,
        [EnumText("发电机单位")]
        FDJDW = 18,
        [EnumText("液位仪单位")]
        YWYDW = 19,
        [EnumText("加油机单位")]
        JYJDW = 14,
        [EnumText("配电柜单位")]
        PDGDW = 20,
        [EnumText("成品井单位")]
        CPJDW = 13,

        [EnumText("站经理")]
        ZJL = 12,
    }
    public enum ScoreRange
    {
        [EnumText("10分")]
        十 = 1,
        [EnumText("8分")]
        八 = 2,
        [EnumText("5分")]
        五 = 3,
        [EnumText("3分")]
        三 = 4,
        [EnumText("0分")]
        零 = 5,
    }
    #endregion
}