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
    /// 工程甲供物资订单
    /// </summary>
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzGcGoodsOrdersApply(Epm_TzGcGoodsOrdersApply model)
        {
            Result<int> result = new Result<int>();
            try
            {
                model = SetCurrentUser(model);
                model = SetCreateUser(model);

                #region 工程甲供物资订单
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)XtBusinessDataState.Auditing && XtWorkFlow == "1")
                {
                    GcGoodsOrdersApplyView view = new GcGoodsOrdersApplyView();

                    view.txt_xmtm = model.ContractName;

                    view.txt_erp = model.ErpOrderNumber;
                    view.txt_xmmc = model.ProjectName;
                    view.txt_xmmcx = model.ProjectName;
                    view.txt_sjr = model.Recipient;
                    view.txt_dh = model.Phone;
                    view.txt_lxr = model.Contact;
                    view.txt_lxrdh = model.ContactNumber;
                    view.select_wzzl = model.MaterialName;
                    view.select_wzzlx = model.MaterialName;
                    view.select_wzzln = model.MaterialName;

                    if (model.ApplicantId != null)
                    {
                        var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.ApplicantId.Value);
                        if (baseUser == null)
                        {
                            throw new Exception("未找到申请人相关信息！");
                        }
                        view.hr_sqr = baseUser.ObjeId;

                    }

                    if (model.CompanyId != null)
                    {
                        var company = DataOperateBasic<Base_Company>.Get().GetModel(model.CompanyId.Value);
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

                    if (model.DepartmentId != null)
                    {
                        var company = DataOperateBasic<Base_Company>.Get().GetModel(model.DepartmentId.Value);
                        if (company == null)
                        {
                            throw new Exception("未获取到申请部门信息！");
                        }
                        view.dep_sqbm = company.ObjeId;
                    }
                    else
                    {
                        throw new Exception("请选择申请部门！");
                    }

                    view.date_sqsj = string.Format("{0:yyyy-MM-dd}", DateTime.Now);

                    if (model.LeaderId != null)
                    {
                        var baseUserHeaderID = DataOperateBasic<Base_User>.Get().GetModel(model.LeaderId.Value);
                        if (baseUserHeaderID == null)
                        {
                            throw new Exception("未找到分管领导相关信息！");
                        }
                        view.hr_fgld = baseUserHeaderID.ObjeId;
                    }
                    if (model.DepLeaderId != null)
                    {
                        var baseUserLeaderID = DataOperateBasic<Base_User>.Get().GetModel(model.DepLeaderId.Value);
                        if (baseUserLeaderID == null)
                        {
                            throw new Exception("未找到部门负责人相关信息！");
                        }
                        view.hr_bmfzr = baseUserLeaderID.ObjeId;
                    }

                    view.txt = model.ApprovalNo;
                    view.date_dqn = DateTime.Now.Year.ToString();
                    view.txt_gysmc = model.SupplierName;
                    view.txt_gysmcx = model.SupplierName;
                    view.txt_gysmc_dy = model.SupplierName;
                    view.txt_gysdz = model.SupplierAdress;
                    view.txt_gysyb = model.SupplierCode;
                    view.txt_htbsxh = model.ContractNumber;

                    //明细表信息
                    if (model.TzGcGoodsOrdersItem != null && model.TzGcGoodsOrdersItem.Any())
                    {
                        view.list = new List<GcGoodsOrdersApplyView.GcGoodsOrdersItem>();
                        foreach (var temp in model.TzGcGoodsOrdersItem)
                        {
                            GcGoodsOrdersApplyView.GcGoodsOrdersItem itemview = new GcGoodsOrdersApplyView.GcGoodsOrdersItem();
                            itemview.dep_zm = temp.StationName;
                            itemview.float_dj = temp.UnitPrice.ToString();
                            itemview.float_je = temp.Amount.ToString();
                            itemview.int_mount = temp.Number.ToString();
                            itemview.txts_bz = temp.Note;
                            itemview.txt_dhdz = temp.Address;
                            itemview.txt_gg = temp.Specifications;
                            itemview.txt_pm = temp.ProductName;
                            itemview.txt_zm = temp.StationName;

                            view.list.Add(itemview);
                        }
                    }

                    model.WorkFlowId = XtWorkFlowService.CreateTzOrdersWorkFlow(view);
                }
                #endregion

                var rows = DataOperateBusiness<Epm_TzGcGoodsOrdersApply>.Get().Add(model);

                if (model.TzGcGoodsOrdersItem != null && model.TzGcGoodsOrdersItem.Any())
                {
                    model.TzGcGoodsOrdersItem.ForEach(item =>
                    {
                        item.ChangeApplyId = model.Id;
                        item = SetCurrentUser(item);
                        item = SetCreateUser(item);
                    });
                    DataOperateBusiness<Epm_TzGcGoodsOrdersItem>.Get().AddRange(model.TzGcGoodsOrdersItem);
                }

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.GcGoodsOrdersApply.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzGcGoodsOrdersApply");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzGcGoodsOrdersApply(Epm_TzGcGoodsOrdersApply model)
        {
            Result<int> result = new Result<int>();
            try
            {
                #region 工程甲供物资订单
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)XtBusinessDataState.Auditing && XtWorkFlow == "1")
                {
                    GcGoodsOrdersApplyView view = new GcGoodsOrdersApplyView();

                    view.txt_xmtm = model.ContractName;

                    view.txt_erp = model.ErpOrderNumber;
                    view.txt_xmmc = model.ProjectName;
                    view.txt_xmmcx = model.ProjectName;
                    view.txt_sjr = model.Recipient;
                    view.txt_dh = model.Phone;
                    view.txt_lxr = model.Contact;
                    view.txt_lxrdh = model.ContactNumber;
                    view.select_wzzl = model.MaterialName;
                    view.select_wzzlx = model.MaterialName;
                    view.select_wzzln = model.MaterialName;

                    if (model.ApplicantId != null)
                    {
                        var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.ApplicantId.Value);
                        if (baseUser == null)
                        {
                            throw new Exception("未找到申请人相关信息！");
                        }
                        view.hr_sqr = baseUser.ObjeId;

                    }

                    if (model.CompanyId != null)
                    {
                        var company = DataOperateBasic<Base_Company>.Get().GetModel(model.CompanyId.Value);
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

                    if (model.DepartmentId != null)
                    {
                        var company = DataOperateBasic<Base_Company>.Get().GetModel(model.DepartmentId.Value);
                        if (company == null)
                        {
                            throw new Exception("未获取到申请部门信息！");
                        }
                        view.dep_sqbm = company.ObjeId;
                    }
                    else
                    {
                        throw new Exception("请选择申请部门！");
                    }

                    view.date_sqsj = string.Format("{0:yyyy-MM-dd}", DateTime.Now);

                    if (model.LeaderId != null)
                    {
                        var baseUserHeaderID = DataOperateBasic<Base_User>.Get().GetModel(model.LeaderId.Value);
                        if (baseUserHeaderID == null)
                        {
                            throw new Exception("未找到分管领导相关信息！");
                        }
                        view.hr_fgld = baseUserHeaderID.ObjeId;
                    }
                    if (model.DepLeaderId != null)
                    {
                        var baseUserLeaderID = DataOperateBasic<Base_User>.Get().GetModel(model.DepLeaderId.Value);
                        if (baseUserLeaderID == null)
                        {
                            throw new Exception("未找到部门负责人相关信息！");
                        }
                        view.hr_bmfzr = baseUserLeaderID.ObjeId;
                    }

                    view.txt = model.ApprovalNo;
                    view.date_dqn = DateTime.Now.Year.ToString();
                    view.txt_gysmc = model.SupplierName;
                    view.txt_gysmcx = model.SupplierName;
                    view.txt_gysmc_dy = model.SupplierName;
                    view.txt_gysdz = model.SupplierAdress;
                    view.txt_gysyb = model.SupplierCode;
                    view.txt_htbsxh = model.ContractNumber;

                    //明细表信息
                    if (model.TzGcGoodsOrdersItem != null && model.TzGcGoodsOrdersItem.Any())
                    {
                        view.list = new List<GcGoodsOrdersApplyView.GcGoodsOrdersItem>();
                        foreach (var temp in model.TzGcGoodsOrdersItem)
                        {
                            GcGoodsOrdersApplyView.GcGoodsOrdersItem itemview = new GcGoodsOrdersApplyView.GcGoodsOrdersItem();
                            itemview.dep_zm = temp.StationName;
                            itemview.float_dj = temp.UnitPrice.ToString();
                            itemview.float_je = temp.Amount.ToString();
                            itemview.int_mount = temp.Number.ToString();
                            itemview.txts_bz = temp.Note;
                            itemview.txt_dhdz = temp.Address;
                            itemview.txt_gg = temp.Specifications;
                            itemview.txt_pm = temp.ProductName;
                            itemview.txt_zm = temp.StationName;

                            view.list.Add(itemview);
                        }
                    }

                    model.WorkFlowId = XtWorkFlowService.CreateTzOrdersWorkFlow(view);
                }
                #endregion

                var rows = DataOperateBusiness<Epm_TzGcGoodsOrdersApply>.Get().Update(model);

                if (model.TzGcGoodsOrdersItem.Any())
                {
                    //先删除
                    var detaileList = DataOperateBusiness<Epm_TzGcGoodsOrdersItem>.Get().GetList(p => p.ChangeApplyId == model.Id);
                    if (detaileList.Any())
                    {
                        DataOperateBusiness<Epm_TzGcGoodsOrdersItem>.Get().DeleteRange(detaileList);
                    }

                    model.TzGcGoodsOrdersItem.ForEach(item =>
                    {
                        item.ChangeApplyId = model.Id;
                        item = SetCurrentUser(item);
                        item = SetCreateUser(item);
                    });
                    DataOperateBusiness<Epm_TzGcGoodsOrdersItem>.Get().AddRange(model.TzGcGoodsOrdersItem);
                }

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.GcGoodsOrdersApply.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzGcGoodsOrdersApply");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzGcGoodsOrdersApplyByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzGcGoodsOrdersApply>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzGcGoodsOrdersApply>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.GcGoodsOrdersApply.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzGcGoodsOrdersApplyByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzGcGoodsOrdersApply>> GetTzGcGoodsOrdersApplyList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzGcGoodsOrdersApply>> result = new Result<List<Epm_TzGcGoodsOrdersApply>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzGcGoodsOrdersApply>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzGcGoodsOrdersApplyList");
            }
            return result;
        }

        /// <summary>
        /// 获取详情数据列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<TzGcGoodsOrdersItemView>> GetTzGcGoodsOrdersApplyListAll(QueryCondition qc)
        {
            Result<List<TzGcGoodsOrdersItemView>> result = new Result<List<TzGcGoodsOrdersItemView>>();

            try
            {
                var query = from a in context.Epm_TzGcGoodsOrdersItem.Where(p => !p.IsDelete)
                            join b in context.Epm_TzGcGoodsOrdersApply.Where(p => !p.IsDelete) on a.ChangeApplyId equals b.Id into bref
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
                                case "Title":
                                    {
                                        query = query.Where(p => p.b.Title.Contains(value));
                                        break;
                                    }
                                case "SupplierName":
                                    {
                                        query = query.Where(p => p.b.SupplierName.Contains(value));
                                        break;
                                    }
                                case "MaterialNumber":
                                    {
                                        query = query.Where(p => p.b.MaterialNumber.Contains(value));
                                        break;
                                    }
                                case "State":
                                    {
                                        query = query.Where(p => value.Contains(p.b.State.ToString()));
                                        break;
                                    }
                                case "ProjectName":
                                    {
                                        query = query.Where(p => p.b.ProjectName.Contains(value));
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
                var list = query.OrderByDescending(p => p.b.ApplyDate).Skip(skip).Take(take).AsEnumerable().Select(p => new TzGcGoodsOrdersItemView()
                {
                    Id = p.b.Id.ToString(),
                    Title = p.b.Title,
                    ApplyDate = p.b.ApplyDate,
                    CompanyName = p.b.CompanyName,
                    ProjectName = p.b.ProjectName,
                    SupplierName = p.b.SupplierName,
                    MaterialName = p.b.MaterialName,
                    StateName = p.b.StateName,
                    ApprovalName = p.b.ApprovalName,
                    State = p.b.State,

                    StationName = p.a.StationName,
                    ProductName = p.a.ProductName,
                    Specifications = p.a.Specifications,
                    Number = p.a.Number,
                    UnitPrice = p.a.UnitPrice,
                    Amount = p.a.Amount,
                    
                }).ToList();

                result.Data = list;
                result.AllRowsCount = total;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Exception = new ExceptionEx(ex, "GetTzGcGoodsOrdersApplyListAll");
                result.Flag = EResultFlag.Failure;
            }
            return result;
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzGcGoodsOrdersApply> GetTzGcGoodsOrdersApplyModel(long id)
        {
            Result<Epm_TzGcGoodsOrdersApply> result = new Result<Epm_TzGcGoodsOrdersApply>();
            try
            {
                var model = DataOperateBusiness<Epm_TzGcGoodsOrdersApply>.Get().GetModel(id);
                if (model != null)
                {
                    model.TzGcGoodsOrdersItem = DataOperateBusiness<Epm_TzGcGoodsOrdersItem>.Get().GetList(t => t.ChangeApplyId == id).ToList();
                }
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzGcGoodsOrdersApplyModel");
            }
            return result;
        }
    }
}
