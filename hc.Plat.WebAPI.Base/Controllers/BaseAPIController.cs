using hc.epm.DataModel.Basic;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using hc.Plat.WebAPI.Base.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Http;

using System.Web.Http.Description;
using hc.Plat.WebAPI.Base.Common;
using System.Configuration;
using System.Net;
using System.Web;

namespace hc.Plat.WebAPI.Base.Controllers
{
    /// <summary>
    /// 移动端API基类:hsw by 2018/05/19
    /// </summary>
    public class BaseAPIController : ApiController
    {
        /// <summary>
        /// 验签超时时间，即超过此时间的请求判定为超时
        /// </summary>
        const int SIGNTIMEOUT = 2;

        /// <summary>
        /// 1为安卓，其他为ios，如需扩展h5,可以后续扩展类型</param>
        /// </summary>
        public int Sys
        {
            get
            {
                int sys = 0;
                IEnumerable<string> accessSys = null;
                bool isExitsSys = Request.Headers.TryGetValues("Sys", out accessSys);
                if (isExitsSys)
                {
                    sys = Int32.Parse(accessSys.FirstOrDefault());
                }
                return sys;
            }
        }

        /// <summary>
        /// 获取 Token
        /// </summary>
        public string Token
        {
            get
            {
                string ticket = "";
                IEnumerable<string> accessToken = null;
                bool isExitsToken = Request.Headers.TryGetValues("AccessToken", out accessToken);
                if (isExitsToken)
                {
                    ticket = accessToken.FirstOrDefault();
                }
                return ticket;
            }
        }

        /// <summary>
        /// APP 名称
        /// </summary>
        public string AppNum
        {
            get
            {
                var appNums = Request.Headers.GetValues("AppNum");
                if (appNums != null && appNums.Any())
                {
                    return appNums.FirstOrDefault();
                }
                return "";
            }
        }

        private Object thisLock = new Object();

