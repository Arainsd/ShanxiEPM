using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using hc.Plat.Common.Extend;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    public class VisaController : BaseWebController
    {
        /// <summary>
        ///查询签证管理列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="name"></param>
        /// <param name="state"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Visa, Right = SystemRight.Browse)]
        public ActionResult Index(string projectName = "", string name = "", string visaTypeName = "", string state = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.projectName = projectName;
            ViewBag.visaTypeName = visaTypeName;
            ViewBag.pageIndex = pageIndex;
            ViewBag.state = typeof(ApprovalState).AsSelectList(true, state, new List<string>() { ApprovalState.WorkPartAppr.ToString(), ApprovalState.WorkFinish.ToString() });

            Result<List<Epm_Visa>> result = new Result<List<Epm_Visa>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                int s = string.IsNullOrEmpty(state) ? -1 : (int)(ApprovalState)Enum.Parse(typeof(ApprovalState), state);

                result = proxy.GetVisaList(projectName, name, s, visaTypeName, pageIndex, pageSize);

                foreach (var item in result.Data)
                {
                    item.VisaContent = item.VisaContent.CutByByteLength(150, "...");
                }

                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            return View(result.Data);
        }
        /// <summary>
        /// 添加签证
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Visa, Right = SystemRight.Add)]
        public ActionResult Add()
        {
            ViewBag.ProjectId = Session[ConstString.COOKIEPROJECTID];
            ViewBag.ProjectName = Session[ConstString.COOKIEPROJECTNAME];
            UserView user = Session[ConstStr_Session.CurrentUserEntity] as UserView;
            ViewBag.SubmitUserName = user.UserName;
            ViewBag.SubmitCompanyNmae = user.CompanyName;
            GetVisaType();
            return View();
        }

        /// <summary>
        /// 添加签证（提交方法）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthCheck(Module = WebModule.Visa, Right = SystemRight.Add)]
        public ActionResult Add(VisaView model)
        {
            Result<int> result = new Result<int>();
            List<Base_Files> fileList = new List<Base_Files>();
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            }

            string dataVisaCompany = Request.Form["CompanyIds"];

            if (!string.IsNullOrEmpty(dataVisaCompany))
            {
                model.Epm_VisaCompany = JsonConvert.DeserializeObject<List<Epm_VisaCompany>>(dataVisaCompany);

            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddVisa(model, fileList);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改签证
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuthCheck(Module = WebModule.Visa, Right = SystemRight.Modify)]
        public ActionResult Edit(long id)
        {
            Result<VisaView> result = new Result<VisaView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetVisaModel(id);
            }
            GetVisaType();
            return View(result.Data);
        }
        /// <summary>
        /// 选择更改列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult ChangeList(long projectId, string name, int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.projectId = projectId;
            ViewBag.name = name;

            Result<List<ChangeView>> result = new Result<List<ChangeView>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var project = proxy.GetProjectModel(projectId);
                var projectName = project.Data.Name;

                result = proxy.GetChangeList(projectName, name, -1, pageIndex, pageSize);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }
        /// <summary>
        /// 修改签证（提交数据）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 
        [AuthCheck(Module = WebModule.Visa, Right = SystemRight.Modify)]
        [HttpPost]
        public ActionResult Edit(VisaView model)
        {
            List<Base_Files> fileList = new List<Base_Files>();
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            }

            string dataVisaCompany = Request.Form["CompanyIds"];

            if (!string.IsNullOrEmpty(dataVisaCompany))
            {
                model.Epm_VisaCompany = JsonConvert.DeserializeObject<List<Epm_VisaCompany>>(dataVisaCompany);

            }

            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateVisa(model, fileList);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 查看签证详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [AuthCheck(Module = WebModule.Visa, Right = SystemRight.Info)]
        public ActionResult Detail(long id)
        {
            Result<VisaView> result = new Result<VisaView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetVisaModel(id);
            }
            GetVisaType();
            return View(result.Data);
        }
        /// <summary>
        /// 审核、驳回、废弃
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        //[AuthCheck(Module = WebModule.Change, Right = SystemRight.Check)]
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
            bool isOk = false;
            if ((ApprovalState)Enum.Parse(typeof(ApprovalState), state) == ApprovalState.ApprSuccess)
                isOk = Helper.IsCheck(HttpContext, WebModule.Visa.ToString(), SystemRight.Check.ToString(), true);
            else if ((ApprovalState)Enum.Parse(typeof(ApprovalState), state) == ApprovalState.ApprFailure)
                isOk = Helper.IsCheck(HttpContext, WebModule.Visa.ToString(), SystemRight.UnCheck.ToString(), true);
            else if ((ApprovalState)Enum.Parse(typeof(ApprovalState), state) == ApprovalState.Discarded)
                isOk = Helper.IsCheck(HttpContext, WebModule.Visa.ToString(), SystemRight.Invalid.ToString(), true);
            if (!isOk)
            {
                view.Flag = false;
                view.Message = "无权限！";
                return Json(view);
            }
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateVisaState(id, state);
            }
            return Json(result.ToResultView());
        }

        [HttpPost]

        [AuthCheck(Module = WebModule.Visa, Right = SystemRight.Delete)]
        public ActionResult Delete(long id)
        {
            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteVisaByIds(new List<long> { id });
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 获取签证类型下拉
        /// </summary>
        private void GetVisaType()
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据字典类型集合获取字典数据
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.VisaType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                ViewBag.VisaType = subjects[DictionaryType.VisaType].ToSelectList("Name", "No", true,"");
            }
        }
    }
}