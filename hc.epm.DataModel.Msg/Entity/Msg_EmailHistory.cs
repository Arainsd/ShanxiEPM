//------------------------------------------------------------------------------
// <auto-generated>
// 此代码由华春网络代码生成工具生成
// version 1.0
// author hanshiwei 2017.07.24
// auto create time:2017-08-14 10:19:39
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using hc.epm.DataModel.BaseCore;
namespace hc.epm.DataModel.Msg
{ 
	///<summary>
	///Msg_EmailHistory:历史邮件
	///</summary>
	 public  class  Msg_EmailHistory: BaseMsg
    { 
		///<summary>
		///邮件所用模板
		///</summary>
		public long TemplateId { get; set; }

		///<summary>
		///消息所属环节
		///</summary>
		public string Step { get; set; }

		///<summary>
		///邮件标题
		///</summary>
		public string Title { get; set; }

		///<summary>
		///邮件内容
		///</summary>
		public string EmailCon { get; set; }

		///<summary>
		///邮件发送者
		///</summary>
		public long SenderId { get; set; }

		///<summary>
		///发送者邮箱
		///</summary>
		public string SenderEmail { get; set; }

		///<summary>
		///邮件接收者
		///</summary>
		public long ReceiveId { get; set; }

		///<summary>
		///接收人邮箱
		///</summary>
		public string ReceiveEmaile { get; set; }

		///<summary>
		///邮件发送时间
		///</summary>
		public DateTime? SenderTime { get; set; }

		///<summary>
		///邮件提交时间
		///</summary>
		public DateTime SubmissionTime { get; set; }

		///<summary>
		///邮件发送状态 0为发送成功，1为发送失败
		///</summary>
		public bool States { get; set; }

	}
}
