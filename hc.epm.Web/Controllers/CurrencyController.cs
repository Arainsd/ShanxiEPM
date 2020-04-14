using DotNetCasClient;
using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace hc.epm.Web.Controllers
{
    public class CurrencyController : Controller
    {
        /// <summary>
        /// 是否启用陕西集成登录 0 否， 1 是
        /// </summary>
        private string IsOpenSxLogin
        {
            get
            {
                string value = ConfigurationManager.AppSettings["IsOpenSxLogin"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = "0";
                }
                return value;
            }
        }


        public ActionResult Index()
        {
            var userInfo = Session[ConstStr_Session.CurrentUserEntity] as UserView;
            if (userInfo == null)
            {
                ViewBag.Title = "首页";
                return View();
            }
            else
            {
                return RedirectToAction("SignIndex", "Home");
            }
        }

        protected ClientProxyExType ProxyEx(HttpRequestBase request, string userName = "youke")
        {
            ClientProxyExType cpet = Session[ConstStr_Session.CurrentProxyExType] as ClientProxyExType;
            if (cpet == null)
            {
                cpet = new ClientProxyExType();
                cpet.UserID = userName;
                cpet.IP_Client = request.UserHostAddress;
                cpet.IP_WebServer = NetTools.GetLocalMachineIP4();
            }
            return cpet;
        }

        /// <summary>
        /// 走，登录去。。。
        /// </summary>
        /// <returns></returns>
        public ActionResult GoToLogin()
        {
            if ("1".Equals(IsOpenSxLogin))
            {
                if (User.Identity.IsAuthenticated)
                {
                    string userAccount = CasAuthentication.CurrentPrincipal.Identity.Name;
                    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request, userAccount)))
                    {
                        Result<UserView> loginResult = proxy.LoginByCas(userAccount);
                        if (loginResult.Flag == 0)
                        {
                            Session[ConstStr_Session.CurrentUserEntity] = loginResult.Data;

                            LoadUserRight(loginResult.Data.RoleType.ToString(), loginResult.Data.UserId);

                            Result<List<Base_Right>> result = proxy.LoadRightList(loginResult.Data.RoleType.ToString(), loginResult.Data.UserId);
                            if (result.Flag == EResultFlag.Success)
                            {
                                var list = result.Data.Where(i => i.ParentId == 0 && i.IsMenu).ToList();

                                List<WebRightNode> rightNodesList = list.Select(p => new WebRightNode()
                                {
                                    Id = p.Id,
                                    Name = p.DisplayName,
                                    Url = p.URL,
                                    Target = p.Target
                                }).ToList();

                                foreach (var rightNode in rightNodesList)
                                {
                                    rightNode.ChildNode = GetChildNode(rightNode.Id, result.Data);
                                }

                                Session[ConstStr_Session.CurrUserRight] = rightNodesList;
                                ClientProxyExType cpet = new ClientProxyExType();
                                cpet = new ClientProxyExType();
                                cpet.UserID = loginResult.Data.UserId.ToString();
                                cpet.IP_Client = Request.UserHostAddress;
                                cpet.IP_WebServer = NetTools.GetLocalMachineIP4();
                                cpet.CurrentUser = loginResult.Data;
                                Session[ConstStr_Session.CurrentProxyExType] = cpet;
                            }
                        }
                    }

                    string url = "Home/SignIndex";
                    if (Request.QueryString.AllKeys.Contains("returnUrl"))
                    {
                        url = Request.QueryString["returnUrl"];
                    }
                    return Redirect(url);
                }
                else
                {
                    CasAuthentication.RedirectToLoginPage();
                    Response.Write("你不登录？");
                    Response.End();
                }
            }
            else
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            HttpCookie ticketCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (ticketCookie != null)
            {
                //获取form auth令牌信息，用以进行令牌清除
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(ticketCookie.Value);
                CasAuthenticationTicket casTicket = CasAuthentication.ServiceTicketManager.GetTicket(ticket.UserData);

                CasAuthentication.ServiceTicketManager.RevokeTicket(casTicket.ServiceTicket);
                CasAuthentication.ClearAuthCookie();

                //移除本地cookie及session
                FormsAuthentication.SignOut();
                Request.Cookies.Remove(FormsAuthentication.FormsCookieName);
                Response.Cookies.Remove(FormsAuthentication.FormsCookieName);
                //cas登出，调用此方法时，远端cas server会再次回调此url
                CasAuthentication.SingleSignOut();
            }
            Session.RemoveAll();
            return RedirectToAction("Login", "Currency");
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            if ("1".Equals(IsOpenSxLogin))
            {
                return RedirectToAction("SignIndex", "Home");
            }

            return View();
        }


        /// <summary>
        /// 登录处理
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">登录密码</param>
        /// <param name="verifyCode">验证码</param>
        /// <param name="isRemember">是否记住密码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(string userName, string password, string verifyCode, string isRemember)
        {
            ResultView<string> result;
            try
            {
                userName = userName ?? Request.Form["userName"];
                password = password ?? Request.Form["password"];
                verifyCode = verifyCode ?? Request.Form["verifyCode"];
                isRemember = isRemember ?? Request.Form["isRemember"];

                if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
                {
                    result = (new ResultView<string> { Flag = false, Message = "用户名或登录密码为空!" });
                    return Json(result);
                }

                if (string.IsNullOrWhiteSpace(verifyCode))
                {
                    result = (new ResultView<string> { Flag = false, Message = "请输入验证码!" });
                    return Json(result);
                }
                string code = (Session[ConstStr_Session.CurrValidateCode] ?? "").ToString();
                if (string.IsNullOrWhiteSpace(code))
                {
                    result = (new ResultView<string> { Flag = false, Message = "验证码超时!" });
                    return Json(result);
                }
                if (!verifyCode.Equals(code))
                {
                    result = (new ResultView<string> { Flag = false, Message = "验证码错误，请重新输入!" });
                    return Json(result);
                }

                password = DesTool.DesEncrypt(password);
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
                {
                    Result<UserView> loginResult = proxy.Login(userName, password, IsOpenSxLogin);
                    if (loginResult.Flag == 0)
                    {
                        Session[ConstStr_Session.CurrentUserEntity] = loginResult.Data;

                        //自动登录
                        if ("true".Equals(isRemember))
                        {
                            //保存用户名
                            HttpCookie cook = new HttpCookie(ConstString.COOKIEADMINNAME);
                            cook.Value = userName;
                            cook.Expires = DateTime.Now.AddDays(7);
                            Response.Cookies.Add(cook);
                            //保存密码
                            cook = new HttpCookie(ConstString.COOKIEADMINPWD);
                            cook.Value = password;
                            cook.Expires = DateTime.Now.AddDays(7);
                            Response.Cookies.Add(cook);

                            //存储在票据中，使用User.Identity或Request 中的Cookie 解密获取Ticket
                            FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, userName, DateTime.Now,
                                 DateTime.Now.AddMinutes(Session.Timeout - 1), false, userName);
                            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                            authCookie.HttpOnly = true;
                            Response.Cookies.Add(authCookie);

                            authTicket = new FormsAuthenticationTicket(1, password, DateTime.Now,
                                    DateTime.Now.AddMinutes(Session.Timeout - 1), false, password);
                            encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                            authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                            authCookie.HttpOnly = true;
                            Response.Cookies.Add(authCookie);
                        }
                        else//清除cookie
                        {
                            var nameCookie = new HttpCookie(ConstString.COOKIEADMINNAME);
                            var pwdCookie = new HttpCookie(ConstString.COOKIEADMINPWD);
                            nameCookie.Expires = DateTime.Now.AddDays(-1);
                            pwdCookie.Expires = DateTime.Now.AddDays(-1);
                            Response.Cookies.Add(nameCookie);
                            Response.Cookies.Add(pwdCookie);
                        }

                        LoadUserRight(loginResult.Data.RoleType.ToString(), loginResult.Data.UserId);

                        result = (new ResultView<string> { Flag = true, Message = "登录成功，正在跳转...", Data = "/Home/SignIndex" });
                    }
                    else
                    {
                        result = (new ResultView<string> { Flag = false, Message = loginResult.Exception.Decription });
                    }
                }
            }
            catch (Exception e)
            {
                //验证不通过，给出错误提示
                return Json(new ResultView<string> { Flag = false, Message = "登录异常！" + e.Message });
            }
            return Json(result);
        }

        /// <summary>
        /// 无权限访问
        /// </summary>
        /// <returns></returns>
        public ActionResult UnAuthorized()
        {
            var user = Session[ConstStr_Session.CurrentUserEntity] as UserView;
            if (user == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        /// <summary>
        /// 错误处理
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public ActionResult Error(string msg)
        {
            ViewBag.Msg = msg;
            return View();
        }

        public ActionResult ValidateCode()
        {
            ValidateCode vc = new ValidateCode();
            string code = vc.CreateValidateCode(6);
            Session[ConstStr_Session.CurrValidateCode] = code;
            byte[] image = vc.CreateValidateGraphic(code);
            return File(image, @"image/jpeg");
        }

        /// <summary>
        /// 获取用户权限
        /// </summary>
        /// <param name="roleType">用户角色</param>
        /// <param name="userId">用户 ID</param>
        private void LoadUserRight(string roleType, long userId)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                Result<List<Base_Right>> result = proxy.LoadRightList(roleType, userId);
                if (result.Flag == EResultFlag.Success)
                {
                    var list = result.Data.Where(i => i.ParentId == 0 && i.IsMenu).ToList();

                    List<WebRightNode> rightNodesList = list.Select(p => new WebRightNode()
                    {
                        Id = p.Id,
                        Name = p.DisplayName,
                        Url = p.URL,
                        Target = p.Target,
                        //Sort = p.Sort,
                        Remark = p.Remark,
                    }).ToList();

                    foreach (var rightNode in rightNodesList)
                    {
                        rightNode.ChildNode = GetChildNode(rightNode.Id, result.Data);
                    }
                    Session[ConstStr_Session.CurrUserRight] = rightNodesList;
                }
            }
        }

        private List<WebRightNode> GetChildNode(long pid, List<Base_Right> list)
        {
            var childList = list.Where(p => p.ParentId == pid && p.IsMenu).Select(p => new WebRightNode()
            {
                Id = p.Id,
                Name = p.DisplayName,
                Url = p.URL,
                Target = p.Target,
                //Sort = p.Sort,
                Remark = p.Remark,
            }).ToList();

            if (childList.Any())
            {
                foreach (var item in childList)
                {
                    item.ChildNode = GetChildNode(item.Id, list);
                }
            }
            return childList;
        }

        public void SetCurrProject(string projectid, string projectname)
        {
            Session[ConstString.COOKIEPROJECTID] = projectid;
            Session[ConstString.COOKIEPROJECTNAME] = projectname;
        }


        /// <summary>
        /// 调起cas登录
        /// </summary>
        /// <returns></returns>
        //此属性用以进行cas登录校验
        [Authorize]
        public ActionResult NeedLogin()
        {

            string userId = "";
            if (CasAuthentication.ServiceTicketManager != null)
            {
                //var cas = CasAuthentication.ServiceTicketManager;
                //var tickes = cas.GetAllTickets();
                userId = CasAuthentication.CurrentPrincipal.Identity.Name;   //获取服务端传过来的ID

            }
            return Content("已成功登录，user信息：" + userId);
        }


        /// <summary>
        /// 启用cookie提示
        /// </summary>
        /// <returns></returns>
        public ActionResult CookiesRequired()
        {
            return Content("请启用cookie");
        }

    }
}