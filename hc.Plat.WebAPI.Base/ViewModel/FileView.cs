using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 附件资源实体
    /// </summary>
    public class FileView
    {
        /// <summary>
        /// 附件 ID
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 附件名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 文件后缀名称
        /// </summary>
        public string suffixName { get; set; }

        /// <summary>
        /// 附件类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 附件小图(只有图片类型有)
        /// </summary>
        public string imageUrlSmall { get; set; }

        /// <summary>
        /// 附件大图(只有图片类型有)
        /// </summary>
        public string imageUrlBig { get; set; }

        /// <summary>
        /// 附件下载地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 附件大小
        /// </summary>
        public string size { get; set; }

        /// <summary>
        /// 附件创建时间
        /// </summary>
        public string time { get; set; }

        public string tableId { get; set; }
    }
}