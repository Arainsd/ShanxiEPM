//------------------------------------------------------------------------------
// <auto-generated>
// 此代码由华春网络代码生成工具生成
// version 1.0
// author hanshiwei 2017.07.24
// auto create time:2019-09-02 11:49:57
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using hc.epm.DataModel.BaseCore;
using System.ComponentModel.DataAnnotations;

namespace hc.epm.DataModel.Business
{ 
	///<summary>
	///Temp_TzFormTalkFile:
	///</summary>
	 public  class  Temp_TzFormTalkFile
	{
        [Key]
        public long Id { get; set; }
        ///<summary>
        ///
        ///</summary>
        public string ProjectId { get; set; }

		///<summary>
		///
		///</summary>
		public string ProjectName { get; set; }

		///<summary>
		///
		///</summary>
		public string ProjectCodeInvest { get; set; }

		///<summary>
		///
		///</summary>
		public string ProjectCodeWhole { get; set; }

		///<summary>
		///
		///</summary>
		public string ProjectCodeProject { get; set; }

		///<summary>
		///
		///</summary>
		public string Header { get; set; }

		///<summary>
		///
		///</summary>
		public string Marker { get; set; }

		///<summary>
		///
		///</summary>
		public string Writer { get; set; }

		///<summary>
		///
		///</summary>
		public string ApplyPerson { get; set; }

		///<summary>
		///
		///</summary>
		public string ApplyPersonID { get; set; }

		///<summary>
		///
		///</summary>
		public string ApplyPersonInvest { get; set; }

		///<summary>
		///
		///</summary>
		public string ApplyPersonWhole { get; set; }

		///<summary>
		///
		///</summary>
		public string ApplyPersonProject { get; set; }

		///<summary>
		///
		///</summary>
		public string ApplytTime { get; set; }

		///<summary>
		///
		///</summary>
		public string OperationTypeType { get; set; }

		///<summary>
		///
		///</summary>
		public string OperationTypeName { get; set; }

		///<summary>
		///
		///</summary>
		public string Remark { get; set; }

		///<summary>
		///
		///</summary>
		public string StateType { get; set; }

		///<summary>
		///
		///</summary>
		public string StateName { get; set; }

		///<summary>
		///
		///</summary>
		public int? State { get; set; }

        public string CreateUserId { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateTime { get; set; }

    }
}
