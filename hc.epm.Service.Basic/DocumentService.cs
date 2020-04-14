using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hc.epm.DataModel.Basic;
using hc.Plat.Common.Global;
using hc.epm.Service.Base;
using hc.epm.Common;
using hc.Plat.Common.Extend;
using hc.Plat.Cache.Helper;
using hc.epm.ViewModel;

namespace hc.epm.Service.Basic
{
    public partial class BasicService : BaseService, IBasicService
    {
        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Log>> GetLogList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Base_Log>> result = new Result<List<Base_Log>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Base_Log>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetLogList");
            }
            return result;
        }
        /// <summary>
        /// 获取审核日志
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_StatusLog>> GetStatusLogList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Base_StatusLog>> result = new Result<List<Base_StatusLog>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Base_StatusLog>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetStatusLogList");
            }
            return result;
        }
        /// <summary>
        /// 获取区域列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Region>> GetRegionList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Base_Region>> result = new Result<List<Base_Region>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Base_Region>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetRegionList");
            }
            return result;
        }
        /// <summary>
        /// 获取区域详情
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Result<Base_Region> GetRegionModel(string code)
        {

            Result<Base_Region> result = new Result<Base_Region>();
            try
            {
                var model = DataOperateBasic<Base_Region>.Get().Single(i => i.RegionCode == code);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetRegionModel");
            }
            return result;
        }
        /// <summary>
        /// 加载区域列表，有缓存
        /// </summary>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        public Result<List<Base_Region>> LoadRegionList(string parentCode = "")
        {
            Result<List<Base_Region>> result = new Result<List<Base_Region>>();
            try
            {
                List<Base_Region> list = new List<Base_Region>();
                    if (!string.IsNullOrEmpty(parentCode))
                {
                    list = DataOperateBasic<Base_Region>.Get().GetList(i => i.ParentCode == parentCode).ToList();
                }
                else
                {
                    list = DataOperateBasic<Base_Region>.Get().GetList().ToList();
                }

                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "LoadRegionList");
            }
            return result;
        }        
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
        /// 过滤掉不会直接返回的文件
        /// </summary>
        private List<string> FilterFileTable
        {
            get
            {
                List<string> list = new List<string>();
                return list;
            }
        }        
        /// <summary>
        /// 添加电子协议
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList">附件列表</param>
        /// <returns></returns>
        public Result<int> AddProtocol(Base_Protocol model, List<Base_Files> fileList)
        {
            model = base.SetCurrentUser(model);
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBasic<Base_Protocol>.Get().Add(model);

                //新增附件
                AddFilesByTable(model, fileList);
                result.Data = rows;
                result.Flag = EResultFlag.Success;


                WriteLog(AdminModule.ElectronicAgreement.GetText(), SystemRight.Add.GetText(), "新增电子协议:" + model.Id + ":" + model.Title);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddProtocol");
            }
            return result;
        }
        /// <summary>
        /// 修改电子协议
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList">附件</param>
        /// <returns></returns>
        public Result<int> UpdateProtocol(Base_Protocol model, List<Base_Files> fileList = null)
        {
            var oldModel = DataOperateBasic<Base_Protocol>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBasic<Base_Protocol>.Get().Update(model);
                if (fileList != null)
                {
                    //删除之前的附件
                    DeleteFilesByTable(model.GetType().Name, new List<long>() { model.Id });
                    //新增附件
                    AddFilesByTable(model, fileList);
                }
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                WriteLog(AdminModule.ElectronicAgreement.GetText(), SystemRight.Add.GetText(), "修改电子协议:" + model.Id + ":" + model.Title);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateDocTemplete");
            }
            return result;
        }
        /// <summary>
        /// 审核电子协议
        /// </summary>
        /// <param name="protocolId">电子协议Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditProtocol(long protocolId, int type)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBasic<Base_Protocol>.Get().GetModel(protocolId);
                if (type == 1)
                {
                    model.IsEnable = !model.IsEnable;
                }
                else if (type == 2)
                {
                    model.IsConfirm = !model.IsConfirm;
                }
                var rows = DataOperateBasic<Base_Protocol>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.ElectronicAgreement.GetText(), SystemRight.Check.GetText(), "审核电子协议:" + model.Id + ":" + model.Title);
                if (type == 1)
                {
                    WriteStateLog(model, (!model.IsEnable).ToString(), (model.IsEnable).ToString());
                }
                else if (type == 2)
                {
                    WriteStateLog(model, (!model.IsConfirm).ToString(), (model.IsConfirm).ToString());
                }

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AuditProtocol");
            }
            return result;
        }
        /// <summary>
        /// 获取电子协议详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Base_Protocol> GetProtocolModel(long id)
        {
            Result<Base_Protocol> result = new Result<Base_Protocol>();
            try
            {
                var model = DataOperateBasic<Base_Protocol>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProtocolModel");
            }
            return result;
        }
        /// <summary>
        /// 根据协议类型获取电子协议详情
        /// </summary>
        /// <param name="protocolType"></param>
        /// <returns></returns>
        public Result<Base_Protocol> GetProtocolModelByType(ProtocolType protocolType)
        {
            Result<Base_Protocol> result = new Result<Base_Protocol>();
            try
            {
                var model = DataOperateBasic<Base_Protocol>.Get().Single(i => i.Type == protocolType.ToString());
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProtocolModelByType");
            }
            return result;
        }
        /// <summary>
        /// 获取电子协议列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Protocol>> GetProtocolList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Base_Protocol>> result = new Result<List<Base_Protocol>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Base_Protocol>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProtocolList");
            }
            return result;
        }
        /// <summary>
        /// 批量删除电子协议
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteProtocoByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBasic<Base_Protocol>.Get().GetList(i => ids.Contains(i.Id)).ToList();

                var rows = DataOperateBasic<Base_Protocol>.Get().DeleteRange(models);
                //删除附件
                DeleteFilesByTable(models.FirstOrDefault().GetType().Name, ids);

                result.Data = rows;
                result.Flag = EResultFlag.Success;

                #region 写日志
                WriteLog(AdminModule.DocTemplete.GetText(), SystemRight.Delete.GetText(), "批量删除电子协议:" + rows);
                #endregion

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteProtocoByIds");
            }
            return result;
        }
    }
}
