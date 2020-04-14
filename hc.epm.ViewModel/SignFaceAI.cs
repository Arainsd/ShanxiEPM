using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hc.epm.ViewModel
{
    /// <summary>
    /// 人脸签到
    /// </summary>
    public class SignFaceAI
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        public long? ProjectId { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// 加油站名称
        /// </summary>
        public string OilStationName { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public string Longitude { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public string Latitude { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public long? UserId { get; set; }
    }
}