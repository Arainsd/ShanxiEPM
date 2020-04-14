using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using hc.Plat.WebAPI.Base.Models;
using hc.Plat.WebAPI.Base.ViewModel;
using hc.Plat.WebAPI.Base.Common;
using hc.epm.ViewModel;
using Newtonsoft.Json;
using hc.Plat.Common.Extend;
using System.Configuration;

namespace hc.Plat.WebAPI.Base.Controllers
{
    public partial class ProjectController
    {
        /// <summary>
        /// 初始化加载节点数据
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object NewAddMonitor(long projectId)
        {
            //判断项目ID是否为空
            if (projectId <= 0)
            {
                return APIResult.GetErrorResult("请选择所属项目！");
            }

            #region 整合变量定义
            var user = CurrentUserView;
            //角色权限定义
            string roleType = string.Empty;
            //监理ID类型
            long jlUserId = 0;
            #endregion

            //获取实体数据表结构checkItemList
            List<Epm_CheckItem> checkItemList = new List<Epm_CheckItem>();
            List<Epm_InspectItem> itemDraftList = new List<Epm_InspectItem>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
            {
                #region RoleType(角色权限控制)
                bool isAgency = proxy.IsAgencyUser(user.UserId);
                if (!isAgency)
                {
                    bool isBranch = proxy.IsBranchCompanyUser(user.UserId);
                    if (!isBranch)
                    {
                        bool isSupervisor = proxy.IsSupervisor(projectId, user.UserId);
                        if (isSupervisor)
                        {
                            roleType = RoleTypeEnum.JL.ToString();
                        }
                        else
                        {
                            roleType = RoleTypeEnum.ZJL.ToString();
                        }
                    }
                    else
                    {
                        roleType = RoleTypeEnum.FGS.ToString();
                        var getProjectCompanyResult = proxy.GetProjectCompanyByProjectId(projectId);
                        if (getProjectCompanyResult.Flag == EResultFlag.Success && getProjectCompanyResult.Data != null)
                        {
                            var company = getProjectCompanyResult.Data.FirstOrDefault(t => t.IsSupervisor == 1);
                            if (company != null)
                            {
                                jlUserId = company.LinkManId.Value;
                            }
                        }
                    }
                }
                else
                {
                    roleType = RoleTypeEnum.SGS.ToString();
                }

                #endregion}
                //获取所有检查项目内容级节点
                checkItemList = proxy.GetCheckItemAll().Data;
                itemDraftList = proxy.GetInspectItemDraft(projectId).Data;
                Epm_Project project = proxy.GetProject(projectId).Data;


                var distinctcheckItem = checkItemList.Where(t => t.RoleType == roleType && t.Level == 3).Select(a => new
                {
                    a.ScoreCompany,
                }).Distinct().ToList();

                var data = new
                {
                    address = project.Name,
                    date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"),
                    checkItems = checkItemList.OrderBy(t => t.Sort).Where(t => t.RoleType == roleType && t.Level == 1).Select(a => new
                    {
                        id = a.Id,
                        name = a.Name,
                        level = 1,
                        selected = false,
                    }),
                    questionType = GetListQue(checkItemList, roleType),
                    //整改单位
                    companies = GetListCom(checkItemList, roleType, projectId),
                    //整改人
                    //rectification = GetListPer(checkItemList, roleType),
                    //分值
                    score = checkItemList.OrderBy(t => t.Sort).Where(t => t.RoleType == roleType && t.Level == 3).FirstOrDefault().ScoreRange.Split(','),
                };
                return APIResult.GetSuccessResult(data);
            }

        }

