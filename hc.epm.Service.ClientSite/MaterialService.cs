using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Service.ClientSite
{
    /// <summary>
    /// 工器具验收
    /// </summary>
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddMaterial(MaterialView model)
        {
            Result<int> result = new Result<int>();
            try
            {
                model.Epm_Material = SetCurrentUser(model.Epm_Material);
                model.Epm_Material = SetCreateUser(model.Epm_Material);
                model.Epm_Material.CompanyId = CurrentCompanyID.ToLongReq();
                model.Epm_Material.CompanyName = CurrentCompanyName;

                DataOperateBusiness<Epm_Material>.Get().Add(model.Epm_Material);
                model.MaterialDetails.ForEach(item =>
                {
                    item.MaterialCheckId = model.Epm_Material.Id;
                    item.CrtCompanyId = CurrentCompanyID.ToLongReq();
                    item.CrtCompanyName = CurrentCompanyName;
                    item.CreateUserId = CurrentUserID.ToLongReq();
                    item.CreateUserName = CurrentUserName;
                    item.CreateTime = DateTime.Now;
                    item.State = 0;
                    item = SetCurrentUser(item);
                    item = SetCreateUser(item);
                });
                DataOperateBusiness<Epm_MaterialDetails>.Get().AddRange(model.MaterialDetails);

                if (model.FileList.Count > 0)
                {
                    AddFilesByTable(model.Epm_Material, model.FileList); //上传附件
                }

                result.Data = 1;
                result.Flag = EResultFlag.Success;

                WriteLog(BusinessType.Equipment.GetText(), SystemRight.Add.GetText(), "新增: " + model.Epm_Material.Id);

                if (model.Epm_Material.State == (int)ConfirmState.WaitConfirm)
                {
                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.Epm_Material.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "提报了工器具机械验收单，待审核";
                    app.Content = CurrentUserName + "提报了工器具机械验收单，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Equipment.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Equipment.GetText();
                    app.BusinessState = (int)(ConfirmState.WaitConfirm);
                    app.BusinessId = model.Epm_Material.Id;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = model.Epm_Material.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Equipment.GetText(), SystemRight.Add.GetText(), "提交工器具机械验收生成待办: " + model.Epm_Material.Id);
                    #endregion

                    #region 消息
                    var waitSend = GetWaitSendMessageList(model.Epm_Material.ProjectId.Value);
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
                        modelMsg.Title = CurrentUserName + "提报了工器具机械验收单，待审核";
                        modelMsg.Content = CurrentUserName + "提报了工器具机械验收单，待审核";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Epm_Material.Id;
                        modelMsg.BussinesType = BusinessType.Equipment.ToString();
                        modelMsg.ProjectId = model.Epm_Material.ProjectId.Value;
                        modelMsg.ProjectName = model.Epm_Material.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion

                    #region 发送短信
                    //Dictionary<string, string> parameterSms = new Dictionary<string, string>();
                    //parameterSms.Add("UserName", CurrentUserName);
                    //WriteSMS(project.ContactUserId.Value, project.CompanyId, MessageStep.MaterialAdd, parameterSms);
                    #endregion
                }
            }
            catch (Exception e)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(e, "AddMaterial");
            }
            return result;
        }

        /// <summary>
        /// 工器具验收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddMaterialNew(MaterialViewNew model)
        {
            Result<int> result = new Result<int>();
            try
            {
                model.Epm_Material = SetCurrentUser(model.Epm_Material);
                model.Epm_Material = SetCreateUser(model.Epm_Material);
                model.Epm_Material.CompanyId = CurrentCompanyID.ToLongReq();
                model.Epm_Material.CompanyName = CurrentCompanyName;

                DataOperateBusiness<Epm_Material>.Get().Add(model.Epm_Material);
                model.MaterialDetails.ForEach(item =>
                {
                    item.MaterialDetails.MaterialCheckId = model.Epm_Material.Id;
                    item.MaterialDetails.CrtCompanyId = CurrentCompanyID.ToLongReq();
                    item.MaterialDetails.CrtCompanyName = CurrentCompanyName;
                    item.MaterialDetails.CreateUserId = CurrentUserID.ToLongReq();
                    item.MaterialDetails.CreateUserName = CurrentUserName;
                    item.MaterialDetails.CreateTime = DateTime.Now;
                    item.MaterialDetails.State = 0;
                    item.MaterialDetails = SetCurrentUser(item.MaterialDetails);
                    item.MaterialDetails = SetCreateUser(item.MaterialDetails);
                });
                List<Epm_MaterialDetails> deList = new List<Epm_MaterialDetails>();
                List<Base_Files> fileList = new List<Base_Files>();
                if (model.MaterialDetails.Any())
                {
                    foreach (var item in model.MaterialDetails)
                    {
                        deList.Add(item.MaterialDetails);

                        if (item.FileList.Any())
                        {
                            foreach (var temp in item.FileList)
                            {
                                temp.TableName = "Epm_MaterialDetails";
                                temp.TableId = item.MaterialDetails.Id;
                                fileList.Add(temp);
                            }
                        }
                    }
                }
                DataOperateBusiness<Epm_MaterialDetails>.Get().AddRange(deList);
                DataOperateBasic<Base_Files>.Get().UpdateRange(fileList);

                result.Data = 1;
                result.Flag = EResultFlag.Success;

                WriteLog(BusinessType.Equipment.GetText(), SystemRight.Add.GetText(), "新增: " + model.Epm_Material.Id);

                if (model.Epm_Material.State == (int)ConfirmState.WaitConfirm)
                {
                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.Epm_Material.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "提报了工器具机械验收单，待审核";
                    app.Content = CurrentUserName + "提报了工器具机械验收单，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Equipment.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Equipment.GetText();
                    app.BusinessState = (int)(ConfirmState.WaitConfirm);
                    app.BusinessId = model.Epm_Material.Id;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = model.Epm_Material.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Equipment.GetText(), SystemRight.Add.GetText(), "提交工器具机械验收生成待办: " + model.Epm_Material.Id);
                    #endregion

                    #region 消息
                    var waitSend = GetWaitSendMessageList(model.Epm_Material.ProjectId.Value);
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
                        modelMsg.Title = CurrentUserName + "提报了工器具机械验收单，待审核";
                        modelMsg.Content = CurrentUserName + "提报了工器具机械验收单，待审核";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Epm_Material.Id;
                        modelMsg.BussinesType = BusinessType.Equipment.ToString();
                        modelMsg.ProjectId = model.Epm_Material.ProjectId.Value;
                        modelMsg.ProjectName = model.Epm_Material.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion

                    #region 发送短信
                    //Dictionary<string, string> parameterSms = new Dictionary<string, string>();
                    //parameterSms.Add("UserName", CurrentUserName);
                    //WriteSMS(project.ContactUserId.Value, project.CompanyId, MessageStep.MaterialAdd, parameterSms);
                    #endregion
                }
            }
            catch (Exception e)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(e, "AddMaterialNew");
            }
            return result;
        }

        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateMaterial(MaterialView model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var oldModel = DataOperateBusiness<Epm_Material>.Get().GetModel(model.Epm_Material.Id);
                if (oldModel == null)
                {
                    throw new Exception("该材料设备验收不存在或已被删除");
                }
                model.Epm_Material = FiterUpdate(oldModel, model.Epm_Material);
                model.Epm_Material.CompanyId = oldModel.CompanyId;
                model.Epm_Material.CompanyName = oldModel.CompanyName;

                var row = DataOperateBusiness<Epm_Material>.Get().Update(model.Epm_Material);
                if (model.MaterialDetails.Count > 0)
                {
                    //先删除
                    var detaileList = DataOperateBusiness<Epm_MaterialDetails>.Get().GetList(p => p.MaterialCheckId == model.Epm_Material.Id);
                    if (detaileList.Any())
                    {
                        DataOperateBusiness<Epm_MaterialDetails>.Get().DeleteRange(detaileList);
                    }

                    model.MaterialDetails.ForEach(item =>
                    {
                        item.MaterialCheckId = model.Epm_Material.Id;
                        item.CrtCompanyId = CurrentCompanyID.ToLongReq();
                        item.CrtCompanyName = CurrentCompanyName;
                        item.CreateUserId = CurrentUserID.ToLongReq();
                        item.CreateUserName = CurrentUserName;
                        item.CreateTime = DateTime.Now;
                        item.State = 0;
                        item = SetCurrentUser(item);
                        item = SetCreateUser(item);
                    });
                    DataOperateBusiness<Epm_MaterialDetails>.Get().AddRange(model.MaterialDetails);
                }
                //上传模型
                if (model.FileList != null)
                {
                    //删除之前的附件
                    DeleteFilesByTable(model.GetType().Name, new List<long>() { model.Epm_Material.Id });
                    //新增附件
                    AddFilesByTable(model.Epm_Material, model.FileList);
                }
                result.Data = row;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Equipment.GetText(), SystemRight.Add.GetText(), "修改: " + model.Epm_Material.Id);

                //处理待办
                var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == model.Epm_Material.Id && t.IsApprover == false).FirstOrDefault();
                if (tempApp != null)
                {
                    ComplateApprover(tempApp.Id);
                }

                if (model.Epm_Material.State == (int)ConfirmState.WaitConfirm)
                {
                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.Epm_Material.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "提报了工器具机械验收单，待审核";
                    app.Content = CurrentUserName + "提报了工器具机械验收单，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Equipment.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Equipment.GetText();
                    app.BusinessState = (int)(ConfirmState.WaitConfirm);
                    app.BusinessId = model.Epm_Material.Id;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = model.Epm_Material.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Equipment.GetText(), SystemRight.Add.GetText(), "提交工器具机械验收生成待办: " + model.Epm_Material.Id);
                    #endregion

                    #region 消息
                    Epm_Massage modelMsg = new Epm_Massage();
                    modelMsg.ReadTime = null;
                    modelMsg.RecId = project.ContactUserId.Value;
                    modelMsg.RecName = project.ContactUserName;
                    modelMsg.RecTime = DateTime.Now;
                    modelMsg.SendId = CurrentUserID.ToLongReq();
                    modelMsg.SendName = CurrentUserName;
                    modelMsg.SendTime = DateTime.Now;
                    modelMsg.Title = CurrentUserName + "提报了工器具机械验收单，待审核";
                    modelMsg.Content = CurrentUserName + "提报了工器具机械验收单，待审核";
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = model.Epm_Material.Id;
                    modelMsg.BussinesType = BusinessType.Equipment.ToString();
                    modelMsg.ProjectId = model.Epm_Material.ProjectId.Value;
                    modelMsg.ProjectName = model.Epm_Material.ProjectName;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    #endregion

                    #region 发送短信
                    //Dictionary<string, string> parameters = new Dictionary<string, string>();
                    //parameters.Add("UserName", CurrentUserName);
                    //WriteSMS(project.ContactUserId.Value, project.CompanyId, MessageStep.MaterialAdd, parameters);
                    #endregion
                }
            }
            catch (Exception e)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(e, "UpdateMaterial");
            }
            return result;
        }

        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteMaterialByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_Material>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                if (!models.Any())
                {
                    throw new Exception("该材料设备验收不存在或已被删除");
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
                        modelMsg.Title = CurrentUserName + "提报的工器具机械验收单已删除";
                        modelMsg.Content = CurrentUserName + "提报的工器具机械验收单已删除";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Equipment.ToString();
                        modelMsg.ProjectId = model.ProjectId;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion
                }

                var rows = DataOperateBusiness<Epm_Material>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Equipment.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteMaterialByIds");
            }
            return result;
        }

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_Material>> GetMaterialList(QueryCondition qc)
        {
            qc = AddDefaultWeb(qc);
            Result<List<Epm_Material>> result = new Result<List<Epm_Material>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Material>(context, qc);
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
        public Result<MaterialView> GetMaterialModel(long id)
        {
            MaterialView list = new MaterialView();
            Result<MaterialView> result = new Result<MaterialView>();
            try
            {
                list.Epm_Material = DataOperateBusiness<Epm_Material>.Get().GetModel(id);
                list.MaterialDetails = DataOperateBusiness<Epm_MaterialDetails>.Get().GetList(p => p.MaterialCheckId == id).ToList();

                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMaterialModel");
            }
            return result;
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateMaterialState(long id, ConfirmState state)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBusiness<Epm_Material>.Get().GetModel(id);
                model.State = (int)state;
                var rows = DataOperateBusiness<Epm_Material>.Get().Update(model);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Equipment.GetText(), SystemRight.Modify.GetText(), "更新状态: " + rows);

                //处理待办
                var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == model.Id && t.IsApprover == false).FirstOrDefault();
                if (tempApp != null)
                {
                    ComplateApprover(tempApp.Id);
                }

                if (state == ConfirmState.Discarded)
                {
                    #region 生成待办
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = model.CreateUserName + "提报的工器具机械验收单，已废弃";
                    app.Content = model.CreateUserName + "提报的工器具机械验收单，已废弃";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Equipment.ToString();
                    app.Action = SystemRight.Invalid.ToString();
                    app.BusinessTypeName = BusinessType.Equipment.GetText();
                    app.BusinessState = (int)(ConfirmState.Discarded);
                    app.BusinessId = model.Id;
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Equipment.GetText(), SystemRight.Invalid.GetText(), "废弃工器具机械验收生成待办: " + model.Id);
                    #endregion

                    #region 消息
                    Epm_Massage modelMsg = new Epm_Massage();
                    modelMsg.ReadTime = null;
                    modelMsg.RecId = model.CreateUserId;
                    modelMsg.RecName = model.CreateUserName;
                    modelMsg.RecTime = DateTime.Now;
                    modelMsg.SendId = CurrentUserID.ToLongReq();
                    modelMsg.SendName = CurrentUserName;
                    modelMsg.SendTime = DateTime.Now;
                    modelMsg.Title = model.CreateUserName + "提报的工器具机械验收单已被" + CurrentUserName + "废弃，请处理";
                    modelMsg.Content = model.CreateUserName + "提报的工器具机械验收单已被" + CurrentUserName + "废弃，请处理";
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = model.Id;
                    modelMsg.BussinesType = BusinessType.Equipment.ToString();
                    modelMsg.ProjectId = model.ProjectId.Value;
                    modelMsg.ProjectName = model.ProjectName;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    #endregion
                }
                else if (state == ConfirmState.Confirm)
                {
                    #region 生成待办
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = model.CreateUserName + "提报的工器具机械验收单，审核通过";
                    app.Content = model.CreateUserName + "提报的工器具机械验收单，审核通过";
                    app.SendUserId = model.CreateUserId;
                    app.SendUserName = model.CreateUserName;
                    app.SendTime = model.CreateTime;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Equipment.ToString();
                    app.Action = SystemRight.Check.ToString();
                    app.BusinessTypeName = BusinessType.Equipment.GetText();
                    app.BusinessState = (int)(ConfirmState.Confirm);
                    app.BusinessId = model.Id;
                    app.ApproverId = model.CreateUserId;
                    app.ApproverName = model.CreateUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = model.ProjectName;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Equipment.GetText(), SystemRight.Check.GetText(), "驳回合同生成待办: " + model.Id);
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
                        modelMsg.Title = model.CreateUserName + "提报的工器具机械验收单，" + CurrentUserName + "已审核通过";
                        modelMsg.Content = model.CreateUserName + "提报的工器具机械验收单，" + CurrentUserName + "已审核通过";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Equipment.ToString();
                        modelMsg.ProjectId = model.ProjectId.Value;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion
                }
                else if (state == ConfirmState.ConfirmFailure)
                {
                    #region 生成待办
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = model.CreateUserName + "提报的工器具机械验收单已被驳回，请处理";
                    app.Content = model.CreateUserName + "提报的工器具机械验收单已被驳回，请处理";
                    app.SendUserId = model.CreateUserId;
                    app.SendUserName = model.CreateUserName;
                    app.SendTime = model.CreateTime;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Equipment.ToString();
                    app.Action = SystemRight.UnCheck.ToString();
                    app.BusinessTypeName = BusinessType.Equipment.GetText();
                    app.BusinessState = (int)(ConfirmState.ConfirmFailure);
                    app.BusinessId = model.Id;
                    app.ApproverId = model.CreateUserId;
                    app.ApproverName = model.CreateUserName;
                    app.ProjectId = model.ProjectId;
                    app.ProjectName = model.ProjectName;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Equipment.GetText(), SystemRight.UnCheck.GetText(), "驳回合同生成待办: " + model.Id);
                    #endregion

                    #region 消息
                    Epm_Massage modelMsg = new Epm_Massage();
                    modelMsg.ReadTime = null;
                    modelMsg.RecId = model.CreateUserId;
                    modelMsg.RecName = model.CreateUserName;
                    modelMsg.RecTime = DateTime.Now;
                    modelMsg.SendId = CurrentUserID.ToLongReq();
                    modelMsg.SendName = CurrentUserName;
                    modelMsg.SendTime = DateTime.Now;
                    modelMsg.Title = model.CreateUserName + "提报的工器具机械验收单已被" + CurrentUserName + "驳回，请处理";
                    modelMsg.Content = model.CreateUserName + "提报的工器具机械验收单已被" + CurrentUserName + "驳回，请处理";
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = model.Id;
                    modelMsg.BussinesType = BusinessType.Equipment.ToString();
                    modelMsg.ProjectId = model.ProjectId.Value;
                    modelMsg.ProjectName = model.ProjectName;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    #endregion

                    #region 发送短信
                    //WriteSMS(model.CreateUserId, 0, MessageStep.MaterialReject, null);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateMaterialState");
            }
            return result;
        }
    }
}
