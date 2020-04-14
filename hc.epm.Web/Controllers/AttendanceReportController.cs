using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    public class AttendanceReportController : BaseWebController
    {
        // GET: AttendanceReport  考勤报表

        /// <summary>
        /// 分公司列表
        /// </summary>
        /// <returns></returns>
        public ActionResult FilialeIndex(string companyId, string startTime, string endTime, int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.pageIndex = pageIndex;
            Result<List<AttendanceBranchCountView>> result = new Result<List<AttendanceBranchCountView>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                companyId = GetComPanyId();
                var compamyList = proxy.GetAreaCompanyList();

                if (!string.IsNullOrEmpty(companyId))
                {
                    long id = Convert.ToInt64(companyId);
                    compamyList.Data = compamyList.Data.Where(t => t.Id == id).ToList();
                }
                //地市公司
                ViewBag.companyId = compamyList.Data.ToSelectList("Name", "Id", true);


                result = proxy.GetBranchCount(companyId, startTime, endTime, pageIndex, pageSize);

                ViewBag.Total = result.AllRowsCount;
            }
            return View(result.Data);
        }

        /// <summary>
        /// 项目列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ProIndex(string ProName, string companyId, string startTime, string endTime, int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.pageIndex = pageIndex;
            Result<List<AttendanceBranchCountView>> result = new Result<List<AttendanceBranchCountView>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                companyId = GetComPanyId();
                var compamyList = proxy.GetAreaCompanyList();

                if (!string.IsNullOrEmpty(companyId))
                {
                    long id = Convert.ToInt64(companyId);
                    compamyList.Data = compamyList.Data.Where(t => t.Id == id).ToList();
                }
                //地市公司
                ViewBag.companyId = compamyList.Data.ToSelectList("Name", "Id", true);
                companyId = GetComPanyId();
                result = proxy.GetBranchProjectCount(ProName, companyId, startTime, endTime, pageIndex, pageSize);
                ViewBag.Total = result.AllRowsCount;
            }
            return View(result.Data);
        }

        /// <summary>
        /// 考勤列表
        /// </summary>
        /// <returns></returns>
        public ActionResult AttendanceIndex(string ProName, string companyId, string startTime, string endTime, string userName, int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.pageIndex = pageIndex;
            Result<List<AttendanceBranchCountView>> result = new Result<List<AttendanceBranchCountView>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                companyId = GetComPanyId();
                var compamyList = proxy.GetAreaCompanyList();

                if (!string.IsNullOrEmpty(companyId))
                {
                    long id = Convert.ToInt64(companyId);
                    compamyList.Data = compamyList.Data.Where(t => t.Id == id).ToList();
                }
                //地市公司
                ViewBag.companyId = compamyList.Data.ToSelectList("Name", "Id", true);
                companyId = GetComPanyId();
                result = proxy.GetBranchUserCount(ProName, companyId, startTime, endTime, userName, pageIndex, pageSize);
                ViewBag.Total = result.AllRowsCount;
            }
            return View(result.Data);
        }

        /// <summary>
        /// 是省公司
        /// </summary>
        /// <returns></returns>
        public string GetComPanyId()
        {
            string companyId = "";
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var companyInfo = proxy.GetCompanyModel(CurrentUser.CompanyId).Data;
                if (companyInfo != null)
                {
                    //是省公司
                    if (companyInfo.OrgType == "1" || (companyInfo.PId == 10 && companyInfo.OrgType == "3"))
                    {
                        companyId = "";
                    }
                    else if (companyInfo.OrgType == "2" || (companyInfo.PId != 10 && companyInfo.OrgType == "3"))
                    {
                        companyId = CurrentUser.CompanyId.ToString();
                    }
                }
            }

            return companyId;
        }

    }
}