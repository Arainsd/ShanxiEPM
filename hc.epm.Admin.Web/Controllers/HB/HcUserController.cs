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

namespace hc.epm.Admin.Web.Controllers.HB
{
    public class HcUserController : BaseHBController
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
        public ActionResult CompanyList(string belong, string name = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.belong = belong;
            ViewBag.pageIndex = pageIndex;
            ViewBag.belongText = belong.ToEnumReq<RoleType>().GetText();
            if (string.IsNullOrEmpty(belong))
            {
                return RedirectToAction("Error", "Home", new { msg = "必须有用户归属" });
            }
            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();
            ce = new ConditionExpression();
            ce.ExpName = "Type";
            ce.ExpValue = "%" + belong + "%";
            ce.ExpOperater = eConditionOperator.Like;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);
            if (!string.IsNullOrEmpty(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Name";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            SortExpression sort = new SortExpression("PreCode", eSortType.Asc);
            qc.SortList.Add(sort);

            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompanyList(qc);
                ViewBag.Total = result.AllRowsCount;
            }

            return View(result.Data);
        }

        // GET: User
        public ActionResult Index(RoleType belong = RoleType.Supplier, long companyId = 0, bool isDelete = false, string userName = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.UserName = userName;
            ViewBag.Belong = belong;
            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();

            if (!string.IsNullOrEmpty(userName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "UserName";
                ce.ExpValue = "%" + userName + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            Result<List<UserListView>> result = new Result<List<UserListView>>();
            Result<Base_Company> userResult = new Result<Base_Company>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                //if (belong == RoleType.Admin)
                //{

                //    userResult = proxy.GetCompanyByRoleType(belong);
                //    //ce = new ConditionExpression();
                //    //ce.ExpName = "CompanyId";
                //    //ce.ExpValue = userResult.Data.Id;
                //    //ce.ExpOperater = eConditionOperator.Equal;
                //    //ce.ExpLogical = eLogicalOperator.And;
                //    //qc.ConditionList.Add(ce);
                //    result = proxy.GetUserList(qc);
                //    ViewBag.CompanyId = userResult.Data.Id;
                //}
                //else
                //{
                //ce = new ConditionExpression();
                //ce.ExpName = "CompanyId";
                //ce.ExpValue = companyId;
                //ce.ExpOperater = eConditionOperator.Equal;
                //ce.ExpLogical = eLogicalOperator.And;
                //qc.ConditionList.Add(ce);
                result = proxy.GetUserCompanyList(qc);
                ViewBag.CompanyId = companyId;
                // }

                ViewBag.Total = result.AllRowsCount;
                ViewBag.pageIndex = pageIndex;
            }
            return View(result.Data);
        }

        public void GetCompanyInfoList()
        {
            ConditionExpression ce = null;
            QueryCondition qc = new QueryCondition();
            ce = new ConditionExpression();
            ce.ExpName = "PreCode";
            ce.ExpValue = "10";
            ce.ExpOperater = eConditionOperator.Equal;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);

            ce = new ConditionExpression();
            ce.ExpName = "Type";
            ce.ExpValue = "Owner";
            ce.ExpOperater = eConditionOperator.NotEqual;
            ce.ExpLogical = eLogicalOperator.Or;
            qc.ConditionList.Add(ce);

            SortExpression sort = new SortExpression("PreCode", eSortType.Asc);
            qc.SortList.Add(sort);

            qc.PageInfo.isAllowPage = false;
            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompanyList(qc);
                ViewBag.CompanyId = result.Data.ToSelectList("Name", "Id", true);
            }
        }

        /// <summary>
        /// 添加会员
        /// </summary>
        /// <param name="id"></param>
        ///  <param name="companyId"></param>
        /// <param name="expId"></param>
        /// <returns></returns>
        /// 此方法为公用方法，除五种不同belong用户的添加，还用于专家管理界面关联用户时的新增
        public ActionResult Add(RoleType belong = RoleType.Supplier, string companyId = "")
        {

            Result<Base_Company> companyResult = new Result<Base_Company>();
            Result<int> userNum = new Result<int>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                userNum = proxy.GetUserCountByCompanyId(long.Parse(companyId));
                if (belong == RoleType.Admin)
                {
                    companyResult = proxy.GetCompanyByRoleType(belong);
                    ViewBag.CompanyId = companyResult.Data.Id;
                }
                else
                {
                    ViewBag.CompanyId = companyId;
                }
            }
            ViewBag.Belong = belong;
            ViewBag.currentTime = System.DateTime.Now;

            GetCompanyInfoList();
            return View();
        }

        /// <summary>
        /// 新增用户（专家用户新增）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(Base_User model)
        {
            string fileDataJson = Request.Form["fileDataJson"];//获取上传文件json字符串
            List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符串序列化为列表

            string belong = RoleType.Supplier.ToString();
            Result<int> result = new Result<int>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                model.PassWord = "123456";
                model.PassWord = DesTool.DesEncrypt(model.PassWord);//用户密码加密

                result = proxy.AddUser(model, fileList);
            }
            return Json(result.ToResultView());
        }


        /// <summary>
        /// 编辑用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(long id, string belong)
        {
            ViewBag.currentTime = System.DateTime.Now;
            ViewBag.belong = belong;
            Result<Base_User> result = new Result<Base_User>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetUserModel(id);
            }
            GetCompanyInfoList();
            return View(result.Data);
        }
        [HttpPost]
        public ActionResult Edit(Base_User model)
        {

            Result<int> result = new Result<int>();
            ResultView<int> view = new ResultView<int>();
            string belong = Request.Form["belong"];
            if (string.IsNullOrEmpty(model.UserName))
            {
                view.Flag = false;
                view.Message = "用户名不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.UserAcct))
            {
                view.Flag = false;
                view.Message = "登录名不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                view.Flag = false;
                view.Message = "电子邮箱不能为空";
                return Json(view);
            }
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
                var oldData = proxy.GetUserModel(model.Id).Data;
                oldData.UserName = model.UserName;
                oldData.UserAcct = model.UserAcct;
                oldData.Sex = model.Sex;
                oldData.UserCode = model.UserCode;
                oldData.Email = model.Email;
                oldData.Phone = model.Phone;
                oldData.QQ = model.QQ;
                result = proxy.UpdateUser(oldData, fileList);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 改变锁定状态
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditState(long id, string belong)
        {
            Result<int> result = new Result<int>();
            ResultView<bool> checkRight = new ResultView<bool>();
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
        public ActionResult Delete(string ids, string belong)
        {
            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteUserByIds(list);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 用户设置角色
        /// </summary>
        /// <param name="belong"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult SetRole(RoleType belong, long id)
        {
            ViewBag.Belong = belong;
            ViewBag.Id = id;
            Result<List<Base_Role>> result = new Result<List<Base_Role>>();
            List<Base_UserRole> userRoles = new List<Base_UserRole>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetRoleListByBelong(belong);//获取当前用户类型下的角色列表
                userRoles = proxy.GetRolesByUserId(id).Data;//加载当前用户所具有的角色
                ViewBag.userRoles = userRoles.Select(i => i.RoleId).ToList();
            }
            return View(result.Data);
        }

        public ActionResult SetRoleFun(long id, RoleType belong, string ids)
        {
            Result<int> result = new Result<int>();
            List<long> list = ids.SplitString(",").ToLongList();//角色列表
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.SetUserRole(id, list, belong);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 获取会员详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(long id, string belong)
        {
            Result<Base_User> result = new Result<Base_User>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetUserModel(id);
            }
            return View(result.Data);
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePWD()
        {
            ViewBag.userName = CurrentUser.UserName;
            ViewBag.userId = CurrentUser.UserId;
            return View();
        }

        public ActionResult ChangePasswod(string oldPWD, string newPWD, string confirmPWD)
        {
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
        public ActionResult EditInformation()
        {
            Result<Base_User> result = new Result<Base_User>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetUserModel(CurrentUser.UserId);
            }
            return View(result.Data);
        }
        [HttpPost]

        public ActionResult EditInformation(Base_User model)
        {

            Result<int> result = new Result<int>();
            Result<Base_User> userResult = new Result<Base_User>();
            ResultView<int> view = new ResultView<int>();
            if (string.IsNullOrEmpty(model.UserName))
            {
                view.Flag = false;
                view.Message = "用户名不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                view.Flag = false;
                view.Message = "电子邮箱不能为空";
                return Json(view);
            }
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
        public ActionResult GetCompanyName(long id)
        {
            Result<Base_Company> result = new Result<Base_Company>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompanyModel(id);
            }
            return Json(result);
        }
        /// <summary>
        /// 修改资料界面根据DepID获取DepName
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetDepName(long id)
        {
            Result<Base_Dep> result = new Result<Base_Dep>();
            using (AdminClientProxy proxy = new AdminClientProxy(ProxyEx(Request)))
            {
                result = proxy.GeDepModel(id);
            }
            return Json(result);
        }

        /// <summary>
        /// 全部用户检索
        /// </summary>
        /// <returns></returns>
        public ActionResult UserRetrieve(string name, int pageIndex = 1, int pageSize = 10, string isLock = "0")
        {
            //权限检查
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

    }
}




