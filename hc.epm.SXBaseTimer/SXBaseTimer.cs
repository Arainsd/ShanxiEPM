using hc.epm.Common;
using hc.epm.DataModel.BaseCore;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.Service.ClientSite;
using hc.epm.SXBaseTimer.XtBaseData;
using hc.epm.SXBaseTimer.XtWorkFlow;
using hc.Plat.Common.Global;
using hc.Plat.Common.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.SXBaseTimer
{
    public partial class SXBaseTimer
    {
        /// <summary>
        /// 本地固定访问IP
        /// </summary>
        public static string UrlIp
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings.Get("AllowIp");
            }
        }
        private ClientSiteService clientService = new ClientSiteService();
        private HrmServicePortTypeClient client = new HrmServicePortTypeClient();
        string[] noSysStatus = new string[] { "退回", "中止", "归档" };

        #region 公司信息同步

        /// <summary>
        /// 公司信息同步接口
        /// </summary>
        public void CompanyInfo()
        {
            try
            {
                var companyInfo = client.getHrmSubcompanyInfo(UrlIp);
                //companyInfo = companyInfo.Where(c => c._canceled != "1" && !string.IsNullOrEmpty(c._code)).ToArray();
                if (companyInfo != null && companyInfo.Any())
                {
                    //LogHelper.Info(this, JsonConvert.SerializeObject(companyInfo));
                    var parentCompany = DataOperateBasic<Base_Company>.Get().GetModel(10);
                    //companyInfo = companyInfo.Where(c => Convert.ToInt32(c._canceled) != 1 && !string.IsNullOrEmpty(c._code)).ToArray();
                    if (parentCompany == null)
                    {
                        LogHelper.Equals(this.GetType(), "未找到公司所属父级 Id 10.");
                        return;
                    }

                    // 1. 取出所有从协同接口通过过来的单位 ID；
                    // 2. 将此部分 ID 作为查询条件在 Base_Company 表中查询，获取所有已存在的数据；
                    // 3. 遍历这些数据，在和已同步的数据对比，判断状态，是新增，还是修改或删除；

                    foreach (SubCompanyBean item in companyInfo)
                    {
                        var com = DataOperateBasic<Base_Company>.Get().Single(p => p.ObjeId == item._subcompanyid);

                        // 0 新增， 1 修改， 2 删除
                        int tempState = 0;
                        if (item._canceled != "1" && !string.IsNullOrWhiteSpace(item._code))
                        {
                            if(com == null)
                            {
                                tempState = 0;
                            }
                            else
                            {
                                tempState = 1;
                            }
                        }
                        else
                        {
                            if(com != null)
                            {
                                tempState = 2;
                            }
                        }

                        if(tempState == 0)
                        {
                            com = new Base_Company();
                            com.Type = RoleType.Owner.ToString();
                            com.PId = parentCompany.Id;
                            com.PreName = parentCompany.Name;
                            com.CreateUserId = 0;
                            com.CreateUserName = "系统定时同步";
                            com.CreateTime = DateTime.Now;
                            com.IsDelete = false;
                            com.ObjeId = item._subcompanyid;
                            com.OrgType = "2";
                        }
                        if (string.IsNullOrWhiteSpace(item._canceled) || item._canceled == "0")
                        {
                            com.OrgState = "1";
                        }
                        else
                        {
                            com.OrgState = "0";
                        }
                        com.Name = item._fullname;
                        com.ShortName = item._shortname;
                        com.Code = item._code ?? "";
                        com.OperateUserId = 0;
                        com.OperateUserName = "系统定时同步";
                        com.OperateTime = DateTime.Now;

                        if (tempState == 0)
                        {
                            DataOperateBasic<Base_Company>.Get().Add(com);
                        }
                        else
                        {
                            // 修改
                            if (tempState == 2)
                            {
                                com.IsDelete = true;
                                com.DeleteTime = DateTime.Now;
                            }
                            DataOperateBasic<Base_Company>.Get().Update(com);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, "时间：" + DateTime.Now + "描述：" + ex.Message);
            }
        }

        #endregion

        #region 同步部门信息

        /// <summary>
        /// 部门信息
        /// </summary>
        public void DepartmentInfo()
        {
            var parentCompanyList = DataOperateBasic<Base_Company>.Get().GetList(p => p.PId == 10).ToList();
            if (parentCompanyList.Any())
            {
                foreach (var item in parentCompanyList)
                {
                    SynDepartment(item);
                }
            }
        }

        /// <summary>
        /// 根据分公司获取分公司下的部门
        /// </summary>
        /// <param name="parentCompany">分公司信息</param>
        private void SynDepartment(Base_Company parentCompany)
        {
            try
            {
                // 调用接口获取部门数据
                var department = client.getHrmDepartmentInfo(UrlIp, parentCompany.ObjeId);
                //department = department.Where(d => d._canceled != "1").ToArray();
                if (department != null && department.Any())
                {
                    //LogHelper.Info(this, JsonConvert.SerializeObject(department[0]._subcompanyid + department.Length));
                    //department = department.Where(d => Convert.ToInt32(d._canceled) != 1 && !string.IsNullOrEmpty(d._code)).ToArray();
                    foreach (var item in department)
                    {
                        var company = DataOperateBasic<Base_Company>.Get().Single(p => p.ObjeId == item._departmentid && p.PId != 10 && p.OrgType != "2");
                        //var company = DataOperateBasic<Base_Company>.Get().Single(p => p.ObjeId == item._departmentid && p.PId == parentCompany.Id);

                        // 0 新增， 1 修改， 2 删除
                        int tempState = 0;
                        if (item._canceled != "1")
                        {
                            if (company == null)
                            {
                                tempState = 0;
                            }
                            else
                            {
                                tempState = 1;
                            }
                        }
                        else
                        {
                            if (company != null)
                            {
                                tempState = 2;
                            }
                        }
                        if (tempState == 0)
                        {
                            company = new Base_Company();
                            company.ObjeId = item._departmentid;
                            company.Type = RoleType.Owner.ToString();
                            company.CreateUserId = 0;
                            company.CreateUserName = "系统定时同步";
                            company.CreateTime = DateTime.Now;
                            company.IsDelete = false;
                        }
                        if (string.IsNullOrWhiteSpace(item._canceled) || item._canceled == "0")
                        {
                            company.OrgState = "1";
                        }
                        else
                        {
                            company.OrgState = "0";
                        }

                        company.PId = parentCompany.Id;
                        company.PreName = parentCompany.Name;
                        company.Name = item._fullname;
                        company.ShortName = item._shortname;
                        company.Code = item._code ?? "";

                        company.OperateUserId = 0;
                        company.OperateUserName = "系统定时同步";
                        company.OperateTime = DateTime.Now;

                        // depkind: 0加油站，1油库，2机关，3生产机关，4研发企业，5车，6船，7机关及其他部门
                        switch (item.depkind)
                        {
                            case "0":
                                company.OrgType = "4";
                                break;
                            case "1":
                                company.OrgType = "6";
                                break;
                            case "2":
                                company.OrgType = "3";
                                break;
                            case "3":
                                company.OrgType = "8";
                                break;
                            case "4":
                                company.OrgType = "6";
                                break;
                            case "5":
                                company.OrgType = "9";
                                break;
                            case "6":
                                company.OrgType = "10";
                                break;
                            default:
                                company.OrgType = "11";
                                break;
                        }


                        if (tempState == 0)
                        {
                            DataOperateBasic<Base_Company>.Get().Add(company);
                        }
                        else
                        {
                            if (tempState == 2)
                            {
                                company.IsDelete = true;
                                company.DeleteTime = DateTime.Now;
                            }
                            DataOperateBasic<Base_Company>.Get().Update(company);
                        }

                        // 加油站处理
                        if (item.depkind == "0")
                        {
                            bool isAddStation = false;
                            Epm_OilStation station = DataOperateBusiness<Epm_OilStation>.Get().Single(p => p.Code == item._departmentid);
                            Base_Company subCompany = DataOperateBasic<Base_Company>.Get().Single(p => p.ObjeId == item._subcompanyid && p.PId == 10);
                            if (station == null)
                            {
                                station = new Epm_OilStation();
                                isAddStation = true;

                                station.Code = item._departmentid;
                                station.CrtCompanyId = 10;
                                station.CrtCompanyName = "陕西销售公司";
                                station.CreateUserId = 0;
                                station.CreateUserName = "系统同步";

                            }
                            station.Name = item._fullname;
                            station.Code3 = item._code;
                            if (subCompany != null)
                            {
                                station.OrgId = subCompany.Id;
                                station.OrgName = subCompany.Name;
                            }
                            else
                            {
                                station.OrgId = parentCompany.Id;
                                station.OrgName = parentCompany.Name;
                            }

                            station.OrgCode = "";
                            station.Code4 = item.no_hos;
                            station.Code5 = item.fmiscode;
                            station.InvestType = item.stationassettype;
                            station.StationType = item.stationkind;
                            station.OperateTime = DateTime.Now;

                            if (isAddStation)
                            {
                                DataOperateBusiness<Epm_OilStation>.Get().Add(station);
                            }
                            else
                            {
                                DataOperateBusiness<Epm_OilStation>.Get().Update(station);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Info(this, ex.Message);
            }
        }

        #endregion

        #region 获取岗位信息

        /// <summary>
        /// 获取岗位信息
        /// </summary>
        public void JobInfo()
        {
            var parentCompanyList = DataOperateBasic<Base_Company>.Get().GetList(p => p.PId == 10).ToList();
            if (!parentCompanyList.Any())
            {
                return;
            }
            foreach (var parentCompany in parentCompanyList)
            {
                SynJobInfoAsync(parentCompany.ObjeId);
            }
        }

        /// <summary>
        /// 根据分公司 ID 同步岗位信息
        /// </summary>
        /// <param name="companyId"></param>
        private async void SynJobInfoAsync(string companyId)
        {
            try
            {
                await Task.Run(() =>
                {
                    var jobList = client.getHrmJobTitleInfo(UrlIp, companyId, "");
                    if (jobList != null && jobList.Any())
                    {
                        foreach (JobTitleBean item in jobList)
                        {
                            if (string.IsNullOrWhiteSpace(item._fullname))
                            {
                                continue;
                            }

                            var dic = DataOperateBasic<Base_TypeDictionary>.Get().GetList(t => t.Type == DictionaryType.PostType.ToString() && t.Name == item._fullname).FirstOrDefault();

                            bool isAdd = false;
                            if (dic == null)// 插入
                            {
                                isAdd = true;
                                dic = new Base_TypeDictionary();
                                dic.CreateUserId = 0;
                                dic.CreateUserName = "系统定时同步";
                                dic.CreateTime = DateTime.Now;
                                dic.IsDelete = false;
                                dic.Type = DictionaryType.PostType.ToString();
                            }

                            dic.No = item._jobtitleid;
                            dic.Name = item._fullname;
                            dic.OperateUserId = 0;
                            dic.OperateUserName = "系统定时同步";
                            dic.OperateTime = DateTime.Now;
                            if (isAdd)
                            {
                                DataOperateBasic<Base_TypeDictionary>.Get().Add(dic);
                            }
                            else
                            {
                                DataOperateBasic<Base_TypeDictionary>.Get().Update(dic);
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex.Message);
            }
        }

        #endregion

        #region 用户信息同步

        #region 批量用户信息同步

        /// <summary>
        /// 用户信息同步接口
        /// </summary>
        public void UserInfo()
        {
            var parentCompanyList = DataOperateBasic<Base_Company>.Get().GetList(p => p.PId == 10).ToList();
            if (!parentCompanyList.Any())
            {
                return;
            }
            var postList = DataOperateBasic<Base_TypeDictionary>.Get().GetList(p => p.Type == DictionaryType.PostType.ToString()).ToList();
            foreach (var parentCompany in parentCompanyList)
            {
                SynUserInfo(parentCompany, postList);
            }
        }

        /// <summary>
        /// 根据分公司 ID 获取分公司用户信息
        /// </summary>
        /// <param name="companyId">分公司 ID</param>
        private void SynUserInfo(Base_Company company, List<Base_TypeDictionary> postList)
        {
            try
            {
                // 1. 取出所有从协同接口通过过来的用户 ID；
                // 2. 将此部分 ID 作为查询条件在 Base_User 表中查询，获取所有已存在的数据；
                // 3. 遍历这些数据，在和已同步的数据对比，判断状态，是新增，还是修改或删除；

                var userList = client.getHrmUserInfo(UrlIp, company.ObjeId, "", "", "", "");
                //userList = userList.Where(s => Convert.ToInt32(s.status) < 3 && s.accounttype != 1 && !string.IsNullOrEmpty(s.workcode)).ToArray();
                if (userList != null && userList.Any())
                {
                    //LogHelper.Info(this, JsonConvert.SerializeObject(userList.Length));
                    var departments = DataOperateBasic<Base_Company>.Get().GetList(p => p.Id == company.Id).ToList();
                    //userList = userList.Where(s => Convert.ToInt32(s.status) < 3 && s.accounttype != 1 && !string.IsNullOrEmpty(s.workcode)).ToArray();
                    var userids = userList.Select(u => u.userid.ToString()).ToArray();
                    var userQuery = DataOperateBasic<Base_User>.Get().GetList(p => userids.Contains(p.ObjeId) && p.CompanyId == company.Id);

                    foreach (var item in userList)
                    {
                        long? departmentId = null;

                        if (!string.IsNullOrWhiteSpace(item.departmentid))
                        {
                            var department = departments.FirstOrDefault(p => p.ObjeId == item.departmentid);
                            if (department != null)
                            {
                                departmentId = department.Id;
                            }
                        }
                        
                        // 0 新增， 1 修改， 2 删除
                        int tempState = 0;
                        var user = userQuery.Where(p => p.ObjeId == item.userid.ToString()).FirstOrDefault();
                        if (Convert.ToInt32(item.status) < 3 && item.accounttype != 1 && !string.IsNullOrEmpty(item.workcode))
                        {
                            if (user == null)
                            {
                                tempState = 0;
                            }
                            else
                            {
                                tempState = 1;
                            }
                        }
                        else
                        {
                            if (user != null)
                            {
                                tempState = 2;
                            }
                        }
                        if (tempState == 0)
                        {
                            user = new Base_User();
                            user.CreateUserId = 0;
                            user.CreateUserName = "系统定时同步";
                            user.CreateTime = DateTime.Now;
                            user.IsDelete = false;
                            user.ObjeId = item.userid.ToString();
                            user.LastLoginTime = DateTime.Now;
                            user.PassTime = DateTime.Now;
                        }
                        user.DepartmentId = departmentId;
                        user.UserCode = item.workcode;
                        user.UserName = item.lastname;
                        user.UserAcct = item.loginid;
                        if (string.IsNullOrWhiteSpace(user.PassWord))
                        {
                            user.PassWord = item.password;
                        }
                        user.PassWord = item.password;
                        user.CompanyId = company.Id;
                        user.CompanyName = company.Name;

                        user.Sex = "0".Equals(item.sex);
                        if (!string.IsNullOrWhiteSpace(item.birthday))
                        {
                            DateTime birthday;
                            if (DateTime.TryParse(item.birthday, out birthday))
                            {
                                user.BirthDate = birthday;
                            }
                        }

                        var post = postList.FirstOrDefault(p => p.No == item.jobtitle);
                        if (post != null)
                        {
                            user.Post = post.Name;
                            user.PostValue = post.Name;
                        }
                        if (!string.IsNullOrWhiteSpace(item.mobile))
                        {
                            user.Phone = item.mobile;
                        }
                        else
                        {
                            user.Phone = "17719566379";
                        }

                        user.OperateUserId = 0;
                        user.OperateUserName = "系统定时同步";
                        user.OperateTime = DateTime.Now;
                        if (tempState == 0)
                        {
                            DataOperateBasic<Base_User>.Get().Add(user);
                        }
                        else
                        {
                            if (tempState == 2)
                            {
                                user.IsDelete = true;
                                user.DeleteTime = DateTime.Now;
                            }
                            DataOperateBasic<Base_User>.Get().Update(user);
                        }

                    }
                    
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex.Message);
            }
        }

        #endregion

        #region 批量同步当天有变化的用户信息

        /// <summary>
        /// 获取当天更新的客户信息
        /// </summary>
        public void UpdateUserInfo()
        {
            var parentCompanyList = DataOperateBasic<Base_Company>.Get().GetList(p => p.PId == 10).ToList();
            if (!parentCompanyList.Any())
            {
                return;
            }
            var postList = DataOperateBasic<Base_TypeDictionary>.Get().GetList(p => p.Type == DictionaryType.PostType.ToString()).ToList();

            foreach (var parentCompany in parentCompanyList)
            {
                SynUpdateUserInfo(parentCompany, postList);
            }
        }

        /// <summary>
        /// 根据分公司 ID 获取当天更新的用户信息
        /// </summary>
        /// <param name="companyId">分公司 ID </param>
        private void SynUpdateUserInfo(Base_Company company, List<Base_TypeDictionary> postList)
        {
            try
            {
                var userList = client.getHrmUserInfo(UrlIp, company.ObjeId, "", "", DateTime.Today.ToString("yyyy-MM-dd"), "");
                if (userList != null && userList.Any())
                {
                    //LogHelper.Info(this, JsonConvert.SerializeObject(userList.Length));
                    var departments = DataOperateBasic<Base_Company>.Get().GetList(p => p.Id == company.Id).ToList();
                    //userList = userList.Where(s => Convert.ToInt32(s.status) < 3 && s.accounttype != 1 && !string.IsNullOrEmpty(s.workcode)).ToArray();
                    var userids = userList.Select(u => u.userid.ToString()).ToArray();
                    var userQuery = DataOperateBasic<Base_User>.Get().GetList(p => userids.Contains(p.ObjeId) && p.CompanyId == company.Id);

                    foreach (var item in userList)
                    {
                        long? departmentId = null;

                        if (!string.IsNullOrWhiteSpace(item.departmentid))
                        {
                            var department = departments.FirstOrDefault(p => p.ObjeId == item.departmentid);
                            if (department != null)
                            {
                                departmentId = department.Id;
                            }
                        }

                        // 0 新增， 1 修改， 2 删除
                        int tempState = 0;
                        var user = userQuery.Where(p => p.ObjeId == item.userid.ToString()).FirstOrDefault();
                        if (Convert.ToInt32(item.status) < 3 && item.accounttype != 1 && !string.IsNullOrEmpty(item.workcode))
                        {
                            if (user == null)
                            {
                                tempState = 0;
                            }
                            else
                            {
                                tempState = 1;
                            }
                        }
                        else
                        {
                            if (user != null)
                            {
                                tempState = 2;
                            }
                        }
                        if (tempState == 0)
                        {
                            user = new Base_User();
                            user.CreateUserId = 0;
                            user.CreateUserName = "系统定时同步";
                            user.CreateTime = DateTime.Now;
                            user.IsDelete = false;
                            user.ObjeId = item.userid.ToString();
                            user.LastLoginTime = DateTime.Now;
                            user.PassTime = DateTime.Now;
                        }
                        user.DepartmentId = departmentId;
                        user.UserCode = item.workcode;
                        user.UserName = item.lastname;
                        user.UserAcct = item.loginid;
                        if (string.IsNullOrWhiteSpace(user.PassWord))
                        {
                            user.PassWord = item.password;
                        }
                        user.PassWord = item.password;
                        user.CompanyId = company.Id;
                        user.CompanyName = company.Name;

                        user.Sex = "0".Equals(item.sex);
                        if (!string.IsNullOrWhiteSpace(item.birthday))
                        {
                            DateTime birthday;
                            if (DateTime.TryParse(item.birthday, out birthday))
                            {
                                user.BirthDate = birthday;
                            }
                        }

                        var post = postList.FirstOrDefault(p => p.No == item.jobtitle);
                        if (post != null)
                        {
                            user.Post = post.Name;
                            user.PostValue = post.Name;
                        }
                        if (!string.IsNullOrWhiteSpace(item.mobile))
                        {
                            user.Phone = item.mobile;
                        }
                        else
                        {
                            user.Phone = "17719566379";
                        }

                        user.OperateUserId = 0;
                        user.OperateUserName = "系统定时同步";
                        user.OperateTime = DateTime.Now;
                        if (tempState == 0)
                        {
                            DataOperateBasic<Base_User>.Get().Add(user);
                        }
                        else
                        {
                            if (tempState == 2)
                            {
                                user.IsDelete = true;
                                user.DeleteTime = DateTime.Now;
                            }
                            DataOperateBasic<Base_User>.Get().Update(user);
                        }

                    }
                    ////userList = userList.Where(s => Convert.ToInt32(s.status) < 3 && s.accounttype == 0 && !string.IsNullOrEmpty(s.workcode)).ToArray();
                    //foreach (UserBean item in userList)
                    //{
                    //    var user = DataOperateBasic<Base_User>.Get().GetList(p => p.Id == item.userid).FirstOrDefault();

                    //    bool isAdd = false;
                    //    if (user == null)// 插入
                    //    {
                    //        isAdd = true;
                    //        user = new Base_User();
                    //        user.CreateUserId = 0;
                    //        user.CreateUserName = "系统定时同步";
                    //        user.CreateTime = DateTime.Now;
                    //        user.IsDelete = false;
                    //    }
                    //    user.UserCode = item.workcode;
                    //    user.CompanyId = company.Id;
                    //    user.CompanyName = company.Name;
                    //    user.UserName = item.lastname;
                    //    user.UserAcct = item.loginid;
                    //    user.PassWord = item.password;
                    //    if (item.mobile != null)
                    //    {
                    //        user.Phone = item.mobile;
                    //    }
                    //    else
                    //    {
                    //        user.Phone = "17719566379";
                    //    }
                    //    user.Sex = "0".Equals(item.sex);
                    //    if (!string.IsNullOrWhiteSpace(item.birthday))
                    //    {
                    //        DateTime birthday;
                    //        if (DateTime.TryParse(item.birthday, out birthday))
                    //        {
                    //            user.BirthDate = birthday;
                    //        }
                    //    }

                    //    var post = postList.FirstOrDefault(p => p.No == item.jobtitle);
                    //    if (post != null)
                    //    {
                    //        user.Post = post.Name;
                    //        user.PostValue = post.Name;
                    //    }

                    //    user.OperateUserId = 0;
                    //    user.OperateUserName = "系统定时同步";
                    //    user.OperateTime = DateTime.Now;
                    //    if (isAdd)
                    //    {
                    //        DataOperateBasic<Base_User>.Get().Add(user);
                    //    }
                    //    else
                    //    {
                    //        DataOperateBasic<Base_User>.Get().Update(user);
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex.Message);
            }
        }

        #endregion

        #endregion

        #region 工作流状态同步

        #region 试运行申请

        /// <summary>
        /// 试运行申请审批状态同步
        /// </summary>
        public void SyxsqWorkFlowStatus()
        {

            try
            {
                var list = DataOperateBusiness<Epm_TzProjectPolit>.Get().GetList(p => !noSysStatus.Contains(p.StateName) && !string.IsNullOrEmpty(p.WorkFlowId) && p.State == (int)PreProjectApprovalState.WaitApproval).ToList();
                List<long> userIds = list.Select(p => p.CreateUserId).Distinct().ToList();
                var baseUser = DataOperateBasic<Base_User>.Get().GetList(p => userIds.Contains(p.Id)).ToList();

                WorkflowServicePortTypeClient workFlowClient = new WorkflowServicePortTypeClient();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        SysSyxsqWorkFlowStatusAsync(baseUser, item, workFlowClient);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex.Message);
            }
        }

        /// <summary>
        /// 同步流程审批状态并将最新状态更新
        /// </summary>
        /// <param name="baseUser">申请人列表</param>
        /// <param name="item">申请对象</param>
        /// <param name="workFlowClient">协同审批流接口服务</param>
        private async void SysSyxsqWorkFlowStatusAsync(List<Base_User> baseUser, Epm_TzProjectPolit item, WorkflowServicePortTypeClient workFlowClient)
        {
            await Task.Run(() =>
            {
                try
                {
                    var user = baseUser.FirstOrDefault(p => p.Id == item.CreateUserId);
                    if (user != null)
                    {
                        // 参数 1：申请 ID， 参数 2：创建人 ID， 参数 3： 流程 ID
                        var data = workFlowClient.getWorkflowRequest(Convert.ToInt32(item.WorkFlowId), Convert.ToInt32(user.ObjeId), (int)XtWorkFlowCode.WfSyxsq);
                        if (data != null && data.workflowRequestLogs != null && data.workflowRequestLogs.Any())
                        {
                            var workLog = data.workflowRequestLogs.OrderByDescending(p => p.id).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(workLog.operateType) && workLog.operateType != item.StateName)
                            {
                                item.StateName = workLog.operateType;
                                item.OperateTime = DateTime.Now;
                                item.ApprovalName = workLog.operatorName;

                                DataOperateBusiness<Epm_TzProjectPolit>.Get().Update(item);

                                //修改状态并且生成下一阶段数据
                                List<long> idList = new List<long> { item.Id };
                                string state = "";
                                if (data.status == "归档")
                                {
                                    state = PreProjectApprovalState.ApprovalSuccess.ToString();
                                }
                                else if (workLog.operateType == "退回" || data.status == "中止")
                                {
                                    state = PreProjectApprovalState.WaitSubmitted.ToString();
                                }
                                if (!string.IsNullOrEmpty(state))
                                {
                                        var result = clientService.UpdateTzProjectPolitState(idList, state);
                                  
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(this, ex.Message);
                }
            });
        }



        #endregion

        #region 项目设计变更申请
        /// <summary>
        /// 项目设计变更申请审批状态同步
        /// </summary>
        public void TzChangeWorkFlowStatus()
        {

            try
            {
                var list = DataOperateBusiness<Epm_TzDesiginChangeApply>.Get().GetList(p => !noSysStatus.Contains(p.StateName) && !string.IsNullOrEmpty(p.WorkFlowId) && p.State == (int)PreProjectApprovalState.WaitApproval).ToList();
                List<long> userIds = list.Select(p => p.CreateUserId).Distinct().ToList();
                var baseUser = DataOperateBasic<Base_User>.Get().GetList(p => userIds.Contains(p.Id)).ToList();

                WorkflowServicePortTypeClient workFlowClient = new WorkflowServicePortTypeClient();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        SysTzChangeWorkFlowStatusAsync(baseUser, item, workFlowClient);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex.Message);
            }
        }

        /// <summary>
        /// 同步流程审批状态并将最新状态更新
        /// </summary>
        /// <param name="baseUser">申请人列表</param>
        /// <param name="item">申请对象</param>
        /// <param name="workFlowClient">协同审批流接口服务</param>
        private async void SysTzChangeWorkFlowStatusAsync(List<Base_User> baseUser, Epm_TzDesiginChangeApply item, WorkflowServicePortTypeClient workFlowClient)
        {
            await Task.Run(() =>
            {
                try
                {
                    var user = baseUser.FirstOrDefault(p => p.Id == item.CreateUserId);
                    if (user != null)
                    {
                        // 参数 1：申请 ID， 参数 2：创建人 ID， 参数 3： 流程 ID
                        var data = workFlowClient.getWorkflowRequest(Convert.ToInt32(item.WorkFlowId), Convert.ToInt32(user.ObjeId), (int)XtWorkFlowCode.WfGcsjbg);
                        if (data != null && data.workflowRequestLogs != null && data.workflowRequestLogs.Any())
                        {
                            var workLog = data.workflowRequestLogs.OrderByDescending(p => p.id).FirstOrDefault();
                            if (workLog.operateType != item.StateName)
                            {
                                item.StateName = workLog.operateType;
                                item.OperateTime = DateTime.Now;
                                item.ApprovalName = workLog.operatorName;
                                item.ApprovalNameId = Convert.ToInt64(workLog.operatorId);

                                DataOperateBusiness<Epm_TzDesiginChangeApply>.Get().Update(item);

                                //修改状态并且生成下一阶段数据
                                List<long> idList = new List<long> { item.Id };
                                string state = "";
                                if (workLog.operateType == "提交")
                                {
                                    state = PreProjectState.ApprovalSuccess.ToString();
                                }
                                else if (workLog.operateType == "退回")
                                {
                                    state = PreProjectState.ApprovalFailure.ToString();
                                }
                                if (!string.IsNullOrEmpty(state))
                                {
                                    var result = clientService.UpdateTzDesiginChangeApplyState(idList, state);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(this, ex.Message);
                }
            });
        }
        #endregion

        #region 项目人员变更
        /// <summary>
        /// 项目人员变更申请审批状态同步
        /// </summary>
        public void TzPeopleChgApplyWorkFlowStatus()
        {

            try
            {
                var list = DataOperateBusiness<Epm_TzPeopleChgApply>.Get().GetList(p => !noSysStatus.Contains(p.StateName) && !string.IsNullOrEmpty(p.WorkFlowId)).ToList();
                List<long> userIds = list.Select(p => p.CreateUserId).Distinct().ToList();
                var baseUser = DataOperateBasic<Base_User>.Get().GetList(p => userIds.Contains(p.Id)).ToList();

                WorkflowServicePortTypeClient workFlowClient = new WorkflowServicePortTypeClient();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        SysTzPeopleChgApplyWorkFlowStatusAsync(baseUser, item, workFlowClient);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex.Message);
            }
        }

        /// <summary>
        /// 同步流程审批状态并将最新状态更新
        /// </summary>
        /// <param name="baseUser">申请人列表</param>
        /// <param name="item">申请对象</param>
        /// <param name="workFlowClient">协同审批流接口服务</param>
        private async void SysTzPeopleChgApplyWorkFlowStatusAsync(List<Base_User> baseUser, Epm_TzPeopleChgApply item, WorkflowServicePortTypeClient workFlowClient)
        {
            await Task.Run(() =>
            {
                try
                {
                    var user = baseUser.FirstOrDefault(p => p.Id == item.CreateUserId);
                    if (user != null)
                    {
                        // 参数 1：申请 ID， 参数 2：创建人 ID， 参数 3： 流程 ID
                        var data = workFlowClient.getWorkflowRequest(Convert.ToInt32(item.WorkFlowId), Convert.ToInt32(user.ObjeId), (int)XtWorkFlowCode.WfGcglrybg);
                        if (data != null && data.workflowRequestLogs != null && data.workflowRequestLogs.Any())
                        {
                            var workLog = data.workflowRequestLogs.OrderByDescending(p => p.id).FirstOrDefault();
                            if (workLog.operateType != item.StateName)
                            {
                                item.StateName = workLog.operateType;
                                item.OperateTime = DateTime.Now;
                                item.ApprovalName = workLog.operatorName;
                                item.ApprovalNameId = workLog.operatorId;

                                if (data.status == "归档")
                                {
                                    item.State = (int)PreProjectApprovalState.ApprovalSuccess;
                                }
                                else if (workLog.operateType == "退回" || data.status == "中止")
                                {
                                    item.State = (int)PreProjectApprovalState.WaitSubmitted;
                                }
                                DataOperateBusiness<Epm_TzPeopleChgApply>.Get().Update(item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(this, ex.Message);
                }
            });
        }
        #endregion

        #region 竞争对手加油（气）站现状申请
        /// <summary>
        /// 竞争对手加油（气）站现状申请审批状态同步
        /// </summary>
        public void TzRivalWorkFlowStatus()
        {

            try
            {
                var list = DataOperateBusiness<Epm_TzRivalStationReport>.Get().GetList(p => !noSysStatus.Contains(p.StateName) && !string.IsNullOrEmpty(p.WorkFlowId)).ToList();
                List<long> userIds = list.Select(p => p.CreateUserId).Distinct().ToList();
                var baseUser = DataOperateBasic<Base_User>.Get().GetList(p => userIds.Contains(p.Id)).ToList();

                WorkflowServicePortTypeClient workFlowClient = new WorkflowServicePortTypeClient();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        SysTzRivalWorkFlowStatusAsync(baseUser, item, workFlowClient);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex.Message);
            }
        }

        /// <summary>
        /// 同步流程审批状态并将最新状态更新
        /// </summary>
        /// <param name="baseUser">申请人列表</param>
        /// <param name="item">申请对象</param>
        /// <param name="workFlowClient">协同审批流接口服务</param>
        private async void SysTzRivalWorkFlowStatusAsync(List<Base_User> baseUser, Epm_TzRivalStationReport item, WorkflowServicePortTypeClient workFlowClient)
        {
            await Task.Run(() =>
            {
                try
                {
                    var user = baseUser.FirstOrDefault(p => p.Id == item.CreateUserId);
                    if (user != null)
                    {
                        // 参数 1：申请 ID， 参数 2：创建人 ID， 参数 3： 流程 ID
                        var data = workFlowClient.getWorkflowRequest(Convert.ToInt32(item.WorkFlowId), Convert.ToInt32(user.ObjeId), (int)XtWorkFlowCode.Wfjzdsjyz);
                        if (data != null && data.workflowRequestLogs != null && data.workflowRequestLogs.Any())
                        {
                            var workLog = data.workflowRequestLogs.OrderByDescending(p => p.id).FirstOrDefault();
                            if (workLog.operateType != item.StateName)
                            {
                                item.StateName = workLog.operateType;
                                item.OperateTime = DateTime.Now;
                                item.ApprovalName = workLog.operatorName;
                                item.ApprovalNameId = workLog.operatorId;

                                DataOperateBusiness<Epm_TzRivalStationReport>.Get().Update(item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(this, ex.Message);
                }
            });
        }
        #endregion

        #region 加油（气）站开发资源上报流程
        /// <summary>
        /// 加油（气）站开发资源上报流程申请审批状态同步
        /// </summary>
        public void TzDevWorkFlowStatus()
        {

            try
            {
                var list = DataOperateBusiness<Epm_TzDevResourceReport>.Get().GetList(p => !noSysStatus.Contains(p.StateName) && !string.IsNullOrEmpty(p.WorkFlowId)).ToList();
                List<long> userIds = list.Select(p => p.CreateUserId).Distinct().ToList();
                var baseUser = DataOperateBasic<Base_User>.Get().GetList(p => userIds.Contains(p.Id)).ToList();

                WorkflowServicePortTypeClient workFlowClient = new WorkflowServicePortTypeClient();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        SysTzDevWorkFlowStatusAsync(baseUser, item, workFlowClient);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex.Message);
            }
        }

        /// <summary>
        /// 同步流程审批状态并将最新状态更新
        /// </summary>
        /// <param name="baseUser">申请人列表</param>
        /// <param name="item">申请对象</param>
        /// <param name="workFlowClient">协同审批流接口服务</param>
        private async void SysTzDevWorkFlowStatusAsync(List<Base_User> baseUser, Epm_TzDevResourceReport item, WorkflowServicePortTypeClient workFlowClient)
        {
            await Task.Run(() =>
            {
                try
                {
                    var user = baseUser.FirstOrDefault(p => p.Id == item.CreateUserId);
                    if (user != null)
                    {
                        // 参数 1：申请 ID， 参数 2：创建人 ID， 参数 3： 流程 ID
                        var data = workFlowClient.getWorkflowRequest(Convert.ToInt32(item.WorkFlowId), Convert.ToInt32(user.ObjeId), (int)XtWorkFlowCode.WfYxkfjyz);
                        if (data != null && data.workflowRequestLogs != null && data.workflowRequestLogs.Any())
                        {
                            var workLog = data.workflowRequestLogs.OrderByDescending(p => p.id).FirstOrDefault();
                            if (workLog.operateType != item.StateName)
                            {
                                item.StateName = workLog.operateType;
                                item.OperateTime = DateTime.Now;
                                item.ApprovalName = workLog.operatorName;
                                item.ApprovalNameId = workLog.operatorId;

                                DataOperateBusiness<Epm_TzDevResourceReport>.Get().Update(item);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(this, ex.Message);
                }
            });
        }
        #endregion

        #region 评审材料上报
        /// <summary>
        /// 评审材料上报审批状态同步
        /// </summary>
        public void TzFormWorkFlowStatus()
        {

            try
            {
                var list = DataOperateBusiness<Epm_TzFormTalkFile>.Get().GetList(p => !noSysStatus.Contains(p.StateName) && !string.IsNullOrEmpty(p.WorkFlowId) && p.State == (int)PreProjectApprovalState.WaitApproval).ToList();
                List<long> userIds = list.Select(p => p.CreateUserId).Distinct().ToList();
                var baseUser = DataOperateBasic<Base_User>.Get().GetList(p => userIds.Contains(p.Id)).ToList();

                WorkflowServicePortTypeClient workFlowClient = new WorkflowServicePortTypeClient();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        SysTzFormWorkFlowStatusAsync(baseUser, item, workFlowClient);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex.Message);
            }
        }

        /// <summary>
        /// 同步流程审批状态并将最新状态更新
        /// </summary>
        /// <param name="baseUser">申请人列表</param>
        /// <param name="item">申请对象</param>
        /// <param name="workFlowClient">协同审批流接口服务</param>
        private async void SysTzFormWorkFlowStatusAsync(List<Base_User> baseUser, Epm_TzFormTalkFile item, WorkflowServicePortTypeClient workFlowClient)
        {
            await Task.Run(() =>
            {
                try
                {
                    var user = baseUser.FirstOrDefault(p => p.Id == item.CreateUserId);
                    if (user != null)
                    {
                        // 参数 1：申请 ID， 参数 2：创建人 ID， 参数 3： 流程 ID
                        var data = workFlowClient.getWorkflowRequest(Convert.ToInt32(item.WorkFlowId), Convert.ToInt32(user.ObjeId), (int)XtWorkFlowCode.WfPsclsb);
                        if (data != null && data.workflowRequestLogs != null && data.workflowRequestLogs.Any())
                        {
                            var workLog = data.workflowRequestLogs.OrderByDescending(p => p.id).FirstOrDefault();
                            if (workLog.operateType != item.StateName)
                            {
                                item.StateName = workLog.operateType;
                                item.OperateTime = DateTime.Now;
                                item.ApprovalName = workLog.operatorName;
                                item.ApprovalNameId = Convert.ToInt64(workLog.operatorId);
                                DataOperateBusiness<Epm_TzFormTalkFile>.Get().Update(item);
                                //修改状态并且生成下一阶段数据
                                List<long> idList = new List<long> { item.ProjectId.Value };
                                string state = "";
                                if (data.status == "归档")
                                {
                                    state = PreProjectApprovalState.ApprovalSuccess.ToString();
                                }
                                else if (workLog.operateType == "退回" || data.status == "中止")
                                {
                                    state = PreProjectApprovalState.WaitSubmitted.ToString();
                                }
                                if (!string.IsNullOrEmpty(state))
                                {
                                        var result = clientService.UpdateTzFormTalkFileState(idList, state);
                                    
                                }

                                
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(this, ex.Message);
                }
            });
        }
        #endregion

        #region 项目评审记录
        /// <summary>
        /// 项目评审记录审批状态同步
        /// </summary>
        public void TzReveiewsWorkFlowStatus()
        {

            try
            {
                var list = DataOperateBusiness<Epm_TzProjectReveiews>.Get().GetList(p => !noSysStatus.Contains(p.StateName) && !string.IsNullOrEmpty(p.WorkFlowId) && p.State == (int)PreProjectApprovalState.WaitApproval).ToList();
                List<long> userIds = list.Select(p => p.CreateUserId).Distinct().ToList();
                var baseUser = DataOperateBasic<Base_User>.Get().GetList(p => userIds.Contains(p.Id)).ToList();

                WorkflowServicePortTypeClient workFlowClient = new WorkflowServicePortTypeClient();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        SysTzReveiewsWorkFlowStatusAsync(baseUser, item, workFlowClient);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex.Message);
            }
        }

        /// <summary>
        /// 同步流程审批状态并将最新状态更新
        /// </summary>
        /// <param name="baseUser">申请人列表</param>
        /// <param name="item">申请对象</param>
        /// <param name="workFlowClient">协同审批流接口服务</param>
        private async void SysTzReveiewsWorkFlowStatusAsync(List<Base_User> baseUser, Epm_TzProjectReveiews item, WorkflowServicePortTypeClient workFlowClient)
        {
            await Task.Run(() =>
            {
                try
                {
                    var user = baseUser.FirstOrDefault(p => p.Id == item.CreateUserId);
                    if (user != null)
                    {
                        // 参数 1：申请 ID， 参数 2：创建人 ID， 参数 3： 流程 ID
                        var data = workFlowClient.getWorkflowRequest(Convert.ToInt32(item.WorkFlowId), Convert.ToInt32(user.ObjeId), (int)XtWorkFlowCode.Wfxmps);
                        if (data != null && data.workflowRequestLogs != null && data.workflowRequestLogs.Any())
                        {
                            var workLog = data.workflowRequestLogs.OrderByDescending(p => p.id).FirstOrDefault();
                            if (workLog.operateType != item.StateName)
                            {
                                item.StateName = workLog.operateType;
                                item.OperateTime = DateTime.Now;
                                item.ApprovalName = workLog.operatorName;
                                item.ApprovalNameId = Convert.ToInt64(workLog.operatorId);
                                DataOperateBusiness<Epm_TzProjectReveiews>.Get().Update(item);
                                //修改状态并且生成下一阶段数据
                                List<long> idList = new List<long> { item.ProjectId };
                                string state = "";
                                if (data.status == "归档")
                                {
                                    state = PreProjectApprovalState.ApprovalSuccess.ToString();
                                }
                                else if (workLog.operateType == "退回" || data.status == "中止")
                                {
                                    state = PreProjectApprovalState.WaitSubmitted.ToString();
                                }
                                if (!string.IsNullOrEmpty(state))
                                {
                                        var result = clientService.UpdateTzProjectReveiewsState(idList, state);
                                    
                                }
                                
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(this, ex.Message);
                }
            });
        }
        #endregion

        #region 上会材料上报
        /// <summary>
        /// 上会材料上报审批状态同步
        /// </summary>
        public void MeetingFileWorkFlowStatus()
        {

            try
            {
                var list = DataOperateBusiness<Epm_MeetingFileReport>.Get().GetList(p => !noSysStatus.Contains(p.StateName) && !string.IsNullOrEmpty(p.WorkFlowId) && p.State == (int)PreProjectApprovalState.WaitApproval).ToList();
                List<long> userIds = list.Select(p => p.CreateUserId).Distinct().ToList();
                var baseUser = DataOperateBasic<Base_User>.Get().GetList(p => userIds.Contains(p.Id)).ToList();

                WorkflowServicePortTypeClient workFlowClient = new WorkflowServicePortTypeClient();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        SysMeetingFileWorkFlowStatusAsync(baseUser, item, workFlowClient);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex.Message);
            }
        }

        /// <summary>
        /// 同步流程审批状态并将最新状态更新
        /// </summary>
        /// <param name="baseUser">申请人列表</param>
        /// <param name="item">申请对象</param>
        /// <param name="workFlowClient">协同审批流接口服务</param>
        private async void SysMeetingFileWorkFlowStatusAsync(List<Base_User> baseUser, Epm_MeetingFileReport item, WorkflowServicePortTypeClient workFlowClient)
        {
            await Task.Run(() =>
            {
                try
                {
                    var user = baseUser.FirstOrDefault(p => p.Id == item.CreateUserId);
                    if (user != null)
                    {
                        // 参数 1：申请 ID， 参数 2：创建人 ID， 参数 3： 流程 ID
                        var data = workFlowClient.getWorkflowRequest(Convert.ToInt32(item.WorkFlowId), Convert.ToInt32(user.ObjeId), (int)XtWorkFlowCode.WfShclsb);
                        if (data != null && data.workflowRequestLogs != null && data.workflowRequestLogs.Any())
                        {
                            var workLog = data.workflowRequestLogs.OrderByDescending(p => p.id).FirstOrDefault();
                            if (workLog.operateType != item.StateName)
                            {
                                item.StateName = workLog.operateType;
                                item.OperateTime = DateTime.Now;
                                item.ApprovalName = workLog.operatorName;
                                item.ApprovalNameId = Convert.ToInt64(workLog.operatorId);
                                DataOperateBusiness<Epm_MeetingFileReport>.Get().Update(item);
                                //修改状态并且生成下一阶段数据
                                List<long> idList = new List<long> { item.ProjectId };
                                string state = "";
                                if (data.status == "归档")
                                {
                                    state = PreProjectApprovalState.ApprovalSuccess.ToString();
                                }
                                else if (workLog.operateType == "退回" || data.status == "中止")
                                {
                                    state = PreProjectApprovalState.WaitSubmitted.ToString();
                                }
                                if (!string.IsNullOrEmpty(state))
                                {
                                        var result = clientService.UpdateMeetingFileReportState(idList, state);
                                    
                                }

                                
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(this, ex.Message);
                }
            });
        }
        #endregion

        #region 项目批复
        /// <summary>
        /// 项目批复审批状态同步
        /// </summary>
        public void TzApprovalInfoWorkFlowStatus()
        {

            try
            {
                var list = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().GetList(p => !noSysStatus.Contains(p.StateName) && !string.IsNullOrEmpty(p.WorkFlowId) && p.State == (int)PreProjectApprovalState.WaitApproval).ToList();
                List<long> userIds = list.Select(p => p.CreateUserId).Distinct().ToList();
                var baseUser = DataOperateBasic<Base_User>.Get().GetList(p => userIds.Contains(p.Id)).ToList();

                WorkflowServicePortTypeClient workFlowClient = new WorkflowServicePortTypeClient();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        SysTzApprovalInfoWorkFlowStatusAsync(baseUser, item, workFlowClient);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex.Message);
            }
        }

        /// <summary>
        /// 同步流程审批状态并将最新状态更新
        /// </summary>
        /// <param name="baseUser">申请人列表</param>
        /// <param name="item">申请对象</param>
        /// <param name="workFlowClient">协同审批流接口服务</param>
        private async void SysTzApprovalInfoWorkFlowStatusAsync(List<Base_User> baseUser, Epm_TzProjectApprovalInfo item, WorkflowServicePortTypeClient workFlowClient)
        {
            await Task.Run(() =>
            {
                try
                {
                    var user = baseUser.FirstOrDefault(p => p.Id == item.CreateUserId);
                    if (user != null)
                    {
                        // 参数 1：申请 ID， 参数 2：创建人 ID， 参数 3： 流程 ID
                        var data = workFlowClient.getWorkflowRequest(Convert.ToInt32(item.WorkFlowId), Convert.ToInt32(user.ObjeId), (int)XtWorkFlowCode.Wfxmpf);
                        if (data != null && data.workflowRequestLogs != null && data.workflowRequestLogs.Any())
                        {
                            var workLog = data.workflowRequestLogs.OrderByDescending(p => p.id).FirstOrDefault();
                            if (workLog.operateType != item.StateName)
                            {
                                item.StateName = workLog.operateType;
                                item.OperateTime = DateTime.Now;
                                item.ApprovalName = workLog.operatorName;
                                item.ApprovalNameId = Convert.ToInt64(workLog.operatorId);
                                DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().Update(item);
                                //修改状态并且生成下一阶段数据
                                List<long> idList = new List<long> { item.ProjectId };
                                string state = "";
                                if (data.status == "归档")
                                {
                                    state = PreProjectApprovalState.ApprovalSuccess.ToString();
                                }
                                else if (workLog.operateType == "退回" || data.status == "中止")
                                {
                                    state = PreProjectApprovalState.WaitSubmitted.ToString();
                                }
                                if (!string.IsNullOrEmpty(state))
                                {
                                        var result = clientService.UpdateTzProjectApprovalInfoState(idList, state);
                                    
                                }

                                
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(this, ex.Message);
                }
            });
        }
        #endregion

        #region 设计方案
        /// <summary>
        /// 设计方案审批状态同步
        /// </summary>
        public void TzDesignWorkFlowStatus()
        {

            try
            {
                var list = DataOperateBusiness<Epm_TzDesignScheme>.Get().GetList(p => !noSysStatus.Contains(p.StateName) && !string.IsNullOrEmpty(p.WorkFlowId) && p.State == (int)PreProjectApprovalState.WaitApproval).ToList();
                List<long> userIds = list.Select(p => p.CreateUserId).Distinct().ToList();
                var baseUser = DataOperateBasic<Base_User>.Get().GetList(p => userIds.Contains(p.Id)).ToList();

                WorkflowServicePortTypeClient workFlowClient = new WorkflowServicePortTypeClient();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        SysTzDesignWorkFlowStatusAsync(baseUser, item, workFlowClient);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex.Message);
            }
        }

        /// <summary>
        /// 同步流程审批状态并将最新状态更新
        /// </summary>
        /// <param name="baseUser">申请人列表</param>
        /// <param name="item">申请对象</param>
        /// <param name="workFlowClient">协同审批流接口服务</param>
        private async void SysTzDesignWorkFlowStatusAsync(List<Base_User> baseUser, Epm_TzDesignScheme item, WorkflowServicePortTypeClient workFlowClient)
        {
            await Task.Run(() =>
            {
                try
                {
                    var user = baseUser.FirstOrDefault(p => p.Id == item.CreateUserId);
                    if (user != null)
                    {
                        // 参数 1：申请 ID， 参数 2：创建人 ID， 参数 3： 流程 ID
                        var data = workFlowClient.getWorkflowRequest(Convert.ToInt32(item.WorkFlowId), Convert.ToInt32(user.ObjeId), (int)XtWorkFlowCode.WfSjfa);
                        if (data != null && data.workflowRequestLogs != null && data.workflowRequestLogs.Any())
                        {
                            var workLog = data.workflowRequestLogs.OrderByDescending(p => p.id).FirstOrDefault();
                            if (workLog.operateType != item.StateName)
                            {
                                item.StateName = workLog.operateType;
                                item.OperateTime = DateTime.Now;
                                item.ApprovalName = workLog.operatorName;
                                item.ApprovalNameId = Convert.ToInt64(workLog.operatorId);

                                DataOperateBusiness<Epm_TzDesignScheme>.Get().Update(item);

                                //修改状态并且生成下一阶段数据
                                List<long> idList = new List<long> { item.Id };
                                string state = "";
                                if (data.status == "归档")
                                {
                                    state = PreProjectApprovalState.ApprovalSuccess.ToString();
                                }
                                else if (workLog.operateType == "退回" || data.status == "中止")
                                {
                                    state = PreProjectApprovalState.WaitSubmitted.ToString();
                                }
                                if (!string.IsNullOrEmpty(state))
                                {
                                        var result = clientService.UpdateTzDesignSchemeState(idList, state);
                                    
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(this, ex.Message);
                }
            });
        }
        #endregion

        #region 施工图纸会审
        /// <summary>
        ///  施工图纸会审审批状态同步
        /// </summary>
        public void TzConDrawingWorkFlowStatus()
        {

            try
            {
                var list = DataOperateBusiness<Epm_TzConDrawing>.Get().GetList(p => !noSysStatus.Contains(p.StateName) && !string.IsNullOrEmpty(p.WorkFlowId) && p.State == (int)PreProjectApprovalState.WaitApproval).ToList();
                List<long> userIds = list.Select(p => p.CreateUserId).Distinct().ToList();
                var baseUser = DataOperateBasic<Base_User>.Get().GetList(p => userIds.Contains(p.Id)).ToList();

                WorkflowServicePortTypeClient workFlowClient = new WorkflowServicePortTypeClient();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        SysTzConDrawingWorkFlowStatusAsync(baseUser, item, workFlowClient);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex.Message);
            }
        }

        /// <summary>
        /// 同步流程审批状态并将最新状态更新
        /// </summary>
        /// <param name="baseUser">申请人列表</param>
        /// <param name="item">申请对象</param>
        /// <param name="workFlowClient">协同审批流接口服务</param>
        private async void SysTzConDrawingWorkFlowStatusAsync(List<Base_User> baseUser, Epm_TzConDrawing item, WorkflowServicePortTypeClient workFlowClient)
        {
            await Task.Run(() =>
            {
                try
                {
                    var user = baseUser.FirstOrDefault(p => p.Id == item.CreateUserId);
                    if (user != null)
                    {
                        // 参数 1：申请 ID， 参数 2：创建人 ID， 参数 3： 流程 ID
                        var data = workFlowClient.getWorkflowRequest(Convert.ToInt32(item.WorkFlowId), Convert.ToInt32(user.ObjeId), (int)XtWorkFlowCode.Wfsgtu);
                        if (data != null && data.workflowRequestLogs != null && data.workflowRequestLogs.Any())
                        {
                            var workLog = data.workflowRequestLogs.OrderByDescending(p => p.id).FirstOrDefault();
                            if (workLog.operateType != item.StateName)
                            {
                                item.StateName = workLog.operateType;
                                item.OperateTime = DateTime.Now;
                                item.ApprovalName = workLog.operatorName;
                                item.ApprovalNameId = Convert.ToInt64(workLog.operatorId);

                                DataOperateBusiness<Epm_TzConDrawing>.Get().Update(item);

                                //修改状态并且生成下一阶段数据
                                List<long> idList = new List<long> { item.Id };
                                string state = "";
                                if (data.status == "归档")
                                {
                                    state = PreProjectApprovalState.ApprovalSuccess.ToString();
                                }
                                else if (workLog.operateType == "退回" || data.status == "中止")
                                {
                                    state = PreProjectApprovalState.WaitSubmitted.ToString();
                                }
                                if (!string.IsNullOrEmpty(state))
                                {
                                        var result = clientService.UpdateTzConDrawingState(idList, state);
                                    
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(this, ex.Message);
                }
            });
        }
        #endregion

        #region 招标申请
        /// <summary>
        /// 招标申请审批状态同步
        /// </summary>
        public void TzTenderingWorkFlowStatus()
        {

            try
            {
                var list = DataOperateBusiness<Epm_TzTenderingApply>.Get().GetList(p => !noSysStatus.Contains(p.StateName) && !string.IsNullOrEmpty(p.WorkFlowId) && p.State == (int)PreProjectApprovalState.WaitApproval).ToList();
                List<long> userIds = list.Select(p => p.CreateUserId).Distinct().ToList();
                var baseUser = DataOperateBasic<Base_User>.Get().GetList(p => userIds.Contains(p.Id)).ToList();

                WorkflowServicePortTypeClient workFlowClient = new WorkflowServicePortTypeClient();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        SysTzTenderingWorkFlowStatusAsync(baseUser, item, workFlowClient);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex.Message);
            }
        }

        /// <summary>
        /// 同步流程审批状态并将最新状态更新
        /// </summary>
        /// <param name="baseUser">申请人列表</param>
        /// <param name="item">申请对象</param>
        /// <param name="workFlowClient">协同审批流接口服务</param>
        private async void SysTzTenderingWorkFlowStatusAsync(List<Base_User> baseUser, Epm_TzTenderingApply item, WorkflowServicePortTypeClient workFlowClient)
        {
            await Task.Run(() =>
            {
                try
                {
                    var user = baseUser.FirstOrDefault(p => p.Id == item.CreateUserId);
                    if (user != null)
                    {
                        // 参数 1：申请 ID， 参数 2：创建人 ID， 参数 3： 流程 ID
                        var data = workFlowClient.getWorkflowRequest(Convert.ToInt32(item.WorkFlowId), Convert.ToInt32(user.ObjeId), (int)XtWorkFlowCode.WfZbsq);
                        if (data != null && data.workflowRequestLogs != null && data.workflowRequestLogs.Any())
                        {
                            var workLog = data.workflowRequestLogs.OrderByDescending(p => p.id).FirstOrDefault();
                            if (workLog.operateType != item.StateName)
                            {
                                item.StateName = workLog.operateType;
                                item.OperateTime = DateTime.Now;
                                item.ApprovalName = workLog.operatorName;
                                item.ApprovalNameId = Convert.ToInt64(workLog.operatorId);
                                DataOperateBusiness<Epm_TzTenderingApply>.Get().Update(item);
                                //修改状态并且生成下一阶段数据
                                List<long> idList = new List<long> { item.Id };
                                string state = "";
                                if (data.status == "归档")
                                {
                                    state = PreProjectApprovalState.ApprovalSuccess.ToString();
                                }
                                else if (workLog.operateType == "退回" || data.status == "中止")
                                {
                                    state = PreProjectApprovalState.WaitSubmitted.ToString();
                                }
                                if (!string.IsNullOrEmpty(state))
                                {
                                        var result = clientService.UpdateTzTenderingApplyState(idList, state);
                                    
                                }

                                
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(this, ex.Message);
                }
            });
        }
        #endregion

        #region 招标结果
        /// <summary>
        /// 招标结果审批状态同步
        /// </summary>
        public void TzBidResultWorkFlowStatus()
        {

            try
            {
                var list = DataOperateBusiness<Epm_TzBidResult>.Get().GetList(p => !noSysStatus.Contains(p.StateName) && !string.IsNullOrEmpty(p.WorkFlowId) && p.State == (int)PreProjectApprovalState.WaitApproval).ToList();
                List<long> userIds = list.Select(p => p.CreateUserId).Distinct().ToList();
                var baseUser = DataOperateBasic<Base_User>.Get().GetList(p => userIds.Contains(p.Id)).ToList();

                WorkflowServicePortTypeClient workFlowClient = new WorkflowServicePortTypeClient();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        SysTzBidResultWorkFlowStatusAsync(baseUser, item, workFlowClient);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex.Message);
            }
        }

        /// <summary>
        /// 同步流程审批状态并将最新状态更新
        /// </summary>
        /// <param name="baseUser">申请人列表</param>
        /// <param name="item">申请对象</param>
        /// <param name="workFlowClient">协同审批流接口服务</param>
        private async void SysTzBidResultWorkFlowStatusAsync(List<Base_User> baseUser, Epm_TzBidResult item, WorkflowServicePortTypeClient workFlowClient)
        {
            await Task.Run(() =>
            {
                try
                {
                    var user = baseUser.FirstOrDefault(p => p.Id == item.CreateUserId);
                    if (user != null)
                    {
                        // 参数 1：申请 ID， 参数 2：创建人 ID， 参数 3： 流程 ID
                        var data = workFlowClient.getWorkflowRequest(Convert.ToInt32(item.WorkFlowId), Convert.ToInt32(user.ObjeId), (int)XtWorkFlowCode.WfZbjgsq);
                        if (data != null && data.workflowRequestLogs != null && data.workflowRequestLogs.Any())
                        {
                            var workLog = data.workflowRequestLogs.OrderByDescending(p => p.id).FirstOrDefault();
                            if (workLog.operateType != item.StateName)
                            {
                                item.StateName = workLog.operateType;
                                item.OperateTime = DateTime.Now;
                                item.ApprovalName = workLog.operatorName;
                                item.ApprovalNameId = Convert.ToInt64(workLog.operatorId);
                                DataOperateBusiness<Epm_TzBidResult>.Get().Update(item);
                                //修改状态并且生成下一阶段数据
                                List<long> idList = new List<long> { item.Id };
                                string state = "";
                                if (data.status == "归档")
                                {
                                    state = PreProjectApprovalState.ApprovalSuccess.ToString();
                                }
                                else if (workLog.operateType == "退回" || data.status == "中止")
                                {
                                    state = PreProjectApprovalState.WaitSubmitted.ToString();
                                }
                                if (!string.IsNullOrEmpty(state))
                                {
                                        var result = clientService.UpdateTzBidResultState(idList, state);
                                    
                                }

                                
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(this, ex.Message);
                }
            });
        }
        #endregion

        #region 项目提出
        /// <summary>
        /// 项目提出审批状态同步
        /// </summary>
        public void TzProjectProposalWorkFlowStatus()
        {

            try
            {
                var list = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetList(p => !noSysStatus.Contains(p.StateName) && !string.IsNullOrEmpty(p.WorkFlowId) && p.State == (int)PreProjectApprovalState.WaitApproval).ToList();
                List<long> userIds = list.Select(p => p.CreateUserId).Distinct().ToList();
                var baseUser = DataOperateBasic<Base_User>.Get().GetList(p => userIds.Contains(p.Id)).ToList();

                WorkflowServicePortTypeClient workFlowClient = new WorkflowServicePortTypeClient();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        SysTzProjectProposalWorkFlowStatusAsync(baseUser, item, workFlowClient);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex.Message);
            }
        }

        /// <summary>
        /// 同步流程审批状态并将最新状态更新
        /// </summary>
        /// <param name="baseUser">申请人列表</param>
        /// <param name="item">申请对象</param>
        /// <param name="workFlowClient">协同审批流接口服务</param>
        private async void SysTzProjectProposalWorkFlowStatusAsync(List<Base_User> baseUser, Epm_TzProjectProposal item, WorkflowServicePortTypeClient workFlowClient)
        {
            await Task.Run(() =>
            {
                try
                {
                    var user = baseUser.FirstOrDefault(p => p.Id == item.CreateUserId);
                    if (user != null)
                    {
                        // 参数 1：申请 ID， 参数 2：创建人 ID， 参数 3： 流程 ID
                        var data = workFlowClient.getWorkflowRequest(Convert.ToInt32(item.WorkFlowId), Convert.ToInt32(user.ObjeId), (int)XtWorkFlowCode.WfXmtcsq);
                        if (data != null && data.workflowRequestLogs != null && data.workflowRequestLogs.Any())
                        {
                            var workLog = data.workflowRequestLogs.OrderByDescending(p => p.id).FirstOrDefault();
                            if (workLog.operateType != item.StateName)
                            {
                                item.StateName = workLog.operateType;
                                item.OperateTime = DateTime.Now;
                                item.ApprovalName = workLog.operatorName;
                                item.ApprovalNameId = Convert.ToInt64(workLog.operatorId);
                                DataOperateBusiness<Epm_TzProjectProposal>.Get().Update(item);

                                //修改状态并且生成下一阶段数据
                                List<long> idList = new List<long> { item.Id };
                                string state = "";
                                if (data.status == "归档")
                                {
                                    state = PreProjectApprovalState.ApprovalSuccess.ToString();
                                }
                                else if (workLog.operateType == "退回" || data.status == "中止")
                                {
                                    state = PreProjectApprovalState.WaitSubmitted.ToString();
                                }
                                if (!string.IsNullOrEmpty(state))
                                {
                                    var result = clientService.UpdateTzProjectProposalState(idList, state);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(this, ex.Message);
                }
            });
        }
        #endregion

        #region 开工申请
        /// <summary>
        /// 开工申请审批状态同步
        /// </summary>
        public void TzProjectStartApplyWorkFlowStatus()
        {

            try
            {
                var list = DataOperateBusiness<Epm_TzProjectStartApply>.Get().GetList(p => !noSysStatus.Contains(p.StateName) && !string.IsNullOrEmpty(p.WorkFlowId) && p.State == (int)PreProjectApprovalState.WaitApproval).ToList();
                List<long> userIds = list.Select(p => p.CreateUserId).Distinct().ToList();
                var baseUser = DataOperateBasic<Base_User>.Get().GetList(p => userIds.Contains(p.Id)).ToList();

                WorkflowServicePortTypeClient workFlowClient = new WorkflowServicePortTypeClient();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        SysTzProjectStartApplyWorkFlowStatusAsync(baseUser, item, workFlowClient);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex.Message);
            }
        }

        /// <summary>
        /// 同步流程审批状态并将最新状态更新
        /// </summary>
        /// <param name="baseUser">申请人列表</param>
        /// <param name="item">申请对象</param>
        /// <param name="workFlowClient">协同审批流接口服务</param>
        private async void SysTzProjectStartApplyWorkFlowStatusAsync(List<Base_User> baseUser, Epm_TzProjectStartApply item, WorkflowServicePortTypeClient workFlowClient)
        {
            await Task.Run(() =>
            {
                try
                {
                    var user = baseUser.FirstOrDefault(p => p.Id == item.CreateUserId);
                    if (user != null)
                    {
                        // 参数 1：申请 ID， 参数 2：创建人 ID， 参数 3： 流程 ID
                        var data = workFlowClient.getWorkflowRequest(Convert.ToInt32(item.WorkFlowId), Convert.ToInt32(user.ObjeId), (int)XtWorkFlowCode.Wfkgsq);
                        if (data != null && data.workflowRequestLogs != null && data.workflowRequestLogs.Any())
                        {
                            var workLog = data.workflowRequestLogs.OrderByDescending(p => p.id).FirstOrDefault();
                            if (workLog.operateType != item.StateName)
                            {
                                item.StateName = workLog.operateType;
                                item.OperateTime = DateTime.Now;
                                item.ApprovalName = workLog.operatorName;
                                item.ApprovalNameId = Convert.ToInt64(workLog.operatorId);

                                DataOperateBusiness<Epm_TzProjectStartApply>.Get().Update(item);

                                //修改状态并且生成下一阶段数据
                                List<long> idList = new List<long> { item.Id };
                                string state = "";
                                if (data.status == "归档")
                                {
                                    state = PreProjectApprovalState.ApprovalSuccess.ToString();
                                }
                                else if (workLog.operateType == "退回" || data.status == "中止")
                                {
                                    state = PreProjectApprovalState.WaitSubmitted.ToString();
                                }
                                if (!string.IsNullOrEmpty(state))
                                {
                                        var result = clientService.UpdateTzProjectStartApplyState(idList, state);
                                    
                                }

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(this, ex.Message);
                }
            });
        }
        #endregion

        #region 甲供物资申请
        /// <summary>
        /// 甲供物资申请谈判审批状态同步
        /// </summary>
        public void TzSupplyMaterialApplyWorkFlowStatus()
        {

            try
            {
                var list = DataOperateBusiness<Epm_TzSupplyMaterialApply>.Get().GetList(p => !noSysStatus.Contains(p.StateName) && !string.IsNullOrEmpty(p.WorkFlowId) && p.State == (int)PreProjectApprovalState.WaitApproval).ToList();
                List<long> userIds = list.Select(p => p.CreateUserId).Distinct().ToList();
                var baseUser = DataOperateBasic<Base_User>.Get().GetList(p => userIds.Contains(p.Id)).ToList();

                WorkflowServicePortTypeClient workFlowClient = new WorkflowServicePortTypeClient();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        SysTzSupplyMaterialApplyWorkFlowStatusAsync(baseUser, item, workFlowClient);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex.Message);
            }
        }

        /// <summary>
        /// 同步流程审批状态并将最新状态更新
        /// </summary>
        /// <param name="baseUser">申请人列表</param>
        /// <param name="item">申请对象</param>
        /// <param name="workFlowClient">协同审批流接口服务</param>
        private async void SysTzSupplyMaterialApplyWorkFlowStatusAsync(List<Base_User> baseUser, Epm_TzSupplyMaterialApply item, WorkflowServicePortTypeClient workFlowClient)
        {
            await Task.Run(() =>
            {
                try
                {
                    var user = baseUser.FirstOrDefault(p => p.Id == item.CreateUserId);
                    if (user != null)
                    {
                        // 参数 1：申请 ID， 参数 2：创建人 ID， 参数 3： 流程 ID
                        var data = workFlowClient.getWorkflowRequest(Convert.ToInt32(item.WorkFlowId), Convert.ToInt32(user.ObjeId), (int)XtWorkFlowCode.WfJgwzsq);
                        if (data != null && data.workflowRequestLogs != null && data.workflowRequestLogs.Any())
                        {
                            var workLog = data.workflowRequestLogs.OrderByDescending(p => p.id).FirstOrDefault();
                            if (workLog.operateType != item.StateName)
                            {
                                item.StateName = workLog.operateType;
                                item.OperateTime = DateTime.Now;
                                item.ApprovalName = workLog.operatorName;
                                item.ApprovalNameId = Convert.ToInt64(workLog.operatorId);
                                DataOperateBusiness<Epm_TzSupplyMaterialApply>.Get().Update(item);
                                //修改状态并且生成下一阶段数据
                                List<long> idList = new List<long> { item.Id };
                                string state = "";
                                if (data.status == "归档")
                                {
                                    state = PreProjectApprovalState.ApprovalSuccess.ToString();
                                }
                                else if (workLog.operateType == "退回" || data.status == "中止")
                                {
                                    state = PreProjectApprovalState.WaitSubmitted.ToString();
                                }
                                if (!string.IsNullOrEmpty(state))
                                {
                                        var result = clientService.UpdateTzSupplyMaterialApplyState(idList, state);
                                }

                                
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(this, ex.Message);
                }
            });
        }
        #endregion

        #region 库站改造
        /// <summary>
        /// 库站改造审批状态同步
        /// </summary>
        public void ReformRecordWorkFlowStatus()
        {

            try
            {
                var list = DataOperateBusiness<Epm_ReformRecord>.Get().GetList(p => !noSysStatus.Contains(p.StateName) && !string.IsNullOrEmpty(p.WorkFlowId) && p.State == (int)PreProjectApprovalState.WaitApproval).ToList();
                List<long> userIds = list.Select(p => p.CreateUserId).Distinct().ToList();
                var baseUser = DataOperateBasic<Base_User>.Get().GetList(p => userIds.Contains(p.Id)).ToList();

                WorkflowServicePortTypeClient workFlowClient = new WorkflowServicePortTypeClient();
                if (list.Any())
                {
                    foreach (var item in list)
                    {
                        SysReformRecordWorkFlowStatusAsync(baseUser, item, workFlowClient);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(this, ex.Message);
            }
        }

        /// <summary>
        /// 同步流程审批状态并将最新状态更新
        /// </summary>
        /// <param name="baseUser">申请人列表</param>
        /// <param name="item">申请对象</param>
        /// <param name="workFlowClient">协同审批流接口服务</param>
        private async void SysReformRecordWorkFlowStatusAsync(List<Base_User> baseUser, Epm_ReformRecord item, WorkflowServicePortTypeClient workFlowClient)
        {
            await Task.Run(() =>
            {
                try
                {
                    var user = baseUser.FirstOrDefault(p => p.Id == item.CreateUserId);
                    if (user != null)
                    {
                        // 参数 1：申请 ID， 参数 2：创建人 ID， 参数 3： 流程 ID
                        var data = workFlowClient.getWorkflowRequest(Convert.ToInt32(item.WorkFlowId), Convert.ToInt32(user.ObjeId), (int)XtWorkFlowCode.WfTdcrxitpsq);
                        if (data != null && data.workflowRequestLogs != null && data.workflowRequestLogs.Any())
                        {
                            var workLog = data.workflowRequestLogs.OrderByDescending(p => p.id).FirstOrDefault();
                            if (workLog.operateType != item.StateName)
                            {
                                item.StateName = workLog.operateType;
                                item.OperateTime = DateTime.Now;
                                item.ApprovalName = workLog.operatorName;
                                item.ApprovalNameId = Convert.ToInt64(workLog.operatorId);

                                DataOperateBusiness<Epm_ReformRecord>.Get().Update(item);

                                //修改状态并且生成下一阶段数据
                                List<long> idList = new List<long> { item.Id };
                                string state = "";
                                if (data.status == "归档")
                                {
                                    state = PreProjectApprovalState.ApprovalSuccess.ToString();
                                }
                                else if (workLog.operateType == "退回" || data.status == "中止")
                                {
                                    state = PreProjectApprovalState.WaitSubmitted.ToString();
                                }
                                if (!string.IsNullOrEmpty(state))
                                {
                                        var result = clientService.UpdateReformRecordState(idList, state);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(this, ex.Message);
                }
            });
        }
        #endregion

        #endregion

        #region 生成周报
        public void CreateWeekly()
        {
            var time = DateTime.Now.AddDays(-7).ToString("D") + '~' + DateTime.Now.ToString("D");
            clientService.AddProjectWeekly(time);
            clientService.AddProjectCountWeekly(time);
        }
        #endregion

        #region 同步供应商信息
        //public void SupplierInfo()
        //{
        //    try
        //    {
        //        var parentSupplier = DataOperateBasic<Base_Company>.Get().Single(p => p.Type == "Supplier" || p.Type == "SSGYS");
        //        if (parentSupplier != null && parentSupplier.Any())
        //        {
        //            var supplierInfo = client.getSupplierList(UrlIp);
        //            if (parentSupplier == null)
        //            {
        //                return;
        //            }
        //            foreach (var item in parentSupplier)
        //            {
        //                for (int i = 0; i < supplierInfo.Count; i++)
        //                {
        //                    var supplier = DataOperateBasic<Base_Company>.Get().Single(p => p.Id == supplierInfo[i].Id);
        //                    bool isAdd = false;
        //                    if (supplier == null)
        //                    {
        //                        deleteSupplier(UrlIp, supplierInfo[i].Id);
        //                    }
        //                    else if(item.Id == supplierInfo[i].Id)
        //                    {
        //                        isAdd = true;
        //                        supplier = new SupplierBean();
        //                        supplier.Id = parentSupplier.Id;
        //                        supplier.PId = parentSupplier.PId;
        //                        supplier.PreCode = parentSupplier.PreCode;
        //                        supplier.PreName = parentSupplier.PreName;
        //                        supplier.Code = parentSupplier.Code;
        //                        supplier.Name = parentSupplier.Name;
        //                        supplier.ShortName = parentSupplier.ShortName;
        //                        supplier.Type = parentSupplier.Type;
        //                        supplier.Phone = parentSupplier.Phone;
        //                        supplier.Email = parentSupplier.Email;
        //                        supplier.LinkManId = parentSupplier.LinkManId;
        //                        supplier.LinkMan = parentSupplier.LinkMan;
        //                        supplier.LinkPhone = parentSupplier.LinkPhone;
        //                        supplier.Address = parentSupplier.Address;
        //                        supplier.AddressInfo = parentSupplier.AddressInfo;
        //                        supplier.Remark = parentSupplier.Remark;
        //                        supplier.FaxPhone = parentSupplier.FaxPhone;
        //                        supplier.OrgType = parentSupplier.OrgType;
        //                        supplier.CompanyType = parentSupplier.CompanyType;
        //                        supplier.OrgState = parentSupplier.OrgState;
        //                        supplier.CompanyState = parentSupplier.CompanyState;
        //                        supplier.ObjeId = parentSupplier.ObjeId;
        //                        supplier.Normal_absence = parentSupplier.Normal_absence;
        //                        supplier.Abnormality = parentSupplier.Abnormality;
        //                        supplier.CreateUserId = 0;
        //                        supplier.CreateUserName = "系统定时同步";
        //                        supplier.CreateTime = DateTime.Now;
        //                        supplier.IsDelete = false;
        //                        supplier.OperateUserId = 0;
        //                        supplier.OperateUserName = "系统定时同步";
        //                        supplier.OperateTime = DateTime.Now;
        //                    }
        //                    else
        //                    {
        //                        isAdd = false;
        //                    }
        //                }
        //                if (isAdd)
        //                {
        //                    addSupplier(UrlIp, supplier);
        //                }
        //                else
        //                {
        //                    updateSupplier(UrlIp, supplier);
        //                }

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Error(this, ex.Message);
        //    }
        //}
        #endregion
        #region 同步供应商人员信息
        //public void SupplierPersonnel()
        //{
        //    try
        //    {
        //        var parentPersonnel = DataOperateBasic<Base_User>.Get();
        //        if (parentPersonnel != null)
        //        {
        //            var personnel = client.getSupplierPersonnelList(UrlIp);
        //            if (parentPersonnel == null)
        //            {
        //                return;
        //            }
        //            foreach (var item in parentPersonnel)
        //            {
        //                for (int i = 0; i < personnel.Count; i++)
        //                {
        //                    var per = DataOperateBasic<Base_User>.Get().Single(p => p.Id == personnel[i].Id);
        //                    bool isAdd = false;
        //                    if (per == null)
        //                    {
        //                        deleteSupplierPersonnel(UrlIp, personnel[i].Id);
        //                    }
        //                    else if (item.Id == personnel[i].Id)
        //                    {
        //                        isAdd = true;
        //                        per = new SupplierPersonnelBean();
        //                        per.Id = item.Id;
        //                        per.UserName = item.UserName;
        //                        per.UserAcct = item.UserAcct;
        //                        per.UserCode = item.UserCode;
        //                        per.PassWord = item.PassWord;
        //                        per.Phone = item.Phone;
        //                        per.WebChat = item.WebChat;
        //                        per.QQ = item.QQ;
        //                        per.Sex = item.Sex;
        //                        per.Email = item.Email;
        //                        per.IsLock = item.IsLock;
        //                        per.LastLoginTime = item.LastLoginTime;
        //                        per.CompanyId = item.CompanyId;
        //                        per.LockNum = item.LockNum;
        //                        per.PassTime = item.PassTime;
        //                        per.AndroidToken = item.AndroidToken;
        //                        per.AndroidTokenTime = item.AndroidTokenTime;
        //                        per.IosToken = item.IosToken;
        //                        per.IosTokenTime = item.IosTokenTime;
        //                        per.BirthDate = item.BirthDate;
        //                        per.University = item.University;
        //                        per.Major = item.Major;
        //                        per.Education = item.Education;
        //                        per.Address = item.Address;
        //                        per.OccupationalStartTime = item.OccupationalStartTime;
        //                        per.OccupationalContent = item.OccupationalContent;
        //                        per.Professional = item.Professional;
        //                        per.ProfessionalValue = item.ProfessionalValue;
        //                        per.Post = item.Post;
        //                        per.PostValue = item.PostValue;
        //                        per.ProfessionalQualification = item.ProfessionalQualification;
        //                        per.ProfessionalQualificationValue = item.ProfessionalQualificationValue;
        //                        per.ObjeId = item.ObjeId;
        //                        per.Card = item.Card;
        //                        per.DepartmentId = item.DepartmentId;
        //                        per.CreateUserId = 0;
        //                        per.CreateUserName = "系统定时同步";
        //                        per.CreateTime = DateTime.Now;
        //                        per.IsDelete = false;
        //                        per.OperateUserId = 0;
        //                        per.OperateUserName = "系统定时同步";
        //                        per.OperateTime = DateTime.Now;
        //                    }
        //                    else
        //                    {
        //                        isAdd = false;
        //                    }
        //                }
        //                if (isAdd)
        //                {
        //                    addSupplierPersonnel(UrlIp, per);
        //                }
        //                else
        //                {
        //                    updateSupplierPersonnel(UrlIp, per);
        //                }

        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogHelper.Error(this, ex.Message);
        //    }
        //}
        #endregion
    }
}
