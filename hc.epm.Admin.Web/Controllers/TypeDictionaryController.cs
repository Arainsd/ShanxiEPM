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

namespace hc.epm.Admin.Web.Controllers
{
    [AuthCheck(Module = AdminModule.TypeDictionary, Right = SystemRight.Browse)]
    public class TypeDictionaryController : BaseController
    {
        /// <summary>
        /// 类型管理
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="isEnable"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        // GET: TypeDictionary
        public ActionResult Index(string name = "", string Type = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.pageIndex = pageIndex;
            ViewBag.Type = Enum<DictionaryType>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", true, Type);

            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();

            if (!string.IsNullOrEmpty(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Name";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (Type != "")
            {
                ce = new ConditionExpression();
                ce.ExpName = "Type";
                ce.ExpValue = Type;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            qc.SortList.Add(new SortExpression("Type", eSortType.Asc));
            qc.SortList.Add(new SortExpression("No", eSortType.Asc));
            Result<List<Base_TypeDictionary>> result = new Result<List<Base_TypeDictionary>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetTypeList(qc);

                ViewBag.Total = result.AllRowsCount;
            }
            return View(result.Data);
        }

        [AuthCheck(Module = AdminModule.TypeDictionary, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            ViewBag.Type = Enum<DictionaryType>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", false);
            return View();
        }

        /// <summary>
        /// 新增类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST: TypeDictionary/Create
        [HttpPost]
        [AuthCheck(Module = AdminModule.TypeDictionary, Right = SystemRight.Add)]
        public ActionResult Add(Base_TypeDictionary model)
        {
            ResultView<int> view = new ResultView<int>();
            //表单验证
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.No))
            {
                view.Flag = false;
                view.Message = "编号不能为空";
                return Json(view);
            }
            Result<int> result = new Result<int>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddType(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 根据id获取当前类型的所属分类，启用禁用状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: TypeDictionary/Edit/5
        [AuthCheck(Module = AdminModule.TypeDictionary, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<Base_TypeDictionary> result = new Result<Base_TypeDictionary>();

            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetTypeModel(id);
            }
            ViewBag.Type = Enum<DictionaryType>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", false, result.Data.Type);
            return View(result.Data);
        }

        ///// <summary>
        ///// 根据分类获取父级类型
        ///// </summary>
        ///// <param name="roleType"></param>
        ///// <returns></returns>
        //[HttpGet]
        //public ActionResult GetParentType(string roleType)
        //{
        //    Result<List<Base_TypeDictionary>> result = new Result<List<Base_TypeDictionary>>();
        //    DictionaryType type = roleType.ToEnumReq<DictionaryType>();
        //    using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
        //    {
        //        result = proxy.GetTypeListByType(type);
        //    }
        //    var list = result.ToResultView();
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// 类型修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST: TypeDictionary/Edit/5
        [AuthCheck(Module = AdminModule.TypeDictionary, Right = SystemRight.Modify)]
        [HttpPost]
        public ActionResult Edit(Base_TypeDictionary model)
        {
            Result<int> result = new Result<int>();
            ResultView<int> view = new ResultView<int>();
            //表单验证
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.No))
            {
                view.Flag = false;
                view.Message = "编号不能为空";
                return Json(view);
            }
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateType(model);
            }
            return Json(result.ToResultView());
        }

        ///// <summary>
        ///// 改变启用禁用状态
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult EditState(long id, int type)
        //{
        //    Result<int> result = new Result<int>();
        //    using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
        //    {
        //        result = proxy.AuditType(id, type);
        //    }
        //    return Json(result);
        //}

        /// <summary>
        /// 批量删除类型
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = AdminModule.TypeDictionary, Right = SystemRight.Delete)]
        public ActionResult Delete(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteTypeByIds(list);
            }
            return Json(result.ToResultView());
        }
    }
}

