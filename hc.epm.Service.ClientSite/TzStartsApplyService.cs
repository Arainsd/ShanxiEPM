using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using Newtonsoft.Json;
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
        public Result<int> AddTzStartsApply(Epm_TzStartsApply model)
        {
            Result<int> result = new Result<int>();
            try
            {
                SetCreateUser(model);
                SetCurrentUser(model);

                //上传附件
                if (model.TzAttachs != null && model.TzAttachs.Any())
                {
                    AddFilesBytzTable(model, model.TzAttachs);
                }

                #region  开工报告流程申请
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)XtBusinessDataState.Auditing && XtWorkFlow == "1")
                {
                    TzStartsApplyApprovalView view = new TzStartsApplyApprovalView();
                    var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.ApplicantID.Value);
                    if (baseUser == null)
                    {
                        throw new Exception("未找到申请人相关信息！");
                    }

                    if (model.UnitID != null)
                    {
                        var company = DataOperateBasic<Base_Company>.Get().GetModel(model.UnitID.Value);
                        if (company == null)
                        {
                            throw new Exception("未获取到申请单位信息！");
                        }
                        view.sub_sqdw = company.ObjeId;
                    }
                    else
                    {
                        throw new Exception("请选择申请单位！");
                    }
                    view.hr_sqr = baseUser.ObjeId;
                    view.date_sqrq = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
                    view.txt_lxdh = model.PhoneNumber;
                    view.txt_jsxmmc = model.ProjectName;
                    view.txt_sjgm = model.DesignAbility;
                    view.txt_xmjyswh = model.ApprovalNumber;
                    view.float_gstz_js = model.BudgetInvestment1.ToString();
                    view.txt_kybgwh = model.EstimateInvestment;
                    view.float_gstz_ky = model.BudgetInvestment2.ToString();
                    view.txt_cbsjwh = model.DesignApprovalNumber;
                    view.float_cbsjtz = model.ReplyInvestment.ToString();
                    view.select_zjly = model.MoneySourceName == "投资" ? "0" : model.MoneySourceName == "费用" ? "1" : "";
                    view.txt_jhtzqk = model.ThatYearPlanInvestment.ToString();
                    view.date_jsgq_ks = string.Format("{0:yyyy-MM-dd}", model.StartTime);
                    view.date_jsgq_js = string.Format("{0:yyyy-MM-dd}", model.EndTime);
                    view.txts_gknr = model.ProjectOverview;
                    view.txts_xmgljg = model.TheStaff;
                    view.txts_ztbs = model.PrepareTheSituation;
                    view.txts_sjdwtz = model.CarryOutTheSituation;
                    view.txts_sgdwls = model.InCase;
                    view.txts_jldwls = model.EngineerCarryOutTheSituation;
                    view.txts_sgqqzb = model.PrepareSituation;
                    view.txts_yssb = model.AOGSituation;
                    view.txts_hjyx = model.Procedures;
                    view.txts_gcxxjd = model.Plan;

                    if (model.HeaderID != null)
                    {
                        var baseUserHeaderID = DataOperateBasic<Base_User>.Get().GetModel(model.HeaderID.Value);
                        if (baseUserHeaderID == null)
                        {
                            throw new Exception("未找到工程建设部门负责人相关信息！");
                        }
                        view.hr_bmfzr = baseUserHeaderID.ObjeId;
                    }
                    if (model.LeaderID != null)
                    {
                        var baseUserLeaderID = DataOperateBasic<Base_User>.Get().GetModel(model.LeaderID.Value);
                        if (baseUserLeaderID == null)
                        {
                            throw new Exception("未找到分公司工程建主管领导相关信息！");
                        }
                        view.hr_fgld = baseUserLeaderID.ObjeId;
                    }
                    if (model.DepartmentID != null)
                    {
                        var company = DataOperateBasic<Base_Company>.Get().GetModel(model.DepartmentID.Value);
                        if (company == null)
                        {
                            throw new Exception("未获取到申请部门信息！");
                        }
                        view.dept_sqbm = company.ObjeId;
                    }
                    else
                    {
                        throw new Exception("请选择申请部门！");
                    }

                    view.txt_sgdw = model.ConstructionUnit;
                    view.txt_xmjl = model.ProjectManager;
                    view.txt_aqkscjsg = model.ConstructionGrade.ToString();
                    view.txt_jldw = model.SupervisionUnit;
                    view.txt_jlgcs = model.SupervisoryEngineer;
                    view.txt_aqkscjjl = model.EngineerGrade.ToString();
                    view.int_jhjsgq = model.TimeLimit.ToString();

                    //上传附件
                    if (model.TzAttachs != null && model.TzAttachs.Any())
                    {
                        string baseFaleUrl = System.Configuration.ConfigurationManager.AppSettings.Get("XtDownloadUrl");
                        foreach (var item in model.TzAttachs)
                        {
                            string fileUrl = string.Format("{0}?fileId={1}&type={2}", baseFaleUrl, item.Id, item.TypeNo);
                            switch (item.TypeNo)
                            {
                                case "XMGLJGWJ":        // 项目管理机构(项目经理部或油库项目组)设立的文件、机构组成和职责分工各一份
                                    view.file_xmgljg = fileUrl + '|' + view.file_xmgljg;
                                    break;
                                case "LXPFWJ":        // 立项批复或项目初步设计批复文件复印件一份
                                    view.file_lxpf = fileUrl + '|' + view.file_lxpf;
                                    break;
                                case "SGSJWJ":        // 经审批的施工组织设计或工程建设总体部署一份
                                    view.file_sgzzsj = fileUrl + '|' + view.file_sgzzsj;
                                    break;
                                case "AQJYZWJ":        // 施工进场人员名单及《安全教育合格证》(复印件)
                                    view.file_sgjcry = fileUrl + '|' + view.file_sgjcry;
                                    break;
                                case "GYSWJ":        // 分公司与供应厂商确定的主要设备材料交付时间表一份
                                    view.file_sbcljf = fileUrl + '|' + view.file_sbcljf;
                                    break;
                                case "HSEWJ":        // 工程建设项目，还应提供审查通过后的HSE作业指导书、HSE作业计划书和HSE现场检查表
                                    view.file_zyzds = fileUrl + '|' + view.file_zyzds;
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (view.file_xmgljg != null)
                        {
                            view.file_xmgljg = view.file_xmgljg.Substring(0, view.file_xmgljg.Length - 1);
                        }
                        if (view.file_lxpf != null)
                        {
                            view.file_lxpf = view.file_lxpf.Substring(0, view.file_lxpf.Length - 1);
                        }
                        if (view.file_sgzzsj != null)
                        {
                            view.file_sgzzsj = view.file_sgzzsj.Substring(0, view.file_sgzzsj.Length - 1);
                        }
                        if (view.file_sgjcry != null)
                        {
                            view.file_sgjcry = view.file_sgjcry.Substring(0, view.file_sgjcry.Length - 1);
                        }
                        if (view.file_sbcljf != null)
                        {
                            view.file_sbcljf = view.file_sbcljf.Substring(0, view.file_sbcljf.Length - 1);
                        }
                        if (view.file_zyzds != null)
                        {
                            view.file_zyzds = view.file_zyzds.Substring(0, view.file_zyzds.Length - 1);
                        }
                    }

                    model.WorkFlowId = XtWorkFlowService.CreateStartsApplyWorkFlow(view);
                }
                #endregion

                var rows = DataOperateBusiness<Epm_TzStartsApply>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzStartsApply.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzStartsApply");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzStartsApply(Epm_TzStartsApply model)
        {
            Result<int> result = new Result<int>();
            try
            {
                //上传附件
                if (model.TzAttachs != null && model.TzAttachs.Any())
                {
                    AddFilesBytzTable(model, model.TzAttachs);
                }

                #region  开工报告流程申请
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)XtBusinessDataState.Auditing && XtWorkFlow == "1")
                {
                    TzStartsApplyApprovalView view = new TzStartsApplyApprovalView();
                    var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.ApplicantID.Value);
                    if (baseUser == null)
                    {
                        throw new Exception("未找到申请人相关信息！");
                    }

                    if (model.UnitID != null)
                    {
                        var company = DataOperateBasic<Base_Company>.Get().GetModel(model.UnitID.Value);
                        if (company == null)
                        {
                            throw new Exception("未获取到申请单位信息！");
                        }
                        view.sub_sqdw = company.ObjeId;
                    }
                    else
                    {
                        throw new Exception("请选择申请单位！");
                    }
                    view.hr_sqr = baseUser.ObjeId;
                    view.date_sqrq = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
                    view.txt_lxdh = model.PhoneNumber;
                    view.txt_jsxmmc = model.ProjectName;
                    view.txt_sjgm = model.DesignAbility;
                    view.txt_xmjyswh = model.ApprovalNumber;
                    view.float_gstz_js = model.BudgetInvestment1.ToString();
                    view.txt_kybgwh = model.EstimateInvestment;
                    view.float_gstz_ky = model.BudgetInvestment2.ToString();
                    view.txt_cbsjwh = model.DesignApprovalNumber;
                    view.float_cbsjtz = model.ReplyInvestment.ToString();
                    view.select_zjly = model.MoneySourceName == "投资" ? "0" : model.MoneySourceName == "费用" ? "1" : "";
                    view.txt_jhtzqk = model.ThatYearPlanInvestment.ToString();
                    view.date_jsgq_ks = string.Format("{0:yyyy-MM-dd}", model.StartTime);
                    view.date_jsgq_js = string.Format("{0:yyyy-MM-dd}", model.EndTime);
                    view.txts_gknr = model.ProjectOverview;
                    view.txts_xmgljg = model.TheStaff;
                    view.txts_ztbs = model.PrepareTheSituation;
                    view.txts_sjdwtz = model.CarryOutTheSituation;
                    view.txts_sgdwls = model.InCase;
                    view.txts_jldwls = model.EngineerCarryOutTheSituation;
                    view.txts_sgqqzb = model.PrepareSituation;
                    view.txts_yssb = model.AOGSituation;
                    view.txts_hjyx = model.Procedures;
                    view.txts_gcxxjd = model.PlanHtml;

                    if (model.HeaderID != null)
                    {
                        var baseUserHeaderID = DataOperateBasic<Base_User>.Get().GetModel(model.HeaderID.Value);
                        if (baseUserHeaderID == null)
                        {
                            throw new Exception("未找到工程建设部门负责人相关信息！");
                        }
                        view.hr_bmfzr = baseUserHeaderID.ObjeId;
                    }
                    if (model.LeaderID != null)
                    {
                        var baseUserLeaderID = DataOperateBasic<Base_User>.Get().GetModel(model.LeaderID.Value);
                        if (baseUserLeaderID == null)
                        {
                            throw new Exception("未找到分公司工程建主管领导相关信息！");
                        }
                        view.hr_fgld = baseUserLeaderID.ObjeId;
                    }
                    if (model.DepartmentID != null)
                    {
                        var company = DataOperateBasic<Base_Company>.Get().GetModel(model.DepartmentID.Value);
                        if (company == null)
                        {
                            throw new Exception("未获取到申请部门信息！");
                        }
                        view.dept_sqbm = company.ObjeId;
                    }
                    else
                    {
                        throw new Exception("请选择申请部门！");
                    }

                    view.txt_sgdw = model.ConstructionUnit;
                    view.txt_xmjl = model.ProjectManager;
                    view.txt_aqkscjsg = model.ConstructionGrade.ToString();
                    view.txt_jldw = model.SupervisionUnit;
                    view.txt_jlgcs = model.SupervisoryEngineer;
                    view.txt_aqkscjjl = model.EngineerGrade.ToString();
                    view.int_jhjsgq = model.TimeLimit.ToString();

                    //上传附件
                    if (model.TzAttachs != null && model.TzAttachs.Any())
                    {
                        string baseFaleUrl = System.Configuration.ConfigurationManager.AppSettings.Get("XtDownloadUrl");
                        foreach (var item in model.TzAttachs)
                        {
                            string fileUrl = string.Format("{0}?fileId={1}&type={2}", baseFaleUrl, item.Id, item.TypeNo);
                            switch (item.TypeNo)
                            {
                                case "XMGLJGWJ":        // 项目管理机构(项目经理部或油库项目组)设立的文件、机构组成和职责分工各一份
                                    view.file_xmgljg = fileUrl + '|' + view.file_xmgljg;
                                    break;
                                case "LXPFWJ":        // 立项批复或项目初步设计批复文件复印件一份
                                    view.file_lxpf = fileUrl + '|' + view.file_lxpf;
                                    break;
                                case "SGSJWJ":        // 经审批的施工组织设计或工程建设总体部署一份
                                    view.file_sgzzsj = fileUrl + '|' + view.file_sgzzsj;
                                    break;
                                case "AQJYZWJ":        // 施工进场人员名单及《安全教育合格证》(复印件)
                                    view.file_sgjcry = fileUrl + '|' + view.file_sgjcry;
                                    break;
                                case "GYSWJ":        // 分公司与供应厂商确定的主要设备材料交付时间表一份
                                    view.file_sbcljf = fileUrl + '|' + view.file_sbcljf;
                                    break;
                                case "HSEWJ":        // 工程建设项目，还应提供审查通过后的HSE作业指导书、HSE作业计划书和HSE现场检查表
                                    view.file_zyzds = fileUrl + '|' + view.file_zyzds;
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (view.file_xmgljg != null)
                        {
                            view.file_xmgljg = view.file_xmgljg.Substring(0, view.file_xmgljg.Length - 1);
                        }
                        if (view.file_lxpf != null)
                        {
                            view.file_lxpf = view.file_lxpf.Substring(0, view.file_lxpf.Length - 1);
                        }
                        if (view.file_sgzzsj != null)
                        {
                            view.file_sgzzsj = view.file_sgzzsj.Substring(0, view.file_sgzzsj.Length - 1);
                        }
                        if (view.file_sgjcry != null)
                        {
                            view.file_sgjcry = view.file_sgjcry.Substring(0, view.file_sgjcry.Length - 1);
                        }
                        if (view.file_sbcljf != null)
                        {
                            view.file_sbcljf = view.file_sbcljf.Substring(0, view.file_sbcljf.Length - 1);
                        }
                        if (view.file_zyzds != null)
                        {
                            view.file_zyzds = view.file_zyzds.Substring(0, view.file_zyzds.Length - 1);
                        }
                    }
                    model.WorkFlowId = XtWorkFlowService.CreateStartsApplyWorkFlow(view);
                }
                #endregion

                SetCreateUser(model);
                SetCurrentUser(model);

                var rows = DataOperateBusiness<Epm_TzStartsApply>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzStartsApply.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzStartsApply");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzStartsApplyByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzStartsApply>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzStartsApply>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzStartsApply.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzStartsApplyByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzStartsApply>> GetTzStartsApplyList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzStartsApply>> result = new Result<List<Epm_TzStartsApply>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzStartsApply>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzStartsApplyList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzStartsApply> GetTzStartsApplyModel(long id)
        {
            Result<Epm_TzStartsApply> result = new Result<Epm_TzStartsApply>();
            try
            {
                var model = DataOperateBusiness<Epm_TzStartsApply>.Get().GetModel(id);
                if (model != null)
                {
                    model.TzAttachs = GetFilesByTZTable("Epm_TzStartsApply", id).Data;
                }
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzStartsApplyModel");
            }
            return result;
        }

    }
}
