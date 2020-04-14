using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.Plat.FileServer.Web.Models
{
    public class CORSActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Current.Response.Cache.SetNoStore();
            var rqstMethod = filterContext.RequestContext.HttpContext.Request.HttpMethod;
            if (rqstMethod.ToUpper() == "OPTIONS")
            {
                filterContext.Result = new EmptyResult();
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
            
        }
    }
}