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
    public class HcOrganizationController : BaseHBController
    {
        public ActionResult Index(string belong = "Supplier", string name = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.belong = belong;
            ViewBag.pageIndex = pageIndex;
            ViewBag.belongText = belong.ToEnumReq<RoleType>().GetText();
            if (string.IsNullOrEmpty(belong))
            {
                return RedirectToAction("Error", "Home", new { msg = "必须有用户归属" });
            }

            QueryCondition qc = new QueryCondition();

            ConditionExpression ce1 = new ConditionExpression();
            if (!string.IsNullOrEmpty(name))
            {
                ce1.ExpName = "Name";
                ce1.ExpValue = "%" + name + "%";
                ce1.ExpOperater = eConditionOperator.Like;
                ce1.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce1);
            }

            ConditionExpression ce2 = new ConditionExpression();

            ConditionExpression ce3 = new ConditionExpression();
            ce3.ExpName = "PreCode";
            ce3.ExpValue = "10";
            ce3.ExpOperater = eConditionOperator.Equal;
            ce2.ConditionList.Add(ce3);

            ConditionExpression ce4 = new ConditionExpression();
            ce4.ExpName = "Type";
            ce4.ExpValue = "Owner";
            ce4.ExpOperater = eConditionOperator.NotEqual;
            ce4.ExpLogical = eLogicalOperator.Or;
            ce2.ConditionList.Add(ce4);

            ce2.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce2);
            

            SortExpression sort = new SortExpression("PreCode", eSortType.Asc);
            qc.SortList.Add(sort);

            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompanyList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            return View(result.Data);
        }

        /// <summary>
        /// 新增企业
        /// </summary>
        /// <param name="belong"></param>
        /// <param name="rightPId"></param>
        /// <returns></returns>
        public ActionResult Add(string belong, long rightPId)
        {
            ViewBag.Belong = belong;
            ViewBag.rightPId = rightPId;//根据当前新增页面是新增集团还是分公司传不同的PId（此处rightPId即PId）
            return View();
        }

        /// <summary>
        /// 新增企业
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(Base_Company model, string belong)
        {
            model.Type = belong;

            string fileDataJson = Request.Form["fileDataJson"];//获取上传文件json字符串
            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符串序列化为列表

            Result<int> result = new Result<int>();
            Result<Base_Company> companyResult = new Result<Base_Company>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                //获取上级单位信息
                if (model.PId == 0)
                {
                    model.PreName = null;
                    model.PreCode = null;
                }
                else
                {
                    companyResult = proxy.GetCompanyModel(model.PId);
                    model.PreName = companyResult.Data.Name;//上级单位名称
                    model.PreCode = companyResult.Data.Code;//上级单位编号
                }

                result = proxy.AddCompany(model, fileList);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 编辑企业
        /// </summary>
        /// <param name="id"></param>
        /// <param name="belong"></param>
        /// <returns></returns>
        public ActionResult Edit(long id, string belong)
        {
            Result<Base_Company> result = new Result<Base_Company>();
            ViewBag.Belong = belong;
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompanyModel(id);
                ConditionExpression ce = null;
                QueryCondition qc = new QueryCondition();
                Result<List<Base_Company>> resultcompany = new Result<List<Base_Company>>();
                ce = new ConditionExpression();
                ce.ExpName = "Type";
                ce.ExpValue = "%" + belong + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
                resultcompany = proxy.GetCompanyList(qc);
                var compantlist = resultcompany.Data;
                ViewBag.PId = compantlist.ToSelectList("Name", "Id", true, (result.Data.PId).ToString());
            }
            return View(result.Data);
        }

        /// <summary>
        /// 编辑企业
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Base_Company model, string belong)
        {
            model.Type = belong;
            Result<int> result = new Result<int>();
            ResultView<string> view = new ResultView<string>();
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Code))
            {
                view.Flag = false;
                view.Message = "编号不能为空";
                return Json(view);
            }

            string fileDataJson = Request.Form["fileDataJson"];//获取上传文件json字符串
            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符串序列化为列表

            Result<Base_Company> companyResult = new Result<Base_Company>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                companyResult = proxy.GetCompanyModel(model.Id);
                model.PreName = companyResult.Data.PreName;
                model.PreCode = companyResult.Data.PreCode;
                model.PId = companyResult.Data.PId;
                result = proxy.UpdateCompany(model, fileList);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 获取企业详情
        /// </summary>
        /// <param name="id"></param>
        /// <param name="belong"></param>
        /// <returns></returns>
        public ActionResult Detail(long id, string belong)
        {
            Result<Base_Company> result = new Result<Base_Company>();

            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompanyModel(id);
                if (string.IsNullOrEmpty(result.Data.Address))
                {
                    ViewBag.Address = "";
                }
                else {
                    var code = result.Data.Address.Split(',')[2];
                    ViewBag.Address = proxy.GetRegionModel(code).Data.Fullname;
                }
                if (string.IsNullOrEmpty(result.Data.PreCode))
                {
                    result.Data.PreCode = "无上级单位";
                }
                if (string.IsNullOrEmpty(result.Data.PreName))
                {
                    result.Data.PreName = "无上级单位";
                }
            }
            return View(result.Data);
        }

        /// <summary>
        /// 删除企业
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="belong"></param>
        /// <returns></returns>      
        [HttpPost]
        public ActionResult Delete(string ids, string belong)
        {
            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteCompanyByIds(list);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 组织机构
        /// </summary>
        /// <param name="belong"></param>
        /// <param name="leftPId"></param>
        /// <param name="rightPId"></param>
        /// <param name="name"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult Branch(string belong, long leftPId, long rightPId, string name = "", int pageIndex = 1, int pageSize = 1000)
        {
            ViewBag.name = name;
            ViewBag.belong = belong;
            ViewBag.leftPId = leftPId;
            ViewBag.rightPId = rightPId;
            ViewBag.pageIndex = pageIndex;
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
            ce.ExpName = "PId";
            ce.ExpValue = rightPId;
            ce.ExpOperater = eConditionOperator.Equal;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);

            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompanyList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            return View(result.Data);
        }

        /// <summary>
        /// 根据parentCode获取地区列表
        /// </summary>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RegionList(string parentCode)
        {
            ViewBag.parentCode = parentCode;
            Result<List<Base_Region>> result = new Result<List<Base_Region>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.LoadRegionList(parentCode);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 根据RegionCode获取地区详情
        /// </summary>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetRegionInfo(string code)
        {
            Result<Base_Region> result = new Result<Base_Region>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetRegionModel(code);
            }
            return Json(result);
        }

        /// <summary>
        /// 判断该企业是否存在下属企业 
        /// 若从在加载其组织机构，若不存在提示
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult IsParent(long id)
        {
            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();
            ce = new ConditionExpression();
            ce.ExpName = "PId";
            ce.ExpValue = id;
            ce.ExpOperater = eConditionOperator.Equal;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);
            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompanyList(qc);
            }
            return Json(result.Data);
        }

        public ActionResult Search()
        {
            return View();
        }

        /// <summary>
        /// 获取企业组织结构
        /// </summary>
        /// <param name="belong"></param>
        /// <param name="pId"></param>
        /// <param name="chk"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetCompanyTree(string belong, long pId, string chk = "")
        {
            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompanyListByRole(pId, true, true);
            }
            var list = result.Data;

            var first = list.FirstOrDefault(i => i.Id == pId);
            RightNode rootTree = new RightNode();

            rootTree.checkboxValue = first.Id.ToString();
            rootTree.@checked = first.Id.ToString() == chk;
            rootTree.data = new { code = first.Code, id = first.Id.ToString() };
            rootTree.name = first.Name;
            rootTree.spread = true;

            var tree = createTree(first.Id, list, chk);
            rootTree.children = tree;
            return Json(rootTree, JsonRequestBehavior.AllowGet); ;
        }

        /// <summary>
        /// 生成包含末节的树
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="allList"></param>
        /// <returns></returns>
        private List<RightNode> createTree(long parentId, List<Base_Company> allList, string chk = "")
        {
            List<RightNode> list = new List<RightNode>();
            var childList = allList.Where(i => i.PId == parentId).ToList();
            //有子公司
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

                    var iteratorList = createTree(item.Id, allList, chk);
                    node.children = iteratorList;
                    list.Add(node);
                }
            }
            return list;
        }
    }
}
