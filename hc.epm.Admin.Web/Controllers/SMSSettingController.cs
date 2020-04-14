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
    public class SMSSettingController : BaseController
    {
        // GET: SMSSetting
        [AuthCheck(Module = AdminModule.SMSSetting, Right = SystemRight.Browse)]

        public ActionResult Index(string Name = "", string IsEnable = "0", string IsConfirm = "0", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.Name = Name;
            ViewBag.pageIndex = pageIndex;
            //下拉框
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
            Result<List<Msg_SMSSetting>> result = new Result<List<Msg_SMSSetting>>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetSMSSettingList(qc);
                ViewBag.Total = result.AllRowsCount;
            }

            return View(result.Data);
        }

        // GET: SMSSetting/Detail/5
        [AuthCheck(Module = AdminModule.SMSSetting, Right = SystemRight.Browse)]
        public ActionResult Detail(long id)
        {
            Result<Msg_SMSSetting> result = new Result<Msg_SMSSetting>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetSMSSettingModel(id);

            }


            return View(result.Data);
        }

        // GET: SMSSetting/Add
        [AuthCheck(Module = AdminModule.SMSSetting, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            ViewBag.IsConfirm = HelperExt.GetConfirmList(false);
            ViewBag.IsEnable = HelperExt.GetEnableList(false);
            return View();
        }

        // POST: SMSSetting/Add
        [HttpPost]
        [AuthCheck(Module = AdminModule.SMSSetting, Right = SystemRight.Add)]
        public ActionResult Add(Msg_SMSSetting model)
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
                result = proxy.AddSMSSetting(model);
                view = result.ToResultView();
            }
            return Json(view);
        }
        // GET: SMSSetting/Edit/5
        [AuthCheck(Module = AdminModule.SMSSetting, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<Msg_SMSSetting> result = new Result<Msg_SMSSetting>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetSMSSettingModel(id);

            }
            var isConfirm = result.Data.IsConfirm ? EnumState.Confirmed.ToString() : EnumState.NoConfim.ToString();
            var isEnable = result.Data.IsEnable ? EnumState.Enable.ToString() : EnumState.Disable.ToString();

            ViewBag.IsEnable = HelperExt.GetEnableList(false, isEnable);
            ViewBag.IsConfirm = HelperExt.GetConfirmList(false, isConfirm);


            return View(result.Data);
        }

        // POST: SMSSetting/Edit/5
        [HttpPost]
        [AuthCheck(Module = AdminModule.SMSSetting, Right = SystemRight.Modify)]
        public ActionResult Edit(Msg_SMSSetting model)
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
                result = proxy.UpdateSMSSetting(model);
                view = result.ToResultView();
            }
            return Json(view);
        }

        // GET: SMSSetting/EditState/5
        [HttpPost]
        public ActionResult EditState(long smsSettingId, int type)
        {
            Result<int> result = new Result<int>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.AuditSMSSetting(smsSettingId, type);
            }
            return Json(result.ToResultView());
        }

        // POST: SMSSetting/Delete/5
        [HttpPost]
        [AuthCheck(Module = AdminModule.SMSSetting, Right = SystemRight.Delete)]
        public ActionResult Delete(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteSMSSettingByIds(list);
            }
            return Json(result.ToResultView());
        }
    }
}
