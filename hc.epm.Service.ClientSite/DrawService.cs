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
        public Result<int> AddDraw(Epm_Draw model, List<Base_Files> fileListFile)
        {
            Result<int> result = new Result<int>();
            try
            {
                model = base.SetCurrentUser(model);
                model.CrtCompanyId = CurrentCompanyID.ToLongReq();
                model.CrtCompanyName = CurrentCompanyName;

                //bool dConfig = DataOperateBusiness<Epm_Draw>.Get().Count(i => i.Name == model.Name && i.ProjectId == model.ProjectId && i.VersionNo == model.VersionNo) > 0;
                //if (dConfig)
                //{
                //    throw new Exception("该图纸名称已经存在");
                //}
                var rows = DataOperateBusiness<Epm_Draw>.Get().Add(model);

                //上传图纸
                if (fileListFile != null)
                {
                    AddFilesByTable(model, fileListFile);
                }
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Draw.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
                if ((ApprovalState)model.State == ApprovalState.WaitAppr)
                {
                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "上传了项目图纸，待审核";
                    app.Content = CurrentUserName + "上传了项目图纸，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Draw.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Draw.GetText();
                    app.BusinessState = (int)(ApprovalState.WaitAppr);
                    app.BusinessId = model.Id;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Draw.GetText(), SystemRight.Add.GetText(), "提交图纸生成待办: " + model.Id);
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
                        modelMsg.Title = CurrentUserName + "上传了项目图纸，待审核";
                        modelMsg.Content = CurrentUserName + "上传了项目图纸，待审核";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Draw.ToString();
                        modelMsg.ProjectId = model.ProjectId;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion

                    #region 发送短信
                    //Dictionary<string, string> parametersms = new Dictionary<string, string>();
                    //parametersms.Add("UserName", CurrentUserName);
                    //WriteSMS(project.ContactUserId.Value, project.CompanyId, MessageStep.DrawAdd, parametersms);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddDraw");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateDraw(Epm_Draw model, List<Base_Files> fileListFile)
        {
            Result<int> result = new Result<int>();
            try
            {
                //bool dConfig = DataOperateBusiness<Epm_Draw>.Get().Count(i => i.Name == model.Name && i.ProjectId == model.ProjectId && i.VersionNo == model.VersionNo && i.Id != model.Id) > 0;
                //if (dConfig)
                //{
                //    throw new Exception("该图纸名称已经存在");
                //}
                //dConfig = DataOperateBusiness<Epm_Draw>.Get().Count(i => i.VersionOrder == model.VersionOrder && i.Id != model.Id) > 0;
                //if (dConfig)
                //{
                //    throw new Exception("该图纸版本号已经存在");
                //}

                var oldModel = DataOperateBusiness<Epm_Draw>.Get().GetModel(model.Id);
                model = FiterUpdate(oldModel, model);

                var rows = DataOperateBusiness<Epm_Draw>.Get().Update(model);

                //上传图纸
                if (fileListFile != null)
                {
                    //删除之前的附件
                    DeleteFilesByTable(model.GetType().Name, new List<long>() { model.Id });
                    //新增附件
                    AddFilesByTable(model, fileListFile);
                }

                //处理待办
                var temp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == model.Id && t.IsApprover == false).FirstOrDefault();
                if (temp != null)
                {
                    ComplateApprover(temp.Id);
                }
                //生成代办消息
                if ((ApprovalState)model.State == ApprovalState.WaitAppr)
                {
                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "上传了项目图纸，待审核";
                    app.Content = CurrentUserName + "上传了项目图纸，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Draw.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Draw.GetText();
                    app.BusinessState = (int)(ApprovalState.WaitAppr);
                    app.BusinessId = model.Id;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Draw.GetText(), SystemRight.Add.GetText(), "提交图纸生成待办: " + model.Id);
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
                        modelMsg.Title = CurrentUserName + "上传了项目图纸，待审核";
                        modelMsg.Content = CurrentUserName + "上传了项目图纸，待审核";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Draw.ToString();
                        modelMsg.ProjectId = model.ProjectId;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion

                    #region 发送短信
                    //Dictionary<string, string> parametersms = new Dictionary<string, string>();
                    //parametersms.Add("UserName", CurrentUserName);
                    //WriteSMS(project.ContactUserId.Value, project.CompanyId, MessageStep.DrawAdd, parametersms);
                    #endregion
                }
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Draw.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateDraw");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteDrawByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_Draw>.Get().GetList(i => ids.Contains(i.Id)).ToList();

                if (models.Count > 0)
                {
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
                            modelMsg.Title = CurrentUserName + "上传的项目图纸已删除";
                            modelMsg.Content = CurrentUserName + "上传的项目图纸已删除";
                            modelMsg.Type = 2;
                            modelMsg.IsRead = false;
                            modelMsg.BussinessId = model.Id;
                            modelMsg.BussinesType = BusinessType.Draw.ToString();
                            modelMsg.ProjectId = model.ProjectId;
                            modelMsg.ProjectName = model.ProjectName;
                            modelMsg = base.SetCurrentUser(modelMsg);
                            modelMsg = base.SetCreateUser(modelMsg);
                            DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                        }
                        #endregion
                    }
                    var rows = DataOperateBusiness<Epm_Draw>.Get().DeleteRange(models);
                    result.Data = rows;
                    result.Flag = EResultFlag.Success;
                    WriteLog(BusinessType.Draw.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
                }
                else
                {
                    throw new Exception("该图纸不存在或已被删除");
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteDrawByIds");
            }
            return result;
        }

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_Draw>> GetDrawList(QueryCondition qc)
        {
            qc = AddDefaultWeb(qc);
            Result<List<Epm_Draw>> result = new Result<List<Epm_Draw>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Draw>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetDrawList");
            }
            return result;
        }


        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_Draw> GetDrawModel(long id)
        {
            Result<Epm_Draw> result = new Result<Epm_Draw>();
            try
            {
                var model = DataOperateBusiness<Epm_Draw>.Get().GetModel(id);

                if (model != null)
                {
                    result.Data = model;
                    result.Flag = EResultFlag.Success;
                }
                else
                {
                    throw new Exception("该图纸详情不存在或已被删除");
                }

            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetDrawModel");
            }
            return result;
        }

        /// <summary>
        /// 修改状态(废弃)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeDrawState(long id, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                var drawState = state.ToEnumReq<ApprovalState>();

                var model = DataOperateBusiness<Epm_Draw>.Get().GetModel(id);

                model.OperateUserId = CurrentUserID.ToLongReq();
                model.OperateUserName = CurrentUserName;
                model.OperateTime = DateTime.Now;
                model.State = int.Parse(drawState.GetValue().ToString());

                var rows = DataOperateBusiness<Epm_Draw>.Get().Update(model);

                //处理待办
                var temp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == model.Id && t.IsApprover == false).FirstOrDefault();
                if (temp != null)
                {
                    ComplateApprover(temp.Id);
                }

                #region 生成待办
                var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                List<Epm_Approver> list = new List<Epm_Approver>();
                Epm_Approver app = new Epm_Approver();
                app.Title = model.CreateUserName + "上传了项目图纸，已废弃";
                app.Content = model.CreateUserName + "上传了项目图纸，已废弃";
                app.SendUserId = CurrentUserID.ToLongReq();
                app.SendUserName = CurrentUserName;
                app.SendTime = DateTime.Now;
                app.LinkURL = string.Empty;
                app.BusinessTypeNo = BusinessType.Draw.ToString();
                app.Action = SystemRight.Invalid.ToString();
                app.BusinessTypeName = BusinessType.Draw.GetText();
                app.BusinessState = (int)(ApprovalState.Discarded);
                app.BusinessId = model.Id;
                app.ApproverId = project.ContactUserId;
                app.ApproverName = project.ContactUserName;
                app.ProjectId = model.ProjectId;
                app.ProjectName = project.Name;
                list.Add(app);
                AddApproverBatch(list);
                WriteLog(BusinessType.Draw.GetText(), SystemRight.Invalid.GetText(), "废弃图纸生成待办: " + model.Id);
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
                    modelMsg.Title = model.CreateUserName + "上传了项目图纸，已废弃";
                    modelMsg.Content = model.CreateUserName + "上传了项目图纸，已废弃";
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = model.Id;
                    modelMsg.BussinesType = BusinessType.Draw.ToString();
                    modelMsg.ProjectId = model.ProjectId;
                    modelMsg.ProjectName = model.ProjectName;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                }
                #endregion
                result.Data = rows;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "ChangeDrawState");
            }
            return result;
        }

        /// <summary>
        /// 审核/驳回
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public Result<int> RejectDraw(long id, string state, string reason)
        {
            Result<int> result = new Result<int>();
            try
            {
                var drawState = state.ToEnumReq<ApprovalState>();

                var model = DataOperateBusiness<Epm_Draw>.Get().GetModel(id);

                model.OperateUserId = CurrentUserID.ToLongReq();
                model.OperateUserName = CurrentUserName;
                model.OperateTime = DateTime.Now;
                model.State = int.Parse(drawState.GetValue().ToString());

                //处理待办
                var temp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == model.Id && t.IsApprover == false).FirstOrDefault();
                if (temp != null)
                {
                    ComplateApprover(temp.Id);
                }
                string title = "";
                if ((ApprovalState)model.State == ApprovalState.ApprSuccess)
                {
                    //每一次审核通过都修改本次IsValidate的值为1,之前已经存在的IsValidate为1的状态修改为0；
                    model.IsValidate = true;

                    var list = DataOperateBusiness<Epm_Draw>.Get().GetList(t => t.ProjectId == model.ProjectId && t.Id != model.Id).ToList();
                    if (list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            item.IsValidate = false;
                        }

                        DataOperateBusiness<Epm_Draw>.Get().UpdateRange(list);
                    }
                    if (model.VersionNo == "BlueprintVersion")
                    {
                        //“已完成施工设计”的初始化状态为“未完成”，当图纸管理中上传项目图纸并审核通过后，所选项目的“已完成施工设计”字段状态改为“完成”。
                        var projectInfo = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                        projectInfo.FinshDesign = 1;
                        DataOperateBusiness<Epm_Project>.Get().Update(projectInfo);
                    }

                    title = model.CreateUserName + "上传的项目图纸，审核通过";
                    #region 生成待办
                    List<Epm_Approver> listApp = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = model.CreateUserName + "上传的项目图纸，审核通过";
                    app.Content = model.CreateUserName + "上传的项目图纸，审核通过";
                    app.SendUserId = model.CreateUserId;
                    app.SendUserName = model.CreateUserName;
                    app.SendTime = model.CreateTime;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Draw.ToString();
                    app.Action = SystemRight.Check.ToString();
                    app.BusinessTypeName = BusinessType.Draw.GetText();
                    app.BusinessState = (int)(ApprovalState.ApprSuccess);
                    app.BusinessId = model.Id;
                    app.ApproverId = model.CreateUserId;
                    app.ApproverName = model.CreateUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = model.ProjectName;
                    listApp.Add(app);
                    AddApproverBatch(listApp);
                    WriteLog(BusinessType.Draw.GetText(), SystemRight.Check.GetText(), "审核通过图纸生成待办: " + model.Id);
                    #endregion
                }
                else if ((ApprovalState)model.State == ApprovalState.ApprFailure)
                {
                    title = model.CreateUserName + "上传的项目图纸已被驳回，请处理";
                    #region 生成待办
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = model.CreateUserName + "上传的项目图纸已被驳回，请处理";
                    app.Content = model.CreateUserName + "上传的项目图纸已被驳回，请处理";
                    app.SendUserId = model.CreateUserId;
                    app.SendUserName = model.CreateUserName;
                    app.SendTime = model.CreateTime;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Draw.ToString();
                    app.Action = SystemRight.UnCheck.ToString();
                    app.BusinessTypeName = BusinessType.Draw.GetText();
                    app.BusinessState = (int)(ApprovalState.ApprFailure);
                    app.BusinessId = model.Id;
                    app.ApproverId = model.CreateUserId;
                    app.ApproverName = model.CreateUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = model.ProjectName;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Draw.GetText(), SystemRight.UnCheck.GetText(), "驳回图纸生成待办: " + model.Id);
                    #endregion

                    #region 发送短信
                    //WriteSMS(model.CreateUserId, 0, MessageStep.DrawReject, null);
                    #endregion
                }
                else if ((ApprovalState)model.State == ApprovalState.Discarded)
                {
                    title = model.CreateUserName + "上传了项目图纸，已废弃";
                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = model.CreateUserName + "上传了项目图纸，已废弃";
                    app.Content = model.CreateUserName + "上传了项目图纸，已废弃";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Draw.ToString();
                    app.Action = SystemRight.Invalid.ToString();
                    app.BusinessTypeName = BusinessType.Draw.GetText();
                    app.BusinessState = (int)(ApprovalState.Discarded);
                    app.BusinessId = model.Id;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Draw.GetText(), SystemRight.Invalid.GetText(), "废弃图纸生成待办: " + model.Id);
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
                    modelMsg.BussinesType = BusinessType.Draw.ToString();
                    modelMsg.ProjectId = model.ProjectId;
                    modelMsg.ProjectName = model.ProjectName;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                }
                #endregion
                var rows = DataOperateBusiness<Epm_Draw>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "RejectDraw");
            }
            return result;
        }
    }
}
