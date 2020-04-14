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
    public class exEntityRegionalTreeJUI
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataMember]
        public string id { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        [DataMember]
        public string text { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public int States { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Remark { get; set; }
        /// <summary>
        /// 是否展开
        /// </summary>
        [DataMember]
        public string state { get; set; }

        /// <summary>
        /// 是否选中
        /// 不能用小写checked，C#关键字； 但是在JUI中，必须用checked，所以在JS客户端脚本中替换去；
        /// </summary>
        [DataMember]
        public bool Checked { get; set; }
        /// <summary>
        /// 图标样式
        /// </summary>
        [DataMember]
        public string iconCls { get; set; }

        /// <summary>
        /// 大区编号
        /// </summary>
        [DataMember]
        public int AreaNum { get; set; }

        /// <summary>
        /// 大区名称
        /// </summary>
        [DataMember]
        public string AreaName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        public int Sort { get; set; }
        /// <summary>
        /// 子节点集合
        /// </summary>
        [DataMember]
        public List<exEntityRegionalTreeJUI> children { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public exEntityRegionalTreeJUI()
        {
            this.children = new List<exEntityRegionalTreeJUI>();
            //this.state = "open";
        }

        /// <summary>
        /// 常用构造函数
        /// </summary>
        public exEntityRegionalTreeJUI(string id, string text, string iconCls = "", string state = "open")
            : this()
        {
            this.id = id;
            this.text = text;
            this.state = state;
            this.iconCls = iconCls;
        }

        /// <summary>
        /// 常用构造函数
        /// </summary>
        public exEntityRegionalTreeJUI(int RegionCode, string Regional, string iconCls = "", string state = "open")
            : this()
        {
            this.id = RegionCode.ToString();
            this.text = Regional;
            this.state = state;
            this.iconCls = iconCls;
        }
    }
}
