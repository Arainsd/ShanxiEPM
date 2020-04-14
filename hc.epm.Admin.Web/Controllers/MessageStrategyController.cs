using hc.epm.Admin.ClientProxy;
using hc.epm.DataModel.Basic;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using hc.epm.Common;
using hc.epm.UI.Common;
using hc.Plat.Common.Extend;
using hc.epm.ViewModel;
using Newtonsoft.Json;
using hc.epm.DataModel.Msg;

namespace hc.epm.Admin.Web.Controllers
{
    public class MessageStrategyController : BaseController
    {
        // GET: MessageStrategy
        [AuthCheck(Module = AdminModule.MessageStrategy, Right = SystemRight.Browse)]
        public ActionResult Index(string Type = "0", string IsEnable = "0", string IsConfirm = "0", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.Type = Type;
            ViewBag.pageIndex = pageIndex;
            //下拉框
            ViewBag.Type = Enum<MessageType>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", true,Type);
            ViewBag.IsConfirm = HelperExt.GetConfirmList(true, IsConfirm);
            ViewBag.IsEnable = HelperExt.GetEnableList(true, IsEnable);
            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();
            ce = new ConditionExpression();
            if (!string.IsNullOrEmpty(Type) && Type != "0")
            {
                ce = new ConditionExpression();
                ce.ExpName = "Type";
                ce.ExpValue = Type;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(IsEnable) && IsEnable != "0")
            {
                ce = new ConditionExpression();
                ce.ExpName = "IsEnable";
                ce.ExpValue = IsEnable == EnumState.Enable.ToString();
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(IsConfirm) && IsConfirm != "0")
            {
                ce = new ConditionExpression();
                ce.ExpName = "IsConfirm";
                ce.ExpValue = IsConfirm == EnumState.Confirmed.ToString();
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            Result<List<Msg_MessageStrategy>> result = new Result<List<Msg_MessageStrategy>>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetStrategyList(qc);
                ViewBag.Total = result.AllRowsCount;
            }

            return View(result.Data);
        }
        // GET: MessageStrategy/Add
        [AuthCheck(Module = AdminModule.MessageStrategy, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            ViewBag.IsConfirm = HelperExt.GetConfirmList(false);
            ViewBag.IsEnable = HelperExt.GetEnableList(false);
            ViewBag.Type = Enum<MessageType>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", true);
            return View();
        }

        // POST: MessageStrategy/Add
        [HttpPost]
        [AuthCheck(Module = AdminModule.MessageStrategy, Right = SystemRight.Add)]
        public ActionResult Add(Msg_MessageStrategy model)
        {
            Result<int> result = new Result<int>();
            ResultView<int> view = new ResultView<int>();
            if (Request.Form["IsEnable"] == EnumState.Enable.ToString())
            {
                model.IsEnable = true;
            }
            else
            {
                model.IsEnable = false;
            }
            if (Request.Form["IsConfirm"] == EnumState.Confirmed.ToString())
            {
                model.IsConfirm = true;
            }
            else
            {
                model.IsConfirm = false;
            }
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddStrategy(model);
                view = result.ToResultView();
            }
            return Json(view);
        }

        // GET: MessageStrategy/Edit/5
        [AuthCheck(Module = AdminModule.MessageStrategy, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<Msg_MessageStrategy> result = new Result<Msg_MessageStrategy>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetStrategyModel(id);

            }
            ViewBag.Type = Enum<MessageType>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", true, result.Data.Type);
            var isConfirm = result.Data.IsConfirm ? EnumState.Confirmed.ToString() : EnumState.NoConfim.ToString();
            var isEnable = result.Data.IsEnable ? EnumState.Enable.ToString() : EnumState.Disable.ToString();

            ViewBag.IsEnable = HelperExt.GetEnableList(false, isEnable);
            ViewBag.IsConfirm = HelperExt.GetConfirmList(false, isConfirm);


            return View(result.Data);
        }

        // POST: MessageStrategy/Edit/5
        [HttpPost]
        [AuthCheck(Module = AdminModule.MessageStrategy, Right = SystemRight.Modify)]
        public ActionResult Edit(Msg_MessageStrategy model)
        {
            Result<int> result = new Result<int>();
            ResultView<int> view = new ResultView<int>();
            if (Request.Form["IsEnable"] == EnumState.Enable.ToString())
            {
                model.IsEnable = true;
            }
            else
            {
                model.IsEnable = false;
            }
            if (Request.Form["IsConfirm"] == EnumState.Confirmed.ToString())
            {
                model.IsConfirm = true;
            }
            else
            {
                model.IsConfirm = false;
            }
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateStrategy(model);
                view = result.ToResultView();
            }
            return Json(view);
        }

        //EditState
        [HttpPost]
        public ActionResult EditState(long strategyId, int type)
        {
            Result<int> result = new Result<int>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.AuditStrategy(strategyId, type);
            }
            return Json(result.ToResultView());
        }

        // POST: MessageStrategy/Delete/5
        [HttpPost]
        [AuthCheck(Module = AdminModule.MessageStrategy, Right = SystemRight.Delete)]
        public ActionResult Delete(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteStrategyByIds(list);
            }
            return Json(result.ToResultView());
        }
    }
}
