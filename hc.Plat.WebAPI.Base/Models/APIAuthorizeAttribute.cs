using hc.epm.DataModel.Basic;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace hc.Plat.WebAPI.Base.Models
{
    public class APIAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 是否进行权限校验(默认为否)
        /// </summary>
        public bool CheckRight { get; set; } = false;

        /// <summary>
        /// 模块
        /// </summary>
        public object Module { get; set; }

        /// <summary>
        /// 操作权限
        /// </summary>
        public object Action { get; set; }

        private Object thisLock = new Object();
        
        /// <summary>
        /// 身份校验
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            try
            {
                lock (thisLock)
                {
                    //从http请求的头里面获取身份验证信息，验证是否是请求发起方的ticket
                    string ticket = "";
                    IEnumerable<string> accessToken = null;
                    //从header中获取token
                    bool isExitsToken = actionContext.Request.Headers.TryGetValues("AccessToken", out accessToken);
                    if (isExitsToken)
                    {
                        ticket = accessToken.FirstOrDefault();
                    }

                    string sys = ""; //1 Android ; 2 IOS
                    IEnumerable<string> accessSys = null;
                    //从header中获取token
                    bool isExitsSys = actionContext.Request.Headers.TryGetValues("Sys", out accessSys);
                    if (isExitsSys)
                    {
                        sys = accessSys.FirstOrDefault();
                    }

                    //获取querystring的值
                    if (!isExitsToken || !isExitsSys)
                    {
                        var list = actionContext.Request.GetQueryNameValuePairs();
                        var dic = list.ToDictionary(i => i.Key, i => i.Value);

                        if (!isExitsToken)
                        {
                            ticket = dic["AccessToken"];
                        }
                        if (!isExitsSys)
                        {
                            sys = dic["Sys"];
                        }
                    }

                    int type = 0;
                    int.TryParse(sys, out type);
                    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(ticket)))
                    {
                        Result<Base_User> login = proxy.GetBaseUserByToken(ticket, type);
                        if (login.Flag == 0 && ((type == 1 && login.Data.AndroidTokenTime.HasValue) || (type == 2 && login.Data.IosTokenTime.HasValue)))
                        {
                            DateTime expiryTime = type == 1 ? login.Data.AndroidTokenTime.Value : login.Data.IosTokenTime.Value;
                            if (DateTime.Now > expiryTime)
                            {
                                actionContext.Response = actionContext.ControllerContext.Request.CreateResponse(HttpStatusCode.OK);
                                var content = APIResult.GetErrorResult(MsgCode.InvalidToken);
                                actionContext.Response.Content = new StringContent(System.Web.Helpers.Json.Encode(content), System.Text.Encoding.UTF8, "application/json");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                actionContext.Response = actionContext.ControllerContext.Request.CreateResponse(HttpStatusCode.OK);
                var content = APIResult.GetErrorResult(MsgCode.InvalidToken, "Exception:" + ex.ToString() + ex.StackTrace);
                actionContext.Response.Content = new StringContent(System.Web.Helpers.Json.Encode(content), System.Text.Encoding.UTF8, "application/json");
            }
        }

        /// <summary>
        /// Token校验
        /// </summary>
        /// <returns></returns>
        //private bool ValidateToken(string ticket, string sys)
        //{
        //    bool result = false;
        //    int type = 0;
        //    int.TryParse(sys, out type);
        //    try
        //    {
        //        using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(ticket)))
        //        {
        //            Result<Base_User> login = proxy.GetBaseUserByToken(ticket, type);
        //            if (login.Flag == 0 && ((type == 1 && login.Data.AndroidTokenTime.HasValue) || (type == 2 && login.Data.IosTokenTime.HasValue)))
        //            {
        //                DateTime expiryTime = type == 1 ? login.Data.AndroidTokenTime.Value : login.Data.IosTokenTime.Value;
        //                if (DateTime.Now <= expiryTime)
        //                {
        //                    //user = login.Data;
        //                    result = true;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //    return result;
        //}

        ///// <summary>
        ///// 权限校验(判断用户登录时，已经获取了当前登录用户的所有权限)
        ///// </summary>
        ///// <returns></returns>
        //private bool ValidateRight()
        //{
        //    string module = (Module ?? "").ToString();
        //    string action = (Action ?? "").ToString();
        //    if (CheckRight)
        //    {
        //        if (string.IsNullOrWhiteSpace(module) || string.IsNullOrWhiteSpace(action))
        //        {
        //            return false;
        //        }
        //        if (user == null)
        //        {
        //            return false;
        //        }
        //        return true;
        //    }
        //    return true;
        //}


        protected ClientProxyExType ProxyEx(string token = "")
        {
            ClientProxyExType cpet = null;
            if (cpet == null)
            {
                cpet = new ClientProxyExType();
                cpet.Token = token;
                cpet.IP_WebServer = NetTools.GetLocalMachineIP4();
            }
            return cpet;
        }
    }
}