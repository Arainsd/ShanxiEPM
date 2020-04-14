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

using hc.Plat.Common.Service;
using System.Configuration;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        #region 移动端方法
        /// <summary>
        /// 新增检查单
        /// </summary>
        /// <param name="type">1：暂存</param>
        /// <returns></returns>
        public Result<bool> AddMonitorNew(long projectId, DateTime time, List<CheckView> dataList, int type = 0)
        {
            Result<bool> result = new Result<bool>();
            //try
            //{
            if (dataList == null && dataList.Count == 0)
            {
                throw new Exception("请填写检查相关内容！");
            }

            ///获取项目信息
            var project = DataOperateBusiness<Epm_Project>.Get().GetModel(projectId);
            ///获取服务商信息
            var companys = DataOperateBusiness<Epm_ProjectCompany>.Get().GetList(t => t.ProjectId == projectId).ToList();
            ///复查单位
            long currentUserId = CurrentUserID.ToLongReq();
            string roleType = "";
            bool isAgency = IsAgencyUser(currentUserId);
            if (!isAgency)
            {
                bool isBranch = IsBranchCompanyUser(currentUserId);
                if (!isBranch)
                {
                    bool isSupervisor = IsSupervisor(projectId, currentUserId);
                    if (isSupervisor)
                    {
                        roleType = RoleTypeEnum.JL.GetText();
                    }
                    else
                    {
                        roleType = RoleTypeEnum.ZJL.GetText();
                    }
                }
                else
                {
                    roleType = RoleTypeEnum.FGS.GetText();
                }
            }
            else
            {
                roleType = RoleTypeEnum.SGS.GetText();
            }

            #region 检查单
            ///保存检查单
            Epm_Inspect inspect = new Epm_Inspect();
            inspect.ProjectId = project.Id;
            inspect.ProjectName = project.Name;
            inspect.InspectName = "防渗改造项目施工检查表";
            inspect.InspectAddress = project.Area + project.Address;
            inspect.InspectDate = time;
            inspect.InspectUserId = currentUserId;
            inspect.InspectUserName = CurrentUserName;
            inspect.IsDraft = type == 1 ? true : false;
            SetCreateUser(inspect);
            SetCurrentUser(inspect);

            ///获取检查配置数据
            var checkItem = DataOperateBusiness<Epm_CheckItem>.Get().GetList(t => t.IsDelete == false).ToList();

            ///复查数据
            List<Epm_InspectItem> fhfcList = GetInspectItemByProjectId(projectId).Data;

            ///检查单对象
            List<Epm_InspectItem> inspectItemList = new List<Epm_InspectItem>();
            Epm_InspectItem inspectItem = null;
            List<Epm_InspectScore> scoreList = new List<Epm_InspectScore>();
            Epm_InspectScore score = null;

            ///整改单对象
            Dictionary<string, List<Epm_RectificationItem>> dic = new Dictionary<string, List<Epm_RectificationItem>>();
            List<Epm_RectificationItem> rectifItemList = null;
            Epm_RectificationItem rectifItem2 = null;
            Epm_RectificationItem rectifItem3 = null;
            Dictionary<string, List<Epm_RectificationRecord>> recordList = new Dictionary<string, List<Epm_RectificationRecord>>();
            List<Epm_RectificationRecord> rectifRecordList = null;
            Epm_RectificationRecord record = null;

            ///非常规作业
            List<EPM_UnconventionalWork> unWorkList = new List<EPM_UnconventionalWork>();
            EPM_UnconventionalWork work = null;

            ///一级名称
            string name1 = string.Empty;

            foreach (var fenbu in dataList)
            {
                if (fenbu.selected.ToLower() == "true")
                {
                    ///分步(一级)
                    var fb = checkItem.Where(t => t.Id == fenbu.id.ToLongReq()).FirstOrDefault();
                    inspectItem = new Epm_InspectItem();
                    inspectItem.InspectId = inspect.Id;
                    inspectItem.CheckId = fb.Id;
                    inspectItem.CheckName = fb.Name;
                    inspectItem.CheckParentId = fb.ParentId;
                    inspectItem.CheckParentName = fb.ParentName;
                    inspectItem.Level = fb.Level;
                    inspectItem.Choice = fenbu.selected.ToLower() == "false" ? false : true;
                    SetCreateUser(inspectItem);
                    SetCurrentUser(inspectItem);
                    inspectItemList.Add(inspectItem);

                    name1 = fb.Name;
                    var fenxianglist = fenbu.children;
                    if (fenxianglist.Count > 0)
                    {
                        foreach (var fenxiang in fenxianglist)
                        {
                            rectifItemList = new List<Epm_RectificationItem>();
                            rectifRecordList = new List<Epm_RectificationRecord>();

                            ///分项
                            var fx = checkItem.Where(t => t.Id == fenxiang.id.ToLongReq()).FirstOrDefault();
                            if (fx != null)
                            {
                                #region 检查单二级
                                inspectItem = new Epm_InspectItem();
                                inspectItem.InspectId = inspect.Id;
                                inspectItem.CheckId = fx.Id;
                                inspectItem.CheckName = fx.Name;
                                inspectItem.CheckParentId = fx.ParentId;
                                inspectItem.CheckParentName = fx.ParentName;
                                inspectItem.Level = fx.Level;
                                inspectItem.Choice = fenxiang.selected.ToLower() == "false" ? false : true;
                                inspectItem.Remark = fenxiang.remark;
                                SetCreateUser(inspectItem);
                                SetCurrentUser(inspectItem);
                                inspectItemList.Add(inspectItem);
                                #endregion

                                #region 整改单，整改项2级数据准备
                                ///整改单，整改项2级数据准备
                                if (!inspect.IsDraft.Value && fenxiang.children.Count > 0)
                                {
                                    rectifItem2 = new Epm_RectificationItem();
                                    rectifItem2.CheckId = fx.Id;
                                    rectifItem2.CheckName = fx.Name;
                                    rectifItem2.CheckParentId = fx.ParentId;
                                    rectifItem2.CheckParentName = fx.ParentName;
                                    rectifItem2.Level = fx.Level;
                                    rectifItem2.Remark = fenxiang.remark;
                                    SetCreateUser(rectifItem2);
                                    SetCurrentUser(rectifItem2);

                                    record = new Epm_RectificationRecord();
                                    record.RectficationItemId = rectifItem2.Id;
                                    record.RectficationDescption = string.Empty;
                                    record.State = (int)RectificationState.WaitRectification;
                                    SetCreateUser(record);
                                    SetCurrentUser(record);
                                }
                                #endregion
                            }

                            //三级
                            var itemlist = fenxiang.children;
                            if (itemlist.Count > 0)
                            {
                                foreach (var item in itemlist)
                                {
                                    ///具体要求
                                    var check = checkItem.Where(t => t.Id == item.id.ToLongReq()).FirstOrDefault();
                                    if (check != null && fenxiang.selected.ToLower() != "false")
                                    {
                                        inspectItem = new Epm_InspectItem();
                                        inspectItem.InspectId = inspect.Id;
                                        inspectItem.CheckId = check.Id;
                                        inspectItem.CheckName = check.Remark;
                                        inspectItem.CheckParentId = check.ParentId;
                                        inspectItem.CheckParentName = check.ParentName;
                                        inspectItem.Level = check.Level;
                                        inspectItem.Choice = item.selected.ToLower() == "false" ? false : true;
                                        inspectItem.RectifRecordPersonKey = item.addRectification == null ? "" : item.addRectification[0].id;
                                        inspectItem.RectifRecordPerson = item.addRectification == null ? "" : item.addRectification[0].name;
                                        inspectItem.ScoreMax = check.ScoreRange.Split(',')[0] == null ? 0 : int.Parse(check.ScoreRange.Split(',')[0]);
                                        inspectItem.Score = item.addScore == null ? 0 : decimal.Parse(item.addScore);
                                        inspectItem.State = item.selected.ToLower() == "false" ? 0 : (int)RectificationState.WaitRectification;
                                        SetCreateUser(inspectItem);
                                        SetCurrentUser(inspectItem);
                                        inspectItemList.Add(inspectItem);

                                        #region 得失分单位、人员
                                        //得失分单位、人员
                                        var companylist = string.IsNullOrEmpty(check.ScoreCompany) ? null : check.ScoreCompany.Split(',').ToList();
                                        var userlist = string.IsNullOrEmpty(check.ScorePerson) ? null : check.ScorePerson.Split(',').ToList();
                                        var scoreCompanyUser = GetGainLoss(companylist, userlist, project, companys);
                                        if (scoreCompanyUser != null)
                                        {
                                            foreach (var cu in scoreCompanyUser)
                                            {
                                                var company = cu.Key.Split(',');
                                                if (string.IsNullOrEmpty(cu.Value))
                                                {
                                                    score = new Epm_InspectScore();
                                                    score.InspectId = inspect.Id;
                                                    score.InspectItemId = inspectItem.Id;
                                                    score.GainLossCompanyId = string.IsNullOrEmpty(company[0]) ? 0 : company[0].ToLongReq();
                                                    score.GainLossCompanyName = string.IsNullOrEmpty(company[1]) ? "" : company[1];
                                                    SetCreateUser(score);
                                                    SetCurrentUser(score);
                                                    scoreList.Add(score);
                                                }
                                                else
                                                {
                                                    var list = cu.Value.Split(';').ToList();
                                                    foreach (var cv in list)
                                                    {
                                                        if (!string.IsNullOrEmpty(cv))
                                                        {
                                                            score = new Epm_InspectScore();
                                                            score.InspectId = inspect.Id;
                                                            score.InspectItemId = inspectItem.Id;
                                                            score.GainLossCompanyId = string.IsNullOrEmpty(company[0]) ? 0 : company[0].ToLongReq();
                                                            score.GainLossCompanyName = string.IsNullOrEmpty(company[1]) ? "" : company[1];
                                                            var user = cv.Split(',');
                                                            score.GainLossUserId = string.IsNullOrEmpty(user[0]) ? 0 : user[0].ToLongReq();
                                                            score.GainLossUserName = string.IsNullOrEmpty(user[1]) ? "" : user[1];
                                                            SetCreateUser(score);
                                                            SetCurrentUser(score);
                                                            scoreList.Add(score);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        #endregion

                                        #region 整改单，整改项3级数据准备
                                        if (!inspect.IsDraft.Value && item.selected.ToLower() != "false")
                                        {
                                            rectifItem3 = new Epm_RectificationItem();
                                            rectifItem3.CheckId = check.Id;
                                            rectifItem3.CheckName = check.Remark;
                                            rectifItem3.CheckParentId = check.ParentId;
                                            rectifItem3.CheckParentName = check.ParentName;
                                            rectifItem3.Level = check.Level;
                                            SetCreateUser(rectifItem3);
                                            SetCurrentUser(rectifItem3);

                                            if (item.addRectification.Any())
                                            {
                                                rectifItemList = new List<Epm_RectificationItem>();
                                                rectifRecordList = new List<Epm_RectificationRecord>();
                                                if (!dic.Keys.Contains(item.addRectification[0].id))
                                                {
                                                    rectifItemList.Add(rectifItem2);
                                                    rectifItemList.Add(rectifItem3);
                                                    dic[item.addRectification[0].id] = rectifItemList;

                                                    rectifRecordList.Add(record);
                                                    recordList[item.addRectification[0].id] = rectifRecordList;
                                                }
                                                else
                                                {
                                                    Epm_RectificationItem item2 = dic[item.addRectification[0].id].Where(t => t.Id == rectifItem2.Id).FirstOrDefault();
                                                    if (item2 != null)
                                                    {
                                                        dic[item.addRectification[0].id].Add(rectifItem3);
                                                    }
                                                    else
                                                    {
                                                        rectifItemList.Add(rectifItem2);
                                                        rectifItemList.Add(rectifItem3);
                                                        dic[item.addRectification[0].id].AddRange(rectifItemList);

                                                        rectifRecordList.Add(record);
                                                        recordList[item.addRectification[0].id].AddRange(rectifRecordList);
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }

                                    #region 非常规作业&复核复查                                    
                                    var work4List = item.children;
                                    if (work4List.Count > 0)
                                    {
                                        if (!item.isChange)
                                        {
                                            #region 非常规作业
                                            foreach (var work4Item in work4List)
                                            {
                                                var work4Check = checkItem.Where(t => t.Id == work4Item.id.ToLongReq()).FirstOrDefault();
                                                if (work4Check != null)
                                                {
                                                    work = new EPM_UnconventionalWork();
                                                    work.Type = check.Remark;
                                                    work.InspectId = inspect.Id;
                                                    work.CheckId = work4Check.Id;
                                                    work.CheckParentId = check.Id;
                                                    work.Name = work4Check.Name;
                                                    work.Level = 4;
                                                    SetCreateUser(work);
                                                    SetCurrentUser(work);
                                                    unWorkList.Add(work);

                                                    //非常规作业五级
                                                    var work5List = work4Item.children;
                                                    foreach (var work5Item in work5List)
                                                    {
                                                        var work5Check = checkItem.Where(t => t.Id == work5Item.id.ToLongReq()).FirstOrDefault();

                                                        work = new EPM_UnconventionalWork();
                                                        work.Type = check.Remark;
                                                        work.InspectId = inspect.Id;
                                                        work.CheckId = work5Check.Id;
                                                        work.CheckParentId = work5Check.ParentId;
                                                        work.Name = work5Check.Remark;
                                                        work.Level = 5;
                                                        work.ScoreRang = work5Check.ScoreRange;
                                                        work.Score = work5Item.addScore == null ? 0 : int.Parse(work5Item.addScore);
                                                        SetCreateUser(work);
                                                        SetCurrentUser(work);
                                                        unWorkList.Add(work);
                                                    }
                                                }
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            #region 复核、复查
                                            var fcItem = fhfcList.Where(p => p.CreateUserId == currentUserId && p.Level == 2 && p.CheckParentName == name1).OrderByDescending(p => p.CreateTime).FirstOrDefault();

                                            var fc2Item = fhfcList.Where(t => t.InspectId == fcItem.InspectId && t.Level == 2 && !"复核,复查".Contains(t.CheckName)).ToList();
                                            foreach (var fc2 in fc2Item)
                                            {
                                                work = new EPM_UnconventionalWork();
                                                work.Type = roleType;
                                                work.InspectId = inspect.Id;
                                                work.CheckId = fc2.CheckId;
                                                work.CheckParentId = check.Id;
                                                work.Name = fc2.CheckName;
                                                work.Level = 4;
                                                SetCreateUser(work);
                                                SetCurrentUser(work);
                                                unWorkList.Add(work);

                                                var fc3Item = fhfcList.Where(t => t.InspectId == fcItem.InspectId && t.CheckParentId == fc2.Id).ToList();
                                                foreach (var fc3 in fc3Item)
                                                {
                                                    work = new EPM_UnconventionalWork();
                                                    work.Type = roleType;
                                                    work.InspectId = inspect.Id;
                                                    work.CheckId = fc3.CheckId;
                                                    work.CheckParentId = fc3.CheckParentId;
                                                    work.Name = fc3.CheckName;
                                                    work.Level = 5;
                                                    work.Score = int.Parse(fc3.Score.ToString());
                                                    SetCreateUser(work);
                                                    SetCurrentUser(work);
                                                    unWorkList.Add(work);
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                }
            }


            //删除草稿数据
            var delInspect = DataOperateBusiness<Epm_Inspect>.Get().GetList(t => t.ProjectId == projectId && t.InspectUserId == currentUserId && t.IsDraft.Value).FirstOrDefault();
            if (delInspect != null)
            {
                var delItems = DataOperateBusiness<Epm_InspectItem>.Get().GetList(t => t.InspectId == delInspect.Id);
                var delScores = DataOperateBusiness<Epm_InspectScore>.Get().GetList(t => t.InspectId == delInspect.Id);
                var unWorks = DataOperateBusiness<EPM_UnconventionalWork>.Get().GetList(t => t.InspectId == delInspect.Id);

                DataOperateBusiness<Epm_Inspect>.Get().Delete(delInspect);
                DataOperateBusiness<Epm_InspectItem>.Get().DeleteRange(delItems);
                DataOperateBusiness<Epm_InspectScore>.Get().DeleteRange(delScores);
                DataOperateBusiness<EPM_UnconventionalWork>.Get().DeleteRange(unWorks);
            }
            ///保存检查单
            DataOperateBusiness<Epm_Inspect>.Get().Add(inspect);
            DataOperateBusiness<Epm_InspectItem>.Get().AddRange(inspectItemList);
            DataOperateBusiness<Epm_InspectScore>.Get().AddRange(scoreList);
            DataOperateBusiness<EPM_UnconventionalWork>.Get().AddRange(unWorkList);

            #region 生成待办
            Epm_Approver appInspect = new Epm_Approver();
            appInspect.Title = CurrentUserName + "已完成本次现场检查";
            appInspect.Content = CurrentUserName + "已完成本次现场检查";
            appInspect.SendUserId = CurrentUser.Id;
            appInspect.SendUserName = CurrentUserName;
            appInspect.SendTime = DateTime.Now;
            appInspect.LinkURL = string.Empty;
            appInspect.BusinessTypeNo = BusinessType.SecurityCheck.ToString();
            appInspect.Action = SystemRight.Add.ToString();
            appInspect.BusinessTypeName = BusinessType.SecurityCheck.GetText();
            appInspect.BusinessState = (int)(RectificationState.WorkFinish);
            appInspect.BusinessId = inspect.Id;
            appInspect.ApproverId = inspect.InspectUserId;
            appInspect.ApproverName = inspect.InspectUserName;
            appInspect.ProjectId = project.Id;
            appInspect.ProjectName = project.Name;
            AddApproverBatch(new List<Epm_Approver>() { appInspect });
            WriteLog(BusinessType.Rectification.GetText(), SystemRight.Add.GetText(), "阶段检查生成检查待办: " + inspect.Id);
            #endregion

            #region 消息
            var waitSend = GetWaitSendMessageList(inspect.ProjectId.Value);
            foreach (var send in waitSend)
            {
                Epm_Massage modelMsg = new Epm_Massage();
                modelMsg.ReadTime = null;
                modelMsg.RecId = send.Key;
                modelMsg.RecName = send.Value;
                modelMsg.RecTime = DateTime.Now;
                modelMsg.SendId = currentUserId;
                modelMsg.SendName = CurrentUserName;
                modelMsg.SendTime = DateTime.Now;
                modelMsg.Title = CurrentUserName + "已完成本次现场检查";
                modelMsg.Content = CurrentUserName + "已完成本次现场检查";
                modelMsg.Type = 2;
                modelMsg.IsRead = false;
                modelMsg.BussinessId = inspect.Id;
                modelMsg.BussinesType = BusinessType.SecurityCheck.ToString();
                modelMsg.ProjectId = inspect.ProjectId;
                modelMsg.ProjectName = inspect.ProjectName;
                modelMsg = base.SetCurrentUser(modelMsg);
                modelMsg = base.SetCreateUser(modelMsg);
                DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
            }
            #endregion
            #endregion

            #region 整改单
            foreach (var key in dic.Keys)
            {
                var rectifUser = GetRectif(key, project, companys);
                if (!string.IsNullOrEmpty(rectifUser))
                {
                    var rectif = new Epm_Rectification();
                    rectif.ProjectId = project.Id;
                    rectif.ProjectName = project.Name;
                    rectif.InsppectId = inspect.Id;
                    rectif.RectificateTitle = "防渗改造项目施工检查表";
                    rectif.InspectAddress = project.Area + project.Address;
                    rectif.InspectDate = time;
                    rectif.InspectUserId = currentUserId;
                    rectif.InspectUserName = CurrentUserName;
                    rectif.RectifRecordUserId = string.IsNullOrEmpty(rectifUser.Split(',')[0]) ? 0 : rectifUser.Split(',')[0].ToLongReq();
                    rectif.RectifRecordUserName = rectifUser.Split(',')[1];
                    rectif.State = (int)RectificationState.WaitRectification;
                    SetCreateUser(rectif);
                    SetCurrentUser(rectif);
                    DataOperateBusiness<Epm_Rectification>.Get().Add(rectif);

                    var rectificateItem = dic[key];
                    foreach (var item in rectificateItem)
                    {
                        item.RectificationId = rectif.Id;
                        item.State = (int)RectificationState.WaitRectification;
                    }
                    DataOperateBusiness<Epm_RectificationItem>.Get().AddRange(rectificateItem);

                    var recordModel = recordList[key];
                    foreach (var item in recordModel)
                    {
                        item.RectficationId = rectif.Id;
                    }
                    DataOperateBusiness<Epm_RectificationRecord>.Get().AddRange(recordModel);

                    #region 生成待办
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "进行现场检查发现问题，请处理";
                    app.Content = CurrentUserName + "进行现场检查发现问题，请处理";
                    app.SendUserId = CurrentUser.Id;
                    app.SendUserName = CurrentUserName;
                    app.SendTime = DateTime.Now;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Rectification.ToString();
                    app.Action = SystemRight.Add.ToString();
                    app.BusinessTypeName = BusinessType.Rectification.GetText();
                    app.BusinessState = (int)(RectificationState.WaitRectification);
                    app.BusinessId = rectif.Id;
                    app.ApproverId = rectif.RectifRecordUserId;
                    app.ApproverName = rectif.RectifRecordUserName;
                    app.ProjectId = project.Id;
                    app.ProjectName = project.Name;
                    AddApproverBatch(new List<Epm_Approver>() { app });
                    WriteLog(BusinessType.Rectification.GetText(), SystemRight.Add.GetText(), "阶段检查生成整改待办: " + rectif.Id);
                    #endregion
                }
            }
            #endregion

            result.Data = true;
            result.Flag = EResultFlag.Success;
            //}
            //catch (Exception ex)
            //{
            //    result.Data = false;
            //    result.Flag = EResultFlag.Failure;
            //    result.Exception = new ExceptionEx(ex, "AddMonitorNew");
            //}
            return result;
        }

        /// <summary>
        /// 根据得失分配置枚举，获取项目得失分单位Id,Name和得失分人员Id,Name;Id,Name
        /// </summary>
        /// <param name="RectifCanpanyEnum"></param>
        /// <param name="RectifPeopleEnum"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        private Dictionary<string, string> GetGainLoss(List<string> RectifCanpanyEnum, List<string> RectifPeopleEnum, Epm_Project project, List<Epm_ProjectCompany> companys)
        {
            if (RectifCanpanyEnum == null || RectifCanpanyEnum.Count == 0)
                return null;

            Epm_ProjectCompany company = null;
            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (var item in RectifCanpanyEnum)
            {
                string companyStr = string.Empty;
                string userStr = string.Empty;

                switch (item)
                {
                    case "FGS": //分公司
                        companyStr = project.CompanyId.Value + "," + project.CompanyName;
                        if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("FGSXMFZR"))
                        {
                            userStr = project.ContactUserId.Value + "," + project.ContactUserName;
                        }
                        dic.Add(companyStr, userStr);
                        break;
                    case "SJDW": //设计
                        company = companys.Where(t => t.Type == "设计费").FirstOrDefault();
                        if (company != null && company.CompanyId.HasValue)
                        {
                            companyStr = company.CompanyId.Value + "," + company.CompanyName;
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("SJFZR"))
                            {
                                userStr = company.LinkManId.Value + "," + company.LinkMan;
                            }
                            dic.Add(companyStr, userStr);
                        }
                        break;
                    case "JLDW": //监理单位
                        company = companys.Where(t => t.Type == "监理").FirstOrDefault();
                        if (company != null && company.CompanyId.HasValue)
                        {
                            companyStr = company.CompanyId.Value + "," + company.CompanyName;
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("ZJ"))
                            {
                                userStr = company.PMId.Value + "," + company.PM + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("XCJL"))
                            {
                                userStr += company.LinkManId.Value + "," + company.LinkMan;
                            }
                            dic.Add(companyStr, userStr);
                        }
                        break;
                    case "TJDW": //土建单位
                        company = companys.Where(t => t.Type == "土建").FirstOrDefault();
                        if (company != null && company.CompanyId.HasValue)
                        {
                            companyStr = company.CompanyId.Value + "," + company.CompanyName;
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("TJZFZR"))
                            {
                                userStr = company.PMId.Value + "," + company.PM + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("TJAQFZR"))
                            {
                                userStr += company.SafeManId.Value + "," + company.SafeMan + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("TJXCFZR"))
                            {
                                try
                                {
                                    userStr += company.LinkManId.Value + "," + company.LinkMan;
                                }
                                catch (Exception ex)
                                { }
                            }
                            dic.Add(companyStr, userStr);
                        }
                        break;
                    case "AZDW": //安装单位
                        company = companys.Where(t => t.Type == "安装").FirstOrDefault();
                        if (company != null && company.CompanyId.HasValue)
                        {
                            companyStr = company.CompanyId.Value + "," + company.CompanyName;
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("AZZFZR"))
                            {
                                userStr = company.PMId.Value + "," + company.PM + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("AZAQFZR"))
                            {
                                userStr += company.SafeManId.Value + "," + company.SafeMan + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("AZXCFZR"))
                            {
                                userStr += company.LinkManId.Value + "," + company.LinkMan;
                            }
                            dic.Add(companyStr, userStr);
                        }
                        break;
                    case "BZDW": //包装单位
                        company = companys.Where(t => t.Type == "包装").FirstOrDefault();
                        if (company != null && company.CompanyId.HasValue)
                        {
                            companyStr = company.CompanyId.Value + "," + company.CompanyName;
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("BZZFZR"))
                            {
                                userStr = company.PMId.Value + "," + company.PM + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("BZAQFZR"))
                            {
                                userStr += company.SafeManId.Value + "," + company.SafeMan + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("BZXCFZR"))
                            {
                                userStr += company.LinkManId.Value + "," + company.LinkMan;
                            }
                            dic.Add(companyStr, userStr);
                        }
                        break;
                    case "JGDW": //加固单位
                        company = companys.Where(t => t.Type == "加固").FirstOrDefault();
                        if (company != null && company.CompanyId.HasValue)
                        {
                            companyStr = company.CompanyId.Value + "," + company.CompanyName;
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("JGZFZR"))
                            {
                                userStr = company.PMId.Value + "," + company.PM + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("JGAQFZR"))
                            {
                                userStr += company.SafeManId.Value + "," + company.SafeMan + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("JGXCFZR"))
                            {
                                userStr += company.LinkManId.Value + "," + company.LinkMan;
                            }
                            dic.Add(companyStr, userStr);
                        }
                        break;
                    case "NCDW": //内衬单位
                        company = companys.Where(t => t.Type == "内衬").FirstOrDefault();
                        if (company != null && company.CompanyId.HasValue)
                        {
                            companyStr = company.CompanyId.Value + "," + company.CompanyName;
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("NCZFZR"))
                            {
                                userStr = company.PMId.Value + "," + company.PM + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("NCAQFZR"))
                            {
                                userStr += company.SafeManId.Value + "," + company.SafeMan + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("NCXCFZR"))
                            {
                                userStr += company.LinkManId.Value + "," + company.LinkMan;
                            }
                            dic.Add(companyStr, userStr);
                        }
                        break;
                    case "QGDW": //油罐清洗单位
                        company = companys.Where(t => t.Type == "油罐清洗费").FirstOrDefault();
                        if (company != null && company.CompanyId.HasValue)
                        {
                            companyStr = company.CompanyId.Value + "," + company.CompanyName;
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("QGZFZR"))
                            {
                                userStr = company.PMId.Value + "," + company.PM + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("QGAQFZR"))
                            {
                                userStr += company.SafeManId.Value + "," + company.SafeMan + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("QGXCFZR"))
                            {
                                userStr += company.LinkManId.Value + "," + company.LinkMan;
                            }
                            dic.Add(companyStr, userStr);
                        }
                        break;
                    case "WJDW": //网架单位
                        company = companys.Where(t => t.Type == "网架").FirstOrDefault();
                        if (company != null && company.CompanyId.HasValue)
                        {
                            companyStr = company.CompanyId.Value + "," + company.CompanyName;
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("WJZFZR"))
                            {
                                userStr = company.PMId.Value + "," + company.PM + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("WJAQFZR"))
                            {
                                userStr += company.SafeManId.Value + "," + company.SafeMan + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("WJXCFZR"))
                            {
                                userStr += company.LinkManId.Value + "," + company.LinkMan;
                            }
                            dic.Add(companyStr, userStr);
                        }
                        break;
                    case "YGDW": //油罐单位
                        company = companys.Where(t => t.Type == "油罐").FirstOrDefault();
                        if (company != null && company.CompanyId.HasValue)
                        {
                            companyStr = company.CompanyId.Value + "," + company.CompanyName;
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("YGZFZR"))
                            {
                                userStr = company.PMId.Value + "," + company.PM + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("YGAQFZR"))
                            {
                                userStr += company.SafeManId.Value + "," + company.SafeMan + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("YGXCFZR"))
                            {
                                userStr += company.LinkManId.Value + "," + company.LinkMan;
                            }
                            dic.Add(companyStr, userStr);
                        }
                        break;
                    case "GXDW": //管线单位
                        company = companys.Where(t => t.Type == "管线").FirstOrDefault();
                        if (company != null && company.CompanyId.HasValue)
                        {
                            companyStr = company.CompanyId.Value + "," + company.CompanyName;
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("GXZFZR"))
                            {
                                userStr = company.PMId.Value + "," + company.PM + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("GXAQFZR"))
                            {
                                userStr += company.SafeManId.Value + "," + company.SafeMan + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("GXXCFZR"))
                            {
                                try
                                {
                                    userStr += company.LinkManId.Value + "," + company.LinkMan;
                                }
                                catch (Exception ex)
                                { }

                            }
                            dic.Add(companyStr, userStr);
                        }
                        break;
                    case "FDJDW": //发电机单位
                        company = companys.Where(t => t.Type == "发电机").FirstOrDefault();
                        if (company != null && company.CompanyId.HasValue)
                        {
                            companyStr = company.CompanyId.Value + "," + company.CompanyName;
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("FDJZFZR"))
                            {
                                userStr = company.PMId.Value + "," + company.PM + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("FDJAQFZR"))
                            {
                                userStr += company.SafeManId.Value + "," + company.SafeMan + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("FDJXCFZR"))
                            {
                                userStr += company.LinkManId.Value + "," + company.LinkMan;
                            }
                            dic.Add(companyStr, userStr);
                        }
                        break;
                    case "YWYDW": //液位仪单位
                        company = companys.Where(t => t.Type == "液位仪").FirstOrDefault();
                        if (company != null && company.CompanyId.HasValue)
                        {
                            companyStr = company.CompanyId.Value + "," + company.CompanyName;
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("YWYZFZR"))
                            {
                                userStr = company.PMId.Value + "," + company.PM + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("YWYAQFZR"))
                            {
                                userStr += company.SafeManId.Value + "," + company.SafeMan + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("YWYXCFZR"))
                            {
                                userStr += company.LinkManId.Value + "," + company.LinkMan;
                            }
                            dic.Add(companyStr, userStr);
                        }
                        break;
                    case "JYJDW": //加油机单位
                        company = companys.Where(t => t.Type == "加油机").FirstOrDefault();
                        if (company != null && company.CompanyId.HasValue)
                        {
                            companyStr = company.CompanyId.Value + "," + company.CompanyName;
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("JYJZFZR"))
                            {
                                userStr = company.PMId.Value + "," + company.PM + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("JYJAQFZR"))
                            {
                                userStr += company.SafeManId.Value + "," + company.SafeMan + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("JYJXCFZR"))
                            {
                                userStr += company.LinkManId.Value + "," + company.LinkMan;
                            }
                            dic.Add(companyStr, userStr);
                        }
                        break;
                    case "PDGDW": //配电柜单位
                        company = companys.Where(t => t.Type == "配低柜").FirstOrDefault();
                        if (company != null && company.CompanyId.HasValue)
                        {
                            companyStr = company.CompanyId.Value + "," + company.CompanyName;
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("PDGZFZR"))
                            {
                                userStr = company.PMId.Value + "," + company.PM + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("PDGAQFZR"))
                            {
                                userStr += company.SafeManId.Value + "," + company.SafeMan + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("PDGXCFZR"))
                            {
                                userStr += company.LinkManId.Value + "," + company.LinkMan;
                            }
                            dic.Add(companyStr, userStr);
                        }
                        break;
                    case "CPJDW": //成品井单位
                        company = companys.Where(t => t.Type == "成品井").FirstOrDefault();
                        if (company != null && company.CompanyId.HasValue)
                        {
                            companyStr = company.CompanyId.Value + "," + company.CompanyName;
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("CPJZFZR"))
                            {
                                userStr = company.PMId.Value + "," + company.PM + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("CPJAQFZR"))
                            {
                                userStr += company.SafeManId.Value + "," + company.SafeMan + ";";
                            }
                            if (RectifPeopleEnum != null && RectifPeopleEnum.Contains("CPJXCFZR"))
                            {
                                userStr += company.LinkManId.Value + "," + company.LinkMan;
                            }
                            dic.Add(companyStr, userStr);
                        }
                        break;
                    case "ZJL": //站经理
                        if (project.ProjectSubjectId.HasValue)
                        {
                            companyStr = project.ProjectSubjectId.Value + "," + project.ProjectSubjectName;
                            if (project.StationManagerId.HasValue)
                            {
                                userStr = project.StationManagerId.Value + "," + project.StationManagerName;
                            }
                            dic.Add(companyStr, userStr);
                        }
                        break;
                }
            }

            return dic;
        }

        /// <summary>
        /// 根据整改负责人枚举，获取项目整改负责人Id,Name
        /// </summary>
        /// <param name="RectifPeopleEnum"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        private string GetRectif(string RectifPeopleEnum, Epm_Project project, List<Epm_ProjectCompany> companys)
        {
            if (string.IsNullOrEmpty(RectifPeopleEnum))
                return string.Empty;

            Epm_ProjectCompany company = null;
            switch (RectifPeopleEnum)
            {
                case "FGSXMFZR": //分公司现场负责人
                    return project.ContactUserId.Value + "," + project.ContactUserName;

                case "SJFZR": //设计负责人
                    company = companys.Where(t => t.Type == "设计费").FirstOrDefault();
                    if (company != null)
                    {
                        return company.LinkManId.Value + "," + company.LinkMan;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "ZJ": //现场监理
                    company = companys.Where(t => t.Type == "监理").FirstOrDefault();
                    if (company != null)
                    {
                        return company.PMId.Value + "," + company.PM;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "XCJL": //监理单位
                    company = companys.Where(t => t.Type == "监理").FirstOrDefault();
                    if (company != null)
                    {
                        return company.LinkManId.Value + "," + company.LinkMan;
                    }
                    else
                    {
                        return string.Empty;
                    }

                case "TJZFZR": //总负责人
                    company = companys.Where(t => t.Type == "土建").FirstOrDefault();
                    if (company != null)
                    {
                        return company.PMId.Value + "," + company.PM;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "TJAQFZR": //安全员
                    company = companys.Where(t => t.Type == "土建").FirstOrDefault();
                    if (company != null)
                    {
                        return company.SafeManId.Value + "," + company.SafeMan;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "TJXCFZR": //现场负责人
                    company = companys.Where(t => t.Type == "土建").FirstOrDefault();
                    if (company != null)
                    {
                        return company.LinkManId.Value + "," + company.LinkMan;
                    }
                    else
                    {
                        return string.Empty;
                    }

                case "AZZFZR": //总负责人
                    company = companys.Where(t => t.Type == "安装").FirstOrDefault();
                    if (company != null)
                    {
                        return company.PMId.Value + "," + company.PM;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "AZAQFZR": //安全员
                    company = companys.Where(t => t.Type == "安装").FirstOrDefault();
                    if (company != null)
                    {
                        return company.SafeManId.Value + "," + company.SafeMan;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "AZXCFZR": //现场负责人
                    company = companys.Where(t => t.Type == "安装").FirstOrDefault();
                    if (company != null)
                    {
                        return company.LinkManId.Value + "," + company.LinkMan;
                    }
                    else
                    {
                        return string.Empty;
                    }

                case "BZZFZR": //总负责人
                    company = companys.Where(t => t.Type == "包装").FirstOrDefault();
                    if (company != null)
                    {
                        return company.PMId.Value + "," + company.PM;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "BZAQFZR": //安全员
                    company = companys.Where(t => t.Type == "包装").FirstOrDefault();
                    if (company != null)
                    {
                        return company.SafeManId.Value + "," + company.SafeMan;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "BZXCFZR": //现场负责人
                    company = companys.Where(t => t.Type == "包装").FirstOrDefault();
                    if (company != null)
                    {
                        return company.LinkManId.Value + "," + company.LinkMan;
                    }
                    else
                    {
                        return string.Empty;
                    }

                case "JGZFZR": //总负责人
                    company = companys.Where(t => t.Type == "加固").FirstOrDefault();
                    if (company != null)
                    {
                        return company.PMId.Value + "," + company.PM;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "JGAQFZR": //安全员
                    company = companys.Where(t => t.Type == "加固").FirstOrDefault();
                    if (company != null)
                    {
                        return company.SafeManId.Value + "," + company.SafeMan;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "JGXCFZR": //现场负责人
                    company = companys.Where(t => t.Type == "加固").FirstOrDefault();
                    if (company != null)
                    {
                        return company.LinkManId.Value + "," + company.LinkMan;
                    }
                    else
                    {
                        return string.Empty;
                    }

                case "NCZFZR": //总负责人
                    company = companys.Where(t => t.Type == "内衬").FirstOrDefault();
                    if (company != null)
                    {
                        return company.PMId.Value + "," + company.PM;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "NCAQFZR": //安全员
                    company = companys.Where(t => t.Type == "内衬").FirstOrDefault();
                    if (company != null)
                    {
                        return company.SafeManId.Value + "," + company.SafeMan;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "NCXCFZR": //现场负责人
                    company = companys.Where(t => t.Type == "内衬").FirstOrDefault();
                    if (company != null)
                    {
                        return company.LinkManId.Value + "," + company.LinkMan;
                    }
                    else
                    {
                        return string.Empty;
                    }

                case "QGZFZR": //总负责人
                    company = companys.Where(t => t.Type == "油罐清洗费").FirstOrDefault();
                    if (company != null)
                    {
                        return company.PMId.Value + "," + company.PM;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "QGAQFZR": //安全员
                    company = companys.Where(t => t.Type == "油罐清洗费").FirstOrDefault();
                    if (company != null)
                    {
                        return company.SafeManId.Value + "," + company.SafeMan;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "QGXCFZR": //现场负责人
                    company = companys.Where(t => t.Type == "油罐清洗费").FirstOrDefault();
                    if (company != null)
                    {
                        return company.LinkManId.Value + "," + company.LinkMan;
                    }
                    else
                    {
                        return string.Empty;
                    }

                case "WJZFZR": //总负责人
                    company = companys.Where(t => t.Type == "网架").FirstOrDefault();
                    if (company != null)
                    {
                        return company.PMId.Value + "," + company.PM;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "WJAQFZR": //安全员
                    company = companys.Where(t => t.Type == "网架").FirstOrDefault();
                    if (company != null)
                    {
                        return company.SafeManId.Value + "," + company.SafeMan;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "WJXCFZR": //现场负责人
                    company = companys.Where(t => t.Type == "网架").FirstOrDefault();
                    if (company != null)
                    {
                        return company.LinkManId.Value + "," + company.LinkMan;
                    }
                    else
                    {
                        return string.Empty;
                    }

                case "YGZFZR": //总负责人
                    company = companys.Where(t => t.Type == "油罐").FirstOrDefault();
                    if (company != null)
                    {
                        return company.PMId.Value + "," + company.PM;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "YGAQFZR": //安全员
                    company = companys.Where(t => t.Type == "油罐").FirstOrDefault();
                    if (company != null)
                    {
                        return company.SafeManId.Value + "," + company.SafeMan;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "YGXCFZR": //现场负责人
                    company = companys.Where(t => t.Type == "油罐").FirstOrDefault();
                    if (company != null)
                    {
                        return company.LinkManId.Value + "," + company.LinkMan;
                    }
                    else
                    {
                        return string.Empty;
                    }

                case "GXZFZR": //总负责人
                    company = companys.Where(t => t.Type == "管线").FirstOrDefault();
                    if (company != null)
                    {
                        return company.PMId.Value + "," + company.PM;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "GXAQFZR": //安全员
                    company = companys.Where(t => t.Type == "管线").FirstOrDefault();
                    if (company != null)
                    {
                        return company.SafeManId.Value + "," + company.SafeMan;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "GXXCFZR": //现场负责人
                    company = companys.Where(t => t.Type == "管线").FirstOrDefault();
                    if (company != null)
                    {
                        return company.LinkManId.Value + "," + company.LinkMan;
                    }
                    else
                    {
                        return string.Empty;
                    }

                case "FDJZFZR": //总负责人
                    company = companys.Where(t => t.Type == "发电机").FirstOrDefault();
                    if (company != null)
                    {
                        return company.PMId.Value + "," + company.PM;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "FDJAQFZR": //安全员
                    company = companys.Where(t => t.Type == "发电机").FirstOrDefault();
                    if (company != null)
                    {
                        return company.SafeManId.Value + "," + company.SafeMan;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "FDJXCFZR": //现场负责人
                    company = companys.Where(t => t.Type == "发电机").FirstOrDefault();
                    if (company != null)
                    {
                        return company.LinkManId.Value + "," + company.LinkMan;
                    }
                    else
                    {
                        return string.Empty;
                    }

                case "YWYZFZR": //总负责人
                    company = companys.Where(t => t.Type == "液位仪").FirstOrDefault();
                    if (company != null)
                    {
                        return company.PMId.Value + "," + company.PM;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "YWYAQFZR": //安全员
                    company = companys.Where(t => t.Type == "液位仪").FirstOrDefault();
                    if (company != null)
                    {
                        return company.SafeManId.Value + "," + company.SafeMan;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "YWYXCFZR": //现场负责人
                    company = companys.Where(t => t.Type == "液位仪").FirstOrDefault();
                    if (company != null)
                    {
                        return company.LinkManId.Value + "," + company.LinkMan;
                    }
                    else
                    {
                        return string.Empty;
                    }

                case "JYJZFZR": //总负责人
                    company = companys.Where(t => t.Type == "加油机").FirstOrDefault();
                    if (company != null)
                    {
                        return company.PMId.Value + "," + company.PM;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "JYJAQFZR": //安全员
                    company = companys.Where(t => t.Type == "加油机").FirstOrDefault();
                    if (company != null)
                    {
                        return company.SafeManId.Value + "," + company.SafeMan;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "JYJXCFZR": //现场负责人
                    company = companys.Where(t => t.Type == "加油机").FirstOrDefault();
                    if (company != null)
                    {
                        if (company.LinkManId == null && company.LinkMan == null)
                        {
                            return string.Empty;
                        }
                        else
                        {
                            return company.LinkManId.Value + "," + company.LinkMan;
                        }
                    }
                    else
                    {
                        return string.Empty;
                    }

                case "PDGZFZR": //总负责人
                    company = companys.Where(t => t.Type == "配电柜").FirstOrDefault();
                    if (company != null)
                    {
                        return company.PMId.Value + "," + company.PM;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "PDGAQFZR": //安全员
                    company = companys.Where(t => t.Type == "配电柜").FirstOrDefault();
                    if (company != null)
                    {
                        return company.SafeManId.Value + "," + company.SafeMan;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "PDGXCFZR": //现场负责人
                    company = companys.Where(t => t.Type == "配电柜").FirstOrDefault();
                    if (company != null)
                    {
                        return company.LinkManId.Value + "," + company.LinkMan;
                    }
                    else
                    {
                        return string.Empty;
                    }

                case "CPJZFZR": //总负责人
                    company = companys.Where(t => t.Type == "成品井").FirstOrDefault();
                    if (company != null)
                    {
                        return company.PMId.Value + "," + company.PM;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "CPJAQFZR": //安全员
                    company = companys.Where(t => t.Type == "成品井").FirstOrDefault();
                    if (company != null)
                    {
                        return company.SafeManId.Value + "," + company.SafeMan;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "CPJXCFZR": //现场负责人
                    company = companys.Where(t => t.Type == "成品井").FirstOrDefault();
                    if (company != null)
                    {
                        return company.LinkManId.Value + "," + company.LinkMan;
                    }
                    else
                    {
                        return string.Empty;
                    }
                case "ZJL": //站经理                    
                    if (project.StationManagerId.HasValue)
                    {
                        return project.StationManagerId.Value + "," + project.StationManagerName;
                    }
                    else
                    {
                        return string.Empty;
                    }
            }

            return string.Empty;
        }


        /// <summary>
        /// 获取整单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_Inspect> GetInspectModel(long id)
        {
            Result<Epm_Inspect> result = new Result<Epm_Inspect>();
            try
            {
                result.Data = DataOperateBusiness<Epm_Inspect>.Get().GetModel(id);
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMonitorModel");
            }
            return result;
        }

        /// <summary>
        /// 获取检查单非常规作业
        /// </summary>
        /// <param name="inspectId"></param>
        /// <returns></returns>
        public Result<List<EPM_UnconventionalWork>> GetIUnconventionalWorkList(long inspectId)
        {
            Result<List<EPM_UnconventionalWork>> result = new Result<List<EPM_UnconventionalWork>>();
            try
            {
                result.Data = DataOperateBusiness<EPM_UnconventionalWork>.Get().GetList(t => t.InspectId == inspectId).ToList();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetIUnconventionalWorkList");
            }
            return result;
        }

        /// <summary>
        /// 获取非常规作业和复查、复核列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<EPM_UnconventionalWork>> GetWorkList(QueryCondition qc)
        {
            Result<List<EPM_UnconventionalWork>> result = new Result<List<EPM_UnconventionalWork>>();
            try
            {
                result = DataOperate.QueryListSimple<EPM_UnconventionalWork>(context, qc);

                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetIUnconventionalWorkList");
            }
            return result;
        }

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_InspectItem>> GetInspectItemList(long inspectId)
        {
            Result<List<Epm_InspectItem>> result = new Result<List<Epm_InspectItem>>();
            try
            {
                result.Data = DataOperateBusiness<Epm_InspectItem>.Get().GetList(t => t.InspectId == inspectId).ToList();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMonitorList");
            }
            return result;
        }
        public Result<List<Epm_InspectItem>> GetInspectItemByProjectId(long projectId)
        {
            Result<List<Epm_InspectItem>> result = new Result<List<Epm_InspectItem>>();
            try
            {
                var ids = DataOperateBusiness<Epm_Inspect>.Get().GetList(t => t.ProjectId == projectId && !t.IsDraft.Value).Select(p => p.Id).Distinct().JoinToString(",");
                result.Data = DataOperateBusiness<Epm_InspectItem>.Get().GetList(t => ids.Contains(t.InspectId.ToString())).ToList();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetInspectItemByProjectId");
            }
            return result;
        }
        public Result<List<Epm_CheckItem>> GetgetCheckItemInfoByProjectId(long ParentId)
        {
            Result<List<Epm_CheckItem>> result = new Result<List<Epm_CheckItem>>();
            try
            {
                result.Data = DataOperateBusiness<Epm_CheckItem>.Get().GetList(t => t.Id == ParentId).ToList();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetgetCheckItemInfoByProjectId");
            }
            return result;
        }
        public Result<List<Epm_InspectItem>> GetInspectItemDraft(long projectId)
        {
            Result<List<Epm_InspectItem>> result = new Result<List<Epm_InspectItem>>();
            try
            {
                long createId = CurrentUserID.ToLongReq();
                var draft = DataOperateBusiness<Epm_Inspect>.Get().GetList(t => t.ProjectId == projectId && t.IsDraft.Value && t.InspectUserId == createId).FirstOrDefault();
                result.Data = DataOperateBusiness<Epm_InspectItem>.Get().GetList(t => t.InspectId == draft.Id).ToList();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetInspectItemDraft");
            }
            return result;
        }

        /// <summary>
        /// 获取整单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_Rectification> GetRectificationModel(long id)
        {
            Result<Epm_Rectification> result = new Result<Epm_Rectification>();
            try
            {
                result.Data = DataOperateBusiness<Epm_Rectification>.Get().GetList(t => t.Id == id).OrderByDescending(t => t.OperateTime).FirstOrDefault();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetRectificationModel");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_RectificationItem>> GetRectificationItemList(long rectifId)
        {
            Result<List<Epm_RectificationItem>> result = new Result<List<Epm_RectificationItem>>();
            try
            {
                result.Data = DataOperateBusiness<Epm_RectificationItem>.Get().GetList(t => t.RectificationId == rectifId).ToList();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetRectificationItemList");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_RectificationRecord>> GetRectificationRecordList(long rectifId)
        {
            Result<List<Epm_RectificationRecord>> result = new Result<List<Epm_RectificationRecord>>();
            try
            {
                result.Data = DataOperateBusiness<Epm_RectificationRecord>.Get().GetList(t => t.RectficationId == rectifId).ToList();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetRectificationRecordList");
            }
            return result;
        }

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_RectificationItem>> GetRectificationItemListByProjectId(long projectId)
        {
            Result<List<Epm_RectificationItem>> result = new Result<List<Epm_RectificationItem>>();
            try
            {
                var rectif = DataOperateBusiness<Epm_Rectification>.Get().GetList(t => t.ProjectId == projectId && t.State == (int)RectificationState.WaitRectification).FirstOrDefault();
                if (rectif != null)
                {
                    result.Data = DataOperateBusiness<Epm_RectificationItem>.Get().GetList(t => t.RectificationId == rectif.Id && t.State == (int)RectificationState.WaitRectification).ToList();
                    result.Flag = EResultFlag.Success;
                }
                else
                {
                    result.Data = new List<Epm_RectificationItem>();
                    result.Flag = EResultFlag.Success;
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetRectificationItemListByProjectId");
            }
            return result;
        }

        public Result<List<Epm_RectificationItem>> GetRectificationItemListALLByProjectId(long projectId)
        {
            Result<List<Epm_RectificationItem>> result = new Result<List<Epm_RectificationItem>>();
            try
            {
                var rectif = DataOperateBusiness<Epm_Rectification>.Get().GetList(t => t.ProjectId == projectId && t.State == (int)RectificationState.WaitRectification);
                if (rectif.Any())
                {
                    List<long> ids = rectif.Select(t => t.Id).ToList();
                    var inWhr = ids.JoinToString(",");

                    result.Data = DataOperateBusiness<Epm_RectificationItem>.Get().GetList(t => inWhr.Contains(t.RectificationId.ToString()) && t.State == (int)RectificationState.WaitRectification).ToList();
                    result.Flag = EResultFlag.Success;
                }
                else
                {
                    result.Data = new List<Epm_RectificationItem>();
                    result.Flag = EResultFlag.Success;
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetRectificationItemListALLByProjectId");
            }
            return result;
        }

        /// <summary>
        /// 提交整改结果
        /// </summary>
        /// <param name="id"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public Result<int> AddMonitorRectifRecord(long id, string content, List<Base_Files> files)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rectifItem = DataOperateBusiness<Epm_RectificationItem>.Get().GetModel(id);
                if (rectifItem == null)
                {
                    throw new Exception("所选整改记录不存在！");
                }

                //更新整改项状态（2级）
                rectifItem.State = (int)RectificationState.Rectificationed;
                rectifItem.Remark = content;
                SetCurrentUser(rectifItem);
                var rows = DataOperateBusiness<Epm_RectificationItem>.Get().Update(rectifItem);

                //更新整改项状态（3级）
                var rectifItem3 = DataOperateBusiness<Epm_RectificationItem>.Get().GetList(t => t.RectificationId == rectifItem.RectificationId && t.CheckParentId == rectifItem.CheckId).ToList();
                foreach (var item in rectifItem3)
                {
                    item.State = (int)RectificationState.Rectificationed;
                    item.OperateUserId = CurrentUserID.ToLongReq();
                    item.OperateUserName = CurrentUserName;
                    item.OperateTime = DateTime.Now;
                }
                DataOperateBusiness<Epm_RectificationItem>.Get().UpdateRange(rectifItem3);

                //添加整改记录 
                Epm_RectificationRecord record = new Epm_RectificationRecord();
                record.RectficationId = rectifItem.RectificationId;
                record.RectficationItemId = rectifItem.Id;
                record.RectficationDescption = content;
                record.State = (int)RectificationState.Rectificationed;
                SetCreateUser(record);
                SetCurrentUser(record);
                DataOperateBusiness<Epm_RectificationRecord>.Get().Add(record);

                //保存整改附件
                if (files != null && files.Count > 0)
                {
                    AddFilesByTable(rectifItem, files);
                }

                //获取整改单
                var rectif = DataOperateBusiness<Epm_Rectification>.Get().GetList(t => t.Id == rectifItem.RectificationId).FirstOrDefault();

                #region 处理待办
                var waitState = (int)RectificationState.WaitRectification;
                var rectifItemAll = DataOperateBusiness<Epm_RectificationItem>.Get().GetList(t => t.RectificationId == rectif.Id).ToList();
                var waitItem = rectifItemAll.Where(t => t.State == waitState).FirstOrDefault();
                if (waitItem == null)
                {
                    var currentUserId = CurrentUserID.ToLongReq();
                    var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == rectif.Id && t.ApproverId == currentUserId && t.IsApprover == false).FirstOrDefault();
                    if (tempApp != null)
                    {
                        ComplateApprover(tempApp.Id);
                    }
                }
                #endregion

                #region 生成待办
                //获取监理
                var companys = DataOperateBusiness<Epm_ProjectCompany>.Get().GetList(t => t.ProjectId == rectif.ProjectId.Value).ToList();
                var company = companys.Where(t => t.IsSupervisor == 1 || t.Type == "监理").FirstOrDefault();
                var approver = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == rectif.Id && t.ApproverId == company.LinkManId && t.IsApprover == false).FirstOrDefault();
                if (approver == null)
                {
                    Epm_Approver app = new Epm_Approver();
                    app.Title = CurrentUserName + "上传了整改结果，待确认";
                    app.Content = CurrentUserName + "上传了整改结果，待确认";
                    app.SendUserId = CurrentUserID.ToLongReq();
                    app.SendUserName = CurrentUserName;
                    app.SendTime = rectif.CreateTime;
                    app.LinkURL = string.Empty;
                    app.BusinessTypeNo = BusinessType.Rectification.ToString();
                    app.Action = SystemRight.Rectif.ToString();
                    app.BusinessTypeName = BusinessType.Rectification.GetText();
                    app.BusinessState = (int)(RectificationState.Rectificationed);
                    app.BusinessId = rectif.Id;
                    app.ApproverId = company.LinkManId;
                    app.ApproverName = company.LinkMan;
                    app.ProjectId = rectif.ProjectId;
                    app.ProjectName = rectif.ProjectName;
                    AddApproverBatch(new List<Epm_Approver>() { app });
                    WriteLog(BusinessType.Rectification.GetText(), SystemRight.Add.GetText(), "提交整改结果生成待办: " + rectif.Id);
                }
                #endregion

                #region 消息
                var waitSend = GetWaitSendMessageList(rectif.ProjectId.Value);
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
                    modelMsg.Title = CurrentUserName + "上传了整改结果，待确认";
                    modelMsg.Content = CurrentUserName + "上传了整改结果，待确认";
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = rectif.Id;
                    modelMsg.BussinesType = BusinessType.Rectification.ToString();
                    modelMsg.ProjectId = rectif.ProjectId;
                    modelMsg.ProjectName = rectif.ProjectName;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                }
                #endregion

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.MonitorRectifRecord.GetText(), SystemRight.Add.GetText(), "提交整改记录: " + rectifItem.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddMonitorRectifRecord");
            }
            return result;
        }


        /// <summary>
        /// 整改单审核、驳回
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeMonitorState(long rectifItemId, RectificationState state, string Remark)
        {
            Result<int> result = new Result<int>();
            try
            {
                //更新整改项状态（2级）
                var rectifItem = DataOperateBusiness<Epm_RectificationItem>.Get().GetModel(rectifItemId);
                rectifItem.State = (int)state;
                SetCurrentUser(rectifItem);
                var rows = DataOperateBusiness<Epm_RectificationItem>.Get().Update(rectifItem);

                //更新整改项状态（3级）
                var rectifItem3 = DataOperateBusiness<Epm_RectificationItem>.Get().GetList(t => t.RectificationId == rectifItem.RectificationId && t.CheckParentId == rectifItem.CheckId).ToList();
                foreach (var item in rectifItem3)
                {
                    item.State = (int)state;
                    item.OperateUserId = CurrentUserID.ToLongReq();
                    item.OperateUserName = CurrentUserName;
                    item.OperateTime = DateTime.Now;
                }
                DataOperateBusiness<Epm_RectificationItem>.Get().UpdateRange(rectifItem3);

                //添加整改记录 
                Epm_RectificationRecord record = new Epm_RectificationRecord();
                record.RectficationId = rectifItem.RectificationId;
                record.RectficationItemId = rectifItem.Id;
                record.RectficationDescption = Remark;
                record.State = (int)state;
                SetCreateUser(record);
                SetCurrentUser(record);
                DataOperateBusiness<Epm_RectificationRecord>.Get().Add(record);

                //获取整改单
                var rectif = DataOperateBusiness<Epm_Rectification>.Get().GetList(t => t.Id == rectifItem.RectificationId).FirstOrDefault();
                var successState = (int)RectificationState.RectificationSuccess;
                var rectifItemAll = DataOperateBusiness<Epm_RectificationItem>.Get().GetList(t => t.RectificationId == rectif.Id).ToList();
                var successItem = rectifItemAll.Where(t => t.State == successState).ToList();
                Epm_Rectification newrectif = null;
                if (successItem.Count == rectifItemAll.Count)
                {
                    newrectif = new Epm_Rectification();
                    newrectif.ProjectId = rectif.ProjectId;
                    newrectif.ProjectName = rectif.ProjectName;
                    newrectif.RectificateTitle = rectif.RectificateTitle;
                    newrectif.InspectUserId = rectif.InspectUserId;
                    newrectif.InspectAddress = rectif.InspectAddress;
                    newrectif.InspectDate = rectif.InspectDate;
                    newrectif.InspectUserId = rectif.InspectUserId;
                    newrectif.InspectUserName = rectif.InspectUserName;
                    newrectif.RectifRecordUserId = rectif.RectifRecordUserId;
                    newrectif.RectifRecordUserName = rectif.RectifRecordUserName;
                    newrectif.State = (int)RectificationState.WorkFinish;
                    SetCreateUser(newrectif);
                    SetCurrentUser(newrectif);
                    DataOperateBusiness<Epm_Rectification>.Get().Add(newrectif);

                    if (rectifItemAll.Count > 0)
                    {
                        foreach (var temp in rectifItemAll)
                        {
                            temp.RectificationId = newrectif.Id;
                        }
                        DataOperateBusiness<Epm_RectificationItem>.Get().UpdateRange(rectifItemAll);
                    }
                    var rectifRecordList = DataOperateBusiness<Epm_RectificationRecord>.Get().GetList(t => t.RectficationId == rectif.Id).ToList();

                    if (rectifRecordList.Count > 0)
                    {
                        foreach (var temp in rectifRecordList)
                        {
                            temp.RectficationId = newrectif.Id;
                        }
                        DataOperateBusiness<Epm_RectificationRecord>.Get().UpdateRange(rectifRecordList);
                    }
                }

                #region 处理待办
                var currentUserId = CurrentUserID.ToLongReq();
                var tempApp = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == rectif.Id && t.ApproverId == currentUserId && t.IsApprover == false).FirstOrDefault();
                if (tempApp != null)
                {
                    ComplateApprover(tempApp.Id);
                }
                #endregion

                #region 生成待办  
                string title = string.Empty;
                if (state == RectificationState.RectificationSuccess)
                {
                    title = CurrentUserName + "上传的整改结果已审核通过";
                }
                else if (state == RectificationState.RectificationOk)
                {
                    title = CurrentUserName + "上传的整改结果已被驳回，请处理";
                    newrectif = rectif;
                }

                //判断整改单位是否存在待办
                var approver = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == rectif.Id && t.ApproverId == rectif.RectifRecordUserId && t.IsApprover == false).FirstOrDefault();
                if (approver != null)
                {
                    ComplateApprover(approver.Id);
                }

                Epm_Approver app = new Epm_Approver();
                app.Title = title;
                app.Content = title;
                app.SendUserId = CurrentUserID.ToLongReq();
                app.SendUserName = CurrentUserName;
                app.SendTime = newrectif.CreateTime;
                app.LinkURL = string.Empty;
                app.BusinessTypeNo = BusinessType.Rectification.ToString();
                app.Action = (state == RectificationState.RectificationSuccess ? SystemRight.AuditRectif.ToString() : SystemRight.RejectRectif.ToString());
                app.BusinessTypeName = BusinessType.Rectification.GetText();
                app.BusinessState = (int)state;
                app.BusinessId = newrectif.Id;
                app.ApproverId = newrectif.RectifRecordUserId;
                app.ApproverName = newrectif.RectifRecordUserName;
                app.ProjectId = newrectif.ProjectId;
                app.ProjectName = newrectif.ProjectName;
                AddApproverBatch(new List<Epm_Approver>() { app });
                WriteLog(BusinessType.Rectification.GetText(), (state == RectificationState.RectificationSuccess ? SystemRight.AuditRectif.GetText() : SystemRight.RejectRectif.GetText()), "审批or驳回整改单生成待办: " + newrectif.Id);
                #endregion

                #region 消息
                var waitSend = GetWaitSendMessageList(rectif.ProjectId.Value);
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
                    modelMsg.Title = title;
                    modelMsg.Content = title;
                    modelMsg.Type = 2;
                    modelMsg.IsRead = false;
                    modelMsg.BussinessId = rectif.Id;
                    modelMsg.BussinesType = BusinessType.Rectification.ToString();
                    modelMsg.ProjectId = rectif.ProjectId;
                    modelMsg.ProjectName = rectif.ProjectName;
                    modelMsg = base.SetCurrentUser(modelMsg);
                    modelMsg = base.SetCreateUser(modelMsg);
                    DataOperateBusiness<Epm_Massage>.Get().Add(modelMsg);
                }
                #endregion

                result.Data = rows;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "ChangeMonitorState");
            }
            return result;
        }
        #endregion


        /// <summary>
        /// 检查列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<InspectView>> GetInspectList(QueryCondition qc)
        {
            Result<List<InspectView>> result = new Result<List<InspectView>>();
            try
            {
                var query = from a in context.Epm_Inspect.Where(p => p.IsDelete == false && !p.IsDraft.Value)
                            join b in context.Epm_InspectItem.Where(p => p.IsDelete == false) on a.Id equals b.InspectId
                            join c in context.Epm_InspectScore.Where(p => p.IsDelete == false) on b.Id equals c.InspectItemId
                            select new
                            {
                                c.Id,
                                c.InspectItemId,
                                c.GainLossUserId,
                                c.GainLossUserName,
                                c.GainLossCompanyId,
                                c.GainLossCompanyName,

                                a.ProjectId,
                                a.ProjectName,
                                a.InspectDate,
                                a.InspectUserId,
                                a.InspectUserName,
                                a.InspectName,

                                b.InspectId,
                                b.ScoreMax,
                                b.Score
                            };
                string projectName = "";
                string gainLossCompanyName = "";
                string gainLossUserName = "";
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
                                        projectName = value;
                                        break;
                                    }
                                case "GainLossCompanyName":
                                    {
                                        gainLossCompanyName = value;
                                        break;
                                    }
                                case "GainLossUserName":
                                    {
                                        gainLossUserName = value;
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }
                query = query.Where(t => (t.ProjectName.Contains(projectName) || projectName == "")
                                      && (t.GainLossCompanyName.Contains(gainLossCompanyName) || gainLossCompanyName == "")
                                      && (t.GainLossUserName.Contains(gainLossUserName) || gainLossUserName == ""));   //修改时间倒序

                var list = query.GroupBy(m => new { m.ProjectName, m.InspectDate, m.InspectId, m.InspectName, m.GainLossUserId, m.GainLossUserName, m.GainLossCompanyName, m.GainLossCompanyId, m.InspectUserId, m.InspectUserName, m.InspectItemId }).Select(m => new InspectView
                {
                    InspectId = m.Key.InspectId,
                    GainLossUserId = m.Key.GainLossUserId,
                    GainLossUserName = m.Key.GainLossUserName,
                    GainLossCompanyId = m.Key.GainLossCompanyId,
                    GainLossCompanyName = m.Key.GainLossCompanyName,
                    ProjectName = m.Key.ProjectName,
                    InspectDate = m.Key.InspectDate,
                    InspectUserId = m.Key.InspectUserId,
                    InspectUserName = m.Key.InspectUserName,
                    InspectName = m.Key.InspectName,
                    ScoreMax = m.Sum(p => p.ScoreMax),
                    Score = m.Sum(p => p.Score),
                    InspectItemId = m.Key.InspectItemId
                }).ToList();

                Dictionary<string, InspectView> dicView = new Dictionary<string, InspectView>();
                InspectView view = null;

                for (int i = 0; i < list.Count; i++)
                {
                    string key = list[i].InspectId.Value.ToString() + ',' + list[i].GainLossCompanyId.Value.ToString();
                    if (!dicView.Keys.Contains(key))
                    {
                        view = new InspectView();
                        view.ProjectName = list[i].ProjectName;
                        view.InspectUserName = list[i].InspectUserName;
                        view.InspectDate = list[i].InspectDate;
                        view.GainLossCompanyName = list[i].GainLossCompanyName;
                        view.GainLossCompanyId = list[i].GainLossCompanyId;
                        view.GainLossUserName = list[i].GainLossUserName;
                        view.ScoreMax = (list[i].ScoreMax ?? 0);
                        view.Score = (list[i].Score ?? 0);
                        view.InspectItemIdStr = list[i].InspectItemId.ToString() + ',';
                        dicView.Add(key, view);
                    }
                    else
                    {
                        dicView[key].ScoreMax += (list[i].ScoreMax ?? 0);
                        dicView[key].Score += (list[i].Score ?? 0);
                        dicView[key].InspectItemIdStr += list[i].InspectItemId.ToString() + ',';
                    }
                }

                List<InspectView> inspectList = new List<InspectView>();
                foreach (var key in dicView.Keys)
                {
                    inspectList.Add(dicView[key]);
                }
                inspectList = inspectList.OrderByDescending(t => t.InspectDate).ToList();
                int count = inspectList.Count();
                int skip = (qc.PageInfo.CurrentPageIndex - 1) * qc.PageInfo.PageRowCount;
                int take = qc.PageInfo.PageRowCount;

                inspectList = inspectList.Skip(skip).Take(take).ToList();
                result.Data = inspectList;
                result.AllRowsCount = count;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "InspectView");
            }
            return result;
        }

        /// <summary>
        /// 得失分明细列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<InspectView>> GetInspectItemListByQc(QueryCondition qc)
        {
            Result<List<InspectView>> result = new Result<List<InspectView>>();
            try
            {
                var query = from b in context.Epm_InspectItem.Where(p => p.IsDelete == false)
                            join c in context.Epm_InspectScore.Where(p => p.IsDelete == false) on b.Id equals c.InspectItemId
                            join d in context.Epm_Rectification.Where(p => p.IsDelete == false) on c.InspectId equals d.InsppectId into temp
                            from tt in temp.DefaultIfEmpty()
                            select new
                            {
                                c.Id,
                                c.CreateUserId,
                                c.CreateUserName,
                                c.CreateTime,
                                c.OperateUserId,
                                c.OperateUserName,
                                c.OperateTime,
                                c.InspectItemId,
                                c.IsDelete,
                                c.GainLossUserId,
                                c.GainLossUserName,
                                c.GainLossCompanyId,
                                c.GainLossCompanyName,
                                b.InspectId,
                                b.ScoreMax,
                                b.Score,
                                b.CheckName,
                                b.Choice,
                                b.CheckId,
                                tt.RectifRecordUserName
                            };
                string inspectItemId = "";
                string gainLossCompanyId = "";
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
                                case "InspectItemId":
                                    {
                                        inspectItemId = value;
                                        break;
                                    }
                                case "GainLossCompanyId":
                                    {
                                        gainLossCompanyId = value;
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }
                query = query.Where(t => (inspectItemId.Contains(t.InspectItemId.ToString()) || inspectItemId == "") && !t.IsDelete && (gainLossCompanyId == "" || t.GainLossCompanyId.ToString() == gainLossCompanyId)).OrderByDescending(p => p.OperateTime);   //修改时间倒序

                var list = query.GroupBy(m => new { m.InspectId, m.GainLossUserId, m.GainLossUserName, m.GainLossCompanyName, m.GainLossCompanyId, m.RectifRecordUserName, m.Choice, m.InspectItemId, m.CheckName, m.CheckId }).Select(m => new InspectView
                {
                    InspectId = m.Key.InspectId,
                    GainLossUserId = m.Key.GainLossUserId,
                    GainLossUserName = m.Key.GainLossUserName,
                    GainLossCompanyId = m.Key.GainLossCompanyId,
                    GainLossCompanyName = m.Key.GainLossCompanyName,
                    InspectItemIdStr = m.Key.RectifRecordUserName,
                    Choice = m.Key.Choice,
                    CheckName = m.Key.CheckName,
                    ScoreMax = m.Sum(p => p.ScoreMax),
                    Score = m.Sum(p => p.Score),
                    InspectItemId = m.Key.InspectItemId,
                    CheckId = m.Key.CheckId
                }).ToList();

                int count = list.Count();
                int skip = (qc.PageInfo.CurrentPageIndex - 1) * qc.PageInfo.PageRowCount;
                int take = qc.PageInfo.PageRowCount;

                list = list.Skip(skip).Take(take).ToList();
                result.Data = list;
                result.AllRowsCount = count;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetInspectItemListByQc");
            }
            return result;
        }
    }
}
