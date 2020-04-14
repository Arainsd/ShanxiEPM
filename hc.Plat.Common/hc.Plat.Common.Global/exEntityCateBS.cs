using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace hc.Plat.Common.Global
{
    [DataContract]
    [Serializable]
    public class exEntityCateBS
    {
        /// <summary>
        /// id
        /// </summary>
        [DataMember]
        public string id { get; set; }
        /// <summary>
        /// 父级id
        /// </summary>
        [DataMember]
        public string pId { get; set; }
        /// <summary>
        /// 文本
        /// </summary>
        [DataMember]
        public string name { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [DataMember]
        public string file { get; set; }


        /// <summary>
        /// 链接
        /// </summary>
        [DataMember]
        public string url { get; set; }

        /// <summary>
        /// 是否选择
        /// </summary>
        [DataMember]
        public bool Checked { get; set; }

        /// <summary>
        /// 是否打开
        /// </summary>
        [DataMember]
        public bool open { get; set; }

        /// <summary>
        /// 是否可选
        /// </summary>
        [DataMember]
        public bool nocheck { get; set; }

    }
}
