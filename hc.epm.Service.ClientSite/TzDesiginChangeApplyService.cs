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
        public Result<int> AddTzDesiginChangeApply(Epm_TzDesiginChangeApply model)
        {
            Result<int> result = new Result<int>();
            try
            {
                #region 建设工程设计变更申请流程
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)PreProjectState.ApprovalSuccess && XtWorkFlow == "1")
                {
                    TzDesiginChangeApplyApprovalView view = new TzDesiginChangeApplyApprovalView();

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
                    view.txt_gcmc = model.ProjectName;
                    view.int_htzj = model.InvestmentCost.ToString();
                    view.int_bgzj = model.ChangeCost.ToString();
                    //view.sub_jswd = model.ConstructionUnit;

                    if (model.ConstructionUnitID != null)
                    {
                        var company = DataOperateBasic<Base_Company>.Get().GetModel(model.ConstructionUnitID.Value);
                        if (company == null)
                        {
                            throw new Exception("未获取到申请单位信息！");
                        }
                        view.sub_jswd = company.ObjeId;
                    }
                    else
                    {
                        throw new Exception("请选择申请单位！");
                    }

                    if (model.DepartmentID != null)
                    {
                        var company = DataOperateBasic<Base_Company>.Get().GetModel(model.DepartmentID.Value);
                        if (company == null)
                        {
                            throw new Exception("未获取到申请部门信息！");
                        }
                        view.dep_sqmb = company.ObjeId;
                    }
                    else
                    {
                        throw new Exception("请选择申请部门！");
                    }

                    view.txt_lxdh = model.ConstructionUnitPeople;
                    view.txt_sgdw = model.WorkUnit;
                    view.txt_sgdwlxdh = model.WorkUnitPeople;
                    view.txt_jldw = model.SupervisionUnit;
                    view.txt_jldwlxf = model.SupervisionUnitPeople;
                    view.txt_sjdw = model.DesignUnit;
                    view.txt_sjdwlxdh = model.DesignUnitPeople;
                    view.txt_bgdyy = model.ChangeCause;
                    view.txt_bgdnr = model.ChangeContent;
                    view.txt_gcljgc = model.ProjectChange;
                    view.txt_bgdgq = model.Impact;

                    if (model.HeaderID != null)
                    {
                        var baseUserHeaderID = DataOperateBasic<Base_User>.Get().GetModel(model.HeaderID.Value);
                        if (baseUserHeaderID == null)
                        {
                            throw new Exception("未找到部门负责人相关信息！");
                        }
                        view.hr_bmfzr = baseUserHeaderID.ObjeId;
                    }
                    if (model.LeaderID != null)
                    {
                        var baseUserLeaderID = DataOperateBasic<Base_User>.Get().GetModel(model.LeaderID.Value);
                        if (baseUserLeaderID == null)
                        {
                            throw new Exception("未找到分管领导相关信息！");
                        }
                        view.hr_fgld = baseUserLeaderID.ObjeId;
                    }
                    //附件
                    if (model.TzAttachs != null && model.TzAttachs.Any())
                    {
                        //string baseFaleUrl = System.Configuration.ConfigurationManager.AppSettings.Get("XtDownloadUrl");
                        //foreach (var item in model.TzAttachs)
                        //{
                        //    string fileUrl = string.Format("{0}?fileId={1}&type={2}", baseFaleUrl, item.Id, item.TypeNo);
                        //    switch (item.TypeNo)
                        //    {
                        //        case "JSSQBG":
                        //            view.fj = fileUrl + '|' + view.fj; ;
                        //            break;
                        //        default:
                        //            break;
                        //    }
                        //}
                        //if (view.fj != null)
                        //{
                        //    view.fj = view.fj.Substring(view.fj.Length - 1);
                        //}
                        view.fj = XtWorkFlowSubmitService.CreateXtAttachPath(model.TzAttachs);
                    }

                    model.WorkFlowId = XtWorkFlowService.CreateTzChangeApplyWorkFlow(view);
                }
                #endregion

                //上传附件
                if (model.TzAttachs != null && model.TzAttachs.Any())
                {
                    AddFilesBytzTable(model, model.TzAttachs);
                }
                SetCreateUser(model);
                SetCurrentUser(model);
                model.ApprovalNameId = model.CreateUserId;
                model.ApprovalName = model.CreateUserName;
                var rows = DataOperateBusiness<Epm_TzDesiginChangeApply>.Get().Add(model);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzDesiginChangeApply.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzDesiginChangeApply");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzDesiginChangeApply(Epm_TzDesiginChangeApply model)
        {
            Result<int> result = new Result<int>();
            try
            {
                #region 建设工程设计变更申请流程
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)PreProjectState.ApprovalSuccess && XtWorkFlow == "1")
                {
                    TzDesiginChangeApplyApprovalView view = new TzDesiginChangeApplyApprovalView();

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
                    view.txt_gcmc = model.ProjectName;
                    view.int_htzj = model.InvestmentCost.ToString();
                    view.int_bgzj = model.ChangeCost.ToString();
                    //view.sub_jswd = model.ConstructionUnit;

                    if (model.ConstructionUnitID != null)
                    {
                        var company = DataOperateBasic<Base_Company>.Get().GetModel(model.ConstructionUnitID.Value);
                        if (company == null)
                        {
                            throw new Exception("未获取到申请单位信息！");
                        }
                        view.sub_jswd = company.ObjeId;
                    }
                    else
                    {
                        throw new Exception("请选择申请单位！");
                    }

                    if (model.DepartmentID != null)
                    {
                        var company = DataOperateBasic<Base_Company>.Get().GetModel(model.DepartmentID.Value);
                        if (company == null)
                        {
                            throw new Exception("未获取到申请部门信息！");
                        }
                        view.dep_sqmb = company.ObjeId;
                    }
                    else
                    {
                        throw new Exception("请选择申请部门！");
                    }

                    view.txt_lxdh = model.ConstructionUnitPeople;
                    view.txt_sgdw = model.WorkUnit;
                    view.txt_sgdwlxdh = model.WorkUnitPeople;
                    view.txt_jldw = model.SupervisionUnit;
                    view.txt_jldwlxf = model.SupervisionUnitPeople;
                    view.txt_sjdw = model.DesignUnit;
                    view.txt_sjdwlxdh = model.DesignUnitPeople;
                    view.txt_bgdyy = model.ChangeCause;
                    view.txt_bgdnr = model.ChangeContent;
                    view.txt_gcljgc = model.ProjectChange;
                    view.txt_bgdgq = model.Impact;

                    if (model.HeaderID != null)
                    {
                        var baseUserHeaderID = DataOperateBasic<Base_User>.Get().GetModel(model.HeaderID.Value);
                        if (baseUserHeaderID == null)
                        {
                            throw new Exception("未找到部门负责人相关信息！");
                        }
                        view.hr_bmfzr = baseUserHeaderID.ObjeId;
                    }
                    if (model.LeaderID != null)
                    {
                        var baseUserLeaderID = DataOperateBasic<Base_User>.Get().GetModel(model.LeaderID.Value);
                        if (baseUserLeaderID == null)
                        {
                            throw new Exception("未找到分管领导相关信息！");
                        }
                        view.hr_fgld = baseUserLeaderID.ObjeId;
                    }
                    //附件
                    if (model.TzAttachs != null && model.TzAttachs.Any())
                    {
                        //string baseFaleUrl = System.Configuration.ConfigurationManager.AppSettings.Get("XtDownloadUrl");
                        //foreach (var item in model.TzAttachs)
                        //{
                        //    string fileUrl = string.Format("{0}?fileId={1}&type={2}", baseFaleUrl, item.Id, item.TypeNo);
                        //    switch (item.TypeNo)
                        //    {
                        //        case "JSSQBG":
                        //            view.fj = fileUrl + '|' + view.fj;
                        //            break;
                        //        default:
                        //            break;
                        //    }
                        //}
                        //if (view.fj != null)
                        //{
                        //    view.fj = view.fj.Substring(view.fj.Length - 1);
                        //}
                        view.fj = XtWorkFlowSubmitService.CreateXtAttachPath(model.TzAttachs);
                    }

                    model.WorkFlowId = XtWorkFlowService.CreateTzChangeApplyWorkFlow(view);
                }
                #endregion

                //上传附件
                if (model.TzAttachs != null && model.TzAttachs.Any())
                {
                    AddFilesBytzTable(model, model.TzAttachs);
                }

                SetCreateUser(model);
                SetCurrentUser(model);
                model.ApprovalNameId = model.CreateUserId;
                model.ApprovalName = model.CreateUserName;
                var rows = DataOperateBusiness<Epm_TzDesiginChangeApply>.Get().Update(model);
                
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzDesiginChangeApply.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzDesiginChangeApply");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzDesiginChangeApplyByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzDesiginChangeApply>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzDesiginChangeApply>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzDesiginChangeApply.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzDesiginChangeApplyByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzDesiginChangeApply>> GetTzDesiginChangeApplyList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzDesiginChangeApply>> result = new Result<List<Epm_TzDesiginChangeApply>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzDesiginChangeApply>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzDesiginChangeApplyList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzDesiginChangeApply> GetTzDesiginChangeApplyModel(long id)
        {
            Result<Epm_TzDesiginChangeApply> result = new Result<Epm_TzDesiginChangeApply>();
            try
            {
                var model = DataOperateBusiness<Epm_TzDesiginChangeApply>.Get().GetModel(id);

                if (model != null)
                {
                    model.TzAttachs = GetFilesByTZTable("Epm_TzDesiginChangeApply", id).Data;
                }
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzDesiginChangeApplyModel");
            }
            return result;
        }

        /// <summary>
        /// 修改设计方案变更申请状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzDesiginChangeApplyState(List<long> ids, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                foreach (var item in ids)
                {
                    var model = DataOperateBusiness<Epm_TzDesiginChangeApply>.Get().GetModel(item);
                    if (model != null)
                    {
                        //SetCurrentUser(model);
                        model.State = (int)state.ToEnumReq<PreProjectState>();
                        var rows = DataOperateBusiness<Epm_TzDesiginChangeApply>.Get().Update(model);

                        result.Data = rows;
                        result.Flag = EResultFlag.Success;
                    }
                    else
                    {
                        throw new Exception("该设计方案变更申请信息不存在");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzDesiginChangeApplyState");
            }
            return result;
        }

    }
}
