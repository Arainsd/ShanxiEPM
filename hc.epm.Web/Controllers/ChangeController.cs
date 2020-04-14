using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using hc.epm.UI.Common;
using hc.epm.Common;
using hc.Plat.Common.Extend;
using hc.epm.DataModel.Business;
using hc.Plat.Common.Global;
using hc.epm.Web.ClientProxy;
using hc.epm.ViewModel;
using hc.epm.DataModel.Basic;
using Newtonsoft.Json;

namespace hc.epm.Web.Controllers
{
    public class ChangeController : BaseWebController
    {
        [AuthCheck(Module = WebModule.EngineeringChange, Right = SystemRight.Browse)]
        public ActionResult Index(string projectName = "", string name = "", string state = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.projectName = projectName;
            ViewBag.pageIndex = pageIndex;
            ViewBag.state = typeof(ApprovalState).AsSelectList(true, state, new List<string>() { ApprovalState.WorkPartAppr.ToString(), ApprovalState.WorkFinish.ToString() });

            Result<List<ChangeView>> result = new Result<List<ChangeView>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                int s = string.IsNullOrEmpty(state) ? -1 : (int)(ApprovalState)Enum.Parse(typeof(ApprovalState), state);

                result = proxy.GetChangeList(projectName, name, s, pageIndex, pageSize);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 添加合同变更
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.EngineeringChange, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID];
            ViewBag.ProjectName = Session[ConstString.COOKIEPROJECTNAME];
            UserView user = base.CurrentUser;
            ViewBag.SubmitUserName = user.UserName;
            ViewBag.SubmitCompanyNmae = user.CompanyName;

            return View();
        }


        [HttpPost]
        [AuthCheck(Module = WebModule.EngineeringChange, Right = SystemRight.Add)]
        public ActionResult Add(ChangeView model)
        {
            Result<int> result = new Result<int>();
            List<Base_Files> fileList = new List<Base_Files>();
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            }

            string dataChangeCompany = Request.Form["CompanyIds"];

            if (!string.IsNullOrEmpty(dataChangeCompany))
            {
                model.Epm_ChangeCompany = JsonConvert.DeserializeObject<List<Epm_ChangeCompany>>(dataChangeCompany);

            }
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddChange(model, fileList);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改合同变更
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.EngineeringChange, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<ChangeView> result = new Result<ChangeView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetChangeModel(id);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 修改合同变更（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebModule.EngineeringChange, Right = SystemRight.Modify)]
        public ActionResult Edit(ChangeView model)
        {
            ResultView<int> view = new ResultView<int>();
            //表单校验
            if (string.IsNullOrEmpty(model.ChangeName))
            {
                view.Flag = false;
                view.Message = "名称不能为空";
                return Json(view);
            }
            List<Base_Files> fileList = new List<Base_Files>();
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            }

            string dataChangeCompany = Request.Form["CompanyIds"];

            if (!string.IsNullOrEmpty(dataChangeCompany))
            {
                model.Epm_ChangeCompany = JsonConvert.DeserializeObject<List<Epm_ChangeCompany>>(dataChangeCompany);

            }

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateChange(model, fileList);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 查看变更详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.EngineeringChange, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            Result<ChangeView> result = new Result<ChangeView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetChangeModel(id);
            }
            return View(result.Data);
        }
        /// <summary>
        /// 审核、驳回、废弃
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.EngineeringChange, Right = SystemRight.Check)]
        [HttpPost]
        public ActionResult UpdateState(long id, string state)
        {
            ResultView<int> view = new ResultView<int>();
            if (string.IsNullOrEmpty(state))
            {
                view.Flag = false;
                view.Message = "状态不能为空";
                return Json(view);
            }
            //判断权限
            //if ((ApprovalState)Enum.Parse(typeof(ApprovalState), state) == ApprovalState.ApprSuccess)
            //    Helper.IsCheck(HttpContext, WebModule.Change.ToString(), SystemRight.Check.ToString(), true);
            //else if ((ApprovalState)Enum.Parse(typeof(ApprovalState), state) == ApprovalState.ApprFailure)
            //    Helper.IsCheck(HttpContext, WebModule.Change.ToString(), SystemRight.UnCheck.ToString(), true);
            //else if ((ApprovalState)Enum.Parse(typeof(ApprovalState), state) == ApprovalState.Discarded)
            //    Helper.IsCheck(HttpContext, WebModule.Change.ToString(), SystemRight.Invalid.ToString(), true);

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateChangeState(id, state);
            }
            return Json(result.ToResultView());
        }

        [HttpPost]
        public ActionResult Delete(long id)
        {
            //ResultView<int> view = new ResultView<int>();
            //if (string.IsNullOrEmpty(state))
            //{
            //    view.Flag = false;
            //    view.Message = "状态不能为空";
            //    return Json(view);
            //}
            //ApprovalState app;
            //if (!Enum.TryParse(state, out app))
            //{
            //    view.Flag = false;
            //    view.Message = "状态值不正确";
            //    return Json(view);
            //}
            //if (app != ApprovalState.Enabled || app != ApprovalState.Discarded)
            //{
            //    view.Flag = false;
            //    view.Message = "草稿，已废弃状态下，才可删除";
            //    return Json(view);
            //}
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteChangeByIds(new List<long> { id });
            }
            return Json(result.ToResultView());
        }
    }
}