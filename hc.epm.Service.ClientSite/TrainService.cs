using hc.epm.Common;
using hc.epm.DataModel.BaseCore;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using hc.Plat.Common.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddTrain(TrainView model, List<Base_Files> fileList = null)
        {
            Result<int> result = new Result<int>();
            try
            {
                Epm_Train train = new Epm_Train();
                List<Epm_TrainCompany> companys = new List<Epm_TrainCompany>();
                model.Id = train.Id;
                ViewToEmp(model, out train, out companys);

                var rows = DataOperateBusiness<Epm_Train>.Get().Add(train);
                DataOperateBusiness<Epm_TrainCompany>.Get().AddRange(companys);

                //新增附件
                AddFilesByTable(train, fileList);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.SecurityTrain.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTrain");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTrain(TrainView model, List<Base_Files> fileList = null)
        {
            Result<int> result = new Result<int>();
            try
            {

                Epm_Train change = new Epm_Train();

                List<Epm_TrainCompany> companys = new List<Epm_TrainCompany>();
                ViewToEmp(model, out change, out companys);

                var rows = DataOperateBusiness<Epm_Train>.Get().Update(change);
                var list = DataOperateBusiness<Epm_TrainCompany>.Get().GetList(t => t.TrainId == change.Id).ToList();
                if (list != null)
                    DataOperateBusiness<Epm_TrainCompany>.Get().DeleteRange(list);
                DataOperateBusiness<Epm_TrainCompany>.Get().AddRange(companys);

                if (fileList != null)
                {
                    //删除之前的附件
                    DeleteFilesByTable(change.GetType().Name, new List<long>() { model.Id });
                    //新增附件
                    AddFilesByTable(change, fileList);
                }



                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.SecurityTrain.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTrain");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTrainByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_Train>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                if (models.Any(t => t.State != (int)ConfirmState.Discarded && t.State != (int)ConfirmState.Enabled))
                {
                    throw new Exception("草稿，已废弃状态下，才可删除");
                }
                var rows = DataOperateBusiness<Epm_Train>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(BusinessType.SecurityTrain.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTrainByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<TrainView>> GetTrainList(QueryCondition qc)
        {
            qc = AddDefaultWeb(qc);

            Result<List<TrainView>> result = new Result<List<TrainView>>();
            try
            {
                QueryCondition qcTrain = new QueryCondition();
                qcTrain.PageInfo = qc.PageInfo;
                qcTrain.SortList = qc.SortList;
                QueryCondition qcCompany = new QueryCondition();
                foreach (var item in qc.ConditionList)
                {
                    if (item.ExpName == "CompanyIds")
                    {
                        qcCompany.ConditionList.Add(new ConditionExpression()
                        {
                            ExpName = "CompanyId",
                            ExpValue = item.ExpValue,
                            ExpLogical = item.ExpLogical,
                            ExpOperater = item.ExpOperater
                        });
                    }
                    else if (item.ExpName == "CompanyNames")
                    {
                        qcCompany.ConditionList.Add(new ConditionExpression()
                        {
                            ExpName = "CompanyName",
                            ExpValue = item.ExpValue,
                            ExpLogical = item.ExpLogical,
                            ExpOperater = item.ExpOperater
                        });
                    }
                    else
                    {
                        qcTrain.ConditionList.Add(item);
                    }
                }
                var res = DataOperate.QueryListSimple<Epm_Train>(context, qcTrain);
                List<TrainView> list = new List<TrainView>();
                if (res.Data.Count > 0)//显示单位
                {
                    ConditionExpression ce = new ConditionExpression();
                    ce.ExpName = "TrainId";
                    ce.ExpValue = string.Join(",", res.Data.Select(t => t.Id));
                    ce.ExpOperater = eConditionOperator.In;
                    qcCompany.ConditionList.Add(ce);

                    var companys = DataOperate.QueryListSimple<Epm_TrainCompany>(context, qcCompany);
                    foreach (Epm_Train item in res.Data)
                    {
                        var com = companys.Data.Where(t => t.TrainId == item.Id).ToList();
                        list.Add(EmpToView(item, com));
                    }
                }
                result.AllRowsCount = res.AllRowsCount;
                result.Flag = res.Flag;
                result.Data = list;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTrainList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<TrainView> GetTrainModel(long id)
        {
            Result<TrainView> result = new Result<TrainView>();
            try
            {
                var model = DataOperateBusiness<Epm_Train>.Get().GetModel(id);
                var companys = DataOperateBusiness<Epm_TrainCompany>.Get().GetList(t => model.Id == t.TrainId).ToList();
                result.Data = EmpToView(model, companys);
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTrainModel");
            }
            return result;
        }

        private TrainView EmpToView(Epm_Train visa, List<Epm_TrainCompany> companys)
        {
            TrainView model = new TrainView();
            model.Id = visa.Id;
            model.CompanyId = visa.CompanyId;
            model.CompanyIds = string.Join(",", companys.Select(t => t.CompanyId));
            model.CompanyName = visa.CompanyName;
            model.CompanyNames = string.Join(",", companys.Select(t => t.CompanyName));
            model.Content = visa.Content;
            model.CrtCompanyId = visa.CrtCompanyId;
            model.CrtCompanyName = visa.CrtCompanyName;
            model.ProjectId = visa.ProjectId;
            model.ProjectName = visa.ProjectName;
            model.Remark = visa.Remark;
            model.EndTime = visa.EndTime;
            model.StartTime = visa.StartTime;
            model.State = visa.State;
            model.Title = visa.Title;
            model.TrainCompanyId = visa.TrainCompanyId;
            model.TrainCompanyName = visa.TrainCompanyName;
            model.TrainTypeName = visa.TrainTypeName;
            model.TrainTypeNo = visa.TrainTypeNo;
            model.TrainUserId = visa.TrainUserId;
            model.TrainUserName = visa.TrainUserName;
            model.CreateUserId = visa.CreateUserId;
            model.CreateUserName = visa.CreateUserName;
            model.CreateTime = visa.CreateTime;
            model.TrainCompany = companys;
            return model;
        }
        private void ViewToEmp(TrainView view, out Epm_Train model, out List<Epm_TrainCompany> companys)
        {
            model = new Epm_Train();
            model = SetCurrentUser(model);
            model.CrtCompanyId = CurrentCompanyID.ToLongReq();
            model.CrtCompanyName = CurrentCompanyName;
            model.CompanyId = CurrentCompanyID.ToLongReq();
            model.CompanyName = CurrentCompanyName;

            model.Id = view.Id;
            model.CompanyId = view.CompanyId;

            model.CompanyName = view.CompanyName;

            model.Content = view.Content;
            model.CrtCompanyId = view.CrtCompanyId;
            model.CrtCompanyName = view.CrtCompanyName;
            model.ProjectId = view.ProjectId;
            model.ProjectName = view.ProjectName;
            model.Remark = view.Remark;
            model.StartTime = view.StartTime;
            model.State = view.State;
            model.Title = view.Title;
            model.TrainCompanyId = view.TrainCompanyId;
            model.TrainCompanyName = view.TrainCompanyName;
            model.TrainTypeName = view.TrainTypeName;
            model.TrainTypeNo = view.TrainTypeNo;
            model.TrainUserId = view.TrainUserId;
            model.TrainUserName = view.TrainUserName;
            model.EndTime = view.EndTime;
            model.CreateUserId = view.CreateUserId;
            model.CreateUserName = view.CreateUserName;
            model.CreateTime = view.CreateTime;
            companys = new List<Epm_TrainCompany>();
            if (!string.IsNullOrWhiteSpace(view.CompanyIds))
            {
                var Ids = view.CompanyIds.Split(',').ToLongList();
                var list = DataOperateBasic<Base_Company>.Get().GetList(t => Ids.Contains(t.Id));
                foreach (var item in list)
                {
                    var comany = new Epm_TrainCompany();
                    base.SetCurrentUser(comany);
                    comany.CrtCompanyId = CurrentCompanyID.ToLongReq();
                    comany.CrtCompanyName = CurrentCompanyName;
                    comany.CompanyId = item.Id;
                    comany.CompanyName = item.Name;
                    comany.TrainId = view.Id;
                    comany.CompanyCode = item.Code;
                    comany.CompanyType = item.Type;
                    companys.Add(comany);
                }
            }
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTrainState(long id, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBusiness<Epm_Train>.Get().GetModel(id);
                if (model != null)
                {
                    model.OperateUserId = CurrentUserID.ToLongReq();
                    model.OperateUserName = CurrentUserName;
                    model.OperateTime = DateTime.Now;
                    model.State = (int)state.ToEnumReq<ConfirmState>();
                    var rows = DataOperateBusiness<Epm_Train>.Get().Update(model);
                    result.Data = rows;
                    result.Flag = EResultFlag.Success;

                    WriteLog(BusinessType.Change.GetText(), SystemRight.Delete.GetText(), "更新状态: " + rows);
                }
                else
                {
                    throw new Exception("id有误");
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateState");
            }
            return result;
        }
        public Result<List<Epm_Template>> GetTemplateList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_Template>> result = new Result<List<Epm_Template>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Template>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTemplateList");
            }
            return result;
        }
    }
}
