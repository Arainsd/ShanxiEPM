using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    public class ProjectWeeklyController : BaseWebController
    {
        // GET: ProjectWeekly
        /// <summary>
        /// 项目周报列表
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.WeeklyManage, Right = SystemRight.Browse)]
        public ActionResult Index(int pageIndex = 1, int pageSize = 10, string startTime = "", string endTime = "", string name = "")
        {
            Result<List<ProjectWeeklyView>> result = new Result<List<ProjectWeeklyView>>();
            ViewBag.pageIndex = pageIndex;
            ViewBag.name = name;
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            List<ProjectWeeklyView> list = new List<ProjectWeeklyView>();
            //开始时间
            DateTime sTime = Convert.ToDateTime("2019-01-01");
            //当前时间
            DateTime now = DateTime.Now;
            GregorianCalendar gc = new GregorianCalendar();
            int weekOfYear = gc.GetWeekOfYear(now, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

            for (int i = 0; i < weekOfYear; i++)
            {
                ProjectWeeklyView view = new ProjectWeeklyView();
                view.name = "陕西公司加油站项目汇总周报（第" + (i + 1) + "期）";
                DateTime dtWeekStart;
                DateTime dtWeekeEnd;
                GetWeek(sTime.Year, (i + 1), out dtWeekStart, out dtWeekeEnd);
                view.sort = i + 1;
                view.time = dtWeekStart.ToString("yyyy-MM-dd") + "~" + dtWeekeEnd.ToString("yyyy-MM-dd");
                view.startTime = dtWeekStart;
                view.endTime = dtWeekeEnd;
                list.Add(view);
            }
            if (list.Count > 0)
            {
                if (!string.IsNullOrEmpty(startTime))
                {
                    DateTime stime = Convert.ToDateTime(startTime);
                    list = list.Where(t => t.startTime >= stime).ToList();
                }
                if (!string.IsNullOrEmpty(endTime))
                {
                    DateTime etime = Convert.ToDateTime(endTime);
                    list = list.Where(t => t.endTime <= etime).ToList();
                }
                if (!string.IsNullOrEmpty(name))
                {
                    list = list.Where(t => t.name.Contains(name)).ToList();
                }
                result.AllRowsCount = list.Count;
                list = list.OrderByDescending(t => t.sort).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                result.Data = list;
            }
            ViewBag.Total = result.AllRowsCount;
            ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);


            return View(result.Data);
        }

        /// <summary>
        /// 所有项目统计
        /// </summary>
        /// <returns></returns>
        public ActionResult AllProject(int pageIndex = 1, int pageSize = 10, string time = "", string name = "")
        {
            ViewBag.pageIndex = pageIndex;
            string stime = "";
            if (string.IsNullOrEmpty(time))
            {
                stime = DateTime.Now.ToString("yyyy-MM-dd") + "~" + DateTime.Now.ToString("yyyy-MM-dd");
                stime = stime.Split('~')[1];
            }
            else
            {
                stime = time.Split('~')[1];
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                #region 未修改
                ////正在施工
                //var projectSum = proxy.GetProjectSum(1, stime, 4, pageIndex, pageSize);
                ////新增项目汇总表
                //var projectCount2 = proxy.GetProjectCount(2, stime);
                ////改造项目汇总
                //var projectCount3 = proxy.GetProjectCount(3, stime);

                //ViewBag.Total = projectSum.AllRowsCount;
                //ViewBag.TotalPage = Math.Ceiling((decimal)projectSum.AllRowsCount / pageSize);

                //ViewBag.projectCount2 = projectCount2.Data;
                //ViewBag.projectCount3 = projectCount3.Data;


                ////新增项目汇总
                //var projectSum1 = proxy.GetProjectSum(2, stime, 1, pageIndex, pageSize);
                //ViewBag.Total1 = projectSum1.AllRowsCount;
                //ViewBag.TotalPage1 = Math.Ceiling((decimal)projectSum1.AllRowsCount / pageSize);
                //ViewBag.projectSum1 = projectSum1.Data;

                //ViewBag.time = time;
                //ViewBag.name = name;
                //return View(projectSum.Data);
                #endregion

                //正在施工
                var projectSum = proxy.GetProjectSum(1, stime, 4, pageIndex, pageSize);

                //项目新建
                var projectCount2 = proxy.GetProjectCount(2, stime);
                //汇总
                var projectSum3 = proxy.GetProjectCount(1, stime);
                ////所有在建
                //var projectSum5 = proxy.GetProjectSum(2, stime, 5, pageIndex, pageSize);
                //ViewBag.Total6 = projectSum5.AllRowsCount;
                //ViewBag.TotalPage6 = Math.Ceiling((decimal)projectSum5.AllRowsCount / pageSize);
                //ViewBag.projectSum5 = projectSum5.Data;


                ViewBag.Total = projectSum.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)projectSum.AllRowsCount / pageSize);

                ViewBag.projectCount2 = projectCount2.Data;
                ViewBag.projectSum3 = projectSum3.Data;

                //项目汇总详情
                var projectSum1 = proxy.GetProjectSum(1, stime, 0, pageIndex, pageSize);
                ViewBag.Total1 = projectSum1.AllRowsCount;
                ViewBag.TotalPage1 = Math.Ceiling((decimal)projectSum1.AllRowsCount / pageSize);
                ViewBag.projectSum1 = projectSum1.Data;

                ViewBag.time = time;
                ViewBag.name = name;
                return View(projectSum.Data);
            }
        }

        /// <summary>
        /// 新增项目统计
        /// </summary>
        /// <returns></returns>
        public ActionResult AddProject(string time = "", int pageIndex = 1, int pageSize = 10, string name = "")
        {
            ViewBag.pageIndex = pageIndex;
            //ViewBag.pageIndex2 = pageIndex2;
            //ViewBag.pageIndex3 = pageIndex3;
            //ViewBag.pageIndex4 = pageIndex4;

            ViewBag.time = time;
            ViewBag.name = name;
            string stime = "";
            if (string.IsNullOrEmpty(time))
            {
                stime = DateTime.Now.ToString("yyyy-MM-dd") + "~" + DateTime.Now.ToString("yyyy-MM-dd");
                stime = stime.Split('~')[1];
            }
            else
            {
                stime = time.Split('~')[1];
            }
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //新增项目汇总表
                var projectCount = proxy.GetProjectCount(2, stime);

                //新增项目汇总
                var projectSum1 = proxy.GetProjectSum(2, stime, 1, pageIndex, pageSize);
                ViewBag.Total1 = projectSum1.AllRowsCount;
                ViewBag.TotalPage1 = Math.Ceiling((decimal)projectSum1.AllRowsCount / pageSize);
                ViewBag.projectSum1 = projectSum1.Data;

                //未完成设计
                var projectSum2 = proxy.GetProjectSum(2, stime, 2, pageIndex, pageSize);
                ViewBag.Total2 = projectSum2.AllRowsCount;
                ViewBag.TotalPage2 = Math.Ceiling((decimal)projectSum2.AllRowsCount / pageSize);
                ViewBag.projectSum2 = projectSum2.Data;

                //完工未投运
                var projectSum3 = proxy.GetProjectSum(2, stime, 3, pageIndex, pageSize);
                ViewBag.Total3 = projectSum3.AllRowsCount;
                ViewBag.TotalPage3 = Math.Ceiling((decimal)projectSum3.AllRowsCount / pageSize);
                ViewBag.projectSum3 = projectSum3.Data;

                //正在施工
                var projectSum4 = proxy.GetProjectSum(2, stime, 4, pageIndex, pageSize);
                ViewBag.Total4 = projectSum4.AllRowsCount;
                ViewBag.TotalPage4 = Math.Ceiling((decimal)projectSum4.AllRowsCount / pageSize);
                ViewBag.projectSum4 = projectSum4.Data;

                //未完成招标
                var projectSum5 = proxy.GetProjectSum(2, stime, 5, pageIndex, pageSize);
                ViewBag.Total5 = projectSum5.AllRowsCount;
                ViewBag.TotalPage5 = Math.Ceiling((decimal)projectSum5.AllRowsCount / pageSize);
                ViewBag.projectSum5 = projectSum5.Data;

                //在建
                var projectSum6 = proxy.GetProjectSum(2, stime, 6, pageIndex, pageSize);
                ViewBag.Total6 = projectSum6.AllRowsCount;
                ViewBag.TotalPage6 = Math.Ceiling((decimal)projectSum6.AllRowsCount / pageSize);
                ViewBag.projectSum6 = projectSum6.Data;

                return View(projectCount.Data);
            }
        }

        /// <summary>
        /// 改造项目统计
        /// </summary>
        /// <returns></returns>
        public ActionResult ReformProject(string time = "", int pageIndex = 1, int pageSize = 10, string name = "")
        {
            string stime = "";
            if (string.IsNullOrEmpty(time))
            {
                stime = DateTime.Now.ToString("yyyy-MM-dd") + "~" + DateTime.Now.ToString("yyyy-MM-dd");
                stime = stime.Split('~')[1];
            }
            else
            {
                stime = time.Split('~')[1];
            }

            ViewBag.time = time;
            ViewBag.name = name;
            ViewBag.pageIndex = pageIndex;
            //ViewBag.pageIndex2 = pageIndex2;
            //ViewBag.pageIndex3 = pageIndex3;
            //ViewBag.pageIndex4 = pageIndex4;

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //改造项目汇总表
                var projectCount = proxy.GetProjectCount(3, stime);

                //改造项目汇总
                var projectSum1 = proxy.GetProjectSum(3, stime, 5, pageIndex, pageSize);
                ViewBag.Total1 = projectSum1.AllRowsCount;
                ViewBag.TotalPage1 = Math.Ceiling((decimal)projectSum1.AllRowsCount / pageSize);
                ViewBag.projectSum1 = projectSum1.Data;

                //未开工
                var projectSum2 = proxy.GetProjectSum(3, stime, 7, pageIndex, pageSize);
                ViewBag.Total2 = projectSum2.AllRowsCount;
                ViewBag.TotalPage2 = Math.Ceiling((decimal)projectSum2.AllRowsCount / pageSize);
                ViewBag.projectSum2 = projectSum2.Data;

                //完工未投运
                var projectSum3 = proxy.GetProjectSum(3, stime, 3, pageIndex, pageSize);
                ViewBag.Total3 = projectSum3.AllRowsCount;
                ViewBag.TotalPage3 = Math.Ceiling((decimal)projectSum3.AllRowsCount / pageSize);
                ViewBag.projectSum3 = projectSum3.Data;

                //在建
                var projectSum6 = proxy.GetProjectSum(3, stime, 6, pageIndex, pageSize);
                ViewBag.Total6 = projectSum6.AllRowsCount;
                ViewBag.TotalPage6 = Math.Ceiling((decimal)projectSum6.AllRowsCount / pageSize);
                ViewBag.projectSum6 = projectSum6.Data;

                //正在施工
                var projectSum4 = proxy.GetProjectSum(3, stime, 4, pageIndex, pageSize);
                ViewBag.Total4 = projectSum4.AllRowsCount;
                ViewBag.TotalPage4 = Math.Ceiling((decimal)projectSum4.AllRowsCount / pageSize);
                ViewBag.projectSum4 = projectSum4.Data;



                return View(projectCount.Data);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <param name="pageIndex"></param>
        /// <param name="type">1:全部，2：新增，3：改造</param>
        /// <param name="state">1:新增项目汇总，2：未完成设计，3：完工未投运，4：正在施工，5：改造项目汇总</param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ProjectPost(string time = "", int pageIndex = 1, int type = 1, int state = 1, int pageSize = 10)
        {
            string stime = "";
            if (string.IsNullOrEmpty(time))
            {
                stime = DateTime.Now.ToString("yyyy-MM-dd") + "~" + DateTime.Now.ToString("yyyy-MM-dd");
                stime = stime.Split('~')[1];
            }
            else
            {
                stime = time.Split('~')[1];
            }

            ViewBag.time = time;
            ViewBag.pageIndex = pageIndex;

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //改造项目汇总表
                //var projectCount = proxy.GetProjectCount(3, stime);

                //改造项目汇总
                var projectSum = proxy.GetProjectSum(type, stime, state, pageIndex, pageSize);
                ViewBag.Total = projectSum.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)projectSum.AllRowsCount / pageSize);

                return Json(projectSum.Data);
            }
        }

        /// <summary>
        /// 得到一年中的某周的起始日和截止日
        /// 年 nYear
        /// 周数 nNumWeek
        /// 周始 out dtWeekStart
        /// 周终 out dtWeekeEnd
        /// </summary>
        /// <param name="nYear"></param>
        /// <param name="nNumWeek"></param>
        /// <param name="dtWeekStart"></param>
        /// <param name="dtWeekeEnd"></param>
        public static void GetWeek(int nYear, int nNumWeek, out DateTime dtWeekStart, out DateTime dtWeekeEnd)
        {
            DateTime dt = new DateTime(nYear, 1, 1);
            dt = dt + new TimeSpan((nNumWeek - 1) * 7, 0, 0, 0);
            dtWeekStart = dt.AddDays(-(int)dt.DayOfWeek + (int)DayOfWeek.Monday);
            dtWeekeEnd = dt.AddDays((int)DayOfWeek.Saturday - (int)dt.DayOfWeek + 1);
        }
        [HttpPost]
        public ActionResult ExportToExcel(string time = "", string name = "")
        {
            var pathUrl = ConfigurationManager.AppSettings["ImportOrExportPath"];
            var suss = false;
            if (string.IsNullOrEmpty(time))
            {
                return Json(suss);
            }
            WeeklyPathView weeklyPathView = new WeeklyPathView();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var query = proxy.GetProjectCountWeekly((int)ProjectType.All, time);
                var weeklyquery = proxy.GetProjectWeekly((int)ProjectType.All, time, (int)ProjectStateType.NewProjectCount, 1, 10000);
                
                if (query.Data != null && weeklyquery.Data != null)
                {
                    #region 所有项目统计
                    var projectCount2 = query.Data.Where(t => t.Type == (int)ProjectType.NewAdd);//项目新建
                    var projectSum3 = query.Data;//汇总
                    var projectSum1 = weeklyquery.Data;//项目汇总详情
                    #endregion
                    #region 新增项目统计表
                    var projectSum7 = weeklyquery.Data.Where(t => t.Type == (int)ProjectType.NewAdd && t.StateType == (int)ProjectStateType.NewProjectCount);//新增项目汇总
                    var projectSum2 = weeklyquery.Data.Where(t => t.Type == (int)ProjectType.NewAdd && t.StateType == (int)ProjectStateType.UnfinishedDesign);//未完成设计
                    var projectSum8 = weeklyquery.Data.Where(t => t.Type == (int)ProjectType.NewAdd && t.StateType == (int)ProjectStateType.CompletedNotOperational);//完工未投运
                    var projectSum4 = weeklyquery.Data.Where(t => t.Type == (int)ProjectType.NewAdd && t.StateType == (int)ProjectStateType.UnderConstruction);//正在施工
                    var projectSum5 = weeklyquery.Data.Where(t => t.Type == (int)ProjectType.NewAdd && t.StateType == (int)ProjectStateType.RetrofitProjectSummary);//未完成招标
                    var projectSum6 = weeklyquery.Data.Where(t => t.Type == (int)ProjectType.NewAdd && t.StateType == (int)ProjectStateType.Construction);//在建
                    #endregion
                    #region 改造项目统计表
                    var projectCount = query.Data.Where(t => t.Type == (int)ProjectType.Modify);//改造项目汇总表
                    var projectSum11 = weeklyquery.Data.Where(t => t.Type == (int)ProjectType.Modify && t.StateType == (int)ProjectStateType.RetrofitProjectSummary);//改造项目汇总
                    var projectSum12 = weeklyquery.Data.Where(t => t.Type == (int)ProjectType.Modify && t.StateType == (int)ProjectStateType.NotStarted);//未开工
                    var projectSum13 = weeklyquery.Data.Where(t => t.Type == (int)ProjectType.Modify && t.StateType == (int)ProjectStateType.CompletedNotOperational);//完工未投运
                    var projectSum16 = weeklyquery.Data.Where(t => t.Type == (int)ProjectType.Modify && t.StateType == (int)ProjectStateType.Construction);//在建
                    var projectSum14 = weeklyquery.Data.Where(t => t.Type == (int)ProjectType.Modify && t.StateType == (int)ProjectStateType.UnderConstruction); ;//正在施工
                    #endregion
                    WeeklyView weeklyView = new WeeklyView();

                    weeklyView.projectCounts = projectSum3 == null ? new List<Epm_ProjectCountWeekly>(): projectSum3.ToList();
                    weeklyView.projectCounts1 = projectCount2 == null ? new List<Epm_ProjectCountWeekly>() : projectCount2.ToList();
                    weeklyView.projectCounts2 = projectCount2 == null ? new List<Epm_ProjectCountWeekly>() : projectCount2.ToList(); ;
                    weeklyView.projectViews =  projectSum1 == null ? new List<Epm_ProjectWeekly>() : projectSum1.ToList();
                    weeklyView.projectViews2 = projectSum2 == null ? new List<Epm_ProjectWeekly>() : projectSum2.ToList();
                    weeklyView.projectViews4 = projectSum4 == null ? new List<Epm_ProjectWeekly>() : projectSum4.ToList();
                    weeklyView.projectViews5 = projectSum5 == null ? new List<Epm_ProjectWeekly>() : projectSum5.ToList();
                    weeklyView.projectViews7 = projectSum7 == null ? new List<Epm_ProjectWeekly>() : projectSum7.ToList();
                    weeklyView.projectViews8 = projectSum8 == null ? new List<Epm_ProjectWeekly>() : projectSum8.ToList();
                    weeklyView.projectViews11 = projectSum11 == null ? new List<Epm_ProjectWeekly>() : projectSum11.ToList();
                    weeklyView.projectViews12 = projectSum12 == null ? new List<Epm_ProjectWeekly>() : projectSum12.ToList();
                    weeklyView.projectViews13 = projectSum13 == null ? new List<Epm_ProjectWeekly>() : projectSum13.ToList();
                    weeklyView.projectViews14 = projectSum14 == null ? new List<Epm_ProjectWeekly>() : projectSum14.ToList();
                    weeklyView.projectViews16 = projectSum16 == null ? new List<Epm_ProjectWeekly>() : projectSum16.ToList();

                    weeklyView.Title = time + "周报.xls";
                    suss = ExcelHelperNew.ExportForExecl(weeklyView);
                    
                    weeklyPathView.Title = time + "周报.xls";
                    weeklyPathView.Path = pathUrl + weeklyPathView.Title;
                    weeklyPathView.suss = true;
                }
                else
                {
                    weeklyPathView.suss = false;
                }
            }
            return Json(weeklyPathView);
        }
    }
}