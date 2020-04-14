using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hc.epm.Service.Base;
using hc.Plat.Common.Global;
using hc.epm.DataModel.Business;
using hc.epm.Common;
using hc.Plat.Common.Extend;
using hc.epm.DataModel.Basic;
using hc.epm.ViewModel;
using System.Data.Entity;
using hc.epm.DataModel.BaseCore;

namespace hc.epm.Service.ClientSite
{
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        /// <summary>
        /// 根据表名和id获取所有附件
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<List<Base_Files>> GetFilesByTable(string tableName, long id)
        {
            Result<List<Base_Files>> result = new Result<List<Base_Files>>();
            try
            {
                var models = DataOperateBasic<Base_Files>.Get().GetList(i => i.TableId == id && i.TableName == tableName).ToList();
                models = models.OrderBy(i => i.Id).ToList();
                result.Data = models;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetFilesByTable");
            }
            return result;
        }

        /// <summary>
        /// 根据表名和id获取所有附件
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<List<Base_Files>> GetFileListByTableIds(string tableName, List<long> ids)
        {
            Result<List<Base_Files>> result = new Result<List<Base_Files>>();
            try
            {
                var models = DataOperateBasic<Base_Files>.Get().GetList(i => ids.Contains(i.TableId) && i.TableName == tableName).ToList();
                models = models.OrderBy(i => i.Id).ToList();
                result.Data = models;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetFilesByTableByTableIds");
            }
            return result;
        }

        public Result<List<Base_Files>> GetFilesByTableName(string tableName, string name)
        {
            Result<List<Base_Files>> result = new Result<List<Base_Files>>();
            try
            {
                var models = DataOperateBasic<Base_Files>.Get().GetList(i => i.TableName == tableName && i.Name == name).ToList();
                models = models.OrderBy(i => i.Id).ToList();
                result.Data = models;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetFilesByTableName");
            }
            return result;
        }


        /// <summary>
        /// 添加投资管理附件
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList"></param>
        /// <param name="isdelete"></param>
        /// <returns></returns>
        public int AddFilesBytzTable(BaseModel model, List<Epm_TzAttachs> fileList, bool isdelete = true)
        {
            if (fileList != null)
            {
                long id = model.Id;
                string tableName = model.GetType().Name;
                //删除之前的附件
                if (isdelete)
                {
                    var oldFiles = DataOperateBusiness<Epm_TzAttachs>.Get().GetList(i => i.DataId == id && i.TableName == tableName).ToList();
                    DataOperateBusiness<Epm_TzAttachs>.Get().DeleteRange(oldFiles);
                }
                //新增附件
                List<Epm_TzAttachs> fileModels = new List<Epm_TzAttachs>();
                foreach (var item in fileList)
                {
                    SetCurrentUser(item);
                    item.DataId = id;
                    item.TableName = tableName;
                    fileModels.Add(item);
                }
                int rows = 0;
                rows = DataOperateBusiness<Epm_TzAttachs>.Get().AddRange(fileModels);
                return rows;
            }
            return 0;
        }

        /// <summary>
        /// 添加投资管理附件/同表不同页面附件
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList"></param>
        /// <param name="isdelete"></param>
        /// <returns></returns>
        public int AddConferenceFiles(string tableName, long id, List<Epm_TzAttachs> fileList, InvestmentEnclosure ie = InvestmentEnclosure.itself, bool isdelete = true)
        {
            if (fileList != null)
            {
                //删除之前的附件
                if (isdelete)
                {
                    var columns = fileList.Select(i => i.TypeNo);
                    var oldFiles = DataOperateBusiness<Epm_TzAttachs>.Get().GetList(i => i.DataId == id && i.TableName == tableName && columns.Contains(i.TypeNo)).ToList();
                    DataOperateBusiness<Epm_TzAttachs>.Get().DeleteRange(oldFiles);
                }
                //新增附件
                List<Epm_TzAttachs> fileModels = new List<Epm_TzAttachs>();
                foreach (var item in fileList)
                {
                    SetCurrentUser(item);
                    if (ie != InvestmentEnclosure.itself)
                    {
                        item.Nature = ie.ToString();
                        item.NatureName = ie.GetText();
                    }
                    item.DataId = id;
                    item.TableName = tableName;
                    fileModels.Add(item);
                }
                int rows = 0;
                rows = DataOperateBusiness<Epm_TzAttachs>.Get().AddRange(fileModels);
                return rows;
            }
            return 0;
        }

        /// <summary>
        /// 根据表名和ID获取附件
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<List<Epm_TzAttachs>> GetFilesByTZTable(string tableName, long id)
        {
            Result<List<Epm_TzAttachs>> result = new Result<List<Epm_TzAttachs>>();
            try
            {
                var models = DataOperateBusiness<Epm_TzAttachs>.Get().GetList(i => i.DataId == id && i.TableName == tableName && !i.IsDelete).ToList();
                if (models != null)
                {
                    foreach (var item in models)
                    {
                        string size= ListExtensionMethod.GetByteLength(item.Size);
                        item.FilesSize = Convert.ToInt32(size);
                    }
                }
                models = models.OrderBy(i => i.Id).ToList();
                result.Data = models;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetFilesByTable");
            }
            return result;
        }

        public Result<List<Epm_TzAttachs>> GetConferenceFiles(string tableName, long id, InvestmentEnclosure ie = InvestmentEnclosure.itself)
        {
            Result<List<Epm_TzAttachs>> result = new Result<List<Epm_TzAttachs>>();
            try
            {
                List<Epm_TzAttachs> models;
                if (ie == InvestmentEnclosure.itself)
                    models = DataOperateBusiness<Epm_TzAttachs>.Get().GetList(i => i.DataId == id && i.TableName == tableName && !i.IsDelete && i.Nature == null).ToList();
                else
                    models = DataOperateBusiness<Epm_TzAttachs>.Get().GetList(i => i.DataId == id && i.TableName == tableName && !i.IsDelete && i.Nature != ie.ToString()).ToList();
                models = models.OrderBy(i => i.Id).ToList();
                result.Data = models;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetFilesByTable");
            }
            return result;
        }
        /// <summary>
        /// 根据Epm_TzAttachs表id获取附件
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<Epm_TzAttachs> GetFileByTzAttachsId(long id)
        {
            Result<Epm_TzAttachs> result = new Result<Epm_TzAttachs>();
            try
            {
                var models = DataOperateBusiness<Epm_TzAttachs>.Get().Single(i => i.Id == id);
                result.Data = models;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetFileByTzAttachsId");
            }
            return result;
        }
    }
}
