using hc.epm.Common;
using hc.epm.Admin.ClientProxy;
using hc.epm.UI.Common;
using hc.epm.DataModel.Basic;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.epm.Admin.Web.Controllers.HB
{
    public class HcConfigController : BaseHBController
    {

        /// <summary>
        /// 获取网站配置详情
        /// </summary>
        /// <returns></returns>
        public ActionResult Details()
        {
            Result<Base_Config> result = new Result<Base_Config>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.LoadConfig();
            }
            return View(result.ToResultView(true).Data);
        }



        /// <summary>
        /// 网站配置编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit()
        {
            Result<Base_Config> result = new Result<Base_Config>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.LoadConfig();
            }
            return View(result.ToResultView(true).Data);
        }

        /// <summary>
        /// 网站配置编辑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Base_Config model)
        {
            Result<int> result = new Result<int>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.UpdateConfig(model);
            }
            return Json(result.ToResultView());
        }


    }
}
