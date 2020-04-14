using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 用户登录信息数据格式
    /// </summary>
    public class UserInfoView
    {
        /// <summary>
        /// userId（用户ID）
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// 请求日期
        /// </summary>
        public string reqDate { get; set; }
        /// <summary>
        /// 请求时间
        /// </summary>
        public string reqTime { get; set; }

        /// <summary>
        /// 用户信息（用户名、密码）
        /// </summary>
        public class data
        {
            /// <summary>
            /// 用户名
            /// </summary>
            public string UserCode { get; set; }
            /// <summary>
            /// 用户密码
            /// </summary>
            public string PassWord { get; set; }

        }
    }
}