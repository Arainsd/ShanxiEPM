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
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTzFirstNegotiation(Epm_TzFirstNegotiation model)
        {
            Result<int> result = new Result<int>();
            try
            {
                int rows = 0;
                bool isAdd = false;
                var first = DataOperateBusiness<Epm_TzFirstNegotiation>.Get().GetList(t => t.ProjectId == model.ProjectId && t.State != (int)PreProjectState.ApprovalFailure && t.State != (int)PreProjectState.Discarded).FirstOrDefault();

                if (first == null)
                {
                    isAdd = true;
                    first = new Epm_TzFirstNegotiation();
                    SetCreateUser(first);
                }
                first.ProjectId = model.ProjectId;
                first.TalkTime = model.TalkTime;
                first.TalkAdress = model.TalkAdress;
                first.Fees = model.Fees;
                first.FeesTime = model.FeesTime;
                first.OurNegotiators = model.OurNegotiators;
                first.OtherNegotiators = model.OtherNegotiators;
                first.TalkResultType = model.TalkResultType;
                first.TalkResultName = model.TalkResultName;
                first.Remark = model.Remark;
                first.State = model.State;
                SetCurrentUser(first);

                #region  项目谈判调用协同接口
                //var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                //if (model.State == (int)PreProjectState.Submitted && XtWorkFlow == "1")
                //{
                //    var project = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId);
                //    XtTzNegotiationView view = new XtTzNegotiationView();
                //    var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.CreateUserId);
                //    if (baseUser == null)
                //    {
                //        throw new Exception("未找到申请人相关信息！");
                //    }
                //    else
                //    {
                //        view.hr_sqr = baseUser.ObjeId;
                //    }
                //    view.ProjectName = project.ProjectName;
                //    view.NatureName = project.NatureName;
                //    view.ApplyTime = string.Format("{0:yyyy-MM-dd}", project.ApplyTime);
                //    view.StationName = project.StationName;
                //    view.Position = project.Position;
                //    view.PredictMoney = project.PredictMoney.ToString();
                //    view.CompanyName = project.CompanyName;
                //    view.TalkTime = string.Format("{0:yyyy-MM-dd}", model.TalkTime);
                //    view.TalkAdress = model.TalkAdress;
                //    view.Fees = model.Fees.ToString();
                //    view.FeesTime = string.Format("{0:yyyy-MM-dd}", model.FeesTime);
                //    view.OurNegotiators = model.OurNegotiators;
                //    view.OtherNegotiators = model.OtherNegotiators;
                //    view.TalkResultName = model.TalkResultName;

                //    //上传附件
                //    if (model.TzAttachs != null && model.TzAttachs.Any())
                //    {
                //        string baseFaleUrl = System.Configuration.ConfigurationManager.AppSettings.Get("XtDownloadUrl");
                //        foreach (var item in model.TzAttachs)
                //        {
                //            string fileUrl = string.Format("{0}?fileId={1}&type={2}", baseFaleUrl, item.Id, item.TypeNo);
                //            view.Temp_TzAttachs = fileUrl + '|' + view.Temp_TzAttachs;
                //        }
                //        if (view.Temp_TzAttachs != null)
                //        {
                //            view.Temp_TzAttachs = view.Temp_TzAttachs.Substring(0, view.Temp_TzAttachs.Length - 1);
                //        }
                //    }

                //    model.WorkFlowId = XtWorkFlowService.CreateTzFirstNegotiationWorkFlow(view);
                //}
                #endregion

                if (isAdd)
                {
                    rows = DataOperateBusiness<Epm_TzFirstNegotiation>.Get().Add(first);
                }
                else
                {
                    rows = DataOperateBusiness<Epm_TzFirstNegotiation>.Get().Update(first);
                }

                //上传附件
                AddFilesBytzTable(first, model.TzAttachs);

                //若状态为已提交，生成下一阶段数据
                if (model.State == (int)PreProjectState.Submitted)
                {
                    //生成评审材料上报记录
                    Epm_TzFormTalkFile tzFormTalkFile = new Epm_TzFormTalkFile();
                    tzFormTalkFile.ProjectId = model.ProjectId;
                    tzFormTalkFile.State = (int)PreProjectState.WaitSubmitted;
                    // AddTzFormTalkFile(tzFormTalkFile);
                    SetCreateUser(tzFormTalkFile);
                    SetCurrentUser(tzFormTalkFile);
                    DataOperateBusiness<Epm_TzFormTalkFile>.Get().Add(tzFormTalkFile);

                    string houstAddress = System.Configuration.ConfigurationManager.AppSettings.Get("RPAPath");

                    var project = DataOperateBusiness<Epm_TzProjectProposal>.Get().Single(t => t.Id == model.ProjectId);
                    //提交-生成回写数据

                    //特许经营项目性质的项目审批到项目批复完成就结束了，不生成工程模块的内容，不用写到机器人表里
                    if (project.Nature != "XMXZTXJY")
                    {
                        #region RPA_TzFirstNegotiation
                        RPA_TzFirstNegotiation rpafirst = new RPA_TzFirstNegotiation();
                        rpafirst.TalkTime = model.TalkTime;
                        rpafirst.TalkAdress = model.TalkAdress;
                        rpafirst.Fees = model.Fees;
                        rpafirst.FeesTime = model.FeesTime;
                        rpafirst.OurNegotiators = model.OurNegotiators;
                        rpafirst.OtherNegotiators = model.OtherNegotiators;
                        rpafirst.TalkResultName = model.TalkResultName;
                        rpafirst.Remark = model.Remark;
                        rpafirst.WriteMark = 0;
                        rpafirst.WriteResult = 0;
                        rpafirst.FollowOperate = "";
                        rpafirst.username = "sxxayw";
                        rpafirst.companys = CurrentUser.CompanyName;
                        rpafirst.ProjectName = project.ProjectName;
                        var fileList = GetFilesByTZTable("Epm_TzFirstNegotiation", first.Id).Data;
                        for (int i = 0; i < fileList.Count; i++)
                        {
                            rpafirst.FilePath += houstAddress + "?path=" + fileList[i].FilePath + "&fileName=" + fileList[i].Name + ",";
                        }
                        rpafirst.FileNumber = fileList.Count;
                        rpafirst.status1 = 0;
                        SetCreateUser(rpafirst);
                        SetCurrentUser(rpafirst);
                        DataOperateBusiness<RPA_TzFirstNegotiation>.Get().Add(rpafirst);
                        #endregion

                        #region OMADS_TzFirstNegotiation
                        var oMADSModel = new OMADS_TzFirstNegotiation();
                        oMADSModel.TalkTime = model.TalkTime;
                        oMADSModel.TalkAdress = model.TalkAdress;
                        oMADSModel.Fees = model.Fees;
                        oMADSModel.FeesTime = model.FeesTime;
                        oMADSModel.OurNegotiators = model.OurNegotiators;
                        oMADSModel.OtherNegotiators = model.OtherNegotiators;
                        oMADSModel.TalkResultName = model.TalkResultName;
                        oMADSModel.Remark = model.Remark;
                        oMADSModel.WriteMark = 0;
                        oMADSModel.WriteResult = 0;
                        oMADSModel.FollowOperate = "";
                        oMADSModel.username = "sxxayw";
                        oMADSModel.companys = CurrentUser.CompanyName;
                        oMADSModel.ProjectName = project.ProjectName;
                        for (int i = 0; i < fileList.Count; i++)
                        {
                            oMADSModel.FilePath += houstAddress + "?path=" + fileList[i].FilePath + "&fileName=" + fileList[i].Name + ",";
                        }
                        oMADSModel.FileNumber = fileList.Count;
                        SetCreateUser(oMADSModel);
                        SetCurrentUser(oMADSModel);
                        DataOperateBusiness<OMADS_TzFirstNegotiation>.Get().Add(oMADSModel);
                        #endregion

                        #region TEMP_TzFirstNegotiation
                        var tEMPModel = new TEMP_TzFirstNegotiation();
                        tEMPModel.TalkTime = model.TalkTime;
                        tEMPModel.TalkAdress = model.TalkAdress;
                        tEMPModel.Fees = model.Fees;
                        tEMPModel.FeesTime = model.FeesTime;
                        tEMPModel.OurNegotiators = model.OurNegotiators;
                        tEMPModel.OtherNegotiators = model.OtherNegotiators;
                        tEMPModel.TalkResultName = model.TalkResultName;
                        tEMPModel.Remark = model.Remark;
                        tEMPModel.WriteMark = 0;
                        tEMPModel.WriteResult = 0;
                        tEMPModel.FollowOperate = "";
                        tEMPModel.username = "sxxayw";
                        tEMPModel.companys = CurrentUser.CompanyName;
                        tEMPModel.ProjectName = project.ProjectName;
                        for (int i = 0; i < fileList.Count; i++)
                        {
                            tEMPModel.FilePath += houstAddress + "?path=" + fileList[i].FilePath + "&fileName=" + fileList[i].Name + ",";
                        }
                        tEMPModel.FileNumber = fileList.Count;
                        tEMPModel.status1 = 0;
                        SetCreateUser(tEMPModel);
                        SetCurrentUser(tEMPModel);
                        DataOperateBusiness<TEMP_TzFirstNegotiation>.Get().Add(tEMPModel);
                        #endregion
                    }
                }

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzFirstNegotiation.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzFirstNegotiation");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzFirstNegotiation(Epm_TzFirstNegotiation model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzFirstNegotiation>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzFirstNegotiation.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzFirstNegotiation");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzFirstNegotiationByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzFirstNegotiation>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzFirstNegotiation>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzFirstNegotiation.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzFirstNegotiationByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectProposal>> GetTzFirstNegotiationList(QueryCondition qc)
        {
            Result<List<Epm_TzProjectProposal>> result = new Result<List<Epm_TzProjectProposal>>();
            try
            {
                var query = from first in context.Epm_TzFirstNegotiation.Where(p => p.IsDelete == false)
                            join project in context.Epm_TzProjectProposal.Where(p => p.IsDelete == false) on first.ProjectId equals project.Id
                            select new
                            {
                                first.ProjectId,
                                project.ProjectName,
                                project.Nature,
                                project.NatureName,
                                project.StationName,
                                project.StationId,
                                project.ApplyTime,
                                first.State,
                                first.OperateUserName,
                                first.OperateTime,
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
                    //OperateUserName = c.OperateUserName,
                    OperateUserName= c.State!=(int)PreProjectState.WaitSubmitted ? c.OperateUserName:"",
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
                                case "name":
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
        public Result<TzFirstNegotiationView> GetTzFirstNegotiationModel(long projectId)
        {
            Result<TzFirstNegotiationView> result = new Result<TzFirstNegotiationView>();
            try
            {
                TzFirstNegotiationView view = new TzFirstNegotiationView();
                var projectModel = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(projectId);
                if (projectModel != null)
                {
                    view.TzProjectProposal = projectModel;
                }

                var tzFirstNegotiation = DataOperateBusiness<Epm_TzFirstNegotiation>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();

                if (tzFirstNegotiation != null)
                {
                    List<Epm_TzAttachs> tzAttachsList = new List<Epm_TzAttachs>();
                    tzAttachsList = GetFilesByTZTable("Epm_TzFirstNegotiation", tzFirstNegotiation.Id).Data;
                    if (tzAttachsList != null && tzAttachsList.Any())
                    {
                        tzFirstNegotiation.TzAttachs = tzAttachsList;
                    }
                    view.TzFirstNegotiation = tzFirstNegotiation;
                }
                result.Data = view;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzFirstNegotiationModel");
            }
            return result;
        }
    }
}
