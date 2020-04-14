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
        public Result<int> AddProjectApproval(ProjectApprovalView model)
        {
            Result<int> result = new Result<int>();
            try
            {
                model.ProjectPolit = SetCreateUser(model.ProjectPolit);

                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");

                if (model.ProjectPolit.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                {
                    ProjectApprovalApplyView view = new ProjectApprovalApplyView();
                    view.txt_xmmc = model.ProjectPolit.ProjectName;

                    if (model.ProjectPolit.CompanyId != null)
                    {
                        var company = DataOperateBasic<Base_Company>.Get().GetModel(model.ProjectPolit.CompanyId.Value);
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

                    view.date_kgrq = string.Format("{0:yyyy-MM-dd}", model.ProjectPolit.StartDate);
                    view.date_jgrq = string.Format("{0:yyyy-MM-dd}", model.ProjectPolit.EndDate);
                    view.date_ysrq = string.Format("{0:yyyy-MM-dd}", model.ProjectPolit.AcceptDate);
                    view.date_zgrq = string.Format("{0:yyyy-MM-dd}", model.ProjectPolit.RectFinishDate);
                    view.date_jssj = string.Format("{0:yyyy-MM-dd}", model.ProjectPolit.FinalDate);
                    view.date_sjsj = string.Format("{0:yyyy-MM-dd}", model.ProjectPolit.AuditDate);
                    view.txts_zlsfqq = model.ProjectPolit.FullFiles;
                    view.txt_ysyj = model.ProjectPolit.AcceptOpinion;
                    view.date_sqrq = string.Format("{0:yyyy-MM-dd}", DateTime.Now);

                    var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.ProjectPolit.CreateUserId);
                    if (baseUser == null)
                    {
                        throw new Exception("未找到申请人相关信息！");
                    }

                    view.hr_sqr = baseUser.ObjeId;
                    view.file_fj = "";

                    model.ProjectPolit = SetCurrentUser(model.ProjectPolit);
                    model.ProjectPolit.State = 0;

                    if (model.FileList.Count > 0)
                    {
                        AddFilesByTable(model.ProjectPolit, model.FileList); //上传附件

                        foreach (var item in model.FileList)
                        {
                            switch (item.TableColumn)
                            {
                                case "SBJJ":        // 销售企业二级单位验收主要事项表
                                    view.file_fj = XtWorkFlowService.GetXtAttachPaht(item.Url);
                                    break;
                                case "SGSC":        // 施工转生产界面交接确认单
                                    view.file_sgdw = XtWorkFlowService.GetXtAttachPaht(item.Url);
                                    break;
                                case "GCJJ":        // 工程交接证书
                                    view.file_gcjjzs = XtWorkFlowService.GetXtAttachPaht(item.Url);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    model.ProjectPolit.WorkFlowId = XtWorkFlowService.CreateApplyWorkFlow(view);
                }

                var rows = DataOperateBusiness<Epm_TzProjectPolit>.Get().Add(model.ProjectPolit);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.ProjectApproval.GetText(), SystemRight.Add.GetText(), "新增: " + model.ProjectPolit.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddProjectApproval");
            }
            return result;
        }

        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateProjectApproval(Epm_TzProjectPolit model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzProjectPolit>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.ProjectApproval.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateProjectApproval");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteProjectApprovalByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzProjectPolit>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzProjectPolit>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.ProjectApproval.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteProjectApprovalByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectPolit>> GetProjectApprovalList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzProjectPolit>> result = new Result<List<Epm_TzProjectPolit>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzProjectPolit>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectApprovalList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<ProjectApprovalView> GetProjectApprovalModel(long id)
        {
            Result<ProjectApprovalView> result = new Result<ProjectApprovalView>();
            try
            {
                ProjectApprovalView view = new ProjectApprovalView();
                var model = DataOperateBusiness<Epm_TzProjectPolit>.Get().GetModel(id);
                if (model!=null)
                {
                   var attachs= DataOperateBusiness<Epm_TzAttachs>.Get().GetList(i => i.DataId == id && i.TableName == "Epm_TzProjectPolit").OrderBy(p => p.Sort).ToList();
                    model.TzAttachs = attachs;
                }
                view.ProjectPolit = model;
                view.ProjectAuditRecordList = DataOperateBusiness<Epm_ProjectAuditRecord>.Get().GetList(t => t.ApprovalId == id).ToList();
                view.FileList = DataOperateBasic<Base_Files>.Get().GetList(i => i.TableId == id && i.TableName == "Epm_TzProjectPolit").ToList();

                result.Data = view;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectApprovalModel");
            }
            return result;
        }

        /// <summary>
        /// 新增审核记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<bool> AddProjectAuditRecord(Epm_ProjectAuditRecord model)
        {
            Result<bool> result = new Result<bool>();
            model = SetCurrentUser(model);
            try
            {
                if (model != null)
                {
                    var user = DataOperateBasic<Base_User>.Get().GetList(t => t.ObjeId == model.AuditUserCode).FirstOrDefault();

                    if(user == null)
                    {
                        throw new Exception("未找到当前审批人信息！");
                    }

                    var usernext = DataOperateBasic<Base_User>.Get().GetList(t => t.ObjeId == model.NextAuditUserCode).FirstOrDefault();
                    
                    if (usernext == null)
                    {
                        throw new Exception("未找到下一步审批人信息！");
                    }
                    
                    var approval = DataOperateBusiness<Epm_TzProjectPolit>.Get().Single(p => p.WorkFlowId == model.WorkFlowId);

                    if(approval == null)
                    {
                        throw new Exception("未找到对应的申请：" + model.WorkFlowId);
                    }

                    //if (user != null)
                    //{
                    //    model.AuditUserId = user.Id;
                    //    model.NextAuditUserId = usernext.Id;
                    //}
                    model.ProjectId = approval.ProjectId;
                    model.ApprovalId = approval.Id;

                    if(model.ApprovalState == 1)
                    {
                        if (model.State == 1)
                        {
                            approval.State = 2;
                        }
                        else
                        {
                            approval.State = 3;
                        }
                    }
                    else
                    {
                        approval.State = 1;
                    }

                    DataOperateBusiness<Epm_TzProjectPolit>.Get().Update(approval);

                    model.State = 0;
                    var rows = DataOperateBusiness<Epm_ProjectAuditRecord>.Get().Add(model);

                    if (rows > 0)
                    {
                        result.Data = true;
                        result.Flag = EResultFlag.Success;
                    }
                    WriteLog(AdminModule.ProjectApproval.GetText(), SystemRight.Add.GetText(), "添加: " + model.Id);
                }
            }
            catch (Exception ex)
            {
                result.Data = false;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddProjectAuditRecord");
            }
            return result;
        }

        public Result<int> UpdateTzProjectPolit(Epm_TzProjectPolit model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var Poli = DataOperateBusiness<Epm_TzProjectPolit>.Get().Single(p => p.Id == model.Id);
               
                SetCurrentUser(model);

                #region 协同接口
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                {
                    ProjectApprovalApplyView view = new ProjectApprovalApplyView();
                    view.txt_xmmc = model.ProjectName;

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

                    view.date_kgrq = string.Format("{0:yyyy-MM-dd}", model.StartDate);
                    view.date_jgrq = string.Format("{0:yyyy-MM-dd}", model.EndDate);
                    view.date_ysrq = string.Format("{0:yyyy-MM-dd}", model.AcceptDate);
                    view.date_zgrq = string.Format("{0:yyyy-MM-dd}", model.RectFinishDate);
                    view.date_jssj = string.Format("{0:yyyy-MM-dd}", model.FinalDate);
                    view.date_sjsj = string.Format("{0:yyyy-MM-dd}", model.AuditDate);
                    view.txts_zlsfqq = model.FullFiles;
                    view.txt_ysyj = model.AcceptOpinion;
                    view.date_sqrq = string.Format("{0:yyyy-MM-dd}", DateTime.Now);

                    var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.CreateUserId);
                    if (baseUser == null)
                    {
                        throw new Exception("未找到申请人相关信息！");
                    }

                    view.hr_sqr = baseUser.ObjeId;
                    view.file_fj = "";
                    if (model.TzAttachs.Count > 0)
                    {
                        //foreach (var item in model.TzAttachs)
                        //{
                        //    switch (item.TypeNo)
                        //    {
                        //        case "SBJJ":        // 销售企业二级单位验收主要事项表
                        //            view.file_fj = XtWorkFlowService.GetXtAttachPaht(item.FilePath);
                        //            break;
                        //        case "SGSC":        // 施工转生产界面交接确认单
                        //            view.file_sgdw = XtWorkFlowService.GetXtAttachPaht(item.FilePath);
                        //            break;
                        //        case "GCJJ":        // 工程交接证书
                        //            view.file_gcjjzs = XtWorkFlowService.GetXtAttachPaht(item.FilePath);
                        //            break;
                        //        default:
                        //            break;
                        //    }
                        //}
                        //销售企业二级单位验收主要事项表
                        var sxbTemp = model.TzAttachs.Where(p => p.TypeNo == "KGSQXSQYEJ").ToList();
                        view.file_fj = XtWorkFlowSubmitService.CreateXtAttachPath(sxbTemp);
                        //施工转生产界面交接确认单
                        var qrdTemp = model.TzAttachs.Where(p => p.TypeNo == "KGSQSGZSCJM").ToList();
                        view.file_fj = XtWorkFlowSubmitService.CreateXtAttachPath(qrdTemp);
                        //工程交接证书
                        var jjzsTemp = model.TzAttachs.Where(p => p.TypeNo == "KGSQGCJJZS").ToList();
                        view.file_fj = XtWorkFlowSubmitService.CreateXtAttachPath(jjzsTemp);
                    }

                    model.WorkFlowId = XtWorkFlowService.CreateApplyWorkFlow(view);

                }
                #endregion
                var rows = DataOperateBusiness<Epm_TzProjectPolit>.Get().Update(model);
                //上传附件
                AddFilesBytzTable(model, model.TzAttachs);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzConDrawing");
            }
            return result;
        }

        /// <summary>
        /// 修改试运行状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzProjectPolitState(List<long> ids, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                foreach (var item in ids)
                {
                    var model = DataOperateBusiness<Epm_TzProjectPolit>.Get().GetModel(item);
                    if (model != null)
                    {
                        //SetCurrentUser(model);
                        model.State = (int)state.ToEnumReq<PreProjectApprovalState>();
                        var rows = DataOperateBusiness<Epm_TzProjectPolit>.Get().Update(model);

                        //如果状态是已经提交，生成数据
                        if (model.State == (int)PreProjectApprovalState.ApprovalSuccess)
                        {
                            #region 生成竣工验收申请数据
                            //生成竣工验收申请数据
                            Epm_CompletionAcceptance temp = new Epm_CompletionAcceptance();
                            temp.ProjectId = model.ProjectId;
                            temp.ProjectName = model.ProjectName;
                            temp.State = (int)PreProjectState.WaitSubmitted;
                            temp.RecCompanyId = CurrentUser.CompanyId;
                            temp.RecCompanyName = CurrentUser.CompanyName;
                            temp.RecUserName = CurrentUser.UserName;
                            temp.RecUserId = CurrentUser.Id;

                            AddCompletionAcceptance(temp, null);

                            #endregion
                        }

                        result.Data = rows;
                        result.Flag = EResultFlag.Success;
                    }
                    else
                    {
                        throw new Exception("该施工图纸会审不存在");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzConDrawingState");
            }
            return result;
        }

        
    }
}
