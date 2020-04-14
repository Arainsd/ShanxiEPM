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
    public class DepController : BaseController
    {
        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="name"></param>
        /// <param name="isEnable"></param>
        /// <param name="isConfirm"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        // GET: Dep
        [AuthCheck(Module = AdminModule.Dep, Right = SystemRight.Browse)]
        public ActionResult Index(long CompanyId, string name = "", string isEnable = "0", string isConfirm = "0", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.pageIndex = pageIndex;
            ViewBag.companyId = CompanyId;
            ViewBag.Name = name;
            ViewBag.IsConfirm = HelperExt.GetConfirmList(true, isConfirm);
            ViewBag.IsEnable = HelperExt.GetEnableList(true, isEnable);
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


            ce = new ConditionExpression();
            ce.ExpName = "CompanyId";
            ce.ExpValue = CompanyId;
            ce.ExpOperater = eConditionOperator.Equal;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);
            if (isEnable != "0")
            {
                ce = new ConditionExpression();
                ce.ExpName = "IsEnable";
                ce.ExpValue = isEnable == EnumState.Enable.ToString();
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (isConfirm != "0")
            {
                ce = new ConditionExpression();
                ce.ExpName = "IsConfirm";
                ce.ExpValue = isConfirm == EnumState.Confirmed.ToString();
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            Result<List<Base_Dep>> result = new Result<List<Base_Dep>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GeDepList(qc);
                foreach (var item in result.Data)
                {
                    if (string.IsNullOrEmpty(item.PreName))
                    {
                        item.PreName = "无上级部门";
                    }
                }
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }
        /// <summary>
        /// 新增部门
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.Dep, Right = SystemRight.Add)]
        public ActionResult Add(long CompanyId)
        {
            ViewBag.CompanyId = CompanyId;
            ViewBag.IsConfirm = HelperExt.GetConfirmList(false);
            ViewBag.IsEnable = HelperExt.GetEnableList(false);
            Result<List<Base_Dep>> result = new Result<List<Base_Dep>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GeDepListByCompanyId(CompanyId);
                ViewBag.PreId = result.Data.ToSelectList("Name", "Id", true);
            }
            return View();
        }

        [AuthCheck(Module = AdminModule.Dep, Right = SystemRight.Add)]
        [HttpPost]
        public ActionResult Add(Base_Dep model)
        {
            ResultView<int> view = new ResultView<int>();
            //表单校验
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "部门名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Code))
            {
                view.Flag = false;
                view.Message = "部门编号不能为空";
                return Json(view);
            }
            if (Request.Form["IsEnable"] == EnumState.Enable.ToString())
            {
                model.IsEnable = true;
            }
            else
            {
                model.IsEnable = false;
            }
            if (Request.Form["IsConfirm"] == EnumState.Confirmed.ToString())
            {
                model.IsConfirm = true;
            }
            else
            {
                model.IsConfirm = false;
            }
            ViewBag.CompanyId = model.CompanyId;
            Result<Base_Dep> preresult = new Result<Base_Dep>();
            if (model.PreId != "0")
            {
                using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
                {
                    preresult = proxy.GeDepModel(long.Parse(model.PreId));
                }
                model.PreName = preresult.Data.Name;
                model.PreCode = preresult.Data.Code;
            }
            Result<int> result = new Result<int>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddDep(model);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 改变启用禁用状态
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditState(long id, int type)
        {
            Result<int> result = new Result<int>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.AuditDep(id, type);
            }
            return Json(result);
        }
        /// <summary>
        /// 修改部门
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.Dep, Right = SystemRight.Modify)]
        // GET: Dep/Edit/5
        public ActionResult Edit(long id)
        {
            Result<Base_Dep> result = new Result<Base_Dep>();

            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GeDepModel(id);
            }
            var isConfirm = result.Data.IsConfirm ? EnumState.Confirmed.ToString() : EnumState.NoConfim.ToString();
            var isEnable = result.Data.IsEnable ? EnumState.Enable.ToString() : EnumState.Disable.ToString();
            ViewBag.IsEnable = HelperExt.GetEnableList(false, isEnable);
            ViewBag.IsConfirm = HelperExt.GetConfirmList(false, isConfirm);
            var companyId = result.Data.CompanyId;
            Result<List<Base_Dep>> resultDep = new Result<List<Base_Dep>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                resultDep = proxy.GeDepListByCompanyId(companyId);
                var depList = resultDep.Data;
                if (result.Data.PreId == "0")
                {
                    ViewBag.PreId = depList.ToSelectList("Name", "Id", true);
                }
                else
                {
                    ViewBag.PreId = depList.ToSelectList("Name", "Id", false, result.Data.PreId);
                }
            }
            return View(result.Data);
        }


        [AuthCheck(Module = AdminModule.Dep, Right = SystemRight.Modify)]
        [HttpPost]
        public ActionResult Edit(Base_Dep model)
        {
            Result<int> result = new Result<int>();
            ResultView<int> view = new ResultView<int>();
            //表单校验
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "部门名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Code))
            {
                view.Flag = false;
                view.Message = "部门编号不能为空";
                return Json(view);
            }
            if (Request.Form["IsEnable"] == EnumState.Enable.ToString())
            {
                model.IsEnable = true;
            }
            else
            {
                model.IsEnable = false;
            }
            if (Request.Form["IsConfirm"] == EnumState.Confirmed.ToString())
            {
                model.IsConfirm = true;
            }
            else
            {
                model.IsConfirm = false;
            }
            if (model.PreId != "0")
            {//判断是否存在上级部门 若存在获取上级部门信息将上级部门编号及名称更新
                Result<Base_Dep> preresult = new Result<Base_Dep>();
                using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
                {
                    preresult = proxy.GeDepModel(long.Parse(model.PreId));
                }
                model.PreName = preresult.Data.Name;
                model.PreCode = preresult.Data.Code;
            }

            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateDep(model);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 获取当前企业详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CompanyInfo(long id)
        {
            Result<Base_Company> result = new Result<Base_Company>();

            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompanyModel(id);

            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 批量删除部门
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.Dep, Right = SystemRight.Delete)]
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteDepByIds(list);
            }
            return Json(result.ToResultView());
        }
    }
}
