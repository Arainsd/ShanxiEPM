using hc.epm.ViewModel;
using hc.Plat.WebAPI.Base.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hc.epm.Web.Models
{
    public class LogAtillsoners
    {
        /// <summary>
        /// 监理日志施工单位
        /// </summary>
        /// <summary>
        /// 单位 ID
        /// </summary>
        public long? companyId { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string companyName { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string managerName { get; set; }
        /// <summary>
        /// 到场许可
        /// </summary>
        public string permit { get; set; }
        /// <summary>
        /// 是否到场
        /// </summary>
        public string bepresent { get; set; }
        /// <summary>
        /// 工种编号ID
        /// </summary>
        public string workPeopleTypeId { get; set; }
        /// <summary>
        /// 工种称呼
        /// </summary>
        public string workPeopleTypeName { get; set; }
        /// <summary>
        /// 工种编号ID
        /// </summary>
        public long? ManagerId { get; set; }





    }
}