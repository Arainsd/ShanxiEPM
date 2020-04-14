using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using hc.epm.Common;

namespace CreateModelTools
{
    public partial class AppendColumn : Form
    {
        public AppendColumn()
        {
            InitializeComponent();
        }
        //数据库连接
        private static string connectionStringBase = ConfigurationManager.ConnectionStrings["connectionStringBase"].ConnectionString;
        private static string connectionStringCore = ConfigurationManager.ConnectionStrings["connectionStringCore"].ConnectionString;
        private static string connectionStringMsg = ConfigurationManager.ConnectionStrings["connectionStringMsg"].ConnectionString;
        private static string connectionStringFile = ConfigurationManager.ConnectionStrings["connectionStringFile"].ConnectionString;
        private void btnAppend_Click(object sender, EventArgs e)
        {
            //core数据库增加企业这一列，重新生成entity
            //string connectionString1 = ConfigurationManager.ConnectionStrings["connectionString1"].ConnectionString;
            //string connectionString2 = ConfigurationManager.ConnectionStrings["connectionString2"].ConnectionString;


            //var tableNames1 = DBHelper.GetTableName(connectionString1);
            //var tableNames2 = DBHelper.GetTableName(connectionString2);


            //List<string> strSql = new List<string>();
            //string sql1 = "alter table tableName add [CreateUserId] bigint NOT NULL  DEFAULT 0";
            //string sql2 = "alter table tableName add [CreateTime] datetime NOT NULL DEFAULT getdate() ";
            //string sql3 = "alter table tableName add [CreateUserName] nvarchar(50) NOT NULL DEFAULT '' ";
            //string sql4 = "alter table tableName add [OperateUserName] nvarchar(50) NOT NULL DEFAULT ''";
            //strSql.Add(sql1);
            //strSql.Add(sql2);
            //strSql.Add(sql3);
            //strSql.Add(sql4);
            //string sql5 = "alter table tableName add [SCompanyId] bigint NOT NULL DEFAULT 0 ";
            ////foreach (var table1 in tableNames1)
            ////{
            ////    DbHelperSQL.connectionString = connectionString1;
            ////    foreach (var sql in strSql)
            ////    {
            ////        string exSql = sql.Replace("tableName", table1);
            ////        var res=DbHelperSQL.ExecuteSql(exSql);
            ////    }
            ////}
            //strSql.Add(sql5);
            //foreach (var table2 in tableNames2)
            //{
            //    DbHelperSQL.connectionString = connectionString2;
            //    foreach (var sql in strSql)
            //    {
            //        try
            //        {
            //            string exSql = sql.Replace("tableName", table2);
            //            var res = DbHelperSQL.ExecuteSql(exSql);
            //        }
            //        catch (Exception)
            //        {

            //            lblError.Text += table2 + ",";
            //        }

            //    }
            //}

            //MessageBox.Show("成功");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString1 = ConfigurationManager.ConnectionStrings["connectionString1"].ConnectionString;

            // string connectionString2 = ConfigurationManager.ConnectionStrings["connectionString2"].ConnectionString;

            // DbHelperSQL.connectionString = connectionString1;
            // string readSql = "select * from Sys_Region";
            // DataTable dt1 = DbHelperSQL.Query(readSql).Tables[0];
            // DbHelperSQL.connectionString = connectionString2;
            // foreach (DataRow dr in dt1.Rows)
            // {
            //     string sql = @" insert into Base_Region([Id]
            //,[RegionCode]
            //,[RegionName]
            //,[ParentCode]
            //,[IsActive]
            //,[Fullname]
            //,[Sort]
            //,[AreaNum]
            //,[AreaName]) values (" + SnowflakeHelper.GetID + @",'" + dr["RegionCode"] + @"','" + dr["RegionName"] + @"','" + dr["ParentCode"] + @"','" + dr["IsActive"] + @"','" + dr["Fullname"] + @"','" + dr["Sort"] + @"','" + dr["AreaNum"] + @"','" + dr["AreaName"] + @"')";
            //     DbHelperSQL.ExecuteSql(sql);
            // }
        }



