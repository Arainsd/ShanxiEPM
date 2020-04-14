using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.ViewModel;
using hc.epm.Web.ClientProxy;
using hc.Plat.Common.Global;
using hc.Plat.Common.Extend;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
//using Microsoft.Office.Interop.MSProject;

namespace hc.epm.Web.Controllers
{
    public class CommController : BaseWebController
    {
        public ActionResult Test()
        {
            return View();
        }

        /// <summary>
        /// 确认终结
        /// </summary>
        /// <returns></returns>
        public ActionResult ConfirmSummary()
        {
            return View();
        }

        /// <summary>
        /// 选择服务商用户
        /// </summary>
        /// <param name="companyId">项目Id</param>
        /// <param name="selectType">1：单选；2：多选</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页显示条数</param>
        /// <returns></returns>
        public ActionResult SelectServiceUser(long companyId = 0, string name = "", string selectType = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.projectId = companyId;
            ViewBag.userName = name;
            ViewBag.SelectType = selectType;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            QueryCondition qc = new QueryCondition();
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            if (companyId != 0)
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "CompanyId",
                    ExpValue = companyId,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
            }
            if (!string.IsNullOrWhiteSpace(name))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "userName",
                    ExpValue = string.Format("%{0}%", name),
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }

            Result<List<Base_User>> result = new Result<List<Base_User>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetUserList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
                return View(result.Data);
            }
        }

        /// <summary>
        /// 选择工程处用户
        /// </summary>
        /// <param name="preCode">100101 本省公司机关本部</param>
        /// <param name="selectType">1：单选；2：多选</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页显示条数</param>
        /// <returns></returns>
        public ActionResult SelectPM(string userName = "", long companyId = 0, string selectType = "1", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.userName = userName;
            ViewBag.SelectType = selectType;
            ViewBag.companyId = companyId;

            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            Result<List<Base_User>> result = new Result<List<Base_User>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                if (companyId == 0)
                {
                    result = proxy.GetAgencyPMList(userName, pageIndex, pageSize);
                }
                else
                {
                    result = proxy.GetBrCompanyPMList(userName, companyId, pageIndex, pageSize);
                }
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
                return View(result.Data);
            }
        }

        /// <summary>
        /// 选择服务商（承包商）
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectContractor(string name = "", string selectType = "1", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.selectType = selectType;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            QueryCondition qc = new QueryCondition();
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "Type",
                ExpValue = "\'" + "SSCBS" + "\'" + "," + "\'" + "SSCBS" + "\'",
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.In
            });
            if (!string.IsNullOrEmpty(name))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Name",
                    ExpValue = string.Format("%{0}%", name),
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "IsBlacklist",
                ExpValue = false,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            qc.SortList.Add(new SortExpression("Code", eSortType.Asc));

            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompanyList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 选择分公司
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectBranchCompany(string name = "", string selectType = "1", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.selectType = selectType;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            QueryCondition qc = new QueryCondition();
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "Type",
                ExpValue = "Owner",
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "PId",
                ExpValue = Convert.ToInt64(10),
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "OrgType",
                ExpValue = "2",
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "OrgState",
                ExpValue = "1",
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            if (!string.IsNullOrEmpty(name))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Name",
                    ExpValue = string.Format("%{0}%", name),
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "IsBlacklist",
                ExpValue = false,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });

            qc.SortList.Add(new SortExpression("Code", eSortType.Asc));

            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompanyList(qc);

                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 选择服务商（供应商）
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectServiceCustomer(string name = "", string selectType = "1", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.selectType = selectType;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            QueryCondition qc = new QueryCondition();
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            //List<string> ll = new List<string>();
            //ll.Add("Supplier");
            //ll.Add("SSGYS");
            //qc.ConditionList.Add(new ConditionExpression()
            //{
            //    ExpName = "Type",
            //    ExpValue = "Supplier",
            //    ExpLogical = eLogicalOperator.And,
            //    ExpOperater = eConditionOperator.Equal
            //});
            //qc.ConditionList.Add(new ConditionExpression()
            //{
            //    ExpName = "Type",
            //    ExpValue = "SSGYS",
            //    ExpLogical = eLogicalOperator.Or,
            //    ExpOperater = eConditionOperator.Equal
            //});


            ConditionExpression ce1 = new ConditionExpression();
            ce1.ExpLogical = eLogicalOperator.And;
            ce1.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "Type",
                ExpValue = "Supplier",
                ExpOperater = eConditionOperator.Equal
            });
            ce1.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "Type",
                ExpValue = "SSGYS",
                ExpLogical = eLogicalOperator.Or,
                ExpOperater = eConditionOperator.Equal
            });
            qc.ConditionList.Add(ce1);
            if (!string.IsNullOrEmpty(name))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Name",
                    ExpValue = string.Format("%{0}%", name),
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }

            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "IsBlacklist",
                ExpValue = false,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            qc.SortList.Add(new SortExpression("Code", eSortType.Asc));

            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompanyList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 选择服务商
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectCustomerByNameOrType(string name = "", string CompanyType = "", string selectType = "1", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.selectType = selectType;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.CompanyType = CompanyType;

            QueryCondition qc = new QueryCondition();
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            if (!string.IsNullOrEmpty(CompanyType))
            {
                if (CompanyType == "SSGYS")//供应商
                {
                    ConditionExpression ce1 = new ConditionExpression();
                    ce1.ExpLogical = eLogicalOperator.And;
                    ce1.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "Type",
                        ExpValue = "Supplier",
                        ExpOperater = eConditionOperator.Equal
                    });
                    ce1.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "Type",
                        ExpValue = "SSGYS",
                        ExpLogical = eLogicalOperator.Or,
                        ExpOperater = eConditionOperator.Equal
                    });
                    qc.ConditionList.Add(ce1);
                }
                else
                {
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "Type",
                        ExpValue = CompanyType,
                        ExpLogical = eLogicalOperator.And,
                        ExpOperater = eConditionOperator.Equal
                    });
                }
            }

            if (!string.IsNullOrEmpty(name))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Name",
                    ExpValue = string.Format("%{0}%", name),
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "IsBlacklist",
                ExpValue = false,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            qc.SortList.Add(new SortExpression("Code", eSortType.Asc));

            ViewBag.CompanyType = typeof(CompanyType).AsSelectList(false);

            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompanyList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 选择服务商（All）
        /// </summary>
        /// <param name="name">服务商名称</param>
        /// <param name="type">服务商类型</param>
        /// <param name="selectType">1：单选，2：多选</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectServiceAll(string name = "", string type = "", string selectType = "1", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.selectType = selectType;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            QueryCondition qc = new QueryCondition();
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            if (!string.IsNullOrEmpty(type))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Type",
                    ExpValue = type,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
            }
            if (!string.IsNullOrEmpty(name))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Name",
                    ExpValue = string.Format("%{0}%", name),
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "IsBlacklist",
                ExpValue = false,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            qc.SortList.Add(new SortExpression("Code", eSortType.Asc));

            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompanyList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }

            //公司类型
            ViewBag.CompanyType = typeof(CompanyType).AsSelectList(true);
            return View(result.Data);
        }

        /// <summary>
        /// 获取服务商（如总批复构成未关联服务商则查询所有服务商信息）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="selectType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetServiceCustomerList(string name = "", string constituteName = "", string selectType = "1", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.selectType = selectType;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.name = name;
            ViewBag.constituteName = constituteName;

            #region 查询所有服务商条件
            QueryCondition qc = new QueryCondition();
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            if (!string.IsNullOrEmpty(name))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Name",
                    ExpValue = string.Format("%{0}%", name),
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "Type",
                ExpValue = "Supplier",
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            #endregion

            #region 查询已关联服务商条件
            QueryCondition qc1 = new QueryCondition();
            qc1.PageInfo = GetPageInfo(pageIndex, pageSize);
            if (!string.IsNullOrEmpty(name))
            {
                qc1.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Name",
                    ExpValue = string.Format("%{0}%", name),
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            if (!string.IsNullOrEmpty(constituteName))
            {
                qc1.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ConstituteName",
                    ExpValue = constituteName,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
            }
            #endregion
            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                List<Epm_ConstituteCompanyDetails> list = proxy.GetConstituteCompanyDetailsList(qc1).Data;
                if (list != null && list.Count() > 0)
                {
                    List<Base_Company> comList = new List<Base_Company>();
                    foreach (var item in list)
                    {
                        Base_Company com = new Base_Company();
                        com.Name = item.CompanyName;
                        com.Id = item.CompanyId.Value;

                        comList.Add(com);
                    }
                    result.Data = comList;
                    result.AllRowsCount = result.Data.Count();
                }
                else
                {
                    result = proxy.GetCompanyList(qc);
                }
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }


        /// <summary>
        /// 选择服务商（项目）
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectProjectCustomer(long projectId = 0, string selectType = "1", string name = "", string companyType = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.selectType = selectType;
            ViewBag.projectId = projectId;
            ViewBag.companyType = companyType;

            ViewBag.name = name;

            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            QueryCondition qc = new QueryCondition();
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            Result<List<Epm_ProjectCompany>> result = new Result<List<Epm_ProjectCompany>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProjectCompanyListByName(projectId, name);
                if (!string.IsNullOrEmpty(companyType))
                {
                    result.Data = result.Data.Where(t => t.CompanyId.HasValue && companyType.Contains(t.Type)).ToList();
                }
                List<Epm_ProjectCompany> comList = new List<Epm_ProjectCompany>();
                if (result.Data.Count > 0)
                {
                    foreach (var item in result.Data)
                    {
                        var isCom = comList.Where(t => t.CompanyId == item.CompanyId);
                        if (isCom.Count() == 0)
                        {
                            Epm_ProjectCompany com = new Epm_ProjectCompany();
                            if (item.CompanyId != null || !string.IsNullOrEmpty(item.CompanyName))
                            {
                                com.CompanyId = item.CompanyId;
                                com.CompanyName = item.CompanyName;
                                comList.Add(com);
                            }
                        }
                    }
                }
                result.Data = comList.ToList();

                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 选择项目
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectProject(string name = "", string selectType = "1", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.SelectType = selectType;
            ViewBag.name = name;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            QueryCondition qc = new QueryCondition
            {
                PageInfo = GetPageInfo(pageIndex, pageSize)
            };
            if (!string.IsNullOrWhiteSpace(name))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Name",
                    ExpValue = string.Format("%{0}%", name),
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }

            Result<List<Epm_Project>> result = new Result<List<Epm_Project>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProjectListByQc(qc);

                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 获取所有项目
        /// </summary>
        /// <param name="name"></param>
        /// <param name="selectType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectProjectAll(string name = "", string selectType = "1", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.SelectType = selectType;
            ViewBag.name = name;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            QueryCondition qc = new QueryCondition
            {
                PageInfo = GetPageInfo(pageIndex, pageSize)
            };
            if (!string.IsNullOrWhiteSpace(name))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Name",
                    ExpValue = string.Format("%{0}%", name),
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }

            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "State",
                ExpValue = 5,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });

            Result<List<Epm_Project>> result = new Result<List<Epm_Project>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProjectList(qc);

                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 选择加油站列表
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectOilStation(string name = "", string selectType = "1", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.SelectType = selectType;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            QueryCondition qc = new QueryCondition();
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            if (!string.IsNullOrWhiteSpace(name))
            {
                ConditionExpression ce = new ConditionExpression();
                ce.ExpName = "Name";
                ce.ExpValue = string.Format("%{0}%", name);
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
                qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            }

            Result<List<Epm_OilStation>> result = new Result<List<Epm_OilStation>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetOilStationList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 选择项目资料
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectData(string name = "", string selectType = "1", int State = 0, int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.SelectType = selectType;
            ViewBag.name = name;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            QueryCondition qc = new QueryCondition();
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);

            ConditionExpression ce = null;
            if (!string.IsNullOrWhiteSpace(name))
            {
                ce = new ConditionExpression
                {
                    ExpName = "Name",
                    ExpValue = "%" + name + "%",
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                };
                qc.ConditionList.Add(ce);

            }

            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "State",
                ExpValue = State,
                ExpOperater = eConditionOperator.Equal,
                ExpLogical = eLogicalOperator.And

            });

            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            Result<List<DataConfigView>> result = new Result<List<DataConfigView>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                List<DataConfigView> lview = new List<DataConfigView>();
                var list = proxy.GetDataConfigList(qc).Data;

                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        DataConfigView dataConfig = new DataConfigView();

                        dataConfig.DataConfig = list[i];
                        var dataId = list[i].Id;
                        var mi = proxy.GetMDataByDataId(dataId);

                        if (mi.Data != null)
                        {
                            dataConfig.MilepostId = mi.Data.MilepostId;
                            dataConfig.MilepostName = mi.Data.MilepostName;
                        }

                        lview.Add(dataConfig);
                    }
                }

                result.Data = lview;
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            var listData = result.Data;
            ViewBag.list = JsonConvert.SerializeObject(listData);

            return View(result.Data);
        }

        /// <summary>
        /// 关联里程碑
        /// </summary>
        /// <returns></returns>
        public ActionResult SelectMilestone()
        {
            Result<List<MilepostView>> result = new Result<List<MilepostView>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetMilepostViewList();
            }

            var list = result.Data;
            ViewBag.list = JsonConvert.SerializeObject(list);

            return View(result.Data);
        }

        /// <summary>
        /// 获取模板列表
        /// </summary>
        /// <param name="SelectType">[1：单选；2：多选]</param>
        /// <param name="TemplateTypeNo">[AQJC：安全检查；ZLJC：质量检查；ZXYS：专项验收；AQPX：安全培训；ZLPX：质量培训]</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult GetTemplateList(string SelectType = "1", string TemplateTypeNo = "", string title = "", int pageIndex = 1, int pageSize = 10)
        {
            Result<List<Epm_Template>> result = new Result<List<Epm_Template>>();
            ViewBag.pageIndex = pageIndex;
            ViewBag.SelectType = SelectType;
            ViewBag.pageSize = pageSize;
            ViewBag.title = title;
            ViewBag.TemplateTypeNo = TemplateTypeNo;
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                //根据字典获取字典ID
                var dic = proxy.GetDictionaryId(TemplateTypeNo);
                if (dic.Data != null)
                {
                    result = proxy.GetTemplateListDicId(dic.Data.Id, title);
                }
            }
            return View(result.Data);
        }

        /// <summary>
        /// 根据模板ID获取模板详情列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetTemplateDetailsList(long id)
        {
            Result<List<Epm_TemplateDetails>> result = new Result<List<Epm_TemplateDetails>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetTemplateDetailsList(id);
            }

            return Json(result.Data);
        }

        /// <summary>
        /// 查看里程碑
        /// </summary>
        /// <returns></returns>
        public ActionResult Milestone_del()
        {
            return View();
        }

        /// <summary>
        /// 关联BIM模型
        /// </summary>
        /// <returns></returns>
        public ActionResult RelationBIM(long projectId, long bimId = 0, int isLook = 1)
        {
            ViewBag.isLook = isLook;
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                Result<List<Epm_Bim>> list = proxy.GetBimModelListByProjectId(projectId);
                if (list.Flag == EResultFlag.Success && list.Data != null)
                {
                    var lastItem = list.Data.OrderByDescending(t => t.CreateTime).FirstOrDefault();
                    string defaultSelect = bimId == 0 ? (lastItem == null ? "" : lastItem.Id.ToString()) : bimId.ToString();
                    ViewBag.BimList = list.Data.ToSelectList("Name", "Id", false, defaultSelect);
                }
                else
                {
                    ViewBag.BimList = new List<Epm_Bim>();
                }
            }
            return View();
        }

        /// <summary>
        /// 获取BIM模型地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetBimUrl(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                Result<Epm_Bim> result = proxy.GetBimModel(id);

                string bimAddress = "";
                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    bimAddress = result.Data.BIMAddress ?? "";
                }
                return Json(bimAddress);
            }
        }

        /// <summary>
        /// 选择合同
        /// </summary>
        /// <param name="name"></param>
        /// <param name="selectType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectContract(string name = "", string selectType = "1", int contractType = 3, long SecondPartyId = 0, int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.SelectType = selectType;
            ViewBag.name = name;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            //ViewBag.contractType = contractType;
            ViewBag.SecondPartyId = SecondPartyId;

            #region  查询条件
            QueryCondition qc = new QueryCondition
            {
                PageInfo = GetPageInfo(pageIndex, pageSize)
            };
            if (!string.IsNullOrWhiteSpace(name))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Name",
                    ExpValue = string.Format("%{0}%", name),
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "State",
                ExpValue = (int)ApprovalState.ApprSuccess,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            //if (contractType == 3)
            //{
            //    qc.ConditionList.Add(new ConditionExpression()
            //    {
            //        ExpName = "ContractType",
            //        ExpValue = contractType,
            //        ExpLogical = eLogicalOperator.And,
            //        ExpOperater = eConditionOperator.Equal
            //    });
            //}
            //else
            //{
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "ContractType",
                ExpValue = 3,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.NotEqual
            });
            //}

            if (SecondPartyId != 0)
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "SecondPartyId",
                    ExpValue = SecondPartyId,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
            }
            #endregion

            Result<List<ContractView>> result = new Result<List<ContractView>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                List<ContractView> viewList = new List<ContractView>();
                var conList = proxy.GetContractList(qc);
                if (conList.Data.Count() > 0)
                {
                    for (int i = 0; i < conList.Data.Count(); i++)
                    {
                        var con = proxy.GetFilesByTable("Epm_Contract", conList.Data[i].Id);
                        ContractView view = new ContractView();
                        view.Id = conList.Data[i].Id;
                        view.Name = conList.Data[i].Name;
                        view.Code = conList.Data[i].Code;
                        view.ContractType = conList.Data[i].ContractType.HasValue ? conList.Data[i].ContractType.Value : 0;
                        if (con.Data.Count() > 0)
                        {
                            string str = "";
                            view.FileName = con.Data.Where(t => string.IsNullOrEmpty(t.ImageType)).Select(t => t.Name).ToList();
                            foreach (var item in con.Data)
                            {
                                if (string.IsNullOrEmpty(item.ImageType))
                                {
                                    str = str + item.Name + ',';
                                }
                            }
                            view.FileNameStr = str;
                            viewList.Add(view);
                        }
                    }
                }
                result.Data = viewList;
                ViewBag.Total = result.Data.Count();
                ViewBag.TotalPage = Math.Ceiling((decimal)result.Data.Count() / pageSize);
            }
            return View(result.Data);
        }


        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userName">用户名称</param>
        /// <param name="companyId">所属公司</param>
        /// <param name="dempName">所属部门</param>
        /// <param name="postName">岗位</param>
        /// <param name="selectType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectUser(string userName = "", long companyId = 0, string dempName = "", string phone = "", string postName = "", string achievementEndTime = "", string selectType = "1", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.userName = userName;
            ViewBag.SelectType = selectType;
            ViewBag.companyId = companyId;
            ViewBag.dempName = dempName;
            ViewBag.postName = postName;
            ViewBag.phone = phone;

            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            #region 查询条件
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            if (!string.IsNullOrEmpty(userName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "UserName";
                ce.ExpValue = userName;
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(phone))
            {
                ce = new ConditionExpression();
                ce.ExpName = "Phone";
                ce.ExpValue = phone;
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (companyId != 0)
            {
                ce = new ConditionExpression();
                ce.ExpName = "CompanyId";
                ce.ExpValue = companyId;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(dempName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "DempName";
                ce.ExpValue = dempName;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            if (!string.IsNullOrEmpty(postName))
            {
                ce = new ConditionExpression();
                ce.ExpName = "PostValue";
                ce.ExpValue = postName;
                ce.ExpOperater = eConditionOperator.Equal;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            //ce = new ConditionExpression();
            //ce.ExpName = "achievementEndTime";
            //ce.ExpValue = achievementEndTime;
            //ce.ExpOperater = eConditionOperator.LessThanOrEqual;
            //ce.ExpLogical = eLogicalOperator.And;
            //qc.ConditionList.Add(ce);

            qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            #endregion

            Result<List<UserView>> result = new Result<List<UserView>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetUserListByDepartment(qc);

                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
                return View(result.Data);
            }
        }

        /// <summary>
        /// 获取加油站项目列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="selectType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectTzGasStation(string name = "", string selectType = "1", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.SelectType = selectType;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            #region 查询条件
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            if (!string.IsNullOrEmpty(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "ProjectName";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }

            int d = (int)PreProjectState.ApprovalSuccess;

            ce = new ConditionExpression();
            ce.ExpName = "State";
            ce.ExpValue = d;
            ce.ExpOperater = eConditionOperator.Equal;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);

            qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            #endregion

            Result<List<Epm_TzProjectProposal>> result = new Result<List<Epm_TzProjectProposal>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetSingleTzProjectProposalList(qc);

                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
                return View(result.Data);
            }
        }

        /// <summary>
        /// 获取库站改造列表
        /// </summary>
        /// <param name="name"></param>
        /// <param name="selectType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectReformRecord(string name = "", string selectType = "1", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.name = name;
            ViewBag.SelectType = selectType;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            #region 查询条件
            QueryCondition qc = new QueryCondition();
            ConditionExpression ce = null;
            if (!string.IsNullOrEmpty(name))
            {
                ce = new ConditionExpression();
                ce.ExpName = "ProjectName";
                ce.ExpValue = "%" + name + "%";
                ce.ExpOperater = eConditionOperator.Like;
                ce.ExpLogical = eLogicalOperator.And;
                qc.ConditionList.Add(ce);
            }
            qc.SortList.Add(new SortExpression("Id", eSortType.Desc));
            qc.PageInfo = GetPageInfo(pageIndex, pageSize);
            #endregion

            Result<List<Epm_ReformRecord>> result = new Result<List<Epm_ReformRecord>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetReformRecordList(qc);

                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
                return View(result.Data);
            }
        }

        public ActionResult SelectTzGasReform()
        {
            ViewBag.pageIndex = 1;
            ViewBag.pageSize = 10;
            return View();
        }


        /// <summary>
        /// 选择投资管理批复完成项目
        /// </summary>
        /// <param name="name"></param>
        /// <param name="selectType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectTzProjectProposal(string name = "", string selectType = "1", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.SelectType = selectType;
            ViewBag.name = name;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            QueryCondition qc = new QueryCondition
            {
                PageInfo = GetPageInfo(pageIndex, pageSize)
            };
            if (!string.IsNullOrWhiteSpace(name))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ProjectName",
                    ExpValue = string.Format("%{0}%", name),
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            Result<List<Epm_TzProjectProposal>> result = new Result<List<Epm_TzProjectProposal>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetProjectProposalList(qc);

                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 选择部门
        /// </summary>
        /// <param name="name"></param>
        /// <param name="selectType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectDepartment(string name = "", string selectType = "1", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.SelectType = selectType;
            ViewBag.name = name;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            QueryCondition qc = new QueryCondition
            {
                PageInfo = GetPageInfo(pageIndex, pageSize)
            };
            if (!string.IsNullOrWhiteSpace(name))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Name",
                    ExpValue = string.Format("%{0}%", name),
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }

            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "PId",
                ExpValue = CurrentUser.CompanyId,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });

            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "Type",
                ExpValue = "Owner",
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });

            Result<List<Base_Company>> result = new Result<List<Base_Company>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetCompanyList(qc);

                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }

        /// <summary>
        /// 选择物资单
        /// </summary>
        /// <param name="name"></param>
        /// <param name="selectType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectTzSupplyMaterialApply(string name = "", string projectId = "", string selectType = "1", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.SelectType = selectType;
            ViewBag.projectId = projectId;
            ViewBag.name = name;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;

            QueryCondition qc = new QueryCondition
            {
                PageInfo = GetPageInfo(pageIndex, pageSize)
            };
            if (!string.IsNullOrWhiteSpace(name))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Name",
                    ExpValue = string.Format("%{0}%", name),
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Like
                });
            }
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "UseType",
                ExpValue = false,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            int state = (int)PreProjectState.ApprovalSuccess;
            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "State",
                ExpValue = state,
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            if (!string.IsNullOrEmpty(projectId))
            {
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ProjectId",
                    ExpValue = Convert.ToInt64(projectId),
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
            }

            Result<List<Epm_TzSupplyMaterialApply>> result = new Result<List<Epm_TzSupplyMaterialApply>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                result = proxy.GetTzSupplyMaterialApplyList(qc);

                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);
            }
            return View(result.Data);
        }
        /// <summary>
        /// 三商下对应的人员信息
        /// </summary>
        /// <param name="CompanyId">三商ID</param>
        /// <param name="name">用户名</param>
        /// <param name="postName">岗位</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult SelectUserListByCompanyId(long CompanyId = 0, string name = "", string postName = "", int pageIndex = 1, int pageSize = 10)
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

                result = proxy.GetUserManageList(qc, "");
            }
            ViewBag.CompanyId = CompanyId;
            ViewBag.Total = result.AllRowsCount;
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.name = name;
            return View(result.Data);
        }

        /// <summary>
        /// 财务决算
        /// </summary>
        /// <param name="id">项目批复id</param>
        /// <returns></returns>
        public ActionResult FinanceSettlementPopup(long id)
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var result = proxy.GetTzProjectApprovalFinanceAccounts(id);

                if (result.Flag == EResultFlag.Success && result.Data != null)
                {
                    return View(result.Data);
                }
                return View();
            }
        }

    }
}