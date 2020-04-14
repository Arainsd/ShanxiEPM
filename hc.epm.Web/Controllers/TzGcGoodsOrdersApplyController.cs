using hc.epm.Common;
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
    public class TzGcGoodsOrdersApplyController : BaseWebController
    {
        // GET: TzGcGoodsOrdersApply
        /// <summary>
        /// 工程甲供物资订单列表
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="projectName">项目名称</param>
        /// <param name="companyName">申请单位</param>
        /// <param name="companyId">申请单位Id</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="supplierName">供应商名称</param>
        /// <param name="materialNumber">物资种类</param>
        /// <param name="state">状态</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult Index(string title = "", string projectName = "", string companyName = "", string companyId = "", string startTime = "", string endTime = "", string supplierName = "", string materialNumber = "", string state = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.projectName = projectName;
            ViewBag.companyName = companyName;
            ViewBag.pageIndex = pageIndex;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.title = title;
            ViewBag.supplierName = supplierName;
            ViewBag.materialNumber = materialNumber;
            ViewBag.state = state;

            Result<List<TzGcGoodsOrdersItemView>> result = new Result<List<TzGcGoodsOrdersItemView>>();

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
                if (!string.IsNullOrEmpty(title))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "Title";
                    ce.ExpValue = title;
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(supplierName))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "SupplierName";
                    ce.ExpValue = supplierName;
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(materialNumber))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "MaterialNumber";
                    ce.ExpValue = materialNumber;
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(state))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "State";
                    ce.ExpValue = state;
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
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

                result = proxy.GetTzGcGoodsOrdersApplyListAll(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);

                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.MaterialNumber };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                // 物资种类
                ViewBag.MaterialNumber = subjects[DictionaryType.MaterialNumber].ToSelectList("Name", "No", true);
            }

            return View(result.Data);
        }

        /// <summary>
        /// 工程甲供物资订单添加
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                ViewBag.Applicant = CurrentUser.RealName;
                ViewBag.ApplicantId = CurrentUser.UserId;
                ViewBag.CompanyName = CurrentUser.CompanyName;
                ViewBag.CompanyId = CurrentUser.CompanyId;
                ViewBag.ApplyDate = DateTime.Now.ToString("yyyy-MM-dd");
                ViewBag.Title = "工程甲供物资订单审批流程" + CurrentUser.RealName + DateTime.Now.ToString("yyyy-MM-dd");
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
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.MaterialNumber };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                // 物资种类
                ViewBag.MaterialNumber = subjects[DictionaryType.MaterialNumber].ToSelectList("Name", "No", false);
            }
            return View();
        }

        /// <summary>
        /// 工程甲供物资订单添加
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(Epm_TzGcGoodsOrdersApply model)
        {
            Result<int> result = new Result<int>();
            string tzGcGoodsOrdersItem = Request.Form["tzGcGoodsOrdersItem"];//获取人员变更情况json字符串

            if (!string.IsNullOrEmpty(tzGcGoodsOrdersItem))
            {
                model.TzGcGoodsOrdersItem = JsonConvert.DeserializeObject<List<Epm_TzGcGoodsOrdersItem>>(tzGcGoodsOrdersItem);
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddTzGcGoodsOrdersApply(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 工程甲供物资订单修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetTzGcGoodsOrdersApplyModel(id);

                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.MaterialNumber };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                // 物资种类
                ViewBag.MaterialNumber = subjects[DictionaryType.MaterialNumber].ToSelectList("Name", "No", false, result.Data.MaterialNumber);

                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }
                return View();
            }
        }

        /// <summary>
        /// 工程甲供物资订单修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Epm_TzGcGoodsOrdersApply model)
        {
            Result<int> result = new Result<int>();
            string tzGcGoodsOrdersItem = Request.Form["tzGcGoodsOrdersItem"];//获取人员变更情况json字符串

            if (!string.IsNullOrEmpty(tzGcGoodsOrdersItem))
            {
                model.TzGcGoodsOrdersItem = JsonConvert.DeserializeObject<List<Epm_TzGcGoodsOrdersItem>>(tzGcGoodsOrdersItem);
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateTzGcGoodsOrdersApply(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 工程甲供物资订单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetTzGcGoodsOrdersApplyModel(id);
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }

                return View();
            }
        }
    }
}