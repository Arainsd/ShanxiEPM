using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        private BusinessDataContext context = new BusinessDataContext();
        private BasicDataContext basicContext = new BasicDataContext();
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddApprover(Epm_Approver model)
        {
            Result<int> result = new Result<int>();
            try
            {
                model.IsApprover = false;
                model.CrtCompanyId = CurrentCompanyID.ToLongReq();
                model.CrtCompanyName = CurrentCompanyName;
                model.CreateUserId = CurrentUserID.ToLongReq();
                model.CreateUserName = CurrentUserName;
                model.CreateTime = DateTime.Now;
                model.OperateUserId = CurrentUserID.ToLongReq();
                model.OperateUserName = CurrentUserName;
                model.OperateTime = DateTime.Now;
                model.IsDelete = false;

                var rows = DataOperateBusiness<Epm_Approver>.Get().Add(model);

                Epm_ApproverRecord record = new Epm_ApproverRecord();
                record.BusinessId = model.BusinessId;
                record.BusinessType = model.BusinessTypeNo;
                record.BusinessName = model.BusinessTypeName;
                record.HandleUserId= CurrentUserID.ToLongReq();
                record.HandleUserName = CurrentUserName;
                record.BusinessState = model.BusinessState.ToString();
                AddApproverRecord(record);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Approver.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddApprover");
            }
            return result;
        }
        public Result<int> AddApproverBatch(List<Epm_Approver> list)
        {
            Result<int> result = new Result<int>();
            try
            {
                foreach (var item in list)
                {
                    item.IsApprover = false;
                    item.CrtCompanyId = CurrentCompanyID.ToLongReq();
                    item.CrtCompanyName = CurrentCompanyName;
                    item.CreateUserId = CurrentUserID.ToLongReq();
                    item.CreateUserName = CurrentUserName;
                    item.CreateTime = DateTime.Now;
                    item.OperateUserId = CurrentUserID.ToLongReq();
                    item.OperateUserName = CurrentUserName;
                    item.OperateTime = DateTime.Now;
                    item.IsDelete = false;
                }
                var rows = DataOperateBusiness<Epm_Approver>.Get().AddRange(list);

                Epm_ApproverRecord record = new Epm_ApproverRecord();
                record.BusinessId = list[0].BusinessId;
                record.BusinessType = list[0].BusinessTypeNo;
                record.BusinessName = list[0].BusinessTypeName;
                record.BusinessState = list[0].BusinessState.ToString();
                record.HandleUserId = CurrentUserID.ToLongReq();
                record.HandleUserName = CurrentUserName;
                AddApproverRecord(record);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Approver.GetText(), SystemRight.Add.GetText(), "新增: " + rows + "条待办");
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddApprover");
            }
            return result;
        }

        /// <summary>
        /// 增加代办记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddApproverRecord(Epm_ApproverRecord model)
        {
            Result<int> result = new Result<int>();
            try
            {
                model.CrtCompanyId = CurrentCompanyID.ToLongReq();
                model.CrtCompanyName = CurrentCompanyName;
                model.CreateUserId = CurrentUserID.ToLongReq();
                model.CreateUserName = CurrentUserName;
                model.CreateTime = DateTime.Now;
                model.OperateUserId = CurrentUserID.ToLongReq();
                model.OperateUserName = CurrentUserName;
                model.OperateTime = DateTime.Now;
                model.IsDelete = false;

                var rows = DataOperateBusiness<Epm_ApproverRecord>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Approver.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddApproverRecord");
            }
            return result;
        }

        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateApprover(Epm_Approver model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_Approver>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Approver.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateApprover");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteApproverByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_Approver>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_Approver>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.Approver.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteApproverByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_Approver>> GetApproverList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_Approver>> result = new Result<List<Epm_Approver>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Approver>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetApproverList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_Approver> GetApproverModel(long id)
        {
            Result<Epm_Approver> result = new Result<Epm_Approver>();
            try
            {
                var model = DataOperateBusiness<Epm_Approver>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetApproverModel");
            }
            return result;
        }

        public Result<Epm_Approver> GetApproverModelByBusinId(long bussinesId, long approverId = 0)
        {
            Result<Epm_Approver> result = new Result<Epm_Approver>();
            try
            {
                bussinesId = DataOperateBusiness<Epm_RectificationItem>.Get().GetList(t => t.Id == bussinesId).FirstOrDefault().RectificationId.Value;
                if (approverId == 0)
                {
                    var model = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == bussinesId && t.IsApprover == false).OrderByDescending(t => t.CreateTime).FirstOrDefault();
                    result.Data = model;
                    result.Flag = EResultFlag.Success;
                }
                else
                {
                    var model = DataOperateBusiness<Epm_Approver>.Get().GetList(t => t.BusinessId == bussinesId && t.ApproverId == approverId && t.IsApprover == false).OrderByDescending(t => t.CreateTime).FirstOrDefault();
                    result.Data = model;
                    result.Flag = EResultFlag.Success;
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetApproverModelByBusinId");
            }
            return result;
        }

        /// <summary>
        /// 处理待办事项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<bool> ComplateApprover(long id)
        {
            Result<bool> result = new Result<bool>();
            try
            {
                var model = DataOperateBusiness<Epm_Approver>.Get().GetModel(id);
                if (model == null)
                {
                    throw new Exception("要处理的待办事项不存在或已被删除！");
                }
                if (model.IsApprover == true)
                {
                    throw new Exception("该待办事项已处理！");
                }

                model.IsApprover = true;
                model.ApproverId = CurrentUser.Id;
                model.ApproverName = CurrentUserName;
                model.OperateTime = DateTime.Now;
                model.OperateUserId = CurrentUser.Id;
                model.OperateUserName = CurrentUserName;

                DataOperateBusiness<Epm_Approver>.Get().Update(model);
                result.Data = true;
                result.Flag = EResultFlag.Success;

                WriteLog(BusinessType.Approver.GetText(), SystemRight.Check.GetText(), string.Format("{0}于{1}处理待办事项{2}。", CurrentUserName, DateTime.Now, model.Id));
            }
            catch (Exception ex)
            {
                result.Data = false;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "ComplateApprover");
            }
            finally
            {
                
            }
            return result;
        }
        
        /// <summary>
        /// 获取当前登录用户待办事项
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_Approver>> GetCurrUserApproverList(QueryCondition qc)
        {
            var currCompanyId = CurrentCompanyID.ToLongReq();
            qc = AddDefault(qc);
            //qc.ConditionList.Add(new ConditionExpression()
            //{
            //    ExpName = "ApproverId",
            //    ExpValue = CurrentUser.Id,
            //    ExpOperater = eConditionOperator.Equal,
            //    ExpLogical = eLogicalOperator.And
            //});

            //qc.ConditionList.Add(new ConditionExpression()
            //{
            //    ExpName = "ApproverId",
            //    ExpValue = currCompanyId,
            //    ExpOperater = eConditionOperator.Equal,
            //    ExpLogical = eLogicalOperator.And
            //});

            ConditionExpression ce4 = new ConditionExpression();
            ConditionExpression ce41 = new ConditionExpression();
            ce41.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "ApproverId",
                ExpValue = CurrentUserID,
                ExpOperater = eConditionOperator.Equal,
            });
            ce4.ConditionList.Add(ce41);

            ConditionExpression ce42 = new ConditionExpression();
            ce42.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "ApproverId",
                ExpValue = currCompanyId,
                ExpOperater = eConditionOperator.Equal

            });
            ce42.ExpLogical = eLogicalOperator.Or;
            ce4.ExpLogical = eLogicalOperator.And;
            ce4.ConditionList.Add(ce42);
            qc.ConditionList.Add(ce4);


            qc.ConditionList.Add(new ConditionExpression()
            {
                ExpName = "IsApprover",
                ExpValue = false,
                ExpOperater = eConditionOperator.Equal,
                ExpLogical = eLogicalOperator.And
            });

            qc.SortList.Add(new SortExpression()
            {
                SortName = "SendTime",
                SortType = eSortType.Desc
            });

            Result<List<Epm_Approver>> result = new Result<List<Epm_Approver>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Approver>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetApproverList");
            }
            return result;
        }
    }
}
