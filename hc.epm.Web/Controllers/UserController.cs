using hc.epm.DataModel.Basic;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using hc.epm.Common;
using hc.epm.Web.ClientProxy;
using hc.epm.DataModel.Business;
using hc.Plat.Common.Extend;
using Newtonsoft.Json;
using System.Data;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System.IO;
using LinqToExcel;
using NPOI.XSSF.UserModel;
using System.Configuration;
using System.Drawing;


namespace hc.epm.Web.Controllers
{
    public class UserController : BaseWebController
    {
        #region 人才库
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="belong">公司类型</param>
        /// <param name="userName"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult Index(string belong, string userName = "", int pageIndex = 1, int pageSize = 10)
        {
            List<string> removeValues = new List<string>();
            removeValues.Add(RoleType.Admin.ToString());
            ViewBag.CompanyType = typeof(RoleType).AsSelectList(true, "", removeValues);
            ViewBag.UserName = userName;
            ViewBag.Belong = belong;


            QueryCondition qc = new QueryCondition();
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            if (!string.IsNullOrWhiteSpace(belong))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Type",
                    ExpValue = belong,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
            }
            if (!string.IsNullOrWhiteSpace(userName))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "UserName",
                    ExpValue = userName,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
            }

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                Result<List<UserListView>> result = new Result<List<UserListView>>();

