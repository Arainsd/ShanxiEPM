using System;
using System.Collections.Generic;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 项目总批复构成
    /// </summary>
    public class ConstitutesModel
    {
        public ConstitutesModel()
        {
            childs = new List<ConstitutesModel>();
            files = new List<FileView>();
        }

        /// <summary>
        /// 构成名称 Key
        /// </summary>
        public string key { get; set; }

        /// <summary>
        /// 构成名称 Value
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 构成子集
        /// </summary>
        public List<ConstitutesModel> childs { get; set; }

        /// <summary>
        /// 文件
        /// </summary>
        public List<FileView> files { get; set; }
    }
}