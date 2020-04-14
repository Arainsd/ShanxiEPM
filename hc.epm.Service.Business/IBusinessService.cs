using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.ViewModel;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
namespace hc.epm.Service.Business
{
    [ServiceContract]
    [ServiceKnownType(typeof(Epm_DataConfig))]
    [ServiceKnownType(typeof(Epm_Milepost))]
    [ServiceKnownType(typeof(Epm_MilepostData))]
    [ServiceKnownType(typeof(Epm_Template))]
    [ServiceKnownType(typeof(Epm_Notice))]
    [ServiceKnownType(typeof(Epm_Project))]
    [ServiceKnownType(typeof(Epm_CheckItem))]
    [ServiceKnownType(typeof(Epm_ConstituteCompany))]
    [ServiceKnownType(typeof(Epm_ConstituteCompanyDetails))]
    public interface IBusinessService
    {
        /// <summary>
        /// 根据id查询项目资料信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Epm_DataConfig> GetDataConfigById(long id);

        /// <summary>
        /// 查询项目资料信息列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_DataConfig>> GetDataConfigListWhr(QueryCondition qc);

        /// <summary>
        /// 添加项目资料信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddDataConfig(Epm_DataConfig model, List<Base_Files> fileList = null);

        /// <summary>
        /// 修改项目资料信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateDataConfig(Epm_DataConfig model, List<Base_Files> fileList = null);

        /// <summary>
        /// 删除项目资料信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleMilestoneIds(List<long> ids);

        [OperationContract]
        Result<List<Epm_Milepost>> GetMilepostListQc(QueryCondition qc);
        /// <summary>
        /// 添加里程碑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddMilepost(Epm_Milepost model);

        /// <summary>
        /// 添加里程碑关联资料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddMilepostData(Epm_MilepostData model);
        /// <summary>
        /// 添加里程碑（多表添加）
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddMilepostAndData(Epm_Milepost model, List<long> ids);

        /// <summary>
        /// 修改里程碑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateMilepost(Epm_Milepost model, List<long> ids);

        /// <summary>
        /// 修改里程碑关联资料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateMilepostData(Epm_MilepostData model);

        /// <summary>
        /// 删除里程碑
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteMilepostbyIds(List<long> ids);

        /// <summary>
        /// 删除里程碑关联资料
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteMilepostDatabyIds(List<long> ids);

        /// <summary>
        /// 获取里程碑列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_Milepost>> GetMilepostList();

        /// <summary>
        /// 获取里程碑信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Epm_Milepost> GetMilepostById(long id);
        /// 查询油站信息列表
        /// </summary> 
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_OilStation>> GetOilStationList(QueryCondition qc);
        [OperationContract]
        Result<List<Base_VideoManage>> GetVideoManageList(QueryCondition qc);
        [OperationContract]
        Result<List<Epm_Project>> GetIndexProject(QueryCondition qc);
        /// <summary>
        /// 查询油站信息 
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Epm_OilStation> GetOilStationById(long id);
        [OperationContract]
        Result<Base_VideoManage> GetBaseVideoManageById(long id);

