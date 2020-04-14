using hc.epm.DataModel.Basic;
using hc.epm.UI.Common;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Web.Controllers
{
    /// <summary>
    /// 咨询动态
    /// </summary>
    public class NewsController : BaseController
    {
        // GET: News
        public ActionResult Index()
        {
            ViewBag.Title = "新闻资讯";
            return View();
        }

        public ActionResult Detail(string id)
        {
            //新闻资讯详情
            return View();
        }
    }
}