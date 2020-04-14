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
    /// 建设工程项目管理人员变更申请流程
    /// </summary>
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzPeopleChgApply(Epm_TzPeopleChgApply model)
        {
            Result<int> result = new Result<int>();
            try
            {
                model = SetCurrentUser(model);
                model = SetCreateUser(model);

                #region 建设工程项目管理人员变更申请流程
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)PreProjectApprovalState.WaitApproval && XtWorkFlow == "1")
                {
                    TzPeopleChgApplyView view = new TzPeopleChgApplyView();

                    if (model.ApplicantId != null)
                    {
                        var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.ApplicantId.Value);
                        if (baseUser == null)
                        {
                            throw new Exception("未找到申请人相关信息！");
                        }
                        view.hr_sqr = baseUser.ObjeId;

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

                    view.date_sqrq = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
                    view.txt_xmmc = model.ProjectName;
                    view.txt_jsdz = model.ConstructionAddress;
                    view.sub_sgdw = model.WorkUnit;
                    view.txt_fzr = model.Leader;

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

                    //明细表信息
                    if (model.TzPeopleChgApplyItem != null && model.TzPeopleChgApplyItem.Any())
                    {
                        view.list = new List<TzPeopleChgApplyView.TzPeopleChgApplyItem>();
                        foreach (var temp in model.TzPeopleChgApplyItem)
                        {
                            TzPeopleChgApplyView.TzPeopleChgApplyItem itemview = new TzPeopleChgApplyView.TzPeopleChgApplyItem();
                            itemview.txt_bgqzyzsmc = temp.ChgCertiNameBefore;
                            itemview.txt_bghzyzsmc = temp.ChgCertiNameAfter;
                            itemview.txt_bggw = temp.ChgPost;
                            itemview.txt_bgqry = temp.ChgPeopleBefore;
                            itemview.txt_bghry = temp.ChgPeopleAfter;
                            itemview.txt_bgqzyzsh = temp.ChgCertiNoBefore;
                            itemview.txt_bghzyzsh = temp.ChgCertiNoAfter;

                            view.list.Add(itemview);
                        }
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
                        //        case "RYBGFJ":
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
                        var fileList = model.TzAttachs.Where(p => p.TypeNo == "RYBGFJ").ToList();
                        view.fj = XtWorkFlowSubmitService.CreateXtAttachPath(fileList);
                    }

                    model.WorkFlowId = XtWorkFlowService.CreateTzPeopleChgApplyWorkFlow(view);
                }
                #endregion

                var rows = DataOperateBusiness<Epm_TzPeopleChgApply>.Get().Add(model);
                if (model.TzPeopleChgApplyItem != null && model.TzPeopleChgApplyItem.Any())
                {
                    model.TzPeopleChgApplyItem.ForEach(item =>
                    {
                        item.ChangeApplyId = model.Id;
                        item = SetCurrentUser(item);
                        item = SetCreateUser(item);
                    });
                    DataOperateBusiness<Epm_TzPeopleChgApplyItem>.Get().AddRange(model.TzPeopleChgApplyItem);
                }
                //上传附件
                if (model.TzAttachs != null && model.TzAttachs.Any())
                {
                    AddFilesBytzTable(model, model.TzAttachs);
                }
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzPeopleChgApply.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzPeopleChgApply");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzPeopleChgApply(Epm_TzPeopleChgApply model)
        {
            Result<int> result = new Result<int>();
            try
            {
                #region 建设工程项目管理人员变更申请流程
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)PreProjectApprovalState.WaitApproval && XtWorkFlow == "1")
                {
                    TzPeopleChgApplyView view = new TzPeopleChgApplyView();

                    if (model.ApplicantId != null)
                    {
                        var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.ApplicantId.Value);
                        if (baseUser == null)
                        {
                            throw new Exception("未找到申请人相关信息！");
                        }
                        view.hr_sqr = baseUser.ObjeId;

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

                    view.date_sqrq = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
                    view.txt_xmmc = model.ProjectName;
                    view.txt_jsdz = model.ConstructionAddress;
                    view.sub_sgdw = model.WorkUnit;
                    view.txt_fzr = model.Leader;

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

                    //明细表信息
                    if (model.TzPeopleChgApplyItem != null && model.TzPeopleChgApplyItem.Any())
                    {
                        view.list = new List<TzPeopleChgApplyView.TzPeopleChgApplyItem>();
                        foreach (var temp in model.TzPeopleChgApplyItem)
                        {
                            TzPeopleChgApplyView.TzPeopleChgApplyItem itemview = new TzPeopleChgApplyView.TzPeopleChgApplyItem();
                            itemview.txt_bgqzyzsmc = temp.ChgCertiNameBefore;
                            itemview.txt_bghzyzsmc = temp.ChgCertiNameAfter;
                            itemview.txt_bggw = temp.ChgPost;
                            itemview.txt_bgqry = temp.ChgPeopleBefore;
                            itemview.txt_bghry = temp.ChgPeopleAfter;
                            itemview.txt_bgqzyzsh = temp.ChgCertiNoBefore;
                            itemview.txt_bghzyzsh = temp.ChgCertiNoAfter;

                            view.list.Add(itemview);
                        }
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
                        //        case "RYBGFJ":
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
                        var fileList = model.TzAttachs.Where(p => p.TypeNo == "RYBGFJ").ToList();
                        view.fj = XtWorkFlowSubmitService.CreateXtAttachPath(fileList);
                    }

                    model.WorkFlowId = XtWorkFlowService.CreateTzPeopleChgApplyWorkFlow(view);
                }
                #endregion

                SetCurrentUser(model);
                    
                var rows = DataOperateBusiness<Epm_TzPeopleChgApply>.Get().Update(model);
                if (model.TzPeopleChgApplyItem.Any())
                {
                    //先删除
                    var detaileList = DataOperateBusiness<Epm_TzPeopleChgApplyItem>.Get().GetList(p => p.ChangeApplyId == model.Id);
                    if (detaileList.Any())
                    {
                        DataOperateBusiness<Epm_TzPeopleChgApplyItem>.Get().DeleteRange(detaileList);
                    }

                    model.TzPeopleChgApplyItem.ForEach(item =>
                    {
                        item.ChangeApplyId = model.Id;
                        item = SetCurrentUser(item);
                        item = SetCreateUser(item);
                    });
                    DataOperateBusiness<Epm_TzPeopleChgApplyItem>.Get().AddRange(model.TzPeopleChgApplyItem);
                }

                //上传附件
                if (model.TzAttachs != null && model.TzAttachs.Any())
                {
                    AddFilesBytzTable(model, model.TzAttachs);
                }
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzPeopleChgApply.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzPeopleChgApply");
            }
            return result;
        }

        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzPeopleChgApplyByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzPeopleChgApply>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzPeopleChgApply>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzPeopleChgApply.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzPeopleChgApplyByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzPeopleChgApply>> GetTzPeopleChgApplyList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzPeopleChgApply>> result = new Result<List<Epm_TzPeopleChgApply>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzPeopleChgApply>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzPeopleChgApplyList");
            }
            return result;
        }

        public Result<List<TzPeopleChgApplyItemView>> GetTzPeopleApplyItemList(QueryCondition qc)
        {
            Result<List<TzPeopleChgApplyItemView>> result = new Result<List<TzPeopleChgApplyItemView>>();

            try
            {
                var query = from a in context.Epm_TzPeopleChgApplyItem.Where(p => !p.IsDelete)
                            join b in context.Epm_TzPeopleChgApply.Where(p => !p.IsDelete) on a.ChangeApplyId equals b.Id into bref
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
                                case "WorkUnit":
                                    {
                                        query = query.Where(p => p.b.WorkUnit.Contains(value));
                                        break;
                                    }
                                case "Title":
                                    {
                                        query = query.Where(p => p.b.Title.Contains(value));
                                        break;
                                    }
                                case "Leader":
                                    {
                                        query = query.Where(p => p.b.Leader.Contains(value));
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
                var list = query.OrderByDescending(p => p.b.ApplyDate).Skip(skip).Take(take).AsEnumerable().Select(p => new TzPeopleChgApplyItemView()
                {
                    Id = p.b.Id,
                    Title = p.b.Title,
                    Applicant = p.b.Applicant,
                    ApplyDate = p.b.ApplyDate,
                    CompanyName = p.b.CompanyName,
                    Department = p.b.Department,
                    ProjectName = p.b.ProjectName,
                    ConstructionAddress = p.b.ConstructionAddress,
                    WorkUnit = p.b.WorkUnit,
                    WorkLeader = p.b.WorkLeader,
                    StateType = p.b.StateType,
                    StateName = p.b.StateName,
                    ChangeApplyId = p.a.ChangeApplyId,
                    ChgPost = p.a.ChgPost,
                    ChgPeopleBefore = p.a.ChgPeopleBefore,
                    ChgPeopleAfter = p.a.ChgPeopleAfter,
                    ChgCertiNameBefore = p.a.ChgCertiNameBefore,
                    ChgCertiNoBefore = p.a.ChgCertiNoBefore,
                    ChgCertiNameAfter = p.a.ChgCertiNameAfter,
                    ChgCertiNoAfter = p.a.ChgCertiNoAfter,
                    ApprovalName = p.b.ApprovalName,
                    State = p.b.State,
                    Leader = p.b.Leader,

                }).ToList();

                result.Data = list;
                result.AllRowsCount = total;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Exception = new ExceptionEx(ex, "GetTzPeopleApplyItemList");
                result.Flag = EResultFlag.Failure;
            }
            return result;
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzPeopleChgApply> GetTzPeopleChgApplyModel(long id)
        {
            Result<Epm_TzPeopleChgApply> result = new Result<Epm_TzPeopleChgApply>();
            try
            {
                var model = DataOperateBusiness<Epm_TzPeopleChgApply>.Get().GetModel(id);
                if (model != null)
                {
                    model.TzPeopleChgApplyItem = DataOperateBusiness<Epm_TzPeopleChgApplyItem>.Get().GetList(t => t.ChangeApplyId == id).ToList();

                    model.TzAttachs = GetFilesByTZTable("Epm_TzPeopleChgApply", id).Data;
                }
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzPeopleChgApplyModel");
            }
            return result;
        }
        
    }
}
