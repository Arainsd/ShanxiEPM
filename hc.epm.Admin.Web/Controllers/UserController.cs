using hc.epm.Admin.ClientProxy;
using hc.epm.DataModel.Basic;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using hc.epm.Common;
using hc.epm.UI.Common;
using hc.Plat.Common.Extend;
using hc.epm.ViewModel;
using Newtonsoft.Json;
using hc.epm.DataModel.Business;
using System.Drawing;
using System.IO;

namespace hc.epm.Admin.Web.Controllers
{
    public class UserController : BaseController
    {
        /// <summary>
        /// 根据belong加载企业列表信息
        /// 用户管理基于当前企业
        /// </summary>
        /// <param name="belong"></param>
        /// <param name="name"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        //    public ActionResult CompanyList(string belong, string name = "", int pageIndex = 1, int pageSize = 10)
        //    {
        //        ViewBag.name = name;
        //        ViewBag.belong = belong;
        //        ViewBag.pageIndex = pageIndex;
        //        ViewBag.belongText = belong.ToEnumReq<RoleType>().GetText();
        //        if (string.IsNullOrEmpty(belong))
        //        {
        //            return RedirectToAction("Error", "Home", new { msg = "必须有用户归属" });
        //        }
        //        ConditionExpression ce = null;
        //        QueryCondition qc = new QueryCondition();
        //        ce = new ConditionExpression();
        //        ce.ExpName = "Type";
        //        ce.ExpValue = "%" + belong + "%";
        //        ce.ExpOperater = eConditionOperator.Like;
        //        ce.ExpLogical = eLogicalOperator.And;
        //        qc.ConditionList.Add(ce);
        //        if (!string.IsNullOrEmpty(name))
        //        {
        //            ce = new ConditionExpression();
        //            ce.ExpName = "Name";
        //            ce.ExpValue = "%" + name + "%";
        //            ce.ExpOperater = eConditionOperator.Like;
        //            ce.ExpLogical = eLogicalOperator.And;
        //            qc.ConditionList.Add(ce);
        //        }
        //        SortExpression sort = new SortExpression("PreCode", eSortType.Asc);
        //        qc.SortList.Add(sort);
        //        qc.PageInfo = GetPageInfo(pageIndex, pageSize);
        //        Result<List<Base_Company>> result = new Result<List<Base_Company>>();
        //        using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
        //        {
        //            result = proxy.GetCompanyList(qc);
        //            ViewBag.Total = result.AllRowsCount;
        //        }
        //        return View(result.Data);
        //    }

