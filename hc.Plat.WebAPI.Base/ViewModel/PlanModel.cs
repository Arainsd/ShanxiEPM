using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 施工计划
    /// </summary>
    public class PlanModel
    {
        public PlanModel()
        {
            child = new List<PlanModel>();
        }

        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 计划名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string startTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string endTime { get; set; }

        /// <summary>
        /// 工期
        /// </summary>
        public string limit { get; set; }

        /// <summary>
        /// 施工状态(1: 未完成（计划），2 提前完成，3 延期完成，4 未完成（延期），5 正常完工)
        /// </summary>
        public string limitState { get; set; }

        /// <summary>
        /// 子级计划
        /// </summary>
        public List<PlanModel> child { get; set; }
    }
}