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
    public class WorkMainsController : BaseController
    {
        /// <summary>
        /// 批复构成列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string dickey = "", string workmain = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.WorkMain = workmain;
            ViewBag.Dickey = dickey;

            ViewBag.pageIndex = pageIndex;
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            if (!string.IsNullOrEmpty(dickey))
            {
                ce = new ConditionExpression();
                ce.ExpName = "DicKey";
                ce.ExpValue = dickey;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(workmain))
            {
                ce = new ConditionExpression();
                ce.ExpName = "WorkMain";
                ce.ExpValue = "%" + workmain + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            qc.SortList.Add(new SortExpression("DicKey", eSortType.Asc));
            qc.SortList.Add(new SortExpression("Sort", eSortType.Asc));

            Result<List<Epm_WorkMainPoints>> result = new Result<List<Epm_WorkMainPoints>>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetWorkMainPointsList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            GetWorkMainPoints(true);
            return View(result.Data);
        }

        /// <summary>
        /// 添加批复构成
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            GetWorkMainPoints(false);
            return View();
        }

        /// <summary>
        /// 添加批复构成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(Epm_WorkMainPoints model)
        {
            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddWorkMainPoints(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改批复构成
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(long id)
        {
            Result<Epm_WorkMainPoints> result = new Result<Epm_WorkMainPoints>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetWorkMainPointsModel(id);
            }
            GetWorkMainPoints(false, result.Data.DicKey);
            return View(result.Data);
        }

        /// <summary>
        /// 修改批复构成
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Epm_WorkMainPoints model)
        {
            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateWorkMainPoints(model);
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
                result = proxy.DeleteWorkMainPointsByIds(list);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 工程内容要点下拉数据
        /// </summary>
        private void GetWorkMainPoints(bool isDefault, string workMain = "")
        {
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                //返回版本标识列表   
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.WorkMainPoints };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                if (!isDefault && workMain == "")
                {
                    workMain = subjects[DictionaryType.WorkMainPoints][0].No;
                }
                ViewBag.DicKey = subjects[DictionaryType.WorkMainPoints].ToSelectList("Name", "No", isDefault, workMain);
            }
        }
    }
}