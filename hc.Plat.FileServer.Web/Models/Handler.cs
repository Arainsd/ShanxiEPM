using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace hc.Plat.FileServer.Web.Models
{
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public abstract class Handler
    {
        public Handler(HttpContextBase context)
        {
            this.Request = context.Request;
            this.Response = context.Response;
            this.Context = context;
            this.Server = context.Server;
        }

        public abstract void Process();

        protected void WriteJson(object response)
        {
            string jsonpCallback = Request["callback"],
                json = JsonConvert.SerializeObject(response);
            if (String.IsNullOrWhiteSpace(jsonpCallback))
            {
                var issimple = Request["issimpleupload"];
                var resulturl = Request["resulturl"];
                if (issimple != null && issimple.ToString() == "true")
                {
                    Response.Redirect(resulturl + "?result=" + json); //把json传递到c.com下面去呈现结果。
                }
                Response.AddHeader("Content-Type", "text/plain");
                Response.Write(json);
            }
            else
            {
                Response.AddHeader("Content-Type", "application/javascript");
                Response.Write(String.Format("{0}({1});", jsonpCallback, json));
            }
            Response.End();
        }

        public HttpRequestBase Request { get; private set; }
        public HttpResponseBase Response { get; private set; }
        public HttpContextBase Context { get; private set; }
        public HttpServerUtilityBase Server { get; private set; }
    }
}