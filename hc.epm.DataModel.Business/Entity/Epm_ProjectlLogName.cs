//------------------------------------------------------------------------------
// <auto-generated>
// 此代码由华春网络代码生成工具生成
// version 1.0
// author hanshiwei 2017.07.24
// auto create time:2018-08-15 09:04:34
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using hc.epm.DataModel.BaseCore;
namespace hc.epm.DataModel.Business
{
    ///<summary>
    ///Epm_ProjectlLogName:
    ///</summary>
    public class Epm_ProjectlLogName : BaseBusiness
    {

        ///<summary>
        ///项目ID
        ///</summary>
        public long? projectid { get; set; }
        ///<summary>
        //姓名ID
        ///</summary>
        public long? personid { get; set; }
        ///<summary>
        ///姓名
        ///</summary>
        public string name { get; set; }
        ///<summary>
        ///类型
        ///</summary>
        public string type { get; set; }
        /// <summary>
        /// 供应商ID
        /// </summary>
        public long? companyid { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string companyname { get; set; }
        /// <summary>
        /// 详情ID
        /// </summary>
        public long? detailsid { get; set; }
        ///<summary>
        ///电话
        ///</summary>
        public string phone { get; set; }

    }
}
