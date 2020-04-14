using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.DataModel.Msg;
using hc.epm.ViewModel;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.PeerResolvers;
using System.Text;
using System.Threading.Tasks;
using hc.epm.DataModel.BaseCore;
using hc.epm.ViewModel.AppView;
using System.Data;
using hc.Plat.WebAPI.Base.ViewModel;

namespace hc.epm.Service.ClientSite
{
    [ServiceContract]
    [ServiceKnownType(typeof(Epm_Approver))]
    [ServiceKnownType(typeof(Epm_Bim))]
    [ServiceKnownType(typeof(Epm_Change))]
    [ServiceKnownType(typeof(Epm_CompletionAcceptance))]
    [ServiceKnownType(typeof(Epm_Contract))]
    [ServiceKnownType(typeof(Epm_DangerousWork))]
    [ServiceKnownType(typeof(Epm_DataConfig))]
    [ServiceKnownType(typeof(Epm_Draw))]
    [ServiceKnownType(typeof(Epm_Material))]
    [ServiceKnownType(typeof(Epm_Materiel))]
    [ServiceKnownType(typeof(Epm_Milepost))]
    [ServiceKnownType(typeof(Epm_Monitor))]
    [ServiceKnownType(typeof(Epm_News))]
    [ServiceKnownType(typeof(Epm_Plan))]
    [ServiceKnownType(typeof(Epm_Project))]
    [ServiceKnownType(typeof(Epm_Question))]
    [ServiceKnownType(typeof(Epm_SpecialAcceptance))]
    [ServiceKnownType(typeof(Epm_SupervisorLog))]
    [ServiceKnownType(typeof(Epm_Train))]
    [ServiceKnownType(typeof(Epm_Visa))]
    [ServiceKnownType(typeof(Base_Files))]
    [ServiceKnownType(typeof(Epm_MonitorRectifRecord))]
    [ServiceKnownType(typeof(Epm_AppVersion))]
    [ServiceKnownType(typeof(Epm_WorkUploadRealScene))]
    [ServiceKnownType(typeof(EPM_CustomProperty))]
    [ServiceKnownType(typeof(Epm_TzProjectPolit))]
    [ServiceKnownType(typeof(Epm_TzAttachs))]
    [ServiceKnownType(typeof(Epm_TzCodeMap))]
    [ServiceKnownType(typeof(EPM_CustomProperty))]
    [ServiceKnownType(typeof(Epm_TzFormTalkFile))]
    [ServiceKnownType(typeof(Epm_TzProjectProposal))]
    [ServiceKnownType(typeof(Epm_TzInitialTalk))]
    [ServiceKnownType(typeof(Epm_TzLandTalk))]
    [ServiceKnownType(typeof(Epm_TzProjectApproval))]
    [ServiceKnownType(typeof(Epm_TzSecondTakl))]
    [ServiceKnownType(typeof(Epm_TzSecondTalkAudit))]
    [ServiceKnownType(typeof(Epm_TzSiteSurvey))]
    [ServiceKnownType(typeof(Epm_TzTalkFileAudit))]
    [ServiceKnownType(typeof(Epm_TzTalkFileHeadAudit))]
    [ServiceKnownType(typeof(Epm_TzTalkLeaderSign))]
    [ServiceKnownType(typeof(Epm_TzTalkRecord))]
    [ServiceKnownType(typeof(Epm_TzTalkRecordConfirm))]
    [ServiceKnownType(typeof(Epm_TzTalkSign))]
    [ServiceKnownType(typeof(Epm_TzDesiginChangeApply))]
    [ServiceKnownType(typeof(Epm_TzStartsApply))]

    public interface IClientSiteService
    {
        #region 待办事项
        ///<summary>
        ///添加待办事项
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddApprover(Epm_Approver model);
        ///<summary>
        ///修改待办事项
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateApprover(Epm_Approver model);
        ///<summary>
        ///删除待办事项
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteApproverByIds(List<long> ids);
        ///<summary>
        ///获取待办事项列表
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_Approver>> GetApproverList(QueryCondition qc);
        ///<summary>
        ///获取待办事项详情
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_Approver> GetApproverModel(long id);

        [OperationContract]
        Result<Epm_Approver> GetApproverModelByBusinId(long bussinesId, long approverId = 0);

        /// <summary>
        /// 处理待办事项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<bool> ComplateApprover(long id);

        /// <summary>
        /// 获取当前登录用户待办事项
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_Approver>> GetCurrUserApproverList(QueryCondition qc);

        #endregion

