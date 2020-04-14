using System;
using System.Collections.Generic;
using System.Linq;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.epm.ViewModel;
using hc.Plat.Common.Global;
using hc.Plat.Common.Service;

using hc.epm.Common;
using hc.Plat.Common.Extend;
using hc.epm.DataModel.Basic;

namespace hc.epm.Service.ClientSite
{
    /// <summary>
    /// 延期申请
    /// </summary>
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        /// <summary>
        /// 获取延期申请列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_PlanDelay>> GetPlanDelayList(QueryCondition qc)
        {
            Result<List<Epm_PlanDelay>> result = new Result<List<Epm_PlanDelay>>();
            try
            {
                AddDefaultWeb(qc);
                qc.PageInfo = new PageListInfo();

                if (!qc.SortList.Any())
                {
                    qc.SortList.Add(new SortExpression()
                    {
                        SortName = "ProjectId",
                        SortType = eSortType.Desc
                    });
                }

                result = DataOperate.QueryListSimple<Epm_PlanDelay>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "");
                result.AllRowsCount = -1;
            }
            return result;
        }

        /// <summary>
        /// 新增延期申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<bool> AddPlanDelay(PlanDelayView model)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                if (model == null)
                {
                    throw new Exception("请填写延期申请内容！");
                }

                Epm_Plan plan = DataOperateBusiness<Epm_Plan>.Get().GetModel(model.PlanId);
                if (plan == null)
                {
                    throw new Exception("所选计划信息不存在！");
                }
                //if (!CurrentProjectIds.Contains(plan.ProjectId.ToString()))
                //{
                //    throw new Exception("你未负责该项目，无权对该项目下的计划进行延期申请！");
                //}

                var listp = DataOperateBusiness<Epm_PlanDelay>.Get().GetList(t => (t.State == (int)ApprovalState.WaitAppr || t.State == (int)ApprovalState.Enabled)
                && t.ProjectId == model.ProjectId
                && t.PlanId == model.PlanId).ToList();
                if (listp.Count > 0)
                {
                    throw new Exception("你选择的工程节点已经存在待审核或者草稿状态的延期申请！");
                }
                Epm_PlanDelay delay = new Epm_PlanDelay();
                delay.Id = model.Id;
                delay.ProjectId = model.ProjectId;
                delay.ProjectName = model.ProjectName;
                delay.PlanId = model.PlanId;
                delay.PlanName = model.PlanName;

                delay.OldStartDate = plan.StartTime.Value;
                delay.OldEndDate = plan.EndTime.Value;
                delay.DelayDay = model.DelayDay;
                delay.Reason = model.Reason;
                delay.Remark = model.Remark;

                delay.State = model.State;
                delay.CreateDate = DateTime.Today;
                delay.ApplyCompanyId = CurrentUser.CompanyId;
                delay.ApplyCompanyName = CurrentCompanyName;
                delay.State = model.State;

                SetCreateUser(delay);
                SetCurrentUser(delay);

                if (!model.PlanDelayCompanys.Any())
                {
                    throw new Exception("请选择责任单位！");
                }
                model.PlanDelayCompanys.ForEach(p =>
                {
                    p.DelayId = model.Id;
                    p.ProjectId = model.ProjectId;
                    p.ProjectName = model.ProjectName;
                    p.State = delay.State;
                    SetCreateUser(p);
                    SetCurrentUser(p);
                });
                DataOperateBusiness<Epm_PlanDelayCompany>.Get().AddRange(model.PlanDelayCompanys);
                DataOperateBusiness<Epm_PlanDelay>.Get().Add(delay);

                // todo: 如果是提交审核，添加消息推送
                if (delay.State == (int)ApprovalState.WaitAppr)
                {
                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(delay.ProjectId);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "提报了变更计划单，待审核";
                    app.Content = CurrentUserName + "提报了变更计划单，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.DelayApply.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.DelayApply.GetText();
                    app.BusinessState = (int)(ApprovalState.WaitAppr);
                    app.BusinessId = delay.Id;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = delay.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.DelayApply.GetText(), SystemRight.Add.GetText(), "提交延期申请生成待办: " + delay.Id);
                    #endregion

                    #region 消息
                    var waitSend = GetWaitSendMessageList(model.ProjectId);
                    foreach (var send in waitSend)
                    {
                        Epm_Massage modelMsg = new Epm_Massage();
                        modelMsg.ReadTime = null;
                        modelMsg.RecId = send.Key;
                        modelMsg.RecName = send.Value;
                        modelMsg.RecTime = DateTime.Now;
                        modelMsg.SendId = CurrentUserID.ToLongReq();
                        modelMsg.SendName = CurrentUserName;
                        modelMsg.SendTime = DateTime.Now;
                        modelMsg.Title = CurrentUserName + "提报了变更计划单，待审核";
                        modelMsg.Content = CurrentUserName + "提报了变更计划单，待审核";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.DelayApply.ToString();
                        modelMsg.ProjectId = model.ProjectId;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion

                    #region 发送短信
                    //Dictionary<string, string> parameterssm = new Dictionary<string, string>();
                    //parameterssm.Add("UserName", CurrentUserName);
                    //WriteSMS(project.ContactUserId.Value, project.CompanyId, MessageStep.DelayApplyAdd, parameterssm);
                    #endregion
                }

                result.Data = true;
                result.Flag = EResultFlag.Success;

                // todo：写操作日志
            }
            catch (Exception ex)
            {
                result.Flag = EResultFlag.Failure;
                result.Data = false;
                result.Exception = new ExceptionEx(ex, "AddPlanDelay");
            }
            return result;
        }

        /// <summary>
        /// 更新延期申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<bool> ModifyPlanDelay(PlanDelayView model)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                if (model == null)
                {
                    throw new Exception("请填写延期申请内容！");
                }

                Epm_PlanDelay planDelay = DataOperateBusiness<Epm_PlanDelay>.Get().GetModel(model.Id);
                if (planDelay == null)
                {
                    throw new Exception("要修改的延期申请不存在！");
                }
                if (planDelay.State != (int)ApprovalState.Enabled && planDelay.State != (int)ApprovalState.ApprFailure)
                {
                    throw new Exception("当前延期申请不可进行修改！");
                }
                if (!CurrentProjectIds.Contains(planDelay.ProjectId.ToString()))
                {
                    throw new Exception("你未负责该项目，无权修改延期申请！");
                }

                var listp = DataOperateBusiness<Epm_PlanDelay>.Get().GetList(t => (t.State == (int)ApprovalState.WaitAppr || t.State == (int)ApprovalState.Enabled)
                && t.ProjectId == model.ProjectId
                && t.PlanId == model.PlanId
                && t.Id != model.Id).ToList();
                if (listp.Count > 0)
                {
                    throw new Exception("你选择的工程节点已经存在待审核或者草稿状态的延期申请！");
                }

                Epm_Plan plan = DataOperateBusiness<Epm_Plan>.Get().GetModel(model.PlanId);
                if (plan == null)
                {
                    throw new Exception("所选计划信息不存在！");
                }

                planDelay.ProjectId = model.ProjectId;
                planDelay.PlanName = model.PlanName;

                planDelay.OldStartDate = plan.StartTime.Value;
                planDelay.OldEndDate = plan.EndTime.Value;
                planDelay.DelayDay = model.DelayDay;
                planDelay.Reason = model.Reason;
                planDelay.Remark = model.Remark;
                planDelay.State = model.State;
                planDelay.ApplyCompanyId = CurrentUser.CompanyId;
                planDelay.ApplyCompanyName = CurrentCompanyName;
                planDelay.State = model.State;

                SetCurrentUser(planDelay);

                if (!model.PlanDelayCompanys.Any())
                {
                    throw new Exception("请选择责任单位！");
                }

                DataOperateBusiness<Epm_PlanDelay>.Get().Update(planDelay);
                var delayCompanyList = DataOperateBusiness<Epm_PlanDelayCompany>.Get().GetList(p => p.DelayId == planDelay.Id).ToList();
                DataOperateBusiness<Epm_PlanDelayCompany>.Get().DeleteRange(delayCompanyList);

                model.PlanDelayCompanys.ForEach(p =>
                {
                    p.DelayId = model.Id;
                    p.ProjectId = model.ProjectId;
                    p.ProjectName = model.ProjectName;
                    p.State = planDelay.State;
                    SetCreateUser(p);
                    SetCurrentUser(p);
                });
                DataOperateBusiness<Epm_PlanDelayCompany>.Get().AddRange(model.PlanDelayCompanys);

                result.Data = true;
                result.Flag = EResultFlag.Success;

                //处理待办
                var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == planDelay.Id && t.IsApprover == false).FirstOrDefault();
                if (tempApp != null)
                {
                    ComplateApprover(tempApp.Id);
                }

                // todo: 如果是提交审核，添加消息推送
                if (planDelay.State == (int)ApprovalState.WaitAppr)
                {
                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(planDelay.ProjectId);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "提报了变更计划单，待审核";
                    app.Content = CurrentUserName + "提报了变更计划更单，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.DelayApply.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.DelayApply.GetText();
                    app.BusinessState = (int)(ApprovalState.WaitAppr);
                    app.BusinessId = planDelay.Id;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = planDelay.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.DelayApply.GetText(), SystemRight.Add.GetText(), "提交延期申请生成待办: " + planDelay.Id);
                    #endregion

                    #region 消息
                    var waitSend = GetWaitSendMessageList(model.ProjectId);
                    foreach (var send in waitSend)
                    {
                        Epm_Massage modelMsg = new Epm_Massage();
                        modelMsg.ReadTime = null;
                        modelMsg.RecId = send.Key;
                        modelMsg.RecName = send.Value;
                        modelMsg.RecTime = DateTime.Now;
                        modelMsg.SendId = CurrentUserID.ToLongReq();
                        modelMsg.SendName = CurrentUserName;
                        modelMsg.SendTime = DateTime.Now;
                        modelMsg.Title = CurrentUserName + "提报了变更计划单，待审核";
                        modelMsg.Content = CurrentUserName + "提报了变更计划单，待审核";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.DelayApply.ToString();
                        modelMsg.ProjectId = model.ProjectId;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion

                    #region 发送短信
                    //Dictionary<string, string> parameterssm = new Dictionary<string, string>();
                    //parameterssm.Add("UserName", CurrentUserName);
                    //WriteSMS(project.ContactUserId.Value, project.CompanyId, MessageStep.DelayApplyAdd, parameterssm);
                    #endregion
                }

                // todo：写操作日志
            }
            catch (Exception ex)
            {
                result.Flag = EResultFlag.Failure;
                result.Data = false;
                result.Exception = new ExceptionEx(ex, "AddPlanDelay");
            }
            return result;
        }

        /// <summary>
        /// 审核延期申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AuditPlanDelay(Epm_PlanDelay model)
        {
            Result<int> result = new Result<int>();
            try
            {
                if (model == null)
                {
                    throw new Exception("请选择要审核的延期申请！");
                }
                Epm_PlanDelay planDelay = DataOperateBusiness<Epm_PlanDelay>.Get().GetModel(model.Id);
                if (planDelay == null)
                {
                    throw new Exception("要审核的延期申请不存在！");
                }
                // todo: 判断延期申请的状态， 是否可以进行审核

                var list = DataOperateBusiness<Epm_PlanDelayCompany>.Get().GetList(p => p.DelayId == model.Id).ToList();
                planDelay.State = model.State;
                planDelay.AuditDate = DateTime.Today;
                planDelay.AuditUserId = CurrentUser.Id;
                planDelay.AuditUserName = CurrentUserName;
                planDelay.OrgId = CurrentUser.CompanyId;
                planDelay.OrgName = CurrentCompanyName;
                SetCurrentUser(planDelay);

                list.ForEach(p =>
                {
                    SetCurrentUser(p);
                    p.State = planDelay.State;
                });

                DataOperateBusiness<Epm_PlanDelay>.Get().Update(planDelay);
                DataOperateBusiness<Epm_PlanDelayCompany>.Get().UpdateRange(list);

                //处理待办
                var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == planDelay.Id && t.IsApprover == false).FirstOrDefault();
                if (tempApp != null)
                {
                    ComplateApprover(tempApp.Id);
                }
                string title = "";
                if ((ApprovalState)Enum.ToObject(typeof(ApprovalState), model.State) == ApprovalState.ApprSuccess)
                {
                    //更新计划里的延期计划完工时间
                    Epm_Plan plan = DataOperateBusiness<Epm_Plan>.Get().GetModel(planDelay.PlanId);
                    plan.DelayTime = plan.EndTime.Value.AddDays(double.Parse(planDelay.DelayDay.ToString()));

                    DateTime start = Convert.ToDateTime(plan.StartTime);
                    DateTime end = Convert.ToDateTime(plan.DelayTime);
                    TimeSpan sp = end.Subtract(start);
                    int days = sp.Days + 1;

                    plan.BuildDays = days > 0 ? days : 0;
                    SetCurrentUser(plan);
                    DataOperateBusiness<Epm_Plan>.Get().Update(plan);

                    title = planDelay.CreateUserName + "提报的变更计划单，审核通过";
                    #region 生成待办
                    List<Epm_Approver> listApp = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = planDelay.CreateUserName + "提报的变更计划单，审核通过";
                    app.Content = planDelay.CreateUserName + "提报的变更计划单，审核通过";
                    app.SendUserId = planDelay.CreateUserId;
                    app.SendUserName = planDelay.CreateUserName;
                    app.SendTime = planDelay.CreateTime;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.DelayApply.ToString();
                    app.Action = SystemRight.Check.ToString();
                    app.BusinessTypeName = BusinessType.DelayApply.GetText();
                    app.BusinessState = (int)(ApprovalState.ApprSuccess);
                    app.BusinessId = planDelay.Id;
                    app.ApproverId = planDelay.CreateUserId;
                    app.ApproverName = planDelay.CreateUserName;
                    app.ProjectId = planDelay.ProjectId;
                    app.ProjectName = planDelay.ProjectName;
                    listApp.Add(app);
                    AddApproverBatch(listApp);
                    WriteLog(BusinessType.DelayApply.GetText(), SystemRight.Check.GetText(), "审核通过延期申请生成待办: " + model.Id);
                    #endregion
                }
                else if ((ApprovalState)Enum.ToObject(typeof(ApprovalState), model.State) == ApprovalState.ApprFailure)
                {
                    title = planDelay.CreateUserName + "提报的变更计划单已被驳回，请处理";
                    #region 生成待办
                    List<Epm_Approver> listApp = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = planDelay.CreateUserName + "提报的变更计划单已被驳回，请处理";
                    app.Content = planDelay.CreateUserName + "提报的变更计划单已被驳回，请处理";
                    app.SendUserId = planDelay.CreateUserId;
                    app.SendUserName = planDelay.CreateUserName;
                    app.SendTime = planDelay.CreateTime;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.DelayApply.ToString();
                    app.Action = SystemRight.UnCheck.ToString();
                    app.BusinessTypeName = BusinessType.DelayApply.GetText();
                    app.BusinessState = (int)(ApprovalState.ApprFailure);
                    app.BusinessId = planDelay.Id;
                    app.ApproverId = planDelay.CreateUserId;
                    app.ApproverName = planDelay.CreateUserName;
                    app.ProjectId = planDelay.ProjectId;
                    app.ProjectName = planDelay.ProjectName;
                    listApp.Add(app);
                    AddApproverBatch(listApp);
                    WriteLog(BusinessType.DelayApply.GetText(), SystemRight.UnCheck.GetText(), "驳回延期申请生成待办: " + model.Id);
                    #endregion

                    #region 发送短信
                    //WriteSMS(model.CreateUserId, 0, MessageStep.DelayApplyReject, null);
                    #endregion
                }
                else if ((ApprovalState)Enum.ToObject(typeof(ApprovalState), model.State) == ApprovalState.Discarded)
                {
                    title = planDelay.CreateUserName + "提报了变更计划单，已废弃";
                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(planDelay.ProjectId);
                    List<Epm_Approver> listApp = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = planDelay.CreateUserName + "提报了变更计划单，已废弃";
                    app.Content = planDelay.CreateUserName + "提报了变更计划单，已废弃";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.DelayApply.ToString();
                    app.Action = SystemRight.Invalid.ToString();
                    app.BusinessTypeName = BusinessType.DelayApply.GetText();
                    app.BusinessState = (int)(ApprovalState.Discarded);
                    app.BusinessId = planDelay.Id;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = planDelay.ProjectId;
                    app.ProjectName = project.Name;
                    listApp.Add(app);
                    AddApproverBatch(listApp);
                    WriteLog(BusinessType.DelayApply.GetText(), SystemRight.Invalid.GetText(), "废弃延期申请生成待办: " + planDelay.Id);
                    #endregion
                }

                #region 消息
                var waitSend = GetWaitSendMessageList(planDelay.ProjectId);
                foreach (var send in waitSend)
                {
                    Epm_Massage modelMsg = new Epm_Massage();
                    modelMsg.ReadTime = null;
                    modelMsg.RecId = send.Key;
                    modelMsg.RecName = send.Value;
                    modelMsg.RecTime = DateTime.Now;
                    modelMsg.SendId = CurrentUserID.ToLongReq();
                    modelMsg.SendName = CurrentUserName;
                    modelMsg.SendTime = DateTime.Now;
                    modelMsg.Title = title;
                    modelMsg.Content = title;
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = planDelay.Id;
                    modelMsg.BussinesType = BusinessType.DelayApply.ToString();
                    modelMsg.ProjectId = planDelay.ProjectId;
                    modelMsg.ProjectName = planDelay.ProjectName;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                }
                #endregion

                result.Data = 1;
                result.Flag = EResultFlag.Success;

                // todo: 添加消息推送
                // todo：写操作日志
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AuditPlanDelay");
            }
            return result;
        }

        /// <summary>
        /// 删除延期申请
        /// </summary>
        /// <param name="id">延期申请 ID</param>
        /// <returns></returns>
        public Result<bool> DeletePlanDelay(long id)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                Epm_PlanDelay delay = DataOperateBusiness<Epm_PlanDelay>.Get().GetModel(id);
                if (delay == null)
                {
                    throw new Exception("所选延期申请不存在或已被删除！");
                }
                if (delay.State != (int)ApprovalState.Enabled && delay.State != (int)ApprovalState.Discarded)
                {
                    throw new Exception("只有未提交审核的延期可以删除！");
                }
                //if (!CurrentProjectIds.Contains(delay.ProjectId.ToString()))
                //{
                //    throw new Exception("你未负责该项目，无权删除延期申请！");
                //}
                //if (delay.CreateUserId != CurrentUserID.ToLongReq())
                //{
                //    throw new Exception("只有创建人才可以删除！");
                //}

                var list = DataOperateBusiness<Epm_PlanDelayCompany>.Get().GetList(p => p.DelayId == id).ToList();
                DataOperateBusiness<Epm_PlanDelay>.Get().Delete(delay);

                //处理待办
                var temp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == delay.Id && t.IsApprover == false).FirstOrDefault();
                if (temp != null)
                {
                    ComplateApprover(temp.Id);
                }

                #region 消息
                var waitSend = GetWaitSendMessageList(delay.ProjectId);
                foreach (var send in waitSend)
                {
                    Epm_Massage modelMsg = new Epm_Massage();
                    modelMsg.ReadTime = null;
                    modelMsg.RecId = send.Key;
                    modelMsg.RecName = send.Value;
                    modelMsg.RecTime = DateTime.Now;
                    modelMsg.SendId = CurrentUserID.ToLongReq();
                    modelMsg.SendName = CurrentUserName;
                    modelMsg.SendTime = DateTime.Now;
                    modelMsg.Title = delay.CreateUserName + "发起的延期申请已删除";
                    modelMsg.Content = delay.CreateUserName + "发起的延期申请已删除";
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = delay.Id;
                    modelMsg.BussinesType = BusinessType.DelayApply.ToString();
                    modelMsg.ProjectId = delay.ProjectId;
                    modelMsg.ProjectName = delay.ProjectName;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                }
                #endregion

                if (list.Any())
                {
                    foreach (var model in list)
                    {
                        model.OperateUserId = CurrentUserID.ToLongReq();
                        model.OperateUserName = CurrentUserName;
                        model.OperateTime = DateTime.Now;
                        model.DeleteTime = DateTime.Now;
                    }

                    DataOperateBusiness<Epm_PlanDelayCompany>.Get().DeleteRange(list);
                }

                result.Data = true;
                result.Flag = EResultFlag.Success;

                // todo：写操作日志
            }
            catch (Exception ex)
            {
                result.Data = false;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteSupervisorlogById");
            }
            return result;
        }

        /// <summary>
        /// 获取延期申请详情
        /// </summary>
        /// <param name="id">延期申请 ID</param>
        /// <returns></returns>
        public Result<PlanDelayView> GetPlanDelayInfo(long id)
        {
            Result<PlanDelayView> result = new Result<PlanDelayView>();
            try
            {
                Epm_PlanDelay delay = DataOperateBusiness<Epm_PlanDelay>.Get().GetModel(id);
                if (delay == null)
                {
                    throw new Exception("延期申请信息不存在！");
                }
                //if (!CurrentProjectIds.Contains(delay.ProjectId.ToString()))
                //{
                //    throw new Exception("你未负责该项目，无权查看延期申请！");
                //}
                PlanDelayView view = new PlanDelayView
                {
                    Id = delay.Id,
                    ProjectId = delay.ProjectId,
                    ProjectName = delay.ProjectName,
                    PlanId = delay.PlanId,
                    PlanName = delay.PlanName,

                    OldStartDate = delay.OldStartDate,
                    OldEndDate = delay.OldEndDate,
                    DelayDay = delay.DelayDay,
                    Reason = delay.Reason,
                    Remark = delay.Remark,

                    State = delay.State,
                    CreateDate = delay.CreateDate,
                    ApplyCompanyId = delay.ApplyCompanyId,
                    ApplyCompanyName = delay.ApplyCompanyName,
                    AuditDate = delay.AuditDate,

                    CreateTime = delay.CreateDate,
                    CreateUserId = delay.CreateUserId,
                    CreateUserName = delay.CreateUserName,
                    AuditUserName = delay.AuditUserName,
                    OrgName = delay.OrgName

                };

                view.PlanDelayCompanys = DataOperateBusiness<Epm_PlanDelayCompany>.Get()
                    .GetList(p => p.DelayId == id).ToList();

                view.CompanyIds = view.PlanDelayCompanys.Select(t => t.CompanyId).JoinToString("、");
                view.CompanyNames = view.PlanDelayCompanys.Select(t => t.CompanyName).JoinToString("、");
                result.Data = view;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSupervisorLogModel");
            }
            return result;
        }
    }
}
