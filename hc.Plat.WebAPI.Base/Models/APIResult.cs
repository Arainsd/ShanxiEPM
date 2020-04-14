using System.Collections.Generic;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;

namespace hc.Plat.WebAPI.Base.Models
{
    /// <summary>
    /// API返回结果类
    /// </summary>
    public class APIResult
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 响应数据
        /// </summary>
        public object data { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// 分页条数
        /// </summary>
        public int pageSize { get; set; }

        /// <summary>
        /// 列表总页数
        /// </summary>
        public int total { get; set; }

        /// <summary>
        /// 返回无数据操作成功提示
        /// </summary>
        /// <returns></returns>
        public static APIResult GetSuccessNoData()
        {
            return GetSuccessResult(new{}, "未获取到相关数据！");
        }

        /// <summary>
        /// 返回列表无数据操作成功提示
        /// </summary>
        /// <returns></returns>
        public static APIResult GetSuccessNoDatas()
        {
            return GetSuccessResult(new List<object>(), "未获取到相关数据！");
        }
        
        /// <summary>
        /// 返回成功操作提示
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static APIResult GetSuccessResult(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = MsgCode.Success.GetText();
            }

            return new APIResult
            {
                code = ((int)MsgCode.Success).ToString(),
                msg = msg
            };
        }

        /// <summary>
        /// 返回成功操作结果
        /// </summary>
        /// <param name="obj">要返回的数据</param>
        /// <param name="msg">消息提示</param>
        /// <returns></returns>
        public static APIResult GetSuccessResult(object obj, string msg = "")
        {
            return GetSuccessResult(obj, 0, 0, 0, msg);
        }

        /// <summary>
        /// 返回成功操作结果，列表数据，带分页参数
        /// </summary>
        /// <param name="obj">列表数据</param>
        /// <param name="page">当前页码</param>
        /// <param name="total">列表总页数</param>
        /// <param name="pageSize">每页显示条数，默认 10 条</param>
        /// <param name="msg">消息提示</param>
        /// <returns></returns>
        public static APIResult GetSuccessResult(object obj, int page, int total, int pageSize = 10, string msg = "")
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = MsgCode.Success.GetText();
            }

            return new APIResult
            {
                code = ((int)MsgCode.Success).ToString(),
                data = obj,
                page = page,
                pageSize = pageSize,
                total = total,
                msg = msg
            };
        }

        /// <summary>
        /// 返回操作错误提示，默认返回错误代码 199
        /// </summary>
        /// <param name="msg">错误提示</param>
        /// <returns></returns>
        public static APIResult GetErrorResult(string msg = "")
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = MsgCode.CommonError.GetText();
            }
            return GetErrorResult(MsgCode.CommonError, msg);
        }

        /// <summary>
        /// 返回错误操作提示
        /// </summary>
        /// <param name="code">错误编码</param>
        /// <param name="msg">消息提示</param>
        /// <returns></returns>
        public static APIResult GetErrorResult(MsgCode code, string msg = "")
        {
            if (string.IsNullOrWhiteSpace(msg))
            {
                msg = code.GetText();
            }

            return new APIResult
            {
                code = ((int)code).ToString(),
                msg = msg
            };
        }

        /// <summary>
        /// 返回操作错误提示
        /// </summary>
        /// <param name="ex">服务异常描述</param>
        /// <returns></returns>
        public static APIResult GetErrorResult(ExceptionEx ex)
        {
            string msg = "获取数据失败！";
            if (ex != null)
            {
                msg = ex.Decription;
            }
            return GetErrorResult(msg);
        }
    }
}