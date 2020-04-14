using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.epm.ViewModel;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Service.ClientSite
{
    /// <summary>
    /// 陕西省各竞争对手加油（气）站现状上报流程
    /// </summary>
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzRivalStationReport(Epm_TzRivalStationReport model)
        {
            Result<int> result = new Result<int>();
            try
            {
                model = SetCurrentUser(model);
                model = SetCreateUser(model);

                #region 陕西省各竞争对手加油（气）站现状上报流程
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)XtBusinessDataState.Auditing && XtWorkFlow == "1")
                {
                    TzRivalStationReportView view = new TzRivalStationReportView();

                    if (model.ApplyUserId != null)
                    {
                        var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.ApplyUserId.Value);
                        if (baseUser == null)
                        {
                            throw new Exception("未找到申请人相关信息！");
                        }
                        view.hr_sbr = baseUser.ObjeId;
                    }
                    view.data_sbrq = string.Format("{0:yyyy-MM-dd}", DateTime.Now);

                    if (model.CompanyId != null)
                    {
                        var company = DataOperateBasic<Base_Company>.Get().GetModel(model.CompanyId.Value);
                        if (company == null)
                        {
                            throw new Exception("未获取到申请单位信息！");
                        }
                        view.sub_sbdw = company.ObjeId;
                    }
                    else
                    {
                        throw new Exception("请选择申请单位！");
                    }
                    if (model.DepartmentId != null)
                    {
                        var company = DataOperateBasic<Base_Company>.Get().GetModel(model.DepartmentId.Value);
                        if (company == null)
                        {
                            throw new Exception("未获取到申请部门信息！");
                        }
                        view.dept_sbbm = company.ObjeId;
                    }
                    else
                    {
                        throw new Exception("请选择申请部门！");
                    }

                    view.data_tjjzrq = string.Format("{0:yyyy-MM-dd}", model.TotalEndDate);
                    view.sel_ds = model.CityCode;
                    view.int_jyzzs = model.OilStationSum.ToString();
                    view.int_qsyyjyz = model.OilStaNum.ToString();
                    view.int_qsyycng = model.GasStaNumCNG.ToString();
                    view.int_qsyylng = model.GasStaNumLNG.ToString();
                    view.int_zsyzs = model.ZsyStaSum.ToString();
                    view.int_zsyjyzyys = model.ZsyOilStaNum.ToString();
                    view.int_zsycng = model.ZsyGasStaNumCNG.ToString();
                    view.int_zsylng = model.ZsyGasStaNumLNG.ToString();
                    view.int_zshzs = model.ZshStaSum.ToString();
                    view.int_zshjyzyys = model.ZshOilStaNum.ToString();
                    view.int_zshcng = model.ZshGasStaNumCNG.ToString();
                    view.int_zshlng = model.ZshGasStaNumLNG.ToString();
                    view.int_ycqpzs = model.YcqpStaSum.ToString();
                    view.int_ycqpjjzyys = model.YcqpOilStaNum.ToString();
                    view.int_ycqpcng = model.YcqpGasStaNumCNG.ToString();
                    view.int_ycqplng = model.YcqpGasStaNumLNG.ToString();
                    view.int_ycsyzs = model.YcsyStaSum.ToString();
                    view.int_ycsyjyzyys = model.YcsyOilStaNum.ToString();
                    view.int_ycsycng = model.YcsyGasStaNumCNG.ToString();
                    view.int_ycsylng = model.YcsyGasStaNumLNG.ToString();
                    view.int_shzzs = model.ShzStaSum.ToString();
                    view.int_shzjzyyys = model.ShzOilStaNum.ToString();
                    view.int_shzcng = model.ShzGasStaNumCNG.ToString();
                    view.int_shzlng = model.ShzGasStaNumLNG.ToString();
                    view.txt_bz = model.Remark.ToString();
                    if (model.DepLeaderId != null)
                    {
                        var baseUserHeaderID = DataOperateBasic<Base_User>.Get().GetModel(model.DepLeaderId.Value);
                        if (baseUserHeaderID == null)
                        {
                            throw new Exception("未找到部门负责人相关信息！");
                        }
                        view.hr_bmfzr = baseUserHeaderID.ObjeId;
                    }
                    if (model.LeaderId != null)
                    {
                        var baseUserLeaderID = DataOperateBasic<Base_User>.Get().GetModel(model.LeaderId.Value);
                        if (baseUserLeaderID == null)
                        {
                            throw new Exception("未找到分管领导相关信息！");
                        }
                        view.hr_fgld = baseUserLeaderID.ObjeId;
                    }

                    model.WorkFlowId = XtWorkFlowService.CreateTzRivalReportWorkFlow(view);
                }
                #endregion

                var rows = DataOperateBusiness<Epm_TzRivalStationReport>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzRivalStationReport.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzRivalStationReport");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzRivalStationReport(Epm_TzRivalStationReport model)
        {
            Result<int> result = new Result<int>();
            try
            {
                #region 陕西省各竞争对手加油（气）站现状上报流程
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)XtBusinessDataState.Auditing && XtWorkFlow == "1")
                {
                    TzRivalStationReportView view = new TzRivalStationReportView();

                    if (model.ApplyUserId != null)
                    {
                        var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.ApplyUserId.Value);
                        if (baseUser == null)
                        {
                            throw new Exception("未找到申请人相关信息！");
                        }
                        view.hr_sbr = baseUser.ObjeId;
                    }
                    view.data_sbrq = string.Format("{0:yyyy-MM-dd}", DateTime.Now);

                    if (model.CompanyId != null)
                    {
                        var company = DataOperateBasic<Base_Company>.Get().GetModel(model.CompanyId.Value);
                        if (company == null)
                        {
                            throw new Exception("未获取到申请单位信息！");
                        }
                        view.sub_sbdw = company.ObjeId;
                    }
                    else
                    {
                        throw new Exception("请选择申请单位！");
                    }
                    if (model.DepartmentId != null)
                    {
                        var company = DataOperateBasic<Base_Company>.Get().GetModel(model.DepartmentId.Value);
                        if (company == null)
                        {
                            throw new Exception("未获取到申请部门信息！");
                        }
                        view.dept_sbbm = company.ObjeId;
                    }
                    else
                    {
                        throw new Exception("请选择申请部门！");
                    }

                    view.data_tjjzrq = string.Format("{0:yyyy-MM-dd}", model.TotalEndDate);
                    view.sel_ds = model.CityCode;
                    view.int_jyzzs = model.OilStationSum.ToString();
                    view.int_qsyyjyz = model.OilStaNum.ToString();
                    view.int_qsyycng = model.GasStaNumCNG.ToString();
                    view.int_qsyylng = model.GasStaNumLNG.ToString();
                    view.int_zsyzs = model.ZsyStaSum.ToString();
                    view.int_zsyjyzyys = model.ZsyOilStaNum.ToString();
                    view.int_zsycng = model.ZsyGasStaNumCNG.ToString();
                    view.int_zsylng = model.ZsyGasStaNumLNG.ToString();
                    view.int_zshzs = model.ZshStaSum.ToString();
                    view.int_zshjyzyys = model.ZshOilStaNum.ToString();
                    view.int_zshcng = model.ZshGasStaNumCNG.ToString();
                    view.int_zshlng = model.ZshGasStaNumLNG.ToString();
                    view.int_ycqpzs = model.YcqpStaSum.ToString();
                    view.int_ycqpjjzyys = model.YcqpOilStaNum.ToString();
                    view.int_ycqpcng = model.YcqpGasStaNumCNG.ToString();
                    view.int_ycqplng = model.YcqpGasStaNumLNG.ToString();
                    view.int_ycsyzs = model.YcsyStaSum.ToString();
                    view.int_ycsyjyzyys = model.YcsyOilStaNum.ToString();
                    view.int_ycsycng = model.YcsyGasStaNumCNG.ToString();
                    view.int_ycsylng = model.YcsyGasStaNumLNG.ToString();
                    view.int_shzzs = model.ShzStaSum.ToString();
                    view.int_shzjzyyys = model.ShzOilStaNum.ToString();
                    view.int_shzcng = model.ShzGasStaNumCNG.ToString();
                    view.int_shzlng = model.ShzGasStaNumLNG.ToString();
                    view.txt_bz = model.Remark.ToString();
                    if (model.DepLeaderId != null)
                    {
                        var baseUserHeaderID = DataOperateBasic<Base_User>.Get().GetModel(model.DepLeaderId.Value);
                        if (baseUserHeaderID == null)
                        {
                            throw new Exception("未找到部门负责人相关信息！");
                        }
                        view.hr_bmfzr = baseUserHeaderID.ObjeId;
                    }
                    if (model.LeaderId != null)
                    {
                        var baseUserLeaderID = DataOperateBasic<Base_User>.Get().GetModel(model.LeaderId.Value);
                        if (baseUserLeaderID == null)
                        {
                            throw new Exception("未找到分管领导相关信息！");
                        }
                        view.hr_fgld = baseUserLeaderID.ObjeId;
                    }

                    model.WorkFlowId = XtWorkFlowService.CreateTzRivalReportWorkFlow(view);
                }
                #endregion

                var rows = DataOperateBusiness<Epm_TzRivalStationReport>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzRivalStationReport.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzRivalStationReport");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzRivalStationReportByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzRivalStationReport>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzRivalStationReport>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzRivalStationReport.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzRivalStationReportByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzRivalStationReport>> GetTzRivalStationReportList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzRivalStationReport>> result = new Result<List<Epm_TzRivalStationReport>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzRivalStationReport>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzRivalStationReportList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzRivalStationReport> GetTzRivalStationReportModel(long id)
        {
            Result<Epm_TzRivalStationReport> result = new Result<Epm_TzRivalStationReport>();
            try
            {
                var model = DataOperateBusiness<Epm_TzRivalStationReport>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzRivalStationReportModel");
            }
            return result;
        }

    }
}
