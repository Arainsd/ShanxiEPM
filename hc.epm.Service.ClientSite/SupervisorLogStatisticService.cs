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
        /// <summary>
        /// 查询监理签到统计
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<SupervisorLogStatisticView>> GetSupervisionAttendance(QueryCondition qc)
        {
            Result<List<SupervisorLogStatisticView>> result = new Result<List<SupervisorLogStatisticView>>();
            try
            {
                var queryPm = from a in context.Epm_Project.Where(p => p.IsDelete == false && (p.State == (int)ProjectState.Construction || p.State == (int)ProjectState.Success))
                              join c in context.Epm_SupervisorLog.Where(p => p.IsDelete == false) on a.Id equals c.ProjectId into temp
                              from tt in temp.DefaultIfEmpty()
                              join b in context.Epm_ProjectCompany.Where(p => p.IsDelete == false && p.IsSupervisor == 1 && !string.IsNullOrEmpty(p.PM) && p.PM != "异常缺位" && p.PM != "正常缺位") on a.Id equals b.ProjectId
                              select new
                              {
                                  tt.SubmitTime,
                                  b.Type,
                                  b.PM,
                                  a.Name,
                                  a.Id,
                                  b.CompanyName,
                                  b.CrtCompanyName,
                                  tt.CreateTime
                              };

                var queryLinkMan = from a in context.Epm_Project.Where(p => p.IsDelete == false && (p.State == (int)ProjectState.Construction || p.State == (int)ProjectState.Success))
                                   join c in context.Epm_SupervisorLog.Where(p => p.IsDelete == false) on a.Id equals c.ProjectId into temp
                                   from tt in temp.DefaultIfEmpty()
                                   join b in context.Epm_ProjectCompany.Where(p => p.IsDelete == false && p.IsSupervisor == 1 && !string.IsNullOrEmpty(p.LinkMan) && p.LinkMan != "异常缺位" && p.LinkMan != "正常缺位") on a.Id equals b.ProjectId
                                   select new
                                   {
                                       tt.SubmitTime,
                                       b.Type,
                                       b.LinkMan,
                                       a.Name,
                                       a.Id,
                                       b.CompanyName,
                                       b.CrtCompanyName,
                                       tt.CreateTime
                                   };
                string pm = "";
                string projectName = "";
                string companyName = "";
                string crtCompanyName = "";
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
                                case "pm":
                                    {
                                        pm = value;
                                        break;
                                    }

                                case "projectName":
                                    {
                                        projectName = value;
                                        break;
                                    }
                                case "companyName":
                                    {
                                        companyName = value;
                                        break;
                                    }
                                case "crtCompanyName":
                                    {
                                        crtCompanyName = value;
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }
                queryPm = queryPm.Where(t => ((t.PM.Contains(pm) || string.IsNullOrEmpty(pm))
                && (t.CompanyName.Contains(companyName) || string.IsNullOrEmpty(companyName))
                && (t.Name.Contains(projectName) || string.IsNullOrEmpty(projectName))
                && (t.CrtCompanyName.Contains(crtCompanyName) || string.IsNullOrEmpty(crtCompanyName))
                )).OrderByDescending(t => t.CreateTime);

                queryLinkMan = queryLinkMan.Where(t => ((string.IsNullOrEmpty(pm) || t.LinkMan.Contains(pm))
                && (t.CompanyName.Contains(companyName) || string.IsNullOrEmpty(companyName))
                && (t.Name.Contains(projectName) || string.IsNullOrEmpty(projectName))
                && (t.CrtCompanyName.Contains(crtCompanyName) || string.IsNullOrEmpty(crtCompanyName))
                )).OrderByDescending(t => t.CreateTime);


                //var list = query.ToList();
                var listPm = queryPm.GroupBy(m => new { m.Name, m.PM, m.CompanyName, m.CrtCompanyName, m.Id }).Select(m => new SupervisorLogStatisticView
                {
                    pm = m.Key.PM,
                    projectName = m.Key.Name,
                    companyName = m.Key.CompanyName,
                    crtCompanyName = m.Key.CrtCompanyName,
                    projectId = m.Key.Id
                }).ToList();

                var listLinkMan = queryLinkMan.GroupBy(m => new { m.Name, m.LinkMan, m.CompanyName, m.CrtCompanyName, m.Id }).Select(m => new SupervisorLogStatisticView
                {
                    pm = m.Key.LinkMan,
                    projectName = m.Key.Name,
                    companyName = m.Key.CompanyName,
                    crtCompanyName = m.Key.CrtCompanyName,
                    projectId = m.Key.Id
                }).ToList();


                List<SupervisorLogStatisticView> dicView = new List<SupervisorLogStatisticView>();
                SupervisorLogStatisticView supervisorLogStatisticView = new SupervisorLogStatisticView();

                for (int i = 0; i < listPm.Count; i++)
                {
                    SupervisorLogStatisticView view = new SupervisorLogStatisticView();
                    view.pm = listPm[i].pm;
                    view.projectName = listPm[i].projectName;
                    view.companyName = listPm[i].companyName;
                    view.crtCompanyName = listPm[i].crtCompanyName;
                    view.limit = getLimit(listPm[i].projectId);
                    view.ActualDays = getActualDays(listPm[i].projectId, listPm[i].pm);
                    //view.submitTime = list[i].SubmitTime.ToString();
                    //view.type = list[i].Type;
                    dicView.Add(view);
                }

                for (int i = 0; i < listLinkMan.Count; i++)
                {
                    SupervisorLogStatisticView view = new SupervisorLogStatisticView();
                    view.pm = listLinkMan[i].pm;
                    view.projectName = listLinkMan[i].projectName;
                    view.companyName = listLinkMan[i].companyName;
                    view.crtCompanyName = listLinkMan[i].crtCompanyName;
                    view.limit = getLimit(listLinkMan[i].projectId);
                    view.ActualDays = getActualDays(listLinkMan[i].projectId, listLinkMan[i].pm);
                    //view.submitTime = list[i].SubmitTime.ToString();
                    //view.type = list[i].Type;
                    dicView.Add(view);
                }

                var listall = dicView.GroupBy(m => new { m.projectId, m.projectName, m.companyName, m.crtCompanyName, m.pm, m.limit, m.ActualDays }).Select(m => new SupervisorLogStatisticView
                {
                    pm = m.Key.pm,
                    projectName = m.Key.projectName,
                    companyName = m.Key.companyName,
                    crtCompanyName = m.Key.crtCompanyName,
                    projectId = m.Key.projectId,
                    limit = m.Key.limit,
                    ActualDays = m.Key.ActualDays
                }).ToList();

                
                int count = listall.Count();
                int skip = (qc.PageInfo.CurrentPageIndex - 1) * qc.PageInfo.PageRowCount;
                int take = qc.PageInfo.PageRowCount;

                listall = listall.Skip(skip).Take(take).ToList();
                result.Data = listall;
                result.AllRowsCount = count;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "SupervisorLogStatisticView");
            }
            return result;
        }

        public string getLimit(long projectId)
        {
            string result = "0";

            var project = DataOperateBusiness<Epm_Project>.Get().GetModel(projectId);
            if (project.PlanWorkStartTime != null)
            {
                DateTime start = Convert.ToDateTime(project.PlanWorkStartTime);
                DateTime end = Convert.ToDateTime(DateTime.Now);
                TimeSpan sp = end.Subtract(start);
                result = (sp.Days + 1).ToString();
            }
            return result;
        }

        public string getActualDays(long projectId, string name)
        {
            string result = "0";
            var list = DataOperateBusiness<Epm_SupervisorLog>.Get().GetList(t => t.ProjectId == projectId && t.CreateUserName == name).ToList();

            if (list.Count() > 0)
            {
                foreach (var item in list)
                {
                    item.SubmitTime = Convert.ToDateTime(item.SubmitTime.ToString("yyyy-MM-dd"));
                }
            }

            var listgroup = list.GroupBy(m => new { m.SubmitTime }).Select(m => new
            {
                m.Key
            }).ToList();
            result = listgroup.Count().ToString();
            return result;
        }

    }
}
