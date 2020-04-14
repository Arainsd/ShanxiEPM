using hc.epm.Admin.ClientProxy;
using hc.epm.Common;
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
    public class HcConstituteController : BaseController
    {
        /// <summary>
        /// 批复构成列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string mainPoints = "", string projectNatureCode = "", string constituteName = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.MainPoints = mainPoints;
            ViewBag.ConstituteName = constituteName;
            ViewBag.ProjectNatureCode = projectNatureCode;
            ViewBag.pageIndex = pageIndex;
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;

            if (!string.IsNullOrEmpty(mainPoints))
            {
                ce = new ConditionExpression();
                ce.ExpName = "MainPoints";
                ce.ExpValue = "%" + mainPoints + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(constituteName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "ConstituteName";
                ce.ExpValue = "%" + constituteName + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(projectNatureCode))
            {
                ce = new ConditionExpression();
                ce.ExpName = "ProjectNatureCode";
                ce.ExpValue = projectNatureCode;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            qc.SortList.Add(new SortExpression("Sort", eSortType.Asc));

            Result<List<Epm_Constitute>> result = new Result<List<Epm_Constitute>>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetConstituteList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            GetProjectNature();

            return View(result.Data);
        }

        /// <summary>
        /// 添加批复构成
        /// </summary>
        /// <returns></returns>
        public ActionResult Add(int isA)
        {
            if (isA == 1)
            {
                GetAProvide();
            }
            else
            {
                GetConstituteName();
            }
            GetProjectNature();
            GetWorkMainPoints();
            return View();
        }

        /// <summary>
        /// 添加批复构成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(Epm_Constitute model)
        {
            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                model.IsCharging = model.IsCharging.HasValue ? model.IsCharging.Value : false;
   
                result = proxy.AddConstitute(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改批复构成
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(long id, int isA)
        {
            Result<Epm_Constitute> result = new Result<Epm_Constitute>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetConstituteModel(id);
            }
            string projectNature = "";
            string constituteName = "";
            if (result.Data != null)
            {
                projectNature = result.Data.ProjectNatureCode;
                constituteName = result.Data.ConstituteKey;
            }

            if (isA == 1)
            {
                GetAProvide(constituteName);
            }
            else
            {
                GetConstituteName(constituteName);
            }
            GetProjectNature(projectNature);

            return View(result.Data);
        }

        /// <summary>
        /// 修改批复构成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Epm_Constitute model)
        {
            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                model.IsCharging = model.IsCharging.HasValue ? model.IsCharging.Value : false;

                result = proxy.UpdateConstitute(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 删除批复构成
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
                result = proxy.DeleteConstituteByIds(list);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 项目性质下拉数据
        /// </summary>
        public void GetProjectNature(string projectNature = "")
        {
            Result<List<Epm_ProjectNature>> result = new Result<List<Epm_ProjectNature>>();
            QueryCondition qc = new QueryCondition();
            qc.PageInfo.isAllowPage = false;
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProjectNatureList(qc);
            }

            ViewBag.ProjectNatureCode = result.Data.ToSelectList("NATURE_MC", "NATURE_BH", true, projectNature);
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
        /// 总批复及构成下拉数据
        /// </summary>
        private void GetAProvide(string aProvide = "")
        {
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                //返回版本标识列表   
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.AProvide };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                ViewBag.ConstituteKey = subjects[DictionaryType.AProvide].ToSelectList("Name", "No", true, aProvide);
            }
        }

        /// <summary>
        /// 工程内容要点下拉数据
        /// </summary>
        private void GetWorkMainPoints()
        {
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                //返回版本标识列表   
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.WorkMainPoints };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                ViewBag.WorkMainPointsKey = subjects[DictionaryType.WorkMainPoints].ToSelectList("Name", "No", true);
            }
        }
    }
}