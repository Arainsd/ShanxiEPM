using hc.epm.Admin.ClientProxy;
using hc.epm.Admin.Web.Models;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Admin.Web.Controllers
{
    public class CommonController : BaseController
    {
        public ActionResult SelectCompany(string IsMultiple = "", string name = "", int pageIndex = 1, int pageSize = 8)
        {
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

            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                //result = proxy.GetCompanyList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            ViewBag.IsMultiple = IsMultiple;
            ViewBag.pageIndex = pageIndex;

            return View(result.Data);
        }

        public ActionResult SelectProject(string IsMultiple = "true", string name = "", int pageIndex = 1, int pageSize = 8)
        {
            ViewBag.Name = name;
            ViewBag.pageIndex = pageIndex;
            ViewBag.IsMultiple = IsMultiple;
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

            var result = new Result<List<Epm_Project>>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                //result = proxy.GetProjectList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        public ActionResult SelectUser(string IsMultiple = "true", string phone = "", string name = "", int pageIndex = 1, int pageSize = 8)
        {
            ViewBag.Name = name;
            ViewBag.pageIndex = pageIndex;
            ViewBag.IsMultiple = IsMultiple;
            ViewBag.Phone = phone;
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            if (!string.IsNullOrEmpty(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "UserName";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(phone))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Phone";
                ce.ExpValue = "%" + phone + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            var result = new Result<List<Base_User>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                //result = proxy.GetUserList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        public ActionResult SelectUsers(long companyId, string name, int pageIndex = 1, int pageSize = 8)
        {
            ViewBag.CompanyId = companyId;
            ViewBag.Name = name;
            ViewBag.pageIndex = pageIndex;
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            if (!string.IsNullOrEmpty(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "UserName";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (companyId > 0)
            {
                ce = new ConditionExpression();
                ce.ExpName = "CompanyId";
                ce.ExpValue = companyId;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            var result = new Result<List<Base_User>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                //result = proxy.GetUserList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        public ActionResult hb()
        {
            return View();
        }
    }
}