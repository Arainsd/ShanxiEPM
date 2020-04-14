using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    /// <summary>
    /// 资源服务HB
    /// </summary>
    public class ResultUpload
    {
        public string GuidId { get; set; }

        /// <summary>
        /// 文件名 XXX.XXX
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 大小 XX KB
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        /// 上传人
        /// </summary>
        public string UploadName { get; set; }

        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime UploadDate { get; set; }

        /// <summary>
        /// 错误代码：0为上传成功，-1为执行错误
        /// </summary>
        public string errorCode { get; set; }

        /// <summary>
        /// 错误描述：为-1时才有值
        /// </summary>
        public string errorString { get; set; }

        /// <summary>
        /// 里边的参数记录下来（查询用到）
        /// </summary>
        public ResponseObject ResponseObject { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ImageType { get; set; }
    }

    public class ResponseObject
    {
        public string[] responseObject { get; set; }

        public string FDFS_GROUP { get; set; }

        public string FDFS_NAME { get; set; }
    }


    public class ResultDownUpload
    {
        /// <summary>
        /// 错误代码：0为上传成功，-1为执行错误
        /// </summary>
        public string errorCode { get; set; }
        /// <summary>
        /// 错误描述：为-1时才有值
        /// </summary>
        public string errorString { get; set; }
        public responseMap responseMap { get; set; }
    }

    public class responseMap
    {
        /// <summary>
        /// 文件二进制数组
        /// </summary>
        public int[] FILE_DATA { get; set; }
    }

    public class LoginResult
    {
        /// <summary>
        /// 错误代码：0为上传成功，-1为执行错误
        /// </summary>
        public string errorCode { get; set; }
        /// <summary>
        /// 错误描述：为-1时才有值
        /// </summary>
        public string errorString { get; set; }
    }
}