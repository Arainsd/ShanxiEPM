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
        public Result<int> AddTzLandNegotiation(Epm_TzLandNegotiation model)
        {
            Result<int> result = new Result<int>();
            try
            {
                int rows = 0;
                bool isAdd = false;
                var land = DataOperateBusiness<Epm_TzLandNegotiation>.Get().GetList(t => t.ProjectId == model.ProjectId && t.State != (int)PreProjectState.ApprovalFailure && t.State != (int)PreProjectState.Discarded).FirstOrDefault();

                if (land == null)
                {
                    isAdd = true;
                    land = new Epm_TzLandNegotiation();
                    SetCreateUser(land);
                }
                land.ProjectId = model.ProjectId;
                land.TalkTime = model.TalkTime;
                land.TalkAdress = model.TalkAdress;
                land.Fees = model.Fees;
                land.FeesTime = model.FeesTime;
                land.OurNegotiationers = model.OurNegotiationers;
                land.OtherNegotiationers = model.OtherNegotiationers;
                land.TalkResultType = model.TalkResultType;
                land.TalkResultName = model.TalkResultName;
                land.Remark = model.Remark;
                land.State = model.State;
                SetCurrentUser(land);

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
                //    view.OurNegotiators = model.OurNegotiationers;
                //    view.OtherNegotiators = model.OtherNegotiationers;
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

                //    model.WorkFlowId = XtWorkFlowService.CreateTzLandNegotiationWorkFlow(view);
                //}
                #endregion

                if (isAdd)
                {
                    rows = DataOperateBusiness<Epm_TzLandNegotiation>.Get().Add(land);
                }
                else
                {
                    rows = DataOperateBusiness<Epm_TzLandNegotiation>.Get().Update(land);
                }

                //上传附件
                AddFilesBytzTable(land, model.TzAttachs);

                //若状态为已提交，生成下一阶段数据
                if (model.State == (int)PreProjectState.Submitted)
                {
                    //生成评审材料上报记录
                    Epm_TzFormTalkFile tzFormTalkFile = new Epm_TzFormTalkFile();
                    tzFormTalkFile.ProjectId = model.ProjectId;
                    tzFormTalkFile.State = (int)PreProjectState.WaitSubmitted;
                    AddTzFormTalkFile(tzFormTalkFile);


                    var project = DataOperateBusiness<Epm_TzProjectProposal>.Get().Single(t => t.Id == model.ProjectId);
                    //特许经营项目性质的项目审批到项目批复完成就结束了，不生成工程模块的内容，不用写到机器人表里
                    if (project.Nature != "XMXZTXJY")
                    {
                        //提交-生成回写数据 RPA
                        #region RPA_TzLandNegotiation
                        string houstAddress = System.Configuration.ConfigurationManager.AppSettings.Get("RPAPath");
                        RPA_TzLandNegotiation rpaModel = new RPA_TzLandNegotiation();
                        rpaModel.TalkTime = model.TalkTime;
                        rpaModel.TalkAdress = model.TalkAdress;
                        rpaModel.Fees = model.Fees;
                        rpaModel.FeesTime = model.FeesTime;
                        rpaModel.OurNegotiators = model.OurNegotiationers;
                        rpaModel.OtherNegotiators = model.OtherNegotiationers;
                        // rpaModel.TalkResultName = model.TalkResultName;
                        rpaModel.TalkResultName = model.TalkResultName == "TPJG1" ? "完全达成合作意向" : model.TalkResultName == "TPJG2" ? "基本达成合作意向" : model.TalkResultName == "TPJG3" ? "存在部分问题" : model.TalkResultName == "TPJG5" ? "终止合作" : model.TalkResultName == "TPJG4" ? "尚需继续谈判" : "";

                        rpaModel.Remark = model.Remark;
                        //var userModel = DataOperateBasic<Base_User>.Get().GetModel(model.OperateUserId);
                        rpaModel.username = "sxxayw";
                        rpaModel.companys = CurrentUser.CompanyName;
                        rpaModel.FollowOperate = "";
                        rpaModel.WriteMark = 0;
                        rpaModel.WriteResult = 0;
                        rpaModel.ProjectName = project.ProjectName;
                        var fileList = GetFilesByTZTable("Epm_TzLandNegotiation", land.Id).Data;
                        for (int i = 0; i < fileList.Count; i++)
                        {
                            rpaModel.FilePath += houstAddress + "?path=" + fileList[i].FilePath + "&fileName=" + fileList[i].Name + ",";
                        }
                        rpaModel.FileNumber = fileList.Count;
                        SetCreateUser(rpaModel);
                        SetCurrentUser(rpaModel);
                        #endregion
                        #region OMADS_TzLandNegotiation
                        OMADS_TzLandNegotiation oMADSModel = new OMADS_TzLandNegotiation();
                        oMADSModel.TalkTime = model.TalkTime;
                        oMADSModel.TalkAdress = model.TalkAdress;
                        oMADSModel.Fees = model.Fees;
                        oMADSModel.FeesTime = model.FeesTime;
                        oMADSModel.OurNegotiators = model.OurNegotiationers;
                        oMADSModel.OtherNegotiators = model.OtherNegotiationers;
                        //  oMADSModel.TalkResultName = model.TalkResultName;
                        oMADSModel.TalkResultName = model.TalkResultType == "TPJG1" ? "完全达成合作意向" : model.TalkResultType == "TPJG2" ? "基本达成合作意向" : model.TalkResultType == "TPJG3" ? "存在部分问题" : model.TalkResultType == "TPJG5" ? "终止合作" : model.TalkResultType == "TPJG4" ? "尚需继续谈判" : "";
                        oMADSModel.Remark = model.Remark;
                        // var ouserModel = DataOperateBasic<Base_User>.Get().GetModel(model.OperateUserId);
                        oMADSModel.username = "sxxayw";
                        oMADSModel.companys = CurrentUser.CompanyName;
                        oMADSModel.FollowOperate = "";
                        oMADSModel.WriteMark = 0;
                        oMADSModel.WriteResult = 0;
                        oMADSModel.ProjectName = project.ProjectName;
                        var ofileList = GetFilesByTZTable("Epm_TzLandNegotiation", land.Id).Data;
                        for (int i = 0; i < ofileList.Count; i++)
                        {
                            oMADSModel.FilePath += houstAddress + "?path=" + ofileList[i].FilePath + "&fileName=" + ofileList[i].Name + ",";
                        }
                        oMADSModel.FileNumber = ofileList.Count;
                        SetCreateUser(oMADSModel);
                        SetCurrentUser(oMADSModel);
                        #endregion
                        #region TEMP_TzLandNegotiation
                        var tEMPModel = new TEMP_TzLandNegotiation();
                        tEMPModel.TalkTime = model.TalkTime;
                        tEMPModel.TalkAdress = model.TalkAdress;
                        tEMPModel.Fees = model.Fees;
                        tEMPModel.FeesTime = model.FeesTime;
                        tEMPModel.OurNegotiators = model.OurNegotiationers;
                        tEMPModel.OtherNegotiators = model.OtherNegotiationers;
                        // tEMPModel.TalkResultName = model.TalkResultName;
                        tEMPModel.TalkResultName = model.TalkResultType == "TPJG1" ? "完全达成合作意向" : model.TalkResultType == "TPJG2" ? "基本达成合作意向" : model.TalkResultType == "TPJG3" ? "存在部分问题" : model.TalkResultType == "TPJG5" ? "终止合作" : model.TalkResultType == "TPJG4" ? "尚需继续谈判" : "";
                        tEMPModel.Remark = model.Remark;
                        //var tuserModel = DataOperateBasic<Base_User>.Get().GetModel(model.OperateUserId);
                        tEMPModel.username = "sxxayw";
                        tEMPModel.companys = CurrentUser.CompanyName;
                        tEMPModel.FollowOperate = "";
                        tEMPModel.WriteMark = 0;
                        tEMPModel.WriteResult = 0;
                        tEMPModel.ProjectName = project.ProjectName;
                        var tfileList = GetFilesByTZTable("Epm_TzLandNegotiation", land.Id).Data;
                        for (int i = 0; i < tfileList.Count; i++)
                        {
                            tEMPModel.FilePath += houstAddress + "?path=" + tfileList[i].FilePath + "&fileName=" + tfileList[i].Name + ",";
                        }
                        tEMPModel.FileNumber = tfileList.Count;
                        SetCreateUser(tEMPModel);
                        SetCurrentUser(tEMPModel);
                        #endregion
                        DataOperateBusiness<RPA_TzLandNegotiation>.Get().Add(rpaModel);
                        DataOperateBusiness<OMADS_TzLandNegotiation>.Get().Add(oMADSModel);
                        DataOperateBusiness<TEMP_TzLandNegotiation>.Get().Add(tEMPModel);
                    }
                }

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzLandNegotiation.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzLandNegotiation");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzLandNegotiation(Epm_TzLandNegotiation model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TzLandNegotiation>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzLandNegotiation.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzLandNegotiation");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzLandNegotiationByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TzLandNegotiation>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TzLandNegotiation>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzLandNegotiation.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzLandNegotiationByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectProposal>> GetTzLandNegotiationList(QueryCondition qc)
        {
            Result<List<Epm_TzProjectProposal>> result = new Result<List<Epm_TzProjectProposal>>();
            try
            {
                var query = from first in context.Epm_TzLandNegotiation.Where(p => p.IsDelete == false)
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
                    // OperateUserName = c.OperateUserName,
                    OperateUserName = c.State != (int)PreProjectState.WaitSubmitted ? c.OperateUserName : "",
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
        public Result<TzLandNegotiationView> GetTzLandNegotiationModel(long projectId)
        {
            Result<TzLandNegotiationView> result = new Result<TzLandNegotiationView>();
            try
            {
                TzLandNegotiationView view = new TzLandNegotiationView();
                var projectModel = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(projectId);
                if (projectModel != null)
                {
                    view.TzProjectProposal = projectModel;
                }

                var tzLandNegotiation = DataOperateBusiness<Epm_TzLandNegotiation>.Get().GetList(t => t.ProjectId == projectId).FirstOrDefault();

                if (tzLandNegotiation != null)
                {
                    List<Epm_TzAttachs> tzAttachsList = new List<Epm_TzAttachs>();
                    tzAttachsList = GetFilesByTZTable("Epm_TzLandNegotiation", tzLandNegotiation.Id).Data;
                    if (tzAttachsList != null && tzAttachsList.Any())
                    {
                        tzLandNegotiation.TzAttachs = tzAttachsList;
                    }
                    view.TzLandNegotiation = tzLandNegotiation;
                }
                result.Data = view;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzLandNegotiationModel");
            }
            return result;
        }
    }
}
