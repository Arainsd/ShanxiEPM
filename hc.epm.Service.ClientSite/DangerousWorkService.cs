using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddDangerousWork(Epm_DangerousWork model, List<Base_Files> files)
        {
            Result<int> result = new Result<int>();
            try
            {
                model = SetCurrentUser(model);
                model.SubmitCompanyId = CurrentCompanyID.ToLongReq();
                model.SubmitCompanyName = CurrentCompanyName;
                model.SubmitUserId = CurrentUserID.ToLongReq();
                model.SubmitUserName = CurrentUserName;
                model.CrtCompanyId = CurrentCompanyID.ToLongReq();
                model.CrtCompanyName = CurrentCompanyName;

                var rows = DataOperateBusiness<Epm_DangerousWork>.Get().Add(model);

                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = new PageListInfo()
                    {
                        isAllowPage = false
                    }
                };
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ProjectId",
                    ExpValue = model.ProjectId,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "UploadTime",
                    ExpValue = DateTime.Today,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });

                // 关联危险作业实景
                var workRealScenenResult = GetWorkRealSceneList(qc);
                if (workRealScenenResult.Flag == EResultFlag.Success && workRealScenenResult.Data != null)
                {
                    List<Epm_SupervisorLogWork> workList = workRealScenenResult.Data.Select(p =>
                        new Epm_SupervisorLogWork
                        {
                            WorkId = p.WorkId,
                            LogId = model.Id,
                            WorkUploadId = p.Id,
                            State = p.State
                        }).ToList();

                    workList.ForEach(p =>
                    {
                        SetCurrentUser(p);
                        SetCreateUser(p);
                    });

                    DataOperateBusiness<Epm_SupervisorLogWork>.Get().AddRange(workList);
                }

                AddFilesByTable(model, files);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Dangerous.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);

                if (model.State == (int)ApprovalState.WaitAppr)
                {
                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "提报了危险作业单，待审核";
                    app.Content = CurrentUserName + "提报了危险作业单，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Dangerous.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Dangerous.GetText();
                    app.BusinessState = (int)(ApprovalState.WaitAppr);
                    app.BusinessId = model.Id;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Dangerous.GetText(), SystemRight.Add.GetText(), "提交危险作业生成待办: " + model.Id);
                    #endregion

                    #region 消息
                    //var waitSend = GetWaitSendMessageList(model.ProjectId.Value);
                    //foreach (var send in waitSend)
                    //{
                    //    Epm_Massage modelMsg = new Epm_Massage();
                    //    modelMsg.ReadTime = null;
                    //    modelMsg.RecId = send.Key;
                    //    modelMsg.RecName = send.Value;
                    //    modelMsg.RecTime = DateTime.Now;
                    //    modelMsg.SendId = CurrentUserID.ToLongReq();
                    //    modelMsg.SendName = CurrentUserName;
                    //    modelMsg.SendTime = DateTime.Now;
                    //    modelMsg.Title = CurrentUserName + "提报了危险作业单，待审核";
                    //    modelMsg.Content = CurrentUserName + "提报了危险作业单，待审核";
                    //    modelMsg.Type = 2;
                    //    modelMsg.IsRead = false;
                    //    modelMsg.BussinessId = model.Id;
                    //    modelMsg.BussinesType = BusinessType.Dangerous.ToString();
                    //    modelMsg.ProjectId = model.ProjectId.Value;
                    //    modelMsg.ProjectName = model.ProjectName;
                    //    modelMsg = base.SetCurrentUser(modelMsg);
                    //    modelMsg = base.SetCreateUser(modelMsg);
                    //    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    //}
                    #endregion

                    #region 发送短信
                    //Dictionary<string, string> parameterSms = new Dictionary<string, string>();
                    //parameterSms.Add("UserName", CurrentUserName);
                    //WriteSMS(project.ContactUserId.Value, project.CompanyId, MessageStep.WorkAdd, parameterSms);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddDangerousWork");
            }
            return result;
        }

        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateDangerousWork(Epm_DangerousWork model, List<Base_Files> files)
        {
            Result<int> result = new Result<int>();
            try
            {
                var oldModel = DataOperateBusiness<Epm_DangerousWork>.Get().GetModel(model.Id);
                model = FiterUpdate(oldModel, model);
                model.SubmitCompanyId = oldModel.SubmitCompanyId;
                model.SubmitCompanyName = oldModel.SubmitCompanyName;
                model.SubmitUserId = oldModel.SubmitUserId;
                model.SubmitUserName = oldModel.SubmitUserName;
                model.CrtCompanyId = oldModel.CrtCompanyId;
                model.CrtCompanyName = oldModel.CrtCompanyName;

                var rows = DataOperateBusiness<Epm_DangerousWork>.Get().Update(model);
                //AddFilesByTable(model, files);

                //删除之前的附件
                DeleteFilesByTable(model.GetType().Name, new List<long>() { model.Id });
                //新增附件
                AddFilesByTable(model, files);

                //处理待办
                var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == model.Id && t.IsApprover == false).FirstOrDefault();
                if (tempApp != null)
                {
                    ComplateApprover(tempApp.Id);
                }

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Dangerous.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);

                if (model.State == (int)ApprovalState.WaitAppr)
                {
                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "提报了危险作业单，待审核";
                    app.Content = CurrentUserName + "提报了危险作业单，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Dangerous.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Dangerous.GetText();
                    app.BusinessState = (int)(ApprovalState.WaitAppr);
                    app.BusinessId = model.Id;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Dangerous.GetText(), SystemRight.Add.GetText(), "提交危险作业生成待办: " + model.Id);
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
                        modelMsg.Title = CurrentUserName + "提报了危险作业单，待审核";
                        modelMsg.Content = CurrentUserName + "提报了危险作业单，待审核";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Dangerous.ToString();
                        modelMsg.ProjectId = model.ProjectId.Value;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion

                    #region 发送短信
                    //Dictionary<string, string> parameterSms = new Dictionary<string, string>();
                    //parameterSms.Add("UserName", CurrentUserName);
                    //WriteSMS(project.ContactUserId.Value, project.CompanyId, MessageStep.WorkAdd, parameterSms);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateDangerousWork");
            }
            return result;
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<int> DeleteDangerousWork(long id)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBusiness<Epm_DangerousWork>.Get().GetModel(id);
                model.IsDelete = true;
                model.DeleteTime = DateTime.Now;

                //处理待办
                var temp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == model.Id && t.IsApprover == false).FirstOrDefault();
                if (temp != null)
                {
                    ComplateApprover(temp.Id);
                }

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
                    modelMsg.Title = CurrentUserName + "提报的危险作业单已删除";
                    modelMsg.Content = CurrentUserName + "提报的危险作业单已删除";
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = model.Id;
                    modelMsg.BussinesType = BusinessType.Dangerous.ToString();
                    modelMsg.ProjectId = model.ProjectId;
                    modelMsg.ProjectName = model.ProjectName;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                }
                #endregion

                var rows = DataOperateBusiness<Epm_DangerousWork>.Get().Delete(model);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Dangerous.GetText(), SystemRight.Delete.GetText(), "删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteDangerousWork");
            }
            return result;
        }

        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteDangerousWorkByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_DangerousWork>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                foreach (var model in models)
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
                        modelMsg.Title = CurrentUserName + "提报的危险作业单已删除";
                        modelMsg.Content = CurrentUserName + "提报的危险作业单已删除";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Dangerous.ToString();
                        modelMsg.ProjectId = model.ProjectId;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion
                }
                var rows = DataOperateBusiness<Epm_DangerousWork>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Dangerous.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteDangerousWorkByIds");
            }
            return result;
        }

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_DangerousWork>> GetDangerousWorkList(QueryCondition qc)
        {
            qc = AddDefaultWeb(qc);
            Result<List<Epm_DangerousWork>> result = new Result<List<Epm_DangerousWork>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_DangerousWork>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetDangerousWorkList");
            }
            return result;
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_DangerousWork> GetDangerousWorkModel(long id)
        {
            Result<Epm_DangerousWork> result = new Result<Epm_DangerousWork>();
            try
            {
                var model = DataOperateBusiness<Epm_DangerousWork>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetDangerousWorkModel");
            }
            return result;
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateDangerousWorkState(long id, ApprovalState state)
        {
            int preState;
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBusiness<Epm_DangerousWork>.Get().GetModel(id);

                preState = model.State.Value;
                if (preState == (int)ApprovalState.WorkPartAppr)
                {
                    if (state == ApprovalState.ApprFailure)
                    {
                        state = ApprovalState.ApprSuccess;
                    }
                    else if (state == ApprovalState.ApprSuccess)
                    {
                        state = ApprovalState.WorkFinish;
                    }
                }
                model.State = (int)state;
                model.OperateUserId = CurrentUserID.ToLongReq();
                model.OperateUserName = CurrentUserName;
                model.OperateTime = DateTime.Now;
                var rows = DataOperateBusiness<Epm_DangerousWork>.Get().Update(model);

                if (state != ApprovalState.WorkPartAppr)
                {
                    //处理待办
                    var temp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == model.Id && t.IsApprover == false).FirstOrDefault();
                    if (temp != null)
                    {
                        ComplateApprover(temp.Id);
                    }
                }
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Dangerous.GetText(), SystemRight.Delete.GetText(), "更新状态: " + rows);

                if (preState == (int)ApprovalState.WorkPartAppr)
                {
                    var sence = DataOperateBusiness<Epm_WorkUploadRealScene>.Get().GetList(t => t.WorkId == model.Id && t.State == (int)ApprovalState.WaitAppr).FirstOrDefault();
                    if (sence != null)
                    {
                        if (state == ApprovalState.WorkFinish)
                        {
                            sence.State = (int)ApprovalState.ApprSuccess;
                        }
                        else
                        {
                            sence.State = (int)ApprovalState.ApprFailure;
                        }
                    }

                    DataOperateBusiness<Epm_WorkUploadRealScene>.Get().Update(sence);
                }
                string title = CurrentUserName + "上传了危险作业实景，待审核";

                if (state == ApprovalState.ApprSuccess && preState == (int)ApprovalState.WaitAppr)
                {
                    title = model.CreateUserName + "提报的危险作业单已审核通过";
                    #region 生成待办
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = model.CreateUserName + "提报的危险作业单已审核通过";
                    app.Content = model.CreateUserName + "提报的危险作业单已审核通过";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = model.CreateTime;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Dangerous.ToString();
                    app.Action = SystemRight.Check.ToString();
                    app.BusinessTypeName = BusinessType.Dangerous.GetText();
                    app.BusinessState = (int)(ApprovalState.ApprSuccess);
                    app.BusinessId = model.Id;
                    app.ApproverId = model.CreateUserId;
                    app.ApproverName = model.CreateUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = model.ProjectName;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Dangerous.GetText(), SystemRight.UnCheck.GetText(), "审核通过危险作业生成待办: " + model.Id);
                    #endregion

                }
                else if (state == ApprovalState.ApprSuccess && preState == (int)ApprovalState.WorkPartAppr)
                {
                    title = model.CreateUserName + "上传的作业实景，审核不通过";
                    #region 生成待办
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = model.CreateUserName + "上传的作业实景，审核不通过";
                    app.Content = model.CreateUserName + "上传的作业实景，审核不通过";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = model.CreateTime;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Dangerous.ToString();
                    app.Action = SystemRight.UnCheck.ToString();
                    app.BusinessTypeName = BusinessType.Dangerous.GetText();
                    app.BusinessState = (int)(ApprovalState.ApprFailure);
                    app.BusinessId = model.Id;
                    app.ApproverId = model.CreateUserId;
                    app.ApproverName = model.CreateUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = model.ProjectName;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Dangerous.GetText(), SystemRight.UnCheck.GetText(), "审核通过危险作业生成待办: " + model.Id);
                    #endregion
                }
                else if (state == ApprovalState.WorkFinish)
                {
                    title = model.CreateUserName + "提报的危险作业单，已完成";
                    #region 生成待办
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = model.CreateUserName + "提报的危险作业单，已完成";
                    app.Content = model.CreateUserName + "提报的危险作业单，已完成";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = model.CreateTime;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Dangerous.ToString();
                    app.Action = SystemRight.Check.ToString();
                    app.BusinessTypeName = BusinessType.Dangerous.GetText();
                    app.BusinessState = (int)(ApprovalState.WorkFinish);
                    app.BusinessId = model.Id;
                    app.ApproverId = model.CreateUserId;
                    app.ApproverName = model.CreateUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = model.ProjectName;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Dangerous.GetText(), SystemRight.UnCheck.GetText(), "审核通过危险作业生成待办: " + model.Id);
                    #endregion
                }
                else if (state == ApprovalState.Discarded)
                {
                    title = model.CreateUserName + "提报的危险作业单已废弃，请处理";
                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = model.CreateUserName + "提报的危险作业单已废弃，请处理";
                    app.Content = model.CreateUserName + "提报的危险作业单已废弃，请处理";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Dangerous.ToString();
                    app.Action = SystemRight.Invalid.ToString();
                    app.BusinessTypeName = BusinessType.Dangerous.GetText();
                    app.BusinessState = (int)(ApprovalState.Discarded);
                    app.BusinessId = model.Id;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Dangerous.GetText(), SystemRight.Invalid.GetText(), "废弃危险作业生成待办: " + model.Id);
                    #endregion

                    #region 发送短信
                    //WriteSMS(model.CreateUserId, 0, MessageStep.WorkReject, null);
                    #endregion
                }
                else if (state == ApprovalState.ApprFailure)
                {
                    title = model.CreateUserName + "提报的危险作业单已被驳回，请处理";
                    #region 生成待办
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = model.CreateUserName + "提报的危险作业单已被驳回，请处理";
                    app.Content = model.CreateUserName + "提报的危险作业单已被驳回，请处理";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = model.CreateTime;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Dangerous.ToString();
                    app.Action = SystemRight.UnCheck.ToString();
                    app.BusinessTypeName = BusinessType.Dangerous.GetText();
                    app.BusinessState = (int)(ApprovalState.ApprFailure);
                    app.BusinessId = model.Id;
                    app.ApproverId = model.CreateUserId;
                    app.ApproverName = model.CreateUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = model.ProjectName;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Dangerous.GetText(), SystemRight.UnCheck.GetText(), "驳回危险作业生成待办: " + model.Id);
                    #endregion

                    #region 发送短信
                    //WriteSMS(model.CreateUserId, 0, MessageStep.WorkReject, null);
                    #endregion
                }

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
                    modelMsg.Title = title;
                    modelMsg.Content = title;
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = model.Id;
                    modelMsg.BussinesType = BusinessType.Dangerous.ToString();
                    modelMsg.ProjectId = model.ProjectId.Value;
                    modelMsg.ProjectName = model.ProjectName;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                }
                #endregion
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateDangerousWorkState");
            }
            return result;
        }
    }
}