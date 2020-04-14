
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
    public class SMSListController : BaseController
    {
        // GET: SMSList
        [AuthCheck(Module = AdminModule.SMSHistory, Right = SystemRight.Browse)]
        public ActionResult Index(string SendCom = "", string BenginDate = "", string EndDate = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.SendCom = SendCom;
            ViewBag.pageIndex = pageIndex;
            var UserID = CurrentUser.UserId;
            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();
            if (!string.IsNullOrEmpty(SendCom))
            {
                ce = new ConditionExpression();
                ce.ExpName = "SendeCompanyId";
                ce.ExpValue = SendCom;
                ce.ExpOperater = eConditionOperator.In;
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
            Result<List<Msg_SMS>> result = new Result<List<Msg_SMS>>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetSMSList(qc);
                ViewBag.Total = result.AllRowsCount;
            }

            return View(result.Data);
        }
        /// <summary>
        /// 查看消息详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.SMSHistory, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            Result<Msg_SMS> result = new Result<Msg_SMS>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetSMSModel(id);

            }
            return View(result.Data);
        }
    }
}

