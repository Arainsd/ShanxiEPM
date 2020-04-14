using System;
using System.Collections.Generic;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 检查
    /// </summary>
    public class MonitorModel
    {
        public MonitorModel()
        {
            files = new List<FileView>();
        }

        ///<summary>
        ///项目表Id
        ///</summary>
        public long projectId { get; set; }

        ///<summary>
        ///项目名称
        ///</summary>
        public string projectName { get; set; }
        
        ///<summary>
        ///标题
        ///</summary>
        public string title { get; set; }

        ///<summary>
        ///说明
        ///</summary>
        public string content { get; set; }
        
        /// <summary>
        /// 整改建议
        /// </summary>
        public string rectification { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
        
        ///<summary>
        ///整改时间
        ///</summary>
        public DateTime monitorTime { get; set; }
        
        public string questions { get; set; }

        public List<FileView> files { get; set; }

        /// <summary>
        /// 整改单位 ID
        /// </summary>
        public long companyId { get; set; }

        /// <summary>
        /// 整改单位
        /// </summary>
        public string companyName { get; set; }
    }

    /// <summary>
    /// 整改问题
    /// </summary>
    public class MonitorDetailModel
    {
        /// <summary>
        /// 问题 ID
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 问题名称
        /// </summary>
        public string name { get; set; }
    }
}