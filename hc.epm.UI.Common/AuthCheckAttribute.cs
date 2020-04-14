using hc.epm.Common;
using hc.epm.ViewModel;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.UI.Common
{
    public class AuthCheckAttribute : AuthorizeAttribute
    {
        public object Module { get; set; }
        public object Right { get; set; }
        /// <summary>
        /// 授权验证
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return Helper.IsCheck(httpContext, Module.ToString(), Right.ToString());
        }

        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }
        /// <summary>
        /// 授权失败的处理
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            else
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    ResultView<string> result = new ResultView<string>();
                    result.Flag = false;
                    result.Message = string.Format("无权限！模块为：{0}，权限项为：{1}",Module,Right);
                    filterContext.Result = new JsonResult { Data=result };
                }
                else
                {
                    filterContext.HttpContext.Response.Redirect("/Currency/Login");
                }
            }
        }
    }
}
