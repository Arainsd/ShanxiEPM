using DotNetCasClient;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CASTest.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        //此属性用以进行cas登录校验
        [Authorize]
        public ActionResult NeedLogin()
        {
            #region 自行代码里进行登录校验，同属性Authorize
            if (!User.Identity.IsAuthenticated)
            {
                DotNetCasClient.CasAuthentication.RedirectToLoginPage();
                return Content("还是未登录");
            }
            #endregion
            //else
            //{
            //    if (CasAuthentication.ServiceTicketManager != null)
            //    {
            //        var cas = CasAuthentication.ServiceTicketManager;
            //        var tickes = cas.GetAllTickets();
            //        var aa = CasAuthentication.CurrentPrincipal.Identity.Name;   //获取服务端传过来的ID

            //        var user = User;

            //        var casPrincipal = (ICasPrincipal)user;

            //        var dict = casPrincipal.Assertion.Attributes;
            //    }

            //}
            string userId = "";
            if (CasAuthentication.ServiceTicketManager != null)
            {
                //var cas = CasAuthentication.ServiceTicketManager;
                //var tickes = cas.GetAllTickets();
                userId = CasAuthentication.CurrentPrincipal.Identity.Name;   //获取服务端传过来的ID
                //自行根据userid查询用户信息，赋给我们自身的session

                //var user = User;

                //var casPrincipal = (ICasPrincipal)user;

                //var dict = casPrincipal.Assertion.Attributes;
            }
            return Content("已成功登录，user信息：" + userId);
        }


        public ActionResult NotAuthorized()
        {
            return Content("此处应是客户端登录界面");
        }

        public ActionResult CookiesRequired()
        {
            return Content("登录成功界面");
        }

        public ActionResult Out()
        {
            HttpCookie ticketCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (ticketCookie != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(ticketCookie.Value);
                CasAuthenticationTicket casTicket = CasAuthentication.ServiceTicketManager.GetTicket(ticket.UserData);

                CasAuthentication.ServiceTicketManager.RevokeTicket(casTicket.ServiceTicket);

                CasAuthentication.ClearAuthCookie();
                FormsAuthentication.SignOut();
                Request.Cookies.Remove(FormsAuthentication.FormsCookieName);
                Response.Cookies.Remove(FormsAuthentication.FormsCookieName);
                Session.RemoveAll();
                //会回调客户端
                CasAuthentication.SingleSignOut();
            }
            return Content("退出成功,获取当前用户信息:" + User.Identity.IsAuthenticated);
        }
    }
}