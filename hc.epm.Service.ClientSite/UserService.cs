using System;
using System.Collections.Generic;
using System.Linq;

using hc.Plat.Common.Global;
using hc.epm.DataModel.Basic;
using hc.epm.Service.Base;
using hc.epm.ViewModel;
using hc.epm.Common;
using hc.Plat.Common.Extend;
using hc.epm.DataModel.Business;

using hc.epm.Service.Msg;
using hc.epm.DataModel.Msg;
using System.Data;
using System.Drawing;
using System.IO;
using Baidu.Aip.Face;
using Newtonsoft.Json.Linq;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        #region 用户登录

        public Result<UserView> Login(string userName, string password, string type)
        {
            Result<UserView> result = new Result<UserView>();
            try
            {
                var model = DataOperateBasic<Base_User>.Get().Single(i => i.UserAcct == userName || i.Phone == userName);
                //去注册站点
                if (model == null)
                {
                    throw new Exception("用户名不存在！");
                }

                //登录日志
                WriteLoginLog("", model);
                if (model.IsLock)
                {
                    throw new Exception("登录账号已被锁定！");
                }
                if (model.PassWord != password && type == "0")
                {
                    model.LockNum += 1;
                    //密码错误超过5次，锁定账户
                    if (model.LockNum > 5)
                    {
                        model.IsLock = true;
                    }

                    DataOperateBasic<Base_User>.Get().Update(model);

                    int count = (5 - model.LockNum);
                    if (count >= 0)
                    {
                        throw new Exception("用户名密码错误，还有" + count + "次机会，账户将被锁定！");
                    }
                    else
                    {
                        throw new Exception("账户已被锁定！");
                    }

                }

                UserView mv = GetUserView(model);

                //修改最后登录时间
                model.LastLoginTime = DateTime.Now;
                model.LockNum = 0;
                DataOperateBasic<Base_User>.Get().Update(model);
                result.Data = mv;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "Login");
            }
            return result;
        }

        private UserView GetUserView(Base_User model)
        {
            BasicDataContext baseDB = new BasicDataContext();
            try
            {
                UserView mv = new UserView();
                var company = DataOperateBasic<Base_Company>.Get(baseDB).GetModel(model.CompanyId);

                if (!string.IsNullOrEmpty(company.Type))
                {
                    mv.CompanyId = model.CompanyId;
                    mv.CompanyName = company.Name;
                    mv.CompanyNo = company.Code;
                    mv.CompanyType = company.Type;
                    var rights = LoadRightList(company.Type, model.Id);


                    //获取所有菜单id
                    mv.MenuIds = new List<long>();
                    if (rights.Data != null && rights.Data.Any())
                    {
                        mv.MenuIds = rights.Data.Where(i => i.IsMenu).Select(i => i.Id).ToList();
                    }

                    mv.UserCode = model.UserCode;
                    //获取所有权限id
                    mv.RightIds = new List<long>();
                    mv.Rights = new Dictionary<string, string>();
                    if (rights.Data != null && rights.Data.Any())
                    {
                        mv.RightIds = rights.Data.Select(i => i.Id).ToList();
                        mv.Rights = rights.Data.ToDictionary(i => i.Id.ToString(), j => j.ParentCode + "_" + j.Code);
                    }
                    //获取所有角色ids
                    var roleIds = GetRolesByUserId(model.Id);
                    mv.RoleIds = new List<long>();
                    if (roleIds.Data != null && roleIds.Data.Any())
                    {
                        mv.RoleIds = roleIds.Data.Select(i => i.Id).ToList();
                    }
                    mv.RoleType = (RoleType)Enum.Parse(typeof(RoleType), company.Type);

                    if (mv.RoleType == RoleType.Admin)
                    {
                        throw new Exception("你暂时无权登录本系统！");
                    }

                    mv.Project = new Dictionary<long, string>();
                    Result<List<Epm_Project>> project = GetProjectListById(model.CompanyId, model.Id);
                    if (project.Data != null)
                    {
                        foreach (var item in project.Data)
                        {
                            mv.Project.Add(item.Id, item.Name);
                        }
                    }

                    mv.UserId = model.Id;
                    mv.UserName = model.UserAcct;
                    mv.Email = model.Email;
                    mv.Phone = model.Phone;
                    mv.RealName = model.UserName;
                    mv.Qq = model.QQ;
                    mv.WeChat = model.WebChat;
                    //获取token
                    mv.AndroidToken = model.AndroidToken;
                    mv.AndroidTokenTime = model.AndroidTokenTime;
                    mv.IosToken = model.IosToken;
                    mv.IosTokenTime = model.IosTokenTime;
                    mv.LastLoginTime = model.LastLoginTime;
                    return mv;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("你暂时无权登录本系统！");
            }
            finally
            {
                if (baseDB.Database.Connection.State != System.Data.ConnectionState.Closed)
                {
                    baseDB.Database.Connection.Dispose();

                }
            }
            throw new Exception("你暂时无权登录本系统！");
        }


        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Base_User> GetUserModel(long id)
        {
            Result<Base_User> result = new Result<Base_User>();
            try
            {
                var model = DataOperateBasic<Base_User>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetUserModel");
            }
            return result;
        }

        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="token"></param>
        /// <param name="type">1:Android,2:IOS</param>
        /// <returns></returns>
        public Result<UserView> GetUserModelByToken(string token, int type)
        {
            Result<UserView> result = new Result<UserView>();
            try
            {
                Base_User model;
                UserView mv = new UserView();
                using (BasicDataContext baseDB = new BasicDataContext())
                {

                    if (type == 1)
                    {
                        model = baseDB.Base_User.SingleOrDefault(i => i.AndroidToken == token);
                    }
                    else
                    {
                        model = baseDB.Base_User.SingleOrDefault(i => i.IosToken == token);
                    }

                    var company = baseDB.Base_Company.SingleOrDefault(i => i.Id == model.CompanyId);

                    if (!string.IsNullOrEmpty(company.Type))
                    {
                        mv.CompanyId = model.CompanyId;
                        mv.CompanyName = company.Name;
                        mv.CompanyNo = company.Code;
                        mv.CompanyType = company.Type;
                        var list = baseDB.Base_Right.Where(i => i.Belong == RoleType.Owner.ToString() && !i.IsDelete).OrderBy(i => i.Sort).ToList();

                        //获取用户角色
                        var roles = baseDB.Base_UserRole.Where(i => i.UserId == model.Id && !i.IsDelete).ToList();
                        //获取所有角色权限
                        var roleIds = roles.Select(i => i.RoleId).ToList();
                        var listRight = baseDB.Base_RoleRight.Where(i => roleIds.Contains(i.RoleId) && !i.IsDelete).Select(i => i.RightId).ToList();

                        list = list.Where(i => listRight.Contains(i.Id) && !i.IsDelete).ToList();
                        var rights = list;

                        //获取所有菜单id
                        mv.MenuIds = new List<long>();
                        if (rights.Any())
                        {
                            mv.MenuIds = rights.Where(i => i.IsMenu).Select(i => i.Id).ToList();
                        }

                        mv.UserCode = model.UserCode;
                        //获取所有权限id
                        mv.RightIds = new List<long>();
                        mv.Rights = new Dictionary<string, string>();
                        if (rights.Any())
                        {
                            mv.RightIds = rights.Select(i => i.Id).ToList();
                            mv.Rights = rights.ToDictionary(i => i.Id.ToString(), j => j.ParentCode + "_" + j.Code);
                        }
                        //获取所有角色ids
                        mv.RoleIds = new List<long>();
                        if (roleIds.Any())
                        {
                            mv.RoleIds = roleIds;
                        }
                        mv.RoleType = (RoleType)Enum.Parse(typeof(RoleType), company.Type);

                        if (mv.RoleType == RoleType.Admin)
                        {
                            throw new Exception("你暂时无权登录本系统！");
                        }

                        mv.UserId = model.Id;
                        mv.UserName = model.UserAcct;
                        mv.Email = model.Email;
                        mv.Phone = model.Phone;
                        mv.RealName = model.UserName;
                        mv.Sex = model.Sex;
                        mv.Qq = model.QQ;
                        mv.WeChat = model.WebChat;
                        //获取token
                        mv.AndroidToken = model.AndroidToken;
                        mv.AndroidTokenTime = model.AndroidTokenTime;
                        mv.IosToken = model.IosToken;
                        mv.IosTokenTime = model.IosTokenTime;
                        mv.LastLoginTime = model.LastLoginTime;
                    }
                }
                result.Data = mv;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetUserModelByToken");
            }

            return result;
        }


        public Result<Base_User> GetBaseUserByToken(string token, int type)
        {
            Result<Base_User> result = new Result<Base_User>();
            try
            {
                Base_User model;
                if (type == 1)
                {
                    model = DataOperateBasic<Base_User>.Get().Single(i => i.AndroidToken == token);
                }
                else
                {
                    model = DataOperateBasic<Base_User>.Get().Single(i => i.IosToken == token);
                }
                if (model == null)
                {
                    throw new Exception("token不存在！");
                }

                result.Data = model;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetBaseUserByToken");
            }
            return result;
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateUser(Base_User model)
        {
            var oldModel = DataOperateBasic<Base_User>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);

            Result<int> result = new Result<int>();
            try
            {
                bool user = DataOperateBasic<Base_User>.Get().Count(i => i.UserAcct == model.UserAcct && i.Id != model.Id) > 0;
                //if (user)
                //{
                //    throw new Exception("该登录账户名已经存在");
                //}
                //user = DataOperateBasic<Base_User>.Get().Count(i => i.Phone == model.Phone && i.Id != model.Id) > 0;
                //if (user)
                //{
                //    throw new Exception("该手机号已经存在");
                //}
                //user = DataOperateBasic<Base_User>.Get().Count(i => i.Email == model.Email && i.Id != model.Id) > 0;
                //if (user)
                //{
                //    throw new Exception("该邮箱已经存在");
                //}
                var rows = DataOperateBasic<Base_User>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                WriteLog(AdminModule.UserManager.GetText(), SystemRight.Modify.GetText(), "用户修改:" + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateUser");
            }
            return result;
        }

        /// <summary>
        /// 根据用户加载对应权限，有缓存
        /// </summary>
        /// <param name="roleType">角色类型</param>
        /// <param name="userId">用户 ID</param>
        /// <param name="listRight"></param>
        /// <returns></returns>
        public Result<List<Base_Right>> LoadRightList(string roleType, long userId = 0, List<long> listRight = null)
        {
            Result<List<Base_Right>> result = new Result<List<Base_Right>>();

            BasicDataContext baseDB = new BasicDataContext();
            try
            {
                var list = DataOperateBasic<Base_Right>.Get(baseDB).GetList(i => i.Belong == RoleType.Owner.ToString()).OrderBy(i => i.Sort).ToList();
                if (userId != 0)
                {
                    if (listRight == null)//当前用户的所有权限
                    {
                        //获取用户角色
                        var roles = DataOperateBasic<Base_UserRole>.Get(baseDB).GetList(i => i.UserId == userId).ToList();
                        //获取所有角色权限
                        var roleIds = roles.Select(i => i.RoleId).ToList();
                        listRight = DataOperateBasic<Base_RoleRight>.Get(baseDB).GetList(i => roleIds.Contains(i.RoleId)).Select(i => i.RightId).ToList();
                    }
                    list = list.Where(i => listRight.Contains(i.Id)).ToList();
                }

                result.Data = list;
                result.AllRowsCount = list.Count();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "LoadMenuList");
            }
            finally
            {
                if (baseDB.Database.Connection.State != System.Data.ConnectionState.Closed)
                {
                    baseDB.Database.Connection.Dispose();
                }
            }
            return result;
        }

        /// <summary>
        /// 根据人员id获取该人所属的所有角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Result<List<Base_UserRole>> GetRolesByUserId(long userId)
        {
            Result<List<Base_UserRole>> result = new Result<List<Base_UserRole>>();
            BasicDataContext baseDB = new BasicDataContext();
            try
            {
                var list = DataOperateBasic<Base_UserRole>.Get(baseDB).GetList(i => i.UserId == userId).ToList();
                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetRolesByUserId");
            }
            finally
            {
                if (baseDB.Database.Connection.State != System.Data.ConnectionState.Closed)
                {
                    baseDB.Database.Connection.Dispose();

                }
            }
            return result;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userName"></param>
        /// <param name="phone"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Result<List<UserView>> GetUserListByWhr(string type, string userName, string phone, int pageIndex, int pageSize)
        {
            Result<List<UserView>> result = new Result<List<UserView>>();
            RoleType rtype = (RoleType)Enum.Parse(typeof(RoleType), type);
            try
            {
                var model = (from user in basicContext.Base_User
                             join com in basicContext.Base_Company on user.CompanyId equals com.Id
                             where (com.Type == type && (userName == "" || user.UserName.Contains(userName)) && (phone == "" || user.Phone == phone))
                             select new UserView()
                             {
                                 UserName = user.UserName,
                                 UserId = user.Id,
                                 UserCode = user.UserCode,
                                 Phone = user.Phone,
                                 CompanyName = com.Name,
                                 CompanyId = user.CompanyId,
                                 RoleType = rtype,
                                 RealName = user.UserAcct
                             }).ToList();

                result.AllRowsCount = model.Count();
                model = model.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetUserListByWhr");
            }
            return result;
        }

        /// <summary>
        /// 获取人才管理库列表
        /// </summary>
        /// <param name="qc"></param>
        /// <param name="type">1：查个人，2：所属企业，默认查全部</param>
        /// <returns></returns>
        public Result<List<UserListView>> GetUserManageList(QueryCondition qc, string type)
        {
            Result<List<UserListView>> result = new Result<List<UserListView>>();
            try
            {
                using (BasicDataContext baseDataContext = new BasicDataContext())
                {
                    var query = from a in baseDataContext.Base_User.Where(p => p.IsDelete == false)
                                join b in baseDataContext.Base_Company.Where(p => p.IsDelete == false) on a.CompanyId equals
                                    b.Id into bref
                                from b in bref.DefaultIfEmpty()
                                select new UserListView
                                {
                                    UserId = a.Id,
                                    UserCode = a.UserCode,
                                    UserName = a.UserName,
                                    Email = a.Email,
                                    Phone = a.Phone,
                                    CompanyId = a.CompanyId,
                                    CompanyType = b.Type,
                                    CompanyName = b.Name,
                                    CompanyNo = b.Code,
                                    WeChat = a.WebChat,
                                    Qq = a.QQ,
                                    PostValue = a.PostValue,
                                    Post = a.Post,
                                    ProfessionalValue = a.ProfessionalValue,
                                    Professional = a.Professional,
                                    ProfessionalQualificationValue = a.ProfessionalQualificationValue,
                                    ProfessionalQualification = a.ProfessionalQualification,
                                    CreateTime = a.OperateTime,
                                };

                    if (qc.ConditionList != null && qc.ConditionList.Any())
                    {
                        foreach (var conditionExpression in qc.ConditionList)
                        {
                            string value = (conditionExpression.ExpValue ?? "").ToString();
                            if (!string.IsNullOrWhiteSpace(value))
                            {
                                switch (conditionExpression.ExpName)
                                {
                                    case "CompanyName":
                                        query = query.Where(p => p.CompanyName.Contains(value));
                                        break;
                                    case "UserName":
                                        query = query.Where(p => p.UserName.Contains(value));
                                        break;
                                    case "PostValue":
                                        query = query.Where(p => p.PostValue.Contains(value));
                                        break;
                                    case "CompanyId":
                                        query = query.Where(p => p.CompanyId.ToString() == value);
                                        break;
                                }
                            }
                        }
                    }

                    if (type == "1")
                    {
                        query = query.Where(t => t.UserId == CurrentUser.Id);
                    }
                    else if (type == "2")
                    {
                        query = query.Where(t => t.CompanyId == CurrentUser.CompanyId);
                    }
                    int count = query.Count();
                    int skip = (qc.PageInfo.CurrentPageIndex - 1) * qc.PageInfo.PageRowCount;
                    int take = qc.PageInfo.PageRowCount;

                    var list = query.OrderByDescending(p => p.CreateTime).Skip(skip).Take(take).ToList();
                    result.Data = list;
                    result.AllRowsCount = count;
                    result.Flag = EResultFlag.Success;
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetUserCompanyList");
            }
            return result;
        }

        /// <summary>
        /// 查询用户详情
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Result<Base_User> GetUserDetail(long userId)
        {
            Result<Base_User> result = new Result<Base_User>();
            //  UserInfoView list = new UserInfoView();

            BasicDataContext baseDB = new BasicDataContext();
            try
            {
                var model = DataOperateBasic<Base_User>.Get().GetModel(userId);
                if (model != null)
                {
                    var com = DataOperateBasic<Base_Company>.Get().GetList(t => t.Id == model.CompanyId).FirstOrDefault();
                    model.CompanyName = com.Name ?? "";
                    var fileList = DataOperateBasic<Base_Files>.Get().GetList(t => t.TableId == userId && t.TableName == "Base_User").ToList();
                    if (!fileList.Any())
                    {
                        fileList = new List<Base_Files>();
                    }
                    else
                    {

                        foreach (var item in fileList)
                        {
                            if (string.IsNullOrEmpty(item.ImageType))
                            {
                                if (!string.IsNullOrEmpty(item.Url) && item.TableColumn == "SSRYGLZP")//照片在保存的时候有进行格式转换
                                {
                                    item.imageUrl = "data:image/jpeg;base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes(item.Url));
                                }
                            }
                            if (!string.IsNullOrEmpty(item.TableColumn))
                            {
                                //转换
                                var noToName = DataOperateBasic<Base_TypeDictionary>.Get().Single(p => p.No == item.TableColumn);
                                if (noToName != null)
                                {
                                    item.TypeName = noToName.Name;

                                }
                            }

                        }
                    }
                    model.fileList = fileList;
                    if (!string.IsNullOrWhiteSpace(model.Address))
                    {
                        string[] codes = model.Address.Split(',');
                        string code = codes[codes.Length - 1];
                        var regmodel = DataOperateBasic<Base_Region>.Get(baseDB).Single(i => i.RegionCode == code);
                        if (regmodel != null)
                        {
                            model.addressName = regmodel.Fullname;
                        }
                    }
                }
                // result.Data = model;

                //  list.AIUserFace = DataOperateBusiness<EPM_AIUserFace>.Get().Single(p=>p.UserId== userId);
                if (!string.IsNullOrEmpty(model.PassWord))
                {
                    model.PassWord = DesTool.DesDecrypt(model.PassWord);
                }

                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetUserDetail");
            }
            return result;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddUserInfo(Base_User model, string image, List<Base_Files> fileList = null)
        {
            model = base.SetCurrentUser(model);
            Result<int> result = new Result<int>();
            try
            {
                bool user = DataOperateBasic<Base_User>.Get().Count(i => i.UserAcct == model.UserAcct) > 0;
                if (user)
                {
                    throw new Exception("该登录账户名已经存在");
                }
                user = DataOperateBasic<Base_User>.Get().Count(i => i.Phone == model.Phone) > 0;
                if (user)
                {
                    throw new Exception("该手机号已经存在");
                }
                if (string.IsNullOrEmpty(model.UserAcct))
                {
                    model.UserAcct = model.Phone;
                }
                model.LastLoginTime = DateTime.Now;
                model.PassTime = DateTime.Now;

                var rows = DataOperateBasic<Base_User>.Get().Add(model);
                //  string image = "";
                if (fileList != null)
                {
                    foreach (var item in fileList)
                    {
                        if (string.IsNullOrEmpty(item.ImageType) && item.TableColumn == "SSRYGLZP")//修改之前是CZ类型
                        {
                            image = item.Url;
                        }

                    }

                    //新增附件
                    AddFilesByTable(model, fileList);
                }
                if (!string.IsNullOrEmpty(image))
                {
                    // image = Convert.ToBase64String(System.IO.File.ReadAllBytes(image));
                    AddAIUserFaceInfo(model.Id, image, "PC");//人脸注册
                }


                result.Data = rows;
                result.Flag = EResultFlag.Success;

                WriteLog(AdminModule.UserManager.GetText(), SystemRight.Add.GetText(), "用户新增:" + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddUser");
            }
            return result;
        }


        /// <summary>
        /// 批量添加用户信息
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public Result<int> AddRangeUser(List<Base_User> models)
        {
            Result<int> result = new Result<int>();
            try
            {
                if (models != null)
                {
                    foreach (var item in models)
                    {
                        SetCreateUser(item);
                        SetCurrentUser(item);
                        item.LastLoginTime = DateTime.Now;
                        item.PassTime = DateTime.Now;
                    }
                }
                var rows = DataOperateBusiness<Base_User>.Get().AddRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddRangeUser");
            }
            return result;
        }

        /// <summary>
        /// 根据用户名称或者电话号码查询用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public Result<Base_User> GetUserInfoByNameAndPhone(string userName, string phone)
        {
            Result<Base_User> result = new Result<Base_User>();

            BasicDataContext baseDB = new BasicDataContext();
            try
            {
                var model = DataOperateBasic<Base_User>.Get().Single(t => t.UserName == userName || t.Phone == phone);

                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetUserInfoByNameAndPhone");
            }
            return result;
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateUserInfo(Base_User model, string image, List<Base_Files> fileList = null)
        {
            var oldModel = DataOperateBasic<Base_User>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);
            model.LastLoginTime = oldModel.LastLoginTime;
            model.PassTime = oldModel.PassTime;
            //model.PassWord = oldModel.PassWord;
            model.CompanyId = oldModel.CompanyId;//所属单位ID

            if (string.IsNullOrEmpty(model.UserAcct))
            {
                model.UserAcct = model.Phone;
            }
            Result<int> result = new Result<int>();
            try
            {
                bool user = DataOperateBasic<Base_User>.Get().Count(i => i.UserAcct == model.UserAcct && i.Id != model.Id) > 0;
                if (user)
                {
                    throw new Exception("该登录账户名已经存在");
                }
                user = DataOperateBasic<Base_User>.Get().Count(i => i.Phone == model.Phone && i.Id != model.Id) > 0;
                if (user)
                {
                    throw new Exception("该手机号已经存在");
                }
                var rows = DataOperateBasic<Base_User>.Get().Update(model);
                //新增附件
                //AddFilesByTable(model, fileList);
                // string image = "";
                if (fileList != null)
                {
                    foreach (var item in fileList)
                    {
                        if (string.IsNullOrEmpty(item.ImageType) && item.TableColumn == "SSRYGLZP")//修改之前是CZ类型
                        {
                            image = item.Url;
                        }
                    }

                    //删除之前的附件
                    DeleteFilesByTable(model.GetType().Name, new List<long>() { model.Id });
                    //新增附件
                    AddFilesByTable(model, fileList);
                }
                if (!string.IsNullOrEmpty(image))
                {
                    // image = Convert.ToBase64String(System.IO.File.ReadAllBytes(image));
                    AddAIUserFaceInfo(model.Id, image, "PC");//人脸注册
                }

                result.Data = rows;
                result.Flag = EResultFlag.Success;

                WriteLog(AdminModule.UserManager.GetText(), SystemRight.Modify.GetText(), "用户修改:" + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateUserInfo");
            }
            return result;
        }

        /// <summary>
        /// 批量删除用户
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteUserByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBasic<Base_User>.Get().GetList(i => ids.Contains(i.Id)).ToList();

                var rows = DataOperateBasic<Base_User>.Get().DeleteRange(models);
                foreach (var item in models)
                {
                    var userface = DataOperateBusiness<EPM_AIUserFace>.Get().GetList(t => t.UserId == item.Id).FirstOrDefault();
                    if (userface != null)
                    {
                        DelAIUserFaceInfo(item.Id, userface.FaceToken);
                        DataOperateBusiness<EPM_AIUserFace>.Get().Delete(userface);
                    }
                }
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                #region 写日志
                WriteLog(AdminModule.UserManager.GetText(), SystemRight.Delete.GetText(), "批量删除用户:" + rows);
                #endregion
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteUserByIds");
            }
            return result;
        }

        /// <summary>
        /// 删除百度人脸库中数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="faceToken"></param>
        /// <returns></returns>
        private Result<int> DelAIUserFaceInfo(long userId, string faceToken)
        {
            Result<int> resultObj = new Result<int>();
            resultObj.Data = -1;
            BusinessDataContext busdb = new BusinessDataContext();
            try
            {
                var groupId = "group1";
                Face client = GetFacaClient();
                JObject result = new JObject();
                //人脸日志表
                EPM_FaceOperateLog faceLog = new EPM_FaceOperateLog();
                //日志
                var requestJson = new
                {
                    groupId = groupId,
                    userId = userId,
                    faceToken = faceToken
                };
                faceLog.ModelId = 0;
                faceLog.RequestJson = requestJson.ToString();
                faceLog.APIType = FaceOperate.Delete.ToString();
                faceLog = SetCurrentUser(faceLog);
                try
                {
                    result = client.FaceDelete(userId.ToString(), groupId, faceToken);

                    if (result["error_code"].ToString() == "0" && result["error_msg"].ToString() == "SUCCESS")
                    {
                        faceLog.IsSuccess = true;

                        resultObj.Data = 1;
                    }
                    else
                    {
                        faceLog.IsSuccess = false;
                    }
                }
                catch (Exception)
                {
                    faceLog.IsSuccess = false;
                }
                faceLog.ResponseJson = result.ToString();
                DataOperateBusiness<EPM_FaceOperateLog>.Get(busdb).Add(faceLog);
                resultObj.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                resultObj.Data = -1;
                resultObj.Flag = EResultFlag.Failure;
                resultObj.Exception = new ExceptionEx(ex, "DelAIUserFaceInfo");
            }
            finally
            {
                if (busdb.Database.Connection.State != ConnectionState.Closed)
                {
                    busdb.Database.Connection.Close();
                    busdb.Database.Connection.Dispose();
                }
            }

            return resultObj;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="oldPwd">加密之后的原旧密码</param>
        /// <param name="pwd">加密之后的密码</param>
        /// <returns></returns>
        public Result<bool> UpdatePassword(long userId, string oldPwd, string pwd)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                var model = DataOperateBasic<Base_User>.Get().GetModel(userId);

                if (!oldPwd.Equals(model.PassWord))
                {
                    throw new Exception("旧密码不正确！");
                }

                //新增历史密码
                Base_HistoryPassword his = new Base_HistoryPassword();
                his.Num = "1";
                his.OldPassword = model.PassWord;
                his.Password = model.PassWord;
                his.UserId = userId;
                his.IsDelete = false;
                his = SetCreateUser(his);
                his = SetCurrentUser(his);

                DataOperateBasic<Base_HistoryPassword>.Get().Add(his);
                //修改密码
                model.PassWord = pwd;
                DataOperateBasic<Base_User>.Get().Update(model);

                result.Data = true;
                result.Flag = EResultFlag.Success;
                //WriteLog(WebModule.PersonalCenter.ToString(), SystemRight.Modify.GetText(), "用户修改密码:" + model.Id);
            }
            catch (Exception ex)
            {
                result.Flag = EResultFlag.Failure;
                result.Data = false;
                result.Exception = new ExceptionEx(ex, "UpdatePassword");
            }
            return result;
        }

        #endregion

        #region 获取站点设置

        /// <summary>
        /// 获取网站设置
        /// </summary>
        /// <returns></returns>
        public Result<Base_Config> LoadConfig()
        {
            Result<Base_Config> result = new Result<Base_Config>();
            try
            {
                var model = DataOperateBasic<Base_Config>.Get().GetList().FirstOrDefault();

                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "LoadConfig");
            }
            return result;
        }

        #endregion


        /// <summary>
        /// 写入站内消息
        /// </summary>
        /// <param name="receiveCompanyId">接收人单位Id</param>
        /// <param name="receiveId">接收人Id</param>
        /// <param name="step"></param>
        /// <param name="parameters"></param>
        /// <param name="linkURL"></param>
        private void WriteMessage(long? receiveCompanyId, long receiveId, MessageStep step, string BusinessUrl, Dictionary<string, string> parameters, string linkURL = "")
        {
            long sendCompnayId = string.IsNullOrWhiteSpace(CurrentCompanyID) ? (CurrentUser != null ? CurrentUser.CompanyId : 0) : CurrentCompanyID.ToLongReq();
            long sendId = string.IsNullOrWhiteSpace(CurrentUserID) ? (CurrentUser != null ? CurrentUser.Id : 0) : CurrentUserID.ToLongReq();
            long reCompanyId = receiveCompanyId.HasValue ? receiveCompanyId.Value : 0;
            new MsgService().AddMessage(sendId, sendCompnayId, receiveId, reCompanyId, step, BusinessUrl, parameters, linkURL);
        }

        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="receiveId"></param>
        /// <param name="receiveCompanyId"></param>
        /// <param name="step"></param>
        /// <param name="parameters"></param>
        private void WriteSMS(long receiveId, long? receiveCompanyId, MessageStep step, Dictionary<string, string> parameters)
        {
            long sendCompnayId = string.IsNullOrWhiteSpace(CurrentCompanyID) ? (CurrentUser != null ? CurrentUser.CompanyId : 0) : CurrentCompanyID.ToLongReq();
            long sendId = string.IsNullOrWhiteSpace(CurrentUserID) ? (CurrentUser != null ? CurrentUser.Id : 0) : CurrentUserID.ToLongReq();
            long reCompanyId = receiveCompanyId.HasValue ? receiveCompanyId.Value : 0;
            new MsgService().AddSMS(sendId, sendCompnayId, receiveId, reCompanyId, step, parameters);
        }

        /// <summary>
        /// 获取消息列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Msg_Message>> GetMessageList(QueryCondition qc)
        {
            return new MsgService().GetMessageList(qc);
        }

        #region 面包屑导航

        /// <summary>
        /// 获取面包屑导航
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public Result<List<Base_Right>> GetBreadcrumbNavigation(string controllerName, string actionName)
        {
            Result<List<Base_Right>> result = new Result<List<Base_Right>>();
            try
            {
                List<Base_Right> list = new List<Base_Right>();

                string route = string.Format("{0}/{1}", controllerName, actionName);
                var singleModel = DataOperateBasic<Base_Right>.Get().Single(p => p.Tips == route);
                if (singleModel != null)
                {
                    if (singleModel.RightType == FunctionType.Module.ToString() || singleModel.RightType == FunctionType.Action.ToString() || singleModel.RightType == FunctionType.Category.ToString())
                    {
                        list.Add(singleModel);
                        GetBreadcrumb(singleModel.ParentId, list);
                    }
                    else if (singleModel.RightType == FunctionType.Nav.ToString())
                    {
                        if (singleModel.Code == WebNav.LoginHome.ToString() || singleModel.Code == WebNav.PersonalCenter.ToString())
                        {
                            list.Add(singleModel);
                        }
                    }
                    else
                    {
                        list.Add(singleModel);
                    }
                }
                list.Reverse();
                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch
            {
                result.Data = new List<Base_Right>();
                result.Flag = EResultFlag.Success;
            }
            return result;
        }

        private void GetBreadcrumb(long id, List<Base_Right> list)
        {
            var singleModel = DataOperateBasic<Base_Right>.Get().Single(p => p.Id == id);
            if (singleModel.RightType == FunctionType.Module.ToString() || singleModel.RightType == FunctionType.Action.ToString() || singleModel.RightType == FunctionType.Category.ToString())
            {
                list.Add(singleModel);
                GetBreadcrumb(singleModel.ParentId, list);
            }
        }

        #endregion

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TemplateDetails>> GetTemplateDetailsList(long templateId)
        {
            Result<List<Epm_TemplateDetails>> result = new Result<List<Epm_TemplateDetails>>();
            try
            {
                var model = DataOperateBusiness<Epm_TemplateDetails>.Get().GetList(t => t.TemplateId == templateId).ToList();

                result.Data = model;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTemplateDetailsList");
            }
            return result;
        }

        /// <summary>
        /// 根据模板编码获取模板列表
        /// </summary>
        /// <param name="templateNo"></param>
        /// <returns></returns>
        public Result<List<Epm_Template>> GetTemplateListByNo(string templateNo)
        {
            Result<List<Epm_Template>> result = new Result<List<Epm_Template>>();
            try
            {
                //TODO：需要将TemplateTypeName改成TemplateTypeNo
                var model = DataOperateBusiness<Epm_Template>.Get().GetList(t => t.TemplateTypeName == templateNo).ToList();

                result.Data = model;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTemplateListByNo");
            }
            return result;
        }

        /// <summary>
        /// 获取字典Id
        /// </summary>
        /// <param name="dicNo"></param>
        /// <returns></returns>
        public Result<Base_TypeDictionary> GetDictionaryId(string dicNo)
        {
            Result<Base_TypeDictionary> result = new Result<Base_TypeDictionary>();
            try
            {
                var model = DataOperateBasic<Base_TypeDictionary>.Get().GetList(t => t.No == dicNo).FirstOrDefault();

                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetDictionaryId");
            }
            return result;
        }

        /// <summary>
        /// 根据字典id获取模板列表
        /// </summary>
        /// <param name="dicId"></param>
        /// <returns></returns>
        public Result<List<Epm_Template>> GetTemplateListDicId(long dicId, string title)
        {
            Result<List<Epm_Template>> result = new Result<List<Epm_Template>>();
            try
            {
                var model = DataOperateBusiness<Epm_Template>.Get().GetList(t => t.TemplateTypeId == dicId && (string.IsNullOrEmpty(title) || t.Title.Contains(title))).ToList();

                result.Data = model;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTemplateListDicId");
            }
            return result;
        }

        /// <summary>
        /// 根据父级ID获取检查项
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public Result<List<Epm_CheckItem>> GetCheckItemList(long pid)
        {
            Result<List<Epm_CheckItem>> result = new Result<List<Epm_CheckItem>>();

            try
            {
                var model = DataOperateBusiness<Epm_CheckItem>.Get().GetList(t => t.ParentId == pid).ToList();

                result.Data = model;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCheckItemList");
            }
            return result;
        }


        #region 获取检查项树形列表数据
        /// <summary>
        /// 获取检查项树形列表数据
        /// </summary>
        /// <returns></returns>
        public Result<List<CheckItemView>> GetCheckItem(long projectid, long userid)
        {
            var checkRoleType = string.Empty;
            bool isAgency = IsAgencyUser(userid);
            if (!isAgency)
            {
                bool isBranch = IsBranchCompanyUser(userid);
                if (!isBranch)
                {
                    bool isSupervisor = IsSupervisor(projectid, userid);
                    if (isSupervisor)
                    {
                        checkRoleType = RoleTypeEnum.JL.ToString();
                    }
                    else
                    {
                        checkRoleType = RoleTypeEnum.ZJL.ToString();
                    }
                }
                else
                {
                    checkRoleType = RoleTypeEnum.FGS.ToString();
                }
            }
            else
            {
                checkRoleType = RoleTypeEnum.SGS.ToString();
            }

            Result<List<CheckItemView>> result = new Result<List<CheckItemView>>();
            List<CheckItemView> list = new List<CheckItemView>();

            try
            {
                var checkitemlist = DataOperateBusiness<Epm_CheckItem>.Get().GetList(t => t.IsDelete == false && t.RoleType == checkRoleType).OrderBy(t => t.Sort).ToList();
                CheckItemView node = null;
                foreach (var item in checkitemlist)
                {
                    node = new CheckItemView();
                    node.id = item.Id.ToString();
                    node.parentId = item.ParentId;
                    node.name = (item.Level.Value == 3 ? item.Remark : item.Name);
                    node.parentName = item.ParentName;
                    node.level = item.Level;
                    node.scoreRange = item.ScoreRange;
                    node.rectificationManName = item.RectificationManName;
                    list.Add(node);
                }

                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCheckItem");
            }
            return result;
        }

        /// <summary>
        /// 根据父级ID获取检查项
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public Result<List<Epm_CheckItem>> GetCheckItemAll()
        {
            Result<List<Epm_CheckItem>> result = new Result<List<Epm_CheckItem>>();

            try
            {
                var model = DataOperateBusiness<Epm_CheckItem>.Get().GetList().ToList();

                result.Data = model;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCheckItemAll");
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 根据检查角色类型获取检查数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Result<List<Epm_CheckItem>> GetCheckItemListByType(string type)
        {
            Result<List<Epm_CheckItem>> result = new Result<List<Epm_CheckItem>>();

            try
            {
                var model = DataOperateBusiness<Epm_CheckItem>.Get().GetList(t => t.RoleType == type).ToList();

                result.Data = model;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCheckItemListByType");
            }
            return result;
        }

        public Result<List<Epm_CheckItem>> GetCheckItemListByTypeName(string type, string name, int level)
        {
            Result<List<Epm_CheckItem>> result = new Result<List<Epm_CheckItem>>();

            try
            {
                var model = DataOperateBusiness<Epm_CheckItem>.Get().GetList(t => t.RoleType == type && t.Level == level && t.Name == name).ToList();

                result.Data = model;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCheckItemListByTypeName");
            }
            return result;
        }

        /// <summary>
        /// 根据广告位编码获取广告投放记录
        /// </summary>
        /// <param name="targetNum"></param>
        /// <returns></returns>
        public Result<List<Base_Files>> GetAdPutRecord(string targetNum, string imageType)
        {
            Result<List<Base_Files>> result = new Result<List<Base_Files>>();
            try
            {
                var models = DataOperateBusiness<Epm_AdPutRecord>.Get().GetList(t => t.AdTargetNum == targetNum && t.State == 1).OrderBy(t => t.Sort).ToList();
                var ids = models.Select(t => t.Id).ToList().JoinToString(",");
                //var model = DataOperateBasic<Base_Files>.Get().GetModel(id);
                List<Base_Files> fileList = new List<Base_Files>();
                if (models.Any())
                {
                    foreach (var item in models)
                    {
                        Base_Files file = DataOperateBasic<Base_Files>.Get().GetList(t => t.ImageType == imageType && t.TableId == item.Id).FirstOrDefault();
                        fileList.Add(file);
                    }
                }
                //QueryCondition qc = new QueryCondition();
                //qc = AddDefault(qc);
                //qc.PageInfo.isAllowPage = false;
                //qc.ConditionList.Add(new ConditionExpression()
                //{
                //    ExpName = "TableId",
                //    ExpValue = ids,
                //    ExpLogical = eLogicalOperator.And,
                //    ExpOperater = eConditionOperator.In
                //});
                //if (!string.IsNullOrEmpty(imageType))
                //{
                //    qc.ConditionList.Add(new ConditionExpression()
                //    {
                //        ExpName = "ImageType",
                //        ExpValue = imageType,
                //        ExpLogical = eLogicalOperator.And,
                //        ExpOperater = eConditionOperator.Equal
                //    });
                //}
                //BasicDataContext basicContext = new BasicDataContext();
                //result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Base_Files>(basicContext, qc);
                result.Data = fileList;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetAdPutRecord");
            }
            return result;
        }

        /// <summary>
        /// 获取服务商（根据总批复构成获取关联的服务商）
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_ConstituteCompanyDetails>> GetConstituteCompanyDetailsList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_ConstituteCompanyDetails>> result = new Result<List<Epm_ConstituteCompanyDetails>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_ConstituteCompanyDetails>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetConstituteCompanyDetailsList");
            }
            return result;
        }


        /// <summary>
        /// 根据 Cas 登录后的用户账号获取用户信息
        /// </summary>
        /// <param name="userName">用户账户</param>
        /// <returns></returns>
        public Result<UserView> LoginByCas(string userName)
        {
            Result<UserView> result = new Result<UserView>();
            try
            {
                var model = DataOperateBasic<Base_User>.Get().Single(i => i.UserAcct == userName);
                //去注册站点
                if (model == null)
                {
                    throw new Exception("用户名不存在！");
                }

                //登录日志
                WriteLoginLog("", model);

                UserView mv = GetUserView(model);

                //修改最后登录时间
                model.LastLoginTime = DateTime.Now;
                model.LockNum = 0;
                DataOperateBasic<Base_User>.Get().Update(model);
                result.Data = mv;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "LoginByCas");
            }
            return result;
        }

        public DataTable GetdtUserList(long companyId)
        {
            DataTable dt = new DataTable();
            Result<List<UserListView>> result = new Result<List<UserListView>>();

            using (BasicDataContext baseDataContext = new BasicDataContext())
            {
                var query = from a in baseDataContext.Base_User.Where(p => p.IsDelete == false)
                            join b in baseDataContext.Base_Company.Where(p => p.IsDelete == false) on a.CompanyId equals
                                b.Id into bref
                            from b in bref.DefaultIfEmpty()
                            select new UserListView
                            {
                                UserId = a.Id,
                                UserCode = a.UserCode,
                                UserName = a.UserName,
                                Email = a.Email,
                                Phone = a.Phone,
                                CompanyId = a.CompanyId,
                                CompanyType = b.Type,
                                CompanyName = b.Name,
                                CompanyNo = b.Code,
                                WeChat = a.WebChat,
                                Qq = a.QQ,
                                PostValue = a.PostValue,
                                Post = a.Post,
                                ProfessionalValue = a.ProfessionalValue,
                                Professional = a.Professional,
                                ProfessionalQualificationValue = a.ProfessionalQualificationValue,
                                ProfessionalQualification = a.ProfessionalQualification,
                                CreateTime = a.CreateTime,
                            };
                result.Data = query.Where(p => p.CompanyId == companyId).ToList();
            }
            if (result.Data.Count() > 0)
            {
                var Times = result.Data.ToList();
                for (int j = 0; j < Times.Count(); j++)
                {
                    //var rows = result.Data.Where(t => t.CreateTime == Times[j]).ToList();
                    if (Times.Count() > 0)
                    {
                        DataRow dr = null;
                        dr = dt.NewRow();
                        for (int i = 0; i < Times.Count(); i++)
                        {
                            dr["公司名称"] = Times[i].CompanyName;
                            dr["公司ID"] = Times[i].CompanyId;
                            dr["用户名"] = Times[i].UserName;
                            dr["用户Code"] = Times[i].UserCode;
                            dr["邮箱"] = Times[i].Email;
                            dr["电话"] = Times[i].Phone;
                            dr["微信"] = Times[i].WeChat;
                            dr["qq"] = Times[i].Qq;
                            dr["导入时间"] = Times[i].CreateTime.ToString("yyyy-MM-dd hh:mm:ss");
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
        }

        public Result<List<Base_User>> GetUserByCompanyId(long companyId)
        {
            Result<List<Base_User>> result = new Result<List<Base_User>>();
            try
            {
                var model = DataOperateBasic<Base_User>.Get().GetList();
                result.Data = model.Where(c => c.CompanyId == companyId).ToList();
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetUserModel");
            }
            return result;
        }
    }
}
