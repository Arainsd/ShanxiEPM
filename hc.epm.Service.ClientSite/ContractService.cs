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
        public Result<int> AddContract(Epm_Contract model, List<Base_Files> fileList = null)
        {
            Result<int> result = new Result<int>();
            try
            {
                base.SetCurrentUser(model);
                model.CrtCompanyId = CurrentCompanyID.ToLongReq();
                model.CrtCompanyName = CurrentCompanyName;
                model.ProjectId = model.ProjectId ?? 0;

                //bool dConfig = DataOperateBusiness<Epm_Contract>.Get().Count(i => i.Name == model.Name) > 0;
                //if (dConfig)
                //{
                //    throw new Exception("该合同名称已经存在");
                //}
                //dConfig = DataOperateBusiness<Epm_Contract>.Get().Count(i => i.Code == model.Code) > 0;
                //if (dConfig)
                //{
                //    throw new Exception("该合同编码已经存在");
                //}

                var rows = DataOperateBusiness<Epm_Contract>.Get().Add(model);

                //新增附件
                AddFilesByTable(model, fileList);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Contract.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);

                if ((ApprovalState)Enum.ToObject(typeof(ApprovalState), model.State) == ApprovalState.WaitAppr)
                {
                    bool isApprSuccess = false;
                    //判断是否工程处提交，如果是直接通过
                    if (IsAgencyUser(CurrentUserID.ToLongReq()))
                    {
                        isApprSuccess = true;
                        model.State = (int)ApprovalState.ApprSuccess;
                        DataOperateBusiness<Epm_Contract>.Get().Update(model);
                    }

                    if (!isApprSuccess && model.ProjectId.Value != 0)
                    {
                        #region 生成待办
                        var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                        List<Epm_Approver> list = new List<Epm_Approver>();
                        Epm_Approver app = new Epm_Approver();
                        app.Title = CurrentUserName + "上传了项目合同，待审核";
                        app.Content = CurrentUserName + "上传了项目合同，待审核";
                        app.SendUserId = CurrentUserID.ToLongReq();
                        app.SendUserName = CurrentUserName;
                        app.SendTime = DateTime.Now;
                        app.LinkURL = string.Empty;
                        app.BusinessTypeNo = BusinessType.Contract.ToString();
                        app.Action = SystemRight.Add.ToString();
                        app.BusinessTypeName = BusinessType.Contract.GetText();
                        app.BusinessState = (int)(ApprovalState.WaitAppr);
                        app.BusinessId = model.Id;
                        app.ApproverId = project.PMId;
                        app.ApproverName = project.PMName;
                        app.ProjectId = model.ProjectId;
                        app.ProjectName = project.Name;
                        list.Add(app);
                        AddApproverBatch(list);
                        WriteLog(BusinessType.Contract.GetText(), SystemRight.Add.GetText(), "提交合同生成待办: " + model.Id);
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
                            modelMsg.Title = CurrentUserName + "上传了项目合同，待审核";
                            modelMsg.Content = CurrentUserName + "上传了项目合同，待审核";
                            modelMsg.Type = 2;
                            modelMsg.IsRead = false;
                            modelMsg.BussinessId = model.Id;
                            modelMsg.BussinesType = BusinessType.Contract.ToString();
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
                        //WriteSMS(project.PMId.Value, 0, MessageStep.ContractAdd, parameterSms);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddContract");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateContract(Epm_Contract model, List<Base_Files> fileList = null)
        {
            Result<int> result = new Result<int>();
            try
            {
                bool dConfig = DataOperateBusiness<Epm_Contract>.Get().Count(i => i.Name == model.Name && i.Id != model.Id) > 0;
                if (dConfig)
                {
                    throw new Exception("该合同名称已经存在");
                }
                dConfig = DataOperateBusiness<Epm_Contract>.Get().Count(i => i.Code == model.Code && i.Id != model.Id) > 0;
                if (dConfig)
                {
                    throw new Exception("该合同编码已经存在");
                }

                if (fileList != null)
                {
                    //删除之前的附件
                    DeleteFilesByTable(model.GetType().Name, new List<long>() { model.Id });
                    //新增附件
                    AddFilesByTable(model, fileList);
                }

                //处理待办
                var temp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == model.Id && t.IsApprover == false).FirstOrDefault();
                if (temp != null)
                {
                    ComplateApprover(temp.Id);
                }

                var oldModel = DataOperateBusiness<Epm_Contract>.Get().GetModel(model.Id);
                model = FiterUpdate(oldModel, model);
                model.CrtCompanyId = oldModel.CrtCompanyId;
                model.CrtCompanyName = oldModel.CrtCompanyName;
                model.ProjectId = model.ProjectId ?? 0;
                var rows = DataOperateBusiness<Epm_Contract>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Contract.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
                //生成代办消息
                if ((ApprovalState)Enum.ToObject(typeof(ApprovalState), model.State) == ApprovalState.WaitAppr)
                {
                    bool isApprSuccess = false;
                    ////判断是否工程处提交，如果是直接通过
                    //if (IsAgencyUser(CurrentUserID.ToLongReq()))
                    //{
                    isApprSuccess = true;
                    model.State = (int)ApprovalState.ApprSuccess;
                    DataOperateBusiness<Epm_Contract>.Get().Update(model);
                    //}

                    if (!isApprSuccess && model.ProjectId.Value != 0)
                    {
                        #region 生成待办
                        var project = DataOperateBusiness<Epm_Project>.Get().GetModel(model.ProjectId.Value);
                        List<Epm_Approver> list = new List<Epm_Approver>();
                        Epm_Approver app = new Epm_Approver();
                        app.Title = CurrentUserName + "上传了项目合同，待审核";
                        app.Content = CurrentUserName + "上传了项目合同，待审核";
                        app.SendUserId = CurrentUserID.ToLongReq();
                        app.SendUserName = CurrentUserName;
                        app.SendTime = DateTime.Now;
                        app.LinkURL = string.Empty;
                        app.BusinessTypeNo = BusinessType.Contract.ToString();
                        app.Action = SystemRight.Add.ToString();
                        app.BusinessTypeName = BusinessType.Contract.GetText();
                        app.BusinessState = (int)(ApprovalState.WaitAppr);
                        app.BusinessId = model.Id;
                        app.ApproverId = project.PMId;
                        app.ApproverName = project.PMName;
                        app.ProjectId = model.ProjectId;
                        app.ProjectName = project.Name;
                        list.Add(app);
                        AddApproverBatch(list);
                        WriteLog(BusinessType.Contract.GetText(), SystemRight.Add.GetText(), "提交合同生成待办: " + model.Id);
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
                            modelMsg.Title = CurrentUserName + "上传了项目合同，待审核";
                            modelMsg.Content = CurrentUserName + "上传了项目合同，待审核";
                            modelMsg.Type = 2;
                            modelMsg.IsRead = false;
                            modelMsg.BussinessId = model.Id;
                            modelMsg.BussinesType = BusinessType.Contract.ToString();
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
                        //WriteSMS(project.PMId.Value, 0, MessageStep.ContractAdd, parameterSms);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateContract");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteContractByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_Contract>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                //if (models.Any(t => t.State != (int)ApprovalState.Discarded && t.State != (int)ApprovalState.Enabled))
                //{
                //    throw new Exception("草稿，已废弃状态下，才可删除");
                //}

                if (models.Any(t => t.ContractType.HasValue && t.ContractType == (int)ContractType.FrameContract))
                {
                    throw new Exception("框架合同不可以删除");
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
                        modelMsg.Title = CurrentUserName + "上传的项目合同已删除";
                        modelMsg.Content = CurrentUserName + "上传的项目合同已删除";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Contract.ToString();
                        modelMsg.ProjectId = model.ProjectId;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion
                }

                var rows = DataOperateBusiness<Epm_Contract>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Contract.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteContractByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_Contract>> GetContractList(QueryCondition qc)
        {
            qc = AddDefault(qc);

            #region  条件

            ConditionExpression ce = new ConditionExpression();
            ce.ExpName = "IsDelete";
            ce.ExpValue = false;
            ce.ExpOperater = eConditionOperator.Equal;
            qc.ConditionList.Add(ce);

            //2、草稿状态数据只有添加人自己可以看（项目无草稿状态）
            ConditionExpression ce1 = new ConditionExpression();
            ConditionExpression ce11 = new ConditionExpression();
            ce11.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "State",
                ExpValue = 0,
                ExpOperater = eConditionOperator.Equal
            });
            ce11.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "CreateUserId",
                ExpValue = CurrentUserID.ToLongReq(),
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            ce11.ExpLogical = eLogicalOperator.And;
            ce1.ConditionList.Add(ce11);

            ConditionExpression ce12 = new ConditionExpression();
            ce12.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "State",
                ExpValue = (int)ApprovalState.Enabled,
                ExpOperater = eConditionOperator.NotEqual

            });
            ce12.ExpLogical = eLogicalOperator.Or;
            ce1.ExpLogical = eLogicalOperator.And;
            ce1.ConditionList.Add(ce12);

            qc.ConditionList.Add(ce1);

            if (!string.IsNullOrEmpty(CurrentCompanyID))
            {
                long companyId = Convert.ToInt64(CurrentCompanyID);
                var m = DataOperateBasic<Base_Company>.Get().GetList(t => t.Id == companyId).FirstOrDefault();

                if (m.Type != "Owner")
                {
                    if (!string.IsNullOrWhiteSpace(CurrentProjectIds))
                    {
                        //3、查询列表数据时需根据登录用户负责项目进行筛选
                        ConditionExpression ce4 = new ConditionExpression();
                        ConditionExpression ce41 = new ConditionExpression();
                        ce41.ConditionList.Add(new ConditionExpression()
                        {
                            ExpName = "ProjectId",
                            ExpValue = CurrentProjectIds,
                            ExpOperater = eConditionOperator.In
                        });
                        ce4.ConditionList.Add(ce41);

                        ConditionExpression ce42 = new ConditionExpression();
                        ce42.ConditionList.Add(new ConditionExpression()
                        {
                            ExpName = "ProjectId",
                            ExpValue = null,
                            ExpOperater = eConditionOperator.Equal

                        });
                        ce42.ExpLogical = eLogicalOperator.Or;
                        ce4.ExpLogical = eLogicalOperator.And;
                        ce4.ConditionList.Add(ce42);
                        qc.ConditionList.Add(ce4);
                    }
                }
            }
            //1、列表数据根据最后修改时间倒序
            qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));

            #endregion

            Result<List<Epm_Contract>> result = new Result<List<Epm_Contract>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Contract>(context, qc);
                var ids = result.Data.Select(t => t.ProjectId);
                var list = DataOperateBusiness<Epm_ProjectStateTrack>.Get().GetList(t => ids.Contains(t.ProjectId) && (t.StateTrackNo == "VisaNum" || t.StateTrackNo == "ChangeNum")).ToList();
                foreach (Epm_Contract item in result.Data)
                {
                    foreach (var track in list)
                    {
                        if (item.ProjectId == track.ProjectId)
                        {
                            if (track.StateTrackNo == "VisaNum")
                                item.VisaNum = track.Qty;
                            if (track.StateTrackNo == "ChangeNum")
                                item.ChangeNum = track.Qty;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetContractList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_Contract> GetContractModel(long id)
        {
            Result<Epm_Contract> result = new Result<Epm_Contract>();
            try
            {
                var model = DataOperateBusiness<Epm_Contract>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetContractModel");
            }
            return result;
        }
        public Result<List<Base_Files>> GetContractModelFile(long id)
        {
            Result<List<Base_Files>> result = new Result<List<Base_Files>>();
            try
            {
                var list = DataOperateBasic<Base_Files>.Get().GetList(i => i.TableId == id).ToList();

                result.Data = list;
                result.AllRowsCount = list.Count();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetContractModelFile");
            }
            return result;
        }
        public Result<List<Base_Files>> GetContractModelFileName(long contractId, string fileNames)
        {
            Result<List<Base_Files>> result = new Result<List<Base_Files>>();
            try
            {
                var list = DataOperateBasic<Base_Files>.Get().GetList(i => i.TableId == contractId && i.Name == fileNames).ToList();

                result.Data = list;
                result.AllRowsCount = list.Count();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetContractModelFileName");
            }
            return result;
        }
        /// <summary>
        /// 修改状态(废弃)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeContractState(long id, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                var drawState = state.ToEnumReq<ApprovalState>();

                var model = DataOperateBusiness<Epm_Contract>.Get().GetModel(id);

                model.OperateUserId = CurrentUserID.ToLongReq();
                model.OperateUserName = CurrentUserName;
                model.OperateTime = DateTime.Now;
                model.State = int.Parse(drawState.GetValue().ToString());

                var rows = DataOperateBusiness<Epm_Contract>.Get().Update(model);

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
                app.Title = model.CreateUserName + "上传了项目合同，已废弃";
                app.Content = model.CreateUserName + "上传了项目合同，已废弃";
                app.SendUserId = CurrentUserID.ToLongReq();
                app.SendUserName = CurrentUserName;
                app.SendTime = DateTime.Now;
                app.LinkURL = string.Empty;
                app.BusinessTypeNo = BusinessType.Contract.ToString();
                app.Action = SystemRight.Invalid.ToString();
                app.BusinessTypeName = BusinessType.Contract.GetText();
                app.BusinessState = (int)(ApprovalState.Discarded);
                app.BusinessId = model.Id;
                app.ApproverId = project.PMId;
                app.ApproverName = project.PMName;
                app.ProjectId = model.ProjectId;
                app.ProjectName = project.Name;
                list.Add(app);
                AddApproverBatch(list);
                WriteLog(BusinessType.Contract.GetText(), SystemRight.Invalid.GetText(), "废弃合同生成待办: " + model.Id);
                #endregion

                if (model.ProjectId.Value != 0)
                {
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
                        modelMsg.Title = model.CreateUserName + "上传了项目合同，已废弃";
                        modelMsg.Content = model.CreateUserName + "上传了项目合同，已废弃";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Contract.ToString();
                        modelMsg.ProjectId = model.ProjectId.Value;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion
                }
                result.Data = rows;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "ChangeContractState");
            }
            return result;
        }
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateContractState(long id, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBusiness<Epm_Contract>.Get().GetModel(id);
                if (model != null)
                {
                    model.OperateUserId = CurrentUserID.ToLongReq();
                    model.OperateUserName = CurrentUserName;
                    model.OperateTime = DateTime.Now;
                    model.State = (int)state.ToEnumReq<ApprovalState>();
                    var rows = DataOperateBusiness<Epm_Contract>.Get().Update(model);
                    result.Data = rows;
                    result.Flag = EResultFlag.Success;
                    WriteLog(BusinessType.Contract.GetText(), SystemRight.Modify.GetText(), "更新状态: " + rows);
                }
                else
                {
                    throw new Exception("该合同id不存在");
                }

                //处理待办
                var temp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == model.Id && t.IsApprover == false).FirstOrDefault();
                if (temp != null)
                {
                    ComplateApprover(temp.Id);
                }

                string title = "";
                if ((ApprovalState)Enum.ToObject(typeof(ApprovalState), model.State) == ApprovalState.ApprSuccess)
                {
                    if (model.ProjectId.Value != 0)
                    {
                        title = model.CreateUserName + "上传的合同信息，审核通过";
                        #region 生成待办
                        List<Epm_Approver> list = new List<Epm_Approver>();
                        Epm_Approver app = new Epm_Approver();
                        app.Title = model.CreateUserName + "上传的合同信息，审核通过";
                        app.Content = model.CreateUserName + "上传的合同信息，审核通过";
                        app.SendUserId = model.CreateUserId;
                        app.SendUserName = model.CreateUserName;
                        app.SendTime = model.CreateTime;
                        app.LinkURL = string.Empty;
                        app.BusinessTypeNo = BusinessType.Contract.ToString();
                        app.Action = SystemRight.Check.ToString();
                        app.BusinessTypeName = BusinessType.Contract.GetText();
                        app.BusinessState = (int)(ApprovalState.ApprSuccess);
                        app.BusinessId = model.Id;
                        app.ApproverId = model.CreateUserId;
                        app.ApproverName = model.CreateUserName;
                        app.ProjectId = model.ProjectId;
                        app.ProjectName = model.ProjectName;
                        list.Add(app);
                        AddApproverBatch(list);
                        WriteLog(BusinessType.Contract.GetText(), SystemRight.Check.GetText(), "审核通过合同生成待办: " + model.Id);
                        #endregion
                    }
                }
                else if ((ApprovalState)Enum.ToObject(typeof(ApprovalState), model.State) == ApprovalState.ApprFailure)
                {
                    if (model.ProjectId.Value != 0)
                    {
                        title = model.CreateUserName + "上传的合同信息已被驳回，请处理";
                        #region 生成待办
                        List<Epm_Approver> list = new List<Epm_Approver>();
                        Epm_Approver app = new Epm_Approver();
                        app.Title = model.CreateUserName + "上传的合同信息已被驳回，请处理";
                        app.Content = model.CreateUserName + "上传的合同信息已被驳回，请处理";
                        app.SendUserId = model.CreateUserId;
                        app.SendUserName = model.CreateUserName;
                        app.SendTime = model.CreateTime;
                        app.LinkURL = string.Empty;
                        app.BusinessTypeNo = BusinessType.Contract.ToString();
                        app.Action = SystemRight.UnCheck.ToString();
                        app.BusinessTypeName = BusinessType.Contract.GetText();
                        app.BusinessState = (int)(ApprovalState.ApprFailure);
                        app.BusinessId = model.Id;
                        app.ApproverId = model.CreateUserId;
                        app.ApproverName = model.CreateUserName;
                        app.ProjectId = model.ProjectId;
                        app.ProjectName = model.ProjectName;
                        list.Add(app);
                        AddApproverBatch(list);
                        WriteLog(BusinessType.Contract.GetText(), SystemRight.UnCheck.GetText(), "驳回合同生成待办: " + model.Id);
                        #endregion

                        #region 发送短信
                        //WriteSMS(model.CreateUserId, 0, MessageStep.ContractReject, null);
                        #endregion
                    }
                }
                if (model.ProjectId.Value != 0)
                {
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
                        modelMsg.BussinesType = BusinessType.Contract.ToString();
                        modelMsg.ProjectId = model.ProjectId.Value;
                        modelMsg.ProjectName = model.ProjectName;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion
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

        public Result<int> DeleteContractModel(long id)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBusiness<Epm_Contract>.Get().GetModel(id);

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

                var rows = DataOperateBusiness<Epm_Contract>.Get().Delete(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Contract.GetText(), SystemRight.Delete.GetText(), "删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteContractModel");
            }
            return result;
        }
        public Result<int> DeleteFilesByTableIds(string tableName, List<long> tableIds)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBasic<Base_Files>.Get().GetList(i => tableIds.Contains(i.TableId) && i.TableName == tableName).ToList();
                foreach (var item in models)
                {
                    item.DeleteTime = DateTime.Now;
                }
                int rows = DataOperateBasic<Base_Files>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Contract.GetText(), SystemRight.Delete.GetText(), "删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteFilesByTable");
            }
            return result;
        }
    }
}
