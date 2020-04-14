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
    public class RegionController : BaseController
    {
        public ActionResult Index(string name = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;


            ViewBag.pageIndex = pageIndex;
            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();
            if (!string.IsNullOrEmpty(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Fullname";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            //SortExpression sort = new SortExpression("RegionCode", eSortType.Asc);
            //qc.SortList.Add(sort);
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            qc.PageInfo.OrderAndSortList = "RegionCode:ASC";
            Result<List<Base_Region>> result = new Result<List<Base_Region>>();
            List<RegionView> list = new List<RegionView>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetRegionList(qc);
                if (result.AllRowsCount > 0)
                {
                    var regionAll = proxy.LoadRegionList().Data;
                    foreach (var item in result.Data)
                    {
                        RegionView view = new RegionView();
                        view.Area = GetProvince(regionAll, item).AreaName;
                        view.Province = GetProvince(regionAll, item).RegionName;
                        view.City = GetCity(regionAll, item);
                        view.Code = item.RegionCode;
                        view.FullName = item.Fullname;
                        view.Region = GetRegion(regionAll, item);
                        list.Add(view);
                    }
                }
                ViewBag.Total = result.AllRowsCount;
            }
            return View(list);

        }

        private Base_Region GetProvince(List<Base_Region> regionAll, Base_Region model)
        {
            if (model.ParentCode == "0")
            {
                return model;
            }
            var parent = regionAll.FirstOrDefault(i => i.RegionCode == model.ParentCode);
            if (parent.ParentCode != "0")
            {
                parent = GetProvince(regionAll, parent);
            }
            return parent;
        }

        private string GetCity(List<Base_Region> regionAll, Base_Region model)
        {
            if (model.ParentCode == "0")
            {
                return "";
            }
            var parent = regionAll.FirstOrDefault(i => i.RegionCode == model.ParentCode);
            if (parent.ParentCode == "0")
            {
                return model.RegionName;
            }
            return parent.RegionName;
        }

        private string GetRegion(List<Base_Region> regionAll, Base_Region model)
        {
            if (model.ParentCode == "0")
            {
                return "";
            }
            var parent = regionAll.FirstOrDefault(i => i.RegionCode == model.ParentCode);
            if (parent.ParentCode == "0")
            {
                return "";
            }
            return model.RegionName;
        }

    }
}
