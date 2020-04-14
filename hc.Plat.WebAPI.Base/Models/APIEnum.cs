using hc.Plat.Common.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hc.Plat.WebAPI.Base.Models
{
    /// <summary>
    /// api接口返回码，建议至少成功或失败情形各划分一类code，如失败提示都是1开始，成功都是2开始等
    /// </summary>
    public enum MsgCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        [EnumText("成功")]
        Success = 200,

        /// <summary>
        /// 用户名或密码错误
        /// </summary>
        [EnumText("用户名或密码错误")]
        LoginError = 100,
        /// <summary>
        /// Token不存在或已失效
        /// </summary>
        [EnumText("用户账号过期，请重新登录")]
        InvalidToken = 101,
        /// <summary>
        /// 时间戳校验失败
        /// </summary>
        [EnumText("时间戳校验失败")]
        SignTimeError = 102,
        /// <summary>
        /// 获取用户信息失败
        /// </summary>
        [EnumText("获取用户信息错误")]
        UserInfoError = 103,

        /// <summary>
        /// 无权限进行该操作
        /// </summary>
        [EnumText("无权限进行该操作！")]
        Unauthorized = 401,

        /// <summary>
        /// 操作失败： 一般性错误
        /// </summary>
        [EnumText("操作失败")]
        CommonError = 500,
        
    }

    public enum State
    {
        [EnumText("草稿")]
        Enabled = 0,
        [EnumText("在建")]
        Construction = 5,
        [EnumText("结项")]
        Success = 10,
        [EnumText("终止")]
        Failure = 15,
        [EnumText("作废")]
        Discard = 20,
        [EnumText("无效")]
        Invalid = 25,
        [EnumText("待审核")]
        WaitAppr = 30,
        [EnumText("审核通过")]
        ApprSuccess = 35,
        [EnumText("审核不通过")]
        ApprFailure = 40,
        [EnumText("待确认")]
        WaitConfirm = 45,
        [EnumText("确认通过")]
        Confirm = 50,
        [EnumText("确认不通过")]
        ConfirmFailure = 55,
        [EnumText("已废弃")]
        Discarded = 60,
        [EnumText("待检查")]
        WaitCheck = 65,
        [EnumText("检查通过")]
        CheckSuccess = 70,
        [EnumText("整改中")]
        Rectification = 75,
        [EnumText("整改后通过")]
        UpdateOk = 80,
        [EnumText("待整改")]
        WaitRectification = 85,
        [EnumText("已整改")]
        Rectificationed = 90,
        [EnumText("整改通过")]
        RectificationSuccess = 95,
        [EnumText("整改不通过")]
        RectificationOk = 99,
    }



    /// <summary>
    /// 文件类型枚举
    /// </summary>
    public enum EpmFileType
    {
        /// <summary>
        /// 图片
        /// </summary>
        [EnumText("图片")]
        Image = 0,

        /// <summary>
        /// 视频
        /// </summary>
        [EnumText("视频")]
        Video = 1,

        /// <summary>
        /// 文件
        /// </summary>
        [EnumText("文件")]
        File = 2,

        /// <summary>
        /// 模型
        /// </summary>
        [EnumText("模型")]
        Model = 3,

        /// <summary>
        /// 未知类型
        /// </summary>
        [EnumText("未知类型")]
        Other = 9
    }
}