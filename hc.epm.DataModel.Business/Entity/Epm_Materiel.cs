//------------------------------------------------------------------------------
// <auto-generated>
// 此代码由华春网络代码生成工具生成
// version 1.0
// author hanshiwei 2017.07.24
// auto create time:2018-05-14 14:26:51
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using hc.epm.DataModel.BaseCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace hc.epm.DataModel.Business
{ 
	///<summary>
	///Epm_Materiel:材料设备验收
	///</summary>
	 public  class  Epm_Materiel:BaseBusiness
	{ 
		///<summary>
		///项目表Id
		///</summary>
		public long? ProjectId { get; set; }

		///<summary>
		///项目名称
		///</summary>
		public string ProjectName { get; set; }

		///<summary>
		///业主单位Id
		///</summary>
		public long? CompanyId { get; set; }

		///<summary>
		///业主单位Name
		///</summary>
		public string CompanyName { get; set; }

		///<summary>
		///收货地址
		///</summary>
		public string ReceiveAddress { get; set; }

		///<summary>
		///供货单位Id
		///</summary>
		public long? SupplierId { get; set; }

		///<summary>
		///供货单位Name
		///</summary>
		public string SupplierName { get; set; }

		///<summary>
		///接收人Id
		///</summary>
		public long? ReceiveUserId { get; set; }

		///<summary>
		///接收人Name
		///</summary>
		public string ReceiveUserName { get; set; }

		///<summary>
		///接收单位Id
		///</summary>
		public long? ReceiveCompanyId { get; set; }

		///<summary>
		///接收单位Name
		///</summary>
		public string ReceiveCompanyName { get; set; }

		///<summary>
		///接收时间
		///</summary>
		public DateTime? ReceiveTime { get; set; }

		///<summary>
		///状态[10待处理,25确认通过,30已驳回,40已废弃]枚举
		///</summary>
		public int? State { get; set; }

		///<summary>
		///备注
		///</summary>
		public string Remark { get; set; }

        /// <summary>
        /// 甲供物资单名称
        /// </summary>
        public string SupMatApplyName { get; set; }

        /// <summary>
        /// 甲供物资单id
        /// </summary>
        public long? SupMatApplyId { get; set; }
    }
}

