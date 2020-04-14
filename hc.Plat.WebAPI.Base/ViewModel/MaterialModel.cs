using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 物料验收或接收
    /// </summary>
    public class MaterialModel
    {
        /// <summary>
        /// 业务类型 物料接收或材料验收
        /// </summary>
        public string businessType { get; set; }

        /// <summary>
        /// 所属项目 ID
        /// </summary>
        public long projectId { get; set; }

        /// <summary>
        /// 所属项目名称
        /// </summary>
        public string projectName { get; set; }

        /// <summary>
        /// 供应商 ID
        /// </summary>
        public long companyId { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string companyName { get; set; }

        /// <summary>
        /// 验收地址
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 接收明细
        /// </summary>
        public string itemList { get; set; }
    }

    /// <summary>
    /// 接收明细
    /// </summary>
    public class MaterialItemModel
    {
        /// <summary>
        /// 物料名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 物料型号
        /// </summary>
        public string model { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string unit { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal num { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public List<FileList> files { get; set; }
    }

    /// <summary>
    /// 附件
    /// </summary>
    public class FileList
    {
        /// <summary>
        /// 附件id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 附件类型（1，删除 0，添加）
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 附件路径
        /// </summary>
        public string url { get; set; }
    }

}