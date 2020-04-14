using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class TzDevResourceReportItemView
    {
        public long Id { get; set; }

        ///<summary>
		///所属申请ID
		///</summary>
		public long? ApplyId { get; set; }
        ///<summary>
		///地市
		///</summary>
		public string Cities { get; set; }

        ///<summary>
        ///区县
        ///</summary>
        public string County { get; set; }

        ///<summary>
        ///项目名称
        ///</summary>
        public string ProjectName { get; set; }

        ///<summary>
        ///项目位置
        ///</summary>
        public string ProjectAdress { get; set; }

        ///<summary>
        ///项目性质
        ///</summary>
        public string ProjectTypeName { get; set; }

        ///<summary>
        ///预计总投资额（万元）
        ///</summary>
        public decimal? ProjectedInvestment { get; set; }

        ///<summary>
        ///可研销售
        ///</summary>
        public decimal? ResearchSales { get; set; }

        ///<summary>
        ///汽柴比
        ///</summary>
        public decimal? GasFuelRatio { get; set; }

        ///<summary>
        ///列为意向站时间
        ///</summary>
        public DateTime? FixHour { get; set; }

        ///<summary>
        ///计划立项时间
        ///</summary>
        public DateTime? PlanningTime { get; set; }

        ///<summary>
        ///业主姓名
        ///</summary>
        public string OwnerName { get; set; }

        ///<summary>
        ///业主电话
        ///</summary>
        public string OwnerPhone { get; set; }

        ///<summary>
        ///备注
        ///</summary>
        public string Remark { get; set; }

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

        public int State { get; set; }
    }
}
