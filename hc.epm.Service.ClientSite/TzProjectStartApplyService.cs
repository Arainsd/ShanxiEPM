using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Service.ClientSite
{
    /// <summary>
    /// 开工申请
    /// </summary>
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzProjectStartApply(Epm_TzProjectStartApply model)
        {
            Result<int> result = new Result<int>();
            try
            {
                SetCurrentUser(model);
                SetCreateUser(model);
                //项目提出信息
                var tzProjectProposal = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId.Value);
                //项目批复信息
                var tzProjectApprovalInfo = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().GetList(t => t.ProjectId == model.ProjectId.Value).FirstOrDefault();

                model.ProjectCode = tzProjectProposal.ProjectCode;
                model.ApprovalNo = tzProjectApprovalInfo.ApprovalNo;
                model.Nature = tzProjectProposal.Nature;
                model.NatureName = tzProjectProposal.NatureName;
                model.ApplyTime = tzProjectProposal.ApplyTime;
                model.StationId = tzProjectProposal.StationId;
                model.StationCodeXt = tzProjectProposal.StationCodeXt;
                model.StationName = tzProjectProposal.StationName;
                model.CompanyId = tzProjectProposal.CompanyId;
                model.CompanyCodeXt = tzProjectProposal.CompanyCodeXt;
                model.CompanyName = tzProjectProposal.CompanyName;
                model.ProjectName = tzProjectProposal.ProjectName;
                model.StartApplyTime = DateTime.Now;
                model.ApplyTitle = model.ApplyTitle;
                model.CameraSerialNumber = model.CameraSerialNumber;
                model.VerificationCode = model.VerificationCode;
                model.UrlAddress = model.UrlAddress;
                AddFilesBytzTable(model, model.TzAttachs);

                #region 协同开工申请  不要删哦
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)XtBusinessDataState.Auditing && XtWorkFlow == "1")
                {
                    XtTzProjectStartApplyView view = new XtTzProjectStartApplyView();

                    view.ProjectName = model.ProjectName;
                    view.ApplyTitle = model.ApplyTitle;
                    view.ApplyUserName = model.ApplyUserName;
                    view.CreateTime = model.CreateTime.ToString();
                    view.ApplyDepartment = model.ApplyDepartment;
                    view.ApplyTel = model.ApplyTel;
                    view.DesignScale = model.DesignScale;
                    view.BuildNumber = model.BuildNumber;
                    view.InvestmentEstimateAmount = model.InvestmentEstimateAmount.ToString();
                    view.FeasibilityReport = model.FeasibilityReport;
                    view.ApprovalNo = model.ReplyNumber;
                    view.ReplyInvestmentAmount = model.ReplyInvestmentAmount.ToString();
                    view.FundsSource = model.FundsSource;
                    view.CurrentPlanned = model.CurrentPlanned;
                    view.PlanStartTime = string.Format("{0:yyyy-MM-dd}", model.PlanStartTime);
                    view.PlanEndTime = string.Format("{0:yyyy-MM-dd}", model.PlanEndTime);
                    view.BuildCycle = model.BuildCycle;
                    view.ProjectSummary = model.ProjectSummary;
                    view.ProjectManagement = model.ProjectManagement;
                    view.BuildDeploy = model.BuildDeploy;
                    view.DesignUnits = model.DesignUnits;
                    view.ConstructionName = model.ConstructionName;
                    view.ProjectManager = model.ProjectManager;
                    view.ConstructionScore = model.ConstructionScore;
                    view.ConstructionSituation = model.ConstructionSituation;
                    view.SupervisionUnit = model.SupervisionUnit;
                    view.SupervisionEngineer = model.SupervisionEngineer;
                    view.SupervisionScore = model.SupervisionScore;
                    view.SupervisionSituation = model.SupervisionSituation;
                    view.ConstructionReady = model.ConstructionReady;
                    view.MainEquipment = model.MainEquipment;
                    view.Environment = model.Environment;
                    view.LeadershipName = model.LeadershipName;
                    view.gcxxTemp_TzAttachs = model.ProjectPlan;//工程形象
                    view.Temp_TzAttachs = "";

                    var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.CreateUserId);
                    if (baseUser == null)
                    {
                        throw new Exception("未找到申请人相关信息！");
                    }
                    else
                    {
                        view.hr_sqr = baseUser.ObjeId;
                    }
                    #region 附件
                    //附件
                    if (model.TzAttachs != null && model.TzAttachs.Any())
                    {
                        var xmglTemp = model.TzAttachs.Where(p => p.TypeNo == "XMGLJGWJ").ToList();
                        view.xmglTemp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(xmglTemp);

                        var lxpfTemp = model.TzAttachs.Where(p => p.TypeNo == "LXPFWJ").ToList();
                        view.lxpfTemp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(lxpfTemp);

                        var jspTemp = model.TzAttachs.Where(p => p.TypeNo == "SGSJWJ").ToList();
                        view.jspTemp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(jspTemp);

                        var sgjcTemp = model.TzAttachs.Where(p => p.TypeNo == "AQJYZWJ").ToList();
                        view.sgjcTemp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(sgjcTemp);

                        var fgsyTemp = model.TzAttachs.Where(p => p.TypeNo == "GYSWJ").ToList();
                        view.fgsyTemp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(fgsyTemp);

                        var gcjsxmTemp = model.TzAttachs.Where(p => p.TypeNo == "HSEWJ").ToList();
                        view.gcjsxmTemp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(gcjsxmTemp);

                        //string baseFaleUrl = System.Configuration.ConfigurationManager.AppSettings.Get("XtDownloadUrl");
                        //foreach (var item in model.TzAttachs)
                        //{
                        //    string fileUrl = string.Format("{0}?fileId={1}&type={2}", baseFaleUrl, item.Id, item.TypeNo);
                        //    switch (item.TypeNo)
                        //    {
                        //        case "XMGLJGWJ"://项目管理机构（项目经理部或油库项目组）设立的文件、机构组成和职责分工各一份
                        //            view.xmglTemp_TzAttachs = fileUrl + '|' + view.xmglTemp_TzAttachs; ;
                        //            break;
                        //        case "LXPFWJ"://立项批复或项目初步设计批复文件复印件一份
                        //            view.lxpfTemp_TzAttachs = fileUrl + '|' + view.lxpfTemp_TzAttachs; ;
                        //            break;
                        //        case "SGSJWJ"://经审批的施工组织设计或工程建设总体部署一份
                        //            view.jspTemp_TzAttachs = fileUrl + '|' + view.jspTemp_TzAttachs; ;
                        //            break;
                        //        case "AQJYZWJ"://施工进场人员名单及《安全教育合格证》（复印件）
                        //            view.sgjcTemp_TzAttachs = fileUrl + '|' + view.sgjcTemp_TzAttachs; ;
                        //            break;
                        //        case "GYSWJ"://分公司与供应厂商确定的主要设备材料交付时间表一份
                        //            view.fgsyTemp_TzAttachs = fileUrl + '|' + view.fgsyTemp_TzAttachs; ;
                        //            break;
                        //        case "HSEWJ"://工程建设项目，海英提供审查通过后的HSE作业指导书、HSE作业计划书和HS场检查表E现
                        //            view.gcjsxmTemp_TzAttachs = fileUrl + '|' + view.gcjsxmTemp_TzAttachs; ;
                        //            break;
                        //        default:
                        //            break;
                        //    }
                        //}
                        //if (view.xmglTemp_TzAttachs != null)
                        //{
                        //    view.xmglTemp_TzAttachs = view.xmglTemp_TzAttachs.Substring(view.xmglTemp_TzAttachs.Length - 1);
                        //}
                        //if (view.lxpfTemp_TzAttachs != null)
                        //{
                        //    view.lxpfTemp_TzAttachs = view.lxpfTemp_TzAttachs.Substring(view.lxpfTemp_TzAttachs.Length - 1);
                        //}
                        //if (view.jspTemp_TzAttachs != null)
                        //{
                        //    view.jspTemp_TzAttachs = view.jspTemp_TzAttachs.Substring(view.jspTemp_TzAttachs.Length - 1);
                        //}
                        //if (view.sgjcTemp_TzAttachs != null)
                        //{
                        //    view.sgjcTemp_TzAttachs = view.sgjcTemp_TzAttachs.Substring(view.sgjcTemp_TzAttachs.Length - 1);
                        //}
                        //if (view.fgsyTemp_TzAttachs != null)
                        //{
                        //    view.fgsyTemp_TzAttachs = view.fgsyTemp_TzAttachs.Substring(view.fgsyTemp_TzAttachs.Length - 1);
                        //}
                        //if (view.gcjsxmTemp_TzAttachs != null)
                        //{
                        //    view.gcjsxmTemp_TzAttachs = view.gcjsxmTemp_TzAttachs.Substring(view.gcjsxmTemp_TzAttachs.Length - 1);
                        //}
                    }
                    #endregion
                    model.WorkFlowId = XtWorkFlowService.CreateTzProjectStartApplyWorkFlow(view);
                }
                #endregion

                var rows = DataOperateBusiness<Epm_TzProjectStartApply>.Get().Add(model);


                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzProjectStartApply.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzProjectStartApply");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzProjectStartApply(Epm_TzProjectStartApply model)
        {
            Result<int> result = new Result<int>();
            try
            {
                SetCurrentUser(model);

                //项目提出信息
                var tzProjectProposal = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId.Value);
                //项目批复信息
                var tzProjectApprovalInfo = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().GetList(t => t.ProjectId == model.ProjectId.Value).FirstOrDefault();

                model.ProjectCode = tzProjectProposal.ProjectCode;
                model.ApprovalNo = tzProjectApprovalInfo.ApprovalNo;
                model.Nature = tzProjectProposal.Nature;
                model.NatureName = tzProjectProposal.NatureName;
                model.ApplyTime = tzProjectProposal.ApplyTime;
                model.StationId = tzProjectProposal.StationId;
                model.StationCodeXt = tzProjectProposal.StationCodeXt;
                model.StationName = tzProjectProposal.StationName;
                model.CompanyId = tzProjectProposal.CompanyId;
                model.CompanyCodeXt = tzProjectProposal.CompanyCodeXt;
                model.CompanyName = tzProjectProposal.CompanyName;
                model.StartApplyTime = DateTime.Now;
                AddFilesBytzTable(model, model.TzAttachs);
                #region 协同开工申请  不要删哦
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)XtBusinessDataState.Auditing && XtWorkFlow == "1")
                {
                    XtTzProjectStartApplyView view = new XtTzProjectStartApplyView();

                    view.ProjectName = model.ProjectName;
                    view.ApplyTitle = model.ApplyTitle;
                    view.ApplyUserName = model.ApplyUserName;
                    view.CreateTime = model.CreateTime.ToString();
                    view.ApplyDepartment = model.ApplyDepartment;
                    view.ApplyTel = model.ApplyTel;
                    view.DesignScale = model.DesignScale;
                    view.BuildNumber = model.BuildNumber;
                    view.InvestmentEstimateAmount = model.InvestmentEstimateAmount.ToString();
                    view.FeasibilityReport = model.FeasibilityReport;
                    view.ApprovalNo = model.ReplyNumber;
                    view.ReplyInvestmentAmount = model.ReplyInvestmentAmount.ToString();
                    view.FundsSource = model.FundsSource;
                    view.CurrentPlanned = model.CurrentPlanned;
                    view.PlanStartTime = string.Format("{0:yyyy-MM-dd}", model.PlanStartTime);
                    view.PlanEndTime = string.Format("{0:yyyy-MM-dd}", model.PlanEndTime);
                    view.BuildCycle = model.BuildCycle;
                    view.ProjectSummary = model.ProjectSummary;
                    view.ProjectManagement = model.ProjectManagement;
                    view.BuildDeploy = model.BuildDeploy;
                    view.DesignUnits = model.DesignUnits;
                    view.ConstructionName = model.ConstructionName;
                    view.ProjectManager = model.ProjectManager;
                    view.ConstructionScore = model.ConstructionScore;
                    view.ConstructionSituation = model.ConstructionSituation;
                    view.SupervisionUnit = model.SupervisionUnit;
                    view.SupervisionEngineer = model.SupervisionEngineer;
                    view.SupervisionScore = model.SupervisionScore;
                    view.SupervisionSituation = model.SupervisionSituation;
                    view.ConstructionReady = model.ConstructionReady;
                    view.MainEquipment = model.MainEquipment;
                    view.Environment = model.Environment;
                    view.LeadershipName = model.LeadershipName;
                    view.gcxxTemp_TzAttachs = model.ProjectPlan;//工程形象
                    view.Temp_TzAttachs = "";
                    var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.CreateUserId);
                    if (baseUser == null)
                    {
                        throw new Exception("未找到申请人相关信息！");
                    }
                    else
                    {
                        view.hr_sqr = baseUser.ObjeId;
                    }
                    #region 附件
                    //附件
                    if (model.TzAttachs != null && model.TzAttachs.Any())
                    {
                        var xmglTemp = model.TzAttachs.Where(p => p.TypeNo == "XMGLJGWJ").ToList();
                        view.xmglTemp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(xmglTemp);

                        var lxpfTemp = model.TzAttachs.Where(p => p.TypeNo == "LXPFWJ").ToList();
                        view.lxpfTemp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(lxpfTemp);

                        var jspTemp = model.TzAttachs.Where(p => p.TypeNo == "SGSJWJ").ToList();
                        view.jspTemp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(jspTemp);

                        var sgjcTemp = model.TzAttachs.Where(p => p.TypeNo == "AQJYZWJ").ToList();
                        view.sgjcTemp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(sgjcTemp);

                        var fgsyTemp = model.TzAttachs.Where(p => p.TypeNo == "GYSWJ").ToList();
                        view.fgsyTemp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(fgsyTemp);

                        var gcjsxmTemp = model.TzAttachs.Where(p => p.TypeNo == "HSEWJ").ToList();
                        view.gcjsxmTemp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(gcjsxmTemp);

                        //string baseFaleUrl = System.Configuration.ConfigurationManager.AppSettings.Get("XtDownloadUrl");
                        //foreach (var item in model.TzAttachs)
                        //{
                        //    string fileUrl = string.Format("{0}?fileId={1}&type={2}", baseFaleUrl, item.Id, item.TypeNo);
                        //    switch (item.TypeNo)
                        //    {
                        //        case "XMGLJGWJ"://项目管理机构（项目经理部或油库项目组）设立的文件、机构组成和职责分工各一份
                        //            view.xmglTemp_TzAttachs = fileUrl + '|' + view.xmglTemp_TzAttachs; ;
                        //            break;
                        //        case "LXPFWJ"://立项批复或项目初步设计批复文件复印件一份
                        //            view.lxpfTemp_TzAttachs = fileUrl + '|' + view.lxpfTemp_TzAttachs; ;
                        //            break;
                        //        case "SGSJWJ"://经审批的施工组织设计或工程建设总体部署一份
                        //            view.jspTemp_TzAttachs = fileUrl + '|' + view.jspTemp_TzAttachs; ;
                        //            break;
                        //        case "AQJYZWJ"://施工进场人员名单及《安全教育合格证》（复印件）
                        //            view.sgjcTemp_TzAttachs = fileUrl + '|' + view.sgjcTemp_TzAttachs; ;
                        //            break;
                        //        case "GYSWJ"://分公司与供应厂商确定的主要设备材料交付时间表一份
                        //            view.fgsyTemp_TzAttachs = fileUrl + '|' + view.fgsyTemp_TzAttachs; ;
                        //            break;
                        //        case "HSEWJ"://工程建设项目，海英提供审查通过后的HSE作业指导书、HSE作业计划书和HS场检查表E现
                        //            view.gcjsxmTemp_TzAttachs = fileUrl + '|' + view.gcjsxmTemp_TzAttachs; ;
                        //            break;
                        //        default:
                        //            break;
                        //    }
                        //}
                        //if (view.xmglTemp_TzAttachs != null)
                        //{
                        //    view.xmglTemp_TzAttachs = view.xmglTemp_TzAttachs.Substring(view.xmglTemp_TzAttachs.Length - 1);
                        //}
                        //if (view.lxpfTemp_TzAttachs != null)
                        //{
                        //    view.lxpfTemp_TzAttachs = view.lxpfTemp_TzAttachs.Substring(view.lxpfTemp_TzAttachs.Length - 1);
                        //}
                        //if (view.jspTemp_TzAttachs != null)
                        //{
                        //    view.jspTemp_TzAttachs = view.jspTemp_TzAttachs.Substring(view.jspTemp_TzAttachs.Length - 1);
                        //}
                        //if (view.sgjcTemp_TzAttachs != null)
                        //{
                        //    view.sgjcTemp_TzAttachs = view.sgjcTemp_TzAttachs.Substring(view.sgjcTemp_TzAttachs.Length - 1);
                        //}
                        //if (view.fgsyTemp_TzAttachs != null)
                        //{
                        //    view.fgsyTemp_TzAttachs = view.fgsyTemp_TzAttachs.Substring(view.fgsyTemp_TzAttachs.Length - 1);
                        //}
                        //if (view.gcjsxmTemp_TzAttachs != null)
                        //{
                        //    view.gcjsxmTemp_TzAttachs = view.gcjsxmTemp_TzAttachs.Substring(view.gcjsxmTemp_TzAttachs.Length - 1);
                        //}
                    }
                    #endregion
                    model.WorkFlowId = XtWorkFlowService.CreateTzProjectStartApplyWorkFlow(view);
                }
                #endregion

                var rows = DataOperateBusiness<Epm_TzProjectStartApply>.Get().Update(model);



                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzProjectStartApply.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzProjectStartApply");
            }
            return result;
        }

        /// <summary>
        /// 修改开工申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateTzProjectStartApplyNew(TzProjectStartApplyView model)
        {
            Result<int> result = new Result<int>();
            var rows = 0;
            try
            {
                //项目提出信息
                var tzProjectProposal = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId.Value);

                //var temp = DataOperateBusiness<Epm_TzProjectStartApply>.Get().Single(t => t.ProjectId == model.ProjectId.Value);
                var temp = DataOperateBusiness<Epm_TzProjectStartApply>.Get().Single(p => p.Id == model.Id);
                //修改开工申请信息
                if (temp != null)
                {
                    #region  字段赋值
                    SetCurrentUser(temp);
                    temp.ProjectCode = tzProjectProposal.ProjectCode;
                    temp.ApprovalNo = tzProjectProposal.ApprovalNo;
                    temp.Nature = tzProjectProposal.Nature;
                    temp.NatureName = tzProjectProposal.NatureName;
                    temp.ApplyTime = tzProjectProposal.ApplyTime;
                    temp.StationId = tzProjectProposal.StationId;
                    temp.StationCodeXt = tzProjectProposal.StationCodeXt;
                    temp.StationName = tzProjectProposal.StationName;
                    temp.CompanyId = tzProjectProposal.CompanyId;
                    temp.CompanyCodeXt = tzProjectProposal.CompanyCodeXt;
                    temp.CompanyName = tzProjectProposal.CompanyName;
                    temp.ApplyTitle = model.ApplyTitle;
                    temp.ApplyUserId = model.ApplyUserId;
                    temp.ApplyUserCodeXt = model.ApplyUserCodeXt;
                    temp.ApplyUserName = model.ApplyUserName;
                    temp.StartApplyTime = model.StartApplyTime;
                    temp.ApplyDepartmentId = model.ApplyDepartmentId;
                    temp.ApplyDepartmentCodeXt = model.ApplyDepartmentCodeXt;
                    temp.ApplyDepartment = model.ApplyDepartment;
                    temp.ApplyCompanyId = model.ApplyCompanyId;
                    temp.ApplyCompanyCodeXt = model.ApplyCompanyCodeXt;
                    temp.ApplyCompanyName = model.ApplyCompanyName;
                    temp.ApplyTel = model.ApplyTel;
                    temp.DesignScale = model.DesignScale;
                    temp.InvestmentEstimateAmount = model.InvestmentEstimateAmount;
                    temp.FeasibilityReport = model.FeasibilityReport;
                    temp.BuildNumber = model.BuildNumber;
                    temp.ReplyInvestmentAmount = model.ReplyInvestmentAmount;
                    temp.FundsSourceType = model.FundsSourceType;
                    temp.FundsSource = model.FundsSource;
                    temp.CurrentPlanned = model.CurrentPlanned;
                    temp.PlanStartTime = model.PlanStartTime;
                    temp.PlanEndTime = model.PlanEndTime;
                    temp.BuildCycle = model.BuildCycle;
                    temp.ProjectSummary = model.ProjectSummary;
                    temp.ProjectManagement = model.ProjectManagement;
                    temp.BuildDeploy = model.BuildDeploy;
                    temp.DesignUnits = model.DesignUnits;
                    temp.ConstructionUnitId = model.ConstructionUnitId;
                    temp.ConstructionName = model.ConstructionName;
                    temp.ProjectManagerId = model.ProjectManagerId;
                    temp.ProjectManager = model.ProjectManager;
                    temp.ConstructionScore = model.ConstructionScore;
                    temp.ConstructionSituation = model.ConstructionSituation;
                    temp.SupervisionUnit = model.SupervisionUnit;
                    temp.SupervisionEngineer = model.SupervisionEngineer;
                    temp.SupervisionScore = model.SupervisionScore;
                    temp.SupervisionSituation = model.SupervisionSituation;
                    temp.ConstructionReady = model.ConstructionReady;
                    temp.MainEquipment = model.MainEquipment;
                    temp.Environment = model.Environment;
                    temp.ProjectPlan = model.ProjectPlan;
                    temp.LeadershipId = model.LeadershipId;
                    temp.LeadershipCodeXt = model.LeadershipCodeXt;
                    temp.LeadershipName = model.LeadershipName;
                    temp.State = model.State;
                    temp.Remark = model.Remark;

                    temp.ReplyNumber = model.ReplyNumber;
                    temp.CameraSerialNumber = model.CameraSerialNumber;
                    temp.VerificationCode = model.VerificationCode;
                    temp.UrlAddress = model.UrlAddress;
                    #endregion

                    rows = DataOperateBusiness<Epm_TzProjectStartApply>.Get().Update(temp);
                    AddFilesBytzTable(temp, model.TzAttachs);
                }
                else
                {
                    throw new Exception("该开工申请信息不存在或者已被删除！");
                }
                bool isAdd = false;
                //修改工期管理和外部手续信息
                var timeLimitAndProcedure = DataOperateBusiness<Epm_TimeLimitAndProcedure>.Get().Single(t => t.ProjectId == model.ProjectId.Value);
                if (timeLimitAndProcedure == null)
                {
                    isAdd = true;
                    timeLimitAndProcedure = new Epm_TimeLimitAndProcedure();
                    timeLimitAndProcedure.ProjectId = model.ProjectId;
                    timeLimitAndProcedure.IsCrossings = model.IsCrossings;
                    timeLimitAndProcedure.ShutdownTime = model.ShutdownTime;
                    timeLimitAndProcedure.PlanWorkStartTime = model.PlanWorkStartTime;
                    timeLimitAndProcedure.PlanWorkEndTime = model.PlanWorkEndTime;
                    timeLimitAndProcedure.TimeLimit = model.TimeLimit;
                    timeLimitAndProcedure.PlanOpeningTime = model.PlanOpeningTime;
                    timeLimitAndProcedure.PlanShutdowLimit = model.PlanShutdowLimit;
                    SetCreateUser(timeLimitAndProcedure);
                    //rows = DataOperateBusiness<Epm_TimeLimitAndProcedure>.Get().Update(timeLimitAndProcedure);

                }
                timeLimitAndProcedure.ProjectId = model.ProjectId;
                timeLimitAndProcedure.IsCrossings = model.IsCrossings;
                timeLimitAndProcedure.ShutdownTime = model.ShutdownTime;
                timeLimitAndProcedure.PlanWorkStartTime = model.PlanWorkStartTime;
                timeLimitAndProcedure.PlanWorkEndTime = model.PlanWorkEndTime;
                timeLimitAndProcedure.TimeLimit = model.TimeLimit;
                timeLimitAndProcedure.PlanOpeningTime = model.PlanOpeningTime;
                timeLimitAndProcedure.PlanShutdowLimit = model.PlanShutdowLimit;
                SetCurrentUser(timeLimitAndProcedure);
                //else
                //{
                //    throw new Exception("该工期管理及外部手续信息不存在或者已被删除！");
                //}
                if (isAdd)
                {
                    rows = DataOperateBusiness<Epm_TimeLimitAndProcedure>.Get().Add(timeLimitAndProcedure);
                }
                else
                {
                    rows = DataOperateBusiness<Epm_TimeLimitAndProcedure>.Get().Update(timeLimitAndProcedure);
                }
                AddFilesBytzTable(timeLimitAndProcedure, model.TzAttachsTime);
                #region 协同开工申请  不要删哦
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)XtBusinessDataState.Auditing && XtWorkFlow == "1")
                {
                    XtTzProjectStartApplyView view = new XtTzProjectStartApplyView();

                    view.ProjectName = model.ProjectName;
                    view.ApplyTitle = model.ApplyTitle;
                    view.ApplyUserName = model.ApplyUserName;
                    view.CreateTime = model.CreateTime.ToString();
                    view.ApplyDepartment = model.ApplyDepartment;
                    view.ApplyTel = model.ApplyTel;
                    view.DesignScale = model.DesignScale;
                    view.BuildNumber = model.BuildNumber;
                    view.ApprovalNo = model.ReplyNumber;
                    view.InvestmentEstimateAmount = model.InvestmentEstimateAmount.ToString();
                    view.FeasibilityReport = model.FeasibilityReport;
                    view.ApprovalNo = model.ReplyNumber;
                    view.ReplyInvestmentAmount = model.ReplyInvestmentAmount.ToString();
                    view.FundsSource = model.FundsSource;
                    view.CurrentPlanned = model.CurrentPlanned;
                    view.PlanStartTime = string.Format("{0:yyyy-MM-dd}", model.PlanStartTime);
                    view.PlanEndTime = string.Format("{0:yyyy-MM-dd}", model.PlanEndTime);
                    view.BuildCycle = model.BuildCycle;
                    view.ProjectSummary = model.ProjectSummary;
                    view.ProjectManagement = model.ProjectManagement;
                    view.BuildDeploy = model.BuildDeploy;
                    view.DesignUnits = model.DesignUnits;
                    view.ConstructionName = model.ConstructionName;
                    view.ProjectManager = model.ProjectManager;
                    view.ConstructionScore = model.ConstructionScore;
                    view.ConstructionSituation = model.ConstructionSituation;
                    view.SupervisionUnit = model.SupervisionUnit;
                    view.SupervisionEngineer = model.SupervisionEngineer;
                    view.SupervisionScore = model.SupervisionScore;
                    view.SupervisionSituation = model.SupervisionSituation;
                    view.ConstructionReady = model.ConstructionReady;
                    view.MainEquipment = model.MainEquipment;
                    view.Environment = model.Environment;
                    view.LeadershipName = model.LeadershipName;
                    view.gcxxTemp_TzAttachs = model.ProjectPlan;//工程形象
                    view.Temp_TzAttachs = "";
                    var baseUser = DataOperateBasic<Base_User>.Get().GetModel((long)model.ApplyUserId);
                    if (baseUser == null)
                    {
                        throw new Exception("未找到申请人相关信息！");
                    }
                    else
                    {
                        view.hr_sqr = baseUser.ObjeId;
                    }
                    #region 附件
                    //附件
                    if (model.TzAttachs != null && model.TzAttachs.Any())
                    {
                        //string baseFaleUrl = System.Configuration.ConfigurationManager.AppSettings.Get("XtDownloadUrl");
                        //foreach (var item in model.TzAttachs)
                        //{
                        //    string fileUrl = string.Format("{0}?fileId={1}&type={2}", baseFaleUrl, item.Id, item.TypeNo);
                        //    switch (item.TypeNo)
                        //    {
                        //        case "XMGLJGWJ"://项目管理机构（项目经理部或油库项目组）设立的文件、机构组成和职责分工各一份
                        //            view.xmglTemp_TzAttachs = fileUrl + '|' + view.xmglTemp_TzAttachs; ;
                        //            break;
                        //        case "LXPFWJ"://立项批复或项目初步设计批复文件复印件一份
                        //            view.lxpfTemp_TzAttachs = fileUrl + '|' + view.lxpfTemp_TzAttachs; ;
                        //            break;
                        //        case "SGSJWJ"://经审批的施工组织设计或工程建设总体部署一份
                        //            view.jspTemp_TzAttachs = fileUrl + '|' + view.jspTemp_TzAttachs; ;
                        //            break;
                        //        case "AQJYZWJ"://施工进场人员名单及《安全教育合格证》（复印件）
                        //            view.sgjcTemp_TzAttachs = fileUrl + '|' + view.sgjcTemp_TzAttachs; ;
                        //            break;
                        //        case "GYSWJ"://分公司与供应厂商确定的主要设备材料交付时间表一份
                        //            view.fgsyTemp_TzAttachs = fileUrl + '|' + view.fgsyTemp_TzAttachs; ;
                        //            break;
                        //        case "HSEWJ"://工程建设项目，海英提供审查通过后的HSE作业指导书、HSE作业计划书和HS场检查表E现
                        //            view.gcjsxmTemp_TzAttachs = fileUrl + '|' + view.gcjsxmTemp_TzAttachs; ;
                        //            break;
                        //        default:
                        //            break;
                        //    }
                        //}
                        //if (view.xmglTemp_TzAttachs != null)
                        //{
                        //    view.xmglTemp_TzAttachs = view.xmglTemp_TzAttachs.Substring(view.xmglTemp_TzAttachs.Length - 1);
                        //}
                        //if (view.lxpfTemp_TzAttachs != null)
                        //{
                        //    view.lxpfTemp_TzAttachs = view.lxpfTemp_TzAttachs.Substring(view.lxpfTemp_TzAttachs.Length - 1);
                        //}
                        //if (view.jspTemp_TzAttachs != null)
                        //{
                        //    view.jspTemp_TzAttachs = view.jspTemp_TzAttachs.Substring(view.jspTemp_TzAttachs.Length - 1);
                        //}
                        //if (view.sgjcTemp_TzAttachs != null)
                        //{
                        //    view.sgjcTemp_TzAttachs = view.sgjcTemp_TzAttachs.Substring(view.sgjcTemp_TzAttachs.Length - 1);
                        //}
                        //if (view.fgsyTemp_TzAttachs != null)
                        //{
                        //    view.fgsyTemp_TzAttachs = view.fgsyTemp_TzAttachs.Substring(view.fgsyTemp_TzAttachs.Length - 1);
                        //}
                        //if (view.gcjsxmTemp_TzAttachs != null)
                        //{
                        //    view.gcjsxmTemp_TzAttachs = view.gcjsxmTemp_TzAttachs.Substring(view.gcjsxmTemp_TzAttachs.Length - 1);
                        //}
                        var xmglTemp = model.TzAttachs.Where(p => p.TypeNo == "XMGLJGWJ").ToList();
                        view.xmglTemp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(xmglTemp);

                        var lxpfTemp = model.TzAttachs.Where(p => p.TypeNo == "LXPFWJ").ToList();
                        view.lxpfTemp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(lxpfTemp);

                        var jspTemp = model.TzAttachs.Where(p => p.TypeNo == "SGSJWJ").ToList();
                        view.jspTemp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(jspTemp);

                        var sgjcTemp = model.TzAttachs.Where(p => p.TypeNo == "AQJYZWJ").ToList();
                        view.sgjcTemp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(sgjcTemp);

                        var fgsyTemp = model.TzAttachs.Where(p => p.TypeNo == "GYSWJ").ToList();
                        view.fgsyTemp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(fgsyTemp);

                        var gcjsxmTemp = model.TzAttachs.Where(p => p.TypeNo == "HSEWJ").ToList();
                        view.gcjsxmTemp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(gcjsxmTemp);
                    }
                    #endregion
                    model.WorkFlowId = XtWorkFlowService.CreateTzProjectStartApplyWorkFlow(view);
                }
                #endregion
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzProjectStartApply.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzProjectStartApplyNew");
            }
            return result;
        }

        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzProjectStartApplyByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzProjectStartApply>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzProjectStartApply>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzProjectStartApply.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzProjectStartApplyByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectStartApply>> GetTzProjectStartApplyList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzProjectStartApply>> result = new Result<List<Epm_TzProjectStartApply>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzProjectStartApply>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzProjectStartApplyList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzProjectStartApply> GetTzProjectStartApplyModel(long id)
        {
            Result<Epm_TzProjectStartApply> result = new Result<Epm_TzProjectStartApply>();
            try
            {
                var model = DataOperateBusiness<Epm_TzProjectStartApply>.Get().GetModel(id);

                if (model != null)
                {
                    List<Epm_TzAttachs> tzAttachsList = new List<Epm_TzAttachs>();
                    tzAttachsList = GetFilesByTZTable("Epm_TzProjectStartApply", model.Id).Data;
                    if (tzAttachsList != null && tzAttachsList.Any())
                    {
                        model.TzAttachs = tzAttachsList;
                    }
                }
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzProjectStartApplyModel");
            }
            return result;
        }

        public Result<TzStartTenderingAndSupplyView> GetTzProjectStartApplyModelAndOther(long id)
        {
            Result<TzStartTenderingAndSupplyView> res = new Result<TzStartTenderingAndSupplyView>();
            try
            {
                TzStartTenderingAndSupplyView view = new TzStartTenderingAndSupplyView();
                var model = DataOperateBusiness<Epm_TzProjectStartApply>.Get().GetModel(id);//开工申请

                if (model != null)
                {
                    List<Epm_TzAttachs> tzAttachsList = new List<Epm_TzAttachs>();
                    tzAttachsList = GetFilesByTZTable("Epm_TzProjectStartApply", model.Id).Data;
                    if (tzAttachsList != null && tzAttachsList.Any())
                    {
                        model.TzAttachs = tzAttachsList;
                    }
                    //工期管理和外部手续

                    var timeAndCrossings = DataOperateBusiness<Epm_TimeLimitAndProcedure>.Get().Single(p => p.ProjectId == model.ProjectId);//根据项目id获取对应的工期管理和外部手续
                    if (timeAndCrossings != null)
                    {
                        List<Epm_TzAttachs> tzAttachsList1 = new List<Epm_TzAttachs>();
                        tzAttachsList1 = GetFilesByTZTable("Epm_TimeLimitAndProcedure", timeAndCrossings.Id).Data;
                        if (tzAttachsList1 != null && tzAttachsList1.Any())
                        {
                            timeAndCrossings.TzAttachs = tzAttachsList1;
                        }
                        view.timeAndCrossings = timeAndCrossings;
                    }

                }
                view.TzProjectStartApply = model;



                res.Data = view;
                res.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                res.Data = null;
                res.Flag = EResultFlag.Failure;
                res.Exception = new ExceptionEx(ex, "GetTzProjectStartApplyModel");
            }
            return res;
        }

        /// <summary>
        /// 根据项目id获取工期和手续
        /// </summary>
        /// <param name="projectid"></param>
        /// <returns></returns>
        public Result<Epm_TimeLimitAndProcedure> GetTimeLimitAndProcedure(long projectid)
        {
            Result<Epm_TimeLimitAndProcedure> res = new Result<Epm_TimeLimitAndProcedure>();
            try
            {

                //工期管理和外部手续
                var timeAndCrossings = DataOperateBusiness<Epm_TimeLimitAndProcedure>.Get().Single(p => p.ProjectId == projectid);//根据项目id获取对应的工期管理和外部手续
                if (timeAndCrossings != null)
                {
                    List<Epm_TzAttachs> tzAttachsList1 = new List<Epm_TzAttachs>();
                    tzAttachsList1 = GetFilesByTZTable("Epm_TimeLimitAndProcedure", timeAndCrossings.Id).Data;
                    if (tzAttachsList1 != null && tzAttachsList1.Any())
                    {
                        timeAndCrossings.TzAttachs = tzAttachsList1;
                    }
                    res.Data = timeAndCrossings;
                    res.Flag = EResultFlag.Success;
                }
            }
            catch (Exception ex)
            {
                res.Data = null;
                res.Flag = EResultFlag.Failure;
                res.Exception = new ExceptionEx(ex, "GetTimeLimitAndProcedure");
            }
            return res;
        }



        //
        public Result<bool> isExistTenderingAndSupply(long projectId)
        {
            Result<bool> res = new Result<bool>();

            Result<Epm_TzTenderingApply> result = new Result<Epm_TzTenderingApply>();
            try
            {
                var Tenderingmodel = DataOperateBusiness<Epm_TzTenderingApply>.Get().Single(p => p.ProjectId.Contains(projectId.ToString()) && p.State == (int)PreProjectState.ApprovalSuccess);
                var SupplyModel = DataOperateBusiness<Epm_TzSupplyMaterialApply>.Get().Single(p => p.ProjectId == projectId && p.State == (int)PreProjectState.ApprovalSuccess);
                if (Tenderingmodel != null && SupplyModel != null)
                {
                    res.Data = true;
                    res.Flag = EResultFlag.Success;
                }
                else
                {
                    res.Data = false;
                    res.Flag = EResultFlag.Success;
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "IsTenderingAndSupply");
            }
            return res;
        }

        /// <summary>
        /// 修改开工申请状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzProjectStartApplyState(List<long> ids, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                foreach (var item in ids)
                {
                    var model = DataOperateBusiness<Epm_TzProjectStartApply>.Get().GetModel(item);
                    if (model != null)
                    {
                        //SetCurrentUser(model);
                        model.State = (int)state.ToEnumReq<PreProjectState>();
                        var rows = DataOperateBusiness<Epm_TzProjectStartApply>.Get().Update(model);

                        //生成项目信息数据
                        if (model.State == (int)PreProjectState.ApprovalSuccess)
                        {
                            //项目提出信息
                            var tzProjectProposal = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId.Value);
                            //项目批复信息
                            var tzProjectApprovalInfo = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().GetList(t => t.ProjectId == model.ProjectId.Value).FirstOrDefault();

                            #region 生成项目主信息
                            Epm_Project project = new Epm_Project();
                            project.Name = model.ProjectName;
                            //项目编码
                            project.Code = model.ProjectCode;
                            //所属分公司
                            project.CompanyId = model.CompanyId;
                            project.CompanyName = model.CompanyName;
                            //项目性质
                            project.ProjectNature = model.Nature;
                            project.ProjectNatureName = model.NatureName;
                            //加油站
                            project.ProjectSubjectId = model.StationId;
                            project.ProjectSubjectName = model.StationName;
                            project.ProjectSubjectCode = tzProjectProposal.StationCode;
                            //项目金额
                            project.Amount = tzProjectApprovalInfo.TotalInvestment;
                            project.CostCourse = model.FundsSource;
                            //项目开始时间
                            project.StartDate = model.StartApplyTime;
                            //项目结束时间
                            project.EndDate = model.PlanEndTime;

                            //柴汽比
                            project.GasolineDieselRatio = tzProjectApprovalInfo.ChaiQibi;
                            //项目类型
                            project.SubjectNo = tzProjectApprovalInfo.ProjectType;
                            project.SubjectName = tzProjectApprovalInfo.ProjectTypeCode;
                            //项目批复文号
                            project.ReplyNumber = tzProjectApprovalInfo.ApprovalNo;
                            //project.PMId = model.CreateUserId;
                            //project.PMName = model.CreateUserName;

                            project.State = (int)ProjectState.Construction;
                            project.TzProjectId = tzProjectProposal.Id;
                            SetCurrentUser(project);
                            SetCreateUser(project);
                            DataOperateBusiness<Epm_Project>.Get().Add(project);
                            #endregion

                            #region 生成项目试运行申请信息
                            Epm_TzProjectPolit ca = new Epm_TzProjectPolit();
                            
                            ca.CompanyId = model.ApplyCompanyId;
                            ca.CompanyName = model.ApplyCompanyName;
                            ca.ProjectId = model.ProjectId;
                            ca.ProjectName = model.ProjectName;
                            ca.State = (int)PreProjectApprovalState.WaitSubmitted;
                            SetCurrentUser(ca);
                            SetCreateUser(ca);
                            DataOperateBusiness<Epm_TzProjectPolit>.Get().Add(ca);
                            #endregion

                            #region 视频管理加信息
                            Base_VideoManage vm = new Base_VideoManage();
                            vm.CameraName = model.ProjectName;
                            vm.CameraState = "1";
                            vm.Companyid = model.CompanyId;
                            vm.Companyname = model.CompanyName;
                            vm.CompanyPerson = model.LeadershipName;
                            vm.DeviceSequence = model.CameraSerialNumber;
                            vm.Projectid = model.ProjectId;
                            vm.ProjectName = model.ProjectName;
                            vm.UrlAddress = model.UrlAddress;
                            vm.VerificationCode = model.VerificationCode;
                            SetCurrentUser(vm);
                            SetCreateUser(vm);
                            DataOperateBasic<Base_VideoManage>.Get().Add(vm);
                            #endregion
                        }
                        result.Data = rows;
                        result.Flag = EResultFlag.Success;
                    }
                    else
                    {
                        throw new Exception("该开工申请信息不存在");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzProjectStartApplyState");
            }
            return result;
        }



    }
}
