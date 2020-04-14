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
    public class exEntityServiceItemTreeJUI : exEntityClassTreeJUI
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 父服务项ID
        /// </summary>
        public string ParentSerID { get; set; }

        /// <summary>
        /// 父服务项名称
        /// </summary>
        public string ParentSerName { get; set; }

        /// <summary>
        /// 子服务项ID
        /// </summary>
        public string SerID { get; set; }

        /// <summary>
        /// 子服务项名称
        /// </summary>
        public string SerName { get; set; }

        /// <summary>
        /// URLS
        /// </summary>
        public string URLS { get; set; }

        /// <summary>
        /// 服务项权限
        /// </summary>
        public string SerAuthority { get; set; }

        /// <summary>
        /// 简称
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        /// 使用系统
        /// </summary>
        public string UseSystem { get; set; }

        /// <summary>
        /// 服务项费用/天
        /// </summary>
        public decimal? SerCostDay { get; set; }

        /// <summary>
        /// 发布次数/天
        /// </summary>
        public int? ReleaseNumDay { get; set; }

        /// <summary>
        /// 服务项费用/月
        /// </summary>
        public decimal? SerCostMonth { get; set; }

        /// <summary>
        /// 发布次数/月
        /// </summary>
        public int? ReleaseNumMonth { get; set; }

        /// <summary>
        /// 服务项费用/季度
        /// </summary>
        public decimal? SerCostQuarter { get; set; }

        /// <summary>
        /// 发布次数/季度
        /// </summary>
        public int? ReleaseNumQuarter { get; set; }

        /// <summary>
        /// 服务项费用/年
        /// </summary>
        public decimal? SerCostYear { get; set; }

        /// <summary>
        /// 发布次数/年
        /// </summary>
        public int? ReleaseNumYear { get; set; }

        /// <summary>
        /// 是否属于定制项
        /// </summary>
        public int? IsCustomized { get; set; }

        /// <summary>
        /// 站点栏目
        /// </summary>
        public string ProgramNum { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? Createtime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateName { get; set; }

        /// <summary>
        /// 操作人编码
        /// </summary>
        public string OpUserNum { get; set; }

        /// <summary>
        /// 服务项类别
        /// </summary>
        public int? SerType { get; set; }

        /// <summary>
        /// 服务项图标
        /// </summary>
        public string Icon { get; set; }
        
    }
}
