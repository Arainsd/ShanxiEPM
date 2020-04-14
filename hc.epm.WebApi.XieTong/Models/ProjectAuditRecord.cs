using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hc.epm.WebAPI.Models
{
    public class ProjectAuditRecord
    {
        /// <summary>
        /// 所属申请(协同传过来的是他们的流程申请 ID)
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 审批步骤ID
        /// </summary>
        public string StepId { get; set; }
        /// <summary>
        /// 审批步骤名称
        /// </summary>
        public string StepName { get; set; }
        /// <summary>
        /// 审批人编码
        /// </summary>
        public string AuditUserCode { get; set; }
        /// <summary>
        /// 审批人
        /// </summary>
        public string AuditName { get; set; }
        /// <summary>
        /// 审批结果
        /// </summary>
        public string AuditState { get; set; }
        /// <summary>
        /// 审批时间
        /// </summary>
        public string AuditTime { get; set; }
        /// <summary>
        /// 审批意见
        /// </summary>
        public string AuditRemark { get; set; }
        /// <summary>
        /// 签章（备用）
        /// </summary>
        public string AuditSign { get; set; }

        /// <summary>
        /// 下一步审批人编码
        /// </summary>
        public string NextAuditUserCode { get; set; }

        /// <summary>
        /// 下一步审批人
        /// </summary>
        public string NextAuditName { get; set; }

        /// <summary>
        /// 下一步审批步骤ID
        /// </summary>
        public string NextStepId { get; set; }

        /// <summary>
        /// 下一步审批步骤名称
        /// </summary>
        public string NextStepName { get; set; }

        /// <summary>
        /// 审批最终状态
        /// </summary>
        public int ApprovalState { get; set; }
    }
}