using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// TopIpoutGetResponse.
    /// </summary>
    public class TopIpoutGetResponse : TopResponse
    {
        /// <summary>
        /// 出口IP段列表
        /// </summary>
        [XmlArray("ip_sections")]
        [XmlArrayItem("string")]
        public List<string> IpSections { get; set; }

    }
}
