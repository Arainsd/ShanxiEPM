using hc.epm.ViewModel;
using System;
using System.Collections.Generic;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 监理日志
    /// </summary>
    public class SuperSaveinfo
    {
        /// <summary>
        /// 监理日志施工单位
        /// </summary>
        /// <summary>
        /// 单位 ID
        /// </summary>
        public long companyId { get; set; }
        /// <summary>
        /// 详情ID
        /// </summary>
        public long logid { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string managerName { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string companyName { get; set; }

        /// <summary>
        /// 到场人员
        /// </summary>
        public List<PeoplesView> personnelList { get; set; }
        /// <summary>
        /// 工种信息
        /// </summary>
        public List<worktypelist> workPersonnel { get; set; }


    }

}