using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 通用权限操作模型
    /// </summary>
    public class BusinessCheck
    {
        /// <summary>
        /// 
        /// </summary>
        public BusinessCheck()
        {
            waitDo = "2";
        }
        /// <summary>
        /// 权限 ID
        /// </summary>
        public string rightId { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string businessType { get; set; }

        /// <summary>
        /// 操作权限编码
        /// </summary>
        public string rightAction { get; set; }

        /// <summary>
        /// 所属项目
        /// </summary>
        public long projectId { get; set; }

        /// <summary>
        /// 所属业务
        /// </summary>
        public long businessId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string reason { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string waitDo { get; set; }
    }
}