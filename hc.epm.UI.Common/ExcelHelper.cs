using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace hc.epm.UI.Common
{
    /// <summary>
    /// Excel 帮助类
    /// </summary>
    public class ExcelHelper
    {
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="fileName">导出文件名</param>
        /// <param name="columns">excel列名，key:属性字段，value:列名称</param>
        /// <param name="data">要导出的数据集合</param>
        /// <param name="contextBase">HttpContextBase 上下文</param>
        public static void ExportExcel(string fileName, Dictionary<string, string> columns, List<object> data, HttpContextBase contextBase)
        {
            const string ext = ".xls";
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = DateTime.Today.ToString("yyyyMMdd");
            }

            if (!fileName.EndsWith(ext))
            {
                fileName += ext;
            }

            StringBuilder sb = CreateExcelTable(data, columns);
            contextBase.Response.Clear();
            contextBase.Response.Buffer = true;
            contextBase.Response.Charset = "utf-8";
            contextBase.Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, Encoding.UTF8));

            contextBase.Response.ContentEncoding = Encoding.UTF8;
            contextBase.Response.ContentType = "application/ms-excel";
            contextBase.Response.Write("<meta http-equiv=\"content-type\" content=\"application/vnd.ms-excel; charset=utf-8\"/>");
            contextBase.Response.Write("<html>\n<head>\n");
            contextBase.Response.Write("<style type=\"text/css\">\n.pb{font-size:13px;border-collapse:collapse;} " +
                                   "\n.pb th{font-weight:bold;text-align:center;border:0.5pt solid windowtext;padding:2px;} " +
                                   "\n.pb td{border:0.5pt solid windowtext;mso-number-format:'\\@';padding:2px; text-align:center}\n</style>\n</head>\n");
            contextBase.Response.Write("<body>\n" + sb + "\n</body>\n</html>");
            contextBase.Response.Flush();
            contextBase.Response.End();
        }

        /// <summary>
        /// 根据要导出的数据以及列名生成 table 字符串
        /// </summary>
        /// <param name="data">要导出的数据</param>
        /// <param name="columns">列名</param>
        /// <returns></returns>
        private static StringBuilder CreateExcelTable(List<object> data, Dictionary<string, string> columns)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<table cellspacing='0' class=\"pb\">");
            sb.Append("<thead><tr>");
            foreach (var item in columns)
            {
                sb.AppendFormat("<th>{0}</th>", item.Value);
            }
            sb.Append("</th></thead>");
            sb.Append("<tbody>");

            if (data != null && data.Any())
            {
                foreach (var item in data)
                {
                    sb.Append("<tr>");
                    Type type = item.GetType();
                    PropertyInfo[] propertys = type.GetProperties();

                    foreach (var column in columns)
                    {
                        var obj = propertys.FirstOrDefault(p => p.Name == column.Key);
                        string columnValue = "";
                        if (obj != null)
                        {
                            var value = obj.GetValue(item, null);
                            if (value != null)
                            {
                                columnValue = value.ToString();
                            }
                        }
                        sb.AppendFormat("<td>{0}</td>", columnValue);
                    }
                    sb.Append("</tr>");
                }
            }

            sb.Append("</tbody></table>");
            return sb;
        }
        /// <summary>
        /// 将Excel表导入数据库(2007版导入依赖Office组件，系统必须安装Office组件，方可使用)
        /// </summary>
        /// <param name="filePath">Excel文件路径</param>
        /// <returns></returns>
        public static DataSet ImportExcelData(string filePath)
        {
            //office2007之前 仅支持.xls
            //const string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;IMEX=1'";
            //string strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties='Excel 8.0;IMEX=1'";
            //支持.xls和.xlsx，即包括office2010等版本的   HDR=Yes代表第一行是标题，不是数据；
            string suffix = filePath.Substring(filePath.LastIndexOf(".") + 1);
            string strCon;
            if (suffix.Equals("xls"))
            {
                //Excel2003以及更低版本用Jet
                strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0; HDR=Yes; IMEX=1;\"";
            }
            else
            {
                //Excel2007以上版本用ACE
                strCon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source ={0};Extended Properties = \"Excel 12.0;HDR=YES;IMEX=1\"";
            }
            //建立数据连接
            OleDbConnection conn = new OleDbConnection(string.Format(strCon, filePath));
            //打开连接
            if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            DataTable schemaTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            //获取Excel的第一个Sheet名称
            string sheetName = schemaTable.Rows[0]["table_name"].ToString().Trim();
            //查询sheet中的数据
            string strSql = "select * from [" + sheetName + "]";
            OleDbDataAdapter myCommand = new OleDbDataAdapter(strSql, conn);
            DataSet ds = new DataSet();
            myCommand.Fill(ds, "[" + sheetName + "]");
            conn.Close();
            return ds;
        }


        //-----------------------------------------------------------------------------------------------
    }
    
}
