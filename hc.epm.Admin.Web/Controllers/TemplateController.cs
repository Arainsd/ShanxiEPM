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
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace hc.epm.Admin.Web.Controllers
{
    public class TemplateController : BaseController
    {
        public ActionResult Index(string name, string typeNo, int pageIndex = 1, int pageSize = 10)
        {
            ResultView<string> view = new ResultView<string>();
            if (string.IsNullOrEmpty(typeNo))
            {
                view.Flag = false;
                view.Message = "模板标识不能为空";
                return Json(view);
            }
            Base_TypeDictionary tempType = GetTempType(typeNo);
            Helper.IsCheck(HttpContext, GetModule(tempType.Name+"模板"), SystemRight.Browse.ToString(), true);

            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            ce = new ConditionExpression();
            ce.ExpName = "TemplateTypeId";
            ce.ExpValue = tempType.Id;
            ce.ExpOperater = eConditionOperator.Equal;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);
            if (!string.IsNullOrEmpty(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Title";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            Result<List<Epm_Template>> result = new Result<List<Epm_Template>>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetTemplateList(qc);
            }
            ViewBag.Total = result.AllRowsCount;
            ViewBag.name = name;
            ViewBag.typeNo = typeNo;
            ViewBag.pageIndex = pageIndex;
            ViewBag.TemplateId = tempType.Id;
            ViewBag.TemplateName = tempType.Name;
            return View(result.Data);
        }

        public ActionResult Add(long templateTypeId, string templateTypeName)
        {
            Epm_Template model = new Epm_Template();
            model.TemplateTypeId = templateTypeId;
            model.TemplateTypeName = templateTypeName;
            Helper.IsCheck(HttpContext, GetModule(model.TemplateTypeName+"模板"), SystemRight.Add.ToString(), true);
            return View(model);
        }
        [HttpPost]
        public ActionResult Add(Epm_Template model)
        {
            ResultView<string> view = new ResultView<string>();
            string fileDataJson = Request.Form["fileDataJson"];//获取上传文件json字符串
            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            if (model.TemplateTypeId == null)
            {
                view.Flag = false;
                view.Message = "模板类型ID不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.TemplateTypeName))
            {
                view.Flag = false;
                view.Message = "模板类型名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Title))
            {
                view.Flag = false;
                view.Message = "标题不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Description))
            {
                view.Flag = false;
                view.Message = "说明不能为空";
                return Json(view);
            }
            if (model.TrainCompanyId == null)
            {
                view.Flag = false;
                view.Message = "单位不能为空";
                return Json(view);
            }
            Result<long> rlTemp = new Result<long>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                rlTemp = proxy.AddTemplate(model, fileList);
            }
          return Json(rlTemp.ToResultView());
        }
        public ActionResult Edit(long id)
        {
            Result<Epm_Template> result = new Result<Epm_Template>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetTemplateModel(id);
            }
            Helper.IsCheck(HttpContext, GetModule(result.Data .TemplateTypeName+ "模板"), SystemRight.Add.ToString(), true);
            return View(result.Data);
        }
        [HttpPost]
        public ActionResult Edit(Epm_Template model)
        {
            ResultView<string> view = new ResultView<string>();
            string fileDataJson = Request.Form["fileDataJson"];//获取上传文件json字符串

            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            if (model.TemplateTypeId == null)
            {
                view.Flag = false;
                view.Message = "模板类型ID不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.TemplateTypeName))
            {
                view.Flag = false;
                view.Message = "模板类型名称不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Title))
            {
                view.Flag = false;
                view.Message = "标题不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Description))
            {
                view.Flag = false;
                view.Message = "说明不能为空";
                return Json(view);
            }
            if (model.TrainCompanyId == null)
            {
                view.Flag = false;
                view.Message = "单位不能为空";
                return Json(view);
            }
            model.IsDelete = false;
            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateTemplate(model, fileList);
            }
            return Json(result.ToResultView());
        }
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            var list=ids.SplitString(",").ToLongList();
            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteTemplateByIds(list);
            }
            return Json(result.ToResultView());
        }
        [HttpPost]
        public ActionResult DeleteDetails(string ids)
        {
            Result<int> result = new Result<int>();
            var list = ids.SplitString(",").ToLongList();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteTemplateDetailsByIds(list);
            }
            return Json(result.ToResultView());
        }

        public ActionResult AddDetails(long templateId)
        {
            return View(templateId);
        }

        public ActionResult GetDetails(long templateId)
        {

            Result<List<MilepostView>> result = new Result<List<MilepostView>>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetTemplateDetailsViewList(templateId);
            }
            var list = result.Data;
            return Json(list);
        }
      


        [HttpPost]
        public ActionResult AddDetails(Epm_TemplateDetails model)
        {
            ResultView<string> view = new ResultView<string>();
            if (string.IsNullOrEmpty(model.Name))
            {
                view.Flag = false;
                view.Message = "名称不能为空";
                return Json(view);
            }
            if (model.ParentId == null)
            {
                view.Flag = false;
                view.Message = "父级不能为空";
                return Json(view);
            }
            if (model.TemplateId==null)
            {
                view.Flag = false;
                view.Message = "模板Id不能为空";
                return Json(view);
            }
            Result<long> result = new Result<long>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddTemplateDetails(model);
            }
            return Json(result.ToResultView());
        }


        public ActionResult SelectParent(long templateId)
        {
            return View(templateId);
        }
        public ActionResult GetTemplateDetailsTree(long TemplateId,long pId = 0, bool last = true, string chk = "")
        {
            Result<List<Epm_TemplateDetails>> result = new Result<List<Epm_TemplateDetails>>();
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            if (TemplateId == 0)
            {
                ce = new ConditionExpression();
                ce.ExpName = "TemplateId";
                ce.ExpValue = "Null";
                ce.ExpOperater = eConditionOperator.Is;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            else
            {
                ce = new ConditionExpression();
                ce.ExpName = "TemplateId";
                ce.ExpValue = TemplateId;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetTemplateDetailsList(qc);
            }
            var list = result.Data;
            list.Insert(0, new Epm_TemplateDetails() { Id = 0, ParentId = -1, Name = "根节点", Sort = -1 });

            var first = list.FirstOrDefault(i => i.Id == pId);
            RightNode rootTree = new RightNode();

            rootTree.checkboxValue = first.Id.ToString();
            rootTree.@checked = chk == first.Id.ToString();
            rootTree.data = new {id = first.Id.ToString() };
            rootTree.name = first.Name;
            rootTree.spread = true;

            var tree = createTree(first.Id, list);
            if (!last)
            {
                tree = createTreeNoLast(first.Id, list, chk);
            }
            rootTree.children = tree;
            return Json(rootTree, JsonRequestBehavior.AllowGet);
        }
        private List<RightNode> createTree(long parentId, List<Epm_TemplateDetails> allList)
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
        private List<RightNode> createTreeNoLast(long parentId, List<Epm_TemplateDetails> allList, string chk = "")
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
        /// <summary>
        /// 获得AdminModule的值
        /// </summary>
        /// <param name="AttrName">枚举描述值</param>
        /// <returns>枚举值</returns>
        private string GetModule(string attrValue)
        {
            var fields = typeof(AdminModule).GetFields(BindingFlags.Static | BindingFlags.Public);
            Type attr = typeof(EnumTextAttribute);
            foreach (var field in fields)
            {
                var txtAttr = field.GetCustomAttributes(attr, false).First() as EnumTextAttribute;
                if (txtAttr.Value == attrValue)
                {
                    return field.Name;
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取模板类型 从字典表中
        /// </summary>
        /// <param name="typeNo">编号</param>
        /// <returns></returns>
        private Base_TypeDictionary GetTempType(string typeNo)
        {
            Result<List<Base_TypeDictionary>> resultType = new Result<List<Base_TypeDictionary>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                resultType = proxy.GetTypeListByNo(typeNo);
            }
            return resultType.Data.SingleOrDefault();
        }
        #region 备用
        //public ActionResult EditDetails(long id)
        //{
        //    Result<Epm_TemplateDetails> result = new Result<Epm_TemplateDetails>();
        //    using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
        //    {
        //        proxy.GetTemplateDetailsModel(id);
        //    }
        //    return View(result.Data);
        //}

        //[HttpPost]
        //public ActionResult EditDetails(Epm_TemplateDetails model)
        //{
        //    ResultView<string> view = new ResultView<string>();
        //    if (string.IsNullOrEmpty(model.Name))
        //    {
        //        view.Flag = false;
        //        view.Message = "名称不能为空";
        //        return Json(view);
        //    }
        //    if (model.ParentId == null)
        //    {
        //        view.Flag = false;
        //        view.Message = "父级不能为空";
        //        return Json(view);
        //    }
        //    Result<int> result = new Result<int>();
        //    using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
        //    {
        //        result = proxy.UpdateTemplateDetails(model);
        //    }
        //    return Json(result.ToResultView());
        //}
        //public ActionResult Add(long templateTypeId, string templateTypeName)
        //{
        //    // Base_TypeDictionary tempType = GetTempType(typeNo);
        //    //建空的模板 用于添加模板明细
        //    Epm_Template model = new Epm_Template();
        //    model.TemplateTypeId = templateTypeId;
        //    model.TemplateTypeName = templateTypeName;
        //    model.Title = "";
        //    model.Description = "";
        //    model.IsDelete = true;
        //    using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
        //    {
        //        proxy.AddTemplate(model);
        //    }
        //    return View(model);
        //}
        //[HttpPost]
        //public ActionResult Add(Epm_Template model)
        //{
        //    ResultView<string> view = new ResultView<string>();
        //    if (model.TemplateTypeId == null)
        //    {
        //        view.Flag = false;
        //        view.Message = "模板类型ID不能为空";
        //        return Json(view);
        //    }
        //    if (string.IsNullOrEmpty(model.TemplateTypeName))
        //    {
        //        view.Flag = false;
        //        view.Message = "模板类型名称不能为空";
        //        return Json(view);
        //    }
        //    Helper.IsCheck(HttpContext, GetModule(model.TemplateTypeName), SystemRight.Add.ToString(), true);
        //    if (string.IsNullOrEmpty(model.Title))
        //    {
        //        view.Flag = false;
        //        view.Message = "标题不能为空";
        //        return Json(view);
        //    }
        //    if (string.IsNullOrEmpty(model.Description))
        //    {
        //        view.Flag = false;
        //        view.Message = "说明不能为空";
        //        return Json(view);
        //    }
        //    if (model.TrainCompanyId == null)
        //    {
        //        view.Flag = false;
        //        view.Message = "单位不能为空";
        //        return Json(view);
        //    }
        //    model.IsDelete = false;
        //    Result<int> result = new Result<int>();
        //    using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
        //    {
        //        result = proxy.UpdateTemplate(model);
        //    }
        //    return Json(result.ToResultView());
        //}
        #endregion
    }
}