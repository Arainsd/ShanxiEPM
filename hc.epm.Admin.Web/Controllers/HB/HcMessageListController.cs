
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

namespace hc.epm.Admin.Web.Controllers.HB
{
    public class HcMessageListController : BaseHBController
    {
        // GET: SMSList
        public ActionResult Index(string SmsCon = "", string BenginDate = "", string EndDate = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.SmsCon = SmsCon;
            ViewBag.pageIndex = pageIndex;
            var UserID = CurrentUser.UserId;
            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();
            if (!string.IsNullOrEmpty(SmsCon))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Title";
                ce.ExpValue = "%" + SmsCon + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(BenginDate))
            {
                ce = new ConditionExpression();
                ce.ExpName = "SendTime";
                ce.ExpValue = Convert.ToDateTime(BenginDate);
                ce.ExpOperater = eConditionOperator.GreaterThanOrEqual;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                ce = new ConditionExpression();
                ce.ExpName = "SendTime";
                ce.ExpValue = Convert.ToDateTime(EndDate + " 23:59:59");
                ce.ExpOperater = eConditionOperator.LessThanOrEqual;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            Result<List<Msg_Message>> result = new Result<List<Msg_Message>>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetMessageList(qc);
                ViewBag.Total = result.AllRowsCount;
            }
            return View(result.Data);
        }
        /// <summary>
        /// 查看消息详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(long id)
        {
            Result<Msg_Message> result = new Result<Msg_Message>();
            Result<Msg_Message> readResult = new Result<Msg_Message>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetMessageModel(id);
            }
            return View(result.Data);
        }
    }
}