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
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Admin.Web.Controllers
{
    public class NewsController : BaseController
    {
        /// <summary>
        /// 新闻资讯列表
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.News, Right = SystemRight.Browse)]
        public ActionResult Index(string NewsTitle, int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.NewsTitle = NewsTitle;
            ViewBag.pageIndex = pageIndex;

            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            ce = new ConditionExpression();
            ce.ExpName = "NewsTitle";
            ce.ExpValue = "%" + NewsTitle + "%";
            ce.ExpOperater = eConditionOperator.Like;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);

            qc.SortList.Add(new SortExpression("IsTop", eSortType.Desc));
            qc.SortList.Add(new SortExpression("Sort", eSortType.Asc));

            qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<Epm_News>> result = new Result<List<Epm_News>>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetNewsList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 添加新闻资讯
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.News, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据
                List<DictionaryType> typeList = new List<DictionaryType>() { DictionaryType.NewsCategory };
                var types = proxy.GetTypeListByTypes(typeList).Data;

                //返回新闻分类列表            
                ViewBag.SelNewsTypeName = types[DictionaryType.NewsCategory].ToSelectList("Name", "Id", true);
            }
            return View();
        }

        /// <summary>
        /// 添加新闻资讯
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [AuthCheck(Module = AdminModule.News, Right = SystemRight.Add)]
        public ActionResult Add(Epm_News model)
        {
            ResultView<int> view = new ResultView<int>();

            string fileDataJson = Request.Form["fileDataJson"];//获取上传文件json字符串

            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符

            //表单校验
            if (string.IsNullOrEmpty(model.NewsTitle))
            {
                view.Flag = false;
                view.Message = "新闻标题不能为空";
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
                result = proxy.AddNews(model, fileList);
            }
            return Json(result.ToResultView());
        }


        /// <summary>
        /// 修改新闻资讯
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.News, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<Epm_News> result = new Result<Epm_News>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetNewsById(id);
            }
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据
                List<DictionaryType> typeList = new List<DictionaryType>() { DictionaryType.NewsCategory };
                var types = proxy.GetTypeListByTypes(typeList).Data;
                //返回新闻分类列表            
                ViewBag.SelNewsTypeName = types[DictionaryType.NewsCategory].ToSelectList("Name", "Id", true);
            }

            return View(result.Data);
        }

        /// <summary>
        /// 修改新闻
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [AuthCheck(Module = AdminModule.News, Right = SystemRight.Modify)]
        public ActionResult Edit(Epm_News model)
        {
            string fileDataJson = Request.Form["fileDataJson"];//获取上传文件json字符串

            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            Result<int> result = new Result<int>();
            ResultView<string> view = new ResultView<string>();
            if (string.IsNullOrEmpty(model.NewsTitle))
            {
                view.Flag = false;
                view.Message = "新闻标题不能为空";
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
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateNews(model, fileList);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 删除新闻
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.News, Right = SystemRight.Delete)]
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteNewsByIds(list);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改新闻状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="type">1,是否置顶；2，是否发布</param>
        /// <returns></returns>
        //[AuthCheck(Module = AdminModule.News, Right = SystemRight.Enable)]
        [HttpPost]
        public ActionResult ChangeState(long id, bool state, int type)
        {
            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.ChangeNewsState(id, state, type);
            }
            return Json(result.ToResultView());
        }
    }
}