using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreateModelTools
{
    /// <summary>
    /// 数据库列
    /// </summary>
    public class TableColumn
    {
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 表说明
        /// </summary>
        public string TableDescription { get; set; }
        /// <summary>
        /// 字段顺序
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        ///字段名称
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 是否是标识列
        /// </summary>
        public bool Identitys { get; set; }

        /// <summary>
        /// 是否是主键
        /// </summary>
        public bool PKey { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string SType { get; set; }

        /// <summary>
        /// 占用字节长度，得到的是物理内存的长度，nvarchar得到的长度是字段长度2倍，varchar和字段长度一致
        /// </summary>
        public int ByteLength { get; set; }

        /// <summary>
        /// 字段长度
        /// </summary>
        public int Lengths { get; set; }


        /// <summary>
        /// 小数精度
        /// </summary>
        public int Precisions { get; set; }

        /// <summary>
        /// 是否允许为空
        /// </summary>
        public bool IsNULLS { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// 字段说明
        /// </summary>
        public string Remark { get; set; }



    }
}
