using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Service.ClientSite
{
    /// <summary>
    /// 甲供物资申请
    /// </summary>
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzSupplyMaterialApply(Epm_TzSupplyMaterialApply model)
        {
            Result<int> result = new Result<int>();
            try
            {
                SetCurrentUser(model);
                SetCreateUser(model);
                model.UseType = false;
                var sumTotal = model.TzSupMatApplyList.Sum(p => p.Number);//根据明细计算总数量
                model.Number = sumTotal;

                if (model.ProjectId.HasValue)
                {
                    var project = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId.Value);
                    if (project != null)
                    {
                        model.ProjectCode = project.ProjectCode;
                        model.Nature = project.Nature;
                        model.NatureName = project.NatureName;
                        model.ApplyTime = project.ApplyTime;
                        model.CompanyId = project.CompanyId;
                        model.CompanyName = project.CompanyName;
                        model.CompanyCodeXt = project.CompanyCodeXt;
                    }
                }

                #region 甲供物资申请  不要删哦
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                {
                    XtTzSupplyMaterialApplyView view = new XtTzSupplyMaterialApplyView();

                    view.ApprovalNo = model.ApprovalNo;
                    view.ApplyTitle = model.ApplyTitle;
                    view.ApplyUserName = model.ApplyUserName;
                    view.CreateTime = model.CreateTime.ToString();
                    view.ApplyDepartment = model.ApplyDepartment;
                    view.ApplyCompanyName = model.ApplyCompanyName;
                    view.ProjectName = model.ProjectName;
                    view.StationName = model.StationName;
                    view.ApprovalNo = model.ApprovalNo;
                    view.ContractName = model.ContractName;
                    view.ContractNumber = model.ContractNumber;
                    view.ErpCode = model.ErpCode;
                    view.ArrivalContacts = model.ArrivalContacts;
                    view.ArrivalAddress = model.ArrivalAddress;
                    view.Supplier = model.Supplier;
                    view.SupplierCode = model.SupplierCode;
                    view.SupplierContacts = model.SupplierContacts;
                    view.SupplierTel = model.SupplierTel;
                    view.SupplierAddress = model.SupplierAddress;
                    view.Number = model.Number.ToString();//数量
                    view.Money = model.ApplyAmount.ToString();
                    view.LeadershipName = model.LeadershipName;
                    view.ArrivalContactsTel = model.ArrivalContactsTel;

                    var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.CreateUserId);
                    if (baseUser == null)
                    {
                        throw new Exception("未找到申请人相关信息！");
                    }
                    else
                    {
                        view.hr_sqr = baseUser.ObjeId;
                    }
                    //明细表信息
                    if (model.TzSupMatApplyList != null && model.TzSupMatApplyList.Any())
                    {
                        view.list = new List<XtTzSupplyMaterialApplyView.SupMatApplyListItem>();
                        foreach (var temp in model.TzSupMatApplyList)
                        {
                            XtTzSupplyMaterialApplyView.SupMatApplyListItem itemview = new XtTzSupplyMaterialApplyView.SupMatApplyListItem();
                            itemview.MaterialCategory = temp.SupMatManagement;
                            itemview.ProductName = temp.ProductName.ToString();
                            itemview.Specification = temp.Specification.ToString();
                            itemview.UnitPrice = temp.UnitPrice.ToString();
                            itemview.Moneys = temp.Money.ToString();
                            itemview.CLNumber = temp.Number.ToString();

                            view.list.Add(itemview);
                        }
                    }

                    model.WorkFlowId = XtWorkFlowService.CreateTzSupplyMaterialApplyWorkFlow(view);
                }
                #endregion

                var rows = DataOperateBusiness<Epm_TzSupplyMaterialApply>.Get().Add(model);

                if (model.TzSupMatApplyList != null && model.TzSupMatApplyList.Any())
                {
                    model.TzSupMatApplyList.ForEach(item =>
                    {
                        item.SupMatApplyId = model.Id;
                        item = SetCurrentUser(item);
                        item = SetCreateUser(item);
                        item.UseType = false;
                        item.UseSum = 0;
                        item.SupplierName = model.Supplier;
                        item.StayUseSum = Convert.ToInt32(item.Number);
                    });
                    DataOperateBusiness<Epm_TzSupMatApplyList>.Get().AddRange(model.TzSupMatApplyList);
                }

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzSupplyMaterialApply.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzSupplyMaterialApply");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzSupplyMaterialApply(Epm_TzSupplyMaterialApply model)
        {
            Result<int> result = new Result<int>();
            try
            {
                SetCurrentUser(model);
                var sumTotal = model.TzSupMatApplyList.Sum(p => p.Number);//根据明细计算总数量
                model.Number = sumTotal;
                if (model.ProjectId.HasValue)
                {
                    var project = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId.Value);
                    if (project != null)
                    {
                        model.ProjectCode = project.ProjectCode;
                        model.Nature = project.Nature;
                        model.NatureName = project.NatureName;
                        model.ApplyTime = project.ApplyTime;
                        model.CompanyId = project.CompanyId;
                        model.CompanyName = project.CompanyName;
                        model.CompanyCodeXt = project.CompanyCodeXt;
                    }
                }

                #region 甲供物资申请  不要删哦
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                {
                    XtTzSupplyMaterialApplyView view = new XtTzSupplyMaterialApplyView();

                    view.ApprovalNo = model.ApprovalNo;
                    view.ApplyTitle = model.ApplyTitle;
                    view.ApplyUserName = model.ApplyUserName;
                    view.CreateTime = model.CreateTime.ToString();
                    view.ApplyDepartment = model.ApplyDepartment;
                    view.ApplyCompanyName = model.ApplyCompanyName;
                    view.ProjectName = model.ProjectName;
                    view.StationName = model.StationName;
                    view.ApprovalNo = model.ApprovalNo;
                    view.ContractName = model.ContractName;
                    view.ContractNumber = model.ContractNumber;
                    view.ErpCode = model.ErpCode;
                    view.ArrivalContacts = model.ArrivalContacts;
                    view.ArrivalAddress = model.ArrivalAddress;
                    view.Supplier = model.Supplier;
                    view.SupplierCode = model.SupplierCode;
                    view.SupplierContacts = model.SupplierContacts;
                    view.SupplierTel = model.SupplierTel;
                    view.SupplierAddress = model.SupplierAddress;
                    view.Number = model.Number.ToString();//数量
                    view.Money = model.ApplyAmount.ToString();
                    view.LeadershipName = model.LeadershipName;
                    view.ArrivalContactsTel = model.ArrivalContactsTel;

                    var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.CreateUserId);
                    if (baseUser == null)
                    {
                        throw new Exception("未找到申请人相关信息！");
                    }
                    else
                    {
                        view.hr_sqr = baseUser.ObjeId;
                    }
                    //明细表信息
                    if (model.TzSupMatApplyList != null && model.TzSupMatApplyList.Any())
                    {
                        view.list = new List<XtTzSupplyMaterialApplyView.SupMatApplyListItem>();
                        foreach (var temp in model.TzSupMatApplyList)
                        {
                            XtTzSupplyMaterialApplyView.SupMatApplyListItem itemview = new XtTzSupplyMaterialApplyView.SupMatApplyListItem();
                            itemview.MaterialCategory = temp.SupMatManagement;
                            itemview.ProductName = temp.ProductName.ToString();
                            itemview.Specification = temp.Specification.ToString();
                            itemview.UnitPrice = temp.UnitPrice.ToString();
                            itemview.Moneys = temp.Money.ToString();
                            itemview.CLNumber = temp.Number.ToString();

                            view.list.Add(itemview);
                        }
                    }

                    model.WorkFlowId = XtWorkFlowService.CreateTzSupplyMaterialApplyWorkFlow(view);
                }
                #endregion
                var rows = DataOperateBusiness<Epm_TzSupplyMaterialApply>.Get().Update(model);

                if (model.TzSupMatApplyList.Any())
                {
                    //先删除
                    var detaileList = DataOperateBusiness<Epm_TzSupMatApplyList>.Get().GetList(p => p.SupMatApplyId == model.Id);
                    if (detaileList.Any())
                    {
                        DataOperateBusiness<Epm_TzGcGoodsOrdersItem>.Get().DeleteRange(detaileList);
                    }

                    model.TzSupMatApplyList.ForEach(item =>
                    {
                        item.SupMatApplyId = model.Id;
                        item = SetCurrentUser(item);
                        item = SetCreateUser(item);
                        item.UseType = false;
                        item.UseSum = 0;
                        item.SupplierName = model.Supplier;
                        item.StayUseSum = Convert.ToInt32(item.Number);
                    });
                    DataOperateBusiness<Epm_TzSupMatApplyList>.Get().AddRange(model.TzSupMatApplyList);
                }

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzSupplyMaterialApply.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzSupplyMaterialApply");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzSupplyMaterialApplyByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzSupplyMaterialApply>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzSupplyMaterialApply>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzSupplyMaterialApply.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzSupplyMaterialApplyByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzSupplyMaterialApply>> GetTzSupplyMaterialApplyList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzSupplyMaterialApply>> result = new Result<List<Epm_TzSupplyMaterialApply>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzSupplyMaterialApply>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzSupplyMaterialApplyList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzSupplyMaterialApply> GetTzSupplyMaterialApplyModel(long id)
        {
            Result<Epm_TzSupplyMaterialApply> result = new Result<Epm_TzSupplyMaterialApply>();
            try
            {
                var model = DataOperateBusiness<Epm_TzSupplyMaterialApply>.Get().GetModel(id);

                if (model != null)
                {
                    long userId = model.ApprovalNameId;
                    if (userId > 0)
                    {
                        var userInfo = DataOperateBasic<Base_User>.Get().GetList(t => t.ObjeId == userId.ToString()).FirstOrDefault();
                        if (userInfo != null)
                        {
                            long depId = userInfo.DepartmentId.Value;

                            if (depId > 0)
                            {
                                var companyInfo = DataOperateBasic<Base_Company>.Get().GetModel(depId);
                                if (companyInfo != null)
                                {
                                    model.ApprovalDep = companyInfo.Name;
                                }
                            }
                            var fileInfo = DataOperateBasic<Base_Files>.Get().GetList(t => t.TableId == userInfo.Id && t.TableColumn == "QM" && t.ImageType == "small").FirstOrDefault();
                            if (fileInfo != null)
                            {
                                if (!string.IsNullOrEmpty(fileInfo.Url))
                                {
                                    string path = ConfigurationManager.AppSettings["ResourceUrl"];
                                    model.SignNameUrl = path + fileInfo.Url;
                                }
                            }
                        }
                    }
                    model.TzSupMatApplyList = DataOperateBusiness<Epm_TzSupMatApplyList>.Get().GetList(t => t.SupMatApplyId == id).ToList();
                    if (model.TzSupMatApplyList.Count > 0)
                    {
                        var list = model.TzSupMatApplyList;
                        model.TotleNum = list.Select(t => t.Number).Sum();
                        model.TotleMoney = list.Select(t => t.Money).Sum();
                    }
                }
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzSupplyMaterialApplyModel");
            }
            return result;
        }

        /// <summary>
        /// 获取甲供物资申请详情列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_TzSupMatApplyList>> GetTzSupMatApplyList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzSupMatApplyList>> result = new Result<List<Epm_TzSupMatApplyList>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzSupMatApplyList>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzSupMatApplyList");
            }
            return result;
        }


        /// <summary>
        /// 修改甲供物资申请状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzSupplyMaterialApplyState(List<long> ids, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                foreach (var item in ids)
                {
                    var model = DataOperateBusiness<Epm_TzSupplyMaterialApply>.Get().GetModel(item);
                    if (model != null)
                    {
                        //SetCurrentUser(model);
                        model.State = (int)state.ToEnumReq<PreProjectState>();
                        var rows = DataOperateBusiness<Epm_TzSupplyMaterialApply>.Get().Update(model);

                        result.Data = rows;
                        result.Flag = EResultFlag.Success;
                    }
                    else
                    {
                        throw new Exception("该甲供物资申请信息不存在");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzSupplyMaterialApplyState");
            }
            return result;
        }

        /// <summary>
        /// 获取甲供物资报表数据
        /// </summary>
        /// <returns></returns>
        public Result<List<SupplyMaterialReportView>> GetSupplyMaterialReport(QueryCondition qc)
        {
            Result<List<SupplyMaterialReportView>> result = new Result<List<SupplyMaterialReportView>>();
            try
            {
                List<SupplyMaterialReportView> supList = new List<SupplyMaterialReportView>();
                var list = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetList(t => t.IsDelete == false && t.State == (int)PreProjectState.ApprovalSuccess);
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        SupplyMaterialReportView view = new SupplyMaterialReportView();
                        view.ProjectName = item.ProjectName;
                        view.ProjectId = item.Id;
                        view.CompanyId = item.CompanyId;
                        view.CompanyName = item.CompanyName;
                        view.StationId = item.StationId;
                        view.StationName = item.StationName;

                        var tzSupMatApplyList = DataOperateBusiness<Epm_TzSupplyMaterialApply>.Get().GetList(t => t.ProjectId == item.Id).ToList();
                        int count = 0;
                        int UseSum = 0;
                        DateTime time = DateTime.Now;

                        if (supList.Select(t => t.ProjectId).Contains(item.Id))
                        {
                            var model = supList.Where(t => t.ProjectId == item.Id).FirstOrDefault();
                            count = model.Number.Value;
                            UseSum = model.AcceptNumber.Value;
                        }

                        if (tzSupMatApplyList != null && tzSupMatApplyList.Any())
                        {
                            view.CompanyNumber = tzSupMatApplyList.Select(t => t.SupplierId).Distinct().Count();

                            foreach (var temp in tzSupMatApplyList)
                            {
                                count = count + Convert.ToInt32(temp.Number);
                                var tzSupMatApplyListDetil = DataOperateBusiness<Epm_TzSupMatApplyList>.Get().GetList(t => t.SupMatApplyId == item.Id).ToList();
                                if (tzSupMatApplyListDetil != null && tzSupMatApplyListDetil.Any())
                                {
                                    UseSum = UseSum + tzSupMatApplyListDetil.Select(t => t.StayUseSum).Sum().Value;

                                    time = tzSupMatApplyListDetil[0].OperateTime.Value;
                                }
                            }
                        }
                        view.Number = count;
                        view.AcceptNumber = Convert.ToInt32(UseSum);
                        view.Time = time;
                        supList.Add(view);
                    }
                    supList = supList.Where(t => t.Number > 0).ToList();

                    result.AllRowsCount = supList.Count();
                    supList = supList.OrderByDescending(t => t.Time).Skip((qc.PageInfo.CurrentPageIndex - 1) * qc.PageInfo.PageRowCount).Take(qc.PageInfo.PageRowCount).ToList();

                    result.Data = supList;
                    result.Flag = EResultFlag.Success;
                }

            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSupplyMaterialReport");
            }
            return result;
        }

        /// <summary>
        /// 获取甲供物资供应商
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<SuppListView>> GetSuppList(long projectId, QueryCondition qc)
        {
            Result<List<SuppListView>> result = new Result<List<SuppListView>>();
            try
            {
                List<SuppListView> supList = new List<SuppListView>();
                var tzSupMatApplyList = DataOperateBusiness<Epm_TzSupplyMaterialApply>.Get().GetList(t => t.ProjectId == projectId).ToList();

                if (tzSupMatApplyList != null && tzSupMatApplyList.Any())
                {
                    SuppListView view = null;
                    List<ProductList> children = null;
                    foreach (var item in tzSupMatApplyList)
                    {
                        var model = supList.Where(t => t.SupplierName == item.Supplier).FirstOrDefault();
                        if (model != null)
                        {
                            children = model.children;

                            var tzSupMatApplyListDetil = DataOperateBusiness<Epm_TzSupMatApplyList>.Get().GetList(t => t.SupMatApplyId == item.Id).ToList();
                            if (tzSupMatApplyListDetil != null && tzSupMatApplyListDetil.Any())
                            {
                                foreach (var temp in tzSupMatApplyListDetil)
                                {
                                    ProductList pro = new ProductList();
                                    pro.SupMatManagement = temp.SupMatManagement;
                                    pro.ProductName = temp.ProductName;
                                    pro.Specification = temp.Specification;
                                    pro.Number = Convert.ToInt32(temp.Number);
                                    pro.AcceptNumber = Convert.ToInt32(temp.UseSum);
                                    children.Add(pro);
                                }
                            }
                            model.children = children;
                        }
                        else
                        {
                            view = new SuppListView();
                            view.SupplierName = item.Supplier;
                            view.Phone = item.SupplierTel;

                            children = new List<ProductList>();

                            var tzSupMatApplyListDetil = DataOperateBusiness<Epm_TzSupMatApplyList>.Get().GetList(t => t.SupMatApplyId == item.Id).ToList();
                            if (tzSupMatApplyListDetil != null && tzSupMatApplyListDetil.Any())
                            {
                                foreach (var temp in tzSupMatApplyListDetil)
                                {
                                    ProductList pro = new ProductList();
                                    pro.SupMatManagement = temp.SupMatManagement;
                                    pro.ProductName = temp.ProductName;
                                    pro.Specification = temp.Specification;
                                    pro.Number = Convert.ToInt32(temp.Number);
                                    pro.AcceptNumber = Convert.ToInt32(temp.UseSum);
                                    children.Add(pro);
                                }
                            }
                            view.children = children;
                            supList.Add(view);
                        }

                    }

                    result.AllRowsCount = supList.Count();
                    //supList = supList.OrderByDescending(t => t.SupplierName).Skip((qc.PageInfo.CurrentPageIndex - 1) * qc.PageInfo.PageRowCount).Take(qc.PageInfo.PageRowCount).ToList();
                    supList = supList.OrderByDescending(t => t.SupplierName).ToList();

                    result.Data = supList;
                    result.Flag = EResultFlag.Success;
                }

            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSuppList");
            }
            return result;
        }

    }
}
