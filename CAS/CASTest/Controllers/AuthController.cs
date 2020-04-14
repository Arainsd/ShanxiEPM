using CASTest.Models;
using System.Web.Http;

namespace CASTest.Controllers
{
    public class AuthController : ApiController
    {
        // GET api/<controller>
        public object LoginCallBack(ReqUserModel model)
        {
            if (model != null)
            {
                return new RespUserModel() { data = new RepInof { code = "0", msg = "用户登录成功", url = "http://localhost:32568/Home/Logined" } };
            }
            else
            {
                var res = Request.Content.ReadAsStringAsync();
                string strRes = res.Result;
                return strRes;
            }
        }


    }
}