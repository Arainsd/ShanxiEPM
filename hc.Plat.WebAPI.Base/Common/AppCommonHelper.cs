using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using hc.epm.DataModel.Basic;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using hc.Plat.WebAPI.Base.Models;
using hc.Plat.WebAPI.Base.ViewModel;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using hc.epm.Common;
using hc.epm.ViewModel;

namespace hc.Plat.WebAPI.Base.Common
{
    /// <summary>
    /// App 帮助类
    /// </summary>
    public class AppCommonHelper
    {

        /// <summary>
        /// 应用程序根目录
        /// </summary>
        public static readonly string AppPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        /// <summary>
        /// 附件上传路径
        /// </summary>
        private static string UploadUrlBase
        {
            get
            {
                string value = ConfigurationManager.AppSettings["UploadUrlBase"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("未配置基础平台资源服务器上传地址！");
                }
                return value;
            }
        }

        /// <summary>
        /// 附件下载路径
        /// </summary>
        private static string DownloadUrlBase
        {
            get
            {
                string value = ConfigurationManager.AppSettings["DownloadUrlBase"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("未配置基础平台资源服务器下载地址！");
                }
                return value;
            }
        }

        /// <summary>
        /// 缩略图附件标识
        /// </summary>
        private static string Small
        {
            get { return "small"; }
        }

        #region 系统设置项

        /// <summary>
        /// 系统默认每页显示数
        /// </summary>
        public static int PageSize
        {
            get
            {
                string pageSize = "10";
                if (SystemSetting.ContainsKey("pageSize"))
                {
                    pageSize = SystemSetting["pageSize"];
                    pageSize = string.IsNullOrWhiteSpace(pageSize) ? "10" : pageSize;
                }
                int pageSizeValue = int.TryParse(pageSize, out pageSizeValue) ? pageSizeValue : 10;
                return pageSizeValue;
            }
        }

        /// <summary>
        /// 资源地址
        /// </summary>
        private static string ResourceUrl
        {
            get
            {
                string url = "";
                if (SystemSetting.ContainsKey("resourceUrl"))
                {
                    url = SystemSetting["resourceUrl"];
                    if (!string.IsNullOrWhiteSpace(url))
                    {
                        if (!url.EndsWith("/"))
                        {
                            url += "/";
                        }
                    }
                }
                return url;
            }
        }


        /// <summary>
        /// 系统设置项
        /// </summary>
        public static Dictionary<string, string> SystemSetting
        {
            get
            {
                string jsonPath = AppPath + "/Config/SysSetting.json";
                string result = File.ReadAllText(jsonPath, Encoding.UTF8);
                var list = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
                return list;
            }
        }



        #endregion

        #region 资源附件

        /// <summary>
        /// 根据用户 ID 列表获取用户头像地址
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="userIds">用户 ID</param>
        /// <returns></returns>
        public static Dictionary<long, string> GetUserProfilePhotoList(ClientSiteClientProxy proxy, List<long> userIds)
        {
            Dictionary<long, string> userPhoto = new Dictionary<long, string>();
            var userProfilePhoto = proxy.GetUserProfilePhoto(userIds);
            if (userProfilePhoto.Flag == EResultFlag.Success && userProfilePhoto.Data != null)
            {
                string url = ResourceUrl;
                if (!string.IsNullOrWhiteSpace(url))
                {
                    foreach (var item in userProfilePhoto.Data)
                    {
                        string photoUrl = string.IsNullOrWhiteSpace(item.Value) ? "" : url + item.Value;
                        userPhoto.Add(item.Key, photoUrl);
                    }
                }
                else
                {
                    userPhoto = userProfilePhoto.Data;
                }
            }
            return userPhoto;
        }

        /// <summary>
        /// 根据用户 ID 获取用户个人头像地址
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userPhotoList"></param>
        /// <returns></returns>
        public static string GetUserProfilePhoto(long userId, Dictionary<long, string> userPhotoList, UserView user)
        {
            string headImg = string.Empty;
            if (userPhotoList == null || !userPhotoList.Any())
                headImg = "";
            if (userPhotoList != null && userPhotoList.ContainsKey(userId))
                headImg = userPhotoList[userId] ?? "";

            if (string.IsNullOrEmpty(headImg))
            {
                if (user != null)
                {
                    headImg = AppCommonHelper.SystemSetting["resourceUrl"] + "/Content/" + (user.Sex ? "mandefault.png" : "womandefault.png");
                }
                else
                {
                    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(new ClientProxyExType()))
                    {
                        Result<Base_User> baseUser = proxy.GetUserModel(userId);
                        if (baseUser.Data != null)
                        {
                            headImg = AppCommonHelper.SystemSetting["resourceUrl"] + "/Content/" + (baseUser.Data.Sex ? "mandefault.png" : "womandefault.png");
                        }
                        else
                        {
                            headImg = "";
                        }
                    }
                }
            }
            return headImg;
        }

