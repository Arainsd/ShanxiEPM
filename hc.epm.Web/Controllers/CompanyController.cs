using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.UI.Common;
using hc.Plat.Common.Global;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Extend;
using Newtonsoft.Json;

namespace hc.epm.Web.Controllers
{
    public class CompanyController : BaseWebController
    {
        #region 改造前
        //单位列表
        public ActionResult Index(string name, string type, int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.type = type;
            ViewBag.pageIndex = pageIndex;
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            ce = new ConditionExpression();
            ce.ExpName = "Type";
            ce.ExpValue = RoleType.Admin.ToString();
            ce.ExpOperater = eConditionOperator.NotEqual;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);
            if (!string.IsNullOrWhiteSpace(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Name";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrWhiteSpace(type))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Type";
                ce.ExpValue = type;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                Result<List<Base_Company>> result = proxy.GetCompanyList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);

                List<string> removeValues = new List<string>();
                removeValues.Add(RoleType.Admin.ToString());
                ViewBag.CompanyType = typeof(RoleType).AsSelectList(true, "", removeValues);
                return View(result.Data);
            }
        }
        public ActionResult Add()
        {
            ViewBag.Title = "新增单位";
            return View();
        }
        /// <summary>
        /// 获取企业详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(long id)
        {
            Result<CompanyView> result = new Result<CompanyView>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompanyModel(id);
                if (string.IsNullOrEmpty(result.Data.PreCode))
                {
                    result.Data.PreCode = "无上级单位";
                }
                if (string.IsNullOrEmpty(result.Data.PreName))
                {
                    result.Data.PreName = "无上级单位";
                }
            }
            return View(result.Data);
        }

        #endregion

        #region 承包商
        /// <summary>
        /// 承包商管理列表
        /// 参数分别是名字，电话，地址，供应商类型，type类型是必须的
        /// </summary>
        /// <returns></returns>
        public ActionResult ContractorIndex(string name, string phone, string address, string companyRank = "", string companyType = "", string isblack = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.phone = phone;
            ViewBag.address = address;
            ViewBag.isblack = isblack;
            ViewBag.pageIndex = pageIndex;
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            //ce = new ConditionExpression();
            //ce.ExpName = "Type";
            //ce.ExpValue = RoleType.Admin.ToString();
            //ce.ExpOperater = eConditionOperator.NotEqual;
            //ce.ExpLogical = eLogicalOperator.And;
            //qc.ConditionList.Add(ce);
            if (!string.IsNullOrWhiteSpace(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Name";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            if (!string.IsNullOrWhiteSpace(companyRank))
            {
                ce = new ConditionExpression();
                ce.ExpName = "CompanyRank";
                ce.ExpValue = companyRank;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            if (!string.IsNullOrWhiteSpace(companyType))
            {
                ce = new ConditionExpression();
                ce.ExpName = "CompanyType";
                ce.ExpValue = companyType;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            if (!string.IsNullOrWhiteSpace(isblack))
            {
                bool black = isblack == "1" ? true : false;
                ce = new ConditionExpression();
                ce.ExpName = "IsBlacklist";
                ce.ExpValue = black;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Phone";
                ce.ExpValue = "%" + phone + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            if (!string.IsNullOrWhiteSpace(address))//地址模糊查询？
            {
                ce = new ConditionExpression();
                ce.ExpName = "AddressName";
                ce.ExpValue = "%" + address + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            ce = new ConditionExpression();
            ce.ExpName = "Type";
            ce.ExpValue = "SSCBS";
            ce.ExpOperater = eConditionOperator.Equal;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);

            ce = new ConditionExpression();
            ce.ExpName = "IsDelete";
            ce.ExpValue = false;
            ce.ExpOperater = eConditionOperator.Equal;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);

            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                Result<List<Base_Company>> result = proxy.GetCompanyListByType(qc);//查询
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);

                //加载数据字典
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ConclusionCode, DictionaryType.SSGLType, DictionaryType.LevelType };

                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //供应商类型
                ViewBag.CompanyType = subjects[DictionaryType.SSGLType].ToList().ToSelectList("Name", "No", true);

                //供应商级别
                ViewBag.CompanyRank = subjects[DictionaryType.LevelType].ToList().ToSelectList("Name", "No", true);

                //List<string> removeValues = new List<string>();
                //removeValues.Add(RoleType.Admin.ToString());
                //ViewBag.CompanyType = typeof(RoleType).AsSelectList(true, "", removeValues);
                return View(result.Data);
            }
        }




        /// <summary>
        /// 承包商管理新增
        /// </summary>
        /// <returns></returns>
        public ActionResult ContractorAdd()
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //加载数据字典
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ContractorFileType, DictionaryType.ConclusionCode, DictionaryType.SSGLType, DictionaryType.LevelType };

                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //供应商类型
                ViewBag.CompanyType = subjects[DictionaryType.SSGLType].ToList().ToSelectList("Name", "No", true);

                //供应商级别
                ViewBag.CompanyRank = subjects[DictionaryType.LevelType].ToList().ToSelectList("Name", "No", true);

                ViewBag.threeBusType = subjects[DictionaryType.ContractorFileType].ToList().ToSelectList("Name", "No", true);
            }
            return View();
        }
        [HttpPost]
        public ActionResult ContractorAdd(Base_Company model)
        {
            Result<int> result = new Result<int>();
            List<Base_Files> fileListFile = new List<Base_Files>();

            model.Type = "SSCBS";
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.baseFiles = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
                //foreach (var item in model.baseFiles)
                //{
                //    item.TableColumn = "SSCBS";
                //}
            }


            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddCompany(model);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 承包商管理编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult ContractorEdit(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //加载数据字典
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ContractorFileType, DictionaryType.ConclusionCode, DictionaryType.SSGLType, DictionaryType.LevelType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                ViewBag.threeBusType = subjects[DictionaryType.ContractorFileType].ToList().ToSelectList("Name", "No", true);

                var result = proxy.GetCompanyModel(id);//获取详情

                //供应商类型
                ViewBag.CompanyType = subjects[DictionaryType.SSGLType].ToList().ToSelectList("Name", "No", true, result.Data.CompanyType);

                //供应商级别
                ViewBag.CompanyRank = subjects[DictionaryType.LevelType].ToList().ToSelectList("Name", "No", true, result.Data.CompanyRank);



                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }
                return View();
            }
        }
        [HttpPost]
        public ActionResult ContractorEdit(Base_Company model)
        {
            Result<int> result = new Result<int>();
            List<Base_Files> fileListFile = new List<Base_Files>();

            model.Type = "SSCBS";
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.baseFiles = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            }
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddCompany(model);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 承包商管理详情
        /// </summary>
        /// <returns></returns>
        public ActionResult ContractorDetail(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetCompanyModel(id);//
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }

                return View();
            }
        }

        /// <summary>
        /// 批量删除承包商
        /// </summary>
        /// <param name="ids">主键id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> idList = ids.SplitString(",").ToLongList();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteCompanyByIds(idList);//删除
            }
            return Json(result.ToResultView());
        }

        #endregion

        #region 服务商
        /// <summary>
        /// 服务商管理列表
        /// </summary>
        /// <returns></returns>
        public ActionResult ServiceProviderIndex(string name, string phone, string address, string companyRank = "", string companyType = "", string isblack = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.phone = phone;
            ViewBag.address = address;
            ViewBag.isblack = isblack;
            ViewBag.pageIndex = pageIndex;
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            //ce = new ConditionExpression();
            //ce.ExpName = "Type";
            //ce.ExpValue = RoleType.Admin.ToString();
            //ce.ExpOperater = eConditionOperator.NotEqual;
            //ce.ExpLogical = eLogicalOperator.And;
            //qc.ConditionList.Add(ce);
            if (!string.IsNullOrWhiteSpace(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Name";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Phone";
                ce.ExpValue = "%" + phone + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            if (!string.IsNullOrWhiteSpace(companyRank))
            {
                ce = new ConditionExpression();
                ce.ExpName = "CompanyRank";
                ce.ExpValue = companyRank;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            if (!string.IsNullOrWhiteSpace(companyType))
            {
                ce = new ConditionExpression();
                ce.ExpName = "CompanyType";
                ce.ExpValue = companyType;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            if (!string.IsNullOrWhiteSpace(isblack))
            {
                bool black = isblack == "1" ? true : false;
                ce = new ConditionExpression();
                ce.ExpName = "IsBlacklist";
                ce.ExpValue = black;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            if (!string.IsNullOrWhiteSpace(address))//地址模糊查询？
            {
                ce = new ConditionExpression();
                ce.ExpName = "AddressName";
                ce.ExpValue = "%" + address + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            ce = new ConditionExpression();
            ce.ExpName = "Type";
            ce.ExpValue = "SSFWS";
            ce.ExpOperater = eConditionOperator.Equal;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);
            ce = new ConditionExpression();
            ce.ExpName = "IsDelete";
            ce.ExpValue = false;
            ce.ExpOperater = eConditionOperator.Equal;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                Result<List<Base_Company>> result = proxy.GetCompanyListByType(qc);//查询
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);

                //加载数据字典
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ConclusionCode, DictionaryType.SSGLType, DictionaryType.LevelType };

                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //供应商类型
                ViewBag.CompanyType = subjects[DictionaryType.SSGLType].ToList().ToSelectList("Name", "No", true);

                //供应商级别
                ViewBag.CompanyRank = subjects[DictionaryType.LevelType].ToList().ToSelectList("Name", "No", true);

                //List<string> removeValues = new List<string>();
                //removeValues.Add(RoleType.Admin.ToString());
                //ViewBag.CompanyType = typeof(RoleType).AsSelectList(true, "", removeValues);
                return View(result.Data);
            }
        }

        /// <summary>
        /// 服务商管理新增
        /// </summary>
        /// <returns></returns>
        public ActionResult ServiceProviderAdd()
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //加载数据字典
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ServiceProviderFileType, DictionaryType.ConclusionCode, DictionaryType.SSGLType, DictionaryType.LevelType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                //供应商类型
                ViewBag.CompanyType = subjects[DictionaryType.SSGLType].ToList().ToSelectList("Name", "No", true);

                //供应商级别
                ViewBag.CompanyRank = subjects[DictionaryType.LevelType].ToList().ToSelectList("Name", "No", true);
                ViewBag.threeBusType = subjects[DictionaryType.ServiceProviderFileType].ToList().ToSelectList("Name", "No", true);
            }
            return View();
        }

        [HttpPost]
        public ActionResult ServiceProviderAdd(Base_Company model)
        {
            Result<int> result = new Result<int>();
            List<Base_Files> fileListFile = new List<Base_Files>();

            //服务商
            model.Type = "SSFWS";
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.baseFiles = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            }


            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddCompany(model);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 服务商管理编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult ServiceProviderEdit(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //加载数据字典
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ServiceProviderFileType, DictionaryType.ConclusionCode, DictionaryType.SSGLType, DictionaryType.LevelType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                var result = proxy.GetCompanyModel(id);//获取详情
                //供应商类型
                ViewBag.CompanyType = subjects[DictionaryType.SSGLType].ToList().ToSelectList("Name", "No", true, result.Data.CompanyType);

                //供应商级别
                ViewBag.CompanyRank = subjects[DictionaryType.LevelType].ToList().ToSelectList("Name", "No", true, result.Data.CompanyRank);
                ViewBag.threeBusType = subjects[DictionaryType.ServiceProviderFileType].ToList().ToSelectList("Name", "No", true);



                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }
                return View();
            }
        }
        [HttpPost]
        public ActionResult ServiceProviderEdit(Base_Company model)
        {
            Result<int> result = new Result<int>();
            List<Base_Files> fileListFile = new List<Base_Files>();

            model.Type = "SSFWS";
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.baseFiles = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            }
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddCompany(model);
            }
            return Json(result.ToResultView());
        }

        /// <summary>
        /// 服务商管理详情
        /// </summary>
        /// <returns></returns>
        public ActionResult ServiceProviderDetail(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetCompanyModel(id);//
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }

                return View();
            }
        }

        [HttpPost]
        public ActionResult ServiceDelete(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> idList = ids.SplitString(",").ToLongList();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteCompanyByIds(idList);//删除
            }
            return Json(result.ToResultView());
        }
        #endregion

        #region 供应商
        /// <summary>
        /// 供应商管理列表
        /// </summary>
        /// <returns></returns>
        public ActionResult SupplierIndex(string name, string phone, string address, string companyRank = "", string companyType = "", string isblack = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.phone = phone;
            ViewBag.address = address;
            ViewBag.isblack = isblack;
            ViewBag.pageIndex = pageIndex;
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            //ce = new ConditionExpression();
            //ce.ExpName = "Type";
            //ce.ExpValue = RoleType.Admin.ToString();
            //ce.ExpOperater = eConditionOperator.NotEqual;
            //ce.ExpLogical = eLogicalOperator.And;
            //qc.ConditionList.Add(ce);
            if (!string.IsNullOrWhiteSpace(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Name";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Phone";
                ce.ExpValue = "%" + phone + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            if (!string.IsNullOrWhiteSpace(address))//地址模糊查询？
            {
                ce = new ConditionExpression();
                ce.ExpName = "AddressName";
                ce.ExpValue = "%" + address + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            ce = new ConditionExpression();
            ce.ExpName = "Type";
            ce.ExpValue = "\'" + "Supplier" + "\'" + "," + "\'" + "SSGYS" + "\'";
            ce.ExpOperater = eConditionOperator.In;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);

            ce = new ConditionExpression();
            ce.ExpName = "IsDelete";
            ce.ExpValue = false;
            ce.ExpOperater = eConditionOperator.Equal;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);

            if (!string.IsNullOrWhiteSpace(companyRank))
            {
                ce = new ConditionExpression();
                ce.ExpName = "CompanyRank";
                ce.ExpValue = companyRank;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            if (!string.IsNullOrWhiteSpace(companyType))
            {
                ce = new ConditionExpression();
                ce.ExpName = "CompanyType";
                ce.ExpValue = companyType;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            if (!string.IsNullOrWhiteSpace(isblack))
            {
                bool black = isblack == "1" ? true : false;
                ce = new ConditionExpression();
                ce.ExpName = "IsBlacklist";
                ce.ExpValue = black;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                Result<List<Base_Company>> result = proxy.GetCompanyListByType(qc);//查询
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);

                //加载数据字典
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.ConclusionCode, DictionaryType.SSGLType, DictionaryType.LevelType };

                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;

                //供应商类型
                ViewBag.CompanyType = subjects[DictionaryType.SSGLType].ToList().ToSelectList("Name", "No", true);

                //供应商级别
                ViewBag.CompanyRank = subjects[DictionaryType.LevelType].ToList().ToSelectList("Name", "No", true);

                //List<string> removeValues = new List<string>();
                //removeValues.Add(RoleType.Admin.ToString());
                //ViewBag.CompanyType = typeof(RoleType).AsSelectList(true, "", removeValues);
                return View(result.Data);
            }
        }

        /// <summary>
        /// 供应商管理新增
        /// </summary>
        /// <returns></returns>
        public ActionResult SupplierAdd()
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //加载数据字典
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.SupplierFileType, DictionaryType.ConclusionCode, DictionaryType.SSGLType, DictionaryType.LevelType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                //供应商类型
                ViewBag.CompanyType = subjects[DictionaryType.SSGLType].ToList().ToSelectList("Name", "No", true);

                //供应商级别
                ViewBag.CompanyRank = subjects[DictionaryType.LevelType].ToList().ToSelectList("Name", "No", true);

                ViewBag.threeBusType = subjects[DictionaryType.SupplierFileType].ToList().ToSelectList("Name", "No", true);
            }
            return View();
        }
        [HttpPost]
        public ActionResult SupplierAdd(Base_Company model)
        {
            Result<int> result = new Result<int>();
            List<Base_Files> fileListFile = new List<Base_Files>();

            //服务商
            model.Type = "SSGYS";
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.baseFiles = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            }


            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddCompany(model);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 供应商管理编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult SupplierEdit(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //加载数据字典
                List<DictionaryType> subjectsList = new List<DictionaryType>() { DictionaryType.SupplierFileType, DictionaryType.ConclusionCode, DictionaryType.SSGLType, DictionaryType.LevelType };
                var subjects = proxy.GetTypeListByTypes(subjectsList).Data;
                var result = proxy.GetCompanyModel(id);//获取详情
                //供应商类型
                ViewBag.CompanyType = subjects[DictionaryType.SSGLType].ToList().ToSelectList("Name", "No", true, result.Data.CompanyType);

                //供应商级别
                ViewBag.CompanyRank = subjects[DictionaryType.LevelType].ToList().ToSelectList("Name", "No", true, result.Data.CompanyRank);

                ViewBag.threeBusType = subjects[DictionaryType.SupplierFileType].ToList().ToSelectList("Name", "No", true);


                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }
                return View();
            }
        }
        [HttpPost]
        public ActionResult SupplierEdit(Base_Company model)
        {
            Result<int> result = new Result<int>();
            List<Base_Files> fileListFile = new List<Base_Files>();

            model.Type = "SSGYS";
            string fileDataJson = Request.Form["fileDataJsonFile"];//获取上传文件json字符串
            if (!string.IsNullOrEmpty(fileDataJson))
            {
                model.baseFiles = JsonConvert.DeserializeObject<List<Base_Files>>(fileDataJson);//将文件信息json字符
            }
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.AddCompany(model);
            }
            return Json(result.ToResultView());
        }
        /// <summary>
        /// 供应商管理详情
        /// </summary>
        /// <returns></returns>
        public ActionResult SupplierDetail(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetCompanyModel(id);//
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }

                return View();
            }
        }
        [HttpPost]
        public ActionResult SupplierDelete(string ids)
        {
            Result<int> result = new Result<int>();
            List<long> idList = ids.SplitString(",").ToLongList();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.DeleteCompanyByIds(idList);//删除
            }
            return Json(result.ToResultView());
        }
        #endregion


    }
}