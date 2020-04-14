using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using hc.Plat.Common.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteProjectByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_Project>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_Project>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Project.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteProjectByIds");
            }
            return result;
        }

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_Project>> GetProjectList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_Project>> result = new Result<List<Epm_Project>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Project>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectList");
            }
            return result;
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_Project> GetProjectModel(long id)
        {
            Result<Epm_Project> result = new Result<Epm_Project>();
            try
            {
                var model = DataOperateBusiness<Epm_Project>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectModel");
            }
            return result;
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_Project> GetProjectModelByTzId(long id)
        {
            Result<Epm_Project> result = new Result<Epm_Project>();
            try
            {
                var model = DataOperateBusiness<Epm_Project>.Get().Single(p => p.TzProjectId == id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectModel");
            }
            return result;
        }

        /// <summary>
        /// 修改项目状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeProjectState(long id, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                var projectState = state.ToEnumReq<ProjectState>();
                var model = DataOperateBusiness<Epm_Project>.Get().GetModel(id);

                model.OperateUserId = CurrentUserID.ToLongReq();
                model.OperateUserName = CurrentUserName;
                model.OperateTime = DateTime.Now;
                model.State = int.Parse(projectState.GetValue().ToString());

                var rows = DataOperateBusiness<Epm_Project>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "ChangeProjectState");
            }
            return result;
        }

        /// <summary>
        /// 检查验收结果
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Result<int> CheckAcceptance(long projectId)
        {
            Result<int> result = new Result<int>();
            int state = (int)ApprovalState.ApprSuccess;
            //专项验收
            //var temp = DataOperateBusiness<Epm_SpecialAcceptance>.Get().GetList(t => t.ProjectId == projectId && t.State == state).ToList().FirstOrDefault();

            //竣工验收
            var model = DataOperateBusiness<Epm_CompletionAcceptance>.Get().GetList(t => t.ProjectId == projectId && t.State == state).ToList().FirstOrDefault();

            if (model != null)
            {
                result.Flag = EResultFlag.Success;
            }
            else
            {
                result.Flag = EResultFlag.Failure;
            }
            return result;
        }

        #region 查询
        /// <summary>
        /// 在建项目列表（多表查询）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public Result<List<ProjectView>> GetProjectListInfo(int pageIndex, int pageSize, string state, string pmName, string name = "", string startTime = "", string endTime = "")
        {
            List<ProjectView> list = new List<ProjectView>();
            Result<List<ProjectView>> result = new Result<List<ProjectView>>();
            try
            {
                DateTime stime = string.IsNullOrEmpty(startTime) ? DateTime.MinValue : Convert.ToDateTime(startTime + " 00:00:00");
                DateTime etime = string.IsNullOrEmpty(endTime) ? DateTime.MaxValue : Convert.ToDateTime(endTime + " 23:59:59");

                var statelist = string.IsNullOrEmpty(state) ? new List<string>() : state.Split(',').ToList();
                var pmNamelist = string.IsNullOrEmpty(pmName) ? new List<string>() : pmName.Split(',').ToList();

                long currentId = CurrentUserID.ToLongReq();
                bool isAgency = IsAgencyUser(currentId);
                int draft = (int)ProjectState.NoStart;

                List<Epm_Project> models = DataOperateBusiness<Epm_Project>.Get().GetList(t =>
                   (string.IsNullOrEmpty(name) || t.Name.Contains(name))
                   && (stime == DateTime.MinValue || (t.PlanWorkStartTime.HasValue && stime <= t.PlanWorkStartTime))
                   && (etime == DateTime.MaxValue || (t.PlanWorkEndTime.HasValue && t.PlanWorkEndTime <= etime))
                   && !t.IsDelete
                   //&& (CurrentProjectIds.Contains(t.Id.ToString()) || (isAgency && t.State != draft) || (isAgency && t.State == draft && t.CreateUserId == currentId) || t.PMId.Value == currentId || t.PMId == null)
                   && (statelist.Contains(t.State.ToString()))  //查询状态
                   && (((string.IsNullOrEmpty(pmName)) || pmNamelist.Contains(t.PMId.ToString())))   //查询项目经理
                    ).OrderByDescending(p => p.OperateTime).ToList(); //修改时间倒序

                if (models.Count > 0)
                {
                    for (int i = 0; i < models.Count; i++)
                    {
                        ProjectView pv = new ProjectView();

                        pv.Id = models[i].Id;
                        pv.Code = models[i].Code;
                        pv.Name = models[i].Name;
                        pv.StartDate = models[i].StartDate;
                        pv.EndDate = models[i].EndDate;
                        pv.City = models[i].City;
                        pv.Province = models[i].Province;
                        pv.Area = models[i].Area;
                        pv.Address = models[i].Address;
                        pv.Amount = models[i].Amount;
                        pv.CompanyName = models[i].CompanyName;
                        pv.PMName = models[i].PMName;
                        pv.ContactUserId = models[i].ContactUserId;
                        pv.ContactUserName = models[i].ContactUserName;
                        pv.ContactPhone = models[i].ContactPhone;
                        pv.State = models[i].State;
                        pv.OperateTime = models[i].OperateTime.Value;
                        pv.ProjectNatureName = models[i].ProjectNatureName;
                        pv.TzProjectId = models[i].TzProjectId;
                        pv.CompanyId = models[i].CompanyId;
                        //查询项目状态数据
                        #region 查询项目状态数据
                        //long ProjectId = models[i].Id;
                        //var temp = DataOperateBusiness<Epm_ProjectStateTrack>.Get().GetList(t => t.ProjectId == ProjectId).ToList();
                        //if (temp.Count > 0)
                        //{
                        //    for (int j = 0; j < temp.Count; j++)
                        //    {
                        //        switch (temp[j].StateTrackNo)
                        //        {
                        //            case "ScheduleRatio":
                        //                pv.ScheduleRatio = temp[j].Qty ?? "0";
                        //                break;
                        //            case "QualityCheckNum":
                        //                pv.QualityCheckNum = temp[j].Qty ?? "0";
                        //                break;
                        //            case "SecurityCheckNum":
                        //                pv.SecurityCheckNum = temp[j].Qty ?? "0";
                        //                break;
                        //            case "SupervisorLogNum":
                        //                pv.SupervisorLogNum = temp[j].Qty ?? "0";
                        //                break;
                        //            case "ProblemNum":
                        //                pv.ProblemNum = temp[j].Qty ?? "0";
                        //                break;
                        //            default:
                        //                break;
                        //        }
                        //    }
                        //}
                        #endregion

                        list.Add(pv);
                    }

                    result.AllRowsCount = list.Count();

                    list=list.OrderByDescending(t => t.OperateTime).ToList();
                    list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).OrderByDescending(t => t.OperateTime).ToList();
                    result.Data = list;
                    result.Flag = EResultFlag.Success;
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectListInfo");
            }
            return result;
        }
        #endregion

        #region 项目基础信息
        /// <summary>
        /// 获取项目
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_Project> GetProject(long id)
        {
            Result<Epm_Project> result = new Result<Epm_Project>();
            try
            {
                var model = DataOperateBusiness<Epm_Project>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProject");
            }
            return result;
        }
        /// <summary>
        /// 添加项目
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddProject(Epm_Project model)
        {
            Result<int> result = new Result<int>();
            try
            {
                bool dConfig = DataOperateBusiness<Epm_Project>.Get().Count(i => i.Name == model.Name || i.Code == model.Code) > 0;
                if (dConfig)
                {
                    throw new Exception("该项目名称或编码已经存在");
                }
                model = base.SetCurrentUser(model);
                model.CrtCompanyId = CurrentCompanyID.ToLongReq();
                var rows = DataOperateBusiness<Epm_Project>.Get().Add(model);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Project.GetText(), SystemRight.Add.GetText(), "添加项目: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddProject");
            }

            return result;
        }
        /// <summary>
        /// 修改项目
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateProject(Epm_Project model, List<Base_Files> attachs)
        {
            Result<int> result = new Result<int>();
            try
            {
                var oldModel = DataOperateBusiness<Epm_Project>.Get().GetModel(model.Id);
                oldModel.Code = model.Code;
                //oldModel.Name = model.Name;
                //oldModel.SubjectNo = model.SubjectNo;
                //oldModel.SubjectName = model.SubjectName;
                //oldModel.ProjectSubjectId = model.ProjectSubjectId;
                //oldModel.ProjectSubjectName = model.ProjectSubjectName;
                //oldModel.CompanyId = model.CompanyId;
                //oldModel.CompanyName = model.CompanyName;
                //oldModel.Province = model.Province;
                oldModel.City = model.City;
                oldModel.Area = model.Area;
                oldModel.Address = model.Address;
                //oldModel.Amount = model.Amount;
                oldModel.Description = model.Description;
                oldModel.ContactUserId = model.ContactUserId;
                oldModel.ContactUserName = model.ContactUserName;
                oldModel.ContactPhone = model.ContactPhone;
                //oldModel.State = model.State;
                oldModel.ReplyDate = model.ReplyDate;
                //oldModel.ProjectAbbreviation = model.ProjectAbbreviation;
                //oldModel.IsHomePageShow = model.IsHomePageShow;
                //oldModel.ProjectNature = model.ProjectNature;
                //oldModel.ProjectNatureName = model.ProjectNatureName;
                //oldModel.CostCourse = model.CostCourse;
                oldModel.PMId = model.PMId;
                oldModel.PMName = model.PMName;
                oldModel.PMPhone = model.PMPhone;
                //oldModel.ReplyNumber = model.ReplyNumber;
                oldModel.BluePrintKey = model.BluePrintKey;
                oldModel.BluePrintValue = model.BluePrintValue;
                //oldModel.GasolineDieselRatio = model.GasolineDieselRatio;
                oldModel.ProjectContent = model.ProjectContent;
                oldModel.Remark = model.Remark;
                oldModel.OperateUserId = CurrentUserID.ToLongReq();
                oldModel.OperateUserName = CurrentUserName;
                oldModel.OperateTime = DateTime.Now;
                oldModel.ProjectSubjectCode = model.ProjectSubjectCode;
                oldModel.ProjectSubjectId = model.ProjectSubjectId;
                oldModel.ProjectSubjectName = model.ProjectSubjectName;
                var rows = DataOperateBusiness<Epm_Project>.Get().Update(oldModel);

                #region 附件
                if (attachs != null)
                {
                    #region 保存附件
                    //删除之前的附件
                    DeleteFilesByTable(model.GetType().Name, new List<long>() { model.Id }, "ReplyDate");

                    AddFilesByTable(model, attachs);
                    #endregion
                }
                #endregion

                //保存需要发送的基础数据
                AddSendDateByProjectId(model.Id);

                #region 项目状态数据
                ////先删除原始数据
                //var projectState = DataOperateBusiness<Epm_ProjectStateTrack>.Get().GetList(t => t.ProjectId == model.Id && (t.StateTrackNo == "ChangeNum" || t.StateTrackNo == "VisaNum")).ToList();
                //foreach (var item in projectState)
                //{
                //    item.OperateUserId = CurrentUserID.ToLongReq();
                //    item.OperateUserName = CurrentUserName;
                //    item.OperateTime = DateTime.Now;
                //    item.DeleteTime = DateTime.Now;
                //}
                //rows = DataOperateBusiness<Epm_ProjectStateTrack>.Get().DeleteRange(projectState);
                //if (project.Epm_ProjectCompany != null)
                //{
                //    //查询项目状态类型
                //    var dictionaryList = DataOperateBasic<Base_TypeDictionary>.Get().GetList(t => t.Type == DictionaryType.ProjectStateTrack.ToString()).ToList();
                //    if (dictionaryList != null && dictionaryList.Count > 0)
                //    {
                //        List<Epm_ProjectStateTrack> trackList = new List<Epm_ProjectStateTrack>();
                //        Epm_ProjectStateTrack track = null;
                //        for (int i = 0; i < dictionaryList.Count; i++)
                //        {
                //            if (dictionaryList[i].No == "ChangeNum" || dictionaryList[i].No == "VisaNum")
                //            {
                //                foreach (var item in project.Epm_ProjectCompany)
                //                {
                //                    track = new Epm_ProjectStateTrack();
                //                    track = base.SetCurrentUser(track);
                //                    track.CrtCompanyId = CurrentCompanyID.ToLongReq();
                //                    track.CrtCompanyName = CurrentCompanyName;
                //                    track.ProjectId = model.Id;
                //                    track.Qty = "0";
                //                    track.StateTrackName = dictionaryList[i].Name;
                //                    track.StateTrackNo = dictionaryList[i].No;
                //                    track.CompanyId = item.Id;
                //                    track.CompanyName = item.CompanyName;
                //                    trackList.Add(track);
                //                }
                //            }
                //        }
                //        rows = DataOperateBusiness<Epm_ProjectStateTrack>.Get().AddRange(trackList);
                //    }
                //}
                #endregion

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Project.GetText(), SystemRight.Modify.GetText(), "更新项目基本信息: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateProject");
            }
            return result;
        }
        #endregion

        #region 项目总批复及构成
        /// <summary>
        /// 获取项目总批复及构成
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        public Result<List<Epm_ProjectConstitute>> GetProjectConstituteByProjectId(long projectId)
        {
            Result<List<Epm_ProjectConstitute>> result = new Result<List<Epm_ProjectConstitute>>();
            try
            {
                List<Epm_ProjectConstitute> pclist = null;
                var project = DataOperateBusiness<Epm_Project>.Get().GetModel(projectId);
                if (project != null)
                {
                    pclist = DataOperateBusiness<Epm_ProjectConstitute>.Get().GetList(p => p.ProjectId == project.Id).ToList();
                    if (pclist == null || pclist.Count == 0)
                    {
                        pclist = new List<Epm_ProjectConstitute>();
                        Epm_ProjectConstitute pc = null;

                        var constitute = DataOperateBusiness<Epm_Constitute>.Get().GetList(p => p.ProjectNatureCode == project.ProjectNature).ToList();
                        constitute = constitute.OrderBy(i => i.Sort).ToList();
                        foreach (var item in constitute)
                        {
                            pc = new Epm_ProjectConstitute();
                            pc.ProjectId = project.Id;
                            pc.ConstituteKey = item.ConstituteKey;
                            pc.ConstituteValue = item.ConstituteName;
                            pc.IsCharging = item.IsCharging;
                            pc.IsAProvide = item.IsAProvide;
                            pc.Sort = item.Sort;
                            pclist.Add(pc);
                        }
                    }
                    result.Data = pclist.OrderBy(i => i.Sort).ToList();
                    result.AllRowsCount = pclist.Count();
                    result.Flag = EResultFlag.Success;
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectConstituteByNature");
            }
            return result;
        }
        /// <summary>
        /// 修改项目总批复及构成
        /// </summary>
        /// <param name="list">总批复及构成集合</param>
        /// <param name="projectId">项目Id</param>
        /// <param name="bluePrintCode">方案类型Key</param>
        /// <param name="bluePrintName">方案类型Value</param>
        /// <param name="isCrossings">是否外部手续bool</param>
        /// <returns></returns>
        public Result<int> UpdateProjectConstitute(Epm_Project project, List<Epm_ProjectConstitute> list, List<Base_Files> attachs)
        {
            Result<int> result = new Result<int>();
            try
            {
                #region 更新项目基本信息
                var model = DataOperateBusiness<Epm_Project>.Get().GetModel(project.Id);
                //model.BluePrintKey = project.BluePrintKey;
                //model.BluePrintValue = project.BluePrintValue;
                model.IsCrossings = project.IsCrossings == null ? model.IsCrossings : project.IsCrossings;
                model.BalanceAmount = project.BalanceAmount == null ? model.BalanceAmount : project.BalanceAmount;
                model.AProvideAmount = project.AProvideAmount == null ? model.AProvideAmount : project.AProvideAmount;
                model = FiterUpdate(model, model);
                var rows = DataOperateBusiness<Epm_Project>.Get().Update(model);
                #endregion

                if (list.Any() && list.Count > 0)
                {
                    #region 记录历史
                    List<Epm_ProjectConstituteHistory> pcHisList = new List<Epm_ProjectConstituteHistory>();
                    Epm_ProjectConstituteHistory pcHis = null;
                    DateTime updateTime = DateTime.Now;
                    foreach (var item in list)
                    {
                        pcHis = new Epm_ProjectConstituteHistory();
                        pcHis.ProjectId = project.Id;
                        pcHis.ConstituteKey = item.ConstituteKey;
                        pcHis.ConstituteValue = item.ConstituteValue;
                        pcHis.IsCharging = item.IsCharging;
                        pcHis.IsAProvide = item.IsAProvide;
                        pcHis.Amount = item.Amount;
                        pcHis.Sort = item.Sort;
                        pcHis = base.SetCurrentUser(pcHis);
                        pcHis.CreateTime = updateTime;
                        pcHis.OperateTime = updateTime;
                        pcHisList.Add(pcHis);
                    }
                    DataOperateBusiness<Epm_ProjectConstituteHistory>.Get().AddRange(pcHisList);
                    #endregion

                    #region 删除数据
                    var models = DataOperateBusiness<Epm_ProjectConstitute>.Get().GetList(p => p.ProjectId == project.Id).ToList();
                    var delRows = DataOperateBusiness<Epm_ProjectConstitute>.Get().DeleteRange(models);
                    #endregion

                    #region 保存数据
                    List<Epm_ProjectConstitute> pcList = new List<Epm_ProjectConstitute>();
                    Epm_ProjectConstitute pc = null;
                    DateTime dt = DateTime.Now;
                    foreach (var item in list)
                    {
                        pc = new Epm_ProjectConstitute();
                        pc.ProjectId = project.Id;
                        pc.ConstituteKey = item.ConstituteKey;
                        pc.ConstituteValue = item.ConstituteValue;
                        pc.IsCharging = item.IsCharging;
                        pc.IsAProvide = item.IsAProvide;
                        pc.Sort = item.Sort;
                        pc.Amount = item.Amount;
                        pc = base.SetCurrentUser(pc);
                        pc.CreateTime = pc.OperateTime = dt;
                        pcList.Add(pc);
                    }
                    DataOperateBusiness<Epm_ProjectConstitute>.Get().AddRange(pcList);
                    #endregion

                    //保存需要发送的基础数据---改版后据说不用发消息了，先注释掉2019-11-22
                    // AddSendDateByProjectId(project.Id);
                }
                if (attachs != null)
                {
                    #region 保存附件
                    //删除之前的附件
                    DeleteFilesByTable(model.GetType().Name, new List<long>() { model.Id }, "Constitute");

                    AddFilesByTable(model, attachs);
                    #endregion
                }

                #region 消息   改版后据说不用发消息了，先注释掉2019-11-22
                //var waitSend = GetWaitSendMessageList(model.Id);
                //foreach (var send in waitSend)
                //{
                //    Epm_Massage modelMsg = new Epm_Massage();
                //    modelMsg.ReadTime = null;
                //    modelMsg.RecId = send.Key;
                //    modelMsg.RecName = send.Value;
                //    modelMsg.RecTime = DateTime.Now;
                //    modelMsg.SendId = CurrentUserID.ToLongReq();
                //    modelMsg.SendName = CurrentUserName;
                //    modelMsg.SendTime = DateTime.Now;
                //    modelMsg.Title = CurrentUserName + "修改项目总批复及构成信息";
                //    modelMsg.Content = CurrentUserName + "修改项目总批复及构成信息";
                //    modelMsg.Type = 2;
                //    modelMsg.IsRead = false;
                //    modelMsg.BussinessId = model.Id;
                //    modelMsg.BussinesType = BusinessType.Project.ToString();
                //    modelMsg.BussinesChild = "Constitute";
                //    modelMsg.ProjectId = model.Id;
                //    modelMsg.ProjectName = model.Name;
                //    modelMsg = base.SetCurrentUser(modelMsg);
                //    modelMsg = base.SetCreateUser(modelMsg);
                //    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                //}
                #endregion
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Project.GetText(), SystemRight.Modify.GetText(), "更新总批复及构成: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateProjectConstitute");
            }
            return result;
        }

        /// <summary>
        /// 保存需要发送的基础数据
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        private bool AddSendDateByProjectId(long projectId)
        {
            bool result = false;
            Epm_Project project = DataOperateBusiness<Epm_Project>.Get().GetList(t => t.Id == projectId).FirstOrDefault();

            List<Bp_SendDate> sendList = new List<Bp_SendDate>();

            DateTime time = DateTime.Now;
            //"XMKGSJ":"开工日期（YYYYMMDD）",
            Bp_SendDate xmkgsj = new Bp_SendDate();
            xmkgsj.IsSend = false;
            xmkgsj.Key = "XMKGSJ";
            xmkgsj.Value = project.PlanWorkStartTime.HasValue ? project.PlanWorkStartTime.ToString("yyyyMMdd") : "";
            xmkgsj.Type = "11";
            xmkgsj.Project = "BIM";
            xmkgsj.KeyValue = project.ObjeId;
            xmkgsj.UserName = CurrentUser.UserCode;
            xmkgsj = SetCreateUser(xmkgsj);
            xmkgsj = SetCurrentUser(xmkgsj);
            xmkgsj.CreateTime = time;
            xmkgsj.OperateTime = time;
            sendList.Add(xmkgsj);

            //"XMWGSJ":"完工日期（YYYYMMDD）",
            Bp_SendDate xmwgsj = new Bp_SendDate();
            xmwgsj.IsSend = false;
            xmwgsj.Key = "XMWGSJ";
            xmwgsj.Value = project.PlanWorkEndTime.HasValue ? project.PlanWorkEndTime.ToString("yyyyMMdd") : "";
            xmwgsj.Type = "11";
            xmwgsj.Project = "BIM";
            xmwgsj.KeyValue = project.ObjeId;
            xmwgsj.UserName = CurrentUser.UserCode;
            xmwgsj = SetCreateUser(xmwgsj);
            xmwgsj = SetCurrentUser(xmwgsj);
            xmwgsj.CreateTime = time;
            xmwgsj.OperateTime = time;
            sendList.Add(xmwgsj);

            decimal tujian = 0;
            decimal cailiao = ((project.AProvideAmount == null || !project.AProvideAmount.HasValue) ? 0 : project.AProvideAmount.Value);
            decimal countAmount = (project.Amount == null ? 0 : project.Amount.Value);
            List<Epm_ProjectConstitute> list = GetProjectConstituteByProjectId(projectId).Data;

            if (list.Any() && list.Count > 0)
            {
                foreach (var item in list)
                {
                    if (item.ConstituteValue == "土建" && item.Amount.HasValue)//土建
                    {
                        tujian = (project.Amount == null ? 0 : item.Amount.Value);
                    }
                    if (item.ConstituteValue == "安装" && item.Amount.HasValue)//安装
                    {
                        cailiao = cailiao + (project.Amount == null ? 0 : item.Amount.Value);
                    }
                    if (item.ConstituteValue == "包装" && item.Amount.HasValue)//包装
                    {
                        cailiao = cailiao + (project.Amount == null ? 0 : item.Amount.Value);
                    }
                    if (item.ConstituteValue == "内衬" && item.Amount.HasValue)//内衬
                    {
                        cailiao = cailiao + (project.Amount == null ? 0 : item.Amount.Value);
                    }
                }
            }

            //"GCJSF":"工程建设费（没有传0）",
            Bp_SendDate gcjsf = new Bp_SendDate();
            gcjsf.IsSend = false;
            gcjsf.Key = "GCJSF";
            gcjsf.Value = tujian.ToString();
            gcjsf.Type = "11";
            gcjsf.Project = "BIM";
            gcjsf.KeyValue = project.ObjeId;
            gcjsf.UserName = CurrentUser.UserCode;
            gcjsf = SetCreateUser(gcjsf);
            gcjsf = SetCurrentUser(gcjsf);
            gcjsf.CreateTime = time;
            gcjsf.OperateTime = time;
            sendList.Add(gcjsf);


            //"CLSBF":"材料设备费（没有传0）",
            Bp_SendDate clsbf = new Bp_SendDate();
            clsbf.IsSend = false;
            clsbf.Key = "CLSBF";
            clsbf.Value = cailiao.ToString();
            clsbf.Type = "11";
            clsbf.Project = "BIM";
            clsbf.KeyValue = project.ObjeId;
            clsbf.UserName = CurrentUser.UserCode;
            clsbf = SetCreateUser(clsbf);
            clsbf = SetCurrentUser(clsbf);
            clsbf.CreateTime = time;
            clsbf.OperateTime = time;
            sendList.Add(clsbf);

            //"GCXGQTSF":"工程相关其他税费（没有传0）",
            Bp_SendDate gcxgqtsf = new Bp_SendDate();
            gcxgqtsf.IsSend = false;
            gcxgqtsf.Key = "GCXGQTSF";
            gcxgqtsf.Value = (countAmount - cailiao - tujian).ToString();
            gcxgqtsf.Type = "11";
            gcxgqtsf.Project = "BIM";
            gcxgqtsf.KeyValue = project.ObjeId;
            gcxgqtsf.UserName = CurrentUser.UserCode;
            gcxgqtsf = SetCreateUser(gcxgqtsf);
            gcxgqtsf = SetCurrentUser(gcxgqtsf);
            gcxgqtsf.CreateTime = time;
            gcxgqtsf.OperateTime = time;
            sendList.Add(gcxgqtsf);

            //"GCZT":"未开工传0，在建传1，已完工传2",
            Bp_SendDate gczt = new Bp_SendDate();
            gczt.IsSend = false;
            gczt.Key = "GCZT";
            gczt.Value = (project.State == 5 ? 1 : project.State == 10 ? 2 : 0).ToString();
            gczt.Type = "11";
            gczt.Project = "BIM";
            gczt.KeyValue = project.ObjeId;
            gczt.UserName = CurrentUser.UserCode;
            gczt = SetCreateUser(gczt);
            gczt = SetCurrentUser(gczt);
            gczt.CreateTime = time;
            gczt.OperateTime = time;
            sendList.Add(gczt);

            //"XMJJ":"项目主题"
            Bp_SendDate xmzt = new Bp_SendDate();
            xmzt.IsSend = false;
            xmzt.Key = "XMJJ";
            xmzt.Value = project.Description ?? "";
            xmzt.Type = "11";
            xmzt.Project = "BIM";
            xmzt.KeyValue = project.ObjeId;
            xmzt.UserName = CurrentUser.UserCode;
            xmzt = SetCreateUser(xmzt);
            xmzt = SetCurrentUser(xmzt);
            xmzt.CreateTime = time;
            xmzt.OperateTime = time;
            sendList.Add(xmzt);

            //"PSSJ":"立项评审时间（YYYYMMDD）
            Bp_SendDate pssj = new Bp_SendDate();
            pssj.IsSend = false;
            pssj.Key = "PSSJ";
            pssj.Value = project.ReplyDate.HasValue ? project.ReplyDate.ToString("yyyyMMdd") : "";
            pssj.Type = "11";
            pssj.Project = "BIM";
            pssj.KeyValue = project.ObjeId;
            pssj.UserName = CurrentUser.UserCode;
            pssj = SetCreateUser(xmzt);
            pssj = SetCurrentUser(xmzt);
            pssj.CreateTime = time;
            pssj.OperateTime = time;
            sendList.Add(pssj);

            //"FZRBH":"分公司负责人编号" 
            var user = GetUserModel(project.ContactUserId.Value).Data;
            if (user != null)
            {
                Bp_SendDate fzrbh = new Bp_SendDate();
                fzrbh.IsSend = false;
                fzrbh.Key = "FZRBH";
                fzrbh.Value = user.ObjeId;
                fzrbh.Type = "11";
                fzrbh.Project = "BIM";
                fzrbh.KeyValue = project.ObjeId;
                fzrbh.UserName = CurrentUser.UserCode;
                fzrbh = SetCreateUser(xmzt);
                fzrbh = SetCurrentUser(xmzt);
                fzrbh.CreateTime = time;
                fzrbh.OperateTime = time;
                sendList.Add(fzrbh);
            }

            //"FZRMC":"分公司负责人名称"
            Bp_SendDate fzrmc = new Bp_SendDate();
            fzrmc.IsSend = false;
            fzrmc.Key = "FZRMC";
            fzrmc.Value = project.CompanyName;
            fzrmc.Type = "11";
            fzrmc.Project = "BIM";
            fzrmc.KeyValue = project.ObjeId;
            fzrmc.UserName = CurrentUser.UserCode;
            fzrmc = SetCreateUser(xmzt);
            fzrmc = SetCurrentUser(xmzt);
            fzrmc.CreateTime = time;
            fzrmc.OperateTime = time;
            sendList.Add(fzrmc);

            //"FZRDH":"分公司负责人电话"
            Bp_SendDate fzrdh = new Bp_SendDate();
            fzrdh.IsSend = false;
            fzrdh.Key = "FZRDH";
            fzrdh.Value = project.ContactPhone;
            fzrdh.Type = "11";
            fzrdh.Project = "BIM";
            fzrdh.KeyValue = project.ObjeId;
            fzrdh.UserName = CurrentUser.UserCode;
            fzrdh = SetCreateUser(xmzt);
            fzrdh = SetCurrentUser(xmzt);
            fzrdh.CreateTime = time;
            fzrdh.OperateTime = time;
            sendList.Add(fzrdh);

            int rows = DataOperateBusiness<Bp_SendDate>.Get().AddRange(sendList);

            if (rows > 0)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 获取项目总批复及构成历史
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        public Result<List<Epm_ProjectConstituteHistory>> GetProjectConstituteHistoryByProjectId(long projectId)
        {
            Result<List<Epm_ProjectConstituteHistory>> result = new Result<List<Epm_ProjectConstituteHistory>>();
            try
            {
                List<Epm_ProjectConstituteHistory> pcHisList = DataOperateBusiness<Epm_ProjectConstituteHistory>.Get().GetList(p => p.ProjectId == projectId).ToList();

                result.Data = pcHisList;
                result.AllRowsCount = pcHisList.Count();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectConstituteHistoryByProjectId");
            }
            return result;
        }
        /// <summary>
        /// 获取项目总批复构成历史
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public DataTable GetConstituteHis(long projectId)
        {
            DataTable dt = new DataTable();
            Result<List<Epm_ProjectConstituteHistory>> result = new Result<List<Epm_ProjectConstituteHistory>>();

            result = GetProjectConstituteHistoryByProjectId(projectId);
            result.Data = result.Data.Where(t => !string.IsNullOrEmpty(t.ConstituteValue)).ToList();


            if (result.Data.Count() > 0)
            {
                dt.Columns.Add("日期", System.Type.GetType("System.String"));
                dt.Columns.Add("修改人", System.Type.GetType("System.String"));
                var cols = result.Data.GroupBy(t => t.ConstituteValue).Select(t => t.Key).ToList();
                for (int i = 0; i < cols.Count(); i++)
                {
                    dt.Columns.Add(cols[i], System.Type.GetType("System.String"));
                }

                var Times = result.Data.GroupBy(t => t.CreateTime).Select(t => t.Key).ToList();
                for (int j = 0; j < Times.Count(); j++)
                {
                    var rows = result.Data.Where(t => t.CreateTime == Times[j]).ToList();
                    if (rows.Count() > 0)
                    {
                        DataRow dr = null;
                        dr = dt.NewRow();
                        for (int i = 0; i < rows.Count(); i++)
                        {
                            dr["日期"] = rows[i].CreateTime.ToString("yyyy-MM-dd hh:mm:ss");
                            dr["修改人"] = rows[i].CreateUserName;
                            dr[rows[i].ConstituteValue] = rows[i].Amount;
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
        }
        #endregion

        #region 工程服务商
        /// <summary>
        /// 获取工程服务商
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        public Result<List<Epm_ProjectCompany>> GetProjectCompanyByProjectId(long projectId)
        {
            Result<List<Epm_ProjectCompany>> result = new Result<List<Epm_ProjectCompany>>();
            try
            {
                List<Epm_ProjectCompany> pclist = DataOperateBusiness<Epm_ProjectCompany>.Get().GetList(p => p.ProjectId == projectId).ToList();
                Epm_ProjectCompany pc = null;

                if (pclist == null || pclist.Count == 0)
                {
                    pclist = new List<Epm_ProjectCompany>();
                    var constitute = DataOperateBusiness<Epm_ProjectConstitute>.Get().GetList(p => p.ProjectId == projectId && p.Amount.HasValue && p.Amount > 0).ToList();
                    constitute = constitute.OrderBy(i => i.IsAProvide).OrderBy(i => i.Sort).ToList();
                    foreach (var item in constitute)
                    {
                        pc = new Epm_ProjectCompany();
                        pc.ProjectId = projectId;
                        pc.Type = item.ConstituteValue;
                        pc.IsSupervisor = (item.ConstituteValue == "监理" ? 1 : 0);
                        pc.IsAProvide = item.IsAProvide;
                        pc.Sort = item.Sort;
                        pc = base.SetCurrentUser(pc);
                        pc.CrtCompanyId = CurrentCompanyID.ToLongReq();
                        pc.CrtCompanyName = CurrentCompanyName;
                        pclist.Add(pc);
                    }
                    DataOperateBusiness<Epm_ProjectCompany>.Get().AddRange(pclist);
                }
                else
                {
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(projectId);
                    if (project.State != (int)ProjectState.Construction)
                    {
                        pclist = pclist.OrderBy(t => t.IsAProvide).ThenBy(t => t.Sort).ToList();
                        result.Data = pclist;
                        result.AllRowsCount = pclist.Count();
                        result.Flag = EResultFlag.Success;
                        return result;
                    }

                    var constitute = DataOperateBusiness<Epm_ProjectConstitute>.Get().GetList(p => p.ProjectId == projectId && p.Amount.HasValue && p.Amount > 0).ToList();
                    constitute = constitute.OrderBy(i => i.IsAProvide).OrderBy(i => i.Sort).ToList();
                    if (constitute != null && constitute.Count > 0)
                    {
                        //添加服务商
                        List<Epm_ProjectCompany> addList = new List<Epm_ProjectCompany>();
                        foreach (var item in constitute)
                        {
                            bool isHas = false;
                            foreach (var c in pclist)
                            {
                                if (c.Type == item.ConstituteValue)
                                {
                                    isHas = true;
                                    break;
                                }
                            }
                            if (!isHas)
                            {
                                pc = new Epm_ProjectCompany();
                                pc.ProjectId = projectId;
                                pc.Type = item.ConstituteValue;
                                pc.IsSupervisor = (item.ConstituteValue == "监理" ? 1 : 0);
                                pc.IsAProvide = item.IsAProvide;
                                pc.Sort = item.Sort;
                                pc = base.SetCurrentUser(pc);
                                pc.CrtCompanyId = CurrentCompanyID.ToLongReq();
                                pc.CrtCompanyName = CurrentCompanyName;
                                pclist.Add(pc);
                                addList.Add(pc);
                            }
                        }
                        if (addList != null && addList.Count > 0)
                        {
                            DataOperateBusiness<Epm_ProjectCompany>.Get().AddRange(addList);
                        }

                        //删除服务商
                        List<Epm_ProjectCompany> delList = new List<Epm_ProjectCompany>();
                        foreach (var item in pclist)
                        {
                            bool isDel = false;
                            foreach (var c in constitute)
                            {
                                if (c.ConstituteValue == item.Type)
                                {
                                    isDel = true;
                                    break;
                                }
                            }
                            if (!isDel)
                            {
                                delList.Add(item);
                            }
                        }
                        if (delList != null && delList.Count > 0)
                        {
                            DataOperateBusiness<Epm_ProjectCompany>.Get().DeleteRange(delList);
                        }
                    }
                }
                pclist = DataOperateBusiness<Epm_ProjectCompany>.Get().GetList(p => p.ProjectId == projectId).ToList();
                pclist = pclist.OrderBy(t => t.IsAProvide).ThenBy(t => t.Sort).ToList();
                result.Data = pclist;
                result.AllRowsCount = pclist.Count();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectCompanyByProjectId");
            }
            return result;
        }
        /// <summary>
        /// 更新工程服务商（服务商、合同、委托书）
        /// </summary>
        /// <param name="list"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Result<int> UpdateProjectCompany(List<Epm_ProjectCompany> list)
        {
            Result<int> result = new Result<int>();
            try
            {
                var projectId = list[0].ProjectId.Value;
                List<Epm_ProjectCompany> oldList = DataOperateBusiness<Epm_ProjectCompany>.Get().GetList(t => t.ProjectId == projectId).ToList();
                foreach (var item in oldList)
                {
                    var newpc = list.Where(t => t.Id == item.Id).FirstOrDefault();
                    if (newpc != null)
                    {
                        if (newpc.CompanyId != item.CompanyId)
                        {
                            item.PMId = null;
                            item.PM = null;
                            item.PMPhone = null;
                            item.LinkManId = null;
                            item.LinkPhone = null;
                            item.LinkMan = null;
                            item.SafeMan = null;
                            item.SafeManId = null;
                            item.SafePhone = null;
                            item.TechnologyMan = null;
                            item.TechnologyManId = null;
                            item.TechnologyPhone = null;

                            item.PMId_New = null;
                            item.PM_New = null;
                            item.PMPhone_New = null;
                            item.LinkManId_New = null;
                            item.LinkPhone_New = null;
                            item.LinkMan_New = null;
                            item.SafeMan_New = null;
                            item.SafeManId_New = null;
                            item.SafePhone_New = null;
                            item.TechnologyMan_New = null;
                            item.TechnologyManId_New = null;
                            item.TechnologyPhone_New = null;

                            item.State = null;
                            item.LinkState = null;
                        }
                        item.CompanyId = newpc.CompanyId;
                        item.CompanyName = newpc.CompanyName;
                        item.ContractId = newpc.ContractId ?? 0;
                        item.ContractName = newpc.ContractName ?? "";
                        item.OrderId = newpc.OrderId ?? 0;
                        item.OrderName = newpc.OrderName ?? "";
                        item.OperateUserId = CurrentUserID.ToLongReq();
                        item.OperateUserName = CurrentUserName;
                        item.OperateTime = DateTime.Now;
                        item.ContractCode = newpc.ContractCode;//合同编码
                    }
                }
                var rows = DataOperateBusiness<Epm_ProjectCompany>.Get().UpdateRange(oldList);

                #region 合同、委托书/订单
                #endregion

                #region 消息
                var model = DataOperateBusiness<Epm_Project>.Get().GetModel(projectId);
                var waitSend = GetWaitSendMessageList(model.Id);
                foreach (var send in waitSend)
                {
                    Epm_Massage modelMsg = new Epm_Massage();
                    modelMsg.ReadTime = null;
                    modelMsg.RecId = send.Key;
                    modelMsg.RecName = send.Value;
                    modelMsg.RecTime = DateTime.Now;
                    modelMsg.SendId = CurrentUserID.ToLongReq();
                    modelMsg.SendName = CurrentUserName;
                    modelMsg.SendTime = DateTime.Now;
                    modelMsg.Title = CurrentUserName + "更新工程服务商信息";
                    modelMsg.Content = CurrentUserName + "更新工程服务商信息";
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = model.Id;
                    modelMsg.BussinesType = BusinessType.Project.ToString();
                    modelMsg.BussinesChild = "ISP";
                    modelMsg.ProjectId = model.Id;
                    modelMsg.ProjectName = model.Name;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                }
                #endregion

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Project.GetText(), SystemRight.Modify.GetText(), "更新工程服务商（服务商、合同、委托书）: " + projectId);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateProjectCompany");
            }
            return result;
        }
        /// <summary>
        /// 获取服务商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_ProjectCompany> GetProjectCompany(long id)
        {
            Result<Epm_ProjectCompany> result = new Result<Epm_ProjectCompany>();
            try
            {
                Epm_ProjectCompany company = DataOperateBusiness<Epm_ProjectCompany>.Get().GetModel(id);
                result.Data = company;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectCompany");
            }
            return result;
        }
        /// <summary>
        /// 更新项目经理信息
        /// </summary>
        /// <param name="projectCompany"></param>
        /// <returns></returns>
        public Result<int> UpdatePMAndPhone(Epm_ProjectCompany projectCompany)
        {
            Result<int> result = new Result<int>();
            try
            {
                int rows = -1;
                Epm_ProjectCompany pc = DataOperateBusiness<Epm_ProjectCompany>.Get().GetModel(projectCompany.Id);

                #region 更新项目经理信息
                if (IsBranchCompanyUser(CurrentUserID.ToLongReq()))//是否分公司
                {
                    //分公司修改不需要审核
                    pc.PMId_New = projectCompany.PMId_New;
                    pc.PM_New = projectCompany.PM_New;
                    pc.PMPhone_New = projectCompany.PMPhone_New;
                    pc.PMId = projectCompany.PMId_New;
                    pc.PM = projectCompany.PM_New;
                    pc.PMPhone = projectCompany.PMPhone_New;

                    pc.LinkManId_New = projectCompany.LinkManId_New;
                    pc.LinkMan_New = projectCompany.LinkMan_New;
                    pc.LinkPhone_New = projectCompany.LinkPhone_New;
                    pc.LinkManId = projectCompany.LinkManId_New;
                    pc.LinkMan = projectCompany.LinkMan_New;
                    pc.LinkPhone = projectCompany.LinkPhone_New;

                    pc.SafeManId = projectCompany.SafeManId_New;
                    pc.SafeMan = projectCompany.SafeMan_New;
                    pc.SafePhone = projectCompany.SafePhone_New;
                    pc.SafeManId_New = projectCompany.SafeManId_New;
                    pc.SafeMan_New = projectCompany.SafeMan_New;
                    pc.SafePhone_New = projectCompany.SafePhone_New;

                    pc.TechnologyManId = projectCompany.TechnologyManId_New;
                    pc.TechnologyMan = projectCompany.TechnologyMan_New;
                    pc.TechnologyPhone = projectCompany.TechnologyPhone_New;
                    pc.TechnologyManId_New = projectCompany.TechnologyManId_New;
                    pc.TechnologyMan_New = projectCompany.TechnologyMan_New;
                    pc.TechnologyPhone_New = projectCompany.TechnologyPhone_New;

                    pc.State = (int)ApprovalState.ApprSuccess;
                    pc = base.FiterUpdate(pc, pc);

                    pc.State = (int)ApprovalState.ApprSuccess;
                    rows = DataOperateBusiness<Epm_ProjectCompany>.Get().Update(pc);
                }
                else
                {
                    pc.PMId_New = projectCompany.PMId_New;
                    pc.PM_New = projectCompany.PM_New;
                    pc.PMPhone_New = projectCompany.PMPhone_New;

                    pc.LinkManId_New = projectCompany.LinkManId_New;
                    pc.LinkMan_New = projectCompany.LinkMan_New;
                    pc.LinkPhone_New = projectCompany.LinkPhone_New;

                    pc.SafeManId_New = projectCompany.SafeManId_New;
                    pc.SafeMan_New = projectCompany.SafeMan_New;
                    pc.SafePhone_New = projectCompany.SafePhone_New;

                    pc.TechnologyManId_New = projectCompany.TechnologyManId_New;
                    pc.TechnologyMan_New = projectCompany.TechnologyMan_New;
                    pc.TechnologyPhone_New = projectCompany.TechnologyPhone_New;

                    pc.State = (int)ApprovalState.WaitAppr;
                    pc = base.FiterUpdate(pc, pc);
                    rows = DataOperateBusiness<Epm_ProjectCompany>.Get().Update(pc);

                    #region 生成待办
                    List<Epm_Approver> list = new List<Epm_Approver>();
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "更新了服务商用户信息，待审核";
                    app.Content = CurrentUserName + "更新了服务商用户信息，待审核";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Project.ToString();
                    app.BussinesChild = "ISP";
                    app.Action = SystemRight.Modify.ToString();
                    app.BusinessTypeName = BusinessType.Project.GetText();
                    app.BusinessState = (int)(ApprovalState.WaitAppr);
                    app.BusinessId = pc.Id;
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(pc.ProjectId.Value);
                    app.ApproverId = project.ContactUserId;
                    app.ApproverName = project.ContactUserName;
                    app.ProjectId = pc.ProjectId;
                    app.ProjectName = project.Name;
                    list.Add(app);
                    AddApproverBatch(list);
                    WriteLog(BusinessType.Project.GetText(), SystemRight.Modify.GetText(), "服务商用户信息更新生成待办: " + pc.ProjectId);
                    #endregion

                    #region 发送短信
                    //Dictionary<string, string> parametersms = new Dictionary<string, string>();
                    //parametersms.Add("UserName", CurrentUserName);
                    //WriteSMS(project.ContactUserId.Value, project.CompanyId, MessageStep.UpdateServicePM, parametersms);
                    #endregion
                }
                #endregion

                #region 消息
                var model = DataOperateBusiness<Epm_Project>.Get().GetModel(pc.ProjectId.Value);
                var waitSend = GetWaitSendMessageList(model.Id);
                foreach (var send in waitSend)
                {
                    Epm_Massage modelMsg = new Epm_Massage();
                    modelMsg.ReadTime = null;
                    modelMsg.RecId = send.Key;
                    modelMsg.RecName = send.Value;
                    modelMsg.RecTime = DateTime.Now;
                    modelMsg.SendId = CurrentUserID.ToLongReq();
                    modelMsg.SendName = CurrentUserName;
                    modelMsg.SendTime = DateTime.Now;
                    modelMsg.Title = CurrentUserName + "更新了服务商用户信息";
                    modelMsg.Content = CurrentUserName + "更新了服务商用户信息";
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = model.Id;
                    modelMsg.BussinesType = BusinessType.Project.ToString();
                    modelMsg.BussinesChild = "ISP";
                    modelMsg.ProjectId = model.Id;
                    modelMsg.ProjectName = model.Name;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                }
                #endregion

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Project.GetText(), SystemRight.Modify.GetText(), "更新服务商用户信息: " + projectCompany.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdatePMAndPhone");
            }
            return result;
        }
        /// <summary>
        /// 审核项目经理信息
        /// </summary>
        /// <param name="projectCompany"></param>
        /// <returns></returns>
        public Result<int> AuditPMAndPhone(Epm_ProjectCompany projectCompany)
        {
            Result<int> result = new Result<int>();
            try
            {
                Epm_ProjectCompany pc = DataOperateBusiness<Epm_ProjectCompany>.Get().GetModel(projectCompany.Id);
                pc.PMId = pc.PMId_New;
                pc.PM = pc.PM_New;
                pc.PMPhone = pc.PMPhone_New;
                pc.State = (int)ApprovalState.ApprSuccess;

                pc.LinkManId = pc.LinkManId_New;
                pc.LinkMan = pc.LinkMan_New;
                pc.LinkPhone = pc.LinkPhone_New;

                pc.SafeManId = pc.SafeManId_New;
                pc.SafeMan = pc.SafeMan_New;
                pc.SafePhone = pc.SafePhone_New;
                pc.LinkState = (int)ApprovalState.ApprSuccess;
                pc = base.FiterUpdate(pc, pc);

                var rows = DataOperateBusiness<Epm_ProjectCompany>.Get().Update(pc);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Project.GetText(), SystemRight.Modify.GetText(), "服务商用户更新审核通过: " + projectCompany.Id);

                #region 处理待办
                var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == pc.Id && t.IsApprover == false).FirstOrDefault();
                if (tempApp != null)
                {
                    ComplateApprover(tempApp.Id);

                    #region 消息
                    var waitSend = GetWaitSendMessageList(pc.ProjectId.Value);
                    var model = DataOperateBusiness<Epm_Project>.Get().GetModel(pc.ProjectId.Value);
                    foreach (var send in waitSend)
                    {
                        Epm_Massage modelMsg = new Epm_Massage();
                        modelMsg.ReadTime = null;
                        modelMsg.RecId = send.Key;
                        modelMsg.RecName = send.Value;
                        modelMsg.RecTime = DateTime.Now;
                        modelMsg.SendId = CurrentUserID.ToLongReq();
                        modelMsg.SendName = CurrentUserName;
                        modelMsg.SendTime = DateTime.Now;
                        modelMsg.Title = CurrentUserName + "审核通过了" + (tempApp.SendUserId == send.Key ? "你" : tempApp.SendUserName) + "提交的服务商用户修改信息";
                        modelMsg.Content = CurrentUserName + "审核通过了" + (tempApp.SendUserId == send.Key ? "你" : tempApp.SendUserName) + "提交的服务商用户修改信息";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Project.ToString();
                        modelMsg.BussinesChild = "ISP";
                        modelMsg.ProjectId = model.Id;
                        modelMsg.ProjectName = model.Name;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AuditPMAndPhone");
            }
            return result;
        }
        /// <summary>
        /// 驳回项目经理信息
        /// </summary>
        /// <param name="projectCompany"></param>
        /// <returns></returns>
        public Result<int> RejectPMManAndPhone(Epm_ProjectCompany projectCompany)
        {
            Result<int> result = new Result<int>();
            try
            {
                Epm_ProjectCompany pc = DataOperateBusiness<Epm_ProjectCompany>.Get().GetModel(projectCompany.Id);
                pc.State = (int)ApprovalState.ApprFailure;
                pc.LinkState = (int)ApprovalState.ApprFailure;
                pc = base.FiterUpdate(pc, pc);

                var rows = DataOperateBusiness<Epm_ProjectCompany>.Get().Update(pc);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Project.GetText(), SystemRight.Modify.GetText(), "服务商用户更新审核不通过: " + projectCompany.Id);

                #region 处理待办
                var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == pc.Id && t.IsApprover == false).FirstOrDefault();
                if (tempApp != null)
                {
                    ComplateApprover(tempApp.Id);

                    #region 消息
                    var waitSend = GetWaitSendMessageList(pc.ProjectId.Value);
                    var model = DataOperateBusiness<Epm_Project>.Get().GetModel(pc.ProjectId.Value);
                    foreach (var send in waitSend)
                    {
                        Epm_Massage modelMsg = new Epm_Massage();
                        modelMsg.ReadTime = null;
                        modelMsg.RecId = send.Key;
                        modelMsg.RecName = send.Value;
                        modelMsg.RecTime = DateTime.Now;
                        modelMsg.SendId = CurrentUserID.ToLongReq();
                        modelMsg.SendName = CurrentUserName;
                        modelMsg.SendTime = DateTime.Now;
                        modelMsg.Title = CurrentUserName + "驳回了" + (tempApp.SendUserId == send.Key ? "你" : tempApp.SendUserName) + "提交的服务商用户修改信息";
                        modelMsg.Content = CurrentUserName + "驳回了" + (tempApp.SendUserId == send.Key ? "你" : tempApp.SendUserName) + "提交的服务商用户修改信息";
                        modelMsg.Type = 2;
                        modelMsg.IsRead = false;
                        modelMsg.BussinessId = model.Id;
                        modelMsg.BussinesType = BusinessType.Project.ToString();
                        modelMsg.BussinesChild = "ISP";
                        modelMsg.ProjectId = model.Id;
                        modelMsg.ProjectName = model.Name;
                        modelMsg = base.SetCurrentUser(modelMsg);
                        modelMsg = base.SetCreateUser(modelMsg);
                        DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                    }
                    #endregion
                }
                #endregion
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "RejectPMManAndPhone");
            }
            return result;
        }
        #endregion

        #region 项目工程内容要点
        /// <summary>
        /// 获取工程内容要点
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        public Result<List<Epm_ProjectWorkMainPoints>> GetProjectPointsByProjectId(long projectId)
        {
            Result<List<Epm_ProjectWorkMainPoints>> result = new Result<List<Epm_ProjectWorkMainPoints>>();
            try
            {
                var project = DataOperateBusiness<Epm_Project>.Get().GetModel(projectId);
                if (project != null)
                {
                    var pplist = DataOperateBusiness<Epm_ProjectWorkMainPoints>.Get().GetList(p => p.ProjectId == project.Id).ToList();

                    //DataOperateBusiness<Epm_ProjectWorkMainPoints>.Get().DeleteRange(pplist);
                    if (pplist.Count == 0)
                    {
                        pplist = new List<Epm_ProjectWorkMainPoints>();

                        var constitute = DataOperateBusiness<Epm_WorkMainPoints>.Get().GetList().ToList();
                        constitute = constitute.OrderBy(p => p.DicKey).ToList();

                        List<string> listKey = new List<string>();
                        foreach (var item in constitute)
                        {
                            var pp = new Epm_ProjectWorkMainPoints();
                            pp.ProjectId = project.Id;
                            pp.DicKey = item.DicKey;
                            pp.DicValue = item.DicValue;
                            pp.WorkMain = item.WorkMain;
                            pp.WorkMainValues = item.WorkMainValues;
                            pp.Unit = item.Unit;
                            pp.Sort = item.Sort;
                            pp = SetCreateUser(pp);
                            pplist.Add(pp);
                        }

                        DataOperateBusiness<Epm_ProjectWorkMainPoints>.Get().AddRange(pplist);
                    }
                    result.Data = pplist.ToList();
                    result.AllRowsCount = pplist.Count();
                    result.Flag = EResultFlag.Success;
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectPointsByProjectId");
            }
            return result;
        }
        /// <summary>
        /// 修改工程内容要点
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Result<int> UpdateProjectPoints(List<Epm_ProjectWorkMainPoints> list, long projectId)
        {
            Result<int> result = new Result<int>();
            try
            {
                //#region 记录历史
                //List<Epm_ProjectWorkMainPointsHistory> pHisList = new List<Epm_ProjectWorkMainPointsHistory>();
                //Epm_ProjectWorkMainPointsHistory pHis = null;
                //DateTime updateTime = DateTime.Now;
                //foreach (var item in list)
                //{
                //    pHis = new Epm_ProjectWorkMainPointsHistory();
                //    pHis.ProjectId = projectId;
                //    pHis.WorkMainPointsKey = item.WorkMainPointsKey;
                //    pHis.WorkMainPointsValue = item.WorkMainPointsValue;
                //    pHis.CompanyId = item.CompanyId;
                //    pHis.CompanyName = item.CompanyName;
                //    pHis.Qty = item.Qty;
                //    pHis.Description = item.Description;
                //    pHis.IsCharging = item.IsCharging;
                //    pHis.Sort = item.Sort;
                //    pHis = base.SetCurrentUser(pHis);
                //    pHis.CreateTime = updateTime;
                //    pHis.OperateTime = updateTime;
                //    pHisList.Add(pHis);
                //}
                //DataOperateBusiness<Epm_ProjectWorkMainPointsHistory>.Get().AddRange(pHisList);
                //#endregion

                #region 更新数据
                var models = DataOperateBusiness<Epm_ProjectWorkMainPoints>.Get().GetList(i => i.ProjectId == projectId).ToList();

                List<Epm_ProjectWorkMainPoints> pList = new List<Epm_ProjectWorkMainPoints>();
                Epm_ProjectWorkMainPoints p = null;

                foreach (var item in list)
                {
                    p = models.Where(i => i.Id == item.Id).First();
                    p.Val = item.Val ?? "";
                    p = base.SetCurrentUser(p);
                    pList.Add(p);
                }
                var rows = DataOperateBusiness<Epm_ProjectWorkMainPoints>.Get().UpdateRange(pList);
                #endregion

                if (pList.Count > 0)
                {
                    var project = DataOperateBusiness<Epm_Project>.Get().GetModel(projectId);
                    List<Bp_SendDate> sendList = new List<Bp_SendDate>();
                    DateTime time = DateTime.Now;
                    foreach (var temp in pList)
                    {
                        Bp_SendDate send = new Bp_SendDate();
                        send.IsSend = false;
                        send.Key = ListExtensionMethod.GetSendDateType(temp.WorkMain);
                        send.Value = temp.Val;
                        send.Type = temp.DicKey;
                        send.Project = "BIM";
                        send.KeyValue = project.ObjeId;
                        send.UserName = CurrentUser.UserCode;
                        send.KeyName = temp.WorkMain;
                        send = SetCreateUser(send);
                        send = SetCurrentUser(send);
                        send.CreateTime = time;
                        send.OperateTime = time;
                        sendList.Add(send);
                    }
                    DataOperateBusiness<Bp_SendDate>.Get().AddRange(sendList);
                }
                #region 消息
                var model = DataOperateBusiness<Epm_Project>.Get().GetModel(projectId);
                var waitSend = GetWaitSendMessageList(model.Id);
                foreach (var send in waitSend)
                {
                    Epm_Massage modelMsg = new Epm_Massage();
                    modelMsg.ReadTime = null;
                    modelMsg.RecId = send.Key;
                    modelMsg.RecName = send.Value;
                    modelMsg.RecTime = DateTime.Now;
                    modelMsg.SendId = CurrentUserID.ToLongReq();
                    modelMsg.SendName = CurrentUserName;
                    modelMsg.SendTime = DateTime.Now;
                    modelMsg.Title = CurrentUserName + "更新工程内容要点信息";
                    modelMsg.Content = CurrentUserName + "更新工程内容要点信息";
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = model.Id;
                    modelMsg.BussinesType = BusinessType.Project.ToString();
                    modelMsg.BussinesChild = "WorkPoints";
                    modelMsg.ProjectId = model.Id;
                    modelMsg.ProjectName = model.Name;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                }
                #endregion

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Project.GetText(), SystemRight.Modify.GetText(), "更新工程内容要点信息: " + projectId);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateProjectPoints");
            }
            return result;
        }

        ///// <summary>
        ///// 新增工程内容要点
        ///// </summary>
        ///// <param name="list"></param>
        ///// <param name="projectId"></param>
        ///// <returns></returns>
        //public Result<int> AddProjectPoints(List<Epm_ProjectWorkMainPoints> list, long projectId)
        //{
        //    Result<int> result = new Result<int>();
        //    try
        //    {
        //        #region 记录历史
        //        List<Epm_ProjectWorkMainPointsHistory> pHisList = new List<Epm_ProjectWorkMainPointsHistory>();
        //        Epm_ProjectWorkMainPointsHistory pHis = null;
        //        DateTime updateTime = DateTime.Now;
        //        foreach (var item in list)
        //        {
        //            pHis = new Epm_ProjectWorkMainPointsHistory();
        //            pHis.ProjectId = projectId;
        //            //pHis.WorkMainPointsKey = item.WorkMainPointsKey;
        //            //pHis.WorkMainPointsValue = item.WorkMainPointsValue;
        //            //pHis.CompanyId = item.CompanyId;
        //            //pHis.CompanyName = item.CompanyName;
        //            //pHis.Qty = item.Qty;
        //            //pHis.Description = item.Description;
        //            //pHis.IsCharging = item.IsCharging;
        //            //pHis.Sort = item.Sort;
        //            pHis = base.SetCurrentUser(pHis);
        //            pHis.CreateTime = updateTime;
        //            pHis.OperateTime = updateTime;

        //            pHisList.Add(pHis);
        //        }
        //        DataOperateBusiness<Epm_ProjectWorkMainPointsHistory>.Get().AddRange(pHisList);
        //        #endregion

        //        #region 保存数据
        //        List<Epm_ProjectWorkMainPoints> pList = new List<Epm_ProjectWorkMainPoints>();
        //        Epm_ProjectWorkMainPoints p = null;
        //        DateTime dt = DateTime.Now;
        //        foreach (var item in list)
        //        {
        //            p = new Epm_ProjectWorkMainPoints();
        //            p.ProjectId = projectId;
        //            //p.WorkMainPointsKey = item.WorkMainPointsKey;
        //            //p.WorkMainPointsValue = item.WorkMainPointsValue;
        //            //p.CompanyId = item.CompanyId;
        //            //p.CompanyName = item.CompanyName;
        //            //p.Qty = item.Qty;
        //            //p.Description = item.Description;
        //            //p.IsCharging = item.IsCharging;
        //            //p.Sort = item.Sort;
        //            p = base.SetCurrentUser(p);
        //            p.CreateTime = p.OperateTime = dt;

        //            SetCreateUser(p);

        //            pList.Add(p);
        //        }
        //        var rows = DataOperateBusiness<Epm_ProjectWorkMainPoints>.Get().AddRange(pList);
        //        #endregion

        //        #region 消息
        //        var model = DataOperateBusiness<Epm_Project>.Get().GetModel(projectId);
        //        var waitSend = GetWaitSendMessageList(model.Id);
        //        foreach (var send in waitSend)
        //        {
        //            Epm_Massage modelMsg = new Epm_Massage();
        //            modelMsg.ReadTime = null;
        //            modelMsg.RecId = send.Key;
        //            modelMsg.RecName = send.Value;
        //            modelMsg.RecTime = DateTime.Now;
        //            modelMsg.SendId = CurrentUserID.ToLongReq();
        //            modelMsg.SendName = CurrentUserName;
        //            modelMsg.SendTime = DateTime.Now;
        //            modelMsg.Title = CurrentUserName + "更新工程内容要点信息";
        //            modelMsg.Content = CurrentUserName + "更新工程内容要点信息";
        //            modelMsg.Type = 2;
        //            modelMsg.IsRead = false;
        //            modelMsg.BussinessId = model.Id;
        //            modelMsg.BussinesType = BusinessType.Project.ToString();
        //            modelMsg.BussinesChild = "WorkPoints";
        //            modelMsg.ProjectId = model.Id;
        //            modelMsg.ProjectName = model.Name;
        //            modelMsg = base.SetCurrentUser(modelMsg);
        //            modelMsg = base.SetCreateUser(modelMsg);
        //            DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
        //        }
        //        #endregion

        //        result.Data = rows;
        //        result.Flag = EResultFlag.Success;
        //        WriteLog(BusinessType.Project.GetText(), SystemRight.Modify.GetText(), "新增工程内容要点信息: " + projectId);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Data = -1;
        //        result.Flag = EResultFlag.Failure;
        //        result.Exception = new ExceptionEx(ex, "AddProjectPoints");
        //    }
        //    return result;
        //}

        /// <summary>
        /// 根据项目Id获取工程内容要点列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Result<List<Epm_ProjectWorkMainPoints>> GetProjectPointsList(long projectId)
        {
            Result<List<Epm_ProjectWorkMainPoints>> result = new Result<List<Epm_ProjectWorkMainPoints>>();
            try
            {
                var pplist = DataOperateBusiness<Epm_ProjectWorkMainPoints>.Get().GetList(p => p.ProjectId == projectId).ToList();
                result.Data = pplist;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectPointsList");
            }
            return result;
        }

        /// <summary>
        /// 获取项目工程内容要点历史
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        public Result<List<Epm_ProjectWorkMainPointsHistory>> GetProjectPointsHistoryByProjectId(long projectId)
        {
            Result<List<Epm_ProjectWorkMainPointsHistory>> result = new Result<List<Epm_ProjectWorkMainPointsHistory>>();
            try
            {
                List<Epm_ProjectWorkMainPointsHistory> pHisList = DataOperateBusiness<Epm_ProjectWorkMainPointsHistory>.Get().GetList(p => p.ProjectId == projectId).ToList();

                result.Data = pHisList;
                result.AllRowsCount = pHisList.Count();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectPointsHistoryByProjectId");
            }
            return result;
        }
        /// <summary>
        /// 获取项目工程内容要点历史
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public DataTable GetPointsHis(long projectId)
        {
            DataTable dt = new DataTable();
            //Result<List<Epm_ProjectWorkMainPointsHistory>> result = new Result<List<Epm_ProjectWorkMainPointsHistory>>();
            //result = GetProjectPointsHistoryByProjectId(projectId);
            //result.Data = result.Data.Where(t => !string.IsNullOrEmpty(t.WorkMainPointsValue)).ToList();

            //if (result.Data.Count() > 0)
            //{
            //    dt.Columns.Add("日期", System.Type.GetType("System.String"));
            //    dt.Columns.Add("修改人", System.Type.GetType("System.String"));
            //    dt.Columns.Add("类型", System.Type.GetType("System.String"));
            //    var cols = result.Data.GroupBy(t => t.WorkMainPointsValue).Select(t => t.Key).ToList();
            //    for (int i = 0; i < cols.Count(); i++)
            //    {
            //        dt.Columns.Add(cols[i], System.Type.GetType("System.String"));
            //    }

            //    var Times = result.Data.GroupBy(t => t.CreateTime).Select(t => t.Key).ToList();
            //    for (int j = 0; j < Times.Count(); j++)
            //    {
            //        var rows = result.Data.Where(t => t.CreateTime == Times[j]).ToList();
            //        if (rows.Count() > 0)
            //        {
            //            DataRow drName = dt.NewRow();
            //            DataRow drQty = dt.NewRow();
            //            DataRow drRemark = dt.NewRow();

            //            for (int i = 0; i < rows.Count(); i++)
            //            {
            //                drName["日期"] = rows[i].CreateTime.ToString("yyyy-MM-dd hh:mm:ss");
            //                drName["修改人"] = rows[i].CreateUserName;
            //                drName["类型"] = "厂家";
            //                drName[rows[i].WorkMainPointsValue] = rows[i].CompanyName;

            //                drQty["日期"] = rows[i].CreateTime.ToString("yyyy-MM-dd hh:mm:ss");
            //                drQty["修改人"] = rows[i].CreateUserName;
            //                drQty["类型"] = "数量";
            //                drQty[rows[i].WorkMainPointsValue] = rows[i].Qty.ToString("#0.00");

            //                drRemark["日期"] = rows[i].CreateTime.ToString("yyyy-MM-dd hh:mm:ss");
            //                drRemark["修改人"] = rows[i].CreateUserName;
            //                drRemark["类型"] = "备注";
            //                drRemark[rows[i].WorkMainPointsValue] = rows[i].Description;
            //            }
            //            dt.Rows.Add(drName);
            //            dt.Rows.Add(drQty);
            //            dt.Rows.Add(drRemark);
            //        }
            //    }
            //}
            return dt;
        }
        #endregion

        #region 工期信息
        /// <summary>
        /// 更新工期信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateTimelimit(Epm_Project model, List<Base_Files> attachs, bool isdelete = true)
        {
            Result<int> result = new Result<int>();
            try
            {
                var oldModel = DataOperateBusiness<Epm_Project>.Get().GetModel(model.Id);
                oldModel.ShutdownTime = model.ShutdownTime;
                //oldModel.PlanWorkStartTime = model.PlanWorkStartTime;
                //oldModel.PlanWorkEndTime = model.PlanWorkEndTime;
                //oldModel.Limit = model.Limit;
                oldModel.PlanOpeningTime = model.PlanOpeningTime;
                oldModel.PlanShutdowLimit = model.PlanShutdowLimit;
                //oldModel.PlanPackStartTime = model.PlanPackStartTime;
                //oldModel.PlanPackEndTime = model.PlanPackEndTime;
                //oldModel.PlanReinforceStartTime = model.PlanReinforceStartTime;
                //oldModel.PlanReinforceEndTime = model.PlanReinforceEndTime;
                oldModel.OperateUserId = CurrentUserID.ToLongReq();
                oldModel.OperateUserName = CurrentUserName;
                oldModel.OperateTime = DateTime.Now;

                var rows = DataOperateBusiness<Epm_Project>.Get().Update(oldModel);

                #region 保存附件
                //删除之前的附件
                //var columns = attachs.Select(p => p.TableName).ToList().Distinct();
                //foreach (var column in columns)
                //{
                //    DeleteFilesByTable(model.GetType().Name, new List<long>() { model.Id }, column);
                //}
                ////新增附件
                //AddFilesByTable(model, attachs, isdelete);

                if (attachs != null)
                {
                    //删除之前的附件
                    DeleteFilesByTable(model.GetType().Name, new List<long>() { model.Id });
                    //新增附件
                    AddFilesByTable(model, attachs);
                }

                #endregion
                if (attachs.Any() && attachs.Count > 0)
                {
                    DateTime time = DateTime.Now;
                    foreach (var item in attachs)
                    {
                        if (item.TableColumn == "Meeting")
                        {
                            Bp_SendDate send = new Bp_SendDate();
                            send.IsSend = false;
                            send.Key = "2002040005";
                            send.Value = "{\"FDFS_NAME\":\"" + item.Url + "\",\"FDFS_GROUP\":\"" + item.Group + "\",\"NAME\":\"" + item.Name + "\",\"WJLX\":\"" + ListExtensionMethod.GetFileType(item.Name) + "\",\"SIZE\":\"" + ListExtensionMethod.GetByteLength(item.Size) + "\",\"USER\":\"" + CurrentUser.UserCode + "\"}";
                            send.Type = "12";
                            send.Project = "BIM";
                            send.KeyValue = oldModel.ObjeId;
                            send.UserName = CurrentUser.UserCode;
                            send = SetCreateUser(send);
                            send = SetCurrentUser(send);
                            send.CreateTime = time;
                            send.OperateTime = time;
                            DataOperateBusiness<Bp_SendDate>.Get().Add(send);
                        }
                    }
                }

                #region 消息
                var waitSend = GetWaitSendMessageList(model.Id);
                foreach (var send in waitSend)
                {
                    Epm_Massage modelMsg = new Epm_Massage();
                    modelMsg.ReadTime = null;
                    modelMsg.RecId = send.Key;
                    modelMsg.RecName = send.Value;
                    modelMsg.RecTime = DateTime.Now;
                    modelMsg.SendId = CurrentUserID.ToLongReq();
                    modelMsg.SendName = CurrentUserName;
                    modelMsg.SendTime = DateTime.Now;
                    modelMsg.Title = CurrentUserName + "更新工期信息";
                    modelMsg.Content = CurrentUserName + "更新工期信息";
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = model.Id;
                    modelMsg.BussinesType = BusinessType.Project.ToString();
                    modelMsg.BussinesChild = "Timelimit";
                    modelMsg.ProjectId = model.Id;
                    modelMsg.ProjectName = model.Name;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                }
                #endregion

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Project.GetText(), SystemRight.Modify.GetText(), "更新工期信息: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTimelimit");
            }
            return result;
        }
        #endregion

        #region 项目资料
        /// <summary>
        /// 获取项目资料
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Result<List<Epm_ProjectDataSubmit>> GetProjectSubmitByProjectId(long projectId)
        {
            Result<List<Epm_ProjectDataSubmit>> result = new Result<List<Epm_ProjectDataSubmit>>();
            try
            {
                List<Epm_ProjectDataSubmit> pdlist = null;
                var project = DataOperateBusiness<Epm_Project>.Get().GetModel(projectId);
                if (project != null)
                {
                    pdlist = DataOperateBusiness<Epm_ProjectDataSubmit>.Get().GetList(p => p.ProjectId == project.Id).ToList();
                    if (pdlist == null || pdlist.Count == 0)
                    {
                        pdlist = new List<Epm_ProjectDataSubmit>();
                        Epm_ProjectDataSubmit pd = null;

                        var i = 1;
                        var dcList = DataOperateBusiness<Epm_DataConfig>.Get().GetList().ToList();
                        foreach (var item in dcList)
                        {
                            pd = new Epm_ProjectDataSubmit();
                            pd.ProjectId = project.Id;
                            pd.ProjectName = project.Name;
                            pd.FileId = item.Id;
                            pd.FileName = item.Name;
                            pd.State = (item.IsRequire.HasValue && item.IsRequire.Value) ? 1 : 0;
                            pd.Sort = i++;
                            pd = SetCurrentUser(pd);
                            pdlist.Add(pd);
                        }

                        DataOperateBusiness<Epm_ProjectDataSubmit>.Get().AddRange(pdlist);
                    }
                    result.Data = pdlist.OrderBy(i => i.Sort).ToList();
                    result.AllRowsCount = pdlist.Count();
                    result.Flag = EResultFlag.Success;
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectSubmitByProjectId");
            }
            return result;
        }
        /// <summary>
        /// 更新项目资料
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="attachs"></param>
        /// <returns></returns>
        public Result<int> UpdateProjectSubmit(long projectId, List<Base_Files> attachs)
        {
            Result<int> result = new Result<int>();
            try
            {
                var project = DataOperateBusiness<Epm_Project>.Get().GetModel(projectId);
                if (project != null)
                {
                    //1.删除之前的附件
                    var tableIds = attachs.Select(i => i.TableId);
                    var oldFiles = DataOperateBasic<Base_Files>.Get().GetList(i => tableIds.Contains(i.TableId) && i.TableName == "Epm_ProjectDataSubmit").ToList();
                    DataOperateBasic<Base_Files>.Get().DeleteRange(oldFiles);

                    //2.新增附件
                    List<Base_Files> fileModels = new List<Base_Files>();
                    foreach (var item in attachs)
                    {
                        SetCurrentUser(item);
                        item.TableName = "Epm_ProjectDataSubmit";
                        fileModels.Add(item);
                    }
                    DataOperateBasic<Base_Files>.Get().AddRange(fileModels);

                    //3.更新数据
                    List<Epm_ProjectDataSubmit> pdsModes = new List<Epm_ProjectDataSubmit>();
                    var pdlist = DataOperateBusiness<Epm_ProjectDataSubmit>.Get().GetList(p => p.ProjectId == project.Id && tableIds.Contains(p.Id)).ToList();
                    foreach (var item in pdlist)
                    {
                        item.UploadUserId = CurrentUserID.ToLongReq();
                        item.UploadUserName = CurrentUserName;
                        item.UploadTime = DateTime.Now;
                        item.OperateUserId = CurrentUserID.ToLongReq();
                        item.OperateUserName = CurrentUserName;
                        item.OperateTime = DateTime.Now;
                        pdsModes.Add(item);
                    }
                    DataOperateBusiness<Epm_ProjectDataSubmit>.Get().UpdateRange(pdlist);

                    result.Data = 1;
                    result.Flag = EResultFlag.Success;
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateProjectSubmit");
            }
            return result;
        }
        #endregion

        #region
        /// <summary>
        /// 获取项目性质
        /// </summary>
        /// <returns></returns>
        public Result<List<Epm_ProjectNature>> GetProjectNature()
        {
            Result<List<Epm_ProjectNature>> result = new Result<List<Epm_ProjectNature>>();
            try
            {
                var nature = DataOperateBusiness<Epm_ProjectNature>.Get().GetList().ToList();
                result.AllRowsCount = nature.Count();
                result.Data = nature;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectNature");
            }
            return result;
        }

        /// <summary>
        /// 获取项目服务商
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Result<List<Epm_ProjectCompany>> GetProjectCompanyList(long projectId)
        {
            Result<List<Epm_ProjectCompany>> result = new Result<List<Epm_ProjectCompany>>();
            try
            {
                var company = DataOperateBusiness<Epm_ProjectCompany>.Get().GetList(p => p.ProjectId == projectId).ToList();
                result.AllRowsCount = company.Count();
                result.Data = company;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "Epm_ProjectCompany");
            }
            return result;
        }

        /// <summary>
        /// 获取合同乙方单位
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Result<List<Epm_ProjectCompany>> GetProjectCompanyListByName(long projectId, string name)
        {
            Result<List<Epm_ProjectCompany>> result = new Result<List<Epm_ProjectCompany>>();
            try
            {
                var company = DataOperateBusiness<Epm_ProjectCompany>.Get().GetList(p => p.ProjectId == projectId && (string.IsNullOrEmpty(name) || p.CompanyName.Contains(name))).ToList();
                result.AllRowsCount = company.Count();
                result.Data = company;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "Epm_ProjectCompany");
            }
            return result;
        }

        #region 查询所有在建项目,根据登录人所属公司查询所有参与项目信息
        /// <summary>
        /// 选择项目弹出
        /// </summary>
        /// <returns></returns>
        public Result<List<Epm_Project>> GetProjectListByQc(QueryCondition qc)
        {
            return GetProjectListByWhr(qc, CurrentCompanyID.ToLongReq(), CurrentUserID.ToLongReq());
        }
        /// <summary>
        /// 查询所有在建项目,根据登录人所属公司查询所有参与项目信息
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Result<List<Epm_Project>> GetProjectListById(long companyId, long userId)
        {
            QueryCondition qc = new QueryCondition();
            qc.PageInfo.isAllowPage = false;
            return GetProjectListByWhr(qc, companyId, userId);
        }
        private Result<List<Epm_Project>> GetProjectListByWhr(QueryCondition qc, long companyId, long userId)
        {
            Result<List<Epm_Project>> result = new Result<List<Epm_Project>>();
            BusinessDataContext businessDB = new BusinessDataContext();
            try
            {
                qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "IsDelete",
                    ExpValue = false,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "State",
                    ExpValue = (int)ProjectState.Construction,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });

                CompanyView company = GetCompanyModel(companyId).Data;
                Base_User user = GetUserModel(userId).Data;

                var LeaderList = GetAgencyLeaderList(string.Empty, 1, int.MaxValue);
                var leader = LeaderList.Data.Where(t => t.UserCode == user.UserCode).FirstOrDefault();
                if (leader != null) //业主管理者
                {
                    result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Project>(businessDB, qc);
                    result.Flag = EResultFlag.Success;
                }
                else if (company.Type == RoleType.Owner.ToString()) //业主
                {
                    ConditionExpression ce = new ConditionExpression();
                    var PMList = GetAgencyPMList(string.Empty, 1, int.MaxValue);
                    var pm = PMList.Data.Where(t => t.UserCode == user.UserCode).FirstOrDefault();
                    if (pm != null) //省公司
                    {
                        ce.ConditionList.Add(new ConditionExpression()
                        {
                            ExpName = "PMId",
                            ExpValue = userId,
                            ExpLogical = eLogicalOperator.And,
                            ExpOperater = eConditionOperator.Equal
                        });
                        qc.ConditionList.Add(ce);
                    }
                    else //分公司
                    {
                        long deptId = IsBranchDeptDirector(userId);
                        if (deptId != 0)
                        {
                            ce.ConditionList.Add(new ConditionExpression()
                            {
                                ExpName = "CompanyId",
                                ExpValue = deptId,
                                ExpLogical = eLogicalOperator.And,
                                ExpOperater = eConditionOperator.Equal
                            });
                        }
                        else
                        {
                            deptId = IsStationManager(userId);
                            if (deptId != 0)
                            {
                                ce.ConditionList.Add(new ConditionExpression()
                                {
                                    ExpName = "ProjectSubjectId",
                                    ExpValue = deptId,
                                    ExpLogical = eLogicalOperator.And,
                                    ExpOperater = eConditionOperator.Equal
                                });
                            }
                            else
                            {
                                ce.ConditionList.Add(new ConditionExpression()
                                {
                                    ExpName = "ContactUserId",
                                    ExpValue = userId,
                                    ExpLogical = eLogicalOperator.And,
                                    ExpOperater = eConditionOperator.Equal
                                });
                            }
                        }
                        qc.ConditionList.Add(ce);
                    }
                    result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Project>(businessDB, qc);
                    result.Flag = EResultFlag.Success;
                }
                else if (company.Type == RoleType.Supplier.ToString()) //服务商
                {
                    var projectCompany = DataOperateBusiness<Epm_ProjectCompany>.Get(businessDB).GetList(t => t.CompanyId == companyId && (t.PMId == userId || t.LinkManId == userId)).ToList();
                    if (projectCompany != null && projectCompany.Count > 0)
                    {
                        var projectIds = projectCompany.Select(p => p.ProjectId).ToList();
                        var inWhr = projectIds.JoinToString(",");
                        qc.ConditionList.Add(new ConditionExpression()
                        {
                            ExpName = "Id",
                            ExpValue = inWhr,
                            ExpLogical = eLogicalOperator.And,
                            ExpOperater = eConditionOperator.In
                        });
                    }
                    else
                    {
                        qc.ConditionList.Add(new ConditionExpression()
                        {
                            ExpName = "Id",
                            ExpValue = 0,
                            ExpLogical = eLogicalOperator.And,
                            ExpOperater = eConditionOperator.Equal
                        });
                    }
                    result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Project>(businessDB, qc);
                    result.Flag = EResultFlag.Success;
                }
                else
                {
                    result.Data = null;
                    result.Flag = EResultFlag.Failure;
                    result.Exception = new ExceptionEx(new Exception("当前用户无权限"), "GetProjectListByWhr");
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectListByWhr");
            }
            finally
            {
                if (businessDB.Database.Connection.State != ConnectionState.Closed)
                {
                    businessDB.Database.Connection.Dispose();
                }
            }
            return result;
        }
        /// <summary>
        /// 是否分公司部门主任
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>返回分公司Id</returns>
        private long IsBranchDeptDirector(long userId)
        {
            var fgsrolelist = new List<string>() { Role.FGBMZR.GetText() };

            var allRole = DataOperateBasic<Base_Role>.Get().GetList(t => t.IsDelete == false && t.Belong == "Owner" && fgsrolelist.Contains(t.RoleName)).ToList();

            var roleIdList = allRole.Select(t => t.Id).ToList();

            var userRole = DataOperateBasic<Base_UserRole>.Get().GetList(t => t.IsDelete == false && t.UserId == userId && roleIdList.Contains(t.RoleId)).FirstOrDefault();

            if (userRole != null)
            {
                var user = DataOperateBasic<Base_User>.Get().GetList(t => t.IsDelete == false && t.Id == userRole.UserId).FirstOrDefault();
                if (user != null)
                {
                    return user.CompanyId;
                }
            }

            return 0;
        }

        /// <summary>
        /// 判断是否是加油站经理
        /// </summary>
        /// <param name="userId">当前登录用户 ID</param>
        /// <returns>返回加油站表 Epm_OilStation 表中的主键 ID</returns>
        private long IsStationManager(long userId)
        {
            var zjlrolelist = new List<string>() { Role.ZJL.GetText() };

            var allRole = DataOperateBasic<Base_Role>.Get().GetList(t => t.IsDelete == false && t.Belong == "Owner" && zjlrolelist.Contains(t.RoleName)).ToList();

            var roleIdList = allRole.Select(t => t.Id).ToList();

            var userRole = DataOperateBasic<Base_UserRole>.Get().GetList(t => t.IsDelete == false && t.UserId == userId && roleIdList.Contains(t.RoleId)).FirstOrDefault();

            if (userRole != null)
            {
                var user = DataOperateBasic<Base_User>.Get().GetList(t => t.IsDelete == false && t.Id == userRole.UserId).FirstOrDefault();
                if (user != null)
                {
                    // 根据当前登录用所属分公司获取分公司 ID
                    var company = DataOperateBasic<Base_Company>.Get().GetList(t => t.IsDelete == false && t.Id == user.CompanyId).FirstOrDefault();
                    if (company != null)
                    {
                        // 根据当前登录用户的所属分公司获取对应的加油站(工程主体)ID
                        var oilStation = DataOperateBusiness<Epm_OilStation>.Get().GetList(t => t.IsDelete == false && t.Code1 == company.Code).FirstOrDefault();
                        return oilStation != null ? oilStation.Id : 0;
                    }
                }
            }

            return 0;
        }

        #endregion
        #endregion

        #region 项目KPI
        /// <summary>
        /// 获取当前年份项目KPI数据
        /// </summary>
        /// <returns></returns>
        public Result<Epm_ProjectKPI> GetProjectKPIList()
        {
            string year = DateTime.Now.Year.ToString();
            Result<Epm_ProjectKPI> result = new Result<Epm_ProjectKPI>();
            try
            {
                result.Data = DataOperateBusiness<Epm_ProjectKPI>.Get().GetList(t => t.Years == year && !t.IsDelete).ToList().FirstOrDefault();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectKPIList");
            }
            return result;
        }

        /// <summary>
        /// 获取项目KPI数据
        /// </summary>
        /// <param name="years"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Result<ProjectKPIView> GetProjectKPIListByWhr(string years, long userId)
        {
            Result<ProjectKPIView> result = new Result<ProjectKPIView>();
            try
            {
                ProjectKPIView pkpi = new ProjectKPIView();
                DateTime stime = Convert.ToDateTime(years + "-01-01");
                DateTime etime = Convert.ToDateTime((Convert.ToInt32(years) + 1).ToString() + "-01-01");

                //查询当前年份的所有项目
                List<Epm_Project> ProjectList = null;
                var user = DataOperateBasic<Base_User>.Get().GetModel(userId);
                Base_Company company = DataOperateBasic<Base_Company>.Get().GetModel(user.CompanyId);

                if (company.Type == RoleType.Owner.ToString())
                {
                    ProjectList = DataOperateBusiness<Epm_Project>.Get().GetList(t => !t.IsDelete && t.PlanWorkStartTime >= stime && t.PlanWorkStartTime < etime).ToList();
                    if (ProjectList != null)
                    {
                        var LeaderList = GetAgencyLeaderList(string.Empty, 1, int.MaxValue);
                        var leader = LeaderList.Data.Where(t => t.UserCode == user.UserCode).FirstOrDefault();
                        if (leader == null)
                        {
                            var PMList = GetAgencyPMList(string.Empty, 1, int.MaxValue);
                            var pm = PMList.Data.Where(t => t.UserCode == user.UserCode).FirstOrDefault();
                            if (pm != null) //省公司
                            {
                                ProjectList = ProjectList.Where(t => t.PMId == userId).ToList();
                            }
                            else  //分公司
                            {
                                ProjectList = ProjectList.Where(t => t.ContactUserId == userId).ToList();
                            }
                        }
                    }
                }

                if (ProjectList != null)
                {
                    int delayNum = 0;
                    DateTime nowTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59");
                    var delayList = ProjectList.Where(t => t.State == (int)ProjectState.Construction && t.PlanWorkEndTime <= nowTime).ToList();
                    if (delayList.Count > 0)
                    {
                        foreach (var item in delayList)
                        {
                            var planList = DataOperateBusiness<Epm_Plan>.Get().GetList(t => t.IsFinish != 1 && t.ProjectId == item.Id).ToList();
                            if (planList.Count > 0)
                            {
                                delayNum = delayNum + 1;
                            }
                        }
                    }

                    pkpi.Years = years;
                    pkpi.DelayNum = delayNum;
                    pkpi.TotelNum = ProjectList.Count();
                    pkpi.ConstrunctionNum = ProjectList.Where(t => t.State == (int)ProjectState.Construction).Count();
                    pkpi.FinishNum = ProjectList.Where(t => t.State == (int)ProjectState.Success).Count();

                    result.Data = pkpi;
                    result.Flag = EResultFlag.Success;
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectKPIListByWhr");
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 获取项目统计数据
        /// </summary>
        /// <param name="type">1:全部，2：新增，3：改造</param>
        /// <returns></returns>
        public Result<List<ProjectCount>> GetProjectCount(int type, string time)
        {
            DateTime stime = Convert.ToDateTime(time);
            if (stime > DateTime.Now)
            {
                stime = DateTime.Now;
            }
            Result<List<ProjectCount>> result = new Result<List<ProjectCount>>();
            string projectNature = "";
            if (type == 2)
            {
                projectNature = "15,01,14,17,16,XINJ,HEZI,HEZUO,JIG";
            }
            if (type == 3)
            {
                projectNature = "2,GAIJ";
            }
            if (type == 1)
            {
                projectNature = "2,15,01,14,17,16,CANGU,KONGG,KUOJ,QIANJ,SHOUG,XINJ,ZUL,HEZI,HEZUO,GAIJ,JIG";
            }
            var query = from a in context.Epm_Project.Where(p => p.IsDelete == false)
                        join b in context.Epm_CompletionAcceptance.Where(p => p.IsDelete == false) on a.Id equals b.ProjectId into temp
                        from tt in temp.DefaultIfEmpty()
                        select new
                        {
                            a,
                            tt,
                        };
            query = query.Where(t => string.IsNullOrEmpty(projectNature) || projectNature.Contains(t.a.ProjectNature));
            int i = 1;
            var list = query.GroupBy(m => new { m.a.CompanyId, m.a.CompanyName }).Select(m => new ProjectCount
            {
                sort = i + 1,
                CompanyId = m.Key.CompanyId.ToString(),
                CompanyName = m.Key.CompanyName,
                Count = m.Count(),
                // NoStartCount = m.Where(t => t.a.State == (int)ProjectState.NoStart).ToList().Count(),
                //  FinshCount = m.Where(t => t.a.State == (int)ProjectState.Success).ToList().Count(),
                // ConstructionCount = m.Where(t => t.a.State == (int)ProjectState.Construction).ToList().Count(),
                //StartCount = m.Where(t => t.a.State != (int)ProjectState.NoStart).ToList().Count(),
                AcceptanceCount = m.Where(t => t.a.ProCompanyAcceptance == 1).ToList().Count(),
                // CommissioningCount = m.Where(t => t.a.State == (int)ProjectState.Commissioning).ToList().Count(),


            }).ToList();

            foreach (var item in list)
            {
                item.DesignSchemeCount = GetDesignSchemeCount(item.CompanyId, projectNature);
                item.CompletionAcceptanceCount = GetCompletionAcceptanceCount(item.CompanyId, projectNature);
                item.TenderingApplyCount = GetTenderingApplyCount(item.CompanyId, projectNature);
                item.CapitalTransferCount = GetTzProjectApprovalInfoCount(item.CompanyId, projectNature);
                item.ProjectPolitCount = GetTzProjectPolitCount(item.CompanyId, projectNature);


                item.NoStartCount = GetTzProjectStartApplyCount(item.CompanyId, projectNature);

                item.StartCount = GetTzProjectStartCount(item.CompanyId, projectNature);
                item.FinshCount = GetFinshCount(item.CompanyId, projectNature);
                item.ConstructionCount = GetConstructionCount(item.CompanyId, projectNature);

                item.BeingBuiltCount = GetTzProjectNewCount(item.CompanyId, projectNature);
            }


            if (list.Count > 0)
            {
                ProjectCount count = new ProjectCount();
                count.CompanyName = "合计";
                count.Count = list.Select(t => t.Count).Sum();
                count.NoStartCount = list.Select(t => t.NoStartCount).Sum();
                count.FinshCount = list.Select(t => t.FinshCount).Sum();
                //count.ConstructionCount = list.Select(t => t.ConstructionCount).Sum();
                //count.StartCount = list.Select(t => t.StartCount).Sum();
                count.AcceptanceCount = list.Select(t => t.AcceptanceCount).Sum();
                count.CommissioningCount = list.Select(t => t.CommissioningCount).Sum();

                count.DesignSchemeCount = list.Select(t => t.DesignSchemeCount).Sum();
                count.CompletionAcceptanceCount = list.Select(t => t.CompletionAcceptanceCount).Sum();
                count.TenderingApplyCount = list.Select(t => t.TenderingApplyCount).Sum();
                count.CapitalTransferCount = list.Select(t => t.CapitalTransferCount).Sum();
                count.ProjectPolitCount = list.Select(t => t.ProjectPolitCount).Sum();


                count.NoStartCount = list.Select(t => t.NoStartCount).Sum();
                count.StartCount = list.Select(t => t.StartCount).Sum();
                count.ConstructionCount = list.Select(t => t.ConstructionCount).Sum();

                count.BeingBuiltCount = list.Select(t => t.BeingBuiltCount).Sum();
                count.sort = 0;
                list.Add(count);

                list = list.OrderBy(t => t.sort).ToList();
                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            else
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
            }
            return result;
        }

        /// <summary>
        /// 在建取项目批复后到项目试运行之前的
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        private int GetTzProjectNewCount(string CompanyId, string projectNature)
        {
            int ii = 0;
            var query = from ta in context.Epm_CompletionAcceptance.Where(p => p.IsDelete == false && p.State == (int)PreProjectApprovalState.ApprovalSuccess)
                        join p in context.Epm_TzProjectProposal.Where(p => projectNature.Contains(p.Nature) && p.State == (int)PreProjectApprovalState.ApprovalSuccess) on ta.ProjectId equals p.Id
                        select new
                        {
                            p.CompanyId
                        };

            var strIds = query.ToList();

            foreach (var item in strIds)
            {
                if (item.CompanyId != null)
                {

                    if (item.CompanyId.ToString() == CompanyId)
                    {
                        ii++;
                    }
                }
            }
            return ii;

        }
        /// <summary>
        /// 正在施工=项目批复完成之后 到 项目试运行申请之前
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        private int GetConstructionCount(string CompanyId, string projectNature)
        {
            int ii = 0;
            var query = from ta in context.Epm_TzProjectPolit.Where(p => p.IsDelete == false && p.State == (int)PreProjectApprovalState.WaitApproval)
                        join p in context.Epm_TzProjectProposal.Where(p => projectNature.Contains(p.Nature)) on ta.ProjectId equals p.Id
                        select new
                        {
                            ta.CompanyId
                        };
            var strIds = query.ToList();

            foreach (var item in strIds)
            {
                if (item.CompanyId != null)
                {

                    if (item.CompanyId.ToString() == CompanyId)
                    {
                        ii++;
                    }
                }
            }
            return ii;

        }

        /// <summary>
        /// 完工项目=竣工验收申请已批复的项目
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        private int GetFinshCount(string CompanyId, string projectNature)
        {
            int ii = 0;
            var query = from ta in context.Epm_CompletionAcceptance.Where(p => p.IsDelete == false && p.State == (int)PreProjectApprovalState.ApprovalSuccess)
                        join p in context.Epm_TzProjectProposal.Where(p => projectNature.Contains(p.Nature)) on ta.ProjectId equals p.Id
                        select new
                        {
                            ta.RecCompanyId
                        };
            var strIds = query.ToList();

            foreach (var item in strIds)
            {
                if (item.RecCompanyId != null)
                {

                    if (item.RecCompanyId.ToString() == CompanyId)
                    {
                        ii++;
                    }
                }
            }
            return ii;

        }
        /// <summary>
        /// 开工项目=开工申请已批复的项目
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        private int GetTzProjectStartCount(string CompanyId, string projectNature)
        {
            int ii = 0;
            var query = from ta in context.Epm_TzProjectStartApply.Where(p => p.IsDelete == false && p.State == (int)PreProjectApprovalState.ApprovalSuccess && projectNature.Contains(p.Nature))
                        select new
                        {
                            ta.CompanyId
                        };
            var strIds = query.ToList();

            foreach (var item in strIds)
            {
                if (item.CompanyId != null)
                {

                    if (item.CompanyId.ToString() == CompanyId)
                    {
                        ii++;
                    }
                }
            }
            return ii;

        }

        /// <summary>
        /// 未开工=开工申请未批复的项目
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        private int GetTzProjectStartApplyCount(string CompanyId, string projectNature)
        {
            int ii = 0;
            var query = from ta in context.Epm_TzProjectStartApply.Where(p => p.IsDelete == false && p.State == (int)PreProjectApprovalState.WaitApproval && projectNature.Contains(p.Nature))
                        select new
                        {
                            ta.CompanyId
                        };
            var strIds = query.ToList();

            foreach (var item in strIds)
            {
                if (item.CompanyId != null)
                {

                    if (item.CompanyId.ToString() == CompanyId)
                    {
                        ii++;
                    }
                }
            }
            return ii;

        }

        /// <summary>
        /// 投运项目是项目试运行的项目，
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        private int GetTzProjectPolitCount(string CompanyId, string projectNature)
        {
            int ii = 0;
            var query = from ta in context.Epm_TzProjectPolit.Where(p => p.IsDelete == false && p.State == (int)PreProjectApprovalState.ApprovalSuccess)
                        join p in context.Epm_TzProjectProposal.Where(p => projectNature.Contains(p.Nature)) on ta.ProjectId equals p.Id
                        select new
                        {
                            ta.CompanyId
                        };

            var strIds = query.ToList();

            foreach (var item in strIds)
            {
                if (item.CompanyId != null)
                {

                    if (item.CompanyId.ToString() == CompanyId)
                    {
                        ii++;
                    }
                }
            }
            return ii;

        }
        /// <summary>
        /// 转资项目 项目批复金额大于项目决算金额的项目，批复金额是项目批复的总投资（TotalInvestment）>财务决算金额（FinanceAccounts）
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        private int GetTzProjectApprovalInfoCount(string CompanyId, string projectNature)
        {
            int ii = 0;
            var query = from ta in context.Epm_TzProjectApprovalInfo.Where(p => p.IsDelete == false && p.TotalInvestment > p.FinanceAccounts)
                        join p in context.Epm_TzProjectProposal.Where(o => projectNature.Contains(o.Nature)) on ta.ProjectId equals p.Id
                        select new
                        {
                            p.CompanyId
                        };

            var strIds = query.ToList();
            foreach (var item in strIds)
            {
                if (item.CompanyId != null)
                {

                    if (item.CompanyId.ToString() == CompanyId)
                    {
                        ii++;
                    }
                }
            }
            return ii;

        }

        /// <summary>
        /// 已完成施工图设计的项目=设计方案已经批复的项目数量
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        private int GetDesignSchemeCount(string CompanyId, string projectNature)
        {
            int ii = 0;

            var query = from ta in context.Epm_TzDesignScheme.Where(p => p.IsDelete == false && p.State == (int)PreProjectApprovalState.ApprovalSuccess && projectNature.Contains(p.Nature))
                        select new
                        {
                            ta.CompanyId
                        };
            var strIds = query.ToList();

            foreach (var item in strIds)
            {
                if (item.CompanyId != null)
                {

                    if (item.CompanyId.ToString() == CompanyId)
                    {
                        ii++;
                    }
                }
            }
            return ii;

        }

        /// <summary>
        /// 省公司验收项目=竣工验收的项目
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        private int GetCompletionAcceptanceCount(string CompanyId, string projectNature)
        {
            int ii = 0;
            var query = from ta in context.Epm_CompletionAcceptance.Where(p => p.IsDelete == false && p.State == (int)PreProjectApprovalState.ApprovalSuccess)
                        join tp in context.Epm_TzProjectProposal.Where(p => projectNature.Contains(p.Nature)) on ta.ProjectId equals tp.Id
                        select new
                        {
                            ta.RecCompanyId
                        };
            var strIds = query.ToList();

            foreach (var item in strIds)
            {
                if (item.RecCompanyId != null)
                {

                    if (item.RecCompanyId.ToString() == CompanyId)
                    {
                        ii++;
                    }
                }
            }
            return ii;

        }

        //已完成招标的项目=招标申请已经批复的项目
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        private int GetTenderingApplyCount(string CompanyId, string projectNature)
        {
            int ii = 0;
            var query = from ta in context.Epm_TzTenderingApply.Where(p => p.IsDelete == false && p.State == (int)PreProjectApprovalState.ApprovalSuccess)
                        join tp in context.Epm_TzProjectProposal.Where(p => projectNature.Contains(p.Nature)) on ta.ProjectId equals tp.Id.ToString()
                        select new
                        {
                            tp.CompanyId
                        };
            var strIds = query.ToList();

            foreach (var item in strIds)
            {
                if (item.CompanyId != null)
                {

                    if (item.CompanyId.ToString() == CompanyId)
                    {
                        ii++;
                    }
                }
            }
            return ii;

        }

        /// <summary>
        /// 项目信息汇总
        /// </summary>
        /// <param name="type">1:全部，2：新增，3：改造</param>
        /// <param name="time"></param>
        /// <param name="stateType">1:新增项目汇总，2：未完成设计，3：完工未投运，4：正在施工，5：改造项目汇总</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Result<List<ProjectView>> GetProjectSum(int type, string time, int stateType, int pageIndex, int pageSize)
        {
            Result<List<ProjectView>> result = new Result<List<ProjectView>>();

            string projectNature = "";
            if (type == 2)
            {
                projectNature = "XINJ,HEZI,HEZUO,JIG";
            }
            if (type == 3)
            {
                projectNature = "GAIJ";
            }
            if (type == 1)
            {
                projectNature = "CANGU,KONGG,KUOJ,QIANJ,SHOUG,XINJ,ZUL,HEZI,HEZUO,GAIJ,JIG";
            }
            var query = from a in context.Epm_Project.Where(p => p.IsDelete == false)
                        join b in context.Epm_CompletionAcceptance.Where(p => p.IsDelete == false) on a.Id equals b.ProjectId into temp
                        from tt in temp.DefaultIfEmpty()
                        select new
                        {
                            a,
                            tt,
                        };
            query = query.OrderByDescending(t => t.a.CompanyName).Where(t => string.IsNullOrEmpty(projectNature) || projectNature.Contains(t.a.ProjectNature));

            if (!string.IsNullOrEmpty(time))
            {
                DateTime stime = Convert.ToDateTime(time);

                //if (stateType == 2)//未完成设计
                //{
                //    query = query.OrderByDescending(t => t.a.CompanyName).Where(t => t.a.FinshDesign != 1);
                //}
                //if (stateType == 3)//完工未投运
                //{
                //    query = query.OrderByDescending(t => t.a.CompanyName).Where(t => t.a.State == (int)ProjectState.Success);
                //}
                //if (stateType == 4)//正在施工
                //{
                //    query = query.OrderByDescending(t => t.a.CompanyName).Where(t => t.a.State == (int)ProjectState.Construction);
                //}
                //if (stateType == 7)//未开工
                //{
                //    query = query.OrderByDescending(t => t.a.CompanyName).Where(t => t.a.State == (int)ProjectState.NoStart);
                //}
            }
            List<ProjectView> listView = new List<ProjectView>();
            var list = query.ToList();
            result.AllRowsCount = list.Count;
            list = list.OrderByDescending(t => t.a.CompanyName).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    ProjectView view = new ProjectView();
                    view.CompanyName = item.a.CompanyName ?? "";
                    view.Name = item.a.Name ?? "";
                    view.ProjectNatureName = item.a.ProjectNatureName ?? "";
                    view.PlanWorkStartTime = item.a.PlanWorkStartTime;
                    view.PlanWorkEndTime = item.a.PlanWorkEndTime;
                    view.Limit = item.a.Limit ?? 0;
                    view.FinalLimit = item.a.PlanWorkStartTime == null ? 0 : DateTime.Now.Subtract(item.a.PlanWorkStartTime.Value).Days + 1;
                    view.SurplusLimit = item.a.PlanWorkStartTime == null ? 0 : item.a.Limit.Value - (DateTime.Now.Subtract(item.a.PlanWorkStartTime.Value).Days + 1);
                    view.WorkSchedule = item.a.PlanWorkStartTime == null ? 0 : ((DateTime.Now.Subtract(item.a.PlanWorkStartTime.Value).Days + 1) / item.a.Limit.Value) * 100;
                    view.WorkUnit = GetProjectCompany(item.a.Id, "土建") == null ? "" : GetProjectCompany(item.a.Id, "土建").CompanyName;
                    view.WorkUnitPMName = GetProjectCompany(item.a.Id, "土建") == null ? "" : GetProjectCompany(item.a.Id, "土建").PM;
                    view.SupervisorUnit = GetProjectCompany(item.a.Id, "监理") == null ? "" : GetProjectCompany(item.a.Id, "监理").CompanyName;
                    view.SupervisorUnitName = GetProjectCompany(item.a.Id, "监理") == null ? "" : GetProjectCompany(item.a.Id, "监理").PM;
                    view.InvestMoney = item.a.Amount;
                    view.ReplyDate = item.a.ReplyDate;
                    view.RecTime = item.tt == null ? null : item.tt.RecTime;
                    view.DesignUnit = GetProjectCompany(item.a.Id, "设计费") == null ? "" : GetProjectCompany(item.a.Id, "设计费").CompanyName;

                    view.GasDailySales = GetGasDailySales(item.a.TzProjectId) == null ? 0 : GetGasDailySales(item.a.TzProjectId).GasDailySales;//当前日销量

                    view.ApprovalNo = GetTzDesignScheme(item.a.TzProjectId) == null ? "" : GetProjectApprovalInfo(item.a.TzProjectId).ApprovalNo;//
                    view.ReplyTime = GetProjectApprovalInfo(item.a.TzProjectId) == null ? null : GetProjectApprovalInfo(item.a.TzProjectId).OperateTime;//
                    view.FinanceTime = GetProjectApprovalInfo(item.a.TzProjectId) == null ? null : GetProjectApprovalInfo(item.a.TzProjectId).FinanceTime;//
                    view.DesignSchemeTime = GetTzDesignScheme(item.a.TzProjectId) == null ? null : GetTzDesignScheme(item.a.TzProjectId).OperateTime;//


                    view.BidResultTime = GetTzBidResult(item.a.TzProjectId) == null ? null : GetTzBidResult(item.a.TzProjectId).OperateTime;//
                    view.ProjectPolitTime = GetTzProjectPolit(item.a.TzProjectId) == null ? null : GetTzProjectPolit(item.a.TzProjectId).OperateTime;//
                                                                                                                                                     /*  view.ConsumptionPeriod =GetTzProjectPolit(item.a.TzProjectId)==null ? 0 : Convert.ToInt32(GetTzProjectStartApply(item.a.TzProjectId).OperateTime.Value.Subtract(GetCompletionAcceptance(item.a.TzProjectId).OperateTime.Value)) ;*///

                    DateTime? time1 = GetTzProjectStartApply(item.a.TzProjectId).StartApplyTime;
                    DateTime? time2 = GetCompletionAcceptance(item.a.TzProjectId).RecTime;
                    if (time1 != null && time2 != null)
                    {
                        TimeSpan ts1 = new TimeSpan(time1.Value.Ticks);

                        TimeSpan ts2 = new TimeSpan(time2.Value.Ticks);

                        TimeSpan ts = ts1.Subtract(ts2).Duration();

                        view.ConsumptionPeriod = ts.Days;
                    }

                    view.ProjectState = GetProjectState(item.a.TzProjectId);
                    listView.Add(view);
                }
            }

            if (list.Count > 0)
            {
                result.Data = listView;
                result.Flag = EResultFlag.Success;
            }
            else
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
            }

            return result;
        }

        public string GetProjectState(long projectId)
        {
            string ProjectState = "项目提出";
            try
            {
                //加油站项目信息表
                var tzProjectProposal = DataOperateBusiness<Epm_TzProjectProposal>.Get().Single(p => p.Id == projectId && p.State == (int)PreProjectState.ApprovalSuccess);

                if (tzProjectProposal != null)
                {
                    //现场工程方面调研
                    var tzResearchOfEngineering = DataOperateBusiness<Epm_TzResearchOfEngineering>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.Submitted));
                    if (tzResearchOfEngineering != null)
                    {
                        ProjectState = "项目勘探";
                    }
                    //土地谈判协议
                    var tzLandNegotiation = DataOperateBusiness<Epm_TzLandNegotiation>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.Submitted));
                    if (tzLandNegotiation != null)
                    {
                        ProjectState = "土地谈判协议";
                    }
                    //初次谈判
                    var tzFirstNegotiation = DataOperateBusiness<Epm_TzFirstNegotiation>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.Submitted));
                    if (tzFirstNegotiation != null)
                    {
                        ProjectState = "初次谈判";
                    }



                    //评审材料上报
                    var tzFormTalkFile = DataOperateBusiness<Epm_TzFormTalkFile>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.ApprovalSuccess));
                    if (tzFormTalkFile != null)
                    {
                        ProjectState = "评审材料上报";
                    }

                    //项目评审
                    var tzProjectReveiews = DataOperateBusiness<Epm_TzProjectReveiews>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.ApprovalSuccess));
                    if (tzProjectReveiews != null)
                    {
                        ProjectState = "项目评审";
                    }

                    //会议决策
                    var meetingFileReport = DataOperateBusiness<Epm_MeetingFileReport>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.ApprovalSuccess));
                    if (meetingFileReport != null)
                    {
                        ProjectState = "会议决策";
                    }

                    //项目批复请示
                    var tzProjectApprovalInfo = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.ApprovalSuccess));
                    if (tzProjectApprovalInfo != null)
                    {
                        ProjectState = "项目批复";
                    }

                    //设计方案
                    var tzDesignScheme = DataOperateBusiness<Epm_TzDesignScheme>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.ApprovalSuccess));
                    if (tzDesignScheme != null)
                    {
                        ProjectState = "设计方案";
                    }

                    //图纸会审
                    var tzConDrawing = DataOperateBusiness<Epm_TzConDrawing>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.ApprovalSuccess));
                    if (tzConDrawing != null)
                    {
                        ProjectState = "图纸会审";
                    }

                    //招标管理
                    var tzBidResult = DataOperateBusiness<Epm_TzBidResult>.Get().Single(p => p.ProjectId == projectId.ToString() && (p.State == (int)PreProjectState.ApprovalSuccess));
                    if (tzBidResult != null)
                    {
                        ProjectState = "招标管理";
                    }

                    //物资申请
                    var tzSupplyMaterialApply = DataOperateBusiness<Epm_TzSupplyMaterialApply>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.ApprovalSuccess));
                    if (tzSupplyMaterialApply != null)
                    {
                        ProjectState = "物资申请";
                    }

                    //开工申请
                    var tzProjectStartApply = DataOperateBusiness<Epm_TzProjectStartApply>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.ApprovalSuccess));
                    if (tzProjectStartApply != null)
                    {
                        ProjectState = "开工申请";
                    }



                    //竣工管理
                    var completionAcceptanceResUpload = DataOperateBusiness<Epm_CompletionAcceptanceResUpload>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.ApprovalSuccess));
                    if (completionAcceptanceResUpload != null)
                    {
                        ProjectState = "竣工管理";
                    }

                    //试运行
                    var tzProjectPolit = DataOperateBusiness<Epm_TzProjectPolit>.Get().Single(p => p.ProjectId == projectId && (p.State == (int)PreProjectState.ApprovalSuccess));
                    if (tzProjectPolit != null)
                    {
                        ProjectState = "试运行";
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return ProjectState;
        }

        //实际消耗工期是项目开工申请批复到项目竣工申请批复的时间
        public Epm_TzProjectStartApply GetTzProjectStartApply(long id)
        {
            var model = DataOperateBusiness<Epm_TzProjectStartApply>.Get().GetList(t => t.ProjectId == id).FirstOrDefault();
            if (model != null)
            {
                return model;
            }
            return new Epm_TzProjectStartApply();
        }
        public Epm_CompletionAcceptance GetCompletionAcceptance(long id)
        {
            var model = DataOperateBusiness<Epm_CompletionAcceptance>.Get().GetList(t => t.ProjectId == id).FirstOrDefault();
            if (model != null)
            {
                return model;
            }
            return new Epm_CompletionAcceptance();
        }

        //投运时间是项目试运行批复时间
        public Epm_TzProjectPolit GetTzProjectPolit(long id)
        {
            var model = DataOperateBusiness<Epm_TzProjectPolit>.Get().GetList(t => t.ProjectId == id).FirstOrDefault();
            if (model != null)
            {
                return model;
            }
            return null;
        }

        //招标完成时间是招标结果上传后审批完成时间
        public Epm_TzBidResult GetTzBidResult(long id)
        {
            var model = DataOperateBusiness<Epm_TzBidResult>.Get().GetList(t => t.ProjectId == id.ToString()).FirstOrDefault();
            if (model != null)
            {
                return model;
            }
            return new Epm_TzBidResult();
        }

        //施工图设计完成时间是设计方案完批复完成时间
        public Epm_TzDesignScheme GetTzDesignScheme(long id)
        {
            var model = DataOperateBusiness<Epm_TzDesignScheme>.Get().GetList(t => t.ProjectId == id).FirstOrDefault();
            if (model != null)
            {
                return model;
            }
            return new Epm_TzDesignScheme();
        }

        //投资文号是项目批复中的批复文号
        //落地时间是项目批复时间
        public Epm_TzProjectApprovalInfo GetProjectApprovalInfo(long id)
        {
            var model = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().GetList(t => t.ProjectId == id && t.State == (int)PreProjectState.ApprovalSuccess).FirstOrDefault();
            if (model != null)
            {
                return model;
            }
            return new Epm_TzProjectApprovalInfo();
        }

        public Epm_TzResearchOfManagement GetGasDailySales(long id)
        {
            var model = DataOperateBusiness<Epm_TzResearchOfManagement>.Get().GetList(t => t.ProjectId == id).FirstOrDefault();
            if (model != null)
            {
                return model;
            }
            return new Epm_TzResearchOfManagement();
        }
        public Epm_ProjectCompany GetProjectCompany(long id, string type)
        {
            var model = DataOperateBusiness<Epm_ProjectCompany>.Get().GetList(t => t.ProjectId.Value == id && t.Type == type).FirstOrDefault();
            if (model != null)
            {
                return model;
            }
            return new Epm_ProjectCompany();
        }

        public Result<Epm_TzProjectApprovalInfo> GetProjectApprovalInfos(long projectId)
        {
            Result<Epm_TzProjectApprovalInfo> result = new Result<Epm_TzProjectApprovalInfo>();
            try
            {
                var model = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().Single(p => p.ProjectId == projectId && p.State == (int)PreProjectState.ApprovalSuccess);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectApprovalInfos");
            }
            return result;
        }

        #region 周报
        /// <summary>
        /// 定时插入周报数据
        /// Type:1:全部，2：新增，3：改造
        /// StateType:1:新增项目汇总，2：未完成设计，3：完工未投运，4：正在施工，5：改造项目汇总，7:未开工
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public Result<int> AddProjectWeekly(string time)
        {
            Result<int> result = new Result<int>();
            var startDate = new DateTime();
            var endDate = new DateTime();
            if (!string.IsNullOrEmpty(time))
            {
                startDate = Convert.ToDateTime(time.Split('~')[0]);
                endDate = Convert.ToDateTime(time.Split('~')[1]);

            }
            var query = from a in context.Epm_Project.Where(p => p.IsDelete == false && p.CreateTime > startDate && p.CreateTime <= endDate)
                        join b in context.Epm_CompletionAcceptance.Where(p => p.IsDelete == false) on a.Id equals b.ProjectId into temp
                        from tt in temp.DefaultIfEmpty()
                        select new
                        {
                            a,
                            tt,
                        };
            query = query.OrderByDescending(t => t.a.CompanyName);//.Where(t => string.IsNullOrEmpty(projectNature) || projectNature.Contains(t.a.ProjectNature));

            List<Epm_ProjectWeekly> listModel = new List<Epm_ProjectWeekly>();
            var list = query.ToList();

            list = list.OrderByDescending(t => t.a.CompanyName).ToList();//.Skip((pageIndex - 1) * pageSize).Take(pageSize)
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    Epm_ProjectWeekly model = new Epm_ProjectWeekly();
                    model.CompanyName = item.a.CompanyName ?? "";
                    model.Name = item.a.Name ?? "";
                    model.ProjectNatureName = item.a.ProjectNatureName ?? "";
                    model.PlanWorkStartTime = item.a.PlanWorkStartTime;
                    model.PlanWorkEndTime = item.a.PlanWorkEndTime;
                    model.Limit = item.a.Limit ?? 0;
                    model.FinalLimit = item.a.PlanWorkStartTime == null ? 0 : DateTime.Now.Subtract(item.a.PlanWorkStartTime.Value).Days + 1;
                    model.SurplusLimit = item.a.PlanWorkStartTime == null ? 0 : item.a.Limit.Value - (DateTime.Now.Subtract(item.a.PlanWorkStartTime.Value).Days + 1);
                    model.WorkSchedule = item.a.PlanWorkStartTime == null ? 0 : ((DateTime.Now.Subtract(item.a.PlanWorkStartTime.Value).Days + 1) / item.a.Limit.Value) * 100;
                    model.WorkUnit = GetProjectCompany(item.a.Id, "土建") == null ? "" : GetProjectCompany(item.a.Id, "土建").CompanyName;
                    model.WorkUnitPMName = GetProjectCompany(item.a.Id, "土建") == null ? "" : GetProjectCompany(item.a.Id, "土建").PM;
                    model.SupervisorUnit = GetProjectCompany(item.a.Id, "监理") == null ? "" : GetProjectCompany(item.a.Id, "监理").CompanyName;
                    model.SupervisorUnitName = GetProjectCompany(item.a.Id, "监理") == null ? "" : GetProjectCompany(item.a.Id, "监理").PM;
                    model.InvestMoney = item.a.Amount;
                    model.ReplyDate = item.a.ReplyDate;
                    model.RecTime = item.tt == null ? null : item.tt.RecTime;
                    model.DesignUnit = GetProjectCompany(item.a.Id, "设计费") == null ? "" : GetProjectCompany(item.a.Id, "设计费").CompanyName;
                    model.GasDailySales = GetGasDailySales(item.a.TzProjectId) == null ? 0 : GetGasDailySales(item.a.TzProjectId).GasDailySales;
                    model.ApprovalNo = GetTzDesignScheme(item.a.TzProjectId) == null ? "" : GetProjectApprovalInfo(item.a.TzProjectId).ApprovalNo;
                    model.ReplyTime = GetProjectApprovalInfo(item.a.TzProjectId) == null ? null : GetProjectApprovalInfo(item.a.TzProjectId).OperateTime;
                    model.FinanceTime = GetProjectApprovalInfo(item.a.TzProjectId) == null ? null : GetProjectApprovalInfo(item.a.TzProjectId).FinanceTime;
                    model.DesignSchemeTime = GetTzDesignScheme(item.a.TzProjectId) == null ? null : GetTzDesignScheme(item.a.TzProjectId).OperateTime;
                    model.BidResultTime = GetTzBidResult(item.a.TzProjectId) == null ? null : GetTzBidResult(item.a.TzProjectId).OperateTime;
                    model.ProjectPolitTime = GetTzProjectPolit(item.a.TzProjectId) == null ? null : GetTzProjectPolit(item.a.TzProjectId).OperateTime;
                    DateTime? time1 = GetTzProjectStartApply(item.a.TzProjectId).StartApplyTime;
                    DateTime? time2 = GetCompletionAcceptance(item.a.TzProjectId).RecTime;
                    if (time1 != null && time2 != null)
                    {
                        TimeSpan ts1 = new TimeSpan(time1.Value.Ticks);
                        TimeSpan ts2 = new TimeSpan(time2.Value.Ticks);
                        TimeSpan ts = ts1.Subtract(ts2).Duration();
                        model.ConsumptionPeriod = ts.Days;
                    }
                    model.Type = item.a.ProjectNature.Contains("XINJ,HEZI,HEZUO,JIG") ? 2 : item.a.ProjectNature.Contains("GAIJ") ? 3 : 1;
                    model.StateType = item.a.FinshDesign != 1 ? 2 : item.a.State == (int)ProjectState.Success ? 3 : item.a.State == (int)ProjectState.Construction ? 4 : 7;
                    model.ProjectState = GetProjectState(item.a.TzProjectId);
                    listModel.Add(model);
                }
            }
            try
            {
                var rows = DataOperateBusiness<Epm_ProjectWeekly>.Get().AddRange(listModel);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddRangeProjectWeekly");
            }
            return result;
        }
        /// <summary>
        /// Type:1:全部，2：新增，3：改造
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public Result<int> AddProjectCountWeekly(string time)
        {
            DateTime stime = Convert.ToDateTime(time);
            var startDate = new DateTime();
            var endDate = new DateTime();
            if (!string.IsNullOrEmpty(time))
            {
                startDate = Convert.ToDateTime(time.Split('~')[0]);
                endDate = Convert.ToDateTime(time.Split('~')[1]);
            }
            Result<int> result = new Result<int>();
            string projectNature = "";

            var query = from a in context.Epm_Project.Where(p => p.IsDelete == false && (p.CreateTime > startDate && p.CreateTime <= endDate))
                        join b in context.Epm_CompletionAcceptance.Where(p => p.IsDelete == false) on a.Id equals b.ProjectId into temp
                        from tt in temp.DefaultIfEmpty()
                        select new
                        {
                            a,
                            tt,
                        };
            query = query.Where(t => string.IsNullOrEmpty(projectNature) || projectNature.Contains(t.a.ProjectNature));
            int i = 1;
            var list = query.GroupBy(m => new { m.a.CompanyId, m.a.CompanyName, m.a.ProjectNature }).Select(m => new Epm_ProjectCountWeekly
            {
                sort = i + 1,
                CompanyId = m.Key.CompanyId,
                CompanyName = m.Key.CompanyName,
                Count = m.Count(),
                AcceptanceCount = m.Where(t => t.a.ProCompanyAcceptance == 1).ToList().Count(),
                Type = m.Key.ProjectNature.Contains("XINJ,HEZI,HEZUO,JIG") ? 2 : m.Key.ProjectNature.Contains("GAIJ") ? 3 : 1
            }).ToList();
            foreach (var item in list)
            {
                Epm_ProjectCountWeekly model = new Epm_ProjectCountWeekly();
                model.CompletionAcceptanceCount = GetCompletionAcceptanceCount(item.CompanyId.ToString(), projectNature);
                model.TenderingApplyCount = GetTenderingApplyCount(item.CompanyId.ToString(), projectNature);
                model.CapitalTransferCount = GetTzProjectApprovalInfoCount(item.CompanyId.ToString(), projectNature);
                model.ProjectPolitCount = GetTzProjectPolitCount(item.CompanyId.ToString(), projectNature);
                model.NoStartCount = GetTzProjectStartApplyCount(item.CompanyId.ToString(), projectNature);
                model.StartCount = GetTzProjectStartCount(item.CompanyId.ToString(), projectNature);
                model.FinshCount = GetFinshCount(item.CompanyId.ToString(), projectNature);
                model.ConstructionCount = GetConstructionCount(item.CompanyId.ToString(), projectNature);
                model.BeingBuiltCount = GetTzProjectNewCount(item.CompanyId.ToString(), projectNature);
                model.Type = item.Type;
            }
            if (list.Count > 0)
            {
                Epm_ProjectCountWeekly count = new Epm_ProjectCountWeekly();
                count.CompanyName = "合计";
                count.Count = list.Select(t => t.Count).Sum();
                count.NoStartCount = list.Select(t => t.NoStartCount).Sum();
                count.FinshCount = list.Select(t => t.FinshCount).Sum();
                count.AcceptanceCount = list.Select(t => t.AcceptanceCount).Sum();
                count.DesignSchemeCount = list.Select(t => t.DesignSchemeCount).Sum();
                count.CompletionAcceptanceCount = list.Select(t => t.CompletionAcceptanceCount).Sum();
                count.TenderingApplyCount = list.Select(t => t.TenderingApplyCount).Sum();
                count.CapitalTransferCount = list.Select(t => t.CapitalTransferCount).Sum();
                count.ProjectPolitCount = list.Select(t => t.ProjectPolitCount).Sum();
                count.NoStartCount = list.Select(t => t.NoStartCount).Sum();
                count.StartCount = list.Select(t => t.StartCount).Sum();
                count.ConstructionCount = list.Select(t => t.ConstructionCount).Sum();
                count.BeingBuiltCount = list.Select(t => t.BeingBuiltCount).Sum();
                count.sort = 0;
                list.Add(count);
                list = list.OrderBy(t => t.sort).ToList();
            }
            try
            {
                var rows = DataOperateBusiness<Epm_ProjectCountWeekly>.Get().AddRange(list);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddRangeProjectCountWeekly");
            }
            return result;
        }
        /// <summary>
        /// 获取周报信息
        /// </summary>
        /// <param name="type">1:全部，2：新增，3：改造</param>
        /// <param name="time"></param>
        /// <param name="stateType">1:新增项目汇总，2：未完成设计，3：完工未投运，4：正在施工，5：改造项目汇总，7:未开工</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Result<List<Epm_ProjectWeekly>> GetProjectWeekly(int type, string time, int stateType, int pageIndex, int pageSize)
        {
            Result<List<Epm_ProjectWeekly>> result = new Result<List<Epm_ProjectWeekly>>();
            var startDate = new DateTime();
            var endDate = new DateTime();
            if (!string.IsNullOrEmpty(time))
            {
                startDate = Convert.ToDateTime(time.Split('~')[0]);
                endDate = Convert.ToDateTime(time.Split('~')[1]);
            }
            try
            {
                var model = DataOperateBusiness<Epm_ProjectWeekly>.Get().GetList(p => p.Type == type && p.StateType == stateType && (p.CreateTime > startDate && p.CreateTime <= endDate)).ToList();
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectWeekly");
            }
            return result;
        }
        /// <summary>
        /// 获取周报汇总信息
        /// </summary>
        /// <param name="type">1:全部，2：新增，3：改造</param>
        /// <param name="time"></param>
        /// <returns></returns>
        public Result<List<Epm_ProjectCountWeekly>> GetProjectCountWeekly(int type, string time)
        {
            Result<List<Epm_ProjectCountWeekly>> result = new Result<List<Epm_ProjectCountWeekly>>();
            var startDate = new DateTime();
            var endDate = new DateTime();
            if (!string.IsNullOrEmpty(time))
            {
                startDate = Convert.ToDateTime(time.Split('~')[0]);
                endDate = Convert.ToDateTime(time.Split('~')[1]);
            }
            try
            {
                var model = DataOperateBusiness<Epm_ProjectCountWeekly>.Get().GetList(p => p.Type == type && (p.CreateTime > startDate && p.CreateTime <= endDate)).ToList();
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "Epm_ProjectCountWeekly");
            }
            return result;
        }
        #endregion
    }
}