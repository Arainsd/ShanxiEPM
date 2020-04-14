using AutoMapper;
using hc.epm.Common;
using hc.epm.DataModel.BaseCore;
using hc.epm.DataModel.Basic;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;

namespace hc.epm.Service.Base
{
    public class BaseService
    {
        /// <summary>
        /// 获取当前用户Id
        /// </summary>
        protected string CurrentUserID
        {
            get
            {
                return ApplicationContext.Current.UserID;
            }
        }

        /// <summary>
        /// 当前用户Ip
        /// </summary>
        protected string CurrentUserIp
        {
            get
            {
                return ApplicationContext.Current.ClientIP;
            }
        }
        /// <summary>
        /// 当前用户名
        /// </summary>
        protected string CurrentUserName
        {
            get
            {
                return ApplicationContext.Current.UserName;
            }
        }
        /// <summary>
        /// 服务器IP
        /// </summary>
        protected string ServerIP
        {
            get
            {
                return ApplicationContext.Current.ServiceIP;
            }
        }
        /// <summary>
        /// 当前用户，慎用，一定用之前确保用户能正常取得
        /// </summary>
        protected Base_User CurrentUser
        {
            get
            {
                try
                {
                    return DataOperateBasic<Base_User>.Get().GetModel(ApplicationContext.Current.UserID.ToLongReq());
                }
                catch (Exception)
                {
                    //默认给的管理员，TODO:需修改
                    return new Base_User() { UserName = "admin", UserAcct = "admin", Id = 897711908373794816 };
                }

            }
        }
        /// <summary>
        /// 获取当前用户单位Id
        /// </summary>
        protected string CurrentCompanyID
        {
            get
            {
                return ApplicationContext.Current.CompanyId;
            }
        }
        /// <summary>
        /// 当前用户登用户单位Name
        /// </summary>
        protected string CurrentCompanyName
        {
            get
            {
                return ApplicationContext.Current.CompanyName;
            }
        }

