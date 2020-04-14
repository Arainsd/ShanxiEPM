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
    public class exEntitySiteColumnTreeJUI:exEntityClassTreeJUI
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 栏目级别
        /// </summary>
        public int ScLevel { get; set; }

        /// <summary>
        /// 栏目编码
        /// </summary>
        public string ScNum { get; set; }

        /// <summary>
        /// 栏目名称
        /// </summary>
        public string ScName { get; set; }

        /// <summary>
        /// 栏目简称
        /// </summary>
        public string ScAbbreviation { get; set; }

        /// <summary>
        /// 父级栏目编码
        /// </summary>
        public string ParentNum { get; set; }

        /// <summary>
        /// 静态模板名称
        /// </summary>
        public string StaticTempName { get; set; }

        /// <summary>
        /// 栏目标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>

        public string Keywords { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }



        

        
        
        
    }
}
