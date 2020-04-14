using hc.epm.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.DataModel.BaseCore
{
    /// <summary>
    /// 基类
    /// </summary>
    public class BaseModel
    {
        private long _id = SnowflakeHelper.GetID;
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// 字符串id，只读，避免js 转换 json id自动四舍五入的问题，前台js一律使用sid
        /// </summary>
        [NotMapped]
        public string SId
        {
            get
            {
                return Id.ToString();
            }
        }

        private long _operateUserId = 0;
        /// <summary>
        /// 操作人
        /// </summary>
        public long OperateUserId
        {
            get { return _operateUserId; }
            set { _operateUserId = value; }
        }

        private bool _isDelete = false;
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDelete
        {
            get { return _isDelete; }
            set { _isDelete = value; }
        }
    }
}
