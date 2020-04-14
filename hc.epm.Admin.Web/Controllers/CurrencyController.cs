using hc.epm.Admin.ClientProxy;
using hc.epm.DataModel.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using hc.epm.Common;
using hc.epm.UI.Common;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using hc.epm.ViewModel;
//using Aliyun.Acs.Jaq.Model.V20161123;
//using Aliyun.Acs.Core;
//using Aliyun.Acs.Core.Profile;
using System.Web.Security;

namespace hc.epm.Admin.Web.Controllers
{
    public class CurrencyController : Controller
    {
        public ActionResult TestF()
        {
            var r = Request;
            return View();
        }
        protected ClientProxyExType ProxyEx(HttpRequestBase request, string userName = "admin")
        {
            ClientProxyExType cpet = Session[ConstStr_Session.CurrentProxyExType] as ClientProxyExType;
            if (cpet == null)
            {
                //TODO:用户登录后需要修改为用户信息，同时给applicationcontext赋值用户信息看在多用户登录情况下服务中是否生效,否则如写日志等操作需要在客户端将用户id传递过去
                cpet = new ClientProxyExType();
                cpet.UserID = userName;
                cpet.IP_Client = request.UserHostAddress;
                cpet.IP_WebServer = hc.Plat.Common.Global.NetTools.GetLocalMachineIP4();
                Session[ConstStr_Session.CurrentProxyExType] = cpet;

            }
            return cpet;

        }
        public ActionResult GoToLogin()
        {
            return View();
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            Session.RemoveAll();
            return RedirectToAction("Login", "Currency", new { isOut = true });
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="isOut"></param>
        /// <returns></returns>
        public ActionResult Login(string isOut = "")
        {
            //返回URL链接

            ViewBag.autoLogin = false;
            Result<Base_Config> result = new Result<Base_Config>();
            //加载网站配置
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                //result = proxy.LoadConfig();
                //ViewBag.webConfig = result.Data;
                ViewBag.userName = "";
                ViewBag.password = "";
                //是否已登录
                if (string.IsNullOrEmpty(isOut))
                {
                    var userInfo = HttpContext.Session[ConstStr_Session.CurrentUserEntity] as UserView;
                    if (userInfo != null)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                //是否是自动登录
                HttpCookie username = Request.Cookies[ConstString.COOKIEADMINNAME];
                HttpCookie password = Request.Cookies[ConstString.COOKIEADMINPWD];
                if (username != null && password != null && !string.IsNullOrEmpty(username.Value) && !string.IsNullOrEmpty(password.Value))
                {
                    ViewBag.autoLogin = true;
                    ViewBag.userName = username.Value;
                    ViewBag.password = DesTool.DesDecrypt(password.Value);
                    if (string.IsNullOrEmpty(isOut))
                    {
                        Result<UserView> loginResult = new Result<UserView>();
                        loginResult = proxy.Login(username.Value, password.Value, RoleType.Admin);
                        if (loginResult.Flag == 0)
                        {
                            Session[ConstStr_Session.CurrentUserEntity] = loginResult.Data;
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
            }
            return View();
        }
        /// <summary>
        /// 登录处理
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(string username, string password, string verifyCode)
        {
            //IAcsClient client = null;

            //IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", ConstString.KeyId, ConstString.KeySecret);
            //client = new DefaultAcsClient(profile);
            //DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", "Jaq", "jaq.aliyuncs.com");

            //AfsCheckRequest request = new AfsCheckRequest();
            //request.Platform = 3;//必填参数，请求来源： 1：Android端； 2：iOS端； 3：PC端及其他
            //request.Session = Request.Params["csessionid"];// 必填参数，从前端获取，不可更改
            //request.Sig = Request.Params["sig"];// 必填参数，从前端获取，不可更改
            //request.Token = Request.Params["token"];// 必填参数，从前端获取，不可更改
            //request.Scene = Request.Params["scene"];// 必填参数，从前端获取，不可更改
            ResultView<string> result = new ResultView<string>();
            password = DesTool.DesEncrypt(password);
            try
            {
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

                //AfsCheckResponse response = client.GetAcsResponse(request);
                //if (response.ErrorCode == null || response.ErrorCode.Value == 0)
                //{
                Result<UserView> loginResult = new Result<UserView>();
                using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request, username)))
                {
                    loginResult = proxy.Login(username, password, RoleType.Admin);

                }
                if (loginResult.Flag == 0)
                {
                    Session[ConstStr_Session.CurrentUserEntity] = loginResult.Data;
                    string isAuto = Request.Form["autologin"];
                    //自动登录
                    if (!string.IsNullOrEmpty(isAuto))
                    {
                        //保存用户名
                        HttpCookie cook = new HttpCookie(ConstString.COOKIEADMINNAME);
                        cook.Value = username;
                        cook.Expires = DateTime.Now.AddDays(7);
                        Response.Cookies.Add(cook);
                        //保存密码
                        cook = new HttpCookie(ConstString.COOKIEADMINPWD);
                        cook.Value = password;
                        cook.Expires = DateTime.Now.AddDays(7);
                        Response.Cookies.Add(cook);

                        //存储在票据中，使用User.Identity或Request 中的Cookie 解密获取Ticket
                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, username, DateTime.Now,
                             DateTime.Now.AddMinutes(Session.Timeout - 1), false, username);
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

                        //Response.Cookies.Remove(ConstString.COOKIEADMINNAME);
                        //Response.Cookies.Remove(ConstString.COOKIEADMINPWD);
                    }
                    result = (new ResultView<string> { Flag = true, Message = "登录成功，正在跳转..." });

                }
                else
                {
                    result = (new ResultView<string> { Flag = false, Message = loginResult.Exception.Decription });
                }
                //}
                //else
                //{
                //    result = (new ResultView<string> { Flag = false, Message = "Error:验证码错误" });
                //}
            }
            catch (Exception e)
            {
                //验证不通过，给出错误提示
                return Json(new ResultView<string> { Flag = false, Message = "验证码错误" + e.Message });
            }
            return Json(result);
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        public ActionResult Register()
        {
            return View();
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
        /// <summary>
        /// 404
        /// </summary>
        /// <returns></returns>
        public ActionResult NotFound()
        {
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
        /// 验证邮件链接
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ActionResult ValidateEmail(string code = "")
        {
            if (string.IsNullOrEmpty(code))
            {
                return RedirectToAction("Error", new { msg = "非法请求" });
            }
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                var result = proxy.ValidateEmailCodeByLink(code);
                if (result.Flag == EResultFlag.Success)
                {
                    var model = result.Data;
                    var step = model.ValidateType.ToEnumReq<MessageStep>();
                    //验证成功
                    switch (step)
                    {
                        case MessageStep.RegisterActive:
                            break;
                        case MessageStep.CertificationValid:
                            break;
                        case MessageStep.FindPwd:
                            break;
                        default:
                            throw new Exception("非法验证");
                    }
                }
            }
            return View();
        }
        public ActionResult Dev()
        {
            ViewBag.NewId = SnowflakeHelper.GetID;

            return View();
        }
        [HttpPost]
        public ActionResult Dev(string action = "")
        {
            switch (action)
            {

                case "right":
                    #region 权限
                    //var adminMoudle = Enum<AdminModule>.AsEnumerable();//管理员
                    //var tenMoudle = Enum<TendererModule>.AsEnumerable();//招标人
                    //var biddingMoudle = Enum<BiddingModule>.AsEnumerable();//代理
                    //var bidderMoudle = Enum<BidderModule>.AsEnumerable();//投标人
                    //var expModule = Enum<ExpModule>.AsEnumerable();//专家
                    //var ops = Enum<SystemRight>.AsEnumerable();//操作
                    //var rt = Enum<RoleType>.AsEnumerable();//角色
                    //List<Base_Right> list = new List<Base_Right>();

                    //using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
                    //{
                    //    var allRight = proxy.GetRightList(null);
                    //    ArrayList al = new ArrayList();
                    //    foreach (var roleType in rt)
                    //    {
                    //        switch (roleType)
                    //        {
                    //            case RoleType.Admin:
                    //                al = new ArrayList();
                    //                foreach (var a in adminMoudle)
                    //                {
                    //                    al.Add(a);
                    //                }
                    //                break;
                    //            case RoleType.Tenderer:
                    //                al = new ArrayList();
                    //                foreach (var a in tenMoudle)
                    //                {
                    //                    al.Add(a);
                    //                }
                    //                break;
                    //            case RoleType.BiddingAgent:
                    //                al = new ArrayList();
                    //                foreach (var a in biddingMoudle)
                    //                {
                    //                    al.Add(a);
                    //                }
                    //                break;
                    //            case RoleType.Bidder:
                    //                al = new ArrayList();
                    //                foreach (var a in bidderMoudle)
                    //                {
                    //                    al.Add(a);
                    //                }
                    //                break;
                    //            case RoleType.Exp:
                    //                al = new ArrayList();
                    //                foreach (var a in expModule)
                    //                {
                    //                    al.Add(a);
                    //                }
                    //                break;
                    //            default:
                    //                throw new ArgumentOutOfRangeException();
                    //        }
                    //        foreach (var module in al)
                    //        {
                    //            bool isAdd = true;
                    //            if (allRight.AllRowsCount > 0)
                    //            {
                    //                foreach (var item in allRight.Data)
                    //                {
                    //                    if (item.Name == module.ToString())
                    //                    {
                    //                        isAdd = false;
                    //                        break;
                    //                    }
                    //                }
                    //            }

                    //            //添加模块权限
                    //            Base_Right moduleRight = new Base_Right();
                    //            if (isAdd)
                    //            {
                    //                moduleRight.Belong = roleType.ToString();
                    //                moduleRight.IsConfirm = true;
                    //                moduleRight.IsEnable = true;
                    //                moduleRight.OperateUserId = 0;

                    //                string s = "";
                    //                switch (roleType)
                    //                {
                    //                    case RoleType.Admin:
                    //                        s = module.ToString().ToEnumReq<AdminModule>().GetText();
                    //                        break;
                    //                    case RoleType.Tenderer:
                    //                        s = module.ToString().ToEnumReq<TendererModule>().GetText();
                    //                        break;
                    //                    case RoleType.BiddingAgent:
                    //                        s = module.ToString().ToEnumReq<BiddingModule>().GetText();
                    //                        break;
                    //                    case RoleType.Bidder:
                    //                        s = module.ToString().ToEnumReq<BidderModule>().GetText();
                    //                        break;
                    //                    case RoleType.Exp:
                    //                        s = module.ToString().ToEnumReq<ExpModule>().GetText();
                    //                        break;
                    //                    default:
                    //                        throw new ArgumentOutOfRangeException();
                    //                }

                    //                moduleRight.ActionName = module.ToString();
                    //                moduleRight.OtherName = s;
                    //                moduleRight.ParentId = 0;
                    //                moduleRight.Remark = "";
                    //                moduleRight.RightName = module.ToString();
                    //                moduleRight.RightType = module.ToString();
                    //                list.Add(moduleRight);
                    //            }
                    //            isAdd = true;
                    //            foreach (var op in ops)
                    //            {
                    //                string module_op = module.ToString() + "_" + op.ToString();

                    //                if (allRight.AllRowsCount > 0)
                    //                {
                    //                    foreach (var item in allRight.Data)
                    //                    {
                    //                        if (item.RightName == module_op)
                    //                        {
                    //                            isAdd = false;
                    //                            break;
                    //                        }
                    //                    }
                    //                }
                    //                //添加操作权限
                    //                if (isAdd)
                    //                {
                    //                    Base_Right model = new Base_Right();
                    //                    model.Belong = roleType.ToString();
                    //                    model.IsConfirm = true;
                    //                    model.IsEnable = true;
                    //                    model.OperateUserId = 0;
                    //                    model.OtherName = op.GetText();
                    //                    model.ParentId = moduleRight.Id;
                    //                    model.Remark = "";
                    //                    model.RightName = module_op;
                    //                    model.RightType = module.ToString();
                    //                    model.ActionName = op.ToString();
                    //                    list.Add(model);

                    //                }
                    //            }

                    //        }
                    //    }

                    //    proxy.AddRightRange(list);
                    //}

                    #endregion
                    break;
                case "setting":
                    var sets = Enum<Settings>.AsEnumerable();//设置项
                    using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
                    {
                        var allSets = proxy.LoadSettings();
                        foreach (var set in sets)
                        {
                            bool isAdd = true;
                            foreach (var s in allSets.Data)
                            {
                                if (s.Code == set.ToString())
                                {
                                    isAdd = false;
                                }
                            }
                            if (isAdd)
                            {
                                Base_Settings model = new Base_Settings();
                                model.Code = set.ToString();
                                model.Name = set.GetText();
                                model.Value = "";
                                proxy.AddSettings(model);
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
            return Json(true);
        }
    }
}