using hc.epm.Admin.ClientProxy;
using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
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
    public class AdTargetController : BaseController
    {
        /// <summary>
        /// 广告位列表
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.AdTarget, Right = SystemRight.Browse)]
        public ActionResult Index(string name = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.pageIndex = pageIndex;

            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;

            if (!string.IsNullOrEmpty(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Name";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<Epm_AdTarget>> result = new Result<List<Epm_AdTarget>>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetAdTargetListWhr(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 添加广告位
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.AdTarget, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// 添加广告位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.AdTarget, Right = SystemRight.Add)]
        [HttpPost]
        public ActionResult Add(Epm_AdTarget model)
        {
            ResultView<int> view = new ResultView<int>();

            //表单校验
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "广告位名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.TargetNum))
            {
                view.Flag = false;
                view.Message = "广告位编号不能为空";
                return Json(view);
            }
            if (model.ShowCount <= 0)
            {
                view.Flag = false;
                view.Message = "显示数量不能为零";
                return Json(view);
            }
            if (model.Hight <= 0)
            {
                view.Flag = false;
                view.Message = "高度不能为零";
                return Json(view);
            }
            if (model.Lenght <= 0)
            {
                view.Flag = false;
                view.Message = "长度不能为零";
                return Json(view);
            }
            if (Convert.ToBoolean(Request.Form["State"]) == true)
            {
                model.State = 1;
            }
            else
            {
                model.State = 0;
            }
            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddAdTarget(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改广告位
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.AdTarget, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<Epm_AdTarget> result = new Result<Epm_AdTarget>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetAdTargetById(id);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 修改广告位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.AdTarget, Right = SystemRight.Modify)]
        [HttpPost]
        public ActionResult Edit(Epm_AdTarget model)
        {
            Result<int> result = new Result<int>();
            ResultView<string> view = new ResultView<string>();
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "广告位名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.TargetNum))
            {
                view.Flag = false;
                view.Message = "广告位编号不能为空";
                return Json(view);
            }
            if (model.ShowCount <= 0)
            {
                view.Flag = false;
                view.Message = "显示数量不能为零";
                return Json(view);
            }
            if (model.Hight <= 0)
            {
                view.Flag = false;
                view.Message = "高度不能为零";
                return Json(view);
            }
            if (model.Lenght <= 0)
            {
                view.Flag = false;
                view.Message = "长度不能为零";
                return Json(view);
            }
            if (Convert.ToBoolean(Request.Form["State"]) == true)
            {
                model.State = 1;
            }
            else
            {
                model.State = 0;
            }
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateAdTarget(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 删除广告位
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.AdTarget, Right = SystemRight.Delete)]
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteAdTargetByIds(list);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改广告位状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.AdTarget, Right = SystemRight.Enable)]
        [HttpPost]
        public ActionResult ChangeState(long id, int state)
        {
            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangeAdTargetState(id, state);
            }
            return Json(result.ToResultView());
        }
    }
}