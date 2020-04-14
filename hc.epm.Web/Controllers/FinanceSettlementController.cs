using hc.epm.Common;
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
    /// <summary>
    /// 财务决算
    /// </summary>
    public class FinanceSettlementController : BaseWebController
    {
        // GET: FinanceSettlement
        /// <summary>
        /// 财务决算列表
        /// </summary>
        /// <param name="name">项目名称</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebCategory.FinanceAccount, Right = SystemRight.Browse)]
        public ActionResult Index(string name = "", string startTime = "", string endTime = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;
            ViewBag.pageIndex = pageIndex;

            Result<List<Epm_TzProjectApprovalInfo>> result = new Result<List<Epm_TzProjectApprovalInfo>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {

                #region 查询条件
                QueryCondition qc = new QueryCondition();
                ConditionExpression ce = null;
                if (!string.IsNullOrEmpty(name))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "name";
                    ce.ExpValue = name;
                    ce.ExpOperater = eConditionOperator.Equal;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrWhiteSpace(startTime))
                {
                    DateTime stime = Convert.ToDateTime(startTime);
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "startTime",
                        ExpValue = stime,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.GreaterThanOrEqual
                    });
                }
                if (!string.IsNullOrWhiteSpace(endTime))
                {
                    DateTime etime = Convert.ToDateTime(endTime);
                    qc.ConditionList.Add(new ConditionExpression()
                    {

                        ExpName = "endTime",
                        ExpValue = etime,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.LessThanOrEqual
                    });
                }
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
                #endregion

                result = proxy.GetTzProjectApprovalListBy(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            return View(result.Data);
        }
        
        /// <summary>
        /// 编辑财务决算
        /// </summary>
        /// <param name="id"></param>
        /// <param name="FinanceAccounts"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebCategory.FinanceAccount, Right = SystemRight.Modify)]
        public ActionResult Add()
        {
            Result<int> result = new Result<int>();
            long id = Convert.ToInt64(Request.Form["id"]);
            decimal financeAccounts = Convert.ToDecimal(Request.Form["FinanceAccounts"]);

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateFinanceAccounts(id, financeAccounts);
            }
            return Json(result.ToResultView());
        }
    }
}