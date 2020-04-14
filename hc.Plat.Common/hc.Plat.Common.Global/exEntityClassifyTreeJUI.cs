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
    public class exEntityClassifyTreeJUI: exEntityClassTreeJUI
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 类别编码
        /// </summary>
        public string ClassNo { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        public string ClassName { get; set; }

        
    }
}
