using System;


namespace hc.Plat.Common.Global
{
    /// <summary>
    /// 全局Session字符串变量；
    /// </summary>
    public static class ConstStr_Session
    {
        /// <summary>
        /// 当前用户Session的全局Session字符串；
        /// </summary>
        public static string CurrentUserEntity = "CurrentUserEntity";

        /// <summary>
        /// 当前站点配置信息的Session字符串
        /// </summary>
        public static string CurrentConfigEntity = "CurrentConfigEntity";
        /// <summary>
        /// 当前代理信息的Session字符串
        /// </summary>
        public static string CurrentProxyExType = "CurrentProxyExType";
        /// <summary>
        /// 当前验证码的Session字符串；
        /// </summary>
        public static string CurrValidateCode = "CurrValidateCode";

        /// <summary>
        /// 当前手机注册验证码
        /// </summary>
        public static string CurrRegPhoneCode = "CurrRegPhoneCode";

        /// <summary>
        /// 当前手机登录验证码
        /// </summary>
        public static string CurrLoginPhoneCode = "CurrLoginPhoneCode";

        /// <summary>
        /// 当前手机认证验证码
        /// </summary>
        public static string CurrQualifPhoneCode = "CurrQualifPhoneCode";

        /// <summary>
        /// 找回密码验证码
        /// </summary>
        public static string CurrFindPwdCode = "CurrFindPwdCode";
        /// <summary>
        /// 验证码时间
        /// </summary>
        public static string CurrValidateCodeTime = "CurrValidateCodeTime";

        /// <summary>
        /// 当前找回密保的手机验证码
        /// </summary>
        public static string CurPhoneEncryptCode = "CurPhoneEncryptCode";

        /// <summary>
        /// 当前登录用户权限的 Session 字符串
        /// </summary>
        public static string CurrUserRight = "CurrUserRight";
    }  
}