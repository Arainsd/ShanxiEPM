using hc.epm.Common;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using hc.epm.DataModel.Msg;
using hc.epm.Service.ClientSite;
using hc.epm.ViewModel;
using hc.Plat.Common.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using hc.epm.ViewModel.AppView;
using System.Data;
using hc.epm.DataModel.BaseCore;
using hc.Plat.WebAPI.Base.ViewModel;

namespace hc.epm.Web.ClientProxy
{
    public class ClientSiteClientProxy : ClientBase<IClientSiteService>, IClientSiteService
    {
        public ClientSiteClientProxy(ClientProxyExType cpet)
        {
            //传输当前用户的信息；
            ApplicationContext.Current.UserID = cpet.UserID;
            ApplicationContext.Current.WebIP = cpet.IP_WebServer;
            ApplicationContext.Current.ClientIP = cpet.IP_Client;
            if (cpet.CurrentUser != null)
            {
                UserView userView = cpet.CurrentUser as UserView;
                if (userView != null)
                {
                    ApplicationContext.Current.UserName = userView.RealName;
                    ApplicationContext.Current.CompanyId = userView.CompanyId.ToString();
                    ApplicationContext.Current.CompanyName = userView.CompanyName;
                    ApplicationContext.Current.RoleType = userView.RoleType.ToString();
                    ApplicationContext.Current.ProjectIds = userView.Project == null ? "" : string.Join(",", userView.Project.Keys.ToArray());
                }
            }
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

        public Result<List<Epm_TzAttachs>> GetConferenceFiles(string tableName, long id, InvestmentEnclosure ie)
        {
            return base.Channel.GetConferenceFiles(tableName, id, ie);
        }

        #region Epm_Approver
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddApprover(Epm_Approver model)
        {
            return base.Channel.AddApprover(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateApprover(Epm_Approver model)
        {
            return base.Channel.UpdateApprover(model);
        }

        public Result<int> AddConferenceMaterials(string tableName, long id, List<Epm_TzAttachs> fileList, InvestmentEnclosure ie)
        {
            return base.Channel.AddConferenceMaterials(tableName, id, fileList, ie);
        }

        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteApproverByIds(List<long> ids)
        {
            return base.Channel.DeleteApproverByIds(ids);
        }

        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_Approver>> GetApproverList(QueryCondition qc)
        {
            return base.Channel.GetApproverList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_Approver> GetApproverModel(long id)
        {
            return base.Channel.GetApproverModel(id);
        }

        public Result<Epm_Approver> GetApproverModelByBusinId(long bussinesId, long approverId = 0)
        {
            return base.Channel.GetApproverModelByBusinId(bussinesId, approverId);
        }

        /// <summary>
        /// 处理待办事项
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<bool> ComplateApprover(long id)
        {
            return Channel.ComplateApprover(id);
        }

        /// <summary>
        /// 获取当前登录用户待办事项
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_Approver>> GetCurrUserApproverList(QueryCondition qc)
        {
            return Channel.GetCurrUserApproverList(qc);
        }

        #endregion

        #region 模型属性
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddCustomProperty(EPM_CustomProperty model)
        {
            return base.Channel.AddCustomProperty(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateCustomProperty(EPM_CustomProperty model)
        {
            return base.Channel.UpdateCustomProperty(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteCustomPropertyByIds(List<long> ids)
        {
            return base.Channel.DeleteCustomPropertyByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<EPM_CustomProperty>> GetCustomPropertyList(QueryCondition qc)
        {
            return base.Channel.GetCustomPropertyList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<EPM_CustomProperty> GetCustomPropertyModel(long id)
        {
            return base.Channel.GetCustomPropertyModel(id);
        }
        #endregion

        #region 模型管理

        /// <summary>
        /// 获取模型属性
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Result<DataSet> GetBimProperty(string path, string SQLString)
        {
            return base.Channel.GetBimProperty(path, SQLString);
        }

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddBim(Epm_Bim model, List<Base_Files> fileListFile)
        {
            return base.Channel.AddBim(model, fileListFile);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateBim(Epm_Bim model, List<Base_Files> fileListFile)
        {
            return base.Channel.UpdateBim(model, fileListFile);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteBimByIds(List<long> ids)
        {
            return base.Channel.DeleteBimByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_Bim>> GetBimList(QueryCondition qc)
        {
            return base.Channel.GetBimList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_Bim> GetBimModel(long id)
        {
            return base.Channel.GetBimModel(id);
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeBimState(long id, string state)
        {
            return base.Channel.ChangeBimState(id, state);
        }

        /// <summary>
        /// 审核/驳回
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public Result<int> RejectBim(long id, string state, string reason)
        {
            return base.Channel.RejectBim(id, state, reason);
        }

        /// <summary>
        /// 生成BIM模型图片
        /// </summary>
        /// <param name="id"></param>
        /// <param name="img"></param>
        /// <returns></returns>
        public Result<int> CreateImgBim(long id, string img, List<Base_Files> fileList)
        {
            return base.Channel.CreateImgBim(id, img, fileList);
        }

        /// <summary>
        /// 根据ProjectId获取BIM模型列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Result<List<Epm_Bim>> GetBimModelListByProjectId(long projectId)
        {
            return base.Channel.GetBimModelListByProjectId(projectId);
        }

        /// <summary>
        /// 获取首页展示模型列表
        /// </summary>
        /// <returns></returns>
        public Result<List<Epm_Bim>> GetBimModelIndexList()
        {
            return Channel.GetBimModelIndexList();
        }

        #endregion

        #region 变更管理
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddChange(ChangeView model, List<Base_Files> fileList)
        {
            return base.Channel.AddChange(model, fileList);
        }

        public object GetProjectCompany(long? projectId)
        {
            throw new NotImplementedException();
        }

        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateChange(ChangeView model, List<Base_Files> fileList)
        {
            return base.Channel.UpdateChange(model, fileList);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteChangeByIds(List<long> ids)
        {
            return base.Channel.DeleteChangeByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<ChangeView>> GetChangeList(string projectName, string name, int state, int pageIndex, int pageSize)
        {
            return base.Channel.GetChangeList(projectName, name, state, pageIndex, pageSize);
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<ChangeView> GetChangeModel(long id)
        {
            return base.Channel.GetChangeModel(id);
        }
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateChangeState(long id, string state)
        {
            return base.Channel.UpdateChangeState(id, state);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_Change>> GetChangListByQc(QueryCondition qc)
        {
            return base.Channel.GetChangListByQc(qc);
        }
        #endregion

        #region 竣工验收

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddCompletionAcceptance(Epm_CompletionAcceptance model, List<Base_Files> fileListFile)
        {
            return base.Channel.AddCompletionAcceptance(model, fileListFile);
        }

        /// <summary>
        /// 新增完工验收
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public Result<int> AddCompletionAcceptanceNew(CompletionAcceptanceView view)
        {
            return Channel.AddCompletionAcceptanceNew(view);
        }


        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateCompletionAcceptance(Epm_CompletionAcceptance model, List<Base_Files> fileListFile)
        {
            return base.Channel.UpdateCompletionAcceptance(model, fileListFile);
        }

        ///<summary>
        ///修改完工验收
        ///</summary>
        /// <param name="view">完工验收</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateCompletionAcceptanceNew(CompletionAcceptanceView view)
        {
            return Channel.UpdateCompletionAcceptanceNew(view);
        }


        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteCompletionAcceptanceByIds(List<long> ids)
        {
            return base.Channel.DeleteCompletionAcceptanceByIds(ids);
        }

        ///<summary>
        ///修改状态:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateCompletionAcceptanceState(List<long> ids, string state)
        {
            return base.Channel.UpdateCompletionAcceptanceState(ids, state);
        }


        /// <summary>
        /// 根据 ID 删除完工验收
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<bool> DeleteCompletionAcceptanceById(long id)
        {
            return Channel.DeleteCompletionAcceptanceById(id);
        }


        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_CompletionAcceptance>> GetCompletionAcceptanceList(QueryCondition qc)
        {
            return base.Channel.GetCompletionAcceptanceList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_CompletionAcceptance> GetCompletionAcceptanceModel(long id)
        {
            return base.Channel.GetCompletionAcceptanceModel(id);
        }

        public Result<CompletionAcceptanceView> GetCompletionAcceptanceModelNew(long id)
        {
            return Channel.GetCompletionAcceptanceModelNew(id);
        }


        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public Result<int> ChangeCompletionAcceptanceState(long id, PreCompletionScceptanceState state, string reason)
        {
            return base.Channel.ChangeCompletionAcceptanceState(id, state, reason);
        }

        /// <summary>
        /// 根据项目 ID 获取验收项资料
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<List<CompletionAcceptanceItemView>> GetCompletionItem(long id)
        {
            return Channel.GetCompletionItem(id);
        }

        #endregion

        #region 合同管理
        public Result<int> DeleteContractModel(long id)
        {
            return base.Channel.DeleteContractModel(id);
        }
        public Result<int> DeleteFilesByTableIds(string tableName, List<long> tableIds)
        {
            return base.Channel.DeleteFilesByTableIds(tableName, tableIds);
        }
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddContract(Epm_Contract model, List<Base_Files> fileList = null)
        {
            return base.Channel.AddContract(model, fileList);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateContract(Epm_Contract model, List<Base_Files> fileList = null)
        {
            return base.Channel.UpdateContract(model, fileList);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteContractByIds(List<long> ids)
        {
            return base.Channel.DeleteContractByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_Contract>> GetContractList(QueryCondition qc)
        {
            return base.Channel.GetContractList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_Contract> GetContractModel(long id)
        {
            return base.Channel.GetContractModel(id);
        }
        public Result<List<Base_Files>> GetContractModelFile(long id)
        {
            return base.Channel.GetContractModelFile(id);
        }
        public Result<List<Base_Files>> GetContractModelFileName(long contractId, string fileNames)
        {
            return base.Channel.GetContractModelFileName(contractId, fileNames);
        }
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateContractState(long id, string state)
        {
            return base.Channel.UpdateContractState(id, state);
        }
        #endregion

        #region 危险作业
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddDangerousWork(Epm_DangerousWork model, List<Base_Files> files)
        {
            return base.Channel.AddDangerousWork(model, files);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateDangerousWork(Epm_DangerousWork model, List<Base_Files> files)
        {
            return base.Channel.UpdateDangerousWork(model, files);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<int> DeleteDangerousWork(long id)
        {
            return base.Channel.DeleteDangerousWork(id);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteDangerousWorkByIds(List<long> ids)
        {
            return base.Channel.DeleteDangerousWorkByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_DangerousWork>> GetDangerousWorkList(QueryCondition qc)
        {
            return base.Channel.GetDangerousWorkList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_DangerousWork> GetDangerousWorkModel(long id)
        {
            return base.Channel.GetDangerousWorkModel(id);
        }
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateDangerousWorkState(long id, ApprovalState state)
        {
            return base.Channel.UpdateDangerousWorkState(id, state);
        }

        #endregion

        #region 项目资料

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddDataConfig(Epm_DataConfig model)
        {
            return base.Channel.AddDataConfig(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateDataConfig(Epm_DataConfig model)
        {
            return base.Channel.UpdateDataConfig(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteDataConfigByIds(List<long> ids)
        {
            return base.Channel.DeleteDataConfigByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_DataConfig>> GetDataConfigList(QueryCondition qc)
        {
            return base.Channel.GetDataConfigList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_DataConfig> GetDataConfigModel(long id)
        {
            return base.Channel.GetDataConfigModel(id);
        }
        #endregion

        #region 图纸管理
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddDraw(Epm_Draw model, List<Base_Files> fileListFile)
        {
            return base.Channel.AddDraw(model, fileListFile);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateDraw(Epm_Draw model, List<Base_Files> fileListFile)
        {
            return base.Channel.UpdateDraw(model, fileListFile);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteDrawByIds(List<long> ids)
        {
            return base.Channel.DeleteDrawByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_Draw>> GetDrawList(QueryCondition qc)
        {
            return base.Channel.GetDrawList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_Draw> GetDrawModel(long id)
        {
            return base.Channel.GetDrawModel(id);
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeDrawState(long id, string state)
        {
            return base.Channel.ChangeDrawState(id, state);
        }

        /// <summary>
        /// 审核/驳回
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public Result<int> RejectDraw(long id, string state, string reason)
        {
            return base.Channel.RejectDraw(id, state, reason);
        }
        #endregion

        #region 工器具验收
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddMaterial(MaterialView model)
        {
            return base.Channel.AddMaterial(model);
        }

        /// <summary>
        /// 工器具验收
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddMaterialNew(MaterialViewNew model)
        {
            return base.Channel.AddMaterialNew(model);
        }

        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateMaterial(MaterialView model)
        {
            return base.Channel.UpdateMaterial(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteMaterialByIds(List<long> ids)
        {
            return base.Channel.DeleteMaterialByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_Material>> GetMaterialList(QueryCondition qc)
        {
            return base.Channel.GetMaterialList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<MaterialView> GetMaterialModel(long id)
        {
            return base.Channel.GetMaterialModel(id);
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateMaterialState(long id, ConfirmState state)
        {
            return base.Channel.UpdateMaterialState(id, state);
        }
        #endregion

        #region 材料设备验收
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddMateriel(MaterielView model)
        {
            return base.Channel.AddMateriel(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateMateriel(MaterielView model)
        {
            return base.Channel.UpdateMateriel(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteMaterielByIds(List<long> ids)
        {
            return base.Channel.DeleteMaterielByIds(ids);
        }

        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_Materiel>> GetMaterielList(QueryCondition qc)
        {
            return base.Channel.GetMaterielList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<MaterielView> GetMaterielModel(long id)
        {
            return base.Channel.GetMaterielModel(id);
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeMaterielState(long id, ConfirmState state)
        {
            return base.Channel.ChangeMaterielState(id, state);
        }

        /// <summary>
        /// 批量修改状态
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeMaterielALLState(List<long> ids, string state)
        {
            return base.Channel.ChangeMaterielALLState(ids, state);
        }
        #endregion

        #region 里程碑
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddMilepost(Epm_Milepost model)
        {
            return base.Channel.AddMilepost(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateMilepost(Epm_Milepost model)
        {
            return base.Channel.UpdateMilepost(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteMilepostByIds(List<long> ids)
        {
            return base.Channel.DeleteMilepostByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_Milepost>> GetMilepostList(QueryCondition qc)
        {
            return base.Channel.GetMilepostList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_Milepost> GetMilepostModel(long id)
        {
            return base.Channel.GetMilepostModel(id);
        }

        /// <summary>
        /// 获取里程碑树形列表
        /// </summary>
        /// <returns></returns>
        public Result<List<MilepostView>> GetMilepostViewList()
        {
            return base.Channel.GetMilepostViewList();
        }

        /// <summary>
        /// 根绝项目资料ID获取里程碑信息
        /// </summary>
        /// <param name="dataConfigId"></param>
        /// <returns></returns>
        public Result<Epm_MilepostData> GetMDataByDataId(long dataConfigId)
        {
            return base.Channel.GetMDataByDataId(dataConfigId);
        }
        #endregion

        #region 安全质量检查

        /// <summary>
        /// 新增检查
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Result<bool> AddMonitorNew(long projectId, DateTime time, List<CheckView> dataList, int type = 0)
        {
            return Channel.AddMonitorNew(projectId, time, dataList, type);
        }
        public Result<bool> AddRectificationNew(long projectId, DateTime time, List<checkItemesPer> dataList, int type = 0)
        {
            return Channel.AddRectificationNew(projectId, time, dataList, type);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_InspectItem>> GetInspectItemList(long inspectId)
        {
            return base.Channel.GetInspectItemList(inspectId);
        }

        public Result<List<Epm_InspectItem>> GetInspectItemByProjectId(long projectId)
        {
            return base.Channel.GetInspectItemByProjectId(projectId);
        }
        public Result<List<Epm_InspectItem>> GetInspectItemDraft(long projectId)
        {
            return base.Channel.GetInspectItemDraft(projectId);
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_Inspect> GetInspectModel(long id)
        {
            return base.Channel.GetInspectModel(id);
        }

        /// <summary>
        /// 获取检查单非常规作业
        /// </summary>
        /// <param name="inspectId"></param>
        /// <returns></returns>
        public Result<List<EPM_UnconventionalWork>> GetIUnconventionalWorkList(long inspectId)
        {
            return base.Channel.GetIUnconventionalWorkList(inspectId);
        }

        /// <summary>
        /// 获取非常规作业和复查、复核列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<EPM_UnconventionalWork>> GetWorkList(QueryCondition qc)
        {
            return base.Channel.GetWorkList(qc);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<Epm_Rectification> GetRectificationModel(long id)
        {
            return base.Channel.GetRectificationModel(id);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<List<Epm_RectificationItem>> GetRectificationItemList(long rectifId)
        {
            return base.Channel.GetRectificationItemList(rectifId);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<List<Epm_RectificationRecord>> GetRectificationRecordList(long rectifId)
        {
            return base.Channel.GetRectificationRecordList(rectifId);
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<List<Epm_RectificationItem>> GetRectificationItemListByProjectId(long projectId)
        {
            return base.Channel.GetRectificationItemListByProjectId(projectId);
        }

        public Result<List<Epm_RectificationItem>> GetRectificationItemListALLByProjectId(long projectId)
        {
            return base.Channel.GetRectificationItemListALLByProjectId(projectId);
        }
        ///<summary>
        ///添加:检查整改记录表
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddMonitorRectifRecord(long id, string content, List<Base_Files> files)
        {
            return base.Channel.AddMonitorRectifRecord(id, content, files);
        }
        /// <summary>
        /// 修改检查整改记录状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeMonitorState(long rectifItemId, RectificationState state, string Remark)
        {
            return base.Channel.ChangeMonitorState(rectifItemId, state, Remark);
        }
        #endregion

        #region  新闻资讯
        ///<summary>
        ///添加:新闻、资讯表
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddNews(Epm_News model)
        {
            return base.Channel.AddNews(model);
        }
        ///<summary>
        ///修改:新闻、资讯表
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateNews(Epm_News model)
        {
            return base.Channel.UpdateNews(model);
        }
        ///<summary>
        ///删除:新闻、资讯表
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteNewsByIds(List<long> ids)
        {
            return base.Channel.DeleteNewsByIds(ids);
        }
        ///<summary>
        ///获取列表:新闻、资讯表
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_News>> GetNewsList(QueryCondition qc)
        {
            return base.Channel.GetNewsList(qc);
        }
        ///<summary>
        ///获取详情:新闻、资讯表
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_News> GetNewsModel(long id)
        {
            return base.Channel.GetNewsModel(id);
        }
        #endregion

        #region 计划管理

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddPlan(Epm_Plan model, List<Epm_PlanComponent> planComponentIds)
        {
            return base.Channel.AddPlan(model, planComponentIds);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdatePlan(Epm_Plan model, List<Epm_PlanComponent> planComponentIds)
        {
            return base.Channel.UpdatePlan(model, planComponentIds);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeletePlanByIds(List<long> ids)
        {
            return base.Channel.DeletePlanByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_Plan>> GetPlanList(QueryCondition qc)
        {
            return base.Channel.GetPlanList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<PlanView> GetPlanModel(long id)
        {
            return base.Channel.GetPlanModel(id);
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="batchNo"></param>
        /// <param name="state"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public Result<int> ChangePlanState(string batchNo, ApprovalState state, string reason)
        {
            return base.Channel.ChangePlanState(batchNo, state, reason);
        }

        /// <summary>
        /// 根据计划id和模型ID获取关联组件列表
        /// </summary>
        /// <param name="PlanId"></param>
        /// <param name="bimId"></param>
        /// <returns></returns>
        public Result<List<Epm_PlanComponent>> GetComponentListByPlanId(long PlanId, long bimId)
        {
            return base.Channel.GetComponentListByPlanId(PlanId, bimId);
        }

        /// <summary>
        /// 计划关联模型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddPlanComponent(Epm_PlanComponent model, string planComponentIds)
        {
            return base.Channel.AddPlanComponent(model, planComponentIds);
        }

        /// <summary>
        /// 根据parentId获取计划信息
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        public Result<Epm_Plan> GetPlanById(long parentId)
        {
            return base.Channel.GetPlanById(parentId);
        }

        /// <summary>
        /// 获取施工计划树形列表数据
        /// </summary>
        /// <returns></returns>
        public Result<List<PlanView>> GetPlanViewList(long ProjectId)
        {
            return base.Channel.GetPlanViewList(ProjectId);
        }

        /// <summary>
        /// 获取进度跟踪树形列表数据
        /// </summary>
        /// <returns></returns>
        public Result<List<PlanView>> GetScheduleViewList(long ProjectId)
        {
            return base.Channel.GetScheduleViewList(ProjectId);
        }

        /// <summary>
        /// 获取里程碑计划
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Result<List<PlanView>> GetMilepostPlan(long projectId)
        {
            return base.Channel.GetMilepostPlan(projectId);
        }

        /// <summary>
        /// 生成里程碑计划
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="planStart"></param>
        /// <returns></returns>
        public Result<List<Epm_Plan>> CreateMilepostPlan(long projectId, DateTime planStart, long mileType, int type = 1)
        {
            return base.Channel.CreateMilepostPlan(projectId, planStart, mileType, type);
        }
        /// <summary>
        /// 更新里程碑计划
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Result<int> UpdateMilepostPlan(List<Epm_Plan> list, int type = 1)
        {
            return base.Channel.UpdateMilepostPlan(list, type);
        }
        /// <summary>
        /// 审核里程碑计划
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Result<int> AuditMilepostPlan(List<Epm_Plan> list)
        {
            return base.Channel.AuditMilepostPlan(list);
        }
        /// <summary>
        /// 驳回里程碑计划
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Result<int> RejectMilepostPlan(long projectId)
        {
            return base.Channel.RejectMilepostPlan(projectId);
        }
        /// <summary>
        /// 关联构件
        /// </summary>
        /// <param name="planId"></param>
        /// <param name="pcList"></param>
        /// <returns></returns>
        public Result<int> BindComponents(long planId, List<Epm_PlanComponent> pcList)
        {
            return base.Channel.BindComponents(planId, pcList);
        }

        /// <summary>
        /// 根据项目施工计划获取项目进度甘特图
        /// </summary>
        /// <param name="projectId">项目 ID</param>
        /// <returns></returns>
        public Result<List<Gantt>> GetProjectGantt(long projectId)
        {
            return Channel.GetProjectGantt(projectId);
        }

        #endregion

        #region 项目管理
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddProject(Epm_Project model)
        {
            return base.Channel.AddProject(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateProject(Epm_Project model, List<Base_Files> attachs)
        {
            return base.Channel.UpdateProject(model, attachs);
        }

        #endregion
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteProjectByIds(List<long> ids)
        {
            return base.Channel.DeleteProjectByIds(ids);
        }

        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_Project>> GetProjectList(QueryCondition qc)
        {
            return base.Channel.GetProjectList(qc);
        }
        public Result<Epm_TzProjectApprovalInfo> GetProjectApprovalInfos(long projectId)
        {
            return base.Channel.GetProjectApprovalInfos(projectId);
        }
        /// <summary>
        /// 在建项目列表（多表查询）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public Result<List<ProjectView>> GetProjectListInfo(int pageIndex, int pageSize, string state, string pmName, string name = "", string startTime = "", string endTime = "")
        {
            return base.Channel.GetProjectListInfo(pageIndex, pageSize, state, pmName, name, startTime, endTime);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_Project> GetProjectModel(long id)
        {
            return base.Channel.GetProjectModel(id);
        }
        public Result<Epm_Project> GetProjectModelByTzId(long id)
        {
            return base.Channel.GetProjectModelByTzId(id);
        }

        /// <summary>
        /// 获取项目详情信息（项目资料、里程碑、第三方单位）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_Project> GetProject(long id)
        {
            return base.Channel.GetProject(id);
        }

        /// <summary>
        /// 修改项目状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeProjectState(long id, string state)
        {
            return base.Channel.ChangeProjectState(id, state);
        }

        /// <summary>
        /// 查询所有在建项目,根据登录人所属公司查询所有参与项目信息
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_Project>> GetProjectListByQc(QueryCondition qc)
        {
            return base.Channel.GetProjectListByQc(qc);
        }
        /// <summary>
        /// 查询所有在建项目,根据登录人所属公司查询所有参与项目信息
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_Project>> GetProjectListById(long companyId, long userId)
        {
            return base.Channel.GetProjectListById(companyId, userId);
        }

        /// <summary>
        /// 检查验收结果
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Result<int> CheckAcceptance(long projectId)
        {
            return base.Channel.CheckAcceptance(projectId);
        }

        public Result<List<Epm_ProjectCompany>> GetProjectCompanyList(long projectId)
        {
            return base.Channel.GetProjectCompanyList(projectId);
        }

        /// <summary>
        /// 获取工程服务商
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        public Result<List<Epm_ProjectCompany>> GetProjectCompanyByProjectId(long projectId)
        {
            return base.Channel.GetProjectCompanyByProjectId(projectId);
        }
        /// <summary>
        /// 更新工程服务商（服务商、合同、委托书）
        /// </summary>
        /// <param name="list"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Result<int> UpdateProjectCompany(List<Epm_ProjectCompany> list)
        {
            return base.Channel.UpdateProjectCompany(list);
        }
        /// <summary>
        /// 获取服务商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_ProjectCompany> GetProjectCompany(long id)
        {
            return base.Channel.GetProjectCompany(id);
        }
        /// <summary>
        /// 更新项目经理信息
        /// </summary>
        /// <param name="projectCompany"></param>
        /// <returns></returns>
        public Result<int> UpdatePMAndPhone(Epm_ProjectCompany projectCompany)
        {
            return base.Channel.UpdatePMAndPhone(projectCompany);
        }
        /// <summary>
        /// 审核项目经理信息
        /// </summary>
        /// <param name="projectCompany"></param>
        /// <returns></returns>
        public Result<int> AuditPMAndPhone(Epm_ProjectCompany projectCompany)
        {
            return base.Channel.AuditPMAndPhone(projectCompany);
        }
        /// <summary>
        /// 驳回项目经理信息
        /// </summary>
        /// <param name="projectCompany"></param>
        /// <returns></returns>
        public Result<int> RejectPMManAndPhone(Epm_ProjectCompany projectCompany)
        {
            return base.Channel.RejectPMManAndPhone(projectCompany);
        }

        ///// <summary>
        ///// 更新负责人信息
        ///// </summary>
        ///// <param name="projectCompany"></param>
        ///// <returns></returns>
        //public Result<int> UpdateLinkManAndPhone(Epm_ProjectCompany projectCompany)
        //{
        //    return base.Channel.UpdateLinkManAndPhone(projectCompany);
        //}
        ///// <summary>
        ///// 审核负责人信息
        ///// </summary>
        ///// <param name="projectCompany"></param>
        ///// <returns></returns>
        //public Result<int> AuditLinkManAndPhone(Epm_ProjectCompany projectCompany)
        //{
        //    return base.Channel.AuditLinkManAndPhone(projectCompany);
        //}
        ///// <summary>
        ///// 驳回负责人信息
        ///// </summary>
        ///// <param name="projectCompany"></param>
        ///// <returns></returns>
        //public Result<int> RejectLinkManAndPhone(Epm_ProjectCompany projectCompany)
        //{
        //    return base.Channel.RejectLinkManAndPhone(projectCompany);
        //}

        /// <summary>
        /// 获取工程内容要点
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        public Result<List<Epm_ProjectWorkMainPoints>> GetProjectPointsByProjectId(long projectId)
        {
            return Channel.GetProjectPointsByProjectId(projectId);
        }
        /// <summary>
        /// 修改工程内容要点
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public Result<int> UpdateProjectPoints(List<Epm_ProjectWorkMainPoints> list, long projectId)
        {
            return Channel.UpdateProjectPoints(list, projectId);
        }

        ///// <summary>
        ///// 新增工程内容要点
        ///// </summary>
        ///// <param name="list"></param>
        ///// <param name="projectId"></param>
        ///// <returns></returns>
        //public Result<int> AddProjectPoints(List<Epm_ProjectWorkMainPoints> list, long projectId)
        //{
        //    return Channel.AddProjectPoints(list, projectId);
        //}

        /// <summary>
        /// 获取项目工程内容要点历史
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        public Result<List<Epm_ProjectWorkMainPointsHistory>> GetProjectPointsHistoryByProjectId(long projectId)
        {
            return Channel.GetProjectPointsHistoryByProjectId(projectId);
        }

        /// <summary>
        /// 更新工期信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateTimelimit(Epm_Project model, List<Base_Files> attachs, bool isdelete = true)
        {
            return Channel.UpdateTimelimit(model, attachs, isdelete);
        }

        /// <summary>
        /// 获取项目资料
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Result<List<Epm_ProjectDataSubmit>> GetProjectSubmitByProjectId(long projectId)
        {
            return Channel.GetProjectSubmitByProjectId(projectId);
        }

        /// <summary>
        /// 更新项目资料
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="attachs"></param>
        /// <returns></returns>
        public Result<int> UpdateProjectSubmit(long projectId, List<Base_Files> attachs)
        {
            return Channel.UpdateProjectSubmit(projectId, attachs);
        }

        /// <summary>
        /// 获取项目总批复构成历史
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public DataTable GetConstituteHis(long projectId)
        {
            return Channel.GetConstituteHis(projectId);
        }

        /// <summary>
        /// 获取项目总批复构成历史
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public DataTable GetPointsHis(long projectId)
        {
            return Channel.GetPointsHis(projectId);
        }

        public DataTable GetdtUserList(long company)
        {
            return base.Channel.GetdtUserList(company);
        }
        public Result<List<Base_User>> GetUserByCompanyId(long companyId)
        {
            return base.Channel.GetUserByCompanyId(companyId);
        }
        /// <summary>
        /// 获取合同乙方单位
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Result<List<Epm_ProjectCompany>> GetProjectCompanyListByName(long projectId, string name)
        {
            return Channel.GetProjectCompanyListByName(projectId, name);
        }

        /// <summary>
        /// 根据项目Id获取工程内容要点列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Result<List<Epm_ProjectWorkMainPoints>> GetProjectPointsList(long projectId)
        {
            return Channel.GetProjectPointsList(projectId);
        }

        #region  问题管理
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddQuestion(QuestionView model)
        {
            return base.Channel.AddQuestion(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateQuestion(Epm_Question model)
        {
            return base.Channel.UpdateQuestion(model);
        }


        public Result<bool> DeleteQuestion(long id)
        {
            return Channel.DeleteQuestion(id);
        }


        ///<summary>
        ///获取列表: GetQuestionList
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_Question>> GetQuestionList(QueryCondition qc)
        {
            return base.Channel.GetQuestionList(qc);
        }
        public Result<List<Base_VideoManage>> GetBaseVideoManageLists(QueryCondition qc)
        {
            return base.Channel.GetBaseVideoManageLists(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<QuestionView> GetQuestionModel(long id)
        {
            return base.Channel.GetQuestionModel(id);
        }

        /// <summary>
        /// 回复问题
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<bool> ReplyQuestion(Epm_QuestionTrack model)
        {
            return Channel.ReplyQuestion(model);
        }

        /// <summary>
        /// 关闭问题
        /// </summary>
        /// <param name="id">问题 ID</param>
        /// <param name="isAccident">是否重大事故</param>
        /// <returns></returns>
        public Result<bool> CloseQuestion(long id, bool isAccident = false)
        {
            return Channel.CloseQuestion(id, isAccident);
        }


        /// <summary>
        /// 获取当前登录人问题
        /// </summary>
        /// <returns></returns>
        public Result<List<Epm_Question>> GetCurrUserQuestion(QueryCondition qc)
        {
            return Channel.GetCurrUserQuestion(qc);
        }

        /// <summary>
        /// 根据问题id和模型ID获取关联组件列表
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public Result<List<Epm_QuestionBIM>> GetComponentListByQuestionId(long questionId)
        {
            return Channel.GetComponentListByQuestionId(questionId);
        }

        /// <summary>
        /// 根据模型ID获取关联组件列表
        /// </summary>
        /// <param name="bimId"></param>
        /// <returns></returns>
        public Result<List<Epm_QuestionBIM>> GetComponentListByBimId(long bimId)
        {
            return Channel.GetComponentListByBimId(bimId);
        }
        /// <summary>
        /// 添加问题关联模型
        /// </summary>
        /// <param name="model"></param>
        /// <param name="componentIds"></param>
        /// <returns></returns>
        public Result<int> AddQuestionBIM(Epm_QuestionBIM model, string componentIds)
        {
            return Channel.AddQuestionBIM(model, componentIds);
        }

        /// <summary>
        /// 获取问题回复列表
        /// </summary>
        /// <param name="qc">查询条件</param>
        /// <returns></returns>
        public Result<List<Epm_QuestionTrack>> GetQuestionTrack(QueryCondition qc)
        {
            return Channel.GetQuestionTrack(qc);
        }

        #endregion

        #region 专项验收
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddSpecialAcceptance(SpecialAcceptanceView model)
        {
            return base.Channel.AddSpecialAcceptance(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateSpecialAcceptance(SpecialAcceptanceView model)
        {
            return base.Channel.UpdateSpecialAcceptance(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="id">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<bool> DeleteSpecialAcceptanceById(long id)
        {
            return base.Channel.DeleteSpecialAcceptanceById(id);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_SpecialAcceptance>> GetSpecialAcceptanceList(QueryCondition qc)
        {
            return base.Channel.GetSpecialAcceptanceList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<SpecialAcceptanceView> GetSpecialAcceptanceModel(long id)
        {
            return base.Channel.GetSpecialAcceptanceModel(id);
        }

        /// <summary>
        /// 专项验收审核
        /// </summary>
        /// <returns></returns>
        public Result<bool> AuditSpecialAccptance(SpecialAcceptanceView model)
        {
            return Channel.AuditSpecialAccptance(model);
        }

        /// <summary>
        /// 废弃专项验收
        /// </summary>
        ///<param name="id">要废弃的专项验收 ID</param>
        /// <returns></returns>
        public Result<bool> DiscardSpecialAccptance(long id)
        {
            return Channel.DiscardSpecialAccptance(id);
        }

        #endregion

        #region 监理日志管理

        /// <summary>
        /// 根据监理日志获取危险作业信息
        /// </summary>
        /// <param name="logId"></param>
        /// <returns></returns>
        public Result<List<Epm_DangerousWork>> GetDangerousWorkByLogId(long logId)
        {
            return base.Channel.GetDangerousWorkByLogId(logId);
        }

        /// <summary>
        /// 审核监理日志
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AuditSupervisorLog(Epm_SupervisorLog model)
        {
            return base.Channel.AuditSupervisorLog(model);
        }
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <param name="details">监理日志明细</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddSupervisorLog(Epm_SupervisorLog model, List<SupervisorLogDetailView> details)
        {
            return base.Channel.AddSupervisorLog(model, details);
        }

        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteSupervisorLogByIds(List<long> ids)
        {
            return base.Channel.DeleteSupervisorLogByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<SupervisorLogDetailView>> GetSupervisorLogList(QueryCondition qc)
        {
            return base.Channel.GetSupervisorLogList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<SupervisorLogDetailView> GetSupervisorLogModel(long id)
        {
            return base.Channel.GetSupervisorLogModel(id);
        }

        ///<summary>
        ///获取监理日志详情列表
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_SupervisorLogDetails>> GetSupervisorLogDetailsList(QueryCondition qc)
        {
            return base.Channel.GetSupervisorLogDetailsList(qc);
        }

        /// <summary>
        /// 新增监理日志
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddSupervisorLogNew(SupervisorLogView model, List<long> workIds)
        {
            return Channel.AddSupervisorLogNew(model, workIds);
        }
        /// <summary>
        /// 新增监理日志(新)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddProjectlLogList(SupervisorLogView model, List<long> workIds)
        {
            return Channel.AddProjectlLogList(model, workIds);
        }
        /// <summary>
        /// 删除监理日志
        /// </summary>
        /// <param name="id">监理日志 ID</param>
        /// <returns></returns>
        public Result<bool> DeleteSupervisorlogByIdNew(long id)
        {
            return Channel.DeleteSupervisorlogByIdNew(id);
        }

        /// <summary>
        /// 获取监理日志列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_SupervisorLog>> GetSupervisorLogListNew(QueryCondition qc)
        {
            return Channel.GetSupervisorLogListNew(qc);
        }

        /// <summary>
        /// 获取监理日志详情
        /// </summary>
        /// <param name="id">监理日志 ID</param>
        /// <param name="isLoadFile">是否同时获取资源，默认获取</param>
        /// <returns></returns>
        public Result<SupervisorLogView> GetSupervisorLogModelNew(long id, bool isLoadFile = true)
        {
            return Channel.GetSupervisorLogModelNew(id, isLoadFile);
        }


        #endregion

        #region 培训管理
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTrain(TrainView model, List<Base_Files> fileList)
        {
            return base.Channel.AddTrain(model, fileList);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTrain(TrainView model, List<Base_Files> fileList)
        {
            return base.Channel.UpdateTrain(model, fileList);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTrainByIds(List<long> ids)
        {
            return base.Channel.DeleteTrainByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<TrainView>> GetTrainList(QueryCondition qc)
        {
            return base.Channel.GetTrainList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<TrainView> GetTrainModel(long id)
        {
            return base.Channel.GetTrainModel(id);
        }
        public Result<int> UpdateTrainState(long id, string state)
        {
            return base.Channel.UpdateTrainState(id, state);
        }
        ///<summary>
        ///获取模板列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_Template>> GetTemplateList(QueryCondition qc)
        {
            return base.Channel.GetTemplateList(qc);
        }
        #endregion

        #region 签证管理
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddVisa(VisaView model, List<Base_Files> fileList)
        {
            return base.Channel.AddVisa(model, fileList);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateVisa(VisaView model, List<Base_Files> fileList)
        {
            return base.Channel.UpdateVisa(model, fileList);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteVisaByIds(List<long> ids)
        {
            return base.Channel.DeleteVisaByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_Visa>> GetVisaList(string projectName, string title, int state, string visaTypeName, int pageIndex, int pageSize)
        {
            return base.Channel.GetVisaList(projectName, title, state, visaTypeName, pageIndex, pageSize);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<VisaView> GetVisaModel(long id)
        {
            return base.Channel.GetVisaModel(id);
        }
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateVisaState(long id, string state)
        {
            return base.Channel.UpdateVisaState(id, state);
        }
        #endregion

        #region 用户登录

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">加密后的登录密码串</param>
        /// <returns></returns>
        public Result<UserView> Login(string userName, string password, string type)
        {
            return Channel.Login(userName, password, type);
        }

        /// <summary>
        /// 根据用户加载对应权限，有缓存
        /// </summary>
        /// <param name="roleType">角色类型</param>
        /// <param name="userId">用户 ID</param>
        /// <param name="listRight"></param>
        /// <returns></returns>
        public Result<List<Base_Right>> LoadRightList(string roleType, long userId = 0, List<long> listRight = null)
        {
            return Channel.LoadRightList(roleType, userId, listRight);
        }


        /// <summary>
        /// 获取网站设置
        /// </summary>
        /// <returns></returns>
        public Result<Base_Config> LoadConfig()
        {
            return Channel.LoadConfig();
        }

        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Base_User> GetUserModel(long id)
        {
            return Channel.GetUserModel(id);
        }

        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="token"></param>
        /// <param name="type">1:Android,2:IOS</param>
        /// <returns></returns>
        public Result<UserView> GetUserModelByToken(string token, int type)
        {
            return Channel.GetUserModelByToken(token, type);
        }
        public Result<Base_User> GetBaseUserByToken(string token, int type)
        {
            return Channel.GetBaseUserByToken(token, type);
        }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateUser(Base_User model)
        {
            return Channel.UpdateUser(model);
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="userName"></param>
        /// <param name="phone"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Result<List<UserView>> GetUserListByWhr(string type, string userName, string phone, int pageIndex, int pageSize)
        {
            return Channel.GetUserListByWhr(type, userName, phone, pageIndex, pageSize);
        }

        public Result<List<UserListView>> GetUserManageList(QueryCondition qc, string type)
        {
            return Channel.GetUserManageList(qc, type);
        }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddUserInfo(Base_User model, string image, List<Base_Files> fileList = null)
        {
            return Channel.AddUserInfo(model, image, fileList);
        }

        /// <summary>
        /// 批量添加用户信息
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public Result<int> AddRangeUser(List<Base_User> models)
        {
            return Channel.AddRangeUser(models);
        }

        /// <summary>
        /// 根据用户名称或者电话号码查询用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public Result<Base_User> GetUserInfoByNameAndPhone(string userName, string phone)
        {
            return Channel.GetUserInfoByNameAndPhone(userName, phone);
        }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateUserInfo(Base_User model, string image, List<Base_Files> fileList = null)
        {
            return Channel.UpdateUserInfo(model, image, fileList);
        }

        /// <summary>
        /// 批量删除用户
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteUserByIds(List<long> ids)
        {
            return Channel.DeleteUserByIds(ids);
        }

        /// <summary>
        /// 查询用户详情
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Result<Base_User> GetUserDetail(long userId)
        {
            return Channel.GetUserDetail(userId);
        }

        /// <summary>
        /// 是否工程处用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsAgencyUser(long userId)
        {
            return Channel.IsAgencyUser(userId);
        }
        /// <summary>
        /// 获取分公司项目负责人
        /// </summary>L
        /// <param name="name"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Result<List<Base_User>> GetBrCompanyPMList(string name, long companyId, int pageIndex, int pageSize)
        {
            return Channel.GetBrCompanyPMList(name, companyId, pageIndex, pageSize);
        }
        /// <summary>
        /// 获取工程处项目经理
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Result<List<Base_User>> GetAgencyPMList(string name, int pageIndex, int pageSize)
        {
            return Channel.GetAgencyPMList(name, pageIndex, pageSize);
        }

        /// <summary>
        /// 是否服务商用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsServiceUser(long userId)
        {
            return Channel.IsServiceUser(userId);
        }

        /// <summary>
        /// 是否分公司用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsBranchCompanyUser(long userId)
        {
            return Channel.IsBranchCompanyUser(userId);
        }
        /// <summary>
        /// 是否分公司部门经理
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsBranchCompanyDirector(long userId)
        {
            return Channel.IsBranchCompanyDirector(userId);
        }
        /// <summary>
        /// 是否监理
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsSupervisor(long projectId, long userId)
        {
            return Channel.IsSupervisor(projectId, userId);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="oldPwd">加密之后的原旧密码</param>
        /// <param name="pwd">加密之后的密码</param>
        /// <returns></returns>
        public Result<bool> UpdatePassword(long userId, string oldPwd, string pwd)
        {
            return Channel.UpdatePassword(userId, oldPwd, pwd);
        }

        /// <summary>
        /// 获取消息列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Msg_Message>> GetMessageList(QueryCondition qc)
        {
            return Channel.GetMessageList(qc);
        }
        #endregion

        #region 企业管理

        /// <summary>
        /// 获取企业列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Company>> GetCompanyList(QueryCondition qc)
        {
            return Channel.GetCompanyList(qc);
        }

        #region 三商管理
        /// <summary>
        /// 根据类型获取对应的企业列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Company>> GetCompanyListByType(QueryCondition qc)
        {
            return Channel.GetCompanyListByType(qc);
        }
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddCompany(Base_Company model)
        {
            return base.Channel.AddCompany(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateCompany(Base_Company model)
        {
            return base.Channel.UpdateCompany(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteCompanyByIds(List<long> ids)
        {
            return base.Channel.DeleteCompanyByIds(ids);
        }


        #endregion

        /// <summary>
        /// 获取地市公司
        /// </summary>
        /// <returns></returns>
        public Result<List<Base_Company>> GetAreaCompanyList()
        {
            return Channel.GetAreaCompanyList();
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_User>> GetUserList(QueryCondition qc)
        {
            return Channel.GetUserList(qc);
        }

        /// <summary>
        /// 根据企业Id获取用户列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<List<Base_User>> GetUserPostList(long id)
        {
            return Channel.GetUserPostList(id);
        }

        /// <summary>
        /// 根据单位ID和岗位名称获取人员信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="postName"></param>
        /// <returns></returns>
        public Result<List<Base_User>> GetUserListByPost(long id, string postName)
        {
            return Channel.GetUserListByPost(id, postName);
        }

        /// <summary>
        /// 获取用户信息（包含部门信息）
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<UserView>> GetUserListByDepartment(QueryCondition qc)
        {
            return Channel.GetUserListByDepartment(qc);
        }
        /// <summary>
        /// 根据企业 ID 获取企业详情
        /// </summary>
        /// <param name="id">企业 ID</param>
        public Result<CompanyView> GetCompanyModel(long id)
        {
            return Channel.GetCompanyModel(id);
        }

        /// <summary>
        /// 获取总批复及构成
        /// </summary>
        /// <param name="code">项目性质编号</param>
        /// <returns></returns>
        public Result<List<Epm_ProjectNature>> GetProjectNature()
        {
            return Channel.GetProjectNature();
        }

        /// <summary>
        /// 获取项目总批复及构成
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        public Result<List<Epm_ProjectConstitute>> GetProjectConstituteByProjectId(long projectId)
        {
            return Channel.GetProjectConstituteByProjectId(projectId);
        }

        /// <summary>
        /// 修改项目总批复及构成
        /// </summary>
        /// <param name="list">总批复及构成集合</param>
        /// <param name="projectId">项目Id</param>
        /// <param name="bluePrintCode">方案类型Key</param>
        /// <param name="bluePrintName">方案类型Value</param>
        /// <param name="isCrossings">是否外部手续bool</param>
        public Result<int> UpdateProjectConstitute(Epm_Project project, List<Epm_ProjectConstitute> list, List<Base_Files> attachs)
        {
            return Channel.UpdateProjectConstitute(project, list, attachs);
        }

        /// <summary>
        /// 获取项目总批复及构成历史
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <returns></returns>
        public Result<List<Epm_ProjectConstituteHistory>> GetProjectConstituteHistoryByProjectId(long projectId)
        {
            return Channel.GetProjectConstituteHistoryByProjectId(projectId);
        }

        #endregion

        #region 区域
        /// <summary>
        /// 获取区域列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Region>> GetRegionList(QueryCondition qc)
        {
            return base.Channel.GetRegionList(qc);
        }
        /// <summary>
        /// 加载区域列表，有缓存
        /// </summary>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        public Result<List<Base_Region>> LoadRegionList(string parentCode = "")
        {
            return base.Channel.LoadRegionList(parentCode);
        }

        /// <summary>
        /// 获取区域详情
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Result<Base_Region> GetRegionModel(string code)
        {
            return base.Channel.GetRegionModel(code);
        }

        /// <summary>
        /// 根据字典类型集合获取字典数据
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public Result<Dictionary<DictionaryType, List<Base_TypeDictionary>>> GetTypeListByTypes(List<DictionaryType> types)
        {
            return base.Channel.GetTypeListByTypes(types);
        }
        #endregion

        #region  加油站信息
        ///<summary>
        ///获取列表:加油站信息表
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_OilStation>> GetOilStationList(QueryCondition qc)
        {
            return Channel.GetOilStationList(qc);
        }

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<List<Epm_OilStation>> GetOilStationAllList()
        {
            return Channel.GetOilStationAllList();
        }
        #endregion

        #region File
        /// <summary>
        /// 根据表名和id获取所有附件
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<List<Base_Files>> GetFilesByTable(string tableName, long id)
        {
            return base.Channel.GetFilesByTable(tableName, id);
        }
        /// <summary>
        /// 根据表名和id获取所有附件
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<List<Base_Files>> GetFileListByTableIds(string tableName, List<long> ids)
        {
            return base.Channel.GetFileListByTableIds(tableName, ids);
        }
        /// <summary>
        /// 根据Epm_TzAttachs表id获取附件
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<Epm_TzAttachs> GetFileByTzAttachsId(long id)
        {
            return base.Channel.GetFileByTzAttachsId(id);
        }
        #endregion

        #region 面包屑导航


        /// <summary>
        /// 获取面包屑导航
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public Result<List<Base_Right>> GetBreadcrumbNavigation(string controllerName, string actionName)
        {
            return Channel.GetBreadcrumbNavigation(controllerName, actionName);
        }



        #endregion

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TemplateDetails>> GetTemplateDetailsList(long templateId)
        {
            return Channel.GetTemplateDetailsList(templateId);
        }

        /// <summary>
        /// 根据模板编码获取模板列表
        /// </summary>
        /// <param name="templateNo"></param>
        /// <returns></returns>
        public Result<List<Epm_Template>> GetTemplateListByNo(string templateNo)
        {
            return Channel.GetTemplateListByNo(templateNo);
        }

        /// <summary>
        /// 获取字典Id
        /// </summary>
        /// <param name="dicNo"></param>
        /// <returns></returns>
        public Result<Base_TypeDictionary> GetDictionaryId(string dicNo)
        {
            return Channel.GetDictionaryId(dicNo);
        }

        /// <summary>
        /// 根据字典id获取模板列表
        /// </summary>
        /// <param name="dicId"></param>
        /// <returns></returns>
        public Result<List<Epm_Template>> GetTemplateListDicId(long dicId, string title)
        {
            return Channel.GetTemplateListDicId(dicId, title);
        }

        /// <summary>
        /// 根据父级ID获取检查项
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public Result<List<Epm_CheckItem>> GetCheckItemList(long pid)
        {
            return Channel.GetCheckItemList(pid);
        }

        public Result<List<Epm_CheckItem>> GetCheckItemAll()
        {
            return Channel.GetCheckItemAll();
        }

        public Result<List<Epm_CheckItem>> GetCheckItemListByTypeName(string type, string name, int level)
        {
            return Channel.GetCheckItemListByTypeName(type, name, level);
        }
        /// <summary>
        /// 获取检查项树形列表数据
        /// </summary>
        /// <returns></returns>
        public Result<List<CheckItemView>> GetCheckItem(long projectid, long userid)
        {
            return Channel.GetCheckItem(projectid, userid);
        }

        /// <summary>
        /// 根据检查角色类型获取检查数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Result<List<Epm_CheckItem>> GetCheckItemListByType(string type)
        {
            return Channel.GetCheckItemListByType(type);
        }
        /// <summary>
        /// 根据广告位编码获取广告投放记录
        /// </summary>
        /// <param name="targetNum"></param>
        /// <returns></returns>
        public Result<List<Base_Files>> GetAdPutRecord(string targetNum, string imageType)
        {
            return Channel.GetAdPutRecord(targetNum, imageType);
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

        public Result<List<Base_Files>> GetFilesByTableName(string tableName, string name)
        {
            return base.Channel.GetFilesByTableName(tableName, name);
        }

        #region 延期申请

        /// <summary>
        /// 获取延期申请列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_PlanDelay>> GetPlanDelayList(QueryCondition qc)
        {
            return Channel.GetPlanDelayList(qc);
        }

        /// <summary>
        /// 新增延期申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<bool> AddPlanDelay(PlanDelayView model)
        {
            return Channel.AddPlanDelay(model);
        }

        /// <summary>
        /// 修改延期申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<bool> ModifyPlanDelay(PlanDelayView model)
        {
            return Channel.ModifyPlanDelay(model);
        }

        /// <summary>
        /// 审核延期申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AuditPlanDelay(Epm_PlanDelay model)
        {
            return Channel.AuditPlanDelay(model);
        }

        /// <summary>
        /// 删除延期申请
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<bool> DeletePlanDelay(long id)
        {
            return Channel.DeletePlanDelay(id);
        }

        /// <summary>
        /// 获取延期申请详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<PlanDelayView> GetPlanDelayInfo(long id)
        {
            return Channel.GetPlanDelayInfo(id);
        }

        #endregion


        #region 作业实景

        /// <summary>
        /// 获取作业实景列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_WorkUploadRealScene>> GetWorkRealSceneList(QueryCondition qc)
        {
            return Channel.GetWorkRealSceneList(qc);
        }

        /// <summary>
        /// 上传危险作业实景
        /// </summary>
        /// <param name="model">作业实景</param>
        /// <param name="files">相关附件</param>
        /// <returns></returns>
        public Result<bool> AddWorkRealScenen(Epm_WorkUploadRealScene model, List<Base_Files> files)
        {
            return Channel.AddWorkRealScenen(model, files);
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateWorkRealScenenState(long id, ApprovalState state)
        {
            return Channel.UpdateWorkRealScenenState(id, state);
        }

        /// <summary>
        /// 删除上传作业实景
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<int> DeleteWorkRealScenen(long id)
        {
            return Channel.DeleteWorkRealScenen(id);
        }

        /// <summary>
        /// 根据监理日志 ID 获取危险作业实景
        /// </summary>
        /// <param name="logId">监理日志 ID</param>
        /// <returns></returns>
        public Result<List<Epm_WorkUploadRealScene>> GetWorkRealSceneByLogId(long logId)
        {
            return Channel.GetWorkRealSceneByLogId(logId);
        }


        #endregion

        #region 消息信息
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddMassage(Epm_Massage model)
        {
            return base.Channel.AddMassage(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateMassage(Epm_Massage model)
        {
            return base.Channel.UpdateMassage(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteMassageByIds(List<long> ids)
        {
            return base.Channel.DeleteMassageByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_Massage>> GetMassageList(QueryCondition qc)
        {
            return base.Channel.GetMassageList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_Massage> GetMassageModel(long id)
        {
            return base.Channel.GetMassageModel(id);
        }

        /// <summary>
        /// 更新所有消息状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateAllMassageState(long recId, bool state)
        {
            return base.Channel.UpdateAllMassageState(recId, state);
        }
        #endregion

        #region  APP 专用接口

        /// <summary>
        /// 获取沟通列表集合
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<QuestionModel>> GetQuestions(QueryCondition qc)
        {
            return Channel.GetQuestions(qc);
        }


        /// <summary>
        /// 获取登录用户待办事项总数和未读消息总数
        /// </summary>
        /// <returns></returns>
        public Result<Dictionary<string, int>> GetApproverCount(long UserId)
        {
            return Channel.GetApproverCount(UserId);
        }

        /// <summary>
        /// 获取可展示广告列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_AdPutRecord>> GetAdShowList(QueryCondition qc)
        {
            return Channel.GetAdShowList(qc);
        }

        /// <summary>
        /// 获取指定数据的附件
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        public Result<List<Base_Files>> GetBaseFiles(List<long> tableId)
        {
            return Channel.GetBaseFiles(tableId);
        }

        public Result<Base_Files> GetBaseFile(long id)
        {
            return Channel.GetBaseFile(id);
        }

        /// <summary>
        /// 根据guid查询图片附件
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public Result<List<Base_Files>> GetBaseFileByGuid(string guid)
        {
            return Channel.GetBaseFileByGuid(guid);
        }

        /// <summary>
        /// 添加附件
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList"></param>
        /// <param name="isdelete"></param>
        /// <returns></returns>
        public Result<List<Base_Files>> AddFilesByTables(string model, List<Base_Files> fileList, bool isdelete)
        {
            return Channel.AddFilesByTables(model, fileList, isdelete);
        }
        /// <summary>
        /// 获取用户头像链接
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Result<Dictionary<long, string>> GetUserProfilePhoto(List<long> userId)
        {
            return Channel.GetUserProfilePhoto(userId);
        }

        /// <summary>
        /// 获取热门问题
        /// </summary>
        /// <returns></returns>
        public Result<QuestionModel> GetHotQuestion()
        {
            return Channel.GetHotQuestion();
        }


        /// <summary>
        /// 获取问题回复数
        /// </summary>
        /// <param name="questionIds">问题 ID 集合</param>
        /// <returns></returns>
        public Result<Dictionary<long, int>> GetQuestionTrackCount(List<long> questionIds)
        {
            return Channel.GetQuestionTrackCount(questionIds);
        }

        /// <summary>
        /// 获取业务相关问题数
        /// </summary>
        /// <param name="businessIds">业务 ID</param>
        /// <returns></returns>
        public Result<Dictionary<long, int>> GetQuestionCount(List<long> businessIds)
        {
            return Channel.GetQuestionCount(businessIds);
        }


        /// <summary>
        /// 根据项目 ID 获取项目相关变更
        /// </summary>
        /// <param name="projectId">项目 ID</param>
        /// <returns></returns>
        public Result<List<Epm_Change>> GetChangeByProjectId(long projectId)
        {
            return Channel.GetChangeByProjectId(projectId);
        }



        /// <summary>
        /// 获取签证列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_Visa>> GetVisaListByQc(QueryCondition qc)
        {
            return Channel.GetVisaListByQc(qc);
        }



        /// <summary>
        /// 获取 APP 最新版本
        /// </summary>
        /// <param name="appNum">APP 包名</param>
        /// <returns></returns>
        public Result<Epm_AppVersion> GetAppVersion(string appNum)
        {
            return Channel.GetAppVersion(appNum);
        }


        ///// <summary>
        ///// 根据 ID 修改工程内容要点
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public Result<bool> UpdateProjectWorkPointById(Epm_ProjectWorkMainPoints model)
        //{
        //    return Channel.UpdateProjectWorkPointById(model);
        //}


        /// <summary>
        /// 修改项目供应商负责人及项目经理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<bool> UpdateProjectCompanyPmInfo(Epm_ProjectCompany model, long userId)
        {
            return Channel.UpdateProjectCompanyPmInfo(model, userId);
        }

        /// <summary>
        /// 审核、驳回服务商PM和负责人
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<bool> AuditProjectCompanyPmAndLink(long Id, ApprovalState state)
        {
            return Channel.AuditProjectCompanyPmAndLink(Id, state);
        }

        ///// <summary>
        ///// 新增工程内容要点
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public Result<bool> AddProjectWorkPoint(Epm_ProjectWorkMainPoints model)
        //{
        //    return Channel.AddProjectWorkPoint(model);
        //}

        #endregion

        /// <summary>
        /// 获取当前年份项目KPI数据
        /// </summary>
        /// <returns></returns>
        public Result<Epm_ProjectKPI> GetProjectKPIList()
        {
            return Channel.GetProjectKPIList();
        }

        /// <summary>
        /// 获取项目KPI数据
        /// </summary>
        /// <param name="years"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Result<ProjectKPIView> GetProjectKPIListByWhr(string years, long userId)
        {
            return Channel.GetProjectKPIListByWhr(years, userId);
        }

        public Result<int> AddSendDate(Bp_SendDate model)
        {
            return Channel.AddSendDate(model);
        }

        /// <summary>
        /// 检查列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<InspectView>> GetInspectList(QueryCondition qc)
        {
            return Channel.GetInspectList(qc);
        }

        /// <summary>
        /// 得失分明细列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<InspectView>> GetInspectItemListByQc(QueryCondition qc)
        {
            return Channel.GetInspectItemListByQc(qc);
        }

        /// <summary>
        /// 人脸注册
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="image"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public Result<int> AddAIUserFaceInfo(long userId, string image, string source)
        {
            return base.Channel.AddAIUserFaceInfo(userId, image, source);
        }

        /// <summary>
        /// 人脸搜索
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> SearchUserFace(SignFaceAI model)
        {
            return base.Channel.SearchUserFace(model);
        }

        /// <summary>
        /// 根据用户ID获取该用户注册的人脸信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Result<EPM_AIUserFace> GetAIUserFace(long userId)
        {
            return base.Channel.GetAIUserFace(userId);
        }
        #region 考勤信息
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_SignInformation>> GetSignInformationList(QueryCondition qc)
        {
            return base.Channel.GetSignInformationList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_SignInformation> GetSignInformationModel(long id)
        {
            return base.Channel.GetSignInformationModel(id);
        }
        #endregion

        #region 项目试运行申请
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddProjectApproval(ProjectApprovalView model)
        {
            return base.Channel.AddProjectApproval(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateProjectApproval(Epm_TzProjectPolit model)
        {
            return base.Channel.UpdateProjectApproval(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteProjectApprovalByIds(List<long> ids)
        {
            return base.Channel.DeleteProjectApprovalByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectPolit>> GetProjectApprovalList(QueryCondition qc)
        {
            return base.Channel.GetProjectApprovalList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<ProjectApprovalView> GetProjectApprovalModel(long id)
        {
            return base.Channel.GetProjectApprovalModel(id);
        }

        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzProjectPolit(Epm_TzProjectPolit model)
        {
            return base.Channel.UpdateTzProjectPolit(model);
        }

        public Result<int> UpdateTzProjectPolitState(List<long> ids, string state)
        {
            return base.Channel.UpdateTzProjectPolitState(ids, state);
        }


        /// <summary>
        /// 新增审核记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<bool> AddProjectAuditRecord(Epm_ProjectAuditRecord model)
        {
            return base.Channel.AddProjectAuditRecord(model);
        }
        #endregion


        /// <summary>
        /// 根据 Cas 登录后的用户账号获取用户信息
        /// </summary>
        /// <param name="userName">用户账户</param>
        /// <returns></returns>
        public Result<UserView> LoginByCas(string userName)
        {
            return Channel.LoginByCas(userName);
        }

        #region 防渗改造投资

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddReformRecord(ReformRecordView model)
        {
            return base.Channel.AddReformRecord(model);
        }
        public Result<int> AddReformRecordeEtity(Epm_ReformRecord model)
        {
            return base.Channel.AddReformRecordeEtity(model);
        }

        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateReformRecord(Epm_ReformRecord model)
        {
            return base.Channel.UpdateReformRecord(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteReformRecordByIds(List<long> ids)
        {
            return base.Channel.DeleteReformRecordByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_ReformRecord>> GetReformRecordList(QueryCondition qc)
        {
            return base.Channel.GetReformRecordList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<ReformRecordView> GetReformRecordModel(long id)
        {
            return base.Channel.GetReformRecordModel(id);
        }

        public Result<Epm_ReformRecord> GetReformRecordEntity(long id)
        {
            return base.Channel.GetReformRecordEntity(id);
        }

        /// <summary>
        /// 修改项目状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateReformRecordState(List<long> ids, string state)
        {
            return base.Channel.UpdateReformRecordState(ids, state);
        }
        #endregion



        #region 编码映射

        /// <summary>
        /// 获取编码映射结果
        /// </summary>
        /// <param name="codeType">编码映射类型</param>
        /// <param name="mapType">映射系统</param>
        /// <param name="fromCode">源编码</param>
        /// <returns></returns>
        public Result<Epm_TzCodeMap> GetCodeMap(string codeType, string mapType, string fromCode)
        {
            return Channel.GetCodeMap(codeType, mapType, fromCode);
        }

        #endregion

        #region 项目提出前



        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzProjectProposal(Epm_TzProjectProposal model)
        {
            return base.Channel.AddTzProjectProposal(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzProjectProposal(Epm_TzProjectProposal model)
        {
            return base.Channel.UpdateTzProjectProposal(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzProjectProposalByIds(List<long> ids)
        {
            return base.Channel.DeleteTzProjectProposalByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectProposal>> GetTzProjectProposalList(QueryCondition qc)
        {
            return base.Channel.GetTzProjectProposalList(qc);
        }

        /// <summary>
        /// 获取项目进度信息列表
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public Result<List<TzProjectScheduleView>> GetTzProjectScheduleList(QueryCondition qc)
        {
            return base.Channel.GetTzProjectScheduleList(qc);
        }

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectProposal>> GetSingleTzProjectProposalList(QueryCondition qc)
        {
            return base.Channel.GetSingleTzProjectProposalList(qc);
        }

        /// <summary>
        /// 获取项目批复通过并且批复号不为空的项目
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_TzProjectProposal>> GetProjectProposalList(QueryCondition qc)
        {
            return base.Channel.GetProjectProposalList(qc);
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzProjectProposal> GetTzProjectProposalModel(long id)
        {
            return base.Channel.GetTzProjectProposalModel(id);
        }

        /// <summary>
        /// 获取项目详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<TzProjectProposalInfoView> GetTzProjectProposalALL(long id)
        {
            return base.Channel.GetTzProjectProposalALL(id);
        }

        /// <summary>
        /// 修改项目状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzProjectProposalState(List<long> ids, string state)
        {
            return base.Channel.UpdateTzProjectProposalState(ids, state);
        }

        /// <summary>
        /// 关闭项目
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Result<int> CloseTzProjectProposal(long projectId)
        {
            return base.Channel.CloseTzProjectProposal(projectId);
        }
        #endregion

        #region 现场调研、初次谈判

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzInitialTalk(Epm_TzInitialTalk model)
        {
            return base.Channel.AddTzInitialTalk(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzInitialTalk(Epm_TzInitialTalk model)
        {
            return base.Channel.UpdateTzInitialTalk(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzInitialTalkByIds(List<long> ids)
        {
            return base.Channel.DeleteTzInitialTalkByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzInitialTalk>> GetTzInitialTalkList(QueryCondition qc)
        {
            return base.Channel.GetTzInitialTalkList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzInitialTalk> GetTzInitialTalkModel(long id)
        {
            return base.Channel.GetTzInitialTalkModel(id);
        }
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzSiteSurvey(TzResearchView model)
        {
            return base.Channel.AddTzSiteSurvey(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzSiteSurvey(Epm_TzSiteSurvey model)
        {
            return base.Channel.UpdateTzSiteSurvey(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzSiteSurveyByIds(List<long> ids)
        {
            return base.Channel.DeleteTzSiteSurveyByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzSiteSurvey>> GetTzSiteSurveyList(QueryCondition qc)
        {
            return base.Channel.GetTzSiteSurveyList(qc);
        }

        /// <summary>
        /// 初次谈判列表查询（连项目前期信息表）
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<TzProjectProposalView>> GetTzInitialTalkAndProjectList(QueryCondition qc)
        {
            return base.Channel.GetTzInitialTalkAndProjectList(qc);
        }

        /// <summary>
        /// 现场调研列表查询（连项目前期信息表）
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<TzProjectProposalView>> GetTzSiteSurveyAndProjectList(QueryCondition qc)
        {
            return base.Channel.GetTzSiteSurveyAndProjectList(qc);
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzSiteSurvey> GetTzSiteSurveyModel(long id)
        {
            return base.Channel.GetTzSiteSurveyModel(id);
        }

        /// <summary>
        /// 获取现场勘查列表
        /// </summary>
        /// <returns></returns>
        public Result<List<Epm_TzProjectProposal>> GetTzResearchList(QueryCondition qc)
        {
            return base.Channel.GetTzResearchList(qc);
        }
        /// <summary>
        /// 根据项目Id获取现场勘探和项目信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public Result<TzResearchAllView> GetTzSiteSurveyProject(long projectId)
        {
            return base.Channel.GetTzSiteSurveyProject(projectId);
        }
        #endregion


        #region 土地协议出让谈判信息

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzLandTalk(Epm_TzLandTalk model)
        {
            return base.Channel.AddTzLandTalk(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzLandTalk(Epm_TzLandTalk model)
        {
            return base.Channel.UpdateTzLandTalk(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzLandTalkByIds(List<long> ids)
        {
            return base.Channel.DeleteTzLandTalkByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzLandTalk>> GetTzLandTalkList(QueryCondition qc)
        {
            return base.Channel.GetTzLandTalkList(qc);
        }

        /// <summary>
        /// 土地谈判协议列表查询（连项目前期信息表）
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<TzProjectProposalView>> GetTzLandTalkAndProjectList(QueryCondition qc)
        {
            return base.Channel.GetTzLandTalkAndProjectList(qc);
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzLandTalk> GetTzLandTalkModel(long id)
        {
            return base.Channel.GetTzLandTalkModel(id);
        }


        #endregion

        #region 组织评审材料、省投资处业务管理员审核、评审材料审核、评审记录、投资部门确认、评审会签记录、省公司领导签发


        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzFormTalkFile(Epm_TzFormTalkFile model)
        {
            return base.Channel.AddTzFormTalkFile(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzFormTalkFile(Epm_TzFormTalkFile model)
        {
            return base.Channel.UpdateTzFormTalkFile(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzFormTalkFileByIds(List<long> ids)
        {
            return base.Channel.DeleteTzFormTalkFileByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectProposal>> GetTzFormTalkFileList(QueryCondition qc)
        {
            return base.Channel.GetTzFormTalkFileList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<TzFormTalkFileView> GetTzFormTalkFileModel(long id)
        {
            return base.Channel.GetTzFormTalkFileModel(id);
        }

        /// <summary>
        /// 修改评审材料上报状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzFormTalkFileState(List<long> ids, string state)
        {
            return base.Channel.UpdateTzFormTalkFileState(ids, state);
        }
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzTalkFileAudit(Epm_TzTalkFileAudit model)
        {
            return base.Channel.AddTzTalkFileAudit(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzTalkFileAudit(Epm_TzTalkFileAudit model)
        {
            return base.Channel.UpdateTzTalkFileAudit(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzTalkFileAuditByIds(List<long> ids)
        {
            return base.Channel.DeleteTzTalkFileAuditByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzTalkFileAudit>> GetTzTalkFileAuditList(QueryCondition qc)
        {
            return base.Channel.GetTzTalkFileAuditList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzTalkFileAudit> GetTzTalkFileAuditModel(long id)
        {
            return base.Channel.GetTzTalkFileAuditModel(id);
        }
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzTalkFileHeadAudit(Epm_TzTalkFileHeadAudit model)
        {
            return base.Channel.AddTzTalkFileHeadAudit(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzTalkFileHeadAudit(Epm_TzTalkFileHeadAudit model)
        {
            return base.Channel.UpdateTzTalkFileHeadAudit(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzTalkFileHeadAuditByIds(List<long> ids)
        {
            return base.Channel.DeleteTzTalkFileHeadAuditByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzTalkFileHeadAudit>> GetTzTalkFileHeadAuditList(QueryCondition qc)
        {
            return base.Channel.GetTzTalkFileHeadAuditList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzTalkFileHeadAudit> GetTzTalkFileHeadAuditModel(long id)
        {
            return base.Channel.GetTzTalkFileHeadAuditModel(id);
        }
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzTalkLeaderSign(Epm_TzTalkLeaderSign model)
        {
            return base.Channel.AddTzTalkLeaderSign(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzTalkLeaderSign(Epm_TzTalkLeaderSign model)
        {
            return base.Channel.UpdateTzTalkLeaderSign(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzTalkLeaderSignByIds(List<long> ids)
        {
            return base.Channel.DeleteTzTalkLeaderSignByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzTalkLeaderSign>> GetTzTalkLeaderSignList(QueryCondition qc)
        {
            return base.Channel.GetTzTalkLeaderSignList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzTalkLeaderSign> GetTzTalkLeaderSignModel(long id)
        {
            return base.Channel.GetTzTalkLeaderSignModel(id);
        }
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzTalkRecord(Epm_TzTalkRecord model)
        {
            return base.Channel.AddTzTalkRecord(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzTalkRecord(Epm_TzTalkRecord model)
        {
            return base.Channel.UpdateTzTalkRecord(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzTalkRecordByIds(List<long> ids)
        {
            return base.Channel.DeleteTzTalkRecordByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzTalkRecord>> GetTzTalkRecordList(QueryCondition qc)
        {
            return base.Channel.GetTzTalkRecordList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzTalkRecord> GetTzTalkRecordModel(long id)
        {
            return base.Channel.GetTzTalkRecordModel(id);
        }
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzTalkRecordConfirm(Epm_TzTalkRecordConfirm model)
        {
            return base.Channel.AddTzTalkRecordConfirm(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzTalkRecordConfirm(Epm_TzTalkRecordConfirm model)
        {
            return base.Channel.UpdateTzTalkRecordConfirm(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzTalkRecordConfirmByIds(List<long> ids)
        {
            return base.Channel.DeleteTzTalkRecordConfirmByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzTalkRecordConfirm>> GetTzTalkRecordConfirmList(QueryCondition qc)
        {
            return base.Channel.GetTzTalkRecordConfirmList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzTalkRecordConfirm> GetTzTalkRecordConfirmModel(long id)
        {
            return base.Channel.GetTzTalkRecordConfirmModel(id);
        }
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzTalkSign(Epm_TzTalkSign model)
        {
            return base.Channel.AddTzTalkSign(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzTalkSign(Epm_TzTalkSign model)
        {
            return base.Channel.UpdateTzTalkSign(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzTalkSignByIds(List<long> ids)
        {
            return base.Channel.DeleteTzTalkSignByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzTalkSign>> GetTzTalkSignList(QueryCondition qc)
        {
            return base.Channel.GetTzTalkSignList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzTalkSign> GetTzTalkSignModel(long id)
        {
            return base.Channel.GetTzTalkSignModel(id);
        }


        #endregion


        #region 项目批复请示、二次、三次、四次。。。谈判、二次、三次、四次。。。谈判审核


        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzProjectApproval(Epm_TzProjectApproval model)
        {
            return base.Channel.AddTzProjectApproval(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzProjectApproval(Epm_TzProjectApproval model)
        {
            return base.Channel.UpdateTzProjectApproval(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzProjectApprovalByIds(List<long> ids)
        {
            return base.Channel.DeleteTzProjectApprovalByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectApproval>> GetTzProjectApprovalList(QueryCondition qc)
        {
            return base.Channel.GetTzProjectApprovalList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzProjectApproval> GetTzProjectApprovalModel(long id)
        {
            return base.Channel.GetTzProjectApprovalModel(id);
        }
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzSecondTakl(Epm_TzSecondTakl model)
        {
            return base.Channel.AddTzSecondTakl(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzSecondTakl(Epm_TzSecondTakl model)
        {
            return base.Channel.UpdateTzSecondTakl(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzSecondTaklByIds(List<long> ids)
        {
            return base.Channel.DeleteTzSecondTaklByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzSecondTakl>> GetTzSecondTaklList(QueryCondition qc)
        {
            return base.Channel.GetTzSecondTaklList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzSecondTakl> GetTzSecondTaklModel(long id)
        {
            return base.Channel.GetTzSecondTaklModel(id);
        }
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzSecondTalkAudit(Epm_TzSecondTalkAudit model)
        {
            return base.Channel.AddTzSecondTalkAudit(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzSecondTalkAudit(Epm_TzSecondTalkAudit model)
        {
            return base.Channel.UpdateTzSecondTalkAudit(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzSecondTalkAuditByIds(List<long> ids)
        {
            return base.Channel.DeleteTzSecondTalkAuditByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzSecondTalkAudit>> GetTzSecondTalkAuditList(QueryCondition qc)
        {
            return base.Channel.GetTzSecondTalkAuditList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzSecondTalkAudit> GetTzSecondTalkAuditModel(long id)
        {
            return base.Channel.GetTzSecondTalkAuditModel(id);
        }


        #endregion

        #region  工程建设项目开工报告、建设工程设计变更申请

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzDesiginChangeApply(Epm_TzDesiginChangeApply model)
        {
            return base.Channel.AddTzDesiginChangeApply(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzDesiginChangeApply(Epm_TzDesiginChangeApply model)
        {
            return base.Channel.UpdateTzDesiginChangeApply(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzDesiginChangeApplyByIds(List<long> ids)
        {
            return base.Channel.DeleteTzDesiginChangeApplyByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzDesiginChangeApply>> GetTzDesiginChangeApplyList(QueryCondition qc)
        {
            return base.Channel.GetTzDesiginChangeApplyList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzDesiginChangeApply> GetTzDesiginChangeApplyModel(long id)
        {
            return base.Channel.GetTzDesiginChangeApplyModel(id);
        }

        /// <summary>
        /// 修改设计方案变更申请状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzDesiginChangeApplyState(List<long> ids, string state)
        {
            return base.Channel.UpdateTzDesiginChangeApplyState(ids, state);
        }
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzStartsApply(Epm_TzStartsApply model)
        {
            return base.Channel.AddTzStartsApply(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzStartsApply(Epm_TzStartsApply model)
        {
            return base.Channel.UpdateTzStartsApply(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzStartsApplyByIds(List<long> ids)
        {
            return base.Channel.DeleteTzStartsApplyByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzStartsApply>> GetTzStartsApplyList(QueryCondition qc)
        {
            return base.Channel.GetTzStartsApplyList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzStartsApply> GetTzStartsApplyModel(long id)
        {
            return base.Channel.GetTzStartsApplyModel(id);
        }

        #endregion


        #region 陕西省各竞争对手加油（气）站现状上报流程   建设工程项目管理人员变更申请流程  加油（气）站开发资源上报流程
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzDevResourceReport(Epm_TzDevResourceReport model)
        {
            return base.Channel.AddTzDevResourceReport(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzDevResourceReport(Epm_TzDevResourceReport model)
        {
            return base.Channel.UpdateTzDevResourceReport(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzDevResourceReportByIds(List<long> ids)
        {
            return base.Channel.DeleteTzDevResourceReportByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzDevResourceReport>> GetTzDevResourceReportList(QueryCondition qc)
        {
            return base.Channel.GetTzDevResourceReportList(qc);
        }

        public Result<List<TzDevResourceReportItemView>> GetTzDevResourceReportItemList(QueryCondition qc)
        {
            return base.Channel.GetTzDevResourceReportItemList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzDevResourceReport> GetTzDevResourceReportModel(long id)
        {
            return base.Channel.GetTzDevResourceReportModel(id);
        }
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzPeopleChgApply(Epm_TzPeopleChgApply model)
        {
            return base.Channel.AddTzPeopleChgApply(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzPeopleChgApply(Epm_TzPeopleChgApply model)
        {
            return base.Channel.UpdateTzPeopleChgApply(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzPeopleChgApplyByIds(List<long> ids)
        {
            return base.Channel.DeleteTzPeopleChgApplyByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzPeopleChgApply>> GetTzPeopleChgApplyList(QueryCondition qc)
        {
            return base.Channel.GetTzPeopleChgApplyList(qc);
        }

        public Result<List<TzPeopleChgApplyItemView>> GetTzPeopleApplyItemList(QueryCondition qc)
        {
            return base.Channel.GetTzPeopleApplyItemList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzPeopleChgApply> GetTzPeopleChgApplyModel(long id)
        {
            return base.Channel.GetTzPeopleChgApplyModel(id);
        }
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzRivalStationReport(Epm_TzRivalStationReport model)
        {
            return base.Channel.AddTzRivalStationReport(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzRivalStationReport(Epm_TzRivalStationReport model)
        {
            return base.Channel.UpdateTzRivalStationReport(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzRivalStationReportByIds(List<long> ids)
        {
            return base.Channel.DeleteTzRivalStationReportByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzRivalStationReport>> GetTzRivalStationReportList(QueryCondition qc)
        {
            return base.Channel.GetTzRivalStationReportList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzRivalStationReport> GetTzRivalStationReportModel(long id)
        {
            return base.Channel.GetTzRivalStationReportModel(id);
        }


        #endregion

        /// <summary>
        /// 获取项目统计数据
        /// </summary>
        /// <param name="type">1:全部，2：新增，3：改造</param>
        /// <returns></returns>
        public Result<List<ProjectCount>> GetProjectCount(int type, string time)
        {
            return base.Channel.GetProjectCount(type, time);
        }
        /// <summary>
        /// 获取周报信息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="time"></param>
        /// <param name="stateType"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Result<List<Epm_ProjectWeekly>> GetProjectWeekly(int type, string time, int stateType, int pageIndex, int pageSize)
        {
            return base.Channel.GetProjectWeekly(type, time, stateType, pageIndex, pageSize);
        }
        public Result<List<Epm_ProjectCountWeekly>> GetProjectCountWeekly(int type, string time)
        {
            return base.Channel.GetProjectCountWeekly(type, time);
        }
        /// <summary>
        /// 项目信息汇总
        /// </summary>
        /// <param name="type">1:全部，2：新增，3：改造</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Result<List<ProjectView>> GetProjectSum(int type, string time, int stateType, int pageIndex, int pageSize)
        {
            return base.Channel.GetProjectSum(type, time, stateType, pageIndex, pageSize);
        }

        #region 工程甲供物资订单

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzGcGoodsOrdersApply(Epm_TzGcGoodsOrdersApply model)
        {
            return base.Channel.AddTzGcGoodsOrdersApply(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzGcGoodsOrdersApply(Epm_TzGcGoodsOrdersApply model)
        {
            return base.Channel.UpdateTzGcGoodsOrdersApply(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzGcGoodsOrdersApplyByIds(List<long> ids)
        {
            return base.Channel.DeleteTzGcGoodsOrdersApplyByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzGcGoodsOrdersApply>> GetTzGcGoodsOrdersApplyList(QueryCondition qc)
        {
            return base.Channel.GetTzGcGoodsOrdersApplyList(qc);
        }

        /// <summary>
        /// 获取详情数据列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<TzGcGoodsOrdersItemView>> GetTzGcGoodsOrdersApplyListAll(QueryCondition qc)
        {
            return base.Channel.GetTzGcGoodsOrdersApplyListAll(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzGcGoodsOrdersApply> GetTzGcGoodsOrdersApplyModel(long id)
        {
            return base.Channel.GetTzGcGoodsOrdersApplyModel(id);
        }

        #endregion

        #region 初次谈判、土地出让协议
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzFirstNegotiation(Epm_TzFirstNegotiation model)
        {
            return base.Channel.AddTzFirstNegotiation(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzFirstNegotiation(Epm_TzFirstNegotiation model)
        {
            return base.Channel.UpdateTzFirstNegotiation(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzFirstNegotiationByIds(List<long> ids)
        {
            return base.Channel.DeleteTzFirstNegotiationByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectProposal>> GetTzFirstNegotiationList(QueryCondition qc)
        {
            return base.Channel.GetTzFirstNegotiationList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<TzFirstNegotiationView> GetTzFirstNegotiationModel(long projectId)
        {
            return base.Channel.GetTzFirstNegotiationModel(projectId);
        }
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzLandNegotiation(Epm_TzLandNegotiation model)
        {
            return base.Channel.AddTzLandNegotiation(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzLandNegotiation(Epm_TzLandNegotiation model)
        {
            return base.Channel.UpdateTzLandNegotiation(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzLandNegotiationByIds(List<long> ids)
        {
            return base.Channel.DeleteTzLandNegotiationByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectProposal>> GetTzLandNegotiationList(QueryCondition qc)
        {
            return base.Channel.GetTzLandNegotiationList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<TzLandNegotiationView> GetTzLandNegotiationModel(long projectId)
        {
            return base.Channel.GetTzLandNegotiationModel(projectId);
        }
        #endregion

        #region 上会材料上报
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddMeetingFileReport(Epm_MeetingFileReport model)
        {
            return base.Channel.AddMeetingFileReport(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateMeetingFileReport(Epm_MeetingFileReport model)
        {
            return base.Channel.UpdateMeetingFileReport(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteMeetingFileReportByIds(List<long> ids)
        {
            return base.Channel.DeleteMeetingFileReportByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectProposal>> GetMeetingFileReportList(QueryCondition qc)
        {
            return base.Channel.GetMeetingFileReportList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<MeetingFileReportView> GetMeetingFileReportModel(long id)
        {
            return base.Channel.GetMeetingFileReportModel(id);
        }

        /// <summary>
        /// 修改上会材料上报状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateMeetingFileReportState(List<long> ids, string state)
        {
            return base.Channel.UpdateMeetingFileReportState(ids, state);
        }
        #endregion

        #region 项目评审记录
        ///<summary>
        ///添加:项目评审记录
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzProjectReveiews(Epm_TzProjectReveiews model)
        {
            return base.Channel.AddTzProjectReveiews(model);
        }
        ///<summary>
        ///修改:项目评审记录
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzProjectReveiews(Epm_TzProjectReveiews model)
        {
            return base.Channel.UpdateTzProjectReveiews(model);
        }
        ///<summary>
        ///删除:项目评审记录
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzProjectReveiewsByIds(List<long> ids)
        {
            return base.Channel.DeleteTzProjectReveiewsByIds(ids);
        }
        ///<summary>
        ///获取列表:项目评审记录
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectProposal>> GetTzProjectReveiewsList(QueryCondition qc)
        {
            return base.Channel.GetTzProjectReveiewsList(qc);
        }
        ///<summary>
        ///获取详情:项目评审记录
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<TzProjectReveiewsView> GetTzProjectReveiewsModel(long id)
        {
            return base.Channel.GetTzProjectReveiewsModel(id);
        }

        /// <summary>
        /// 修改项目评审记录状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzProjectReveiewsState(List<long> ids, string state)
        {
            return base.Channel.UpdateTzProjectReveiewsState(ids, state);
        }
        #endregion

        #region 项目批复信息
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzProjectApprovalInfo(Epm_TzProjectApprovalInfo model)
        {
            return base.Channel.AddTzProjectApprovalInfo(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzProjectApprovalInfo(Epm_TzProjectApprovalInfo model)
        {
            return base.Channel.UpdateTzProjectApprovalInfo(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzProjectApprovalInfoByIds(List<long> ids)
        {
            return base.Channel.DeleteTzProjectApprovalInfoByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectProposal>> GetTzProjectApprovalInfoList(QueryCondition qc)
        {
            return base.Channel.GetTzProjectApprovalInfoList(qc);
        }

        /// <summary>
        /// 财务决算查询
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_TzProjectApprovalInfo>> GetTzProjectApprovalListBy(QueryCondition qc)
        {
            return base.Channel.GetTzProjectApprovalListBy(qc);
        }

        /// <summary>
        /// 编辑财务决算信息
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="FinanceAccounts"></param>
        /// <returns></returns>
        public Result<int> UpdateFinanceAccounts(long id, decimal financeAccounts)
        {
            return base.Channel.UpdateFinanceAccounts(id, financeAccounts);
        }

        /// <summary>
        /// 财务决算详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_TzProjectApprovalInfo> GetTzProjectApprovalFinanceAccounts(long id)
        {
            return base.Channel.GetTzProjectApprovalFinanceAccounts(id);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<TzProjectApprovalInfoView> GetTzProjectApprovalInfoModel(long id)
        {
            return base.Channel.GetTzProjectApprovalInfoModel(id);
        }

        /// <summary>
        /// 修改项目批复信息状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzProjectApprovalInfoState(List<long> ids, string state)
        {
            return base.Channel.UpdateTzProjectApprovalInfoState(ids, state);
        }
        #endregion

        #region 甲供物资申请
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzSupplyMaterialApply(Epm_TzSupplyMaterialApply model)
        {
            return base.Channel.AddTzSupplyMaterialApply(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzSupplyMaterialApply(Epm_TzSupplyMaterialApply model)
        {
            return base.Channel.UpdateTzSupplyMaterialApply(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzSupplyMaterialApplyByIds(List<long> ids)
        {
            return base.Channel.DeleteTzSupplyMaterialApplyByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzSupplyMaterialApply>> GetTzSupplyMaterialApplyList(QueryCondition qc)
        {
            return base.Channel.GetTzSupplyMaterialApplyList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzSupplyMaterialApply> GetTzSupplyMaterialApplyModel(long id)
        {
            return base.Channel.GetTzSupplyMaterialApplyModel(id);
        }

        /// <summary>
        /// 获取甲供物资申请详情列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_TzSupMatApplyList>> GetTzSupMatApplyList(QueryCondition qc)
        {
            return base.Channel.GetTzSupMatApplyList(qc);
        }

        /// <summary>
        /// 修改甲供物资申请状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzSupplyMaterialApplyState(List<long> ids, string state)
        {
            return base.Channel.UpdateTzSupplyMaterialApplyState(ids, state);
        }

        /// <summary>
        /// 获取甲供物资报表数据
        /// </summary>
        /// <returns></returns>
        public Result<List<SupplyMaterialReportView>> GetSupplyMaterialReport(QueryCondition qc)
        {
            return base.Channel.GetSupplyMaterialReport(qc);
        }

        /// <summary>
        /// 获取甲供物资供应商
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<SuppListView>> GetSuppList(long projectId, QueryCondition qc)
        {
            return base.Channel.GetSuppList(projectId, qc);
        }
        #endregion

        #region 招标结果
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzBidResult(Epm_TzBidResult model)
        {
            return base.Channel.AddTzBidResult(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzBidResult(Epm_TzBidResult model)
        {
            return base.Channel.UpdateTzBidResult(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzBidResultByIds(List<long> ids)
        {
            return base.Channel.DeleteTzBidResultByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzBidResult>> GetTzBidResultList(QueryCondition qc)
        {
            return base.Channel.GetTzBidResultList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzBidResult> GetTzBidResultModel(long id)
        {
            return base.Channel.GetTzBidResultModel(id);
        }

        /// <summary>
        /// 修改招标结果状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzBidResultState(List<long> ids, string state)
        {
            return base.Channel.UpdateTzBidResultState(ids, state);
        }
        #endregion

        #region 招标申请
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzTenderingApply(Epm_TzTenderingApply model)
        {
            return base.Channel.AddTzTenderingApply(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzTenderingApply(Epm_TzTenderingApply model)
        {
            return base.Channel.UpdateTzTenderingApply(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzTenderingApplyByIds(List<long> ids)
        {
            return base.Channel.DeleteTzTenderingApplyByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzTenderingApply>> GetTzTenderingApplyList(QueryCondition qc)
        {
            return base.Channel.GetTzTenderingApplyList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzTenderingApply> GetTzTenderingApplyModel(long id)
        {
            return base.Channel.GetTzTenderingApplyModel(id);
        }

        /// <summary>
        /// 修改招标申请状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzTenderingApplyState(List<long> ids, string state)
        {
            return base.Channel.UpdateTzTenderingApplyState(ids, state);
        }
        #endregion

        #region 招标申请统计
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<TzTenderingCountView>> GetTzTenderingCountList(QueryCondition qc)
        {
            return base.Channel.GetTzTenderingCountList(qc);
        }
        #endregion

        #region 甲供物资管理
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzSupMatManagement(Epm_TzSupMatManagement model)
        {
            return base.Channel.AddTzSupMatManagement(model);
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public Result<int> AddRangeTzSupMatManagement(List<Epm_TzSupMatManagement> models)
        {
            return base.Channel.AddRangeTzSupMatManagement(models);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzSupMatManagement(Epm_TzSupMatManagement model)
        {
            return base.Channel.UpdateTzSupMatManagement(model);
        }
        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="ids">数据集合</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        public Result<int> UpdateTzSupMatManagementState(List<long> ids, int state)
        {
            return base.Channel.UpdateTzSupMatManagementState(ids, state);
        }

        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzSupMatManagementByIds(List<long> ids)
        {
            return base.Channel.DeleteTzSupMatManagementByIds(ids);
        }
        ///<summary>
        ///根据供应商名称获取对应供应商信息
        ///</summary>
        ///<param name="companyName">供应商名称</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<Base_Company> GetCompanyByName(string companyName)
        {
            return base.Channel.GetCompanyByName(companyName);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzSupMatManagement>> GetTzSupMatManagementList(QueryCondition qc)
        {
            return base.Channel.GetTzSupMatManagementList(qc);
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzSupMatManagement> GetTzSupMatManagementModel(long id)
        {
            return base.Channel.GetTzSupMatManagementModel(id);
        }

        /// <summary>
        /// 获取已启用的甲供物资申请数据
        /// </summary>
        /// <param name="name">物资种类</param>
        /// <param name="productName">品名</param>
        /// <returns></returns>
        public Result<List<Epm_TzSupMatManagement>> GetTzSupMatManagementListBy(long SupplierId, string name, string productName)
        {
            return base.Channel.GetTzSupMatManagementListBy(SupplierId, name, productName);
        }

        /// <summary>
        /// 根据物资种类、品名、规格获取物资管理详细信息
        /// </summary>
        /// <param name="name">物资种类</param>
        /// <param name="productName">品名</param>
        /// <param name="specification">规格</param>
        /// <returns></returns>
        public Result<Epm_TzSupMatManagement> GetTzSupMatManagementModelBy(long SupplierId, string name, string productName, string specification)
        {
            return base.Channel.GetTzSupMatManagementModelBy(SupplierId, name, productName, specification);
        }

        #endregion

        #region 设计方案
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzDesignScheme(Epm_TzDesignScheme model)
        {
            return base.Channel.AddTzDesignScheme(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzDesignScheme(Epm_TzDesignScheme model)
        {
            return base.Channel.UpdateTzDesignScheme(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzDesignSchemeByIds(List<long> ids)
        {
            return base.Channel.DeleteTzDesignSchemeByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzDesignScheme>> GetTzDesignSchemeList(QueryCondition qc)
        {
            return base.Channel.GetTzDesignSchemeList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzDesignScheme> GetTzDesignSchemeModel(long id)
        {
            return base.Channel.GetTzDesignSchemeModel(id);
        }
        /// <summary>
        /// 加载项目批复信息
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_TzProjectApproval>> GetTzProjectApprovalList_Choice(QueryCondition qc)
        {
            return base.Channel.GetTzProjectApprovalList_Choice(qc);
        }

        /// <summary>
        /// 修改设计方案状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzDesignSchemeState(List<long> ids, string state)
        {
            return base.Channel.UpdateTzDesignSchemeState(ids, state);
        }
        #endregion

        #region 开工申请
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzProjectStartApply(Epm_TzProjectStartApply model)
        {
            return base.Channel.AddTzProjectStartApply(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzProjectStartApply(Epm_TzProjectStartApply model)
        {
            return base.Channel.UpdateTzProjectStartApply(model);
        }

        /// <summary>
        /// 修改开工申请
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateTzProjectStartApplyNew(TzProjectStartApplyView model)
        {
            return base.Channel.UpdateTzProjectStartApplyNew(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzProjectStartApplyByIds(List<long> ids)
        {
            return base.Channel.DeleteTzProjectStartApplyByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzProjectStartApply>> GetTzProjectStartApplyList(QueryCondition qc)
        {
            return base.Channel.GetTzProjectStartApplyList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TzProjectStartApply> GetTzProjectStartApplyModel(long id)
        {
            return base.Channel.GetTzProjectStartApplyModel(id);
        }
        public Result<TzStartTenderingAndSupplyView> GetTzProjectStartApplyModelAndOther(long id)
        {
            return base.Channel.GetTzProjectStartApplyModelAndOther(id);
        }
        /// <summary>
        /// 根据项目id查看工期和手续
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_TimeLimitAndProcedure> GetTimeLimitAndProcedure(long id)
        {
            return base.Channel.GetTimeLimitAndProcedure(id);
        }

        public Result<bool> isExistTenderingAndSupply(long projectId)
        {
            return base.Channel.isExistTenderingAndSupply(projectId);
        }

        /// <summary>
        /// 修改开工申请状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzProjectStartApplyState(List<long> ids, string state)
        {
            return base.Channel.UpdateTzProjectStartApplyState(ids, state);
        }
        #endregion

        #region 图纸会审
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTzConDrawing(Epm_TzConDrawing model, List<Base_Files> fileListFile)
        {
            return base.Channel.AddTzConDrawing(model, fileListFile);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTzConDrawing(Epm_TzConDrawing model, List<Base_Files> fileListFile)
        {
            return base.Channel.UpdateTzConDrawing(model, fileListFile);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTzConDrawingByIds(List<long> ids)
        {
            return base.Channel.DeleteTzConDrawingByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TzConDrawing>> GetTzConDrawingList(QueryCondition qc)
        {
            return base.Channel.GetTzConDrawingList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<TzConDrawingView> GetTzConDrawingModel(long id)
        {
            return base.Channel.GetTzConDrawingModel(id);
        }
        ///<summary>
        ///根据id获取项目基础信息:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<ProjectBasicsInfoView> GetProjectBasicInfoByID(long id)
        {
            return base.Channel.GetProjectBasicInfoByID(id);
        }

        /// <summary>
        /// 修改施工图纸会审状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> UpdateTzConDrawingState(List<long> ids, string state)
        {
            return base.Channel.UpdateTzConDrawingState(ids, state);
        }
        #endregion

        #region 工期和手续
        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddTimeLimitAndCrossings(Epm_TimeLimitAndProcedure model)
        {
            return base.Channel.AddTimeLimitAndCrossings(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateTimeLimitAndCrossings(Epm_TimeLimitAndProcedure model)
        {
            return base.Channel.UpdateTimeLimitAndCrossings(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteTimeLimitAndCrossingsByIds(List<long> ids)
        {
            return base.Channel.DeleteTimeLimitAndCrossingsByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_TimeLimitAndProcedure>> GetTimeLimitAndCrossingsList(QueryCondition qc)
        {
            return base.Channel.GetTimeLimitAndCrossingsList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<Epm_TimeLimitAndProcedure> GetTimeLimitAndCrossingsModel(long id)
        {
            return base.Channel.GetTimeLimitAndCrossingsModel(id);
        }
        #endregion

        #region 竣工验收

        ///<summary>
        ///添加:
        ///</summary>
        ///<param name="model">要添加的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> AddCompletionAcceptanceResUpload(Epm_CompletionAcceptanceResUpload model)
        {
            return base.Channel.AddCompletionAcceptanceResUpload(model);
        }
        ///<summary>
        ///修改:
        ///</summary>
        ///<param name="model">要修改的model</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateCompletionAcceptanceResUpload(Epm_CompletionAcceptanceResUpload model)
        {
            return base.Channel.UpdateCompletionAcceptanceResUpload(model);
        }
        ///<summary>
        ///删除:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> DeleteCompletionAcceptanceResUploadByIds(List<long> ids)
        {
            return base.Channel.DeleteCompletionAcceptanceResUploadByIds(ids);
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        ///<param name="qc">查询条件</param>
        ///<returns>符合条件的数据集合</returns>
        public Result<List<Epm_CompletionAcceptanceResUpload>> GetCompletionAcceptanceResUploadList(QueryCondition qc)
        {
            return base.Channel.GetCompletionAcceptanceResUploadList(qc);
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        ///<param name="id">数据Id</param>
        ///<returns>数据详情model</returns>
        public Result<CompletionAcceptanceResUploadView> GetCompletionAcceptanceResUploadModel(long id)
        {
            return base.Channel.GetCompletionAcceptanceResUploadModel(id);
        }
        ///<summary>
        ///修改状态:
        ///</summary>
        ///<param name="ids">要删除的Id集合</param>
        ///<returns>受影响的行数</returns>
        public Result<int> UpdateCompletionAcceptanceResUploadState(List<long> idList, string state)
        {
            return base.Channel.UpdateCompletionAcceptanceResUploadState(idList, state);
        }
        #endregion

        #region 考勤统计
        /// <summary>
        /// 分公司统计
        /// </summary>
        /// <returns></returns>
        public Result<List<AttendanceBranchCountView>> GetBranchCount(string companyId, string startTime, string endTime, int pageIndex, int pageSize)
        {
            return base.Channel.GetBranchCount(companyId, startTime, endTime, pageIndex, pageSize);
        }
        /// <summary>
        /// 项目统计
        /// </summary>
        /// <returns></returns>
        public Result<List<AttendanceBranchCountView>> GetBranchProjectCount(string name, string companyId, string startTime, string endTime, int pageIndex, int pageSize)
        {
            return base.Channel.GetBranchProjectCount(name, companyId, startTime, endTime, pageIndex, pageSize);
        }

        /// <summary>
        /// 人员统计
        /// </summary>
        /// <returns></returns>
        public Result<List<AttendanceBranchCountView>> GetBranchUserCount(string ProName, string companyId, string startTime, string endTime, string userName, int pageIndex, int pageSize)
        {
            return base.Channel.GetBranchUserCount(ProName, companyId, startTime, endTime, userName, pageIndex, pageSize);
        }
        #endregion


        /// <summary>
        /// 查询监理签到统计
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<SupervisorLogStatisticView>> GetSupervisionAttendance(QueryCondition qc)
        {
            return base.Channel.GetSupervisionAttendance(qc);
        }


        /// <summary>
        /// 获取考勤设置信息
        /// </summary>
        /// <returns></returns>
        public Result<AttendanceView> GetAttendanceModel()
        {
            return base.Channel.GetAttendanceModel();
        }
    }
}