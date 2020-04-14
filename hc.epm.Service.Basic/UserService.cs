using AutoMapper;
using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using hc.Plat.Common.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace hc.epm.Service.Basic
{
    public partial class BasicService : BaseService, IBasicService
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password">传递过来的密码是密文</param>
        /// <returns></returns>
        public Result<UserView> Login(string userName, string password, RoleType roleType, string cmtCode = null)
        {
            Result<UserView> result = new Result<UserView>();
            try
            {
                var model = DataOperateBasic<Base_User>.Get().Single(i => i.UserAcct == userName);
                //去注册站点
                if (model == null)
                {
                    throw new Exception("-1");
                }
                if (model != null) //用户名存在
                {
                    //登录日志
                    WriteLoginLog("", model);
                    if (model.IsLock)
                    {
                        throw new Exception("账户已被锁定");
                    }
                    if (model.PassWord != password)
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

                    UserView mv = new UserView();
                    var company = DataOperateBasic<Base_Company>.Get().GetModel(model.CompanyId);
                    //身份角色验证
                    if (company.Type.Contains(roleType.ToString()))
                    {
                        mv.CompanyId = model.CompanyId;
                        mv.CompanyName = company.Name;
                        mv.CompanyNo = company.Code;
                        mv.CompanyType = company.Type;
                        var rights = LoadRightList(roleType, model.Id);

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
                        mv.RoleType = roleType;
                        if (mv.RoleType != RoleType.Admin)
                        {
                            throw new Exception("你暂时无权登录本系统！");
                        }

                        mv.UserId = model.Id;
                        mv.UserName = model.UserAcct;
                        mv.Email = model.Email;
                        mv.Phone = model.Phone;
                        mv.RealName = model.UserName;
                        //修改最后登录时间
                        model.LastLoginTime = DateTime.Now;
                        model.LockNum = 0;
                        DataOperateBasic<Base_User>.Get().Update(model);
                        result.Data = mv;
                        result.Flag = EResultFlag.Success;
                    }
                    else
                    {
                        throw new Exception("角色验证不符");
                    }

                }
                else
                {
                    throw new Exception("用户名不存在");
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "Login");
            }
            return result;
        }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddUser(Base_User model, List<Base_Files> fileList = null)
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
                //user = DataOperateBasic<Base_User>.Get().Count(i => i.Email == model.Email) > 0;
                //if (user)
                //{
                //    throw new Exception("该邮箱已经存在");
                //}
                var rows = DataOperateBasic<Base_User>.Get().Add(model);
                //新增附件
                AddFilesByTable(model, fileList);

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
        /// 获取指定企业下用户数量
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public Result<int> GetUserCountByCompanyId(long companyId)
        {
            Result<int> result = new Result<int>();
            try
            {
                var count = DataOperateBasic<Base_User>.Get().Count(i => i.CompanyId == companyId);

                result.Data = count;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetUserCountByCompanyId");
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
        public Result<int> GetRolesByUserIds(List<long> userId)
        {
            Result<int> result = new Result<int>();
            try
            {
                var list = DataOperateBasic<Base_UserRole>.Get().GetList(i => userId.Contains(i.Id));
                var rows = DataOperateBasic<Base_User>.Get().UpdateRange(list);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                #region 写日志
                WriteLog(AdminModule.UserManager.GetText(), SystemRight.Delete.GetText(), "批量设置用户角色:" + rows);
                #endregion

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetRolesByUserIds");
            }
            return result;
        }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateUser(Base_User model, List<Base_Files> fileList = null)
        {
            var oldModel = DataOperateBasic<Base_User>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);

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
                //user = DataOperateBasic<Base_User>.Get().Count(i => i.Email == model.Email && i.Id != model.Id) > 0;
                //if (user)
                //{
                //    throw new Exception("该邮箱已经存在");
                //}
                var rows = DataOperateBasic<Base_User>.Get().Update(model);
                //新增附件
                AddFilesByTable(model, fileList);

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
        ///// <summary>
        ///// 用户绑定Ca
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <param name="caSN"></param>
        ///// <returns></returns>
        //public Result<int> UpdateUserCA(long userId, string caSN)
        //{
        //    Result<int> result = new Result<int>();
        //    try
        //    {
        //        var model = DataOperateBasic<Base_User>.Get().GetModel(userId);
        //        var caCount = DataOperateBasic<Base_User>.Get().Count(i => i.CASN == caSN);
        //        if (caCount > 0)
        //        {
        //            throw new Exception("当前USB Key已绑定过其他用户！");
        //        }
        //        model.CASN = caSN;
        //        var rows = DataOperateBasic<Base_User>.Get().Update(model);

        //        result.Data = rows;
        //        result.Flag = EResultFlag.Success;
        //        WriteLog(AdminModule.UserManager.GetText(), SystemRight.Modify.GetText(), "用户绑定CA:" + model.Id);
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Data = -1;
        //        result.Flag = EResultFlag.Failure;
        //        result.Exception = new ExceptionEx(ex, "UpdateUser");
        //    }
        //    return result;
        //}
        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="passowrd">密文密码</param>
        /// <returns></returns>
        public Result<int> UpdateUserPassword(long id, string passowrd)
        {

            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBasic<Base_User>.Get().GetModel(id);

                //新增历史密码
                Base_HistoryPassword his = new Base_HistoryPassword();
                his.Num = "";
                his.OldPassword = model.PassWord;
                his.Password = model.PassWord;
                his.UserId = id;
                //his = base.SetCurrentUser(his);
                DataOperateBasic<Base_HistoryPassword>.Get().Add(his);
                //修改密码
                model.PassWord = passowrd;
                var rows = DataOperateBasic<Base_User>.Get().Update(model);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.UserManager.GetText(), SystemRight.Modify.GetText(), "用户修改密码:" + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateUserPassword");
            }
            return result;
        }
        /// <summary>
        /// 检查用户是否存在
        /// </summary>
        /// <param name="userCode">用户名/邮箱/电话</param>
        /// <returns></returns>
        public Result<bool> IsExitsUser(string userCode)
        {

            Result<bool> result = new Result<bool>();
            try
            {
                var model = DataOperateBasic<Base_User>.Get().Single(i => i.UserAcct == userCode || i.Phone == userCode || i.Email == userCode);
                result.Data = model != null;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = false;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "IsExitsUser");
            }
            return result;
        }
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userCode">用户名/邮箱/电话</param>
        /// <returns></returns>
        public Result<Base_User> GetUserByCode(string userCode)
        {

            Result<Base_User> result = new Result<Base_User>();
            try
            {
                var model = DataOperateBasic<Base_User>.Get().Single(i => i.UserAcct == userCode || i.Phone == userCode || i.Email == userCode);
                result.Data = model;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetUserByCode");
            }
            return result;
        }
        /// <summary>
        /// 锁定/解锁用户
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public Result<int> AuditUser(long userId)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBasic<Base_User>.Get().GetModel(userId);
                model.IsLock = !model.IsLock;
                model.LockNum = 0;
                var rows = DataOperateBasic<Base_User>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                var user = ApplicationContext.Current.UserID;

                WriteLog(AdminModule.UserManager.GetText(), SystemRight.Lock.GetText(), "用户锁定/解锁:" + model.Id);

                WriteStateLog(model, (!model.IsLock).ToString(), (model.IsLock).ToString());
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
        /// 根据条件获取用户列表
        /// </summary>
        /// <param name="qc"></param>
        /// <param name="roleType">角色</param>
        /// <returns></returns>
        public Result<List<Base_User>> GetUserList(QueryCondition qc, RoleType? roleType = null)
        {
            qc = AddDefault(qc);
            if (roleType.HasValue)
            {
                var companys = DataOperateBasic<Base_Company>.Get().GetList(i => i.Type.Contains(roleType.Value.ToString())).ToList().Select(i => i.Id).JoinToString(",");
                if (companys != "")
                {
                    ConditionExpression ce = new ConditionExpression();
                    ce.ExpName = "CompanyId";
                    ce.ExpValue = companys;
                    ce.ExpOperater = eConditionOperator.In;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
            }
            Result<List<Base_User>> result = new Result<List<Base_User>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Base_User>(context, qc);

            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetUserList");
            }
            return result;
        }
        /// <summary>
        /// 根据企业获取用户列表
        /// </summary>
        /// <param name="companyId">企业Id</param>
        /// <param name="qc">条件</param>
        /// <returns></returns>
        public Result<List<Base_User>> GetUserListByCompanyId(long companyId, QueryCondition qc)
        {
            qc = AddDefault(qc);
            ConditionExpression ce = new ConditionExpression();
            ce.ExpName = "CompanyId";
            ce.ExpValue = companyId;
            ce.ExpOperater = eConditionOperator.Equal;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);

            Result<List<Base_User>> result = new Result<List<Base_User>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Base_User>(context, qc);

            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetUserListByCompanyId");
            }
            return result;
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
        /// 新增权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddRight(Base_Right model)
        {
            model = base.SetCurrentUser(model);
            Result<int> result = new Result<int>();
            try
            {
                model.ParentCode = model.Code;
                model.ParentName = model.Name;
                if (model.ParentId > 0)
                {
                    var parent = DataOperateBasic<Base_Right>.Get().GetModel(model.ParentId);
                    model.ParentCode = parent.Code;
                    model.ParentName = parent.Name;
                }
                var rows = DataOperateBasic<Base_Right>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(model.Belong.ToEnumReq<RoleType>().GetText() + "权限", SystemRight.Add.GetText(), "新增权限:" + model.Id);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddRight");
            }
            return result;
        }
        /// <summary>
        /// 批量新增权限
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public Result<int> AddRightRange(List<Base_Right> models)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBasic<Base_Right>.Get().AddRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(models.FirstOrDefault().Belong.ToEnumReq<RoleType>().GetText() + "权限", SystemRight.Add.GetText(), "新增权限:" + rows);


            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddRight");
            }
            return result;
        }
        /// <summary>
        /// 查询尚未添加的权限
        /// </summary>
        /// <param name="rType"></param>
        /// <param name="pId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Result<Dictionary<string, string>> GetRightUNSelect(RoleType rType, long pId, out string functionType)
        {
            Dictionary<string, string> dicList = new Dictionary<string, string>();
            IEnumerable<string> allRight = null;
            var type = "";
            if (pId == 0)//添加的是模块
            {
                switch (rType)
                {
                    case RoleType.Admin: //后端
                        dicList = Enum<AdminNav>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText());
                        break;
                    case RoleType.Owner: //前端
                        dicList = Enum<WebNav>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText());
                        break;
                }
            }
            else
            {
                var pm = DataOperateBasic<Base_Right>.Get().GetModel(pId);
                switch (rType)
                {
                    case RoleType.Admin: //后台

                        FunctionType pType = pm.RightType.ToEnumReq<FunctionType>();//该父类型所属的功能类型

                        switch (pType)
                        {
                            case FunctionType.Nav://加载所有分类Name
                                type = FunctionType.Category.ToString();
                                dicList = Enum<AdminCategory>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText());
                                break;
                            case FunctionType.Category://加载所有模块Name
                                type = FunctionType.Module.ToString();
                                dicList = Enum<AdminModule>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText());
                                break;
                            case FunctionType.Module://加载所有操作Name
                                type = FunctionType.Action.ToString();
                                dicList = Enum<SystemRight>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText());
                                break;
                            case FunctionType.Action://无此操作
                                break;
                            default:
                                break;
                        }
                        break;
                    case RoleType.Owner: //前端

                        WebFunctionType webType = pm.RightType.ToEnumReq<WebFunctionType>();//该父类型所属的功能类型

                        switch (webType)
                        {
                            case WebFunctionType.Nav://加载所有分类Name
                                type = WebFunctionType.Category.ToString();
                                dicList = Enum<WebCategory>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText());
                                break;
                            case WebFunctionType.Category://加载所有模块Name
                                type = WebFunctionType.Module.ToString();
                                dicList = Enum<WebModule>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText());
                                break;
                            case WebFunctionType.Module://加载所有操作Name
                                type = WebFunctionType.Action.ToString();
                                dicList = Enum<SystemRight>.AsEnumerable().ToDictionary(i => i.ToString(), j => j.GetText());
                                break;
                            case WebFunctionType.Action://无此操作
                                break;
                            default:
                                break;
                        }
                        break;
                }
            }
            functionType = type;
            //只返回未添加过的权限
            var allList = DataOperateBasic<Base_Right>.Get().GetList(i => i.Belong == rType.ToString() && i.RightType == type).ToList();
            if (type == FunctionType.Action.ToString())
            {
                allList = allList.Where(i => i.ParentId == pId).ToList();
            }
            allRight = allList.Select(i => i.Code);
            dicList = dicList.Where(i => !allRight.Contains(i.Key)).ToDictionary(i => i.Key, j => j.Value);
            Result<Dictionary<string, string>> result = new Result<Dictionary<string, string>>();
            result.Data = dicList;
            result.Flag = EResultFlag.Success;

            return result;
        }

        /// <summary>
        /// 获取权限详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Base_Right> GetRightModel(long id)
        {
            Result<Base_Right> result = new Result<Base_Right>();
            try
            {
                var model = DataOperateBasic<Base_Right>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetRightModel");
            }
            return result;
        }
        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateRight(Base_Right model)
        {
            var oldModel = DataOperateBasic<Base_Right>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);

            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBasic<Base_Right>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(model.Belong.ToEnumReq<RoleType>().GetText() + "权限", SystemRight.Modify.GetText(), "修改权限:" + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateRight");
            }
            return result;
        }
        /// <summary>
        /// 审核权限
        /// </summary>
        /// <param name="rightId">权限Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditRight(long rightId, int type)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBasic<Base_Right>.Get().GetModel(rightId);
                var rows = DataOperateBasic<Base_Right>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(model.Belong.ToEnumReq<RoleType>().GetText() + "权限", SystemRight.Check.GetText(), "审核权限:" + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AuditRight");
            }
            return result;
        }
        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Right>> GetRightList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Base_Right>> result = new Result<List<Base_Right>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Base_Right>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetRightList");
            }
            return result;
        }
        /// <summary>
        /// 根据用户加载对应权限，有缓存
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="listRight"></param>
        /// <returns></returns>
        public Result<List<Base_Right>> LoadRightList(RoleType roleType, long userId = 0, List<long> listRight = null)
        {
            Result<List<Base_Right>> result = new Result<List<Base_Right>>();
            try
            {

                var list = DataOperateBasic<Base_Right>.Get().GetList(i => i.Belong == roleType.ToString()).OrderBy(i => i.Sort).ToList();
                if (userId != 0)
                {
                    if (listRight == null)//当前用户的所有权限
                    {
                        //获取用户角色
                        var roles = DataOperateBasic<Base_UserRole>.Get().GetList(i => i.UserId == userId).ToList();
                        //获取所有角色权限
                        var roleIds = roles.Select(i => i.RoleId).ToList();
                        listRight = DataOperateBasic<Base_RoleRight>.Get().GetList(i => roleIds.Contains(i.RoleId)).Select(i => i.RightId).ToList();
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
            return result;
        }
        /// <summary>
        /// 根据父级权限获取子权限
        /// </summary>
        /// <param name="roleType"></param>
        /// <param name="parentId"></param>
        /// <param name="isIncludeSelf">是否包含自身</param>
        /// <param name="isChildAll">是否包含所有自权限</param>
        /// <returns></returns>
        public Result<List<Base_Right>> GetRightListByRole(RoleType roleType, long parentId, bool isIncludeSelf = false, bool isChildAll = false)
        {
            Result<List<Base_Right>> result = new Result<List<Base_Right>>();
            try
            {
                var queryList = DataOperateBasic<Base_Right>.Get().GetList(i => i.Belong == roleType.ToString());
                List<Base_Right> list = new List<Base_Right>();
                //包含所有子权限
                if (isChildAll)
                {
                    var belongAll = queryList.ToList();
                    var childAll = getAllChildRightList(parentId, belongAll);
                    list = childAll;
                }
                else
                {
                    queryList = queryList.Where(i => i.ParentId == parentId);
                    list = queryList.ToList();
                }

                //包含自身
                if (isIncludeSelf)
                {
                    var self = queryList.FirstOrDefault(i => i.Id == parentId);
                    list.Insert(0, self);
                }

                result.Data = list;
                result.AllRowsCount = list.Count();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMenuListByRole");
            }
            return result;
        }
        /// <summary>
        /// 迭代获取某父权限的所有子权限
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="allList"></param>
        /// <returns></returns>
        private List<Base_Right> getAllChildRightList(long parentId, List<Base_Right> allList)
        {
            List<Base_Right> list = new List<Base_Right>();
            var childList = allList.Where(i => i.ParentId == parentId).ToList();
            //有子权限
            if (childList != null && childList.Any())
            {
                list.AddRange(childList);
                foreach (var item in childList)
                {
                    var iteratorList = getAllChildRightList(item.Id, allList);
                    list.AddRange(iteratorList);
                }
            }

            return list;
        }
        /// <summary>
        /// 根据人员id获取该人所属的所有角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Result<List<Base_UserRole>> GetRolesByUserId(long userId)
        {
            Result<List<Base_UserRole>> result = new Result<List<Base_UserRole>>();
            try
            {
                var list = DataOperateBasic<Base_UserRole>.Get().GetList(i => i.UserId == userId).ToList();
                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetRolesByUserId");
            }
            return result;
        }
        /// <summary>
        /// 根据角色id获取角色权限
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public Result<List<Base_RoleRight>> GetRightByRoleIds(List<long> roleIds)
        {
            Result<List<Base_RoleRight>> result = new Result<List<Base_RoleRight>>();
            try
            {
                var list = DataOperateBasic<Base_RoleRight>.Get().GetList(i => roleIds.Contains(i.RoleId)).ToList();
                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetRightByRoleId");
            }
            return result;
        }
        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="rightId"></param>
        /// <returns></returns>
        public Result<int> DeleteRight(long rightId)
        {
            Result<int> result = new Result<int>();
            try
            {

                var models = DataOperateBasic<Base_Right>.Get().GetList(i => i.ParentId == rightId).ToList();
                var rows = DataOperateBasic<Base_Right>.Get().DeleteRange(models);

                var model = DataOperateBasic<Base_Right>.Get().GetModel(rightId);
                rows = DataOperateBasic<Base_Right>.Get().Delete(model);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(model.Belong.ToEnumReq<RoleType>().GetText() + "权限", SystemRight.Delete.GetText(), "删除权限:" + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteRight");
            }
            return result;
        }
        /// <summary>
        /// 批量删除权限
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteRightbyIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                //删除对应权限和子权限
                var models = DataOperateBasic<Base_Right>.Get().GetList(i => ids.Contains(i.Id) || ids.Contains(i.ParentId)).ToList();

                var rows = DataOperateBasic<Base_Right>.Get().DeleteRange(models);

                result.Data = rows;
                result.Flag = EResultFlag.Success;

                WriteLog(models.FirstOrDefault().Belong.ToEnumReq<RoleType>().GetText() + "权限", SystemRight.Delete.GetText(), "批量删除权限:" + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteRight");
            }
            return result;
        }
        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddRole(Base_Role model)
        {
            model = base.SetCurrentUser(model);
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBasic<Base_Role>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                WriteLog(model.Belong.ToEnum<RoleType>().GetText() + "角色", SystemRight.Add.GetText(), "新增角色: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddRole");
            }
            return result;
        }
        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateRole(Base_Role model)
        {
            var oldModel = DataOperateBasic<Base_Role>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);

            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBasic<Base_Role>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                WriteLog(model.Belong.ToEnum<RoleType>().GetText() + "角色", SystemRight.Modify.GetText(), "修改角色:" + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateRole");
            }
            return result;
        }
        /// <summary>
        /// 审核角色
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditRole(long roleId, int type)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBasic<Base_Role>.Get().GetModel(roleId);
                var rows = DataOperateBasic<Base_Role>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                WriteLog(model.Belong.ToEnum<RoleType>().GetText() + "角色", SystemRight.Check.GetText(), "审核角色:" + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AuditRole");
            }
            return result;
        }
        /// <summary>
        /// 获取角色详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Base_Role> GetRoleModel(long id)
        {
            Result<Base_Role> result = new Result<Base_Role>();
            try
            {
                var model = DataOperateBasic<Base_Role>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetRoleModel");
            }
            return result;
        }
        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Role>> GetRoleList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Base_Role>> result = new Result<List<Base_Role>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Base_Role>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetRoleList");
            }
            return result;
        }

        /// <summary>
        /// 根据身份获取角色列表
        /// </summary>
        /// <param name="roleType">角色类型</param>
        /// <returns></returns>
        public Result<List<Base_Role>> GetRoleListByBelong(RoleType roleType)
        {

            Result<List<Base_Role>> result = new Result<List<Base_Role>>();
            try
            {

                var list = DataOperateBasic<Base_Role>.Get().GetList(i => i.Belong == roleType.ToString()).ToList();
                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetRoleListByBelong");
            }
            return result;
        }
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteRoleByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBasic<Base_Role>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var roleIds = models.Select(i => i.Id);
                var userRole = DataOperateBasic<Base_UserRole>.Get().Count(i => roleIds.Contains(i.RoleId)) > 0;
                if (userRole)
                {
                    throw new Exception("要删除的角色中存在正在使用的角色，删除失败");
                }
                var rows = DataOperateBasic<Base_Role>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                #region 写日志
                WriteLog(models.FirstOrDefault().Belong.ToEnum<RoleType>().GetText() + "角色", SystemRight.Delete.GetText(), "批量删除角色:" + rows);
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
        /// 设置角色权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="rightIds"></param>
        /// <returns></returns>
        public Result<int> SetRoleRight(long roleId, List<long> rightIds)
        {
            Result<int> result = new Result<int>();

            try
            {
                rightIds.RemoveAll(i => i == 0);
                rightIds = rightIds.Distinct().ToList();
                Base_Role role = DataOperateBasic<Base_Role>.Get().GetModel(roleId);
                //List<Base_Right> rights = DataOperateBasic<Base_Right>.Get().GetList(i => rightIds.Contains(i.Id)).ToList();

                //删除原有角色权限
                var deleteModels = GetRightByRoleIds(new List<long>() { roleId }).Data;
                int rows = 0;
                if (deleteModels != null)
                {
                    rows = DataOperateBasic<Base_RoleRight>.Get().DeleteRange<Base_RoleRight>(deleteModels);
                }

                //新增新的角色权限
                List<Base_RoleRight> list = new List<Base_RoleRight>();
                foreach (var item in rightIds)
                {
                    Base_RoleRight rr = new Base_RoleRight();
                    rr.RightId = item;
                    rr.RoleId = roleId;
                    rr = base.SetCurrentUser(rr);
                    list.Add(rr);
                }
                rows = DataOperateBasic<Base_RoleRight>.Get().AddRange(list);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(role.Belong.ToEnumReq<RoleType>().GetText() + "角色", SystemRight.SetRight.GetText(), "设置角色权限:" + role.RoleName);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "SetRoleRight");
            }
            return result;
        }
        /// <summary>
        /// 设置用户角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="rightIds"></param>
        /// <returns></returns>
        public Result<int> SetUserRole(long userId, List<long> roleIds, RoleType roleType)
        {
            Result<int> result = new Result<int>();

            try
            {
                Base_User user = DataOperateBasic<Base_User>.Get().GetModel(userId);
                //删除原有用户权限
                var deleteModels = GetRolesByUserId(userId).Data;
                int rows = 0;
                if (deleteModels != null)
                {
                    rows = DataOperateBasic<Base_UserRole>.Get().DeleteRange<Base_UserRole>(deleteModels);
                }

                //新增新用户角色
                List<Base_UserRole> list = new List<Base_UserRole>();
                foreach (var item in roleIds)
                {
                    Base_UserRole ur = new Base_UserRole();
                    ur.RoleId = item;
                    ur.UserId = userId;
                    ur.Belong = roleType.ToString();
                    ur = base.SetCurrentUser(ur);
                    list.Add(ur);
                }
                rows = DataOperateBasic<Base_UserRole>.Get().AddRange(list);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(roleType.GetText() + "角色", SystemRight.SetRight.GetText(), "设置用户角色:" + user.UserAcct);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "SetUserRole");
            }
            return result;
        }

        /// <summary>
        /// 添加企业
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddCompany(Base_Company model, List<Base_Files> fileList = null)
        {
            model = base.SetCurrentUser(model);
            Result<int> result = new Result<int>();
            try
            {
                bool company = DataOperateBasic<Base_Company>.Get().Count(i => i.Name == model.Name) > 0;
                if (company)
                {
                    throw new Exception("该企业名称已经存在");
                }
                var rows = DataOperateBasic<Base_Company>.Get().Add(model);
                //新增附件
                AddFilesByTable(model, fileList);

                result.Data = rows;
                result.Flag = EResultFlag.Success;

                WriteLog(model.Type + "组织结构添加", SystemRight.Add.GetText(), "添加企业:" + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddCompany");
            }
            return result;
        }
        /// <summary>
        /// 新增加油站信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddOilStation(Epm_OilStation model)
        {
            model = base.SetCurrentUser(model);
            Result<int> result = new Result<int>();
            try
            {
                bool OilStation = DataOperateBusiness<Epm_OilStation>.Get().Count(i => i.Name == model.Name) > 0;
                if (OilStation)
                {
                    throw new Exception("该加油站已经存在");
                }
                var rows = DataOperateBusiness<Epm_OilStation>.Get().Add(model);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(model.Name + "加油站添加", SystemRight.Add.GetText(), "添加油站:" + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddOilStation");
            }
            return result;
        }

        /// <summary>
        /// 修改企业
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateCompany(Base_Company model, List<Base_Files> fileList = null)
        {


            Result<int> result = new Result<int>();
            try
            {
                bool company = DataOperateBasic<Base_Company>.Get().Count(i => i.Name == model.Name && i.Id != model.Id) > 0;
                if (company)
                {
                    throw new Exception("该企业名称已经存在");
                }
                var oldModel = DataOperateBasic<Base_Company>.Get().GetModel(model.Id);
                model = FiterUpdate(oldModel, model);
                if (fileList != null)
                {
                    //删除之前的附件
                    DeleteFilesByTable(model.GetType().Name, new List<long>() { model.Id });
                    //新增附件
                    AddFilesByTable(model, fileList);
                }
                var rows = DataOperateBasic<Base_Company>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                WriteLog(model.Type + "组织结构修改", SystemRight.Modify.GetText(), "修改企业:" + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateCompany");
            }
            return result;
        }
        /// <summary>
        /// 修改加油站
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList"></param>
        /// <returns></returns>
        public Result<int> UpdateOilStation(Epm_OilStation model)
        {
            Result<int> result = new Result<int>();
            try
            {
                bool OilStation = DataOperateBusiness<Epm_OilStation>.Get().Count(i => i.Name == model.Name && i.Id != model.Id) > 0;
                if (OilStation)
                {
                    throw new Exception("该加油站名称已经存在");
                }
                var oldModel = DataOperateBusiness<Epm_OilStation>.Get().GetModel(model.Id);
                model = FiterUpdate(oldModel, model);

                var rows = DataOperateBusiness<Epm_OilStation>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                WriteLog(model.Name + "油站修改", SystemRight.Modify.GetText(), "修改油站:" + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateCompany");
            }
            return result;
        }
        public Result<int> UpdateBaseVideoManage(Base_VideoManage model)
        {
            Result<int> result = new Result<int>();
            try
            {
                bool OilStation = DataOperateBasic<Base_VideoManage>.Get().Count(i => i.CameraName == model.CameraName && i.Id != model.Id) > 0;
                if (OilStation)
                {
                    throw new Exception("该摄像机名称已经存在");
                }
                var oldModel = DataOperateBasic<Base_VideoManage>.Get().GetModel(model.Id);
                model = FiterUpdate(oldModel, model);

                var rows = DataOperateBasic<Base_VideoManage>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                //WriteLog(model.CameraName + "摄像机修改", SystemRight.Modify.GetText(), "修改摄像机信息:" + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateBaseVideoManage");
            }
            return result;
        }
        /// <summary>
        /// 审核企业
        /// </summary>
        /// <param name="companyId">企业Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditCompany(long companyId, int type)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBasic<Base_Company>.Get().GetModel(companyId);
                var rows = DataOperateBasic<Base_Company>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                WriteLog(model.Type + "组织结构修改", SystemRight.Check.GetText(), "审核企业:" + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AuditCompany");
            }
            return result;
        }
        /// <summary>
        /// 获取企业列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Company>> GetCompanyList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            try
            {
                using (BasicDataContext baseDataContext = new BasicDataContext())
                {
                    result = DataOperate.QueryListSimple<Base_Company>(baseDataContext, qc);
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCompanyList");
            }
            return result;
        }
        /// <summary>
        /// 获取企业列表
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<List<Base_Company>> GetCompanyListByIds(List<long> ids)
        {
            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            try
            {
                var res = DataOperateBasic<Base_Company>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                result.Data = res;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCompanyListByIds");
            }
            return result;
        }
        /// <summary>
        /// 根据父级企业获取子企业
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="isIncludeSelf">是否包含自身</param>
        /// <param name="isChildAll">是否包含所有子公司</param>
        /// <returns></returns>
        public Result<List<Base_Company>> GetCompanyListByRole(long parentId, bool isIncludeSelf = false, bool isChildAll = false)
        {
            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            try
            {
                var queryList = DataOperateBasic<Base_Company>.Get().GetList();
                List<Base_Company> list = new List<Base_Company>();
                //包含所有子公司
                if (isChildAll)
                {
                    var belongAll = queryList.ToList();
                    var childAll = getAllChildEntList(parentId, belongAll);
                    list = childAll;
                }
                else
                {
                    queryList = queryList.Where(i => i.PId == parentId);
                    list = queryList.ToList();
                }

                //包含自身
                if (isIncludeSelf)
                {
                    var self = queryList.FirstOrDefault(i => i.Id == parentId);
                    list.Insert(0, self);
                }
                result.Data = list;
                result.AllRowsCount = list.Count();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCompanyListByRole");
            }
            return result;
        }
        /// <summary>
        /// 迭代获取某父公司的所有子公司
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="allList"></param>
        /// <returns></returns>
        private List<Base_Company> getAllChildEntList(long parentId, List<Base_Company> allList)
        {
            List<Base_Company> list = new List<Base_Company>();
            var childList = allList.Where(i => i.PId == parentId).ToList();
            //有子公司
            if (childList != null && childList.Any())
            {
                list.AddRange(childList);
                foreach (var item in childList)
                {
                    var iteratorList = getAllChildEntList(item.Id, allList);
                    list.AddRange(iteratorList);
                }
            }

            return list;
        }
        /// <summary>
        /// 获取企业详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Base_Company> GetCompanyModel(long id)
        {
            Result<Base_Company> result = new Result<Base_Company>();
            try
            {
                result.Data = DataOperateBasic<Base_Company>.Get().GetModel(id);
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCompanyModel");
            }
            return result;
        }
        /// <summary>
        /// 获取加油站详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_OilStation> GetOilStation(long id)
        {
            Result<Epm_OilStation> result = new Result<Epm_OilStation>();
            try
            {
                result.Data = DataOperateBusiness<Epm_OilStation>.Get().GetModel(id);
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetOilStation");
            }
            return result;
        }
        /// <summary>
        /// 获取摄像机设备信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Base_VideoManage> GetBaseVideoManages(long id)
        {
            Result<Base_VideoManage> result = new Result<Base_VideoManage>();
            try
            {
                result.Data = DataOperateBasic<Base_VideoManage>.Get().GetModel(id);
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetBaseVideoManages");
            }
            return result;
        }
        /// <summary>
        /// 获取管理员或者专家企业
        /// </summary>
        /// <param name="roleType"></param>
        /// <returns></returns>
        public Result<Base_Company> GetCompanyByRoleType(RoleType roleType)
        {
            Result<Base_Company> result = new Result<Base_Company>();
            try
            {
                result.Data = DataOperateBasic<Base_Company>.Get().Single(i => i.Type.Contains(roleType.ToString()));
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCompanyByRoleType");
            }
            return result;
        }
        /// <summary>
        /// 删除企业
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteCompanyByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBasic<Base_Company>.Get().GetList(i => ids.Contains(i.Id)).ToList();

                var rows = DataOperateBasic<Base_Company>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                #region 写日志
                WriteLog("组织机构删除", SystemRight.Delete.GetText(), "批量删除企业:" + rows);
                #endregion

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteCompanyByIds");
            }
            return result;
        }
        /// <summary>
        /// 删除加油站 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteOilStation(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_OilStation>.Get().GetList(i => ids.Contains(i.Id)).ToList();

                var rows = DataOperateBusiness<Epm_OilStation>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                #region 写日志
                WriteLog("加油站删除", SystemRight.Delete.GetText(), "批量删除加油站:" + rows);
                #endregion

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteOilStation");
            }
            return result;
        }

        /// <summary>
        /// 添加部门
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddDep(Base_Dep model)
        {
            model = base.SetCurrentUser(model);
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBasic<Base_Dep>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                WriteLog(AdminModule.Dep.GetText(), SystemRight.Add.GetText(), "添加部门:" + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddDep");
            }
            return result;
        }
        /// <summary>
        /// 修改部门
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateDep(Base_Dep model)
        {
            var oldModel = DataOperateBasic<Base_Dep>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);

            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBasic<Base_Dep>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                WriteLog(AdminModule.Dep.GetText(), SystemRight.Modify.GetText(), "修改企业:" + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateDep");
            }
            return result;
        }
        /// <summary>
        /// 审核部门
        /// </summary>
        /// <param name="companyId">部门Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditDep(long depId, int type)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBasic<Base_Dep>.Get().GetModel(depId);
                if (type == 1)
                {
                    model.IsEnable = !model.IsEnable;
                }
                else if (type == 2)
                {
                    model.IsConfirm = !model.IsConfirm;
                }
                var rows = DataOperateBasic<Base_Dep>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                WriteLog(AdminModule.Dep.GetText(), SystemRight.Check.GetText(), "审核企业:" + model.Id);
                if (type == 1)
                {
                    WriteStateLog(model, (!model.IsEnable).ToString(), (model.IsEnable).ToString());
                }
                else if (type == 2)
                {
                    WriteStateLog(model, (!model.IsConfirm).ToString(), (model.IsConfirm).ToString());
                }

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AuditDep");
            }
            return result;
        }
        /// <summary>
        /// 获取部门详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Base_Dep> GeDepModel(long id)
        {
            Result<Base_Dep> result = new Result<Base_Dep>();
            try
            {
                result.Data = DataOperateBasic<Base_Dep>.Get().GetModel(id);
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GeDepModel");
            }
            return result;
        }
        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Dep>> GeDepList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Base_Dep>> result = new Result<List<Base_Dep>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Base_Dep>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GeDepList");
            }
            return result;
        }
        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Dep>> GeDepListByCompanyId(long companyId)
        {

            Result<List<Base_Dep>> result = new Result<List<Base_Dep>>();
            List<Base_Dep> list = new List<Base_Dep>();
            try
            {
                list = DataOperateBasic<Base_Dep>.Get().GetList(i => i.CompanyId == companyId && i.IsEnable && i.IsConfirm).ToList();
                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GeDepList");
            }
            return result;
        }
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteDepByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBasic<Base_Dep>.Get().GetList(i => ids.Contains(i.Id)).ToList();

                var rows = DataOperateBasic<Base_Dep>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                #region 写日志
                WriteLog(AdminModule.Dep.GetText(), SystemRight.Delete.GetText(), "批量删除部门:" + rows);
                #endregion

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteDepByIds");
            }
            return result;
        }
        /// <summary>
        /// 根据code生成token
        /// </summary>
        /// <param name="code">value</param>
        /// <param name="seconds">失效秒数</param>
        /// <returns></returns>
        public Result<Dictionary<string, string>> CreateToken(string code, int seconds = 0)
        {
            Result<Dictionary<string, string>> result = new Result<Dictionary<string, string>>();
            try
            {
                string guid = Guid.NewGuid().ToString();
                string token = DesTool.DesEncrypt(guid);
                var invalidTime = DateTime.Now.AddSeconds(seconds);

                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                dictionary.Add(token, code);
                result.Data = dictionary;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "CreateToken");
            }
            return result;
        }
        /// <summary>
        /// 生成随机token
        /// </summary>
        /// <param name="code">value</param>
        /// <param name="seconds">失效秒数</param>
        /// <returns></returns>
        public Result<string> CreateRandomToken(int seconds)
        {
            Result<string> result = new Result<string>();
            try
            {
                string guid = Guid.NewGuid().ToString();
                string token = DesTool.DesEncrypt(guid);
                var invalidTime = DateTime.Now.AddSeconds(seconds);
                result.Data = token;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "CreateRandomToken");
            }
            return result;
        }
        /// <summary>
        /// 检查token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Result<string> CheckToken(string token)
        {
            Result<string> result = new Result<string>();
            try
            {
                string code = "";
                result.Data = code;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "CheckToken");
            }
            return result;
        }
        /// <summary>
        /// 删除token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Result<string> DeleteToken(string token)
        {
            Result<string> result = new Result<string>();
            try
            {
                result.Data = "";
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteToken");
            }
            return result;
        }

        public Result<List<UserListView>> GetUserCompanyList(QueryCondition qc)
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
                                select new
                                {
                                    a.Id,
                                    a.UserName,
                                    a.CompanyId,
                                    a.UserCode,
                                    a.Email,
                                    a.Phone,
                                    a.WebChat,
                                    a.QQ,
                                    a.IsLock,
                                    a.UserAcct,
                                    b.Name,
                                    b.Type,
                                    b.Code
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
                                        query = query.Where(p => value.Contains(p.Name));
                                        break;
                                    case "UserName":
                                        query = query.Where(p => p.UserName.Contains(value) || p.UserCode.Contains(value) || p.UserAcct.Contains(value));
                                        break;
                                    case "type":
                                        query = query.Where(p => value.Contains(p.Type));
                                        break;
                                }
                            }
                        }
                    }

                    // query = query.Where(p => p.Type != RoleType.Admin.ToString());

                    int count = query.Count();
                    int skip = (qc.PageInfo.CurrentPageIndex - 1) * qc.PageInfo.PageRowCount;
                    int take = qc.PageInfo.PageRowCount;

                    var list = query.OrderByDescending(p => p.CompanyId).Skip(skip).Take(take).Select(p =>
                        new UserListView
                        {
                            UserId = p.Id,
                            UserCode = p.UserCode,
                            UserName = p.UserName,
                            Email = p.Email,
                            Phone = p.Phone,
                            CompanyId = p.CompanyId,
                            CompanyType = p.Type,
                            CompanyName = p.Name,
                            CompanyNo = p.Code,
                            WeChat = p.WebChat,
                            Qq = p.QQ,
                            RealName = p.UserAcct,
                            IsLock = p.IsLock
                        }).ToList();
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

        #region 视频设备

        /// <summary>
        /// 新增视频设备
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddVideoManage(Base_VideoManage model)
        {
            Result<int> result = new Result<int>();
            try
            {
                if (string.IsNullOrWhiteSpace(model.VerificationCode))
                {
                    throw new Exception("请填写设备序列号！");
                }

                model = base.SetCurrentUser(model);
                bool VideoManage = DataOperateBasic<Base_VideoManage>.Get().Count(i => i.DeviceSequence == model.DeviceSequence) > 0;
                if (VideoManage)
                {
                    throw new Exception("该设备序列号已经存在");
                }

                //#region 获取设备直播地址

                //var addResult = VideoManagerHelper.AddEquipment(model.DeviceSequence, model.VerificationCode);
                //if (addResult.Flag == EResultFlag.Success && addResult.Data)
                //{
                //    model.CameraState = "0";
                //}
                //else
                //{
                //    throw new Exception(addResult.Exception.Decription);
                //}

                //#endregion

                #region 获取设备直播地址

                var addResult = VideoManagerHelper.AddEquipment(model.DeviceSequence, model.VerificationCode);
                if (addResult.Flag == EResultFlag.Success && addResult.Data)
                {
                    var getUrlResult = VideoManagerHelper.GetVideoAddress(model.DeviceSequence);
                    if (getUrlResult.Flag == EResultFlag.Failure)
                    {
                        throw new Exception(getUrlResult.Exception.Decription);
                    }
                    model.UrlAddress = getUrlResult.Data;
                }
                else
                {
                    throw new Exception(addResult.Exception.Decription);
                }

                #endregion

                var rows = DataOperateBasic<Base_VideoManage>.Get().Add(model);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(model.CameraName + "摄像设备添加", SystemRight.Add.GetText(), "添加摄像设备:" + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddOilStation");
            }
            return result;
        }

        /// <summary>
        /// 激活设备
        /// </summary>
        /// <param name="id">设备 ID</param>
        /// <returns></returns>
        public Result<bool> ActivatedVideo(long id)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                var model = DataOperateBasic<Base_VideoManage>.Get().GetModel(id);
                if (model == null)
                {
                    throw new Exception("设备不存在或已删除！");
                }

                #region 获取设备直播地址

                var getUrlResult = VideoManagerHelper.GetVideoAddress(model.DeviceSequence);
                if (getUrlResult.Flag == EResultFlag.Failure)
                {
                    throw new Exception(getUrlResult.Exception.Decription);
                }
                model.UrlAddress = getUrlResult.Data;
                model.CameraState = "1";
                DataOperateBasic<Base_VideoManage>.Get().Update(model);

                #endregion

                result.Data = true;
                result.Flag = EResultFlag.Success;
                WriteLog(model.CameraName + "摄像设备", SystemRight.Add.GetText(), "激活摄像设备:" + model.DeviceSequence);
            }
            catch (Exception ex)
            {
                result.Data = false;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddOilStation");
            }
            return result;
        }

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteBaseVideoManage(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBasic<Base_VideoManage>.Get().GetList(i => ids.Contains(i.Id)).ToList();

                if (models.Any())
                {
                    foreach (var item in models)
                    {
                        item.IsDelete = true;
                        var deleteResult = VideoManagerHelper.DeleteEquipment(item.DeviceSequence);
                        if (deleteResult.Flag == EResultFlag.Success && deleteResult.Data)
                        {
                            DataOperateBasic<Base_VideoManage>.Get().Delete(item);
                        }
                    }
                }

                result.Data = 1;
                result.Flag = EResultFlag.Success;

                #region 写日志

                WriteLog("摄像设备删除", SystemRight.Delete.GetText(), "批量删除摄像设备:" + string.Join(",", models.Select(p => p.DeviceSequence).ToList()));

                #endregion

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteBaseVideoManage");
            }
            return result;
        }
        public Result<Base_VideoManage> GetBaseVideoManageById(long id)
        {
            Result<Base_VideoManage> result = new Result<Base_VideoManage>();
            try
            {
                var rows = DataOperateBasic<Base_VideoManage>.Get().GetModel(id);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetBaseVideoManageById");
            }
            return result;
        }
        public Result<List<Base_VideoManage>> GetVideoManageList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Base_VideoManage>> result = new Result<List<Base_VideoManage>>();
            try
            {
                result = DataOperate.QueryListSimple<Base_VideoManage>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetVideoManageList");
            }
            return result;
        }
        #endregion
    }
}
