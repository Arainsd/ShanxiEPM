using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hc.Plat.FileServer.Web.Models
{
    [Serializable]
    public class BidFilePathView
    {
        /// <summary>
        /// 直传文件路路径
        /// </summary>
        public string SourcePath { get; set; }
        /// <summary>
        /// 加密文件路径
        /// </summary>
        public string EncPath { get; set; }
        /// <summary>
        /// 解密文件路径
        /// </summary>
        public string DecPath { get; set; }
    }
}