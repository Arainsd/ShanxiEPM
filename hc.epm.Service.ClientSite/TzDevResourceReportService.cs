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
    /// 加油（气）站开发资源上报流程
    /// </summary>
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzDevResourceReport(Epm_TzDevResourceReport model)
        {
            Result<int> result = new Result<int>();
            try
            {
                model = SetCurrentUser(model);
                model = SetCreateUser(model);

                #region 加油（气）站开发资源上报流程
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)XtBusinessDataState.Auditing && XtWorkFlow == "1")
                {
                    TzDevResourceReportView view = new TzDevResourceReportView();

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
                        view.dept_sbdw = company.ObjeId;
                    }
                    else
                    {
                        throw new Exception("请选择申请单位！");
                    }

                    if (model.LeaderId != null)
                    {
                        var baseUserHeaderID = DataOperateBasic<Base_User>.Get().GetModel(model.LeaderId.Value);
                        if (baseUserHeaderID == null)
                        {
                            throw new Exception("未找到分管领导相关信息！");
                        }
                        view.hr_fglds = baseUserHeaderID.ObjeId;
                    }
                    if (model.DepLeaderId != null)
                    {
                        var baseUserLeaderID = DataOperateBasic<Base_User>.Get().GetModel(model.DepLeaderId.Value);
                        if (baseUserLeaderID == null)
                        {
                            throw new Exception("未找到部门负责人相关信息！");
                        }
                        view.hr_bfzr = baseUserLeaderID.ObjeId;
                    }

                    if (model.DepartmentId != null)
                    {
                        var company = DataOperateBasic<Base_Company>.Get().GetModel(model.DepartmentId.Value);
                        if (company == null)
                        {
                            throw new Exception("未获取到申请部门信息！");
                        }
                        view.dpt_bm = company.ObjeId;
                    }
                    else
                    {
                        throw new Exception("请选择申请部门！");
                    }
                    if (model.TzDevResourceReportItem != null && model.TzDevResourceReportItem.Any())
                    {
                        view.list = new List<TzDevResourceReportView.TzDevResourceItem>();
                        foreach (var temp in model.TzDevResourceReportItem)
                        {
                            TzDevResourceReportView.TzDevResourceItem itemview = new TzDevResourceReportView.TzDevResourceItem();
                            itemview.select_ds = temp.Cities;
                            itemview.txt_qx = temp.County;
                            itemview.txt_xmmc = temp.ProjectName;
                            itemview.txt_xmwz = temp.ProjectAdress;
                            itemview.select_xmxz = temp.ProjectType;
                            itemview.int_yjztz = temp.ProjectedInvestment.ToString();
                            itemview.int_kyxs = temp.ResearchSales.ToString();
                            itemview.int_qcb = temp.GasFuelRatio.ToString();
                            itemview.data_lwyxzsj = temp.FixHour.ToString();
                            itemview.data_jhlxsj = temp.PlanningTime.ToString();
                            itemview.txt_yzxm = temp.OwnerName;
                            itemview.txt_yzdh = temp.OwnerPhone;
                            itemview.txt_bz = temp.Remark;
                            view.list.Add(itemview);
                        }
                    }

                    model.WorkFlowId = XtWorkFlowService.CreateTzDevWorkFlow(view);
                }
                #endregion

                var rows = DataOperateBusiness<Epm_TzDevResourceReport>.Get().Add(model);

                if (model.TzDevResourceReportItem != null && model.TzDevResourceReportItem.Any())
                {
                    model.TzDevResourceReportItem.ForEach(item =>
                    {
                        item.ApplyId = model.Id;
                        item = SetCurrentUser(item);
                        item = SetCreateUser(item);
                    });
                    DataOperateBusiness<Epm_TzDevResourceReportItem>.Get().AddRange(model.TzDevResourceReportItem);
                }
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzDevResourceReport.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzDevResourceReport");
            }
            return result;
        }

        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzDevResourceReport(Epm_TzDevResourceReport model)
        {
            Result<int> result = new Result<int>();
            try
            {
                #region 加油（气）站开发资源上报流程
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)XtBusinessDataState.Auditing && XtWorkFlow == "1")
                {
                    TzDevResourceReportView view = new TzDevResourceReportView();

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
                        view.dept_sbdw = company.ObjeId;
                    }
                    else
                    {
                        throw new Exception("请选择申请单位！");
                    }

                    if (model.LeaderId != null)
                    {
                        var baseUserHeaderID = DataOperateBasic<Base_User>.Get().GetModel(model.LeaderId.Value);
                        if (baseUserHeaderID == null)
                        {
                            throw new Exception("未找到分管领导相关信息！");
                        }
                        view.hr_fglds = baseUserHeaderID.ObjeId;
                    }
                    if (model.DepLeaderId != null)
                    {
                        var baseUserLeaderID = DataOperateBasic<Base_User>.Get().GetModel(model.DepLeaderId.Value);
                        if (baseUserLeaderID == null)
                        {
                            throw new Exception("未找到部门负责人相关信息！");
                        }
                        view.hr_bfzr = baseUserLeaderID.ObjeId;
                    }

                    if (model.DepartmentId != null)
                    {
                        var company = DataOperateBasic<Base_Company>.Get().GetModel(model.DepartmentId.Value);
                        if (company == null)
                        {
                            throw new Exception("未获取到申请部门信息！");
                        }
                        view.dpt_bm = company.ObjeId;
                    }
                    else
                    {
                        throw new Exception("请选择申请部门！");
                    }
                    if (model.TzDevResourceReportItem != null && model.TzDevResourceReportItem.Any())
                    {
                        view.list = new List<TzDevResourceReportView.TzDevResourceItem>();
                        foreach (var temp in model.TzDevResourceReportItem)
                        {
                            TzDevResourceReportView.TzDevResourceItem itemview = new TzDevResourceReportView.TzDevResourceItem();
                            itemview.select_ds = temp.Cities;
                            itemview.txt_qx = temp.County;
                            itemview.txt_xmmc = temp.ProjectName;
                            itemview.txt_xmwz = temp.ProjectAdress;
                            itemview.select_xmxz = temp.ProjectType;
                            itemview.int_yjztz = temp.ProjectedInvestment.ToString();
                            itemview.int_kyxs = temp.ResearchSales.ToString();
                            itemview.int_qcb = temp.GasFuelRatio.ToString();
                            itemview.data_lwyxzsj = temp.FixHour.ToString();
                            itemview.data_jhlxsj = temp.PlanningTime.ToString();
                            itemview.txt_yzxm = temp.OwnerName;
                            itemview.txt_yzdh = temp.OwnerPhone;
                            itemview.txt_bz = temp.Remark;
                            view.list.Add(itemview);
                        }
                    }

                    model.WorkFlowId = XtWorkFlowService.CreateTzDevWorkFlow(view);
                }
                #endregion
                var rows = DataOperateBusiness<Epm_TzDevResourceReport>.Get().Update(model);

                if (model.TzDevResourceReportItem.Any())
                {
                    //先删除
                    var detaileList = DataOperateBusiness<Epm_TzDevResourceReportItem>.Get().GetList(p => p.ApplyId == model.Id);
                    if (detaileList.Any())
                    {
                        DataOperateBusiness<Epm_TzDevResourceReportItem>.Get().DeleteRange(detaileList);
                    }

                    model.TzDevResourceReportItem.ForEach(item =>
                    {
                        item.ApplyId = model.Id;
                        item = SetCurrentUser(item);
                        item = SetCreateUser(item);
                    });
                    DataOperateBusiness<Epm_TzDevResourceReportItem>.Get().AddRange(model.TzDevResourceReportItem);
                }

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzDevResourceReport.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzDevResourceReport");
            }
            return result;
        }

        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzDevResourceReportByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzDevResourceReport>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzDevResourceReport>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzDevResourceReport.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzDevResourceReportByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzDevResourceReport>> GetTzDevResourceReportList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzDevResourceReport>> result = new Result<List<Epm_TzDevResourceReport>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzDevResourceReport>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzDevResourceReportList");
            }
            return result;
        }

        public Result<List<TzDevResourceReportItemView>> GetTzDevResourceReportItemList(QueryCondition qc)
        {
            Result<List<TzDevResourceReportItemView>> result = new Result<List<TzDevResourceReportItemView>>();

            try
            {
                var query = from a in context.Epm_TzDevResourceReportItem.Where(p => !p.IsDelete)
                            join b in context.Epm_TzDevResourceReport.Where(p => !p.IsDelete) on a.ApplyId equals b.Id into bref
                            from b in bref.DefaultIfEmpty()
                            select new
                            {
                                a,
                                b,
                            };

                if (qc.ConditionList != null && qc.ConditionList.Any())
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
                                        query = query.Where(p => p.a.ProjectName.Contains(value));
                                        break;
                                    }
                                case "CompanyId":
                                    {
                                        long id = Convert.ToInt64(value);
                                        query = query.Where(p => p.b.CompanyId == id);
                                        break;
                                    }
                                case "startTime":
                                    {
                                        DateTime startTime;
                                        if (DateTime.TryParse(value, out startTime))
                                        {
                                            query = query.Where(p => p.b.ApplyDate >= startTime);
                                        }
                                        break;
                                    }
                                case "endTime":
                                    {
                                        DateTime endTime;
                                        if (DateTime.TryParse(value, out endTime))
                                        {
                                            query = query.Where(p => p.b.ApplyDate < endTime);
                                        }
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }

                int skip = (qc.PageInfo.CurrentPageIndex - 1) * qc.PageInfo.PageRowCount;
                int take = qc.PageInfo.PageRowCount;
                int total = query.Count();
                var list = query.OrderByDescending(p => p.b.ApplyDate).Skip(skip).Take(take).AsEnumerable().Select(p => new TzDevResourceReportItemView()
                {
                    Cities = p.a.Cities,
                    County = p.a.County,
                    ProjectName = p.a.ProjectName,
                    ProjectAdress = p.a.ProjectAdress,
                    ProjectTypeName = p.a.ProjectTypeName,
                    ProjectedInvestment = p.a.ProjectedInvestment,
                    ResearchSales = p.a.ResearchSales,
                    GasFuelRatio = p.a.GasFuelRatio,
                    FixHour = p.a.FixHour,
                    PlanningTime = p.a.PlanningTime,
                    OwnerName = p.a.OwnerName,
                    OwnerPhone = p.a.OwnerPhone,
                    Remark = p.a.Remark,
                    StateType = p.b.StateType,
                    StateName = p.b.StateName,
                    ApplyId = p.a.Id,
                    Id = p.b.Id,
                    ApprovalName = p.b.ApprovalName,
                    State = p.b.State,
                }).ToList();

                result.Data = list;
                result.AllRowsCount = total;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Exception = new ExceptionEx(ex, "GetTzDevResourceReportItemList");
                result.Flag = EResultFlag.Failure;
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzDevResourceReport> GetTzDevResourceReportModel(long id)
        {
            Result<Epm_TzDevResourceReport> result = new Result<Epm_TzDevResourceReport>();
            try
            {
                var model = DataOperateBusiness<Epm_TzDevResourceReport>.Get().GetModel(id);
                if (model != null)
                {
                    model.TzDevResourceReportItem = DataOperateBusiness<Epm_TzDevResourceReportItem>.Get().GetList(t => t.ApplyId == id).ToList();
                }
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzDevResourceReportModel");
            }
            return result;
        }

    }
}
