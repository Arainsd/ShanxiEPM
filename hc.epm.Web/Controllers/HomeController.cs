using hc.epm.ViewModel;
using hc.Plat.Common.Global;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using hc.epm.DataModel.Business;
using hc.epm.UI.Common;
using hc.epm.Web.ClientProxy;
using hc.epm.DataModel.Msg;
using hc.epm.DataModel.Basic;
using System.IO;
using System.Configuration;
using hc.Plat.Common.Extend;

namespace hc.epm.Web.Controllers
{
    /// <summary>
    /// 首页
    /// </summary>
    public class HomeController : BaseWebController
    {
        public ActionResult SignIndex()
        {
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                #region 待办事项
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = GetPageInfo(1, 10)
                };
                qc.SortList.Add(new SortExpression("CreateTime", eSortType.Desc));
                var result = proxy.GetCurrUserApproverList(qc);
                ViewBag.ApproverList = result.Data;
                #endregion

                #region  监理日志
                //qc = new QueryCondition();
                //qc.PageInfo = GetPageInfo();
                //qc.ConditionList.Add(new ConditionExpression()
                //{
                //    ExpName = "ReceiveId",
                //    ExpValue = CurrentUser.UserId,
                //    ExpLogical = eLogicalOperator.And,
                //    ExpOperater = eConditionOperator.Equal
                //});
                //qc.ConditionList.Add(new ConditionExpression()
                //{
                //    ExpName = "Step",
                //    ExpValue = "SupervisorLogAdd",
                //    ExpLogical = eLogicalOperator.And,
                //    ExpOperater = eConditionOperator.Equal
                //});
                //List<Epm_SupervisorLogDetails> supervisorLogList = new List<Epm_SupervisorLogDetails>();
                //var logResult = proxy.GetSupervisorLogDetailsList(qc);
                //if (logResult.Flag == EResultFlag.Success && logResult.Data != null)
                //{
                //    supervisorLogList = logResult.Data;
                //}

                //ViewBag.SupervisorLogList = supervisorLogList;
                #endregion

                #region 消息列表
                qc = new QueryCondition();
                qc.PageInfo = GetPageInfo();
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "RecId",
                    ExpValue = CurrentUser.UserId,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
                List<MessageView> msgList = new List<MessageView>();
                var msgResult = proxy.GetMassageList(qc);
                if (msgResult.Data != null)
                {
                    var messageList = msgResult.Data;
                    foreach (var item in messageList)
                    {
                        MessageView msg = new MessageView();
                        msg.SendUserName = proxy.GetUserModel(item.SendId.Value).Data.UserName;
                        msg.SendTime = item.SendTime.Value;
                        msg.Id = item.Id;
                        msg.Title = item.Title.CutByByteLength(35, "...");
                        msg.BussinessId = item.BussinessId.Value;
                        msg.BussinesType = item.BussinesType;
                        msgList.Add(msg);
                    }
                }
                ViewBag.MsgList = msgList;
                #endregion

                #region 模型展示

                List<Epm_Bim> bimList = new List<Epm_Bim>();
                var bimResult = proxy.GetBimModelIndexList();
                if (bimResult.Flag == EResultFlag.Success && bimResult.Data != null)
                {
                    bimList = bimResult.Data.Select(p => new Epm_Bim()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        BIMImg = p.BIMImg,
                        ProjectName = p.ProjectName
                    }).ToList();
                }

                ViewBag.BimList = bimList;

