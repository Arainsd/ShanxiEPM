using hc.epm.Common;
using hc.epm.Admin.ClientProxy;
using hc.epm.DataModel.Basic;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hc.epm.UI.Common
{
    public class Helper
    {
        /// <summary>
        /// 权限检查
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="module">模块</param>
        /// <param name="right">权限</param>
        /// <param name="isFilter">如果判断不符合权限是否直接在此验证里截断请求直接跳转到无权限页面</param>
        /// <returns></returns>
        public static bool IsCheck(HttpContextBase httpContext, string module, string right, bool isFilter = false)
        {
            bool isOK = true;
            if (httpContext == null)
            {
                isOK = false;
                throw new ArgumentNullException("HttpContext");
            }

            if (string.IsNullOrEmpty(module))
            {
                isOK = false; ;
            }
            if (string.IsNullOrEmpty(right))
            {
                isOK = false; ;
            }
            //用户session为空，不通过
            var userInfo = httpContext.Session[ConstStr_Session.CurrentUserEntity] as UserView;
            if (userInfo == null)
            {
                isOK = false; ;
                httpContext.Response.Redirect("/Currency/Error?msg=登录超时");
                httpContext.Response.End();
            }
            else
            {
                //根据模块和action获取权限
                long rightId = 0;
                Result<List<Base_Right>> result = new Result<List<Base_Right>>();
                using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(httpContext, userInfo)))
                {
                    RoleType rt = (userInfo.RoleType == RoleType.Admin) ? RoleType.Admin.ToString().ToEnumReq<RoleType>() : RoleType.Owner.ToString().ToEnumReq<RoleType>();

                    result = proxy.LoadRightList(rt, userInfo.UserId);
                    var model = result.Data.FirstOrDefault(i => i.ParentCode == module && i.Code == right);
                    if (model == null)
                    {
                        isOK = false;
                    }
                    else
                    {
                        rightId = model.Id;
                    }
                }
                //没有权限，不通过
                if (!userInfo.RightIds.Contains(rightId))
                {
                    isOK = false; ;
                }
            }

            if (isFilter && !isOK)
            {
                httpContext.Response.Redirect("/Currency/Error?msg=未授权操作");
                httpContext.Response.End();
            }
            return isOK;
        }
        
        /// <summary>
        /// 权限检查
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="module">模块</param>
        /// <param name="right">权限</param>
        /// <returns></returns>
        public static ResultView<bool> IsCheckAjax(HttpContextBase httpContext, string module, string right)
        {
            bool isOK = true;
            if (httpContext == null)
            {
                isOK = false;
                throw new ArgumentNullException("HttpContext");
            }

            if (string.IsNullOrEmpty(module))
            {
                isOK = false; ;
            }
            if (string.IsNullOrEmpty(right))
            {
                isOK = false; ;
            }
            //用户session为空，不通过
            var userInfo = httpContext.Session[ConstStr_Session.CurrentUserEntity] as UserView;
            if (userInfo == null)
            {
                isOK = false; ;
                return NoUserJson;
            }
            else
            {
                //根据模块和action获取权限
                long rightId = 0;
                Result<List<Base_Right>> result = new Result<List<Base_Right>>();
                using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(httpContext, userInfo)))
                {
                    result = proxy.LoadRightList(userInfo.RoleType, userInfo.UserId);
                    var model = result.Data.FirstOrDefault(i => i.ParentCode == module && i.Code == right);
                    if (model == null)
                    {
                        isOK = false;
                    }
                    else
                    {
                        rightId = model.Id;
                    }

                }
                //没有权限，不通过
                if (!userInfo.RightIds.Contains(rightId))
                {
                    isOK = false; ;
                }
            }
            if (!isOK)
            {
                return UnAuthorizedJson;
            }
            return AuthorizedJson;
        }

        public static ResultView<bool> UnAuthorizedJson
        {
            get
            {
                ResultView<bool> result = new ResultView<bool>();
                result.Data = false;
                result.Flag = false;
                result.Message = "无权限进行此操作";
                return result;
            }
        }
        public static ResultView<bool> NoUserJson
        {
            get
            {
                ResultView<bool> result = new ResultView<bool>();
                result.Data = false;
                result.Flag = false;
                result.Message = "登录超时";
                return result;
            }
        }
        public static ResultView<bool> AuthorizedJson
        {
            get
            {
                ResultView<bool> result = new ResultView<bool>();
                result.Data = true;
                result.Flag = true;
                result.Message = "";
                return result;
            }
        }
        /// <summary>
        /// 代理信息获取
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected static ClientProxyExType ProxyEx(HttpContextBase httpContext, UserView user)
        {
            if (user == null)
            {
                httpContext.Response.Redirect("/Currency/Login");
                httpContext.Response.End();
            }
            ClientProxyExType cpet = httpContext.Session[ConstStr_Session.CurrentProxyExType] as ClientProxyExType;
            if (cpet == null)
            {
                //TODO:用户登录后需要修改为用户信息，同时给applicationcontext赋值用户信息看在多用户登录情况下服务中是否生效,否则如写日志等操作需要在客户端将用户id传递过去
                cpet = new ClientProxyExType();
                cpet.UserID = user.UserId.ToString();
                cpet.IP_Client = httpContext.Request.UserHostAddress;
                cpet.IP_WebServer = hc.Plat.Common.Global.NetTools.GetLocalMachineIP4();
                httpContext.Session[ConstStr_Session.CurrentProxyExType] = cpet;

            }
            return cpet;

        }
        /// <summary>
        /// 后台站点权限根
        /// </summary>
        public static Base_Right AdminSite
        {
            get
            {
                Base_Right model = new Base_Right();
                model.Id = 0;
                model.Belong = RoleType.Admin.ToString();
                model.Code = "AdminSite";
                model.Name = "管理员站点权限";
                model.Icon = "";
                model.IsMenu = false;
                model.ParentCode = "AdminSite";
                model.ParentId = -1;
                model.ParentName = "AdminSite";
                model.RightType = "Site";
                model.Sort = -1;
                model.Tips = "";
                model.URL = "";
                return model;
            }
        }

        public static Base_Right WebSite
        {
            get
            {
                Base_Right model = new Base_Right();
                model.Id = 0;
                //model.Belong = RoleType.Admin.ToString();
                model.Code = "WebSite";
                model.Name = "业务应用站点权限";
                model.Icon = "";
                model.IsMenu = false;
                model.ParentCode = "WebSite";
                model.ParentId = -1;
                model.ParentName = "WebSite";
                model.RightType = "Site";
                model.Sort = -1;
                model.Tips = "";
                model.URL = "";
                return model;
            }
        }
    }
}
