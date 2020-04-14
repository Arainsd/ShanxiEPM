using hc.epm.Admin.ClientProxy;
using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Admin.Web.Controllers.HB
{
    public class HcOilStationController : BaseHBController
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
    }
}