                ViewBag.Total = result.AllRowsCount;
                ViewBag.pageIndex = pageIndex;
                return View(result.Data);
            }
        }
        public ActionResult Add()
        {
            ViewBag.Title = "新增用户";
            return View();
        }

        public ActionResult Detail(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                Result<Base_User> result = proxy.GetUserModel(id);
                return View(result.Data);
            }
        }
        public ActionResult GetCompanyName(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                Result<CompanyView> result = proxy.GetCompanyModel(id);
                return Json(result);
            }
        }

        /// <summary>
        /// 考勤记录列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="userId"></param>
        /// <param name="projectName"></param>
        /// <param name="oilName"></param>
        /// <param name="time"></param>
        /// <param name="endtime"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetSignList(string name, string projectName, string oilName, string time, string endtime, int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.projectName = projectName;
            ViewBag.oilName = oilName;
            ViewBag.time = time;
            ViewBag.endtime = endtime;
            Result<List<Epm_SignInformation>> result = new Result<List<Epm_SignInformation>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = SignList(name, projectName, oilName, time, endtime, 0, pageIndex, pageSize);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.pageIndex = pageIndex;
                ViewBag.pageSize = pageSize;
            }
            return View(result.Data);
        }

        /// <summary>
        /// 根据条件获取签到信息列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="projectName"></param>
        /// <param name="oilName"></param>
        /// <param name="time"></param>
        /// <param name="endtime"></param>
        /// <param name="userId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private Result<List<Epm_SignInformation>> SignList(string name, string projectName, string oilName, string time, string endtime, long userId, int pageIndex = 1, int pageSize = 10)
        {
            Result<List<Epm_SignInformation>> result = new Result<List<Epm_SignInformation>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                QueryCondition qc = new QueryCondition();
                ConditionExpression ce = new ConditionExpression();
                if (userId != 0)
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "userId";
                    ce.ExpValue = userId;
                    ce.ExpOperater = eConditionOperator.Equal;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
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
            }
            return result;
        }

        /// <summary>
        /// 考勤记录详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetSignDetail(long id)
        {
            Result<Epm_SignInformation> result = new Result<Epm_SignInformation>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetSignInformationModel(id);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 人才管理库
        /// </summary>
        /// <returns></returns>
        //public ActionResult UserList(string type = "", string name = "", string companyName = "", string postName = "", int pageIndex = 1, int pageSize = 10)
        //{
        //    Result<List<UserListView>> result = new Result<List<UserListView>>();
        //    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
        //    {
        //        QueryCondition qc = new QueryCondition();
        //        ConditionExpression ce = null;
        //        qc.PageInfo = GetPageInfo(pageIndex, pageSize);
        //        if (!string.IsNullOrEmpty(name))
        //        {
        //            ce = new ConditionExpression();
        //            ce.ExpName = "UserName";
        //            ce.ExpValue = name;
        //            ce.ExpOperater = eConditionOperator.Like;
        //            ce.ExpLogical = eLogicalOperator.And;
        //            qc.ConditionList.Add(ce);
        //        }
        //        if (!string.IsNullOrEmpty(companyName))
        //        {
        //            ce = new ConditionExpression();
        //            ce.ExpName = "CompanyName";
        //            ce.ExpValue = companyName;
        //            ce.ExpOperater = eConditionOperator.Like;
        //            ce.ExpLogical = eLogicalOperator.And;
        //            qc.ConditionList.Add(ce);
        //        }
        //        if (!string.IsNullOrEmpty(postName))
        //        {
        //            ce = new ConditionExpression();
        //            ce.ExpName = "PostValue";
        //            ce.ExpValue = postName;
        //            ce.ExpOperater = eConditionOperator.Like;
        //            ce.ExpLogical = eLogicalOperator.And;
        //            qc.ConditionList.Add(ce);
        //        }
        //        result = proxy.GetUserManageList(qc, type);
        //    }
        //    ViewBag.Total = result.AllRowsCount;
        //    ViewBag.pageIndex = pageIndex;
        //    ViewBag.pageSize = pageSize;
        //    return View(result.Data);
        //}

        ///// <summary>
        ///// 新增人员信息
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult AddUser()
        //{
        //    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
        //    {
        //        List<DictionaryType> dic = new List<DictionaryType>() { DictionaryType.ProfessionalType, DictionaryType.PostType, DictionaryType.QualificationType };
        //        var diclist = proxy.GetTypeListByTypes(dic).Data;

        //        ViewBag.Code = SnowflakeHelper.GetID;
        //        //职称
        //        ViewBag.Professional = diclist[DictionaryType.ProfessionalType].ToSelectList("Name", "No", true);
        //        //岗位
        //        ViewBag.Post = diclist[DictionaryType.PostType].ToSelectList("Name", "No", true);
        //        //职业资格
        //        ViewBag.ProfessionalQualification = diclist[DictionaryType.QualificationType].ToSelectList("Name", "No", true);
        //    }
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult AddUser(Base_User model)
        //{
        //    ResultView<int> view = new ResultView<int>();
        //    if (string.IsNullOrEmpty(model.UserCode))
        //    {
        //        view.Flag = false;
        //        view.Message = "用户编码不能为空";
        //        return Json(view);
        //    }
        //    //if (string.IsNullOrEmpty(model.UserAcct))
        //    //{
        //    //    view.Flag = false;
        //    //    view.Message = "登录账号不能为空";
        //    //    return Json(view);
        //    //}
        //    if (string.IsNullOrEmpty(model.UserName))
        //    {
        //        view.Flag = false;
        //        view.Message = "用户名称不能为空";
        //        return Json(view);
        //    }
        //    string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
        //    List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符串序列化为列表

        //    Result<int> result = new Result<int>();
        //    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
        //    {
        //        model.PassWord = DesTool.DesEncrypt("123456");//用户密码加密
        //        result = proxy.AddUserInfo(model, fileList);
        //    }
        //    return Json(result.ToResultView());
        //}

        ///// <summary>
        ///// 修改人员信息
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult UpdateUser(long id)
        //{
        //    Result<Base_User> result = new Result<Base_User>();
        //    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
        //    {
        //        List<DictionaryType> dic = new List<DictionaryType>() { DictionaryType.ProfessionalType, DictionaryType.PostType, DictionaryType.QualificationType };
        //        var diclist = proxy.GetTypeListByTypes(dic).Data;
        //        //职称
        //        ViewBag.Professional = diclist[DictionaryType.ProfessionalType].ToSelectList("Name", "No", true);
        //        //岗位
        //        ViewBag.Post = diclist[DictionaryType.PostType].ToSelectList("Name", "No", true);
        //        //职业资格
        //        ViewBag.ProfessionalQualification = diclist[DictionaryType.QualificationType].ToSelectList("Name", "No", true);

        //        result = proxy.GetUserDetail(id);
        //    }
        //    return View(result.Data);
        //}

        //[HttpPost]
        //public ActionResult UpdateUser(Base_User model)
        //{
        //    ResultView<int> view = new ResultView<int>();
        //    if (string.IsNullOrEmpty(model.UserCode))
        //    {
        //        view.Flag = false;
        //        view.Message = "用户编码不能为空";
        //        return Json(view);
        //    }
        //    if (string.IsNullOrEmpty(model.UserAcct))
        //    {
        //        view.Flag = false;
        //        view.Message = "登录账号不能为空";
        //        return Json(view);
        //    }
        //    if (string.IsNullOrEmpty(model.UserName))
        //    {
        //        view.Flag = false;
        //        view.Message = "用户名称不能为空";
        //        return Json(view);
        //    }

        //    string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
        //    List<Base_Files> fileList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符串序列化为列表

        //    Result<int> result = new Result<int>();
        //    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
        //    {
        //        result = proxy.UpdateUserInfo(model, fileList);
        //    }
        //    return Json(result.ToResultView());
        //}

        /// <summary>
        /// 查询人员信息详情
        /// </summary>
        /// <returns></returns>
        public ActionResult UserDetail(long id, string projectName = "", string time = "", string endtime = "", int pageIndex = 1, int pageSize = 10)
        {
            Result<List<Epm_SignInformation>> signResult = new Result<List<Epm_SignInformation>>();
            Result<Base_User> result = new Result<Base_User>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetUserDetail(id);

                signResult = SignList("", projectName, "", time, endtime, id, pageIndex, pageSize);
            }
            ViewBag.Total = signResult.AllRowsCount;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.SignList = signResult.Data;
            return View(result.Data);
        }

        /// <summary>
        /// 批量删除用户
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> idList = ids.SplitString(",").ToLongList();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteUserByIds(idList);
            }
            return Json(result.ToResultView());
        }
        #endregion


        #region 三商管理-人员管理
        /// <summary>
        /// 人才管理库
        /// </summary>CompanyId 必须传值
        /// <returns></returns>
        public ActionResult UserList(long CompanyId, string type = "", string name = "", string postName = "", int pageIndex = 1, int pageSize = 10)
        {
            // toDB("");
            //toDB(@"D:\EPM\ImportOrExportFile\导入人员信息.xlsx");
            // ExportToExcel(1208304798651256832);
            var pathUrl = ConfigurationManager.AppSettings["PathUserUrl"]; ;
            ViewBag.PathUserUrl = pathUrl;
            Result<List<UserListView>> result = new Result<List<UserListView>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                QueryCondition qc = new QueryCondition();
                ConditionExpression ce = null;
                ResultView<int> view = new ResultView<int>();
                if (CompanyId == 0)
                {
                    view.Flag = false;
                    view.Message = "单位ID不能为空";
                    return Json(view);

                }
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
                if (!string.IsNullOrEmpty(name))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "UserName";
                    ce.ExpValue = name;
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }


                if (!string.IsNullOrEmpty(postName))
                {
                    ce = new ConditionExpression();
                    ce.ExpName = "PostValue";
                    ce.ExpValue = postName;
                    ce.ExpOperater = eConditionOperator.Like;
                    ce.ExpLogical = eLogicalOperator.And;
                    qc.ConditionList.Add(ce);
                }
                ce = new ConditionExpression();
                ce.ExpName = "CompanyId";
                ce.ExpValue = CompanyId;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);

                //qc.SortList.Add(new SortExpression("CreateTime", eSortType.Desc));
                result = proxy.GetUserManageList(qc, type);

                var companyRes = proxy.GetCompanyModel(CompanyId);
                ViewBag.companyType = companyRes.Data.Type;
            }


            ViewBag.CompanyId = CompanyId;
            ViewBag.Total = result.AllRowsCount;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            return View(result.Data);
        }


        /// <summary>
        /// 新增人员信息
        /// </summary>
        /// <returns></returns>
        public ActionResult AddUser(long CompanyId)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                List<DictionaryType> dic = new List<DictionaryType>() { DictionaryType.ProfessionalType, DictionaryType.PostType, DictionaryType.QualificationType, DictionaryType.UserFileType };
                var diclist = proxy.GetTypeListByTypes(dic).Data;

                ViewBag.Code = SnowflakeHelper.GetID;
                //职称
                ViewBag.Professional = diclist[DictionaryType.ProfessionalType].ToSelectList("Name", "No", true);
                //岗位
                ViewBag.Post = diclist[DictionaryType.PostType].ToSelectList("Name", "No", true);
                //职业资格
                ViewBag.ProfessionalQualification = diclist[DictionaryType.QualificationType].ToSelectList("Name", "No", true);

                //附件类型
                ViewBag.UserFileType = diclist[DictionaryType.UserFileType].Where(p => p.No != "GYSZP").ToList().ToSelectList("Name", "No", true);

                var compamy = proxy.GetAreaCompanyList();
                //地市公司
                ViewBag.companyList = compamy.Data.ToSelectList("Name", "Id", true);

                //所属单位名字
                ViewBag.companyName = proxy.GetCompanyModel(CompanyId).Data.Name;

                //所属单位ID
                ViewBag.CompanyId = CompanyId;
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddUser(Base_User model)
        {
            ResultView<int> view = new ResultView<int>();
            if (string.IsNullOrEmpty(model.UserCode))
            {
                view.Flag = false;
                view.Message = "用户编码不能为空";
                return Json(view);
            }
            if (model.CompanyId == 0)
            {
                view.Flag = false;
                view.Message = "单位ID不能为空";
                return Json(view);
            }
            if (string.IsNullOrEmpty(model.UserName))
            {
                view.Flag = false;
                view.Message = "用户名称不能为空";
                return Json(view);
            }
            List<Base_Files> fileList = new List<Base_Files>();

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                List<Base_Files> baseList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符串序列化为列表
                fileList.AddRange(baseList);
            }

            string fileDataJsonZP = Request.Form["fileDataJsonFileZP"];//获取上传头像json
            if (!string.IsNullOrEmpty(fileDataJsonZP))
            {
                List<Base_Files> fileZPList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJsonZP);//将文件信息json字符串序列化为列表
                fileList.AddRange(fileZPList);
            }
            string image = Request.Form["ImageInfo"];
            if (!string.IsNullOrEmpty(image))
            {
                Bitmap bmp = new Bitmap(image);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                image = Convert.ToBase64String(arr);
            }


            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                if (!string.IsNullOrEmpty(model.PassWord))
                {
                    model.PassWord = DesTool.DesEncrypt(model.PassWord);//用户密码加密
                }
                else
                {
                    model.PassWord = DesTool.DesEncrypt("123456");//默认密码为123456
                }
                result = proxy.AddUserInfo(model, image, fileList);
            }

            return Json(result.ToResultView());
        }

        /// <summary>
        /// 修改人员信息
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateUser(long id)
        {
            Result<Base_User> result = new Result<Base_User>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                List<DictionaryType> dic = new List<DictionaryType>() { DictionaryType.ProfessionalType, DictionaryType.PostType, DictionaryType.QualificationType, DictionaryType.UserFileType };
                var diclist = proxy.GetTypeListByTypes(dic).Data;

                result = proxy.GetUserDetail(id);

                //职称
                ViewBag.Professional = diclist[DictionaryType.ProfessionalType].ToSelectList("Name", "No", true, result.Data.ProfessionalValue);
                //岗位
                ViewBag.Post = diclist[DictionaryType.PostType].ToSelectList("Name", "No", true, result.Data.PostValue);
                //职业资格
                ViewBag.ProfessionalQualification = diclist[DictionaryType.QualificationType].ToSelectList("Name", "No", true, result.Data.ProfessionalQualificationValue);

                //附件类型
                ViewBag.UserFileType = diclist[DictionaryType.UserFileType].Where(p => p.No != "GYSZP").ToList().ToSelectList("Name", "No", true);

                var compamy = proxy.GetAreaCompanyList();
                //地市公司
                ViewBag.companyList = compamy.Data.ToSelectList("Name", "Id", true);

            }
            return View(result.Data);
        }

        [HttpPost]
        public ActionResult UpdateUser(Base_User model)
        {
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
            //if (model.CompanyId == 0)
            //{
            //    view.Flag = false;
            //    view.Message = "单位ID不能为空";
            //    return Json(view);
            //}

            List<Base_Files> fileList = new List<Base_Files>();

            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                List<Base_Files> baseList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符串序列化为列表
                fileList.AddRange(baseList);
            }

            string fileDataJsonZP = Request.Form["fileDataJsonFileZP"];//获取上传头像json
            if (!string.IsNullOrEmpty(fileDataJsonZP))
            {
                List<Base_Files> fileZPList = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJsonZP);//将文件信息json字符串序列化为列表
                fileList.AddRange(fileZPList);
            }
            string image = Request.Form["ImageInfo"];
            if (!string.IsNullOrEmpty(image))
            {
                Bitmap bmp = new Bitmap(image);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                image = Convert.ToBase64String(arr);
            }




            Result<int> result = new Result<int>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                if (!string.IsNullOrEmpty(model.PassWord))
                {
                    model.PassWord = DesTool.DesEncrypt(model.PassWord);//用户密码加密
                }
                else
                {
                    model.PassWord = DesTool.DesEncrypt("123456");//默认密码为123456
                }
                result = proxy.UpdateUserInfo(model, image, fileList);
            }
            return Json(result.ToResultView());
        }
        #endregion


        #region 批量导入导出
        [HttpPost]
        #region 批量导出
        //导出              
        public ActionResult ExportToExcel(long? CompanyId)
        {
            Result<List<UserListView>> result = new Result<List<UserListView>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                QueryCondition qc = new QueryCondition();
                ConditionExpression ce = null;
                ResultView<int> view = new ResultView<int>();
                if (CompanyId == 0)
                {
                    view.Flag = false;
                    view.Message = "单位ID不能为空";
                    return Json(view);

                }
                ce = new ConditionExpression();
                ce.ExpName = "CompanyId";
                ce.ExpValue = CompanyId;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);

                result = proxy.GetUserManageList(qc, "");
                var suss = ListToExcel(result, result.Data.First().CompanyName);

                return Json(suss);
            }
        }

        public ResultView<string> ListToExcel(Result<List<UserListView>> userlist, string cumpanyName)
        {
            ResultView<string> result = new ResultView<string>();
            HSSFWorkbook workbook = new HSSFWorkbook();
            HSSFSheet sheet = (HSSFSheet)workbook.CreateSheet();
            sheet.DefaultColumnWidth = 15;
            //创建表格样式  
            IFont font = workbook.CreateFont();
            font.FontName = "宋体";
            font.FontHeightInPoints = 10;
            ICellStyle style = workbook.CreateCellStyle(); ;
            style.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
            style.BottomBorderColor = HSSFColor.Black.Index;
            style.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
            style.LeftBorderColor = HSSFColor.Black.Index;
            style.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
            style.RightBorderColor = HSSFColor.Black.Index;
            style.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
            style.TopBorderColor = HSSFColor.Black.Index;
            style.Alignment = HorizontalAlignment.Center;
            style.SetFont(font);
            IRow rowHead = sheet.CreateRow(0);
            rowHead.CreateCell(0).SetCellValue("公司名称");
            rowHead.GetCell(0).CellStyle = style;
            sheet.SetColumnWidth(0, 256 * 20);

            rowHead.CreateCell(1).SetCellValue("企业类型名称");
            rowHead.GetCell(1).CellStyle = style;
            sheet.SetColumnWidth(1, 256 * 20);

            rowHead.CreateCell(2).SetCellValue("用户名");
            rowHead.GetCell(2).CellStyle = style;
            sheet.SetColumnWidth(2, 256 * 20);

            rowHead.CreateCell(3).SetCellValue("电话");
            rowHead.GetCell(3).CellStyle = style;
            sheet.SetColumnWidth(3, 256 * 20);

            rowHead.CreateCell(4).SetCellValue("用户编码");
            rowHead.GetCell(4).CellStyle = style;
            sheet.SetColumnWidth(4, 256 * 20);

            rowHead.CreateCell(5).SetCellValue("真实姓名");
            rowHead.GetCell(5).CellStyle = style;
            sheet.SetColumnWidth(5, 256 * 20);

            rowHead.CreateCell(6).SetCellValue("邮箱");
            rowHead.GetCell(6).CellStyle = style;
            sheet.SetColumnWidth(6, 256 * 20);

            rowHead.CreateCell(7).SetCellValue("岗位");
            rowHead.GetCell(7).CellStyle = style;
            sheet.SetColumnWidth(7, 256 * 20);

            rowHead.CreateCell(8).SetCellValue("职称");
            rowHead.GetCell(8).CellStyle = style;
            sheet.SetColumnWidth(8, 256 * 20);

            rowHead.CreateCell(9).SetCellValue("资格Value");
            rowHead.GetCell(9).CellStyle = style;
            sheet.SetColumnWidth(9, 256 * 20);

            rowHead.CreateCell(10).SetCellValue("导入时间");
            rowHead.GetCell(10).CellStyle = style;
            sheet.SetColumnWidth(10, 256 * 20);
            int rowindex = 1;
            foreach (var item in userlist.Data)
            {
                IRow rowContent = sheet.CreateRow(rowindex);
                rowContent.CreateCell(0).SetCellValue(item.CompanyName);
                rowContent.GetCell(0).CellStyle = style;

                rowContent.CreateCell(1).SetCellValue(item.CompanyTypeName);
                rowContent.GetCell(1).CellStyle = style;

                rowContent.CreateCell(2).SetCellValue(item.UserName);
                rowContent.GetCell(2).CellStyle = style;

                rowContent.CreateCell(3).SetCellValue(item.Phone);
                rowContent.GetCell(3).CellStyle = style;

                rowContent.CreateCell(4).SetCellValue(item.UserCode);
                rowContent.GetCell(4).CellStyle = style;

                rowContent.CreateCell(5).SetCellValue(item.RealName);
                rowContent.GetCell(5).CellStyle = style;

                rowContent.CreateCell(6).SetCellValue(item.Email);
                rowContent.GetCell(6).CellStyle = style;

                rowContent.CreateCell(7).SetCellValue(item.Post);
                rowContent.GetCell(7).CellStyle = style;

                rowContent.CreateCell(8).SetCellValue(item.Professional);
                rowContent.GetCell(8).CellStyle = style;

                rowContent.CreateCell(9).SetCellValue(item.ProfessionalQualification);
                rowContent.GetCell(9).CellStyle = style;

                rowContent.CreateCell(10).SetCellValue(item.CreateTime.ToString());
                rowContent.GetCell(10).CellStyle = style;

                rowindex++;
            }
            var path = System.Configuration.ConfigurationManager.AppSettings["ImportOrExportPath"];
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);  //创建目录
            }

            DirectoryInfo dir = new DirectoryInfo(path);
            string bookname = cumpanyName + DateTime.Now.ToString("yyyyMMddHHmmss") + "UserInfo.xls";
            string filePath = path + bookname;
            try
            {
                using (FileStream fs = new FileStream(dir + "\\" + bookname, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(fs);
                    result.Flag = true;
                    result.Other = filePath;
                    result.Data = System.IO.Path.GetFileName(filePath);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }
        #endregion


        #region 批量导入
        [HttpPost]
        [AuthCheck(Module = WebCategory.GoodsManage, Right = SystemRight.Import)]
        public ActionResult ExcelToDB()
        {
            ResultView<int> view = new ResultView<int>();
            HttpPostedFileBase file = Request.Files[0];
            var fileName = file.FileName;
            //判断目录是否存在,不存在创建
            if (!Directory.Exists(ImportOrExportPath))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(ImportOrExportPath);
                directoryInfo.Create();
            }
            //将文件重新命名，保证唯一性
            string pathFile = ImportOrExportPath + "success_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(fileName);
            //将上传的文件保存
            file.SaveAs(pathFile);
            List<Base_User> userList = excelToDT(pathFile);//excel转实体

            Result<int> result = new Result<int>();
            int index = 0;
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                foreach (var item in userList)
                {
                    item.PassWord = DesTool.DesEncrypt("123456");//用户密码加密
                                                                 // item.CompanyId = "";
                    result = proxy.AddUserInfo(item, "", new List<Base_Files>());
                    index++;
                }
                return Json(index);
            }
        }

        /// <summary>
        /// 文档导入导出路径
        /// </summary>
        private static string ImportOrExportPath
        {
            get
            {
                string value = ConfigurationManager.AppSettings["ImportOrExportPath"];
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new Exception("未配置文档导入导出路径！");
                }
                value = string.Format("{0}{1}\\{2}\\{3}\\{4}\\", value,"人员信息", DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
                return value;
            }
        }
        public void toDB(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                //view.Flag = false;
                //view.Message = "没有选择任何文件";
                //return Json(view);
            }
            if (!System.IO.File.Exists(fileName))
            {
                //view.Flag = false;
                //view.Message = "文件不存在";
                //return Json(view);
            }


            List<Base_User> userList = excelToDT(fileName);
            Result<int> result = new Result<int>();
            int index = 0;
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                foreach (var item in userList)
                {
                    item.PassWord = DesTool.DesEncrypt("123456");//用户密码加密
                                                                 // item.CompanyId = "";
                    result = proxy.AddUserInfo(item, "", new List<Base_Files>());
                    index++;
                }

            }
        }

        public LinqToExcel.Query.ExcelQueryable<Base_User> GetImportData(string fileName)
        {
            var excelFile = new ExcelQueryFactory(fileName);
            //对应列头
            excelFile.AddMapping<Base_User>(x => x.UserName, "*  用户名");
            excelFile.AddMapping<Base_User>(x => x.Phone, "*  联系电话");

            excelFile.AddMapping<Base_User>(x => x.UserAcct, "*   登录账号");
            excelFile.AddMapping<Base_User>(x => x.CompanyId, "*  所属单位");
            excelFile.AddMapping<Base_User>(x => x.Post, "*  岗位");//value?


            //SheetName
            var excelContent = excelFile.Worksheet<Base_User>(0);
            return excelContent;
        }

        bool checkFile(string filePath)
        {
            LinqToExcel.Query.ExcelQueryable<Base_User> excelContent = GetImportData(filePath);

            string msg = "";
            string path = Path.GetDirectoryName(filePath);
            string tempPath = path;
            FileStream fs = new FileStream(tempPath, FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter m_streamWriter = new StreamWriter(fs);
            m_streamWriter.BaseStream.Seek(0, SeekOrigin.End);
            int rowIndex = 1;
            bool isok = false;
            //检查数据正确性
            foreach (var row in excelContent)
            {
                if (string.IsNullOrEmpty(row.UserName) && string.IsNullOrEmpty(row.UserCode) && string.IsNullOrEmpty(row.Phone) && string.IsNullOrEmpty(row.UserAcct) && string.IsNullOrEmpty(row.Post))
                {
                    break;//excel结束
                }
                rowIndex += 1;
                var Phone = excelContent.GroupBy(x => new { x.Phone }).Where(x => x.Count() > 1)
                .SelectMany(x => x.ToList()).ToList();

                var UserAcct = excelContent.GroupBy(x => new { x.UserAcct }).Where(x => x.Count() > 1)
             .SelectMany(x => x.ToList()).ToList();
                Result<List<UserListView>> resultDb = new Result<List<UserListView>>();
                using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
                {
                    QueryCondition qc = new QueryCondition();
                    resultDb = proxy.GetUserManageList(qc, "");
                }
                var ckPhone = (from a in excelContent join b in resultDb.Data on a.Phone equals b.Phone select new { Phone = a.Phone }).ToList();

                var ckUserAcct = (from a in excelContent join b in resultDb.Data on a.UserAcct equals b.UserName select new { UserAcct = a.UserAcct }).ToList();

                if (Phone != null)
                {
                    msg += "Excel中【 *  联系电话 】 有重复，请确认；";
                    isok = true;
                }
                if (UserAcct != null)
                {
                    msg += "Excel中【 *  登录账户名 】 有重复，请确认；";
                    isok = true;
                }
                if (ckPhone.Count > 0)
                {
                    msg += "Excel中该【 *  联系电话 】已经存在，请确认；" + string.Join(",", ckPhone);
                    isok = true;
                }
                if (ckUserAcct.Count > 0)
                {
                    msg += "Excel中该【 *  登录账户名 】已经存在，请确认；" + string.Join(",", ckUserAcct);
                    isok = true;
                }
                if (string.IsNullOrEmpty(row.UserName))
                {
                    msg += "字段:【 *  用户名 】 为空；";
                    isok = true;
                }
                if (string.IsNullOrEmpty(row.Phone))
                {
                    msg += "字段:【 *  联系电话】 为空；";
                    isok = true;
                }
                if (string.IsNullOrEmpty(row.UserAcct))
                {
                    msg += "字段:【*   登录账号】 为空；";
                    isok = true;
                }

                if (string.IsNullOrEmpty(row.Post))
                {
                    msg += "字段:【*  岗位】 为空；";
                    isok = true;
                }
                if (isok == true)
                {
                    m_streamWriter.WriteLine("检查结果:第" + rowIndex + "行，" + msg);
                    m_streamWriter.Flush();

                }
            }
            m_streamWriter.Close();
            fs.Close();
            return isok;
        }

        #endregion

        #region 转换
        public List<Base_User> excelToDT(string fileName)
        {
            List<Base_User> userList = new List<Base_User>();
            // var dt = hc.epm.UI.Common.ExcelHelper.ImportExcelData(fileName);

            var dt1 = ExcelToTable1(fileName);
            //获取字典表数据，进行匹配
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.PostType, DictionaryType.ProfessionalType, DictionaryType.QualificationType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                var PostTypeList = subjects[DictionaryType.PostType].ToList();//岗位
                var ProfessionalTypeList = subjects[DictionaryType.ProfessionalType].ToList();//职称
                var QualificationTypeList = subjects[DictionaryType.QualificationType].ToList();//职业资格

                List<string> Educations = new List<string> { "本科", "大专", "高中", "中专", "初中" };

                foreach (DataRow mDr in dt1.Rows)
                {
                    if (string.IsNullOrEmpty(mDr["*  用户名"].ToString()) && string.IsNullOrEmpty(mDr["*  联系电话"].ToString())
                          && string.IsNullOrEmpty(mDr["*   登录账号"].ToString()) && string.IsNullOrEmpty(mDr["*  岗位"].ToString()))
                    {
                        break;//excel结束
                    }
                    Base_User user = new Base_User();
                    foreach (DataColumn mDc in mDr.Table.Columns)
                    {
                        //List<string> listFind = Educations.FindAll(delegate (string s) {
                        //    return s.Contains(mDr["学历"].ToString());
                        //});

                        user.UserName = mDr["*  用户名"].ToString();
                        user.Phone = mDr["*  联系电话"].ToString();
                        user.BirthDate = Convert.ToDateTime(mDr["出生日期"]);
                        user.UserAcct = mDr["*   登录账号"].ToString();
                        user.CompanyId = proxy.GetCompanyByName(mDr["*  所属单位"].ToString()).Data.Id;

                        var post = PostTypeList.FirstOrDefault(p => p.Name.Contains(mDr["*  岗位"].ToString()));

                        if (post != null)  //mDr["*  岗位"].ToString()
                        {
                            user.Post = post.Name;
                            user.PostValue = post.No;
                        }
                        user.UserCode = mDr["用户编码"].ToString();
                        user.Sex = mDr["性别"].ToString() == "男" ? true : false;
                        user.Email = mDr["电子邮箱"].ToString();
                        user.University = mDr["毕业学校"].ToString();
                        user.Major = mDr["专业名称"].ToString();
                        user.Education = mDr["学历"].ToString();/*listFind.DefaultIfEmpty().ToString();*/
                        user.Address = mDr["项目地区"].ToString();


                        var ProfessionalType = ProfessionalTypeList.FirstOrDefault(p => p.Name.Contains(mDr["职称"].ToString()));
                        if (post != null)  //mDr["职称"].ToString()
                        {
                            user.Professional = ProfessionalType.Name;
                            user.ProfessionalValue = ProfessionalType.No;
                        }


                        var QualificationType = QualificationTypeList.FirstOrDefault(p => p.Name.Contains(mDr["职业资质"].ToString()));
                        if (post != null)  //mDr["职业资质"].ToString()
                        {
                            user.ProfessionalQualification = QualificationType.Name;
                            user.ProfessionalQualificationValue = QualificationType.No;
                        }


                        if (!string.IsNullOrEmpty(mDr["开始职业日期"].ToString()))
                        {
                            user.OccupationalStartTime = Convert.ToDateTime(mDr["开始职业日期"]);
                        }

                        if (!string.IsNullOrEmpty(mDr["职业简述"].ToString()))
                        {
                            user.OccupationalContent = mDr["职业简述"].ToString();
                        }

                        if (!string.IsNullOrEmpty(mDr["成绩"].ToString()))
                        {
                            user.achievement = Convert.ToDecimal(mDr["成绩"].ToString());
                        }

                        if (!string.IsNullOrEmpty(mDr["成绩开始日期"].ToString()))
                        {
                            user.achievementStartTime = Convert.ToDateTime(mDr["成绩开始日期"]);
                        }

                        if (!string.IsNullOrEmpty(mDr["成绩结束日期"].ToString()))
                        {
                            user.achievementEndTime = Convert.ToDateTime(mDr["成绩结束日期"]);
                        }
                    }
                    userList.Add(user);
                }

                return userList;
            }
        }
        /// <summary>
        /// 获取单元格类型
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        private static object GetValueType(ICell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank: //BLANK:  
                    return null;
                case CellType.Boolean: //BOOLEAN:  
                    return cell.BooleanCellValue;
                case CellType.Numeric: //NUMERIC:  
                    return cell.NumericCellValue;
                case CellType.String: //STRING:  
                    return cell.StringCellValue;
                case CellType.Error: //ERROR:  
                    return cell.ErrorCellValue;
                case CellType.Formula: //FORMULA:  
                default:
                    return "=" + cell.CellFormula;
            }
        }
        public DataTable ExcelToTable1(string file)
        {
            DataTable dt = new DataTable();
            IWorkbook workbook;
            string fileExt = Path.GetExtension(file).ToLower();
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                //XSSFWorkbook 适用XLSX格式，HSSFWorkbook 适用XLS格式
                if (fileExt == ".xlsx") { workbook = new XSSFWorkbook(fs); } else if (fileExt == ".xls") { workbook = new HSSFWorkbook(fs); } else { workbook = null; }
                if (workbook == null) { return null; }
                ISheet sheet = workbook.GetSheetAt(0);

                //表头  
                IRow header = sheet.GetRow(sheet.FirstRowNum);
                List<int> columns = new List<int>();
                for (int i = 0; i < header.LastCellNum; i++)
                {
                    object obj = GetValueType(header.GetCell(i));
                    if (obj == null || obj.ToString() == string.Empty)
                    {
                        dt.Columns.Add(new DataColumn("Columns" + i.ToString()));
                    }
                    else
                        dt.Columns.Add(new DataColumn(obj.ToString()));
                    columns.Add(i);
                }
                //数据  
                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                {
                    DataRow dr = dt.NewRow();
                    if (string.IsNullOrEmpty(sheet.GetRow(i).GetCell(2).ToString()) && string.IsNullOrEmpty(sheet.GetRow(i).GetCell(3).ToString()) && string.IsNullOrEmpty(sheet.GetRow(i).GetCell(4).ToString()))
                    {
                        break;
                    }

                    bool hasValue = false;
                    dt.Rows.Add(dr);
                    foreach (int j in columns)
                    {
                        dr[j] = GetValueType(sheet.GetRow(i).GetCell(j));
                        if (dr[j] != null && dr[j].ToString() != string.Empty)
                        {
                            hasValue = true;
                        }
                    }
                    if (hasValue)
                    {

                    }
                }
            }
            return dt;
        }
        #endregion

        #endregion

        #region 人员数据导入

        /// <summary>
        /// 导入人员信信到数据库
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportUserToDB(long companyId)
        {
            Result<int> result = new Result<int>();
            //失败数据
            List<int> list = new List<int>();
            try
            {
                List<Base_User> userList = new List<Base_User>();
                //当前页面所属单位Id
                //long companyId = Request.Form["CompanyId"].ToLongReq();
                HttpPostedFileBase file = Request.Files[0];
                var fileName = file.FileName;
                //判断目录是否存在,不存在创建
                if (!Directory.Exists(ImportOrExportPath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(ImportOrExportPath);
                    directoryInfo.Create();
                }
                //将文件重新命名，保证唯一性
                string pathFile = ImportOrExportPath + "success_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(fileName);
                //将上传的文件保存
                file.SaveAs(pathFile);
                //方式一：创建数据表
                DataTable dt = ExcelHelperNew.ExcelToTable(pathFile);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //用户名为必填项
                    string userName = dt.Rows[i]["*用户名"].ToString();
                    if (string.IsNullOrEmpty(userName))
                    {
                        list.Add(i);
                        continue;
                    }
                    //联系电话为必填项
                    string phone = dt.Rows[i]["*联系电话"].ToString();
                    if (string.IsNullOrEmpty(phone))
                    {
                        list.Add(i);
                        continue;
                    }
                    //出生日期
                    string birthDate = dt.Rows[i]["出生日期"].ToString();

                    //登录账号为必填项
                    string userAcct = dt.Rows[i]["*登录账号"].ToString();
                    if (string.IsNullOrEmpty(userAcct))
                    {
                        list.Add(i);
                        continue;
                    }
                    //所属单位为必填项
                    string companyName = dt.Rows[i]["*所属单位"].ToString();
                    if (string.IsNullOrEmpty(companyName))
                    {
                        list.Add(i);
                        continue;
                    }
                    //岗位为必填项
                    string post = dt.Rows[i]["*岗位"].ToString();
                    if (string.IsNullOrEmpty(post))
                    {
                        list.Add(i);
                        continue;
                    }
                    string userCode = dt.Rows[i]["用户编码"].ToString();//用户编码
                    string sex = dt.Rows[i]["性别"].ToString();//性别
                    string email = dt.Rows[i]["电子邮箱"].ToString();//电子邮箱
                    string university = dt.Rows[i]["毕业学校"].ToString();//毕业学校
                    string major = dt.Rows[i]["专业名称"].ToString();//专业名称
                    string education = dt.Rows[i]["学历"].ToString();//学历
                    string address = dt.Rows[i]["项目地区"].ToString();//项目地区

                    string professional = dt.Rows[i]["职称"].ToString();//职称
                    string professionalQualification = dt.Rows[i]["职业资质"].ToString();//职业资质
                    string occupationalStartTime = dt.Rows[i]["开始职业日期"].ToString();//开始职业日期
                    string occupationalContent = dt.Rows[i]["职业简述"].ToString();//职业简述
                    string achievement = dt.Rows[i]["成绩"].ToString();//成绩
                    string achievementStartTime = dt.Rows[i]["成绩开始日期"].ToString();//成绩开始日期
                    string achievementEndTime = dt.Rows[i]["成绩结束日期"].ToString();//成绩结束日期

                    //数据转换model
                    Base_User model = new Base_User();
                    if (!string.IsNullOrWhiteSpace(birthDate)) //出生日期
                    {
                        model.BirthDate = Convert.ToDateTime(birthDate);
                    }
                    model.UserAcct = userAcct ?? "";  //登录名
                    model.UserCode = userCode ?? ""; //用户编码
                    model.Sex = sex == "男" ? true : false; //性别
                    model.Email = email ?? ""; //电子邮箱
                    model.University = university ?? "";  //毕业学校
                    model.Major = major ?? "";  //专业名称
                    model.Education = education ?? "";  //学历
                    model.Address = address ?? ""; //项目地区
                    model.PassWord = DesTool.DesEncrypt("123456");
                    if (!string.IsNullOrWhiteSpace(occupationalStartTime))//开始职业日期
                    {
                        model.OccupationalStartTime = occupationalStartTime.ToDateTimeReq();
                    }
                    model.OccupationalContent = occupationalContent ?? ""; //职业简述
                    if (!string.IsNullOrWhiteSpace(achievement)) //成绩
                    {
                        model.achievement = achievement.ToDecimalReq();
                    }
                    if (!string.IsNullOrWhiteSpace(achievementStartTime))//成绩开始日期
                    {
                        model.achievementStartTime = achievementStartTime.ToDateTimeReq();
                    }
                    if (!string.IsNullOrWhiteSpace(achievementEndTime))//成绩结束日期
                    {
                        model.achievementEndTime = achievementEndTime.ToDateTimeReq();
                    }
                    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
                    {
                        List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.PostType, DictionaryType.ProfessionalType, DictionaryType.QualificationType };
                        var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                        var PostTypeList = subjects[DictionaryType.PostType].ToList();//岗位
                        var ProfessionalTypeList = subjects[DictionaryType.ProfessionalType].ToList();//职称
                        var QualificationTypeList = subjects[DictionaryType.QualificationType].ToList();//职业资格

                        var postInfo = PostTypeList.Where(t => t.Name == post).FirstOrDefault();
                        if (postInfo == null)  //岗位
                        {
                            list.Add(i);
                            continue;
                        }
                        else
                        {
                            model.Post = postInfo.No; //岗位
                            model.PostValue = postInfo.Name;
                        }
                        //职称信息
                        var titleInfo = ProfessionalTypeList.Where(t => t.Name == professional).FirstOrDefault();
                        if (titleInfo == null)  
                        {
                            list.Add(i);
                            continue;
                        }
                        else
                        {
                            model.Professional = titleInfo.No; 
                            model.ProfessionalValue = titleInfo.Name;
                        }

                        //职业资格信息
                        var zyzgInfo = QualificationTypeList.Where(t => t.Name == professional).FirstOrDefault();
                        if (zyzgInfo == null)
                        {
                            list.Add(i);
                            continue;
                        }
                        else
                        {
                            model.ProfessionalQualification = zyzgInfo.No;
                            model.ProfessionalQualificationValue = zyzgInfo.Name;
                        }

                        //根据单位名称查询对应单位，获取ID
                        Result<Base_Company> resultModel = proxy.GetCompanyByName(companyName);
                        if (resultModel.Flag == EResultFlag.Success && resultModel.Data != null)
                        {
                            var companyIds = resultModel.Data.Id;
                            //只能导入当前单位的人员数据
                            if (companyId == companyIds)  
                            {
                                model.CompanyId = companyIds;  //所属公司
                                userList.Add(model);
                            }
                            else
                            {
                                list.Add(i);
                                continue;
                            }
                        }
                        else {
                            list.Add(i);
                            continue;
                        }

                        //同一用户名称或者同一用户电话只能存在一条数据
                        var modelInfo = proxy.GetUserInfoByNameAndPhone(userName, phone);
                        if (modelInfo.Flag == EResultFlag.Success && modelInfo.Data != null)
                        {
                            list.Add(i);
                            continue;
                        }
                        else
                        {
                            model.UserName = userName ?? "";  //用户名
                            model.Phone = phone ?? "";   //电话
                            userList.Add(model);
                        }
                    }
                }

                if (userList.Count > 0)
                {
                    using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
                    {
                        var rows = proxy.AddRangeUser(userList);

                        result.Data = rows.Data;
                        result.Flag = EResultFlag.Success;
                    }
                }
                string errorPath = "";
                //导入数据异常时，记录错误信息进行导出，方便客户下次修改，再次执行导入。
                if (list.Count > 0)
                {
                    //待导出文件路径,将文件重新命名，保证唯一性
                    errorPath = ImportOrExportPath + "failure_" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(fileName);
                    //导出失败的Excel文档
                    ExcelHelperNew.TableToExcel(dt, errorPath, list);
                }

                return Json(new
                {
                    Data = result.Data,
                    Flag = result.Flag,
                    failureCount = list.Count,//导入失败条数
                    successCount = dt.Rows.Count - list.Count,//导入成功条数
                    download = errorPath,//失败文件下载链接
                    fileName = Path.GetFileName(errorPath)
                });

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "导入程序异常:" + ex.Message);

            }
            return Json(result.ToResultView());
        }
        #endregion
    }
}