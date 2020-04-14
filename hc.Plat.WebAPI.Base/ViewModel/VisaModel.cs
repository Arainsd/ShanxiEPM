using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 签证
    /// </summary>
    public class VisaModel
    {
        /// <summary>
        /// 所属项目 ID
        /// </summary>
        public long projectId { get; set; }

        /// <summary>
        /// 所属项目名称
        /// </summary>
        public string projectName { get; set; }

        /// <summary>
        /// 签证标题
        /// </summary>
        public string title { get; set; }
        
        /// <summary>
        /// 签证类型名称
        /// </summary>
        public string typeName { get; set; }

        /// <summary>
        /// 签证号
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 签证内容
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 签证原因
        /// </summary>
        public string reason { get; set; }

        /// <summary>
        /// 签证金额
        /// </summary>
        public decimal amount { get; set; }

        /// <summary>
        /// 签证开始时间
        /// </summary>
        public DateTime startTime { get; set; }

        /// <summary>
        /// 签证结束时间
        /// </summary>
        public DateTime endTime { get; set; }
    }
}