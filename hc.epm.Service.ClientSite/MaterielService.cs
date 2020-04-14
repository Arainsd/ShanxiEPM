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
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddMateriel(MaterielView model)
        {
            Result<int> result = new Result<int>();
            try
            {
                model.Epm_Materiel = SetCurrentUser(model.Epm_Materiel);
                model.Epm_Materiel.CompanyId = CurrentCompanyID.ToLongReq();
                model.Epm_Materiel.CompanyName = CurrentCompanyName;
                if (model.MaterielDetails.Count>0)
                {
                    model.Epm_Materiel.SupplierName = model.MaterielDetails[0].Unit;
                
                    DataOperateBusiness<Epm_Materiel>.Get().Add(model.Epm_Materiel);

                    foreach (var item in model.MaterielDetails)
                    {
                        item.MaterielReceiveId = model.Epm_Materiel.Id;
                        item.UseSum = Convert.ToInt32(item.Qty) - item.UseSum - item.StayUseSum;
                        item.SupMatApplyId = model.Epm_Materiel.SupMatApplyId;
                        SetCreateUser(item);

                        //修改甲供物资申请详情数据
                        var tzSupMatApplyList = DataOperateBusiness<Epm_TzSupMatApplyList>.Get().GetModel(item.SupMatApplyListId.Value);
                        if (tzSupMatApplyList != null)
                        {
                            tzSupMatApplyList.UseSum = tzSupMatApplyList.UseSum + item.UseSum;
                            tzSupMatApplyList.StayUseSum = item.StayUseSum;

                       
                            if (tzSupMatApplyList.StayUseSum == 0)
                            {
                                tzSupMatApplyList.UseType = true;
                            }
                            DataOperateBusiness<Epm_TzSupMatApplyList>.Get().Update(tzSupMatApplyList);
                        }
                    }
                    DataOperateBusiness<Epm_MaterielDetails>.Get().AddRange(model.MaterielDetails);

                    var stayList = DataOperateBusiness<Epm_MaterielDetails>.Get().GetList(p => p.SupMatApplyId == model.Epm_Materiel.SupMatApplyId).ToList();

                    foreach (var temp in stayList)
                    {
                        var tzSupMatApply = DataOperateBusiness<Epm_TzSupMatApplyList>.Get().GetModel(temp.SupMatApplyListId.Value);
                        temp.StayUseSum = tzSupMatApply.StayUseSum;
                    }
                    DataOperateBusiness<Epm_MaterielDetails>.Get().UpdateRange(stayList);

                    //修改甲供物资申请验收状态
                    var tzSupplyMaterialApply = DataOperateBusiness<Epm_TzSupplyMaterialApply>.Get().GetModel(model.Epm_Materiel.SupMatApplyId.Value);
                    if (tzSupplyMaterialApply != null)
                    {
                        int count = DataOperateBusiness<Epm_TzSupMatApplyList>.Get().GetList(t => t.SupMatApplyId == tzSupplyMaterialApply.Id && t.UseType == false).Count();
                        if (count == 0)
                        {
                            tzSupplyMaterialApply.UseType = true;
                            DataOperateBusiness<Epm_TzSupplyMaterialApply>.Get().Update(tzSupplyMaterialApply);
                        }
                    }

                    if (model.FileList != null)
                    {
                        AddFilesByTable(model.Epm_Materiel, model.FileList);//上传附件
                    }

                    result.Data = 1;
                    result.Flag = EResultFlag.Success;

                    if (model.Epm_Materiel.State == (int)ConfirmState.WaitConfirm)
                    {
                        #region 生成待办
                        var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.Epm_Materiel.ProjectId.Value);
                        List<Epm_Approver> list = new List<Epm_Approver>();
                        Epm_Approver app = new Epm_Approver();
                        app.Title = CurrentUserName + "提报了材料设备验收单，待审核";
                        app.Content = CurrentUserName + "提报了材料设备验收单，待审核";
                        app.SendUserId = CurrentUserID.ToLongReq();
                        app.SendUserName = CurrentUserName;
                        app.SendTime = DateTime.Now;
                        app.LinkURL = string.Empty;
                        app.BusinessTypeNo = BusinessType.Track.ToString();
                        app.Action = SystemRight.Add.ToString();
                        app.BusinessTypeName = BusinessType.Track.GetText();
                        app.BusinessState = (int)(ConfirmState.WaitConfirm);
                        app.BusinessId = model.Epm_Materiel.Id;
                        app.ApproverId = project.ContactUserId;
                        app.ApproverName = project.ContactUserName;
                        app.ProjectId = model.Epm_Materiel.ProjectId;
                        app.ProjectName = project.Name;
                        list.Add(app);
                        AddApproverBatch(list);
                        WriteLog(BusinessType.Track.GetText(), SystemRight.Add.GetText(), "提交物料验收生成待办: " + model.Epm_Materiel.Id);
                        #endregion

                        #region 消息
                        var waitSend = GetWaitSendMessageList(model.Epm_Materiel.ProjectId.Value);
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
                            modelMsg.Title = CurrentUserName + "提报了材料设备验收单，待审核";
                            modelMsg.Content = CurrentUserName + "提报了材料设备验收单，待审核";
                            modelMsg.Type = 2;
                            modelMsg.IsRead = false;
                            modelMsg.BussinessId = model.Epm_Materiel.Id;
                            modelMsg.BussinesType = BusinessType.Track.ToString();
                            modelMsg.ProjectId = model.Epm_Materiel.ProjectId;
                            modelMsg.ProjectName = model.Epm_Materiel.ProjectName;
                            modelMsg = base.SetCurrentUser(modelMsg);
                            modelMsg = base.SetCreateUser(modelMsg);
                            DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                        }
                            #endregion

                            #region 发送短信
                            //Dictionary<string, string> parametersms = new Dictionary<string, string>();
                            //parametersms.Add("UserName", CurrentUserName);
                            //WriteSMS(project.ContactUserId.Value, project.CompanyId, MessageStep.MaterielAdd, parametersms);
                            #endregion
                    }
                }
            }
            catch (Exception e)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(e, "AddMateriel");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateMateriel(MaterielView model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var oldModel = DataOperateBusiness<Epm_Materiel>.Get().GetModel(model.Epm_Materiel.Id);

                if (oldModel == null)
                {
                    throw new Exception("该材料设备验收信息不存在或已被删除");
                }
                model.Epm_Materiel = FiterUpdate(oldModel, model.Epm_Materiel);
                model.Epm_Materiel.CompanyId = oldModel.CompanyId;
                model.Epm_Materiel.CompanyName = oldModel.CompanyName;
                model.Epm_Materiel.SupplierName = model.MaterielDetails[0].Unit;
                var row = DataOperateBusiness<Epm_Materiel>.Get().Update(model.Epm_Materiel);

                if (model.MaterielDetails.Count > 0)
                {
                    //先删除
                    var detaileList = DataOperateBusiness<Epm_MaterielDetails>.Get().GetList(p => p.MaterielReceiveId == model.Epm_Materiel.Id);
                    if (detaileList.Any())
                    {
                        DataOperateBusiness<Epm_MaterielDetails>.Get().DeleteRange(detaileList);
                    }

                    foreach (var item in model.MaterielDetails)
                    {
                        item.MaterielReceiveId = model.Epm_Materiel.Id;
                        int num = Convert.ToInt32(item.Qty) - item.StayUseSum.Value - item.UseSum.Value;
                        item.UseSum = item.TotalUseSum;
                        item.State = 0;
                        item.SupMatApplyId = model.Epm_Materiel.SupMatApplyId;
                        SetCreateUser(item);

                        //修改甲供物资申请详情数据
                        var tzSupMatApplyList = DataOperateBusiness<Epm_TzSupMatApplyList>.Get().GetModel(item.SupMatApplyListId.Value);
                        if (tzSupMatApplyList != null)
                        {
                            tzSupMatApplyList.UseSum = Convert.ToInt32(item.Qty) - item.StayUseSum.Value;
                            tzSupMatApplyList.StayUseSum = item.StayUseSum;

                            if (item.UseSum == tzSupMatApplyList.Number)
                            {
                                tzSupMatApplyList.UseType = true;
                            }

                            DataOperateBusiness<Epm_TzSupMatApplyList>.Get().Update(tzSupMatApplyList);
                        }
                    }
                    DataOperateBusiness<Epm_MaterielDetails>.Get().AddRange(model.MaterielDetails);


                    var stayList = DataOperateBusiness<Epm_MaterielDetails>.Get().GetList(p => p.SupMatApplyId == model.Epm_Materiel.SupMatApplyId).ToList();

                    foreach (var temp in stayList)
                    {
                        var tzSupMatApply = DataOperateBusiness<Epm_TzSupMatApplyList>.Get().GetModel(temp.SupMatApplyListId.Value);
                        temp.StayUseSum = tzSupMatApply.StayUseSum;
                    }
                    DataOperateBusiness<Epm_MaterielDetails>.Get().UpdateRange(stayList);
                }

                //上传模型
                if (model.FileList != null)
                {
                    //删除之前的附件
                    DeleteFilesByTable(model.GetType().Name, new List<long>() { model.Epm_Materiel.Id });
                    //新增附件
                    AddFilesByTable(model.Epm_Materiel, model.FileList);
                }

                //修改甲供物资申请验收状态
                var tzSupplyMaterialApply = DataOperateBusiness<Epm_TzSupplyMaterialApply>.Get().GetModel(model.Epm_Materiel.SupMatApplyId.Value);
                if (tzSupplyMaterialApply != null)
                {
                    int count = DataOperateBusiness<Epm_TzSupMatApplyList>.Get().GetList(t => t.SupMatApplyId == tzSupplyMaterialApply.Id && t.UseType == false).Count();
                    if (count == 0)
                    {
                        tzSupplyMaterialApply.UseType = true;
                        DataOperateBusiness<Epm_TzSupplyMaterialApply>.Get().Update(tzSupplyMaterialApply);
                    }
                }

                result.Data = row;
                result.Flag = EResultFlag.Success;
                //WriteLog(BusinessType(WebModule.Material.GetText(), SystemRight.Add.GetText(), "修改: " + model.Epm_Materiel.Id);

                //处理待办
                var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == model.Epm_Materiel.Id && t.IsApprover == false).FirstOrDefault();
                if (tempApp != null)
                {
                    ComplateApprover(tempApp.Id);
                }

                if (model.Epm_Materiel.State == (int)ConfirmState.WaitConfirm)
                {
                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.Epm_Materiel.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "提报了材料设备验收单，待审核";
                    app.Content = CurrentUserName + "提报了材料设备验收单，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Track.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Track.GetText();
                    app.BusinessState = (int)(ConfirmState.WaitConfirm);
                    app.BusinessId = model.Epm_Materiel.Id;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = model.Epm_Materiel.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Track.GetText(), SystemRight.Add.GetText(), "提交物料验收生成待办: " + model.Epm_Materiel.Id);
                    #endregion

                    #region 消息
                    var waitSend = GetWaitSendMessageList(model.Epm_Materiel.ProjectId.Value);
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
                        modelMsg.Title = CurrentUserName + "提报了材料设备验收单，待审核";
                        modelMsg.Content = CurrentUserName + "提报了材料设备验收单，待审核";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Epm_Materiel.Id;
                        modelMsg.BussinesType = BusinessType.Track.ToString();
                        modelMsg.ProjectId = model.Epm_Materiel.ProjectId;
                        modelMsg.ProjectName = model.Epm_Materiel.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion

                    #region 发送短信
                    //Dictionary<string, string> parametersms = new Dictionary<string, string>();
                    //parametersms.Add("UserName", CurrentUserName);
                    //WriteSMS(project.ContactUserId.Value, project.CompanyId, MessageStep.MaterielAdd, parametersms);
                    #endregion
                }
            }
            catch (Exception e)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(e, "UpdateMateriel");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteMaterielByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_Materiel>.Get().GetList(i => ids.Contains(i.Id)).ToList();

                if (!models.Any())
                {
                    throw new Exception("该材料设备验收信息不存在或已被删除");
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
                        modelMsg.Title = CurrentUserName + "提报的材料设备验收单已删除";
                        modelMsg.Content = CurrentUserName + "提报的材料设备验收单已删除";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Track.ToString();
                        modelMsg.ProjectId = model.ProjectId;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion
                }
                var rows = DataOperateBusiness<Epm_Materiel>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Equipment.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteMaterielByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_Materiel>> GetMaterielList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_Materiel>> result = new Result<List<Epm_Materiel>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Materiel>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMaterielList");
            }
            return result;
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<MaterielView> GetMaterielModel(long id)
        {
            MaterielView list = new MaterielView();
            Result<MaterielView> result = new Result<MaterielView>();
            try
            {
                list.Epm_Materiel = DataOperateBusiness<Epm_Materiel>.Get().GetModel(id);

                if (list.Epm_Materiel != null)
                {
                    list.FileList = DataOperateBasic<Base_Files>.Get().GetList(t => t.TableId == id && string.IsNullOrEmpty(t.ImageType)).ToList();
                }
                list.MaterielDetails = DataOperateBusiness<Epm_MaterielDetails>.Get().GetList(p => p.MaterielReceiveId == id).ToList();

                if (list.MaterielDetails.Any())
                {
                    foreach (var item in list.MaterielDetails)
                    {
                        var tzSupMatApplyList = DataOperateBusiness<Epm_TzSupMatApplyList>.Get().GetModel(item.SupMatApplyListId.Value);
                        if (tzSupMatApplyList != null)
                        {
                            item.TotalUseSum = tzSupMatApplyList.UseSum;
                        }
                    }
                }

                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMaterielModel");
            }
            return result;
        }


        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeMaterielState(long id, ConfirmState state)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBusiness<Epm_Materiel>.Get().GetModel(id);
                model.State = (int)state;
                var rows = DataOperateBusiness<Epm_Materiel>.Get().Update(model);
                MessageStep step = MessageStep.MaterielAudit;

                //处理待办
                var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == model.Id && t.IsApprover == false).FirstOrDefault();
                if (tempApp != null)
                {
                    ComplateApprover(tempApp.Id);
                }
                string title = "";
                if (state == ConfirmState.Discarded)
                {
                    title = model.CreateUserName + "提报的材料设备验收单，已废弃";
                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = model.CreateUserName + "提报的材料设备验收单，已废弃";
                    app.Content = model.CreateUserName + "提报的材料设备验收单，已废弃";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Track.ToString();
                    app.Action = SystemRight.Invalid.ToString();
                    app.BusinessTypeName = BusinessType.Track.GetText();
                    app.BusinessState = (int)(ConfirmState.Discarded);
                    app.BusinessId = model.Id;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Track.GetText(), SystemRight.Invalid.GetText(), "废弃物料验收生成待办: " + model.Id);
                    #endregion

                }
                else if (state == ConfirmState.Confirm)
                {
                    #region 生成待办
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = model.CreateUserName + "提报的材料设备验收单已审核通过";
                    app.Content = model.CreateUserName + "提报的材料设备验收单已审核通过";
                    app.SendUserId = model.CreateUserId;
                    app.SendUserName = model.CreateUserName;
                    app.SendTime = model.CreateTime;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Track.ToString();
                    app.Action = SystemRight.Check.ToString();
                    app.BusinessTypeName = BusinessType.Track.GetText();
                    app.BusinessState = (int)(ConfirmState.Confirm);
                    app.BusinessId = model.Id;
                    app.ApproverId = model.CreateUserId;
                    app.ApproverName = model.CreateUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = model.ProjectName;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Track.GetText(), SystemRight.Check.GetText(), "审核通过物料验收生成待办: " + model.Id);
                    #endregion
                }
                else if (state == ConfirmState.ConfirmFailure)
                {
                    title = model.CreateUserName + "提报的材料设备验收已被驳回，请处理";
                    #region 生成待办
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = model.CreateUserName + "提报的材料设备验收已被驳回，请处理";
                    app.Content = model.CreateUserName + "提报的材料设备验收已被驳回，请处理";
                    app.SendUserId = model.CreateUserId;
                    app.SendUserName = model.CreateUserName;
                    app.SendTime = model.CreateTime;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Track.ToString();
                    app.Action = SystemRight.UnCheck.ToString();
                    app.BusinessTypeName = BusinessType.Track.GetText();
                    app.BusinessState = (int)(ConfirmState.ConfirmFailure);
                    app.BusinessId = model.Id;
                    app.ApproverId = model.CreateUserId;
                    app.ApproverName = model.CreateUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = model.ProjectName;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Track.GetText(), SystemRight.UnCheck.GetText(), "驳回物料验收生成待办: " + model.Id);
                    #endregion

                    #region 发送短信
                    //WriteSMS(model.CreateUserId, 0, MessageStep.MaterielReject, null);
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
                    modelMsg.BussinesType = BusinessType.Track.ToString();
                    modelMsg.ProjectId = model.ProjectId;
                    modelMsg.ProjectName = model.ProjectName;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                }
                #endregion

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Track.GetText(), SystemRight.Modify.GetText(), "更新状态: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "ChangeMaterielState");
            }
            return result;
        }

        /// <summary>
        /// 批量修改状态
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeMaterielALLState(List<long> ids, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                foreach (var item in ids)
                {
                    var model = DataOperateBusiness<Epm_Materiel>.Get().GetModel(item);
                    if (model != null)
                    {
                        SetCurrentUser(model);
                        model.State = (int)state.ToEnumReq<PreProjectState>();
                        var rows = DataOperateBusiness<Epm_Materiel>.Get().Update(model);

                        result.Data = rows;
                        result.Flag = EResultFlag.Success;
                    }
                    else
                    {
                        throw new Exception("该材料设备验收信息不存在");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "ChangeMaterielState");
            }
            return result;
        }
    }
}