        #region 模型属性
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddCustomProperty(EPM_CustomProperty model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateCustomProperty(EPM_CustomProperty model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteCustomPropertyByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<EPM_CustomProperty>> GetCustomPropertyList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<EPM_CustomProperty> GetCustomPropertyModel(long id);
        #endregion

        #region 模型管理
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddBim(Epm_Bim model, List<Base_Files> fileListFile);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateBim(Epm_Bim model, List<Base_Files> fileListFile);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteBimByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_Bim>> GetBimList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_Bim> GetBimModel(long id);

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> ChangeBimState(long id, string state);

        /// <summary>
        /// 审核/驳回
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> RejectBim(long id, string state, string reason);


        /// <summary>
        /// 生成BIM模型图片
        /// </summary>
        /// <param name="id"></param>
        /// <param name="img"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> CreateImgBim(long id, string img, List<Base_Files> fileList);

        /// <summary>
        /// 根据ProjectId获取BIM模型列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_Bim>> GetBimModelListByProjectId(long projectId);

        /// <summary>
        /// 获取首页展示模型列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_Bim>> GetBimModelIndexList();

        /// <summary>
        /// 获取模型属性
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [OperationContract]
        Result<DataSet> GetBimProperty(string path, string SQLString);

        #endregion

        #region 变更管理
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddChange(ChangeView model, List<Base_Files> fileList);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateChange(ChangeView model, List<Base_Files> fileList);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteChangeByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<ChangeView>> GetChangeList(string projectName, string name, int state, int pageIndex, int pageSize);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<ChangeView> GetChangeModel(long id);
        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateChangeState(long id, string state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_Change>> GetChangListByQc(QueryCondition qc);
        #endregion

        #region 竣工验收
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddCompletionAcceptance(Epm_CompletionAcceptance model, List<Base_Files> fileListFile);

        /// <summary>
        /// 新增完工验收
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddCompletionAcceptanceNew(CompletionAcceptanceView view);

        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateCompletionAcceptance(Epm_CompletionAcceptance model, List<Base_Files> fileListFile);

        /// <summary>
        /// 修改完工验收
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateCompletionAcceptanceNew(CompletionAcceptanceView view);

        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteCompletionAcceptanceByIds(List<long> ids);

        [OperationContract]
        Result<int> UpdateCompletionAcceptanceState(List<long> ids, string state);

        /// <summary>
        /// 删除完工验收
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<bool> DeleteCompletionAcceptanceById(long id);

        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_CompletionAcceptance>> GetCompletionAcceptanceList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_CompletionAcceptance> GetCompletionAcceptanceModel(long id);

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<CompletionAcceptanceView> GetCompletionAcceptanceModelNew(long id);

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> ChangeCompletionAcceptanceState(long id, PreCompletionScceptanceState state, string reason);

        /// <summary>
        /// 根据项目 ID 获取验收项资料
        /// </summary>
        /// <param name="id">项目 ID</param>
        /// <returns></returns>
        [OperationContract]
        Result<List<CompletionAcceptanceItemView>> GetCompletionItem(long id);

        #endregion

        #region 合同管理
        [OperationContract]
        Result<int> DeleteContractModel(long id);
        [OperationContract]
        Result<int> DeleteFilesByTableIds(string tableName, List<long> tableIds);
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddContract(Epm_Contract model, List<Base_Files> fileList);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateContract(Epm_Contract model, List<Base_Files> fileList);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteContractByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_Contract>> GetContractList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_Contract> GetContractModel(long id);
        [OperationContract]
        Result<List<Base_Files>> GetContractModelFile(long id);
        [OperationContract]
        Result<List<Base_Files>> GetContractModelFileName(long contractId, string fileNames);
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateContractState(long id, string state);

        #endregion

        #region 危险作业
        /// <summary>
        /// 根据监理日志获取危险作业信息
        /// </summary>
        /// <param name="logId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_DangerousWork>> GetDangerousWorkByLogId(long logId);

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddDangerousWork(Epm_DangerousWork model, List<Base_Files> files);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateDangerousWork(Epm_DangerousWork model, List<Base_Files> files);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteDangerousWorkByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_DangerousWork>> GetDangerousWorkList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_DangerousWork> GetDangerousWorkModel(long id);

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateDangerousWorkState(long id, ApprovalState state);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteDangerousWork(long id);
        #endregion

        #region 项目资料
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddDataConfig(Epm_DataConfig model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateDataConfig(Epm_DataConfig model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteDataConfigByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_DataConfig>> GetDataConfigList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_DataConfig> GetDataConfigModel(long id);

        #endregion

        #region 图纸管理
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddDraw(Epm_Draw model, List<Base_Files> fileListFile);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateDraw(Epm_Draw model, List<Base_Files> fileListFile);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteDrawByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_Draw>> GetDrawList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_Draw> GetDrawModel(long id);

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> ChangeDrawState(long id, string state);

        /// <summary>
        /// 审核/驳回
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> RejectDraw(long id, string state, string reason);



        #endregion

        #region 材料设备验收
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddMaterial(MaterialView model);

        /// <summary>
        /// 工器具验收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddMaterialNew(MaterialViewNew model);

        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateMaterial(MaterialView model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteMaterialByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_Material>> GetMaterialList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<MaterialView> GetMaterialModel(long id);

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateMaterialState(long id, ConfirmState state);

        #endregion

        #region 物料接收
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddMateriel(MaterielView model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateMateriel(MaterielView model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteMaterielByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_Materiel>> GetMaterielList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<MaterielView> GetMaterielModel(long id);

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> ChangeMaterielState(long id, ConfirmState state);

        /// <summary>
        /// 批量修改状态
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> ChangeMaterielALLState(List<long> ids, string state);
        #endregion

        #region 里程碑
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddMilepost(Epm_Milepost model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateMilepost(Epm_Milepost model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteMilepostByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_Milepost>> GetMilepostList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_Milepost> GetMilepostModel(long id);

        /// <summary>
        /// 获取里程碑树形列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<MilepostView>> GetMilepostViewList();

        /// <summary>
        /// 根绝项目资料ID获取里程碑信息
        /// </summary>
        /// <param name="dataConfigId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Epm_MilepostData> GetMDataByDataId(long dataConfigId);
        #endregion

        #region 安全质量检查

        /// <summary>
        /// 新增检查
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [OperationContract]
        Result<bool> AddMonitorNew(long projectId, DateTime time, List<CheckView> dataList, int type = 0);
        [OperationContract]
        Result<bool> AddRectificationNew(long projectId, DateTime time, List<checkItemesPer> dataList, int type = 0);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_InspectItem>> GetInspectItemList(long inspectId);

        [OperationContract]
        Result<List<Epm_InspectItem>> GetInspectItemByProjectId(long projectId);

        [OperationContract]
        Result<List<Epm_InspectItem>> GetInspectItemDraft(long projectId);

        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_Inspect> GetInspectModel(long id);

        /// <summary>
        /// 获取检查单非常规作业
        /// </summary>
        /// <param name="inspectId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<EPM_UnconventionalWork>> GetIUnconventionalWorkList(long inspectId);

        /// <summary>
        /// 获取非常规作业和复查、复核列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<EPM_UnconventionalWork>> GetWorkList(QueryCondition qc);

        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<Epm_Rectification> GetRectificationModel(long id);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<List<Epm_RectificationItem>> GetRectificationItemList(long rectifId);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<List<Epm_RectificationRecord>> GetRectificationRecordList(long rectifId);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<List<Epm_RectificationItem>> GetRectificationItemListByProjectId(long projectId);

        [OperationContract]
        Result<List<Epm_RectificationItem>> GetRectificationItemListALLByProjectId(long projectId);
        ///<summary>
        ///添加:检查整改记录表
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddMonitorRectifRecord(long id, string content, List<Base_Files> files);

        /// <summary>
        /// 修改检查整改记录状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> ChangeMonitorState(long rectifItemId, RectificationState state, string Remark);

        #endregion

        #region 新闻资讯
        ///<summary>
        ///添加:新闻、资讯表
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddNews(Epm_News model);
        ///<summary>
        ///修改:新闻、资讯表
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateNews(Epm_News model);
        ///<summary>
        ///删除:新闻、资讯表
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteNewsByIds(List<long> ids);
        ///<summary>
        ///获取列表:新闻、资讯表
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_News>> GetNewsList(QueryCondition qc);
        ///<summary>
        ///获取详情:新闻、资讯表
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_News> GetNewsModel(long id);

        #endregion

        #region 计划管理
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddPlan(Epm_Plan model, List<Epm_PlanComponent> planComponentIds);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdatePlan(Epm_Plan model, List<Epm_PlanComponent> planComponentIds);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeletePlanByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_Plan>> GetPlanList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<PlanView> GetPlanModel(long id);

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="batchNo"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> ChangePlanState(string batchNo, ApprovalState state, string reason);

        /// <summary>
        /// 根据计划id和模型ID获取关联组件列表
        /// </summary>
        /// <param name="PlanId"></param>
        /// <param name="bimId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_PlanComponent>> GetComponentListByPlanId(long PlanId, long bimId);

        /// <summary>
        /// 计划关联模型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddPlanComponent(Epm_PlanComponent model, string planComponentIds);

        /// <summary>
        /// 根据parentId获取计划信息
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Epm_Plan> GetPlanById(long parentId);

        /// <summary>
        /// 获取施工计划树形列表数据
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<PlanView>> GetPlanViewList(long ProjectId);

        /// <summary>
        /// 获取进度跟踪树形列表数据
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<PlanView>> GetScheduleViewList(long ProjectId);

        /// <summary>
        /// 获取里程碑计划
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<PlanView>> GetMilepostPlan(long projectId);

        /// <summary>
        /// 生成里程碑计划
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="planStart"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_Plan>> CreateMilepostPlan(long projectId, DateTime planStart, long mileType, int type = 1);
        /// <summary>
        /// 更新里程碑计划
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateMilepostPlan(List<Epm_Plan> list, int type = 1);
        /// <summary>
        /// 审核里程碑计划
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AuditMilepostPlan(List<Epm_Plan> list);
        /// <summary>
        /// 驳回里程碑计划
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> RejectMilepostPlan(long projectId);
        /// <summary>
        /// 关联构件
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="pcList"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> BindComponents(long planId, List<Epm_PlanComponent> pcList);

        /// <summary>
        /// 根据项目施工计划获取项目进度甘特图
        /// </summary>
        /// <param name="projectId">项目 ID</param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Gantt>> GetProjectGantt(long projectId);

        #endregion

        #region 项目管理

        /// <summary>
        /// 根据项目Id获取工程内容要点列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_ProjectWorkMainPoints>> GetProjectPointsList(long projectId);

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddProject(Epm_Project model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateProject(Epm_Project model, List<Base_Files> attachs);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteProjectByIds(List<long> ids);

        [OperationContract]
        Result<List<Epm_Project>> GetProjectList(QueryCondition qc);
        [OperationContract]
        Result<Epm_TzProjectApprovalInfo> GetProjectApprovalInfos(long projectId);
        /// <summary>
        /// 在建项目列表（多表查询）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<ProjectView>> GetProjectListInfo(int pageIndex, int pageSize, string state, string pmName, string name = "", string startTime = "", string endTime = "");

        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_Project> GetProjectModel(long id);

        [OperationContract]
        Result<Epm_Project> GetProjectModelByTzId(long id);

        /// <summary>
        /// 获取项目详情信息（项目资料、里程碑、第三方单位）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Epm_Project> GetProject(long id);

        /// <summary>
        /// 修改项目状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> ChangeProjectState(long id, string state);

        /// <summary>
        /// 查询所有在建项目,根据登录人所属公司查询所有参与项目信息
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_Project>> GetProjectListByQc(QueryCondition qc);
        /// <summary>
        /// 查询所有在建项目,根据登录人所属公司查询所有参与项目信息
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_Project>> GetProjectListById(long companyId, long userId);

        /// <summary>
        /// 检查验收结果
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> CheckAcceptance(long projectId);

        [OperationContract]
        Result<List<Epm_ProjectCompany>> GetProjectCompanyList(long projectId);

        /// <summary>
        /// 获取总批复及构成
        /// </summary>
        /// <param name="code">项目性质编号</param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_ProjectNature>> GetProjectNature();

        /// <summary>
        /// 获取项目总批复及构成
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_ProjectConstitute>> GetProjectConstituteByProjectId(long projectId);

        /// <summary>
        /// 修改项目总批复及构成
        /// </summary>
        /// <param name="list">总批复及构成集合</param>
        /// <param name="projectId">项目Id</param>
        /// <param name="bluePrintCode">方案类型Key</param>
        /// <param name="bluePrintName">方案类型Value</param>
        /// <param name="isCrossings">是否外部手续bool</param>
        [OperationContract]
        Result<int> UpdateProjectConstitute(Epm_Project project, List<Epm_ProjectConstitute> list, List<Base_Files> attachs);

        /// <summary>
        /// 获取项目总批复及构成历史
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_ProjectConstituteHistory>> GetProjectConstituteHistoryByProjectId(long projectId);

        /// <summary>
        /// 获取工程服务商
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_ProjectCompany>> GetProjectCompanyByProjectId(long projectId);

        /// <summary>
        /// 更新工程服务商（服务商、合同、委托书）
        /// </summary>
        /// <param name="list"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateProjectCompany(List<Epm_ProjectCompany> list);
        /// <summary>
        /// 获取服务商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Epm_ProjectCompany> GetProjectCompany(long id);
        /// <summary>
        /// 更新项目经理信息
        /// </summary>
        /// <param name="projectCompany"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdatePMAndPhone(Epm_ProjectCompany projectCompany);

        /// <summary>
        /// 审核项目经理信息
        /// </summary>
        /// <param name="projectCompany"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AuditPMAndPhone(Epm_ProjectCompany projectCompany);

        /// <summary>
        /// 驳回项目经理信息
        /// </summary>
        /// <param name="projectCompany"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> RejectPMManAndPhone(Epm_ProjectCompany projectCompany);

        ///// <summary>
        ///// 更新负责人信息
        ///// </summary>
        ///// <param name="projectCompany"></param>
        ///// <returns></returns>
        //[OperationContract]
        //Result<int> UpdateLinkManAndPhone(Epm_ProjectCompany projectCompany);

        ///// <summary>
        ///// 审核负责人信息
        ///// </summary>
        ///// <param name="projectCompany"></param>
        ///// <returns></returns>
        //[OperationContract]
        //Result<int> AuditLinkManAndPhone(Epm_ProjectCompany projectCompany);

        ///// <summary>
        ///// 驳回负责人信息
        ///// </summary>
        ///// <param name="projectCompany"></param>
        ///// <returns></returns>
        //[OperationContract]
        //Result<int> RejectLinkManAndPhone(Epm_ProjectCompany projectCompany);

        /// <summary>
        /// 获取工程内容要点
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_ProjectWorkMainPoints>> GetProjectPointsByProjectId(long projectId);
        /// <summary>
        /// 修改工程内容要点
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateProjectPoints(List<Epm_ProjectWorkMainPoints> list, long projectId);

        ///// <summary>
        ///// 新增工程内容要点
        ///// </summary>
        ///// <param name="list"></param>
        ///// <param name="projectId"></param>
        ///// <returns></returns>
        //[OperationContract]
        //Result<int> AddProjectPoints(List<Epm_ProjectWorkMainPoints> list, long projectId);

        /// <summary>
        /// 获取项目工程内容要点历史
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_ProjectWorkMainPointsHistory>> GetProjectPointsHistoryByProjectId(long projectId);

        /// <summary>
        /// 更新工期信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateTimelimit(Epm_Project model, List<Base_Files> attachs, bool isdelete = true);

        /// <summary>
        /// 获取项目资料
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_ProjectDataSubmit>> GetProjectSubmitByProjectId(long projectId);

        /// <summary>
        /// 更新项目资料
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="attachs"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateProjectSubmit(long projectId, List<Base_Files> attachs);

        /// <summary>
        /// 获取项目总批复构成历史
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [OperationContract]
        DataTable GetConstituteHis(long projectId);

        /// <summary>
        /// 获取项目工程内容要点历史
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [OperationContract]
        DataTable GetPointsHis(long projectId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [OperationContract]
        DataTable GetdtUserList(long companyId);
        [OperationContract]
        Result<List<Base_User>> GetUserByCompanyId(long companyId);

        /// <summary>
        /// 获取合同乙方单位
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_ProjectCompany>> GetProjectCompanyListByName(long projectId, string name);
        #endregion

        #region 问题管理
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddQuestion(QuestionView model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateQuestion(Epm_Question model);

        /// <summary>
        /// 删除问题
        /// </summary>
        /// <param name="id">问题 ID</param>
        /// <returns></returns>
        [OperationContract]
        Result<bool> DeleteQuestion(long id);

        ///<summary>
        ///获取列表: 
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_Question>> GetQuestionList(QueryCondition qc);
        [OperationContract]
        Result<List<Base_VideoManage>> GetBaseVideoManageLists(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<QuestionView> GetQuestionModel(long id);

        /// <summary>
        /// 关闭问题
        /// </summary>
        /// <param name="id">问题 ID</param>
        /// <param name="isAccident">是否重大事故</param>
        /// <returns></returns>
        [OperationContract]
        Result<bool> CloseQuestion(long id, bool isAccident = false);

        /// <summary>
        /// 回复问题
        /// </summary>
        /// <param name="model">回复内容</param>
        /// <returns></returns>
        [OperationContract]
        Result<bool> ReplyQuestion(Epm_QuestionTrack model);

        /// <summary>
        /// 获取当前登录人问题
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_Question>> GetCurrUserQuestion(QueryCondition qc);

        /// <summary>
        /// 根据问题id和模型ID获取关联组件列表
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_QuestionBIM>> GetComponentListByQuestionId(long questionId);

        /// <summary>
        /// 根据模型ID获取关联组件列表
        /// </summary>
        /// <param name="bimId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_QuestionBIM>> GetComponentListByBimId(long bimId);

        /// <summary>
        /// 添加问题关联模型
        /// </summary>
        /// <param name="model"></param>
        /// <param name="componentIds"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddQuestionBIM(Epm_QuestionBIM model, string componentIds);

        /// <summary>
        /// 获取问题回复列表
        /// </summary>
        /// <param name="qc">查询条件</param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_QuestionTrack>> GetQuestionTrack(QueryCondition qc);
        #endregion

        #region 专项验收
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddSpecialAcceptance(SpecialAcceptanceView model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateSpecialAcceptance(SpecialAcceptanceView model);

        /// <summary>
        /// 删除专项验收
        /// </summary>
        /// <param name="id">要删除的专项验收 ID</param>
        /// <returns></returns>
        [OperationContract]
        Result<bool> DeleteSpecialAcceptanceById(long id);

        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_SpecialAcceptance>> GetSpecialAcceptanceList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<SpecialAcceptanceView> GetSpecialAcceptanceModel(long id);

        /// <summary>
        /// 专项验收审核
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<bool> AuditSpecialAccptance(SpecialAcceptanceView model);

        /// <summary>
        /// 废弃专项验收
        /// </summary>
        ///<param name="id">要废弃的专项验收 ID</param>
        /// <returns></returns>
        [OperationContract]
        Result<bool> DiscardSpecialAccptance(long id);


        #endregion

        #region 监理日志
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <param name="details">监理日志明细</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddSupervisorLog(Epm_SupervisorLog model, List<SupervisorLogDetailView> details);

        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteSupervisorLogByIds(List<long> ids);
        ///<summary>
        ///获取监理日志列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<SupervisorLogDetailView>> GetSupervisorLogList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<SupervisorLogDetailView> GetSupervisorLogModel(long id);

        ///<summary>
        ///获取监理日志详情列表
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_SupervisorLogDetails>> GetSupervisorLogDetailsList(QueryCondition qc);

        /// <summary>
        /// 新增监理日志 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddSupervisorLogNew(SupervisorLogView model, List<long> workIdList);
        /// <summary>
        /// 新增监理日志（新）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddProjectlLogList(SupervisorLogView model, List<long> workIdList);

        /// <summary>
        /// 删除监理日志
        /// </summary>
        /// <param name="id">监理日志 ID</param>
        /// <returns></returns>
        [OperationContract]
        Result<bool> DeleteSupervisorlogByIdNew(long id);

        /// <summary>
        /// 获取监理日志列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_SupervisorLog>> GetSupervisorLogListNew(QueryCondition qc);

        /// <summary>
        /// 获取监理日志详情
        /// </summary>
        /// <param name="id">监理日志 ID</param>
        /// <param name="isLoadFile">是否同时获取资源，默认获取</param>
        /// <returns></returns>
        [OperationContract]
        Result<SupervisorLogView> GetSupervisorLogModelNew(long id, bool isLoadFile = true);

        /// <summary>
        /// 审核监理日志
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AuditSupervisorLog(Epm_SupervisorLog model);


        #endregion

        #region 培训管理
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTrain(TrainView model, List<Base_Files> fileList);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTrain(TrainView model, List<Base_Files> fileList);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTrainByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<TrainView>> GetTrainList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<TrainView> GetTrainModel(long id);
        ///<summary>
        ///更新状态
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<int> UpdateTrainState(long id, string state);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_Template>> GetTemplateList(QueryCondition qc);

        #endregion

        #region 签证管理
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddVisa(VisaView model, List<Base_Files> fileList);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateVisa(VisaView model, List<Base_Files> fileList);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteVisaByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_Visa>> GetVisaList(string projectName, string title, int state, string visaTypeName, int pageIndex, int pageSize);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<VisaView> GetVisaModel(long id);
        /// <summary>
        /// 更改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateVisaState(long id, string state);
        #endregion

        #region 用户登录

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">加密之后的密码</param>
        /// <returns></returns>
        [OperationContract]
        Result<UserView> Login(string userName, string password, string type);

        /// <summary>
        /// 根据用户加载对应权限，有缓存
        /// </summary>
        /// <param name="roleType">用户角色</param>
        /// <param name="userId">用户ID</param>
        /// <param name="listRight">权限ID</param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Right>> LoadRightList(string roleType, long userId = 0, List<long> listRight = null);

        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Base_User> GetUserModel(long id);

        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="token"></param>
        /// <param name="type">1:Android,2:IOS</param>
        /// <returns></returns>
        [OperationContract]
        Result<UserView> GetUserModelByToken(string token, int type);

        [OperationContract]
        Result<Base_User> GetBaseUserByToken(string token, int type);

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateUser(Base_User model);

        /// <summary>
        /// 获取网站设置,有缓存
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<Base_Config> LoadConfig();

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userName"></param>
        /// <param name="phone"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<UserView>> GetUserListByWhr(string type, string userName, string phone, int pageIndex, int pageSize);


        [OperationContract]
        Result<List<UserListView>> GetUserManageList(QueryCondition qc, string type);
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddUserInfo(Base_User model, string image, List<Base_Files> fileList = null);

        /// <summary>
        /// 批量添加用户信息
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddRangeUser(List<Base_User> models);

        /// <summary>
        /// 根据用户名称或者电话号码查询用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Base_User> GetUserInfoByNameAndPhone(string userName, string phone);

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateUserInfo(Base_User model, string image, List<Base_Files> fileList = null);

        /// <summary>
        /// 批量删除用户
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteUserByIds(List<long> ids);

        /// <summary>
        /// 查询用户详情
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Base_User> GetUserDetail(long userId);

        /// <summary>
        /// 是否工程处用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract]
        bool IsAgencyUser(long userId);
        /// <summary>
        /// 获取分公司项目负责人
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_User>> GetBrCompanyPMList(string name, long companyId, int pageIndex, int pageSize);
        /// <summary>
        /// 获取工程处项目经理
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_User>> GetAgencyPMList(string name, int pageIndex, int pageSize);

        /// <summary>
        /// 是否服务商用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract]
        bool IsServiceUser(long userId);


        /// <summary>
        /// 是否分公司用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract]
        bool IsBranchCompanyUser(long userId);

        /// <summary>
        /// 是否分公司部门经理
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract]
        bool IsBranchCompanyDirector(long userId);

        /// <summary>
        /// 是否监理
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract]
        bool IsSupervisor(long projectId, long userId);
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="oldPwd">加密之后的原旧密码</param>
        /// <param name="pwd">加密之后的密码</param>
        /// <returns></returns>
        [OperationContract]
        Result<bool> UpdatePassword(long userId, string oldPwd, string pwd);

        /// <summary>
        /// 获取消息列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Msg_Message>> GetMessageList(QueryCondition qc);

        #endregion

        #region 企业管理

        /// <summary>
        /// 获取企业列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Company>> GetCompanyList(QueryCondition qc);


        #region 三商管理改造
        /// <summary>
        /// 根据类型获取企业列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Company>> GetCompanyListByType(QueryCondition qc);

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddCompany(Base_Company model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateCompany(Base_Company model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteCompanyByIds(List<long> ids);

        #endregion


        /// <summary>
        /// 获取地市公司
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Company>> GetAreaCompanyList();
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_User>> GetUserList(QueryCondition qc);


        /// <summary>
        /// 根据企业Id获取用户列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_User>> GetUserPostList(long id);


        /// <summary>
        /// 根据单位ID和岗位名称获取人员信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="postName"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_User>> GetUserListByPost(long id, string postName);

        /// <summary>
        /// 获取用户信息（包含部门信息）
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<UserView>> GetUserListByDepartment(QueryCondition qc);

        /// <summary>
        /// 根据企业 ID 获取企业详情
        /// </summary>
        /// <param name="id">企业 ID</param>
        /// <returns></returns>
        [OperationContract]
        Result<CompanyView> GetCompanyModel(long id);

        #endregion

        #region  区域
        /// <summary>
        /// 获取区域列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Region>> GetRegionList(QueryCondition qc);
        /// <summary>
        /// 加载区域列表，有缓存
        /// </summary>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Region>> LoadRegionList(string parentCode = "");

        /// <summary>
        /// 获取区域详情
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Base_Region> GetRegionModel(string code);

        /// <summary>
        /// 根据字典类型集合获取字典数据
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Dictionary<DictionaryType, List<Base_TypeDictionary>>> GetTypeListByTypes(List<DictionaryType> types);
        #endregion

        #region 加油站信息
        ///<summary>
        ///获取列表:加油站信息表
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_OilStation>> GetOilStationList(QueryCondition qc);

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_OilStation>> GetOilStationAllList();
        #endregion

        #region Files
        /// <summary>
        /// 根据表名和id获取所有附件
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Files>> GetFilesByTable(string tableName, long id);

        /// <summary>
        /// 根据表名和id获取所有附件
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Files>> GetFileListByTableIds(string tableName, List<long> ids);
        /// <summary>
        /// 根据Epm_TzAttachs表id获取附件
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Epm_TzAttachs> GetFileByTzAttachsId(long id);
        #endregion

        #region 面包屑导航

        /// <summary>
        /// 获取面包屑导航
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Right>> GetBreadcrumbNavigation(string controllerName, string actionName);

        #endregion

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="templateId">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TemplateDetails>> GetTemplateDetailsList(long templateId);

        /// <summary>
        /// 根据模板编码获取模板列表
        /// </summary>
        /// <param name="templateNo"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_Template>> GetTemplateListByNo(string templateNo);

        /// <summary>
        /// 获取字典Id
        /// </summary>
        /// <param name="dicNo"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Base_TypeDictionary> GetDictionaryId(string dicNo);

        /// <summary>
        /// 根据字典id获取模板列表
        /// </summary>
        /// <param name="dicId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_Template>> GetTemplateListDicId(long dicId, string title);


        /// <summary>
        /// 根据父级ID获取检查项
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_CheckItem>> GetCheckItemList(long pid);

        [OperationContract]
        Result<List<Epm_CheckItem>> GetCheckItemAll();

        [OperationContract]
        Result<List<Epm_CheckItem>> GetCheckItemListByTypeName(string type, string name, int level);
        /// <summary>
        /// 获取检查项树形列表数据
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<CheckItemView>> GetCheckItem(long projectid, long userid);

        /// <summary>
        /// 根据检查角色类型获取检查数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_CheckItem>> GetCheckItemListByType(string type);

        /// <summary>
        /// 根据广告位编码获取广告投放记录
        /// </summary>
        /// <param name="targetNum"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Files>> GetAdPutRecord(string targetNum, string imageType);

        /// <summary>
        /// 获取服务商（根据总批复构成获取关联的服务商）
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_ConstituteCompanyDetails>> GetConstituteCompanyDetailsList(QueryCondition qc);

        #region 延期申请

        /// <summary>
        /// 获取延期申请列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_PlanDelay>> GetPlanDelayList(QueryCondition qc);

        /// <summary>
        /// 新增延期申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<bool> AddPlanDelay(PlanDelayView model);

        /// <summary>
        /// 修改延期申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<bool> ModifyPlanDelay(PlanDelayView model);

        /// <summary>
        /// 审核延期申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AuditPlanDelay(Epm_PlanDelay model);

        /// <summary>
        /// 删除延期申请
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<bool> DeletePlanDelay(long id);

        /// <summary>
        /// 获取延期申请详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<PlanDelayView> GetPlanDelayInfo(long id);

        #endregion

        #region 作业实景

        /// <summary>
        /// 获取作业实景列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_WorkUploadRealScene>> GetWorkRealSceneList(QueryCondition qc);

        /// <summary>
        /// 上传作业实景
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<bool> AddWorkRealScenen(Epm_WorkUploadRealScene model, List<Base_Files> files);

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateWorkRealScenenState(long id, ApprovalState state);

        /// <summary>
        /// 删除上传作业实景
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> DeleteWorkRealScenen(long id);

        /// <summary>
        /// 根据监理日志 ID 获取危险作业实景
        /// </summary>
        /// <param name="logId">监理日志 ID</param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_WorkUploadRealScene>> GetWorkRealSceneByLogId(long logId);

        #endregion

        #region 消息信息
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddMassage(Epm_Massage model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateMassage(Epm_Massage model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteMassageByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_Massage>> GetMassageList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_Massage> GetMassageModel(long id);
        /// <summary>
        /// 更新所有消息状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateAllMassageState(long recId, bool state);

        #endregion

        #region  APP 专用接口

        /// <summary>
        /// 获取沟通列表集合
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<QuestionModel>> GetQuestions(QueryCondition qc);

        /// <summary>
        /// 获取登录用户待办事项总数和未读消息总数
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<Dictionary<string, int>> GetApproverCount(long UserId);

        /// <summary>
        /// 获取可展示广告列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_AdPutRecord>> GetAdShowList(QueryCondition qc);

        /// <summary>
        /// 获取指定数据的附件
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Files>> GetBaseFiles(List<long> tableId);
        [OperationContract]
        Result<Base_Files> GetBaseFile(long id);

        /// <summary>
        /// 根据guid查询图片附件
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Files>> GetBaseFileByGuid(string guid);

        /// <summary>
        /// 添加附件
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList"></param>
        /// <param name="isdelete"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Base_Files>> AddFilesByTables(string model, List<Base_Files> fileList, bool isdelete);
        /// <summary>
        /// 获取用户头像链接
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Dictionary<long, string>> GetUserProfilePhoto(List<long> userId);

        /// <summary>
        /// 获取热门问题
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<QuestionModel> GetHotQuestion();

        /// <summary>
        /// 获取问题回复数
        /// </summary>
        /// <param name="questionIds">问题 ID 集合</param>
        /// <returns></returns>
        [OperationContract]
        Result<Dictionary<long, int>> GetQuestionTrackCount(List<long> questionIds);


        /// <summary>
        /// 获取业务相关问题数
        /// </summary>
        /// <param name="businessIds">业务 ID</param>
        /// <returns></returns>
        [OperationContract]
        Result<Dictionary<long, int>> GetQuestionCount(List<long> businessIds);


        /// <summary>
        /// 根据项目 ID 获取项目相关变更 
        /// </summary>
        /// <param name="projectId">项目 ID</param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_Change>> GetChangeByProjectId(long projectId);

        /// <summary>
        /// 获取签证列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_Visa>> GetVisaListByQc(QueryCondition qc);

        /// <summary>
        /// 获取 APP 最新版本
        /// </summary>
        /// <param name="appNum">APP 包名</param>
        /// <returns></returns>
        [OperationContract]
        Result<Epm_AppVersion> GetAppVersion(string appNum);


        ///// <summary>
        ///// 新增工程内容要点
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[OperationContract]
        //Result<bool> AddProjectWorkPoint(Epm_ProjectWorkMainPoints model);

        ///// <summary>
        ///// 根据 ID 修改工程内容要点
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[OperationContract]
        //Result<bool> UpdateProjectWorkPointById(Epm_ProjectWorkMainPoints model);

        /// <summary>
        /// 修改项目供应商负责人及项目经理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<bool> UpdateProjectCompanyPmInfo(Epm_ProjectCompany model, long userId);

        /// <summary>
        /// 审核、驳回服务商PM和负责人
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<bool> AuditProjectCompanyPmAndLink(long Id, ApprovalState state);

        #endregion

        [OperationContract]
        Result<List<Base_Files>> GetFilesByTableName(string tableName, string name);

        /// <summary>
        /// 获取当前年份项目KPI数据
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<Epm_ProjectKPI> GetProjectKPIList();

        /// <summary>
        /// 获取项目KPI数据
        /// </summary>
        /// <param name="years"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<ProjectKPIView> GetProjectKPIListByWhr(string years, long userId);

        [OperationContract]
        Result<int> AddSendDate(Bp_SendDate model);

        /// <summary>
        /// 检查列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<InspectView>> GetInspectList(QueryCondition qc);

        /// <summary>
        /// 得失分明细列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<InspectView>> GetInspectItemListByQc(QueryCondition qc);

        /// <summary>
        /// 人脸注册
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="image"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddAIUserFaceInfo(long userId, string image, string source);

        /// <summary>
        /// 人脸搜索
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> SearchUserFace(SignFaceAI model);

        /// <summary>
        /// 根据用户ID获取该用户注册的人脸信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<EPM_AIUserFace> GetAIUserFace(long userId);

        #region 考勤信息
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_SignInformation>> GetSignInformationList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_SignInformation> GetSignInformationModel(long id);
        #endregion

        #region 项目试运行申请
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddProjectApproval(ProjectApprovalView model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateProjectApproval(Epm_TzProjectPolit model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteProjectApprovalByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzProjectPolit>> GetProjectApprovalList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<ProjectApprovalView> GetProjectApprovalModel(long id);

        [OperationContract]
        Result<int> UpdateTzProjectPolit(Epm_TzProjectPolit model);

        [OperationContract]
        Result<int> UpdateTzProjectPolitState(List<long> ids, string state);


        /// <summary>
        /// 新增审核记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<bool> AddProjectAuditRecord(Epm_ProjectAuditRecord model);
        #endregion

        /// <summary>
        /// 根据 Cas 登录后的用户账号获取用户信息
        /// </summary>
        /// <param name="userName">用户账户</param>
        /// <returns></returns>
        [OperationContract]
        Result<UserView> LoginByCas(string userName);

        #region 防渗改造投资

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddReformRecord(ReformRecordView model);

        [OperationContract]
        Result<int> AddReformRecordeEtity(Epm_ReformRecord model);

        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateReformRecord(Epm_ReformRecord model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteReformRecordByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_ReformRecord>> GetReformRecordList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<ReformRecordView> GetReformRecordModel(long id);

        [OperationContract]
        Result<Epm_ReformRecord> GetReformRecordEntity(long id);
        /// <summary>
        /// 修改项目状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateReformRecordState(List<long> ids, string state);

        #endregion

        #region 编码映射

        /// <summary>
        /// 获取编码映射结果
        /// </summary>
        /// <param name="codeType">编码映射类型</param>
        /// <param name="mapType">映射系统</param>
        /// <param name="fromCode">源编码</param>
        /// <returns></returns>
        Result<Epm_TzCodeMap> GetCodeMap(string codeType, string mapType, string fromCode);

        #endregion

        #region 项目投资前

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzProjectProposal(Epm_TzProjectProposal model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzProjectProposal(Epm_TzProjectProposal model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzProjectProposalByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzProjectProposal>> GetTzProjectProposalList(QueryCondition qc);

        /// <summary>
        /// 获取项目进度信息列表
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<TzProjectScheduleView>> GetTzProjectScheduleList(QueryCondition qc);

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzProjectProposal>> GetSingleTzProjectProposalList(QueryCondition qc);

        /// <summary>
        /// 获取项目批复通过并且批复号不为空的项目
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_TzProjectProposal>> GetProjectProposalList(QueryCondition qc);

        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzProjectProposal> GetTzProjectProposalModel(long id);

        /// <summary>
        /// 获取项目详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<TzProjectProposalInfoView> GetTzProjectProposalALL(long id);

        /// <summary>
        /// 修改项目状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateTzProjectProposalState(List<long> ids, string state);

        /// <summary>
        /// 关闭项目
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> CloseTzProjectProposal(long projectId);
        #endregion

        #region 现场调研、初次谈判

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzInitialTalk(Epm_TzInitialTalk model);

        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzInitialTalk(Epm_TzInitialTalk model);

        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzInitialTalkByIds(List<long> ids);

        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzInitialTalk>> GetTzInitialTalkList(QueryCondition qc);

        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzInitialTalk> GetTzInitialTalkModel(long id);

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzSiteSurvey(TzResearchView model);

        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzSiteSurvey(Epm_TzSiteSurvey model);

        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzSiteSurveyByIds(List<long> ids);

        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzSiteSurvey>> GetTzSiteSurveyList(QueryCondition qc);

        /// <summary>
        /// 现场调研列表查询（连项目前期信息表）
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<TzProjectProposalView>> GetTzSiteSurveyAndProjectList(QueryCondition qc);

        /// <summary>
        /// 初次谈判列表查询（连项目前期信息表）
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<TzProjectProposalView>> GetTzInitialTalkAndProjectList(QueryCondition qc);

        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzSiteSurvey> GetTzSiteSurveyModel(long id);

        /// <summary>
        /// 根据项目Id获取现场勘探和项目信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [OperationContract]
        Result<TzResearchAllView> GetTzSiteSurveyProject(long projectId);

        /// <summary>
        /// 获取现场勘查列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_TzProjectProposal>> GetTzResearchList(QueryCondition qc);

        #endregion

        #region 土地协议出让谈判信息
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzLandTalk(Epm_TzLandTalk model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzLandTalk(Epm_TzLandTalk model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzLandTalkByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzLandTalk>> GetTzLandTalkList(QueryCondition qc);

        /// <summary>
        /// 土地谈判协议列表查询（连项目前期信息表）
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<TzProjectProposalView>> GetTzLandTalkAndProjectList(QueryCondition qc);

        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzLandTalk> GetTzLandTalkModel(long id);

        #endregion

        #region 组织评审材料、省投资处业务管理员审核、评审材料审核、评审记录、投资部门确认、评审会签记录、省公司领导签发

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzFormTalkFile(Epm_TzFormTalkFile model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzFormTalkFile(Epm_TzFormTalkFile model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzFormTalkFileByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzProjectProposal>> GetTzFormTalkFileList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<TzFormTalkFileView> GetTzFormTalkFileModel(long id);

        /// <summary>
        /// 修改评审材料上报状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateTzFormTalkFileState(List<long> ids, string state);

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzTalkFileAudit(Epm_TzTalkFileAudit model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzTalkFileAudit(Epm_TzTalkFileAudit model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzTalkFileAuditByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzTalkFileAudit>> GetTzTalkFileAuditList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzTalkFileAudit> GetTzTalkFileAuditModel(long id);
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzTalkFileHeadAudit(Epm_TzTalkFileHeadAudit model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzTalkFileHeadAudit(Epm_TzTalkFileHeadAudit model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzTalkFileHeadAuditByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzTalkFileHeadAudit>> GetTzTalkFileHeadAuditList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzTalkFileHeadAudit> GetTzTalkFileHeadAuditModel(long id);
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzTalkLeaderSign(Epm_TzTalkLeaderSign model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzTalkLeaderSign(Epm_TzTalkLeaderSign model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzTalkLeaderSignByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzTalkLeaderSign>> GetTzTalkLeaderSignList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzTalkLeaderSign> GetTzTalkLeaderSignModel(long id);
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzTalkRecord(Epm_TzTalkRecord model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzTalkRecord(Epm_TzTalkRecord model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzTalkRecordByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzTalkRecord>> GetTzTalkRecordList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzTalkRecord> GetTzTalkRecordModel(long id);
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzTalkRecordConfirm(Epm_TzTalkRecordConfirm model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzTalkRecordConfirm(Epm_TzTalkRecordConfirm model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzTalkRecordConfirmByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzTalkRecordConfirm>> GetTzTalkRecordConfirmList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzTalkRecordConfirm> GetTzTalkRecordConfirmModel(long id);
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzTalkSign(Epm_TzTalkSign model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzTalkSign(Epm_TzTalkSign model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzTalkSignByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzTalkSign>> GetTzTalkSignList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzTalkSign> GetTzTalkSignModel(long id);


        #endregion

        #region 项目批复请示、二次、三次、四次。。。谈判、二次、三次、四次。。。谈判审核
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzProjectApproval(Epm_TzProjectApproval model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzProjectApproval(Epm_TzProjectApproval model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzProjectApprovalByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzProjectApproval>> GetTzProjectApprovalList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzProjectApproval> GetTzProjectApprovalModel(long id);
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzSecondTakl(Epm_TzSecondTakl model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzSecondTakl(Epm_TzSecondTakl model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzSecondTaklByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzSecondTakl>> GetTzSecondTaklList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzSecondTakl> GetTzSecondTaklModel(long id);
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzSecondTalkAudit(Epm_TzSecondTalkAudit model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzSecondTalkAudit(Epm_TzSecondTalkAudit model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzSecondTalkAuditByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzSecondTalkAudit>> GetTzSecondTalkAuditList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzSecondTalkAudit> GetTzSecondTalkAuditModel(long id);


        #endregion

        #region  工程建设项目开工报告、建设工程设计变更申请

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzDesiginChangeApply(Epm_TzDesiginChangeApply model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzDesiginChangeApply(Epm_TzDesiginChangeApply model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzDesiginChangeApplyByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzDesiginChangeApply>> GetTzDesiginChangeApplyList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzDesiginChangeApply> GetTzDesiginChangeApplyModel(long id);

        /// <summary>
        /// 修改设计方案变更申请状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateTzDesiginChangeApplyState(List<long> ids, string state);

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzStartsApply(Epm_TzStartsApply model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzStartsApply(Epm_TzStartsApply model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzStartsApplyByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzStartsApply>> GetTzStartsApplyList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzStartsApply> GetTzStartsApplyModel(long id);
        #endregion

        #region 陕西省各竞争对手加油（气）站现状上报流程   建设工程项目管理人员变更申请流程  加油（气）站开发资源上报流程

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzDevResourceReport(Epm_TzDevResourceReport model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzDevResourceReport(Epm_TzDevResourceReport model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzDevResourceReportByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzDevResourceReport>> GetTzDevResourceReportList(QueryCondition qc);
        [OperationContract]
        Result<List<TzDevResourceReportItemView>> GetTzDevResourceReportItemList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzDevResourceReport> GetTzDevResourceReportModel(long id);
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzPeopleChgApply(Epm_TzPeopleChgApply model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzPeopleChgApply(Epm_TzPeopleChgApply model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzPeopleChgApplyByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzPeopleChgApply>> GetTzPeopleChgApplyList(QueryCondition qc);

        [OperationContract]
        Result<List<TzPeopleChgApplyItemView>> GetTzPeopleApplyItemList(QueryCondition qc);

        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzPeopleChgApply> GetTzPeopleChgApplyModel(long id);
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzRivalStationReport(Epm_TzRivalStationReport model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzRivalStationReport(Epm_TzRivalStationReport model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzRivalStationReportByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzRivalStationReport>> GetTzRivalStationReportList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzRivalStationReport> GetTzRivalStationReportModel(long id);

        #endregion

        /// <summary>
        /// 获取项目统计数据
        /// </summary>
        /// <param name="type">1:全部，2：新增，3：改造</param>
        /// <returns></returns>
        [OperationContract]
        Result<List<ProjectCount>> GetProjectCount(int type, string time);

        /// <summary>
        /// 项目信息汇总
        /// </summary>
        /// <param name="type">1:全部，2：新增，3：改造</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<ProjectView>> GetProjectSum(int type, string time, int stateType, int pageIndex, int pageSize);
        [OperationContract]
        Result<List<Epm_ProjectWeekly>> GetProjectWeekly(int type, string time, int stateType, int pageIndex, int pageSize);
        [OperationContract]
        Result<List<Epm_ProjectCountWeekly>> GetProjectCountWeekly(int type, string time);
        #region 工程甲供物资订单
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzGcGoodsOrdersApply(Epm_TzGcGoodsOrdersApply model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzGcGoodsOrdersApply(Epm_TzGcGoodsOrdersApply model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzGcGoodsOrdersApplyByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzGcGoodsOrdersApply>> GetTzGcGoodsOrdersApplyList(QueryCondition qc);

        /// <summary>
        /// 获取详情数据列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<TzGcGoodsOrdersItemView>> GetTzGcGoodsOrdersApplyListAll(QueryCondition qc);

        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzGcGoodsOrdersApply> GetTzGcGoodsOrdersApplyModel(long id);

        /// <summary>
        /// 上会材料上报
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddConferenceMaterials(string tableName, long id, List<Epm_TzAttachs> fileList, InvestmentEnclosure ie);
        #endregion
        /// <summary>
        /// 上会材料上报
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_TzAttachs>> GetConferenceFiles(string tableName, long id, InvestmentEnclosure ie = InvestmentEnclosure.itself);

        #region 初次谈判、土地出让协议

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzFirstNegotiation(Epm_TzFirstNegotiation model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzFirstNegotiation(Epm_TzFirstNegotiation model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzFirstNegotiationByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzProjectProposal>> GetTzFirstNegotiationList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<TzFirstNegotiationView> GetTzFirstNegotiationModel(long projectId);
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzLandNegotiation(Epm_TzLandNegotiation model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzLandNegotiation(Epm_TzLandNegotiation model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzLandNegotiationByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzProjectProposal>> GetTzLandNegotiationList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<TzLandNegotiationView> GetTzLandNegotiationModel(long projectId);
        #endregion

        #region  项目评审记录 上会材料上报 项目批复信息
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddMeetingFileReport(Epm_MeetingFileReport model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateMeetingFileReport(Epm_MeetingFileReport model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteMeetingFileReportByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzProjectProposal>> GetMeetingFileReportList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<MeetingFileReportView> GetMeetingFileReportModel(long id);

        /// <summary>
        /// 修改上会材料上报状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateMeetingFileReportState(List<long> ids, string state);

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzProjectApprovalInfo(Epm_TzProjectApprovalInfo model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzProjectApprovalInfo(Epm_TzProjectApprovalInfo model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzProjectApprovalInfoByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzProjectProposal>> GetTzProjectApprovalInfoList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<TzProjectApprovalInfoView> GetTzProjectApprovalInfoModel(long id);

        /// <summary>
        /// 财务决算查询
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_TzProjectApprovalInfo>> GetTzProjectApprovalListBy(QueryCondition qc);

        /// <summary>
        /// 编辑财务决算信息
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="FinanceAccounts"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateFinanceAccounts(long Id, decimal financeAccounts);

        /// <summary>
        /// 财务决算详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        Result<Epm_TzProjectApprovalInfo> GetTzProjectApprovalFinanceAccounts(long id);

        /// <summary>
        /// 修改项目批复信息状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateTzProjectApprovalInfoState(List<long> ids, string state);
        ///<summary>
        ///添加:项目评审记录
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzProjectReveiews(Epm_TzProjectReveiews model);
        ///<summary>
        ///修改:项目评审记录
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzProjectReveiews(Epm_TzProjectReveiews model);
        ///<summary>
        ///删除:项目评审记录
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzProjectReveiewsByIds(List<long> ids);
        ///<summary>
        ///获取列表:项目评审记录
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzProjectProposal>> GetTzProjectReveiewsList(QueryCondition qc);
        ///<summary>
        ///获取详情:项目评审记录
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<TzProjectReveiewsView> GetTzProjectReveiewsModel(long id);

        /// <summary>
        /// 修改项目评审记录状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateTzProjectReveiewsState(List<long> ids, string state);

        #endregion

        #region 甲供物资申请
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzSupplyMaterialApply(Epm_TzSupplyMaterialApply model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzSupplyMaterialApply(Epm_TzSupplyMaterialApply model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzSupplyMaterialApplyByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzSupplyMaterialApply>> GetTzSupplyMaterialApplyList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzSupplyMaterialApply> GetTzSupplyMaterialApplyModel(long id);

        /// <summary>
        /// 获取甲供物资申请详情列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_TzSupMatApplyList>> GetTzSupMatApplyList(QueryCondition qc);

        /// <summary>
        /// 修改甲供物资申请状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateTzSupplyMaterialApplyState(List<long> ids, string state);

        /// <summary>
        /// 获取甲供物资报表数据
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<SupplyMaterialReportView>> GetSupplyMaterialReport(QueryCondition qc);

        /// <summary>
        /// 获取甲供物资供应商
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<SuppListView>> GetSuppList(long projectId, QueryCondition qc);
        #endregion

        #region 招标申请
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzTenderingApply(Epm_TzTenderingApply model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzTenderingApply(Epm_TzTenderingApply model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzTenderingApplyByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzTenderingApply>> GetTzTenderingApplyList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzTenderingApply> GetTzTenderingApplyModel(long id);

        /// <summary>
        /// 修改招标申请状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateTzTenderingApplyState(List<long> ids, string state);
        #endregion

        #region 招标结果
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzBidResult(Epm_TzBidResult model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzBidResult(Epm_TzBidResult model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzBidResultByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzBidResult>> GetTzBidResultList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzBidResult> GetTzBidResultModel(long id);

        /// <summary>
        /// 修改招标结果状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateTzBidResultState(List<long> ids, string state);
        #endregion

        #region 招标申请统计
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<TzTenderingCountView>> GetTzTenderingCountList(QueryCondition qc);
        #endregion

        #region 甲供物资管理

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzSupMatManagement(Epm_TzSupMatManagement model);

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> AddRangeTzSupMatManagement(List<Epm_TzSupMatManagement> models);

        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzSupMatManagement(Epm_TzSupMatManagement model);
        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="ids">数据集合</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateTzSupMatManagementState(List<long> ids, int state);

        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzSupMatManagementByIds(List<long> ids);

        /// <summary>
        /// 根据供应商名称获取供应商信息
        /// </summary>
        /// <param name="companyName">供应商名称</param>
        [OperationContract]
        Result<Base_Company> GetCompanyByName(string companyName);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzSupMatManagement>> GetTzSupMatManagementList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzSupMatManagement> GetTzSupMatManagementModel(long id);

        /// <summary>
        /// 获取已启用的甲供物资申请数据
        /// </summary>
        /// <param name="name">物资种类</param>
        /// <param name="productName">品名</param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_TzSupMatManagement>> GetTzSupMatManagementListBy(long SupplierId, string name, string productName);

        /// <summary>
        /// 根据物资种类、品名、规格获取物资管理详细信息
        /// </summary>
        /// <param name="name">物资种类</param>
        /// <param name="productName">品名</param>
        /// <param name="specification">规格</param>
        /// <returns></returns>
        [OperationContract]
        Result<Epm_TzSupMatManagement> GetTzSupMatManagementModelBy(long SupplierId, string name, string productName, string specification);
        #endregion

        #region 设计方案
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzDesignScheme(Epm_TzDesignScheme model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzDesignScheme(Epm_TzDesignScheme model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzDesignSchemeByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzDesignScheme>> GetTzDesignSchemeList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzDesignScheme> GetTzDesignSchemeModel(long id);

        /// <summary>
        /// 选择项目时加载所有项目信息（暂时加载所有的项目信息，后面要加具体的筛选条件）
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<Epm_TzProjectApproval>> GetTzProjectApprovalList_Choice(QueryCondition qc);

        /// <summary>
        /// 修改设计方案状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateTzDesignSchemeState(List<long> ids, string state);
        #endregion

        #region 开工申请
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzProjectStartApply(Epm_TzProjectStartApply model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzProjectStartApply(Epm_TzProjectStartApply model);

        /// <summary>
        /// 修改开工申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateTzProjectStartApplyNew(TzProjectStartApplyView model);

        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzProjectStartApplyByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzProjectStartApply>> GetTzProjectStartApplyList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TzProjectStartApply> GetTzProjectStartApplyModel(long id);

        [OperationContract]
        Result<TzStartTenderingAndSupplyView> GetTzProjectStartApplyModelAndOther(long id);

        [OperationContract]
        Result<Epm_TimeLimitAndProcedure> GetTimeLimitAndProcedure(long id);



        [OperationContract]
        Result<bool> isExistTenderingAndSupply(long projectId);
        /// <summary>
        /// 修改开工申请状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateTzProjectStartApplyState(List<long> ids, string state);
        #endregion

        #region 图纸会审
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTzConDrawing(Epm_TzConDrawing model, List<Base_Files> fileListFile);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTzConDrawing(Epm_TzConDrawing model, List<Base_Files> fileListFile);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTzConDrawingByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TzConDrawing>> GetTzConDrawingList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<TzConDrawingView> GetTzConDrawingModel(long id);

        ///<summary>
        ///根据ID加载项目基础信息:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<ProjectBasicsInfoView> GetProjectBasicInfoByID(long id);

        /// <summary>
        /// 修改施工图纸会审状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        [OperationContract]
        Result<int> UpdateTzConDrawingState(List<long> ids, string state);
        #endregion

        #region 工期和手续
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddTimeLimitAndCrossings(Epm_TimeLimitAndProcedure model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateTimeLimitAndCrossings(Epm_TimeLimitAndProcedure model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteTimeLimitAndCrossingsByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_TimeLimitAndProcedure>> GetTimeLimitAndCrossingsList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<Epm_TimeLimitAndProcedure> GetTimeLimitAndCrossingsModel(long id);

        #endregion

        #region 竣工验收
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> AddCompletionAcceptanceResUpload(Epm_CompletionAcceptanceResUpload model);
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> UpdateCompletionAcceptanceResUpload(Epm_CompletionAcceptanceResUpload model);
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        [OperationContract]
        Result<int> DeleteCompletionAcceptanceResUploadByIds(List<long> ids);
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        [OperationContract]
        Result<List<Epm_CompletionAcceptanceResUpload>> GetCompletionAcceptanceResUploadList(QueryCondition qc);
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        [OperationContract]
        Result<CompletionAcceptanceResUploadView> GetCompletionAcceptanceResUploadModel(long id);

        [OperationContract]
        Result<int> UpdateCompletionAcceptanceResUploadState(List<long> ids, string state);

        #endregion

        #region 站库改造
        /////<summary>
        /////添加:
        /////</summary>
        /////<param name="model">要添加的model</param>
        /////<returns>受影响的行数</returns>
        //[OperationContract]
        //Result<int> AddReformRecord(Epm_ReformRecord model);
        /////<summary>
        /////修改:
        /////</summary>
        /////<param name="model">要修改的model</param>
        /////<returns>受影响的行数</returns>
        //[OperationContract]
        //Result<int> UpdateReformRecord(Epm_ReformRecord model);
        /////<summary>
        /////删除:
        /////</summary>
        /////<param name="ids">要删除的Id集合</param>
        /////<returns>受影响的行数</returns>
        //[OperationContract]
        //Result<int> DeleteReformRecordByIds(List<long> ids);
        /////<summary>
        /////获取列表:
        /////</summary>
        /////<param name="qc">查询条件</param>
        /////<returns>符合条件的数据集合</returns>
        //[OperationContract]
        //Result<List<Epm_ReformRecord>> GetReformRecordList(QueryCondition qc);
        /////<summary>
        /////获取详情:
        /////</summary>
        /////<param name="id">数据Id</param>
        /////<returns>数据详情model</returns>
        //[OperationContract]
        //Result<Epm_ReformRecord> GetReformRecordModel(long id);


        #endregion

        #region 考勤统计
        /// <summary>
        /// 分公司统计
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<AttendanceBranchCountView>> GetBranchCount(string companyId, string startTime, string endTime, int pageIndex, int pageSize);

        /// <summary>
        /// 项目统计
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<AttendanceBranchCountView>> GetBranchProjectCount(string name, string companyId, string startTime, string endTime, int pageIndex, int pageSize);

        /// <summary>
        /// 人员统计
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<List<AttendanceBranchCountView>> GetBranchUserCount(string ProName, string companyId, string startTime, string endTime, string userName, int pageIndex, int pageSize);


        #endregion

        /// <summary>
        /// 查询监理签到统计
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        [OperationContract]
        Result<List<SupervisorLogStatisticView>> GetSupervisionAttendance(QueryCondition qc);

        /// <summary>
        /// 获取考勤设置信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Result<AttendanceView> GetAttendanceModel();
    }
}
