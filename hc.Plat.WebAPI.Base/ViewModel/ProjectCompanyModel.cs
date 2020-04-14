using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 项目服务商
    /// </summary>
    public class ProjectCompanyModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long linkManId { get; set; }
        /// <summary>
        /// 项目负责人
        /// </summary>
        public string linkMan { get; set; }

        /// <summary>
        /// 项目负责人电话
        /// </summary>
        public string linkPhone { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public long pmManId { get; set; }
        /// <summary>
        /// 项目经理
        /// </summary>
        public string pmMan { get; set; }

        /// <summary>
        /// 项目经理电话
        /// </summary>
        public string pmPhone { get; set; }
    }
}