using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.DataModel.BaseCore
{
    /// <summary>
    /// 核心业务基类
    /// </summary>
    public class BaseBusiness : BaseModel
    {

        private long _createUserId = 0;
        /// <summary>
        /// 创建人Id
        /// </summary>
        public long CreateUserId
        {
            get
            {
                if (_createUserId == 0)
                {
                    return OperateUserId;
                }
                return _createUserId;
            }
            set { _createUserId = value; }
        }
        private string _createUserName = "";
        /// <summary>
        /// 创建人用户名
        /// </summary>
        public string CreateUserName
        {
            get
            {
                if (string.IsNullOrEmpty(_createUserName))
                {
                    return OperateUserName;
                }
                return _createUserName;
            }
            set { _createUserName = value; }
        }
        private DateTime? _createTime = null;
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime
        {
            get
            {
                if (_createTime == null)
                {
                    return OperateTime;
                }
                return _createTime;
            }
            set { _createTime = value; }
        }

        /// <summary>
        /// 操作人用户名
        /// </summary>
        public string OperateUserName { get; set; }

        ///<summary>
        /// 操作时间
        ///</summary>
        public DateTime? OperateTime { get; set; }

        ///<summary>
        /// 删除时间
        ///</summary>
        public DateTime? DeleteTime { get; set; }
    }
}
