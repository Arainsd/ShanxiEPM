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
namespace hc.epm.DataModel.Business
{ 
	///<summary>
	///Epm_News:新闻、资讯表
	///</summary>
	 public  class  Epm_News:BaseBusiness
	{ 
		///<summary>
		///新闻类型Key
		///</summary>
		public long? NewsTypeId { get; set; }

		///<summary>
		///新闻类型Value
		///</summary>
		public string NewsTypeName { get; set; }

		///<summary>
		///标题
		///</summary>
		public string NewsTitle { get; set; }

		///<summary>
		///摘要
		///</summary>
		public string ShortDesc { get; set; }

		///<summary>
		///来源
		///</summary>
		public string Source { get; set; }

		///<summary>
		///排序
		///</summary>
		public int? Sort { get; set; }

		///<summary>
		///是否置顶 1 是 0 否
		///</summary>
		public bool? IsTop { get; set; }

		///<summary>
		///是否发布 1 是 0 否
		///</summary>
		public bool? IsPublish { get; set; }

		///<summary>
		///发布人Id
		///</summary>
		public long? PublishUserId { get; set; }

		///<summary>
		///发布人Name
		///</summary>
		public string PublishUserName { get; set; }

		///<summary>
		///发布时间
		///</summary>
		public DateTime? PublishTime { get; set; }

		///<summary>
		///发布内容
		///</summary>
		public string Content { get; set; }

		///<summary>
		///状态
		///</summary>
		public int? State { get; set; }

		///<summary>
		///备注
		///</summary>
		public string Remark { get; set; }

		///<summary>
		///创建单位Id
		///</summary>
		public long? CrtCompanyId { get; set; }

		///<summary>
		///创建单位Name
		///</summary>
		public string CrtCompanyName { get; set; }

        /// <summary>
        /// 是否图片显示
        /// </summary>
        public bool? IsImgShow { get; set; }
    }
}
