using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class ProjectView
    {
        /// <summary>
        /// 项目Id
        /// </summary>
        public long Id { get; set; }
        ///<summary>
		///编码
		///</summary>
		public string Code { get; set; }

        ///<summary>
        ///项目名称
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        ///省
        ///</summary>
        public string Province { get; set; }

        ///<summary>
        ///市
        ///</summary>
        public string City { get; set; }

        ///<summary>
        ///区
        ///</summary>
        public string Area { get; set; }

        ///<summary>
        ///地址
        ///</summary>
        public string Address { get; set; }

        ///<summary>
        ///项目开始时间
        ///</summary>
        public DateTime? StartDate { get; set; }

        ///<summary>
        ///项目结束时间
        ///</summary>
        public DateTime? EndDate { get; set; }

        ///<summary>
        ///项目金额，单位万元
        ///</summary>
        public decimal? Amount { get; set; }

        ///<summary>
        ///项目负责人Id
        ///</summary>
        public long? ContactUserId { get; set; }

        ///<summary>
        ///项目负责人Name
        ///</summary>
        public string ContactUserName { get; set; }

        ///<summary>
        ///负责人电话
        ///</summary>
        public string ContactPhone { get; set; }

        /// <summary>
        /// 监理负责人
        /// </summary>
        public string SupervisorUserName { get; set; }

        /// <summary>
        /// 监理负责人电话
        /// </summary>
        public string SupervisorPhone { get; set; }

        /// <summary>
        /// 施工负责人
        /// </summary>
        public string BuildUserName { get; set; }

        /// <summary>
        /// 施工负责人电话
        /// </summary>
        public string BuildPhone { get; set; }

        ///<summary>
        ///状态
        ///</summary>
        public int? State { get; set; }

        /// <summary>
        /// 计划进度
        /// </summary>
        public string ScheduleRatio { get; set; }

        /// <summary>
        /// 质量检查
        /// </summary>
        public string QualityCheckNum { get; set; }

        /// <summary>
        /// 安全检查
        /// </summary>
        public string SecurityCheckNum { get; set; }

        /// <summary>
        /// 监理日志
        /// </summary>
        public string SupervisorLogNum { get; set; }

        /// <summary>
        /// 问题沟通
        /// </summary>
        public string ProblemNum { get; set; }

        public DateTime OperateTime { get; set; }

        public long? CompanyId { get; set; }
        ///<summary>
        ///所属分公司
        ///</summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 项目经理
        /// </summary>
        public string PMName { get; set; }

        /// <summary>
        /// 项目性质
        /// </summary>
        public string ProjectNatureName { get; set; }

        /// <summary>
        /// 计划开工日期
        /// </summary>
        public DateTime? PlanWorkStartTime { get; set; }
        /// <summary>
        /// 计划完工日期
        /// </summary>
        public DateTime? PlanWorkEndTime { get; set; }
        /// <summary>
        /// 工期
        /// </summary>
        public int? Limit { get; set; }
        /// <summary>
        /// 实际消耗工期：当前日期-开工时间+1
        /// </summary>
        public int FinalLimit { get; set; }
        /// <summary>
        /// 剩余工期：合同工期-实际消耗工期
        /// </summary>
        public int SurplusLimit { get; set; }

        /// <summary>
        /// 工程进度：实际消耗工期/合同工期*100%，保留整数
        /// </summary>
        public int WorkSchedule { get; set; }

        /// <summary>
        /// 施工单位：工程服务商中的土建服务商
        /// </summary>
        public string WorkUnit { get; set; }
        /// <summary>
        /// 施工单位项目经理
        /// </summary>
        public string WorkUnitPMName { get; set; }
        /// <summary>
        /// 监理单位
        /// </summary>
        public string SupervisorUnit { get; set; }
        /// <summary>
        /// 监理工程师
        /// </summary>
        public string SupervisorUnitName { get; set; }
        /// <summary>
        /// 投资概算（总批复金额）
        /// </summary>
        public decimal? InvestMoney { get; set; }
        /// <summary>
        /// 项目批复下达时间
        /// </summary>
        public DateTime? ReplyDate { get; set; }
        /// <summary>
        /// 省公司验收时间
        /// </summary>
        public DateTime? RecTime { get; set; }
        /// <summary>
        /// 设计单位
        /// </summary>
        public string DesignUnit { get; set; }

        public long TzProjectId { get; set; }

        /// <summary>
        /// 可研销量
        /// </summary>
        public decimal? GasDailySales { get; set; }

        //投资文号
        public string ApprovalNo { get; set; }
        //落地时间
        public DateTime? ReplyTime { get; set; }
        //结算完成时间和转资时间
        public DateTime? FinanceTime { get; set; }
        //施工图设计完成时间
        public DateTime? DesignSchemeTime { get; set; }

        //招标完成时间
        public DateTime? BidResultTime { get; set; }
        //投运时间
        public DateTime? ProjectPolitTime { get; set; }
        //实际消耗工期
        public int ConsumptionPeriod { get; set; }

        //项目现状
        public string ProjectState { get; set; }
        

    }
}
