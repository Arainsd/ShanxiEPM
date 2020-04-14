using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using hc.epm.ViewModel;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddCompletionAcceptance(Epm_CompletionAcceptance model, List<Base_Files> fileListFile)
        {
            Result<int> result = new Result<int>();
            try
            {
                model = base.SetCurrentUser(model);
                model.CrtCompanyId = CurrentCompanyID.ToLongReq();
                model.CrtCompanyName = CurrentCompanyName;

                var rows = DataOperateBusiness<Epm_CompletionAcceptance>.Get().Add(model);

                if (fileListFile != null)
                {
                    AddFilesByTable(model, fileListFile);
                }

                result.Data = rows;
                result.Flag = EResultFlag.Success;

                #region  竣工验收协同接口
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                {
                    XtCompletionAcceptanceView view = new XtCompletionAcceptanceView();
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                    var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.CreateUserId);
                    if (baseUser == null)
                    {
                        throw new Exception("未找到申请人相关信息！");
                    }
                    else
                    {
                        view.hr_sqr = baseUser.ObjeId;
                    }

                    view.Name = model.ProjectName;
                    view.SubjectName = project.SubjectName;
                    view.StartDate = string.Format("{0:yyyy-MM-dd}", project.StartDate);
                    view.Remark = project.Remark;
                    view.CompanyName = project.CompanyName;
                    view.ProjectTypeName = project.ProjectTypeName;
                    view.ProjectSubjectName = project.ProjectSubjectName;
                    view.ProjectNatureName = project.ProjectNatureName;
                    view.PMName = project.PMName;
                    view.EndDate = string.Format("{0:yyyy-MM-dd}", project.EndDate);
                    view.Description = project.Description;
                    view.CrtCompanyName = project.CrtCompanyName;
                    view.ContactUserName = project.ContactUserName;
                    view.Address = project.Address;

                    ////上传附件
                    //if (model.TzAttachs != null && model.TzAttachs.Any())
                    //{
                    //    string baseFaleUrl = System.Configuration.ConfigurationManager.AppSettings.Get("XtDownloadUrl");
                    //    foreach (var item in model.TzAttachs)
                    //    {
                    //        string fileUrl = string.Format("{0}?fileId={1}&type={2}", baseFaleUrl, item.Id, item.TypeNo);
                    //        view.Temp_TzAttachs = fileUrl + '|' + view.Temp_TzAttachs;
                    //    }
                    //    if (view.Temp_TzAttachs != null)
                    //    {
                    //        view.Temp_TzAttachs = view.Temp_TzAttachs.Substring(0, view.Temp_TzAttachs.Length - 1);
                    //    }
                    //}

                    //model.WorkFlowId = XtWorkFlowService.CreateTzProjectProposalWorkFlow(view);
                }
                #endregion

                WriteLog(BusinessType.Completed.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddCompletionAcceptance");
            }
            return result;
        }

        /// <summary>
        /// 新增完工验收
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public Result<int> AddCompletionAcceptanceNew(CompletionAcceptanceView view)
        {
            Result<int> result = new Result<int>();
            try
            {
                var completionAcceptance = view.CompletionAcceptance;
                var completionRectifyCompanys = view.CompletionRectifyCompanys;
                completionAcceptance = base.SetCurrentUser(completionAcceptance);
                completionAcceptance.CrtCompanyId = CurrentCompanyID.ToLongReq();
                completionAcceptance.CrtCompanyName = CurrentCompanyName;
                completionAcceptance.RecUserId = CurrentUser.Id;
                completionAcceptance.RecUserName = CurrentUserName;

                var rows = DataOperateBusiness<Epm_CompletionAcceptance>.Get().Add(completionAcceptance);

                if (completionRectifyCompanys != null && completionRectifyCompanys.Any())
                {
                    completionRectifyCompanys.ForEach(p =>
                    {
                        SetCreateUser(p);
                        SetCurrentUser(p);
                        p.AcceptanceId = completionAcceptance.Id;
                    });
                    DataOperateBusiness<Epm_CompletionRectifyCompany>.Get().AddRange(completionRectifyCompanys);
                }

                if (view.BaseFiles != null)
                {
                    AddFilesByTable(completionAcceptance, view.BaseFiles);
                }

                //生成代办消息
                if ((PreCompletionScceptanceState)completionAcceptance.State == PreCompletionScceptanceState.WaitApproval)
                {
                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(completionAcceptance.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "提交了" + project.Name + "完工验收申请，待审核";
                    app.Content = CurrentUserName + "提交了" + project.Name + "完工验收申请，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Completed.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Completed.GetText();
                    app.BusinessState = (int)(PreCompletionScceptanceState.WaitApproval);
                    app.BusinessId = completionAcceptance.Id;
                    app.ApproverId = project.PMId;
                    app.ApproverName = project.PMName;
                    app.ProjectId = completionAcceptance.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Completed.GetText(), SystemRight.Add.GetText(), "提交完工验收生成待办: " + completionAcceptance.Id);
                    #endregion

                    #region 消息
                    var waitSend = GetWaitSendMessageList(completionAcceptance.ProjectId.Value);
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
                        modelMsg.Title = CurrentUserName + "提交了" + project.Name + "完工验收申请，待审核";
                        modelMsg.Content = CurrentUserName + "提交了" + project.Name + "完工验收申请，待审核";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = completionAcceptance.Id;
                        modelMsg.BussinesType = BusinessType.Completed.ToString();
                        modelMsg.ProjectId = completionAcceptance.ProjectId.Value;
                        modelMsg.ProjectName = completionAcceptance.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion

                    #region 发送短信
                    //Dictionary<string, string> parameters = new Dictionary<string, string>();
                    //parameters.Add("UserName", CurrentUserName);
                    //WriteSMS(project.PMId.Value, 0, MessageStep.ComplationAdd, parameters);
                    #endregion
                }

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Completed.GetText(), SystemRight.Add.GetText(), "新增: " + completionAcceptance.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddCompletionAcceptanceNew");
            }
            return result;
        }

        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateCompletionAcceptance(Epm_CompletionAcceptance model, List<Base_Files> fileListFile)
        {
            Result<int> result = new Result<int>();
            try
            {
                var oldModel = DataOperateBusiness<Epm_CompletionAcceptance>.Get().GetModel(model.Id);
                model = FiterUpdate(oldModel, model);

                var rows = DataOperateBusiness<Epm_CompletionAcceptance>.Get().Update(model);

                if (fileListFile != null)
                {
                    //删除之前的附件
                    DeleteFilesByTable(model.GetType().Name, new List<long>() { model.Id });
                    //新增附件
                    AddFilesByTable(model, fileListFile);
                }

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Completed.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateCompletionAcceptance");
            }
            return result;
        }

        ///<summary>
        ///修改完工验收
        ///</summary>
        /// <param name="view">完工验收</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateCompletionAcceptanceNew(CompletionAcceptanceView view)
        {
            Result<int> result = new Result<int>();
            try
            {
                var completionAcceptance = view.CompletionAcceptance;
                var completionRectifyCompanys = view.CompletionRectifyCompanys;
                var baseFiles = view.BaseFiles;
                if (completionAcceptance == null)
                {
                    throw new Exception("请选择要修改的内容！");
                }

                var oldModel = DataOperateBusiness<Epm_CompletionAcceptance>.Get().GetModel(completionAcceptance.Id);

                if (oldModel == null)
                {
                    throw new Exception("要修改的信息不存在！");
                }
                //if (oldModel.State != (int)PreCompletionScceptanceState.WaitApproval)
                //{
                //    throw new Exception("只有未提交审核的信息才可以进行修改！");
                //}

                oldModel.Title = completionAcceptance.Title;
                oldModel.RecTime = completionAcceptance.RecTime;
                oldModel.Content = completionAcceptance.Content;
                oldModel.Num = completionAcceptance.Num;
                oldModel.RecTime = completionAcceptance.RecTime;
                oldModel.RecCompanyId = completionAcceptance.RecCompanyId;
                oldModel.RecCompanyName = completionAcceptance.RecCompanyName;
                oldModel.RecUserId = completionAcceptance.RecUserId;
                oldModel.RecUserName = completionAcceptance.RecUserName;
                oldModel.RectifContent = completionAcceptance.RectifContent;
                oldModel.AcceptanceResult = completionAcceptance.AcceptanceResult;
                oldModel.State = completionAcceptance.State;

                var oldCompanyList = DataOperateBusiness<Epm_CompletionRectifyCompany>.Get().GetList(p => p.AcceptanceId == oldModel.Id);

                var rows = DataOperateBusiness<Epm_CompletionAcceptance>.Get().Update(oldModel);
                if (oldCompanyList.Any())
                {
                    DataOperateBusiness<Epm_CompletionRectifyCompany>.Get().DeleteRange(oldCompanyList);
                }
                if (completionRectifyCompanys != null && completionRectifyCompanys.Any())
                {
                    completionRectifyCompanys.ForEach(p =>
                    {
                        SetCreateUser(p);
                        SetCurrentUser(p);
                        p.AcceptanceId = oldModel.Id;
                    });
                    DataOperateBusiness<Epm_CompletionRectifyCompany>.Get().AddRange(completionRectifyCompanys);
                }

                if (baseFiles != null)
                {
                    //删除之前的附件
                    DeleteFilesByTable(oldModel.GetType().Name, new List<long>() { oldModel.Id });
                    //新增附件
                    AddFilesByTable(oldModel, baseFiles);
                }

                //处理待办
                //var temp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == oldModel.Id && t.IsApprover == false).FirstOrDefault();
                //if (temp != null)
                //{
                //    ComplateApprover(temp.Id);
                //}

                //生成代办消息
                //if ((PreCompletionScceptanceState)oldModel.State == PreCompletionScceptanceState.WaitApproval)
                //{
                //    #region 生成待办
                //    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(oldModel.ProjectId.Value);
                //    List<Epm_Approver> list = new List<Epm_Approver>();
                //    Epm_Approver app = new Epm_Approver();
                //    app.Title = CurrentUserName + "提交了" + project.Name + "完工验收申请，待审核";
                //    app.Content = CurrentUserName + "提交了" + project.Name + "完工验收申请，待审核";
                //    app.SendUserId = CurrentUserID.ToLongReq();
                //    app.SendUserName = CurrentUserName;
                //    app.SendTime = DateTime.Now;
                //    app.LinkURL = string.Empty;
                //    app.BusinessTypeNo = BusinessType.Completed.ToString();
                //    app.Action = SystemRight.Add.ToString();
                //    app.BusinessTypeName = BusinessType.Completed.GetText();
                //    app.BusinessState = (int)(PreCompletionScceptanceState.WaitApproval);
                //    app.BusinessId = oldModel.Id;
                //    app.ApproverId = project.PMId;
                //    app.ApproverName = project.PMName;
                //    app.ProjectId = oldModel.ProjectId;
                //    app.ProjectName = project.Name;
                //    list.Add(app);
                //    AddApproverBatch(list);
                //    WriteLog(BusinessType.Completed.GetText(), SystemRight.Add.GetText(), "提交完工验收生成待办: " + oldModel.Id);
                //    #endregion

                //    #region 消息
                //    var waitSend = GetWaitSendMessageList(oldModel.ProjectId.Value);
                //    foreach (var send in waitSend)
                //    {
                //        Epm_Massage modelMsg = new Epm_Massage();
                //        modelMsg.ReadTime = null;
                //        modelMsg.RecId = send.Key;
                //        modelMsg.RecName = send.Value;
                //        modelMsg.RecTime = DateTime.Now;
                //        modelMsg.SendId = CurrentUserID.ToLongReq();
                //        modelMsg.SendName = CurrentUserName;
                //        modelMsg.SendTime = DateTime.Now;
                //        modelMsg.Title = CurrentUserName + "提交了" + project.Name + "完工验收申请，待审核";
                //        modelMsg.Content = CurrentUserName + "提交了" + project.Name + "完工验收申请，待审核";
                //        modelMsg.Type = 2;
                //        modelMsg.IsRead = false;
                //        modelMsg.BussinessId = oldModel.Id;
                //        modelMsg.BussinesType = BusinessType.Completed.ToString();
                //        modelMsg.ProjectId = oldModel.ProjectId.Value;
                //        modelMsg.ProjectName = oldModel.ProjectName;
                //        modelMsg = base.SetCurrentUser(modelMsg);
                //        modelMsg = base.SetCreateUser(modelMsg);
                //        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                //    }
                //    #endregion

                //    #region 发送短信
                //    //Dictionary<string, string> parameters = new Dictionary<string, string>();
                //    //parameters.Add("UserName", CurrentUserName);
                //    //WriteSMS(project.PMId.Value, 0, MessageStep.ComplationAdd, parameters);
                //    #endregion
                //}

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Completed.GetText(), SystemRight.Modify.GetText(), "修改: " + oldModel.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateCompletionAcceptance");
            }
            return result;
        }

        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteCompletionAcceptanceByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_CompletionAcceptance>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                if (models.Count > 0)
                {
                    foreach (var item in models)
                    {
                        //处理待办
                        var temp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == item.Id && t.IsApprover == false).FirstOrDefault();
                        if (temp != null)
                        {
                            ComplateApprover(temp.Id);
                        }
                        #region 消息
                        var waitSend = GetWaitSendMessageList(item.ProjectId.Value);
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
                            modelMsg.Title = item.CreateUserName + "提报的完工验收已删除";
                            modelMsg.Content = item.CreateUserName + "提报的完工验收已删除";
                            modelMsg.Type = 2;
                            modelMsg.IsRead = false;
                            modelMsg.BussinessId = item.Id;
                            modelMsg.BussinesType = BusinessType.Contract.ToString();
                            modelMsg.ProjectId = item.ProjectId;
                            modelMsg.ProjectName = item.ProjectName;
                            modelMsg = base.SetCurrentUser(modelMsg);
                            modelMsg = base.SetCreateUser(modelMsg);
                            DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                        }
                        #endregion
                    }

                    var rows = DataOperateBusiness<Epm_CompletionAcceptance>.Get().DeleteRange(models);
                    result.Data = rows;
                    result.Flag = EResultFlag.Success;
                    WriteLog(BusinessType.Completed.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
                }
                else
                {
                    throw new Exception("该完工验收信息不存在或已被删除！");
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteCompletionAcceptanceByIds");
            }
            return result;
        }

        /// <summary>
        /// 根据 ID 删除完工验收
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<bool> DeleteCompletionAcceptanceById(long id)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                var model = DataOperateBusiness<Epm_CompletionAcceptance>.Get().GetModel(id);

                if (model == null)
                {
                    throw new Exception("该完工验收信息不存在或已被删除！");
                }
                if (model.State != (int)PreCompletionScceptanceState.WaitApproval)
                {
                    throw new Exception("只有未提交审核的才能进行删除！");
                }

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
                    modelMsg.Title = model.CreateUserName + "提报的完工验收已删除";
                    modelMsg.Content = model.CreateUserName + "提报的完工验收已删除";
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = model.Id;
                    modelMsg.BussinesType = BusinessType.Completed.ToString();
                    modelMsg.ProjectId = model.ProjectId;
                    modelMsg.ProjectName = model.ProjectName;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                }
                #endregion

                DataOperateBusiness<Epm_CompletionAcceptance>.Get().Delete(model);

                var oldCompanyList = DataOperateBusiness<Epm_CompletionRectifyCompany>.Get().GetList(p => p.AcceptanceId == id);
                if (oldCompanyList.Any())
                {
                    DataOperateBusiness<Epm_CompletionRectifyCompany>.Get().DeleteRange(oldCompanyList);
                }

                result.Data = true;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Completed.GetText(), SystemRight.Delete.GetText(), "删除: " + id);
            }
            catch (Exception ex)
            {
                result.Data = false;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteCompletionAcceptanceByIds");
            }
            return result;
        }

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_CompletionAcceptance>> GetCompletionAcceptanceList(QueryCondition qc)
        {
            //qc = AddDefaultWeb(qc);
            Result<List<Epm_CompletionAcceptance>> result = new Result<List<Epm_CompletionAcceptance>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_CompletionAcceptance>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCompletionAcceptanceList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_CompletionAcceptance> GetCompletionAcceptanceModel(long id)
        {
            Result<Epm_CompletionAcceptance> result = new Result<Epm_CompletionAcceptance>();
            try
            {
                var model = DataOperateBusiness<Epm_CompletionAcceptance>.Get().GetModel(id);
                if (model != null)
                {
                    result.Data = model;
                    result.Flag = EResultFlag.Success;
                }
                else
                {
                    throw new Exception("该完工验收详情信息不存在或已被删除！");
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCompletionAcceptanceModel");
            }
            return result;
        }

        public Result<CompletionAcceptanceView> GetCompletionAcceptanceModelNew(long id)
        {
            Result<CompletionAcceptanceView> result = new Result<CompletionAcceptanceView>();
            try
            {
                var model = DataOperateBusiness<Epm_CompletionAcceptance>.Get().GetModel(id);

                if (model == null)
                {
                    throw new Exception("该完工验收详情信息不存在或已被删除！");
                }

                CompletionAcceptanceView view = new CompletionAcceptanceView();

                view.CompletionAcceptance = model;
                view.CompletionRectifyCompanys = DataOperateBusiness<Epm_CompletionRectifyCompany>.Get()
                    .GetList(p => p.AcceptanceId == id).ToList();

                string tableName = model.GetType().Name;

                view.BaseFiles = DataOperateBasic<Base_Files>.Get().GetList(p => p.TableName == tableName && p.TableId == id).ToList();

                result.Data = view;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCompletionAcceptanceModelNew");
            }
            return result;
        }


        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public Result<int> ChangeCompletionAcceptanceState(long id, PreCompletionScceptanceState state, string reason)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBusiness<Epm_CompletionAcceptance>.Get().GetModel(id);

                if (model != null)
                {
                    model.OperateUserId = CurrentUserID.ToLongReq();
                    model.OperateUserName = CurrentUserName;
                    model.OperateTime = DateTime.Now;
                    model.State = (int)state;
                    string title = "";
                    var projectInfo = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                    if ((PreCompletionScceptanceState)Enum.ToObject(typeof(PreCompletionScceptanceState), model.State) == PreCompletionScceptanceState.ApprovalSuccess)
                    {
                        #region 消息
                        title = model.CreateUserName + "提报的完工验收单，审核通过";
                        #endregion

                        //“省公司验收”的初始化状态为“未完成”，当完工验收功能的流程结束后，将项目的“省公司验收”字段状态改为“完成”
                        projectInfo.ProCompanyAcceptance = 1;
                        DataOperateBusiness<Epm_Project>.Get().Update(projectInfo);

                        #region 生成待办
                        List<Epm_Approver> list = new List<Epm_Approver>();
                        Epm_Approver app = new Epm_Approver();
                        app.Title = model.CreateUserName + "提报的完工验收单，审核通过";
                        app.Content = model.CreateUserName + "提报的完工验收单，审核通过";
                        app.SendUserId = model.CreateUserId;
                        app.SendUserName = model.CreateUserName;
                        app.SendTime = model.CreateTime;
                        app.LinkURL = string.Empty;
                        app.BusinessTypeNo = BusinessType.Completed.ToString();
                        app.Action = SystemRight.Check.ToString();
                        app.BusinessTypeName = BusinessType.Completed.GetText();
                        app.BusinessState = (int)(ApprovalState.ApprSuccess);
                        app.BusinessId = model.Id;
                        app.ApproverId = projectInfo.PMId;
                        app.ApproverName = projectInfo.PMName;
                        app.ProjectId = model.ProjectId;
                        app.ProjectName = model.ProjectName;
                        list.Add(app);
                        AddApproverBatch(list);
                        WriteLog(BusinessType.Contract.GetText(), SystemRight.Check.GetText(), "审核通过完工验收单生成待办: " + model.Id);
                        #endregion

                    }
                    else if ((PreCompletionScceptanceState)Enum.ToObject(typeof(PreCompletionScceptanceState), model.State) == PreCompletionScceptanceState.ApprovalFailure)
                    {
                        #region 消息
                        title = model.CreateUserName + "提报的完工验收单，已被驳回";
                        #endregion

                        #region 生成待办
                        List<Epm_Approver> list = new List<Epm_Approver>();
                        Epm_Approver app = new Epm_Approver();
                        app.Title = model.CreateUserName + "提报的完工验收单已被驳回，请处理";
                        app.Content = model.CreateUserName + "提报的完工验收单已被驳回，请处理";
                        app.SendUserId = model.CreateUserId;
                        app.SendUserName = model.CreateUserName;
                        app.SendTime = model.CreateTime;
                        app.LinkURL = string.Empty;
                        app.BusinessTypeNo = BusinessType.Completed.ToString();
                        app.Action = SystemRight.UnCheck.ToString();
                        app.BusinessTypeName = BusinessType.Completed.GetText();
                        app.BusinessState = (int)(PreCompletionScceptanceState.ApprovalFailure);
                        app.BusinessId = model.Id;
                        app.ApproverId = model.CreateUserId;
                        app.ApproverName = model.CreateUserName;
                        app.ProjectId = model.ProjectId;
                        app.ProjectName = model.ProjectName;
                        list.Add(app);
                        AddApproverBatch(list);
                        WriteLog(BusinessType.Contract.GetText(), SystemRight.UnCheck.GetText(), "驳回完工验收单生成待办: " + model.Id);
                        #endregion
                    }

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
                        modelMsg.Title = title;
                        modelMsg.Content = title;
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Completed.ToString();
                        modelMsg.ProjectId = model.ProjectId.Value;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion

                    var rows = DataOperateBusiness<Epm_CompletionAcceptance>.Get().Update(model);

                    //保存需要发送的基础数据
                    AddSendDateByProjectId(projectInfo.Id);

                    result.Data = rows;
                    result.Flag = EResultFlag.Success;
                }
                else
                {
                    throw new Exception("该完工验收信息不存在或已被删除！");
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "ChangeCompletionAcceptanceState");
            }
            return result;
        }

        /// <summary>
        /// 根据项目 ID 获取验收项资料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<List<CompletionAcceptanceItemView>> GetCompletionItem(long id)
        {
            var result = new Result<List<CompletionAcceptanceItemView>>();
            try
            {
                List<CompletionAcceptanceItemView> list = new List<CompletionAcceptanceItemView>();

                // 1. 检查项
                var monitor = DataOperateBusiness<Epm_Monitor>.Get().GetList(p => p.ProjectId == id).ToList();
                list.Add(new CompletionAcceptanceItemView()
                {
                    Id = string.Empty,
                    Name = monitor.Any() ? monitor.Count.ToString() : "0",
                    Type = "Epm_Monitor",
                    Result = true
                });
                if (monitor.Any())
                {
                    list.AddRange(monitor.Select(p => new CompletionAcceptanceItemView()
                    {
                        Id = p.SId,
                        Name = p.Title,
                        Type = "Epm_Monitor",
                        Result = p.State == (int)RectificationState.WorkFinish ? true : false
                    }));
                }

                // 2. 获取项目资料
                var projectDataSubmit = DataOperateBusiness<Epm_ProjectDataSubmit>.Get().GetList(p => p.ProjectId == id).ToList();
                list.Add(new CompletionAcceptanceItemView()
                {
                    Id = string.Empty,
                    Name = projectDataSubmit.Any() ? projectDataSubmit.Count.ToString() : "0",
                    Type = "Epm_ProjectDataSubmit",
                    Result = true
                });
                if (projectDataSubmit.Any())
                {
                    list.AddRange(projectDataSubmit.Select(p => new CompletionAcceptanceItemView()
                    {
                        Id = p.ProjectId.Value.ToString(),
                        Name = p.FileName,
                        Type = "Epm_ProjectDataSubmit",
                        Result = p.UploadUserId.HasValue ? true : false
                    }));
                }

                // 3. 获取项目问题
                var question = DataOperateBusiness<Epm_Question>.Get().GetList(p => p.ProjectId == id).ToList();
                list.Add(new CompletionAcceptanceItemView()
                {
                    Id = string.Empty,
                    Name = question.Any() ? question.Count.ToString() : "0",
                    Type = "Epm_Question",
                    Result = true
                });
                if (question.Any())
                {
                    list.AddRange(question.Select(p => new CompletionAcceptanceItemView()
                    {
                        Id = p.SId,
                        Name = p.Title,
                        Type = "Epm_Question",
                        Result = p.State == 2 ? true : false
                    }));
                }
                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Exception = new ExceptionEx(ex, "GetCompletionItem");
                result.Flag = EResultFlag.Failure;
            }
            return result;
        }

        /// <summary>
        /// 修改验收申请状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateCompletionAcceptanceState(List<long> ids, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                foreach (var item in ids)
                {
                    //验收申请信息
                    var model = DataOperateBusiness<Epm_CompletionAcceptance>.Get().GetModel(item);
                    if (model != null)
                    {
                        SetCurrentUser(model);
                        //model.State = (int)state.ToEnumReq<ApprovalState>();
                        model.State =(int)PreCompletionScceptanceState.ApprovalSuccess;
                        var rows = DataOperateBusiness<Epm_CompletionAcceptance>.Get().Update(model);

                        //生成验收信息数据
                        if (model.State == (int)PreCompletionScceptanceState.ApprovalSuccess)
                        {
                            ////验收申请信息
                            //var CompletionAcceptance = DataOperateBusiness<Epm_CompletionAcceptance>.Get().GetModel(model.ProjectId.Value);

                            Epm_CompletionAcceptanceResUpload caru = new Epm_CompletionAcceptanceResUpload();//验收表
                            caru.ProjectId = model.ProjectId;
                            caru.ProjectName = model.ProjectName;
                            caru.Content = model.Content;
                            caru.Num = model.Num;
                            caru.RecCompanyId = model.RecCompanyId;
                            caru.RecCompanyName = model.RecCompanyName;
                            caru.RecUserId = model.RecUserId;
                            caru.RecUserName = model.RecUserName;
                            caru.RecTime = model.RecTime;
                            caru.AcceptanceResult = model.AcceptanceResult;
                            caru.State = (int)PreProjectApprovalState.WaitSubmitted;
                            caru.Remark = model.Remark;
                            caru.CrtCompanyId = model.CrtCompanyId;
                            caru.CrtCompanyName = model.CrtCompanyName;
                            caru.Title = model.Title;
                            caru.RectifContent = model.RectifContent;
                           

                            SetCurrentUser(caru);
                            SetCreateUser(caru);
                            DataOperateBusiness<Epm_CompletionAcceptanceResUpload>.Get().Add(caru);
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