        /// <summary>
        /// 根据 ID 获取附件集合
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="id">附件所属 ID</param>
        /// <returns></returns>
        public static List<Base_Files> GetBaseFileList(ClientSiteClientProxy proxy, long id)
        {
            return GetBaseFileList(proxy, new List<long>() { id });
        }

        /// <summary>
        /// 根据 ID 获取附件集合
        /// </summary>
        /// <param name="proxy"></param>
        /// <param name="id">附件所属 ID</param>
        /// <param name="isShowList">是否在列表中展示</param>
        /// <returns></returns>
        public static List<Base_Files> GetBaseFileList(ClientSiteClientProxy proxy, List<long> id, bool isShowList = false)
        {
            List<Base_Files> files = new List<Base_Files>();
            var filesResult = proxy.GetBaseFiles(id);
            if (filesResult.Flag == EResultFlag.Success && filesResult.Data.Any())
            {
                if (isShowList)
                {
                    foreach (var baseFile in filesResult.Data)
                    {
                        EpmFileType type = GetFileType(baseFile.Name);
                        if (type == EpmFileType.Image || type == EpmFileType.Video)
                        {
                            files.Add(baseFile);
                        }
                    }
                }
                else
                {
                    files = filesResult.Data;
                }
            }
            return files;
        }

