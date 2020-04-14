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
        public Result<int> AddChange(ChangeView model, List<Base_Files> fileList = null)
        {
            Result<int> result = new Result<int>();
            try
            {
                Epm_Change change = new Epm_Change();
                List<Epm_ChangeCompany> companys = new List<Epm_ChangeCompany>();
                model.Id = change.Id;
                ViewToEmp(model, out change, out companys);

                bool dConfig = DataOperateBusiness<Epm_Change>.Get().Count(i => i.ProjectId == change.ProjectId && i.State != (int)ApprovalState.ApprSuccess) > 0;
                if (dConfig)
                {
                    throw new Exception("已存在该项目的变更申请！");
                }

                var dictionaryList = DataOperateBasic<Base_TypeDictionary>.Get().GetList(t => t.Type == DictionaryType.ChangeRatio.ToString() && t.No == "ReduceRatio").ToList().FirstOrDefault();
                if (change.ChangeAmount < 0 && change.State == (int)ApprovalState.WaitAppr)
                {
                    int ratio = Convert.ToInt32(dictionaryList.Name);
                    if ((-change.ChangeAmount) < change.TotalAmount * ((decimal)ratio / 100))
                    {
                        change.State = (int)ApprovalState.ApprSuccess;
                        model.State = (int)ApprovalState.ApprSuccess;
                    }
                }
                var rows = DataOperateBusiness<Epm_Change>.Get().Add(change);
                DataOperateBusiness<Epm_ChangeCompany>.Get().AddRange(companys);

                //新增附件
                AddFilesByTable(change, fileList);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Change.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);

                if (model.State == (int)ApprovalState.ApprSuccess)
                {
                    var project = DataOperateBusiness<Epm_Project>.Get().GetList(t => t.Id == model.ProjectId).FirstOrDefault();
                    if (project != null)
                    {
                        project.Amount = model.TotalAmount + model.ChangeAmount;
                        DataOperateBusiness<Epm_Project>.Get().Update(project);
                    }
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
                    WriteLog(BusinessType.Change.GetText(), SystemRight.Add.GetText(), "生成消息: " + model.Id);

                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                    #region 生成待办
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "提交了变更申请，待审核";
                    app.Content = CurrentUserName + "提交了变更申请，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Change.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Change.GetText();
                    app.BusinessState = (int)(ApprovalState.WaitAppr);
                    app.BusinessId = model.Id;
                    app.ApproverId = project.PMId;
                    app.ApproverName = project.PMName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Change.GetText(), SystemRight.Add.GetText(), "提交变更生成待办: " + model.Id);
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
                        modelMsg.Title = CurrentUserName + "提交了变更申请，待审核";
                        modelMsg.Content = CurrentUserName + "提交了变更申请，待审核";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Change.ToString();
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
                    //WriteSMS(project.PMId.Value, 0, MessageStep.ChangeAdd, parameterSms);
                    #endregion
                }

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddChange");
            }
            return result;
        }

        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateChange(ChangeView model, List<Base_Files> fileList = null)
        {
            Result<int> result = new Result<int>();
            try
            {
                Epm_Change change = new Epm_Change();
                List<Epm_ChangeCompany> companys = new List<Epm_ChangeCompany>();
                ViewToEmp(model, out change, out companys);

                var dictionaryList = DataOperateBasic<Base_TypeDictionary>.Get().GetList(t => t.Type == DictionaryType.ChangeRatio.ToString() && t.No == "ReduceRatio").ToList().FirstOrDefault();
                if (change.ChangeAmount < 0 && change.State == (int)ApprovalState.WaitAppr)
                {
                    int ratio = Convert.ToInt32(dictionaryList.Name);
                    if ((-change.ChangeAmount) < change.TotalAmount * ((decimal)ratio / 100))
                    {
                        change.State = (int)ApprovalState.ApprSuccess;
                        model.State = (int)ApprovalState.ApprSuccess;
                    }
                }

                var rows = DataOperateBusiness<Epm_Change>.Get().Update(change);

                //处理待办
                var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == change.Id && t.IsApprover == false).FirstOrDefault();
                if (tempApp != null)
                {
                    ComplateApprover(tempApp.Id);
                }

                var list = DataOperateBusiness<Epm_ChangeCompany>.Get().GetList(t => t.ChangeId == change.Id).ToList();
                if (list != null)
                    DataOperateBusiness<Epm_TrainCompany>.Get().DeleteRange(list);
                DataOperateBusiness<Epm_ChangeCompany>.Get().AddRange(companys);

                if (fileList != null)
                {
                    //删除之前的附件
                    DeleteFilesByTable(change.GetType().Name, new List<long>() { model.Id });
                    //新增附件
                    AddFilesByTable(change, fileList);
                }
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Change.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);

                if (model.State == (int)ApprovalState.ApprSuccess)
                {
                    var project = DataOperateBusiness<Epm_Project>.Get().GetList(t => t.Id == model.ProjectId).FirstOrDefault();
                    if (project != null)
                    {
                        project.Amount = model.TotalAmount + model.ChangeAmount;
                        DataOperateBusiness<Epm_Project>.Get().Update(project);
                    }
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
                    WriteLog(BusinessType.Change.GetText(), SystemRight.Add.GetText(), "生成消息: " + model.Id);

                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                    List<Epm_Approver> listApp = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "提交了变更申请，待审核";
                    app.Content = CurrentUserName + "提交了变更申请，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Change.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Change.GetText();
                    app.BusinessState = (int)(ApprovalState.WaitAppr);
                    app.BusinessId = model.Id;
                    app.ApproverId = project.PMId;
                    app.ApproverName = project.PMName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = project.Name;
                    listApp.Add(app);
                    AddApproverBatch(listApp);
                    WriteLog(BusinessType.Change.GetText(), SystemRight.Add.GetText(), "提交变更生成待办: " + model.Id);
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
                        modelMsg.Title = CurrentUserName + "提交了变更申请，待审核";
                        modelMsg.Content = CurrentUserName + "提交了变更申请，待审核";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Change.ToString();
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
                    //WriteSMS(project.PMId.Value, 0, MessageStep.ChangeAdd, parameterSms);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateChange");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteChangeByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_Change>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                if (models.Any(t => t.State != (int)ApprovalState.Discarded && t.State != (int)ApprovalState.Enabled))
                {
                    throw new Exception("草稿，已废弃状态下，才可删除");
                }
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
                            modelMsg.Title = CurrentUserName + "提交的变更申请已删除";
                            modelMsg.Content = CurrentUserName + "提交的变更申请已删除";
                            modelMsg.Type = 2;
                            modelMsg.IsRead = false;
                            modelMsg.BussinessId = model.Id;
                            modelMsg.BussinesType = BusinessType.Change.ToString();
                            modelMsg.ProjectId = model.ProjectId.Value;
                            modelMsg.ProjectName = model.ProjectName;
                            modelMsg = base.SetCurrentUser(modelMsg);
                            modelMsg = base.SetCreateUser(modelMsg);
                            DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                        }
                        #endregion
                    }

                    var rows = DataOperateBusiness<Epm_Change>.Get().DeleteRange(models);
                    result.Data = rows;
                    result.Flag = EResultFlag.Success;
                    WriteLog(BusinessType.Change.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
                }
                else
                {
                    throw new Exception("要删除的变更合同信息不存在或已被删除！");
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteChangeByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<ChangeView>> GetChangeList(string projectName, string name, int state, int pageIndex, int pageSize)
        {
            Result<List<ChangeView>> result = new Result<List<ChangeView>>();
            try
            {
                var currentUserID = CurrentUserID.ToLongReq();
                int statu = (int)ApprovalState.Enabled;
                var query = from b in context.Epm_Change.Where(p => p.IsDelete == false)
                            where CurrentProjectIds.Contains(b.ProjectId.ToString())
                        && (b.ProjectName.Contains(projectName) || string.IsNullOrEmpty(projectName))
                        && (b.ChangeName.Contains(name) || string.IsNullOrEmpty(name))
                        && (b.State == state || state == -1)
                        && ((b.State == statu && b.CreateUserId == currentUserID) || b.State != statu)
                            select b;

                int count = query.Count();
                int skip = (pageIndex - 1) * pageSize;
                int take = pageSize;

                var list = query.OrderByDescending(p => p.OperateTime).Skip(skip).Take(take).Select(chang =>
                new ChangeView
                {
                    Id = chang.Id,
                    TotalAmount = chang.TotalAmount,
                    AddAmount = chang.AddAmount,
                    ReduceAmount = chang.ReduceAmount,
                    ChangeAmount = chang.ChangeAmount,
                    ChangeContent = chang.ChangeContent,
                    ChangeDays = chang.ChangeDays,
                    ChangeName = chang.ChangeName,
                    ChangeNo = chang.ChangeNo,
                    ChangeReason = chang.ChangeReason,
                    ChangeStartTime = chang.ChangeStartTime,
                    ChangeEndTime = chang.ChangeEndTime,
                    ProjectId = chang.ProjectId,
                    ProjectName = chang.ProjectName,
                    Remark = chang.Remark,
                    State = chang.State,
                    SubmitUserId = chang.SubmitUserId,
                    SubmitUserName = chang.SubmitUserName,
                    CreateTime = chang.CreateTime,
                    CrtCompanyName = chang.CrtCompanyName
                }).ToList();

                result.Data = list;
                result.AllRowsCount = count;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetChangeList");
            }
            return result;
        }

        private ChangeView EmpToView(Epm_Change chang, List<Epm_ChangeCompany> companys)
        {
            ChangeView model = new ChangeView();
            model.Id = chang.Id;
            model.ChangeAmount = chang.ChangeAmount;
            model.ChangeContent = chang.ChangeContent;
            model.ChangeDays = chang.ChangeDays;
            model.ChangeEndTime = chang.ChangeEndTime;
            model.ChangeName = chang.ChangeName;
            model.ChangeNo = chang.ChangeNo;
            model.ChangeReason = chang.ChangeReason;
            model.ChangeStartTime = chang.ChangeStartTime;
            model.CompanyIds = string.Join(",", companys.Select(t => t.CompanyId));
            model.CompanyNames = string.Join(",", companys.Select(t => t.CompanyName));
            model.ProjectId = chang.ProjectId;
            model.ProjectName = chang.ProjectName;
            model.Remark = chang.Remark;
            model.State = chang.State;
            model.SubmitUserId = chang.SubmitUserId;
            model.SubmitUserName = chang.SubmitUserName;
            model.CreateTime = chang.CreateTime;
            model.CrtCompanyName = chang.CrtCompanyName;
            model.AddAmount = chang.AddAmount;
            model.TotalAmount = chang.TotalAmount;
            model.ReduceAmount = chang.ReduceAmount;
            model.Epm_ChangeCompany = companys;
            model.CreateUserId = chang.CreateUserId;
            model.CreateUserName = chang.CreateUserName;
            return model;
        }
        private void ViewToEmp(ChangeView view, out Epm_Change change, out List<Epm_ChangeCompany> companys)
        {
            change = new Epm_Change();
            base.SetCurrentUser(change);
            change.CrtCompanyId = CurrentCompanyID.ToLongReq();
            change.CrtCompanyName = CurrentCompanyName;
            change.Id = view.Id;
            change.ChangeAmount = view.ChangeAmount;
            change.ChangeContent = view.ChangeContent;
            change.ChangeDays = view.ChangeDays;
            change.ChangeEndTime = view.ChangeEndTime;
            change.ChangeName = view.ChangeName;
            change.ChangeNo = view.ChangeNo;
            change.ChangeReason = view.ChangeReason;
            change.ChangeStartTime = view.ChangeStartTime;
            change.ProjectId = view.ProjectId;
            change.ProjectName = view.ProjectName;
            change.Remark = view.Remark;
            change.State = view.State;
            change.SubmitUserId = view.SubmitUserId;
            change.SubmitUserName = view.SubmitUserName;
            change.CreateTime = view.CreateTime;
            change.CrtCompanyName = view.CrtCompanyName;
            change.AddAmount = view.AddAmount;
            change.TotalAmount = view.TotalAmount;
            change.ReduceAmount = view.ReduceAmount;

            companys = new List<Epm_ChangeCompany>();
            if (view.Epm_ChangeCompany != null)
            {
                foreach (var item in view.Epm_ChangeCompany)
                {
                    var comany = new Epm_ChangeCompany();
                    base.SetCurrentUser(comany);
                    comany.CrtCompanyId = CurrentCompanyID.ToLongReq();
                    comany.CrtCompanyName = CurrentCompanyName;
                    comany.CompanyId = item.CompanyId;
                    comany.CompanyName = item.CompanyName;
                    comany.ChangeId = view.Id;
                    comany.CompanyType = item.CompanyType;
                    companys.Add(comany);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_Change>> GetChangListByQc(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_Change>> result = new Result<List<Epm_Change>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Change>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetChangListByQc");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<ChangeView> GetChangeModel(long id)
        {
            Result<ChangeView> result = new Result<ChangeView>();
            try
            {
                var model = DataOperateBusiness<Epm_Change>.Get().GetModel(id);
                if (model == null)
                {
                    throw new Exception("详情不存在或已被删除！");
                }

                var companys = DataOperateBusiness<Epm_ChangeCompany>.Get().GetList(t => model.Id == t.ChangeId).ToList();
                result.Data = EmpToView(model, companys);
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetChangeModel");
            }
            return result;
        }
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateChangeState(long id, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBusiness<Epm_Change>.Get().GetModel(id);
                if (model != null)
                {
                    model.OperateUserId = CurrentUserID.ToLongReq();
                    model.OperateUserName = CurrentUserName;
                    model.OperateTime = DateTime.Now;
                    model.State = (int)state.ToEnumReq<ApprovalState>();
                    var rows = DataOperateBusiness<Epm_Change>.Get().Update(model);
                    result.Data = rows;
                    result.Flag = EResultFlag.Success;

                    WriteLog(BusinessType.Change.GetText(), SystemRight.Modify.GetText(), "更新状态: " + rows);

                    //处理待办
                    var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == model.Id && t.IsApprover == false).FirstOrDefault();
                    if (tempApp != null)
                    {
                        ComplateApprover(tempApp.Id);
                    }
                    string title = "";
                    if (model.State == (int)ApprovalState.Discarded)
                    {
                        var list = DataOperateBusiness<Epm_ChangeCompany>.Get().GetList(t => t.ChangeId == model.Id).ToList();
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

                        #region 生成待办
                        var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                        List<Epm_Approver> listApp = new List<Epm_Approver>();
                        Epm_Approver app = new Epm_Approver();
                        app.Title = model.CreateUserName + "提交了变更申请，已废弃";
                        app.Content = model.CreateUserName + "提交了变更申请，已废弃";
                        app.SendUserId = CurrentUserID.ToLongReq();
                        app.SendUserName = CurrentUserName;
                        app.SendTime = DateTime.Now;
                        app.LinkURL = string.Empty;
                        app.BusinessTypeNo = BusinessType.Change.ToString();
                        app.Action = SystemRight.Invalid.ToString();
                        app.BusinessTypeName = BusinessType.Change.GetText();
                        app.BusinessState = (int)(ApprovalState.Discarded);
                        app.BusinessId = model.Id;
                        app.ApproverId = project.PMId;
                        app.ApproverName = project.PMName;
                        app.ProjectId = model.ProjectId;
                        app.ProjectName = project.Name;
                        listApp.Add(app);
                        AddApproverBatch(listApp);
                        WriteLog(BusinessType.Contract.GetText(), SystemRight.Invalid.GetText(), "废弃变更生成待办: " + model.Id);
                        #endregion

                        title = model.CreateUserName + "提交了变更申请，已废弃";
                    }

                    if ((ApprovalState)Enum.ToObject(typeof(ApprovalState), model.State) == ApprovalState.ApprSuccess)
                    {
                        #region 更改项目状态表
                        var companys = DataOperateBusiness<Epm_ChangeCompany>.Get().GetList(t => t.ChangeId == model.Id).ToList();
                        if (companys.Any())
                        {
                            var companyIds = companys.Select(t => t.CompanyId);

                            var track = DataOperateBusiness<Epm_ProjectStateTrack>.Get().GetList(t => t.ProjectId == model.ProjectId && companyIds.Contains(t.CompanyId) && t.StateTrackNo == "ChangeNum");
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

                        var project = DataOperateBusiness<Epm_Project>.Get().GetList(t => t.Id == model.ProjectId).FirstOrDefault();
                        if (project != null)
                        {
                            project.Amount = model.TotalAmount + model.ChangeAmount;
                            DataOperateBusiness<Epm_Project>.Get().Update(project);
                        }

                        #region 生成待办
                        List<Epm_Approver> list = new List<Epm_Approver>();
                        Epm_Approver app = new Epm_Approver();
                        app.Title = model.CreateUserName + "提交的变更申请，审核通过";
                        app.Content = model.CreateUserName + "提交的变更申请，审核通过";
                        app.SendUserId = model.CreateUserId;
                        app.SendUserName = model.CreateUserName;
                        app.SendTime = model.CreateTime;
                        app.LinkURL = string.Empty;
                        app.BusinessTypeNo = BusinessType.Change.ToString();
                        app.Action = SystemRight.Check.ToString();
                        app.BusinessTypeName = BusinessType.Change.GetText();
                        app.BusinessState = (int)(ApprovalState.ApprSuccess);
                        app.BusinessId = model.Id;
                        app.ApproverId = model.CreateUserId;
                        app.ApproverName = model.CreateUserName;
                        app.ProjectId = model.ProjectId;
                        app.ProjectName = model.ProjectName;
                        list.Add(app);
                        AddApproverBatch(list);
                        WriteLog(BusinessType.Change.GetText(), SystemRight.Check.GetText(), "审核通过变更生成待办: " + model.Id);
                        #endregion
                        title = model.CreateUserName + "提交的变更申请，审核通过";
                    }
                    else if ((ApprovalState)Enum.ToObject(typeof(ApprovalState), model.State) == ApprovalState.ApprFailure)
                    {

                        #region 生成待办
                        List<Epm_Approver> list = new List<Epm_Approver>();
                        Epm_Approver app = new Epm_Approver();
                        app.Title = model.CreateUserName + "提交的变更申请已被驳回，请处理";
                        app.Content = model.CreateUserName + "提交的变更申请已被驳回，请处理";
                        app.SendUserId = model.CreateUserId;
                        app.SendUserName = model.CreateUserName;
                        app.SendTime = model.CreateTime;
                        app.LinkURL = string.Empty;
                        app.BusinessTypeNo = BusinessType.Change.ToString();
                        app.Action = SystemRight.UnCheck.ToString();
                        app.BusinessTypeName = BusinessType.Change.GetText();
                        app.BusinessState = (int)(ApprovalState.ApprFailure);
                        app.BusinessId = model.Id;
                        app.ApproverId = model.CreateUserId;
                        app.ApproverName = model.CreateUserName;
                        app.ProjectId = model.ProjectId;
                        app.ProjectName = model.ProjectName;
                        list.Add(app);
                        AddApproverBatch(list);
                        WriteLog(BusinessType.Change.GetText(), SystemRight.UnCheck.GetText(), "驳回变更生成待办: " + model.Id);
                        #endregion

                        #region 发送短信
                        //WriteSMS(model.CreateUserId, 0, MessageStep.ChangeReject, null);
                        #endregion

                        title = model.CreateUserName + "提交的变更申请已被驳回，请处理";
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
                        modelMsg.BussinesType = BusinessType.Change.ToString();
                        modelMsg.ProjectId = model.ProjectId.Value;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion

                    SystemRight right = (model.State == (int)ApprovalState.ApprSuccess) ? SystemRight.Check : SystemRight.UnCheck;

                    WriteLog(BusinessType.Change.GetText(), right.GetText(), "生成消息: " + model.Id);
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
    }
}
