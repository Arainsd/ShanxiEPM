using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class TzGcGoodsOrdersItemView
    {
        public string Id { get; set; }
        ///<summary>
        ///标题
        ///</summary>
        public string Title { get; set; }

        ///<summary>
        ///申请单位
        ///</summary>
        public string CompanyName { get; set; }

        ///<summary>
        ///申请日期
        ///</summary>
        public DateTime? ApplyDate { get; set; }

        ///<summary>
        ///项目名称
        ///</summary>
        public string ProjectName { get; set; }

        ///<summary>
        ///物资种类名称
        ///</summary>
        public string MaterialName { get; set; }

        ///<summary>
        ///供应商名称
        ///</summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 当前审批人
        /// </summary>
        public string ApprovalName { get; set; }

        ///<summary>
        ///状态编码
        ///</summary>
        public string StateName { get; set; }

        ///<summary>
		///站名
		///</summary>
		public string StationName { get; set; }

        ///<summary>
        ///品名
        ///</summary>
        public string ProductName { get; set; }

        ///<summary>
        ///规格
        ///</summary>
        public string Specifications { get; set; }

        ///<summary>
        ///数量
        ///</summary>
        public decimal? Number { get; set; }

        ///<summary>
        ///单价
        ///</summary>
        public decimal? UnitPrice { get; set; }

        ///<summary>
        ///金额
        ///</summary>
        public decimal? Amount { get; set; }

        public int? State { get; set; }
    }
}
