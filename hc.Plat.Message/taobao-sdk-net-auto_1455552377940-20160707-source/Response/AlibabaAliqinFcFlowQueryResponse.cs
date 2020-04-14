using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaAliqinFcFlowQueryResponse.
    /// </summary>
    public class AlibabaAliqinFcFlowQueryResponse : TopResponse
    {
        /// <summary>
        /// 无
        /// </summary>
        [XmlElement("value")]
        public ResultDomain Value { get; set; }

	/// <summary>
	/// ResultDomain Data Structure.
	/// </summary>
	[Serializable]
	public class ResultDomain : TopObject
	{
	        /// <summary>
	        /// 错误码
	        /// </summary>
	        [XmlElement("code")]
	        public string Code { get; set; }
	
	        /// <summary>
	        /// "id":"唯一流水号",     "time":"提交时间",     "phone":"号码",     "error":"false",     "reason":"原因",     "status":"充值状态",     "flow":"流量",     "operator":"中国移动"
	        /// </summary>
	        [XmlElement("model")]
	        public string Model { get; set; }
	
	        /// <summary>
	        /// 原因
	        /// </summary>
	        [XmlElement("msg")]
	        public string Msg { get; set; }
	
	        /// <summary>
	        /// 状态
	        /// </summary>
	        [XmlElement("success")]
	        public bool Success { get; set; }
	}

    }
}
