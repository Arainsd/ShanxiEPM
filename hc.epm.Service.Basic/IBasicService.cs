using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.ViewModel;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
namespace hc.epm.Service.Basic
{
    [ServiceContract]
    [ServiceKnownType(typeof(Base_Company))]
    [ServiceKnownType(typeof(Base_Config))]
    [ServiceKnownType(typeof(Base_Dep))]
    [ServiceKnownType(typeof(Base_Dictionary))]
    [ServiceKnownType(typeof(Base_Files))]
    [ServiceKnownType(typeof(Base_HistoryPassword))]
    [ServiceKnownType(typeof(Base_Log))]
    [ServiceKnownType(typeof(Base_Protocol))]
    [ServiceKnownType(typeof(Base_Right))]
    [ServiceKnownType(typeof(Base_Role))]
    [ServiceKnownType(typeof(Base_RoleRight))]
    [ServiceKnownType(typeof(Base_Settings))]
    [ServiceKnownType(typeof(Base_StatusLog))]
    [ServiceKnownType(typeof(Base_User))]
    [ServiceKnownType(typeof(Base_UserRole))]
    [ServiceKnownType(typeof(Base_Region))]
    [ServiceKnownType(typeof(Base_TypeDictionary))]
    public interface IBasicService
    {
        #region 菜单、配置
        /// <summary>
        /// 新增网站设置，触发缓存更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddConfig(Base_Config model);
        /// <summary>
        /// 修改网站设置，触发缓存更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateConfig(Base_Config model);
        /// <summary>
        /// 获取网站设置,有缓存
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<Base_Config> LoadConfig();
        /// <summary>
        /// 新增配置,触发缓存更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddSettings(Base_Settings model);
        /// <summary>
        /// 修改配置,触发缓存更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateSettings(Base_Settings model);
        /// <summary>
        /// 获取系统所有配置，有缓存
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Settings>> LoadSettings();
        /// <summary>
        /// 根据配置项查找指定配置信息，有缓存
        /// </summary>
        /// <param name="setKey"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Base_Settings> LoadSettingsByKey(Settings setKey);
        /// <summary>
        /// 删除配置项
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteSettingsByIds(List<long> ids);
        /// <summary>
        /// 添加类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddType(Base_TypeDictionary model);
        /// <summary>
        /// 修改类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateType(Base_TypeDictionary model);
        /// <summary>
        /// 获取类型详情
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<Base_TypeDictionary> GetTypeModel(long id);
        /// <summary>
        /// 获取类型列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_TypeDictionary>> GetTypeList(QueryCondition qc);
        /// <summary>
        ///根据指定的父类型
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_TypeDictionary>> GetTypeListByPId(long pId);

        /// <summary>
        ///根据编号
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_TypeDictionary>> GetTypeListByNo(string No);
        /// <summary>
        /// 删除类型
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteTypeByIds(List<long> ids);
        /// <summary>
        /// 获取字典列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Dictionary>> GetDictionaryList(QueryCondition qc);
        /// <summary>
        /// 根据指定类型获取所有类型数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_TypeDictionary>> GetTypeListByType(DictionaryType type);
        /// <summary>
        /// 获取字典列表，，有缓存
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Dictionary>> LoadDictionaryList();
        #endregion

        #region 文档类
        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Log>> GetLogList(QueryCondition qc);
        /// <summary>
        /// 获取审核日志列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_StatusLog>> GetStatusLogList(QueryCondition qc);
        /// <summary>
        /// 获取区域列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Region>> GetRegionList(QueryCondition qc);
        /// <summary>
        /// 加载区域列表，有缓存
        /// </summary>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Region>> LoadRegionList(string parentCode = "");
        /// <summary>
        /// 根据表名和id获取所有附件
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Files>> GetFilesByTable(string tableName, long id);
        /// 添加电子协议
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList">附件列表</param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddProtocol(Base_Protocol model, List<Base_Files> fileList);
        /// <summary>
        /// 修改电子协议
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList">附件</param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateProtocol(Base_Protocol model, List<Base_Files> fileList = null);
        /// <summary>
        /// 获取电子协议详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Base_Protocol> GetProtocolModel(long id);
        /// <summary>
        /// 获取电子协议列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Protocol>> GetProtocolList(QueryCondition qc);
        /// <summary>
        /// 批量删除电子协议
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteProtocoByIds(List<long> ids);
        #endregion

