/************************************************************************************
 * Copyright (c) 2019  All Rights Reserved.
 * CLR版本： 4.0.30319.42000
 * 公司名称：陕西华春网络科技股份有限公司
 * 命名空间：hc.epm.Service.ClientSite
 * 文件名：  TzFormTalkFileService
 * 版本号：  V1.0.0.0
 * 创建人：  wmg	
 * 电子邮箱：wmgwugang@huachun.com
 * 创建时间：2019/8/23 16:03:02
 * 描述：
 * 
 * 
 * 
 ************************************************************************************/

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
    /// 评审材料上报
    /// </summary>
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzFormTalkFile(Epm_TzFormTalkFile model)
        {
            Result<int> result = new Result<int>();
            try
            {
                int rows = 0;
                bool isAdd = false;
                var tzFormTalkFile = DataOperateBusiness<Epm_TzFormTalkFile>.Get().GetList(t => t.ProjectId == model.ProjectId && t.State != (int)PreProjectState.ApprovalFailure && t.State != (int)PreProjectState.Discarded).FirstOrDefault();

                if (tzFormTalkFile == null)
                {
                    isAdd = true;
                    tzFormTalkFile = new Epm_TzFormTalkFile();
                    SetCreateUser(tzFormTalkFile);
                }
                tzFormTalkFile.ProjectId = model.ProjectId;
                tzFormTalkFile.ProjectLeaderId = model.ProjectLeaderId;
                tzFormTalkFile.ProjectLeaderName = model.ProjectLeaderName;
                tzFormTalkFile.ProjectLeaderXT = model.ProjectLeaderXT;
                tzFormTalkFile.ProjectDecisionerId = model.ProjectDecisionerId;
                tzFormTalkFile.ProjectDecisionerName = model.ProjectDecisionerName;
                tzFormTalkFile.ProjectDecisionerIdXT = model.ProjectDecisionerIdXT;
                tzFormTalkFile.WriterId = model.WriterId;
                tzFormTalkFile.WriterName = model.WriterName;
                tzFormTalkFile.WriterIdXT = model.WriterIdXT;
                tzFormTalkFile.Remark = model.Remark;
                tzFormTalkFile.State = model.State;
                SetCurrentUser(tzFormTalkFile);
                //上传附件
                AddFilesBytzTable(tzFormTalkFile, model.TzAttachs);

                var projectInfo = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId.Value);
                //特许经营项目性质的项目审批到项目批复完成就结束了，不生成工程模块的内容，不用写到机器人表里
                if (projectInfo.Nature != "XMXZTXJY")
                {
                    #region  评审材料上报流程申请
                    var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                    if (model.State == (int)XtBusinessDataState.Auditing && XtWorkFlow == "1")
                    {
                        TzFormTalkFileWorkFlowView view = new TzFormTalkFileWorkFlowView();

                        var project = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId.Value);
                        view.ProjectName = project.ProjectName;
                        view.StationName = project.StationName;
                        view.NatureName = project.NatureName;
                        view.Position = project.Position;
                        view.ApplyTime = string.Format("{0:yyyy-MM-dd}", project.ApplyTime);
                        view.CompanyName = project.CompanyName;
                        view.PredictMoney = project.PredictMoney.ToString();
                        view.ProjectLeaderName = model.ProjectLeaderName;
                        view.ProjectDecisionerName = model.ProjectDecisionerName;
                        view.WriterName = model.WriterName;
                        var baseUser = DataOperateBasic<Base_User>.Get().GetModel(tzFormTalkFile.CreateUserId);
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

                        tzFormTalkFile.WorkFlowId = XtWorkFlowService.CreateFormTalkFileWorkFlow(view);
                    }
                    #endregion
                }
                if (isAdd)
                {
                    if (projectInfo.Nature != "XMXZTXJY")
                    {
                        #region 自动生成RPA数据
                        var RPAModel = new RPA_TzFormTalkFileHX();

                        RPAModel.ProjectDecisionerName = model.ProjectDecisionerName;
                        RPAModel.ProjectLeaderName = model.ProjectLeaderName;
                        RPAModel.Remark = model.Remark;
                        RPAModel.username = CurrentUser.UserAcct;
                        RPAModel.companys = CurrentUser.CompanyName;
                        RPAModel.WriteMark = 0;
                        RPAModel.WriteResult = 0;
                        RPAModel.FollowOperate = "";

                        var fileList = GetFilesByTZTable("Epm_TzFormTalkFile", model.Id).Data;
                        for (int i = 0; i < fileList.Count; i++)
                        {
                            RPAModel.FilePath += fileList[i].FilePath + ",";
                        }
                        RPAModel.FileNumber = fileList.Count;
                        RPAModel.status1 = 0;
                        RPAModel.status2 = 0;
                        SetCreateUser(RPAModel);
                        SetCurrentUser(RPAModel);
                        DataOperateBusiness<RPA_TzFormTalkFileHX>.Get().Add(RPAModel);
                        #endregion
                        #region 自动生成OMADS数据
                        var oMADSModel = new OMADS_TzFormTalkFileHX();

                        oMADSModel.ProjectDecisionerName = model.ProjectDecisionerName;
                        oMADSModel.ProjectLeaderName = model.ProjectLeaderName;
                        oMADSModel.Remark = model.Remark;
                        oMADSModel.username = CurrentUser.UserAcct;
                        oMADSModel.companys = CurrentUser.CompanyName;
                        oMADSModel.WriteMark = 0;
                        oMADSModel.WriteResult = 0;
                        oMADSModel.FollowOperate = "";

                        var ofileList = GetFilesByTZTable("Epm_TzFormTalkFile", model.Id).Data;
                        for (int i = 0; i < ofileList.Count; i++)
                        {
                            oMADSModel.FilePath += ofileList[i].FilePath + ",";
                        }
                        oMADSModel.FileNumber = ofileList.Count;
                        oMADSModel.status2 = 0;
                        oMADSModel.status1 = 0;
                        SetCreateUser(oMADSModel);
                        SetCurrentUser(oMADSModel);
                        DataOperateBusiness<OMADS_TzFormTalkFileHX>.Get().Add(oMADSModel);
                        #endregion
                        #region 自动生成TEMP数据
                        var tEMPModel = new TEMP_TzFormTalkFileHX();

                        tEMPModel.ProjectDecisionerName = model.ProjectDecisionerName;
                        tEMPModel.ProjectLeaderName = model.ProjectLeaderName;
                        tEMPModel.Remark = model.Remark;
                        tEMPModel.username = CurrentUser.UserAcct;
                        tEMPModel.companys = CurrentUser.CompanyName;
                        tEMPModel.WriteMark = 0;
                        tEMPModel.WriteResult = 0;
                        tEMPModel.FollowOperate = "";

                        var tfileList = GetFilesByTZTable("Epm_TzFormTalkFile", model.Id).Data;
                        for (int i = 0; i < tfileList.Count; i++)
                        {
                            tEMPModel.FilePath += tfileList[i].FilePath + ",";
                        }
                        tEMPModel.FileNumber = tfileList.Count;
                        tEMPModel.status1 = 0;
                        tEMPModel.status2 = 0;
                        SetCreateUser(tEMPModel);
                        SetCurrentUser(tEMPModel);
                        DataOperateBusiness<TEMP_TzFormTalkFileHX>.Get().Add(tEMPModel);
                        #endregion
                    }
                    rows = DataOperateBusiness<Epm_TzFormTalkFile>.Get().Add(tzFormTalkFile);
                }
                else
                {
                    rows = DataOperateBusiness<Epm_TzFormTalkFile>.Get().Update(tzFormTalkFile);
                }

                                
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzFormTalkFile.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzFormTalkFile");
            }
            return result;
        }

        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzFormTalkFile(Epm_TzFormTalkFile model)
        {
            Result<int> result = new Result<int>();
            try
            {
                #region  评审材料上报流程申请     暂时注释  勿删！！！
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)XtBusinessDataState.Auditing && XtWorkFlow == "1")
                {
                    TzFormTalkFileWorkFlowView view = new TzFormTalkFileWorkFlowView();

                    //view.ProjectName = model.;
                    //view.StationName = model.StationName;
                    //view.NatureName = model.NatureName;
                    //view.Position = model.Position;
                    //view.ApplyTime = model.ApplyTime;
                    //view.CompanyName = ;
                    //view.PredictMoney = model.ProjectCode;
                    //view.ProjectLeaderName = model.ProjectName;
                    //view.ProjectDecisionerName = model.CompanyName;
                    //view.WriterName = model.ReviewAddress;
                    //view.Temp_TzAttachs = model.ReviewExperts;


                    //上传附件
                    //if (model.TzAttachs != null && model.TzAttachs.Any())
                    //{
                    //    string baseFaleUrl = System.Configuration.ConfigurationManager.AppSettings.Get("XtDownloadUrl");
                    //    foreach (var item in model.TzAttachs)
                    //    {
                    //        string fileUrl = string.Format("{0}?fileId={1}&type={2}", baseFaleUrl, item.Id, item.TypeNo);
                    //        switch (item.TypeNo)
                    //        {
                    //            case "XMGLJGWJ":        // 项目管理机构(项目经理部或油库项目组)设立的文件、机构组成和职责分工各一份
                    //                view.file_xmgljg = fileUrl + '|' + view.file_xmgljg;
                    //                break;

                    //            default:
                    //                break;
                    //        }
                    //    }

                    //    if (view.file_zyzds != null)
                    //    {
                    //        view.file_zyzds = view.file_zyzds.Substring(0, view.file_zyzds.Length - 1);
                    //    }
                    //}

                    //model.WorkFlowId = XtWorkFlowService.CreateConDrawingWorkFlow(view);
                }
                #endregion
                var rows = DataOperateBusiness<Epm_TzFormTalkFile>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzFormTalkFile.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzFormTalkFile");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzFormTalkFileByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzFormTalkFile>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzFormTalkFile>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzFormTalkFile.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzFormTalkFileByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectProposal>> GetTzFormTalkFileList(QueryCondition qc)
        {
            Result<List<Epm_TzProjectProposal>> result = new Result<List<Epm_TzProjectProposal>>();
            try
            {
                var query = from formTalk in context.Epm_TzFormTalkFile.Where(p => p.IsDelete == false)
                            join project in context.Epm_TzProjectProposal.Where(p => p.IsDelete == false) on formTalk.ProjectId equals project.Id
                            select new
                            {
                                formTalk.ProjectId,
                                project.ProjectName,
                                project.Nature,
                                project.NatureName,
                                project.StationName,
                                project.StationId,
                                project.ApplyTime,
                                formTalk.State,
                                formTalk.OperateUserName,
                                formTalk.OperateTime,
                                formTalk.CreateUserName,
                                project.CompanyId
                            };

                var projectList = query.ToList().Select(c => new Epm_TzProjectProposal
                {
                    Id = c.ProjectId.Value,
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
                result.Exception = new ExceptionEx(ex, "GetTzFormTalkFileList");
            }
            return result;
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<TzFormTalkFileView> GetTzFormTalkFileModel(long projectId)
        {
            Result<TzFormTalkFileView> result = new Result<TzFormTalkFileView>();
            try
            {
                TzFormTalkFileView view = new TzFormTalkFileView();
                var projectModel = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(projectId);
                if (projectModel != null)
                {
                    view.TzProjectProposal = projectModel;
                }

                var tzFormTalkFile = DataOperateBusiness<Epm_TzFormTalkFile>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();

                if (tzFormTalkFile != null)
                {
                    List<Epm_TzAttachs> tzAttachsList = new List<Epm_TzAttachs>();
                    tzAttachsList = GetFilesByTZTable("Epm_TzFormTalkFile", tzFormTalkFile.Id).Data;
                    if (tzAttachsList != null && tzAttachsList.Any())
                    {
                        tzFormTalkFile.TzAttachs = tzAttachsList;
                    }
                    view.TzFormTalkFile = tzFormTalkFile;
                }
                result.Data = view;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzFormTalkFileModel");
            }
            return result;
        }

        /// <summary>
        /// 修改评审材料上报状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzFormTalkFileState(List<long> ids, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                foreach (var item in ids)
                {
                    var model = DataOperateBusiness<Epm_TzFormTalkFile>.Get().GetList(t => t.ProjectId == item).FirstOrDefault();
                    if (model != null)
                    {
                        //SetCurrentUser(model);
                        model.State = (int)state.ToEnumReq<PreProjectApprovalState>();
                        var rows = DataOperateBusiness<Epm_TzFormTalkFile>.Get().Update(model);

                        //若状态为已审批，生成下一阶段数据
                        if (model.State == (int)PreProjectState.ApprovalSuccess)
                        {
                            //生成评审记录信息
                            Epm_TzProjectReveiews tzProjectReveiews = new Epm_TzProjectReveiews();
                            tzProjectReveiews.ProjectId = model.ProjectId.Value;
                            tzProjectReveiews.State = (int)PreProjectState.WaitSubmitted;
                            AddTzProjectReveiews(tzProjectReveiews);
                        }

                        var projectInfo = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(item);
                        //特许经营项目性质的项目审批到项目批复完成就结束了，不生成工程模块的内容，不用写到机器人表里
                        if (projectInfo.Nature != "XMXZTXJY")
                        {
                            #region 自动生成RPA数据
                            var RPAModel = new RPA_TzFormTalkFileHX();
                            var project = DataOperateBusiness<Epm_TzProjectProposal>.Get().Single(t => t.Id == model.ProjectId);
                            var hostAddress = System.Configuration.ConfigurationManager.AppSettings.Get("RPAPath");
                            RPAModel.ProjectDecisionerName = model.ProjectDecisionerName;
                            RPAModel.ProjectLeaderName = model.ProjectLeaderName;
                            RPAModel.Remark = model.Remark;
                            RPAModel.username = "sxxayw";
                            RPAModel.companys = CurrentUser.CompanyName;
                            RPAModel.WriteMark = 0;
                            RPAModel.WriteResult = 0;
                            RPAModel.FollowOperate = "";
                            RPAModel.ProjectName = project.ProjectName;
                            var fileList = GetFilesByTZTable("Epm_TzFormTalkFile", model.Id).Data;
                            for (int i = 0; i < fileList.Count; i++)
                            {
                                RPAModel.FilePath += hostAddress + "?path=" + fileList[i].FilePath + "&fileName=" + fileList[i].Name + ",";
                            }
                            RPAModel.FileNumber = fileList.Count;
                            RPAModel.status1 = 0;
                            RPAModel.status2 = 0;
                            SetCreateUser(RPAModel);
                            SetCurrentUser(RPAModel);
                            DataOperateBusiness<RPA_TzFormTalkFileHX>.Get().Add(RPAModel);
                            #endregion
                            #region 自动生成OMADS数据
                            var oMADSModel = new OMADS_TzFormTalkFileHX();

                            oMADSModel.ProjectDecisionerName = model.ProjectDecisionerName;
                            oMADSModel.ProjectLeaderName = model.ProjectLeaderName;
                            oMADSModel.Remark = model.Remark;
                            oMADSModel.username = "sxxayw";
                            oMADSModel.companys = CurrentUser.CompanyName;
                            oMADSModel.WriteMark = 0;
                            oMADSModel.WriteResult = 0;
                            oMADSModel.FollowOperate = "";
                            oMADSModel.ProjectName = project.ProjectName;
                            var ofileList = GetFilesByTZTable("Epm_TzFormTalkFile", model.Id).Data;
                            for (int i = 0; i < ofileList.Count; i++)
                            {
                                oMADSModel.FilePath += hostAddress + "?path=" + ofileList[i].FilePath + "&fileName=" + ofileList[i].Name + ",";
                            }
                            oMADSModel.FileNumber = ofileList.Count;
                            oMADSModel.status2 = 0;
                            oMADSModel.status1 = 0;
                            SetCreateUser(oMADSModel);
                            SetCurrentUser(oMADSModel);
                            DataOperateBusiness<OMADS_TzFormTalkFileHX>.Get().Add(oMADSModel);
                            #endregion
                            #region 自动生成TEMP数据
                            var tEMPModel = new TEMP_TzFormTalkFileHX();

                            tEMPModel.ProjectDecisionerName = model.ProjectDecisionerName;
                            tEMPModel.ProjectLeaderName = model.ProjectLeaderName;
                            tEMPModel.Remark = model.Remark;
                            tEMPModel.username = "sxxayw";
                            tEMPModel.companys = CurrentUser.CompanyName;
                            tEMPModel.WriteMark = 0;
                            tEMPModel.WriteResult = 0;
                            tEMPModel.FollowOperate = "";
                            tEMPModel.ProjectName = project.ProjectName;
                            var tfileList = GetFilesByTZTable("Epm_TzFormTalkFile", model.Id).Data;
                            for (int i = 0; i < tfileList.Count; i++)
                            {
                                tEMPModel.FilePath += hostAddress + "?path=" + tfileList[i].FilePath + "&fileName=" + tfileList[i].Name + ",";
                            }
                            tEMPModel.FileNumber = tfileList.Count;
                            tEMPModel.status1 = 0;
                            tEMPModel.status2 = 0;
                            SetCreateUser(tEMPModel);
                            SetCurrentUser(tEMPModel);
                            DataOperateBusiness<TEMP_TzFormTalkFileHX>.Get().Add(tEMPModel);
                            #endregion
                        }
                        result.Data = rows;
                        result.Flag = EResultFlag.Success;
                    }
                    else
                    {
                        throw new Exception("该评审材料上报信息不存在");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzFormTalkFileState");
            }
            return result;
        }

        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzTalkFileAudit(Epm_TzTalkFileAudit model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzTalkFileAudit>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTalkFileAudit.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzTalkFileAudit");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzTalkFileAudit(Epm_TzTalkFileAudit model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzTalkFileAudit>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTalkFileAudit.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzTalkFileAudit");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzTalkFileAuditByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzTalkFileAudit>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzTalkFileAudit>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTalkFileAudit.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzTalkFileAuditByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzTalkFileAudit>> GetTzTalkFileAuditList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzTalkFileAudit>> result = new Result<List<Epm_TzTalkFileAudit>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzTalkFileAudit>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzTalkFileAuditList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzTalkFileAudit> GetTzTalkFileAuditModel(long id)
        {
            Result<Epm_TzTalkFileAudit> result = new Result<Epm_TzTalkFileAudit>();
            try
            {
                var model = DataOperateBusiness<Epm_TzTalkFileAudit>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzTalkFileAuditModel");
            }
            return result;
        }
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzTalkFileHeadAudit(Epm_TzTalkFileHeadAudit model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzTalkFileHeadAudit>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTalkFileHeadAudit.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzTalkFileHeadAudit");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzTalkFileHeadAudit(Epm_TzTalkFileHeadAudit model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzTalkFileHeadAudit>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTalkFileHeadAudit.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzTalkFileHeadAudit");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzTalkFileHeadAuditByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzTalkFileHeadAudit>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzTalkFileHeadAudit>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTalkFileHeadAudit.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzTalkFileHeadAuditByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzTalkFileHeadAudit>> GetTzTalkFileHeadAuditList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzTalkFileHeadAudit>> result = new Result<List<Epm_TzTalkFileHeadAudit>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzTalkFileHeadAudit>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzTalkFileHeadAuditList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzTalkFileHeadAudit> GetTzTalkFileHeadAuditModel(long id)
        {
            Result<Epm_TzTalkFileHeadAudit> result = new Result<Epm_TzTalkFileHeadAudit>();
            try
            {
                var model = DataOperateBusiness<Epm_TzTalkFileHeadAudit>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzTalkFileHeadAuditModel");
            }
            return result;
        }
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzTalkLeaderSign(Epm_TzTalkLeaderSign model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzTalkLeaderSign>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTalkLeaderSign.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzTalkLeaderSign");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzTalkLeaderSign(Epm_TzTalkLeaderSign model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzTalkLeaderSign>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTalkLeaderSign.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzTalkLeaderSign");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzTalkLeaderSignByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzTalkLeaderSign>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzTalkLeaderSign>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTalkLeaderSign.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzTalkLeaderSignByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzTalkLeaderSign>> GetTzTalkLeaderSignList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzTalkLeaderSign>> result = new Result<List<Epm_TzTalkLeaderSign>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzTalkLeaderSign>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzTalkLeaderSignList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzTalkLeaderSign> GetTzTalkLeaderSignModel(long id)
        {
            Result<Epm_TzTalkLeaderSign> result = new Result<Epm_TzTalkLeaderSign>();
            try
            {
                var model = DataOperateBusiness<Epm_TzTalkLeaderSign>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzTalkLeaderSignModel");
            }
            return result;
        }
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzTalkRecord(Epm_TzTalkRecord model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzTalkRecord>.Get().Add(model);
                AddFilesBytzTable(model, model.TzAttachs);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTalkRecord.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzTalkRecord");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzTalkRecord(Epm_TzTalkRecord model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzTalkRecord>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTalkRecord.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzTalkRecord");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzTalkRecordByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzTalkRecord>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzTalkRecord>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTalkRecord.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzTalkRecordByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzTalkRecord>> GetTzTalkRecordList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzTalkRecord>> result = new Result<List<Epm_TzTalkRecord>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzTalkRecord>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzTalkRecordList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzTalkRecord> GetTzTalkRecordModel(long id)
        {
            Result<Epm_TzTalkRecord> result = new Result<Epm_TzTalkRecord>();
            try
            {
                var model = DataOperateBusiness<Epm_TzTalkRecord>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzTalkRecordModel");
            }
            return result;
        }
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzTalkRecordConfirm(Epm_TzTalkRecordConfirm model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzTalkRecordConfirm>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTalkRecordConfirm.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzTalkRecordConfirm");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzTalkRecordConfirm(Epm_TzTalkRecordConfirm model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzTalkRecordConfirm>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTalkRecordConfirm.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzTalkRecordConfirm");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzTalkRecordConfirmByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzTalkRecordConfirm>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzTalkRecordConfirm>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTalkRecordConfirm.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzTalkRecordConfirmByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzTalkRecordConfirm>> GetTzTalkRecordConfirmList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzTalkRecordConfirm>> result = new Result<List<Epm_TzTalkRecordConfirm>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzTalkRecordConfirm>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzTalkRecordConfirmList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzTalkRecordConfirm> GetTzTalkRecordConfirmModel(long id)
        {
            Result<Epm_TzTalkRecordConfirm> result = new Result<Epm_TzTalkRecordConfirm>();
            try
            {
                var model = DataOperateBusiness<Epm_TzTalkRecordConfirm>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzTalkRecordConfirmModel");
            }
            return result;
        }
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzTalkSign(Epm_TzTalkSign model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzTalkSign>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTalkSign.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzTalkSign");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzTalkSign(Epm_TzTalkSign model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzTalkSign>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTalkSign.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzTalkSign");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzTalkSignByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzTalkSign>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzTalkSign>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzTalkSign.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzTalkSignByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzTalkSign>> GetTzTalkSignList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TzTalkSign>> result = new Result<List<Epm_TzTalkSign>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzTalkSign>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzTalkSignList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TzTalkSign> GetTzTalkSignModel(long id)
        {
            Result<Epm_TzTalkSign> result = new Result<Epm_TzTalkSign>();
            try
            {
                var model = DataOperateBusiness<Epm_TzTalkSign>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzTalkSignModel");
            }
            return result;
        }
    }
}
