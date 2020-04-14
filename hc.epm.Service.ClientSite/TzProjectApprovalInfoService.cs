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
    /// 项目批复信息
    /// </summary>
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzProjectApprovalInfo(Epm_TzProjectApprovalInfo model)
        {
            Result<int> result = new Result<int>();
            try
            {
                int rows = 0;
                bool isAdd = false;
                var approvalInfo = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().GetList(t => t.ProjectId == model.ProjectId && t.State != (int)PreProjectState.ApprovalFailure && t.State != (int)PreProjectState.Discarded).FirstOrDefault();

                if (approvalInfo == null)
                {
                    isAdd = true;
                    approvalInfo = new Epm_TzProjectApprovalInfo();

                    approvalInfo.RegionCompany = model.RegionCompany;
                    approvalInfo.ProjectType = model.ProjectType;
                    approvalInfo.ProjectTypeCode = model.ProjectTypeCode;

                    SetCreateUser(approvalInfo);
                }
                approvalInfo.ProjectId = model.ProjectId;
                approvalInfo.LimitCode = model.LimitCode;
                approvalInfo.LimitName = model.LimitName;
                approvalInfo.StandardCode = model.StandardCode;
                approvalInfo.StandardName = model.StandardName;
                approvalInfo.ReviewCode = model.ReviewCode;
                approvalInfo.ReviewName = model.ReviewName;
                approvalInfo.DraftAuthorId = model.DraftAuthorId;
                approvalInfo.DraftAuthorName = model.DraftAuthorName;
                approvalInfo.DraftAuthorXt = model.DraftAuthorXt;
                approvalInfo.SignPeopleId = model.SignPeopleId;
                approvalInfo.SignPeopleName = model.SignPeopleName;
                approvalInfo.SignPeopleXt = model.SignPeopleXt;
                approvalInfo.SignerId = model.SignerId;
                approvalInfo.SignerName = model.SignerName;
                approvalInfo.SignerXt = model.SignerXt;
                approvalInfo.FieldManagerId = model.FieldManagerId;
                approvalInfo.FieldManagerName = model.FieldManagerName;
                approvalInfo.FieldManagerXt = model.FieldManagerXt;
                approvalInfo.DecisionMakerId = model.DecisionMakerId;
                approvalInfo.DecisionMakerName = model.DecisionMakerName;
                approvalInfo.DecisionMakerXt = model.DecisionMakerXt;
                approvalInfo.HeadOperationsId = model.HeadOperationsId;
                approvalInfo.HeadOperationsName = model.HeadOperationsName;
                approvalInfo.HeadOperationsXt = model.HeadOperationsXt;
                approvalInfo.ApprovalNo = model.ApprovalNo;
                approvalInfo.TotalInvestment = model.TotalInvestment;
                approvalInfo.ContractPayment = model.ContractPayment;
                approvalInfo.EngineeringCost = model.EngineeringCost;
                approvalInfo.LandCosts = model.LandCosts;
                approvalInfo.OtherExpenses = model.OtherExpenses;
                approvalInfo.IssuedPlan = model.IssuedPlan;
                approvalInfo.DateFirstScheme = model.DateFirstScheme;
                approvalInfo.MdmCode = model.MdmCode;
                approvalInfo.ReleaseInvestmentAmount = model.ReleaseInvestmentAmount;
                approvalInfo.Batch = model.Batch;
                approvalInfo.Year = model.Year;
                approvalInfo.MachineOfOilStage = model.MachineOfOilStage;
                approvalInfo.MachineOfOil = model.MachineOfOil;
                approvalInfo.MachineOfGasStage = model.MachineOfGasStage;
                approvalInfo.MachineOfGas = model.MachineOfGas;
                approvalInfo.Shelter = model.Shelter;
                approvalInfo.OilTank = model.OilTank;
                approvalInfo.StationRoom = model.StationRoom;
                approvalInfo.TankNumber = model.TankNumber;
                approvalInfo.GasWells = model.GasWells;
                approvalInfo.LandPaymentType = model.LandPaymentType;
                approvalInfo.LandPayment = model.LandPayment;
                approvalInfo.AssetType = model.AssetType;
                approvalInfo.AssetTypeName = model.AssetTypeName;
                approvalInfo.ProportionOfShare = model.ProportionOfShare;
                approvalInfo.LandStatusCode = model.LandStatusCode;
                approvalInfo.LandStatus = model.LandStatus;
                approvalInfo.LandUseCode = model.LandUseCode;
                approvalInfo.LandUse = model.LandUse;
                approvalInfo.AreaOfLand = model.AreaOfLand;
                approvalInfo.RemarkOnLandCost = model.RemarkOnLandCost;
                approvalInfo.ExpectedPaymentThisYear = model.ExpectedPaymentThisYear;
                approvalInfo.EstimatedTimeOfSales = model.EstimatedTimeOfSales;
                approvalInfo.UnitFeasibilityCompilation = model.UnitFeasibilityCompilation;
                approvalInfo.TotalInvestmentAfterChange = model.TotalInvestmentAfterChange;
                approvalInfo.ChangeReplyNo = model.ChangeReplyNo;
                approvalInfo.FormOfEquityInvestment = model.FormOfEquityInvestment;
                approvalInfo.RegisteredCapital = model.RegisteredCapital;
                approvalInfo.HoldingTheProportion = model.HoldingTheProportion;
                approvalInfo.CapitalSourceCode = model.CapitalSourceCode;
                approvalInfo.CapitalSource = model.CapitalSource;
                approvalInfo.InvestmentAmount = model.InvestmentAmount;
                approvalInfo.CostAmount = model.CostAmount;
                approvalInfo.Leasehold = model.Leasehold;
                approvalInfo.AnnualRent = model.AnnualRent;
                approvalInfo.StratLeaseDate = model.StratLeaseDate;
                approvalInfo.EndLeaseDate = model.EndLeaseDate;
                approvalInfo.AvgLeaseDate = model.AvgLeaseDate;
                approvalInfo.PaymentMethod = model.PaymentMethod;
                approvalInfo.ProfitRateOfRent = model.ProfitRateOfRent;
                approvalInfo.DailyDieselSales = model.DailyDieselSales;
                approvalInfo.DailyGasolineSales = model.DailyGasolineSales;
                approvalInfo.ChaiQibi = model.ChaiQibi;
                approvalInfo.CNGY = model.CNGY;
                approvalInfo.LNGQ = model.LNGQ;
                approvalInfo.PayBackPeriod = model.PayBackPeriod;
                approvalInfo.InternalRateReturn = model.InternalRateReturn;
                approvalInfo.AnnualNonOilIncome = model.AnnualNonOilIncome;
                approvalInfo.NonOilAnnualCost = model.NonOilAnnualCost;
                approvalInfo.ScheduledComTime = model.ScheduledComTime;
                approvalInfo.FeasibleApprovalDate = model.FeasibleApprovalDate;
                approvalInfo.HasEInformation = model.HasEInformation;

                approvalInfo.RegionCompany = approvalInfo.RegionCompany;
                approvalInfo.ProjectType = approvalInfo.ProjectType;
                approvalInfo.ProjectTypeCode = approvalInfo.ProjectTypeCode;
                approvalInfo.ImplementationCcompany = model.ImplementationCcompany;
                approvalInfo.State = model.State;

                SetCurrentUser(approvalInfo);
                //上传附件
                AddFilesBytzTable(approvalInfo, model.TzAttachs);

                var project = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId);
                //特许经营项目性质的项目审批到项目批复完成就结束了，不生成工程模块的内容，不用写到机器人表里
                if (project.Nature != "XMXZTXJY")
                {
                    #region  项目批复流程申请     暂时注释  勿删！！！
                    var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                    if (model.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                    {
                        TzProjectApprovalInfoWorkFlowView view = new TzProjectApprovalInfoWorkFlowView();
                        
                        view.ProjectName = project.ProjectName;
                        view.StationName = project.StationName;
                        view.NatureName = project.NatureName;
                        view.Position = project.Position;
                        view.ApplyTime = project.ApplyTime.ToString();
                        view.CompanyName = project.CompanyName;
                        view.PredictMoney = project.ProjectCode;
                        view.OilSalesTotal = project.OilSalesTotal.ToString();
                        view.CNGY = model.CNG.ToString();
                        view.LNGQ = model.LNG.ToString();
                        view.StationTypeName = project.StationTypeName;
                        view.ProvinceName = project.ProvinceName;
                        view.LimitName = model.LimitName;
                        view.StandardName = model.StandardName;
                        view.ProjectCode = project.ProjectCode;
                        view.ApprovalNo = model.ApprovalNo;
                        view.SignerName = model.SignerName;
                        view.Name = model.CreateUserName;
                        view.SignPeopleName = model.SignPeopleName;
                        view.DecisionMakerNam = model.DecisionMakerName;
                        view.FieldManagerName = model.FieldManagerName;
                        view.HeadOperationsName = model.HeadOperationsName;
                        view.TotalInvestment = model.TotalInvestment.ToString();
                        view.ContractPayment = model.ContractPayment.ToString();
                        view.EngineeringCost = model.EngineeringCost.ToString();
                        view.LandCosts = model.LandCosts.ToString();
                        view.OtherExpenses = model.OtherExpenses.ToString();
                        view.DateFirstScheme = model.DateFirstScheme.ToString();
                        view.IssuedPlan = model.IssuedPlan.ToString();
                        view.MachineOfOilStage = model.MachineOfOilStage.ToString();
                        view.MachineOfOil = model.MachineOfOil.ToString();
                        view.MachineOfGasStage = model.MachineOfGasStage.ToString();
                        view.MachineOfGas = model.MachineOfGas.ToString();
                        view.Shelter = model.Shelter.ToString();
                        view.OilTank = model.OilTank.ToString();
                        view.StationRoom = model.StationRoom.ToString();
                        view.TankNumber = model.TankNumber.ToString();
                        view.GasWells = model.GasWells.ToString();
                        view.LandPaymentType = model.LandPaymentType;
                        view.AssetTypeName = model.AssetTypeName;
                        view.InvestmentAmount = model.InvestmentAmount.ToString();
                        view.RemarkOnLandCost = model.RemarkOnLandCost;
                        view.LandStatus = model.LandStatus;
                        view.LandUse = model.LandUse;
                        view.AreaOfLand = model.AreaOfLand.ToString();
                        view.ExpectedPaymentThisYear = model.ExpectedPaymentThisYear.ToString();
                        view.EstimatedTimeOfSales = model.EstimatedTimeOfSales.ToString();
                        view.UnitFeasibilityCompilation = model.UnitFeasibilityCompilation;
                        view.FormOfEquityInvestment = model.FormOfEquityInvestment;
                        view.ImplementationCcompany = model.ImplementationCcompany;
                        view.RegisteredCapital = model.RegisteredCapital.ToString();
                        view.HoldingTheProportion = model.HoldingTheProportion;
                        view.DailyDieselSales = model.DailyDieselSales.ToString();
                        view.DailyGasolineSales = model.DailyGasolineSales.ToString();
                        view.ChaiQibi = model.ChaiQibi;
                        view.CNGY = model.CNGY.ToString();
                        view.LNGQ = model.LNGQ.ToString();
                        view.PayBackPeriod = model.PayBackPeriod;
                        view.InternalRateReturn = model.InternalRateReturn;
                        view.AnnualNonOilIncome = model.AnnualNonOilIncome.ToString();
                        view.NonOilAnnualCost = model.NonOilAnnualCost.ToString();
                        view.ScheduledComTime = model.ScheduledComTime.ToString();
                        view.FeasibleApprovalDate = model.FeasibleApprovalDate.ToString();
                        view.HasEInformation = model.HasEInformation.ToString();
                        var baseUser = DataOperateBasic<Base_User>.Get().GetModel(approvalInfo.CreateUserId);
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
                            //    string fileUrl = string.Format("{0}?fileId={1}&type={2}", baseFaleUrl, item.Id, item.TypeNo);
                            //    view.Temp_TzAttachs = fileUrl + '|' + view.Temp_TzAttachs;
                            //}

                            //if (view.Temp_TzAttachs != null)
                            //{
                            //    view.Temp_TzAttachs = view.Temp_TzAttachs.Substring(0, view.Temp_TzAttachs.Length - 1);
                            //}
                            view.Temp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(model.TzAttachs);
                        }

                        approvalInfo.WorkFlowId = XtWorkFlowService.CreateProjectApprovalInfoWorkFlow(view);
                    }
                    #endregion
                }

                if (isAdd)
                {
                    rows = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().Add(approvalInfo);
                }
                else
                {
                    rows = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().Update(approvalInfo);
                }


                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzProjectApprovalInfo.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzProjectApprovalInfo");
            }
            return result;
        }

        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzProjectApprovalInfo(Epm_TzProjectApprovalInfo model)
        {
            Result<int> result = new Result<int>();
            try
            {
                #region  项目批复流程申请     暂时注释  勿删！！！
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                {
                    TzProjectApprovalInfoWorkFlowView view = new TzProjectApprovalInfoWorkFlowView();

                    var project = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId);
                    view.ProjectName = project.ProjectName;
                    view.StationName = project.StationName;
                    view.NatureName = project.NatureName;
                    view.Position = project.Position;
                    view.ApplyTime = project.ApplyTime.ToString();
                    view.CompanyName = project.CompanyName;
                    view.PredictMoney = project.ProjectCode;
                    view.OilSalesTotal = project.OilSalesTotal.ToString();
                    view.CNGY = model.CNG.ToString();
                    view.LNGQ = model.LNG.ToString();
                    view.StationTypeName = project.StationTypeName;
                    view.ProvinceName = project.ProvinceName;
                    view.LimitName = model.LimitName;
                    view.StandardName = model.StandardName;
                    view.ProjectCode = project.ProjectCode;
                    view.ApprovalNo = model.ApprovalNo;
                    view.SignerName = model.SignerName;
                    view.Name = model.CreateUserName;
                    view.SignPeopleName = model.SignPeopleName;
                    view.DecisionMakerNam = model.DecisionMakerName;
                    view.FieldManagerName = model.FieldManagerName;
                    view.HeadOperationsName = model.HeadOperationsName;
                    view.TotalInvestment = model.TotalInvestment.ToString();
                    view.ContractPayment = model.ContractPayment.ToString();
                    view.EngineeringCost = model.EngineeringCost.ToString();
                    view.LandCosts = model.LandCosts.ToString();
                    view.OtherExpenses = model.OtherExpenses.ToString();
                    view.DateFirstScheme = model.DateFirstScheme.ToString();
                    view.IssuedPlan = model.IssuedPlan.ToString();
                    view.MachineOfOilStage = model.MachineOfOilStage.ToString();
                    view.MachineOfOil = model.MachineOfOil.ToString();
                    view.MachineOfGasStage = model.MachineOfGasStage.ToString();
                    view.MachineOfGas = model.MachineOfGas.ToString();
                    view.Shelter = model.Shelter.ToString();
                    view.OilTank = model.OilTank.ToString();
                    view.StationRoom = model.StationRoom.ToString();
                    view.TankNumber = model.TankNumber.ToString();
                    view.GasWells = model.GasWells.ToString();
                    view.LandPaymentType = model.LandPaymentType;
                    view.AssetTypeName = model.AssetTypeName;
                    view.InvestmentAmount = model.InvestmentAmount.ToString();
                    view.RemarkOnLandCost = model.RemarkOnLandCost;
                    view.LandStatus = model.LandStatus;
                    view.LandUse = model.LandUse;
                    view.AreaOfLand = model.AreaOfLand.ToString();
                    view.ExpectedPaymentThisYear = model.ExpectedPaymentThisYear.ToString();
                    view.EstimatedTimeOfSales = model.EstimatedTimeOfSales.ToString();
                    view.UnitFeasibilityCompilation = model.UnitFeasibilityCompilation;
                    view.FormOfEquityInvestment = model.FormOfEquityInvestment;
                    view.ImplementationCcompany = model.ImplementationCcompany;
                    view.RegisteredCapital = model.RegisteredCapital.ToString();
                    view.HoldingTheProportion = model.HoldingTheProportion;
                    view.DailyDieselSales = model.DailyDieselSales.ToString();
                    view.DailyGasolineSales = model.DailyGasolineSales.ToString();
                    view.ChaiQibi = model.ChaiQibi;
                    view.CNG = model.CNG.ToString();
                    view.LNG = model.LNG.ToString();
                    view.PayBackPeriod = model.PayBackPeriod;
                    view.InternalRateReturn = model.InternalRateReturn;
                    view.AnnualNonOilIncome = model.AnnualNonOilIncome.ToString();
                    view.NonOilAnnualCost = model.NonOilAnnualCost.ToString();
                    view.ScheduledComTime = model.ScheduledComTime.ToString();
                    view.FeasibleApprovalDate = model.FeasibleApprovalDate.ToString();
                    view.HasEInformation = model.HasEInformation.ToString();

                    //view.Temp_TzAttachs = model.Temp_TzAttachs;
                    //上传附件
                    if (model.TzAttachs != null && model.TzAttachs.Any())
                    {
                        //string baseFaleUrl = System.Configuration.ConfigurationManager.AppSettings.Get("XtDownloadUrl");
                        //foreach (var item in model.TzAttachs)
                        //{
                        //    string fileUrl = string.Format("{0}?fileId={1}&type={2}", baseFaleUrl, item.Id, item.TypeNo);
                        //    view.Temp_TzAttachs = fileUrl + '|' + view.Temp_TzAttachs;
                        //}

                        //if (view.Temp_TzAttachs != null)
                        //{
                        //    view.Temp_TzAttachs = view.Temp_TzAttachs.Substring(0, view.Temp_TzAttachs.Length - 1);
                        //}
                        view.Temp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(model.TzAttachs);
                    }

                    model.WorkFlowId = XtWorkFlowService.CreateProjectApprovalInfoWorkFlow(view);
                }
                #endregion

                var rows = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzProjectApprovalInfo.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzProjectApprovalInfo");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzProjectApprovalInfoByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzProjectApprovalInfo.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzProjectApprovalInfoByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectProposal>> GetTzProjectApprovalInfoList(QueryCondition qc)
        {
            Result<List<Epm_TzProjectProposal>> result = new Result<List<Epm_TzProjectProposal>>();
            try
            {
                var query = from info in context.Epm_TzProjectApprovalInfo.Where(p => p.IsDelete == false)
                            join project in context.Epm_TzProjectProposal.Where(p => p.IsDelete == false) on info.ProjectId equals project.Id
                            select new
                            {
                                info.ProjectId,
                                project.ProjectName,
                                project.Nature,
                                project.NatureName,
                                project.StationName,
                                project.StationId,
                                project.ApplyTime,
                                info.State,
                                info.OperateUserName,
                                info.OperateTime,
                                info.CreateUserName,
                                project.CompanyId
                            };

                var projectList = query.ToList().Select(c => new Epm_TzProjectProposal
                {
                    Id = c.ProjectId,
                    ProjectName = c.ProjectName,
                    Nature = c.Nature,
                    NatureName = c.NatureName,
                    StationName = c.StationName,
                    StationId = c.StationId,
                    ApplyTime = c.ApplyTime,
                    State = c.State.Value,
                    //OperateUserName = c.OperateUserName,
                    OperateUserName = c.State == (int)PreProjectState.WaitSubmitted || c.State == (int)PreProjectState.ApprovalFailure ? c.CreateUserName : c.State == (int)PreProjectState.WaitApproval ? c.OperateUserName : "",
                    OperateTime = c.OperateTime,
                    CompanyId = c.CompanyId
                }).ToList();

                if (qc != null && qc.ConditionList.Any())
                {
                    foreach (var conditionExpression in qc.ConditionList)
                    {
                        string value = (conditionExpression.ExpValue ?? "").ToString();
                        string valueName = (conditionExpression.ExpName ?? "").ToString();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            switch (valueName)
                            {
                                case "ProjectName":
                                    {
                                        projectList = projectList.Where(t => t.ProjectName.Contains(value)).ToList();
                                        break;
                                    }
                                case "ProjectNature":
                                    {
                                        projectList = projectList.Where(t => t.Nature == value).ToList();
                                        break;
                                    }
                                case "CompanyId":
                                    {
                                        long id = Convert.ToInt64(value);
                                        projectList = projectList.Where(t => t.CompanyId == id).ToList();
                                        break;
                                    }
                                case "StationName":
                                    {
                                        projectList = projectList.Where(t => t.StationName.Contains(value)).ToList();
                                        break;
                                    }
                                case "startTime":
                                    {
                                        DateTime startTime1 = Convert.ToDateTime(value);
                                        projectList = projectList.Where(t => t.ApplyTime >= startTime1).ToList();
                                        break;
                                    }
                                case "endTime":
                                    {
                                        DateTime endTime1 = Convert.ToDateTime(value);
                                        projectList = projectList.Where(t => t.ApplyTime <= endTime1).ToList();
                                        break;
                                    }
                                case "State":
                                    {
                                        int state = Convert.ToInt32(value);
                                        projectList = projectList.Where(t => t.State == state).ToList();
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }
                result.AllRowsCount = projectList.Count();
                projectList = projectList.OrderByDescending(t => t.OperateTime).Skip((qc.PageInfo.CurrentPageIndex - 1) * qc.PageInfo.PageRowCount).Take(qc.PageInfo.PageRowCount).ToList();
                result.Data = projectList;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzProjectApprovalInfoList");
            }
            return result;
        }

        /// <summary>
        /// 财务决算查询
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_TzProjectApprovalInfo>> GetTzProjectApprovalListBy(QueryCondition qc)
        {
            Result<List<Epm_TzProjectApprovalInfo>> result = new Result<List<Epm_TzProjectApprovalInfo>>();
            try
            {
                var query = from info in context.Epm_TzProjectApprovalInfo.Where(p => p.IsDelete == false && p.State == (int)PreProjectState.ApprovalSuccess)
                            join project in context.Epm_TzProjectProposal.Where(p => p.IsDelete == false) on info.ProjectId equals project.Id
                            select new
                            {
                                info.ProjectId,
                                project.ProjectName,
                                project.Nature,
                                project.NatureName,
                                info.State,
                                info.FinanceAccounts,
                                info.FinanceUserName,
                                info.FinanceUserId,
                                info.FinanceTime,
                                info.Id,
                                info.TotalInvestment,
                            };

                var projectList = query.ToList().Select(c => new Epm_TzProjectApprovalInfo
                {
                    Id = c.Id,
                    ProjectName = c.ProjectName,
                    Nature = c.Nature,
                    NatureName = c.NatureName,
                    State = c.State,
                    FinanceAccounts = c.FinanceAccounts,
                    FinanceUserName = c.FinanceUserName,
                    FinanceUserId = c.FinanceUserId,
                    FinanceTime = c.FinanceTime,
                    ProjectId = c.ProjectId,
                    TotalInvestment = c.TotalInvestment,
                }).ToList();

                if (qc != null && qc.ConditionList.Any())
                {
                    foreach (var conditionExpression in qc.ConditionList)
                    {
                        string value = (conditionExpression.ExpValue ?? "").ToString();
                        string valueName = (conditionExpression.ExpName ?? "").ToString();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            switch (valueName)
                            {
                                case "name":
                                    {
                                        projectList = projectList.Where(t => t.ProjectName.Contains(value)).ToList();
                                        break;
                                    }
                                case "startTime":
                                    {
                                        DateTime startTime1 = Convert.ToDateTime(value);
                                        projectList = projectList.Where(t => t.FinanceTime >= startTime1).ToList();
                                        break;
                                    }
                                case "endTime":
                                    {
                                        DateTime endTime1 = Convert.ToDateTime(value);
                                        projectList = projectList.Where(t => t.FinanceTime <= endTime1).ToList();
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }
                result.AllRowsCount = projectList.Count();
                projectList = projectList.OrderByDescending(t => t.FinanceTime).Skip((qc.PageInfo.CurrentPageIndex - 1) * qc.PageInfo.PageRowCount).Take(qc.PageInfo.PageRowCount).ToList();
                result.Data = projectList;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzProjectApprovalListBy");
            }
            return result;
        }

        /// <summary>
        /// 财务决算详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_TzProjectApprovalInfo> GetTzProjectApprovalFinanceAccounts(long id)
        {
            Result<Epm_TzProjectApprovalInfo> result = new Result<Epm_TzProjectApprovalInfo>();
            try
            {
                var tzProjectApprovalInfo = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().GetModel(id);
                if (tzProjectApprovalInfo != null)
                {
                    var projectModel = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(tzProjectApprovalInfo.ProjectId);
                    if (projectModel != null)
                    {
                        tzProjectApprovalInfo.ProjectName = projectModel.ProjectName;
                        tzProjectApprovalInfo.Nature = projectModel.Nature;
                        tzProjectApprovalInfo.NatureName = projectModel.NatureName;

                        result.Data = tzProjectApprovalInfo;
                        result.Flag = EResultFlag.Success;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzProjectApprovalFinanceAccounts");
            }
            return result;
        }

        /// <summary>
        /// 编辑财务决算信息
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="FinanceAccounts"></param>
        /// <returns></returns>
        public Result<int> UpdateFinanceAccounts(long id, decimal financeAccounts)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().GetModel(id);

                if (model != null)
                {
                    SetCurrentUser(model);
                    model.FinanceAccounts = financeAccounts;
                    model.FinanceUserName = CurrentUser.UserName;
                    model.FinanceUserId = CurrentUser.Id;
                    model.FinanceTime = DateTime.Now;
                    var rows = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().Update(model);

                    result.Data = rows;
                    result.Flag = EResultFlag.Success;
                }
                else
                {
                    throw new Exception("该信息不存在");
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzProjectApprovalFinanceAccounts");
            }
            return result;
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<TzProjectApprovalInfoView> GetTzProjectApprovalInfoModel(long projectId)
        {
            Result<TzProjectApprovalInfoView> result = new Result<TzProjectApprovalInfoView>();
            try
            {
                TzProjectApprovalInfoView view = new TzProjectApprovalInfoView();
                var projectModel = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(projectId);
                if (projectModel != null)
                {
                    view.TzProjectProposal = projectModel;
                    view.StationIds = projectModel.StationId.ToString();
                    view.CompanyIds = projectModel.CompanyId.ToString();
                }

                var tzProjectApprovalInfo = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();

                if (tzProjectApprovalInfo != null)
                {
                    List<Epm_TzAttachs> tzAttachsList = new List<Epm_TzAttachs>();
                    tzAttachsList = GetFilesByTZTable("Epm_TzProjectApprovalInfo", tzProjectApprovalInfo.Id).Data;
                    if (tzAttachsList != null && tzAttachsList.Any())
                    {
                        tzProjectApprovalInfo.TzAttachs = tzAttachsList;
                    }
                    view.TzProjectApprovalInfo = tzProjectApprovalInfo;
                }

                var tzResearchOfEngineering = DataOperateBusiness<Epm_TzResearchOfEngineering>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();

                if (tzResearchOfEngineering != null)
                {
                    //加油机（台）
                    if (tzProjectApprovalInfo.MachineOfOilStage == null)
                    {
                        tzProjectApprovalInfo.MachineOfOilStage = tzResearchOfEngineering.MachineOfOilStage;
                    }
                    //加油机（枪）
                    if (tzProjectApprovalInfo.MachineOfOil == null)
                    {
                        tzProjectApprovalInfo.MachineOfOil = tzResearchOfEngineering.MachineOfOil;
                    }
                    //加气机（枪）
                    if (tzProjectApprovalInfo.MachineOfGasStage == null)
                    {
                        tzProjectApprovalInfo.MachineOfGasStage = tzResearchOfEngineering.MachineOfGasStage;
                    }
                    //加气机（台）
                    if (tzProjectApprovalInfo.MachineOfGas == null)
                    {
                        tzProjectApprovalInfo.MachineOfGas = tzResearchOfEngineering.MachineOfGas;
                    }
                    //罩棚
                    if (tzProjectApprovalInfo.Shelter == null)
                    {
                        tzProjectApprovalInfo.Shelter = tzResearchOfEngineering.Shelter;
                    }
                    //油罐
                    if (tzProjectApprovalInfo.OilTank == null)
                    {
                        tzProjectApprovalInfo.OilTank = tzResearchOfEngineering.OilTank;
                    }
                    //站房
                    if (tzProjectApprovalInfo.StationRoom == null)
                    {
                        tzProjectApprovalInfo.StationRoom = tzResearchOfEngineering.StationRoon;
                    }
                    //储气井（气罐）
                    if (tzProjectApprovalInfo.GasWells == null)
                    {
                        tzProjectApprovalInfo.GasWells = tzResearchOfEngineering.GasWells;
                    }
                }

                var tzResearchOfInvestment = DataOperateBusiness<Epm_TzResearchOfInvestment>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();

                if (tzResearchOfInvestment != null)
                {
                    //土地用途名称
                    if (string.IsNullOrEmpty(tzProjectApprovalInfo.LandUse))
                    {
                        tzProjectApprovalInfo.LandUse = tzResearchOfInvestment.LandUseName;
                    }
                    //土地用途编码
                    if (string.IsNullOrEmpty(tzProjectApprovalInfo.LandUseCode))
                    {
                        tzProjectApprovalInfo.LandUseCode = tzResearchOfInvestment.LandUseCode;
                    }
                    //土地性质编码
                    if (string.IsNullOrEmpty(tzProjectApprovalInfo.LandStatusCode))
                    {
                        tzProjectApprovalInfo.LandStatusCode = tzResearchOfInvestment.LandNatureCode;
                    }
                    //土地性质名称
                    if (string.IsNullOrEmpty(tzProjectApprovalInfo.LandStatus))
                    {
                        tzProjectApprovalInfo.LandStatus = tzResearchOfInvestment.LandNatureName;
                    }
                    //土地面积
                    if (tzProjectApprovalInfo.AreaOfLand == null)
                    {
                        tzProjectApprovalInfo.AreaOfLand = tzResearchOfInvestment.LandArea;
                    }
                }

                result.Data = view;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzProjectApprovalInfoModel");
            }
            return result;
        }

        /// <summary>
        /// 修改项目批复信息状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzProjectApprovalInfoState(List<long> ids, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                foreach (var item in ids)
                {
                    var model = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().GetList(t => t.ProjectId == item).FirstOrDefault();
                    if (model != null)
                    {
                        //SetCurrentUser(model);
                        model.State = (int)state.ToEnumReq<PreProjectApprovalState>();
                        var rows = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().Update(model);

                        //若状态为已审批，生成下一阶段数据
                        if (model.State == (int)PreProjectState.ApprovalSuccess)
                        {
                            var project = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId);

                            //特许经营项目性质的项目审批到项目批复完成就结束了，不生成工程模块的内容，不用写到机器人表里
                            if (project.Nature != "XMXZTXJY")
                            {
                                var ProjectReveiews = DataOperateBusiness<Epm_TzProjectReveiews>.Get().Single(t => t.ProjectId == model.ProjectId);
                                #region //生成设计方案信息

                                Epm_TzDesignScheme tzDesignScheme = new Epm_TzDesignScheme();
                                tzDesignScheme.ProjectId = model.ProjectId;
                                tzDesignScheme.ProjectCode = model.ProjectTypeCode;
                                tzDesignScheme.ProjectName = project.ProjectName;
                                tzDesignScheme.ApprovalNo = project.ApprovalNo;
                                tzDesignScheme.Nature = project.Nature;
                                tzDesignScheme.NatureName = project.NatureName;
                                tzDesignScheme.ApplyTime = project.ApplyTime;
                                tzDesignScheme.StationId = project.StationId;
                                tzDesignScheme.StationCodeXt = project.StationCodeXt;
                                tzDesignScheme.StationName = project.StationName;
                                tzDesignScheme.CompanyId = project.CompanyId;
                                tzDesignScheme.CompanyCodeXt = project.CompanyCodeXt;
                                tzDesignScheme.CompanyName = project.CompanyName;
                                tzDesignScheme.PredictMoney = project.PredictMoney;

                                tzDesignScheme.EngineeringCost = model.EngineeringCost;
                                tzDesignScheme.OtherExpenses = model.OtherExpenses;
                                tzDesignScheme.LandCosts = model.LandCosts;
                                tzDesignScheme.RegionCompany = model.RegionCompany;
                                tzDesignScheme.ProjectType = model.ProjectType;

                                tzDesignScheme.LandArea = model.AreaOfLand;//占地面积
                                tzDesignScheme.MachineofOilStage = model.MachineOfOilStage;//加油机
                                tzDesignScheme.MachineofGasStage = model.MachineOfGasStage;//加气机
                                tzDesignScheme.GasWells = model.GasWells;//储气井
                                tzDesignScheme.OilTank = model.OilTank;//油罐
                                tzDesignScheme.Shelter = model.Shelter;//罩棚面积
                                tzDesignScheme.StationRoom = model.StationRoom;//站房面积
                                                                               // tzDesignScheme.ConvenienceRoom = model.ConvenienceRoom;//便利店面积
                                tzDesignScheme.ReleaseInvestmentAmount = model.TotalInvestment;//批复概算投资
                                tzDesignScheme.ApprovalNo = model.ApprovalNo;//批复文号

                                tzDesignScheme.State = (int)PreProjectState.WaitSubmitted;
                                AddTzDesignScheme(tzDesignScheme);
                                #endregion

                                string houstAddress = System.Configuration.ConfigurationManager.AppSettings.Get("RPAPath");
                                #region 生成RPA表数据
                                // var user = DataOperateBasic<Base_User>.Get().Single(t => t.Id == model.OperateUserId);
                                RPA_TzProjectApprovalHX RPAModel = new RPA_TzProjectApprovalHX();

                                RPAModel.ApplyName = model.CreateUserName;//申请人
                                RPAModel.ApplyTime = model.CreateTime;//提出时间？
                                RPAModel.LimitTypeType = model.LimitName == "XMXS1" ? "地区公司限上项目" : model.LimitName == "XMXS2" ? "地区公司限下项目" : "";
                                RPAModel.Drafter = model.DraftAuthorName;
                                RPAModel.SignPeople = model.SignPeopleName;
                                RPAModel.Signer = model.SignerName;
                                RPAModel.Titanic = model.ChangeReplyNo;
                                RPAModel.Remark = model.AreaOfLand.ToString();//土地费用备注
                                RPAModel.username = "sxtzyw";
                                RPAModel.companys = CurrentUser.CompanyName;
                                RPAModel.WriteMark = 0;
                                RPAModel.WriteResult = 0;
                                RPAModel.FollowOperate = "";
                                RPAModel.ProjectName = model.ProjectName;

                                var fileList = GetFilesByTZTable("Epm_TzProjectApprovalInfo", model.Id).Data;
                                for (int i = 0; i < fileList.Count; i++)
                                {
                                    RPAModel.FilePath += houstAddress + "?path=" + fileList[i].FilePath + ",";
                                }
                                RPAModel.FileNumber = fileList.Count;

                                SetCreateUser(RPAModel);
                                SetCurrentUser(RPAModel);
                                DataOperateBusiness<RPA_TzProjectApprovalHX>.Get().Add(RPAModel);
                                #endregion
                                #region 生成OMADS表数据
                                OMADS_TzProjectApprovalHX oMADSModel = new OMADS_TzProjectApprovalHX();

                                oMADSModel.ApplyName = model.CreateUserName;//申请人
                                oMADSModel.ApplyTime = model.CreateTime;//提出时间？
                                                                        // oMADSModel.LimitTypeType = model.LimitName;//是否限上/限下
                                oMADSModel.LimitTypeType = model.LimitName == "XMXS1" ? "地区公司限上项目" : model.LimitName == "XMXS2" ? "地区公司限下项目" : "";
                                oMADSModel.Drafter = model.DraftAuthorName;
                                oMADSModel.SignPeople = model.SignPeopleName;
                                oMADSModel.Signer = model.SignerName;
                                oMADSModel.Titanic = model.ChangeReplyNo;
                                oMADSModel.Remark = model.AreaOfLand.ToString();//土地费用备注
                                oMADSModel.username = "sxtzyw";
                                oMADSModel.companys = CurrentUser.CompanyName;
                                oMADSModel.WriteMark = 0;
                                oMADSModel.WriteResult = 0;
                                oMADSModel.FollowOperate = "";
                                oMADSModel.ProjectName = model.ProjectName;

                                var ofileList = GetFilesByTZTable("Epm_TzProjectApprovalInfo", model.Id).Data;
                                for (int i = 0; i < ofileList.Count; i++)
                                {
                                    oMADSModel.FilePath += houstAddress + "?path=" + ofileList[i].FilePath + ",";
                                }
                                oMADSModel.FileNumber = ofileList.Count;

                                SetCreateUser(oMADSModel);
                                SetCurrentUser(oMADSModel);
                                DataOperateBusiness<OMADS_TzProjectApprovalHX>.Get().Add(oMADSModel);
                                #endregion
                                #region 生成TEMP表数据
                                TEMP_TzProjectApprovalHX tEMPModel = new TEMP_TzProjectApprovalHX();

                                tEMPModel.ApplyName = model.CreateUserName;//申请人
                                tEMPModel.ApplyTime = model.CreateTime;//提出时间？
                                                                       //tEMPModel.LimitTypeType = model.LimitName;//是否限上/限下
                                tEMPModel.LimitTypeType = model.LimitName == "XMXS1" ? "地区公司限上项目" : model.LimitName == "XMXS2" ? "地区公司限下项目" : "";
                                tEMPModel.Drafter = model.DraftAuthorName;
                                tEMPModel.SignPeople = model.SignPeopleName;
                                tEMPModel.Signer = model.SignerName;
                                tEMPModel.Titanic = model.ChangeReplyNo;
                                tEMPModel.Remark = model.AreaOfLand.ToString();//土地费用备注
                                tEMPModel.username = "sxtzyw";
                                tEMPModel.companys = CurrentUser.CompanyName;
                                tEMPModel.WriteMark = 0;
                                tEMPModel.WriteResult = 0;
                                tEMPModel.FollowOperate = "";
                                tEMPModel.ProjectName = model.ProjectName;

                                var tfileList = GetFilesByTZTable("Epm_TzProjectApprovalInfo", model.Id).Data;
                                for (int i = 0; i < tfileList.Count; i++)
                                {
                                    tEMPModel.FilePath += houstAddress + "?path=" + tfileList[i].FilePath + ",";
                                }
                                tEMPModel.FileNumber = tfileList.Count;

                                SetCreateUser(tEMPModel);
                                SetCurrentUser(tEMPModel);
                                DataOperateBusiness<TEMP_TzProjectApprovalHX>.Get().Add(tEMPModel);
                                #endregion

                                #region 可研批复信息插入到生成RPA表
                                RPA_TzProjectApprovalInfo rPAApprovalInfo = new RPA_TzProjectApprovalInfo();

                                rPAApprovalInfo.ProjectName = project.ProjectName;
                                rPAApprovalInfo.TotalInvestment = model.TotalInvestment;
                                rPAApprovalInfo.ContractPayment = model.ContractPayment;
                                rPAApprovalInfo.EngineeringCost = model.EngineeringCost;
                                rPAApprovalInfo.LandCosts = model.LandCosts;
                                rPAApprovalInfo.OtherExpenses = model.OtherExpenses;
                                rPAApprovalInfo.MachineOfOilStage = model.MachineOfOilStage;
                                rPAApprovalInfo.MachineOfOil = model.MachineOfOil;
                                rPAApprovalInfo.MachineOfGas = model.MachineOfGas;
                                rPAApprovalInfo.MachineOfGasStage = model.MachineOfGasStage;
                                rPAApprovalInfo.Shelter = model.Shelter;
                                rPAApprovalInfo.OilTank = model.OilTank;
                                rPAApprovalInfo.StationRoom = model.StationRoom;
                                rPAApprovalInfo.TankNumber = model.TankNumber;
                                rPAApprovalInfo.GasWells = model.GasWells;
                                rPAApprovalInfo.LandPayment = model.LandPayment;
                                rPAApprovalInfo.AssetTypeName = model.AssetTypeName;
                                rPAApprovalInfo.RemarkOnLandCost = model.RemarkOnLandCost;
                                rPAApprovalInfo.LandStatus = model.LandStatus;
                                rPAApprovalInfo.LandUse = model.LandUse;
                                rPAApprovalInfo.AreaOfLand = model.AreaOfLand;
                                rPAApprovalInfo.ExpectedPaymentThisYear = model.ExpectedPaymentThisYear;
                                rPAApprovalInfo.EstimatedTimeOfSales = model.EstimatedTimeOfSales;
                                // rPAApprovalInfo.LimitName = model.LimitName;
                                rPAApprovalInfo.LimitName = model.LimitName == "XMXS1" ? "地区公司限上项目" : model.LimitName == "XMXS2" ? "地区公司限下项目" : "";
                                rPAApprovalInfo.StandardName = model.StandardName;
                                rPAApprovalInfo.UnitFeasibilityCompilation = model.UnitFeasibilityCompilation;
                                rPAApprovalInfo.ApprovalNo = model.ApprovalNo;
                                rPAApprovalInfo.FormOfEquityInvestment = model.FormOfEquityInvestment;
                                rPAApprovalInfo.ImplementationCcompany = model.ImplementationCcompany;
                                rPAApprovalInfo.RegisteredCapital = model.RegisteredCapital;
                                rPAApprovalInfo.HoldingTheProportion = model.HoldingTheProportion;
                                rPAApprovalInfo.DailyDieselSales = model.DailyDieselSales;
                                rPAApprovalInfo.DailyGasolineSales = model.DailyGasolineSales;
                                rPAApprovalInfo.ChaiQibi = model.ChaiQibi;
                                rPAApprovalInfo.CNGYAndLNGQ = model.CNGY + model.LNGQ;
                                rPAApprovalInfo.PayBackPeriod = model.PayBackPeriod;
                                rPAApprovalInfo.InternalRateReturn = model.InternalRateReturn;
                                rPAApprovalInfo.AnnualNonOilIncome = model.AnnualNonOilIncome;
                                rPAApprovalInfo.NonOilAnnualCost = model.NonOilAnnualCost;
                                rPAApprovalInfo.ScheduledComTime = string.Format("{0:yyyy-MM}", model.ScheduledComTime);
                                rPAApprovalInfo.FeasibleApprovalDate = model.FeasibleApprovalDate;
                                //rPAApprovalInfo.HasEInformation = model.HasEInformation== "GCXXBH1"?"True" : model.HasEInformation == "GCXXBH2" ? "False":"";
                                rPAApprovalInfo.HasEInformation = model.HasEInformation == "GCXXBH1" ? true : model.HasEInformation == "GCXXBH2" ? false : false;
                                rPAApprovalInfo.username = "sxtzyw";
                                rPAApprovalInfo.companys = CurrentUser.CompanyName;

                                rPAApprovalInfo.WriteMark = 0;
                                rPAApprovalInfo.WriteResult = 0;
                                rPAApprovalInfo.FollowOperate = "";
                                var rifileList = GetFilesByTZTable("Epm_TzProjectApprovalInfo", model.Id).Data;
                                for (int i = 0; i < rifileList.Count; i++)
                                {
                                    rPAApprovalInfo.FilePath += houstAddress + "?path=" + rifileList[i].FilePath + "&fileName=" + rifileList[i].Name + ",";
                                }
                                rPAApprovalInfo.FileNumber = rifileList.Count;
                                rPAApprovalInfo.status1 = 0;
                                SetCreateUser(rPAApprovalInfo);
                                SetCurrentUser(rPAApprovalInfo);
                                DataOperateBusiness<RPA_TzProjectApprovalInfo>.Get().Add(rPAApprovalInfo);
                                #endregion
                                #region 可研批复信息插入到生成OMADS表
                                OMADS_TzProjectApprovalInfo oMADSApprovalInfo = new OMADS_TzProjectApprovalInfo();
                                oMADSApprovalInfo.ProjectName = project.ProjectName;
                                oMADSApprovalInfo.TotalInvestment = model.TotalInvestment;
                                oMADSApprovalInfo.ContractPayment = model.ContractPayment;
                                oMADSApprovalInfo.EngineeringCost = model.EngineeringCost;
                                oMADSApprovalInfo.LandCosts = model.LandCosts;
                                oMADSApprovalInfo.OtherExpenses = model.OtherExpenses;
                                oMADSApprovalInfo.MachineOfOilStage = model.MachineOfOilStage;
                                oMADSApprovalInfo.MachineOfOil = model.MachineOfOil;
                                oMADSApprovalInfo.MachineOfGas = model.MachineOfGas;
                                oMADSApprovalInfo.MachineOfGasStage = model.MachineOfGasStage;
                                oMADSApprovalInfo.Shelter = model.Shelter;
                                oMADSApprovalInfo.OilTank = model.OilTank;
                                oMADSApprovalInfo.StationRoom = model.StationRoom;
                                oMADSApprovalInfo.TankNumber = model.TankNumber;
                                oMADSApprovalInfo.GasWells = model.GasWells;
                                oMADSApprovalInfo.LandPayment = model.LandPayment;
                                oMADSApprovalInfo.AssetTypeName = model.AssetTypeName;
                                oMADSApprovalInfo.RemarkOnLandCost = model.RemarkOnLandCost;
                                oMADSApprovalInfo.LandStatus = model.LandStatus;
                                oMADSApprovalInfo.LandUse = model.LandUse;
                                oMADSApprovalInfo.AreaOfLand = model.AreaOfLand;
                                oMADSApprovalInfo.ExpectedPaymentThisYear = model.ExpectedPaymentThisYear;
                                oMADSApprovalInfo.EstimatedTimeOfSales = model.EstimatedTimeOfSales;
                                // oMADSApprovalInfo.LimitName = model.LimitName;
                                oMADSApprovalInfo.LimitName = model.LimitName == "XMXS1" ? "地区公司限上项目" : model.LimitName == "XMXS2" ? "地区公司限下项目" : "";
                                oMADSApprovalInfo.StandardName = model.StandardName;
                                oMADSApprovalInfo.UnitFeasibilityCompilation = model.UnitFeasibilityCompilation;
                                oMADSApprovalInfo.ApprovalNo = model.ApprovalNo;
                                oMADSApprovalInfo.FormOfEquityInvestment = model.FormOfEquityInvestment;
                                oMADSApprovalInfo.ImplementationCcompany = model.ImplementationCcompany;
                                oMADSApprovalInfo.RegisteredCapital = model.RegisteredCapital;
                                oMADSApprovalInfo.HoldingTheProportion = model.HoldingTheProportion;
                                oMADSApprovalInfo.DailyDieselSales = model.DailyDieselSales;
                                oMADSApprovalInfo.DailyGasolineSales = model.DailyGasolineSales;
                                oMADSApprovalInfo.ChaiQibi = model.ChaiQibi;
                                oMADSApprovalInfo.CNGYAndLNGQ = model.CNGY + model.LNGQ;
                                oMADSApprovalInfo.PayBackPeriod = model.PayBackPeriod;
                                oMADSApprovalInfo.InternalRateReturn = model.InternalRateReturn;
                                oMADSApprovalInfo.AnnualNonOilIncome = model.AnnualNonOilIncome;
                                oMADSApprovalInfo.NonOilAnnualCost = model.NonOilAnnualCost;
                                oMADSApprovalInfo.ScheduledComTime = string.Format("{0:yyyy-MM}", model.ScheduledComTime);
                                oMADSApprovalInfo.FeasibleApprovalDate = model.FeasibleApprovalDate;
                                // oMADSApprovalInfo.HasEInformation = model.HasEInformation;
                                oMADSApprovalInfo.HasEInformation = model.HasEInformation == "GCXXBH1" ? true : model.HasEInformation == "GCXXBH2" ? false : false;
                                oMADSApprovalInfo.username = "sxtzyw";
                                oMADSApprovalInfo.companys = CurrentUser.CompanyName;

                                oMADSApprovalInfo.WriteMark = 0;
                                oMADSApprovalInfo.WriteResult = 0;
                                oMADSApprovalInfo.FollowOperate = "";
                                var oifileList = GetFilesByTZTable("Epm_TzProjectApprovalInfo", model.Id).Data;
                                for (int i = 0; i < oifileList.Count; i++)
                                {
                                    oMADSApprovalInfo.FilePath += houstAddress + "?path=" + oifileList[i].FilePath + "&fileName=" + oifileList[i].Name + ",";
                                }
                                oMADSApprovalInfo.FileNumber = oifileList.Count;
                                oMADSApprovalInfo.status1 = 0;
                                SetCreateUser(oMADSApprovalInfo);
                                SetCurrentUser(oMADSApprovalInfo);
                                DataOperateBusiness<OMADS_TzProjectApprovalInfo>.Get().Add(oMADSApprovalInfo);
                                #endregion
                                #region 可研批复信息插入到生成TEMP表
                                TEMP_TzProjectApprovalInfo tEMPApprovalInfo = new TEMP_TzProjectApprovalInfo();
                                tEMPApprovalInfo.ProjectName = project.ProjectName;
                                tEMPApprovalInfo.TotalInvestment = model.TotalInvestment;
                                tEMPApprovalInfo.ContractPayment = model.ContractPayment;
                                tEMPApprovalInfo.EngineeringCost = model.EngineeringCost;
                                tEMPApprovalInfo.LandCosts = model.LandCosts;
                                tEMPApprovalInfo.OtherExpenses = model.OtherExpenses;
                                tEMPApprovalInfo.MachineOfOilStage = model.MachineOfOilStage;
                                tEMPApprovalInfo.MachineOfOil = model.MachineOfOil;
                                tEMPApprovalInfo.MachineOfGas = model.MachineOfGas;
                                tEMPApprovalInfo.MachineOfGasStage = model.MachineOfGasStage;
                                tEMPApprovalInfo.Shelter = model.Shelter;
                                tEMPApprovalInfo.OilTank = model.OilTank;
                                tEMPApprovalInfo.StationRoom = model.StationRoom;
                                tEMPApprovalInfo.TankNumber = model.TankNumber;
                                tEMPApprovalInfo.GasWells = model.GasWells;
                                tEMPApprovalInfo.LandPayment = model.LandPayment;
                                tEMPApprovalInfo.AssetTypeName = model.AssetTypeName;
                                tEMPApprovalInfo.RemarkOnLandCost = model.RemarkOnLandCost;
                                tEMPApprovalInfo.LandStatus = model.LandStatus;
                                tEMPApprovalInfo.LandUse = model.LandUse;
                                tEMPApprovalInfo.AreaOfLand = model.AreaOfLand;
                                tEMPApprovalInfo.ExpectedPaymentThisYear = model.ExpectedPaymentThisYear;
                                tEMPApprovalInfo.EstimatedTimeOfSales = model.EstimatedTimeOfSales;
                                //tEMPApprovalInfo.LimitName = model.LimitName;
                                tEMPApprovalInfo.LimitName = model.LimitName == "XMXS1" ? "地区公司限上项目" : model.LimitName == "XMXS2" ? "地区公司限下项目" : "";
                                tEMPApprovalInfo.StandardName = model.StandardName;
                                tEMPApprovalInfo.UnitFeasibilityCompilation = model.UnitFeasibilityCompilation;
                                tEMPApprovalInfo.ApprovalNo = model.ApprovalNo;
                                tEMPApprovalInfo.FormOfEquityInvestment = model.FormOfEquityInvestment;
                                tEMPApprovalInfo.ImplementationCcompany = model.ImplementationCcompany;
                                tEMPApprovalInfo.RegisteredCapital = model.RegisteredCapital;
                                tEMPApprovalInfo.HoldingTheProportion = model.HoldingTheProportion;
                                tEMPApprovalInfo.DailyDieselSales = model.DailyDieselSales;
                                tEMPApprovalInfo.DailyGasolineSales = model.DailyGasolineSales;
                                tEMPApprovalInfo.ChaiQibi = model.ChaiQibi;
                                tEMPApprovalInfo.CNGYAndLNGQ = model.CNGY + model.LNGQ;
                                tEMPApprovalInfo.PayBackPeriod = model.PayBackPeriod;
                                tEMPApprovalInfo.InternalRateReturn = model.InternalRateReturn;
                                tEMPApprovalInfo.AnnualNonOilIncome = model.AnnualNonOilIncome;
                                tEMPApprovalInfo.NonOilAnnualCost = model.NonOilAnnualCost;
                                tEMPApprovalInfo.ScheduledComTime = string.Format("{0:yyyy-MM}", model.ScheduledComTime);
                                tEMPApprovalInfo.FeasibleApprovalDate = model.FeasibleApprovalDate;
                                //tEMPApprovalInfo.HasEInformation = model.HasEInformation;
                                tEMPApprovalInfo.HasEInformation = model.HasEInformation == "GCXXBH1" ? true : model.HasEInformation == "GCXXBH2" ? false : false;
                                tEMPApprovalInfo.username = "sxtzyw";
                                tEMPApprovalInfo.companys = CurrentUser.CompanyName;

                                tEMPApprovalInfo.WriteMark = 0;
                                tEMPApprovalInfo.WriteResult = 0;
                                tEMPApprovalInfo.FollowOperate = "";
                                var tifileList = GetFilesByTZTable("Epm_TzProjectApprovalInfo", model.Id).Data;
                                for (int i = 0; i < tifileList.Count; i++)
                                {
                                    tEMPApprovalInfo.FilePath += houstAddress + "?path=" + tifileList[i].FilePath + "&fileName=" + tifileList[i].Name + ",";
                                }
                                tEMPApprovalInfo.FileNumber = tifileList.Count;
                                tEMPApprovalInfo.status1 = 0;
                                SetCreateUser(tEMPApprovalInfo);
                                SetCurrentUser(tEMPApprovalInfo);
                                DataOperateBusiness<TEMP_TzProjectApprovalInfo>.Get().Add(tEMPApprovalInfo);
                                #endregion

                                #region 项目主信息
                                #region 生成RPA表数据
                                RPA_ProjectInfo rPAProjectInfo = new RPA_ProjectInfo();

                                rPAProjectInfo.ProjectName = project.ProjectName;
                                rPAProjectInfo.ProjectAddress = project.ProjectAddress;
                                rPAProjectInfo.ProjectDecisionerName = model.DecisionMakerName;
                                rPAProjectInfo.ProjectLeaderName = model.FieldManagerName;
                                rPAProjectInfo.ProjectNatureName = project.NatureName;
                                rPAProjectInfo.ProjectTypeName = project.ProjectType;
                                rPAProjectInfo.username = "sxtzyw";
                                rPAProjectInfo.companys = CurrentUser.CompanyName;
                                rPAProjectInfo.WriteMark = 0;
                                rPAProjectInfo.WriteResult = 0;
                                rPAProjectInfo.FollowOperate = "";

                                var pfileList = GetFilesByTZTable("Epm_Project", model.Id).Data;
                                for (int i = 0; i < pfileList.Count; i++)
                                {
                                    rPAProjectInfo.FilePath += houstAddress + "?path=" + pfileList[i].FilePath + ",";
                                }
                                rPAProjectInfo.FileNumber = pfileList.Count;

                                SetCreateUser(rPAProjectInfo);
                                SetCurrentUser(rPAProjectInfo);
                                DataOperateBusiness<RPA_ProjectInfo>.Get().Add(rPAProjectInfo);
                                #endregion
                                #region 生成OMADS表数据
                                OMADS_ProjectInfo oMADSProjectInfo = new OMADS_ProjectInfo();
                                oMADSProjectInfo.ProjectName = project.ProjectName;
                                oMADSProjectInfo.ProjectAddress = project.ProjectAddress;
                                oMADSProjectInfo.ProjectDecisionerName = model.DecisionMakerName;
                                oMADSProjectInfo.ProjectLeaderName = model.FieldManagerName;
                                oMADSProjectInfo.ProjectNatureName = project.NatureName;
                                oMADSProjectInfo.ProjectTypeName = project.ProjectType;
                                oMADSProjectInfo.username = "sxtzyw";
                                oMADSProjectInfo.companys = CurrentUser.CompanyName;
                                oMADSProjectInfo.WriteMark = 0;
                                oMADSProjectInfo.WriteResult = 0;
                                oMADSProjectInfo.FollowOperate = "";

                                var opfileList = GetFilesByTZTable("Epm_Project", model.Id).Data;
                                for (int i = 0; i < opfileList.Count; i++)
                                {
                                    oMADSProjectInfo.FilePath += houstAddress + "?path=" + opfileList[i].FilePath + ",";
                                }
                                oMADSProjectInfo.FileNumber = opfileList.Count;

                                SetCreateUser(oMADSProjectInfo);
                                SetCurrentUser(oMADSProjectInfo);
                                DataOperateBusiness<OMADS_ProjectInfo>.Get().Add(oMADSProjectInfo);
                                #endregion
                                #region 生成RPA表数据
                                TEMP_ProjectInfo tEMPProjectInfo = new TEMP_ProjectInfo();

                                tEMPProjectInfo.ProjectName = project.ProjectName;
                                tEMPProjectInfo.ProjectAddress = project.ProjectAddress;
                                tEMPProjectInfo.ProjectDecisionerName = model.DecisionMakerName;
                                tEMPProjectInfo.ProjectLeaderName = model.FieldManagerName;
                                tEMPProjectInfo.ProjectNatureName = project.NatureName;
                                tEMPProjectInfo.ProjectTypeName = project.ProjectType;
                                tEMPProjectInfo.username = "sxtzyw";
                                tEMPProjectInfo.companys = CurrentUser.CompanyName;
                                tEMPProjectInfo.WriteMark = 0;
                                tEMPProjectInfo.WriteResult = 0;
                                tEMPProjectInfo.FollowOperate = "";

                                var tpfileList = GetFilesByTZTable("Epm_Project", model.Id).Data;
                                for (int i = 0; i < tpfileList.Count; i++)
                                {
                                    tEMPProjectInfo.FilePath += houstAddress + "?path=" + tpfileList[i].FilePath + ",";
                                }
                                tEMPProjectInfo.FileNumber = tpfileList.Count;

                                SetCreateUser(tEMPProjectInfo);
                                SetCurrentUser(tEMPProjectInfo);
                                DataOperateBusiness<TEMP_ProjectInfo>.Get().Add(tEMPProjectInfo);
                                #endregion
                                #endregion

                                #region 生成项目评审记录RPA表数据
                                RPA_TzProjectReveiews rPAModelReveiews = new RPA_TzProjectReveiews();

                                rPAModelReveiews.Address = ProjectReveiews.Address;
                                rPAModelReveiews.Attendees = ProjectReveiews.Attendees;
                                rPAModelReveiews.ConclusionName = ProjectReveiews.ConclusionName;
                                rPAModelReveiews.HostUser = ProjectReveiews.HostUser;
                                rPAModelReveiews.InvitedExperts = ProjectReveiews.InvitedExperts;
                                rPAModelReveiews.ProjectName = project.ProjectName;
                                rPAModelReveiews.ReveiewDate = ProjectReveiews.ReveiewDate;
                                rPAModelReveiews.username = "sxxayw";
                                rPAModelReveiews.WriteMark = 0;
                                rPAModelReveiews.WriteResult = 0;
                                rPAModelReveiews.FollowOperate = "";
                                rPAModelReveiews.status1 = 0;
                                rPAModelReveiews.status2 = 0;
                                rPAModelReveiews.status3 = 0;
                                rPAModelReveiews.status4 = 0;
                                rPAModelReveiews.status5 = 0;
                                rPAModelReveiews.status6 = 0;
                                rPAModelReveiews.status7 = 0;
                                rPAModelReveiews.status8 = 0;

                                var reveiewsFileList = GetFilesByTZTable("Epm_TzProjectReveiews", ProjectReveiews.Id).Data;
                                for (int i = 0; i < reveiewsFileList.Count; i++)
                                {
                                    rPAModelReveiews.FilePath += houstAddress + "?path=" + reveiewsFileList[i].FilePath + ",";
                                }
                                rPAModelReveiews.FileNumber = reveiewsFileList.Count;

                                SetCreateUser(rPAModelReveiews);
                                SetCurrentUser(rPAModelReveiews);
                                DataOperateBusiness<RPA_TzProjectReveiews>.Get().Add(rPAModelReveiews);
                                #endregion

                                #region 生成项目评审记录OMADS表数据
                                OMADS_TzProjectReveiews oMADSModelReveiews = new OMADS_TzProjectReveiews();

                                oMADSModelReveiews.Address = ProjectReveiews.Address;
                                oMADSModelReveiews.Attendees = ProjectReveiews.Attendees;
                                oMADSModelReveiews.ConclusionName = ProjectReveiews.ConclusionName;
                                oMADSModelReveiews.HostUser = ProjectReveiews.HostUser;
                                oMADSModelReveiews.InvitedExperts = ProjectReveiews.InvitedExperts;
                                oMADSModelReveiews.ProjectName = project.ProjectName;
                                oMADSModelReveiews.ReveiewDate = ProjectReveiews.ReveiewDate;
                                oMADSModelReveiews.username = "sxxayw";
                                oMADSModelReveiews.WriteMark = 0;
                                oMADSModelReveiews.WriteResult = 0;
                                oMADSModelReveiews.FollowOperate = "";
                                oMADSModelReveiews.status1 = 0;
                                oMADSModelReveiews.status2 = 0;
                                oMADSModelReveiews.status3 = 0;
                                oMADSModelReveiews.status4 = 0;
                                oMADSModelReveiews.status5 = 0;
                                oMADSModelReveiews.status6 = 0;
                                oMADSModelReveiews.status7 = 0;
                                oMADSModelReveiews.status8 = 0;
                                oMADSModelReveiews.FilePath = rPAModelReveiews.FilePath;
                                rPAModelReveiews.FileNumber = reveiewsFileList.Count;

                                SetCreateUser(oMADSModelReveiews);
                                SetCurrentUser(oMADSModelReveiews);
                                DataOperateBusiness<OMADS_TzProjectReveiews>.Get().Add(oMADSModelReveiews);
                                #endregion

                            }
                        }

                        result.Data = rows;
                        result.Flag = EResultFlag.Success;
                    }
                    else
                    {
                        throw new Exception("该项目评审记录信息不存在");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzProjectApprovalInfoState");
            }
            return result;
        }
    }
}
