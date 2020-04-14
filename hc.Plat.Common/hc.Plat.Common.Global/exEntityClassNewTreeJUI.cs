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
    public class exEntityClassNewTreeJUI
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataMember]
        public string id { get; set; }

        /// <summary>
        /// 类别编码
        /// </summary>
        public string ClassNum { get; set; }

        /// <summary>
        /// 类别名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public string PId { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        [DataMember]
        public string text { get; set; }

        /// <summary>
        /// 操作人编码
        /// </summary>
        [DataMember]
        public string OpUserNum { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        [DataMember]
        public DateTime OpTime { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        [DataMember]
        public string OpUserName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public int States { get; set; }

        /// <summary>
        /// 深度  
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Descrition { get; set; }
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
        /// 子节点集合
        /// </summary>
        [DataMember]
        public List<exEntityClassNewTreeJUI> children { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public exEntityClassNewTreeJUI()
        {
            this.children = new List<exEntityClassNewTreeJUI>();
        }

        /// <summary>
        /// 常用构造函数
        /// </summary>
        public exEntityClassNewTreeJUI(string id, string pid, string text, string iconCls = "", string state = "open")
            : this()
        {
            this.id = id;
            this.PId = pid;
            this.text = text;
            this.state = state;
            this.iconCls = iconCls;
        }
        /// <summary>
        /// 常用构造函数
        /// </summary>
        public exEntityClassNewTreeJUI(int id, string pid, string text, string iconCls = "", string state = "open")
            : this()
        {
            this.id = id.ToString();
            this.PId = pid;
            this.text = text;
            this.state = state;
            this.iconCls = iconCls;
        }

    }
}
