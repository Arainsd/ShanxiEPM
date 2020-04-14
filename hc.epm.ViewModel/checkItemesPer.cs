using hc.epm.DataModel.Basic;
using hc.epm.ViewModel;
using System;
using System.Collections.Generic;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 检查单
    /// </summary>
    public class checkItemesPer
    {
        /// <summary>
        /// 整改项目
        /// </summary>
        public List<CheckItes> checkItems { get; set; }

        /// <summary>
        /// 问题类型
        /// </summary>
        public List<QuestType> questType { get; set; }
        /// <summary>
        /// 整改信息
        /// </summary>
        public List<Companies> companies { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string describe { get; set; }
        /// <summary>
        /// 建议
        /// </summary>
        public string proposal { get; set; }

        //public List<Files> files { get; set; }
        public List<Files> files { get; set; }
        public class CheckItes
        {
           
            public string id { get; set; }
            public string name { get; set; }
            public string level { get; set; }
            public string addScore { get; set; }
            public List<CheckItes> children { get; set; }
            public List<Companies> companies { get; set; }

        }
        /// <summary>
        /// 附件信息
        /// </summary>
        public class Files
        {
            public string id { get; set; }
            public string type { get; set; }
            public string TableName { get; set; }
            public long? TableId { get; set; }

        }
    }
}

