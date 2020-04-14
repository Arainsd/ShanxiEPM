using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using hc.epm.UI.Common;
using hc.epm.Common;
using Aliyun.Acs.Jaq.Model.V20161123;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Profile;
using hc.epm.ViewModel;
using hc.Plat.Common.Global;
using hc.epm.Admin.ClientProxy;
using hc.epm.DataModel.Basic;
using hc.Plat.Common.Extend;
using System.Text;
using hc.epm.DataModel.Msg;

namespace hc.epm.Admin.Web.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            Result<Base_Config> result = new Result<Base_Config>();
            //加载网站配置
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.LoadConfig();
                ViewBag.webConfig = result.Data;
                ViewBag.userName = CurrentUser.RealName;

            }
            return View();
        }

        private void Test()
        {
        }
        /// <summary>
        /// 加载顶部菜单
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="rightType"></param>
        /// <returns></returns>
      
        public ActionResult LoadNavMenu(long pId, string rightType = "Nav")
        {
            Result<List<Base_Right>> result = new Result<List<Base_Right>>();
            //加载顶部菜单

            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.LoadRightList(CurrentUser.RoleType, CurrentUser.UserId);

            }
            var list = result.Data.Where(i => i.RightType == rightType && i.ParentId == pId && i.IsMenu).ToList();
            ResultView<List<Base_Right>> obj = result.ToResultView();
            obj.Data = list;
            return Json(obj);
        }
        /// <summary>
        /// 加载子菜单
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadChildMenu(long pId)
        {
            RightNode model = GetRightTree(CurrentUser.RoleType, pId, CurrentUser.UserId);
            return Json(model);
        }

        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <param name="belong"></param>
        /// <param name="pId"></param>
        /// <returns></returns>
        public RightNode GetRightTree(RoleType roleType, long pId, long userId)
        {
            Result<List<Base_Right>> result = new Result<List<Base_Right>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.LoadRightList(roleType, userId);
            }
            RightNode rootTree = new RightNode();

            var list = result.Data.Where(i => i.IsMenu).ToList();
            list.Insert(0, Helper.AdminSite);

            var first = list.FirstOrDefault(i => i.Id == pId);

            rootTree.checkboxValue = first.Id.ToString();
            rootTree.@checked = false;
            rootTree.data = new { code = first.Code, id = first.SId, url = string.IsNullOrEmpty(first.URL) ? "javascript:void(0);" : first.URL, icon = string.IsNullOrEmpty(first.Icon) ? "" : first.Icon, target = string.IsNullOrEmpty(first.Target) ? "" : first.Target, display = string.IsNullOrEmpty(first.DisplayName) ? first.Name : first.DisplayName };
            rootTree.name = first.Name;
            rootTree.spread = true;

            var tree = createTree(first.Id, list);

            rootTree.children = tree;
            return rootTree; ;
        }
        /// <summary>
        /// 生成树
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="allList"></param>
        /// <returns></returns>
        private List<RightNode> createTree(long parentId, List<Base_Right> allList)
        {
            List<RightNode> list = new List<RightNode>();
            var childList = allList.Where(i => i.ParentId == parentId).ToList();
            //有子权限
            if (childList != null && childList.Any())
            {
                foreach (var item in childList)
                {
                    RightNode node = new RightNode();
                    node.checkboxValue = item.Id.ToString();
                    node.@checked = false;
                    node.data = new { code = item.Code, id = item.SId, url = string.IsNullOrEmpty(item.URL) ? "javascript:void(0);" : item.URL, icon = string.IsNullOrEmpty(item.Icon) ? "" : item.Icon, target = string.IsNullOrEmpty(item.Target) ? "" : item.Target, display = string.IsNullOrEmpty(item.DisplayName) ? item.Name : item.DisplayName };
                    node.name = item.Name;
                    node.spread = true;

                    var iteratorList = createTree(item.Id, allList);
                    node.children = iteratorList;
                    list.Add(node);
                }
            }
            return list;
        }



        public ActionResult Welcome()
        {
            //获取首页最新消息列表
            Result<List<Msg_Message>> result = new Result<List<Msg_Message>>();
            using (MessageClientProxy proxy = new MessageClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetMessageNoReadList(CurrentUser.UserId, CurrentUser.CompanyId, 7);
                ViewBag.MessageList = result.Data;
            }
            return View();
        }

        /// <summary>
        /// 百度编辑器跨域单文件上传回调页面
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public ActionResult UESimpleFile(string result)
        {
            return Content(result);
        }

        //TODO:所有手动去除ValidateInput的地方，需要验证script标签，以防止脚本注入
        [ValidateInput(false)]//去除对于html危险字符的筛选
        public ActionResult TestUE(FormCollection fc)
        {
            string a = fc["content"];
            ViewBag.con = a; ;
            return View();
        }

        public ActionResult TestCode()
        {
            return View();

        }
        [HttpPost]
        public ActionResult TestCode(string username, string password)
        {
            IAcsClient client = null;



            IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", ConstString.KeyId, ConstString.KeySecret);
            client = new DefaultAcsClient(profile);
            DefaultProfile.AddEndpoint("cn-hangzhou", "cn-hangzhou", "Jaq", "jaq.aliyuncs.com");


            AfsCheckRequest request = new AfsCheckRequest();
            request.Platform = 3;//必填参数，请求来源： 1：Android端； 2：iOS端； 3：PC端及其他
            request.Session = Request.Params["csessionid"];// 必填参数，从前端获取，不可更改
            request.Sig = Request.Params["sig"];// 必填参数，从前端获取，不可更改
            request.Token = Request.Params["token"];// 必填参数，从前端获取，不可更改
            request.Scene = Request.Params["scene"];// 必填参数，从前端获取，不可更改

            ResultView<string> result = new ResultView<string>();
            try
            {
                AfsCheckResponse response = client.GetAcsResponse(request);
                if (response.ErrorCode == null || response.ErrorCode.Value == 0)
                {
                    //写正常业务逻辑

                    result = (new ResultView<string> { Flag = true, Message = "登录成功" });
                }
                else
                {
                    result = (new ResultView<string> { Flag = false, Message = "Error:" + response.ErrorMsg });
                }

            }
            catch (Exception e)
            {
                //验证不通过，给出错误提示
                return Json(new ResultView<string> { Flag = false, Message = "验证码错误" + e.Message });
            }
            return Json(result);
        }

    }
}