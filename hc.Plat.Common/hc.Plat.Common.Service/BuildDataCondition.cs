using System;
using System.Collections.Generic;
using System.Linq;
using hc.Plat.Common.Global;
using System.Text;

namespace hc.Plat.Common.Service
{
    public class BuildDataCondition
    {

        public BuildDataCondition(string tableName, QueryCondition condition)
        {
            this.TableName = tableName;
            this.Condition = condition;
        }

        public string TableName { get; set; }

        public QueryCondition Condition { get; set; }

        public string FreeWhereSQL { get; set; }
        /// <summary>
        /// 构造排序语句
        /// </summary>
        /// <returns></returns>
        public string GetOrderESQL()
        {
            string ESQL = " ";
            if (Condition.SortList.Count == 0)
            {
                SortExpression se = new SortExpression();
                se.SortName = "Id";
                se.SortType = eSortType.Desc;
                Condition.SortList.Add(se);
            }
            ESQL = " ORDER BY ";
            bool isfirst = true;
            foreach (SortExpression se in Condition.SortList)
            {
                string fn = se.SortName;
                if (fn.IndexOf(TableName + ".") == -1)
                {
                    fn = TableName + "." + fn;
                }
                if (se.SortType == eSortType.Desc)
                {
                    if (isfirst)
                    {
                        ESQL = ESQL + " " + fn + " DESC ";
                    }
                    else
                    {
                        ESQL = ESQL + " , " + fn + " DESC ";
                    }
                }
                else
                {
                    if (isfirst)
                    {
                        ESQL = ESQL + " " + fn + " ASC ";
                    }
                    else
                    {
                        ESQL = ESQL + " , " + fn + " ASC ";
                    }
                }
                isfirst = false;
            }
            return ESQL;
        }
        /// <summary>
        /// 构造where条件
        /// </summary>
        /// <returns></returns>
        public string GetWhereESQL()
        {
            string ESQL = "";

            foreach (ConditionExpression ce in Condition.ConditionList)
            {
                if (ce.ExpValue is Boolean)
                {
                    ce.ExpValue = ce.ExpValue.ToString();
                }
                ESQL = ESQL + "  " + ce.ToESQLString(TableName);
            }

            if (ESQL != "")
            {
                if (!string.IsNullOrEmpty(FreeWhereSQL))
                {
                    ESQL += FreeWhereSQL;
                }
                if (ESQL.ToUpper().Trim().StartsWith("AND"))
                {
                    ESQL = ESQL.Trim().Remove(0, 3);
                }
                if (ESQL.ToUpper().Trim().StartsWith("OR"))
                {
                    ESQL = ESQL.Trim().Remove(0, 2);
                }
                string res = " WHERE " + ESQL;

                return res;
            }
            else
            {
                return " ";
            }
        }
        /// <summary>
        /// 分页查询sql语句构造
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public string BuildQuerySQLByPage(int startIndex, int endIndex)
        {
            string strWhere = GetWhereESQL();
            string orderby = GetOrderESQL();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append(orderby);
            }
            strSql.Append(")AS Row, " + TableName + ".*  from " + TableName + " " + TableName + " ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            return strSql.ToString();
        }
        /// <summary>
        /// 不分页查询语句构造
        /// </summary>
        /// <returns></returns>
        public string BuildQuerySQL()
        {
            string strWhere = GetWhereESQL();
            string orderby = GetOrderESQL();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM " + TableName + "  ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(strWhere);
            }
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append(orderby);
            }
            return strSql.ToString();
        }
        /// <summary>
        /// 构造查询数量sql
        /// </summary>
        /// <returns></returns>
        public string BuildQueryCountSQL()
        {
            string strWhere = GetWhereESQL();
            string orderby = GetOrderESQL();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT Count(Id) FROM " + TableName + "  ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(strWhere);
            }
            return strSql.ToString();
        }

        /// <summary>
        /// 构造查询语句，自动判定是否需要分页
        /// </summary>
        /// <returns></returns>
        public string BuildSQL()
        {
            string result = BuildQuerySQL();
            bool isAllowPage = false;
            if (Condition != null && Condition.PageInfo != null && Condition.PageInfo.isAllowPage)
            {
                isAllowPage = true;
            }
            if (isAllowPage)
            {
                int startIndex = ((this.Condition.PageInfo.CurrentPageIndex - 1) * this.Condition.PageInfo.PageRowCount + 1);
                int endIndex = this.Condition.PageInfo.CurrentPageIndex * this.Condition.PageInfo.PageRowCount;
                result = BuildQuerySQLByPage(startIndex, endIndex);
            }
            return result;
        }
    }
}
