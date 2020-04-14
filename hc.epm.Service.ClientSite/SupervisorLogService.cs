using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;

using hc.epm.DataModel.Basic;
using hc.epm.ViewModel;
using hc.Plat.Common.Service;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <param name="details">监理日志明细</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddSupervisorLog(Epm_SupervisorLog model, List<SupervisorLogDetailView> details)
        {
            Result<int> result = new Result<int>();
            try
            {
                if (details == null || !details.Any())
                {
                    throw new Exception("请填写施工计划完成情况！");
                }

                List<Epm_Plan> planList = DataOperateBusiness<Epm_Plan>.Get().GetList(p => p.ProjectId == model.ProjectId).ToList();
                List<Epm_Plan> updatePlanList = new List<Epm_Plan>();

                List<Epm_SupervisorLogDetails> logDetailses = new List<Epm_SupervisorLogDetails>();
                List<Base_Files> files = new List<Base_Files>();
                foreach (var supervisorLogDetailView in details)
                {
                    Epm_SupervisorLogDetails supervisorLog = new Epm_SupervisorLogDetails()
                    {
                        Id = supervisorLogDetailView.Id,
                        LogId = model.Id,
                        ProjectId = model.ProjectId,
                        ProjectName = model.ProjectName,
                        PlanId = supervisorLogDetailView.PlanId,
                        PlanName = supervisorLogDetailView.PlanName,
                        IsFinish = supervisorLogDetailView.IsFinish,
                        StartTime = supervisorLogDetailView.StartTime,
                        EndTime = supervisorLogDetailView.EndTime,
                        FinishScale = supervisorLogDetailView.FinishScale,
                        ToResean = supervisorLogDetailView.ToResean,
                        State = supervisorLogDetailView.State,
                        Remark = supervisorLogDetailView.Remark,
                        CrtCompanyId = supervisorLogDetailView.CrtCompanyId,
                        CrtCompanyName = supervisorLogDetailView.CrtCompanyName
                    };

                    supervisorLog = SetCreateUser(supervisorLog);
                    supervisorLog = SetCurrentUser(supervisorLog);
                    supervisorLog.CrtCompanyId = CurrentCompanyID.ToLongReq();
                    supervisorLog.CrtCompanyName = CurrentCompanyName;

                    logDetailses.Add(supervisorLog);

                    Epm_Plan plan = planList.FirstOrDefault(p => p.Id == supervisorLogDetailView.PlanId);
                    if (plan != null)
                    {
                        plan.StartDate = supervisorLogDetailView.StartTime;
                        plan.EndDate = supervisorLogDetailView.EndTime;
                        plan.IsFinish = supervisorLogDetailView.IsFinish;
                        plan.FinishScale = supervisorLogDetailView.FinishScale.ToString();
                        plan.NoFinishResean = supervisorLogDetailView.ToResean;

                        updatePlanList.Add(plan);
                    }
                }

                Epm_SupervisorLogDetails temp = logDetailses.FirstOrDefault();

                model = SetCreateUser(model);
                model = SetCurrentUser(model);
                model.CrtCompanyId = CurrentCompanyID.ToLongReq();
                model.CrtCompanyName = CurrentCompanyName;

                var rows = DataOperateBusiness<Epm_SupervisorLog>.Get().Add(model);
                DataOperateBusiness<Epm_SupervisorLogDetails>.Get().AddRange(logDetailses);
                DataOperateBusiness<Epm_Plan>.Get().UpdateRange(updatePlanList);

                if (files.Any())
                {
                    AddFilesByTable(temp, files);
                }

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Log.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddSupervisorLog");
            }
            return result;
        }

        /// <summary>
        /// 新增监理日志
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddSupervisorLogNew(SupervisorLogView model, List<long> workIdList)
        {
            Result<int> result = new Result<int>();
            try
            {
                if (model == null)
                {
                    throw new Exception("请填写监理日志相关信息！");
                }
                Epm_SupervisorLog log = ToMapModel(model);

                if (!string.IsNullOrEmpty(model.PlanId))
                {
                    var planList = model.PlanId.Split(',');
                    foreach (var item in planList)
                    {
                        var plan = DataOperateBusiness<Epm_Plan>.Get().GetModel(item.ToLongReq());
                        plan.StartDate = log.SubmitTime.HasValue ? log.SubmitTime.Value : DateTime.Now;
                        DataOperateBusiness<Epm_Plan>.Get().Update(plan);
                    }
                }

                if (!string.IsNullOrEmpty(model.nextPlanId))
                {
                    var nextplanList = model.nextPlanId.Split(',');
                    foreach (var item in nextplanList)
                    {
                        var plan = DataOperateBusiness<Epm_Plan>.Get().GetModel(item.ToLongReq());
                        plan.EndDate = log.SubmitTime.HasValue ? log.SubmitTime.Value : DateTime.Now;
                        DataOperateBusiness<Epm_Plan>.Get().Update(plan);
                    }
                }

                SetCreateUser(log);
                SetCurrentUser(log);
                log.IsDelete = false;

                if ((string.IsNullOrEmpty(log.NextPlanId)) && (workIdList == null || workIdList.Count == 0) && (!model.SenceList.Any() || model.SenceList.Count == 0))
                {
                    log.State = (int)ApprovalState.ApprSuccess;
                }
                else
                {
                    var temp = DataOperateBusiness<Epm_SupervisorLog>.Get().GetList(t => t.ProjectId == log.ProjectId && t.PlanId == log.PlanId && log.PlanId != null && t.State != (int)ApprovalState.ApprFailure).FirstOrDefault();
                    if (temp != null)
                    {
                        throw new Exception("所选工程节点计划已提交完成申请，不可重复提交！");
                    }

                    log.State = (int)ApprovalState.WaitAppr;
                }

                DataOperateBusiness<Epm_SupervisorLog>.Get().Add(log);
                //if (model.SupervisorLogCompanys != null && model.SupervisorLogCompanys.Any())
                //{
                //    model.SupervisorLogCompanys.ForEach(p =>
                //    {
                //        p.LogId = log.Id;
                //        p.ProjectId = log.ProjectId ?? 0;
                //        SetCreateUser(p);
                //        SetCurrentUser(p);
                //        p.IsDelete = false;
                //    });

                //    DataOperateBusiness<Epm_SupervisorLogCompany>.Get().AddRange(model.SupervisorLogCompanys);
                //}

                List<Epm_AttendanceList> aeList = new List<Epm_AttendanceList>();
                List<Epm_ProjectlLogName> epmLog = new List<Epm_ProjectlLogName>();
                foreach (var item in model.SupervisorLogCompanys)
                {
                    item.LogId = log.Id;
                    item.ProjectId = log.ProjectId ?? 0;
                    SetCreateUser(item);
                    SetCurrentUser(item);
                    item.IsDelete = false;
                    if (item.ProjectlLogName.Any())
                    {
                        foreach (var chrens in item.ProjectlLogName)
                        {
                            chrens.detailsid = item.Id;
                            chrens.personid = item.ProjectlLogName[0].personid;
                            chrens.name = item.ProjectlLogName[0].name;
                            chrens.type = item.ProjectlLogName[0].type;
                            SetCreateUser(chrens);
                            SetCurrentUser(chrens);
                            chrens.IsDelete = false;
                            epmLog.Add(chrens);
                        }
                    }
                    if (item.AttendanceList.Any())
                    {
                        foreach (var temps in item.AttendanceList)
                        {
                            temps.detailsid = item.Id;
                            //temps.permit = item.Permit == "是" ? "0" : "1";
                            if (temps.permit == "是")
                            {
                                temps.permit = "0";
                            }
                            else
                            {
                                temps.permit = "1";
                            }
                            //temps.bepresent = item.BePresent == "是" ? "0" : "1";
                            if (temps.bepresent == "是")
                            {
                                temps.bepresent = "0";
                            }
                            else
                            {
                                temps.bepresent = "1";
                            }
                            SetCreateUser(temps);
                            SetCurrentUser(temps);
                            temps.IsDelete = false;
                            aeList.Add(temps);
                        }
                    }
                }
                DataOperateBusiness<Epm_SupervisorLogCompany>.Get().AddRange(model.SupervisorLogCompanys);
                DataOperateBusiness<Epm_AttendanceList>.Get().AddRange(aeList);
                DataOperateBusiness<Epm_ProjectlLogName>.Get().AddRange(epmLog);

                //附件
                if (model.Attachs.Any())
                {
                    AddFilesByTable(log, model.Attachs);
                }

                //上传实景作业 PC
                if (model.SenceList.Any() && model.SenceList.Count > 0)
                {
                    List<Epm_SupervisorLogWork> logWorkList = new List<Epm_SupervisorLogWork>();
                    Epm_SupervisorLogWork logWork = null;
                    foreach (var item in model.SenceList)
                    {
                        if (item.Type == 2)
                        {
                            #region 添加实景
                            Epm_WorkUploadRealScene scene = new Epm_WorkUploadRealScene();
                            scene.ProjectId = model.ProjectId;
                            scene.WorkId = item.Id;
                            scene.UploadTime = DateTime.Now;
                            scene.State = (int)ApprovalState.WaitAppr;
                            AddWorkRealScenen(scene, item.Attachs);
                            #endregion

                            #region 添加监理日志关联作业实景
                            logWork = new Epm_SupervisorLogWork();
                            logWork.LogId = log.Id;
                            logWork.WorkId = item.Id;
                            logWork.WorkUploadId = scene.Id;
                            logWork.State = (int)ApprovalState.WaitAppr;
                            SetCreateUser(logWork);
                            SetCurrentUser(logWork);
                            DataOperateBusiness<Epm_SupervisorLogWork>.Get().Add(logWork);
                            #endregion
                        }
                        else
                        {
                            #region 添加监理日志关联作业实景
                            logWork = new Epm_SupervisorLogWork();
                            logWork.LogId = log.Id;
                            logWork.WorkId = item.Id;
                            logWork.WorkUploadId = 0;
                            logWork.State = (int)ApprovalState.WaitAppr;
                            SetCreateUser(logWork);
                            SetCurrentUser(logWork);
                            DataOperateBusiness<Epm_SupervisorLogWork>.Get().Add(logWork);
                            #endregion
                        }
                    }
                }
                //上传实景作业 APP
                if (workIdList != null && workIdList.Count > 0)
                {
                    List<Epm_SupervisorLogWork> logWorkList = new List<Epm_SupervisorLogWork>();
                    Epm_SupervisorLogWork logWork = null;
                    foreach (var workId in workIdList)
                    {
                        var workScene = DataOperateBusiness<Epm_WorkUploadRealScene>.Get().GetList(p => p.WorkId == workId).FirstOrDefault();
                        if (workScene != null)
                        {
                            logWork = new Epm_SupervisorLogWork();
                            logWork.LogId = log.Id;
                            logWork.WorkId = workScene.WorkId;
                            logWork.WorkUploadId = workScene.Id;
                            logWork.State = (int)ApprovalState.WaitAppr;
                            SetCreateUser(logWork);
                            SetCurrentUser(logWork);
                            logWorkList.Add(logWork);
                        }
                        else
                        {
                            logWork = new Epm_SupervisorLogWork();
                            logWork.LogId = log.Id;
                            logWork.WorkId = workId;
                            logWork.WorkUploadId = 0;
                            logWork.State = (int)ApprovalState.WaitAppr;
                            SetCreateUser(logWork);
                            SetCurrentUser(logWork);
                            logWorkList.Add(logWork);
                        }
                    }
                    DataOperateBusiness<Epm_SupervisorLogWork>.Get().AddRange(logWorkList);
                }

                if (log.State == (int)ApprovalState.WaitAppr)
                {
                    #region 生成待办
                    var projectList = DataOperateBusiness<Epm_Project>.Get().GetModel(log.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "提报了监理日志单，待审核";
                    app.Content = CurrentUserName + "提报了监理日志单，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Log.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Log.GetText();
                    app.BusinessState = (int)(ApprovalState.WaitAppr);
                    app.BusinessId = log.Id;
                    app.ApproverId = projectList.ContactUserId;
                    app.ApproverName = projectList.ContactUserName;
                    app.ProjectId = projectList.Id;
                    app.ProjectName = projectList.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Log.GetText(), SystemRight.Add.GetText(), "提交监理日志生成待办: " + log.Id);
                    #endregion

                    #region 消息
                    var waitSend = GetWaitSendMessageList(model.ProjectId.Value);
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
                        modelMsg.Title = CurrentUserName + "提报了监理日志单，待审核";
                        modelMsg.Content = CurrentUserName + "提报了监理日志单，待审核";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Log.ToString();
                        modelMsg.ProjectId = model.ProjectId.Value;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion

                    #region 发送短信
                    //Dictionary<string, string> parameterssm = new Dictionary<string, string>();
                    //parameterssm.Add("UserName", CurrentUserName);
                    //WriteSMS(projectList.ContactUserId.Value, projectList.CompanyId, MessageStep.SupervisorLogAdd, parameterssm);
                    #endregion
                }
                else {

                    #region 生成待办
                    var projectList = DataOperateBusiness<Epm_Project>.Get().GetModel(log.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "提报了监理日志单，请查看";
                    app.Content = CurrentUserName + "提报了监理日志单，请查看";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Log.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Log.GetText();
                    app.BusinessState = (int)(ApprovalState.ApprSuccess);
                    app.BusinessId = log.Id;
                    app.ApproverId = projectList.ContactUserId;
                    app.ApproverName = projectList.ContactUserName;
                    app.ProjectId = projectList.Id;
                    app.ProjectName = projectList.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Log.GetText(), SystemRight.Add.GetText(), "提交监理日志生成待办: " + log.Id);
                    #endregion

                    #region 消息
                    var waitSend = GetWaitSendMessageList(model.ProjectId.Value);
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
                        modelMsg.Title = CurrentUserName + "提报了监理日志单，请查看";
                        modelMsg.Content = CurrentUserName + "提报了监理日志单，请查看";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Log.ToString();
                        modelMsg.ProjectId = model.ProjectId.Value;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion
                }
                result.Data = 1;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Log.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddSupervisorLog");
            }
            return result;
        }

        /// <summary>
        /// 删除监理日志
        /// </summary>
        /// <param name="id">监理日志 ID</param>
        /// <returns></returns>
        public Result<bool> DeleteSupervisorlogByIdNew(long id)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                Epm_SupervisorLog log = DataOperateBusiness<Epm_SupervisorLog>.Get().GetModel(id);
                if (log == null)
                {
                    throw new Exception("所选监理日志不存在或已被删除！");
                }
                if (log.SubmitTime <= DateTime.Today)
                {
                    throw new Exception("历史监理日志不能删除！");
                }
                //if (!CurrentProjectIds.Contains(log.ProjectId.ToString()))
                //{
                //    throw new Exception("你未负责该项目，无权新增监理日志！");
                //}

                var list = DataOperateBusiness<Epm_SupervisorLogCompany>.Get().GetList(p => p.LogId == id).ToList();
                DataOperateBusiness<Epm_SupervisorLog>.Get().Delete(log);
                if (list.Any())
                {
                    foreach (var model in list)
                    {
                        model.OperateUserId = CurrentUserID.ToLongReq();
                        model.OperateUserName = CurrentUserName;
                        model.OperateTime = DateTime.Now;
                        model.DeleteTime = DateTime.Now;

                        //处理待办
                        var temp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == model.Id && t.IsApprover == false).FirstOrDefault();
                        if (temp != null)
                        {
                            ComplateApprover(temp.Id);
                        }
                    }
                    DataOperateBusiness<Epm_SupervisorLogCompany>.Get().DeleteRange(list);
                }

                #region 消息
                var waitSend = GetWaitSendMessageList(log.ProjectId.Value);
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
                    modelMsg.Title = CurrentUserName + "提报的监理日志单已删除";
                    modelMsg.Content = CurrentUserName + "提报的监理日志单已删除";
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = log.Id;
                    modelMsg.BussinesType = BusinessType.Log.ToString();
                    modelMsg.ProjectId = log.ProjectId.Value;
                    modelMsg.ProjectName = log.ProjectName;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                }
                #endregion

                result.Data = true;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = false;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteSupervisorlogById");
            }
            return result;
        }

        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteSupervisorLogByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_SupervisorLog>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var details = DataOperateBusiness<Epm_SupervisorLogDetails>.Get().GetList(p => ids.Contains(p.LogId ?? 0)).ToList();
                var rows = DataOperateBusiness<Epm_SupervisorLog>.Get().DeleteRange(models);
                DataOperateBusiness<Epm_SupervisorLogDetails>.Get().DeleteRange(details);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Log.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteSupervisorLogByIds");
            }
            return result;
        }

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<SupervisorLogDetailView>> GetSupervisorLogList(QueryCondition qc)
        {
            Result<List<SupervisorLogDetailView>> result = new Result<List<SupervisorLogDetailView>>();
            try
            {
                var query = from a in context.Epm_SupervisorLogDetails.Where(p => p.IsDelete == false)
                            join b in context.Epm_SupervisorLog.Where(p => p.IsDelete == false) on a.LogId equals b.Id into bref
                            from b in bref.DefaultIfEmpty()
                            select new
                            {
                                a.Id,
                                a.ProjectId,
                                a.ProjectName,
                                b.SubmitTime,
                                a.State,
                                a.Remark,
                                a.CrtCompanyId,
                                a.CrtCompanyName,
                                a.CreateUserId,
                                a.CreateUserName,
                                a.CreateTime,
                                a.OperateUserId,
                                a.OperateUserName,
                                a.OperateTime,
                                a.IsDelete,
                                a.DeleteTime,
                                a.ToResean,
                                a.FinishScale,
                                a.IsFinish,
                                b.TypeNo,
                                b.TypeName,
                                b.Content
                            };

                string projectName = "";
                if (qc != null && qc.ConditionList.Any())
                {
                    foreach (var conditionExpression in qc.ConditionList)
                    {
                        string value = (conditionExpression.ExpValue ?? "").ToString();
                        string valueName = (conditionExpression.ExpName ?? "").ToString();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            switch (valueName)
                            {
                                case "ProjectName":
                                    {
                                        projectName = value;
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }

                query = query.Where(t => (t.ProjectName.Contains(projectName) || projectName == "") && !t.IsDelete && CurrentProjectIds.Contains(t.ProjectId.ToString())).OrderByDescending(p => p.OperateTime);   //修改时间倒序

                int count = query.Count();
                int skip = (qc.PageInfo.CurrentPageIndex - 1) * qc.PageInfo.PageRowCount;
                int take = qc.PageInfo.PageRowCount;

                var list = query.OrderByDescending(p => p.SubmitTime).Skip(skip).Take(take).Select(p =>
                    new SupervisorLogDetailView
                    {
                        Id = p.Id,
                        ProjectId = p.ProjectId,
                        ProjectName = p.ProjectName,
                        SubmitTime = p.SubmitTime,
                        State = p.State,
                        Remark = p.Remark,
                        CrtCompanyId = p.CrtCompanyId,
                        CrtCompanyName = p.CrtCompanyName,
                        CreateUserId = p.CreateUserId,
                        CreateUserName = p.CreateUserName,
                        CreateTime = p.CreateTime,
                        OperateUserId = p.OperateUserId,
                        OperateUserName = p.OperateUserName,
                        OperateTime = p.OperateTime,
                        ToResean = p.ToResean,
                        FinishScale = p.FinishScale,
                        IsFinish = p.IsFinish,
                        TypeNo = p.TypeNo,
                        TypeName = p.TypeName,
                        Content = p.Content
                    }).ToList();
                result.Data = list;
                result.AllRowsCount = count;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSupervisorLogList");
            }
            return result;
        }

        /// <summary>
        /// 获取监理日志列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_SupervisorLog>> GetSupervisorLogListNew(QueryCondition qc)
        {
            Result<List<Epm_SupervisorLog>> result = new Result<List<Epm_SupervisorLog>>();
            try
            {
                if (!qc.SortList.Any())
                {
                    //注释掉根据ProjectId排序
                    //qc.SortList.Add(new SortExpression()
                    //{
                    //    SortName = "ProjectId",
                    //    SortType = eSortType.Desc
                    //});
                    qc.SortList.Add(new SortExpression()
                    {
                        SortName = "SubmitTime",
                        SortType = eSortType.Desc
                    });
                }

                result = DataOperate.QueryListSimple<Epm_SupervisorLog>(context, qc);
            }
            catch (Exception ex)
            {
                result.Flag = EResultFlag.Failure;
                result.Data = null;
                result.AllRowsCount = -1;
                result.Exception = new ExceptionEx(ex, "GetSupervisorLogList");
            }
            return result;
        }

        /// <summary>
        /// 获取监理日志详情
        /// </summary>
        /// <param name="id">监理日志 ID</param>
        /// <param name="isLoadFile">是否同时获取资源，默认获取</param>
        /// <returns></returns>
        public Result<SupervisorLogView> GetSupervisorLogModelNew(long id, bool isLoadFile = true)
        {
            Result<SupervisorLogView> result = new Result<SupervisorLogView>();
            try
            {
                Epm_SupervisorLog log = DataOperateBusiness<Epm_SupervisorLog>.Get().GetModel(id);
                if (log == null)
                {
                    throw new Exception("监理日志信息不存在！");
                }
                SupervisorLogView view = ToMapView(log);

                view.SupervisorLogCompanys = DataOperateBusiness<Epm_SupervisorLogCompany>.Get()
                    .GetList(p => p.LogId == id).ToList();
                foreach (var item in view.SupervisorLogCompanys)
                {
                    item.ProjectlLogName = DataOperateBusiness<Epm_ProjectlLogName>.Get().GetList(p => p.detailsid == item.Id).OrderBy(t => t.CreateTime).ToList();
                    item.AttendanceList = DataOperateBusiness<Epm_AttendanceList>.Get().GetList(p => p.detailsid == item.Id).OrderBy(t => t.CreateTime).ToList();
                    item.Permit = item.AttendanceList[0].permit;
                    item.BePresent = item.AttendanceList[0].bepresent;
                }

                if (isLoadFile)
                {
                    view.Attachs = DataOperateBasic<Base_Files>.Get().GetList(p => p.TableId == id).ToList();
                }

                var WorkIds = DataOperateBusiness<Epm_SupervisorLogWork>.Get().GetList(p => p.LogId == id).ToList();

                if (WorkIds.Count > 0)
                {
                    List<WorkUploadRealSceneView> sceneList = new List<WorkUploadRealSceneView>();
                    foreach (var item in WorkIds)
                    {
                        long workid = item.WorkId.Value;
                        WorkUploadRealSceneView scene = new WorkUploadRealSceneView();
                        var workModel = DataOperateBusiness<Epm_DangerousWork>.Get().GetModel(workid);
                        if (workModel != null)
                        {
                            scene.name = workModel.TaskContent;
                            scene.time = workModel.StartTime.ToString("yyyy-MM-dd");
                            scene.Attachs = DataOperateBasic<Base_Files>.Get().GetList(p => p.TableId == item.WorkUploadId && string.IsNullOrEmpty(p.ImageType) && p.TableName == "Epm_WorkUploadRealScene").ToList();

                            sceneList.Add(scene);
                        }
                    }
                    view.SenceList = sceneList;
                }
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

        ///<summary>
        ///获取监理日志详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<SupervisorLogDetailView> GetSupervisorLogModel(long id)
        {
            Result<SupervisorLogDetailView> result = new Result<SupervisorLogDetailView>();
            try
            {
                var model = DataOperateBusiness<Epm_SupervisorLogDetails>.Get().GetModel(id);
                if (model == null)
                {
                    throw new Exception("信息不存在或已被删除！");
                }

                var log = DataOperateBusiness<Epm_SupervisorLog>.Get().GetModel(model.LogId.Value);
                if (log == null)
                {
                    throw new Exception("信息不存在或已被删除！");
                }
                var planId = model.PlanId.Value;

                var plan = DataOperateBusiness<Epm_Plan>.Get().GetModel(planId);
                SupervisorLogDetailView data = new SupervisorLogDetailView();

                if (plan != null)
                {
                    var projectData = DataOperateBusiness<Epm_ProjectData>.Get().GetList(t => t.ProjectId == log.ProjectId && t.MilepostId == plan.MilepostId).ToList();
                    if (projectData.Count() > 0)
                    {
                        data.ProjectData = projectData;
                    }
                }

                data.Id = model.Id;
                data.ProjectId = model.ProjectId;
                data.ProjectName = model.ProjectName;
                data.SubmitTime = log.SubmitTime;
                data.State = model.State;
                data.Remark = model.Remark;
                data.CrtCompanyId = model.CrtCompanyId;
                data.CrtCompanyName = model.CrtCompanyName;
                data.CreateUserId = model.CreateUserId;
                data.CreateUserName = model.CreateUserName;
                data.CreateTime = model.CreateTime;
                data.OperateUserId = model.OperateUserId;
                data.OperateUserName = model.OperateUserName;
                data.OperateTime = model.OperateTime;
                data.TypeNo = log.TypeNo;
                data.TypeName = log.TypeName;
                data.Content = log.Content;
                data.StartTime = model.StartTime;
                data.EndTime = model.EndTime;
                data.IsFinish = model.IsFinish;
                data.ToResean = model.ToResean;
                data.FinishScale = model.FinishScale;
                data.PlanId = model.PlanId;
                data.PlanName = model.PlanName;
                result.Data = data;
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

        ///<summary>
        ///获取监理日志详情列表
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_SupervisorLogDetails>> GetSupervisorLogDetailsList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_SupervisorLogDetails>> result = new Result<List<Epm_SupervisorLogDetails>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_SupervisorLogDetails>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSupervisorLogDetailsList");
            }
            return result;
        }

        /// <summary>
        /// 将视图转换为模型
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        private Epm_SupervisorLog ToMapModel(SupervisorLogView view)
        {
            Epm_SupervisorLog model = new Epm_SupervisorLog();
            if (view != null)
            {
                model.Id = view.Id;
                model.ProjectId = view.ProjectId;
                model.ProjectName = view.ProjectName;
                model.SubmitTime = view.SubmitTime;
                model.TypeNo = view.TypeNo;
                model.TypeName = view.TypeName;
                model.Content = view.Content;
                model.State = view.State;
                model.Remark = view.Remark;
                model.CrtCompanyId = view.CrtCompanyId;
                model.CrtCompanyName = view.CrtCompanyName;
                model.WindPower = view.WindPower;
                model.Temperature = view.Temperature;
                model.PlanId = view.PlanId;
                model.PlanName = view.PlanName;
                model.PlanState = view.planState;
                model.NextPlanId = view.nextPlanId;
                model.NextPlanName = view.nextPlanName;
                model.TomorrowProject = view.TomorrowProject;
                model.Schedule = view.Schedule;
                model.Reason = view.Reason;
                model.WorkId = view.WorkId;
            }
            return model;
        }

        /// <summary>
        /// 将视图转换为模型(xin)
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        private Epm_ProjectlLogName ToMapModela(Epm_ProjectlLogName view)
        {
            Epm_ProjectlLogName model = new Epm_ProjectlLogName();
            if (view != null)
            {
                model.Id = view.Id;
                model.projectid = view.projectid;
                model.name = view.name;
                model.type = view.type;
                model.phone = view.phone;
                model.detailsid = view.detailsid;
                model.personid = view.personid;
            }
            return model;
        }

        /// <summary>
        /// 将模型转换为视图
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private SupervisorLogView ToMapView(Epm_SupervisorLog model)
        {
            SupervisorLogView view = new SupervisorLogView();
            if (model != null)
            {
                view.Id = model.Id;
                view.ProjectId = model.ProjectId;
                view.ProjectName = model.ProjectName;
                view.SubmitTime = model.SubmitTime;
                view.TypeNo = model.TypeNo;
                view.TypeName = model.TypeName;
                view.Content = model.Content;
                view.State = model.State;
                view.Remark = model.Remark;
                view.CrtCompanyId = model.CrtCompanyId;
                view.CrtCompanyName = model.CrtCompanyName;
                view.WindPower = model.WindPower;
                view.Temperature = model.Temperature;
                view.PlanId = model.PlanId;
                view.PlanName = model.PlanName;
                view.planState = model.PlanState;
                view.nextPlanId = model.NextPlanId;
                view.nextPlanName = model.NextPlanName;
                view.TomorrowProject = model.TomorrowProject;
                view.Schedule = model.Schedule;
                view.Reason = model.Reason;
                view.WorkId = model.WorkId;
                view.CreateUserId = model.CreateUserId;
                view.CreateTime = model.CreateTime;
                view.CreateUserName = model.CreateUserName;
                view.OperateUserId = model.OperateUserId;
                view.OperateUserName = model.OperateUserName;
                view.OperateTime = model.OperateTime;
            }
            return view;
        }

        /// <summary>
        /// 审核监理日志
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AuditSupervisorLog(Epm_SupervisorLog model)
        {
            Result<int> result = new Result<int>();
            try
            {
                if (model == null)
                {
                    throw new Exception("请选择要审核的监理日志！");
                }
                Epm_SupervisorLog temp = DataOperateBusiness<Epm_SupervisorLog>.Get().GetModel(model.Id);
                if (temp == null)
                {
                    throw new Exception("要审核的监理日志不存在！");
                }
                temp.State = model.State;
                SetCurrentUser(temp);

                DataOperateBusiness<Epm_SupervisorLog>.Get().Update(temp);

                //处理待办
                var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == temp.Id && t.IsApprover == false).FirstOrDefault();
                if (tempApp != null)
                {
                    ComplateApprover(tempApp.Id);
                }
                string title = "";
                if (model.State == (int)ApprovalState.ApprSuccess)
                {
                    //修改危险作业状态
                    var workIDs = DataOperateBusiness<Epm_SupervisorLogWork>.Get().GetList(t => t.LogId == model.Id).ToList();

                    if (workIDs.Count > 0)
                    {
                        for (int i = 0; i < workIDs.Count; i++)
                        {
                            Epm_DangerousWork work = DataOperateBusiness<Epm_DangerousWork>.Get().GetModel(workIDs[i].WorkId.Value);
                            //处理待办
                            var tempApp1 = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == work.Id && t.IsApprover == false).FirstOrDefault();
                            if (tempApp1 != null)
                            {
                                ComplateApprover(tempApp.Id);
                            }
                            if (work.State == (int)ApprovalState.WaitAppr)
                            {
                                work.State = (int)ApprovalState.ApprSuccess;
                                //SetCurrentUser(work);
                                //DataOperateBusiness<Epm_DangerousWork>.Get().Update(work);

                                UpdateDangerousWorkState(work.Id, ApprovalState.ApprSuccess);
                            }
                            else if (work.State == (int)ApprovalState.WorkPartAppr)
                            {
                                work.State = (int)ApprovalState.WorkFinish;
                                SetCurrentUser(work);
                                DataOperateBusiness<Epm_DangerousWork>.Get().Update(work);

                                UpdateWorkRealScenenState(work.Id, ApprovalState.ApprSuccess);
                            }
                        }
                    }
                    //修改危险作业状态
                    //if (temp.WorkId.HasValue && temp.WorkId.Value != 0)
                    //{
                    //    Epm_DangerousWork work = DataOperateBusiness<Epm_DangerousWork>.Get().GetModel(temp.WorkId.Value);
                    //    if (work.State == (int)ApprovalState.WaitAppr)
                    //    {
                    //        work.State = (int)ApprovalState.ApprSuccess;
                    //        SetCurrentUser(work);
                    //        DataOperateBusiness<Epm_DangerousWork>.Get().Update(work);
                    //    }
                    //    else if (work.State == (int)ApprovalState.WorkPartAppr)
                    //    {
                    //        work.State = (int)ApprovalState.WorkFinish;
                    //        SetCurrentUser(work);
                    //        DataOperateBusiness<Epm_DangerousWork>.Get().Update(work);

                    //        UpdateWorkRealScenenState(temp.WorkId.Value, ApprovalState.ApprSuccess);
                    //    }
                    //}

                    // 如果选择了施工计划，表示该施工计划已完成
                    if (!string.IsNullOrEmpty(temp.NextPlanId))
                    {
                        List<Epm_Plan> planList = DataOperateBusiness<Epm_Plan>.Get().GetList(t => temp.NextPlanId.Contains(t.Id.ToString())).ToList();

                        foreach (var item in planList)
                        {
                            item.IsFinish = 1;

                            DateTime start = Convert.ToDateTime(item.StartDate);
                            DateTime end = Convert.ToDateTime(item.EndDate);
                            TimeSpan sp = end.Subtract(start);
                            int days = sp.Days + 1;

                            item.BuildDays = days > 0 ? days : 0;

                            SetCurrentUser(item);
                        }
                        DataOperateBusiness<Epm_Plan>.Get().UpdateRange(planList);
                    }
                    title = temp.CreateUserName + "提报了监理日志单，审核通过";

                    #region 生成待办
                    var projectList = DataOperateBusiness<Epm_Project>.Get().GetModel(temp.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = temp.CreateUserName + "提报了监理日志单，审核通过";
                    app.Content = temp.CreateUserName + "提报了监理日志单，审核通过";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Log.ToString();
                    app.Action = SystemRight.Check.ToString();
                    app.BusinessTypeName = BusinessType.Log.GetText();
                    app.BusinessState = (int)(ApprovalState.ApprSuccess);
                    app.BusinessId = temp.Id;
                    app.ApproverId = temp.CreateUserId;
                    app.ApproverName = temp.CreateUserName;
                    app.ProjectId = projectList.Id;
                    app.ProjectName = projectList.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Log.GetText(), SystemRight.Add.GetText(), "提交监理日志生成待办: " + temp.Id);
                    #endregion
                }
                else if (model.State == (int)ApprovalState.ApprFailure)
                {
                    title = temp.CreateUserName + "提报了监理日志单，已驳回";

                    #region 生成待办
                    var projectList = DataOperateBusiness<Epm_Project>.Get().GetModel(temp.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = temp.CreateUserName + "提报了监理日志单，已驳回";
                    app.Content = temp.CreateUserName + "提报了监理日志单，已驳回";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Log.ToString();
                    app.Action = SystemRight.UnCheck.ToString();
                    app.BusinessTypeName = BusinessType.Log.GetText();
                    app.BusinessState = (int)(ApprovalState.ApprFailure);
                    app.BusinessId = temp.Id;
                    app.ApproverId = temp.CreateUserId;
                    app.ApproverName = temp.CreateUserName;
                    app.ProjectId = projectList.Id;
                    app.ProjectName = projectList.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Log.GetText(), SystemRight.Add.GetText(), "提交监理日志已驳回生成消息: " + temp.Id);
                    #endregion
                }
                else if (model.State == (int)ApprovalState.Discarded)
                {
                    title = temp.CreateUserName + "提报了监理日志单，已废弃";

                    #region 生成待办
                    var projectList = DataOperateBusiness<Epm_Project>.Get().GetModel(temp.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = temp.CreateUserName + "提报了监理日志单，已废弃";
                    app.Content = temp.CreateUserName + "提报了监理日志单，已废弃";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Log.ToString();
                    app.Action = SystemRight.Invalid.ToString();
                    app.BusinessTypeName = BusinessType.Log.GetText();
                    app.BusinessState = (int)(ApprovalState.Discarded);
                    app.BusinessId = temp.Id;
                    app.ApproverId = projectList.ContactUserId;
                    app.ApproverName = projectList.ContactUserName;
                    app.ProjectId = projectList.Id;
                    app.ProjectName = projectList.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Log.GetText(), SystemRight.Invalid.GetText(), "已废弃监理日志生成待办: " + temp.Id);
                    #endregion
                }

                #region 消息
                var waitSend = GetWaitSendMessageList(temp.ProjectId.Value);
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
                    modelMsg.BussinessId = temp.Id;
                    modelMsg.BussinesType = BusinessType.Log.ToString();
                    modelMsg.ProjectId = temp.ProjectId.Value;
                    modelMsg.ProjectName = temp.ProjectName;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                }
                #endregion

                result.Data = 1;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AuditSupervisorLog");
            }
            return result;
        }

        /// <summary>
        /// 新增监理日志（新）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddProjectlLogList(SupervisorLogView model, List<long> workIdList)
        {
            Result<int> result = new Result<int>();
            try
            {
                if (model == null)
                {
                    throw new Exception("请填写监理日志相关信息！");
                }
                Epm_SupervisorLog log = ToMapModel(model);
                //Epm_ProjectlLogName logs = ToMapModela(model);

                if (!string.IsNullOrEmpty(model.PlanId))
                {
                    var planList = model.PlanId.Split(',');
                    foreach (var item in planList)
                    {
                        var plan = DataOperateBusiness<Epm_Plan>.Get().GetModel(item.ToLongReq());
                        plan.StartDate = log.SubmitTime.HasValue ? log.SubmitTime.Value : DateTime.Now;
                        DataOperateBusiness<Epm_Plan>.Get().Update(plan);
                    }
                }

                if (!string.IsNullOrEmpty(model.nextPlanId))
                {
                    var nextplanList = model.nextPlanId.Split(',');
                    foreach (var item in nextplanList)
                    {
                        var plan = DataOperateBusiness<Epm_Plan>.Get().GetModel(item.ToLongReq());
                        plan.EndDate = log.SubmitTime.HasValue ? log.SubmitTime.Value : DateTime.Now;
                        DataOperateBusiness<Epm_Plan>.Get().Update(plan);
                    }
                }

                SetCreateUser(log);
                SetCurrentUser(log);
                log.IsDelete = false;

                if ((string.IsNullOrEmpty(log.NextPlanId)) && (workIdList == null || workIdList.Count == 0) && (!model.SenceList.Any() || model.SenceList.Count == 0))
                {
                    log.State = (int)ApprovalState.ApprSuccess;
                }
                else
                {
                    var temp = DataOperateBusiness<Epm_SupervisorLog>.Get().GetList(t => t.ProjectId == log.ProjectId && t.PlanId == log.PlanId && t.State != (int)ApprovalState.ApprFailure).FirstOrDefault();
                    if (temp != null)
                    {
                        throw new Exception("所选工程节点计划已提交完成申请，不可重复提交！");
                    }

                    log.State = (int)ApprovalState.WaitAppr;
                }

                DataOperateBusiness<Epm_SupervisorLog>.Get().Add(log);
                List<Epm_ProjectlLogName> epmLog = new List<Epm_ProjectlLogName>();
                List<Epm_AttendanceList> aeList = new List<Epm_AttendanceList>();
                if (model.SupervisorLogCompanys != null && model.SupervisorLogCompanys.Any())
                {
                    foreach (var item in model.SupervisorLogCompanys)
                    {
                        item.LogId = log.Id;
                        item.ProjectId = log.ProjectId ?? 0;
                        SetCreateUser(item);
                        SetCurrentUser(item);
                        item.IsDelete = false;

                        if (item.ProjectlLogName.Any())
                        {
                            foreach (var temp in item.ProjectlLogName)
                            {
                                temp.detailsid = item.Id;
                                SetCreateUser(temp);
                                SetCurrentUser(temp);
                                temp.IsDelete = false;
                                epmLog.Add(temp);
                            }

                        }
                        if (item.AttendanceList.Any())
                        {
                            foreach (var temps in item.AttendanceList)
                            {
                                temps.detailsid = item.Id;
                                temps.permit = item.Permit = true ? "0" : "1";
                                temps.bepresent = item.BePresent = true ? "0" : "1";
                                SetCreateUser(temps);
                                SetCurrentUser(temps);
                                temps.IsDelete = false;
                                aeList.Add(temps);
                            }
                        }
                    }
                    DataOperateBusiness<Epm_SupervisorLogCompany>.Get().AddRange(model.SupervisorLogCompanys);
                    DataOperateBusiness<Epm_ProjectlLogName>.Get().AddRange(epmLog);
                    DataOperateBusiness<Epm_AttendanceList>.Get().AddRange(aeList);
                }

                //附件
                if (model.Attachs.Any())
                {
                    AddFilesByTable(log, model.Attachs);
                }

                //上传实景作业 PC
                if (model.SenceList.Any() && model.SenceList.Count > 0)
                {
                    List<Epm_SupervisorLogWork> logWorkList = new List<Epm_SupervisorLogWork>();
                    Epm_SupervisorLogWork logWork = null;
                    foreach (var item in model.SenceList)
                    {
                        if (item.Type == 2)
                        {
                            #region 添加实景
                            Epm_WorkUploadRealScene scene = new Epm_WorkUploadRealScene();
                            scene.ProjectId = model.ProjectId;
                            scene.WorkId = item.Id;
                            scene.UploadTime = DateTime.Now;
                            scene.State = (int)ApprovalState.WaitAppr;
                            AddWorkRealScenen(scene, item.Attachs);
                            #endregion

                            #region 添加监理日志关联作业实景
                            logWork = new Epm_SupervisorLogWork();
                            logWork.LogId = log.Id;
                            logWork.WorkId = item.Id;
                            logWork.WorkUploadId = scene.Id;
                            logWork.State = (int)ApprovalState.WaitAppr;
                            SetCreateUser(logWork);
                            SetCurrentUser(logWork);
                            DataOperateBusiness<Epm_SupervisorLogWork>.Get().Add(logWork);
                            #endregion
                        }
                        else
                        {
                            #region 添加监理日志关联作业实景
                            logWork = new Epm_SupervisorLogWork();
                            logWork.LogId = log.Id;
                            logWork.WorkId = item.Id;
                            logWork.WorkUploadId = 0;
                            logWork.State = (int)ApprovalState.WaitAppr;
                            SetCreateUser(logWork);
                            SetCurrentUser(logWork);
                            DataOperateBusiness<Epm_SupervisorLogWork>.Get().Add(logWork);
                            #endregion
                        }
                    }
                }
                //上传实景作业 APP
                if (workIdList != null && workIdList.Count > 0)
                {
                    List<Epm_SupervisorLogWork> logWorkList = new List<Epm_SupervisorLogWork>();
                    Epm_SupervisorLogWork logWork = null;
                    foreach (var workId in workIdList)
                    {
                        var workScene = DataOperateBusiness<Epm_WorkUploadRealScene>.Get().GetList(p => p.WorkId == workId).FirstOrDefault();
                        if (workScene != null)
                        {
                            logWork = new Epm_SupervisorLogWork();
                            logWork.LogId = log.Id;
                            logWork.WorkId = workScene.WorkId;
                            logWork.WorkUploadId = workScene.Id;
                            logWork.State = (int)ApprovalState.WaitAppr;
                            SetCreateUser(logWork);
                            SetCurrentUser(logWork);
                            logWorkList.Add(logWork);
                        }
                        else
                        {
                            logWork = new Epm_SupervisorLogWork();
                            logWork.LogId = log.Id;
                            logWork.WorkId = workId;
                            logWork.WorkUploadId = 0;
                            logWork.State = (int)ApprovalState.WaitAppr;
                            SetCreateUser(logWork);
                            SetCurrentUser(logWork);
                            logWorkList.Add(logWork);
                        }
                    }
                    DataOperateBusiness<Epm_SupervisorLogWork>.Get().AddRange(logWorkList);
                }

                if (log.State == (int)ApprovalState.WaitAppr)
                {
                    #region 生成待办
                    var projectList = DataOperateBusiness<Epm_Project>.Get().GetModel(log.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "提报了监理日志单，待审核";
                    app.Content = CurrentUserName + "提报了监理日志单，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Log.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Log.GetText();
                    app.BusinessState = (int)(ApprovalState.WaitAppr);
                    app.BusinessId = log.Id;
                    app.ApproverId = projectList.ContactUserId;
                    app.ApproverName = projectList.ContactUserName;
                    app.ProjectId = projectList.Id;
                    app.ProjectName = projectList.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Log.GetText(), SystemRight.Add.GetText(), "提交监理日志生成待办: " + log.Id);
                    #endregion

                    #region 消息
                    var waitSend = GetWaitSendMessageList(model.ProjectId.Value);
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
                        modelMsg.Title = CurrentUserName + "提报了监理日志单，待审核";
                        modelMsg.Content = CurrentUserName + "提报了监理日志单，待审核";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Log.ToString();
                        modelMsg.ProjectId = model.ProjectId.Value;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion

                    #region 发送短信
                    //Dictionary<string, string> parameterssm = new Dictionary<string, string>();
                    //parameterssm.Add("UserName", CurrentUserName);
                    //WriteSMS(projectList.ContactUserId.Value, projectList.CompanyId, MessageStep.SupervisorLogAdd, parameterssm);
                    #endregion
                }
                else {

                    #region 生成待办
                    var projectList = DataOperateBusiness<Epm_Project>.Get().GetModel(log.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "提报了监理日志单，请查看";
                    app.Content = CurrentUserName + "提报了监理日志单，请查看";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Log.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Log.GetText();
                    app.BusinessState = (int)(ApprovalState.ApprSuccess);
                    app.BusinessId = log.Id;
                    app.ApproverId = projectList.ContactUserId;
                    app.ApproverName = projectList.ContactUserName;
                    app.ProjectId = projectList.Id;
                    app.ProjectName = projectList.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Log.GetText(), SystemRight.Add.GetText(), "提交监理日志生成待办: " + log.Id);
                    #endregion

                    #region 消息
                    var waitSend = GetWaitSendMessageList(model.ProjectId.Value);
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
                        modelMsg.Title = CurrentUserName + "提报了监理日志单，请查看";
                        modelMsg.Content = CurrentUserName + "提报了监理日志单，请查看";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Log.ToString();
                        modelMsg.ProjectId = model.ProjectId.Value;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion
                }
                result.Data = 1;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Log.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddSupervisorLog");
            }
            return result;
        }
    }
}
