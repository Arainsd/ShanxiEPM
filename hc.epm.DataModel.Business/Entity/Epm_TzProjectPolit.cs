/************************************************************************************
 * Copyright (c) 2019  All Rights Reserved.
 * CLR版本： 4.0.30319.42000
 * 公司名称：陕西华春网络科技股份有限公司
 * 命名空间：hc.epm.DataModel.Business
 * 文件名：  Epm_TzProjectApproval
 * 版本号：  V1.0.0.0
 * 创建人：  wmg	
 * 电子邮箱：wmgwugang@huachun.com
 * 创建时间：2019/8/28 11:00:44
 * 描述：
 * 
 * 
 * 
 ************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hc.epm.DataModel.BaseCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace hc.epm.DataModel.Business
{
    /// <summary>
    /// 加油站试运行申请
    /// </summary>
    public class Epm_TzProjectPolit : BaseBusiness
    {

        public Epm_TzProjectPolit()
        {
            TzAttachs = new List<Epm_TzAttachs>();
        }
        ///<summary>
        ///
        ///</summary>
        public long? ProjectId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        ///<summary>
        ///
        ///</summary>
        public long? CompanyId { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string CompanyName { get; set; }

        ///<summary>
        ///
        ///</summary>
        public DateTime? StartDate { get; set; }

        ///<summary>
        ///
        ///</summary>
        public DateTime? EndDate { get; set; }

        ///<summary>
        ///
        ///</summary>
        public DateTime? AcceptDate { get; set; }

        ///<summary>
        ///
        ///</summary>
        public DateTime? RectFinishDate { get; set; }

        ///<summary>
        ///
        ///</summary>
        public DateTime? FinalDate { get; set; }

        ///<summary>
        ///
        ///</summary>
        public DateTime? AuditDate { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string FullFiles { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string AcceptOpinion { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string Remark { get; set; }

        ///<summary>
        /// 状态 参考枚举：XtBusinessDataState
        ///</summary>
        public int? State { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        public string StateType { get; set; }

        /// <summary>
        /// 流程状态名称
        /// </summary>
        public string StateName { get; set; }

        /// <summary>
        /// 审批人
        /// </summary>
        public string ApprovalName { get; set; }

        /// <summary>
        /// 审批人Id
        /// </summary>
        public long? ApprovalNameId { get; set; }


        /// <summary>
        /// 审批流程 ID
        /// </summary>
        public string WorkFlowId { get; set; }

        [NotMapped]
        public List<Epm_TzAttachs> TzAttachs { get; set; }
    }
}