        private static void CleanData(bool isDelete = false)
        {
            var tableNamesBase = DBHelper.GetTableName(connectionStringBase);
            var tableNamesCore = DBHelper.GetTableName(connectionStringCore);
            var tableNamesMsg = DBHelper.GetTableName(connectionStringMsg);
            var tableNamesFile = DBHelper.GetTableName(connectionStringFile);
            int res = 0;

            DbHelperSQL.connectionString = connectionStringBase;
            foreach (var item in tableNamesBase)
            {
                string strSql = "delete from " + item + " where isdelete=1";
                if (isDelete)
                {
                    if (!FilterTable.Contains(item))
                    {
                        strSql = "delete from " + item;
                    }

                }
                res += DbHelperSQL.ExecuteSql(strSql);
            }

            DbHelperSQL.connectionString = connectionStringCore;
            foreach (var item in tableNamesCore)
            {
                string strSql = "delete from " + item + " where isdelete=1";
                if (isDelete)
                {
                    if (!FilterTable.Contains(item))
                    {
                        strSql = "delete from " + item;
                    }
                }
                res += DbHelperSQL.ExecuteSql(strSql);
            }

            DbHelperSQL.connectionString = connectionStringMsg;
            foreach (var item in tableNamesMsg)
            {
                string strSql = "delete from " + item + " where isdelete=1";
                if (isDelete)
                {
                    if (!FilterTable.Contains(item))
                    {
                        strSql = "delete from " + item;
                    }
                }
                res += DbHelperSQL.ExecuteSql(strSql);
            }

            DbHelperSQL.connectionString = connectionStringFile;
            foreach (var item in tableNamesFile)
            {
                if (isDelete)
                {
                    if (!FilterTable.Contains(item))
                    {
                        string strSql = "delete from " + item;
                        res += DbHelperSQL.ExecuteSql(strSql);
                    }
                }

            }

            MessageBox.Show("成功,共清理数据：" + res + "条");
        }

        public static List<string> FilterTable
        {
            get
            {
                List<string> list = new List<string>() { "Base_User", "Base_Company", "Base_Right", "Base_Role", "Base_RoleRight", "Base_Config", "Base_UserRole", "Base_Region", "Base_Settings", "Base_TypeDictionary", "Base_Protocol", "BA_ProjectOperateConfig", "BA_ProjectStateConfig", "Msg_EmailTemplete", "Msg_MessageTemplete", "Msg_SMSTemplete", "Msg_MessageSection", "Msg_MessageStrategy", "Msg_SMSSetting", "Msg_EmailSetting", "Base_ProcurementMethod", "Base_BidEvaluationMethod", "Base_Professional" };
                return list;
            }
        }
        public void InitData()
        {
            //清除除超级管理员以外的用户
            DbHelperSQL.connectionString = connectionStringBase;
            string strSql = " delete from Base_User where UserAcct<>'admin' ";
            DbHelperSQL.ExecuteSql(strSql);
            //清除多余的角色
            strSql = " delete from Base_Role where RoleName not in('admin','Tenderer','Bidder','Exp','BiddingAgent') ";
            DbHelperSQL.ExecuteSql(strSql);
            //角色权限
            strSql = " delete from Base_RoleRight where RoleId not in(select Id from  Base_Role where RoleName in ('admin','Tenderer','Bidder','Exp','BiddingAgent')) ";
            DbHelperSQL.ExecuteSql(strSql);
            //用户角色
            strSql = " delete from Base_UserRole where UserId not in (select Id from Base_User where UserAcct='admin')";
            DbHelperSQL.ExecuteSql(strSql);
            //清除企业
            strSql = " delete from Base_Company where Code not in('Sys01','Sys02') ";
            DbHelperSQL.ExecuteSql(strSql);
            //初始化专家、管理员默认企业
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            CleanData();
        }

        private void btnInit_Click(object sender, EventArgs e)
        {
            CleanData(true);
            InitData();
        }
    }
}