                #endregion
                string years = DateTime.Now.Year.ToString();
                ViewBag.kpiList = proxy.GetProjectKPIListByWhr(years, CurrentUser.UserId).Data;
            }
            return View();
        }

        public ActionResult Message(string name = "", int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.Title = "消息列表";
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.Name = name;
            ViewBag.SelectPage = "Message";

            Result<List<MessageView>> result = new Result<List<MessageView>>();

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = GetPageInfo(pageIndex, pageSize)
                };
                #region 条件
                //qc = new QueryCondition();
                //qc.PageInfo = GetPageInfo();
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "RecId",
                    ExpValue = CurrentUser.UserId,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
                #endregion
                if (!string.IsNullOrWhiteSpace(name))
                {
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "Title",
                        ExpValue = string.Format("%{0}%", name),
                        ExpOperater = eConditionOperator.Like,
                        ExpLogical = eLogicalOperator.And
                    });
                }

                List<MessageView> msgList = new List<MessageView>();
                var msgResult = proxy.GetMassageList(qc);
                if (msgResult.Data != null)
                {
                    var messageList = msgResult.Data;
                    foreach (var item in messageList)
                    {
                        MessageView msg = new MessageView();
                        msg.SendUserName = proxy.GetUserModel(item.SendId.Value).Data.UserName;
                        msg.SendTime = item.SendTime.Value;
                        msg.Id = item.Id;
                        msg.Title = item.Title;
                        msg.BussinessId = item.BussinessId.Value;
                        msg.BussinesType = item.BussinesType;
                        msgList.Add(msg);
                    }
                }

                ViewBag.Total = msgResult.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)msgResult.AllRowsCount / pageSize);

                return View(msgList);
            }

        }
        public ActionResult MyJob(string name, int pageIndex = 1, int pageSize = 10)
        {
            ViewBag.Title = "我的待办";
            ViewBag.pageIndex = pageIndex;
            ViewBag.pageSize = pageSize;
            ViewBag.Name = name;
            ViewBag.SelectPage = "MyJob";

            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                QueryCondition qc = new QueryCondition()
                {
                    PageInfo = GetPageInfo(pageIndex, pageSize)
                };

                if (!string.IsNullOrWhiteSpace(name))
                {
                    qc.ConditionList.Add(new ConditionExpression()
                    {
                        ExpName = "Title",
                        ExpValue = string.Format("%{0}%", name),
                        ExpOperater = eConditionOperator.Like,
                        ExpLogical = eLogicalOperator.And
                    });
                }

                var result = proxy.GetCurrUserApproverList(qc);
                ViewBag.Total = result.AllRowsCount;
                ViewBag.TotalPage = Math.Ceiling((decimal)result.AllRowsCount / pageSize);

                return View(result.Data);
            }
        }

        public ActionResult UpdatePwd()
        {
            ViewBag.Title = "修改密码";
            ViewBag.SelectPage = "UpdatePwd";
            return View();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdatePwd(string oldPwd, string pwd, string cpwd)
        {
            var userInfo = Session[ConstStr_Session.CurrentUserEntity] as UserView;
            if (userInfo == null)
            {
                return RedirectToAction("Login", "Currency");
            }

            if (string.IsNullOrWhiteSpace(oldPwd) || string.IsNullOrWhiteSpace(pwd))
            {
                return Json(new ResultView<bool>
                {
                    Flag = false,
                    Data = false,
                    Message = "请输入登录密码！"
                });
            }

            if (!pwd.Equals(cpwd))
            {
                return Json(new ResultView<bool>
                {
                    Flag = false,
                    Data = false,
                    Message = "两次密码输入不一致！"
                });
            }

            if (oldPwd.Equals(pwd))
            {
                return Json(new ResultView<bool>
                {
                    Flag = false,
                    Data = false,
                    Message = "新密码不能和旧密码相同！"
                });
            }
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var checkResult = proxy.GetUserModel(userInfo.UserId);
                if (checkResult.Flag == EResultFlag.Success && checkResult.Data != null)
                {
                    if (!checkResult.Data.PassWord.Equals(DesTool.DesEncrypt(oldPwd)))
                    {
                        return Json(new ResultView<bool>
                        {
                            Flag = false,
                            Data = false,
                            Message = "旧密码不正确！"
                        });
                    }

                    var result = proxy.UpdatePassword(userInfo.UserId, DesTool.DesEncrypt(oldPwd), DesTool.DesEncrypt(pwd));
                    return Json(result.ToResultView());
                }
                return Json(new ResultView<bool>
                {
                    Flag = false,
                    Data = false,
                    Message = "新密码不能和旧密码相同！"
                });
            }
        }

        public ActionResult PersonInfo()
        {
            ViewBag.Title = "个人中心";
            ViewBag.SelectPage = "PersonInfo";
            var userInfo = Session[ConstStr_Session.CurrentUserEntity] as UserView;
            if (userInfo == null)
            {
                return RedirectToAction("Login", "Currency");
            }

            return View(userInfo);
        }

        /// <summary>
        /// 广告位
        /// </summary>
        /// <returns></returns>、
        [HttpGet]
        public ActionResult GetAdPutRecord(string targetNum)
        {
            Result<List<Base_Files>> result = new Result<List<Base_Files>>();
            using (ClientSiteClientProxy proxy = new ClientSiteClientProxy(ProxyEx(Request)))
            {
                var otherController = DependencyResolver.Current.GetService<FilesController>();
                result = proxy.GetAdPutRecord(targetNum, "small");
                if (result.Data != null)
                {
                    foreach (var item in result.Data)
                    {
                        if(item != null && !string.IsNullOrWhiteSpace(item.Url))
                        {
                            item.Url = GetImageBase64(item.Url);
                        }
                    }
                }
            }
            return Json(result.ToResultView(), JsonRequestBehavior.AllowGet);
        }

        private string GetImageBase64(string path)
        {
            string ThumbnailPath = ConfigurationManager.AppSettings["ThumbnailPath"];
            FileInfo file = new FileInfo(ThumbnailPath + path.Substring(path.IndexOf('/') + 1));//创建一个文件对象
            if (file.Exists)
            {
                FileStream stream = file.OpenRead();
                var bufferLength = stream.Length;
                byte[] bytes = new byte[bufferLength];
                stream.Read(bytes, 0, Convert.ToInt32(bufferLength));
                stream.Flush();
                stream.Close();

                return "data:image/" + path.Substring(path.LastIndexOf('.') + 1) + ";base64," + Convert.ToBase64String(bytes);
            }
            else
            {
                return string.Empty;
            }
        }

        public ActionResult Information()
        {
            ViewBag.Title = "最新资讯列表";
            return View();
        }
        public ActionResult InformationDetail()
        {
            ViewBag.Title = "最新资讯详情";
            return View();
        }
        public ActionResult Engineering()
        {
            ViewBag.Title = "工程动态列表";
            return View();
        }
        public ActionResult EngineeringDetail()
        {
            ViewBag.Title = "工程动态详情";
            return View();
        }
        public ActionResult CompanyDetail()
        {
            ViewBag.Title = "单位详情";
            return View();
        }
        public ActionResult Notice()
        {
            ViewBag.Title = "最新公告列表";
            return View();
        }
    }
}