        #region 用户、企业
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [OperationContract]
        Result<UserView> Login(string userName, string password, RoleType roleType, string cmtCode = null);
        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Base_User> GetUserModel(long id);
        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddUser(Base_User model, List<Base_Files> fileList);
        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateUser(Base_User model, List<Base_Files> fileList);
        ///// <summary>
        ///// 修改用户所属子公司
        ///// </summary>
        ///// <param name="id"></param>
        ///// <param name="childId"></param>
        ///// <returns></returns>
        //Result<int> UpdateUserChildCompany(long id, long childId);
        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="passowrd">密文密码</param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateUserPassword(long id, string passowrd);
        /// <summary>
        /// 锁定/解锁用户
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AuditUser(long userId);
        /// <summary>
        /// 根据条件获取用户列表，首先加上默认条件，未删除的数据
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_User>> GetUserList(QueryCondition qc, RoleType? roleType = null);
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteUserByIds(List<long> ids);
        /// <summary>
        /// 添加权限，触发缓存更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddRight(Base_Right model);
        /// <summary>
        /// 批量新增权限,触发缓存更新
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddRightRange(List<Base_Right> models);
        /// <summary>
        /// 获取权限详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Base_Right> GetRightModel(long id);
        /// <summary>
        /// 编辑权限,触发缓存更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateRight(Base_Right model);
        /// <summary>
        /// 根据用户加载对应权限，有缓存
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="listRight"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Right>> LoadRightList(RoleType roleType, long userId = 0, List<long> listRight = null);
        /// <summary>
        /// 根据父级权限获取子权限
        /// </summary>
        /// <param name="roleType"></param>
        /// <param name="parentId"></param>
        /// <param name="isIncludeSelf">是否包含自身</param>
        /// <param name="isChildAll">是否包含所有自权限</param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Right>> GetRightListByRole(RoleType roleType, long parentId, bool isIncludeSelf = false, bool isChildAll = false);
        /// <summary>
        /// 根据条件获取权限
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Right>> GetRightList(QueryCondition qc);
        /// <summary>
        /// 查询尚未添加的权限
        /// </summary>
        /// <param name="rType"></param>
        /// <param name="pId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Dictionary<string, string>> GetRightUNSelect(RoleType rType, long pId, out string type);
        /// <summary>
        /// 根据人员id获取该人所属的所有角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_UserRole>> GetRolesByUserId(long userId);
        /// <summary>
        /// 批量设置角色
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> GetRolesByUserIds(List<long> ids);
        /// <summary>
        /// 根据角色id获取角色权限
        /// </summary>
        /// <param name="roleIds"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_RoleRight>> GetRightByRoleIds(List<long> roleIds);
        /// <summary>
        /// 设置用户角色
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleIds"></param>
        /// <param name="roleType"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> SetUserRole(long userId, List<long> roleIds, RoleType roleType);
        /// <summary>
        /// 删除权限,触发缓存更新
        /// </summary>
        /// <param name="rightId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteRight(long rightId);
        /// <summary>
        /// 批量删除权限，触发缓存更新
        /// </summary>
        /// <param name="rightIds"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteRightbyIds(List<long> rightIds);
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddRole(Base_Role model);
        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateRole(Base_Role model);
        /// <summary>
        /// 获取角色详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Base_Role> GetRoleModel(long id);
        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Role>> GetRoleList(QueryCondition qc);
        /// <summary>
        /// 批量删除角色
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteRoleByIds(List<long> ids);
        /// <summary>
        /// 设置角色权限
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="rightIds"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> SetRoleRight(long roleId, List<long> rightIds);
        /// <summary>
        /// 添加企业
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddCompany(Base_Company model, List<Base_Files> fileList = null);

        [OperationContract]
        Result<int> AddOilStation(Epm_OilStation model);

