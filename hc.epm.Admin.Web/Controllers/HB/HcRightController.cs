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
using Newtonsoft.Json;

namespace hc.epm.Admin.Web.Controllers.HB
{
    public class HcRightController : BaseHBController
    {
        /// <summary>
        /// 权限首页
        /// </summary>
        /// <param name="belong"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActionResult Index(string belong, string name = "")
        {
            ViewBag.name = name;
            if (string.IsNullOrEmpty(belong))
            {
                return RedirectToAction("Error", "Home", new { msg = "必须有用户归属" });
            }
            ViewBag.belong = belong;
            ViewBag.belongText = belong.ToEnumReq<RoleType>().GetText();

            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();
            //查询顶级权限
            ce = new ConditionExpression();
            ce.ExpName = "ParentId";
            ce.ExpValue = 0;
            ce.ExpOperater = eConditionOperator.Equal;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);
            //对应归属
            ce = new ConditionExpression();
            ce.ExpName = "Belong";
            ce.ExpValue = belong;
            ce.ExpOperater = eConditionOperator.Equal;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);

            if (!string.IsNullOrEmpty(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Name";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            SortExpression sort = new SortExpression("Sort", eSortType.Asc);
            qc.SortList.Add(sort);
            Result<List<Base_Right>> result = new Result<List<Base_Right>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetRightList(qc);
            }
            return View(result.Data);

        }