        /// <summary>
        /// 根据文件后缀名获取文件类型
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        public static EpmFileType GetFileType(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return EpmFileType.Other;
            }
            string[] temp = fileName.Split('.');
            string ext = temp[temp.Length - 1];
            ext = ext.ToLower();
            switch (ext)
            {
                case "jpg":
                case "jpeg":
                case "gif":
                case "bmp":
                case "png":
                    return EpmFileType.Image;
                case "mp4":
                case "3gp":
                case "avi":
                case "mov":
                case "rmvb":
                case "flv":
                case "wmv":
                    return EpmFileType.Video;
                case "doc":
                case "docx":
                case "xls":
                case "xlsx":
                case "ppt":
                case "pptx":
                case "pdf":
                case "zip":
                case "7z":
                case "rar":
                    return EpmFileType.File;
                case "dwt":
                    return EpmFileType.Model;
                default:
                    return EpmFileType.File;
            }
        }

        /// <summary>
        /// 将附件列表转换为 APP 所需附件列表
        /// </summary>
        /// <param name="files">附件集合，某个具体业务下的附件</param>
        /// <param name="isShowList">是否在列表中展示，如果需要在列表中展示，则只显示图片或视频</param>
        /// <returns></returns>
        public static List<FileView> GetFileList(List<Base_Files> files, bool isShowList = false)
        {
            List<FileView> list = new List<FileView>();
            if (files != null && files.Any())
            {
                Base_Files small = files.Where(p => p.ImageType == Small).FirstOrDefault();

                if (small != null && small.TableName == "Epm_Bim")
                {
                    var resourceUrl = AppCommonHelper.SystemSetting["resourceUrl"];
                    list = new List<FileView>(){ new FileView()
                    {
                        id = (small == null ? Guid.NewGuid().ToString() : small.SId),
                        name =(small == null ? "" : small.Name),
                        type =(small == null ? "": GetFileType(small.Name).ToString()),
                        suffixName =(small == null ? "": small.Name.Substring(small.Name.LastIndexOf('.') + 1)),
                        size = (small == null ? "0 KB" : small.Size),
                        time = string.Format("{0:yyyy-MM-dd HH:mm}",(small == null ? DateTime.Now : small.CreateTime)),
                        //Url = (small == null ? ( resourceUrl+ "/Content/bimdoing.png") : (resourceUrl + small.Url)),
                        //imageUrlBig = (small == null ? (resourceUrl + "/Content/bimdoing.png") : (resourceUrl+ small.Url)),
                        imageUrlSmall =  (small == null ? (resourceUrl + "/Content/bimdoing.png") : (resourceUrl + small.Url)),
                        tableId = (small == null ? Guid.NewGuid().ToString() : small.TableId.ToString())
                    } };
                }
                else
                {
                    List<Base_Files> source = files.Where(p => string.IsNullOrEmpty(p.ImageType)).ToList();
                    list = source.Select(p => new FileView
                    {
                        id = p.SId,
                        name = p.Name,
                        type = GetFileType(p.Name).ToString(),
                        suffixName = p.Name.Substring(p.Name.LastIndexOf('.') + 1),
                        size = p.Size,
                        time = string.Format("{0:yyyy-MM-dd HH:mm}", p.CreateTime),
                        //Url = (GetFileType(p.Name) == EpmFileType.Image ? GetResourceUrl(files.Where(t => t.ImageType == Small && t.GuidId == p.GuidId).FirstOrDefault()) : string.Empty),
                        //imageUrlBig = (GetFileType(p.Name) == EpmFileType.Image ? GetResourceUrl(files.Where(t => t.ImageType == Small && t.GuidId == p.GuidId).FirstOrDefault()) : string.Empty),
                        imageUrlSmall = (GetFileType(p.Name) == EpmFileType.Image ? GetResourceUrl(files.Where(t => t.ImageType == Small && t.GuidId == p.GuidId).FirstOrDefault()) : string.Empty),
                        tableId = p.TableId.ToString()
                    }).ToList();
                }

                if (isShowList)
                {
                    // 是在列表中展示附件时，优先展示图片
                    list = list.Where(p => p.type == EpmFileType.Image.ToString()).Take(9).ToList();
                    if (!list.Any())
                    {
                        list = list.Where(p => p.type == EpmFileType.Video.ToString()).Take(1).ToList();
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获取资源 Url 路径
        /// </summary>
        /// <param name="url">资源相对路径</param>
        /// <returns></returns>
        public static string GetResourceUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return "";
            string resourceUrl = ResourceUrl;
            if (string.IsNullOrWhiteSpace(resourceUrl))
            {
                return "";
            }
            return resourceUrl + url;
        }

        /// <summary>
        /// 获取资源 Url 路径
        /// </summary>
        /// <param name="file">资源相对路径</param>
        /// <returns></returns>
        public static string GetResourceUrl(Base_Files file)
        {
            if (file != null && GetFileType(file.Name) == EpmFileType.Image)
            {
                return ResourceUrl + "/" + file.Url;
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion

        #region 权限相关

        /// <summary>
        /// 返回业务对应权限操作按钮
        /// </summary>
        /// <param name="user">当前登录用户</param>
        /// <param name="type">具体操作业务</param>
        /// <param name="createUserId">创建人</param>
        /// <param name="monitorCompanyId">整改单位</param>
        /// <returns></returns>
        public static List<Button> CreateButtonRight(UserView user, string type, long createUserId = 0, bool IsSupervisor = false)
        {
            List<Button> list = new List<Button>();
            if (user != null && user.Rights != null && user.Rights.Any())
            {
                type = type.ToLower() == "model" ? "Bim" : type;
                var right = user.Rights.Where(p => p.Value.Contains(type));
                if (right.Any())
                {
                    // 目前只返回 审核、驳回、作废、删除 四种权限中的可操作权限
                    foreach (KeyValuePair<string, string> item in right)
                    {
                        Button btn = new Button();
                        btn.rightId = item.Key;

                        string action = item.Value.Split('_')[1];

                        if (action.Equals(SystemRight.Check.ToString()))
                        {
                            btn.rightAction = SystemRight.Check.ToString();
                            btn.title = SystemRight.Check.GetText();
                            btn.color = GetButtonColor(SystemRight.Check);
                            list.Add(btn);
                            continue;
                        }
                        if (action.Equals(SystemRight.UnCheck.ToString()))
                        {
                            btn.rightAction = SystemRight.UnCheck.ToString();
                            btn.title = SystemRight.UnCheck.GetText();
                            btn.color = GetButtonColor(SystemRight.UnCheck);
                            list.Add(btn);
                            continue;
                        }
                        if (action.Equals(SystemRight.AuditRectif.ToString()))
                        {
                            btn.rightAction = SystemRight.AuditRectif.ToString();
                            btn.title = SystemRight.AuditRectif.GetText();
                            btn.color = GetButtonColor(SystemRight.AuditRectif);
                            list.Add(btn);
                            continue;
                        }
                        if (action.Equals(SystemRight.UploadSecurityCheck.ToString()) && IsSupervisor)
                        {
                            btn.rightAction = SystemRight.UploadSecurityCheck.ToString();
                            btn.title = SystemRight.UploadSecurityCheck.GetText();
                            btn.color = GetButtonColor(SystemRight.UploadSecurityCheck);
                            list.Add(btn);
                            continue;
                        }
                        if (action.Equals(SystemRight.RejectRectif.ToString()))
                        {
                            btn.rightAction = SystemRight.RejectRectif.ToString();
                            btn.title = SystemRight.RejectRectif.GetText();
                            btn.color = GetButtonColor(SystemRight.RejectRectif);
                            list.Add(btn);
                            continue;
                        }
                        if (action.Equals(SystemRight.Invalid.ToString()) && user.UserId == createUserId)
                        {
                            btn.rightAction = SystemRight.Invalid.ToString();
                            btn.title = SystemRight.Invalid.GetText();
                            btn.color = GetButtonColor(SystemRight.Invalid);
                            list.Add(btn);
                            continue;
                        }
                        if (action.Equals(SystemRight.Delete.ToString()) && user.UserId == createUserId)
                        {
                            btn.rightAction = SystemRight.Delete.ToString();
                            btn.title = SystemRight.Delete.GetText();
                            btn.color = GetButtonColor(SystemRight.Delete);
                            list.Add(btn);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 获取工程服务商操作权限
        /// </summary>
        /// <param name="user"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Button> CreateButtonRightProject(UserView user, string type, int? state, int isForS)
        {
            List<Button> btns = new List<Button>();
            if (user != null && user.Rights != null && user.Rights.Any())
            {
                var userRight = user.Rights.Where(p => p.Value.Contains(type));
                if (userRight.Any())
                {
                    foreach (var keyValuePair in userRight)
                    {
                        //(!pmId.HasValue && !linkId.HasValue && !state.HasValue && user.CompanyType == RoleType.Owner.ToString())
                        //    || (state.HasValue && linkState.HasValue && user.CompanyType == RoleType.Supplier.ToString() && (
                        //    (state.Value == (int)ApprovalState.ApprSuccess && linkState.Value == (int)ApprovalState.ApprSuccess)
                        //    || (state.HasValue && linkState.HasValue && state.Value == (int)ApprovalState.ApprFailure && linkState.Value == (int)ApprovalState.ApprFailure)))
                        if ((isForS == 2 && (state == (int)ApprovalState.ApprSuccess || state == (int)ApprovalState.ApprFailure))
                            || (isForS == 1 && !state.HasValue)
                            )
                        {
                            if (keyValuePair.Value.Contains(SystemRight.SetCustomerUser.ToString()))
                            {
                                Button btn = new Button();
                                btn.rightId = keyValuePair.Key;
                                btn.title = "提交";
                                btn.rightAction = SystemRight.SetCustomerUser.ToString();
                                btn.color = "";

                                btns.Add(btn);
                                continue;
                            }
                        }
                        else if (state.HasValue && state.Value == (int)ApprovalState.WaitAppr && isForS == 1)
                        {
                            if (keyValuePair.Value.Contains(SystemRight.AuditCustomerUser.ToString()))
                            {
                                Button btn = new Button();
                                btn.rightId = keyValuePair.Key;
                                btn.title = "审核";
                                btn.rightAction = SystemRight.AuditCustomerUser.ToString();
                                btn.color = "";

                                btns.Add(btn);
                                continue;
                            }

                            if (keyValuePair.Value.Contains(SystemRight.RejectCustomerUser.ToString()))
                            {
                                Button btn = new Button();
                                btn.rightId = keyValuePair.Key;
                                btn.title = "驳回";
                                btn.rightAction = SystemRight.RejectCustomerUser.ToString();
                                btn.color = "";

                                btns.Add(btn);
                            }
                        }
                    }
                }
            }
            return btns;
        }

        /// <summary>
        /// 获取项目工期操作权限
        /// </summary>
        /// <param name="user"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Button> CreateButtonRightProjectLimit(UserView user, string type)
        {
            List<Button> btns = new List<Button>();
            if (user != null && user.Rights != null && user.Rights.Any())
            {
                var userRight = user.Rights.Where(p => p.Value.Contains(type));
                if (userRight.Any())
                {
                    foreach (var keyValuePair in userRight)
                    {
                        if (keyValuePair.Value.Contains(SystemRight.SetProjectLimit.ToString()))
                        {
                            Button btn = new Button();
                            btn.rightId = keyValuePair.Key;
                            btn.title = SystemRight.SetProjectLimit.GetText();
                            btn.rightAction = SystemRight.SetProjectLimit.ToString();
                            btn.color = "";

                            btns.Add(btn);
                            continue;
                        }
                    }
                }
            }
            return btns;
        }
        /// <summary>
        /// 检查用户操作权限
        /// </summary>
        /// <param name="businessType">业务类型</param>
        /// <param name="action">操作类型</param>
        /// <param name="user">用户</param>
        /// <returns></returns>
        public static bool CheckRight(BusinessType businessType, SystemRight action, UserView user)
        {
            string rightValue = "";
            if (businessType == BusinessType.Project)
            {
                rightValue = string.Format("{0}_{1}", "ConProject", action.ToString());
            }
            else
            {
                rightValue = string.Format("{0}_{1}", businessType.ToString(), action.ToString());
            }
            return user.Rights.ContainsValue(rightValue);
        }

        /// <summary>
        /// 根据状态返回相应权限
        /// </summary>
        /// <param name="state"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<Button> GetRightByState(string state, List<Button> list)
        {
            List<Button> btnList = list;
            if (list.Count() > 0)
            {
                switch (state)
                {
                    case "WaitAppr":
                        btnList = list.Where(t => (t.rightAction == "Check" || t.rightAction == "UnCheck")).ToList();
                        break;
                    case "ApprSuccess":
                        btnList = null;
                        break;
                    case "ApprFailure":
                        btnList = list.Where(t => (t.rightAction == "Invalid")).ToList();
                        break;
                    case "Discarded":
                        btnList = list.Where(t => t.rightAction == "Delete").ToList();
                        break;
                    case "WaitConfirm":
                        btnList = list.Where(t => (t.rightAction == "Check" || t.rightAction == "UnCheck")).ToList();
                        break;
                    case "Confirm":
                        btnList = null;
                        break;
                    case "ConfirmFailure":
                        btnList = list.Where(t => (t.rightAction == "Invalid")).ToList();
                        break;
                    case "WorkPartAppr":
                        btnList = list.Where(t => (t.rightAction == "Check" || t.rightAction == "UnCheck")).ToList();
                        break;
                    case "WorkFinish":
                        btnList = null;
                        break;
                    case "Rectificationed":
                        btnList = null;
                        break;
                    case "RectificationSuccess":
                        btnList = null;
                        break;
                    case "WaitRectification":
                        btnList = list.Where(t => (t.rightAction == "AuditRectif" || t.rightAction == "RejectRectif" || t.rightAction == "UploadSecurityCheck")).ToList();
                        break;
                    case "RectificationOk":
                        //btnList = list.Where(t => (t.rightAction == "Invalid")).ToList();
                        break;
                    default:
                        break;
                }
            }
            return btnList;
        }

        /// <summary>
        /// 检查用户操作权限
        /// </summary>
        /// <param name="rightId">权限ID</param>
        /// <param name="user">用户</param>
        /// <returns></returns>
        public static bool CheckRight(string rightId, UserView user)
        {
            return user.Rights.ContainsKey(rightId);
        }


        /// <summary>
        /// 获取颜色按钮
        /// </summary>
        /// <param name="right"></param>
        /// <returns></returns>
        public static string GetButtonColor(SystemRight right)
        {
            switch (right)
            {
                case SystemRight.Check:
                    return "#5cb85c";
                case SystemRight.UnCheck:
                    return "#5bc0de";
                case SystemRight.Invalid:
                    return "#f0ad4e";
                case SystemRight.Delete:
                    return "#d9534f";
                case SystemRight.AuditRectif:
                    return "#5cb85c";
                case SystemRight.RejectRectif:
                    return "#5bc0de";
                default:
                    return "#286090";

            }
        }

        public static string GetWorkMainValue(string val, string WorkMainValues)
        {
            string result = "";
            if (!string.IsNullOrEmpty(WorkMainValues))
            {
                if (!string.IsNullOrEmpty(val))
                {
                    var list = (((WorkMainValues).Split(';').ToList().Select(k => new
                    {
                        id = k.Split(',')[0],
                        name = k.Split(',')[1]
                    })).Where(t => t.id == val || t.name == val)).ToList();

                    if (list.Count > 0)
                    {
                        result = list[0].name;
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(val))
                {
                    result = val;
                }
            }
            return result;
        }

        #endregion

        #region 附件上传、下载

        /// <summary>
        /// 附件上传
        /// <para>1. 如果启用图片压缩，在上传图片的时候会同步生成缩略图图片并保存</para>
        /// <para>2. 目前缩略图存储路径默认为站点根目录下，且发布时无需单独发布，可以直接访问</para>
        /// </summary>
        /// <param name="context">HttpContext 上下文</param>
        /// <param name="user">当前用户</param>
        /// <param name="columnName">附件功能区分</param>
        /// <returns></returns>
        public static List<Base_Files> UploadFile(HttpContext context, UserView user, string columnName = "")
        {
            List<Base_Files> fileList = new List<Base_Files>();

            HttpFileCollection filesCollection = context.Request.Files;
            if (filesCollection.Count > 0)
            {
                try
                {
                    int j = 0;
                    Dictionary<string, List<HttpPostedFile>> dictionary = new Dictionary<string, List<HttpPostedFile>>();
                    foreach (string fileKey in filesCollection)
                    {
                        HttpPostedFile httpPostedFile = filesCollection[j];
                        var key = fileKey;
                        if (!dictionary.ContainsKey(key))
                        {
                            dictionary.Add(key, new List<HttpPostedFile>() { httpPostedFile });
                        }
                        else
                        {
                            var files = dictionary[key];
                            files.Add(httpPostedFile);
                            dictionary[key] = files;
                        }
                        j++;
                    }
                    foreach (var item in dictionary)
                    {
                        List<HttpPostedFile> httpPostedFileList = item.Value;

                        for (int m = 0; m < httpPostedFileList.Count; m++)
                        {
                            HttpPostedFile httpPostedFile = httpPostedFileList[m];
                            string fileName = httpPostedFile.FileName;
                            string extension = Path.GetExtension(fileName);
                            string key = item.Key;
                            string fullPath = ConfigurationManager.AppSettings["TepmoraryPath"];
                            if (!Directory.Exists(fullPath))
                            {
                                Directory.CreateDirectory(fullPath);
                            }
                            string pathFile = fullPath + Guid.NewGuid() + extension;
                            httpPostedFile.SaveAs(pathFile);

                            string guid = Guid.NewGuid().ToString();

                            //获取文件大小
                            var file = new FileInfo(pathFile);

                            #region 上传服务器
                            // 判断是否启用本省资源服务器
                            string hbResourceUrl = ConfigurationManager.AppSettings["useHbResourceUrl"];
                            if ("false".Equals(hbResourceUrl))
                            {
                                Base_Files newFile = new Base_Files();
                                newFile.TableName = "";
                                newFile.TableId = 0;
                                newFile.TableColumn = (key == "stop" ? "Stop" : (key == "meeting" ? "Meeting" : key));
                                newFile.Name = fileName;
                                newFile.Size = GetLength(file.Length);
                                newFile.UploadUserId = user.UserId;
                                newFile.OperateUserId = user.UserId;
                                newFile.OperateTime = DateTime.Now;
                                newFile.IsDelete = false;
                                newFile.CreateUserId = user.UserId;
                                newFile.CreateTime = DateTime.Now;
                                newFile.CreateUserName = user.RealName;
                                newFile.OperateUserName = user.RealName;
                                newFile.Url = pathFile;
                                newFile.GuidId = guid;

                                fileList.Add(newFile);
                            }
                            else {
                                #region  上传本省服务器
                                string ucode = user.UserCode;
                                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(UploadUrlBase + "?fileName=" + fileName + "&RequestParam={%22Param%22:{%22envRoot%22:{%22UserName%22:%22" + ucode + "%22,%22Product%22:%22BIM%22},%22paramRoot%22:{%22FILE_NAME%22:%22" + fileName + "%22}}}");
                                // 边界符
                                var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
                                var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");

                                // 设置属性
                                webRequest.Method = "POST";
                                webRequest.Timeout = 600000;
                                webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

                                // Header
                                const string filePartHeader = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" + "Content-Type: application/octet-stream\r\n\r\n";
                                var header = string.Format(filePartHeader, "filepath", pathFile);
                                var headerbytes = Encoding.UTF8.GetBytes(header);

                                // 写入文件
                                var fileStream = new FileStream(pathFile, FileMode.Open, FileAccess.Read);

                                var memStream = new MemoryStream();
                                memStream.Write(beginBoundary, 0, beginBoundary.Length);
                                memStream.Write(headerbytes, 0, headerbytes.Length);

                                var buffer = new byte[fileStream.Length];
                                int bytesRead;
                                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                                {
                                    memStream.Write(buffer, 0, bytesRead);
                                }

                                // 写入字符串的Key
                                Dictionary<string, string> dicr = new Dictionary<string, string>();
                                dicr.Add("status", "1");
                                var stringKeyHeader = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"" + "\r\n\r\n{1}\r\n";
                                foreach (byte[] formitembytes in from string k in dicr.Keys select string.Format(stringKeyHeader, k, dicr[k]) into formitem select Encoding.UTF8.GetBytes(formitem))
                                {
                                    memStream.Write(formitembytes, 0, formitembytes.Length);
                                }

                                // 最后的结束符
                                var endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");
                                memStream.Write(endBoundary, 0, endBoundary.Length);
                                memStream.Position = 0;

                                webRequest.ContentLength = memStream.Length;
                                var requestStream = webRequest.GetRequestStream();

                                var tempBuffer = new byte[memStream.Length];

                                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                                memStream.Close();

                                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                                requestStream.Close();

                                string responseContent;
                                var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
                                using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("utf-8")))
                                {
                                    responseContent = httpStreamReader.ReadToEnd();
                                }
                                fileStream.Close();
                                httpWebResponse.Close();
                                webRequest.Abort();

                                if (!string.IsNullOrEmpty(responseContent))
                                {
                                    ResultUpload result = JsonConvert.DeserializeObject<ResultUpload>(responseContent);//将文件信息json字符

                                    Base_Files newFile = new Base_Files();
                                    newFile.TableName = "";
                                    newFile.TableId = 0;
                                    newFile.TableColumn = (key == "stop" ? "Stop" : (key == "meeting" ? "Meeting" : key));
                                    newFile.Name = fileName;
                                    newFile.Size = GetLength(file.Length);
                                    newFile.UploadUserId = user.UserId;
                                    newFile.OperateUserId = user.UserId;
                                    newFile.OperateTime = DateTime.Now;
                                    newFile.IsDelete = false;
                                    newFile.CreateUserId = user.UserId;
                                    newFile.CreateTime = DateTime.Now;
                                    newFile.CreateUserName = user.RealName;
                                    newFile.OperateUserName = user.RealName;
                                    newFile.Url = result.ResponseObject.responseObject[1];
                                    newFile.Group = result.ResponseObject.responseObject[0];
                                    newFile.GuidId = guid;

                                    fileList.Add(newFile);
                                }
                                #endregion
                            }
                            #endregion

                            #region 生成缩略图
                            // 生成缩略图
                            if (ImageHelper.IsImage(extension))
                            {
                                string makeThumbnail = ConfigurationManager.AppSettings["MakeThumbnail"];
                                bool isMakeThumbnail = Boolean.TryParse(makeThumbnail, out isMakeThumbnail) ? isMakeThumbnail : false;

                                /*
                                 * 缩略图存储路径处理和上传图片临时存储路径处理方式不同；
                                 * 缩略图存储路径在站点部署根目录下
                                 */
                                if (isMakeThumbnail)
                                {
                                    string thumbnailPath = ConfigurationManager.AppSettings["ThumbnailPath"];
                                    if (!string.IsNullOrWhiteSpace(thumbnailPath))
                                    {
                                        if (!thumbnailPath.EndsWith("\\"))
                                        {
                                            thumbnailPath += "\\";
                                        }

                                        //拼接相对路径
                                        string RelativePath = thumbnailPath.Substring(0, thumbnailPath.Length - 1);
                                        RelativePath = string.Format("{0}\\{1}\\{2}\\", RelativePath.Substring(RelativePath.LastIndexOf("\\") + 1), DateTime.Today.Year, DateTime.Today.Month);

                                        // 缩略图存储文件夹按年月格式生成
                                        thumbnailPath = string.Format("{0}{1}\\{2}\\", thumbnailPath, DateTime.Today.Year, DateTime.Today.Month);
                                        if (!Directory.Exists(thumbnailPath))
                                        {
                                            Directory.CreateDirectory(thumbnailPath);
                                        }

                                        string smallReName = string.Format("{0}{1}", Small, Guid.NewGuid() + fileName);

                                        if (file.Length < 1024 * 200)
                                        {
                                            File.Copy(pathFile, thumbnailPath + smallReName);
                                        }
                                        else
                                        {
                                            //ImageHelper.MakeThumbnailImage(pathFile, thumbnailPath + smallReName, 300, 300, ImageHelper.ImageCutMode.Cut);
                                            ImageHelper.CompressImage(pathFile, thumbnailPath + smallReName);
                                        }

                                        Base_Files newFile = new Base_Files();
                                        newFile.TableName = "";
                                        newFile.TableId = 0;
                                        newFile.TableColumn = (key == "stop" ? "Stop" : (key == "meeting" ? "Meeting" : key));
                                        newFile.Name = smallReName;
                                        newFile.Size = GetLength(file.Length);
                                        newFile.UploadUserId = user.UserId;
                                        newFile.OperateUserId = user.UserId;
                                        newFile.OperateTime = DateTime.Now;
                                        newFile.IsDelete = false;
                                        newFile.CreateUserId = user.UserId;
                                        newFile.CreateTime = DateTime.Now;
                                        newFile.CreateUserName = user.RealName;
                                        newFile.OperateUserName = user.RealName;
                                        newFile.Url = RelativePath.Replace("\\", "/") + smallReName;
                                        newFile.GuidId = guid;
                                        newFile.ImageType = Small;

                                        fileList.Add(newFile);
                                    }
                                }
                            }
                            #endregion

                            if (!"false".Equals(hbResourceUrl))
                            {
                                // 上传成功后， 删除临时文件
                                File.Delete(pathFile);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return fileList;
        }
        private static byte[] JavaBytesToNetBytes(int[] arr)
        {
            byte[] newByte = new byte[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] < 0)
                    newByte[i] = (byte)(arr[i] + 256);
                else
                    newByte[i] = (byte)arr[i];
            }
            return newByte;
        }

        /// <summary>
        /// 获取附件文件流
        /// </summary>
        /// <param name="group">资源组</param>
        /// <param name="path">资源路径</param>
        /// <param name="userCode">用户编码</param>
        /// <returns></returns>
        public static byte[] GetFilesBytes(string group, string path, string userCode, string fileName)
        {
            byte[] bytes;
            string tempPath = "c:\\temp\\";
            // 判断是否启用本省资源服务器
            string hbResourceUrl = ConfigurationManager.AppSettings["useHbResourceUrl"];
            if ("false".Equals(hbResourceUrl))
            {
                FileInfo file = file = new FileInfo(path);//创建一个文件对象

                if (file.Exists)
                {
                    FileStream stream = file.OpenRead();
                    var bufferLength = stream.Length;
                    bytes = new byte[bufferLength];
                    stream.Read(bytes, 0, Convert.ToInt32(bufferLength));
                    stream.Flush();
                    stream.Close();

                    return bytes;
                }
                else
                {
                    return new byte[0];
                }
            }
            else
            {
                HttpWebRequest request = null;
                try
                {
                    //string url = DownloadUrl + "?fileName=" + fileName + "&RequestParam={%22Param%22:{%22envRoot%22:{%22UserName%22:%22" + userCode + "%22,%22Product%22:%22BIM%22},%22paramRoot%22:{%22FDFS_GROUP%22:%22" + group + "%22,%22FDFS_NAME%22:%22" + path + "%22}}}";

                    string url = DownloadUrlBase + "?RequestParam={%22Param%22:{%22envRoot%22:{%22UserName%22:%22" + userCode + "%22,%22Product%22:%22BIM%22},%22paramRoot%22:{%22FDFS_GROUP%22:%22" + group + "%22,%22FDFS_NAME%22:%22" + path + "%22}}}";
                    request = (HttpWebRequest)HttpWebRequest.Create(url);
                    request.Method = "POST";
                    request.Timeout = 600000;
                    request.ContentType = "application/octet-stream";

                    string responseStr = string.Empty;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8"));
                        responseStr = reader.ReadToEnd().ToString();
                        reader.Close();

                        if (!string.IsNullOrEmpty(responseStr))
                        {
                            ResultDownUpload result = JsonConvert.DeserializeObject<ResultDownUpload>(responseStr);//将文件信息json字符
                            byte[] bytesBasePlat = JavaBytesToNetBytes(result.responseMap.FILE_DATA);
                            MemoryStream ms = new MemoryStream(bytesBasePlat);

                            //本地存储路径
                            if (!Directory.Exists(tempPath))
                            {
                                Directory.CreateDirectory(tempPath);
                            }
                            string pathFile = tempPath + Guid.NewGuid().ToString() + Path.GetExtension(fileName);
                            FileStream fs = new FileStream(pathFile, FileMode.OpenOrCreate);
                            ms.WriteTo(fs);
                            ms.Close();
                            fs.Close();

                            FileInfo file = new FileInfo(pathFile);
                            FileStream stream = file.OpenRead();
                            var bufferLength = stream.Length;
                            bytes = new byte[bufferLength];
                            stream.Read(bytes, 0, Convert.ToInt32(bufferLength));
                            stream.Flush();
                            stream.Close();

                            //// 下载成功后， 删除临时文件
                            //System.IO.File.Delete(pathFile);

                            return bytes;

                        }
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    if (request != null)
                    {
                        request.Abort();
                    }
                    bytes = new byte[0];
                }
                return bytes;
            }
        }

        /// <summary>
        /// 因为C#提供的文件的大小是以B为单位的，所以显示文件大小的时候会出现一大串数字很不方便
        /// 所以，该函数为了方便地显示文件大小而出现
        /// 函数说明，
        ///     如果文件大小是0-1024B 以内的   显示以B为单位
        ///     如果文件大小是1KB-1024KB之间的 显示以KB为单位
        ///     如果文件大小是1M-1024M之间的   显示以M为单位
        ///     如果文件大小是1024M以上的      显示以GB为单位
        /// </summary>
        /// <param name="lengthOfDocument"> 文件的大小 单位：B 类型：long</param>
        /// <returns></returns>
        private static string GetLength(long lengthOfDocument)
        {
            if (lengthOfDocument < 1024)
            {
                return string.Format(Math.Round((lengthOfDocument / 1.0), 2).ToString() + "B");
            }
            else if (lengthOfDocument > 1024 && lengthOfDocument <= Math.Pow(1024, 2))
            {
                return string.Format(Math.Round((lengthOfDocument / 1024.0), 2).ToString() + "KB");
            }
            else if (lengthOfDocument > Math.Pow(1024, 2) && lengthOfDocument <= Math.Pow(1024, 3))
            {
                return string.Format(Math.Round((lengthOfDocument / 1024.0 / 1024.0), 2).ToString() + "M");
            }
            else
            {
                return string.Format(Math.Round((lengthOfDocument / 1024.0 / 1024.0 / 1024.0), 2).ToString() + "GB");
            }
        }

        #endregion
    }
}