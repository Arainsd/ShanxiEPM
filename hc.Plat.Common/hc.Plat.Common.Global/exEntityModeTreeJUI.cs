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
    public class exEntityModeTreeJUI
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataMember]
        public string id { get; set; }

        /// <summary>
        /// 节点编号
        /// </summary>
        [DataMember]
        public string TypeNo { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        [DataMember]
        public string text { get; set; }

        /// <summary>
        /// 类型名称
        /// </summary>
        [DataMember]
        public string ModeName { get; set; }

        /// <summary>
        /// 类型编号
        /// </summary>
        [DataMember]
        public int IndexNo { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        [DataMember]
        public DateTime OpTime { get; set; }
     
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public int State { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Remark { get; set; }
        /// <summary>
        /// 是否展开
        /// </summary>
        [DataMember]
        public string IsOpen { get; set; }

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
        /// 子节点集合
        /// </summary>
        [DataMember]
        public List<exEntityModeTreeJUI> children { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public exEntityModeTreeJUI()
        {
            this.children = new List<exEntityModeTreeJUI>();
            //this.state = "open";
        }

        /// <summary>
        /// 常用构造函数
        /// </summary>
        public exEntityModeTreeJUI(string id, string text, string iconCls = "", string IsOpen = "open")
            : this()
        {
            this.id = id;
            this.text = text;
            this.IsOpen = IsOpen;
            this.iconCls = iconCls;
        }

        /// <summary>
        /// 常用构造函数
        /// </summary>
        public exEntityModeTreeJUI(int id, string text, string iconCls = "", string IsOpen = "open")
            : this()
        {
            this.id = id.ToString();
            this.text = text;
            this.IsOpen = IsOpen;
            this.iconCls = iconCls;
        }
    }

}