        /// <summary>
        /// 获取里程碑树形列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<MilepostView>> GetMilepostViewList(long parentId, int pageIndex, int pageSize);

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>Id</returns>
        [OperationContract]
        Result<long> AddTemplate(Epm_Template model, List<Base_Files> fileList = null);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTemplate(Epm_Template model, List<Base_Files> fileList = null);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTemplateByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_Template>> GetTemplateList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_Template> GetTemplateModel(long id);
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<long> AddTemplateDetails(Epm_TemplateDetails model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTemplateDetails(Epm_TemplateDetails model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTemplateDetailsByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TemplateDetails>> GetTemplateDetailsList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TemplateDetails> GetTemplateDetailsModel(long id);



        /// <summary>
        /// 根据里程碑id获取项目资料里程碑关联信息
        /// </summary>
        /// <param name="milepostId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_MilepostData>> GetMilepostDataByMilepostId(long milepostId);

        /// <summary>
        /// 修改项目资料状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="type">1：是否必填，2：是否启用</param>
        /// <returns></returns>
        [OperationContract]
        Result<int> ChangeDataConfigState(long id, bool state, int type);

        /// <summary>
        /// 里程碑启用禁用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> ChangeMilepostState(long id, bool state);

        /// <summary>
        /// 获取新闻类型列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_NewTarget>> GetNewTargetListWhr(QueryCondition qc);

        /// <summary>
        /// 获取新闻类型列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_NewTarget>> GetNewTargetList();

        /// <summary>
        /// 获取新闻分类详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Epm_NewTarget> GetNewTargetById(long id);

        /// <summary>
        /// 添加新闻类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddNewTarget(Epm_NewTarget model);

        /// <summary>
        /// 修改新闻类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateNewTarget(Epm_NewTarget model);

        /// <summary>
        /// 修改新闻类型状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> ChangeNewTargetState(long id, int state);

        /// <summary>
        /// 删除新闻类型
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteNewTargetByIds(List<long> ids);

        /// <summary>
        /// 获取新闻列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_News>> GetNewsList(QueryCondition qc);

        /// <summary>
        /// 获取新闻详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Epm_News> GetNewsById(long id);

        /// <summary>
        /// 添加新闻
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddNews(Epm_News model, List<Base_Files> fileList = null);

        /// <summary>
        /// 修改新闻
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateNews(Epm_News model, List<Base_Files> fileList = null);

        /// <summary>
        /// 删除新闻
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteNewsByIds(List<long> ids);

        /// <summary>
        /// 修改新闻状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="type">1,是否置顶；2，是否发布</param>
        /// <returns></returns>
        [OperationContract]
        Result<int> ChangeNewsState(long id, bool state, int type);

        /// <summary>
        /// 获取广告位列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_AdTarget>> GetAdTargetListWhr(QueryCondition qc);

        /// <summary>
        /// 获取广告位列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_AdTarget>> GetAdTargetList();

        /// <summary>
        /// 获取广告位详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Epm_AdTarget> GetAdTargetById(long id);

        /// <summary>
        /// 添加广告位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddAdTarget(Epm_AdTarget model);

        /// <summary>
        /// 修改广告位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateAdTarget(Epm_AdTarget model);

        /// <summary>
        /// 修改广告位状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> ChangeAdTargetState(long id, int state);

        /// <summary>
        /// 删除广告位
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteAdTargetByIds(List<long> ids);

        /// <summary>
        /// 获取广告投放列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_AdPutRecord>> GetAdPutRecordList(QueryCondition qc);

        /// <summary>
        /// 获取广告投放详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Epm_AdPutRecord> GetAdPutRecordById(long id);

        /// <summary>
        /// 添加广告投放
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddAdPutRecord(Epm_AdPutRecord model, List<Base_Files> fileList = null);

        /// <summary>
        /// 修改广告投放
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateAdPutRecord(Epm_AdPutRecord model, List<Base_Files> fileList = null);

        /// <summary>
        /// 删除广告投放
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteAdPutRecordByIds(List<long> ids);

        /// <summary>
        /// 修改广告投放状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> ChangeAdPutRecordState(long id, int state);

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddNotice(NoticeView model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteNoticeByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<NoticeView>> GetNoticeViewList(QueryCondition qc);
        [OperationContract]
        Result<List<Epm_Project>> GetProjectList(QueryCondition qc);
        [OperationContract]
        Result<List<MilepostView>> GetTemplateDetailsViewList(long templateId);

        #region 检查项
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddCheckItem(Epm_CheckItem model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateCheckItem(Epm_CheckItem model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteCheckItemByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_CheckItem>> GetCheckItemList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_CheckItem> GetCheckItemModel(long id);

        #endregion

        #region 批复构成
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddConstitute(Epm_Constitute view);

        /// <summary>
        /// 修改批复构成
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateConstitute(Epm_Constitute view);

        /// <summary>
        /// 删除批复构成表
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteConstituteByIds(List<long> ids);

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_Constitute>> GetConstituteList(QueryCondition qc);

        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_Constitute> GetConstituteModel(long id);

        #endregion

        #region 工程内容
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddWorkMainPoints(Epm_WorkMainPoints model);

        /// <summary>
        /// 修改批复构成
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateWorkMainPoints(Epm_WorkMainPoints model);

        /// <summary>
        /// 删除批复构成表
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteWorkMainPointsByIds(List<long> ids);

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_WorkMainPoints>> GetWorkMainPointsList(QueryCondition qc);

        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_WorkMainPoints> GetWorkMainPointsModel(long id);

        #endregion

        /// <summary>
        /// 项目性质列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_ProjectNature>> GetProjectNatureList(QueryCondition qc);

        #region 总批复构成与服务商关联

        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddConstituteCompany(ConstituteCompanyView view);

        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateConstituteCompany(ConstituteCompanyView view);

        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteConstituteCompanyByIds(List<long> ids);

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_ConstituteCompany>> GetConstituteCompanyList(QueryCondition qc);

        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        [OperationContract]
        Result<ConstituteCompanyView> GetConstituteCompanyModel(long id);


        /// <summary>
        /// 获取服务商（根据总批复构成获取关联的服务商）
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_ConstituteCompanyDetails>> GetConstituteCompanyDetailsList(QueryCondition qc);

        #endregion

        [OperationContract]
        Result<int> AddSendDate(Bp_SendDate model);

        #region  考勤设置
        ///<summary>
        ///添加:项目考勤设置表
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddProjectAttendance(ProjectAttendanceView model);
        ///<summary>
        ///修改:项目考勤设置表
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateProjectAttendance(Epm_ProjectAttendance model);
        ///<summary>
        ///删除:项目考勤设置表
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteProjectAttendanceByIds(List<long> ids);
        ///<summary>
        ///获取列表:项目考勤设置表
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_ProjectAttendance>> GetProjectAttendanceList(QueryCondition qc);
        ///<summary>
        ///获取详情:项目考勤设置表
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_ProjectAttendance> GetProjectAttendanceModel(long id);

        /// <summary>
        /// 获取考勤设置信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<AttendanceView> GetAttendanceModel();
        #endregion
    }
}
