using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hc.Plat.Common.Global;
using hc.Plat.Common.Service;
using hc.Plat.Common.Extend;
using hc.epm.DataModel.Basic;
using hc.epm.ViewModel;
using hc.epm.Common;
using hc.epm.DataModel.Business;
using System.Configuration;
using System.Data;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService
    {

        #region 改造前-没有动
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
        /// 获取地市公司
        /// </summary>
        /// <returns></returns>
        public Result<List<Base_Company>> GetAreaCompanyList()
        {
            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            try
            {
                var list = DataOperateBasic<Base_Company>.Get().GetList(t => !t.IsDelete && t.Type == "Owner" && t.PId == 10 && t.OrgType == "2" && t.OrgState == "1").ToList();
                if (list.Count > 0)
                {
                    result.Data = list;
                    result.Flag = EResultFlag.Success;
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetAreaCompanyList");
            }
            return result;
        }


        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_User>> GetUserList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Base_User>> result = new Result<List<Base_User>>();
            try
            {
                using (BasicDataContext baseDataContext = new BasicDataContext())
                {
                    result = DataOperate.QueryListSimple<Base_User>(baseDataContext, qc);
                }
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
        /// 根据企业Id获取用户列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<List<Base_User>> GetUserPostList(long id)
        {
            Result<List<Base_User>> result = new Result<List<Base_User>>();
            try
            {
                //新增了人员成绩和成绩有效期，影响到人员变更，选择人员的时候加成绩有效期的限制
                var list = DataOperateBasic<Base_User>.Get().GetList(t => !t.IsDelete && t.CompanyId == id && (string.IsNullOrEmpty(t.achievementEndTime.ToString()) || t.achievementEndTime > DateTime.Now)).ToList();

                result.Data = list;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetUserPostList");
            }
            return result;
        }

        /// <summary>
        /// 根据单位ID和岗位名称获取人员信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="postName"></param>
        /// <returns></returns>
        public Result<List<Base_User>> GetUserListByPost(long id, string postName)
        {
            Result<List<Base_User>> result = new Result<List<Base_User>>();
            try
            {
                var list = DataOperateBasic<Base_User>.Get().GetList(t => !t.IsDelete && t.CompanyId == id && t.PostValue == postName).ToList();
                result.Data = list;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetUserListByPost");
            }
            return result;
        }

        /// <summary>
        /// 获取用户信息（包含部门信息）
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<UserView>> GetUserListByDepartment(QueryCondition qc)
        {
            Result<List<UserView>> result = new Result<List<UserView>>();

            try
            {
                var query = from a in basicContext.Base_User.Where(p => !p.IsDelete && p.DepartmentId != null)
                            join b in basicContext.Base_Company.Where(p => !p.IsDelete) on a.DepartmentId equals b.Id into bref
                            from c in bref.DefaultIfEmpty()
                            select new UserView
                            {
                                UserId = a.Id,
                                CompanyId = a.CompanyId,
                                DepartmentName = c.Name ?? "",
                                DepartmentId = c.Id,
                                UserName = a.UserName ?? "",
                                Phone = a.Phone ?? "",
                                PostValue = a.PostValue ?? "",
                                LastLoginTime = a.OperateTime,
                                ProfessionalValue = a.ProfessionalValue,
                                ProfessionalQualificationValue = a.ProfessionalQualificationValue,
                                RoleType = RoleType.Owner
                            };
                if (qc.ConditionList != null && qc.ConditionList.Any())
                {
                    foreach (var conditionExpression in qc.ConditionList)
                    {
                        string value = (conditionExpression.ExpValue ?? "").ToString();
                        string valueName = (conditionExpression.ExpName ?? "").ToString();
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            switch (valueName)
                            {
                                case "UserName":
                                    {
                                        query = query.Where(p => p.UserName.Contains(value));
                                        break;
                                    }
                                case "CompanyId":
                                    {
                                        long id = Convert.ToInt64(value);
                                        query = query.Where(p => p.CompanyId == id);
                                        break;
                                    }
                                case "DempName":
                                    {
                                        query = query.Where(p => p.DepartmentName.Contains(value));
                                        break;
                                    }
                                case "PostValue":
                                    {
                                        query = query.Where(p => p.PostValue.Contains(value));
                                        break;
                                    }
                                case "Phone":
                                    {
                                        query = query.Where(p => p.Phone.Contains(value));
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }
                result.AllRowsCount = query.Count();
                query = query.OrderByDescending(t => t.LastLoginTime).Skip((qc.PageInfo.CurrentPageIndex - 1) * qc.PageInfo.PageRowCount).Take(qc.PageInfo.PageRowCount);

                result.Data = query.ToList();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetUserListByDepartment");
            }
            return result;
        }

        #region 选择用户

        /// <summary>
        /// 获取分公司项目负责人
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Result<List<Base_User>> GetBrCompanyPMList(string name, long companyId, int pageIndex, int pageSize)
        {
            var rolelist = new List<string>() { Role.FGXMFZR.GetText() };
            var allUser = DataOperateBasic<Base_User>.Get().GetList(t => t.IsDelete == false && t.CompanyId == companyId).ToList();
            return GetAgencyUserList(name, allUser, pageIndex, pageSize, rolelist);
        }
        /// <summary>
        /// 获取工程处项目经理
        /// </summary>L
        /// <param name="name"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Result<List<Base_User>> GetAgencyPMList(string name, int pageIndex, int pageSize)
        {
            var rolelist = new List<string>() { Role.GCCXMJL.GetText() };
            var allUser = DataOperateBasic<Base_User>.Get().GetList(t => t.IsDelete == false && (t.Card != "QY" || string.IsNullOrEmpty(t.Card))).ToList();
            return GetAgencyUserList(name, allUser, pageIndex, pageSize, rolelist);
        }
        /// <summary>
        /// 获取工程处领导
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private Result<List<Base_User>> GetAgencyLeaderList(string name, int pageIndex, int pageSize)
        {
            var rolelist = new List<string>() { Role.LDBZ.GetText() };
            var allUser = DataOperateBasic<Base_User>.Get().GetList(t => t.IsDelete == false).ToList();
            return GetAgencyUserList(name, allUser, pageIndex, pageSize, rolelist);
        }
        private Result<List<Base_User>> GetAgencyUserList(string name, List<Base_User> allUser, int pageIndex, int pageSize, IList<string> rolelist)
        {
            Result<List<Base_User>> result = new Result<List<Base_User>>();
            try
            {
                var allRole = DataOperateBasic<Base_Role>.Get().GetList(t => t.IsDelete == false && t.Belong == "Owner" && rolelist.Contains(t.RoleName)).ToList();

                var roleIdList = allRole.Select(t => t.Id).ToList();

                var userRole = DataOperateBasic<Base_UserRole>.Get().GetList(t => t.IsDelete == false && roleIdList.Contains(t.RoleId)).ToList();

                var userList = (from user in allUser
                                join ur in userRole on user.Id equals ur.UserId
                                where (user.UserName.Contains(name) || user.UserCode.Contains(name) || user.Phone.Contains(name))
                                select user).Distinct().ToList();

                result.AllRowsCount = userList.Count();
                userList = userList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                result.Data = userList;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetAgencyUserList");
            }
            return result;
        }
        #endregion

        #region 是否工程处用户、是否服务商用户
        /// <summary>
        /// 是否省公司用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsAgencyUser(long userId)
        {
            var sgsrolelist = new List<string>() { Role.LDBZ.GetText(), Role.GCCXMJL.GetText() };

            var allRole = DataOperateBasic<Base_Role>.Get().GetList(t => t.IsDelete == false && t.Belong == "Owner" && sgsrolelist.Contains(t.RoleName)).ToList();

            var roleIdList = allRole.Select(t => t.Id).ToList();

            var userRole = DataOperateBasic<Base_UserRole>.Get().GetList(t => t.IsDelete == false && t.UserId == userId && roleIdList.Contains(t.RoleId)).FirstOrDefault();

            if (userRole == null)
                return false;

            return true;
        }
        /// <summary>
        /// 是否分公司用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsBranchCompanyUser(long userId)
        {
            var fgsrolelist = new List<string>() { Role.FGJL.GetText(), Role.FGBMZR.GetText(), Role.FGXMFZR.GetText() };

            var allRole = DataOperateBasic<Base_Role>.Get().GetList(t => t.IsDelete == false && t.Belong == "Owner" && fgsrolelist.Contains(t.RoleName)).ToList();

            var roleIdList = allRole.Select(t => t.Id).ToList();

            var userRole = DataOperateBasic<Base_UserRole>.Get().GetList(t => t.IsDelete == false && t.UserId == userId && roleIdList.Contains(t.RoleId)).FirstOrDefault();

            if (userRole == null)
                return false;

            return true;
        }
        /// <summary>
        /// 是否分公司部门经理
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsBranchCompanyDirector(long userId)
        {
            var fgsrolelist = new List<string>() { Role.FGBMZR.GetText() };

            var allRole = DataOperateBasic<Base_Role>.Get().GetList(t => t.IsDelete == false && t.Belong == "Owner" && fgsrolelist.Contains(t.RoleName)).ToList();

            var roleIdList = allRole.Select(t => t.Id).ToList();

            var userRole = DataOperateBasic<Base_UserRole>.Get().GetList(t => t.IsDelete == false && t.UserId == userId && roleIdList.Contains(t.RoleId)).FirstOrDefault();

            if (userRole == null)
                return false;

            return true;
        }
        /// <summary>
        /// 是否服务商用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsServiceUser(long userId)
        {
            var sgdwrolelist = new List<string>() { Role.SGDW.GetText() };

            var allRole = DataOperateBasic<Base_Role>.Get().GetList(t => t.IsDelete == false && t.Belong == "Owner" && sgdwrolelist.Contains(t.RoleName)).ToList();

            var roleIdList = allRole.Select(t => t.Id).ToList();

            var userRole = DataOperateBasic<Base_UserRole>.Get().GetList(t => t.IsDelete == false && t.UserId == userId && roleIdList.Contains(t.RoleId)).FirstOrDefault();

            if (userRole == null)
                return false;

            return true;
        }
        /// <summary>
        /// 是否服务商（监理）
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsSupervisor(long projectId, long userId)
        {
            var company = DataOperateBusiness<Epm_ProjectCompany>.Get().GetList(t =>
            t.ProjectId == projectId && (t.IsSupervisor == 1 || t.Type == "监理") && (t.PMId == userId || t.LinkManId == userId)).FirstOrDefault();
            if (company == null)
                return false;

            return true;
        }
        #endregion

        /// <summary>
        /// 根据企业 ID 获取企业详情
        /// </summary>
        /// <param name="id">企业 ID</param>
        public Result<CompanyView> GetCompanyModel(long id)
        {
            Result<CompanyView> result = new Result<CompanyView>();
            BasicDataContext baseDB = new BasicDataContext();
            try
            {
                var viewModel = new CompanyView();
                var model = DataOperateBasic<Base_Company>.Get(baseDB).GetModel(id);
                if (model != null)
                {
                    viewModel.Id = model.Id;
                    viewModel.PId = model.PId;
                    viewModel.PreCode = model.PreCode;
                    viewModel.PreName = model.PreName;
                    viewModel.Type = model.Type;
                    viewModel.Code = model.Code;
                    viewModel.Name = model.Name;
                    viewModel.ShortName = model.ShortName;
                    viewModel.FaxPhone = model.FaxPhone;
                    viewModel.Email = model.Email;
                    viewModel.Address = model.Address;
                    viewModel.AddressInfo = model.AddressInfo;
                    viewModel.Remark = model.Remark;
                    viewModel.Phone = model.Phone;
                    viewModel.LinkManId = model.LinkManId;
                    viewModel.LinkMan = model.LinkMan;
                    viewModel.LinkPhone = model.LinkPhone;
                    viewModel.OrgType = model.OrgType;
                    viewModel.AddressName = model.AddressName;
                    viewModel.IsBlacklist = model.IsBlacklist;
                    viewModel.CompanyType = model.CompanyType;
                    viewModel.CompanyRank = model.CompanyRank;
                    viewModel.CompanyTypeName = model.CompanyTypeName;
                    viewModel.CompanyRankName = model.CompanyRankName;
                    if (!string.IsNullOrWhiteSpace(model.Address))
                    {
                        string[] codes = model.Address.Split(',');
                        string code = codes[codes.Length - 1];

                        var regionModel = DataOperateBasic<Base_Region>.Get(baseDB).Single(i => i.RegionCode == code);
                        if (regionModel != null)
                        {
                            viewModel.RegionName = regionModel.Fullname;
                        }

                        var file = DataOperateBasic<Base_Files>.Get(baseDB).Single(p => p.TableName == "Base_Company" && p.TableId == model.Id);
                        if (file != null)
                        {
                            viewModel.Logo = file.Url;
                        }
                    }
                    var attachsList = GetFilesByTable("Base_Company", model.Id).Data;//根据表名和id获取附件信息
                    if (attachsList != null && attachsList.Any())
                    {
                        List<Base_Files> fileList = new List<Base_Files>();
                        foreach (var item in attachsList)
                        {
                            //转换
                            var noToName = DataOperateBasic<Base_TypeDictionary>.Get().Single(p => p.No == item.TableColumn).Name;
                            item.TypeName = noToName;
                            fileList.Add(item);
                        }

                        viewModel.baseFiles = fileList;
                    }

                }
                result.Data = viewModel;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCompanyModel");
            }
            finally
            {
                if (baseDB.Database.Connection.State == ConnectionState.Closed)
                {
                    baseDB.Database.Connection.Dispose();
                }
            }
            return result;
        }

        /// <summary>
        /// 根据项目获取需要发送消息通知的用户Id和用户名称
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        private Dictionary<long, string> GetWaitSendMessageList(long? projectId)
        {
            Dictionary<long, string> dic = new Dictionary<long, string>();
            var project = DataOperateBusiness<Epm_Project>.Get().GetModel(projectId.Value);
            if (project != null)
            {
                //项目经理
                if (project.PMId != null && project.PMId != CurrentUserID.ToLongReq())
                {
                    if (!dic.Keys.Contains(project.PMId.Value))
                        dic.Add(project.PMId.Value, project.PMName);
                }
                //分公司项目负责人
                if (project.ContactUserId != null && project.ContactUserId != CurrentUserID.ToLongReq())
                {
                    if (!dic.Keys.Contains(project.ContactUserId.Value))
                        dic.Add(project.ContactUserId.Value, project.ContactUserName);
                }
            }
            var projectCompany = DataOperateBusiness<Epm_ProjectCompany>.Get().GetList(t => t.ProjectId == projectId).ToList();
            //List<Base_User> allUser = DataOperateBasic<Base_User>.Get().GetList().ToList();
            //服务商 项目经理 和负责人
            if (projectCompany.Count > 0)
            {
                foreach (var item in projectCompany)
                {
                    //var users = allUser.Where(t => t.Id == item.PMId || t.Id == item.LinkManId).ToList();
                    //foreach (var user in users)
                    //{
                    //    if (user.Id != CurrentUserID.ToLongReq() && !dic.Keys.Contains(user.Id))
                    //        dic.Add(user.Id, user.UserName);
                    //}
                    if (item.LinkManId.HasValue)
                    {
                        if (!dic.Keys.Contains(item.LinkManId.Value))
                            dic.Add(item.LinkManId.Value, item.LinkMan);
                    }
                }
            }
            return dic;
        }

        public Result<int> AddSendDate(Bp_SendDate model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Bp_SendDate>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddSendDate");
            }
            return result;
        }
        #endregion


        #region 三商管理
        /// <summary>
        /// 获取企业列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Company>> GetCompanyListByType(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            try
            {
                using (BasicDataContext baseDataContext = new BasicDataContext())
                {
                    result = DataOperate.QueryListSimple<Base_Company>(baseDataContext, qc);

                    if (result.Flag == EResultFlag.Success && result.Data != null && result.Data.Any())
                    {
                        string[] regionCodes = result.Data.Where(p => !string.IsNullOrWhiteSpace(p.Address))
                            .Select(p => p.Address.Split(',').LastOrDefault()).Distinct().ToArray();
                        if (regionCodes.Any())
                        {
                            var regionModels = baseDataContext.Base_Region.Where(p => regionCodes.Contains(p.RegionCode))
                                .ToDictionary(p => p.RegionCode, p => p.Fullname);

                            if (regionModels.Any())
                            {
                                result.Data.ForEach(p =>
                                {
                                    if (!string.IsNullOrWhiteSpace(p.Address))
                                    {
                                        string regeionCode = p.Address.Split(',').LastOrDefault();
                                        if (regionModels.Keys.Contains(regeionCode))
                                        {
                                            p.RegionName = regionModels[regeionCode];
                                        }
                                    }
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCompanyListByType");
            }
            return result;
        }
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddCompany(Base_Company model)
        {

            Result<int> result = new Result<int>();
            try
            {

                int rows = 0;
                bool isAdd = false;
                var company = DataOperateBasic<Base_Company>.Get().GetList(t => t.Id == model.Id).FirstOrDefault();

                if (company == null)
                {
                    isAdd = true;
                    company = new Base_Company();
                    //当前创建人
                    SetCreateUser(company);
                }
                company.PId = model.PId;//上级单位id
                company.PreCode = model.PreCode;//上级单位编码
                company.PreName = model.PreName;//上级单位
                company.Type = model.Type;//单位类型
                company.Code = model.Code;//单位编码
                company.Name = model.Name;//单位名称
                company.ShortName = model.ShortName;//简称
                company.FaxPhone = model.FaxPhone;//传真电话
                company.Email = model.Email;//邮箱
                company.Address = model.Address;//地址
                company.AddressInfo = model.AddressInfo;//详细地址
                company.Remark = model.Remark;//备注
                company.Phone = model.Phone;//电话
                company.LinkMan = model.LinkMan;//负责人
                company.LinkPhone = model.LinkPhone;//负责人电话
                company.LinkManId = model.LinkManId;//负责人id
                company.OrgType = model.OrgType;//组织机构
                company.OrgState = model.OrgState;//组织机构状态
                company.CompanyState = model.CompanyState;//供应商状态
                company.ObjeId = model.ObjeId;//外部唯一标志
                company.Normal_absence = model.Normal_absence;//正常缺位
                company.Abnormality = model.Abnormality;//异常缺位
                company.AddressName = model.AddressName;//地址名字
                company.IsDelete = false;
                company.IsBlacklist = model.IsBlacklist;
                company.CompanyType = model.CompanyType;
                company.CompanyRank = model.CompanyRank;
                company.CompanyTypeName = model.CompanyTypeName;
                company.CompanyRankName = model.CompanyRankName;
                //model.PId=

                //当前操作人
                SetCurrentUser(company);

                if (isAdd)
                {
                    rows = DataOperateBasic<Base_Company>.Get().Add(company);
                }
                else
                {
                    rows = DataOperateBasic<Base_Company>.Get().Update(company);
                }

                //上传附件
                AddFilesByTable(company, model.baseFiles);

                //if (model.baseFiles != null)
                //{
                //    //删除之前的附件
                //    DeleteFilesByTable(company.GetType().Name, new List<long>() { company.Id });
                //    //新增附件
                //    AddFilesByTable(company, model.baseFiles);
                //}

                result.Data = rows;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddCompany");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateCompany(Base_Company model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var company = DataOperateBasic<Base_Company>.Get().Single(p => p.Id == model.Id);
                if (company == null)
                {
                    throw new Exception("服务商不存在！");
                }
                //company.PId = model.PId;//上级单位id
                //company.PreCode = model.PreCode;//上级单位编码
                //company.PreName = model.PreName;//上级单位
                company.Type = model.Type;//单位类型
                company.Code = model.Code;//单位编码
                company.Name = model.Name;//单位名称
                company.ShortName = model.ShortName;//简称
                company.FaxPhone = model.FaxPhone;//传真电话
                company.Email = model.Email;//邮箱
                company.Address = model.Address;//地址
                company.AddressInfo = model.AddressInfo;//详细地址
                company.Remark = model.Remark;//备注
                company.Phone = model.Phone;//电话
                company.AddressName = model.AddressName;//地址名字
                company.IsBlacklist = model.IsBlacklist;
                company.CompanyType = model.CompanyType;
                company.CompanyRank = model.CompanyRank;
                company.CompanyTypeName = model.CompanyTypeName;
                company.CompanyRankName = model.CompanyRankName;
                //company.LinkMan = model.LinkMan;//负责人
                //company.LinkPhone = model.LinkPhone;//负责人电话
                //company.LinkManId = model.LinkManId;//负责人id
                //company.OrgType = model.OrgType;//组织机构
                //company.CompanyType = model.CompanyType;//供应商类型
                //company.OrgState = model.OrgState;//组织机构状态
                //company.CompanyState = model.CompanyState;//供应商状态
                //company.ObjeId = model.ObjeId;//外部唯一标志
                //company.Normal_absence = model.Normal_absence;//正常缺位
                //company.Abnormality = model.Abnormality;//异常缺位

                //当前操作人
                SetCurrentUser(company);

                var rows = DataOperateBasic<Base_Company>.Get().Update(company);

                //上传附件
                AddFilesByTable(company, model.baseFiles);
                if (model.baseFiles != null)
                {
                    //删除之前的附件
                    DeleteFilesByTable(company.GetType().Name, new List<long>() { company.Id });
                    //新增附件
                    AddFilesByTable(company, model.baseFiles);
                }


                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //  WriteLog(AdminModule.Company.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateCompany");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteCompanyByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBasic<Base_Company>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBasic<Base_Company>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                // WriteLog(AdminModule.Company.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteCompanyByIds");
            }
            return result;
        }
        #endregion

        #region 甲供物资管理关联供应商ID
        /// <summary>
        /// 根据供应商名称获取供应商信息
        /// </summary>
        /// <param name="companyName">供应商名称</param>
        public Result<Base_Company> GetCompanyByName(string companyName)
        {
            Result<Base_Company> result = new Result<Base_Company>();
            BasicDataContext baseDB = new BasicDataContext();
            try
            {
                var model = DataOperateBasic<Base_Company>.Get(baseDB).GetList(t => t.Name == companyName).FirstOrDefault();
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCompanyByName");
            }
            finally
            {
                if (baseDB.Database.Connection.State == ConnectionState.Closed)
                {
                    baseDB.Database.Connection.Dispose();
                }
            }
            return result;
        }
        #endregion
    }
}