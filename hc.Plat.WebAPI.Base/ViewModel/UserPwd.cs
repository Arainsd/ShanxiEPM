using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    public class UserPwd
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        public string oldpwd { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        public string pwd { get; set; }

        /// <summary>
        /// 短信验证码
        /// </summary>
        public string code { get; set; }
    }
}