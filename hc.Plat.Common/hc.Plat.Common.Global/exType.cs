using System;
using System.Runtime.Serialization;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace hc.Plat.Common.Global
{


    /// <summary>
    ///  成功标志
    /// </summary>
    [DataContract]
    [Serializable]
    public enum EResultFlag
    {
        /// <summary>
        ///  成功
        /// </summary>
        [EnumMember]
        Success = 0,

        /// <summary>
        ///  失败
        /// </summary>
        [EnumMember]
        Failure = -1,
    }

    /// <summary>
    ///  异常类型
    /// </summary>
    [DataContract]
    [Serializable]
    public enum EExceptionType
    {
        /// <summary>
        ///  未知错误
        /// </summary>
        [EnumMember]
        Unknown = -1,

        /// <summary>
        ///  记录日志
        /// </summary>
        [EnumMember]
        WriteLogging = 0,

        /// <summary>
        ///  异常错误
        /// </summary>
        [EnumMember]
        ExceptionError = 1,

        /// <summary>
        ///  业务错误
        /// </summary>
        [EnumMember]
        BussinessError = 2,

        /// <summary>
        ///  网络错误
        /// </summary>
        [EnumMember]
        NetError = 3,

        /// <summary>
        ///  接口错误
        /// </summary>
        [EnumMember]
        InterfaceError = 4,
    }



    [Serializable]
    [DataContract]
    public class ExceptionEx
    {
        /// <summary>
        ///  异常编码
        /// </summary>
        [DataMember]
        public string Code { get; set; }
        /// <summary>
        ///  异常描述
        /// </summary>
        [DataMember]
        public string Decription { get; set; }
        /// <summary>
        ///  异常类型
        /// </summary>
        [DataMember]
        public EExceptionType ExceptionType { get; set; }

        /// <summary>
        /// 错误源：应用程序名/程序集/模块名/类名
        /// </summary>
        [DataMember]
        public string Source { get; set; }


        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="type">异常类型</param>
        /// <param name="source">错误源：应用程序名/程序集/模块名/类名</param>
        /// <param name="code">异常编码</param>
        /// <param name="description">异常描述</param>
        public ExceptionEx(EExceptionType type, string source, string code, string description)
        {
            this.ExceptionType = type;
            this.Source = source;
            this.Code = code;
            this.Decription = description;
        }

        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="exception">系统异常</param>
        /// <param name="source">错误源：应用程序名/程序集/模块名/类名</param>
        public ExceptionEx(Exception exception, string source)
        {
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
            }

            this.Source = source;
            this.ExceptionType = EExceptionType.ExceptionError;
            this.Decription = exception.Message;

            // 通过反射获取异常错误代码
            System.Reflection.PropertyInfo pCode = exception.GetType().GetProperties()
                .FirstOrDefault(p => p.Name != null && p.Name.Contains("Code"));
            if (pCode != null)
            {
                this.Code = string.Format("{0}", pCode.GetValue(exception, null));
            }

        }
    }



    /// <summary>
    ///  操作结果对象
    /// </summary>
    /// <typeparam name="TData">结果数据类型</typeparam>
    /// [Serializable]
    /// 
    [DataContract(Name = "Result_{0}")]
    [Serializable]
   public class Result<TData>
    {
        private int? fAllRowsCount;
        /// <summary>
        ///  总计的记录数（非当前返回的记录数，如果未设置记录数时，取当前返回集合的总数）
        /// </summary>
        [DataMember]
        public int AllRowsCount
        {
            set { fAllRowsCount = value; }
            get
            {
                int iCount = 0;

                // 未设置记录数时，取当前返回集合的总数
                if ((fAllRowsCount == null) && (this.Data != null))
                {
                    PropertyInfo propCount = this.Data.GetType().GetProperty("Count");
                    if (propCount != null)
                    {
                        iCount = (int)propCount.GetValue(this.Data, null);
                    }
                }
                else if (fAllRowsCount != null)
                {
                    iCount = fAllRowsCount.Value;
                }
                return iCount;
            }
        }


        /// <summary>
        ///  执行结果标志
        /// </summary>
        [DataMember]
        public EResultFlag Flag { get; set; }

        /// <summary>
        ///  结果数据
        /// </summary>
        [DataMember]
        public TData Data { get; set; }

        /// <summary>
        ///  异常信息
        /// </summary>
        [DataMember]
        public ExceptionEx Exception { get; set; }

        /// <summary>
        ///  构造函数，默认操作成功
        /// </summary>
        public Result()
        {
            //this.RecordCount = 0;
            this.Flag = EResultFlag.Success;
        }

        /// <summary>
        ///  相等运算
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(Result<TData> a, EResultFlag b)
        {
            bool bEqual = false;
            if (a != null && a.Flag == b)
            {
                bEqual = true;
            }

            return bEqual;
        }

        /// <summary>
        ///  相等运算
        /// </summary>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static bool operator ==(EResultFlag b, Result<TData> a)
        {
            return (a == b);
        }

        /// <summary>
        ///  不等运算符
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(Result<TData> a, EResultFlag b)
        {
            bool bEqual = false;
            if (a != null && a.Flag == b)
            {
                bEqual = true;
            }

            return !bEqual;
        }

        /// <summary>
        ///  不等运算符
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(EResultFlag b, Result<TData> a)
        {
            return (a != b);
        }


        public override int GetHashCode()
        {
            return base.GetHashCode() ^ this.ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Result<TData> b = obj as Result<TData>;

            if (b == null)
            {
                return false;
            }

            return this.AllRowsCount == b.AllRowsCount && this.Data.Equals(b.Data);
        }    
    }
}