        /// <summary>
        /// 获取二级节点内容
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="checkidlist"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        [HttpGet]
        [APIAuthorize]
        public object LevelChecklists(long projectId, long checkidlist, int level)
        {
            #region 整合变量定义
            var user = CurrentUserView;
            //角色权限定义
            string roleType = string.Empty;
            //监理ID类型
            long jlUserId = 0;
            #endregion
            //获取实体数据表结构checkItemList
            List<Epm_CheckItem> checkItemList = new List<Epm_CheckItem>();
            List<Epm_InspectItem> itemDraftList = new List<Epm_InspectItem>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
            {
                #region RoleType(角色权限控制)
                bool isAgency = proxy.IsAgencyUser(user.UserId);
                if (!isAgency)
                {
                    bool isBranch = proxy.IsBranchCompanyUser(user.UserId);
                    if (!isBranch)
                    {
                        bool isSupervisor = proxy.IsSupervisor(projectId, user.UserId);
                        if (isSupervisor)
                        {
                            roleType = RoleTypeEnum.JL.ToString();
                        }
                        else
                        {
                            roleType = RoleTypeEnum.ZJL.ToString();
                        }
                    }
                    else
                    {
                        roleType = RoleTypeEnum.FGS.ToString();
                        var getProjectCompanyResult = proxy.GetProjectCompanyByProjectId(projectId);
                        if (getProjectCompanyResult.Flag == EResultFlag.Success && getProjectCompanyResult.Data != null)
                        {
                            var company = getProjectCompanyResult.Data.FirstOrDefault(t => t.IsSupervisor == 1);
                            if (company != null)
                            {
                                jlUserId = company.LinkManId.Value;
                            }
                        }
                    }
                }
                else
                {
                    roleType = RoleTypeEnum.SGS.ToString();
                }

                #endregion}
                //获取所有检查项目内容级节点
                checkItemList = proxy.GetCheckItemAll().Data;
                itemDraftList = proxy.GetInspectItemDraft(projectId).Data;
                Epm_Project project = proxy.GetProject(projectId).Data;
                #region 根据推送级别查询2，3级详细数据
                if (level == 2)
                {
                    var data = new
                    {
                        checkItems = checkItemList.OrderBy(t => t.Sort).Where(t => t.RoleType == roleType && t.Level == level && t.ParentId == checkidlist).Select(b => new
                        {
                            id = b.Id,
                            parentId = b.ParentId,
                            parentName = b.Name,
                            name = b.Name,
                            level = level,
                            selected = false,
                        })
                    };
                    return APIResult.GetSuccessResult(data);
                }
                else
                {
                    var data = new
                    {
                        checkItems = checkItemList.Where(t => t.RoleType == roleType && t.Level == 3 && t.ParentId == checkidlist && !string.IsNullOrEmpty(t.RectificationManName)).Select(c => new
                        {
                            id = c.Id,
                            parentId = c.ParentId,
                            parentName = c.ParentName = (c.ParentName == null) ? "" : c.ParentName,
                            name = c.Remark,
                            level = level,
                            selected = false,
                            questionType = GetListQue(checkItemList, roleType),
                            //整改单位/整改人
                            companies = GetListComs(checkItemList, roleType, projectId, c.Id),
                            //分值
                            score = string.IsNullOrEmpty(c.ScoreRange) ? (new string[1] { "10" }) : c.ScoreRange.Split(','),

                        })
                    };
                    return APIResult.GetSuccessResult(data);
                }
                #endregion

            }
        }
        /// <summary>
        /// 获取责任单位
        /// </summary>
        /// <param name="checkItemList"></param>
        /// <param name="roleType"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        private object GetListCom(List<Epm_CheckItem> checkItemList, string roleType, long projectId)
        {
            //List<Epm_ProjectCompany> companyList = new List<Epm_ProjectCompany>();
            //var user = CurrentUserView;
            //using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
            //{
            //    var companyResult = proxy.GetProjectCompanyList(projectId);
            //    if (companyResult.Flag == EResultFlag.Success && companyResult.Data != null)
            //    {
            //        companyList = companyResult.Data.Where(p => p.ProjectId == projectId && !string.IsNullOrWhiteSpace(p.CompanyName)).ToList();
            //    }
            //}
            if (checkItemList.Any())
            {
                List<CompaniesView> companies = new List<CompaniesView>();
                var distinctcheckItem = checkItemList.Where(t => t.RoleType == roleType && t.Level == 3).Select(a => new
                {
                    a.ScoreCompany,
                    a.RectificationManName
                }).Distinct().ToList();
                #region 责任单位
                if (distinctcheckItem.Any())
                {
                    foreach (var item in distinctcheckItem)
                    {
                        CompaniesView company = new CompaniesView();
                        if (item.ScoreCompany.IndexOf(",") == -1 && item.RectificationManName.IndexOf(",") == -1)
                        {
                            company.id = item.ScoreCompany;
                            company.name = string.IsNullOrEmpty(item.ScoreCompany) ? "" : ((RectificationCompany)Enum.Parse(typeof(RectificationCompany), item.ScoreCompany)).GetText();

                            CompaniesView personnel = new CompaniesView();
                            personnel.id = item.RectificationManName;
                            personnel.name = string.IsNullOrEmpty(item.RectificationManName) ? "" : ((RectificationPeople)Enum.Parse(typeof(RectificationPeople), item.RectificationManName)).GetText();

                            company.personnelList.Add(personnel);
                        }
                        companies.Add(company);
                    }
                }
                #endregion
                return companies;
            }
            return (new List<object>() { }).ToArray();
        }
        /// <summary>
        /// 获取整改人
        /// </summary>
        /// <param name="checkItemList"></param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        private object GetListPer(List<Epm_CheckItem> checkItemList, string roleType)
        {
            if (checkItemList.Any())
            {
                List<Epm_CheckItem> list = new List<Epm_CheckItem>();
                string strCom = "";
                var distinctcheckItem = checkItemList.Where(t => t.RoleType == roleType && t.Level == 3).Select(a => new
                {
                    a.RectificationManName,
                }).Distinct().ToList();

                if (distinctcheckItem.Any())
                {
                    foreach (var item in distinctcheckItem)
                    {
                        if (item.RectificationManName.IndexOf(",") == -1)
                        {
                            if (string.IsNullOrEmpty(strCom) || !strCom.Contains(item.RectificationManName))
                            {
                                Epm_CheckItem check = new Epm_CheckItem();
                                check.RectificationManName = item.RectificationManName;
                                list.Add(check);
                                strCom = strCom + item.RectificationManName;
                            }
                        }
                        else
                        {
                            var str = item.RectificationManName.Split(',');
                            foreach (var temp in str)
                            {
                                if (string.IsNullOrEmpty(strCom) || !strCom.Contains(temp))
                                {
                                    Epm_CheckItem check = new Epm_CheckItem();
                                    check.RectificationManName = temp;
                                    list.Add(check);
                                    strCom = strCom + temp;
                                }
                            }
                        }
                    }
                }
                var data = list.Select(e => new
                {
                    id = e.RectificationManName,
                    name = string.IsNullOrEmpty(e.RectificationManName) ? "" : ((RectificationPeople)Enum.Parse(typeof(RectificationPeople), e.RectificationManName)).GetText()
                });

                return data;
            }
            return (new List<object>() { }).ToArray();
        }
        /// <summary>
        /// 获取问题类型
        /// </summary>
        /// <param name="checkItemList"></param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        private object GetListQue(List<Epm_CheckItem> checkItemList, string roleType)
        {
            if (checkItemList.Any())
            {
                List<Epm_CheckItem> list = new List<Epm_CheckItem>();
                string strCom = "";
                var distinctcheckItem = checkItemList.Where(t => t.RoleType == roleType && t.Level == 3 && t.QuestionType != null).Select(a => new
                {
                    a.QuestionType,
                }).Distinct().ToList();

                if (distinctcheckItem.Any())
                {
                    foreach (var item in distinctcheckItem)
                    {

                        try
                        {
                            if (item.QuestionType.IndexOf(",") == -1)
                            {
                                if (string.IsNullOrEmpty(strCom) || !strCom.Contains(item.QuestionType))
                                {
                                    Epm_CheckItem check = new Epm_CheckItem();
                                    check.QuestionType = item.QuestionType;
                                    list.Add(check);
                                    strCom = strCom + item.QuestionType;
                                }
                            }
                            else
                            {
                                var str = item.QuestionType.Split(',');
                                foreach (var temp in str)
                                {
                                    if (string.IsNullOrEmpty(strCom) || !strCom.Contains(temp))
                                    {
                                        Epm_CheckItem check = new Epm_CheckItem();
                                        check.QuestionType = temp;
                                        list.Add(check);
                                        strCom = strCom + temp;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        { }
                    }
                    var data = list.Select(e => new
                    {
                        questionType = string.IsNullOrEmpty(e.QuestionType) ? (new string[1] { "10" }) : e.QuestionType.Split(','),
                    });
                    return data;
                }
            }
            return (new List<object>() { }).ToArray();
        }

        private object GetListComs(List<Epm_CheckItem> checkItemList, string roleType, long projectId, long id)
        {
            if (checkItemList.Any())
            {
                List<CompaniesView> companies = new List<CompaniesView>();
                var distinctcheckItem = checkItemList.Where(t => t.RoleType == roleType && t.Level == 3 && t.Id == id).Select(a => new
                {
                    a.ScoreCompany,
                    a.RectificationManName
                }).Distinct().ToList();
                #region 责任单位
                if (distinctcheckItem.Any())
                {
                    foreach (var item in distinctcheckItem)
                    {
                        CompaniesView company = new CompaniesView();
                        if (item.ScoreCompany.IndexOf(",") == -1 && item.RectificationManName.IndexOf(",") == -1)
                        {
                            company.id = item.ScoreCompany;
                            company.name = string.IsNullOrEmpty(item.ScoreCompany) ? "" : ((RectificationCompany)Enum.Parse(typeof(RectificationCompany), item.ScoreCompany)).GetText();

                            CompaniesView personnel = new CompaniesView();
                            personnel.id = item.RectificationManName;
                            personnel.name = string.IsNullOrEmpty(item.RectificationManName) ? "" : ((RectificationPeople)Enum.Parse(typeof(RectificationPeople), item.RectificationManName)).GetText();

                            company.personnelList.Add(personnel);
                        }
                        companies.Add(company);
                    }
                }
                #endregion
                return companies;
            }
            return (new List<object>() { }).ToArray();
        }

    }
}