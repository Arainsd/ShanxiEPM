using hc.epm.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Admin.Web.Controllers
{
    public class NewTargetController : BaseController
    {
        /// <summary>
        /// 新闻类型列表
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 添加新闻类型
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            return View();
        }

        /// <summary>
        /// 修改新闻类型
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit()
        {
            return View();
        }
    }
}