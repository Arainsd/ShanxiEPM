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

using hc.Plat.Common.Service;
using System.Configuration;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzDesignScheme(Epm_TzDesignScheme model)
        {
            Result<int> result = new Result<int>();
            try
            {
                int rows = 0;
                bool isAdd = false;
                //根据批复信息自动生成一条方案信息，如果已经存在==修改
                var reveiews = DataOperateBusiness<Epm_TzDesignScheme>.Get().GetList(t => t.ProjectId == model.ProjectId && t.State != (int)PreProjectApprovalState.ApprovalFailure && t.State != (int)PreProjectApprovalState.Closed).FirstOrDefault();
                #region receiews
                if (reveiews == null)//没有-新增
                {
                    isAdd = true;
                    reveiews = new Epm_TzDesignScheme();
                    SetCreateUser(reveiews);
                }
                ///所属项目ID
                reveiews.ProjectId = model.ProjectId;
                ///项目编码
                reveiews.ProjectCode = model.ProjectCode;
                ///项目名称
                reveiews.ProjectName = model.ProjectName;
                ///项目批复号
                reveiews.ApprovalNo = model.ApprovalNo;
                ///项目性质（数据字典）
                reveiews.Nature = model.Nature;
                ///项目性质名称
                reveiews.NatureName = model.NatureName;
                ///项目提出日期（冗余）
                reveiews.ApplyTime = model.ApplyTime;
                //库站ID
                reveiews.StationId = model.StationId;
                ///库站协同编码
                reveiews.StationCodeXt = model.StationCodeXt;
                ///库站名称
                reveiews.StationName = model.StationName;
                ///地市公司ID
                reveiews.CompanyId = model.CompanyId;
                ///地市公司协同编码
                reveiews.CompanyCodeXt = model.CompanyCodeXt;
                ///地市公司名称
                reveiews.CompanyName = model.CompanyName;
                ///初步设计单位
                reveiews.DesignUnit = model.DesignUnit;
                ///示范/标注数据字典编码
                reveiews.StandarCode = model.StandarCode;
                ///示范名称
                reveiews.StandarName = model.StandarName;
                ///上报概算
                reveiews.Estimate = model.Estimate;
                ///总工程费用（冗余）
                reveiews.TotalInvestment = model.TotalInvestment;
                ///其它工程费用（冗余）
                reveiews.OtheInvestment = model.OtheInvestment;
                ///设计单位招标日期	
                reveiews.InviteTime = model.InviteTime;
                ///设计单位负责人
                reveiews.DesignUnitCharge = model.DesignUnitCharge;
                //设计单位负责人职务
                reveiews.DesignJob = model.DesignJob;
                ///项目经理（冗余）
                reveiews.ProjectManager = model.ProjectManager;
                ///项目经理职务（冗余）
                reveiews.ProjectJob = model.ProjectJob;
                ///占地面积
                reveiews.LandArea = model.LandArea;
                ///加油机（台）
                reveiews.MachineofOilStage = model.MachineofOilStage;
                ///加气机（台）
                reveiews.MachineofGasStage = model.MachineofGasStage;
                ///储气机
                reveiews.GasWells = model.GasWells;
                ///油罐
                reveiews.OilTank = model.OilTank;
                ///罩棚面积（冗余）
                reveiews.Shelter = model.Shelter;
                ///站房面积（冗余）
                reveiews.StationRoom = model.StationRoom;
                ///便利店面积--暂时没有
                reveiews.ConvenienceRoom = model.ConvenienceRoom;
                ///批复概算投资（冗余）
                reveiews.ReleaseInvestmentAmount = model.ReleaseInvestmentAmount;
                ///其他工程内容
                reveiews.OtherProject = model.OtherProject;
                ///项目信息是否同步：是、否
                reveiews.IsSynchro = model.IsSynchro;
                ///状态：暂存、待审核、审批通过、不通过
                reveiews.State = model.State;
                ///工程费用
                reveiews.EngineeringCost = model.EngineeringCost;
                ///其它费用
                reveiews.OtherExpenses = model.OtherExpenses;
                ///土地费用
                reveiews.LandCosts = model.LandCosts;
                ///估算投资
                reveiews.PredictMoney = model.PredictMoney;
                ///地区公司
                reveiews.RegionCompany = model.RegionCompany;
                //项目类型
                reveiews.ProjectType = model.ProjectType;
                #endregion

                SetCurrentUser(reveiews);
                #region  设计方案流程申请     暂时注释  勿删！！！
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                {
                    TzDesignSchemeWorkFlowView view = new TzDesignSchemeWorkFlowView();

                    view.ApprovalNo = model.ApprovalNo;
                    view.CompanyName = model.CompanyName;
                    view.ConvenienceRoom = model.ConvenienceRoom.ToString();
                    view.DesignJob = model.DesignJob;
                    view.DesignUnit = model.DesignUnit;
                    view.DesignUnitCharge = model.DesignUnitCharge;
                    view.Estimate = model.Estimate.ToString();
                    view.GasWells = model.GasWells.ToString();
                    view.InviteTime = string.Format("{0:yyyy-MM-dd}", model.InviteTime);
                    view.IsSynchro = model.IsSynchro;
                    view.LandArea = model.LandArea.ToString();
                    view.MachineofGasStage = model.MachineofGasStage.ToString();
                    view.MachineofOilStage = model.MachineofOilStage.ToString();
                    view.NatureName = model.NatureName;
                    view.OilTank = model.OilTank.ToString();
                    view.OtheInvestment = model.OtheInvestment.ToString();
                    view.OtherProject = model.OtherProject;
                    view.PredictMoney = model.PredictMoney.ToString();
                    view.ProjectCode = model.ProjectCode;
                    view.ProjectJob = model.ProjectJob;
                    view.ProjectManager = model.ProjectManager;
                    view.ProjectName = model.ProjectName;
                    view.ProvinceName = model.RegionCompany;
                    view.ReleaseInvestmentAmount = model.ReleaseInvestmentAmount.ToString();
                    view.Shelter = model.Shelter.ToString();
                    view.StandarName = model.StandarName;
                    view.StationName = model.StationName;
                    view.StationRoom = model.StationRoom.ToString();
                    view.StationTypeName = model.ProjectType;
                    view.TotalInvestment = model.TotalInvestment.ToString();
                    view.LandCosts = model.LandCosts.ToString();
                    view.EngineeringCost = model.EngineeringCost.ToString();
                    var baseUser = DataOperateBasic<Base_User>.Get().GetModel(reveiews.CreateUserId);
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

                    reveiews.WorkFlowId = XtWorkFlowService.CreateDesignSchemeWorkFlow(view);
            }
                #endregion
            if (isAdd)
                {
                    rows = DataOperateBusiness<Epm_TzDesignScheme>.Get().Add(reveiews);
                }
                else
                {
                    rows = DataOperateBusiness<Epm_TzDesignScheme>.Get().Update(reveiews);
                }

                //上传附件
                AddFilesBytzTable(reveiews, model.TzAttachs);



                result.Data = rows;
                result.Flag = EResultFlag.Success;
                // WriteLog(AdminModule.TzDesignScheme.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzDesignScheme");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzDesignScheme(Epm_TzDesignScheme model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var scheme = DataOperateBusiness<Epm_TzDesignScheme>.Get().Single(p => p.ProjectId == model.ProjectId);
                if (scheme == null)
                {
                    throw new Exception("修改的数据不存在或者已被删除！");
                }
                //生成设计方案信息
                //scheme.ProjectId = model.ProjectId;
                //scheme.ProjectCode = model.ProjectCode;
                //scheme.ProjectName = model.ProjectName;
                //scheme.ApprovalNo = model.ApprovalNo;
                //scheme.Nature = model.Nature;
                //scheme.NatureName = model.NatureName;
                //scheme.ApplyTime = model.ApplyTime;
                //scheme.StationId = model.StationId;
                //scheme.StationCodeXt = model.StationCodeXt;
                //scheme.StationName = model.StationName;
                //scheme.CompanyId = model.CompanyId;
                //scheme.CompanyCodeXt = model.CompanyCodeXt;
                //scheme.CompanyName = model.CompanyName;
                //scheme.PredictMoney = model.PredictMoney;

                //scheme.DesignUnit = model.DesignUnit;
                //scheme.StandarCode = model.StandarCode;
                //scheme.StandarName = model.StandarName;
                //scheme.Estimate = model.Estimate;
                //scheme.TotalInvestment = model.TotalInvestment;
                //scheme.OtheInvestment = model.OtheInvestment;
                //scheme.InviteTime = model.InviteTime;
                //scheme.DesignUnitCharge = model.DesignUnitCharge;
                //scheme.DesignJob = model.DesignJob;
                //scheme.ProjectManager = model.ProjectManager;
                //scheme.ProjectJob = model.ProjectJob;

                //scheme.ConvenienceRoom = model.ConvenienceRoom;
                //scheme.ReleaseInvestmentAmount = model.ReleaseInvestmentAmount;
                //scheme.OtherProject = model.OtherProject;
                //scheme.IsSynchro = model.IsSynchro;


                //scheme.EngineeringCost = model.EngineeringCost;
                //scheme.OtherExpenses = model.OtherExpenses;
                //scheme.LandCosts = model.LandCosts;
                //scheme.RegionCompany = model.RegionCompany;
                //scheme.ProjectType = model.ProjectType;

                //scheme.LandArea = model.LandArea;//占地面积
                //scheme.MachineofOilStage = model.MachineofOilStage;//加油机
                //scheme.MachineofGasStage = model.MachineofGasStage;//加气机
                //scheme.GasWells = model.GasWells;//储气井
                //scheme.OilTank = model.OilTank;//油罐
                //scheme.Shelter = model.Shelter;//罩棚面积
                //scheme.StationRoom = model.StationRoom;//站房面积
                //scheme.TotalInvestment = model.TotalInvestment;//批复概算投资
                //scheme.ApprovalNo = model.ApprovalNo;//批复文号

                SetCurrentUser(model);

                //上传附件
                AddFilesBytzTable(model, model.TzAttachs);

                #region  设计方案流程申请   暂时注释  勿删！！！
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                {
                    TzDesignSchemeWorkFlowView view = new TzDesignSchemeWorkFlowView();

                    view.ApprovalNo = model.ApprovalNo;
                    view.CompanyName = model.CompanyName;
                    view.ConvenienceRoom = model.ConvenienceRoom.ToString();
                    view.DesignJob = model.DesignJob;
                    view.DesignUnit = model.DesignUnit;
                    view.DesignUnitCharge = model.DesignUnitCharge;
                    view.Estimate = model.Estimate.ToString();
                    view.GasWells = model.GasWells.ToString();
                    view.InviteTime = string.Format("{0:yyyy-MM-dd}", model.InviteTime);
                    view.IsSynchro = model.IsSynchro;
                    view.LandArea = model.LandArea.ToString();
                    view.MachineofGasStage = model.MachineofGasStage.ToString();
                    view.MachineofOilStage = model.MachineofOilStage.ToString();
                    view.NatureName = model.NatureName;
                    view.OilTank = model.OilTank.ToString();
                    view.OtheInvestment = model.OtheInvestment.ToString();
                    view.OtherProject = model.OtherProject;
                    view.PredictMoney = model.PredictMoney.ToString();
                    view.ProjectCode = model.ProjectCode;
                    view.ProjectJob = model.ProjectJob;
                    view.ProjectManager = model.ProjectManager;
                    view.ProjectName = model.ProjectName;
                    view.ProvinceName = model.RegionCompany;
                    view.ReleaseInvestmentAmount = model.ReleaseInvestmentAmount.ToString();
                    view.Shelter = model.Shelter.ToString();
                    view.StandarName = model.StandarName;
                    view.StationName = model.StationName;
                    view.StationRoom = model.StationRoom.ToString();
                    view.StationTypeName = model.ProjectType;
                    view.TotalInvestment = model.TotalInvestment.ToString();
                    view.LandCosts = model.LandCosts.ToString();
                    view.EngineeringCost = model.EngineeringCost.ToString();
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

                    model.WorkFlowId = XtWorkFlowService.CreateDesignSchemeWorkFlow(view);
            }
                #endregion
            var rows = DataOperateBusiness<Epm_TzDesignScheme>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                // WriteLog(AdminModule.TzDesignScheme.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzDesignScheme");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzDesignSchemeByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzDesignScheme>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzDesignScheme>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                // WriteLog(AdminModule.TzDesignScheme.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzDesignSchemeByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzDesignScheme>> GetTzDesignSchemeList(QueryCondition qc)
        {
            Result<List<Epm_TzDesignScheme>> result = new Result<List<Epm_TzDesignScheme>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzDesignScheme>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzDesignSchemeList");
            }
            return result;
        }
        ///<summary>
        ///获取设计方案详情:
        ///</summary>
        /// <param name="id">数据Id，</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzDesignScheme> GetTzDesignSchemeModel(long id)
        {
            Result<Epm_TzDesignScheme> result = new Result<Epm_TzDesignScheme>();
            try
            {
                var model = DataOperateBusiness<Epm_TzDesignScheme>.Get().Single(p => p.Id == id);

                List<Epm_TzAttachs> tzAttachsList = new List<Epm_TzAttachs>();
                tzAttachsList = GetFilesByTZTable("Epm_TzDesignScheme", model.Id).Data;
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
                result.Exception = new ExceptionEx(ex, "GetTzDesignSchemeModel");
            }
            return result;
        }

        #region 获取所有项目批复信息
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件，暂时查出来所有的</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectApproval>> GetTzProjectApprovalList_Choice(QueryCondition qc)
        {

            Result<List<Epm_TzProjectApproval>> result = new Result<List<Epm_TzProjectApproval>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzProjectApproval>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzProjectApprovalList_Choice");
            }
            return result;
        }
        #endregion


        /// <summary>
        /// 修改设计方案状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzDesignSchemeState(List<long> ids, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                foreach (var item in ids)
                {
                    var model = DataOperateBusiness<Epm_TzDesignScheme>.Get().GetModel(item);
                    if (model != null)
                    {
                        //SetCurrentUser(model);
                        model.State = (int)state.ToEnumReq<PreProjectApprovalState>();
                        var rows = DataOperateBusiness<Epm_TzDesignScheme>.Get().Update(model);

                        //如果状态是已经提交，自动生成施工图纸信息
                        if (model.State == (int)PreProjectApprovalState.ApprovalSuccess)
                        {
                            Epm_TzConDrawing tzdrawing = new Epm_TzConDrawing();//图纸实体
                            List<Base_Files> filesList = new List<Base_Files>();

                            tzdrawing.ProjectId = model.ProjectId;//项目ID
                            tzdrawing.State = (int)PreProjectState.WaitSubmitted;//状态
                            tzdrawing.ProjectCode = model.ProjectCode;
                            tzdrawing.ProjectName = model.ProjectName;
                            tzdrawing.ProjectId = model.ProjectId;//项目id
                            tzdrawing.ProjectCode = model.ProjectCode;//项目编码
                            tzdrawing.ProjectName = model.ProjectName;//项目名称
                            tzdrawing.ApprovalNo = model.ApprovalNo;//批复号
                            tzdrawing.Nature = model.Nature;//项目性质
                            tzdrawing.NatureName = model.NatureName;//项目性质名称
                            tzdrawing.ApplyTime = model.ApplyTime;//项目提出日期
                            tzdrawing.StationId = model.StationId;//站库id
                            tzdrawing.StationCodeXt = model.StationCodeXt;//站库协同编码
                            tzdrawing.StationName = model.StationName;//站库名称
                            tzdrawing.CompanyId = model.CompanyId;//地市公司id
                            tzdrawing.CompanyCodeXt = model.CompanyCodeXt;//地市公司协同编码
                            tzdrawing.CompanyName = model.CompanyName;//地市公司名称

                            AddTzConDrawing(tzdrawing, filesList);
                        }

                        result.Data = rows;
                        result.Flag = EResultFlag.Success;

                        #region 更新RPA数据
                        var hostAddress = System.Configuration.ConfigurationManager.AppSettings.Get("RPAPath");
                        var rpaModel = new RPA_TzDesignScheme();
                        var omadsModel = new OMADS_TzDesignScheme();
                        #region rpaModel
                        rpaModel.ApplyTime = model.ApplyTime;
                        rpaModel.ApprovalNo = model.ApprovalNo;
                        rpaModel.CompanyName = model.CompanyName;
                        rpaModel.ConvenienceRoom = model.ConvenienceRoom;
                        rpaModel.DesignJob = model.DesignJob;
                        rpaModel.DesignUnit = model.DesignUnit;
                        var files = GetFilesByTZTable("Epm_TzDesignScheme", model.Id);

                        for (int i = 0; i < files.Data.Count; i++)
                        {
                            rpaModel.FilePath += hostAddress + "?path=" + files.Data[i].FilePath + "&fileName=" + files.Data[i].Name + ',';
                        }
                        rpaModel.NatureName = model.NatureName;
                        rpaModel.DesignUnitCharge = model.DesignUnitCharge;
                        rpaModel.Estimate = model.Estimate;
                        rpaModel.FileNumber = files.Data.Count;
                        rpaModel.FollowOperate = "";
                        rpaModel.GasWells = model.GasWells;
                        rpaModel.ProjectCode = model.ProjectCode;
                        rpaModel.ProjectName = model.ProjectName;
                        rpaModel.InviteTime = model.InviteTime;
                        rpaModel.IsSynchro = model.IsSynchro;
                        rpaModel.LandArea = model.LandArea;
                        rpaModel.MachineofGasStage = model.MachineofGasStage;
                        rpaModel.Remark = model.Remark;
                        rpaModel.MachineofOilStage = model.MachineofOilStage;
                        rpaModel.StationName = model.StationName;
                        rpaModel.OilTank = model.OilTank;
                        rpaModel.OtheInvestment = model.OtheInvestment;
                        rpaModel.OtherProject = model.OtherProject;
                        rpaModel.ProjectJob = model.ProjectJob;
                        rpaModel.ProjectManager = model.ProjectManager;
                        rpaModel.ReleaseInvestmentAmount = model.ReleaseInvestmentAmount;
                        rpaModel.Shelter = model.Shelter;
                        rpaModel.StandarName = model.StandarName;
                        rpaModel.State = model.State;

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
                        rpaModel.StationRoom = model.StationRoom;
                        rpaModel.status1 = 0;
                        rpaModel.status2 = 0;
                        rpaModel.status3 = 0;
                        rpaModel.TotalInvestment = model.TotalInvestment;
                        //SetCreateUser(rpaModel);
                        //SetCurrentUser(rpaModel);
                        rpaModel.OperateUserId = model.CreateUserId;
                        rpaModel.OperateUserName = model.CreateUserName;
                        rpaModel.OperateTime = DateTime.Now;
                        rpaModel.CreateUserId = model.CreateUserId;
                        rpaModel.CreateUserName = model.CreateUserName;
                        rpaModel.OperateTime = DateTime.Now;
                        #endregion
                        #region omadsModel
                        omadsModel.ApplyTime = model.ApplyTime;
                        omadsModel.ApprovalNo = model.ApprovalNo;
                        omadsModel.CompanyName = model.CompanyName;
                        omadsModel.ConvenienceRoom = model.ConvenienceRoom;
                        omadsModel.DesignJob = model.DesignJob;
                        omadsModel.DesignUnit = model.DesignUnit;
                        omadsModel.FilePath = rpaModel.FilePath;
                        omadsModel.NatureName = model.NatureName;
                        omadsModel.DesignUnitCharge = model.DesignUnitCharge;
                        omadsModel.Estimate = model.Estimate;
                        omadsModel.FileNumber = files.Data.Count;
                        omadsModel.FollowOperate = "";
                        omadsModel.GasWells = model.GasWells;
                        omadsModel.ProjectCode = model.ProjectCode;
                        omadsModel.ProjectName = model.ProjectName;
                        omadsModel.InviteTime = model.InviteTime;
                        omadsModel.IsSynchro = model.IsSynchro;
                        omadsModel.LandArea = model.LandArea;
                        omadsModel.MachineofGasStage = model.MachineofGasStage;
                        omadsModel.Remark = model.Remark;
                        omadsModel.MachineofOilStage = model.MachineofOilStage;
                        omadsModel.StationName = model.StationName;
                        omadsModel.OilTank = model.OilTank;
                        omadsModel.OtheInvestment = model.OtheInvestment;
                        omadsModel.OtherProject = model.OtherProject;
                        omadsModel.ProjectJob = model.ProjectJob;
                        omadsModel.ProjectManager = model.ProjectManager;
                        omadsModel.ReleaseInvestmentAmount = model.ReleaseInvestmentAmount;
                        omadsModel.Shelter = model.Shelter;
                        omadsModel.StandarName = model.StandarName;
                        omadsModel.State = model.State;
                        omadsModel.username = rpaModel.username;

                        omadsModel.WriteMark = 0;
                        omadsModel.WriteResult = 0;
                        omadsModel.StationRoom = model.StationRoom;
                        omadsModel.status1 = 0;
                        omadsModel.status2 = 0;
                        omadsModel.status3 = 0;
                        omadsModel.TotalInvestment = model.TotalInvestment;
                        //SetCreateUser(rpaModel);
                        //SetCurrentUser(rpaModel);
                        omadsModel.OperateUserId = model.CreateUserId;
                        omadsModel.OperateUserName = model.CreateUserName;
                        omadsModel.OperateTime = DateTime.Now;
                        omadsModel.CreateUserId = model.CreateUserId;
                        omadsModel.CreateUserName = model.CreateUserName;
                        omadsModel.OperateTime = DateTime.Now;
                        #endregion
                        DataOperateBusiness<RPA_TzDesignScheme>.Get().Add(rpaModel);
                        DataOperateBusiness<OMADS_TzDesignScheme>.Get().Add(omadsModel);
                        #endregion
                    }
                    else
                    {
                        throw new Exception("该设计方案信息不存在");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzDesignSchemeState");
            }
            return result;
        }
    }
}
