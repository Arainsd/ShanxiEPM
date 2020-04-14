using System;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 危险作业
    /// </summary>
    public class DangerourWorkModel
    {
        public long id { get; set; }
        /// <summary>
        /// 所属项目 ID
        /// </summary>
        public long projectId { get; set; }

        /// <summary>
        /// 所属项目名称
        /// </summary>
        public string projectName { get; set; }

        /// <summary>
        /// 危险作业名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 作业类型编码
        /// </summary>
        public string typeNo { get; set; }

        /// <summary>
        /// 作业类型名称
        /// </summary>
        public string typeName { get; set; }

        /// <summary>
        /// 开始i日期
        /// </summary>
        public DateTime startTime { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime endTime { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string area { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 防护措施
        /// </summary>
        public string protective { get; set; }

        public long workCompanyId { get; set; }

        public string workCompanyName { get; set; }
    }

    public class UploadWorkRealScenenModel
    {
        /// <summary>
        /// 危险作业 ID
        /// </summary>
        public long workId { get; set; }

        /// <summary>
        /// 所属项目 ID
        /// </summary>
        public long projectId { get; set; }

        /// <summary>
        /// 危险作业名称
        /// </summary>
        public string workName { get; set; }
    }
}