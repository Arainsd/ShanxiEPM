/************************************************************************************
 * Copyright (c) 2019  All Rights Reserved.
 * CLR版本： 4.0.30319.42000
 * 公司名称：陕西华春网络科技股份有限公司
 * 命名空间：hc.epm.Service.ClientSite
 * 文件名：  TzSiteSurveyService
 * 版本号：  V1.0.0.0
 * 创建人：  wmg	
 * 电子邮箱：wmgwugang@huachun.com
 * 创建时间：2019/8/22 16:38:22
 * 描述：
 * 
 * 
 * 
 ************************************************************************************/

using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzInitialTalk(Epm_TzInitialTalk model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzInitialTalk>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzInitialTalk.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzInitialTalk");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzInitialTalk(Epm_TzInitialTalk model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzInitialTalk>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzInitialTalk.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzInitialTalk");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzInitialTalkByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzInitialTalk>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzInitialTalk>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzInitialTalk.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzInitialTalkByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzInitialTalk>> GetTzInitialTalkList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzInitialTalk>> result = new Result<List<Epm_TzInitialTalk>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzInitialTalk>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzInitialTalkList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzInitialTalk> GetTzInitialTalkModel(long id)
        {
            Result<Epm_TzInitialTalk> result = new Result<Epm_TzInitialTalk>();
            try
            {
                var model = DataOperateBusiness<Epm_TzInitialTalk>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzInitialTalkModel");
            }
            return result;
        }
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzSiteSurvey(TzResearchView model)
        {
            Result<int> result = new Result<int>();
            //DbContextTransaction transaction = context.Database.BeginTransaction();
            try
            {
                int rows = 0;

                #region 现场工程方面调研
                bool isAddResearch = false;
                var tzResearchOfEngineering = DataOperateBusiness<Epm_TzResearchOfEngineering>.Get().GetList(t => t.ProjectId == model.ProjectId).FirstOrDefault();
                if (tzResearchOfEngineering == null)
                {
                    isAddResearch = true;
                    tzResearchOfEngineering = new Epm_TzResearchOfEngineering();
                    SetCreateUser(tzResearchOfEngineering);
                }
                tzResearchOfEngineering.ProjectId = model.ProjectId;
                tzResearchOfEngineering.State = model.State;
                tzResearchOfEngineering.ResearchStartDate = model.StartDate;
                tzResearchOfEngineering.ResearchEndDate = model.ResearchEndDate;
                tzResearchOfEngineering.ResearchUserId = model.ResearchUserId;
                tzResearchOfEngineering.ReaearchUserName = model.ReaearchUserName;
                tzResearchOfEngineering.ReaearchUserXt = model.ReaearchUserXt;
                tzResearchOfEngineering.JobCode = model.JobCode;
                tzResearchOfEngineering.JobName = model.JobName;
                tzResearchOfEngineering.IndustryPlanning = model.IndustryPlanning;
                tzResearchOfEngineering.Shelter = model.Shelter;
                tzResearchOfEngineering.MachineOfOilStage = model.MachineOfOilStage;
                tzResearchOfEngineering.MachineOfOil = model.MachineOfOil;
                tzResearchOfEngineering.MachineOfGasStage = model.MachineOfGasStage;
                tzResearchOfEngineering.MachineOfGas = model.MachineOfGas;
                tzResearchOfEngineering.OilTank = model.OilTank;
                tzResearchOfEngineering.StationRoon = model.StationRoon;
                tzResearchOfEngineering.GasWells = model.GasWells;
                tzResearchOfEngineering.HasInformationSystem = model.HasInformationSystem;
                tzResearchOfEngineering.ReformCode = model.ReformCode;
                tzResearchOfEngineering.ReformName = model.ReformName;
                XTSetCurrentUser(tzResearchOfEngineering);

                if (isAddResearch)
                {
                    rows = DataOperateBusiness<Epm_TzResearchOfEngineering>.Get().Add(tzResearchOfEngineering);
                }
                else
                {
                    rows = DataOperateBusiness<Epm_TzResearchOfEngineering>.Get().Update(tzResearchOfEngineering);
                }
                #endregion

                #region 信息方面调研
                bool isAddInfo = false;
                var tzResearchOfInformation = DataOperateBusiness<Epm_TzResearchOfInformation>.Get().GetList(t => t.ProjectId == model.ProjectId).FirstOrDefault();
                if (tzResearchOfInformation == null)
                {
                    isAddInfo = true;
                    tzResearchOfInformation = new Epm_TzResearchOfInformation();
                    SetCreateUser(tzResearchOfInformation);
                }
                tzResearchOfInformation.ProjectId = model.ProjectId;
                tzResearchOfInformation.State = model.State;
                tzResearchOfInformation.ResearchStartDate = model.StartDate;
                tzResearchOfInformation.ResearchEndDate = model.InfoResearchEndDate;
                tzResearchOfInformation.ResearchUserId = model.InfoResearchUserId;
                tzResearchOfInformation.ReaearchUserName = model.InfoReaearchUserName;
                tzResearchOfInformation.ReaearchUserXt = model.InfoReaearchUserXt;
                tzResearchOfInformation.JobCode = model.InfoJobCode;
                tzResearchOfInformation.JobName = model.InfoJobName;
                tzResearchOfInformation.Improvement = model.Improvement;
                XTSetCurrentUser(tzResearchOfInformation);

                //上传附件
                AddFilesBytzTable(tzResearchOfInformation, model.TzAttachs);

                if (isAddInfo)
                {
                    rows = DataOperateBusiness<Epm_TzResearchOfInformation>.Get().Add(tzResearchOfInformation);
                }
                else
                {
                    rows = DataOperateBusiness<Epm_TzResearchOfInformation>.Get().Update(tzResearchOfInformation);
                }
                #endregion

                #region  现场踏勘调用协同接口
                //var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                //if (model.State == (int)PreProjectState.Submitted && XtWorkFlow == "1")
                //{
                //    var project = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId);
                //    XtTzResearchView view = new XtTzResearchView();
                //    var baseUser = DataOperateBasic<Base_User>.Get().GetModel(CurrentUser.Id);
                //    if (baseUser == null)
                //    {
                //        throw new Exception("未找到申请人相关信息！");
                //    }
                //    else
                //    {
                //        view.hr_sqr = baseUser.ObjeId;
                //    }
                //    view.ProjectName = project.ProjectName;
                //    view.NatureName = project.NatureName;
                //    view.ApplyTime = string.Format("{0:yyyy-MM-dd}", project.ApplyTime);
                //    view.StationName = project.StationName;
                //    view.Position = project.Position;
                //    view.PredictMoney = project.PredictMoney.ToString();
                //    view.CompanyName = project.CompanyName;
                //    view.ResearchStartDate = string.Format("{0:yyyy-MM-dd}", model.StartDate);
                //    view.Address = model.Address;
                //    view.EnvironmentTypeName = model.EnvironmentTypeName;
                //    view.LandNatureName = model.LandNatureName;
                //    view.LandUseName = model.LandUseName;
                //    view.LandArea = model.LandArea.ToString();
                //    view.LandShape = model.LandShape.ToString();
                //    view.IsMeetAreaPlan = model.IsMeetAreaPlan;
                //    view.AroundCarCount = model.AroundCarCount.ToString();
                //    view.DailyTraffic = model.DailyTraffic.ToString();
                //    view.OilSaleTotal = model.OilSaleTotal.ToString();
                //    view.DieselGasolineRatio = model.DieselGasolineRatio;
                //    view.LandPrice = model.LandPrice.ToString();
                //    view.GasSaleTotal = model.GasSaleTotal.ToString();
                //    view.PropertyRights = model.PropertyRights;
                //    view.AssetSubject = model.AssetSubject;
                //    view.DisputesJudgment = model.DisputesJudgment;
                //    view.License = model.License;
                //    view.IndustryPlanning = model.IndustryPlanning;
                //    view.Shelter = model.Shelter.ToString();
                //    view.OilTank = model.OilTank.ToString();
                //    view.MachineOfOilStage = model.MachineOfOilStage.ToString();
                //    view.MachineOfOil = model.MachineOfOil.ToString();
                //    view.MachineOfGasStage = model.MachineOfGasStage.ToString();
                //    view.MachineOfGas = model.MachineOfGas.ToString();
                //    view.StationRoon = model.StationRoon.ToString();
                //    view.GasWells = model.GasWells.ToString();
                //    view.HasInformationSystem = model.HasInformationSystem;
                //    view.CurrentSalesVolume = model.CurrentSalesVolume.ToString();
                //    view.ReformName = model.ReformName;
                //    view.CargoDistance = model.CargoDistance.ToString();
                //    view.GasDailySales = model.GasDailySales.ToString();
                //    view.SalesMeans = model.SalesMeans;
                //    view.SalesRealizability = model.SalesRealizability;
                //    view.SourceOfOil = model.SourceOfOil;
                //    view.Environmental = model.Environmental;
                //    view.hiddenDanger = model.hiddenDanger;
                //    view.ImprovementMeasures = model.ImprovementMeasures;
                //    view.Improvement = model.Improvement;
                //    view.tzResearchUserName = model.InvestResearchUserName;
                //    view.tzJobName = model.InvestJobName;
                //    view.flResearchUserName = model.LawResearchUserName;
                //    view.flJobName = model.LawJobName;
                //    view.flResearchUserName = model.ReaearchUserName;
                //    view.flJobName = model.JobName;
                //    view.flResearchUserName = model.ManageReaearchUserName;
                //    view.flJobName = model.ManageJobName;
                //    view.flResearchUserName = model.SafeReaearchUserName;
                //    view.flJobName = model.SafeJobName;
                //    view.flResearchUserName = model.InfoReaearchUserName;
                //    view.flJobName = model.InfoJobName;

                //    //上传附件
                //    if (model.TzAttachs != null && model.TzAttachs.Any())
                //    {
                //        string baseFaleUrl = System.Configuration.ConfigurationManager.AppSettings.Get("XtDownloadUrl");
                //        foreach (var item in model.TzAttachs)
                //        {
                //            string fileUrl = string.Format("{0}?fileId={1}&type={2}", baseFaleUrl, item.Id, item.TypeNo);
                //            view.Temp_TzAttachs = fileUrl + '|' + view.Temp_TzAttachs;
                //        }
                //        if (view.Temp_TzAttachs != null)
                //        {
                //            view.Temp_TzAttachs = view.Temp_TzAttachs.Substring(0, view.Temp_TzAttachs.Length - 1);
                //        }
                //    }

                //    tzResearchOfInformation.WorkFlowId = XtWorkFlowService.CreateTzResearchWorkFlow(view);
                //}
                #endregion

                #region 现场投资调研
                bool isAddInvest = false;
                var tzResearchOfInvestment = DataOperateBusiness<Epm_TzResearchOfInvestment>.Get().GetList(t => t.ProjectId == model.ProjectId).FirstOrDefault();
                if (tzResearchOfInvestment == null)
                {
                    isAddInvest = true;
                    tzResearchOfInvestment = new Epm_TzResearchOfInvestment();
                    SetCreateUser(tzResearchOfInvestment);
                }
                tzResearchOfInvestment.ProjectId = model.ProjectId;
                tzResearchOfInvestment.State = model.State;
                tzResearchOfInvestment.ResearchStartDate = model.StartDate;
                tzResearchOfInvestment.ResearchEndDate = model.InvestResearchEndDate;
                tzResearchOfInvestment.ResearchUserId = model.InvestResearchUserId;
                tzResearchOfInvestment.ResearchUserName = model.InvestResearchUserName;
                tzResearchOfInvestment.ResearchUserXt = model.InvestResearchUserXt;
                tzResearchOfInvestment.JobCode = model.InvestJobCode;
                tzResearchOfInvestment.JobName = model.InvestJobName;
                tzResearchOfInvestment.Address = model.Address;
                tzResearchOfInvestment.EnvironmentTypeCode = model.EnvironmentTypeCode;
                tzResearchOfInvestment.EnvironmentTypeName = model.EnvironmentTypeName;
                tzResearchOfInvestment.LandNatureCode = model.LandNatureCode;
                tzResearchOfInvestment.LandNatureName = model.LandNatureName;
                tzResearchOfInvestment.LandUseCode = model.LandUseCode;
                tzResearchOfInvestment.LandUseName = model.LandUseName;
                tzResearchOfInvestment.LandArea = model.LandArea;
                tzResearchOfInvestment.LandShape = model.LandShape;
                tzResearchOfInvestment.IsMeetAreaPlan = model.IsMeetAreaPlan;
                tzResearchOfInvestment.AroundCarCount = model.AroundCarCount;
                tzResearchOfInvestment.DailyTraffic = model.DailyTraffic;
                tzResearchOfInvestment.OilSaleTotal = model.OilSaleTotal;
                tzResearchOfInvestment.DieselGasolineRatio = model.DieselGasolineRatio;
                tzResearchOfInvestment.LandPrice = model.LandPrice;
                tzResearchOfInvestment.GasSaleTotal = model.GasSaleTotal;
                XTSetCurrentUser(tzResearchOfInvestment);

                if (isAddInvest)
                {
                    rows = DataOperateBusiness<Epm_TzResearchOfInvestment>.Get().Add(tzResearchOfInvestment);
                }
                else
                {
                    rows = DataOperateBusiness<Epm_TzResearchOfInvestment>.Get().Update(tzResearchOfInvestment);
                }
                #endregion

                #region 现场法律调研
                bool isAddLaw = false;
                var tzResearchOfLaw = DataOperateBusiness<Epm_TzResearchOfLaw>.Get().GetList(t => t.ProjectId == model.ProjectId).FirstOrDefault();
                if (tzResearchOfLaw == null)
                {
                    isAddLaw = true;
                    tzResearchOfLaw = new Epm_TzResearchOfLaw();
                    SetCreateUser(tzResearchOfLaw);
                }
                tzResearchOfLaw.ProjectId = model.ProjectId;
                tzResearchOfLaw.State = model.State;
                tzResearchOfLaw.ResearchStartDate = model.StartDate;
                tzResearchOfLaw.ResearchEndDate = model.LawResearchEndDate;
                tzResearchOfLaw.ResearchUserId = model.LawResearchUserId;
                tzResearchOfLaw.ResearchUserName = model.LawResearchUserName;
                tzResearchOfLaw.ResearchUserXt = model.LawResearchUserXt;
                tzResearchOfLaw.JobCode = model.LawJobCode;
                tzResearchOfLaw.JobName = model.LawJobName;
                tzResearchOfLaw.PropertyRights = model.PropertyRights;
                tzResearchOfLaw.AssetSubject = model.AssetSubject;
                tzResearchOfLaw.DisputesJudgment = model.DisputesJudgment;
                tzResearchOfLaw.License = model.License;
                XTSetCurrentUser(tzResearchOfLaw);

                if (isAddLaw)
                {
                    rows = DataOperateBusiness<Epm_TzResearchOfLaw>.Get().Add(tzResearchOfLaw);
                }
                else
                {
                    rows = DataOperateBusiness<Epm_TzResearchOfLaw>.Get().Update(tzResearchOfLaw);
                }
                #endregion

                #region 经营方面调研
                bool isAddManage = false;
                var tzResearchOfManagement = DataOperateBusiness<Epm_TzResearchOfManagement>.Get().GetList(t => t.ProjectId == model.ProjectId).FirstOrDefault();
                if (tzResearchOfManagement == null)
                {
                    isAddManage = true;
                    tzResearchOfManagement = new Epm_TzResearchOfManagement();
                    SetCreateUser(tzResearchOfManagement);
                }
                tzResearchOfManagement.ProjectId = model.ProjectId;
                tzResearchOfManagement.State = model.State;
                tzResearchOfManagement.ResearchStartDate = model.StartDate;
                tzResearchOfManagement.ResearchEndDate = model.ManageResearchEndDate;
                tzResearchOfManagement.ResearchUserId = model.ManageResearchUserId;
                tzResearchOfManagement.ReaearchUserName = model.ManageReaearchUserName;
                tzResearchOfManagement.ReaearchUserXt = model.ManageReaearchUserXt;
                tzResearchOfManagement.JobCode = model.ManageJobCode;
                tzResearchOfManagement.JobName = model.ManageJobName;
                tzResearchOfManagement.SalesRealizability = model.SalesRealizability;
                tzResearchOfManagement.CargoDistance = model.CargoDistance;
                tzResearchOfManagement.SourceOfOil = model.SourceOfOil;
                tzResearchOfManagement.CurrentSalesVolume = model.CurrentSalesVolume;
                tzResearchOfManagement.SalesMeans = model.SalesMeans;
                tzResearchOfManagement.GasDailySales = model.GasDailySales;
                XTSetCurrentUser(tzResearchOfManagement);

                if (isAddManage)
                {
                    rows = DataOperateBusiness<Epm_TzResearchOfManagement>.Get().Add(tzResearchOfManagement);
                }
                else
                {
                    rows = DataOperateBusiness<Epm_TzResearchOfManagement>.Get().Update(tzResearchOfManagement);
                }
                #endregion

                #region 安全方面调研
                bool isAddSafe = false;
                var tzResearchOfSecurity = DataOperateBusiness<Epm_TzResearchOfSecurity>.Get().GetList(t => t.ProjectId == model.ProjectId).FirstOrDefault();
                if (tzResearchOfSecurity == null)
                {
                    isAddSafe = true;
                    tzResearchOfSecurity = new Epm_TzResearchOfSecurity();
                    SetCreateUser(tzResearchOfSecurity);
                }
                tzResearchOfSecurity.ProjectId = model.ProjectId;
                tzResearchOfSecurity.State = model.State;
                tzResearchOfSecurity.ResearchStartDate = model.StartDate;
                tzResearchOfSecurity.ResearchEndDate = model.SafeResearchEndDate;
                tzResearchOfSecurity.ResearchUserId = model.SafeResearchUserId;
                tzResearchOfSecurity.ReaearchUserName = model.SafeReaearchUserName;
                tzResearchOfSecurity.ReaearchUserXt = model.SafeReaearchUserXt;
                tzResearchOfSecurity.JobCode = model.SafeJobCode;
                tzResearchOfSecurity.JobName = model.SafeJobName;
                tzResearchOfSecurity.Environmental = model.Environmental;
                tzResearchOfSecurity.ImprovementMeasures = model.ImprovementMeasures;
                tzResearchOfSecurity.hiddenDanger = model.hiddenDanger;
                //SetCurrentUser(tzResearchOfSecurity);

                if (isAddSafe)
                {
                    rows = DataOperateBusiness<Epm_TzResearchOfSecurity>.Get().Add(tzResearchOfSecurity);
                }
                else
                {
                    rows = DataOperateBusiness<Epm_TzResearchOfSecurity>.Get().Update(tzResearchOfSecurity);
                }
                #endregion

                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ProjectNature, DictionaryType.PermitType };
                var subjects = GetTypeListByTypes(subjectsList).Data;
                //若状态为已提交，生成下一阶段数据
                if (model.State == (int)PreProjectState.Submitted)
                {
                    var project = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId);
                    if (project != null)
                    {
                        //土地协议谈判
                        List<string> tdcrxy = subjects[DictionaryType.ProjectNature].Where(t => t.No == "XINJ").Select(t => t.No).ToList();
                        //评审材料上报
                        List<string> psclsb = subjects[DictionaryType.ProjectNature].Where(t => t.No == "JIG" || t.No == "GAIJ").Select(t => t.No).ToList();
                        //初次谈判
                        List<string> cctp = subjects[DictionaryType.ProjectNature].Where(t => t.No != "XINJ" && t.No != "JIG" && t.No != "GAIJ").Select(t => t.No).ToList();

                        //土地出让协议谈判：新建
                        if (tdcrxy.Contains(project.Nature))
                        {
                            Epm_TzLandNegotiation land = new Epm_TzLandNegotiation();
                            land.ProjectId = project.Id;
                            land.State = (int)PreProjectState.WaitSubmitted;
                            AddTzLandNegotiation(land);
                        }

                        //跳过初次谈判和土地出让协议谈判生成评审材料上报记录：技改、改建
                        if (psclsb.Contains(project.Nature))
                        {
                            Epm_TzFormTalkFile tzFormTalkFile = new Epm_TzFormTalkFile();
                            tzFormTalkFile.ProjectId = project.Id;
                            tzFormTalkFile.State = (int)PreProjectState.WaitSubmitted;
                            AddTzFormTalkFile(tzFormTalkFile);
                        }

                        //初次谈判：参股、合资、合作、控股、收购、租赁、扩建、迁建
                        if (cctp.Contains(project.Nature))
                        {
                            Epm_TzFirstNegotiation tzfirst = new Epm_TzFirstNegotiation();
                            tzfirst.ProjectId = project.Id;
                            tzfirst.State = (int)PreProjectState.WaitSubmitted;
                            AddTzFirstNegotiation(tzfirst);
                        }

                        var projectInfo = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId);
                        //特许经营项目性质的项目审批到项目批复完成就结束了，不生成工程模块的内容，不用写到机器人表里
                        if (projectInfo.Nature != "XMXZTXJY")
                        {
                            #region 更新RPA数据
                            var hostAddress = System.Configuration.ConfigurationManager.AppSettings.Get("RPAPath");
                            var projects = DataOperateBusiness<Epm_TzProjectProposal>.Get().Single(t => t.Id == model.ProjectId);
                            #region RPA_FieldInvestigation
                            var rpaModel = new RPA_FieldInvestigation();
                            rpaModel.ResearchStartDate = tzResearchOfInvestment.ResearchStartDate;
                            rpaModel.ResearchUserNameT = tzResearchOfInvestment.ResearchUserName;
                            rpaModel.JobNameT = tzResearchOfInvestment.JobName;
                            rpaModel.Address = tzResearchOfInvestment.Address;
                            rpaModel.EnvironmentTypeName = tzResearchOfInvestment.EnvironmentTypeName;
                            rpaModel.LandNatureName = tzResearchOfInvestment.LandNatureName;
                            rpaModel.LandUseName = tzResearchOfInvestment.LandUseName;
                            rpaModel.LandArea = tzResearchOfInvestment.LandArea;
                            rpaModel.LandShape = tzResearchOfInvestment.LandShape;
                            rpaModel.IsMeetAreaPlan = tzResearchOfInvestment.IsMeetAreaPlan == "SFFH1" ? "是" : tzResearchOfInvestment.IsMeetAreaPlan == "SFFH2" ? "否" : "";
                            rpaModel.AroundCarCount = tzResearchOfInvestment.AroundCarCount;
                            rpaModel.DailyTraffic = tzResearchOfInvestment.DailyTraffic;
                            rpaModel.OilSaleTotal = tzResearchOfInvestment.OilSaleTotal;
                            rpaModel.DieselGasolineRatio = tzResearchOfInvestment.DieselGasolineRatio;
                            rpaModel.LandPrice = tzResearchOfInvestment.LandPrice;
                            rpaModel.GasSaleTotal = tzResearchOfInvestment.GasSaleTotal;
                            rpaModel.ResearchUserNameF = tzResearchOfLaw.ResearchUserName;
                            rpaModel.JobNameF = tzResearchOfLaw.JobName;
                            rpaModel.PropertyRights = tzResearchOfLaw.PropertyRights == "CQQX1" ? "是" : tzResearchOfLaw.PropertyRights == "CQQX2" ? "否" : "";
                            rpaModel.AssetSubject = tzResearchOfLaw.AssetSubject == "ZCZT1" ? "符合" : tzResearchOfLaw.AssetSubject == "ZCZT2" ? "不符合" : "";
                            rpaModel.DisputesJudgment = tzResearchOfLaw.DisputesJudgment == "JFPD1" ? "有" : tzResearchOfLaw.DisputesJudgment == "JFPD2" ? "无" : "";
                            rpaModel.License = tzResearchOfLaw.License;
                            if (!string.IsNullOrEmpty(tzResearchOfLaw.License))
                            {
                                var licenses = tzResearchOfLaw.License.Split(',');
                                for (int i = 0; i < licenses.Length; i++)
                                {
                                    var licenseList = subjects[DictionaryType.PermitType].ToList();
                                    for (int j = 0; j < licenseList.Count; j++)
                                    {
                                        if (licenses[i] == licenseList[j].No)
                                        {
                                            rpaModel.License = licenseList[j].Name + ',';
                                        }
                                    }
                                }
                            }
                            rpaModel.ResearchUserNameG = tzResearchOfEngineering.ReaearchUserName;
                            rpaModel.JobNameG = tzResearchOfEngineering.JobName;
                            rpaModel.IndustryPlanning = tzResearchOfEngineering.IndustryPlanning == "XINGXGC1" ? "符合" : tzResearchOfEngineering.IndustryPlanning == "XINGXGC2" ? "基本符合" : tzResearchOfEngineering.IndustryPlanning == "XINGXGC3" ? "不符合" : "";
                            rpaModel.Shelter = tzResearchOfEngineering.Shelter;
                            rpaModel.MachineOfGas = tzResearchOfEngineering.MachineOfGas;
                            rpaModel.MachineOfGasStage = tzResearchOfEngineering.MachineOfGasStage;
                            rpaModel.MachineOfOil = tzResearchOfEngineering.MachineOfOil;
                            rpaModel.MachineOfOilStage = tzResearchOfEngineering.MachineOfOilStage;
                            rpaModel.OilTank = tzResearchOfEngineering.OilTank;
                            rpaModel.StationRoon = tzResearchOfEngineering.StationRoon;
                            rpaModel.GasWells = tzResearchOfEngineering.GasWells;
                            rpaModel.HasInformationSystem = tzResearchOfEngineering.HasInformationSystem == "XXXT1" ? "有" : tzResearchOfEngineering.HasInformationSystem == "XXXT2" ? "无" : "";
                            rpaModel.ReformName = tzResearchOfEngineering.ReformName;
                            rpaModel.ResearchUserNameJ = tzResearchOfManagement.ReaearchUserName;
                            rpaModel.JobName = tzResearchOfManagement.JobName;
                            rpaModel.SalesRealizability = tzResearchOfManagement.SalesRealizability == "XIAOLKSX1" ? "是" : tzResearchOfManagement.SalesRealizability == "XIAOLKSX2" ? "否" : "";
                            rpaModel.CargoDistance = tzResearchOfManagement.CargoDistance;
                            rpaModel.SourceOfOil = tzResearchOfManagement.SourceOfOil;
                            rpaModel.CurrentSalesVolume = tzResearchOfManagement.CurrentSalesVolume;
                            rpaModel.SalesMeans = tzResearchOfManagement.SalesMeans == "TSSD1" ? "有" : tzResearchOfManagement.SalesMeans == "TSSD2" ? "无" : "";
                            rpaModel.GasDailySales = tzResearchOfManagement.GasDailySales;
                            rpaModel.ResearchUserNameA = tzResearchOfSecurity.ReaearchUserName;
                            rpaModel.JobNameA = tzResearchOfSecurity.JobName;
                            rpaModel.Environmental = tzResearchOfSecurity.Environmental == "HBWT1" ? "有" : tzResearchOfSecurity.Environmental == "HBWT2" ? "无" : "";
                            rpaModel.hiddenDanger = tzResearchOfSecurity.hiddenDanger == "YHWT1" ? "有" : tzResearchOfSecurity.hiddenDanger == "YHWT2" ? "无" : "";
                            rpaModel.ImprovementMeasures = tzResearchOfSecurity.ImprovementMeasures;
                            rpaModel.ResearchUserNameX = tzResearchOfInformation.ReaearchUserName;
                            rpaModel.JobNameX = tzResearchOfInformation.JobName;
                            rpaModel.Improvement = tzResearchOfInformation.Improvement;
                            rpaModel.WriteMark = 0;
                            rpaModel.WriteResult = 0;
                            rpaModel.FollowOperate = "";
                            //var userModel = DataOperateBasic<Base_User>.Get().GetModel(tzResearchOfInvestment.OperateUserId);
                            var strName = projects.CompanyName.Substring(0, 2);
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
                            rpaModel.companys = "";
                            rpaModel.ProjectName = projects.ProjectName;
                            rpaModel.Auditor = "";
                            var files = GetFilesByTZTable("Epm_FieldInvestigation", tzResearchOfInformation.Id);
                            for (int i = 0; i < files.Data.Count; i++)
                            {
                                rpaModel.FilePath += hostAddress + "?path=" + files.Data[i].FilePath + "&fileName=" + files.Data[i].Name + ',';
                            }
                            rpaModel.FileNumber = files.Data.Count;
                            rpaModel.status1 = 0;
                            SetCreateUser(rpaModel);
                            XTSetCurrentUser(rpaModel);
                            #endregion

                            #region OMADS_FieldInvestigation
                            var oMADSModel = new OMADS_FieldInvestigation();
                            oMADSModel.ResearchStartDate = tzResearchOfInvestment.ResearchStartDate;
                            oMADSModel.ResearchUserNameT = tzResearchOfInvestment.ResearchUserName;
                            oMADSModel.JobNameT = tzResearchOfInvestment.JobName;
                            oMADSModel.Address = tzResearchOfInvestment.Address;
                            oMADSModel.EnvironmentTypeName = tzResearchOfInvestment.EnvironmentTypeName;
                            oMADSModel.LandNatureName = tzResearchOfInvestment.LandNatureName;
                            oMADSModel.LandUseName = tzResearchOfInvestment.LandUseName;
                            oMADSModel.LandArea = tzResearchOfInvestment.LandArea;
                            oMADSModel.LandShape = tzResearchOfInvestment.LandShape;
                            oMADSModel.IsMeetAreaPlan = tzResearchOfInvestment.IsMeetAreaPlan == "SFFH1" ? "是" : tzResearchOfInvestment.IsMeetAreaPlan == "SFFH2" ? "否" : "";
                            oMADSModel.AroundCarCount = tzResearchOfInvestment.AroundCarCount;
                            oMADSModel.DailyTraffic = tzResearchOfInvestment.DailyTraffic;
                            oMADSModel.OilSaleTotal = tzResearchOfInvestment.OilSaleTotal;
                            oMADSModel.DieselGasolineRatio = tzResearchOfInvestment.DieselGasolineRatio;
                            oMADSModel.LandPrice = tzResearchOfInvestment.LandPrice;
                            oMADSModel.GasSaleTotal = tzResearchOfInvestment.GasSaleTotal;
                            oMADSModel.ResearchUserNameF = tzResearchOfLaw.ResearchUserName;
                            oMADSModel.JobNameF = tzResearchOfLaw.JobName;
                            oMADSModel.PropertyRights = tzResearchOfLaw.PropertyRights == "CQQX1" ? "是" : tzResearchOfLaw.PropertyRights == "CQQX2" ? "否" : "";
                            oMADSModel.AssetSubject = tzResearchOfLaw.AssetSubject == "ZCZT1" ? "符合" : tzResearchOfLaw.AssetSubject == "ZCZT2" ? "不符合" : "";
                            oMADSModel.DisputesJudgment = tzResearchOfLaw.DisputesJudgment == "JFPD1" ? "有" : tzResearchOfLaw.DisputesJudgment == "JFPD2" ? "无" : "";
                            oMADSModel.License = tzResearchOfLaw.License;
                            if (!string.IsNullOrEmpty(tzResearchOfLaw.License))
                            {
                                var olicenses = tzResearchOfLaw.License.Split(',');
                                for (int i = 0; i < olicenses.Length; i++)
                                {
                                    var olicenseList = subjects[DictionaryType.PermitType].ToList();
                                    for (int j = 0; j < olicenseList.Count; j++)
                                    {
                                        if (olicenses[i] == olicenseList[j].No)
                                        {
                                            oMADSModel.License = olicenseList[j].Name + ',';
                                        }
                                    }
                                }
                            }
                            oMADSModel.ResearchUserNameG = tzResearchOfEngineering.ReaearchUserName;
                            oMADSModel.JobNameG = tzResearchOfEngineering.JobName;
                            oMADSModel.IndustryPlanning = tzResearchOfEngineering.IndustryPlanning == "XINGXGC1" ? "符合" : tzResearchOfEngineering.IndustryPlanning == "XINGXGC2" ? "基本符合" : tzResearchOfEngineering.IndustryPlanning == "XINGXGC3" ? "不符合" : "";
                            oMADSModel.Shelter = tzResearchOfEngineering.Shelter;
                            oMADSModel.MachineOfGas = tzResearchOfEngineering.MachineOfGas;
                            oMADSModel.MachineOfGasStage = tzResearchOfEngineering.MachineOfGasStage;
                            oMADSModel.MachineOfOil = tzResearchOfEngineering.MachineOfOil;
                            oMADSModel.MachineOfOilStage = tzResearchOfEngineering.MachineOfOilStage;
                            oMADSModel.OilTank = tzResearchOfEngineering.OilTank;
                            oMADSModel.StationRoon = tzResearchOfEngineering.StationRoon;
                            oMADSModel.GasWells = tzResearchOfEngineering.GasWells;
                            oMADSModel.HasInformationSystem = tzResearchOfEngineering.HasInformationSystem == "XXXT1" ? "有" : tzResearchOfEngineering.HasInformationSystem == "XXXT2" ? "无" : "";
                            oMADSModel.ReformName = tzResearchOfEngineering.ReformName;
                            oMADSModel.ResearchUserNameJ = tzResearchOfManagement.ReaearchUserName;
                            oMADSModel.JobName = tzResearchOfManagement.JobName;
                            oMADSModel.SalesRealizability = tzResearchOfManagement.SalesRealizability == "XIAOLKSX1" ? "是" : tzResearchOfManagement.SalesRealizability == "XIAOLKSX2" ? "否" : "";
                            oMADSModel.CargoDistance = tzResearchOfManagement.CargoDistance;
                            oMADSModel.SourceOfOil = tzResearchOfManagement.SourceOfOil;
                            oMADSModel.CurrentSalesVolume = tzResearchOfManagement.CurrentSalesVolume;
                            oMADSModel.SalesMeans = tzResearchOfManagement.SalesMeans == "TSSD1" ? "有" : tzResearchOfManagement.SalesMeans == "TSSD2" ? "无" : "";
                            oMADSModel.GasDailySales = tzResearchOfManagement.GasDailySales;
                            oMADSModel.ResearchUserNameA = tzResearchOfSecurity.ReaearchUserName;
                            oMADSModel.JobNameA = tzResearchOfSecurity.JobName;
                            oMADSModel.Environmental = tzResearchOfSecurity.Environmental == "HBWT1" ? "有" : tzResearchOfSecurity.Environmental == "HBWT2" ? "无" : "";
                            oMADSModel.hiddenDanger = tzResearchOfSecurity.hiddenDanger == "YHWT1" ? "有" : tzResearchOfSecurity.hiddenDanger == "YHWT2" ? "无" : "";
                            oMADSModel.ImprovementMeasures = tzResearchOfSecurity.ImprovementMeasures;
                            oMADSModel.ResearchUserNameX = tzResearchOfInformation.ReaearchUserName;
                            oMADSModel.JobNameX = tzResearchOfInformation.JobName;
                            oMADSModel.Improvement = tzResearchOfInformation.Improvement;
                            oMADSModel.WriteMark = 0;
                            oMADSModel.WriteResult = 0;
                            oMADSModel.FollowOperate = "";
                            var ouserModel = DataOperateBasic<Base_User>.Get().GetModel(tzResearchOfInvestment.OperateUserId);
                            oMADSModel.username = rpaModel.username;
                            oMADSModel.companys = "";
                            oMADSModel.ProjectName = projects.ProjectName;
                            oMADSModel.Auditor = "";
                            var ofiles = GetFilesByTZTable("Epm_FieldInvestigation", tzResearchOfInformation.Id);
                            for (int i = 0; i < ofiles.Data.Count; i++)
                            {
                                rpaModel.FilePath += hostAddress + "?path=" + ofiles.Data[i].FilePath + "&fileName=" + ofiles.Data[i].Name + ',';
                            }
                            oMADSModel.FileNumber = ofiles.Data.Count;
                            oMADSModel.status1 = 0;
                            SetCreateUser(oMADSModel);
                            XTSetCurrentUser(oMADSModel);
                            #endregion

                            #region TEMP_FieldInvestigation
                            var tEMPModel = new TEMP_FieldInvestigation();
                            tEMPModel.ResearchStartDate = tzResearchOfInvestment.ResearchStartDate;
                            tEMPModel.ResearchUserNameT = tzResearchOfInvestment.ResearchUserName;
                            tEMPModel.JobNameT = tzResearchOfInvestment.JobName;
                            tEMPModel.Address = tzResearchOfInvestment.Address;
                            tEMPModel.EnvironmentTypeName = tzResearchOfInvestment.EnvironmentTypeName;
                            tEMPModel.LandNatureName = tzResearchOfInvestment.LandNatureName;
                            tEMPModel.LandUseName = tzResearchOfInvestment.LandUseName;
                            tEMPModel.LandArea = tzResearchOfInvestment.LandArea;
                            tEMPModel.LandShape = tzResearchOfInvestment.LandShape;
                            tEMPModel.IsMeetAreaPlan = tzResearchOfInvestment.IsMeetAreaPlan == "SFFH1" ? "是" : tzResearchOfInvestment.IsMeetAreaPlan == "SFFH2" ? "否" : "";
                            tEMPModel.AroundCarCount = tzResearchOfInvestment.AroundCarCount;
                            tEMPModel.DailyTraffic = tzResearchOfInvestment.DailyTraffic;
                            tEMPModel.OilSaleTotal = tzResearchOfInvestment.OilSaleTotal;
                            tEMPModel.DieselGasolineRatio = tzResearchOfInvestment.DieselGasolineRatio;
                            tEMPModel.LandPrice = tzResearchOfInvestment.LandPrice;
                            tEMPModel.GasSaleTotal = tzResearchOfInvestment.GasSaleTotal;
                            tEMPModel.ResearchUserNameF = tzResearchOfLaw.ResearchUserName;
                            tEMPModel.JobNameF = tzResearchOfLaw.JobName;
                            tEMPModel.PropertyRights = tzResearchOfLaw.PropertyRights == "CQQX1" ? "是" : tzResearchOfLaw.PropertyRights == "CQQX2" ? "否" : "";
                            tEMPModel.AssetSubject = tzResearchOfLaw.AssetSubject == "ZCZT1" ? "符合" : tzResearchOfLaw.AssetSubject == "ZCZT2" ? "不符合" : "";
                            tEMPModel.DisputesJudgment = tzResearchOfLaw.DisputesJudgment == "JFPD1" ? "有" : tzResearchOfLaw.DisputesJudgment == "JFPD2" ? "无" : "";
                            tEMPModel.License = tzResearchOfLaw.License;
                            if (!string.IsNullOrEmpty(tzResearchOfLaw.License))
                            {
                                var tlicenses = tzResearchOfLaw.License.Split(',');
                                for (int i = 0; i < tlicenses.Length; i++)
                                {
                                    var tlicenseList = subjects[DictionaryType.PermitType].ToList();
                                    for (int j = 0; j < tlicenseList.Count; j++)
                                    {
                                        if (tlicenses[i] == tlicenseList[j].No)
                                        {
                                            tEMPModel.License = tlicenseList[j].Name + ',';
                                        }
                                    }
                                }
                            }
                            tEMPModel.ResearchUserNameG = tzResearchOfEngineering.ReaearchUserName;
                            tEMPModel.JobNameG = tzResearchOfEngineering.JobName;
                            tEMPModel.IndustryPlanning = tzResearchOfEngineering.IndustryPlanning == "XINGXGC1" ? "符合" : tzResearchOfEngineering.IndustryPlanning == "XINGXGC2" ? "基本符合" : tzResearchOfEngineering.IndustryPlanning == "XINGXGC3" ? "不符合" : "";
                            tEMPModel.Shelter = tzResearchOfEngineering.Shelter;
                            tEMPModel.MachineOfGas = tzResearchOfEngineering.MachineOfGas;
                            tEMPModel.MachineOfGasStage = tzResearchOfEngineering.MachineOfGasStage;
                            tEMPModel.MachineOfOil = tzResearchOfEngineering.MachineOfOil;
                            tEMPModel.MachineOfOilStage = tzResearchOfEngineering.MachineOfOilStage;
                            tEMPModel.OilTank = tzResearchOfEngineering.OilTank;
                            tEMPModel.StationRoon = tzResearchOfEngineering.StationRoon;
                            tEMPModel.GasWells = tzResearchOfEngineering.GasWells;
                            tEMPModel.HasInformationSystem = tzResearchOfEngineering.HasInformationSystem == "XXXT1" ? "有" : tzResearchOfEngineering.HasInformationSystem == "XXXT2" ? "无" : "";
                            tEMPModel.ReformName = tzResearchOfEngineering.ReformName;
                            tEMPModel.ResearchUserNameJ = tzResearchOfManagement.ReaearchUserName;
                            tEMPModel.JobName = tzResearchOfManagement.JobName;
                            tEMPModel.SalesRealizability = tzResearchOfManagement.SalesRealizability == "XIAOLKSX1" ? "是" : tzResearchOfManagement.SalesRealizability == "XIAOLKSX2" ? "否" : "";
                            tEMPModel.CargoDistance = tzResearchOfManagement.CargoDistance;
                            tEMPModel.SourceOfOil = tzResearchOfManagement.SourceOfOil;
                            tEMPModel.CurrentSalesVolume = tzResearchOfManagement.CurrentSalesVolume;
                            tEMPModel.SalesMeans = tzResearchOfManagement.SalesMeans == "TSSD1" ? "有" : tzResearchOfManagement.SalesMeans == "TSSD2" ? "无" : "";
                            tEMPModel.GasDailySales = tzResearchOfManagement.GasDailySales;
                            tEMPModel.ResearchUserNameA = tzResearchOfSecurity.ReaearchUserName;
                            tEMPModel.JobNameA = tzResearchOfSecurity.JobName;
                            tEMPModel.Environmental = tzResearchOfSecurity.Environmental == "HBWT1" ? "有" : tzResearchOfSecurity.Environmental == "HBWT2" ? "无" : "";
                            tEMPModel.hiddenDanger = tzResearchOfSecurity.hiddenDanger == "YHWT1" ? "有" : tzResearchOfSecurity.hiddenDanger == "YHWT2" ? "无" : "";
                            tEMPModel.ImprovementMeasures = tzResearchOfSecurity.ImprovementMeasures;
                            tEMPModel.ResearchUserNameX = tzResearchOfInformation.ReaearchUserName;
                            tEMPModel.JobNameX = tzResearchOfInformation.JobName;
                            tEMPModel.Improvement = tzResearchOfInformation.Improvement;
                            tEMPModel.WriteMark = 0;
                            tEMPModel.WriteResult = 0;
                            tEMPModel.FollowOperate = "";
                            var tuserModel = DataOperateBasic<Base_User>.Get().GetModel(tzResearchOfInvestment.OperateUserId);
                            tEMPModel.username = rpaModel.username;
                            tEMPModel.companys = "";
                            tEMPModel.ProjectName = projects.ProjectName;
                            tEMPModel.Auditor = "";
                            var tfiles = GetFilesByTZTable("Epm_FieldInvestigation", tzResearchOfInformation.Id);
                            for (int i = 0; i < tfiles.Data.Count; i++)
                            {
                                tEMPModel.FilePath += hostAddress + "?path=" + tfiles.Data[i].FilePath + "&fileName=" + tfiles.Data[i].Name + ',';
                            }
                            tEMPModel.FileNumber = tfiles.Data.Count;
                            tEMPModel.status1 = 0;
                            SetCreateUser(tEMPModel);
                            XTSetCurrentUser(tEMPModel);
                            #endregion

                            DataOperateBusiness<RPA_FieldInvestigation>.Get().Add(rpaModel);
                            DataOperateBusiness<OMADS_FieldInvestigation>.Get().Add(oMADSModel);
                            DataOperateBusiness<TEMP_FieldInvestigation>.Get().Add(tEMPModel);
                            //transaction.Commit();
                            #endregion
                        }
                    }
                }
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzSiteSurvey.GetText(), SystemRight.Add.GetText(), "新增: ");

                
            }
            catch (Exception ex)
            {
                //transaction.Rollback();
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzSiteSurvey");
            }
            return result;
        }

        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzSiteSurvey(Epm_TzSiteSurvey model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzSiteSurvey>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzSiteSurvey.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzSiteSurvey");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzSiteSurveyByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzSiteSurvey>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzSiteSurvey>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
               // WriteLog(AdminModule.TzSiteSurvey.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzSiteSurveyByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzSiteSurvey>> GetTzSiteSurveyList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzSiteSurvey>> result = new Result<List<Epm_TzSiteSurvey>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzSiteSurvey>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzSiteSurveyList");
            }
            return result;
        }

        /// <summary>
        /// 现场调研列表查询（连项目前期信息表）
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<TzProjectProposalView>> GetTzSiteSurveyAndProjectList(QueryCondition qc)
        {
            Result<List<TzProjectProposalView>> result = new Result<List<TzProjectProposalView>>();
            try
            {
                var query = from a in context.Epm_TzProjectProposal.Where(p => p.IsDelete == false)
                            join b in context.Epm_TzSiteSurvey.Where(p => p.IsDelete == false) on a.Id equals b.ProjectId into temp
                            from tt in temp.DefaultIfEmpty()
                            select new TzProjectProposalView
                            {
                                TzProjectProposal = a,
                                TzSiteSurvey = tt
                            };
                string projectName = "";
                string projectNature = "";
                string companyName = "";
                string startTime = "";
                string endTime = "";
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
                                        projectName = value;
                                        break;
                                    }
                                case "projectNature":
                                    {
                                        projectNature = value;
                                        break;
                                    }
                                case "companyName":
                                    {
                                        companyName = value;
                                        break;
                                    }
                                case "startTime":
                                    {
                                        startTime = value;
                                        break;
                                    }
                                case "endTime":
                                    {
                                        endTime = value;
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
                {
                    query = query.Where(t => (t.TzProjectProposal.ProjectName.Contains(projectName) || projectName == "")
                                      && (t.TzProjectProposal.Nature.Contains(projectNature) || projectNature == "")
                                      && (t.TzProjectProposal.CompanyName.Contains(companyName) || companyName == ""));
                }
                else {
                    DateTime startTime1 = Convert.ToDateTime(startTime);
                    DateTime endTime1 = Convert.ToDateTime(endTime);

                    query = query.Where(t => (t.TzProjectProposal.ProjectName.Contains(projectName) || projectName == "")
                                          && (t.TzProjectProposal.Nature.Contains(projectNature) || projectNature == "")
                                          && (t.TzProjectProposal.CompanyName.Contains(companyName) || companyName == "")
                                          && (t.TzSiteSurvey.InspectStTime.Value >= startTime1 && t.TzSiteSurvey.InspectStTime.Value <= endTime1));
                }

                result.AllRowsCount = query.Count();
                query = query.OrderByDescending(t => t.TzSiteSurvey.OperateTime).Skip((qc.PageInfo.CurrentPageIndex - 1) * qc.PageInfo.PageRowCount).Take(qc.PageInfo.PageRowCount);
                result.Data = query.ToList();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzSiteSurveyAndProjectList");
            }
            return result;
        }


        /// <summary>
        /// 初次谈判列表查询（连项目前期信息表）
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<TzProjectProposalView>> GetTzInitialTalkAndProjectList(QueryCondition qc)
        {
            Result<List<TzProjectProposalView>> result = new Result<List<TzProjectProposalView>>();
            try
            {
                var query = from a in context.Epm_TzProjectProposal.Where(p => p.IsDelete == false)
                            join b in context.Epm_TzInitialTalk.Where(p => p.IsDelete == false) on a.Id equals b.ProjectId into temp
                            from tt in temp.DefaultIfEmpty()
                            select new TzProjectProposalView
                            {
                                TzProjectProposal = a,
                                TzInitialTalk = tt
                            };
                string projectName = "";
                string projectNature = "";
                string companyName = "";
                string startTime = "";
                string endTime = "";
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
                                        projectName = value;
                                        break;
                                    }
                                case "projectNature":
                                    {
                                        projectNature = value;
                                        break;
                                    }
                                case "companyName":
                                    {
                                        companyName = value;
                                        break;
                                    }
                                case "startTime":
                                    {
                                        startTime = value;
                                        break;
                                    }
                                case "endTime":
                                    {
                                        endTime = value;
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(startTime) && string.IsNullOrEmpty(endTime))
                {
                    query = query.Where(t => (t.TzProjectProposal.ProjectName.Contains(projectName) || projectName == "")
                                      && (t.TzProjectProposal.Nature.Contains(projectNature) || projectNature == "")
                                      && (t.TzProjectProposal.CompanyName.Contains(companyName) || companyName == ""));
                }
                else {
                    DateTime startTime1 = Convert.ToDateTime(startTime);
                    DateTime endTime1 = Convert.ToDateTime(endTime);

                    query = query.Where(t => (t.TzProjectProposal.ProjectName.Contains(projectName) || projectName == "")
                                          && (t.TzProjectProposal.Nature.Contains(projectNature) || projectNature == "")
                                          && (t.TzProjectProposal.CompanyName.Contains(companyName) || companyName == "")
                                          && (t.TzInitialTalk.TalkTime.Value >= startTime1 && t.TzInitialTalk.TalkTime.Value <= endTime1));
                }

                result.AllRowsCount = query.Count();
                query = query.OrderByDescending(t => t.TzInitialTalk.OperateTime).Skip((qc.PageInfo.CurrentPageIndex - 1) * qc.PageInfo.PageRowCount).Take(qc.PageInfo.PageRowCount);
                result.Data = query.ToList();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzInitialTalkAndProjectList");
            }
            return result;
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzSiteSurvey> GetTzSiteSurveyModel(long id)
        {
            Result<Epm_TzSiteSurvey> result = new Result<Epm_TzSiteSurvey>();
            try
            {
                var model = DataOperateBusiness<Epm_TzSiteSurvey>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzSiteSurveyModel");
            }
            return result;
        }

        /// <summary>
        /// 获取现场勘查列表
        /// </summary>
        /// <returns></returns>
        public Result<List<Epm_TzProjectProposal>> GetTzResearchList(QueryCondition qc)
        {
            Result<List<Epm_TzProjectProposal>> result = new Result<List<Epm_TzProjectProposal>>();
            try
            {
                var query = from invest in context.Epm_TzResearchOfInvestment.Where(p => p.IsDelete == false)
                            join eng in context.Epm_TzResearchOfEngineering.Where(p => p.IsDelete == false) on invest.ProjectId equals eng.ProjectId
                            join info in context.Epm_TzResearchOfInformation.Where(p => p.IsDelete == false) on invest.ProjectId equals info.ProjectId
                            join law in context.Epm_TzResearchOfLaw.Where(p => p.IsDelete == false) on invest.ProjectId equals law.ProjectId
                            join manage in context.Epm_TzResearchOfManagement.Where(p => p.IsDelete == false) on invest.ProjectId equals manage.ProjectId
                            join safes in context.Epm_TzResearchOfSecurity.Where(p => p.IsDelete == false) on invest.ProjectId equals safes.ProjectId
                            join project in context.Epm_TzProjectProposal.Where(p => p.IsDelete == false) on invest.ProjectId equals project.Id
                            select new
                            {
                                invest.ProjectId,
                                project.ProjectName,
                                project.Nature,
                                project.NatureName,
                                project.StationName,
                                project.StationId,
                                project.ApplyTime,
                                invest.State,
                                invest.OperateUserName,
                                invest.OperateTime,
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
                    State = c.State,
                    //OperateUserName = c.OperateUserName,
                    OperateUserName = c.State != (int)PreProjectState.WaitSubmitted ? c.OperateUserName : "",
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
                result.Exception = new ExceptionEx(ex, "GetTzResearchList");
            }
            return result;
        }

        /// <summary>
        /// 根据项目Id获取现场勘探和项目信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Result<TzResearchAllView> GetTzSiteSurveyProject(long projectId)
        {
            Result<TzResearchAllView> result = new Result<TzResearchAllView>();
            try
            {
                TzResearchAllView view = new TzResearchAllView();
                var projectModel = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(projectId);
                if (projectModel != null)
                {
                    view.TzProjectProposal = projectModel;
                }

                var tzResearchOfEngineering = DataOperateBusiness<Epm_TzResearchOfEngineering>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();
                var tzResearchOfInformation = DataOperateBusiness<Epm_TzResearchOfInformation>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();
                var tzResearchOfInvestment = DataOperateBusiness<Epm_TzResearchOfInvestment>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();
                var tzResearchOfLaw = DataOperateBusiness<Epm_TzResearchOfLaw>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();
                var tzResearchOfManagement = DataOperateBusiness<Epm_TzResearchOfManagement>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();
                var tzResearchOfSecurity = DataOperateBusiness<Epm_TzResearchOfSecurity>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();

                if (tzResearchOfEngineering != null)
                {
                    view.TzResearchOfEngineering = tzResearchOfEngineering;
                }
                if (tzResearchOfInformation != null)
                {
                    List<Epm_TzAttachs> tzAttachsList = new List<Epm_TzAttachs>();
                    tzAttachsList = GetFilesByTZTable("Epm_TzResearchOfInformation", tzResearchOfInformation.Id).Data;
                    if (tzAttachsList != null && tzAttachsList.Any())
                    {
                        tzResearchOfInformation.TzAttachs = tzAttachsList;
                    }
                    view.TzResearchOfInformation = tzResearchOfInformation;
                }
                if (tzResearchOfInvestment != null)
                {
                    view.TzResearchOfInvestment = tzResearchOfInvestment;
                }
                if (tzResearchOfLaw != null)
                {
                    view.TzResearchOfLaw = tzResearchOfLaw;
                }
                if (tzResearchOfManagement != null)
                {
                    view.TzResearchOfManagement = tzResearchOfManagement;
                }
                if (tzResearchOfSecurity != null)
                {
                    view.TzResearchOfSecurity = tzResearchOfSecurity;
                }
                result.Data = view;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzSiteSurveyProject");
            }
            return result;
        }
    }
}
