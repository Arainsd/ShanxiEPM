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
    public class RoleController : BaseController
    {
        public ActionResult Index(string RoleName = "", string IsEnable = "0", string IsConfirm = "0", int pageIndex = 1, int pageSize = 10)
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminRole.ToString(), SystemRight.Browse.ToString(), true);

            ViewBag.RoleName = RoleName;
            ViewBag.pageIndex = pageIndex;
            ViewBag.IsConfirm = HelperExt.GetConfirmList(true, IsConfirm);
            ViewBag.IsEnable = HelperExt.GetEnableList(true, IsEnable);
            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();
            ce = new ConditionExpression();

            if (!string.IsNullOrEmpty(RoleName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "RoleName";
                ce.ExpValue = "%" + RoleName + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (IsEnable != "0")
            {
                ce = new ConditionExpression();
                ce.ExpName = "IsEnable";
                ce.ExpValue = IsEnable == EnumState.Enable.ToString();
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (IsConfirm != "0")
            {
                ce = new ConditionExpression();
                ce.ExpName = "IsConfirm";
                ce.ExpValue = IsConfirm == EnumState.Confirmed.ToString();
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            Result<List<Base_Role>> result = new Result<List<Base_Role>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetRoleList(qc);

                foreach (var item in result.Data)
                {
                    item.Remark = item.Remark.CutByByteLength(200, "...");
                    item.RoleName = item.RoleName.CutByByteLength(15, "...");
                }

                ViewBag.Total = result.AllRowsCount;

            }
            return View(result.Data);
        }

        public ActionResult Add(string Belong)
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminRole.ToString(), SystemRight.Add.ToString(), true);
            ViewBag.IsConfirm = HelperExt.GetConfirmList(false);
            ViewBag.IsEnable = HelperExt.GetEnableList(false);
            return View();
        }

        [HttpPost]
        public ActionResult Add(Base_Role model)
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminRole.ToString(), SystemRight.Add.ToString(), true);

            ResultView<int> view = new ResultView<int>();
            ResultView<bool> checkRight = new ResultView<bool>();
            if (string.IsNullOrEmpty(model.RoleName))
            {
                view.Flag = false;
                view.Message = "角色名不能为空";
                return Json(view);
            }

            model.Belong = RoleType.Owner.ToString();

            Result<int> result = new Result<int>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddRole(model);
                view = result.ToResultView();
            }
            return Json(view);
        }

        public ActionResult Edit(long id)
        {
            Helper.IsCheck(HttpContext, AdminModule.AdminRole.ToString(), SystemRight.Modify.ToString(), true);
            Result<Base_Role> result = new Result<Base_Role>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetRoleModel(id);

            }
            var model = result.Data;
            return View(result.Data);
        }

        [HttpPost]
        public ActionResult Edit(Base_Role model)
        {
            Helper.IsCheck(HttpContext, AdminModule.AdminRole.ToString(), SystemRight.Modify.ToString());

            ResultView<int> view = new ResultView<int>();
            ResultView<bool> checkRight = new ResultView<bool>();
            if (string.IsNullOrEmpty(model.RoleName))
            {
                view.Flag = false;
                view.Message = "角色名不能为空";
                return Json(view);
            }

            model.Belong = RoleType.Owner.ToString();

            Result<int> result = new Result<int>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateRole(model);
                view = result.ToResultView();
            }
            return Json(view);
        }

        [HttpPost]
        public ActionResult EditState(long roleId, int type)
        {
            Helper.IsCheck(HttpContext, AdminModule.AdminRole.ToString(), SystemRight.Modify.ToString());

            Result<int> result = new Result<int>();
            ResultView<bool> checkRight = new ResultView<bool>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                if (checkRight.Flag)
                {
                    result = proxy.AuditRole(roleId, type);
                }
                else {
                    return Json(checkRight);
                }

            }
            return Json(result.ToResultView());
        }

        [HttpPost]
        public ActionResult Delete(string ids, string belong)
        {
            Helper.IsCheck(HttpContext, AdminModule.AdminRole.ToString(), SystemRight.Delete.ToString());

            Result<int> result = new Result<int>();
            ResultView<bool> checkRight = new ResultView<bool>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                var model = proxy.GetRoleModel(list.First());
                result = proxy.DeleteRoleByIds(list);
            }

            return Json(result.ToResultView());
        }

        public ActionResult Set(string id, string Belong)
        {
            ViewBag.Belong = Belong;
            ViewBag.id = id;
            return View();
        }

        [HttpPost]
        public ActionResult Set(string belong)
        {
            Helper.IsCheck(HttpContext, AdminModule.AdminRole.ToString(), SystemRight.SetRight.ToString());

            long id = Request.Form["id"].ToLongReq();
            List<long> rightIds = Request.Form["rights"].SplitString(",").ToLongList();
            Result<int> result = new Result<int>();
            ResultView<bool> checkRight = new ResultView<bool>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.SetRoleRight(id, rightIds);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <param name="belong">归属</param>
        /// <param name="roleId">角色</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetRightTree(string belong, long roleId)
        {
            Result<List<Base_Right>> result = new Result<List<Base_Right>>();
            RoleType roleType = belong.ToEnumReq<RoleType>();

            Result<List<Base_RoleRight>> roleRight = new Result<List<Base_RoleRight>>();
            List<long> roleRightIds = new List<long>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.LoadRightList(roleType);
                //获取角色的所有权限
                roleRight = proxy.GetRightByRoleIds(new List<long>() { roleId });
                if (roleRight.Data != null && roleRight.Data.Any())
                {
                    roleRightIds = roleRight.Data.Select(i => i.RightId).ToList();
                }

            }
            Base_Right right = null;
            var list = result.Data;
            switch (roleType)
            {
                case RoleType.Admin://管理员
                    list.Insert(0, Helper.AdminSite);
                    break;
                default://业务应用站点，包括（业主，监理，施工，咨询，设计）
                    right = Helper.WebSite;
                    right.Belong = roleType.ToString();
                    list.Insert(0, right);
                    break;
            }


            var first = list.FirstOrDefault(i => i.Id == 0);
            RightNode rootTree = new RightNode();

            rootTree.checkboxValue = first.Id.ToString();
            rootTree.@checked = roleRightIds.Count() > 0;
            rootTree.data = new { code = first.Code, id = first.Id.ToString() };
            rootTree.name = first.ShortName ?? first.Name;
            rootTree.spread = true;

            var tree = createTree(first.Id, list, roleRightIds);

            rootTree.children = tree;
            return Json(rootTree, JsonRequestBehavior.AllowGet); ;
        }
        /// <summary>
        /// 生成包含末节的树
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="allList"></param>
        /// <returns></returns>
        private List<RightNode> createTree(long parentId, List<Base_Right> allList, List<long> roleRightIds)
        {
            List<RightNode> list = new List<RightNode>();
            var childList = allList.Where(i => i.ParentId == parentId).ToList();
            //有子权限
            if (childList != null && childList.Any())
            {
                foreach (var item in childList)
                {
                    RightNode node = new RightNode();
                    node.checkboxValue = item.Id.ToString();
                    node.@checked = roleRightIds.Contains(item.Id);
                    node.data = new { code = item.Code, id = item.Id.ToString() };
                    node.name = item.ShortName ?? item.Name;
                    node.spread = true;

                    var iteratorList = createTree(item.Id, allList, roleRightIds);
                    node.children = iteratorList;
                    list.Add(node);
                }
            }
            return list;
        }
    }
}
