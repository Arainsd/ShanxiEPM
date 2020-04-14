using System;
using System.Collections.Generic;
using System.Linq;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.Plat.Common.Global;
using hc.Plat.Common.Service;
using hc.epm.Common;
using hc.Plat.Common.Extend;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        #region 作业实景

        /// <summary>
        /// 获取作业实景列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_WorkUploadRealScene>> GetWorkRealSceneList(QueryCondition qc)
        {
            Result<List<Epm_WorkUploadRealScene>> result = new Result<List<Epm_WorkUploadRealScene>>();
            try
            {
                result = DataOperate.QueryListSimple<Epm_WorkUploadRealScene>(context, qc);
            }
            catch (Exception ex)
            {
                result.Exception = new ExceptionEx(ex, "GetWorkRealSceneList");
                result.Flag = EResultFlag.Failure;
                result.Data = null;
                result.AllRowsCount = -1;
            }
            return result;
        }

        /// <summary>
        /// 上传危险作业实景
        /// </summary>
        /// <param name="model">作业实景</param>
        /// <param name="files">相关附件</param>
        /// <returns></returns>
        public Result<bool> AddWorkRealScenen(Epm_WorkUploadRealScene model, List<Base_Files> files)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                if (model == null)
                {
                    throw new Exception("请上传危险作业实景！");
                }

                SetCurrentUser(model);
                SetCreateUser(model);

                DataOperateBusiness<Epm_WorkUploadRealScene>.Get().Add(model);

                if (files.Any())
                {
                    AddFilesByTable(model, files);
                }
                // TODO: 生成待办
                if (model.State == (int)ApprovalState.WaitAppr)
                {
                    //处理待办
                    var temp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == model.WorkId && t.IsApprover == false).FirstOrDefault();
                    if (temp != null)
                    {
                        ComplateApprover(temp.Id);
                    }

                    var work = DataOperateBusiness<Epm_DangerousWork>.Get().GetModel(model.WorkId.Value);

                    work.State = (int)ApprovalState.WorkPartAppr;

                    DataOperateBusiness<Epm_DangerousWork>.Get().Update(work);
                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "上传了危险作业实景，待审核";
                    app.Content = CurrentUserName + "上传了危险作业实景，待审核";
                    app.SendUserId = work.CreateUserId;
                    app.SendUserName = work.CreateUserName;
                    app.SendTime = work.CreateTime;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Dangerous.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Dangerous.GetText();
                    app.BusinessState = (int)(ApprovalState.WorkPartAppr);
                    app.BusinessId = model.WorkId;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = project.Name;
                    app.IsApprover = true;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Dangerous.GetText(), SystemRight.Add.GetText(), "提交危险作业生成待办: " + model.Id);
                    #endregion
                }
                result.Data = true;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Exception = new ExceptionEx(ex, "AddWorkRealScenen");
                result.Flag = EResultFlag.Failure;
                result.Data = false;
                result.AllRowsCount = -1;
            }
            return result;
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id">危险作业Id</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateWorkRealScenenState(long id, ApprovalState state)
        {
            Result<int> result = new Result<int>();
            try
            {
                int WorkWaitAppr = (int)ApprovalState.WaitAppr;
                var list = DataOperateBusiness<Epm_WorkUploadRealScene>.Get().GetList(t => t.WorkId == id && t.State == WorkWaitAppr).ToList();
                if (list.Any())
                {
                    string uploadTime = "";
                    foreach (var item in list)
                    {
                        uploadTime = item.UploadTime.Value.ToShortDateString();
                        item.State = (int)state;
                        item.OperateUserId = CurrentUserID.ToLongReq();
                        item.OperateUserName = CurrentUserName;
                        item.OperateTime = DateTime.Now;
                        var rows = DataOperateBusiness<Epm_WorkUploadRealScene>.Get().Update(item);
                    }

                    result.Data = list.Count;
                    result.Flag = EResultFlag.Success;
                    WriteLog(BusinessType.Dangerous.GetText(), SystemRight.Delete.GetText(), "更新状态: " + list.Count);

                    var work = DataOperateBusiness<Epm_DangerousWork>.Get().GetModel(id);

                    //最后一天实景审核通过时，危险作业标记完成
                    if (state == ApprovalState.ApprSuccess)
                    {
                        if (uploadTime == work.EndTime.Value.ToShortDateString())
                        {
                            work.State = (int)ApprovalState.WorkFinish;
                            work.OperateUserId = CurrentUserID.ToLongReq();
                            work.OperateUserName = CurrentUserName;
                            work.OperateTime = DateTime.Now;
                            DataOperateBusiness<Epm_DangerousWork>.Get().Update(work);
                        }

                        //处理待办
                        var temp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == work.Id && t.IsApprover == false).FirstOrDefault();
                        if (temp != null)
                        {
                            ComplateApprover(temp.Id);
                        }

                        #region 生成待办
                        List<Epm_Approver> li = new List<Epm_Approver>();
                        Epm_Approver app = new Epm_Approver();
                        app.Title = work.CreateUserName + "上传的作业实景，审核通过";
                        app.Content = work.CreateUserName + "上传的作业实景，审核通过";
                        app.SendUserId = work.CreateUserId;
                        app.SendUserName = work.CreateUserName;
                        app.SendTime = work.CreateTime;
                        app.LinkURL = string.Empty;
                        app.BusinessTypeNo = BusinessType.Dangerous.ToString();
                        app.Action = SystemRight.Check.ToString();
                        app.BusinessTypeName = BusinessType.Dangerous.GetText();
                        app.BusinessState = (int)(ApprovalState.ApprSuccess);
                        app.BusinessId = work.Id;
                        app.ApproverId = work.CreateUserId;
                        app.ApproverName = work.CreateUserName;
                        app.ProjectId = work.ProjectId;
                        app.ProjectName = work.ProjectName;
                        li.Add(app);
                        AddApproverBatch(li);
                        WriteLog(BusinessType.Dangerous.GetText(), SystemRight.UnCheck.GetText(), "审核通过危险作业生成待办: " + work.Id);
                        #endregion
                    }

                    #region 消息
                    Epm_Massage modelMsg = new Epm_Massage();
                    modelMsg.ReadTime = null;
                    modelMsg.RecId = work.CreateUserId;
                    modelMsg.RecName = work.CreateUserName;
                    modelMsg.RecTime = DateTime.Now;
                    modelMsg.SendId = CurrentUserID.ToLongReq();
                    modelMsg.SendName = CurrentUserName;
                    modelMsg.SendTime = DateTime.Now;
                    modelMsg.Title = work.CreateUserName + "上传的作业实景，审核通过";
                    modelMsg.Content = work.CreateUserName + "上传的作业实景，审核通过";
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = work.Id;
                    modelMsg.BussinesType = BusinessType.Dangerous.ToString();
                    modelMsg.ProjectId = work.ProjectId.Value;
                    modelMsg.ProjectName = work.ProjectName;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateWorkRealScenenState");
            }
            return result;
        }

        /// <summary>
        /// 删除危险作业实景
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<int> DeleteWorkRealScenen(long id)
        {
            Result<int> result = new Result<int>();
            try
            {
                var scene = DataOperateBusiness<Epm_WorkUploadRealScene>.Get().GetList(t => t.WorkId == id).FirstOrDefault();
                if (scene != null)
                {
                    //处理附件
                    string tableName = scene.GetType().Name;
                    var files = DataOperateBasic<Base_Files>.Get().GetList(i => i.TableId == scene.Id && i.TableName == tableName).ToList();
                    DataOperateBasic<Base_Files>.Get().DeleteRange(files);

                    //删除实景
                    DataOperateBusiness<Epm_WorkUploadRealScene>.Get().Delete(scene);

                    //处理待办
                    var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == id).FirstOrDefault();
                    DataOperateBusiness<Epm_Approver>.Get().Delete(tempApp);
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteWorkRealScenenState");
            }
            return result;
        }

        /// <summary>
        /// 根据监理日志 ID 获取危险作业实景
        /// </summary>
        /// <param name="logId">监理日志 ID</param>
        /// <returns></returns>
        public Result<List<Epm_WorkUploadRealScene>> GetWorkRealSceneByLogId(long logId)
        {
            Result<List<Epm_WorkUploadRealScene>> result = new Result<List<Epm_WorkUploadRealScene>>();
            try
            {
                List<Epm_SupervisorLogWork> logWorks = DataOperateBusiness<Epm_SupervisorLogWork>.Get().GetList(p => p.LogId == logId).ToList();
                List<long> workUploadIds = logWorks.Select(p => p.WorkUploadId ?? 0).Distinct().ToList();
                var list = DataOperateBusiness<Epm_WorkUploadRealScene>.Get().GetList(p => workUploadIds.Contains(p.Id))
                    .ToList();

                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetWorkRealSceneByLogId");
            }
            return result;
        }

        /// <summary>
        /// 根据监理日志获取危险作业信息
        /// </summary>
        /// <param name="logId"></param>
        /// <returns></returns>
        public Result<List<Epm_DangerousWork>> GetDangerousWorkByLogId(long logId)
        {
            Result<List<Epm_DangerousWork>> result = new Result<List<Epm_DangerousWork>>();
            try
            {
                List<Epm_SupervisorLogWork> logWorks = DataOperateBusiness<Epm_SupervisorLogWork>.Get().GetList(p => p.LogId == logId).ToList();
                List<long> workUploadIds = logWorks.Select(p => p.WorkId ?? 0).Distinct().ToList();
                var list = DataOperateBusiness<Epm_DangerousWork>.Get().GetList(p => workUploadIds.Contains(p.Id))
                    .ToList();

                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetDangerousWorkByLogId");
            }
            return result;
        }
        #endregion
    }
}

