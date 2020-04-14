using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    public class TzDevResourceReportController : BaseWebController
    {
        /// <summary>
        /// 加油（气）站开发资源上报流程
        /// </summary>
        /// <returns></returns>
        // GET: TzDevResourceReport
        public ActionResult Index(string projectName = "", string companyName = "", string companyId = "", string startTime = "", string endTime = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.projectName = projectName;
            ViewBag.companyName = companyName;
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;

            Result<List<TzDevResourceReportItemView>> result = new Result<List<TzDevResourceReportItemView>>();

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
                if (!string.IsNullOrEmpty(projectName))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "ProjectName";
                    ce.ExpValue = projectName;
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(companyId))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "CompanyId";
                    ce.ExpValue = companyId;
                    ce.ExpOperater = eConditionOperator.Equal;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrWhiteSpace(startTime))
                {
                    DateTime stime = Convert.ToDateTime(startTime);
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "ApplyDate",
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

                        ExpName = "ApplyDate",
                        ExpValue = etime,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.LessThanOrEqual
                    });
                }
                qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
                #endregion

                result = proxy.GetTzDevResourceReportItemList(qc);
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
                ViewBag.ApplyDate = DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.Title = "陕西省意向开发加油（气）站上报流程" + CurrentUser.RealName + DateTime.Now.ToString("yyyy-MM-dd");
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

                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ProjectNature };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                // 项目性质
                ViewBag.ProjectNature = subjects[DictionaryType.ProjectNature].Where(t => t.No == "HZHZ" || t.No == "XINJ" || t.No == "ZUL" || t.No == "SHOUG").ToList().ToSelectList("Name", "No", false);
            }
            return View();
        }

        /// <summary>
        /// 加油（气）站开发资源上报
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(Epm_TzDevResourceReport model)
        {
            Result<int> result = new Result<int>();
            string tzDevResourceReportItem = Request.Form["tzDevResourceReportItem"];//获取人员变更情况json字符串

            if (!string.IsNullOrEmpty(tzDevResourceReportItem))
            {
                model.TzDevResourceReportItem = JsonConvert.DeserializeObject<List<Epm_TzDevResourceReportItem>>(tzDevResourceReportItem);
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddTzDevResourceReport(model);
            }
            return Json(result.ToResultView());
        }


        public ActionResult Edit(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetTzDevResourceReportModel(id);

                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ProjectNature };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                // 项目性质
                ViewBag.ProjectNature = subjects[DictionaryType.ProjectNature].Where(t => t.No == "HZHZ" || t.No == "XINJ" || t.No == "ZUL" || t.No == "SHOUG").ToList().ToSelectList("Name", "No", false);

                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }
                return View();
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Epm_TzDevResourceReport model)
        {
            Result<int> result = new Result<int>();
            string tzDevResourceReportItem = Request.Form["tzDevResourceReportItem"];//获取人员变更情况json字符串

            if (!string.IsNullOrEmpty(tzDevResourceReportItem))
            {
                model.TzDevResourceReportItem = JsonConvert.DeserializeObject<List<Epm_TzDevResourceReportItem>>(tzDevResourceReportItem);
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateTzDevResourceReport(model);
            }
            return Json(result.ToResultView());
        }

        public ActionResult Detail(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetTzDevResourceReportModel(id);
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