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
    /// 项目评审记录
    /// </summary>
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:项目评审记录
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzProjectReveiews(Epm_TzProjectReveiews model)
        {
            Result<int> result = new Result<int>();
            try
            {
                int rows = 0;
                bool isAdd = false;
                var reveiews = DataOperateBusiness<Epm_TzProjectReveiews>.Get().GetList(t => t.ProjectId == model.ProjectId && t.State != (int)PreProjectState.ApprovalFailure && t.State != (int)PreProjectState.Discarded).FirstOrDefault();

                if (reveiews == null)
                {
                    isAdd = true;
                    reveiews = new Epm_TzProjectReveiews();
                    SetCreateUser(reveiews);
                }
                reveiews.ProjectId = model.ProjectId;
                reveiews.ReveiewDate = model.ReveiewDate;
                reveiews.HostUser = model.HostUser;
                reveiews.Address = model.Address;
                reveiews.ConclusionCode = model.ConclusionCode;
                reveiews.ConclusionName = model.ConclusionName;
                reveiews.OtherInfo = model.OtherInfo;
                reveiews.InvitedExperts = model.InvitedExperts;
                reveiews.Attendees = model.Attendees;
                reveiews.PerfectContent = model.PerfectContent;
                reveiews.State = model.State;

                SetCurrentUser(reveiews);
                //上传附件
                AddFilesBytzTable(reveiews, model.TzAttachs);

                var projects = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId);
                //特许经营项目性质的项目审批到项目批复完成就结束了，不生成工程模块的内容，不用写到机器人表里
                if (projects.Nature != "XMXZTXJY")
                {
                    #region  项目评审记录流程申请     暂时注释  勿删！！！
                    var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                    if (model.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                    {
                        TzProjectReveiewsWorkFlowView view = new TzProjectReveiewsWorkFlowView();

                        var subjects = DataOperateBasic<Base_TypeDictionary>.Get().GetList(t => t.Type == DictionaryType.OtherInfo.ToString()).ToList();
                        string str = "";
                        foreach (var item in subjects)
                        {
                            if (subjects.Where(t => model.OtherInfo.Contains(t.No)).Any())
                            {
                                str = str + item.Name + ",";
                            }
                        }
                        str = str.Substring(0, str.Length - 1);
                        view.ProjectName = projects.ProjectName;
                        view.StationName = projects.StationName;
                        view.NatureName = projects.NatureName;
                        view.Position = projects.Position;
                        view.ApplyTime = projects.ApplyTime.ToString();
                        view.CompanyName = projects.CompanyName;
                        view.PredictMoney = projects.PredictMoney.ToString();
                        view.ReveiewDate = model.ReveiewDate.ToString();
                        view.HostUser = model.HostUser;
                        view.Attendees = model.Attendees;
                        view.ConclusionName = model.ConclusionName;
                        view.OtherInfo = str;
                        view.Address = model.Address;
                        view.PerfectContent = model.PerfectContent;
                        view.InvitedExperts = model.InvitedExperts;
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

                        reveiews.WorkFlowId = XtWorkFlowService.CreateProjectReveiewsWorkFlow(view);
                    }
                    #endregion
                }

                if (isAdd)
                {
                    rows = DataOperateBusiness<Epm_TzProjectReveiews>.Get().Add(reveiews);
                }
                else
                {
                    rows = DataOperateBusiness<Epm_TzProjectReveiews>.Get().Update(reveiews);
                }

                
                result.Data = rows;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzProjectReveiews");
            }
            return result;
        }
        ///<summary>
        ///修改:项目评审记录
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzProjectReveiews(Epm_TzProjectReveiews model)
        {
            Result<int> result = new Result<int>();
            try
            {
                #region  项目评审记录流程申请     暂时注释  勿删！！！
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                {
                    TzProjectReveiewsWorkFlowView view = new TzProjectReveiewsWorkFlowView();

                    var project = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId);
                    view.ProjectName = project.ProjectName;
                    view.StationName = project.StationName;
                    view.NatureName = project.NatureName;
                    view.Position = project.Position;
                    view.ApplyTime = project.ApplyTime.ToString();
                    view.CompanyName = project.CompanyName;
                    view.PredictMoney = project.ProjectCode;
                    view.ReveiewDate = DateTime.Now.ToString();
                    view.HostUser = model.HostUser;
                    view.Attendees = model.Attendees;
                    view.ConclusionName = model.ConclusionName;
                    view.OtherInfo = model.InvitedExperts;
                    view.Address = model.Address;
                    view.PerfectContent = model.PerfectContent;
                    view.InvitedExperts = model.InvitedExperts;
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

                    model.WorkFlowId = XtWorkFlowService.CreateProjectReveiewsWorkFlow(view);
                }
                #endregion

                var rows = DataOperateBusiness<Epm_TzProjectReveiews>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzProjectReveiews.GetText(), SystemRight.Modify.GetText(), "修改项目评审记录: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzProjectReveiews");
            }
            return result;
        }
        ///<summary>
        ///删除:项目评审记录
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzProjectReveiewsByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzProjectReveiews>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzProjectReveiews>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzProjectReveiews.GetText(), SystemRight.Delete.GetText(), "批量删除项目评审记录: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzProjectReveiewsByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:项目评审记录
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectProposal>> GetTzProjectReveiewsList(QueryCondition qc)
        {
            Result<List<Epm_TzProjectProposal>> result = new Result<List<Epm_TzProjectProposal>>();
            try
            {
                var query = from reveiews in context.Epm_TzProjectReveiews.Where(p => p.IsDelete == false)
                            join project in context.Epm_TzProjectProposal.Where(p => p.IsDelete == false) on reveiews.ProjectId equals project.Id
                            select new
                            {
                                reveiews.ProjectId,
                                project.ProjectName,
                                project.Nature,
                                project.NatureName,
                                project.StationName,
                                project.StationId,
                                project.ApplyTime,
                                reveiews.State,
                                reveiews.OperateUserName,
                                reveiews.OperateTime,
                                reveiews.CreateUserName,
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
                    State = c.State.Value,
                   // OperateUserName = c.OperateUserName,
                    OperateUserName = c.State == (int)PreProjectState.WaitSubmitted || c.State == (int)PreProjectState.ApprovalFailure ? c.CreateUserName : c.State == (int)PreProjectState.WaitApproval ? c.OperateUserName : "",
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
                result.Exception = new ExceptionEx(ex, "GetTzFirstNegotiationList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:项目评审记录
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<TzProjectReveiewsView> GetTzProjectReveiewsModel(long projectId)
        {
            Result<TzProjectReveiewsView> result = new Result<TzProjectReveiewsView>();
            try
            {
                TzProjectReveiewsView view = new TzProjectReveiewsView();
                var projectModel = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(projectId);
                if (projectModel != null)
                {
                    view.TzProjectProposal = projectModel;
                }

                var tzProjectReveiews = DataOperateBusiness<Epm_TzProjectReveiews>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();
                
                if (tzProjectReveiews != null)
                {
                    List<Epm_TzAttachs> tzAttachsList = new List<Epm_TzAttachs>();
                    tzAttachsList = GetFilesByTZTable("Epm_TzProjectReveiews", tzProjectReveiews.Id).Data;
                    if (tzAttachsList != null && tzAttachsList.Any())
                    {
                        tzProjectReveiews.TzAttachs = tzAttachsList;
                    }
                    view.TzProjectReveiews = tzProjectReveiews;
                }
                result.Data = view;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzProjectReveiewsModel");
            }
            return result;
        }


        /// <summary>
        /// 修改项目评审记录状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzProjectReveiewsState(List<long> ids, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                foreach (var item in ids)
                {
                    var model = DataOperateBusiness<Epm_TzProjectReveiews>.Get().GetList(t => t.ProjectId == item).FirstOrDefault();
                    if (model != null)
                    {
                        //SetCurrentUser(model);
                        model.State = (int)state.ToEnumReq<PreProjectState>();
                        var rows = DataOperateBusiness<Epm_TzProjectReveiews>.Get().Update(model);

                        //若状态为已审批，生成下一阶段数据
                        if (model.State == (int)PreProjectState.ApprovalSuccess)
                        {
                            //生成上会材料上报信息
                            Epm_MeetingFileReport meetingFileReport = new Epm_MeetingFileReport();
                            meetingFileReport.ProjectId = model.ProjectId;
                            meetingFileReport.State = (int)PreProjectState.WaitSubmitted;
                            AddMeetingFileReport(meetingFileReport);
                        }

                        result.Data = rows;
                        result.Flag = EResultFlag.Success;

                       
                        #region 地区公司方案审核
                            var project = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId);
                            string houstAddress = System.Configuration.ConfigurationManager.AppSettings.Get("RPAPath");
                            #region 生成RPA表数据
                            RPA_CompanyProjectReveiews rPACompanyReveiews = new RPA_CompanyProjectReveiews();

                            rPACompanyReveiews.ProjectName = project.ProjectName;
                            rPACompanyReveiews.Address = model.Address;
                            rPACompanyReveiews.Attendees = model.Attendees;
                            rPACompanyReveiews.ReveiewDate = model.ReveiewDate;
                            rPACompanyReveiews.HostUser = model.HostUser;
                            rPACompanyReveiews.username = "sxgcyw";
                            rPACompanyReveiews.WriteMark = 0;
                            rPACompanyReveiews.WriteResult = 0;
                            rPACompanyReveiews.FollowOperate = "";
                            rPACompanyReveiews.ConclusionName = model.ConclusionCode == "XMPSJL1" ? "同意" : model.ConclusionCode == "XMPSJL2" ? "原则同意" : model.ConclusionCode == "XMPSJL3" ? "不同意" : "";
                            rPACompanyReveiews.InvitedExperts = model.InvitedExperts;

                            var rcfileList = GetFilesByTZTable("Epm_TzProjectReveiews", model.Id).Data;
                            for (int i = 0; i < rcfileList.Count; i++)
                            {
                                rPACompanyReveiews.FilePath += houstAddress + "?path=" + rcfileList[i].FilePath + ",";
                            }
                            rPACompanyReveiews.FileNumber = rcfileList.Count;

                            SetCreateUser(rPACompanyReveiews);
                            SetCurrentUser(rPACompanyReveiews);
                            DataOperateBusiness<RPA_CompanyProjectReveiews>.Get().Add(rPACompanyReveiews);
                            #endregion
                            #region 生成OMADS表数据
                            OMADS_CompanyProjectReveiews oMADSCompanyReveiews = new OMADS_CompanyProjectReveiews();

                            oMADSCompanyReveiews.ProjectName = project.ProjectName;
                            oMADSCompanyReveiews.Address = model.Address;
                            oMADSCompanyReveiews.Attendees = model.Attendees;
                            oMADSCompanyReveiews.ReveiewDate = model.ReveiewDate;
                            oMADSCompanyReveiews.HostUser = model.HostUser;
                            oMADSCompanyReveiews.username = "sxgcyw";
                            oMADSCompanyReveiews.WriteMark = 0;
                            oMADSCompanyReveiews.WriteResult = 0;
                            oMADSCompanyReveiews.FollowOperate = "";
                            oMADSCompanyReveiews.ConclusionName = model.ConclusionCode == "XMPSJL1" ? "同意" : model.ConclusionCode == "XMPSJL2" ? "原则同意" : model.ConclusionCode == "XMPSJL3" ? "不同意" : "";
                            oMADSCompanyReveiews.InvitedExperts = model.InvitedExperts;
                            var ocfileList = GetFilesByTZTable("Epm_TzProjectReveiews", model.Id).Data;
                            for (int i = 0; i < ocfileList.Count; i++)
                            {
                                oMADSCompanyReveiews.FilePath += houstAddress + "?path=" + ocfileList[i].FilePath + ",";
                            }
                            oMADSCompanyReveiews.FileNumber = ocfileList.Count;

                            SetCreateUser(oMADSCompanyReveiews);
                            SetCurrentUser(oMADSCompanyReveiews);
                            DataOperateBusiness<OMADS_CompanyProjectReveiews>.Get().Add(oMADSCompanyReveiews);
                            #endregion
                            #region 生成TEMP表数据
                            TEMP_CompanyProjectReveiews tEMPCompanyReveiews = new TEMP_CompanyProjectReveiews();

                            tEMPCompanyReveiews.ProjectName = project.ProjectName;
                            tEMPCompanyReveiews.Address = model.Address;
                            tEMPCompanyReveiews.Attendees = model.Attendees;
                            tEMPCompanyReveiews.ReveiewDate = model.ReveiewDate;
                            tEMPCompanyReveiews.HostUser = model.HostUser;
                            tEMPCompanyReveiews.username = "sxgcyw";
                            tEMPCompanyReveiews.WriteMark = 0;
                            tEMPCompanyReveiews.WriteResult = 0;
                            tEMPCompanyReveiews.FollowOperate = "";
                            tEMPCompanyReveiews.ConclusionName = model.ConclusionCode == "XMPSJL1" ? "同意" : model.ConclusionCode == "XMPSJL2" ? "原则同意" : model.ConclusionCode == "XMPSJL3" ? "不同意" : "";
                            tEMPCompanyReveiews.InvitedExperts = model.InvitedExperts;
                            var tcfileList = GetFilesByTZTable("Epm_TzProjectReveiews", model.Id).Data;
                            for (int i = 0; i < tcfileList.Count; i++)
                            {
                                tEMPCompanyReveiews.FilePath += houstAddress + "?path=" + tcfileList[i].FilePath + ",";
                            }
                            tEMPCompanyReveiews.FileNumber = tcfileList.Count;

                            SetCreateUser(tEMPCompanyReveiews);
                            SetCurrentUser(tEMPCompanyReveiews);
                            DataOperateBusiness<TEMP_CompanyProjectReveiews>.Get().Add(tEMPCompanyReveiews);
                            #endregion
                            #endregion

                        #region 板块公司方案审查审核
                            #region 生成RPA表数据
                            RPA_BoardCompanyReveiews rPABoardReveiews = new RPA_BoardCompanyReveiews();

                            rPABoardReveiews.ProjectName = project.ProjectName;
                            rPABoardReveiews.Address = model.Address;
                            rPABoardReveiews.Attendees = model.Attendees;
                            rPABoardReveiews.ReveiewDate = model.ReveiewDate;
                            rPABoardReveiews.HostUser = model.HostUser;
                            rPABoardReveiews.username = "sxgcyw";
                            rPABoardReveiews.WriteMark = 0;
                            rPABoardReveiews.WriteResult = 0;
                            rPABoardReveiews.FollowOperate = "";

                            var rbfileList = GetFilesByTZTable("Epm_TzProjectReveiews", model.Id).Data;
                            for (int i = 0; i < rbfileList.Count; i++)
                            {
                                rPABoardReveiews.FilePath += houstAddress + "?path=" + rbfileList[i].FilePath + ",";
                            }
                            rPABoardReveiews.FileNumber = rbfileList.Count;

                            SetCreateUser(rPABoardReveiews);
                            SetCurrentUser(rPABoardReveiews);
                            DataOperateBusiness<RPA_BoardCompanyReveiews>.Get().Add(rPABoardReveiews);
                            #endregion
                            #region 生成OMADS表数据
                            OMADS_BoardCompanyReveiews oMADSBoardReveiews = new OMADS_BoardCompanyReveiews();

                            oMADSBoardReveiews.ProjectName = project.ProjectName;
                            oMADSBoardReveiews.Address = model.Address;
                            oMADSBoardReveiews.Attendees = model.Attendees;
                            oMADSBoardReveiews.ReveiewDate = model.ReveiewDate;
                            oMADSBoardReveiews.HostUser = model.HostUser;
                            oMADSBoardReveiews.username = "sxgcyw";
                            oMADSBoardReveiews.WriteMark = 0;
                            oMADSBoardReveiews.WriteResult = 0;
                            oMADSBoardReveiews.FollowOperate = "";

                            var obfileList = GetFilesByTZTable("Epm_TzProjectReveiews", model.Id).Data;
                            for (int i = 0; i < obfileList.Count; i++)
                            {
                                oMADSBoardReveiews.FilePath += houstAddress + "?path=" + obfileList[i].FilePath + ",";
                            }
                            oMADSBoardReveiews.FileNumber = obfileList.Count;

                            SetCreateUser(oMADSBoardReveiews);
                            SetCurrentUser(oMADSBoardReveiews);
                            DataOperateBusiness<OMADS_BoardCompanyReveiews>.Get().Add(oMADSBoardReveiews);
                            #endregion
                            #region 生成TEMP表数据
                            TEMP_BoardCompanyReveiews tEMPBoardReveiews = new TEMP_BoardCompanyReveiews();

                            tEMPBoardReveiews.ProjectName = project.ProjectName;
                            tEMPBoardReveiews.Address = model.Address;
                            tEMPBoardReveiews.Attendees = model.Attendees;
                            tEMPBoardReveiews.ReveiewDate = model.ReveiewDate;
                            tEMPBoardReveiews.HostUser = model.HostUser;
                            tEMPBoardReveiews.username = "sxgcyw";
                            tEMPBoardReveiews.WriteMark = 0;
                            tEMPBoardReveiews.WriteResult = 0;
                            tEMPBoardReveiews.FollowOperate = "";

                            var tbfileList = GetFilesByTZTable("Epm_TzProjectReveiews", model.Id).Data;
                            for (int i = 0; i < tbfileList.Count; i++)
                            {
                                tEMPBoardReveiews.FilePath += houstAddress + "?path=" + tbfileList[i].FilePath + ",";
                            }
                            tEMPBoardReveiews.FileNumber = tbfileList.Count;

                            SetCreateUser(tEMPBoardReveiews);
                            SetCurrentUser(tEMPBoardReveiews);
                            DataOperateBusiness<TEMP_BoardCompanyReveiews>.Get().Add(tEMPBoardReveiews);
                            #endregion
                            #endregion
                        
                    }
                    else
                    {
                        throw new Exception("该项目评审记录信息不存在");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzProjectReveiewsState");
            }
            return result;
        }
    }
}
