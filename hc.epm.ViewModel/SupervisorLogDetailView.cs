using System;
using System.Collections.Generic;
using hc.epm.DataModel.BaseCore;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;

namespace hc.epm.ViewModel
{
    /// <summary>
    /// 监理日志明细
    /// </summary>
    public class SupervisorLogDetailView : BaseBusiness
    {
        public SupervisorLogDetailView()
        {
            ProjectData = new List<Epm_ProjectData>();
            Attachs = new List<Base_Files>();
        }

        public List<Base_Files> Attachs { get; set; }

        /// <summary>
        /// 项目资料表
        /// </summary>
        public List<Epm_ProjectData> ProjectData { get; set; }

        ///<summary>
        ///日志表Id
        ///</summary>
        public long? LogId { get; set; }

        ///<summary>
        ///项目表Id
        ///</summary>
        public long? ProjectId { get; set; }

        ///<summary>
        ///项目名称
        ///</summary>
        public string ProjectName { get; set; }

        ///<summary>
        ///施工计划表Id
        ///</summary>
        public long? PlanId { get; set; }

        ///<summary>
        ///计划Name
        ///</summary>
        public string PlanName { get; set; }

        ///<summary>
        ///是否完成[1完成，2未完成]枚举
        ///</summary>
        public int? IsFinish { get; set; }

        ///<summary>
        ///实际开始时间
        ///</summary>
        public DateTime? StartTime { get; set; }

        ///<summary>
        ///实际结束时间
        ///</summary>
        public DateTime? EndTime { get; set; }

        ///<summary>
        ///完成比,0-100之间
        ///</summary>
        public decimal? FinishScale { get; set; }

        ///<summary>
        ///未完成原因
        ///</summary>
        public string ToResean { get; set; }

        ///<summary>
        ///状态
        ///</summary>
        public int? State { get; set; }

        ///<summary>
        ///备注
        ///</summary>
        public string Remark { get; set; }

        ///<summary>
        ///创建单位Id
        ///</summary>
        public long? CrtCompanyId { get; set; }

        ///<summary>
        ///创建单位Name
        ///</summary>
        public string CrtCompanyName { get; set; }

        /// <summary>
        /// 天气类型
        /// </summary>
        public string TypeNo { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string TypeName { get; set; }


        /// <summary>
        /// 提交日期
        /// </summary>
        public DateTime? SubmitTime { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string Content { get; set; }
    }
}
