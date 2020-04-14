using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class PlanView
    {
        public List<Epm_PlanComponent> EpmPlanComponent { get; set; }
        public Epm_Plan Plan { get; set; }
        public long Id { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        public long ProjectId { get; set; }

        public long SupervisorLogDetailsId { get; set; }
        ///<summary>
		///父级计划表Id
		///</summary>
		public long? ParentId { get; set; }

        ///<summary>
        ///计划名称
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        ///计划开始时间
        ///</summary>
        public DateTime? StartTime { get; set; }

        ///<summary>
        ///计划截止时间
        ///</summary>
        public DateTime? EndTime { get; set; }

        ///<summary>
        ///实际开始时间
        ///</summary>
        public DateTime? FactStartTime { get; set; }

        ///<summary>
        ///实际截止时间
        ///</summary>
        public DateTime? FactEndTime { get; set; }

        /// <summary>
        /// 计划延期完工时间
        /// </summary>
        public DateTime? DelayTime { get; set; }

        ///<summary>
		///是否完成[1完成，2未完成]枚举
		///</summary>
		public int? IsFinish { get; set; }

        ///<summary>
		///完成比,0-100之间
		///</summary>
		public string FinishScale { get; set; }

        ///<summary>
        ///未完成原因
        ///</summary>
        public string ToResean { get; set; }

        ///<summary>
        ///工期，单位天
        ///</summary>
        public decimal? BuildDays { get; set; }

        ///<summary>
		///施工负责人Id
		///</summary>
		public long? ContactUserId { get; set; }

        ///<summary>
        ///施工负责人Name
        ///</summary>
        public string ContactUserName { get; set; }

        ///<summary>
        ///施工单位Id
        ///</summary>
        public long? BuildCompanyId { get; set; }

        ///<summary>
        ///施工单位Name
        ///</summary>
        public string BuildCompanyName { get; set; }

        ///<summary>
		///状态[10待处理,20审核通过,30已驳回,40已废弃]枚举
		///</summary>
		public int? State { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNo { get; set; }

        ///<summary>
		///关键里程碑Name
		///</summary>
		public string MilepostName { get; set; }
        public string MilepostId { get; set; }

        /// <summary>
        /// 计划说明
        /// </summary>
        public string PlanContent { get; set; }

        public string WBS { get; set; }

        public string OutlineParent { get; set; }

        public int iTaskLevel { get; set; }

        /// <summary>
        /// 创建人名称
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public long CreateUserId { get; set; }
        /// <summary>
        /// 复选框的值
        /// </summary>
        public string checkboxValue { get; set; }

        public List<PlanView> children { get; set; }
    }
}
