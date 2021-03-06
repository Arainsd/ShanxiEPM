//------------------------------------------------------------------------------
// <auto-generated>
// 此代码由华春网络代码生成工具生成
// version 1.0
// author hanshiwei 2017.07.24
// auto create time:2019-01-17 16:19:50
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using hc.epm.DataModel.BaseCore;
namespace hc.epm.DataModel.Business
{
    ///<summary>
    ///Bp_Project:
    ///</summary>
    public class Bp_Project : BaseBusiness
    {
        ///<summary>
        ///
        ///</summary>
        public string HB_ID { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string Code { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        /// 01：新建站；02：租赁改造；03：收购改造；04：拆迁重建；05：改扩建；06：检维修；07：安全隐患治理；08：技术改造；09：油气回收；
        ///</summary>
        public string ProjectNatureKey { get; set; }

        ///<summary>
        /// 01：新建站；02：租赁改造；03：收购改造；04：拆迁重建；05：改扩建；06：检维修；07：安全隐患治理；08：技术改造；09：油气回收；
        ///</summary>
        public string ProjectNatureValue { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string HB_OilStationID { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string OilStationName { get; set; }

        ///<summary>
        /// 本省省
        ///</summary>
        public string Province { get; set; }

        ///<summary>
        /// 武汉市
        ///</summary>
        public string City { get; set; }

        ///<summary>
        /// 江岸区
        ///</summary>
        public string Area { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string Address { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string Amount { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string Description { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string GasolineDieselRatio { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string ReplyNumber { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string HB_PMID { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string PMName { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string PMPhone { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string HB_CompanyID { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string CompanyName { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string HB_ContactUserID { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string ContactUserName { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string ContactPhone { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string State { get; set; }

    }
}

