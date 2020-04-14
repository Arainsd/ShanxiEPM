using hc.epm.Common;
using hc.epm.Admin.ClientProxy;
using hc.epm.UI.Common;
using hc.epm.DataModel.Basic;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Admin.Web.Controllers
{
    public class SettingsController : BaseController
    {
        /// <summary>
        /// 系统设置列表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.SystemParameter, Right = SystemRight.Browse)]
        public ActionResult Index(string name = "")
        {
            ViewBag.name = name;
            Result<List<Base_Settings>> result = new Result<List<Base_Settings>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.LoadSettings();
            }
            var list = result.Data;
            if (name != "")
            {
                list = list.Where(i => i.Name.Contains(name)).ToList();
            }
            return View(list);

        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.SystemParameter, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// 添加处理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = AdminModule.SystemParameter, Right = SystemRight.Add)]
        public ActionResult Add(Base_Settings model)
        {
            ResultView<int> view = new ResultView<int>();
            if (string.IsNullOrEmpty(model.Code))
            {
                view.Flag = false;
                view.Message = "编码不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Value))
            {
                view.Flag = false;
                view.Message = "值不能为空";
                return Json(view);
            }

            Result<int> result = new Result<int>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddSettings(model);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.SystemParameter, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<List<Base_Settings>> result = new Result<List<Base_Settings>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.LoadSettings();
            }
            return View(result.ToResultView().Data.FirstOrDefault(i => i.Id == id));
        }

        /// <summary>
        /// 编辑处理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.SystemParameter, Right = SystemRight.Modify)]
        [HttpPost]
        public ActionResult Edit(Base_Settings model)
        {
            ResultView<int> view = new ResultView<int>();
            if (string.IsNullOrEmpty(model.Code))
            {
                view.Flag = false;
                view.Message = "编码不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Value))
            {
                view.Flag = false;
                view.Message = "值不能为空";
                return Json(view);
            }

            Result<int> result = new Result<int>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateSettings(model);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.SystemParameter, Right = SystemRight.Delete)]
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteSettingsByIds(list);
            }
            return Json(result.ToResultView());
        }
    }
}
