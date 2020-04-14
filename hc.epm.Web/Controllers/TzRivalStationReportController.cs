using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    public class TzRivalStationReportController : BaseWebController
    {
        /// <summary>
        /// 陕西省各竞争对手加油（气）站现状上报流程
        /// </summary>
        /// <returns></returns>
        // GET: TzRivalStationReport
        public ActionResult Index(string companyName = "", string companyId = "", string startTime = "", string endTime = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.companyName = companyName;
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            Result<List<Epm_TzRivalStationReport>> result = new Result<List<Epm_TzRivalStationReport>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                ViewBag.IsAgencyUser = false;
                var companyInfo = proxy.GetCompanyModel(CurrentUser.CompanyId).Data;
                if (companyInfo != null)
                {
                    //是省公司
                    if (companyInfo.OrgType == "1" || (companyInfo.PId == 10 && companyInfo.OrgType == "3"))
                    {
                        ViewBag.IsAgencyUser = true;
                        companyId = "";
                    }
                    else if (companyInfo.OrgType == "2" || (companyInfo.PId != 10 && companyInfo.OrgType == "3"))
                    {
                        companyId = CurrentUser.CompanyId.ToString();
                        ViewBag.CompanyName = CurrentUser.CompanyName;
                    }
                }

                #region 查询条件
                QueryCondition qc = new QueryCondition();
                ConditionExpression ce = null;
                if (!string.IsNullOrEmpty(companyId))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "CompanyId";
                    ce.ExpValue = Convert.ToInt64(companyId);
                    ce.ExpOperater = eConditionOperator.Equal;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrWhiteSpace(startTime))
                {
                    DateTime stime = Convert.ToDateTime(startTime);
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "TotalEndDate",
                        ExpValue = stime,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.GreaterThanOrEqual
                    });
                }
                if (!string.IsNullOrWhiteSpace(endTime))
                {
                    DateTime etime = Convert.ToDateTime(endTime + " 23:59:59");
                    qc.ConditionList.Add(new ConditionExpression()
                    {

                        ExpName = "TotalEndDate",
                        ExpValue = etime,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.LessThanOrEqual
                    });
                }
                qc.SortList.Add(new SortExpression("TotalEndDate", eSortType.Desc));
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
                #endregion

                result = proxy.GetTzRivalStationReportList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            return View(result.Data);
        }

        public ActionResult Add()
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                ViewBag.ApplyUser = CurrentUser.RealName;
                ViewBag.ApplyUserId = CurrentUser.UserId;
                ViewBag.CompanyName = CurrentUser.CompanyName;
                ViewBag.CompanyId = CurrentUser.CompanyId;
                ViewBag.Title = "陕西省各竞争对手加油（气）站现状上报流程" + CurrentUser.RealName + DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.ApplyDate = DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.DepartmentID = "";
                ViewBag.Department = "";
                //获取用户信息
                var userInfo = proxy.GetUserModel(CurrentUser.UserId);
                if (userInfo.Data != null)
                {
                    if (userInfo.Data.DepartmentId != null)
                    {
                        long dempId = userInfo.Data.DepartmentId.Value;
                        if (dempId != 0)
                        {
                            var companyInfo = proxy.GetCompanyModel(dempId);

                            if (companyInfo.Data != null)
                            {
                                ViewBag.DepartmentID = companyInfo.Data.Id;
                                ViewBag.Department = companyInfo.Data.Name;
                            }
                        }
                    }
                }

            }
            return View();
        }

        /// <summary>
        /// 各竞争对手加油（气）站现状上报
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(Epm_TzRivalStationReport model)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddTzRivalStationReport(model);
            }
            return Json(result.ToResultView());
        }


        public ActionResult Edit(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetTzRivalStationReportModel(id);
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }

                return View();
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Epm_TzRivalStationReport model)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateTzRivalStationReport(model);
            }
            return Json(result.ToResultView());
        }

        public ActionResult Detail(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetTzRivalStationReportModel(id);
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }

                return View();
            }
        }

        #region 获取地区
        /// <summary>
        /// 根据parentCode获取地区列表
        /// </summary>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RegionList(string parentCode)
        {
            ViewBag.parentCode = parentCode;
            Result<List<Base_Region>> result = new Result<List<Base_Region>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.LoadRegionList(parentCode);
            }
            return Json(result.ToResultView());
        }
        #endregion
    }
}