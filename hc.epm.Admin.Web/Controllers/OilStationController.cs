using hc.epm.Admin.ClientProxy;
using hc.epm.Common;
using hc.epm.DataModel.Basic;
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
    public class OilStationController : BaseController
    {
        public ActionResult Index(string name = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.pageIndex = pageIndex;

            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = new ConditionExpression();
            ce.ExpName = "Name";
            ce.ExpValue = "%" + name + "%";
            ce.ExpOperater = eConditionOperator.Like;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<Epm_OilStation>> result = new Result<List<Epm_OilStation>>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetOilStationList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }
        //查看
        public ActionResult Detail(long id)
        {
            Result<Epm_OilStation> result = new Result<Epm_OilStation>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetOilStationById(id);
            }
            return View(result.Data);
        }
        public ActionResult Add()
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminOrganization.ToString(), SystemRight.Add.ToString(), true);
            return View();
        }
        /// <summary>
        /// 新增加油站信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(Epm_OilStation mode)
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminOrganization.ToString(), SystemRight.Add.ToString(), true);
            Result<int> result = new Result<int>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddOilStation(mode);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改加油站信息
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(long id)
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminOrganization.ToString(), SystemRight.Modify.ToString(), true);

            Result<Epm_OilStation> result = new Result<Epm_OilStation>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetOilStation(id);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 修改加油站
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Epm_OilStation model)
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminOrganization.ToString(), SystemRight.Modify.ToString(), true);

            Result<int> result = new Result<int>();
            ResultView<string> view = new ResultView<string>();
            if (string.IsNullOrEmpty(model.Code1))
            {
                view.Flag = false;
                view.Message = "单位编码不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Code))
            {
                view.Flag = false;
                view.Message = "编号不能为空";
                return Json(view);
            }

            Result<Epm_OilStation> OilStationResult = new Result<Epm_OilStation>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                OilStationResult = proxy.GetOilStation(model.Id);
                OilStationResult.Data.Code = model.Code;
                OilStationResult.Data.Code1 = model.Code1;
                OilStationResult.Data.Name = model.Name;
                OilStationResult.Data.Description = model.Description;
                OilStationResult.Data.Address = model.Address;
                result = proxy.UpdateOilStation(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 删除加油站
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="belong"></param>
        /// <returns></returns>      
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminOrganization.ToString(), SystemRight.Delete.ToString(), true);

            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteOilStation(list);
            }
            return Json(result.ToResultView());
        }


    }
}