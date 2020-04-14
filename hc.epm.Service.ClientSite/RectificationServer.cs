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
using hc.Plat.WebAPI.Base.ViewModel;

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
        public Result<bool> AddRectificationNew(long projectId, DateTime time, List<checkItemesPer> dataList, int type = 0)
        {
            Result<bool> result = new Result<bool>();
            if (dataList == null && dataList.Count == 0)
            {
                throw new Exception("请填写检查相关内容！");
            }

            ///获取项目信息
            var project = DataOperateBusiness<Epm_Project>.Get().GetModel(projectId);
            ///获取服务商信息
            var companys = DataOperateBusiness<Epm_ProjectCompany>.Get().GetList(t => t.ProjectId == projectId).ToList();
            #region 或者岗位角色roleType
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
            #endregion

            #region 检查单


            ///获取检查配置数据
            var checkItem = DataOperateBusiness<Epm_CheckItem>.Get().GetList(t => t.IsDelete == false).ToList();
            ///复查数据
            List<Epm_InspectItem> fhfcList = GetInspectItemByProjectId(projectId).Data;

            ///检查单对象
            List<Epm_InspectItem> inspectItemList = new List<Epm_InspectItem>();
            List<Epm_CheckItem> EpmCheckItemList = new List<Epm_CheckItem>();
            Epm_CheckItem epmCheckItem = null;
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

            foreach (var item1 in dataList)
            {
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
                foreach (var fenbu in item1.checkItems)
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
                    inspectItem.Choice = true;
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
                                inspectItem.Choice = true;
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
                                    long ids = 0;
                                    var check = new Epm_CheckItem();
                                    //具体要求
                                    if (item.id == "")
                                    {
                                        epmCheckItem = new Epm_CheckItem();
                                        epmCheckItem.ParentId = fx.Id;
                                        epmCheckItem.ParentName = fx.ParentName;
                                        epmCheckItem.Level = 3;
                                        epmCheckItem.Remark = itemlist[0].name;
                                        epmCheckItem.CrtCompanyId = long.Parse("902414008118808576");
                                        epmCheckItem.CrtCompanyName = "管理员企业";
                                        epmCheckItem.IsDelete = false;
                                        epmCheckItem.RectificationManName = dataList[0].companies[0].personnelList[0].name;
                                        epmCheckItem.ScoreRange = itemlist[0].addScore;
                                        epmCheckItem.ScoreCompany = dataList[0].companies[0].id;
                                        epmCheckItem.ScorePerson = dataList[0].companies[0].personnelList[0].id;
                                        SetCreateUser(epmCheckItem);
                                        SetCurrentUser(epmCheckItem);
                                        EpmCheckItemList.Add(epmCheckItem);
                                        DataOperateBusiness<Epm_CheckItem>.Get().AddRange(EpmCheckItemList);

                                        check = epmCheckItem;
                                    }
                                    else
                                    {
                                        check = checkItem.Where(t => t.Id == item.id.ToLongReq()).FirstOrDefault();
                                    }
                                    if (check != null)
                                    {
                                        inspectItem = new Epm_InspectItem();
                                        inspectItem.InspectId = inspect.Id;
                                        inspectItem.CheckId = check.Id;
                                        inspectItem.CheckName = check.Remark;
                                        inspectItem.CheckParentId = check.ParentId;
                                        inspectItem.CheckParentName = check.ParentName;
                                        inspectItem.Level = check.Level;
                                        inspectItem.Choice = true;
                                        inspectItem.RectifRecordPersonKey = check.RectificationManName == null ? "" : check.RectificationManName;
                                        inspectItem.RectifRecordPerson = check.RectificationManName == null ? "" : check.RectificationManName;
                                        inspectItem.ScoreMax = check.ScoreRange.Split(',')[0] == null ? 0 : int.Parse(check.ScoreRange.Split(',')[0]);
                                        //inspectItem.Score = check.ScoreRange == null ? 0 : decimal.Parse(check.ScoreRange);
                                        inspectItem.Score = decimal.Parse(item.addScore);
                                        //inspectItem.State = item1.checkItems[0].selected.ToLower() == "false" ? 0 : (int)RectificationState.WaitRectification;
                                        inspectItem.State = check.State = false ? 0 : (int)RectificationState.WaitRectification;
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
                                        if (!inspect.IsDraft.Value)
                                        {
                                            rectifItem3 = new Epm_RectificationItem();
                                            rectifItem3.CheckId = check.Id;
                                            rectifItem3.CheckName = check.Remark;
                                            rectifItem3.CheckParentId = check.ParentId;
                                            rectifItem3.CheckParentName = check.ParentName;
                                            rectifItem3.Level = check.Level;
                                            SetCreateUser(rectifItem3);
                                            SetCurrentUser(rectifItem3);
                                            if (item1.companies[0].personnelList.Any())
                                            {
                                                rectifItemList = new List<Epm_RectificationItem>();
                                                rectifRecordList = new List<Epm_RectificationRecord>();
                                                var materialLists = new List<checkItemesPer>();
                                                if (!dic.Keys.Contains(item1.companies[0].personnelList[0].id))
                                                {
                                                    rectifItemList.Add(rectifItem2);
                                                    rectifItemList.Add(rectifItem3);
                                                    dic[item1.companies[0].personnelList[0].id] = rectifItemList;

                                                    rectifRecordList.Add(record);
                                                    recordList[item1.companies[0].personnelList[0].id] = rectifRecordList;

                                                }
                                                else
                                                {
                                                    Epm_RectificationItem item2 = dic[item1.companies[0].personnelList[0].id].Where(t => t.Id == rectifItem2.Id).FirstOrDefault();
                                                    if (item2 != null)
                                                    {
                                                        dic[item1.companies[0].personnelList[0].id].Add(rectifItem3);
                                                    }
                                                    else
                                                    {
                                                        rectifItemList.Add(rectifItem2);
                                                        rectifItemList.Add(rectifItem3);
                                                        dic[item1.companies[0].personnelList[0].id].AddRange(rectifItemList);

                                                        rectifRecordList.Add(record);
                                                        recordList[item1.companies[0].personnelList[0].id].AddRange(rectifRecordList);
                                                    }
                                                }

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
                //var delInspect = DataOperateBusiness<Epm_Inspect>.Get().GetList(t => t.ProjectId == projectId && t.InspectUserId == currentUserId && t.IsDraft.Value).FirstOrDefault();
                //if (delInspect != null)
                //{
                //    var delItems = DataOperateBusiness<Epm_InspectItem>.Get().GetList(t => t.InspectId == delInspect.Id);
                //    var delScores = DataOperateBusiness<Epm_InspectScore>.Get().GetList(t => t.InspectId == delInspect.Id);
                //    var unWorks = DataOperateBusiness<EPM_UnconventionalWork>.Get().GetList(t => t.InspectId == delInspect.Id);

                //    DataOperateBusiness<Epm_Inspect>.Get().Delete(delInspect);
                //    DataOperateBusiness<Epm_InspectItem>.Get().DeleteRange(delItems);
                //    DataOperateBusiness<Epm_InspectScore>.Get().DeleteRange(delScores);
                //    DataOperateBusiness<EPM_UnconventionalWork>.Get().DeleteRange(unWorks);
                //}
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

                        if (rectifUser == null)
                        {
                            rectif.RectifRecordUserId = long.Parse("缺失");
                            rectif.RectifRecordUserName = "缺失";
                        }
                        else
                        {
                            rectif.RectifRecordUserId = string.IsNullOrEmpty(rectifUser.Split(',')[0]) ? 0 : rectifUser.Split(',')[0].ToLongReq();
                            rectif.RectifRecordUserName = rectifUser.Split(',')[1];
                        }

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
                        var imagesList = item1.files;

                        if (imagesList != null && imagesList.Count > 0)
                        {

                            foreach (var item in imagesList)
                            {
                                long id = item.id.ToLongReq();
                                Base_Files baseFiles = DataOperateBasic<Base_Files>.Get().GetModel(id);
                                if (item.type == "0")
                                {
                                    baseFiles.IsDelete = false;
                                }
                                else
                                {
                                    baseFiles.IsDelete = true;
                                }
                                List<Base_Files> guidList = DataOperateBasic<Base_Files>.Get().GetList(t => t.GuidId == baseFiles.GuidId).ToList();
                                foreach (var temp in guidList)
                                {
                                    temp.TableName = "Epm_RectificationRecord";
                                    temp.TableId = rectif.Id;
                                    temp.IsDelete = baseFiles.IsDelete;
                                }
                                DataOperateBasic<Base_Files>.Get().UpdateRange(guidList);
                            }


                        }
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
            }
            #endregion


            result.Data = true;
            result.Flag = EResultFlag.Success;
            return result;
        }
        #endregion
    }
}