        /// <summary>
        /// 当前登录用户信息，可能为空
        /// </summary>
        protected UserView CurrentUserView
        {
            get
            {
                lock (thisLock)
                {
                    UserView userView = null;
                    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Token)))
                    {
                        Result<UserView> result = proxy.GetUserModelByToken(Token, Sys);
                        //var temp = JsonConvert.SerializeObject(result);
                        //LogHelper.Info("CurrentUserView", "CurrentUserView:" + temp);
                        if (result.Flag == EResultFlag.Success && result.Data != null)
                        {
                            userView = result.Data;
                        }
                    }
                    return userView;
                }
            }
        }

        /// <summary>
        /// type=1表示安卓，2表示ios
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        [HttpGet]
        public object GetAuthTicket(string user, string pwd)
        {
            string tokenTicket = "";
            //登录
            var loginResult = login(user, pwd, Sys, out tokenTicket);
            if (loginResult)
            {
                var obj = new
                {
                    token = tokenTicket
                };
                return APIResult.GetSuccessResult(obj);
            }
            return APIResult.GetErrorResult(MsgCode.LoginError);
        }

        /// <summary>
        /// 仅校验token是否有效，在移动端记住密码之类的场景使用
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public object ValidateToken(string token, int type)
        {
            string tempToken = "";
            DateTime expiryTime = DateTime.MinValue;

            Result<Base_User> login = new Result<Base_User>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(token)))
            {
                login = proxy.GetBaseUserByToken(token, type);
                if (login.Flag == 0)
                {
                    var data = login.Data;
                    tempToken = type == 1 ? data.AndroidToken : data.IosToken;//模拟用户数据库中的token
                    expiryTime = type == 1 ? data.AndroidTokenTime.Value : data.IosTokenTime.Value;//模拟用户数据库中的token过期时间
                }
            }

            //没有根据token查到用户
            if (token == tempToken && DateTime.Now <= expiryTime)
            {
                return APIResult.GetSuccessResult(new { token });
            }
            return APIResult.GetErrorResult(MsgCode.InvalidToken);
        }

        /// <summary>
        /// 获取站点msg配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object GetMsgInfo()
        {
            return APIResult.GetSuccessResult(MsgView);
        }

        /// <summary>
        /// 获取服务器时间
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object GetTime()
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return APIResult.GetSuccessResult(new { time });
        }

        /// <summary>
        /// 获取消息配置信息
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        protected List<MsgView> MsgView
        {
            get
            {
                string jsonPath = AppCommonHelper.AppPath + "/Config/msg.json";
                string result = File.ReadAllText(jsonPath, Encoding.UTF8);
                var list = JsonConvert.DeserializeObject<List<MsgView>>(result);
                return list;
            }
        }

        /// <summary>
        /// 获取默认分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="isAllowPage"></param>
        /// <returns></returns>
        protected PageListInfo GetPageInfo(int pageIndex = 0, bool isAllowPage = true)
        {
            PageListInfo pageInfo = new PageListInfo
            {
                isAllowPage = isAllowPage,
                CurrentPageIndex = pageIndex,
                PageRowCount = AppCommonHelper.PageSize
            };

            return pageInfo;
        }

        #region  Helper Method

        /// <summary>
        /// 是否启用基础平台登录
        /// </summary>
        private static string IsOpenHbLogin
        {
            get
            {
                string value = ConfigurationManager.AppSettings["IsOpenHbLogin"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    value = "0";
                }
                return value;
            }
        }

        /// <summary>
        /// 登录地址
        /// </summary>
        private static string LoginUrl
        {
            get
            {
                string value = ConfigurationManager.AppSettings["LoginUrl"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("未配置基础服务器登录地址！");
                }
                return value;
            }
        }

        /// <summary>
        /// 执行登录
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pwd">OCeSdjE6K7zhDnoxh07rqg==,是111111的aes加密结果，DF57306D30FED672是平台111111加密结果</param>
        /// <param name="type"></param>
        /// <param name="token">登录成功输出token</param>
        /// <returns></returns>
        private bool login(string user, string pwd, int sys, out string token)
        {
            //移动端过来的密码先通过通用解密，再通过c#加密
            pwd = APIAESTool.AesDecrypt(pwd);
            pwd = DesTool.DesEncrypt(pwd);
            token = "";

            #region 调用基础平台验证用户账号密码
            if (IsOpenHbLogin == "1")
            {
                string url = LoginUrl + "?RequestParam={%22Param%22:{%22envRoot%22:{%22Product%22:%22BIM%22},%22paramRoot%22:{%22UserName%22:%22" + user + "%22,%22UserPass%22:%22" + pwd + "%22}}}";
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "multipart/form-data";

                string responseStr = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
                    responseStr = reader.ReadToEnd().ToString();
                    reader.Close();

                    LoginResult result = null;
                    if (!string.IsNullOrEmpty(responseStr))
                    {
                        result = JsonConvert.DeserializeObject<LoginResult>(responseStr);//将文件信息json字符
                    }

                    if (result == null || result.errorCode != "0")
                    {
                        return false;
                    }
                }
            }
            #endregion

            //获取数据库用户信息
            Result<UserView> login = new Result<UserView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx("")))
            {
                login = proxy.Login(user, pwd, IsOpenHbLogin);
            }
            if (login.Flag == 0 && login.Data != null)
            {
                string tempToken = sys == 1 ? login.Data.AndroidToken : login.Data.IosToken;//模拟用户数据库中的token
                //先去数据库查询该用户是否有token，没有则生成token
                if (!string.IsNullOrEmpty(tempToken))
                {
                    DateTime expiryTime = sys == 1 ? login.Data.AndroidTokenTime.Value : login.Data.IosTokenTime.Value;//模拟用户数据库中的token过期时间
                    //如果用户有token,检查是否在有效期
                    if (DateTime.Now <= expiryTime)
                    {
                        token = tempToken;
                    }
                }
                //无token或不在有效期则生成新token
                if (string.IsNullOrEmpty(token))
                {
                    token = CreateToken(sys);
                }

                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(login.Data)))
                {
                    Result<Base_User> baseUser = proxy.GetUserModel(login.Data.UserId);
                    Base_User u = baseUser.Data;
                    if (sys == 1)
                    {
                        u.AndroidToken = token;
                        u.AndroidTokenTime = DateTime.Now.AddDays(7);
                    }
                    else
                    {
                        u.IosToken = token;
                        u.IosTokenTime = DateTime.Now.AddDays(7);
                    }
                    var xxx = proxy.UpdateUser(u);
                }
                //TODO:因目前的token只是一个验证凭据，本身不附带业务信息，所以加密需求不强，但一定要使用https连接;如后期有扩展token需求，需要做加密或签名操作
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 生成随机token
        /// </summary>
        /// <param name="type">设备类型</param>
        /// <returns></returns>
        protected string CreateToken(int sys)
        {
            string[] sourceData = Guid.NewGuid().ToString().Replace("-", "").ToCharArray().Select(i => i.ToString()).ToArray();
            string ticketData = getRanNums(sourceData, 19).Replace(",", "") + (sys == 1 ? "A" : "I");
            return ticketData;
        }

        /// <summary>
        /// 时间签名校验
        /// </summary>
        /// <param name="signTime"></param>
        /// <returns></returns>
        protected bool CheckSignTime(string signTime)
        {
            try
            {
                if (!string.IsNullOrEmpty(signTime))
                {
                    //获取请求签名时间
                    var strTime = APIAESTool.AesDecrypt(signTime);
                    //时间转换
                    var time = strTime.ToDateTimeReq();
                    //得到客户端发起请求时间与当前时间差
                    var ts = DateTime.Now - time;
                    //时间范围内
                    if (ts.TotalSeconds <= 2 * 60)
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// 取得指定数组内指定数量的随机数
        /// </summary>
        /// <param name="s"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private string getRanNums(string[] s, int count)
        {
            Random rd = new Random();
            string str = "";
            List<string> list = new List<string>();
            for (int i = 0; i < count; i++)
            {
                string rNum = s[rd.Next(s.Length)];
                if (list.Count == 0)
                {
                    list.Add(rNum);
                }
                else
                {
                    list.Add(rNum);
                }
                str += rNum + ",";
            }
            return str.ToString().TrimEnd(',');
        }
        #endregion

        #region Proxy
        /// <summary>
        /// 获取初始化代理要传递的信息对象
        /// </summary>
        /// <param name="userId">
        /// 用户标识。
        /// 1.pc端为用户Id，如此时无需登录，此参数留空；
        /// 2.移动端为用户token，如此时无需登录，此参数留空；
        /// </param>
        /// <param name="request">当前webapi 的HttpRequest，移动端必传，pc端一定不要传入</param>
        /// <returns></returns>
        protected ClientProxyExType ProxyEx(UserView user)
        {
            ClientProxyExType cpet = MemoryNETCache.Get<ClientProxyExType>(user.UserId.ToString());
            if (cpet == null)
            {
                //用户登录后需要修改为用户信息
                cpet = new ClientProxyExType();
                cpet.CurrentUser = user;
                cpet.UserID = user.UserId.ToString();
                cpet.IP_Client = HttpContext.Current.Request.UserHostAddress;  //request.UserHostAddress;
                cpet.IP_WebServer = hc.Plat.Common.Global.NetTools.GetLocalMachineIP4();
                MemoryNETCache.Set(user.UserId.ToString(), cpet, 60000);
            }
            return cpet;
        }
        protected ClientProxyExType ProxyEx(string userId = "")
        {
            ClientProxyExType cpet = new ClientProxyExType();
            cpet.UserID = userId;
            cpet.IP_Client = HttpContext.Current.Request.UserHostAddress;  //request.UserHostAddress;
            cpet.IP_WebServer = hc.Plat.Common.Global.NetTools.GetLocalMachineIP4();
            return cpet;
        }
        #endregion
    }
}
