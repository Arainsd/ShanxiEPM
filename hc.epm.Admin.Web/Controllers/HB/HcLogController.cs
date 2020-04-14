using hc.epm.Common;
using hc.epm.Admin.ClientProxy;
using hc.epm.UI.Common;
using hc.epm.DataModel.Basic;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Admin.Web.Controllers.HB
{
    public class HcLogController : BaseHBController
    {
        /// <summary>
        /// 日志列表
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="actionDesc"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult Index(string moduleName = "", string actionDesc = "", string startTime = "", string endTime = "", int pageIndex = 1, int pageSize = 10)
        {

            ViewBag.moduleName = moduleName;
            ViewBag.actionDesc = actionDesc;

            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;

            ViewBag.pageIndex = pageIndex;
            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();
            if (!string.IsNullOrEmpty(moduleName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "ModuleName";
                ce.ExpValue = "%" + moduleName + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(actionDesc))
            {
                ce = new ConditionExpression();
                ce.ExpName = "ActionDesc";
                ce.ExpValue = "%" + actionDesc + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(startTime))
            {
                ce = new ConditionExpression();
                ce.ExpName = "CreateTime";
                ce.ExpValue = startTime.ToDateTimeReq();
                ce.ExpOperater = eConditionOperator.GreaterThanOrEqual;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                ce = new ConditionExpression();
                ce.ExpName = "CreateTime";
                ce.ExpValue = endTime.ToDateTimeReq();
                ce.ExpOperater = eConditionOperator.LessThanOrEqual;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            Result<List<Base_Log>> result = new Result<List<Base_Log>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetLogList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);

        }

        /// <summary>
        /// 审核日志
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tableId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult AutigLog(string tableName = "", string tableId = "", string startTime = "", string endTime = "", int pageIndex = 1, int pageSize = 10)
        {

            ViewBag.moduleName = tableName;
            ViewBag.actionDesc = tableId;

            ViewBag.startTime = startTime;
            ViewBag.endTime = endTime;

            ViewBag.pageIndex = pageIndex;
            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();
            if (!string.IsNullOrEmpty(tableName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "tableName";
                ce.ExpValue = "%" + tableName + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(tableId))
            {
                ce = new ConditionExpression();
                ce.ExpName = "tableId";
                ce.ExpValue = tableId;
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(startTime))
            {
                ce = new ConditionExpression();
                ce.ExpName = "CreateTime";
                ce.ExpValue = startTime.ToDateTimeReq();
                ce.ExpOperater = eConditionOperator.GreaterThanOrEqual;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                ce = new ConditionExpression();
                ce.ExpName = "CreateTime";
                ce.ExpValue = endTime.ToDateTimeReq();
                ce.ExpOperater = eConditionOperator.LessThanOrEqual;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            Result<List<Base_StatusLog>> result = new Result<List<Base_StatusLog>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetStatusLogList(qc);

                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);

        }
    }
}
