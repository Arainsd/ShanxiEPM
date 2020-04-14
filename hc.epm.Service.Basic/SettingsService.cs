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
using AutoMapper;
using hc.epm.DataModel.Business;
using hc.epm.ViewModel;
using System.Data;
using Baidu.Aip.Face;
using Newtonsoft.Json.Linq;

namespace hc.epm.Service.Basic
{
    public partial class BasicService : BaseService, IBasicService
    {
        private BasicDataContext context = new BasicDataContext();
        private BusinessDataContext contextbs = new BusinessDataContext();

        /// <summary>
        /// 获取所有配置，有缓存
        /// </summary>
        /// <returns></returns>
        public Result<List<Base_Settings>> LoadSettings()
        {
            Result<List<Base_Settings>> result = new Result<List<Base_Settings>>();
            try
            {
                var list = DataOperateBasic<Base_Settings>.Get().GetList().ToList();

                result.Data = list;
                result.AllRowsCount = list.Count();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSettings");
            }
            return result;
        }
        /// <summary>
        /// 新增配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddSettings(Base_Settings model)
        {
            model = base.SetCurrentUser(model);
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBasic<Base_Settings>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Add.GetText(), "新增配置项:" + model.Id + ":" + model.Name);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddSettings");
            }
            return result;
        }
        /// <summary>
        /// 获取指定配置项
        /// </summary>
        /// <param name="setKey"></param>
        /// <returns></returns>
        public Result<Base_Settings> LoadSettingsByKey(Settings setKey)
        {
            Result<Base_Settings> result = new Result<Base_Settings>();
            try
            {
                var model = LoadSettings().Data.FirstOrDefault(i => i.Code == setKey.ToString());
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "LoadSettingsByKey");
            }
            return result;
        }
        /// <summary>
        /// 修改配置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateSettings(Base_Settings model)
        {
            var oldModel = DataOperateBasic<Base_Settings>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);

            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBasic<Base_Settings>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Modify.GetText(), "修改配置项:" + model.Id + ":" + model.Name);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateSettings");
            }
            return result;
        }
        /// <summary>
        /// 删除配置
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteSettingsByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBasic<Base_Settings>.Get().GetList(i => ids.Contains(i.Id)).ToList();

                var rows = DataOperateBasic<Base_Settings>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Delete.GetText(), "删除配置项:" + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteSettingsByIds");
            }
            return result;
        }
        /// <summary>
        /// 获取网站设置
        /// </summary>
        /// <returns></returns>
        public Result<Base_Config> LoadConfig()
        {
            Result<Base_Config> result = new Result<Base_Config>();
            try
            {

                var model = DataOperateBasic<Base_Config>.Get().GetList().FirstOrDefault();

                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "LoadConfig");
            }
            return result;
        }
        /// <summary>
        /// 添加网站设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddConfig(Base_Config model)
        {
            model = base.SetCurrentUser(model);
            Result<int> result = new Result<int>();
            try
            {

                var rows = DataOperateBasic<Base_Config>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Add.GetText(), "新增网站设置:" + model.Id + ":" + model.Name);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddConfig");
            }
            return result;
        }
        /// <summary>
        /// 修改网站设置
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateConfig(Base_Config model)
        {
            var oldModel = DataOperateBasic<Base_Config>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);
            model.Logo = oldModel.Logo;
            Result<int> result = new Result<int>();
            try
            {

                var rows = DataOperateBasic<Base_Config>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Add.GetText(), "修改网站设置:" + model.Id + ":" + model.Name);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateConfig");
            }
            return result;
        }
        /// <summary>
        /// 添加类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddType(Base_TypeDictionary model)
        {
            model = base.SetCurrentUser(model);
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBasic<Base_TypeDictionary>.Get().Add(model);
                if (model.Type == "MilepostType")
                {
                    var list = DataOperateBusiness<Epm_Milepost>.Get().GetList(i => i.ParentId == 0 && i.IsDelete == false).ToList();
                    Epm_Milepost mil = new Epm_Milepost();
                    mil.ParentId = 0;
                    mil.Code = model.No;
                    mil.Name = model.Name;
                    mil.State = 1;
                    mil.IsDelete = false;
                    mil = base.SetCurrentUser(mil);
                    DataOperateBusiness<Epm_Milepost>.Get().Add(mil);
                }

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.TypeDictionary.GetText(), SystemRight.Add.GetText(), "新增数据类型:" + model.Id + ":" + model.Name);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddType");
            }
            return result;
        }
        /// <summary>
        /// 修改数据类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateType(Base_TypeDictionary model)
        {
            var oldModel = DataOperateBasic<Base_TypeDictionary>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);

            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBasic<Base_TypeDictionary>.Get().Update(model);

                if (model.Type == "MilepostType")
                {
                    var parent = DataOperateBusiness<Epm_Milepost>.Get().GetList(i => i.ParentId == 0 && i.Code == model.No && i.IsDelete == false).FirstOrDefault();

                    parent.Name = model.Name;

                    DataOperateBusiness<Epm_Milepost>.Get().Update(parent);
                }
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.TypeDictionary.GetText(), SystemRight.Modify.GetText(), "修改数据类型:" + model.Id + ":" + model.Name);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateType");
            }
            return result;
        }
        /// <summary>
        /// 审核类型数据
        /// </summary>
        /// <param name="typeId">类型数据Id</param>
        /// <param name="type">1代表启用/禁用，2代表确认/未确认</param>
        /// <returns></returns>
        public Result<int> AuditType(long typeId, int type)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBasic<Base_TypeDictionary>.Get().GetModel(typeId);
                var rows = DataOperateBasic<Base_TypeDictionary>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.TypeDictionary.GetText(), SystemRight.Check.GetText(), "审核数据类型:" + model.Id + ":" + model.Name);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AuditType");
            }
            return result;
        }
        /// <summary>
        /// 获取数据类型详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Base_TypeDictionary> GetTypeModel(long id)
        {
            Result<Base_TypeDictionary> result = new Result<Base_TypeDictionary>();
            try
            {
                var rows = DataOperateBasic<Base_TypeDictionary>.Get().GetModel(id);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTypeModel");
            }
            return result;
        }
        /// <summary>
        /// 获取类型列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_TypeDictionary>> GetTypeList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Base_TypeDictionary>> result = new Result<List<Base_TypeDictionary>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Base_TypeDictionary>(context, qc);

                var pIds = result.Data.Select(i => i.PId).ToList();
                pIds.RemoveAll(i => i == 0);
                if (pIds.Count() > 0)
                {
                    List<Base_TypeDictionary> list = new List<Base_TypeDictionary>();
                    var parents = DataOperateBasic<Base_TypeDictionary>.Get().GetList(i => pIds.Contains(i.Id)).ToList();
                    foreach (var item in result.Data)
                    {
                        Base_TypeDictionary model = item;
                        model.PName = "";
                        if (item.PId != 0)
                        {
                            model.PName = parents.FirstOrDefault(i => i.Id == model.PId).Name;
                        }
                        list.Add(model);
                    }
                    result.Data = list;
                }
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTypeList");
            }
            return result;
        }
        /// <summary>
        /// 根据父类型获取子类型
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        public Result<List<Base_TypeDictionary>> GetTypeListByPId(long pId)
        {
            Result<List<Base_TypeDictionary>> result = new Result<List<Base_TypeDictionary>>();
            try
            {
                var list = DataOperateBasic<Base_TypeDictionary>.Get().GetList(i => i.PId == pId).ToList();
                result.Data = list;
                result.AllRowsCount = list.Count();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTypeListByPId");
            }
            return result;
        }
        /// <summary>
        /// 根据指定类型获取所有类型数据
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Result<List<Base_TypeDictionary>> GetTypeListByType(DictionaryType type)
        {
            Result<List<Base_TypeDictionary>> result = new Result<List<Base_TypeDictionary>>();
            try
            {
                var list = DataOperateBasic<Base_TypeDictionary>.Get().GetList(i => i.Type == type.ToString()).ToList();
                result.Data = list;
                result.AllRowsCount = list.Count();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTypeListByType");
            }
            return result;
        }
        /// <summary>
        /// 根据字典类型集合获取字典数据
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public Result<Dictionary<DictionaryType, List<Base_TypeDictionary>>> GetTypeListByTypes(List<DictionaryType> types)
        {
            Dictionary<DictionaryType, List<Base_TypeDictionary>> dictionary = new Dictionary<DictionaryType, List<Base_TypeDictionary>>();
            Result<Dictionary<DictionaryType, List<Base_TypeDictionary>>> result = new Result<Dictionary<DictionaryType, List<Base_TypeDictionary>>>();
            try
            {
                var strTypes = types.Select(i => i.ToString());
                var list = DataOperateBasic<Base_TypeDictionary>.Get().GetList(i => strTypes.Contains(i.Type)).ToList();
                dictionary = list.GroupBy(i => i.Type).ToDictionary(i => i.Key.ToEnumReq<DictionaryType>(), j => j.ToList().OrderBy(i => i.No).ToList());
                result.Data = dictionary;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTypeListByType");
            }
            return result;
        }
        /// <summary>
        /// 获取所有类型,已启用，已确认
        /// </summary>
        /// <returns></returns>
        public Result<List<Base_TypeDictionary>> GetAllTypeList()
        {
            Result<List<Base_TypeDictionary>> result = new Result<List<Base_TypeDictionary>>();
            try
            {
                var list = DataOperateBasic<Base_TypeDictionary>.Get().GetList().ToList();
                result.Data = list;
                result.AllRowsCount = list.Count();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetAllTypeList");
            }
            return result;
        }
        /// <summary>
        /// 类型删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteTypeByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBasic<Base_TypeDictionary>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                foreach (var model in models)
                {
                    var list = GetTypeListByPId(model.Id);
                    //删除对应子类型
                    DataOperateBasic<Base_TypeDictionary>.Get().DeleteRange(list.Data);
                }
                var rows = DataOperateBasic<Base_TypeDictionary>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

                #region 写日志
                WriteLog(AdminModule.TypeDictionary.GetText(), SystemRight.Delete.GetText(), "批量删除类型:" + rows);
                #endregion

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTypeByIds");
            }
            return result;
        }
        /// <summary>
        /// 获取数据字典列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Dictionary>> GetDictionaryList(QueryCondition qc)
        {
            Result<List<Base_Dictionary>> result = new Result<List<Base_Dictionary>>();
            try
            {

                var list = DataOperateBasic<Base_Dictionary>.Get().GetList().ToList();

                result.Data = list;
                result.AllRowsCount = list.Count();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetDictionaryList");
            }
            return result;
        }
        /// <summary>
        /// 获取数据字典列表，有缓存
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_Dictionary>> LoadDictionaryList()
        {
            Result<List<Base_Dictionary>> result = new Result<List<Base_Dictionary>>();
            try
            {
                var list = DataOperateBasic<Base_Dictionary>.Get().GetList().ToList();
                result.Data = list;
                result.AllRowsCount = list.Count();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "LoadDictionaryList");
            }
            return result;
        }
        /// <summary>
        /// 根据No获取字典列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Base_TypeDictionary>> GetTypeListByNo(string No)
        {
            Result<List<Base_TypeDictionary>> result = new Result<List<Base_TypeDictionary>>();
            try
            {
                var list = DataOperateBasic<Base_TypeDictionary>.Get().GetList(i => i.No == No).ToList();
                result.Data = list;
                result.AllRowsCount = list.Count();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTypeListByNo");
            }
            return result;
        }

        #region  人脸信息
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddAIUserFace(EPM_AIUserFace model)
        {
            Result<int> result = new Result<int>();
            try
            {
                model = SetCurrentUser(model);
                model = SetCreateUser(model);
                var rows = DataOperateBusiness<EPM_AIUserFace>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.AIUserFace.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddAIUserFace");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateAIUserFace(EPM_AIUserFace model)
        {
            Result<int> result = new Result<int>();
            try
            {
                model = SetCurrentUser(model);
                var rows = DataOperateBusiness<EPM_AIUserFace>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.AIUserFace.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateAIUserFace");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteAIUserFaceByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                //DelAIUserFaceInfo(0,)
                var models = DataOperateBusiness<EPM_AIUserFace>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                foreach (var item in models)
                {
                    DelAIUserFaceInfo(item.UserId, item.FaceToken);
                }
                var rows = DataOperateBusiness<EPM_AIUserFace>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.AIUserFace.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteAIUserFaceByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<EPM_AIUserFace>> GetAIUserFaceList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<EPM_AIUserFace>> result = new Result<List<EPM_AIUserFace>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<EPM_AIUserFace>(contextbs, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetAIUserFaceList");
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Result<EPM_AIUserFace> GetAIUserFaceByUserId(long userId)
        {
            Result<EPM_AIUserFace> result = new Result<EPM_AIUserFace>();
            try
            {
                var model = DataOperateBusiness<EPM_AIUserFace>.Get().GetList(t => t.UserId == userId).FirstOrDefault();
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetAIUserFaceByUserId");
            }
            return result;
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<EPM_AIUserFace> GetAIUserFaceModel(long id)
        {
            Result<EPM_AIUserFace> result = new Result<EPM_AIUserFace>();
            try
            {
                var model = DataOperateBusiness<EPM_AIUserFace>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetAIUserFaceModel");
            }
            return result;
        }

        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddFaceOperateLog(EPM_FaceOperateLog model)
        {
            Result<int> result = new Result<int>();
            try
            {
                model = SetCurrentUser(model);
                model = SetCreateUser(model);
                var rows = DataOperateBusiness<EPM_FaceOperateLog>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.AIUserFace.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddFaceOperateLog");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateFaceOperateLog(EPM_FaceOperateLog model)
        {
            Result<int> result = new Result<int>();
            try
            {
                model = SetCurrentUser(model);
                var rows = DataOperateBusiness<EPM_FaceOperateLog>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.AIUserFace.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateFaceOperateLog");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteFaceOperateLogByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<EPM_FaceOperateLog>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<EPM_FaceOperateLog>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.AIUserFace.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteFaceOperateLogByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<EPM_FaceOperateLog>> GetFaceOperateLogList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<EPM_FaceOperateLog>> result = new Result<List<EPM_FaceOperateLog>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<EPM_FaceOperateLog>(contextbs, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetFaceOperateLogList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<EPM_FaceOperateLog> GetFaceOperateLogModel(long id)
        {
            Result<EPM_FaceOperateLog> result = new Result<EPM_FaceOperateLog>();
            try
            {
                var model = DataOperateBusiness<EPM_FaceOperateLog>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetFaceOperateLogModel");
            }
            return result;
        }
        #endregion

        #region 考勤信息
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddSignInformation(Epm_SignInformation model)
        {
            Result<int> result = new Result<int>();
            try
            {
                model = SetCurrentUser(model);
                model = SetCreateUser(model);
                var rows = DataOperateBusiness<Epm_SignInformation>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.AdminUserManager.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddSignInformation");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateSignInformation(Epm_SignInformation model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_SignInformation>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.AdminUserManager.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateSignInformation");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteSignInformationByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_SignInformation>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_SignInformation>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.AdminUserManager.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteSignInformationByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_SignInformation>> GetSignInformationList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_SignInformation>> result = new Result<List<Epm_SignInformation>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_SignInformation>(contextbs, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSignInformationList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_SignInformation> GetSignInformationModel(long id)
        {
            Result<Epm_SignInformation> result = new Result<Epm_SignInformation>();
            try
            {
                var model = DataOperateBusiness<Epm_SignInformation>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSignInformationModel");
            }
            return result;
        }

        /// <summary>
        /// 根据项目ID和用户ID获取已签到用户信息
        /// </summary>
        /// <param name="ProjectId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Result<Epm_SignInformation> GetSignBy(long projectId, long userId)
        {
            Result<Epm_SignInformation> result = new Result<Epm_SignInformation>();
            try
            {
                var model = DataOperateBusiness<Epm_SignInformation>.Get().GetList(t => t.projectId == projectId && t.userId == userId).FirstOrDefault();
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetSignInformationModel");
            }
            return result;
        }
        #endregion


        /// <summary>
        /// 人脸识别API参数初始化
        /// </summary>
        /// <returns></returns>
        private Face GetFacaClient()
        {
            string API_KEY = "hDFL0bNfdMtIj1B5oCEyfHMG";
            string SECRET_KEY = "QygQEX73YOYL23IQIE3GLdyV0PnA7z43";
            Face client = new Face(API_KEY, SECRET_KEY);

            client.Timeout = 60000;  // 修改超时时间
            return client;
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
            Result<int> resultObj = new Result<int>();
            resultObj.Data = -1;
            BasicDataContext basedb = new BasicDataContext();
            BusinessDataContext busdb = new BusinessDataContext();
            try
            {
                var imageType = "BASE64";
                var groupId = "group1";
                string control = "LOW";
                image = image.Substring(image.IndexOf(",") + 1);      //将‘，’以前的多余字符串删除
                // 如果有可选参数
                var options = new Dictionary<string, object>{
                                    {"quality_control", control},
                                    {"liveness_control", control}
                                };
                Face client = GetFacaClient();
                JObject result = new JObject();

                //人脸管理
                EPM_AIUserFace userFaceAI = new EPM_AIUserFace();
                //人脸日志表
                EPM_FaceOperateLog faceLog = new EPM_FaceOperateLog();
                //查询用户详情
                var userResult = DataOperateBasic<Base_User>.Get(basedb).GetModel(userId);
                //查询该用户是否已经注册的人脸信息
                userFaceAI = DataOperateBusiness<EPM_AIUserFace>.Get(busdb).GetList(t => t.UserId == userId).FirstOrDefault();
                //已存在人脸更新现有数据
                //是否已经存在人脸
                bool isExitsUserFace = false;
                //是否添加人脸
                bool isAddFace = false;
                if (userFaceAI != null && userFaceAI.IsSuccess)
                {
                    faceLog.APIType = FaceOperate.Update.ToString();
                    isExitsUserFace = true;
                }
                else  //不存在添加新数据
                {
                    isAddFace = true;
                    faceLog.APIType = FaceOperate.Add.ToString();
                    if (userFaceAI == null)
                    {
                        userFaceAI = new EPM_AIUserFace();
                        isExitsUserFace = false;
                    }
                    else
                    {
                        isExitsUserFace = true;
                    }
                    userFaceAI.IsSuccess = true;
                    userFaceAI.UserId = userId;
                    userFaceAI.UserName = userResult.UserName;
                    userFaceAI.UserPhone = userResult.Phone;
                    userFaceAI.ImageType = imageType;
                    userFaceAI.GroupId = groupId;
                    userFaceAI.LivenessControl = control;
                    userFaceAI.QualityControl = control;
                    userFaceAI.Source = source;
                }
                userFaceAI.ImageBase64 = "data:image/jpeg;base64," + image;
                //日志
                var requestJson = new
                {
                    imageType = imageType,
                    groupId = groupId,
                    control = control,
                    userId = userId,
                    options = new
                    {
                        quality_control = control,
                        liveness_control = control
                    }
                };
                faceLog.ModelId = userFaceAI.Id;
                faceLog.RequestJson = requestJson.ToString();
                faceLog = SetCurrentUser(faceLog);
                try
                {
                    if (isAddFace)
                    {
                        result = client.UserAdd(image, imageType, groupId, userId.ToString(), options);
                    }
                    else
                    {
                        result = client.UserUpdate(image, imageType, groupId, userId.ToString(), options);
                    }

                    userFaceAI.LogId = result["log_id"].ToString();

                    if (result["error_code"].ToString() == "0" && result["error_msg"].ToString() == "SUCCESS")
                    {
                        var result_list = Newtonsoft.Json.JsonConvert.DeserializeObject(result["result"].ToString()) as JObject;

                        var result_location = Newtonsoft.Json.JsonConvert.DeserializeObject(result_list["location"].ToString()) as JObject;

                        userFaceAI.FaceToken = result_list["face_token"].ToString();
                        userFaceAI.Left = Convert.ToDecimal(result_location["left"]);
                        userFaceAI.Top = Convert.ToDecimal(result_location["top"]);
                        userFaceAI.Width = Convert.ToDecimal(result_location["width"]);
                        userFaceAI.Height = Convert.ToDecimal(result_location["height"]);
                        userFaceAI.Rotation = result_location["rotation"].ToString();
                        userFaceAI.Location = result_list["location"].ToString();
                        userFaceAI.IsSuccess = true;
                        userFaceAI = SetCurrentUser(userFaceAI);

                        faceLog.IsSuccess = true;

                        resultObj.Data = 1;
                    }
                    else
                    {
                        faceLog.IsSuccess = false;

                        userFaceAI.IsSuccess = false;
                    }
                }
                catch (Exception)
                {
                    faceLog.IsSuccess = false;
                    userFaceAI.IsSuccess = false;
                }
                faceLog.ResponseJson = result.ToString();
                if (isExitsUserFace)
                {
                    DataOperateBusiness<EPM_AIUserFace>.Get(busdb).Update(userFaceAI);
                }
                else
                {
                    DataOperateBusiness<EPM_AIUserFace>.Get(busdb).Add(userFaceAI);
                }
                DataOperateBusiness<EPM_FaceOperateLog>.Get(busdb).Add(faceLog);
                resultObj.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                resultObj.Data = -1;
                resultObj.Flag = EResultFlag.Failure;
                resultObj.Exception = new ExceptionEx(ex, "AddAIUserFaceInfo");
            }
            finally
            {
                if (basedb.Database.Connection.State != ConnectionState.Closed)
                {
                    basedb.Database.Connection.Close();
                    basedb.Database.Connection.Dispose();
                }
                if (busdb.Database.Connection.State != ConnectionState.Closed)
                {
                    busdb.Database.Connection.Close();
                    busdb.Database.Connection.Dispose();
                }
            }

            return resultObj;
        }

        private Result<int> DelAIUserFaceInfo(long userId, string faceToken)
        {
            Result<int> resultObj = new Result<int>();
            resultObj.Data = -1;
            BusinessDataContext busdb = new BusinessDataContext();
            try
            {
                var groupId = "group1";
                Face client = GetFacaClient();
                JObject result = new JObject();
                //人脸日志表
                EPM_FaceOperateLog faceLog = new EPM_FaceOperateLog();
                //日志
                var requestJson = new
                {
                    groupId = groupId,
                    userId = userId,
                    faceToken = faceToken
                };
                faceLog.ModelId = 0;
                faceLog.RequestJson = requestJson.ToString();
                faceLog.APIType = FaceOperate.Delete.ToString();
                faceLog = SetCurrentUser(faceLog);
                try
                {
                    result = client.FaceDelete(userId.ToString(), groupId, faceToken);

                    if (result["error_code"].ToString() == "0" && result["error_msg"].ToString() == "SUCCESS")
                    {
                        faceLog.IsSuccess = true;

                        resultObj.Data = 1;
                    }
                    else
                    {
                        faceLog.IsSuccess = false;
                    }
                }
                catch (Exception)
                {
                    faceLog.IsSuccess = false;
                }
                faceLog.ResponseJson = result.ToString();
                DataOperateBusiness<EPM_FaceOperateLog>.Get(busdb).Add(faceLog);
                resultObj.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                resultObj.Data = -1;
                resultObj.Flag = EResultFlag.Failure;
                resultObj.Exception = new ExceptionEx(ex, "DelAIUserFaceInfo");
            }
            finally
            {
                if (busdb.Database.Connection.State != ConnectionState.Closed)
                {
                    busdb.Database.Connection.Close();
                    busdb.Database.Connection.Dispose();
                }
            }

            return resultObj;
        }
    }
}