        public ActionResult Index(string userName = "", string type = "", string CompanyName = "", int pageIndex = 1, int pageSize = 10)
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminUserManager.ToString(), SystemRight.Browse.ToString(), true);
            ViewBag.UserName = userName;
            ViewBag.CompanyName = CompanyName;
            ViewBag.type = typeof(RoleTypes).AsSelectList(true, type);
            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();
            if (!string.IsNullOrEmpty(userName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "UserName";
                ce.ExpValue = userName;
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(CompanyName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "CompanyName";
                ce.ExpValue = "%" + CompanyName + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            if (!string.IsNullOrEmpty(type))
            {
                ce = new ConditionExpression();
                ce.ExpName = "type";
                ce.ExpValue = "%" + type + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            SortExpression sort = new SortExpression("type", eSortType.Desc);
            qc.SortList.Add(sort);
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<UserListView>> result = new Result<List<UserListView>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetUserCompanyList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.pageIndex = pageIndex;
            }
            return View(result.Data);
        }

        //private void GetCompanyInfoList()
        //{
        //    ConditionExpression ce = null;
        //    QueryCondition qc = new QueryCondition();
        //    ce = new ConditionExpression();
        //    ce.ExpName = "PreCode";
        //    ce.ExpValue = "10";
        //    ce.ExpOperater = eConditionOperator.Equal;
        //    ce.ExpLogical = eLogicalOperator.And;
        //    qc.ConditionList.Add(ce);

        //    ce = new ConditionExpression();
        //    ce.ExpName = "Type";
        //    ce.ExpValue = "Owner";
        //    ce.ExpOperater = eConditionOperator.NotEqual;
        //    ce.ExpLogical = eLogicalOperator.Or;
        //    qc.ConditionList.Add(ce);

        //    SortExpression sort = new SortExpression("PreCode", eSortType.Asc);
        //    qc.SortList.Add(sort);

        //    qc.PageInfo.isAllowPage = false;
        //    Result<List<Base_Company>> result = new Result<List<Base_Company>>();
        //    using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
        //    {
        //        result = proxy.GetCompanyList(qc);
        //        ViewBag.CompanyId = result.Data.ToSelectList("Name", "Id", true);
        //    }
        //}

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <returns></returns>
        public ActionResult Add()
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminUserManager.ToString(), SystemRight.Add.ToString(), true);
            ViewBag.currentTime = System.DateTime.Now;
            ViewBag.Code = SnowflakeHelper.GetID;
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                List<DictionaryType> dic = new List<DictionaryType>() { DictionaryType.ProfessionalType, DictionaryType.PostType, DictionaryType.QualificationType };
                var diclist = proxy.GetTypeListByTypes(dic).Data;

                ViewBag.Professional = diclist[DictionaryType.ProfessionalType].ToSelectList("Name", "No", true);
                ViewBag.Post = diclist[DictionaryType.PostType].ToSelectList("Name", "No", true);
                ViewBag.ProfessionalQualification = diclist[DictionaryType.QualificationType].ToSelectList("Name", "No", true);
            }
            return View();
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(Base_User model)
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminUserManager.ToString(), SystemRight.Add.ToString(), true);

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符串序列化为列表

            Result<int> result = new Result<int>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                model.PassWord = DesTool.DesEncrypt("123456");//用户密码加密
                result = proxy.AddUser(model, fileList);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 编辑用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(long id)
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminUserManager.ToString(), SystemRight.Modify.ToString(), true);
            ViewBag.currentTime = System.DateTime.Now;
            Result<Base_User> result = new Result<Base_User>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetUserModel(id);

                //ViewBag.CompanyName = "本省销售公司";
                if (result.Data.CompanyId != 10)
                {
                    var company = proxy.GetCompanyModel(result.Data.CompanyId).Data;
                    //if (company != null && company.Type == RoleType.Supplier.ToString())
                    //{ }
                    if (company != null)
                    {
                        ViewBag.CompanyName = company.Name;
                    }
                }

                List<DictionaryType> dic = new List<DictionaryType>() { DictionaryType.ProfessionalType, DictionaryType.PostType, DictionaryType.QualificationType };
                var diclist = proxy.GetTypeListByTypes(dic).Data;

                ViewBag.Professional = diclist[DictionaryType.ProfessionalType].ToSelectList("Name", "No", true, result.Data.Professional);
                ViewBag.Post = diclist[DictionaryType.PostType].ToSelectList("Name", "No", true, result.Data.Post);
                ViewBag.ProfessionalQualification = diclist[DictionaryType.QualificationType].ToSelectList("Name", "No", true, result.Data.ProfessionalQualification);
            }
            //GetCompanyInfoList();
            return View(result.Data);
        }

        /// <summary>
        /// 编辑用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(Base_User model)
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminUserManager.ToString(), SystemRight.Modify.ToString(), true);

            ResultView<int> view = new ResultView<int>();
            if (string.IsNullOrEmpty(model.UserCode))
            {
                view.Flag = false;
                view.Message = "用户编码不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.UserAcct))
            {
                view.Flag = false;
                view.Message = "登录账号不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.UserName))
            {
                view.Flag = false;
                view.Message = "用户名称不能为空";
                return Json(view);
            }

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符串序列化为列表

            Result<int> result = new Result<int>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                var oldData = proxy.GetUserModel(model.Id).Data;
                oldData.UserName = model.UserName;
                oldData.UserAcct = model.UserAcct;
                oldData.Sex = model.Sex;
                oldData.UserCode = model.UserCode;
                oldData.Email = model.Email;
                oldData.Phone = model.Phone;
                oldData.QQ = model.QQ;
                oldData.Post = model.Post;//岗位
                oldData.PostValue = model.PostValue;//岗位值
                oldData.ProfessionalValue = model.ProfessionalValue;//职称
                oldData.Education = model.Education;//学历
                oldData.CompanyId = model.CompanyId;//单位
                oldData.Email = model.Email;//邮箱
                oldData.BirthDate = model.BirthDate;//生日
                oldData.University = model.University;
                oldData.Major = model.Major;//专业名称
                oldData.Address = model.Address;//地址
                oldData.ProfessionalQualificationValue = oldData.ProfessionalQualificationValue;//资质
                oldData.OccupationalStartTime = model.OccupationalStartTime;//职业日期
                oldData.OccupationalContent = model.OccupationalContent;//职业简述
                result = proxy.UpdateUser(oldData, fileList);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(long id)
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminUserManager.ToString(), SystemRight.Info.ToString(), true);
            Result<Base_User> result = new Result<Base_User>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetUserModel(id);

                ViewBag.CompanyName = "本省销售公司";
                if (result.Data.CompanyId != 10)
                {
                    var company = proxy.GetCompanyModel(result.Data.CompanyId).Data;
                    if (company != null && company.Type == RoleType.Supplier.ToString())
                    {
                        ViewBag.CompanyName = company.Name;
                    }
                }

                if (string.IsNullOrEmpty(result.Data.Address))
                {
                    ViewBag.Address = "";
                }
                else {
                    var code = result.Data.Address.Split(',')[2];
                    ViewBag.Address = proxy.GetRegionModel(code).Data.Fullname;
                }
            }
            return View(result.Data);
        }

        /// <summary>
        /// 改变锁定状态
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditState(long id)
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminUserManager.ToString(), SystemRight.Lock.ToString(), true);

            Result<int> result = new Result<int>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.AuditUser(id);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 批量删除用户
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminUserManager.ToString(), SystemRight.Delete.ToString(), true);

            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteUserByIds(list);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 批量设置角色
        /// </summary>
        /// <param name="belong"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SetRoles(RoleType belong, string id)
        {
            Result<List<Base_Role>> result = new Result<List<Base_Role>>();
            Result<int> userRoles = new Result<int>();
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminUserManager.ToString(), SystemRight.Delete.ToString(), true);
            List<long> list = id.SplitString(",").ToLongList();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetRoleListByBelong(belong);//获取当前用户类型下的角色列表
                //if (!string.IsNullOrEmpty(id))
                //{
                //    userRoles = proxy.GetRolesByUserIds(list);//加载当前用户所具有的角色
                //}
            }
            return View(result.Data);
        }
        public ActionResult SetRoleFuns(string id, RoleType belong, string ids)
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminUserManager.ToString(), SystemRight.SetRole.ToString(), true);

            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();//角色列表
            List<long> userList = id.SplitString(",").ToLongList();//用户列表
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                for (int i = 0; i < userList.Count; i++)
                {
                    result = proxy.SetUserRole(userList[i], list, belong);
                }
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 用户设置角色
        /// </summary>
        /// <param name="belong"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SetRole(RoleType belong, string id, int type = 1)
        {
            //权限检查
            //Helper.IsCheck(HttpContext, AdminModule.AdminUserManager.ToString(), SystemRight.SetRole.ToString(), true);
            ViewBag.Belong = belong;
            ViewBag.Id = id;
            ViewBag.userRoles = new List<Base_UserRole>();

            //Result<List<Base_Role>> result = new Result<List<Base_Role>>();
            //List<Base_UserRole> userRoles = new List<Base_UserRole>();
            //using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            //{
            //    result = proxy.GetRoleListByBelong(belong);//获取当前用户类型下的角色列表
            //    userRoles = proxy.GetRolesByUserId(id).Data;//加载当前用户所具有的角色
            //    ViewBag.userRoles = userRoles.Select(i => i.RoleId).ToList();
            //}
            //return View(result.Data);
            Result<List<Base_Role>> result = new Result<List<Base_Role>>();
            Result<int> userRoles = new Result<int>();
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminUserManager.ToString(), SystemRight.Delete.ToString(), true);
            List<long> list = id.SplitString(",").ToLongList();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                if (type != 1)
                {
                    var userjiaose = proxy.GetRolesByUserId(id.ToLongReq());
                    if (userjiaose.Data.Any())
                    {
                        ViewBag.userRoles = userjiaose.Data.Select(i => i.RoleId).ToList();
                    }
                }
                result = proxy.GetRoleListByBelong(belong);//获取当前用户类型下的角色列表
            }
            return View(result.Data);
        }
        public ActionResult SetRoleFun(string id, RoleType belong, string ids)
        {
            //权限检查
            //Helper.IsCheck(HttpContext, AdminModule.AdminUserManager.ToString(), SystemRight.SetRole.ToString(), true);

            //Result<int> result = new Result<int>();
            //List<long> list = ids.SplitString(",").ToLongList();//角色列表
            //using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            //{
            //    result = proxy.SetUserRole(id, list, belong);
            //}
            //return Json(result.ToResultView());
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.AdminUserManager.ToString(), SystemRight.SetRole.ToString(), true);

            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();//角色列表
            List<long> userList = id.SplitString(",").ToLongList();//用户列表
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                for (int i = 0; i < userList.Count; i++)
                {
                    result = proxy.SetUserRole(userList[i], list, belong);
                }
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.PasswordManager, Right = SystemRight.Modify)]
        public ActionResult ChangePWD()
        {
            ViewBag.userName = CurrentUser.UserName;
            ViewBag.userId = CurrentUser.UserId;
            return View();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oldPWD"></param>
        /// <param name="newPWD"></param>
        /// <param name="confirmPWD"></param>
        /// <returns></returns>
        public ActionResult ChangePasswod(string oldPWD, string newPWD, string confirmPWD)
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.PasswordManager.ToString(), SystemRight.Modify.ToString(), true);

            oldPWD = DesTool.DesEncrypt(oldPWD);

            ResultView<int> view = new ResultView<int>();
            if (newPWD != confirmPWD)
            {
                view.Flag = false;
                view.Message = "请重新确认密码！";
                return Json(view);
            }
            Result<Base_User> userResult = new Result<Base_User>();
            Result<int> result = new Result<int>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                var id = CurrentUser.UserId;
                userResult = proxy.GetUserModel(id);
                if (userResult.Data.PassWord != oldPWD)
                {
                    view.Flag = false;
                    view.Message = "原密码输入错误！";
                    return Json(view);
                }
                else
                {
                    result = proxy.UpdateUserPassword(id, DesTool.DesEncrypt(newPWD));
                }
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改个人资料
        /// </summary>
        /// <returns></returns>
        [AuthCheck(Module = AdminModule.EditUserInfo, Right = SystemRight.Modify)]
        public ActionResult EditInformation()
        {
            Result<Base_User> result = new Result<Base_User>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetUserModel(CurrentUser.UserId);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 修改个人资料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditInformation(Base_User model)
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.EditUserInfo.ToString(), SystemRight.Modify.ToString(), true);

            Result<int> result = new Result<int>();
            Result<Base_User> userResult = new Result<Base_User>();
            ResultView<int> view = new ResultView<int>();
            if (string.IsNullOrEmpty(model.UserName))
            {
                view.Flag = false;
                view.Message = "用户名不能为空";
                return Json(view);
            }
            //if (string.IsNullOrEmpty(model.Email))
            //{
            //    view.Flag = false;
            //    view.Message = "电子邮箱不能为空";
            //    return Json(view);
            //}
            if (string.IsNullOrEmpty(model.Phone))
            {
                view.Flag = false;
                view.Message = "电话号码不能为空";
                return Json(view);
            }

            string fileDataJson = Request.Form["fileDataJson"];//获取上传文件json字符串
            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符串序列化为列表

            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                model.Id = CurrentUser.UserId;
                userResult = proxy.GetUserModel(CurrentUser.UserId);
                var userInfo = userResult.Data;
                userInfo.UserName = model.UserName;
                userInfo.Email = model.Email;
                userInfo.UserCode = model.UserCode;
                userInfo.Phone = model.Phone;
                userInfo.Sex = model.Sex;
                userInfo.QQ = model.QQ;
                result = proxy.UpdateUser(userInfo, fileList);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改资料界面根据companyID获取companyName
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public ActionResult GetCompanyName(long id)
        //{
        //    Result<Base_Company> result = new Result<Base_Company>();
        //    using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
        //    {
        //        result = proxy.GetCompanyModel(id);
        //    }
        //    return Json(result);
        //}

        /// <summary>
        /// 修改资料界面根据DepID获取DepName
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public ActionResult GetDepName(long id)
        //{
        //    Result<Base_Dep> result = new Result<Base_Dep>();
        //    using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
        //    {
        //        result = proxy.GeDepModel(id);
        //    }
        //    return Json(result);
        //}

        /// <summary>
        /// 全部用户检索
        /// </summary>
        /// <returns></returns>
        public ActionResult UserRetrieve(string name, int pageIndex = 1, int pageSize = 10, string isLock = "0")
        {
            //权限检查
            Helper.IsCheck(HttpContext, AdminModule.UserRetrieve.ToString(), SystemRight.Browse.ToString(), true);
            ViewBag.name = name;
            ViewBag.IsLock = Enum<EnumState>.AsEnumerable().Where(i => i == EnumState.Lock || i == EnumState.NoLock).ToDictionary(i => i.ToString(), j => j.GetText()).ToList().ToSelectList("Value", "Key", true);
            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();

            if (!string.IsNullOrEmpty(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "UserName";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (isLock != "0")
            {
                ce = new ConditionExpression();
                ce.ExpName = "IsLock";
                ce.ExpValue = isLock == EnumState.Lock.ToString(); ;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            Result<List<Base_User>> result = new Result<List<Base_User>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetUserList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.pageIndex = pageIndex;
            }
            return View(result.Data);
        }


        //选择单位
        public ActionResult SelectCompany(string name = "", string type = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.type = typeof(RoleTypes).AsSelectList(true, type);
            QueryCondition qc = new QueryCondition();

            ConditionExpression ce = new ConditionExpression();

            if (!string.IsNullOrEmpty(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Name";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(type))
            {
                ce = new ConditionExpression();
                ce.ExpName = "type";
                ce.ExpValue = "%" + type + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            SortExpression se = new SortExpression();
            se.SortName = "Code";
            se.SortType = eSortType.Asc;
            qc.SortList.Add(se);


            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<Base_Company>> result = new Result<List<Base_Company>>();

            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompanyList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.pageIndex = pageIndex;
            }
            return View(result.Data);
        }

        /// <summary>
        /// 注册人脸页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddUserFace(long userId)
        {
            Result<EPM_AIUserFace> result = new Result<EPM_AIUserFace>();
            ViewBag.UserName = "";
            ViewBag.UserId = userId;
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                var userResult = proxy.GetUserModel(userId);
                if (userResult.Flag == EResultFlag.Success && userResult.Data != null)
                {
                    ViewBag.UserName = userResult.Data.UserName;
                }
                result = proxy.GetAIUserFaceByUserId(userId);
            }

            return View(result.Data);
        }

        [HttpPost]
        public ActionResult AddUserFace()
        {
            Result<int> result = new Result<int>();
            ResultView<string> view = new ResultView<string>();
            long userId = Request.Form["userId"].ToLongReq();
            string image = Request.Form["ImageInfo"];
            Bitmap bmp = new Bitmap(image);

            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] arr = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(arr, 0, (int)ms.Length);
            ms.Close();
            image = Convert.ToBase64String(arr);

            if (string.IsNullOrEmpty(image))
            {
                view.Flag = false;
                view.Message = "请上传图片！";
                return Json(view);
            }

            string source = RoleType.Admin.ToString();

            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddAIUserFaceInfo(userId, image, source);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 人脸库
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetAIUserFaceList(string name, int pageIndex = 1, int pageSize = 10)
        {
            Result<List<EPM_AIUserFace>> result = new Result<List<EPM_AIUserFace>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                QueryCondition qc = new QueryCondition();
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
                ConditionExpression ce = new ConditionExpression();
                if (!string.IsNullOrEmpty(name))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "userName";
                    ce.ExpValue = "%" + name + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                result = proxy.GetAIUserFaceList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.pageIndex = pageIndex;
            }
            return View(result.Data);
        }

        /// <summary>
        /// 人脸库详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetAIUserFaceDetails(long id)
        {
            Result<EPM_AIUserFace> result = new Result<EPM_AIUserFace>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetAIUserFaceModel(id);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 删除人脸信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public ActionResult DelUserFace(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteAIUserFaceByIds(list);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 考勤记录列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="projectName"></param>
        /// <param name="oilName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetSignInfoList(string name, string projectName, string oilName, string time,string endtime, int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.projectName = projectName;
            ViewBag.oilName = oilName;
            ViewBag.time = time;
            Result<List<Epm_SignInformation>> result = new Result<List<Epm_SignInformation>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                QueryCondition qc = new QueryCondition();
                ConditionExpression ce = new ConditionExpression();
                if (!string.IsNullOrEmpty(name))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "userName";
                    ce.ExpValue = "%" + name + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(projectName))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "projectName";
                    ce.ExpValue = "%" + projectName + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrEmpty(oilName))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "gasstationName";
                    ce.ExpValue = "%" + oilName + "%";
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                if (!string.IsNullOrWhiteSpace(time))
                {
                    DateTime stime = Convert.ToDateTime(Convert.ToDateTime(time).ToString("yyyy-MM-dd"));
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "SignTime",
                        ExpValue = stime,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.GreaterThanOrEqual
                    });
                }
                if (!string.IsNullOrEmpty(endtime))
                {
                    DateTime etime = Convert.ToDateTime(Convert.ToDateTime(endtime).ToString("yyyy-MM-dd") + " 23:59:59");
                    qc.ConditionList.Add(new ConditionExpression()
                    {

                        ExpName = "SignTime",
                        ExpValue = etime,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.LessThanOrEqual
                    });
                }
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
                result = proxy.GetSignInformationList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.pageIndex = pageIndex;
            }
            return View(result.Data);
        }

        /// <summary>
        /// 考勤记录详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetSignInfoDetails(long id)
        {
            Result<Epm_SignInformation> result = new Result<Epm_SignInformation>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetSignInformationModel(id);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 人脸识别/考勤操作日志
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetFaceLogList(int pageIndex = 1, int pageSize = 10)
        {
            Result<List<EPM_FaceOperateLog>> result = new Result<List<EPM_FaceOperateLog>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                QueryCondition qc = new QueryCondition();
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
                result = proxy.GetFaceOperateLogList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.pageIndex = pageIndex;
            }
            return View(result.Data);
        }
    }
}