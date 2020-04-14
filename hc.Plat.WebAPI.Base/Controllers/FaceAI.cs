using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Baidu.Aip.Face;
using hc.Plat.WebAPI.Base.Models;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using hc.epm.DataModel.Business;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using hc.epm.DataModel.Basic;
using hc.epm.Common;
using hc.epm.ViewModel;

namespace hc.Plat.WebAPI.Base.Controllers
{
    public partial class ProjectController
    {
        /// <summary>
        /// 人脸注册/更新
        /// </summary>
        /// <returns></returns>
        [APIAuthorize]
        [HttpPost]
        public object FaceRegistration()
        {
            try
            {
                var user = CurrentUserView;
                if (user == null)
                {
                    throw new Exception("获取用户信息失败！");
                }
                var http = HttpContext.Current;
                var form = http.Request.Form;

                string image = form["image"];
                string source = RoleType.Supplier.ToString();
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var userFaceResult = proxy.AddAIUserFaceInfo(user.UserId, image, source);

                    if (userFaceResult.Flag == EResultFlag.Success && userFaceResult.Data == 1)
                    {
                        return APIResult.GetSuccessResult(MsgCode.Success, "操作成功！");
                    }
                    else
                    {
                        return APIResult.GetErrorResult(MsgCode.CommonError, "操作失败！");
                    }
                }
            }
            catch
            {
                return APIResult.GetErrorResult(MsgCode.UserInfoError, "获取用户信息错误！");
            }
        }

        /// <summary>
        /// 人脸识别
        /// </summary>
        /// <returns></returns>
        [APIAuthorize]
        [HttpPost]
        public object FaceDistinguish()
        {
            SignFaceAI model = new SignFaceAI();
            try
            {
                var user = CurrentUserView;
                if (user == null)
                {
                    throw new Exception("获取用户信息失败！");
                }
                var http = HttpContext.Current;
                var form = http.Request.Form;
                model.Image = form["image"];
                model.ProjectId = Convert.ToInt64(form["projectId"]);
                model.ProjectName = form["projectName"];
                model.Longitude = form["longitude"];
                model.Latitude = form["latitude"];
                model.OilStationName = form["oilStationName"];
                model.UserId = user.UserId;
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(user)))
                {
                    var userFaceResult = proxy.SearchUserFace(model);

                    if (userFaceResult.Flag == EResultFlag.Success && userFaceResult.Data == 1)
                    {
                        return APIResult.GetSuccessResult(MsgCode.Success, "操作成功！");
                    }
                    else
                    {
                        string msg = "人脸不匹配！";
                        if (userFaceResult.Exception != null)
                        {
                            msg = userFaceResult.Exception.Decription;
                        }
                        return APIResult.GetErrorResult(MsgCode.CommonError, msg);
                    }
                }
            }
            catch
            {
                return APIResult.GetErrorResult(MsgCode.UserInfoError, "获取用户信息错误！");
            }
        }
    }
}