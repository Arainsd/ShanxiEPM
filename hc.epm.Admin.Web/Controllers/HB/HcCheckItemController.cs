using hc.epm.Admin.ClientProxy;
using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
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
    public class HcCheckItemController : BaseController
    {
        /// <summary>
        /// 检查项列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string name = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.Name = name;
            ViewBag.pageIndex = pageIndex;

            Result<List<Epm_CheckItem>> result = new Result<List<Epm_CheckItem>>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                QueryCondition qc = new QueryCondition();
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Level",
                    ExpValue = 2,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });
                qc.PageInfo.isAllowPage = false;
                qc.SortList.Add(new SortExpression("Sort", eSortType.Asc));
                List<Epm_CheckItem> categoryList = proxy.GetCheckItemList(qc).Data;

                qc = new QueryCondition();
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Level",
                    ExpValue = 3,
                    ExpOperater = eConditionOperator.Equal,
                    ExpLogical = eLogicalOperator.And
                });
                if (!string.IsNullOrEmpty(name))
                {
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "Name",
                        ExpValue = "%" + name + "%",
                        ExpOperater = eConditionOperator.Like,
                        ExpLogical = eLogicalOperator.And
                    });
                }
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
                qc.SortList.Add(new SortExpression("Sort", eSortType.Asc));
                result = proxy.GetCheckItemList(qc);
                foreach (var item in result.Data)
                {
                    item.Remark = categoryList.Where(p => p.Id == item.ParentId).First().ParentName;
                }
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 增加检查项
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// 增加检查项
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(Epm_CheckItem model)
        {
            ResultView<int> view = new ResultView<int>();
            //表单校验
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "检查名称不能为空";
                return Json(view);
            }
            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddCheckItem(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改检查项
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(long id)
        {
            Result<Epm_CheckItem> result = new Result<Epm_CheckItem>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCheckItemModel(id);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 修改检查项
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Epm_CheckItem model)
        {
            ResultView<int> view = new ResultView<int>();
            //表单校验
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "检查名称不能为空";
                return Json(view);
            }
            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateCheckItem(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 删除检查项
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
                result = proxy.DeleteCheckItemByIds(list);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 选择所属父级
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectParent()
        {
            return View();
        }

        #region  加载父级菜单方法
        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="last">是否包含末节</param>
        /// <param name="chk">选中项id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCheckItemTree(long pId = 0, bool last = true, string chk = "")
        {
            Result<List<Epm_CheckItem>> result = new Result<List<Epm_CheckItem>>();
            QueryCondition qc = new QueryCondition();
            qc.PageInfo.isAllowPage = false;
            ConditionExpression ce = new ConditionExpression();
            ce.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "Level",
                ExpValue = 1,
                ExpOperater = eConditionOperator.Equal,
                ExpLogical = eLogicalOperator.Or
            });
            ce.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "Level",
                ExpValue = 2,
                ExpOperater = eConditionOperator.Equal,
                ExpLogical = eLogicalOperator.Or
            });
            qc.ConditionList.Add(ce);
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCheckItemList(qc);
            }
            var list = result.Data;

            list.Insert(0, new Epm_CheckItem() { Id = 0, ParentId = -1, Name = "根节点", Sort = -1, Level = 0 });

            var first = list.FirstOrDefault(i => i.Id == pId);
            RightNode rootTree = new RightNode();

            rootTree.checkboxValue = first.Id.ToString();
            rootTree.@checked = chk == first.Id.ToString();
            rootTree.data = new { id = first.Id.ToString() };
            rootTree.name = first.Name;
            rootTree.spread = true;
            rootTree.Level = first.Level;

            var tree = createTree(first.Id, list);
            if (!last)
            {
                tree = createTreeNoLast(first.Id, list, chk);
            }
            rootTree.children = tree;
            return Json(rootTree, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 生成包含末节的树
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="allList"></param>
        /// <returns></returns>
        private List<RightNode> createTree(long parentId, List<Epm_CheckItem> allList)
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
                    node.data = new { id = item.Id.ToString() };
                    node.name = item.Name;
                    node.spread = true;
                    node.Level = item.Level;

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
        private List<RightNode> createTreeNoLast(long parentId, List<Epm_CheckItem> allList, string chk = "")
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
                    node.data = new { id = item.Id.ToString() };
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

        #endregion
    }
}