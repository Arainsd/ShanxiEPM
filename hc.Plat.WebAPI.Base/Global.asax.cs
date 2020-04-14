using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace hc.Plat.WebAPI.Base
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            
        }
        


        //public override void Init()
        //{
        //    this.AuthenticateRequest += WebApiApplication_AuthenticateRequest;
        //    base.Init();
        //}

        //void WebApiApplication_AuthenticateRequest(object sender, EventArgs e)
        //{
        //    //启用 webapi 支持session 会话
        //    HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        //}
    }
}