        /// <summary>
        /// 当前用户涉及项目Id
        /// </summary>
        protected string CurrentProjectIds
        {
            get
            {
                return ApplicationContext.Current.ProjectIds;
            }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="action">功能</param>
        /// <param name="content">描述</param>
        /// <returns></returns>
        public virtual Result<Base_Log> WriteLog(string module, string action, string content)
        {
            Result<Base_Log> result = new Result<Base_Log>();
            try
            {
                Base_Log log = new Base_Log();
                log.ClientIP = ApplicationContext.Current.ClientIP;
                log.WebServerIP = ApplicationContext.Current.WebIP;
                log.ServiceServerIP = ApplicationContext.Current.ServiceIP;
                log.ActionDesc = action;
                log.ModuleName = module;
                log.Contents = content;
                log.UserAcct = CurrentUserName;
                log.CreateUserId = CurrentUser.Id;
                log.CreateUserName = CurrentUser.UserAcct;
                log.CreateTime = DateTime.Now;
                log.OperateUserId = CurrentUser.Id;
                log.OperateUserName = CurrentUser.UserAcct;
                log.OperateTime = DateTime.Now;

                try
                {
                    log.UserId = CurrentUser.Id;
                }
                catch
                {
                    log.UserId = 0;
                }
                var rows = DataOperateBasic<Base_Log>.Get().Add(log);
                result.Data = log;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "WriteLog");
            }
            return result;
        }
        /// <summary>
        /// 保存用户登录日志 注册日志
        /// </summary>
        /// <param name="content"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Result<Base_Log> WriteLoginLog(string content, Base_User user)
        {
            Result<Base_Log> result = new Result<Base_Log>();
            try
            {
                Base_Log log = new Base_Log();
                log.ClientIP = ApplicationContext.Current.ClientIP;
                log.WebServerIP = ApplicationContext.Current.WebIP;
                log.ServiceServerIP = ApplicationContext.Current.ServiceIP;
                log.ActionDesc = "用户登录";
                log.ModuleName = "用户管理";
                log.Contents = content;
                log.UserAcct = user.UserAcct;
                log.UserId = user.Id;//登录失败则写入id为0
                log.CreateUserId = user.Id;
                log.CreateUserName = user.UserAcct;
                log.CreateTime = DateTime.Now;
                log.OperateUserId = user.Id;
                log.OperateUserName = user.UserAcct;
                log.OperateTime = DateTime.Now;
                var rows = DataOperateBasic<Base_Log>.Get().Add(log);
                result.Data = log;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "WriteLoginLog");
            }
            return result;
        }
        /// <summary>
        /// 写审核日志
        /// </summary>
        /// <param name="model"></param>
        /// <param name="oldState"></param>
        /// <param name="newState"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Result<Base_StatusLog> WriteStateLog(BaseModel model, string oldState, string newState)
        {
            Result<Base_StatusLog> result = new Result<Base_StatusLog>();

            try
            {
                Base_StatusLog log = new Base_StatusLog();
                log.OperateUserId = CurrentUserID.ToLongReq();
                log.OperateUserName = CurrentUserName;
                log.OldState = oldState;
                log.State = newState;
                log.TableId = model.Id;
                log.TableName = model.GetType().Name;

                var rows = DataOperateBasic<Base_StatusLog>.Get().Add(log);
                result.Data = log;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "WriteStateLog");
            }
            return result;
        }
        /// <summary>
        /// 添加默认条件
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        protected QueryCondition AddDefault(QueryCondition qc)
        {
            if (qc == null)
            {
                qc = new QueryCondition();
                qc.PageInfo = null;
            }
            ConditionExpression ce = new ConditionExpression();
            ce.ExpName = "IsDelete";
            ce.ExpValue = false;
            ce.ExpOperater = eConditionOperator.Equal;
            ce.ExpLogical = eLogicalOperator.And;
            qc.ConditionList.Add(ce);
            return qc;
        }
        /// <summary>
        /// 添加默认条件
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        protected QueryCondition AddDefaultWeb(QueryCondition qc)
        {
            if (qc == null)
            {
                qc = new QueryCondition();
                qc.PageInfo = null;
            }
            ConditionExpression ce = new ConditionExpression();
            ce.ExpName = "IsDelete";
            ce.ExpValue = false;
            ce.ExpOperater = eConditionOperator.Equal;
            qc.ConditionList.Add(ce);

            //2、草稿状态数据只有添加人自己可以看（项目无草稿状态）
            ConditionExpression ce1 = new ConditionExpression();
            ConditionExpression ce11 = new ConditionExpression();
            ce11.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "State",
                ExpValue = (int)ApprovalState.Enabled,
                ExpOperater = eConditionOperator.Equal
            });
            ce11.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "CreateUserId",
                ExpValue = CurrentUserID.ToLongReq(),
                ExpLogical = eLogicalOperator.And,
                ExpOperater = eConditionOperator.Equal
            });
            ce11.ExpLogical = eLogicalOperator.And;
            ce1.ConditionList.Add(ce11);

            ConditionExpression ce12 = new ConditionExpression();
            ce12.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "State",
                ExpValue = (int)ApprovalState.Enabled,
                ExpOperater = eConditionOperator.NotEqual

            });
            ce12.ExpLogical = eLogicalOperator.Or;
            ce1.ExpLogical = eLogicalOperator.And;
            ce1.ConditionList.Add(ce12);

            qc.ConditionList.Add(ce1);

            //3、查询列表数据时需根据登录用户负责项目进行筛选

            if (!string.IsNullOrWhiteSpace(CurrentProjectIds))
            {
                ConditionExpression ce2 = new ConditionExpression();
                ce2.ExpName = "ProjectId";
                ce2.ExpValue = CurrentProjectIds;
                ce2.ExpLogical = eLogicalOperator.And;
                ce2.ExpOperater = eConditionOperator.In;

                qc.ConditionList.Add(ce2);
            }

            //1、列表数据根据最后修改时间倒序
            qc.SortList.Add(new SortExpression("OperateTime", eSortType.Desc));

            return qc;
        }

        /// <summary>
        /// 删除对应业务数据的附件
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteFilesByTable(string tableName, List<long> ids)
        {
            int rows = 0;
            var models = DataOperateBasic<Base_Files>.Get().GetList(i => ids.Contains(i.TableId) && i.TableName == tableName).ToList();
            foreach (var item in models)
            {
                item.DeleteTime = DateTime.Now;
            }
            rows = DataOperateBasic<Base_Files>.Get().DeleteRange(models);
            return rows;
        }
        /// <summary>
        /// 删除对应业务数据的附件
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DeleteFilesByTable(string tableName, List<long> ids, string column)
        {
            int rows = 0;
            var models = DataOperateBasic<Base_Files>.Get().GetList(i => ids.Contains(i.TableId) && i.TableName == tableName && i.TableColumn == column).ToList();
            foreach (var item in models)
            {
                item.DeleteTime = DateTime.Now;
            }
            rows = DataOperateBasic<Base_Files>.Get().DeleteRange(models);
            return rows;
        }

        /// <summary>
        /// 添加附件
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList"></param>
        /// <returns></returns>
        public int AddFilesByTable(BaseModel model, List<Base_Files> fileList, bool isdelete = true)
        {
            if (fileList != null)
            {
                long id = model.Id;
                string tableName = model.GetType().Name;
                //删除之前的附件
                if (isdelete)
                {
                    var columns = fileList.Select(i => i.TableColumn);
                    //var oldFiles = DataOperateBasic<Base_Files>.Get().GetList(i => i.TableId == id && i.TableName == tableName && columns.Contains(i.TableColumn)).ToList();
                    var oldFiles = DataOperateBasic<Base_Files>.Get().GetList(i => i.TableId == id && i.TableName == tableName).ToList();
                    DataOperateBasic<Base_Files>.Get().DeleteRange(oldFiles);
                }
                //新增附件
                List<Base_Files> fileModels = new List<Base_Files>();
                foreach (var item in fileList)
                {
                    SetCurrentUser(item);
                    item.TableId = id;
                    item.TableName = tableName;
                    fileModels.Add(item);
                }
                int rows = 0;
                rows = DataOperateBasic<Base_Files>.Get().AddRange(fileModels);
                return rows;
            }
            return 0;
        }
        
        /// <summary>
        /// 获取指定配置项
        /// </summary>
        /// <param name="setKey"></param>
        /// <returns></returns>
        public Base_Settings LoadSettingsByKeys(Settings setKey)
        {
            Result<List<Base_Settings>> result = new Result<List<Base_Settings>>();


            var list = DataOperateBasic<Base_Settings>.Get().GetList().ToList();

            result.Data = list;
            var model = list.FirstOrDefault(i => i.Code == setKey.ToString());
            return model;
        }

        /// <summary>
        /// 去除ef代理类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        protected List<T> ToMap<T>(List<T> source)
        {
            List<T> list = Mapper.Map<List<T>, List<T>>(source);
            return list;
        }
        /// <summary>
        /// 为数据附加当前操作人数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public T SetCurrentUser<T>(T model) where T : BaseBusiness
        {
            //model.OperateUserId = CurrentUserID.ToLongReq();
            model.OperateUserId = CurrentUser.Id;
            model.OperateUserName = CurrentUserName;
            model.OperateTime = DateTime.Now;
            return model;
        }

        /// <summary>
        /// 修改数据时，保留创建人，创建时间不变，修改操作人和操作时间
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldModel">原数据</param>
        /// <param name="newModel">要修改的新数据</param>
        /// <returns></returns>
        public T FiterUpdate<T>(T oldModel, T newModel) where T : BaseBusiness
        {
            newModel.CreateUserId = oldModel.CreateUserId;
            newModel.CreateUserName = oldModel.CreateUserName;
            newModel.CreateTime = oldModel.CreateTime;

            newModel.OperateUserId = CurrentUserID.ToLongReq();
            newModel.OperateUserName = CurrentUserName;
            newModel.OperateTime = DateTime.Now;
            return newModel;
        }

        /// <summary>
        /// 当前创建人信息赋值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public T SetCreateUser<T>(T model) where T : BaseBusiness
        {
            model.CreateTime = DateTime.Now;
            model.CreateUserId = CurrentUser.Id;
            model.CreateUserName = CurrentUserName;

            return model;
        }

        public T XTSetCurrentUser<T>(T model) where T : BaseBusiness
        {
            model.OperateTime = DateTime.Now;
            model.OperateUserId = CurrentUser.Id;
            model.OperateUserName = CurrentUserName;

            return model;
        }
    }
}