        /// <summary>
        /// 修改企业 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateCompany(Base_Company model, List<Base_Files> fileList = null);
        [OperationContract]
        Result<int> UpdateOilStation(Epm_OilStation model);
        [OperationContract]
        Result<int> UpdateBaseVideoManage(Base_VideoManage model);
        /// <summary>
        /// 获取企业列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Company>> GetCompanyList(QueryCondition qc);
        /// <summary>
        /// 获取指定企业的子企业
        /// </summary>
        /// <param name="roleType"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Company>> GetCompanyListByRole(long parentId, bool isIncludeSelf = false, bool isChildAll = false);
        /// <summary>
        /// 获取企业详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Base_Company> GetCompanyModel(long id);
        /// <summary>
        /// 获取加油站详情 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Epm_OilStation> GetOilStation(long id);
        [OperationContract]
        Result<Base_VideoManage> GetBaseVideoManages(long id);
        /// <summary>
        /// 批量删除企业
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteCompanyByIds(List<long> ids);
        /// <summary>
        /// 删除加油站 
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteOilStation(List<long> ids);
        /// <summary>
        /// 添加部门
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddDep(Base_Dep model);
        /// <summary>
        /// 修改部门
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateDep(Base_Dep model);
        /// <summary>
        /// 获取部门详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Base_Dep> GeDepModel(long id);
        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Dep>> GeDepList(QueryCondition qc);
        /// <summary>
        /// 根据企业获取部门
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Dep>> GeDepListByCompanyId(long companyId);
        /// <summary>
        /// 批量删除部门
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteDepByIds(List<long> ids);
        /// <summary>
        /// 审核权限
        /// </summary>
        /// <param name="rightId">权限Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AuditRight(long rightId, int type);
        /// <summary>
        /// 审核角色
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AuditRole(long roleId, int type);
        /// <summary>
        /// 审核企业
        /// </summary>
        /// <param name="companyId">企业Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AuditCompany(long companyId, int type);
        /// <summary>
        /// 审核部门
        /// </summary>
        /// <param name="companyId">部门Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AuditDep(long depId, int type);
        /// <summary>
        /// 审核类型数据
        /// </summary>
        /// <param name="typeId">类型数据Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AuditType(long typeId, int type);
        /// <summary>
        /// 审核电子协议
        /// </summary>
        /// <param name="protocolId">电子协议Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AuditProtocol(long protocolId, int type);
        /// <summary>
        /// 根据身份获取角色列表
        /// </summary>
        /// <param name="roleType">角色类型</param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Role>> GetRoleListByBelong(RoleType roleType);
        /// <summary>
        /// 获取管理员或者专家企业
        /// </summary>
        /// <param name="roleType"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Base_Company> GetCompanyByRoleType(RoleType roleType);
        /// <summary>
        /// 获取区域详情
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Base_Region> GetRegionModel(string code);
        /// <summary>
        /// 获取所有类型，已启用，已确认
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_TypeDictionary>> GetAllTypeList();
        /// <summary>
        /// 检查用户是否存在
        /// </summary>
        /// <param name="userCode">用户名/邮箱/电话</param>
        /// <returns></returns>
        [OperationContract]
        Result<bool> IsExitsUser(string userCode);
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userCode">用户名/邮箱/电话</param>
        /// <returns></returns>
        [OperationContract]
        Result<Base_User> GetUserByCode(string userCode);
        /// <summary>
        /// 根据协议类型获取电子协议详情
        /// </summary>
        /// <param name="protocolType"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Base_Protocol> GetProtocolModelByType(ProtocolType protocolType);
        /// <summary>
        /// 根据字典类型集合获取字典数据
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Dictionary<DictionaryType, List<Base_TypeDictionary>>> GetTypeListByTypes(List<DictionaryType> types);
        /// <summary>
        /// 获取指定企业下用户数量
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> GetUserCountByCompanyId(long companyId);

        [OperationContract]
        Result<List<UserListView>> GetUserCompanyList(QueryCondition qc);

        #endregion

        #region 人脸信息

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddAIUserFace(EPM_AIUserFace model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateAIUserFace(EPM_AIUserFace model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteAIUserFaceByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<EPM_AIUserFace>> GetAIUserFaceList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<EPM_AIUserFace> GetAIUserFaceModel(long id);

        [OperationContract]
        Result<EPM_AIUserFace> GetAIUserFaceByUserId(long userId);

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddFaceOperateLog(EPM_FaceOperateLog model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateFaceOperateLog(EPM_FaceOperateLog model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteFaceOperateLogByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<EPM_FaceOperateLog>> GetFaceOperateLogList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<EPM_FaceOperateLog> GetFaceOperateLogModel(long id);

        #endregion

        #region 考勤信息
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddSignInformation(Epm_SignInformation model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateSignInformation(Epm_SignInformation model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteSignInformationByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_SignInformation>> GetSignInformationList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_SignInformation> GetSignInformationModel(long id);

        /// <summary>
        /// 根据项目ID和用户ID获取已签到用户信息
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Epm_SignInformation> GetSignBy(long projectId, long userId);

        #endregion

        /// <summary>
        /// 人脸注册
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="image"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddAIUserFaceInfo(long userId, string image, string source);

        #region 视频设备


        /// <summary>
        /// 新增视频设备
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddVideoManage(Base_VideoManage model);

        [OperationContract]
        Result<Base_VideoManage> GetBaseVideoManageById(long id);
        [OperationContract]
        Result<List<Base_VideoManage>> GetVideoManageList(QueryCondition qc);
        /// <summary>
        /// 激活设备
        /// </summary>
        /// <param name="id">设备 ID</param>
        /// <returns></returns>
        [OperationContract]
        Result<bool> ActivatedVideo(long id);

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteBaseVideoManage(List<long> ids);
        #endregion
    }
}
