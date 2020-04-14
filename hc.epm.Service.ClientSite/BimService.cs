using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddBim(Epm_Bim model, List<Base_Files> fileListFile)
        {
            Result<int> result = new Result<int>();
            try
            {
                model = base.SetCurrentUser(model);
                model.CrtCompanyId = CurrentCompanyID.ToLongReq();
                model.CrtCompanyName = CurrentCompanyName;

                //bool dConfig = DataOperateBusiness<Epm_Bim>.Get().Count(i => i.Name == model.Name) > 0;
                //if (dConfig)
                //{
                //    throw new Exception("该模型名称已经存在");
                //}

                var rows = DataOperateBusiness<Epm_Bim>.Get().Add(model);

                //上传模型
                if (fileListFile != null)
                {
                    AddFilesByTable(model, fileListFile);
                }

                //生成代办消息
                if ((ApprovalState)model.State == ApprovalState.WaitAppr)
                {
                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "上传了BIM模型，待审核";
                    app.Content = CurrentUserName + "上传了BIM模型，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Model.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Model.GetText();
                    app.BusinessState = (int)(ApprovalState.WaitAppr);
                    app.BusinessId = model.Id;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Model.GetText(), SystemRight.Add.GetText(), "提交模型生成待办: " + model.Id);
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
                        modelMsg.Title = CurrentUserName + "上传了BIM模型，待审核";
                        modelMsg.Content = CurrentUserName + "上传了BIM模型，待审核";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Model.ToString();
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
                    //WriteSMS(project.ContactUserId.Value, project.CompanyId, MessageStep.ModelAdd, parameterSms);
                    #endregion
                }

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Model.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddBim");
            }
            return result;
        }

        ///<summary>
        ///修改
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateBim(Epm_Bim model, List<Base_Files> fileListFile)
        {
            Result<int> result = new Result<int>();
            try
            {
                //bool dConfig = DataOperateBusiness<Epm_Bim>.Get().Count(i => i.Name == model.Name && i.Id != model.Id) > 0;
                //if (dConfig)
                //{
                //    throw new Exception("该模型名称已经存在");
                //}

                var oldModel = DataOperateBusiness<Epm_Bim>.Get().GetModel(model.Id);
                model = FiterUpdate(oldModel, model);

                var rows = DataOperateBusiness<Epm_Bim>.Get().Update(model);

                //上传模型
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
                    app.Title = CurrentUserName + "上传了BIM模型，待审核";
                    app.Content = CurrentUserName + "上传了BIM模型，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Model.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Model.GetText();
                    app.BusinessState = (int)(ApprovalState.WaitAppr);
                    app.BusinessId = model.Id;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Model.GetText(), SystemRight.Add.GetText(), "提交模型生成待办: " + model.Id);
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
                        modelMsg.Title = CurrentUserName + "上传了BIM模型，待审核";
                        modelMsg.Content = CurrentUserName + "上传了BIM模型，待审核";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Model.ToString();
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
                    //WriteSMS(project.ContactUserId.Value, project.CompanyId, MessageStep.ModelAdd, parameterSms);
                    #endregion
                }

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Model.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateBim");
            }
            return result;
        }

        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteBimByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_Bim>.Get().GetList(i => ids.Contains(i.Id)).ToList();

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
                            modelMsg.Title = CurrentUserName + "上传的BIM模型已删除";
                            modelMsg.Content = CurrentUserName + "上传的BIM模型已删除";
                            modelMsg.Type = 2;
                            modelMsg.IsRead = false;
                            modelMsg.BussinessId = model.Id;
                            modelMsg.BussinesType = BusinessType.Model.ToString();
                            modelMsg.ProjectId = model.ProjectId.Value;
                            modelMsg.ProjectName = model.ProjectName;
                            modelMsg = base.SetCurrentUser(modelMsg);
                            modelMsg = base.SetCreateUser(modelMsg);
                            DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                        }
                        #endregion
                    }
                    var rows = DataOperateBusiness<Epm_Bim>.Get().DeleteRange(models);
                    result.Data = rows;
                    result.Flag = EResultFlag.Success;
                    WriteLog(BusinessType.Model.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
                }
                else
                {
                    throw new Exception("要删除的模型不存在或已被删除！");
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteBimByIds");
            }
            return result;
        }

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_Bim>> GetBimList(QueryCondition qc)
        {
            qc = AddDefaultWeb(qc);
            Result<List<Epm_Bim>> result = new Result<List<Epm_Bim>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Bim>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetBimList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_Bim> GetBimModel(long id)
        {
            Result<Epm_Bim> result = new Result<Epm_Bim>();
            try
            {
                var model = DataOperateBusiness<Epm_Bim>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetBimModel");
            }
            return result;
        }

        /// <summary>
        /// 根据ProjectId获取BIM模型列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Result<List<Epm_Bim>> GetBimModelListByProjectId(long projectId)
        {
            Result<List<Epm_Bim>> result = new Result<List<Epm_Bim>>();
            try
            {
                var model = DataOperateBusiness<Epm_Bim>.Get().GetList(p => p.ProjectId == projectId).ToList();
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetBimModelListByProjectId");
            }
            return result;
        }

        /// <summary>
        /// 修改状态（废弃）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeBimState(long id, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                var drawState = state.ToEnumReq<ApprovalState>();

                var model = DataOperateBusiness<Epm_Bim>.Get().GetModel(id);

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

                #region 生成待办
                var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                List<Epm_Approver> list = new List<Epm_Approver>();
                Epm_Approver app = new Epm_Approver();
                app.Title = model.CreateUserName + "上传了BIM模型，已废弃";
                app.Content = model.CreateUserName + "上传了BIM模型，已废弃";
                app.SendUserId = CurrentUserID.ToLongReq();
                app.SendUserName = CurrentUserName;
                app.SendTime = DateTime.Now;
                app.LinkURL = string.Empty;
                app.BusinessTypeNo = BusinessType.Model.ToString();
                app.Action = SystemRight.Invalid.ToString();
                app.BusinessTypeName = BusinessType.Model.GetText();
                app.BusinessState = (int)(ApprovalState.Discarded);
                app.BusinessId = model.Id;
                app.ApproverId = project.ContactUserId;
                app.ApproverName = project.ContactUserName;
                app.ProjectId = model.ProjectId;
                app.ProjectName = project.Name;
                list.Add(app);
                AddApproverBatch(list);
                WriteLog(BusinessType.Model.GetText(), SystemRight.Invalid.GetText(), "废弃模型生成待办: " + model.Id);
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
                    modelMsg.Title = model.CreateUserName + "上传了BIM模型，已废弃";
                    modelMsg.Content = model.CreateUserName + "上传了BIM模型，已废弃";
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = model.Id;
                    modelMsg.BussinesType = BusinessType.Model.ToString();
                    modelMsg.ProjectId = model.ProjectId.Value;
                    modelMsg.ProjectName = model.ProjectName;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                }
                #endregion
                var rows = DataOperateBusiness<Epm_Bim>.Get().Update(model);

                result.Data = rows;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "ChangeBimState");
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
        public Result<int> RejectBim(long id, string state, string reason)
        {
            Result<int> result = new Result<int>();
            try
            {
                var drawState = state.ToEnumReq<ApprovalState>();

                var model = DataOperateBusiness<Epm_Bim>.Get().GetModel(id);

                //处理待办
                var temp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == model.Id && t.IsApprover == false).FirstOrDefault();
                if (temp != null)
                {
                    ComplateApprover(temp.Id);
                }
                model.OperateUserId = CurrentUserID.ToLongReq();
                model.OperateUserName = CurrentUserName;
                model.OperateTime = DateTime.Now;
                model.State = int.Parse(drawState.GetValue().ToString());

                string title = "";
                if ((ApprovalState)Enum.ToObject(typeof(ApprovalState), model.State) == ApprovalState.ApprSuccess)
                {
                    //每一次审核通过都修改本次IsValidate的值为1,之前已经存在的IsValidate为1的状态修改为0；
                    model.IsValidate = true;

                    var list = DataOperateBusiness<Epm_Bim>.Get().GetList(t => t.ProjectId == model.ProjectId && t.Id != model.Id).ToList();
                    if (list.Count > 0)
                    {
                        foreach (var item in list)
                        {
                            item.IsValidate = false;
                        }

                        DataOperateBusiness<Epm_Bim>.Get().UpdateRange(list);
                    }
                    
                    title= model.CreateUserName + "上传的BIM模型，审核通过";

                    #region 生成待办
                    List<Epm_Approver> listApp = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = model.CreateUserName + "上传的BIM模型，审核通过";
                    app.Content = model.CreateUserName + "上传的BIM模型，审核通过";
                    app.SendUserId = model.CreateUserId;
                    app.SendUserName = model.CreateUserName;
                    app.SendTime = model.CreateTime;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Model.ToString();
                    app.Action = SystemRight.Check.ToString();
                    app.BusinessTypeName = BusinessType.Model.GetText();
                    app.BusinessState = (int)(ApprovalState.ApprSuccess);
                    app.BusinessId = model.Id;
                    app.ApproverId = model.CreateUserId;
                    app.ApproverName = model.CreateUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = model.ProjectName;
                    listApp.Add(app);
                    AddApproverBatch(listApp);
                    WriteLog(BusinessType.Model.GetText(), SystemRight.Check.GetText(), "审核通过模型生成待办: " + model.Id);
                    #endregion
                }
                else if ((ApprovalState)Enum.ToObject(typeof(ApprovalState), model.State) == ApprovalState.ApprFailure)
                {
                    title = model.CreateUserName + "上传的BIM模型已被驳回，请处理";

                    #region 生成待办
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = model.CreateUserName + "上传的BIM模型已被驳回，请处理";
                    app.Content = model.CreateUserName + "上传的BIM模型已被驳回，请处理";
                    app.SendUserId = model.CreateUserId;
                    app.SendUserName = model.CreateUserName;
                    app.SendTime = model.CreateTime;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Model.ToString();
                    app.Action = SystemRight.UnCheck.ToString();
                    app.BusinessTypeName = BusinessType.Model.GetText();
                    app.BusinessState = (int)(ApprovalState.ApprFailure);
                    app.BusinessId = model.Id;
                    app.ApproverId = model.CreateUserId;
                    app.ApproverName = model.CreateUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = model.ProjectName;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Model.GetText(), SystemRight.UnCheck.GetText(), "驳回模型生成待办: " + model.Id);
                    #endregion

                    #region 发送短信
                    //WriteSMS(model.CreateUserId, 0, MessageStep.ModelReject, null);
                    #endregion
                }
                var rows = DataOperateBusiness<Epm_Bim>.Get().Update(model);

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
                    modelMsg.BussinesType = BusinessType.Model.ToString();
                    modelMsg.ProjectId = model.ProjectId.Value;
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
                result.Exception = new ExceptionEx(ex, "RejectBim");
            }
            return result;
        }

        /// <summary>
        /// 生成BIM模型图片
        /// </summary>
        /// <param name="id"></param>
        /// <param name="img"></param>
        /// <returns></returns>
        public Result<int> CreateImgBim(long id, string img, List<Base_Files> fileList)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBusiness<Epm_Bim>.Get().GetModel(id);
                if (model != null && string.IsNullOrEmpty(model.BIMImg))
                {
                    model.OperateUserId = CurrentUserID.ToLongReq();
                    model.OperateUserName = CurrentUserName;
                    model.OperateTime = DateTime.Now;
                    model.BIMImg = img;

                    if (fileList.Count > 0)
                    {
                        AddFilesByTable(model, fileList);
                    }
                    var rows = DataOperateBusiness<Epm_Bim>.Get().Update(model);
                    result.Data = rows;
                    result.Flag = EResultFlag.Success;
                }
                else
                {
                    result.Data = -1;
                    result.Flag = EResultFlag.Success;
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "CreateImgBim");
            }
            return result;
        }

        /// <summary>
        /// 获取首页展示模型列表
        /// </summary>
        /// <returns></returns>
        public Result<List<Epm_Bim>> GetBimModelIndexList()
        {
            Result<List<Epm_Bim>> result = new Result<List<Epm_Bim>>();
            try
            {
                var tempQuery = from a in context.Epm_Bim
                                where a.IsDelete == false
                                group a by a.ProjectId
                                into g
                                select new
                                {
                                    ProjectId = g.Key,
                                    CreateTime = g.Max(x => x.CreateTime)
                                };

                var list = (from a in tempQuery
                            join b in context.Epm_Bim.Where(p => p.IsDelete == false)
                                on new { a.ProjectId, a.CreateTime } equals new { b.ProjectId, b.CreateTime } into bref
                            from b in bref.DefaultIfEmpty()
                            select b).ToList();

                list.Skip(0).Take(10).OrderByDescending(t => t.CreateTime).ToList();

                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.AllRowsCount = 0;
            }
            return result;
        }

        /// <summary>
        /// 获取模型属性
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Result<DataSet> GetBimProperty(string path, string SQLString)
        {
            Result<DataSet> result = new Result<DataSet>();
            //string str = @"Data Source='http://192.168.1.239:8088/Tools/output/XRJYZ.db';Version=3;";
            //db文件不可以远程访问，所有在服务器做API提供方法  
            string str = "Data Source='" + ConfigurationManager.AppSettings["BIMoutput"] + path + "';Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(str))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    SQLiteDataAdapter command = new SQLiteDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (System.Data.SQLite.SQLiteException ex)
                {
                    result.Data = null;
                    result.Flag = EResultFlag.Failure;
                }

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    result.Data = ds;
                    result.Flag = EResultFlag.Success;
                }
                else
                {
                    result.Data = null;
                    result.Flag = EResultFlag.Failure;
                }
                return result;
            }
        }
    }
}
