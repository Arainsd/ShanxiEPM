using hc.epm.ViewModel;
using System;
using System.Collections.Generic;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 监理日志
    /// </summary>
    public class worktypelist
    {
       
        /// <summary>
        /// 工种
        /// </summary>
        public string workPeopleType { get; set; }
        /// <summary>
        /// 工种姓名
        /// </summary>
        public string workPeopleTypeName { get; set; }
        /// <summary>
        /// 工种ID
        /// </summary>
        public string workPeopleTypeId { get; set; }
        /// <summary>
        /// 许可
        /// </summary>
        public string permit { get; set; }
        /// <summary>
        /// 到场
        /// </summary>
        public string bePresent { get; set; }
        /// <summary>
        /// type
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// name
        /// </summary>
        public string name { get; set; }
    }

}