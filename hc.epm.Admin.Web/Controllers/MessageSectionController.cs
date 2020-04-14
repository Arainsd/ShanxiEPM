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
    public class MessageSectionController : BaseController
    {
        // GET: MessageSection
        [AuthCheck(Module = AdminModule.MessageSection, Right = SystemRight.Browse)]
        public ActionResult Index( string Name = "0", string IsEnable = "0", string IsConfirm = "0", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.pageIndex = pageIndex;
            //下拉框
            ViewBag.Name = Enum<MessageStep>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", true,Name);
            ViewBag.IsConfirm = HelperExt.GetConfirmList(true, IsConfirm);
            ViewBag.IsEnable = HelperExt.GetEnableList(true, IsEnable);
            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();
            ce = new ConditionExpression();
            if (Name != "0")
            {
                ce = new ConditionExpression();
                ce.ExpName = "Name";
                ce.ExpValue = Name;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.Or;
                qc.ConditionList.Add(ce);
            }
            if (IsEnable != "0")
            {
                ce = new ConditionExpression();
                ce.ExpName = "IsEnable";
                ce.ExpValue = IsEnable == EnumState.Enable.ToString();
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (IsConfirm != "0")
            {
                ce = new ConditionExpression();
                ce.ExpName = "IsConfirm";
                ce.ExpValue = IsConfirm == EnumState.Confirmed.ToString();
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            Result<List<Msg_MessageSection>> result = new Result<List<Msg_MessageSection>>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetSectionList(qc);
                ViewBag.Total = result.AllRowsCount;
            }
            
            return View(result.Data);
        }
        // GET: MessageSection/Create
        [AuthCheck(Module = AdminModule.MessageSection, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            ViewBag.IsConfirm = HelperExt.GetConfirmList(false);
            ViewBag.IsEnable = HelperExt.GetEnableList(false);
            ViewBag.Name = Enum<MessageStep>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", true);
            return View();
        }

        // POST: MessageSection/Create
        [HttpPost]
        [AuthCheck(Module = AdminModule.MessageSection, Right = SystemRight.Add)]
        public ActionResult Add(Msg_MessageSection model)
        {
            string RoleTypes = Request.Form["RoleTypes"];
            model.RoleTypes = RoleTypes;
            string MsgTypes = Request.Form["MsgTypes"];
            model.MsgTypes = MsgTypes;
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
                result = proxy.AddSection(model);
                view = result.ToResultView();
            }
            return Json(view);
        }

        // GET: MessageSection/Edit/5
        [AuthCheck(Module = AdminModule.MessageSection, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<Msg_MessageSection> result = new Result<Msg_MessageSection>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetSectionModel(id);

            }
            ViewBag.Name = Enum<MessageStep>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", true, result.Data.Name);
            var isConfirm = result.Data.IsConfirm ? EnumState.Confirmed.ToString() : EnumState.NoConfim.ToString();
            var isEnable = result.Data.IsEnable ? EnumState.Enable.ToString() : EnumState.Disable.ToString();

            ViewBag.IsEnable = HelperExt.GetEnableList(false, isEnable);
            ViewBag.IsConfirm = HelperExt.GetConfirmList(false, isConfirm);


            return View(result.Data);
        }

        // POST: MessageSection/Edit/5
        [HttpPost]
        [AuthCheck(Module = AdminModule.MessageSection, Right = SystemRight.Modify)]
        public ActionResult Edit(Msg_MessageSection model)
        {
            string RoleTypes = Request.Form["RoleTypes"];
            model.RoleTypes = RoleTypes;
            string MsgTypes = Request.Form["MsgTypes"];
            model.MsgTypes = MsgTypes;
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
                result = proxy.UpdateSection(model);
                view = result.ToResultView();
            }
            return Json(view);
        }
        //EditState
        [HttpPost]
        public ActionResult EditState(long sectionId,  int type)
        {
            Result<int> result = new Result<int>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.AuditSection(sectionId, type);
            }
            return Json(result.ToResultView());
        }
        // POST: MessageSection/Delete/5
        [HttpPost]
        [AuthCheck(Module = AdminModule.MessageSection, Right = SystemRight.Delete)]
        public ActionResult Delete(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteSectionByIds(list);
            }
            return Json(result.ToResultView());
        }
    }
}
