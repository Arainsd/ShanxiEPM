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
    public class NoticeController : BaseController
    {
        [AuthCheck(Module = AdminModule.NoticeManage, Right = SystemRight.Browse)]
        public ActionResult Index(string name,int pageIndex=1,int pageSize=10)
        { 
            ViewBag.Name = name;
            ViewBag.pageIndex = pageIndex;

            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            if (!string.IsNullOrEmpty(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Title";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            //SortExpression sort = new SortExpression("OperateTime", eSortType.Desc);
            //qc.SortList.Add(sort);

            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<NoticeView>> result = new Result<List<NoticeView>>();

            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetNoticeViewList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }
        [AuthCheck(Module = AdminModule.NoticeManage, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [AuthCheck(Module = AdminModule.NoticeManage, Right = SystemRight.Add)]
        public ActionResult Add(NoticeView model)
        {
            ResultView<int> view = new ResultView<int>();
            if (string.IsNullOrEmpty(model.Content))
            {
                view.Flag = false;
                view.Message = "内容不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Title))
            {
                view.Flag = false;
                view.Message = "标题不能为空";
                return Json(view);
            }
            if (model.WayOfRelease==null)
            {
                view.Flag = false;
                view.Message = "发布途径不能为空";
                return Json(view);
            }
            Result<int> result = new Result<int>();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddNotice(model);
            }
            return Json(result.ToResultView());
        }

        [AuthCheck(Module = AdminModule.NoticeManage, Right = SystemRight.Delete)]
        public ActionResult Delete(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (BusinessClientProxy proxy = new BusinessClientProxy(ProxyEx(Request)))
            {
                result= proxy.DeleteNoticeByIds(list);
            }
            return Json(result.ToResultView());
        }
    }
}