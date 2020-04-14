using hc.epm.DataModel.BaseCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.DataModel.Business
{
    /// <summary>
    /// 人脸日志表
    /// </summary>
    public class EPM_FaceOperateLog : BaseBusiness
    {
        /// <summary>
        /// 业务Id
        /// </summary>
        public long ModelId { get; set; }

        /// <summary>
        /// 接口类型
        /// </summary>
        public string APIType { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public string RequestJson { get; set; }

        /// <summary>
        /// 返回参数
        /// </summary>
        public string ResponseJson { get; set; }

        /// <summary>
        /// 操作结果
        /// </summary>
        public bool IsSuccess { get; set; }
    }
}
