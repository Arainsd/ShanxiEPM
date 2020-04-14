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
    /// 上会材料上报
    /// </summary>
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddMeetingFileReport(Epm_MeetingFileReport model)
        {
            Result<int> result = new Result<int>();
            try
            {
                int rows = 0;
                bool isAdd = false;
                var meeting = DataOperateBusiness<Epm_MeetingFileReport>.Get().GetList(t => t.ProjectId == model.ProjectId && t.State != (int)PreProjectState.ApprovalFailure && t.State != (int)PreProjectState.Discarded).FirstOrDefault();

                if (meeting == null)
                {
                    isAdd = true;
                    meeting = new Epm_MeetingFileReport();
                    SetCreateUser(meeting);
                }
                meeting.ProjectId = model.ProjectId;
                meeting.State = model.State;
                SetCurrentUser(meeting);

                //上传附件
                AddFilesBytzTable(meeting, model.TzAttachs);

                var projectInfo = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId);
                //特许经营项目性质的项目审批到项目批复完成就结束了，不生成工程模块的内容，不用写到机器人表里
                if (projectInfo.Nature != "XMXZTXJY")
                {
                    #region  上会材料上报流程申请
                    var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                    if (model.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                    {
                        TzMeetingFileReportWorkFlowView view = new TzMeetingFileReportWorkFlowView();
                        var project = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId);
                        view.ProjectName = project.ProjectName;
                        view.StationName = project.StationName;
                        view.NatureName = project.NatureName;
                        view.Position = project.Position;
                        view.ApplyTime = string.Format("{0:yyyy-MM-dd}", project.ApplyTime);
                        view.CompanyName = project.CompanyName;
                        view.PredictMoney = project.PredictMoney.ToString();
                        view.OilSalesTotal = project.OilSalesTotal.ToString();
                        view.CNGY = model.CNG.ToString();
                        view.LNGQ = model.LNG.ToString();
                        var baseUser = DataOperateBasic<Base_User>.Get().GetModel(meeting.CreateUserId);
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

                        meeting.WorkFlowId = XtWorkFlowService.CreateMeetingFileReportWorkFlow(view);
                    }
                    #endregion
                }

                if (isAdd)
                {
                    rows = DataOperateBusiness<Epm_MeetingFileReport>.Get().Add(meeting);
                }
                else
                {
                    rows = DataOperateBusiness<Epm_MeetingFileReport>.Get().Update(meeting);
                }

                
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.MeetingFileReport.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddMeetingFileReport");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateMeetingFileReport(Epm_MeetingFileReport model)
        {
            Result<int> result = new Result<int>();
            try
            {
                #region  上会材料上报流程申请     暂时注释  勿删！！！
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                {
                    TzMeetingFileReportWorkFlowView view = new TzMeetingFileReportWorkFlowView();
                    var project = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId);
                    view.ProjectName = project.ProjectName;
                    view.StationName = project.StationName;
                    view.NatureName = project.NatureName;
                    view.Position = project.Position;
                    view.ApplyTime = project.ApplyTime.ToString();
                    view.CompanyName = project.CompanyName;
                    view.PredictMoney = project.ProjectCode;
                    view.OilSalesTotal = project.OilSalesTotal.ToString();
                    view.CNGY = model.CNG.ToString();
                    view.LNGQ = model.LNG.ToString();
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

                    model.WorkFlowId = XtWorkFlowService.CreateMeetingFileReportWorkFlow(view);
                }
                #endregion

                var rows = DataOperateBusiness<Epm_MeetingFileReport>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.MeetingFileReport.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateMeetingFileReport");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteMeetingFileReportByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_MeetingFileReport>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_MeetingFileReport>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.MeetingFileReport.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteMeetingFileReportByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectProposal>> GetMeetingFileReportList(QueryCondition qc)
        {
            Result<List<Epm_TzProjectProposal>> result = new Result<List<Epm_TzProjectProposal>>();
            try
            {
                var query = from meeting in context.Epm_MeetingFileReport.Where(p => p.IsDelete == false)
                            join project in context.Epm_TzProjectProposal.Where(p => p.IsDelete == false) on meeting.ProjectId equals project.Id
                            select new
                            {
                                meeting.ProjectId,
                                project.ProjectName,
                                project.Nature,
                                project.NatureName,
                                project.StationName,
                                project.StationId,
                                project.ApplyTime,
                                meeting.State,
                                meeting.OperateUserName,
                                meeting.OperateTime,
                                meeting.CreateUserName,
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
                    State = c.State,
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
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<MeetingFileReportView> GetMeetingFileReportModel(long projectId)
        {
            Result<MeetingFileReportView> result = new Result<MeetingFileReportView>();
            try
            {
                MeetingFileReportView view = new MeetingFileReportView();
                var projectModel = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(projectId);
                if (projectModel != null)
                {
                    view.TzProjectProposal = projectModel;
                }

                var meetingFileReport = DataOperateBusiness<Epm_MeetingFileReport>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();
                
                if (meetingFileReport != null)
                {
                    List<Epm_TzAttachs> tzAttachsList = new List<Epm_TzAttachs>();
                    tzAttachsList = GetFilesByTZTable("Epm_MeetingFileReport", meetingFileReport.Id).Data;
                    if (tzAttachsList != null && tzAttachsList.Any())
                    {
                        meetingFileReport.TzAttachs = tzAttachsList;
                    }
                    view.MeetingFileReport = meetingFileReport;
                }
                result.Data = view;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMeetingFileReportModel");
            }
            return result;
        }

        /// <summary>
        /// 修改上会材料上报状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateMeetingFileReportState(List<long> ids, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                foreach (var item in ids)
                {
                    var model = DataOperateBusiness<Epm_MeetingFileReport>.Get().GetList(t => t.ProjectId == item).FirstOrDefault();
                    if (model != null)
                    {
                        //SetCurrentUser(model);
                        model.State = (int)state.ToEnumReq<PreProjectApprovalState>();
                        var rows = DataOperateBusiness<Epm_MeetingFileReport>.Get().Update(model);

                        //若状态为已审批，生成下一阶段数据
                        if (model.State == (int)PreProjectState.ApprovalSuccess)
                        {
                            var ProjectProposal = DataOperateBusiness<Epm_TzProjectProposal>.Get().Single(p=>p.Id==model.ProjectId);//读取项目提出的数据
                            //生成项目批复信息
                            Epm_TzProjectApprovalInfo tzProjectApprovalInfo = new Epm_TzProjectApprovalInfo();
                            tzProjectApprovalInfo.ProjectId = model.ProjectId;
                            tzProjectApprovalInfo.State = (int)PreProjectState.WaitSubmitted;
                            //tzProjectApprovalInfo.ProjectType= ProjectProposal
                            tzProjectApprovalInfo.RegionCompany ="陕西";
                            tzProjectApprovalInfo.ProjectTypeCode = ProjectProposal.ProjectCode;
                           
                            tzProjectApprovalInfo.ProjectType = ProjectProposal.ProjectType;
                            tzProjectApprovalInfo.ApprovalNo = ProjectProposal.ApprovalNo;
                            AddTzProjectApprovalInfo(tzProjectApprovalInfo);
                        }

                        result.Data = rows;
                        result.Flag = EResultFlag.Success;
                    }
                    else
                    {
                        throw new Exception("该上会材料上报信息不存在");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateMeetingFileReportState");
            }
            return result;
        }
    }
}
