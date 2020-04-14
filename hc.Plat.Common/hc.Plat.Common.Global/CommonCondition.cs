using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace hc.Plat.Common.Global
{
    /// <summary>
    ///  查询操作
    /// </summary>
    [DataContract]
    [Serializable]
    public enum eConditionOperator
    {
        /// <summary>
        /// 等于 =
        /// </summary>
        [EnumMember]
        Equal = 0,

        /// <summary>
        /// 大于 >
        /// </summary>
        [EnumMember]
        GreaterThan,

        /// <summary>
        /// 小于 &lt;
        /// </summary>
        [EnumMember]
        LessThan,

        /// <summary>
        /// 大于等于 >=
        /// </summary>
        [EnumMember]
        GreaterThanOrEqual,

        /// <summary>
        /// 小于等于 &lt;=
        /// </summary>
        [EnumMember]
        LessThanOrEqual,

        /// <summary>
        /// 不等于 !=
        /// </summary>
        [EnumMember]
        NotEqual,

        /// <summary>
        /// 模糊查询 like
        /// </summary>
        [EnumMember]
        Like,

        /// <summary>
        /// 模糊查询 not like
        /// </summary>
        [EnumMember]
        NotLike,

        /// <summary>
        ///  IS 操作
        /// </summary>
        [EnumMember]
        Is,

        /// <summary>
        ///  IS Not 操作 
        /// </summary>
        [EnumMember]
        IsNot,

        /// <summary>
        ///  存在 操作 
        /// </summary>
        [EnumMember]
        Exists,

        /// <summary>
        ///  不存在 操作 
        /// </summary>
        [EnumMember]
        NotExists,

        /// <summary>
        ///  In 操作 值的例子："'1','2','3'" 或者 "1,2,3"
        /// </summary>
        [EnumMember]
        In,

        /// <summary>
        ///  Not In 操作 值的例子："'1','2','3'" 或者 "1,2,3"
        /// </summary>
        [EnumMember]
        NotIn,
    }

    /// <summary>
    ///  查询逻辑
    /// </summary>
    [DataContract]
    [Serializable]
    public enum eLogicalOperator
    {
        /// <summary>
        ///  与
        /// </summary>
        [EnumMember]
        And,

        /// <summary>
        ///  或
        /// </summary>
        [EnumMember]
        Or
    }

    [DataContract]
    [Serializable]
    public enum eSortType
    {

        /// <summary>
        ///  升序 
        /// </summary>
        [EnumMember]
        Asc = 0,

        /// <summary>
        ///  降序
        /// </summary>
        [EnumMember]
        Desc = -1,

    }

    [Serializable]
    [DataContract]
    public class SortExpression
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        [DataMember]
        public string SortName { get; set; }
        /// <summary>
        /// 排序顺序；
        /// </summary>
        [DataMember]
        public eSortType SortType { get; set; }

        public SortExpression()
        {

        }
        public SortExpression(string name)
        {
            SortName = name;
            SortType = eSortType.Asc;
        }
        public SortExpression(string name, eSortType st)
        {
            SortName = name;
            SortType = st;
        }

    }


    /// <summary>
    /// 查询条件表达式；
    /// </summary>
    [Serializable]
    [DataContract]
    public class ConditionExpression
    {
        /// <summary>
        /// 条件名称；
        /// </summary>
        [DataMember]
        public string ExpName { get; set; }
        /// <summary>
        /// 条件值；
        /// </summary>
        [DataMember]
        public object ExpValue { get; set; }
        /// <summary>
        /// 条件等式；
        /// </summary>
        [DataMember]
        public eConditionOperator ExpOperater { get; set; }
        /// <summary>
        /// 条件上下文关系
        /// </summary>
        [DataMember]
        public eLogicalOperator ExpLogical { get; set; }
        /// <summary>
        ///  子条件集合（嵌套条件）
        /// </summary>
        [DataMember]
        public List<ConditionExpression> ConditionList { get; set; }

        /// <summary>
        ///  构造函数
        /// </summary>
        public ConditionExpression()
        {
            this.ExpLogical = eLogicalOperator.And;
            this.ConditionList = new List<ConditionExpression>();
        }
        /// <summary>
        /// 构造函数【默认等于】
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        public ConditionExpression(string name, object value)
        {
            this.ExpName = name;
            this.ExpOperater = eConditionOperator.Equal;
            this.ExpValue = value;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">条件名</param>
        /// <param name="Operator">条件</param>
        /// <param name="value">条件值</param>
        public ConditionExpression(string name, eConditionOperator Operator, object value)
        {
            this.ExpName = name;
            this.ExpOperater = Operator;
            this.ExpValue = value;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">条件名</param>
        /// <param name="Operator">条件</param>
        /// <param name="value">条件值</param>
        /// <param name="logic">逻辑关系</param>
        public ConditionExpression(string name, eConditionOperator Operator, object value, eLogicalOperator logic)
        {
            this.ExpName = name;
            this.ExpOperater = Operator;
            this.ExpValue = value;
            this.ExpLogical = logic;
        }

        /// <summary>
        /// 将操作符转换为EFSQL;
        /// </summary>
        /// <returns></returns>
        private string ConvertOperatorToESQL()
        {
            switch (this.ExpOperater)
            {
                case eConditionOperator.Equal:
                    return " {0} = {1} ";
                case eConditionOperator.Exists:
                    return " Can Not Support ";
                case eConditionOperator.GreaterThan:
                    return " {0} > {1} ";
                case eConditionOperator.GreaterThanOrEqual:
                    return " {0}>= {1}";
                case eConditionOperator.In:
                    return " {0} IN {1} "; //  
                case eConditionOperator.Is:
                    return " {0} IS {1} ";
                case eConditionOperator.IsNot:
                    return " {0} IS NOT {1}";
                case eConditionOperator.LessThan:
                    return " {0} < {1} ";
                case eConditionOperator.LessThanOrEqual:
                    return " {0} <= {1} ";
                case eConditionOperator.Like:  //条件是精确LIKE，外部决定是否需要加%;
                    return " {0} LIKE {1} ";
                case eConditionOperator.NotEqual:
                    return " {0} <> {1} ";
                case eConditionOperator.NotExists:
                    return " Can Not Support ";
                case eConditionOperator.NotIn:
                    return " {0} NOT IN {1}  "; // 
                case eConditionOperator.NotLike:
                    return " {0} NOT LIKE {1} ";
                default:
                    return " = ";
            }
        }


        /// <summary>
        /// 转换为ESQL
        /// </summary>
        /// <returns></returns>
        public string ToESQLString(string TableName)
        {

            string FieldName = this.ExpName;
            if ((this.ConditionList.Count == 0) && (FieldName != null) && (FieldName == ""))
            {
                return "";
            }

            string ESQL = "";
            if ((FieldName != null) && (FieldName != ""))
            {
                //{0}  ??  {1} ;
                ESQL = ConvertOperatorToESQL();

                if (FieldName.ToUpper().IndexOf(TableName.ToUpper() + ".") == -1)
                {
                    //增加表名；
                    FieldName = TableName + "." + FieldName;
                }

                if (ExpValue == null || ExpValue == DBNull.Value || ExpValue.ToString().ToLower().Trim() == "null")
                {
                    // NULL 值处理 = 和 is 不需要处理，linqTOsql会自动处理 = 和 is的；
                    ExpValue = "null";
                    ESQL = string.Format(ESQL, FieldName, ExpValue);
                }
                else if (ExpValue is string)
                {
                    //字符串
                    string ValueString = ExpValue.ToString();
                    if ((ExpOperater != eConditionOperator.In) && (ExpOperater != eConditionOperator.NotIn))
                    {
                        ValueString = "'" + ValueString + "'";
                    }
                    else
                    {
                        if (!ValueString.ToLower().Contains("select"))//非子表的查询
                        {
                            ValueString = "{" + ValueString + "}";
                        }
                    }
                    ESQL = string.Format(ESQL, FieldName, ValueString);
                }
                else if (ExpValue is DateTime)
                {
                    //日期
                    string ValueString = "DATETIME'" + ((DateTime)ExpValue).ToString("yyyy-MM-dd HH:mm:ss.fffffff") + "'";
                    ESQL = string.Format(ESQL, FieldName, ValueString);
                }
                else if (ExpValue is ValueType)
                {
                    //数值
                    ESQL = string.Format(ESQL, FieldName, ExpValue);
                }
                else if (ExpValue is Array)
                {
                    //对于IN 的，必须采用Array传入对象；
                    //待处理；
                }
                else
                {
                    ESQL = string.Format(ESQL, FieldName, ExpValue);
                }
            }

            if (ESQL != "")
            {
                if (ExpLogical == eLogicalOperator.And)
                {
                    ESQL = " AND " + ESQL;
                }
                if (ExpLogical == eLogicalOperator.Or)
                {
                    ESQL = " OR " + ESQL;
                }
            }

            string ChildESQL = "";
            foreach (ConditionExpression ce in this.ConditionList)
            {
                ChildESQL = ChildESQL + ce.ToESQLString(TableName);
            }
            if (ChildESQL != "")
            {
                if (ChildESQL.ToUpper().Trim().StartsWith("AND"))
                {
                    ChildESQL = ChildESQL.Trim().Remove(0, 3);
                }
                if (ChildESQL.ToUpper().Trim().StartsWith("OR"))
                {
                    ChildESQL = ChildESQL.Trim().Remove(0, 2);
                }

                if (ChildESQL != "")
                {
                    if (ExpLogical == eLogicalOperator.And)
                    {
                        ChildESQL = " AND ( " + ChildESQL + " ) ";
                    }
                    if (ExpLogical == eLogicalOperator.Or)
                    {
                        ChildESQL = " or ( " + ChildESQL + " ) ";
                    }
                }
                return ChildESQL;
                //return " AND ( " + ChildESQL + " ) ";
            }

            ESQL = ESQL + ChildESQL;
            return ESQL;
        }
    }

    /// <summary>
    /// 查询分页
    /// </summary>
    [Serializable]
    [DataContract]
    public class PageListInfo
    {
        /// <summary>
        /// 是否允许分页
        /// </summary>
        [DataMember]
        public bool isAllowPage { get; set; }
        /// <summary>
        /// 页面行数
        /// </summary>
        [DataMember]
        public Int32 PageRowCount { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        [DataMember]
        public Int32 CurrentPageIndex { get; set; }
        /// <summary>
        /// 排序字符串[含排序方式]  例如： "USERNAME:DESC,USERID:ASC"
        /// </summary>
        [DataMember]
        public string OrderAndSortList
        {
            get;
            set;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public PageListInfo()
        {
            isAllowPage = true;
            PageRowCount = 20;
            CurrentPageIndex = 1;
            OrderAndSortList = "";
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="isPage">是否分页</param>
        /// <param name="RowNum">每页行数</param>
        public PageListInfo(bool isPage, int RowNum)
        {
            isAllowPage = isPage;
            PageRowCount = RowNum;
        }

    }


    [Serializable]
    [DataContract]
    public class QueryCondition
    {
        /// <summary>
        /// 获取附件完整路径
        /// </summary>
        [DataMember]
        public bool NeedFullUrl { get; set; }
        /// <summary>
        /// 查询条件 列表
        /// </summary>
        [DataMember]
        public List<ConditionExpression> ConditionList { get; set; }
        /// <summary>
        /// 分页方式
        /// </summary>
        [DataMember]
        public PageListInfo PageInfo { get; set; }

        /// <summary>
        /// 按照条件名称查找条件
        /// </summary>
        /// <param name="ExpName"></param>
        /// <returns></returns>
        public ConditionExpression FindConditon(string ExpName)
        {
            ConditionExpression ce = ConditionList.Where(o => o.ExpName == ExpName).FirstOrDefault();
            if (ce == null)
            {
                throw new Exception("没有约定条件：" + ExpName);
            }
            return ce;
        }

        /// <summary>
        /// 排序方式 列表
        /// </summary>
        [DataMember]
        public List<SortExpression> SortList { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public QueryCondition()
        {
            PageInfo = new PageListInfo();
            ConditionList = new List<ConditionExpression>();
            SortList = new List<SortExpression>();
        }
        /// <summary>
        /// 将PageInfo的排序字符串转换为Sort表达式列表
        /// </summary>
        /// <returns></returns>
        public bool CheckOrderAndSortList()
        {
            //待处理； 
            if (PageInfo.OrderAndSortList.Trim() == "")
            {
                return true;
            }
            //例如： "USERNAME:DESC,USERID:ASC";

            SortList.Clear();
            string[] SortField = PageInfo.OrderAndSortList.Split(',');
            for (int i = 0; i < SortField.Length; i++)
            {
                string[] FieldNameAndValue = SortField[i].Split(':');
                if (FieldNameAndValue.Length < 2)
                {
                    break;
                }
                string FieldName = FieldNameAndValue[0];
                string FieldSort = FieldNameAndValue[1];
                SortExpression se = new SortExpression();
                se.SortName = FieldName;
                se.SortType = eSortType.Asc;
                if (FieldSort.ToUpper() == "DESC")
                {
                    se.SortType = eSortType.Desc;
                }
                SortList.Add(se);
            }
            return true;
        }
    }


}
