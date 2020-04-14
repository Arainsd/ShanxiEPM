using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace hc.epm.ViewModel
{
    ///<summary>
    ///XtTzSupplyMaterialApplyView:甲供物资申请
    ///</summary>
    public class XtTzSupplyMaterialApplyView
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string ApplyTitle { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string ApplyUserName { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 申请部门
        /// </summary>
        public string ApplyDepartment { get; set; }
        /// <summary>
        /// 分公司
        /// </summary>
        public string ApplyCompanyName { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 库站名称
        /// </summary>
        public string StationName { get; set; }
        /// <summary>
        /// 批复文号
        /// </summary>
        public string ApprovalNo { get; set; }
        /// <summary>
        /// 合同名称
        /// </summary>
        public string ContractName { get; set; }
        /// <summary>
        /// 合同报审序号
        /// </summary>
        public string ContractNumber { get; set; }
        /// <summary>
        /// erp订单号
        /// </summary>
        public string ErpCode { get; set; }
        /// <summary>
        /// 到货联系人
        /// </summary>
        public string ArrivalContacts { get; set; }
        /// <summary>
        /// 到货联系地址
        /// </summary>
        public string ArrivalAddress { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string Supplier { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string SupplierCode { get; set; }
        /// <summary>
        /// 供应商联系人
        /// </summary>
        public string SupplierContacts { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string SupplierTel { get; set; }
        /// <summary>
        /// 供应商地址
        /// </summary>
        public string SupplierAddress { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public string Money { get; set; }
        /// <summary>
        /// 分管领导
        /// </summary>
        public string LeadershipName { get; set; }
        /// <summary>
        /// 到货联系人电话
        /// </summary>
        public string ArrivalContactsTel { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string hr_sqr { get; set; }


        public List<SupMatApplyListItem> list { get; set; }

        public class SupMatApplyListItem
        {
            /// <summary>
            /// 物资种类
            /// </summary>
            public string MaterialCategory { get; set; }
            /// <summary>
            /// 品名
            /// </summary>
            public string ProductName { get; set; }
            /// <summary>
            /// 规格
            /// </summary>
            public string Specification { get; set; }
            /// <summary>
            /// 单价
            /// </summary>
            public string UnitPrice { get; set; }
            /// <summary>
            /// 金额
            /// </summary>
            public string Moneys { get; set; }
            /// <summary>
            /// 数量
            /// </summary>
            public string CLNumber { get; set; }
        }
    }
}

