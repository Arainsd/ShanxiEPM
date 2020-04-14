using hc.epm.Admin.ClientProxy;
using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Admin.Web.Controllers
{
    public class ISPController : BaseController
    {
        // GET: ISP
        /// <summary>
        /// 工程服务商
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string constituteName = "", string CompanyNames = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.ConstituteName = constituteName;
            ViewBag.CompanyNames = CompanyNames;
            ViewBag.pageIndex = pageIndex;
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
         
            if (!string.IsNullOrEmpty(constituteName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "ConstituteName";
                ce.ExpValue = "%" + constituteName + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(CompanyNames))
            {
                ce = new ConditionExpression();
                ce.ExpName = "CompanyNames";
                ce.ExpValue = "%" + CompanyNames + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<Epm_ConstituteCompany>> result = new Result<List<Epm_ConstituteCompany>>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetConstituteCompanyList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            GetConstituteName();
            return View();
        }
        /// <summary>
        /// 添加工程服务商
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(Epm_ConstituteCompany model)
        {
            ConstituteCompanyView cv = new ConstituteCompanyView();
            ResultView<int> view = new ResultView<int>();

            string ISPValue = Request.Form["ISPValue"];
            cv.ConstituteCompanyDetails = JsonConvert.DeserializeObject<List<Epm_ConstituteCompanyDetails>>(ISPValue);
            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                cv.ConstituteCompany = model;
                result = proxy.AddConstituteCompany(cv);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 修改服务商
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(long id)
        {
            Result<ConstituteCompanyView> result = new Result<ConstituteCompanyView>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetConstituteCompanyModel(id);
            }

            string constituteName = "";
            if (result.Data != null)
            {
                constituteName = result.Data.ConstituteCompany.ConstituteKey;
            }
            GetConstituteName(constituteName);
            //GetWorkMainPoints();
            
            return View(result.Data);
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Epm_ConstituteCompany model)
        {
            ConstituteCompanyView cv = new ConstituteCompanyView();
            ResultView<int> view = new ResultView<int>();
            string ISPValue = Request.Form["ISPValue"];
           if (!string.IsNullOrWhiteSpace(ISPValue))
            { 
                cv.ConstituteCompanyDetails = JsonConvert.DeserializeObject<List<Epm_ConstituteCompanyDetails>>(ISPValue);
            }

            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                cv.ConstituteCompany = model;
                result = proxy.UpdateConstituteCompany(cv);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteConstituteCompanyByIds(list);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 总批复及构成下拉数据
        /// </summary>
        private void GetConstituteName(string constituteName = "")
        {
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                //返回版本标识列表   
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.Constitute };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                ViewBag.ConstituteKey = subjects[DictionaryType.Constitute].ToSelectList("Name", "No", true, constituteName);
            }
        }

        /// <summary>
        /// 选择服务商（All）
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectServiceCustomer(string name = "", string selectType = "1", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.selectType = selectType;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            QueryCondition qc = new QueryCondition();
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "Type",
                ExpValue = "Supplier",
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            if (!string.IsNullOrEmpty(name))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Name",
                    ExpValue = string.Format("%{0}%", name),
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            qc.SortList.Add(new SortExpression("Code", eSortType.Asc));

            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompanyList(qc);

                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }
    }
}