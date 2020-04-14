
using hc.epm.Admin.ClientProxy;
using hc.epm.Common;
using hc.epm.DataModel.Msg;
using hc.epm.UI.Common;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Admin.Web.Controllers
{
    public class EmailListController : BaseController
    {
        // GET: EmailList
        [AuthCheck(Module = AdminModule.EmailList, Right = SystemRight.Browse)]
        public ActionResult Index(string SmsCons = "",string BenginDate="",string EndDate="", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.SmsCon = SmsCons;
            ViewBag.pageIndex = pageIndex;
            var UserID = CurrentUser.UserId;
            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();
            if (!string.IsNullOrEmpty(SmsCons))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Title";
                ce.ExpValue = "%" + SmsCons + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(BenginDate))
            {
                ce = new ConditionExpression();
                ce.ExpName = "SenderTime";
                ce.ExpValue = Convert.ToDateTime(BenginDate);
                ce.ExpOperater = eConditionOperator.GreaterThanOrEqual;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                ce = new ConditionExpression();
                ce.ExpName = "SenderTime";
                ce.ExpValue = Convert.ToDateTime(EndDate + " 23:59:59");
                ce.ExpOperater = eConditionOperator.LessThanOrEqual;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            Result<List<Msg_Email>> result = new Result<List<Msg_Email>>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetEmailList(qc);
                ViewBag.Total = result.AllRowsCount;
            }

            return View(result.Data);
        }
        /// <summary>
        /// 查看消息详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.EmailList, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            Result<Msg_Email> result = new Result<Msg_Email>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetEmailModel(id);

            }
            return View(result.Data);
        }
    }
}

