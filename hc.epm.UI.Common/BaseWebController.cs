using hc.epm.Common;
using hc.epm.ViewModel;
using hc.Plat.Common.Global;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using hc.Plat.Common.Extend;
using System.Net;
using System.IO;
using System.Configuration;

namespace hc.epm.UI.Common
{
    public class BaseWebController : Controller
    {
        /// <summary>
        /// session检查
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ViewBag.CurrentAccount = null;
            var areaName = (string)RouteData.Values["area"];

            string controllerName = (string)RouteData.Values["controller"];
            string actionName = (string)RouteData.Values["action"];

            if (controllerName == "Project")
            {
                switch (actionName)
                {
                    case "ApprovalConstitute":
                    case "ProjectISP":
                    case "ProjectMainPoint":
                    case "MilestonePlan":
                    case "TimeManage":
                    case "ProjectData":
                    case "CrossingsManage":
                        actionName = "Edit";
                        break;
                    case "DetailApprovalConstitute":
                    case "DetailProjectISP":
                    case "DetailProjectMainPoint":
                    case "DetailMilestonePlan":
                    case "DetailTimeManage":
                    case "ScheduleInfo":
                    case "VisaInfo":
                    case "QuestionInfo":
                    case "ChangeInfo":
                    case "ContractInfo":
                    case "DetailProjectData":
                    case "DetailCrossingsManage":
                        actionName = "Detail";
                        break;
                    default:
                        break;
                }
            }

            ViewBag.controllerName = controllerName;
            ViewBag.actionName = actionName;

            try
            {
                var userInfo = Session[ConstStr_Session.CurrentUserEntity] as UserView;
                if (userInfo == null)
                {
                    var request = filterContext.HttpContext.Request;
                    string url = "";
                    if (request.IsAjaxRequest())
                    {
                        url = request.UrlReferrer.AbsoluteUri;
                    }
                    else
                    {
                        url = request.Url.ToString();
                    }

                    filterContext.Result = base.RedirectToAction("GoToLogin", "Currency", new { returnUrl = Server.UrlEncode(url) });
                }
                else
                {
                    string rights = Session[ConstString.RIGHTSSESSION] as string;
                    if (string.IsNullOrEmpty(rights))
                    {
                        rights = JsonConvert.SerializeObject(userInfo.Rights);
                        Session[ConstString.RIGHTSSESSION] = rights;
                    }
                }
            }
            catch
            {

                filterContext.Result = base.RedirectToAction("GoToLogin", "Currency");
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
        }

        /// <summary>
        /// action内的检查
        /// </summary>
        protected void ChekcRight(string module, string right)
        {
            bool result = Helper.IsCheck(HttpContext, module, right);
            if (!result)
            {
                Response.Redirect("/Currency/UnAuthorized");
                Response.End();
            }
        }

        /// <summary>
        /// 当前用户
        /// </summary>
        protected virtual UserView CurrentUser
        {
            get
            {
                var userInfo = HttpContext.Session[ConstStr_Session.CurrentUserEntity] as UserView;
                if (userInfo == null)
                {
                    Response.Redirect("/Currency/Login");
                    Response.End();
                }

                return userInfo;
            }
            set
            {
                HttpContext.Session[ConstStr_Session.CurrentUserEntity] = value;
            }
        }

        /// <summary>
        /// 是否登录检查
        /// </summary>
        public void IsLogin()
        {
            var userInfo = HttpContext.Session[ConstStr_Session.CurrentUserEntity] as UserView;
            if (userInfo == null)
            {
                Response.Redirect("/Currency/Login");
                Response.End();
            }
        }

        /// <summary>
        /// 代理信息获取
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected virtual ClientProxyExType ProxyEx(HttpRequestBase request)
        {
            ClientProxyExType cpet = Session[ConstStr_Session.CurrentProxyExType] as ClientProxyExType;
            if (cpet == null || cpet.UserID == "youke")
            {
                if (CurrentUser == null)
                {
                    CurrentUser = new UserView() { UserId = 897711908373794816 };
                }
                //TODO:用户登录后需要修改为用户信息，同时给applicationcontext赋值用户信息看在多用户登录情况下服务中是否生效,否则如写日志等操作需要在客户端将用户id传递过去
                cpet = new ClientProxyExType();
                cpet.UserID = CurrentUser.UserId.ToString();
                cpet.IP_Client = request.UserHostAddress;
                cpet.IP_WebServer = hc.Plat.Common.Global.NetTools.GetLocalMachineIP4();
                cpet.CurrentUser = CurrentUser;
                Session[ConstStr_Session.CurrentProxyExType] = cpet;

            }
            return cpet;

        }

        /// <summary>
        /// 获取分页信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public PageListInfo GetPageInfo(int pageIndex = 1, int pageSize = 10, Dictionary<string, string> orderBy = null)
        {
            PageListInfo pli = new PageListInfo();
            pli.isAllowPage = true;
            //写PAGE cooike
            HttpCookie cook = Request.Cookies["hc.Plat.currentgridlinenumber"];
            if (cook == null)
            {
                cook = new HttpCookie("hc.Plat.currentgridlinenumber");
                cook.Value = pageSize.ToString();
                cook.Expires = DateTime.Now.AddDays(7);
                Response.Cookies.Add(cook);

                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, pageSize.ToString(), DateTime.Now,
                        DateTime.Now.AddMinutes(Session.Timeout - 1), false, pageSize.ToString());
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                Response.Cookies.Add(authCookie);
            }

            pli.PageRowCount = pageSize;
            pli.CurrentPageIndex = pageIndex;
            if (orderBy == null)
            {
                orderBy = new Dictionary<string, string>();
                orderBy.Add("ID", "DESC");
            }
            string strOrder = "";
            foreach (var item in orderBy)
            {
                strOrder += item.Key + ":" + item.Value.ToUpper() + ",";
            }
            pli.OrderAndSortList = strOrder.TrimEnd(',');
            return pli;
        }

        public ActionResult Download(string guid)
        {
            return Redirect("http://192.168.1.239:8086/home/Download?guid=" + guid);
        }
    }
}