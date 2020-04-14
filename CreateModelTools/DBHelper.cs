using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using hc.Plat.Common.Extend;

namespace CreateModelTools
{
    /// <summary>
    /// 所有生成的操作
    /// </summary>
    public enum Operate
    {
        [EnumText("Add{Model}")]
        Add,
        [EnumText("Update{Model}")]
        Edit,
        [EnumText("Delete{Model}ByIds")]
        Delete,
        [EnumText("Get{Model}List")]
        List,
        [EnumText("Get{Model}Model")]
        Detail
    }


    /// <summary>
    /// 数据库帮助类（SQLServer）
    /// </summary>
    public class DBHelper
    {
        public static Dictionary<Operate, string> ActionText
        {
            get
            {
                Dictionary<Operate, string> list = new Dictionary<Operate, string>();
                list.Add(Operate.Add, "添加");
                list.Add(Operate.Edit, "修改");
                list.Add(Operate.Delete, "删除");
                list.Add(Operate.List, "获取列表");
                list.Add(Operate.Detail, "获取详情");
                return list;
            }
        }
        /// <summary>
        /// 获取数据库的表
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static List<string> GetTableName(string connectionString)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            List<string> tableNames = new List<string>();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT name FROM sysobjects WHERE XType = 'U' ", conn);
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    tableNames.Add(row[0].ToString());
                }
            }
            finally
            {
                conn.Close();
            }

            return tableNames;
        }

        /// <summary>
        /// 根据数据库表获取对应列
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static List<TableColumn> GetColumn(string connectionString, string tableName)
        {

            string strSql = @"SELECT TableName = CASE 
              WHEN a.colorder = 1 THEN d.name
              ELSE ''
            END
       ,TableDescription = CASE 
                WHEN a.colorder = 1 THEN Isnull(f.VALUE,'')
                ELSE ''
              END
       ,ColumnIndex = a.colorder
       ,ColumnName = a.name
       ,Identitys = CASE 
               WHEN Columnproperty(a.id,a.name,'IsIdentity') = 1 THEN 1
               ELSE 0
             END
       ,PKey = CASE 
               WHEN EXISTS (SELECT 1
                            FROM   sysobjects
                            WHERE  xtype = 'PK'
                                   AND name IN (SELECT name
                                                FROM   sysindexes
                                                WHERE  indid IN (SELECT indid
                                                                 FROM   sysindexkeys
                                                                 WHERE  id = a.id
                                                                        AND colid = a.colid))) THEN 1
               ELSE 0
             END
       ,SType = b.name
       ,ByteLength = a.length
       ,Lengths = Columnproperty(a.id,a.name,'PRECISION')
       ,Precisions = Isnull(Columnproperty(a.id,a.name,'Scale'),0)
       ,IsNULLS = CASE 
                WHEN a.isnullable = 1 THEN 1
                ELSE 0
              END
       ,DefaultValue = Isnull(e.TEXT,'')
       ,Remark = Isnull(g.[value],'')
FROM     syscolumns a
         LEFT JOIN systypes b
           ON a.xusertype = b.xusertype
         INNER JOIN sysobjects d
           ON (a.id = d.id)
              AND (d.xtype = 'U')
              AND (d.name <> 'dtproperties') 
          INNER JOIN  sys.all_objects c
            ON d.id=c.object_id 
                AND  schema_name(schema_id)='dbo'
         LEFT JOIN syscomments e
           ON a.cdefault = e.id
         LEFT JOIN sys.extended_properties g
           ON (a.id = g.major_id)
              AND (a.colid = g.minor_id)
         LEFT JOIN sys.extended_properties f
           ON (d.id = f.major_id)
              AND (f.minor_id = 0)
where d.name='" + tableName + @"'         --如果只查询指定表,加上此条件
ORDER BY a.id,a.colorder
";

            List<TableColumn> columns = new List<TableColumn>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandTimeout = 60;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = strSql;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            columns = Mapper.Map<IDataReader, IEnumerable<TableColumn>>(reader).ToList();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }
            return columns;
        }
    }
}
