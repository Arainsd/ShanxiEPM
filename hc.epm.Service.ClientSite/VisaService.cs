using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using hc.Plat.Common.Service;
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
        public Result<int> AddVisa(VisaView model, List<Base_Files> fileList)
        {
            Result<int> result = new Result<int>();
            try
            {
                Epm_Visa visa = new Epm_Visa();
                List<Epm_VisaCompany> companys = new List<Epm_VisaCompany>();
                model.Id = visa.Id;
                ViewToEmp(model, out visa, out companys);

                var rows = DataOperateBusiness<Epm_Visa>.Get().Add(visa);
                DataOperateBusiness<Epm_VisaCompany>.Get().AddRange(companys);
                if (model.State == (int)ApprovalState.WaitAppr)
                {
                    if (companys.Any() && companys.Count > 0)
                    {
                        for (int i = 0; i < companys.Count; i++)
                        {
                            var comID = companys[i].CompanyId;
                            var temp = DataOperateBusiness<Epm_ProjectStateTrack>.Get().GetList(t => t.CompanyId == comID).FirstOrDefault();
                            if (temp != null)
                            {
                                temp.Qty = (Convert.ToInt32(temp.Qty) + 1).ToString();

                                DataOperateBusiness<Epm_ProjectStateTrack>.Get().Update(temp);
                            }
                        }
                    }

                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "提报了隐蔽工程作业单，待审核";
                    app.Content = CurrentUserName + "提报了隐蔽工程作业单，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Visa.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Visa.GetText();
                    app.BusinessState = (int)(ApprovalState.WaitAppr);
                    app.BusinessId = visa.Id;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Visa.GetText(), SystemRight.Add.GetText(), "提交签证生成待办: " + visa.Id);
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
                        modelMsg.Title = CurrentUserName + "提报了隐蔽工程作业单，待审核";
                        modelMsg.Content = CurrentUserName + "提报了隐蔽工程作业单，待审核";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Visa.ToString();
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
                    //WriteSMS(project.ContactUserId.Value, project.CompanyId, MessageStep.VisaAdd, parameterssm);
                    #endregion
                }

                if (fileList != null)
                {
                    //新增附件
                    AddFilesByTable(visa, fileList);
                }
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Visa.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddVisa");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateVisa(VisaView model, List<Base_Files> fileList)
        {
            Result<int> result = new Result<int>();
            try
            {
                Epm_Visa visa = new Epm_Visa();
                List<Epm_VisaCompany> companys = new List<Epm_VisaCompany>();
                ViewToEmp(model, out visa, out companys);

                var rows = DataOperateBusiness<Epm_Visa>.Get().Update(visa);

                var list = DataOperateBusiness<Epm_VisaCompany>.Get().GetList(t => t.VisaId == visa.Id).ToList();
                if (list != null)
                    DataOperateBusiness<Epm_VisaCompany>.Get().DeleteRange(list);
                DataOperateBusiness<Epm_VisaCompany>.Get().AddRange(companys);


                if (fileList != null)
                {
                    //删除之前的附件
                    DeleteFilesByTable(visa.GetType().Name, new List<long>() { model.Id });
                    //新增附件
                    AddFilesByTable(visa, fileList);
                }

                //处理待办
                var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == visa.Id && t.IsApprover == false).FirstOrDefault();
                if (tempApp != null)
                {
                    ComplateApprover(tempApp.Id);
                }

                if (model.State == (int)ApprovalState.WaitAppr)
                {
                    if (companys.Any() && companys.Count > 0)
                    {
                        for (int i = 0; i < companys.Count; i++)
                        {
                            var comID = companys[i].CompanyId;
                            var temp = DataOperateBusiness<Epm_ProjectStateTrack>.Get().GetList(t => t.CompanyId == comID).FirstOrDefault();
                            if (temp != null)
                            {
                                temp.Qty = (Convert.ToInt32(temp.Qty) + 1).ToString();

                                DataOperateBusiness<Epm_ProjectStateTrack>.Get().Update(temp);
                            }
                        }
                    }

                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                    List<Epm_Approver> listApp = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "提报了隐蔽工程作业单，待审核";
                    app.Content = CurrentUserName + "提报了隐蔽工程作业单，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Visa.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Visa.GetText();
                    app.BusinessState = (int)(ApprovalState.WaitAppr);
                    app.BusinessId = visa.Id;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = project.Name;
                    listApp.Add(app);
                    AddApproverBatch(listApp);
                    WriteLog(BusinessType.Visa.GetText(), SystemRight.Add.GetText(), "提交签证生成待办: " + visa.Id);
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
                        modelMsg.Title = CurrentUserName + "提报了隐蔽工程作业单，待审核";
                        modelMsg.Content = CurrentUserName + "提报了隐蔽工程作业单，待审核";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Visa.ToString();
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
                    //WriteSMS(project.ContactUserId.Value, project.CompanyId, MessageStep.VisaAdd, parameterssm);
                    #endregion
                }
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Visa.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateVisa");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteVisaByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_Visa>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                if (models.Any(t => t.State != (int)ApprovalState.Discarded && t.State != (int)ApprovalState.Enabled))
                {
                    throw new Exception("草稿，已废弃状态下，才可删除");
                }

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
                        modelMsg.Title = CurrentUserName + "提报的签证单已删除";
                        modelMsg.Content = CurrentUserName + "提报的签证单已删除";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Visa.ToString();
                        modelMsg.ProjectId = model.ProjectId.Value;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion
                }
                var rows = DataOperateBusiness<Epm_Visa>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Visa.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteVisaByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_Visa>> GetVisaList(string projectName, string title, int state, string visaTypeName, int pageIndex, int pageSize)
        {
            Result<List<Epm_Visa>> result = new Result<List<Epm_Visa>>();
            try
            {
                var currentUserID = CurrentUserID.ToLongReq();
                int statu = (int)ApprovalState.Enabled;
                var query = from b in context.Epm_Visa.Where(p => p.IsDelete == false)
                            where CurrentProjectIds.Contains(b.ProjectId.ToString())
                        && (b.ProjectName.Contains(projectName) || string.IsNullOrEmpty(projectName))
                        && (b.VisaTypeName.Contains(visaTypeName) || string.IsNullOrEmpty(visaTypeName))
                        && (b.VisaTitle.Contains(title) || string.IsNullOrEmpty(title))
                        && (b.State == state || state == -1)
                        && ((b.State == statu && b.CreateUserId == currentUserID) || b.State != statu)
                            select b;
                //query = query.Distinct();

                int count = query.Count();
                int skip = (pageIndex - 1) * pageSize;
                int take = pageSize;

                var list = query.OrderByDescending(p => p.OperateTime).Skip(skip).Take(take).ToList();
                result.Data = list;
                result.AllRowsCount = count;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetVisaList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<VisaView> GetVisaModel(long id)
        {
            Result<VisaView> result = new Result<VisaView>();
            try
            {
                var model = DataOperateBusiness<Epm_Visa>.Get().GetModel(id);
                var companys = DataOperateBusiness<Epm_VisaCompany>.Get().GetList(t => model.Id == t.VisaId).ToList();
                result.Data = EmpToView(model, companys);
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetVisaModel");
            }
            return result;
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateVisaState(long id, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBusiness<Epm_Visa>.Get().GetModel(id);
                if (model != null)
                {
                    model.OperateUserId = CurrentUserID.ToLongReq();
                    model.OperateUserName = CurrentUserName;
                    model.OperateTime = DateTime.Now;
                    model.State = (int)state.ToEnumReq<ApprovalState>();
                    var rows = DataOperateBusiness<Epm_Visa>.Get().Update(model);
                    result.Data = rows;
                    result.Flag = EResultFlag.Success;

                    WriteLog(BusinessType.Change.GetText(), SystemRight.Delete.GetText(), "更新状态: " + rows);

                    //处理待办
                    var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == model.Id && t.IsApprover == false).FirstOrDefault();
                    if (tempApp != null)
                    {
                        ComplateApprover(tempApp.Id);
                    }
                    string title = "";
                    if (model.State == (int)ApprovalState.Discarded)
                    {
                        var list = DataOperateBusiness<Epm_VisaCompany>.Get().GetList(t => t.VisaId == model.Id).ToList();
                        if (list.Any())
                        {
                            for (int i = 0; i < list.Count; i++)
                            {
                                var comID = list[i].CompanyId;
                                var temp = DataOperateBusiness<Epm_ProjectStateTrack>.Get().GetList(t => t.CompanyId == comID).FirstOrDefault();

                                temp.Qty = (Convert.ToInt32(temp.Qty) - 1).ToString();

                                DataOperateBusiness<Epm_ProjectStateTrack>.Get().Update(temp);
                            }
                        }
                        title = model.CreateUserName + "提报了签证单，已废弃";
                        #region 生成待办
                        var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                        List<Epm_Approver> listApp = new List<Epm_Approver>();
                        Epm_Approver app = new Epm_Approver();
                        app.Title = model.CreateUserName + "提报了签证单，已废弃";
                        app.Content = model.CreateUserName + "提报了签证单，已废弃";
                        app.SendUserId = CurrentUserID.ToLongReq();
                        app.SendUserName = CurrentUserName;
                        app.SendTime = DateTime.Now;
                        app.LinkURL = string.Empty;
                        app.BusinessTypeNo = BusinessType.Visa.ToString();
                        app.Action = SystemRight.Invalid.ToString();
                        app.BusinessTypeName = BusinessType.Visa.GetText();
                        app.BusinessState = (int)(ApprovalState.Discarded);
                        app.BusinessId = model.Id;
                        app.ApproverId = project.ContactUserId;
                        app.ApproverName = project.ContactUserName;
                        app.ProjectId = model.ProjectId;
                        app.ProjectName = project.Name;
                        listApp.Add(app);
                        AddApproverBatch(listApp);
                        WriteLog(BusinessType.Visa.GetText(), SystemRight.Invalid.GetText(), "废弃签证生成待办: " + model.Id);
                        #endregion
                    }

                    if ((ApprovalState)Enum.ToObject(typeof(ApprovalState), model.State) == ApprovalState.ApprSuccess)
                    {
                        #region 更改项目状态表
                        var companys = DataOperateBusiness<Epm_VisaCompany>.Get().GetList(t => t.VisaId == model.Id).ToList();
                        if (companys.Any())
                        {
                            var companyIds = companys.Select(t => t.CompanyId);

                            var track = DataOperateBusiness<Epm_ProjectStateTrack>.Get().GetList(t => t.ProjectId == model.ProjectId && companyIds.Contains(t.CompanyId) && t.StateTrackNo == "VisaNum");
                            if (track.Any())
                            {
                                foreach (var item in track)
                                {
                                    int qty = Convert.ToInt32(item.Qty) + 1;
                                    item.Qty = qty.ToString();
                                }
                                DataOperateBusiness<Epm_ProjectStateTrack>.Get().UpdateRange(track);
                            }
                        }
                        #endregion
                        
                        #region 生成待办
                        List<Epm_Approver> list = new List<Epm_Approver>();
                        Epm_Approver app = new Epm_Approver();
                        app.Title = model.CreateUserName + "提报的隐蔽工程作业单已审核通过";
                        app.Content = model.CreateUserName + "提报的隐蔽工程作业单已审核通过";
                        app.SendUserId = model.CreateUserId;
                        app.SendUserName = model.CreateUserName;
                        app.SendTime = model.CreateTime;
                        app.LinkURL = string.Empty;
                        app.BusinessTypeNo = BusinessType.Visa.ToString();
                        app.Action = SystemRight.Check.ToString();
                        app.BusinessTypeName = BusinessType.Visa.GetText();
                        app.BusinessState = (int)(ApprovalState.ApprSuccess);
                        app.BusinessId = model.Id;
                        app.ApproverId = model.CreateUserId;
                        app.ApproverName = model.CreateUserName;
                        app.ProjectId = model.ProjectId;
                        app.ProjectName = model.ProjectName;
                        list.Add(app);
                        AddApproverBatch(list);
                        WriteLog(BusinessType.Visa.GetText(), SystemRight.Check.GetText(), "审核通过签证生成待办: " + model.Id);
                        #endregion
                    }

                    if ((ApprovalState)Enum.ToObject(typeof(ApprovalState), model.State) == ApprovalState.ApprFailure)
                    {
                        title = model.CreateUserName + "提报的签证单已被驳回，请处理";
                        #region 生成待办
                        List<Epm_Approver> list = new List<Epm_Approver>();
                        Epm_Approver app = new Epm_Approver();
                        app.Title = model.CreateUserName + "提报的签证单已被驳回，请处理";
                        app.Content = model.CreateUserName + "提报的签证单已被驳回，请处理";
                        app.SendUserId = model.CreateUserId;
                        app.SendUserName = model.CreateUserName;
                        app.SendTime = model.CreateTime;
                        app.LinkURL = string.Empty;
                        app.BusinessTypeNo = BusinessType.Visa.ToString();
                        app.Action = SystemRight.UnCheck.ToString();
                        app.BusinessTypeName = BusinessType.Visa.GetText();
                        app.BusinessState = (int)(ApprovalState.ApprFailure);
                        app.BusinessId = model.Id;
                        app.ApproverId = model.CreateUserId;
                        app.ApproverName = model.CreateUserName;
                        app.ProjectId = model.ProjectId;
                        app.ProjectName = model.ProjectName;
                        list.Add(app);
                        AddApproverBatch(list);
                        WriteLog(BusinessType.Visa.GetText(), SystemRight.UnCheck.GetText(), "驳回签证生成待办: " + model.Id);
                        #endregion

                        #region 发送短信
                        //WriteSMS(model.CreateUserId, 0, MessageStep.VisaReject, null);
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
                        modelMsg.BussinesType = BusinessType.Visa.ToString();
                        modelMsg.ProjectId = model.ProjectId.Value;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion
                }
                else
                {
                    throw new Exception("id有误");
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateState");
            }
            return result;
        }
        private VisaView EmpToView(Epm_Visa visa, List<Epm_VisaCompany> companys)
        {
            VisaView model = new VisaView();
            model.ChangeId = visa.ChangeId;
            model.ChangeName = visa.ChangeName;
            model.CrtCompanyId = visa.CrtCompanyId;
            model.CrtCompanyName = visa.CrtCompanyName;
            model.Id = visa.Id;
            model.ProjectId = visa.ProjectId;
            model.ProjectName = visa.ProjectName;
            model.Remark = visa.Remark;
            model.State = visa.State;
            model.SubmitTime = visa.SubmitTime;
            model.SubmitUserId = visa.SubmitUserId;
            model.SubmitUserName = visa.SubmitUserName;
            model.VisaAmount = visa.VisaAmount;
            model.VisaContent = visa.VisaContent;
            model.VisaDays = visa.VisaDays;
            model.VisaEndTime = visa.VisaEndTime;
            model.VisaNo = visa.VisaNo;
            model.VisaResean = visa.VisaResean;
            model.VisaStartTime = visa.VisaStartTime;
            model.VisaTitle = visa.VisaTitle;
            model.CompanyIds = string.Join(",", companys.Select(t => t.CompanyId));
            model.CompanyNames = string.Join(",", companys.Select(t => t.CompanyName));
            model.Epm_VisaCompany = companys;
            model.VisaTypeName = visa.VisaTypeName ?? "";
            model.CreateTime = visa.CreateTime;
            model.CreateUserId = visa.CreateUserId;
            model.CreateUserName = visa.CreateUserName;
            return model;
        }
        private void ViewToEmp(VisaView view, out Epm_Visa visa, out List<Epm_VisaCompany> companys)
        {
            visa = new Epm_Visa();
            base.SetCurrentUser(visa);
            visa.ChangeId = view.ChangeId ?? 0;
            visa.ChangeName = view.ChangeName ?? "";
            visa.CrtCompanyId = CurrentCompanyID.ToLongReq();
            visa.CrtCompanyName = CurrentCompanyName ?? "";
            visa.Id = view.Id;
            visa.VisaAmount = view.VisaAmount ?? 0;
            visa.VisaTitle = view.VisaTitle ?? "";
            visa.VisaContent = view.VisaContent ?? "";
            visa.VisaDays = view.VisaDays ?? 0;
            visa.VisaEndTime = view.VisaEndTime;
            visa.VisaNo = view.VisaNo ?? "";
            visa.VisaResean = view.VisaResean ?? "";
            visa.VisaStartTime = view.VisaStartTime;
            visa.ProjectId = view.ProjectId ?? 0;
            visa.ProjectName = view.ProjectName ?? "";
            visa.Remark = view.Remark ?? "";
            visa.State = view.State;
            visa.SubmitUserId = view.SubmitUserId;
            visa.SubmitUserName = view.SubmitUserName ?? "";
            visa.VisaTypeName = view.VisaTypeName ?? "";

            companys = new List<Epm_VisaCompany>();
            if (view.Epm_VisaCompany != null)
            {
                foreach (var item in view.Epm_VisaCompany)
                {
                    var comany = new Epm_VisaCompany();
                    base.SetCurrentUser(comany);
                    comany.CrtCompanyId = CurrentCompanyID.ToLongReq();
                    comany.CrtCompanyName = CurrentCompanyName;
                    comany.CompanyId = item.CompanyId;
                    comany.CompanyName = item.CompanyName;
                    comany.VisaId = view.Id;
                    comany.CompanyType = item.CompanyType;
                    companys.Add(comany);
                }
            }
        }
    }
}
