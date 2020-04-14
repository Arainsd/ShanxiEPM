using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class TzPeopleChgApplyItemView
    {
        public long Id { get; set; }
        ///<summary>
        ///标题
        ///</summary>
        public string Title { get; set; }

        ///<summary>
		///申请人
		///</summary>
		public string Applicant { get; set; }

        ///<summary>
        ///所属单位
        ///</summary>
        public string CompanyName { get; set; }
        
        ///<summary>
        ///所属部门
        ///</summary>
        public string Department { get; set; }

        ///<summary>
        ///申请日期
        ///</summary>
        public DateTime? ApplyDate { get; set; }

        ///<summary>
        ///项目名称
        ///</summary>
        public string ProjectName { get; set; }

        ///<summary>
        ///建设地址
        ///</summary>
        public string ConstructionAddress { get; set; }

        ///<summary>
        ///施工（监理）单位
        ///</summary>
        public string WorkUnit { get; set; }

        ///<summary>
		///施工(监理)负责人
		///</summary>
		public string WorkLeader { get; set; }

        ///<summary>
		///所属申请ID
		///</summary>
		public long? ChangeApplyId { get; set; }

        ///<summary>
		///变更岗位
		///</summary>
		public string ChgPost { get; set; }

        ///<summary>
        ///变更前人员
        ///</summary>
        public string ChgPeopleBefore { get; set; }

        ///<summary>
        ///变更后人员
        ///</summary>
        public string ChgPeopleAfter { get; set; }

        ///<summary>
        ///变更前执业证书名称
        ///</summary>
        public string ChgCertiNameBefore { get; set; }

        ///<summary>
        ///变更前执业证书号
        ///</summary>
        public string ChgCertiNoBefore { get; set; }

        ///<summary>
        ///变更后执业证书名称
        ///</summary>
        public string ChgCertiNameAfter { get; set; }

        ///<summary>
        ///变更后执业证书号
        ///</summary>
        public string ChgCertiNoAfter { get; set; }

        ///<summary>
		///当前状态
		///</summary>
		public string StateType { get; set; }

        ///<summary>
        ///状态编码
        ///</summary>
        public string StateName { get; set; }

        /// <summary>
        /// 当前审核人
        /// </summary>
        public string ApprovalName { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        public string Leader { get; set; }
        
        public int State { get; set; }
    }
}
