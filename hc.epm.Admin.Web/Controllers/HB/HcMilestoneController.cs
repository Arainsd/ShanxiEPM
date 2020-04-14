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
    public class HcMilestoneController : BaseController
    {
        public ActionResult Index(long parentId = 0, int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.parentId = parentId;
            Result<List<MilepostView>> result = new Result<List<MilepostView>>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                QueryCondition qc = new QueryCondition();
                ConditionExpression ce = null;
                ce = new ConditionExpression();
                ce.ExpName = "ParentId";
                ce.ExpValue = 0;
                ce.ExpOperater = eConditionOperator.Equal;
                qc.ConditionList.Add(ce);
                qc.PageInfo.isAllowPage = false;
                Result<List<Epm_Milepost>> data = proxy.GetMilepostListQc(qc);
                ViewBag.parentId = data.Data.ToSelectList("Name", "Id", true, parentId.ToString());

                result = proxy.GetMilepostViewList(parentId, pageIndex, pageSize);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.pageIndex = pageIndex;
            }
            return View(result.Data);
        }
        
        /// <summary>
        /// 添加里程碑
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// 添加里程碑
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dataConfigId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(Epm_Milepost model, string dataConfigId)
        {
            ResultView<int> view = new ResultView<int>();
            //表单校验
            if (string.IsNullOrEmpty(model.ParentId.ToString()))
            {
                view.Flag = false;
                view.Message = "项目性质不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "名称不能为空";
                return Json(view);
            }
            if (Convert.ToBoolean(Request.Form["State"]) == true)
            {
                model.State = 1;
            }
            else
            {
                model.State = 0;
            }
            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                List<long> list = dataConfigId.SplitString(",").ToLongList();
                result = proxy.AddMilepostAndData(model, list);
            }
            return Json(result.ToResultView());
        }


        //选择所属父级
        public ActionResult SelectParent()
        {
            return View();
        }

        #region  加载父级菜单方法
        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <param name="belong"></param>
        /// <param name="pId"></param>
        /// <param name="last">是否包含末节</param>
        /// <param name="chk">选中项id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetMilestoneTree(long pId = 0, bool last = true, string chk = "")
        {
            Result<List<Epm_Milepost>> result = new Result<List<Epm_Milepost>>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetMilepostList();
            }
            var list = result.Data;
            list = list.Where(t => t.ParentId == pId).ToList();
            
            var tree = createTree(pId, list);
            if (!last)
            {
                tree = createTreeNoLast(pId, list, chk);
            }
            return Json(tree, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 生成包含末节的树
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="allList"></param>
        /// <returns></returns>
        private List<RightNode> createTree(long parentId, List<Epm_Milepost> allList)
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
        private List<RightNode> createTreeNoLast(long parentId, List<Epm_Milepost> allList, string chk = "")
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

        #endregion
        
        public ActionResult SelectProject(string name = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.pageIndex = pageIndex;

            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            ce = new ConditionExpression();
            ce.ExpName = "Name";
            ce.ExpValue = "%" + name + "%";
            ce.ExpOperater = eConditionOperator.Like;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);

            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<Epm_DataConfig>> result = new Result<List<Epm_DataConfig>>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetDataConfigListWhr(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }
        
        public ActionResult Edit(long id)
        {
            Result<Epm_Milepost> result = new Result<Epm_Milepost>();
            string dataName = "";

            string dataConfigId = "";
            ViewBag.ParName = "";
            ViewBag.DataName = "";
            ViewBag.DataConfigId = "";
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetMilepostById(id);

                if (result.Data.ParentId.Value == 0)
                {
                    ViewBag.ParName = "";
                }
                else {
                    //所属父级名称
                    ViewBag.ParName = proxy.GetMilepostById(result.Data.ParentId.Value).Data.Name;
                }
                var list = proxy.GetMilepostDataByMilepostId(result.Data.Id);
                if (list.Data.Count > 0)
                {
                    foreach (var item in list.Data)
                    {
                        dataName += item.DataName + ',';
                        dataConfigId += item.DataConfigId.ToString() + ',';
                    }
                    ViewBag.DataConfigId = dataConfigId.Substring(0, dataConfigId.Length - 1);
                    ViewBag.DataName = dataName.Substring(0, dataName.Length - 1);
                }
            }

            return View(result.Data);
        }

        /// <summary>
        /// 修改里程碑
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dataConfigID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Epm_Milepost model, string dataConfigID)
        {
            List<long> list = dataConfigID.SplitString(",").ToLongList();
            Result<int> result = new Result<int>();
            ResultView<string> view = new ResultView<string>();
            if (string.IsNullOrEmpty(model.ParentId.ToString()))
            {
                view.Flag = false;
                view.Message = "项目性质不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "名称不能为空";
                return Json(view);
            }
            if (Convert.ToBoolean(Request.Form["State"]) == true)
            {
                model.State = 1;
            }
            else
            {
                model.State = 0;
            }
            Result<Epm_Milepost> companyResult = new Result<Epm_Milepost>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateMilepost(model, list);
            }
            return Json(result.ToResultView());
        }


        /// <summary>
        /// 删除里程碑
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
                result = proxy.DeleteMilepostbyIds(list);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改里程碑状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ChangeState(long id, bool state)
        {
            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangeMilepostState(id, state);
            }
            return Json(result.ToResultView());
        }

    }
}