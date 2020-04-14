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
        public Result<int> AddReformRecord(ReformRecordView model)
        {
            Result<int> result = new Result<int>();
            try
            {
                model.ReformRecord = base.SetCurrentUser(model.ReformRecord);
                model.ReformRecord.State = 0;
                model.ReformRecord.AreaCompanyCode = "10";
                model.ReformRecord.AreaCompanyName = "陕西";
                var rows = DataOperateBusiness<Epm_ReformRecord>.Get().Add(model.ReformRecord);

                if (model.Attachs != null)
                {
                    //新增投资管理附件
                    AddFilesBytzTable(model.ReformRecord, model.Attachs);
                }

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.ReformRecord.GetText(), SystemRight.Add.GetText(), "新增: " + model.ReformRecord.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddReformRecord");
            }
            return result;
        }

        public Result<int> AddReformRecordeEtity(Epm_ReformRecord model)
        {
            Result<int> result = new Result<int>();
            try
            {
                int rows = 0;
                bool isAdd = false;
                var first = DataOperateBusiness<Epm_ReformRecord>.Get().GetList(t => t.ProjectName == model.ProjectName && t.State != (int)PreProjectState.ApprovalFailure && t.State != (int)PreProjectState.Discarded).FirstOrDefault();

                if (first == null)
                {
                    isAdd = true;
                    first = new Epm_ReformRecord();
                    SetCreateUser(first);
                }
                first.ProjectName = model.ProjectName;
                first.ProjectId = model.ProjectId;
                first.ProjectCodeInvest = model.ProjectCodeInvest;
                first.ProjectCodeWhole = model.ProjectCodeWhole;
                first.ProjectCodeProject = model.ProjectCodeProject;
                first.RemarkType = model.RemarkType;
                first.StationName = model.StationName;
                first.StationId = model.StationId;
                first.StationCodeInvest = model.StationCodeInvest;
                first.StationCodeWhole = model.StationCodeWhole;
                first.StationCodeProject = model.StationCodeProject;
                first.CompanyName = model.CompanyName;
                first.CompanyId = model.CompanyId;
                first.LimitType = model.LimitType;
                first.SourceFund = model.SourceFund;
                first.Investment = model.Investment;
                first.ItemNumber = model.ItemNumber;
                first.ItemTime = model.ItemTime;
                first.ItemUnit = model.ItemUnit;
                first.ItemMoney = model.ItemMoney;
                first.InitialNumber = model.InitialNumber;
                first.InitialMoney = model.InitialMoney;
                first.RemarkStartTime = model.RemarkStartTime;
                first.RemarkEndTime = model.RemarkEndTime;
                first.RemarkMoney = model.RemarkMoney;
                first.DecisionMaker = model.DecisionMaker;
                first.LeaderPerson = model.LeaderPerson;
                first.Operator = model.Operator;
                first.PipetteCoding = model.PipetteCoding;
                first.InitialSalesOfRefinedOil = model.InitialSalesOfRefinedOil;
                first.RemarkSalesOfRefinedOil = model.RemarkSalesOfRefinedOil;
                first.InitialSalesOfGas = model.InitialSalesOfGas;
                first.RemarkSalesOfGas = model.RemarkSalesOfGas;
                first.IRR = model.IRR;
                first.ProjectContent = model.ProjectContent;
                first.Remark = model.Remark;
                first.AnnexType = model.AnnexType;
                first.AnnexAddress = model.AnnexAddress;
                first.State = model.State;
                first.Approver = model.Approver;
                first.ApproverId = model.ApproverId;
                first.ApproveTime = model.ApproveTime;
                first.ApproveResult = model.ApproveResult;
                first.ApproveRemark = model.ApproveRemark;
                first.AreaCompanyName = model.AreaCompanyName;
                first.AreaCompanyCode = model.AreaCompanyCode;
                first.CostAmount = model.CostAmount;
                first.RemarkTypeName = model.RemarkTypeName;
                first.DecisionMakerId = model.DecisionMakerId;
                first.LeaderPersonId = model.LeaderPersonId;
                first.OperatorId = model.OperatorId;

                SetCurrentUser(first);

            

                if (isAdd)
                {
                    rows = DataOperateBusiness<Epm_ReformRecord>.Get().Add(first);
                }
                else
                {
                    rows = DataOperateBusiness<Epm_ReformRecord>.Get().Update(first);
                }

                //上传附件
                AddFilesBytzTable(first, model.TzAttachs);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzFirstNegotiation.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzFirstNegotiation");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateReformRecord(Epm_ReformRecord model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_ReformRecord>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.ReformRecord.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateReformRecord");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteReformRecordByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_ReformRecord>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_ReformRecord>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.ReformRecord.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteReformRecordByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_ReformRecord>> GetReformRecordList(QueryCondition qc)
        {
           // qc = AddDefault(qc);
            Result<List<Epm_ReformRecord>> result = new Result<List<Epm_ReformRecord>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_ReformRecord>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetReformRecordList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<ReformRecordView> GetReformRecordModel(long id)
        {
            Result<ReformRecordView> result = new Result<ReformRecordView>();
            try
            {
                ReformRecordView view = new ReformRecordView();
                var model = DataOperateBusiness<Epm_ReformRecord>.Get().GetModel(id);
                var tzAttachsList = GetFilesByTZTable("Epm_ReformRecord", id).Data;
                if (tzAttachsList != null && tzAttachsList.Any())
                {
                    view.Attachs = tzAttachsList;
                }

                view.ReformRecord = model;
                view.StationIds = model.StationId.ToString();
                view.CompanyIds = model.CompanyId.ToString();

                result.Data = view;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetReformRecordModel");
            }
            return result;
        }


        ///<summary>
        ///详情:
        ///</summary>
        /// <param name="id">数据Id，</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_ReformRecord> GetReformRecordEntity(long id)
        {
            Result<Epm_ReformRecord> result = new Result<Epm_ReformRecord>();
            try
            {
                var model = DataOperateBusiness<Epm_ReformRecord>.Get().Single(p => p.Id == id);

                List<Epm_TzAttachs> tzAttachsList = new List<Epm_TzAttachs>();
                tzAttachsList = GetFilesByTZTable("Epm_ReformRecord", model.Id).Data;
                if (tzAttachsList != null && tzAttachsList.Any())
                {
                    model.TzAttachs = tzAttachsList;
                }
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetReformRecordEntity");
            }
            return result;
        }

        public Result<int> UpdateReformRecordState(List<long> ids, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                foreach (var item in ids)
                {
                    var model = DataOperateBusiness<Epm_ReformRecord>.Get().GetModel(item);
                    var userModel = DataOperateBasic<Base_User>.Get().GetModel(model.OperateUserId);
                    if (model != null)
                    {
                        //SetCurrentUser(model);
                        model.State = (int)state.ToEnumReq<PreProjectApprovalState>();
                        var rows = DataOperateBusiness<Epm_ReformRecord>.Get().Update(model);

                        if (model.State == (int)PreProjectState.ApprovalSuccess)
                        {
                            TzResearchView view = new TzResearchView();
                            view.ProjectId = model.Id;
                            view.State = (int)PreProjectState.WaitSubmitted;
                            AddTzSiteSurvey(view);
                        }

                        result.Data = rows;
                        result.Flag = EResultFlag.Success;

                        #region 更新RPA数据
                        var hostAddress = System.Configuration.ConfigurationManager.AppSettings.Get("RPAPath");
                        var rpaModel = new RPA_ReformRecordHX();
                        var omadsModel = new OMADS_ReformRecordHX();
                        var tempModel = new TEMP_ReformRecordHX();
                        #region rpaModel
                        rpaModel.AnnexType = model.AnnexType;
                        rpaModel.Approver = model.Approver;
                        rpaModel.CompanyName = model.CompanyName;
                        rpaModel.ApproveRemark = model.ApproveRemark;
                        rpaModel.ApproveResult = model.ApproveResult == 0 ?"审批通过": model.ApproveResult == 1?"审批不通过":"";
                        rpaModel.ApproveTime = model.ApproveTime;
                        var files = GetFilesByTZTable("Epm_ReformRecord", model.Id);

                        for (int i = 0; i < files.Data.Count; i++)
                        {
                            rpaModel.FilePath += hostAddress + "?path=" + files.Data[i].FilePath + "&fileName=" + files.Data[i].Name + ',';
                        }
                        rpaModel.AreaCompanyCode = model.AreaCompanyCode;
                        rpaModel.AreaCompanyName = model.AreaCompanyName;
                        rpaModel.DecisionMaker = model.DecisionMaker;
                        rpaModel.InitialMoney = model.InitialMoney;
                        rpaModel.InitialNumber = model.InitialNumber;
                        rpaModel.InitialSalesOfGas = model.InitialSalesOfGas;
                        rpaModel.InitialSalesOfRefinedOil = model.InitialSalesOfRefinedOil;
                        rpaModel.ProjectName = model.ProjectName;
                        rpaModel.Investment = model.Investment;
                        rpaModel.IRR = model.IRR;
                        rpaModel.ItemMoney = model.ItemMoney;
                        rpaModel.ItemNumber = model.ItemNumber;
                        rpaModel.ItemTime = model.ItemTime;
                        rpaModel.ItemUnit = model.ItemUnit;
                        rpaModel.LeaderPerson = model.LeaderPerson;
                        rpaModel.LimitType = model.LimitType;
                        rpaModel.PipetteCoding = model.PipetteCoding;
                        rpaModel.ProjectContent = model.ProjectContent;
                        rpaModel.Remark = model.Remark;
                        rpaModel.RemarkEndTime = model.RemarkEndTime;
                        rpaModel.RemarkMoney = model.RemarkMoney;
                        rpaModel.RemarkSalesOfGas = model.RemarkSalesOfGas;
                        rpaModel.RemarkSalesOfRefinedOil = model.RemarkSalesOfRefinedOil;
                        rpaModel.RemarkStartTime = model.RemarkStartTime;
                        rpaModel.RemarkType = model.RemarkType;
                        rpaModel.SourceFund = model.SourceFund;
                        rpaModel.StationName = model.StationName;
                        var strName = model.CompanyName.Substring(0, 2);
                        switch (strName)
                        {
                            case "西安":
                                rpaModel.username = "sxxayw";
                                break;
                            case "咸阳":
                                rpaModel.username = "sxxyyw";
                                break;
                            case "渭南":
                                rpaModel.username = "sxwnyw";
                                break;
                            case "宝鸡":
                                rpaModel.username = "sxbjyw";
                                break;
                            case "铜川":
                                rpaModel.username = "sxtcyw";
                                break;
                            case "商洛":
                                rpaModel.username = "sxslyw";
                                break;
                            case "汉中":
                                rpaModel.username = "sxhzyw";
                                break;
                            case "安康":
                                rpaModel.username = "sxakyw";
                                break;
                            case "榆林":
                                rpaModel.username = "sxylyw";
                                break;
                            case "延安":
                                rpaModel.username = "sxyayw";
                                break;
                            default:
                                break;
                        }

                        rpaModel.WriteMark = 0;
                        rpaModel.WriteResult = 0;
                        rpaModel.FollowOperate = "";
                        rpaModel.FileNumber = files.Data.Count;

                        SetCreateUser(rpaModel);
                        SetCurrentUser(rpaModel);

                       
                        #endregion
                        #region omadsModel
                        omadsModel.AnnexType = model.AnnexType;
                        omadsModel.Approver = model.Approver;
                        omadsModel.CompanyName = model.CompanyName;
                        omadsModel.ApproveRemark = model.ApproveRemark;
                        omadsModel.ApproveResult = model.ApproveResult == 0 ? "审批通过" : model.ApproveResult == 1 ? "审批不通过" : "";
                        omadsModel.ApproveTime = model.ApproveTime;
                        var ofiles = GetFilesByTZTable("Epm_ReformRecord", model.Id);

                        for (int i = 0; i < ofiles.Data.Count; i++)
                        {
                            omadsModel.FilePath += hostAddress + "?path=" + ofiles.Data[i].FilePath + "&fileName=" + ofiles.Data[i].Name + ',';
                        }
                        omadsModel.AreaCompanyCode = model.AreaCompanyCode;
                        omadsModel.AreaCompanyName = model.AreaCompanyName;
                        omadsModel.DecisionMaker = model.DecisionMaker;
                        omadsModel.InitialMoney = model.InitialMoney;
                        omadsModel.InitialNumber = model.InitialNumber;
                        omadsModel.InitialSalesOfGas = model.InitialSalesOfGas;
                        omadsModel.InitialSalesOfRefinedOil = model.InitialSalesOfRefinedOil;
                        omadsModel.ProjectName = model.ProjectName;
                        omadsModel.Investment = model.Investment;
                        omadsModel.IRR = model.IRR;
                        omadsModel.ItemMoney = model.ItemMoney;
                        omadsModel.ItemNumber = model.ItemNumber;
                        omadsModel.ItemTime = model.ItemTime;
                        omadsModel.ItemUnit = model.ItemUnit;
                        omadsModel.LeaderPerson = model.LeaderPerson;
                        omadsModel.LimitType = model.LimitType;
                        omadsModel.PipetteCoding = model.PipetteCoding;
                        omadsModel.ProjectContent = model.ProjectContent;
                        omadsModel.Remark = model.Remark;
                        omadsModel.RemarkEndTime = model.RemarkEndTime;
                        omadsModel.RemarkMoney = model.RemarkMoney;
                        omadsModel.RemarkSalesOfGas = model.RemarkSalesOfGas;
                        omadsModel.RemarkSalesOfRefinedOil = model.RemarkSalesOfRefinedOil;
                        omadsModel.RemarkStartTime = model.RemarkStartTime;
                        omadsModel.RemarkType = model.RemarkType;
                        omadsModel.SourceFund = model.SourceFund;
                        omadsModel.StationName = model.StationName;
                        omadsModel.username = rpaModel.username;

                        omadsModel.WriteMark = 0;
                        omadsModel.WriteResult = 0;
                        omadsModel.FollowOperate = "";
                        omadsModel.FileNumber = files.Data.Count;

                        SetCreateUser(omadsModel);
                        SetCurrentUser(omadsModel);

                        #endregion
                        #region tempModel
                        tempModel.AnnexType = model.AnnexType;
                        tempModel.Approver = model.Approver;
                        tempModel.CompanyName = model.CompanyName;
                        tempModel.ApproveRemark = model.ApproveRemark;
                        tempModel.ApproveResult = model.ApproveResult == 0 ? "审批通过" : model.ApproveResult == 1 ? "审批不通过" : "";
                        tempModel.ApproveTime = model.ApproveTime;
                        var tfiles = GetFilesByTZTable("Epm_ReformRecord", model.Id);

                        for (int i = 0; i < files.Data.Count; i++)
                        {
                            tempModel.FilePath += hostAddress + "?path=" + tfiles.Data[i].FilePath + "&fileName=" + tfiles.Data[i].Name + ',';
                        }
                        tempModel.AreaCompanyCode = model.AreaCompanyCode;
                        tempModel.AreaCompanyName = model.AreaCompanyName;
                        tempModel.DecisionMaker = model.DecisionMaker;
                        tempModel.InitialMoney = model.InitialMoney;
                        tempModel.InitialNumber = model.InitialNumber;
                        tempModel.InitialSalesOfGas = model.InitialSalesOfGas;
                        tempModel.InitialSalesOfRefinedOil = model.InitialSalesOfRefinedOil;
                        tempModel.ProjectName = model.ProjectName;
                        tempModel.Investment = model.Investment;
                        tempModel.IRR = model.IRR;
                        tempModel.ItemMoney = model.ItemMoney;
                        tempModel.ItemNumber = model.ItemNumber;
                        tempModel.ItemTime = model.ItemTime;
                        tempModel.ItemUnit = model.ItemUnit;
                        tempModel.LeaderPerson = model.LeaderPerson;
                        tempModel.LimitType = model.LimitType;
                        tempModel.PipetteCoding = model.PipetteCoding;
                        tempModel.ProjectContent = model.ProjectContent;
                        tempModel.Remark = model.Remark;
                        tempModel.RemarkEndTime = model.RemarkEndTime;
                        tempModel.RemarkMoney = model.RemarkMoney;
                        tempModel.RemarkSalesOfGas = model.RemarkSalesOfGas;
                        tempModel.RemarkSalesOfRefinedOil = model.RemarkSalesOfRefinedOil;
                        tempModel.RemarkStartTime = model.RemarkStartTime;
                        tempModel.RemarkType = model.RemarkType;
                        tempModel.SourceFund = model.SourceFund;
                        tempModel.StationName = model.StationName;
                        tempModel.username = rpaModel.username;

                        tempModel.WriteMark = 0;
                        tempModel.WriteResult = 0;
                        tempModel.FollowOperate = "";
                        tempModel.FileNumber = files.Data.Count;

                        SetCreateUser(tempModel);
                        SetCurrentUser(tempModel);

                        
                        #endregion
                        DataOperateBusiness<RPA_ReformRecordHX>.Get().Add(rpaModel);
                        DataOperateBusiness<OMADS_ReformRecordHX>.Get().Add(omadsModel);
                        DataOperateBusiness<TEMP_ReformRecordHX>.Get().Add(tempModel);
                        #endregion
                    }
                    else
                    {
                        throw new Exception("该项目信息不存在");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateReformRecordState");
            }
            return result;
        }
    }
}
