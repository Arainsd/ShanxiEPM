using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 项目工期明细
    /// </summary>
    public class ProjectDurationModel
    {
        /// <summary>
        /// 项目 ID
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 实际停业时间
        /// </summary>
        public DateTime? shutDownTime { get; set; }

        /// <summary>
        /// 计划开工时间
        /// </summary>
        public DateTime? planStartTime { get; set; }

        /// <summary>
        /// 计划完工时间
        /// </summary>
        public DateTime? planEndTime { get; set; }

        /// <summary>
        /// 工期
        /// </summary>
        public int? limit { get; set; }

        /// <summary>
        /// 计划开业时间
        /// </summary>
        public DateTime? planOpenTime { get; set; }

        /// <summary>
        /// 计划停业时长
        /// </summary>
        public int? planShutdownLimit { get; set; }

        /// <summary>
        /// 计划包装开工时间
        /// </summary>
        public DateTime? planPackStartTime { get; set; }

        /// <summary>
        /// 计划包装结束时间
        /// </summary>
        public DateTime? planPackEndTime { get; set; }

        /// <summary>
        /// 计划加固开工时间
        /// </summary>
        public DateTime? planReinforceStartTime { get; set; }

        /// <summary>
        /// 计划加固结束时间
        /// </summary>
        public DateTime? planReinforceEndTime { get; set; }
    }
}