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

namespace hc.epm.Admin.Web.Controllers.HB
{
    public class HcSMSTempleteController : BaseHBController
    {
        // GET: SMSTemplete

        public ActionResult Index(string Name = "", string IsEnable = "", string IsConfirm = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.Name = Name;
            ViewBag.pageIndex = pageIndex;
            //下拉框
            ViewBag.Step = Enum<MessageStep>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", true);
            ViewBag.IsConfirm = HelperExt.GetConfirmList(true, IsConfirm);
            ViewBag.IsEnable = HelperExt.GetEnableList(true, IsEnable);
            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();
            ce = new ConditionExpression();
            if (!string.IsNullOrEmpty(Name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Name";
                ce.ExpValue = "%" + Name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (IsEnable != "")
            {
                ce = new ConditionExpression();
                ce.ExpName = "IsEnable";
                ce.ExpValue = IsEnable == EnumState.Enable.ToString();
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (IsConfirm != "")
            {
                ce = new ConditionExpression();
                ce.ExpName = "IsConfirm";
                ce.ExpValue = IsConfirm == EnumState.Confirmed.ToString();
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            Result<List<Msg_SMSTemplete>> result = new Result<List<Msg_SMSTemplete>>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetSMSTempleteList(qc);
                ViewBag.Total = result.AllRowsCount;
            }

            return View(result.Data);
        }

        // GET: SMSTemplete/Detail/5
        public ActionResult Detail(long id)
        {
            Result<Msg_SMSTemplete> result = new Result<Msg_SMSTemplete>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetSMSTempleteModel(id);

            }


            return View(result.Data);
        }

        // GET: SMSTemplete/Create
        public ActionResult Add()
        {
            ViewBag.IsConfirm = HelperExt.GetConfirmList(false);
            ViewBag.IsEnable = HelperExt.GetEnableList(false);
            ViewBag.Step = Enum<MessageStep>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", true);
            return View();
        }

        // POST: SMSTemplete/Create
        [HttpPost]
        public ActionResult Add(Msg_SMSTemplete model)
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
                result = proxy.AddSMSTemplete(model);
                view = result.ToResultView();
            }
            return Json(view);
        }
        // GET: SMSTemplete/Edit/5
        public ActionResult Edit(long id)
        {
            Result<Msg_SMSTemplete> result = new Result<Msg_SMSTemplete>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetSMSTempleteModel(id);

            }
            ViewBag.Step = Enum<MessageStep>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", true, result.Data.Step);
            var isConfirm = result.Data.IsConfirm ? EnumState.Confirmed.ToString() : EnumState.NoConfim.ToString();
            var isEnable = result.Data.IsEnable ? EnumState.Enable.ToString() : EnumState.Disable.ToString();

            ViewBag.IsEnable = HelperExt.GetEnableList(false, isEnable);
            ViewBag.IsConfirm = HelperExt.GetConfirmList(false, isConfirm);


            return View(result.Data);
        }

        // POST: SMSTemplete/Edit/5
        [HttpPost]
        public ActionResult Edit(Msg_SMSTemplete model)
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
                result = proxy.UpdateSMSTemplete(model);
                view = result.ToResultView();
            }
            return Json(view);
        }
        //EditState
        [HttpPost]

        public ActionResult EditState(long smsTempleteId, int type)
        {
            Result<int> result = new Result<int>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.AuditSMSTemplete(smsTempleteId, type);
            }
            return Json(result.ToResultView());
        }

        // POST: SMSTemplete/Delete/5
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteSMSTempleteByIds(list);
            }
            return Json(result.ToResultView());
        }
    }
}
