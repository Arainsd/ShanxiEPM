using System;
using System.Collections.Generic;
using System.Linq;

using hc.Plat.Common.Extend;
using System.Web.Mvc;
using hc.epm.DataModel.Basic;
using hc.epm.UI.Common;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using hc.epm.DataModel.Business;
using hc.epm.ViewModel;
//using Microsoft.Office.Interop.MSProject;
using Newtonsoft.Json;
using hc.epm.Common;
using System.Net;
using System.IO.Compression;
using System.IO;
using System.Text;
using System.Configuration;
using hc.Plat.WebAPI.Base.ViewModel;
using hc.epm.Web.Models;

namespace hc.epm.Web.Controllers
{
    public class SupervisorLogStatisticController : BaseWebController
    {
        // GET: SupervisorLogStatistic
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pm">监理</param>
        /// <param name="projectName">项目名称</param>
        /// <param name="companyName">供应商</param>
        /// <param name="crtCompanyName">分公司</param>
        /// <returns></returns>
        public ActionResult Index(string pm, string projectName, string companyName, string crtCompanyName, int pageIndex = 1, int pageSize = 10)
        {
            //获取入参
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID] as string;
            ViewBag.projectName = projectName;
            ViewBag.companyName = companyName;
            ViewBag.crtCompanyName = crtCompanyName;
            ViewBag.pm = pm;
            ViewBag.pageIndex = pageIndex;

            #region 查询条件
            //获取查询条件对象
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = new ConditionExpression();

            //监理名称条件
            if (!string.IsNullOrEmpty(pm))
            {
                ce = new ConditionExpression();
                ce.ExpName = "pm";
                ce.ExpValue = pm;
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            //项目名称条件
            if (!string.IsNullOrEmpty(projectName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "projectName";
                ce.ExpValue = projectName;
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            //供应商名称条件
            if (!string.IsNullOrEmpty(companyName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "companyName";
                ce.ExpValue = companyName;
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            //分公司名称条件
            if (!string.IsNullOrEmpty(crtCompanyName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "crtCompanyName";
                ce.ExpValue = crtCompanyName;
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            #endregion

            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<SupervisorLogStatisticView>> result = new Result<List<SupervisorLogStatisticView>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetSupervisionAttendance(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }


        /// <summary>
        /// 监理考勤统计
        /// </summary>
        [HttpPost]
        public void OutputSupervisorLogStatisticExcel()
        {
            string fileName = "监理考勤统计" + DateTime.Now.ToString("yyyyMMddhhmmss");

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                QueryCondition qc = new QueryCondition();
                qc.PageInfo.isAllowPage = false;

                Result<List<SupervisorLogStatisticView>> result = proxy.GetSupervisionAttendance(qc);
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>()
                    {
                        {"pm" , "姓名"},
                        {"companyName" , "所属供应商"},
                        {"projectName","所属项目" },
                        {"crtCompanyName","项目所属分公司" },
                        {"limit","入场天数" },
                        {"ActualDays","考勤情况(实际提交/入场天数)" },
                    };

                    var data = result.Data.Select(p => new
                    {
                        p.pm,
                        p.companyName,
                        p.projectName,
                        p.crtCompanyName,
                        p.limit,
                        ActualDays = p.ActualDays.ToString() + "/" + p.limit.ToString(),
                    });

                    ExcelHelper.ExportExcel(fileName, dic, data.Cast<object>().ToList(), HttpContext);
                }
            }
        }
    }
}