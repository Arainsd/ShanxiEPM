using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hc.Plat.WebAPI.Base.Models
{
    public class User
    {
        public string userid { get; set; }
        public string username { get; set; }
        public string img { get; set; }
        public string phone { get; set; }
        public string companyname { get; set; }
        public string roleid { get; set; }
        public string rolename { get; set; }
        public string qq { get; set; }
        public string wechat { get; set; }
        public string mail { get; set; }
    }
}