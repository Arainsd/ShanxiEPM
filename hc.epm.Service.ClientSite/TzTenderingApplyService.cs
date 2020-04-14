using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using hc.epm.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hc.epm.DataModel.Basic;

namespace hc.epm.Service.ClientSite
{
    /// <summary>
    /// 招标申请
    /// </summary>
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzTenderingApply(Epm_TzTenderingApply model)
        {
            Result<int> result = new Result<int>();
            try
            {
                SetCreateUser(model);
                SetCurrentUser(model);

                //上传附件
                AddFilesBytzTable(model, model.TzAttachs);

                #region  招标申请协同接口
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                {
                    XtTzTenderingApplyView view = new XtTzTenderingApplyView();

                    view.ProjectName = model.ProjectName;
                    view.UndertakeDepartment = model.UndertakeDepartment;
                    view.UndertakeContacts = model.UndertakeContacts;
                    view.UndertakeTel = model.UndertakeTel;
                    view.Minutes = model.ApprovalNo;
                    view.TenderingName = model.TenderingName;
                    view.TenderingType = model.TenderingType;
                    view.BidName = model.BidName;
                    view.CapitalBudget = model.CapitalBudget;
                    view.ProjectSummary = model.ProjectSummary;
                    var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.CreateUserId);
                    if (baseUser == null)
                    {
                        throw new Exception("未找到申请人相关信息！");
                    }

                    view.hr_sqr = baseUser.ObjeId;
                    //上传附件
                    if (model.TzAttachs != null && model.TzAttachs.Any())
                    {
                        //string baseFaleUrl = System.Configuration.ConfigurationManager.AppSettings.Get("XtDownloadUrl");
                        //foreach (var item in model.TzAttachs)
                        //{
                        //    //string fileUrl = string.Format("{0}?fileId={1}&type={2}", baseFaleUrl, item.Id, item.TypeNo);
                        //    //view.Temp_TzAttachs = fileUrl + '|' + view.Temp_TzAttachs;
                        //    string fileUrl = XtWorkFlowService.GetXtAttachPaht(item.FilePath);
                        //    view.Temp_TzAttachs = fileUrl + '|' + view.Temp_TzAttachs;
                        //}

                        //if (view.Temp_TzAttachs != null)
                        //{
                        //    view.Temp_TzAttachs = view.Temp_TzAttachs.Substring(0, view.Temp_TzAttachs.Length - 1);
                        //}
                        view.Temp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(model.TzAttachs);
                    }

                    model.WorkFlowId = XtWorkFlowService.CreateTzTenderingApplyWorkFlow(view);
                }
                #endregion

                var rows = DataOperateBusiness<Epm_TzTenderingApply>.Get().Add(model);

                
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTenderingApply.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzTenderingApply");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzTenderingApply(Epm_TzTenderingApply model)
        {
            Result<int> result = new Result<int>();
            try
            {
                SetCurrentUser(model);
                //上传附件
                AddFilesBytzTable(model, model.TzAttachs);
                #region  招标申请协同接口
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                {
                    XtTzTenderingApplyView view = new XtTzTenderingApplyView();

                    view.ProjectName = model.ProjectName;
                    view.UndertakeDepartment = model.UndertakeDepartment;
                    view.UndertakeContacts = model.UndertakeContacts;
                    view.UndertakeTel = model.UndertakeTel;
                    view.Minutes = model.ApprovalNo;
                    view.TenderingName = model.TenderingName;
                    view.TenderingType = model.TenderingType;
                    view.BidName = model.BidName;
                    view.CapitalBudget = model.CapitalBudget;
                    view.ProjectSummary = model.ProjectSummary;
                    var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.CreateUserId);
                    if (baseUser == null)
                    {
                        throw new Exception("未找到申请人相关信息！");
                    }

                    view.hr_sqr = baseUser.ObjeId;
                    //上传附件
                    if (model.TzAttachs != null && model.TzAttachs.Any())
                    {
                        // string baseFaleUrl = System.Configuration.ConfigurationManager.AppSettings.Get("XtDownloadUrl");
                        //foreach (var item in model.TzAttachs)
                        //{
                        //    //string fileUrl = string.Format("{0}?fileId={1}&type={2}", baseFaleUrl, item.Id, item.TypeNo);
                        //    //view.Temp_TzAttachs = fileUrl + '|' + view.Temp_TzAttachs;
                        //    string fileUrl = XtWorkFlowService.GetXtAttachPaht(item.FilePath);
                        //    view.Temp_TzAttachs = fileUrl + '|' + view.Temp_TzAttachs;
                        //}

                        //if (view.Temp_TzAttachs != null)
                        //{
                        //    view.Temp_TzAttachs = view.Temp_TzAttachs.Substring(0, view.Temp_TzAttachs.Length - 1);
                        //}
                        view.Temp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(model.TzAttachs);
                    }

                    model.WorkFlowId = XtWorkFlowService.CreateTzTenderingApplyWorkFlow(view);
                }
                #endregion

                var rows = DataOperateBusiness<Epm_TzTenderingApply>.Get().Update(model);

                
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTenderingApply.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzTenderingApply");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzTenderingApplyByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzTenderingApply>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzTenderingApply>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTenderingApply.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzTenderingApplyByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzTenderingApply>> GetTzTenderingApplyList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzTenderingApply>> result = new Result<List<Epm_TzTenderingApply>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzTenderingApply>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzTenderingApplyList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzTenderingApply> GetTzTenderingApplyModel(long id)
        {
            Result<Epm_TzTenderingApply> result = new Result<Epm_TzTenderingApply>();
            try
            {
                var model = DataOperateBusiness<Epm_TzTenderingApply>.Get().GetModel(id);
                if (model != null)
                {
                    List<Epm_TzAttachs> tzAttachsList = new List<Epm_TzAttachs>();
                    tzAttachsList = GetFilesByTZTable("Epm_TzTenderingApply", id).Data;
                    if (tzAttachsList != null && tzAttachsList.Any())
                    {
                        model.TzAttachs = tzAttachsList;
                    }
                }
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzTenderingApplyModel");
            }
            return result;
        }


        /// <summary>
        /// 修改招标申请状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzTenderingApplyState(List<long> ids, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                foreach (var item in ids)
                {
                    var model = DataOperateBusiness<Epm_TzTenderingApply>.Get().GetModel(item);
                    if (model != null)
                    {
                        //SetCurrentUser(model);
                        model.State = (int)state.ToEnumReq<PreProjectState>();
                        var rows = DataOperateBusiness<Epm_TzTenderingApply>.Get().Update(model);

                        //若状态为已审批，生成下一阶段数据
                        if (model.State == (int)PreProjectState.ApprovalSuccess)
                        {
                            //生成招标结果
                            Epm_TzBidResult tzBidResult = new Epm_TzBidResult();
                            tzBidResult.ProjectId = model.ProjectId;
                            tzBidResult.ProjectName = model.ProjectName;
                            tzBidResult.ApprovalNo = model.ApprovalNo;
                            tzBidResult.UndertakeContacts = model.UndertakeContacts;
                            tzBidResult.BidType = model.BidType;
                            tzBidResult.BidName = model.BidName;
                            tzBidResult.UndertakeTel = model.UndertakeTel;
                            tzBidResult.TenderingName = model.TenderingName;

                            tzBidResult.BidTd = model.Id;
                            tzBidResult.State = (int)PreProjectState.WaitSubmitted;
                            AddTzBidResult(tzBidResult);
                        }

                        result.Data = rows;
                        result.Flag = EResultFlag.Success;
                    }
                    else
                    {
                        throw new Exception("该招标申请信息不存在");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzTenderingApplyState");
            }
            return result;
        }
    }
}
