using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 工程内容要点
    /// </summary>
    public class ProjectWorkPointModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 所属项目
        /// </summary>
        public long projectId { get; set; }

        /// <summary>
        /// 所属项目名称
        /// </summary>
        public string projectName { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 厂家
        /// </summary>
        public string companyName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal num { get; set; }

        /// <summary>
        /// 是否计费
        /// </summary>
        public bool isCharging { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int sort { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }

        public string value { get; set; }
    }
}