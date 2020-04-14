/************************************************************************************
 * Copyright (c) 2019  All Rights Reserved.
 * CLR版本： 4.0.30319.42000
 * 公司名称：陕西华春网络科技股份有限公司
 * 命名空间：hc.epm.Service.Basic
 * 文件名：  VideoManagerHelper
 * 版本号：  V1.0.0.0
 * 创建人：  wmg	
 * 电子邮箱：wmgwugang@huachun.com
 * 创建时间：2019/6/20 17:23:45
 * 描述：
 * 
 * 
 * 
 ************************************************************************************/

using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace hc.epm.Service.Basic
{
    /// <summary>
    /// 海康视频接口帮助
    /// </summary>
    public class VideoManagerHelper
    {
        /// <summary>
        /// 获取接口相关配置参数(包含 accessToken)
        /// </summary>
        /// <returns></returns>
        private static Result<HkInterfaceModel> GetAccessToken()
        {
            Result<HkInterfaceModel> result = new Result<HkInterfaceModel>();
            try
            {
                List<string> keys = new List<string>()
                {
                    Settings.HkAppKey.ToString(),
                    Settings.HkSecret.ToString(),
                    Settings.HkAccessToken.ToString(),
                    Settings.HkGetAccessTokenUrl.ToString(),
                    Settings.HkOpenVideoUrl.ToString(),
                    Settings.HkVideoUrl.ToString(),
                    Settings.HkAddVideoUrl.ToString(),
                    Settings.HkEditVideoUrl.ToString(),
                    Settings.HkDeleteVideoUrl.ToString()
                };

                var dic = DataOperateBasic<Base_Settings>.Get().GetList(p => keys.Contains(p.Code)).ToDictionary(p => p.Code, p => p.Value);
                if (dic == null || !dic.Any())
                {
                    throw new Exception("未配置设备相关参数接口！");
                }

                if (!dic.ContainsKey(Settings.HkAppKey.ToString()) ||
                    !dic.ContainsKey(Settings.HkSecret.ToString()) ||
                    !dic.ContainsKey(Settings.HkGetAccessTokenUrl.ToString()) ||
                    !dic.ContainsKey(Settings.HkOpenVideoUrl.ToString()) ||
                    !dic.ContainsKey(Settings.HkVideoUrl.ToString()) ||
                    !dic.ContainsKey(Settings.HkAddVideoUrl.ToString()) ||
                    !dic.ContainsKey(Settings.HkEditVideoUrl.ToString()) ||
                    !dic.ContainsKey(Settings.HkDeleteVideoUrl.ToString()))
                {
                    throw new Exception("未配置设备相关参数接口！");
                }

                HkInterfaceModel model = new HkInterfaceModel();
                model.Appkey = dic[Settings.HkAppKey.ToString()];
                model.Secret = dic[Settings.HkSecret.ToString()];
                model.AccessToken = dic[Settings.HkAccessToken.ToString()];
                model.AccessTokenUrl = dic[Settings.HkGetAccessTokenUrl.ToString()];
                model.OpenVideoUrl = dic[Settings.HkOpenVideoUrl.ToString()];
                model.VideoUrl = dic[Settings.HkVideoUrl.ToString()];
                model.AddVideoUrl = dic[Settings.HkAddVideoUrl.ToString()];
                model.EditVideoUrl = dic[Settings.HkEditVideoUrl.ToString()];
                model.DeleteVideoUrl = dic[Settings.HkDeleteVideoUrl.ToString()];

                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetAccessToken");
            }
            {
            }
            return result;
        }

        /// <summary>
        /// 获取可用 accessToken
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private static string GetNewAccessToken(HkInterfaceModel model)
        {
            try
            {
                string paramsValue = string.Format("appKey={0}&appSecret={1}", model.Appkey, model.Secret);
                var response = HttpHelper.Post(model.AccessTokenUrl, paramsValue);
                var data = JsonConvert.DeserializeObject<PackageData<AccessTokenModel>>(response);
                if (data != null)
                {
                    if (data.code == ReturnCode.Success)
                    {
                        if (data.data != null && !string.IsNullOrWhiteSpace(data.data.accessToken))
                        {
                            UpdateAccessToken(data.data.accessToken);
                            return data.data.accessToken;
                        }
                        else
                        {
                            throw new Exception("获取接口 accessToken 失败！");
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("接口错误：{0}-{1}", data.code, data.msg));
                    }
                }
                else
                {
                    throw new Exception("获取接口 accessToken 失败！");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新 accessToken
        /// </summary>
        /// <param name="newAccessToken"></param>
        /// <returns></returns>
        private static void UpdateAccessToken(string newAccessToken)
        {
            try
            {
                string accessTokenCode = Settings.HkAccessToken.ToString();
                var model = DataOperateBasic<Base_Settings>.Get().Single(p => p.Code == accessTokenCode);
                bool isAdd = false;
                if (model == null)
                {
                    isAdd = true;
                    model = new Base_Settings();
                    model.Code = accessTokenCode;
                    model.Name = Settings.HkAccessToken.GetText();
                    model.IsDelete = false;
                    model.CreateTime = DateTime.Now;
                    model.CreateUserId = 897711908373794816;
                    model.CreateUserName = "admin";
                }
                model.Value = newAccessToken;
                model.OperateTime = DateTime.Now;
                model.OperateUserId = 897711908373794816;
                model.OperateUserName = "admin";

                if (isAdd)
                {
                    DataOperateBasic<Base_Settings>.Get().Add(model);
                }
                else
                {
                    DataOperateBasic<Base_Settings>.Get().Update(model);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 根据设备编码获取直播地址 接口地址
        /// </summary>
        /// <param name="sn">设备序列号</param>
        /// <param name="isExpired">accessToken 是否过期</param>
        /// <returns></returns>
        private static string GetVideoAddress(string sn, bool isExpired)
        {
            var tokenResult = GetAccessToken();
            if (tokenResult.Flag == EResultFlag.Success && tokenResult.Data != null)
            {
                string accessToken = tokenResult.Data.AccessToken;
                if (string.IsNullOrWhiteSpace(accessToken) || isExpired)
                {
                    accessToken = GetNewAccessToken(tokenResult.Data);
                }
                string paramsValue = string.Format("accessToken={0}&deviceSerial={1}&channelNo={2}", accessToken, sn, 1);

                var response = HttpHelper.Post(tokenResult.Data.VideoUrl, paramsValue);
                var data = JsonConvert.DeserializeObject<PackageData<VideoAddressModel>>(response);

                if (data != null)
                {
                    if (data.code == ReturnCode.Success)
                    {
                        if (data.data != null && (!string.IsNullOrWhiteSpace(data.data.rtmp) || !string.IsNullOrWhiteSpace(data.data.hlsHd)))
                        {
                            return string.IsNullOrWhiteSpace(data.data.hlsHd) ? data.data.rtmp : data.data.hlsHd;
                        }
                        else
                        {
                            throw new Exception("获取设备直播地址失败！");
                        }
                    }
                    else if (data.code == ReturnCode.AccessTokenExpired)
                    {
                        return GetVideoAddress(sn, true);
                    }
                    else
                    {
                        throw new Exception(string.Format("接口错误：{0}-{1}", data.code, data.msg));
                    }
                }
                else
                {
                    throw new Exception("获取接口 accessToken 失败！");
                }
            }
            else
            {
                throw new Exception(tokenResult.Exception.Decription);
            }
        }

        /// <summary>
        /// 新增设备 接口地址
        /// </summary>
        /// <param name="sn">设备编码</param>
        /// <param name="code">设备验证码</param>
        /// <param name="isExpired">accessToken 是否过期</param>
        /// <returns></returns>
        private static bool AddVideo(string sn, string code, bool isExpired)
        {
            var tokenResult = GetAccessToken();
            if (tokenResult.Flag == EResultFlag.Success && tokenResult.Data != null)
            {
                if (string.IsNullOrWhiteSpace(tokenResult.Data.AddVideoUrl))
                {
                    throw new Exception("未配置新增设备接口地址！");
                }
                string accessToken = tokenResult.Data.AccessToken;
                if (string.IsNullOrWhiteSpace(accessToken) || isExpired)
                {
                    accessToken = GetNewAccessToken(tokenResult.Data);
                }
                string paramsValue = string.Format("accessToken={0}&deviceSerial={1}&validateCode={2}", accessToken, sn, code);

                var response = HttpHelper.Post(tokenResult.Data.AddVideoUrl, paramsValue);
                var data = JsonConvert.DeserializeObject<PackageData<VideoModel>>(response);

                if (data != null)
                {
                    if (data.code == ReturnCode.Success)
                    {
                        return true;
                    }
                    else if (data.code == ReturnCode.AccessTokenExpired)
                    {
                        return AddVideo(sn, code, true);
                    }
                    else
                    {
                        throw new Exception(string.Format("接口错误：{0}-{1}", data.code, data.msg));
                    }
                }
                else
                {
                    throw new Exception("新增设备失败！");
                }
            }
            else
            {
                throw new Exception(tokenResult.Exception.Decription);
            }
        }

        /// <summary>
        /// 删除设备 接口地址
        /// </summary>
        /// <param name="sn">设备编码</param>
        /// <param name="isExpired">accessToken 是否过期</param>
        /// <returns></returns>
        private static bool DeleteVideo(string sn, bool isExpired)
        {
            var tokenResult = GetAccessToken();
            if (tokenResult.Flag == EResultFlag.Success && tokenResult.Data != null)
            {
                if (string.IsNullOrWhiteSpace(tokenResult.Data.DeleteVideoUrl))
                {
                    throw new Exception("未配置删除设备接口地址！");
                }
                string accessToken = tokenResult.Data.AccessToken;
                if (string.IsNullOrWhiteSpace(accessToken) || isExpired)
                {
                    accessToken = GetNewAccessToken(tokenResult.Data);
                }
                string paramsValue = string.Format("accessToken={0}&deviceSerial={1}", accessToken, sn);

                var response = HttpHelper.Post(tokenResult.Data.DeleteVideoUrl, paramsValue);
                var data = JsonConvert.DeserializeObject<PackageData<VideoModel>>(response);

                if (data != null)
                {
                    if (data.code == ReturnCode.Success)
                    {
                        return true;
                    }
                    else if (data.code == ReturnCode.AccessTokenExpired)
                    {
                        return DeleteVideo(sn, true);
                    }
                    else
                    {
                        throw new Exception(string.Format("接口错误：{0}-{1}", data.code, data.msg));
                    }
                }
                else
                {
                    throw new Exception("删除设备失败！");
                }
            }
            else
            {
                throw new Exception(tokenResult.Exception.Decription);
            }
        }

        /// <summary>
        /// 根据设备编码获取设备直播地址
        /// </summary>
        /// <param name="sn">设备唯一编码</param>
        /// <returns></returns>
        public static Result<string> GetVideoAddress(string sn)
        {
            Result<string> result = new Result<string>();
            try
            {
                result.Data = GetVideoAddress(sn, false);
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Exception = new ExceptionEx(ex, "");
                result.Data = string.Empty;
                result.Flag = EResultFlag.Failure;
            }
            return result;
        }

        /// <summary>
        /// 添加设备至萤石云
        /// </summary>
        /// <param name="sn">设备编码</param>
        /// <param name="code">设备验证码</param>
        /// <returns></returns>
        public static Result<bool> AddEquipment(string sn, string code)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                result.Data = AddVideo(sn, code, false);
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Exception = new ExceptionEx(ex, "");
                result.Data = false;
                result.Flag = EResultFlag.Failure;
            }
            return result;
        }

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="sn">设备编码</param>
        /// <returns></returns>
        public static Result<bool> DeleteEquipment(string sn)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                result.Data = DeleteVideo(sn, false);
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Exception = new ExceptionEx(ex, "");
                result.Data = false;
                result.Flag = EResultFlag.Failure;
            }
            return result;
        }
    }

    /// <summary>
    /// 海康视频接口参数模型
    /// </summary>
    public class HkInterfaceModel
    {
        /// <summary>
        /// Appkey
        /// </summary>
        public string Appkey { get; set; }

        /// <summary>
        /// Secret
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// AccessToken
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// 获取最新 AccessToken 地址
        /// </summary>
        public string AccessTokenUrl { get; set; }

        /// <summary>
        /// 开通视频直播地址
        /// </summary>
        public string OpenVideoUrl { get; set; }

        /// <summary>
        /// 获取视频直播地址(永久有效)
        /// </summary>
        public string VideoUrl { get; set; }

        /// <summary>
        /// 新增设备接口地址
        /// </summary>
        public string AddVideoUrl { get; set; }

        /// <summary>
        /// 修改设备接口地址
        /// </summary>
        public string EditVideoUrl { get; set; }

        /// <summary>
        /// 删除设备接口地址
        /// </summary>
        public string DeleteVideoUrl { get; set; }
    }

    /// <summary>
    /// 接口返回对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PackageData<T>
    {
        public string code { get; set; }

        public string msg { get; set; }

        public T data { get; set; }
    }

    /// <summary>
    /// 获取 accessToken 返回结果类型
    /// </summary>
    public class AccessTokenModel
    {
        public string accessToken { get; set; }

        public long expireTime { get; set; }
    }

    /// <summary>
    /// 开通视频直播返回结果类型
    /// </summary>
    public class OpenVideoModel
    {
        public string deviceSerial { get; set; }

        public int channelNo { get; set; }

        public string ret { get; set; }

        public string desc { get; set; }
    }

    /// <summary>
    /// 获取视频直播地址返回结果模型
    /// </summary>
    public class VideoAddressModel
    {
        public string deviceSerial { get; set; }

        public int channelNo { get; set; }

        public string deviceName { get; set; }

        public string hls { get; set; }

        public string hlsHd { get; set; }

        public string rtmp { get; set; }

        public string rtmpHd { get; set; }

        public int status { get; set; }

        public int exception { get; set; }

        public string ret { get; set; }

        public string desc { get; set; }
    }


    public class VideoModel
    {

    }

    /// <summary>
    /// 接口返回码
    /// </summary>
    public class ReturnCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        public const string Success = "200";

        /// <summary>
        /// AccessToken 过期
        /// </summary>
        public const string AccessTokenExpired = "10002";
    }
}
