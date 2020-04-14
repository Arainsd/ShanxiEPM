using System;
using System.Collections.Generic;
using hc.epm.DataModel.BaseCore;
namespace hc.epm.DataModel.Business
{
    //<summary>
    ///Bp_SendDate:
    ///</summary>
    public class Bp_SendDate : BaseBusiness
    {
        ///<summary>
        ///
        ///</summary>
        public string Project { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string UserName { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string KeyValue { get; set; }
        public string KeyName { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string Type { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string Group { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string Key { get; set; }

        ///<summary>
        ///
        ///</summary>
        public string Value { get; set; }

        ///<summary>
        ///
        ///</summary>
        public bool? IsSend { get; set; }

        ///<summary>
        ///
        ///</summary>
        public DateTime? SendTime { get; set; }

    }
}
