using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hc.epm.Admin.Web.Models
{
    public class SelectCommanyModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string IsMultiple { get; set; } = "true";
        public int pageIndex { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        /// <summary>
        /// 默认选中项
        /// </summary>
        public string SelectIds { get; set; }

    }
}