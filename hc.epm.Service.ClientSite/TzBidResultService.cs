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
    /// <summary>
    /// 招标结果
    /// </summary>
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzBidResult(Epm_TzBidResult model)
        {
            Result<int> result = new Result<int>();
            try
            {
                int rows = 0;
                bool isAdd = false;
                var bidResult = DataOperateBusiness<Epm_TzBidResult>.Get().GetList(t => t.BidTd == model.BidTd && t.State != (int)PreProjectState.ApprovalFailure && t.State != (int)PreProjectState.Discarded).FirstOrDefault();

                if (bidResult == null)
                {
                    isAdd = true;
                    bidResult = new Epm_TzBidResult();
                    SetCreateUser(bidResult);
                }
                bidResult.TenderingName = model.TenderingName;
                bidResult.ProjectId = model.ProjectId;
                bidResult.BidTd = model.BidTd;
                bidResult.ProjectName = model.ProjectName;
                bidResult.ApprovalNo = model.ApprovalNo;
                bidResult.UndertakeContacts = model.UndertakeContacts;
                bidResult.BidType = model.BidType;
                bidResult.BidName = model.BidName;
                bidResult.UndertakeTel = model.UndertakeTel;
                bidResult.BidderOne = model.BidderOne;
                bidResult.QuotationOne = model.QuotationOne;
                bidResult.RemarkOne = model.RemarkOne;
                bidResult.BidderTwo = model.BidderTwo;
                bidResult.QuotationTwo = model.QuotationTwo;
                bidResult.RemarkTwo = model.RemarkTwo;
                bidResult.BidderThree = model.BidderThree;
                bidResult.QuotationThree = model.QuotationThree;
                bidResult.RemarkThree = model.RemarkThree;
                bidResult.BidderFour = model.BidderFour;
                bidResult.QuotationFour = model.QuotationFour;
                bidResult.RemarkFour = model.RemarkFour;
                bidResult.RecommendUnit = model.RecommendUnit;
                bidResult.RecommendReason = model.RecommendReason;
                bidResult.Remark = model.Remark;
                bidResult.State = model.State;
                SetCurrentUser(bidResult);

                #region  招标结果协同接口
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                {
                    //招标申请实体信息
                    var tenderingApplyInfo = DataOperateBusiness<Epm_TzTenderingApply>.Get().GetModel(model.BidTd.Value);
                    XtTzBidResultView view = new XtTzBidResultView();
                    view.ProjectName = tenderingApplyInfo.ProjectName;
                    view.UndertakeDepartment = tenderingApplyInfo.UndertakeDepartment;
                    view.UndertakeContacts = tenderingApplyInfo.UndertakeContacts;
                    view.UndertakeTel = tenderingApplyInfo.UndertakeTel;
                    view.Minutes = tenderingApplyInfo.Minutes;
                    view.BidName = tenderingApplyInfo.BidName;
                    view.CapitalBudget = tenderingApplyInfo.CapitalBudget;
                    view.ProjectSummary = tenderingApplyInfo.ProjectSummary;
                    view.InvitationNegotiate = tenderingApplyInfo.InvitationNegotiate;
                    view.InvitationNegotiator = tenderingApplyInfo.InvitationNegotiator;
                    view.BidderOne = model.BidderOne;
                    view.QuotationOne = model.QuotationOne;
                    view.RemarkOne = model.RemarkOne;
                    view.BidderTwo = model.BidderTwo;
                    view.QuotationTwo = model.QuotationTwo;
                    view.RemarkTwo = model.RemarkTwo;
                    view.BidderThree = model.BidderThree;
                    view.QuotationThree = model.QuotationThree;
                    view.RemarkThree = model.RemarkThree;
                    view.RecommendUnit = model.RecommendUnit;
                    view.RecommendReason = model.RecommendReason;
                    var baseUser = DataOperateBasic<Base_User>.Get().GetModel(bidResult.CreateUserId);
                    if (baseUser == null)
                    {
                        throw new Exception("未找到申请人相关信息！");
                    }

                    view.hr_sqr = baseUser.ObjeId;
                    bidResult.WorkFlowId = XtWorkFlowService.CreateTzBidResultWorkFlow(view);
                }
                #endregion
                if (isAdd)
                {
                    rows = DataOperateBusiness<Epm_TzBidResult>.Get().Add(bidResult);
                }
                else
                {
                    rows = DataOperateBusiness<Epm_TzBidResult>.Get().Update(bidResult);
                }
                result.Data = rows;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzBidResult");
            }
            return result;
        }

        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzBidResult(Epm_TzBidResult model)
        {
            Result<int> result = new Result<int>();
            try
            {
                #region  招标结果协同接口
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                {
                    //招标申请实体信息
                    var tenderingApplyInfo = DataOperateBusiness<Epm_TzTenderingApply>.Get().GetModel(model.BidTd.Value);
                    XtTzBidResultView view = new XtTzBidResultView();
                    view.ProjectName = tenderingApplyInfo.ProjectName;
                    view.UndertakeDepartment = tenderingApplyInfo.UndertakeDepartment;
                    view.UndertakeContacts = tenderingApplyInfo.UndertakeContacts;
                    view.UndertakeTel = tenderingApplyInfo.UndertakeTel;
                    view.Minutes = tenderingApplyInfo.Minutes;
                    view.BidName = tenderingApplyInfo.BidName;
                    view.CapitalBudget = tenderingApplyInfo.CapitalBudget;
                    view.ProjectSummary = tenderingApplyInfo.ProjectSummary;
                    view.InvitationNegotiate = tenderingApplyInfo.InvitationNegotiate;
                    view.InvitationNegotiator = tenderingApplyInfo.InvitationNegotiator;
                    view.BidderOne = model.BidderOne;
                    view.QuotationOne = model.QuotationOne;
                    view.RemarkOne = model.RemarkOne;
                    view.BidderTwo = model.BidderTwo;
                    view.QuotationTwo = model.QuotationTwo;
                    view.RemarkTwo = model.RemarkTwo;
                    view.BidderThree = model.BidderThree;
                    view.QuotationThree = model.QuotationThree;
                    view.RemarkThree = model.RemarkThree;
                    view.RecommendUnit = model.RecommendUnit;
                    view.RecommendReason = model.RecommendReason;
                    var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.CreateUserId);
                    if (baseUser == null)
                    {
                        throw new Exception("未找到申请人相关信息！");
                    }

                    view.hr_sqr = baseUser.ObjeId;
                    model.WorkFlowId = XtWorkFlowService.CreateTzBidResultWorkFlow(view);
                }
                #endregion

                var rows = DataOperateBusiness<Epm_TzBidResult>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzBidResult.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzBidResult");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzBidResultByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzBidResult>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzBidResult>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzBidResult.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzBidResultByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzBidResult>> GetTzBidResultList(QueryCondition qc)
        {
          //  qc = AddDefault(qc);
            Result<List<Epm_TzBidResult>> result = new Result<List<Epm_TzBidResult>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzBidResult>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzBidResultList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzBidResult> GetTzBidResultModel(long id)
        {
            Result<Epm_TzBidResult> result = new Result<Epm_TzBidResult>();
            try
            {
                var model = DataOperateBusiness<Epm_TzBidResult>.Get().GetModel(id);
                if (model != null)
                {
                    var tzTenderingApply = DataOperateBusiness<Epm_TzTenderingApply>.Get().GetModel(model.BidTd.Value);
                    model.TzTenderingApply = tzTenderingApply;
                }

                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzBidResultModel");
            }
            return result;
        }


        /// <summary>
        /// 修改招标结果状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzBidResultState(List<long> ids, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                foreach (var item in ids)
                {
                    var model = DataOperateBusiness<Epm_TzBidResult>.Get().GetModel(item);
                    if (model != null)
                    {
                        SetCurrentUser(model);
                        model.State = (int)state.ToEnumReq<PreProjectState>();
                        var rows = DataOperateBusiness<Epm_TzBidResult>.Get().Update(model);
                        
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
                result.Exception = new ExceptionEx(ex, "UpdateTzBidResultState");
            }
            return result;
        }
    }
}
