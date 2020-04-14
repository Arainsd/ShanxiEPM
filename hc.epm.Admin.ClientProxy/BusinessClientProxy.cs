using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.Service.Business;
using hc.epm.ViewModel;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Admin.ClientProxy
{
    public class BusinessClientProxy : ClientBase<IBusinessService>, IBusinessService
    {
        public object PreCode { get; set; }
        public object PreName { get; set; }
        public object PId { get; set; }

        public BusinessClientProxy(hc.Plat.Common.Global.ClientProxyExType cpet)
        {

            //传输当前用户的信息；
            ApplicationContext.Current.UserID = cpet.UserID;
            ApplicationContext.Current.WebIP = cpet.IP_WebServer;
            ApplicationContext.Current.ClientIP = cpet.IP_Client;


            /*以下密码是用作在应用服务器中使用程序验证密码的作用*/
            string FilePath = AppDomain.CurrentDomain.BaseDirectory + "bin\\";
            string user = "";
            string pass = "";
            string msg = DesTool.LoadCertUserPass(FilePath, out user, out pass);
            if (msg != "")
            {
                throw new Exception(msg);
            }
            ClientCredentials.UserName.UserName = user;
            ClientCredentials.UserName.Password = pass;
            /*OK*/
        }
        /// <summary>
        /// 添加项目资料信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddDataConfig(Epm_DataConfig model, List<Base_Files> fileList = null)
        {
            return base.Channel.AddDataConfig(model, fileList);
        }

        /// <summary>
        /// 修改项目资料信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateDataConfig(Epm_DataConfig model, List<Base_Files> fileList = null)
        {
            return base.Channel.UpdateDataConfig(model, fileList);
        }

        /// <summary>
        /// 删除项目资料信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleMilestoneIds(List<long> ids)
        {
            return base.Channel.DeleMilestoneIds(ids);
        }

        /// <summary>
        /// 获取项目资料信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_DataConfig> GetDataConfigById(long id)
        {
            return base.Channel.GetDataConfigById(id);
        }

        /// <summary>
        /// 获取项目资料列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_DataConfig>> GetDataConfigListWhr(QueryCondition qc)
        {
            return base.Channel.GetDataConfigListWhr(qc);
        }

        public Result<List<Epm_Milepost>> GetMilepostListQc(QueryCondition qc)
        {
            return base.Channel.GetMilepostListQc(qc);
        }

        /// <summary>
        /// 添加里程碑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddMilepost(Epm_Milepost model)
        {
            return base.Channel.AddMilepost(model);
        }

        /// <summary>
        /// 添加里程碑关联资料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddMilepostData(Epm_MilepostData model)
        {
            return base.Channel.AddMilepostData(model);
        }

        /// <summary>
        /// 添加里程碑（多表添加）
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> AddMilepostAndData(Epm_Milepost model, List<long> ids)
        {
            return base.Channel.AddMilepostAndData(model, ids);
        }


        /// <summary>
        /// 修改里程碑
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateMilepost(Epm_Milepost model, List<long> ids)
        {
            return base.Channel.UpdateMilepost(model, ids);
        }

        /// <summary>
        /// 修改里程碑关联资料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateMilepostData(Epm_MilepostData model)
        {
            return base.Channel.UpdateMilepostData(model);
        }

        /// <summary>
        /// 删除里程碑
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteMilepostbyIds(List<long> ids)
        {
            return base.Channel.DeleteMilepostbyIds(ids);
        }

        /// <summary>
        /// 删除里程碑关联资料
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteMilepostDatabyIds(List<long> ids)
        {
            return base.Channel.DeleteMilepostDatabyIds(ids);
        }

        /// <summary>
        /// 获取里程碑列表
        /// </summary>
        /// <returns></returns>
        public Result<List<Epm_Milepost>> GetMilepostList()
        {
            return base.Channel.GetMilepostList();
        }

        /// <summary>
        /// 获取里程碑信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_Milepost> GetMilepostById(long id)
        {
            return base.Channel.GetMilepostById(id);
        }
        /// 查询油站信息列表
        /// </summary>
        /// <param name="qc"></param> 
        /// <returns></returns>
        public Result<List<Epm_OilStation>> GetOilStationList(QueryCondition qc)
        {
            return base.Channel.GetOilStationList(qc);
        }
        public Result<List<Base_VideoManage>> GetVideoManageList(QueryCondition qc)
        {
            return base.Channel.GetVideoManageList(qc);
        }
        public Result<List<Epm_Project>> GetIndexProject(QueryCondition qc)
        {
            return base.Channel.GetIndexProject(qc);
        }

        /// <summary>
        /// 查询油站信息列表 GetOilStationById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_OilStation> GetOilStationById(long id)
        {
            return base.Channel.GetOilStationById(id);
        }
        public Result<Base_VideoManage> GetBaseVideoManageById(long id)
        {
            return base.Channel.GetBaseVideoManageById(id);
        }

        /// <summary>
        /// 获取里程碑树形列表
        /// </summary>
        /// <returns></returns>
        public Result<List<MilepostView>> GetMilepostViewList(long parentId, int pageIndex, int pageSize)
        {
            return base.Channel.GetMilepostViewList(parentId, pageIndex, pageSize);
        }


        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>Id</returns>
        public Result<long> AddTemplate(Epm_Template model, List<Base_Files> fileList = null)
        {
            return base.Channel.AddTemplate(model, fileList);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTemplate(Epm_Template model, List<Base_Files> fileList = null)
        {
            return base.Channel.UpdateTemplate(model, fileList);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTemplateByIds(List<long> ids)
        {
            return base.Channel.DeleteTemplateByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_Template>> GetTemplateList(QueryCondition qc)
        {
            return base.Channel.GetTemplateList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_Template> GetTemplateModel(long id)
        {
            return base.Channel.GetTemplateModel(id);
        }
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<long> AddTemplateDetails(Epm_TemplateDetails model)
        {
            return base.Channel.AddTemplateDetails(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTemplateDetails(Epm_TemplateDetails model)
        {
            return base.Channel.UpdateTemplateDetails(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTemplateDetailsByIds(List<long> ids)
        {
            return base.Channel.DeleteTemplateDetailsByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TemplateDetails>> GetTemplateDetailsList(QueryCondition qc)
        {
            return base.Channel.GetTemplateDetailsList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TemplateDetails> GetTemplateDetailsModel(long id)
        {
            return base.Channel.GetTemplateDetailsModel(id);
        }


        /// <summary>
        /// 根据里程碑id获取项目资料里程碑关联信息
        /// </summary>
        /// <param name="milepostId"></param>
        /// <returns></returns>
        public Result<List<Epm_MilepostData>> GetMilepostDataByMilepostId(long milepostId)
        {
            return base.Channel.GetMilepostDataByMilepostId(milepostId);
        }

        /// <summary>
        /// 修改项目资料状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="type">1：是否必填，2：是否启用</param>
        /// <returns></returns>
        public Result<int> ChangeDataConfigState(long id, bool state, int type)
        {
            return base.Channel.ChangeDataConfigState(id, state, type);
        }

        /// <summary>
        /// 里程碑启用禁用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeMilepostState(long id, bool state)
        {
            return base.Channel.ChangeMilepostState(id, state);
        }

        /// <summary>
        /// 获取新闻类型列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_NewTarget>> GetNewTargetListWhr(QueryCondition qc)
        {
            return base.Channel.GetNewTargetListWhr(qc);
        }

        /// <summary>
        /// 获取新闻类型列表
        /// </summary>
        /// <returns></returns>
        public Result<List<Epm_NewTarget>> GetNewTargetList()
        {
            return base.Channel.GetNewTargetList();
        }

        /// <summary>
        /// 获取新闻分类详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_NewTarget> GetNewTargetById(long id)
        {
            return base.Channel.GetNewTargetById(id);
        }

        /// <summary>
        /// 添加新闻类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddNewTarget(Epm_NewTarget model)
        {
            return base.Channel.AddNewTarget(model);
        }

        /// <summary>
        /// 修改新闻类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateNewTarget(Epm_NewTarget model)
        {
            return base.Channel.UpdateNewTarget(model);
        }

        /// <summary>
        /// 修改新闻类型状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeNewTargetState(long id, int state)
        {
            return base.Channel.ChangeNewTargetState(id, state);
        }
        /// <summary>
        /// 删除新闻类型
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteNewTargetByIds(List<long> ids)
        {
            return base.Channel.DeleteNewTargetByIds(ids);
        }

        /// <summary>
        /// 获取新闻列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_News>> GetNewsList(QueryCondition qc)
        {
            return base.Channel.GetNewsList(qc);
        }

        /// <summary>
        /// 获取新闻详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_News> GetNewsById(long id)
        {
            return base.Channel.GetNewsById(id);
        }

        /// <summary>
        /// 添加新闻
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList"></param>
        /// <returns></returns>
        public Result<int> AddNews(Epm_News model, List<Base_Files> fileList = null)
        {
            return base.Channel.AddNews(model, fileList);
        }

        /// <summary>
        /// 修改新闻
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateNews(Epm_News model, List<Base_Files> fileList = null)
        {
            return base.Channel.UpdateNews(model, fileList);
        }

        /// <summary>
        /// 删除新闻
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteNewsByIds(List<long> ids)
        {
            return base.Channel.DeleteNewsByIds(ids);
        }

        /// <summary>
        /// 修改新闻状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="type">1,是否置顶；2，是否发布</param>
        /// <returns></returns>
        public Result<int> ChangeNewsState(long id, bool state, int type)
        {
            return base.Channel.ChangeNewsState(id, state, type);
        }

        /// <summary>
        /// 获取广告位列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_AdTarget>> GetAdTargetListWhr(QueryCondition qc)
        {
            return base.Channel.GetAdTargetListWhr(qc);
        }

        /// <summary>
        /// 获取广告位列表
        /// </summary>
        /// <returns></returns>
        public Result<List<Epm_AdTarget>> GetAdTargetList()
        {
            return base.Channel.GetAdTargetList();
        }

        /// <summary>
        /// 获取广告位详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_AdTarget> GetAdTargetById(long id)
        {
            return base.Channel.GetAdTargetById(id);
        }

        /// <summary>
        /// 添加广告位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddAdTarget(Epm_AdTarget model)
        {
            return base.Channel.AddAdTarget(model);
        }

        /// <summary>
        /// 修改广告位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateAdTarget(Epm_AdTarget model)
        {
            return base.Channel.UpdateAdTarget(model);
        }

        /// <summary>
        /// 修改广告位状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeAdTargetState(long id, int state)
        {
            return base.Channel.ChangeAdTargetState(id, state);
        }

        /// <summary>
        /// 删除广告位
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteAdTargetByIds(List<long> ids)
        {
            return base.Channel.DeleteAdTargetByIds(ids);
        }

        /// <summary>
        /// 获取广告投放列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_AdPutRecord>> GetAdPutRecordList(QueryCondition qc)
        {
            return base.Channel.GetAdPutRecordList(qc);
        }

        /// <summary>
        /// 获取广告投放详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_AdPutRecord> GetAdPutRecordById(long id)
        {
            return base.Channel.GetAdPutRecordById(id);
        }

        /// <summary>
        /// 添加广告投放
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList"></param>
        /// <returns></returns>
        public Result<int> AddAdPutRecord(Epm_AdPutRecord model, List<Base_Files> fileList = null)
        {
            return base.Channel.AddAdPutRecord(model, fileList);
        }

        /// <summary>
        /// 修改广告投放
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateAdPutRecord(Epm_AdPutRecord model, List<Base_Files> fileList = null)
        {
            return base.Channel.UpdateAdPutRecord(model, fileList);
        }

        /// <summary>
        /// 删除广告投放
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteAdPutRecordByIds(List<long> ids)
        {
            return base.Channel.DeleteAdPutRecordByIds(ids);
        }

        /// <summary>
        /// 修改广告投放状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeAdPutRecordState(long id, int state)
        {
            return base.Channel.ChangeAdPutRecordState(id, state);
        }


        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddNotice(NoticeView model)
        {
            return base.Channel.AddNotice(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteNoticeByIds(List<long> ids)
        {
            return base.Channel.DeleteNoticeByIds(ids);
        }
        public Result<List<NoticeView>> GetNoticeViewList(QueryCondition qc)
        {
            return base.Channel.GetNoticeViewList(qc);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_Project>> GetProjectList(QueryCondition qc)
        {
            return base.Channel.GetProjectList(qc);
        }

        public Result<List<MilepostView>> GetTemplateDetailsViewList(long templateId)
        {
            return base.Channel.GetTemplateDetailsViewList(templateId);
        }

        #region  检查项
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddCheckItem(Epm_CheckItem model)
        {
            return base.Channel.AddCheckItem(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateCheckItem(Epm_CheckItem model)
        {
            return base.Channel.UpdateCheckItem(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteCheckItemByIds(List<long> ids)
        {
            return base.Channel.DeleteCheckItemByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_CheckItem>> GetCheckItemList(QueryCondition qc)
        {
            return base.Channel.GetCheckItemList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_CheckItem> GetCheckItemModel(long id)
        {
            return base.Channel.GetCheckItemModel(id);
        }
        #endregion

        #region 批复构成
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddConstitute(Epm_Constitute model)
        {
            return base.Channel.AddConstitute(model);
        }

        /// <summary>
        /// 修改批复构成
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public Result<int> UpdateConstitute(Epm_Constitute model)
        {
            return base.Channel.UpdateConstitute(model);
        }

        /// <summary>
        /// 删除批复构成表
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteConstituteByIds(List<long> ids)
        {
            return base.Channel.DeleteConstituteByIds(ids);
        }

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_Constitute>> GetConstituteList(QueryCondition qc)
        {
            return base.Channel.GetConstituteList(qc);
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_Constitute> GetConstituteModel(long id)
        {
            return base.Channel.GetConstituteModel(id);
        }

        #endregion

        #region 工程内容
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddWorkMainPoints(Epm_WorkMainPoints model)
        {
            return base.Channel.AddWorkMainPoints(model);
        }

        /// <summary>
        /// 修改批复构成
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public Result<int> UpdateWorkMainPoints(Epm_WorkMainPoints model)
        {
            return base.Channel.UpdateWorkMainPoints(model);
        }

        /// <summary>
        /// 删除批复构成表
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteWorkMainPointsByIds(List<long> ids)
        {
            return base.Channel.DeleteWorkMainPointsByIds(ids);
        }

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_WorkMainPoints>> GetWorkMainPointsList(QueryCondition qc)
        {
            return base.Channel.GetWorkMainPointsList(qc);
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_WorkMainPoints> GetWorkMainPointsModel(long id)
        {
            return base.Channel.GetWorkMainPointsModel(id);
        }
        #endregion

        /// <summary>
        /// 项目性质列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_ProjectNature>> GetProjectNatureList(QueryCondition qc)
        {
            return base.Channel.GetProjectNatureList(qc);
        }

        #region 总批复构成与服务商关联

        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddConstituteCompany(ConstituteCompanyView view)
        {
            return base.Channel.AddConstituteCompany(view);
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateConstituteCompany(ConstituteCompanyView view)
        {
            return base.Channel.UpdateConstituteCompany(view);
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteConstituteCompanyByIds(List<long> ids)
        {
            return base.Channel.DeleteConstituteCompanyByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_ConstituteCompany>> GetConstituteCompanyList(QueryCondition qc)
        {
            return base.Channel.GetConstituteCompanyList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<ConstituteCompanyView> GetConstituteCompanyModel(long id)
        {
            return base.Channel.GetConstituteCompanyModel(id);
        }

        /// <summary>
        /// 获取服务商（根据总批复构成获取关联的服务商）
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_ConstituteCompanyDetails>> GetConstituteCompanyDetailsList(QueryCondition qc)
        {
            return base.Channel.GetConstituteCompanyDetailsList(qc);
        }
        #endregion

        public Result<int> AddSendDate(Bp_SendDate model)
        {
            return base.Channel.AddSendDate(model);
        }

        #region 考勤设置
        ///<summary>
        ///添加:项目考勤设置表
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddProjectAttendance(ProjectAttendanceView model)
        {
            return base.Channel.AddProjectAttendance(model);
        }
        ///<summary>
        ///修改:项目考勤设置表
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateProjectAttendance(Epm_ProjectAttendance model)
        {
            return base.Channel.UpdateProjectAttendance(model);
        }
        ///<summary>
        ///删除:项目考勤设置表
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteProjectAttendanceByIds(List<long> ids)
        {
            return base.Channel.DeleteProjectAttendanceByIds(ids);
        }
        ///<summary>
        ///获取列表:项目考勤设置表
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_ProjectAttendance>> GetProjectAttendanceList(QueryCondition qc)
        {
            return base.Channel.GetProjectAttendanceList(qc);
        }
        ///<summary>
        ///获取详情:项目考勤设置表
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_ProjectAttendance> GetProjectAttendanceModel(long id)
        {
            return base.Channel.GetProjectAttendanceModel(id);
        }

        /// <summary>
        /// 获取考勤设置信息
        /// </summary>
        /// <returns></returns>
        public Result<AttendanceView> GetAttendanceModel()
        {
            return base.Channel.GetAttendanceModel();
        }
        #endregion
    }
}

