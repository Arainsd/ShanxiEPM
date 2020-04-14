using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.ViewModel;
using hc.Plat.Common.Extend;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Service.ClientSite
{
   public partial class ClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddCompletionAcceptanceResUpload(Epm_CompletionAcceptanceResUpload model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_CompletionAcceptanceResUpload>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
              //  WriteLog(AdminModule.CompletionAcceptanceResUpload.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddCompletionAcceptanceResUpload");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateCompletionAcceptanceResUpload(Epm_CompletionAcceptanceResUpload model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var entity = DataOperateBusiness<Epm_CompletionAcceptanceResUpload>.Get().GetModel(model.Id);
                model.ProjectId = entity.ProjectId;
                model.ProjectName = entity.ProjectName;
                model.Content = entity.Content;
                model.Num = entity.Num;
                model.RecCompanyId = entity.RecCompanyId;
                model.RecCompanyName = entity.RecCompanyName;
                model.RecUserId = entity.RecUserId;
                model.RecUserName = entity.RecUserName;
                model.RecTime = entity.RecTime;
                model.AcceptanceResult = entity.AcceptanceResult;
                model.State = model.State;
                model.Remark = entity.Remark;
                model.CrtCompanyId = entity.CrtCompanyId;
                model.CrtCompanyName = entity.CrtCompanyName;
                model.Title = entity.Title;
                model.RectifContent = entity.RectifContent;
                //当前操作人
                SetCurrentUser(model);
                //上传附件
                AddFilesBytzTable(model, model.TzAttachs);

                var rows = DataOperateBusiness<Epm_CompletionAcceptanceResUpload>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
              //  WriteLog(AdminModule.CompletionAcceptanceResUpload.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateCompletionAcceptanceResUpload");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteCompletionAcceptanceResUploadByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_CompletionAcceptanceResUpload>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_CompletionAcceptanceResUpload>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.CompletionAcceptanceResUpload.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteCompletionAcceptanceResUploadByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_CompletionAcceptanceResUpload>> GetCompletionAcceptanceResUploadList(QueryCondition qc)
        {
            Result<List<Epm_CompletionAcceptanceResUpload>> result = new Result<List<Epm_CompletionAcceptanceResUpload>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_CompletionAcceptanceResUpload>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCompletionAcceptanceList");
            }
            return result;
        }
        
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        //public Result<Epm_CompletionAcceptanceResUpload> GetCompletionAcceptanceResUploadModel(long id)
        //{
        //    Result<Epm_CompletionAcceptanceResUpload> result = new Result<Epm_CompletionAcceptanceResUpload>();
        //    try
        //    {
        //        var model = DataOperateBusiness<Epm_CompletionAcceptanceResUpload>.Get().GetModel(id);
        //        result.Data = model;
        //        result.Flag = EResultFlag.Success;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Data = null;
        //        result.Flag = EResultFlag.Failure;
        //        result.Exception = new ExceptionEx(ex, "GetCompletionAcceptanceResUploadModel");
        //    }
        //    return result;
        //}

        public Result<CompletionAcceptanceResUploadView> GetCompletionAcceptanceResUploadModel(long id)
        {
            Result<CompletionAcceptanceResUploadView> result = new Result<CompletionAcceptanceResUploadView>();
            try
            {
                var model = DataOperateBusiness<Epm_CompletionAcceptanceResUpload>.Get().GetModel(id);

                if (model == null)
                {
                    throw new Exception("该竣工验收信息不存在或已被删除！");
                }

                List<Epm_TzAttachs> attachsList = new List<Epm_TzAttachs>();
                attachsList = GetFilesByTZTable("Epm_CompletionAcceptanceResUpload", model.Id).Data;//根据表名和id获取附件信息
                if (attachsList != null && attachsList.Any())
                {
                    model.TzAttachs = attachsList;
                }

                CompletionAcceptanceResUploadView view = new CompletionAcceptanceResUploadView();

                view.CompletionAcceptanceResUpload = model;
                view.CompletionRectifyCompanys = DataOperateBusiness<Epm_CompletionRectifyCompany>.Get()
                    .GetList(p => p.AcceptanceId == id).ToList();

                string tableName = model.GetType().Name;

                view.BaseFiles = DataOperateBasic<Base_Files>.Get().GetList(p => p.TableName == tableName && p.TableId == id).ToList();

                result.Data = view;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCompletionAcceptanceResUploadModel");
            }
            return result;
        }
        /// <summary>
        /// 修改验收状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateCompletionAcceptanceResUploadState(List<long> ids, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                foreach (var item in ids)
                {
                    //验收申请信息
                    var model = DataOperateBusiness<Epm_CompletionAcceptanceResUpload>.Get().GetModel(item);
                    if (model != null)
                    {
                        SetCurrentUser(model);
                        model.State = (int)state.ToEnumReq<PreProjectApprovalState>();
                        var rows = DataOperateBusiness<Epm_CompletionAcceptanceResUpload>.Get().Update(model);

                        #region 试运行申请
                        //如果状态是已经提交，生成数据
                        if (model.State == (int)PreProjectApprovalState.ApprovalSuccess)
                        {

                            ProjectApprovalView vi = new ProjectApprovalView();
                            Epm_TzProjectPolit polt = new Epm_TzProjectPolit();

                            polt.ProjectId = model.ProjectId;//项目ID
                            polt.State = (int)PreProjectApprovalState.WaitSubmitted;//状态
                            polt.CompanyId = CurrentUser.CompanyId; ;

                            polt.CompanyName = CurrentUser.CompanyName;
                            polt.ProjectName = model.ProjectName;
                            SetCurrentUser(polt);
                            SetCreateUser(polt);
                            vi.ProjectPolit = polt;

                            AddProjectApproval(vi);
                        }
                        #endregion

                        result.Data = rows;
                        result.Flag = EResultFlag.Success;

                        #region 更新RPA数据
                        var hostAddress = System.Configuration.ConfigurationManager.AppSettings.Get("RPAPath");
                        var rpaModel = new RPA_CompletionInfo();
                        var omadsModel = new OMADS_CompletionInfo();
                        var tempModel = new TEMP_CompletionInfo();
                        #region rpaModel
                        rpaModel.CompletionDate = DateTime.Now;
                        rpaModel.Settlement = 0;
                        rpaModel.IsSynchron = "";
                        rpaModel.username = "sxxayw";

                        rpaModel.WriteMark = 0;
                        rpaModel.WriteResult = 0;
                        rpaModel.FollowOperate = "";

                        SetCreateUser(rpaModel);
                        SetCurrentUser(rpaModel);


                        #endregion
                        #region omadsModel
                        omadsModel.CompletionDate = DateTime.Now;
                        omadsModel.Settlement = 0;
                        omadsModel.IsSynchron = "";
                        omadsModel.username = "sxxayw";

                        omadsModel.WriteMark = 0;
                        omadsModel.WriteResult = 0;
                        omadsModel.FollowOperate = "";

                        SetCreateUser(rpaModel);
                        SetCurrentUser(rpaModel);

                        #endregion
                        #region tempModel
                        tempModel.CompletionDate = DateTime.Now;
                        tempModel.Settlement = 0;
                        tempModel.IsSynchron = "";
                        tempModel.username = "sxxayw";
                        tempModel.WriteMark = 0;
                        tempModel.WriteResult = 0;
                        tempModel.FollowOperate = "";

                        SetCreateUser(rpaModel);
                        SetCurrentUser(rpaModel);

                        #endregion

                        DataOperateBusiness<RPA_CompletionInfo>.Get().Add(rpaModel);
                        DataOperateBusiness<OMADS_CompletionInfo>.Get().Add(omadsModel);
                        DataOperateBusiness<TEMP_CompletionInfo>.Get().Add(tempModel);
                        #endregion
                    }
                    else
                    {
                        throw new Exception("该竣工验收信息不存在");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateCompletionAcceptanceResUploadState");
            }
            return result;
        }
    }
}
