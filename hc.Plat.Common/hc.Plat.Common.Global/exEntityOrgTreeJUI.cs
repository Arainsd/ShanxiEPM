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
    public class exEntityOrgTreeJUI
    {
        /// <summary>
        /// ID
        /// </summary>
        [DataMember]
        public string ID { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        /// 父级名称
        /// </summary>
        public string ParentName { get; set; }

        /// <summary>
        /// 机构类别Id
        /// </summary>
        public string OrgTypeId { get; set; }

        /// <summary>
        /// 机构类别名称
        /// </summary>
        public string OrgTypeName { get; set; }

        /// <summary>
        /// 机构区域Id
        /// </summary>
        public string OrgAreaId { get; set; }

        /// <summary>
        /// 机构编号
        /// </summary>
        public string OrgCode { get; set; }

        /// <summary>
        /// 机构全名
        /// </summary>
        public string OrgFullName { get; set; }


        /// <summary>
        /// 机构简称
        /// </summary>
        public string OrgShortName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public int States { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 节点名称
        /// </summary>
        [DataMember]
        public string text { get; set; }

        /// <summary>
        /// 操作人Id
        /// </summary>
        [DataMember]
        public string OpUserId { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        [DataMember]
        public string OpUserName { get; set; }

        /// <summary>
        /// 操作人组织机构Id
        /// </summary>
        [DataMember]
        public string OpOrgId { get; set; }

        /// <summary>
        /// 操作人组织机构名称
        /// </summary>
        [DataMember]
        public string OpOrgName { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        [DataMember]
        public DateTime OpTime { get; set; }

        /// <summary>
        /// 深度  
        /// </summary>
        public int Depth { get; set; }

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
        public List<exEntityOrgTreeJUI> children { get; set; }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public exEntityOrgTreeJUI()
        {
            this.children = new List<exEntityOrgTreeJUI>();
        }

        /// <summary>
        /// 常用构造函数
        /// </summary>
        public exEntityOrgTreeJUI(string id, string pid, string text, string iconCls = "", string state = "open")
            : this()
        {
            this.ID = id;
            this.ParentId = pid;
            this.text = text;
            this.state = state;
            this.iconCls = iconCls;
        }
        /// <summary>
        /// 常用构造函数
        /// </summary>
        public exEntityOrgTreeJUI(int id, string pid, string text, string iconCls = "", string state = "open")
            : this()
        {
            this.ID = id.ToString();
            this.ParentId = pid;
            this.text = text;
            this.state = state;
            this.iconCls = iconCls;
        }
    }
}
