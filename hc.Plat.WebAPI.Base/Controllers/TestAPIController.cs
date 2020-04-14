using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using hc.Plat.WebAPI.Base.Models;
using hc.Plat.WebAPI.Base.ViewModel;
using hc.Plat.WebAPI.Base.Common;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace hc.Plat.WebAPI.Base.Controllers
{
    /// <summary>
    /// 时间戳
    /// </summary>
    public class TestAPIController : BaseAPIController
    {
        /// <summary>
        /// 测试api
        /// </summary>
        /// <param name="signTime">参数传递的是加密后的时间</param>
        /// <returns></returns>
        [APIAuthorize]
        [HttpGet]
        public object GetList(string signTime = "2018-05-21 08:55:00")
        {
            signTime = APIAESTool.AesEncrypt(signTime);
            if (!string.IsNullOrEmpty(signTime))
            {
                var isSign = CheckSignTime(signTime);
                if (!isSign)
                {
                    return Json(APIResult.GetErrorResult(MsgCode.SignTimeError));
                }
            }
            return APIResult.GetSuccessResult("token验证通过，已取得数据！");
        }
    }
}