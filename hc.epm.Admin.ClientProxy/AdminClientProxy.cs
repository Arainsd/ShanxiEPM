using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using hc.epm.Service.Basic;
using hc.Plat.Common.Global;
using hc.epm.DataModel.Basic;
using hc.epm.Common;
using hc.epm.ViewModel;
using hc.epm.DataModel.Business;

namespace hc.epm.Admin.ClientProxy
{
    public class AdminClientProxy : ClientBase<IBasicService>, IBasicService
    {
        public AdminClientProxy(hc.Plat.Common.Global.ClientProxyExType cpet)
        {

            //传输当前用户的信息；
            ApplicationContext.Current.UserID = cpet.UserID;
            ApplicationContext.Current.WebIP = cpet.IP_WebServer;
            ApplicationContext.Current.ClientIP = cpet.IP_Client;
            if (cpet.CurrentUser != null)
            {
                UserView userView = cpet.CurrentUser as UserView;
                if (userView != null)
                {
                    ApplicationContext.Current.UserName = userView.UserName;
                    ApplicationContext.Current.CompanyId = userView.CompanyId.ToString();
                    ApplicationContext.Current.CompanyName = userView.CompanyName;
                    ApplicationContext.Current.RoleType = userView.RoleType.ToString();
                }

            }
            /*以下密码是用作在应用服务器中使用程序验证密码的作用*/
            string FilePath = AppDomain.CurrentDomain.BaseDirectory + "bin\\";
            string user = "";
            string pass = "";
            string msg = DesTool.LoadCertUserPass(FilePath, out user, out pass);
            if (msg != "")
            {
                throw new Exception(msg);
            }
            ClientCredentials.UserName.UserName = user;
            ClientCredentials.UserName.Password = pass;
            /*OK*/
        }
        /// <summary>
        /// 新增站点设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddConfig(Base_Config model)
        {
            return base.Channel.AddConfig(model);
        }
        /// <summary>
        /// 新增系统配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddSettings(Base_Settings model)
        {
            return base.Channel.AddSettings(model);
        }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddUser(Base_User model, List<Base_Files> fileList)
        {
            return base.Channel.AddUser(model, fileList);
        }
        /// <summary>
        /// 获取网站设置
        /// </summary>
        /// <returns></returns>
        public Result<Base_Config> LoadConfig()
        {
            return base.Channel.LoadConfig();
        }
        /// <summary>
        /// 根据角色加载权限，有缓存
        /// </summary>
        /// <param name="roleType"></param>
        /// <param name="userId"></param>
        /// <param name="listRight"></param>
        /// <returns></returns>
        public Result<List<Base_Right>> LoadRightList(RoleType roleType, long userId = 0, List<long> listRight = null)
        {
            return base.Channel.LoadRightList(roleType, userId, listRight);
        }
        /// <summary>
        /// 根据角色和父级权限加载权限列表
        /// </summary>
        /// <param name="roleType"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public Result<List<Base_Right>> GetRightListByRole(RoleType roleType, long parentId, bool isIncludeSelf = false, bool isChildAll = false)
        {
            return base.Channel.GetRightListByRole(roleType, parentId, isIncludeSelf, isChildAll);
        }
        /// <summary>
        /// 获取系统配置，有缓存
        /// </summary>
        /// <returns></returns>
        public Result<List<Base_Settings>> LoadSettings()
        {
            return base.Channel.LoadSettings();
        }
        /// <summary>
        /// 获取指定配置项，有缓存
        /// </summary>
        /// <param name="setKey"></param>
        /// <returns></returns>
        public Result<Base_Settings> LoadSettingsByKey(Settings setKey)
        {
            return base.Channel.LoadSettingsByKey(setKey);
        }
        /// <summary>
        /// 修改网站设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateConfig(Base_Config model)
        {
            return base.Channel.UpdateConfig(model);
        }
        /// <summary>
        /// 修改配置项
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateSettings(Base_Settings model)
        {
            return base.Channel.UpdateSettings(model);
        }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateUser(Base_User model, List<Base_Files> fileList)
        {
            return base.Channel.UpdateUser(model, fileList);
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_User>> GetUserList(QueryCondition qc, RoleType? roleType = null)
        {
            return base.Channel.GetUserList(qc, roleType);
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Result<UserView> Login(string userName, string password, RoleType roleType, string cmtCode = null)
        {
            return base.Channel.Login(userName, password, roleType, cmtCode);
        }
        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Base_User> GetUserModel(long id)
        {
            return base.Channel.GetUserModel(id);
        }
        /// <summary>
        /// 添加权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddRight(Base_Right model)
        {
            return base.Channel.AddRight(model);
        }
        /// <summary>
        /// 获取权限详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Base_Right> GetRightModel(long id)
        {
            return base.Channel.GetRightModel(id);
        }
        /// <summary>
        /// 修改权限
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateRight(Base_Right model)
        {
            return base.Channel.UpdateRight(model);
        }
        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Right>> GetRightList(QueryCondition qc)
        {
            return base.Channel.GetRightList(qc);
        }
        /// <summary>
        /// 删除权限
        /// </summary>
        /// <param name="rightId"></param>
        /// <returns></returns>
        public Result<int> DeleteRight(long rightId)
        {
            return base.Channel.DeleteRight(rightId);
        }
        /// <summary>
        /// 批量删除权限
        /// </summary>
        /// <param name="rightIds"></param>
        /// <returns></returns>
        public Result<int> DeleteRightbyIds(List<long> rightIds)
        {
            return base.Channel.DeleteRightbyIds(rightIds);
        }
        /// <summary>
        /// 添加类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddType(Base_TypeDictionary model)
        {
            return base.Channel.AddType(model);
        }
        /// <summary>
        /// 修改类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateType(Base_TypeDictionary model)
        {
            return base.Channel.UpdateType(model);
        }
        /// <summary>
        /// 获取类型详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Base_TypeDictionary> GetTypeModel(long id)
        {
            return base.Channel.GetTypeModel(id);
        }
        /// <summary>
        /// 获取类型列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_TypeDictionary>> GetTypeList(QueryCondition qc)
        {
            return base.Channel.GetTypeList(qc);
        }
        /// <summary>
        /// 根据指定pid获取所有子类型
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        public Result<List<Base_TypeDictionary>> GetTypeListByPId(long pId)
        {
            return base.Channel.GetTypeListByPId(pId);
        }
        /// <summary>
        /// 批量删除类型
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteTypeByIds(List<long> ids)
        {
            return base.Channel.DeleteTypeByIds(ids);
        }
        /// <summary>
        /// 获取字典列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Dictionary>> GetDictionaryList(QueryCondition qc)
        {
            return base.Channel.GetDictionaryList(qc);
        }
        /// <summary>
        /// 批量添加权限
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public Result<int> AddRightRange(List<Base_Right> models)
        {
            return base.Channel.AddRightRange(models);
        }
        /// <summary>
        /// 获取字典列表
        /// </summary>
        /// <returns></returns>
        public Result<List<Base_Dictionary>> LoadDictionaryList()
        {
            return base.Channel.LoadDictionaryList();
        }
        /// <summary>
        /// 批量删除用户
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteUserByIds(List<long> ids)
        {
            return base.Channel.DeleteUserByIds(ids);
        }
        /// <summary>
        /// 根据用户id获取角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Result<List<Base_UserRole>> GetRolesByUserId(long userId)
        {
            return base.Channel.GetRolesByUserId(userId);
        }
        public Result<int> GetRolesByUserIds(List<long> userId)
        {
            return base.Channel.GetRolesByUserIds(userId);
        }
        /// <summary>
        /// 根据角色ids获取权限
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        public Result<List<Base_RoleRight>> GetRightByRoleIds(List<long> roleIds)
        {
            return base.Channel.GetRightByRoleIds(roleIds);
        }
        /// <summary>
        /// 获取日志列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Log>> GetLogList(QueryCondition qc)
        {
            return base.Channel.GetLogList(qc);
        }

        /// <summary>
        /// 获取审核日志列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_StatusLog>> GetStatusLogList(QueryCondition qc)
        {
            return base.Channel.GetStatusLogList(qc);
        }
        /// <summary>
        /// 根据类型获取所有类型数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Result<List<Base_TypeDictionary>> GetTypeListByType(DictionaryType type)
        {
            return base.Channel.GetTypeListByType(type);
        }
        /// <summary>
        /// 获取区域列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Region>> GetRegionList(QueryCondition qc)
        {
            return base.Channel.GetRegionList(qc);
        }
        /// <summary>
        /// 加载区域列表，有缓存
        /// </summary>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        public Result<List<Base_Region>> LoadRegionList(string parentCode = "")
        {
            return base.Channel.LoadRegionList(parentCode);
        }
        /// <summary>
        /// 删除配置项
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteSettingsByIds(List<long> ids)
        {
            return base.Channel.DeleteSettingsByIds(ids);
        }
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddRole(Base_Role model)
        {
            return base.Channel.AddRole(model);
        }
        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateRole(Base_Role model)
        {
            return base.Channel.UpdateRole(model);
        }
        /// <summary>
        /// 获取角色详情
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<Base_Role> GetRoleModel(long id)
        {
            return base.Channel.GetRoleModel(id);
        }
        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Role>> GetRoleList(QueryCondition qc)
        {
            return base.Channel.GetRoleList(qc);
        }
        /// <summary>
        /// 批量删除角色
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteRoleByIds(List<long> ids)
        {
            return base.Channel.DeleteRoleByIds(ids);
        }
        /// <summary>
        /// 设置角色
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="rightIds"></param>
        /// <returns></returns>
        public Result<int> SetRoleRight(long roleId, List<long> rightIds)
        {
            return base.Channel.SetRoleRight(roleId, rightIds);
        }
        /// <summary>
        /// 添加企业
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddCompany(Base_Company model, List<Base_Files> fileList = null)
        {
            return base.Channel.AddCompany(model, fileList);
        }
        /// <summary>
        /// 加油站新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddOilStation(Epm_OilStation model)
        {
            return base.Channel.AddOilStation(model);
        }

        /// <summary>
        /// 修改企业
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateCompany(Base_Company model, List<Base_Files> fileList = null)
        {
            return base.Channel.UpdateCompany(model, fileList);
        }
        /// <summary>
        /// 修改加油站信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList"></param>
        /// <returns></returns>
        public Result<int> UpdateOilStation(Epm_OilStation model)
        {
            return base.Channel.UpdateOilStation(model);
        }
        public Result<int> UpdateBaseVideoManage(Base_VideoManage model)
        {
            return base.Channel.UpdateBaseVideoManage(model);
        }
        /// <summary>
        /// 获取企业列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Company>> GetCompanyList(QueryCondition qc)
        {
            return base.Channel.GetCompanyList(qc);
        }
        /// <summary>
        /// 批量删除企业
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteCompanyByIds(List<long> ids)
        {
            return base.Channel.DeleteCompanyByIds(ids);
        }
        /// <summary>
        /// 删除加油站 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteOilStation(List<long> ids)
        {
            return base.Channel.DeleteOilStation(ids);
        }
        /// <summary>
        /// 添加部门
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddDep(Base_Dep model)
        {
            return base.Channel.AddDep(model);
        }
        /// <summary>
        /// 修改部门
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateDep(Base_Dep model)
        {
            return base.Channel.UpdateDep(model);
        }
        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Dep>> GeDepList(QueryCondition qc)
        {
            return base.Channel.GeDepList(qc);
        }
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteDepByIds(List<long> ids)
        {
            return base.Channel.DeleteDepByIds(ids);
        }
        /// <summary>
        /// 获取企业详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Base_Company> GetCompanyModel(long id)
        {
            return base.Channel.GetCompanyModel(id);
        }
        /// <summary>
        /// 获取加油站详情 BaseVideoManage
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_OilStation> GetOilStation(long id)
        {
            return base.Channel.GetOilStation(id);
        }
        public Result<Base_VideoManage> GetBaseVideoManages(long id)
        {
            return base.Channel.GetBaseVideoManages(id);
        }
        /// <summary>
        /// 获取部门详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Base_Dep> GeDepModel(long id)
        {
            return base.Channel.GeDepModel(id);
        }
        /// <summary>
        /// 根据父公司获取子公司
        /// </summary>
        /// <param name="roleType"></param>
        /// <param name="parentId"></param>
        /// <param name="isIncludeSelf"></param>
        /// <param name="isChildAll"></param>
        /// <returns></returns>
        public Result<List<Base_Company>> GetCompanyListByRole(long parentId, bool isIncludeSelf = false, bool isChildAll = false)
        {
            return base.Channel.GetCompanyListByRole(parentId, isIncludeSelf, isChildAll);
        }
        /// <summary>
        /// 查询尚未添加的权限
        /// </summary>
        /// <param name="rType"></param>
        /// <param name="pId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Result<Dictionary<string, string>> GetRightUNSelect(RoleType rType, long pId, out string type)
        {
            return base.Channel.GetRightUNSelect(rType, pId, out type);
        }
        /// <summary>
        /// 根据表名和id获取所有附件
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<List<Base_Files>> GetFilesByTable(string tableName, long id)
        {
            return base.Channel.GetFilesByTable(tableName, id);
        }
        /// <summary>
        /// 添加电子协议
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList">附件列表</param>
        /// <returns></returns>
        public Result<int> AddProtocol(Base_Protocol model, List<Base_Files> fileList)
        {
            return base.Channel.AddProtocol(model, fileList);
        }
        /// <summary>
        /// 修改电子协议
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList">附件</param>
        /// <returns></returns>
        public Result<int> UpdateProtocol(Base_Protocol model, List<Base_Files> fileList)
        {
            return base.Channel.UpdateProtocol(model, fileList);
        }
        /// <summary>
        /// 获取电子协议详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Base_Protocol> GetProtocolModel(long id)
        {
            return base.Channel.GetProtocolModel(id);
        }
        /// <summary>
        /// 获取电子协议列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Protocol>> GetProtocolList(QueryCondition qc)
        {
            return base.Channel.GetProtocolList(qc);
        }
        /// <summary>
        /// 批量删除电子协议
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteProtocoByIds(List<long> ids)
        {
            return base.Channel.DeleteProtocoByIds(ids);
        }
        /// <summary>
        /// 根据公司获取部门
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public Result<List<Base_Dep>> GeDepListByCompanyId(long companyId)
        {
            return base.Channel.GeDepListByCompanyId(companyId);
        }
        /// <summary>
        /// 设置用户角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleIds"></param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        public Result<int> SetUserRole(long userId, List<long> roleIds, RoleType roleType)
        {
            return base.Channel.SetUserRole(userId, roleIds, roleType);
        }
        /// <summary>
        /// 审核权限
        /// </summary>
        /// <param name="rightId">权限Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditRight(long rightId, int type)
        {
            return base.Channel.AuditRight(rightId, type);
        }
        /// <summary>
        /// 审核角色
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditRole(long roleId, int type)
        {
            return base.Channel.AuditRole(roleId, type);
        }
        /// <summary>
        /// 审核企业
        /// </summary>
        /// <param name="companyId">企业Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditCompany(long companyId, int type)
        {
            return base.Channel.AuditCompany(companyId, type);
        }
        /// <summary>
        /// 审核部门
        /// </summary>
        /// <param name="companyId">部门Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditDep(long depId, int type)
        {
            return base.Channel.AuditDep(depId, type);
        }
        /// <summary>
        /// 审核类型数据
        /// </summary>
        /// <param name="typeId">类型数据Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditType(long typeId, int type)
        {
            return base.Channel.AuditType(typeId, type);
        }
        /// <summary>
        /// 审核电子协议
        /// </summary>
        /// <param name="protocolId">电子协议Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditProtocol(long protocolId, int type)
        {
            return base.Channel.AuditProtocol(protocolId, type);
        }
        /// <summary>
        /// 根据身份获取角色列表
        /// </summary>
        /// <param name="roleType">角色类型</param>
        /// <returns></returns>
        public Result<List<Base_Role>> GetRoleListByBelong(RoleType roleType)
        {
            return base.Channel.GetRoleListByBelong(roleType);
        }
        /// <summary>
        /// 获取管理员或者专家企业
        /// </summary>
        /// <param name="roleType"></param>
        /// <returns></returns>
        public Result<Base_Company> GetCompanyByRoleType(RoleType roleType)
        {
            return base.Channel.GetCompanyByRoleType(roleType);
        }
        /// <summary>
        /// 获取区域详情
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Result<Base_Region> GetRegionModel(string code)
        {
            return base.Channel.GetRegionModel(code);
        }
        /// <summary>
        /// 获取所有类型，已启用，已确认
        /// </summary>
        /// <returns></returns>
        public Result<List<Base_TypeDictionary>> GetAllTypeList()
        {
            return base.Channel.GetAllTypeList();
        }
        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="passowrd">密文密码</param>
        /// <returns></returns>
        public Result<int> UpdateUserPassword(long id, string passowrd)
        {
            return base.Channel.UpdateUserPassword(id, passowrd);
        }
        /// <summary>
        /// 检查用户是否存在
        /// </summary>
        /// <param name="userCode">用户名/邮箱/电话</param>
        /// <returns></returns>
        public Result<bool> IsExitsUser(string userCode)
        {
            return base.Channel.IsExitsUser(userCode);
        }
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userCode">用户名/邮箱/电话</param>
        /// <returns></returns>
        public Result<Base_User> GetUserByCode(string userCode)
        {
            return base.Channel.GetUserByCode(userCode);
        }
        /// <summary>
        /// 根据协议类型获取电子协议详情
        /// </summary>
        /// <param name="protocolType"></param>
        /// <returns></returns>
        public Result<Base_Protocol> GetProtocolModelByType(ProtocolType protocolType)
        {
            return base.Channel.GetProtocolModelByType(protocolType);
        }
        /// <summary>
        /// 根据字典类型集合获取字典数据
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public Result<Dictionary<DictionaryType, List<Base_TypeDictionary>>> GetTypeListByTypes(List<DictionaryType> types)
        {
            return base.Channel.GetTypeListByTypes(types);
        }
        /// <summary>
        /// 获取指定企业下用户数量
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public Result<int> GetUserCountByCompanyId(long companyId)
        {
            return base.Channel.GetUserCountByCompanyId(companyId);
        }
        /// <summary>
        /// 锁定/解锁用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Result<int> AuditUser(long userId)
        {
            return base.Channel.AuditUser(userId);
        }

        public Result<List<Base_TypeDictionary>> GetTypeListByNo(string No)
        {
            return base.Channel.GetTypeListByNo(No);
        }

        public Result<List<UserListView>> GetUserCompanyList(QueryCondition qc)
        {
            return base.Channel.GetUserCompanyList(qc);
        }

        #region 人脸信息
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddAIUserFace(EPM_AIUserFace model)
        {
            return base.Channel.AddAIUserFace(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateAIUserFace(EPM_AIUserFace model)
        {
            return base.Channel.UpdateAIUserFace(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteAIUserFaceByIds(List<long> ids)
        {
            return base.Channel.DeleteAIUserFaceByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<EPM_AIUserFace>> GetAIUserFaceList(QueryCondition qc)
        {
            return base.Channel.GetAIUserFaceList(qc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Result<EPM_AIUserFace> GetAIUserFaceByUserId(long userId)
        {
            return base.Channel.GetAIUserFaceByUserId(userId);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<EPM_AIUserFace> GetAIUserFaceModel(long id)
        {
            return base.Channel.GetAIUserFaceModel(id);
        }
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddFaceOperateLog(EPM_FaceOperateLog model)
        {
            return base.Channel.AddFaceOperateLog(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateFaceOperateLog(EPM_FaceOperateLog model)
        {
            return base.Channel.UpdateFaceOperateLog(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteFaceOperateLogByIds(List<long> ids)
        {
            return base.Channel.DeleteFaceOperateLogByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<EPM_FaceOperateLog>> GetFaceOperateLogList(QueryCondition qc)
        {
            return base.Channel.GetFaceOperateLogList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<EPM_FaceOperateLog> GetFaceOperateLogModel(long id)
        {
            return base.Channel.GetFaceOperateLogModel(id);
        }
        #endregion

        #region 考勤信息
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddSignInformation(Epm_SignInformation model)
        {
            return base.Channel.AddSignInformation(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateSignInformation(Epm_SignInformation model)
        {
            return base.Channel.UpdateSignInformation(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteSignInformationByIds(List<long> ids)
        {
            return base.Channel.DeleteSignInformationByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_SignInformation>> GetSignInformationList(QueryCondition qc)
        {
            return base.Channel.GetSignInformationList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_SignInformation> GetSignInformationModel(long id)
        {
            return base.Channel.GetSignInformationModel(id);
        }

        /// <summary>
        /// 根据项目ID和用户ID获取已签到用户信息
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Result<Epm_SignInformation> GetSignBy(long projectId, long userId)
        {
            return base.Channel.GetSignBy(projectId, userId);
        }

        #endregion

        /// <summary>
        /// 人脸注册
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="image"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public Result<int> AddAIUserFaceInfo(long userId, string image, string source)
        {
            return base.Channel.AddAIUserFaceInfo(userId, image, source);
        }

        #region 视频设备

        /// <summary>
        /// 摄像设备新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddVideoManage(Base_VideoManage model)
        {
            return base.Channel.AddVideoManage(model);
        }
        public Result<Base_VideoManage> GetBaseVideoManageById(long id)
        {
            return base.Channel.GetBaseVideoManageById(id);
        }
        public Result<List<Base_VideoManage>> GetVideoManageList(QueryCondition qc)
        {
            return base.Channel.GetVideoManageList(qc);
        }
        /// <summary>
        /// 激活设备
        /// </summary>
        /// <param name="id">设备 ID</param>
        /// <returns></returns>
        public Result<bool> ActivatedVideo(long id)
        {
            return Channel.ActivatedVideo(id);
        }

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="ids">设备ID</param>
        /// <returns></returns>
        public Result<int> DeleteBaseVideoManage(List<long> ids)
        {
            return base.Channel.DeleteBaseVideoManage(ids);
        }

        #endregion
    }
}
