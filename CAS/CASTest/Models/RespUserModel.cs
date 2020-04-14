using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CASTest.Models
{
    public class RespUserModel
    {

        public RepInof data { get; set; }
    }

    public class RepInof
    {
        public string code { get; set; }
        public string msg { get; set; }
        public string url { get; set; }
    }
}