        /// <summary>
        /// 全部权限检索
        /// </summary>
        /// <param name="belong"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActionResult Search(string belong, string name = "", int pageIndex = 1, int pageSize = 10)
        {

            ViewBag.name = name;
            if (string.IsNullOrEmpty(belong))
            {
                return RedirectToAction("Error", "Home", new { msg = "必须有用户归属" });
            }
            ViewBag.belong = belong;
            ViewBag.belongText = belong.ToEnumReq<RoleType>().GetText();
            ViewBag.pageIndex = pageIndex;
            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();

            //对应归属
            ce = new ConditionExpression();
            ce.ExpName = "Belong";
            ce.ExpValue = belong;
            ce.ExpOperater = eConditionOperator.Equal;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);

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
            Result<List<Base_Right>> result = new Result<List<Base_Right>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetRightList(qc);
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);

        }

        /// <summary>
        /// 权限树预览
        /// </summary>
        /// <param name="belong"></param>
        /// <param name="pId"></param>
        /// <returns></returns>
        public ActionResult Look(string belong, long pId = 0)
        {
            string files = Request.Form["files"];
            ViewBag.belong = belong;
            ViewBag.pId = pId;
            return View();
        }
        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <param name="belong"></param>
        /// <param name="pId"></param>
        /// <param name="last">是否包含末节</param>
        /// <param name="chk">选中项id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetRightTree(string belong, long pId = 0, bool last = true, string chk = "")
        {
            Result<List<Base_Right>> result = new Result<List<Base_Right>>();
            RoleType roleType = belong.ToEnumReq<RoleType>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.LoadRightList(roleType);
            }
            Base_Right right = null;
            var list = result.Data;
            var be = belong.ToEnumReq<RoleType>();
            switch (be)
            {
                case RoleType.Admin:
                    list.Insert(0, Helper.AdminSite);
                    break;
                //case RoleType.Tenderer:
                //    break;
                //case RoleType.BiddingAgent:
                //    list.Insert(0, Helper.BiddingAgentSite);
                //    break;
                //case RoleType.Bidder:
                //    break;
                //case RoleType.Exp:
                //    break;
                default:
                    right = Helper.WebSite;
                    right.Belong = roleType.ToString();
                    list.Insert(0, right);
                    break;
            }


            var first = list.FirstOrDefault(i => i.Id == pId);
            RightNode rootTree = new RightNode();

            rootTree.checkboxValue = first.Id.ToString();
            rootTree.@checked = chk == first.Id.ToString();
            rootTree.data = new { code = first.Code, id = first.Id.ToString() };
            rootTree.name = first.Name;
            rootTree.spread = true;

            var tree = createTree(first.Id, list);
            if (!last)
            {
                tree = createTreeNoLast(first.Id, list, chk);
            }
            rootTree.children = tree;
            return Json(rootTree, JsonRequestBehavior.AllowGet); ;
        }
        /// <summary>
        /// 生成包含末节的树
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="allList"></param>
        /// <returns></returns>
        private List<RightNode> createTree(long parentId, List<Base_Right> allList)
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
                    node.@checked = false;
                    node.data = new { code = item.Code, id = item.Id.ToString() };
                    node.name = item.Name;
                    node.spread = true;

                    var iteratorList = createTree(item.Id, allList);
                    node.children = iteratorList;
                    list.Add(node);
                }
            }
            return list;
        }

        /// <summary>
        /// 不包含 末节的树
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="allList"></param>
        /// <param name="chk"></param>
        /// <returns></returns>
        private List<RightNode> createTreeNoLast(long parentId, List<Base_Right> allList, string chk = "")
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
                    node.@checked = false;
                    if (item.Id.ToString() == chk)
                    {
                        node.@checked = true;
                    }
                    node.data = new { code = item.Code, id = item.Id.ToString() };
                    node.name = item.Name;
                    node.spread = true;

                    var iteratorList = createTreeNoLast(item.Id, allList, chk);
                    node.children = iteratorList;
                    var ccp = allList.Where(i => i.ParentId == item.Id).ToList();
                    if (ccp.Count() < 1)
                    {
                        continue;
                    }
                    list.Add(node);
                }
            }
            return list;
        }

        /// <summary>
        /// 权限结构
        /// </summary>
        /// <param name="belong"></param>
        /// <param name="leftPId"></param>
        /// <param name="rightPId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ActionResult Child(string belong, long leftPId, long rightPId, string name = "")
        {
            ViewBag.name = name;
            ViewBag.leftPId = leftPId;
            ViewBag.rightPId = rightPId;
            ViewBag.belong = belong;
            var roleType = belong.ToEnumReq<RoleType>();
            ViewBag.belongText = roleType.GetText();
            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();
            //查询对应的子权限
            ce = new ConditionExpression();
            ce.ExpName = "ParentId";
            ce.ExpValue = rightPId;
            ce.ExpOperater = eConditionOperator.Equal;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);
            //对应归属
            ce = new ConditionExpression();
            ce.ExpName = "Belong";
            ce.ExpValue = belong;
            ce.ExpOperater = eConditionOperator.Equal;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);

            if (!string.IsNullOrEmpty(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Name";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            SortExpression sort = new SortExpression("Sort", eSortType.Asc);
            qc.SortList.Add(sort);
            Result<List<Base_Right>> result = new Result<List<Base_Right>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetRightList(qc);
            }
            return View(result.Data);

        }

        /// <summary>
        /// 添加权限页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Add(string belong, long pId)
        {
            Dictionary<string, string> isMenuList = new Dictionary<string, string>();
            isMenuList.Add("False", "否");
            isMenuList.Add("True", "是");

            ViewBag.IsMenu = isMenuList.ToList().ToSelectList("Value", "Key", false);

            ViewBag.Belong = belong;
            ViewBag.pId = pId;

            //当前角色类型
            RoleType rType = belong.ToEnumReq<RoleType>();
            string selectedType = "";
            ViewBag.Code = getSelectName(rType, pId, out selectedType);

            if (rType == RoleType.Admin)
            {
                ViewBag.RightType = Enum<FunctionType>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", false, selectedType);
            }
            else
            {
                ViewBag.RightType = Enum<WebFunctionType>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", false, selectedType);
            }
            return View();
        }
        private SelectList getSelectName(RoleType rType, long pId, out string type)
        {
            SelectList sel = null;
            type = "";
            Result<Dictionary<string, string>> result = new Result<Dictionary<string, string>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetRightUNSelect(rType, pId, out type);

            }
            sel = result.Data.ToList().ToSelectList("Value", "Key", false);
            return sel;
        }

        /// <summary>
        /// 添加权限表单提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(Base_Right model)
        {
            ResultView<string> view = new ResultView<string>();
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "名称不能为空";
                return Json(view);
            }
            Result<int> result = new Result<int>();

            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddRight(model);
            }
            return Json(result.ToResultView());
        }


        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(long id)
        {
            //父级权限，名称，类型不可修改
            Result<Base_Right> result = new Result<Base_Right>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetRightModel(id);
            }
            var model = result.Data;
            Dictionary<string, string> isMenuList = new Dictionary<string, string>();
            isMenuList.Add("False", "否");
            isMenuList.Add("True", "是");
            ViewBag.IsMenu = isMenuList.ToList().ToSelectList("Value", "Key", false, model.IsMenu.ToString());

            RoleType rType = model.Belong.ToEnumReq<RoleType>();
            if (rType == RoleType.Admin)
            {
                ViewBag.RightType = Enum<FunctionType>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", false, model.RightType);
            }
            else
            {
                ViewBag.RightType = Enum<WebFunctionType>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", false, model.RightType);
            }
            //ViewBag.RightType = Enum<FunctionType>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", false, model.RightType);
            return View(model);
        }
        /// <summary>
        /// 修改权限表单提交
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Base_Right model)
        {
            Result<int> result = new Result<int>();

            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                var oldModel = proxy.GetRightModel(model.Id).Data;
                //只允许修改以下项
                oldModel.Sort = model.Sort;
                oldModel.Tips = model.Tips;
                oldModel.IsMenu = model.IsMenu;
                oldModel.URL = model.URL;
                oldModel.CssClass = model.CssClass;
                oldModel.Icon = model.Icon;
                oldModel.Remark = model.Remark;
                oldModel.Target = model.Target;
                oldModel.DisplayName = model.DisplayName;
                result = proxy.UpdateRight(oldModel);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 删除提交
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();

            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                var model = proxy.GetRightModel(list.First());

                result = proxy.DeleteRightbyIds(list);
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
            ResultView<int> view = new ResultView<int>();

            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                var data = proxy.GetRightModel(id);
                var model = data.Data;
                result = proxy.AuditRight(model.Id, type);
                view = result.ToResultView();
            }
            return Json(view);
        }
    }
}
