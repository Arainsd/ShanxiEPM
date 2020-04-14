using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
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
    /// <summary>
    /// 工程建设项目开工报告申请表
    /// </summary>
    public class TzStartsApplyController : BaseWebController
    {
        // GET: TzStartsApply
        /// <summary>
        /// 查询工程建设项目开工报告列表
        /// </summary>
        /// <param name="projectName">建设项目名称</param>
        /// <param name="unit">所属单位</param>
        /// <param name="startTime">计划建设工期开始</param>
        /// <param name="endTime">计划建设工期结束</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult Index(string projectName = "", string unit = "", string companyId = "", string startTime = "", string endTime = "", string startTime2 = "", string endTime2 = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.projectName = projectName;
            ViewBag.unit = unit;
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;

            ViewBag.startTime2 = startTime2;
            ViewBag.endTime2 = endTime2;

            Result<List<Epm_TzStartsApply>> result = new Result<List<Epm_TzStartsApply>>();

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
                    ce.ExpValue = "%" + projectName + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(companyId))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "UnitID";
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
                        ExpName = "StartTime",
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

                        ExpName = "StartTime",
                        ExpValue = etime,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.LessThanOrEqual
                    });
                }

                if (!string.IsNullOrWhiteSpace(startTime2))
                {
                    DateTime stime2 = Convert.ToDateTime(startTime2);
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "EndTime",
                        ExpValue = stime2,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.GreaterThanOrEqual
                    });
                }
                if (!string.IsNullOrWhiteSpace(endTime2))
                {
                    DateTime etime2 = Convert.ToDateTime(endTime2 + " 23:59:59");
                    qc.ConditionList.Add(new ConditionExpression()
                    {

                        ExpName = "EndTime",
                        ExpValue = etime2,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.LessThanOrEqual
                    });
                }

                qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
                #endregion
                result = proxy.GetTzStartsApplyList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            return View(result.Data);
        }

        /// <summary>
        /// 工程建设项目开工报告添加页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.CapitalSource };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //资金来源
                ViewBag.MoneySourceType = subjects[DictionaryType.CapitalSource].ToSelectList("Name", "No", false);

                ViewBag.Applicant = CurrentUser.RealName;
                ViewBag.ApplicantID = CurrentUser.UserId;
                ViewBag.Unit = CurrentUser.CompanyName;
                ViewBag.UnitID = CurrentUser.CompanyId;
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
        /// 工程建设项目开工报告添加方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(Epm_TzStartsApply model)
        {
            Result<int> result = new Result<int>();
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }

            model.PlanHtml = HttpUtility.HtmlDecode(model.Plan);
            model.SignIdeaHtml = HttpUtility.HtmlDecode(model.SignIdea);

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddTzStartsApply(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 工程建设项目开工报告修改页面
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetTzStartsApplyModel(id);
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.CapitalSource };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //资金来源
                ViewBag.MoneySourceType = subjects[DictionaryType.CapitalSource].Where(t => t.No == "TZJH" || t.No == "FY").ToList().ToSelectList("Name", "No", false, result.Data == null ? "" : result.Data.MoneySourceType);

                ViewBag.UnitID = CurrentUser.CompanyId;
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }
                return View();
            }
        }

        /// <summary>
        /// 工程建设项目开工报告修改方法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Epm_TzStartsApply model)
        {
            Result<int> result = new Result<int>();
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.TzAttachs = JsonConvert.DeserializeObject<List<Epm_TzAttachs>>(fileDataJson);//将文件信息json字符
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateTzStartsApply(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 查询工程建设项目开工报告详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetTzStartsApplyModel(id);
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }

                return View();
            }
        }
    }
}