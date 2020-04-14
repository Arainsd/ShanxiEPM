using hc.epm.DataModel.BaseCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.DataModel.Business
{
    /// <summary>
    /// 人脸管理，api人脸注册完全对应
    /// </summary>
    public class EPM_AIUserFace : BaseBusiness
    {

        public string ImageBase64 { get; set; }

        public string ImageType { get; set; }

        public string GroupId { get; set; }

        public long UserId { get; set; }

        public string UserName { get; set; }

        public string UserPhone { get; set; }

        /// <summary>
        /// user,admin
        /// </summary>
        public string Source { get; set; }

        public string UserInfo { get; set; }

        public string QualityControl { get; set; }

        public string LivenessControl { get; set; }

        public bool IsSuccess { get; set; }
        /// <summary>
        /// 以下为返回值
        /// </summary>
        public string LogId { get; set; }

        public string FaceToken { get; set; }

        public string Location { get; set; }

        public decimal? Left { get; set; }

        public decimal? Top { get; set; }

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }

        public string Rotation { get; set; }

       
    }
}
