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
        public Result<int> AddTzConDrawing(Epm_TzConDrawing model,List<Base_Files> baseEntity)
        {
            Result<int> result = new Result<int>();
            try
            {

                int rows = 0;
                bool isAdd = false;
                var reveiews = DataOperateBusiness<Epm_TzConDrawing>.Get().GetList(t => t.ProjectId == model.ProjectId).FirstOrDefault();
                var ApprovalInfo = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().GetList(t => t.ProjectId == model.ProjectId).FirstOrDefault();
                if (reveiews == null)
                {
                    isAdd = true;
                    reveiews = new Epm_TzConDrawing();
                    //当前创建人
                    SetCreateUser(reveiews);
                }
                reveiews.ProjectId = model.ProjectId;//项目id
                reveiews.ProjectCode = model.ProjectCode;//项目编码
                reveiews.ProjectName = model.ProjectName;//项目名称
                reveiews.ApprovalNo = model.ApprovalNo;//批复号
                reveiews.Nature = model.Nature;//项目性质
                reveiews.NatureName = model.NatureName;//项目性质名称
                reveiews.ApplyTime = model.ApplyTime;//项目提出日期
                reveiews.StationId = model.StationId;//站库id
                reveiews.StationCodeXt = model.StationCodeXt;//站库协同编码
                reveiews.StationName = model.StationName;//站库名称
                reveiews.CompanyId = model.CompanyId;//地市公司id
                reveiews.CompanyCodeXt = model.CompanyCodeXt;//地市公司协同编码
                reveiews.CompanyName = model.CompanyName;//地市公司名称
                reveiews.ReviewTime = model.ReviewTime;//评审日期
                reveiews.Moderator = model.Moderator;//主持人
                reveiews.ReviewAddress = model.ReviewAddress;//评审地点
                reveiews.ReviewExperts = model.ReviewExperts;//评审专家
                reveiews.Participants = model.Participants;//参会人员
                reveiews.Conclusion = model.Conclusion;//评审结论

                reveiews.State = model.State;//状态
                reveiews.Remark = model.Remark;//备注
                reveiews.DrawingId = model.DrawingId;//图纸id
                reveiews.ProjectDrawingID = model.ProjectDrawingID;//施工项目id

                //当前操作人
                SetCurrentUser(reveiews);
                //上传附件
                AddFilesBytzTable(reveiews, model.TzAttachs);
                #region  施工图纸会审流程申请     暂时注释  勿删！！！
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                {
                    TzConDrawingWorkFlowView view = new TzConDrawingWorkFlowView();

                    view.CompanyName = model.CompanyName;
                    view.Conclusion = model.CompanyName;
                    view.Moderator = model.Moderator;
                    view.NatureName = model.NatureName;
                    view.Participants = model.Participants;
                    view.PredictMoney = "";
                    view.ProjectCode = model.ProjectCode;
                    view.ProjectName = model.ProjectName;
                    view.ProvinceName = model.CompanyName;
                    view.ReviewAddress = model.ReviewAddress;
                    view.ReviewExperts = model.ReviewExperts;
                    view.ReviewTime = model.ReviewTime.ToString();
                    view.StationName = model.StationName;
                    view.StationTypeName = model.Nature;
                    view.LandCosts = ApprovalInfo.LandCosts.ToString();
                    view.OtherExpenses = ApprovalInfo.OtherExpenses.ToString();
                    view.EngineeringCost = ApprovalInfo.EngineeringCost.ToString();
                    var baseUser = DataOperateBasic<Base_User>.Get().GetModel(reveiews.CreateUserId);
                    if (baseUser == null)
                    {
                        throw new Exception("未找到申请人相关信息！");
                    }

                    view.hr_sqr = baseUser.ObjeId;
                    //上传附件
                    if (model.TzAttachs != null && model.TzAttachs.Any())
                    {
                        //string baseFaleUrl = System.Configuration.ConfigurationManager.AppSettings.Get("XtDownloadUrl");
                        //foreach (var item in model.TzAttachs)
                        //{
                        //    //string fileUrl = string.Format("{0}?fileId={1}&type={2}", baseFaleUrl, item.Id, item.TypeNo);
                        //    //view.Temp_TzAttachs = fileUrl + '|' + view.Temp_TzAttachs;
                        //    string fileUrl = XtWorkFlowService.GetXtAttachPaht(item.FilePath);
                        //    view.Temp_TzAttachs = fileUrl + '|' + view.Temp_TzAttachs;
                        //}
                        //if (view.Temp_TzAttachs != null)
                        //{
                        //    view.Temp_TzAttachs = view.Temp_TzAttachs.Substring(0, view.Temp_TzAttachs.Length - 1);
                        //}
                        view.Temp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(model.TzAttachs);
                    }

                    reveiews.WorkFlowId = XtWorkFlowService.CreateConDrawingWorkFlow(view);
                }
                #endregion
                if (isAdd)
                {

                    rows = DataOperateBusiness<Epm_TzConDrawing>.Get().Add(reveiews);
                }
                else
                {
                    rows = DataOperateBusiness<Epm_TzConDrawing>.Get().Update(reveiews);
                }
                
                
                if (baseEntity.Count!= 0 && model.State== (int)PreProjectApprovalState.ApprovalSuccess)//如果附件选择了图纸类型&&状态是通过了审核，往图纸管理同步一条数据
                 {
                    baseEntity = baseEntity.Where(p => p.ImageType == "SGTZSHTZ").ToList();//筛选附件是图纸类型（SGTZSHTZ）
                    drawingToDraw(reveiews, baseEntity);//整理图纸管理数据
                }

                

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                // WriteLog(AdminModule.TzConDrawing.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTzConDrawing");
            }
            return result;
        }



        #region 如果附件类型是SGTZSHTZ往图纸管理同步一条数据

        public bool drawingToDraw(Epm_TzConDrawing model, List<Base_Files> fileListFile)
        {
            
            Epm_Draw ed = new Epm_Draw();
            SetCreateUser(ed);
            ed.ProjectId = model.ProjectId;
            ed.ProjectName = model.ProjectName;
           // ed.Name = model.;
            //ed.Desciption = model.
            //ed.VersionOrder = model.Desciption;
            ed.VersionNo = "FirstVersion";
            ed.VersionName = "首版本";
            ed.SubmitUserId = CurrentUser.Id;
            ed.SubmitUserName = CurrentUser.UserName;
            ed.SubmitCompanyId = CurrentCompanyID.ToLongReq();
            ed.SubmitCompanyName = CurrentCompanyName;
            ed.SubmitDate = DateTime.Now;
            ed.DesignCompanyId = model.CompanyId;
            ed.DesignCompanyName = model.CompanyName;
            ed.IsNew = true;
            ed.State = (int)ApprovalState.ApprSuccess; //设置状态为已审核
            ed.Remark = model.Remark;
            ed.CrtCompanyId = model.CompanyId;
            ed.CrtCompanyName = model.CompanyName;
            ed.IsDelete = false;
            ed.OperateTime = DateTime.Now;
            ed.OperateUserId= CurrentUser.Id;
            ed.OperateUserName= CurrentUser.UserName;
            ed.IsValidate = false;
            ed.CrtCompanyId = CurrentCompanyID.ToLongReq();
            ed.CrtCompanyName = CurrentCompanyName;

            var result = AddDraw(ed, fileListFile);//调用图纸管理新增服务-同步数据
            if (result.Data<1)
            {
                return false;
            }
            return true;
        }
    
        #endregion

        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTzConDrawing(Epm_TzConDrawing model, List<Base_Files> baseEntity)
        {
            Result<int> result = new Result<int>();
            try
            {
                var drawing = DataOperateBusiness<Epm_TzConDrawing>.Get().Single(p => p.Id == model.Id);
                model.ProjectId = drawing.ProjectId;//项目ID
                model.ProjectCode = drawing.ProjectCode;//项目编码
                model.ProjectName = drawing.ProjectName;//项目名称
                model.ApprovalNo = drawing.ApprovalNo;//项目批复号
                model.Nature = drawing.Nature;//项目性质
                model.NatureName = drawing.NatureName;//项目性质名称
                model.ApplyTime = drawing.ApplyTime;//项目提出时间
                model.StationId = drawing.StationId;//站库id
                model.StationCodeXt = drawing.StationCodeXt;//站库协同编码
                model.StationName = drawing.StationName;//站库名称
                model.CompanyId = drawing.CompanyId;//地市公司ID
                model.CompanyCodeXt = drawing.CompanyCodeXt;//地市公司编码
                model.CompanyName = drawing.CompanyName;//地市公司名称

                SetCurrentUser(model);
                //上传图纸
                AddFilesBytzTable(model, model.TzAttachs);
                #region  施工图纸会审流程申请     暂时注释  勿删！！！
                var XtWorkFlow = System.Configuration.ConfigurationManager.AppSettings.Get("XtWorkFlow");
                if (model.State == (int)PreProjectState.WaitApproval && XtWorkFlow == "1")
                {
                    TzConDrawingWorkFlowView view = new TzConDrawingWorkFlowView();
                    var baseUser = DataOperateBasic<Base_User>.Get().GetModel(model.CreateUserId);
                    if (baseUser == null)
                    {
                        throw new Exception("未找到申请人相关信息！");
                    }
                    else
                    {
                        view.hr_sqr = baseUser.ObjeId;
                    }
                    view.CompanyName = model.CompanyName;
                    view.Conclusion = model.CompanyName;
                    view.Moderator = model.Moderator;
                    view.NatureName = model.NatureName;
                    view.Participants = model.Participants;
                    view.PredictMoney = "";
                    view.ProjectCode = model.ProjectCode;
                    view.ProjectName = model.ProjectName;
                    view.ProvinceName = model.CompanyName;
                    view.ReviewAddress = model.ReviewAddress;
                    view.ReviewExperts = model.ReviewExperts;
                    view.ReviewTime = model.ReviewTime.ToString();
                    view.StationName = model.StationName;
                    view.StationTypeName = model.Nature;

                    //上传附件
                    if (model.TzAttachs != null && model.TzAttachs.Any())
                    {
                        //string baseFaleUrl = System.Configuration.ConfigurationManager.AppSettings.Get("XtDownloadUrl");
                        //foreach (var item in model.TzAttachs)
                        //{
                        //    //string fileUrl = string.Format("{0}?fileId={1}&type={2}", baseFaleUrl, item.Id, item.TypeNo);
                        //    //view.Temp_TzAttachs = fileUrl + '|' + view.Temp_TzAttachs;
                        //    string fileUrl = XtWorkFlowService.GetXtAttachPaht(item.FilePath);
                        //    view.Temp_TzAttachs = fileUrl + '|' + view.Temp_TzAttachs;
                        //}
                        //if (view.Temp_TzAttachs != null)
                        //{
                        //    view.Temp_TzAttachs = view.Temp_TzAttachs.Substring(0, view.Temp_TzAttachs.Length - 1);
                        //}
                        view.Temp_TzAttachs = XtWorkFlowSubmitService.CreateXtAttachPath(model.TzAttachs);
                    }

                    model.WorkFlowId = XtWorkFlowService.CreateConDrawingWorkFlow(view);
                }
                #endregion

                var rows = DataOperateBusiness<Epm_TzConDrawing>.Get().Update(model);
                //AddFilesBytzTable(reveiews, model.TzAttachs);
                
                //if (model.TzAttachs != null)
                //{
                //    //删除之前的附件
                //    DeleteFilesByTable(model.GetType().Name, new List<long>() { model.Id });
                //    //新增附件
                //    AddFilesBytzTable(model, model.TzAttachs);
                //}
                //else
                //{
                //    //删除之前的附件
                //    DeleteFilesByTable(model.GetType().Name, new List<long>() { model.Id });
                //}

                if (baseEntity.Count != 0 && model.State == (int)PreProjectApprovalState.ApprovalSuccess)//如果附件选择了图纸类型&&状态是通过了审核，往图纸管理同步一条数据
                {
                    baseEntity = baseEntity.Where(p => p.ImageType == "SGTZSHTZ").ToList();
                    drawingToDraw(model, baseEntity);
                }

                result.Data = rows;
                result.Flag = EResultFlag.Success;
               // WriteLog(AdminModule.TzConDrawing.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzConDrawing");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTzConDrawingByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {

                var models = DataOperateBusiness<Epm_TzConDrawing>.Get().GetList(i => ids.Contains(i.Id)).ToList();

                var rows = DataOperateBusiness<Epm_TzConDrawing>.Get().DeleteRange(models);//删除施工图纸

                DeleteFilesByTable(models.FirstOrDefault().GetType().Name, ids);//删除附件

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.TzConDrawing.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTzConDrawingByIds");
            }
            return result;
        }

        public Result<ProjectBasicsInfoView> GetProjectBasicInfoByID(long id)
        {
            Result<ProjectBasicsInfoView> projInfo = new Result<ProjectBasicsInfoView>();
            try
            {
                var res = from a in context.Epm_TzProjectProposal
                          join b in context.Epm_TzProjectApprovalInfo on a.Id equals b.ProjectId
                          where a.Id == id
                          select new { a.Id, a.StationName, a.NatureName, a.ProjectCode, a.CompanyName, a.PredictMoney, b.EngineeringCost, b.LandCosts, b.OtherExpenses,b.ProjectType,b.RegionCompany };

                var pro = res.Select(p => new ProjectBasicsInfoView
                {
                    Id = p.Id,//id
                    StationName = p.StationName,//项目id
                    NatureName = p.NatureName,//站库名称
                    ProjectCode = p.ProjectCode,//项目代码
                    CompanyName = p.CompanyName,//地市公司
                    PredictMoney = p.PredictMoney,//估算投资
                    EngineeringCost = p.EngineeringCost,//工程费用
                    LandCosts = p.LandCosts,//土地费用
                    OtherExpenses = p.OtherExpenses,//其它费用
                    ProjectType = p.ProjectType,//项目类型
                    RegionCompany = p.RegionCompany,//地区公司

                }).FirstOrDefault();

                if (pro==null)
                {
                    throw new Exception("该项目没有批复信息");
                }
                projInfo.Data = pro;
                projInfo.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {

                projInfo.Data = null;
                projInfo.Flag = EResultFlag.Failure;
                projInfo.Exception = new ExceptionEx(ex, "GetProjectBasicInfoByID");
            }
           
            return projInfo;
        }

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合Epm_TzProjectProposal项目信息表，Epm_TzConDrawing图纸信息表</returns>
        public Result<List<Epm_TzConDrawing>> GetTzConDrawingList(QueryCondition qc)
        {
            //  qc = AddDefault(qc);
            Result<List<Epm_TzConDrawing>> result = new Result<List<Epm_TzConDrawing>>();
            try { 
           
            var res = from a in context.Epm_TzConDrawing
                      where a.State != (int)PreProjectApprovalState.ApprovalFailure
                      join b in context.Epm_TzProjectProposal.Where(p => p.State != (int)PreProjectApprovalState.ApprovalFailure) on a.ProjectId equals b.Id
                      select new
                      {
                          a.Id,
                          a.ProjectId,
                          b.ProjectName,
                          b.Nature,
                          b.NatureName,
                          b.StationName,
                          b.StationId,
                          b.ApplyTime,
                          a.State,
                          a.OperateUserName,
                          a.OperateTime,
                          a.CompanyId,
                          a.CreateUserName,
                          a.CompanyName
                      };
            var drawingList = res.ToList().Select(p => new Epm_TzConDrawing
            {
                Id = p.Id,//施工图纸ID
                ProjectId = p.ProjectId,//项目id
                ProjectName = p.ProjectName,//项目名称
                Nature = p.Nature,//项目性质
                NatureName = p.NatureName,//项目性质名称
                StationName = p.StationName,//库站名称
                StationId = p.StationId,//库站id
                ApplyTime = p.ApplyTime,//最后操作时间
                State = p.State,//状态
                //如果是待审批，当前操作人为当前审批人；如果是待提交、已驳回状态，当前操作人是流程发起人；如果是已审批、已关闭状态是，当前操作人为空；
                OperateUserName = p.State==0 || p.State == 20 ? p.CreateUserName: p.State ==10 ? p.OperateUserName :"",
                OperateTime =p.OperateTime,
                CompanyId=p.CompanyId,
                CompanyName = p.CompanyName
            }).ToList();
            if (qc != null && qc.ConditionList.Any())
            {
                foreach (var conditionExpression in qc.ConditionList)
                {
                    string value = (conditionExpression.ExpValue ?? "").ToString();
                    string valueName = (conditionExpression.ExpName ?? "").ToString();
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        switch (valueName)
                        {
                            case "ProjectName":
                                {
                                    drawingList = drawingList.Where(t => t.ProjectName.Contains(value)).ToList();//项目名称
                                    break;
                                }
                            case "Nature":
                                {
                                    drawingList = drawingList.Where(t => t.Nature == value).ToList();//项目性质名称
                                    break;
                                }
                            case "StationId":
                                {
                                    long id = Convert.ToInt64(value);
                                    drawingList = drawingList.Where(t => t.StationId == id).ToList();//库站id
                                    break;
                                }
                            case "StationName":
                                {
                                    drawingList = drawingList.Where(t => t.StationName.Contains(value)).ToList();//库站名称
                                    break;
                                }
                            case "startTime":
                                {
                                    DateTime startTime1 = Convert.ToDateTime(value);
                                    drawingList = drawingList.Where(t => t.ApplyTime >= startTime1).ToList();
                                    break;
                                }
                            case "endTime":
                                {
                                    DateTime endTime1 = Convert.ToDateTime(value);
                                    drawingList = drawingList.Where(t => t.ApplyTime <= endTime1).ToList();
                                    break;
                                }
                            case "State":
                                {
                                    int state = Convert.ToInt32(value);
                                    drawingList = drawingList.Where(t => t.State == state).ToList();//状态
                                    break;
                                }
                                case "CompanyId":
                                    {
                                        long id = Convert.ToInt64(value);
                                        drawingList = drawingList.Where(t => t.CompanyId == id).ToList();
                                        break;
                                    }
                                default:
                                break;
                        }
                    }
                }
            }

            result.Data = drawingList;
            result.Flag = EResultFlag.Success;
        }
            //Result<List<Epm_TzConDrawing>> result = new Result<List<Epm_TzConDrawing>>();
            //try
            //{
            //    result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TzConDrawing>(context, qc);
            //}
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzConDrawingList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:需要返回两张表的信息（项目信息，施工图纸信息）
        ///</summary>
        /// <param name="id">施工图纸信息表的id</param>
        /// <returns>施工图纸数据详情model</returns>
        public Result<TzConDrawingView> GetTzConDrawingModel(long id)
        {
            Result<TzConDrawingView> result = new Result<TzConDrawingView>();
            try
            {
                TzConDrawingView view = new TzConDrawingView();
                var model = DataOperateBusiness<Epm_TzConDrawing>.Get().GetModel(id);
               // var model = DataOperateBusiness<Epm_TzConDrawing>.Get().Single(p => p.ProjectId == id);
                if (model != null)
                {
                    var proInfo = DataOperateBusiness<Epm_TzProjectApprovalInfo>.Get().Single(p=>p.ProjectId == model.ProjectId);//根据施工图纸信息表里的项目id取得项目批复信息Epm_TzProjectApprovalInfo

                    var proposal = DataOperateBusiness<Epm_TzProjectProposal>.Get().Single(p => p.Id == model.ProjectId);
                    if (proposal!=null)
                    {
                        view.StationName = proposal.StationName;

                        view.ProjectName = proposal.ProjectName;
                        view.NatureName = proposal.NatureName;
                        view.ProjectCode = proposal.ProjectCode;
                        view.CompanyName = proposal.CompanyName;
                        view.PredictMoney = proposal.PredictMoney;

                    }

                    List<Epm_TzAttachs> attachsList = new List<Epm_TzAttachs>();
                    attachsList = GetFilesByTZTable("Epm_TzConDrawing", model.Id).Data;//根据表名和id获取附件信息
                    if (attachsList!=null&& attachsList.Any())
                    {
                        model.TzAttachs = attachsList;
                    }
                    view.TzProjectApproval = proInfo;
                }
                view.TzConDrawing = model;
                result.Data = view;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTzConDrawingModel");
            }
            return result;
        }


        /// <summary>
        /// 修改施工图纸会审状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzConDrawingState(List<long> ids, string state)
        {
            Result<int> result = new Result<int>();
            try
            {
                foreach (var item in ids)
                {
                    var model = DataOperateBusiness<Epm_TzConDrawing>.Get().GetModel(item);
                    if (model != null)
                    {
                        //SetCurrentUser(model);
                        model.State = (int)state.ToEnumReq<PreProjectApprovalState>();
                        var rows = DataOperateBusiness<Epm_TzConDrawing>.Get().Update(model);

                        //如果状态是已经提交，生成数据
                        if (model.State == (int)PreProjectApprovalState.ApprovalSuccess)
                        {
                            #region 自动生成开工申请信息
                            //项目提出信息
                            var tzProjectProposal = DataOperateBusiness<Epm_TzProjectProposal>.Get().GetModel(model.ProjectId.Value);

                            Epm_TzProjectStartApply tzProjectStartApply = new Epm_TzProjectStartApply();

                            tzProjectStartApply.ProjectId = model.ProjectId;//项目ID
                            tzProjectStartApply.State = (int)PreProjectApprovalState.WaitSubmitted;//状态
                            tzProjectStartApply.ApplyTitle = model.ProjectName; ;


                            //申请人
                            tzProjectStartApply.ApplyUserName = CurrentUser.UserAcct;
                            tzProjectStartApply.ApplyUserId = CurrentUser.Id;
                            //申请单位
                            tzProjectStartApply.ApplyCompanyName = CurrentUser.CompanyName;
                            tzProjectStartApply.ApplyCompanyId = CurrentUser.CompanyId;
                            //申请时间
                            tzProjectStartApply.StartApplyTime = DateTime.Now;
                           

                            //标题
                            tzProjectStartApply.ApplyTitle = model.ProjectName+ "项目开工申请" + CurrentUser.UserAcct + DateTime.Now.ToString("yyyy-MM-dd");

                            tzProjectStartApply.ProjectCode = tzProjectProposal.ProjectCode;
                            tzProjectStartApply.ApprovalNo = tzProjectProposal.ApprovalNo;
                            tzProjectStartApply.Nature = tzProjectProposal.Nature;
                            tzProjectStartApply.NatureName = tzProjectProposal.NatureName;
                            tzProjectStartApply.ApplyTime = tzProjectProposal.ApplyTime;
                            tzProjectStartApply.StationId = tzProjectProposal.StationId;
                            tzProjectStartApply.StationCodeXt = tzProjectProposal.StationCodeXt;
                            tzProjectStartApply.StationName = tzProjectProposal.StationName;
                            tzProjectStartApply.CompanyId = tzProjectProposal.CompanyId;
                            tzProjectStartApply.CompanyCodeXt = tzProjectProposal.CompanyCodeXt;
                            tzProjectStartApply.CompanyName = tzProjectProposal.CompanyName;
                            tzProjectStartApply.ProjectName = tzProjectProposal.ProjectName;

                            //AddTzProjectStartApply(tzProjectStartApply);
                            SetCreateUser(tzProjectStartApply);
                            SetCurrentUser(tzProjectStartApply);
                            DataOperateBusiness<Epm_TzProjectStartApply>.Get().Add(tzProjectStartApply);
                            #endregion

                            #region 自动生成RPA数据
                            string houstAddress = System.Configuration.ConfigurationManager.AppSettings.Get("RPAPath");
                            RPA_TzConDrawing RPAModel = new RPA_TzConDrawing();

                            RPAModel.ProjectTypeName = model.NatureName;//项目类型
                            RPAModel.ReviewTime = model.ReviewTime;
                            RPAModel.ReviewAddress = model.ReviewAddress;
                            RPAModel.Moderator = model.Moderator;
                            RPAModel.ReviewExperts = model.ReviewExperts;
                            RPAModel.Participants = model.Participants;
                            RPAModel.Conclusion = model.Conclusion;
                            RPAModel.Conclusion = model.Conclusion == "XMPSJL1" ? "同意" : model.Conclusion == "XMPSJL2" ? "原则同意" : model.Conclusion == "XMPSJL3" ? "不同意" : "";
                            RPAModel.Remark = model.Remark;
                            RPAModel.username = "sxgcyw";
                            RPAModel.companys = CurrentUser.CompanyName;
                            RPAModel.WriteMark = 0;
                            RPAModel.WriteResult = 0;
                            RPAModel.FollowOperate = "";

                            var fileList = GetFilesByTZTable("Epm_TzConDrawing", model.Id).Data;
                            for (int i = 0; i < fileList.Count; i++)
                            {
                                RPAModel.FilePath += houstAddress + "?path=" + fileList[i].FilePath + "&fileName=" + fileList[i].Name + ",";
                            }
                            RPAModel.FileNumber = fileList.Count;

                            SetCreateUser(RPAModel);
                            SetCurrentUser(RPAModel);
                            DataOperateBusiness<RPA_TzConDrawing>.Get().Add(RPAModel);
                            #endregion
                            #region 自动生成OMADS数据
                            OMADS_TzConDrawing oMADSModel = new OMADS_TzConDrawing();

                            oMADSModel.ProjectTypeName = model.NatureName;//项目类型
                            oMADSModel.ReviewTime = model.ReviewTime;
                            oMADSModel.ReviewAddress = model.ReviewAddress;
                            oMADSModel.Moderator = model.Moderator;
                            oMADSModel.ReviewExperts = model.ReviewExperts;
                            oMADSModel.Participants = model.Participants;
                            oMADSModel.Conclusion = model.Conclusion == "XMPSJL1" ? "同意" : model.Conclusion == "XMPSJL2" ? "原则同意" : model.Conclusion == "XMPSJL3" ? "不同意" : "";
                            oMADSModel.Remark = model.Remark;
                            oMADSModel.username = CurrentUser.UserAcct;
                            oMADSModel.companys = CurrentUser.CompanyName;
                            oMADSModel.WriteMark = 0;
                            oMADSModel.WriteResult = 0;
                            oMADSModel.FollowOperate = "";

                            var ofileList = GetFilesByTZTable("Epm_TzConDrawing", model.Id).Data;
                            for (int i = 0; i < ofileList.Count; i++)
                            {
                                oMADSModel.FilePath += houstAddress + "?path=" + ofileList[i].FilePath + "&fileName=" + fileList[i].Name + ",";
                            }
                            oMADSModel.FileNumber = ofileList.Count;

                            SetCreateUser(oMADSModel);
                            SetCurrentUser(oMADSModel);
                            DataOperateBusiness<OMADS_TzConDrawing>.Get().Add(oMADSModel);
                            #endregion
                            #region 自动生成TEMP数据
                            TEMP_TzConDrawing tEMPModel = new TEMP_TzConDrawing();

                            tEMPModel.ProjectTypeName = model.NatureName;//项目类型
                            tEMPModel.ReviewTime = model.ReviewTime;
                            tEMPModel.ReviewAddress = model.ReviewAddress;
                            tEMPModel.Moderator = model.Moderator;
                            tEMPModel.ReviewExperts = model.ReviewExperts;
                            tEMPModel.Participants = model.Participants;
                            tEMPModel.Conclusion = model.Conclusion == "XMPSJL1" ? "同意" : model.Conclusion == "XMPSJL2" ? "原则同意" : model.Conclusion == "XMPSJL3" ? "不同意" : "";
                            tEMPModel.Remark = model.Remark;
                            tEMPModel.username = CurrentUser.UserAcct;
                            tEMPModel.companys = CurrentUser.CompanyName;
                            tEMPModel.WriteMark = 0;
                            tEMPModel.WriteResult = 0;
                            tEMPModel.FollowOperate = "";

                            var tfileList = GetFilesByTZTable("Epm_TzConDrawing", model.Id).Data;
                            for (int i = 0; i < tfileList.Count; i++)
                            {
                                tEMPModel.FilePath += houstAddress + "?path=" + tfileList[i].FilePath + "&fileName=" + fileList[i].Name + ",";
                            }
                            tEMPModel.FileNumber = tfileList.Count;

                            SetCreateUser(tEMPModel);
                            SetCurrentUser(tEMPModel);
                            DataOperateBusiness<TEMP_TzConDrawing>.Get().Add(tEMPModel);
                            #endregion
                        }

                        result.Data = rows;
                        result.Flag = EResultFlag.Success;
                    }
                    else
                    {
                        throw new Exception("该施工图纸会审不存在");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTzConDrawingState");
            }
            return result;
        }
    }
}
