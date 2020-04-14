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
    public class exEntityTreeJUI
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
        /// 所属父级
        /// </summary>
        [DataMember]
        public string ParentID { get; set; }

        /// <summary>
        /// 权限类型
        /// </summary>
        [DataMember]
        public string RightType { get; set; }

        /// <summary>
        /// 权限类型名称
        /// </summary>
        [DataMember]
        public string RightTypeName { get; set; }

        /// <summary>
        /// 权限唯一标识
        /// </summary>
        [DataMember]
        public string RightName { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        [DataMember]
        public DateTime OpTime { get; set; }
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
        /// 子节点集合
        /// </summary>
        [DataMember]
        public List<exEntityTreeJUI> children { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public exEntityTreeJUI()
        {
            this.children = new List<exEntityTreeJUI>();
            //this.state = "open";
        }

        /// <summary>
        /// 常用构造函数
        /// </summary>
        public exEntityTreeJUI(string id, string text, string iconCls = "", string state = "open")
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
        public exEntityTreeJUI(int id, string text, string iconCls = "", string state = "open")
            : this()
        {
            this.id = id.ToString();
            this.text = text;
            this.state = state;
            this.iconCls = iconCls;
        }
    }

    public class EasyUiTreeModel
    {
        public string id { get; set; }

        public string parentId { get; set; }

        public string text { get; set; }

        public string state { get;set; }

        public bool Checked { get; set; }

        public string attributes { get; set; }

        public string iconCls { get; set; }

        public List<EasyUiTreeModel> children { get; set; }

        public EasyUiTreeModel()
        {
            children = new List<EasyUiTreeModel>();
        }
    }
}