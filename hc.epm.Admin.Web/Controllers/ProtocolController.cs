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


namespace hc.epm.Admin.Web.Controllers
{
    public class ProtocolController : BaseController
    {
        // GET: Protocol
        [AuthCheck(Module = AdminModule.ElectronicAgreement, Right = SystemRight.Browse)]
        public ActionResult Index(string Title = "", string IsEnable = "0", string IsConfirm = "0", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.Title = Title;
            ViewBag.pageIndex = pageIndex;
            ViewBag.IsConfirm = HelperExt.GetConfirmList(true, IsConfirm);
            ViewBag.IsEnable = HelperExt.GetEnableList(true, IsEnable);
            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();
            if (!string.IsNullOrEmpty(Title))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Title";
                ce.ExpValue = "%" + Title + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
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
            Result<List<Base_Protocol>> result = new Result<List<Base_Protocol>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProtocolList(qc);

                ViewBag.Total = result.AllRowsCount;
            }
            return View(result.Data);
        }

        // GET: Protocol/Add/5
        [AuthCheck(Module = AdminModule.ElectronicAgreement, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            ViewBag.IsConfirm = HelperExt.GetConfirmList(false);
            ViewBag.IsEnable = HelperExt.GetEnableList(false);
            ViewBag.Type = Enum<ProtocolType>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", false);
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        [AuthCheck(Module = AdminModule.ElectronicAgreement, Right = SystemRight.Add)]
        public ActionResult Add(Base_Protocol model)
        {

            Result<int> result = new Result<int>();
            ResultView<int> view = new ResultView<int>();
            model.Info = model.Info.FilterHtml();
            if (string.IsNullOrEmpty(model.Title))
            {
                view.Flag = false;
                view.Message = "标题不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Info))
            {
                view.Flag = false;
                view.Message = "内容不能为空";
                return Json(view);
            }
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

            string fileDataJson = Request.Form["fileDataJson"];

            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);
            foreach (var item in fileList)
            {
                item.TableId = model.Id;
                item.TableName = model.GetType().Name;

            }

            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddProtocol(model, fileList);
                view = result.ToResultView();
            }
            return Json(view);
        }
        //GET: Protocol/Detail/5
        [AuthCheck(Module = AdminModule.ElectronicAgreement, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {

            Result<Base_Protocol> result = new Result<Base_Protocol>();

            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProtocolModel(id);

            }
            var isConfirm = result.Data.IsConfirm ? EnumState.Confirmed.ToString() : EnumState.NoConfim.ToString();
            var isEnable = result.Data.IsEnable ? EnumState.Enable.ToString() : EnumState.Disable.ToString();

            ViewBag.IsEnable = HelperExt.GetEnableList(false, isEnable);
            ViewBag.IsConfirm = HelperExt.GetConfirmList(false, isConfirm);


            return View(result.Data);
        }
        //
        [HttpPost]
        [AuthCheck(Module = AdminModule.ElectronicAgreement, Right = SystemRight.Browse)]
        public ActionResult EditFiles(string tableName, long id)
        {
            Result<List<Base_Files>> result = new Result<List<Base_Files>>();
            ResultView<List<Base_Files>> view = new ResultView<List<Base_Files>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetFilesByTable(tableName, id);
                view = result.ToResultView();
            }
            return Json(view);
        }

        // GET: Protocol/Edit/5
        [AuthCheck(Module = AdminModule.ElectronicAgreement, Right = SystemRight.Modify)]
        public ActionResult Edit(string tableName, long id)
        {
            ViewBag.tableName = tableName;
            ViewBag.id = id;
            Result<Base_Protocol> result = new Result<Base_Protocol>();

            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProtocolModel(id);

            }
            var isConfirm = result.Data.IsConfirm ? EnumState.Confirmed.ToString() : EnumState.NoConfim.ToString();
            var isEnable = result.Data.IsEnable ? EnumState.Enable.ToString() : EnumState.Disable.ToString();
            ViewBag.Type = Enum<ProtocolType>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", false, result.Data.Type);
            ViewBag.IsEnable = HelperExt.GetEnableList(false, isEnable);
            ViewBag.IsConfirm = HelperExt.GetConfirmList(false, isConfirm);


            return View(result.Data);
        }

        // POST: Protocol/Edit/5
        [AuthCheck(Module = AdminModule.ElectronicAgreement, Right = SystemRight.Modify)]
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Base_Protocol model)
        {

            Result<int> result = new Result<int>();
            ResultView<int> view = new ResultView<int>();
            if (string.IsNullOrEmpty(model.Title))
            {
                view.Flag = false;
                view.Message = "标题不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Info))
            {
                view.Flag = false;
                view.Message = "内容不能为空";
                return Json(view);
            }
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

            string fileDataJson = Request.Form["fileDataJson"];

            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);
            foreach (var item in fileList)
            {
                item.TableId = model.Id;
                item.TableName = model.GetType().Name;

            }
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateProtocol(model, fileList);
                view = result.ToResultView();
            }
            return Json(view);
        }
        /// <summary>
        /// 启用/禁用状态切换
        /// </summary>
        /// <param name="protocolId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPost]

        public ActionResult EditState(long protocolId, int type)
        {
            Result<int> result = new Result<int>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.AuditProtocol(protocolId, type);
            }
            return Json(result.ToResultView());
        }
        // POST: Protocol/Delete/5
        [AuthCheck(Module = AdminModule.ElectronicAgreement, Right = SystemRight.Delete)]
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteProtocoByIds(list);
            }
            return Json(result.ToResultView());
        }
    }
}
