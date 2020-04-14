using AutoMapper;
using hc.epm.Common;
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

namespace hc.epm.Service.Business
{
    public partial class BusinessService : BaseService, IBusinessService
    {
        private BusinessDataContext context = new BusinessDataContext();
        private BasicDataContext contextBase = new BasicDataContext();
        /// <summary>
        /// 添加项目资料信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddDataConfig(Epm_DataConfig model, List<Base_Files> fileList = null)
        {
            model = base.SetCurrentUser(model);
            Result<int> result = new Result<int>();
            try
            {
                bool dConfig = DataOperateBusiness<Epm_DataConfig>.Get().Count(i => i.Name == model.Name) > 0;
                if (dConfig)
                {
                    throw new Exception("该资料名称已经存在");
                }
                dConfig = DataOperateBusiness<Epm_DataConfig>.Get().Count(i => i.Code == model.Code) > 0;
                if (dConfig)
                {
                    throw new Exception("该资料编码已经存在");
                }
                var rows = DataOperateBusiness<Epm_DataConfig>.Get().Add(model);

                //新增附件
                AddFilesByTable(model, fileList);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Add.GetText(), "新增项目资料信息:" + model.Id + ":" + model.Name);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddDataConfig");
            }
            return result;
        }

        /// <summary>
        /// 修改项目资料信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateDataConfig(Epm_DataConfig model, List<Base_Files> fileList = null)
        {
            var oldModel = DataOperateBusiness<Epm_DataConfig>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);

            Result<int> result = new Result<int>();
            try
            {
                bool dConfig = DataOperateBusiness<Epm_DataConfig>.Get().Count(i => i.Name == model.Name && i.Id != model.Id) > 0;
                if (dConfig)
                {
                    throw new Exception("该资料名称已经存在");
                }
                dConfig = DataOperateBusiness<Epm_DataConfig>.Get().Count(i => i.Code == model.Code && i.Id != model.Id) > 0;
                if (dConfig)
                {
                    throw new Exception("该资料编码已经存在");
                }

                if (fileList != null)
                {
                    //删除之前的附件
                    DeleteFilesByTable(model.GetType().Name, new List<long>() { model.Id });
                    //新增附件
                    AddFilesByTable(model, fileList);
                }

                var rows = DataOperateBusiness<Epm_DataConfig>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Modify.GetText(), "修改项目资料信息:" + model.Id + ":" + model.Name);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateDataConfig");
            }
            return result;
        }

        /// <summary>
        /// 删除项目资料信息
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleMilestoneIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_DataConfig>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                foreach (var model in models)
                {
                    model.OperateUserId = CurrentUserID.ToLongReq();
                    model.OperateUserName = CurrentUserName;
                    model.OperateTime = DateTime.Now;
                    model.DeleteTime = DateTime.Now;
                }
                var rows = DataOperateBusiness<Epm_DataConfig>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Delete.GetText(), "删除项目资料信息:" + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteDataConfigbyIds");
            }
            return result;
        }

        /// <summary>
        /// 获取项目资料列表（无条件）
        /// </summary>
        /// <returns></returns>
        public Result<List<Epm_DataConfig>> GetDataConfigList()
        {
            Result<List<Epm_DataConfig>> result = new Result<List<Epm_DataConfig>>();
            try
            {
                var list = DataOperateBusiness<Epm_DataConfig>.Get().GetList().ToList();
                result.Data = list;
                result.AllRowsCount = list.Count();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetDataConfigList");
            }
            return result;
        }

        /// <summary>
        /// 获取项目资料信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_DataConfig> GetDataConfigById(long id)
        {
            Result<Epm_DataConfig> result = new Result<Epm_DataConfig>();
            try
            {
                var rows = DataOperateBusiness<Epm_DataConfig>.Get().GetModel(id);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetDataConfigModel");
            }
            return result;
        }

        /// <summary>
        /// 获取项目资料列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_DataConfig>> GetDataConfigListWhr(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_DataConfig>> result = new Result<List<Epm_DataConfig>>();
            try
            {
                result = DataOperate.QueryListSimple<Epm_DataConfig>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetDataConfigListWhr");
            }
            return result;
        }

        public Result<List<Epm_Milepost>> GetMilepostListQc(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_Milepost>> result = new Result<List<Epm_Milepost>>();
            try
            {
                result = DataOperate.QueryListSimple<Epm_Milepost>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMilepostList");
            }
            return result;
        }

        /// <summary>
        /// 添加里程碑（单表添加）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddMilepost(Epm_Milepost model)
        {
            model = base.SetCurrentUser(model);
            Result<int> result = new Result<int>();
            try
            {
                bool dConfig = DataOperateBusiness<Epm_Milepost>.Get().Count(i => i.Name == model.Name) > 0;
                if (dConfig)
                {
                    throw new Exception("该工程节点名称已经存在");
                }
                //dConfig = DataOperateBusiness<Epm_Milepost>.Get().Count(i => i.Code == model.Code) > 0;
                //if (dConfig)
                //{
                //    throw new Exception("该里程碑编码已经存在");
                //}
                var rows = DataOperateBusiness<Epm_Milepost>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Add.GetText(), "添加工程节点:" + model.Id + ":" + model.Name);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddMilepost");
            }
            return result;
        }

        /// <summary>
        /// 添加里程碑（多表添加）
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> AddMilepostAndData(Epm_Milepost model, List<long> ids)
        {
            model = base.SetCurrentUser(model);
            Result<int> result = new Result<int>();
            try
            {
                bool dConfig = DataOperateBusiness<Epm_Milepost>.Get().Count(i => i.Name == model.Name && i.ParentId == model.ParentId) > 0;
                if (dConfig)
                {
                    throw new Exception("该工程节点称已经存在");
                }
                //dConfig = DataOperateBusiness<Epm_Milepost>.Get().Count(i => i.Code == model.Code) > 0;
                //if (dConfig)
                //{
                //    throw new Exception("该里程碑编码已经存在");
                //}
                //添加里程碑表数据
                var rows = DataOperateBusiness<Epm_Milepost>.Get().Add(model);
                //获取项目资料信息
                var dataConfig = DataOperateBusiness<Epm_DataConfig>.Get().GetList(t => ids.Contains(t.Id)).ToList();

                List<Epm_MilepostData> milepostDataList = new List<Epm_MilepostData>();
                foreach (var item in dataConfig)
                {
                    Epm_MilepostData temp = new Epm_MilepostData();
                    temp = base.SetCurrentUser(temp);
                    temp.MilepostId = model.Id;
                    temp.MilepostName = model.Name;
                    temp.DataCode = item.Code;
                    temp.DataConfigId = item.Id;
                    temp.DataName = item.Name;

                    milepostDataList.Add(temp);
                }
                //批量增加里程碑项目资料关联表
                rows = DataOperateBusiness<Epm_MilepostData>.Get().AddRange(milepostDataList);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Add.GetText(), "添加工程节点:" + model.Id + ":" + model.Name);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddMilepostAndData");
            }
            return result;
        }
        /// <summary>
        /// 添加里程碑关联资料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddMilepostData(Epm_MilepostData model)
        {
            model = base.SetCurrentUser(model);
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_MilepostData>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Add.GetText(), "添加工程节点关联资料:" + model.Id + ":" + model.DataName);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddMilepostData");
            }
            return result;
        }

        /// <summary>
        /// 修改里程碑
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> UpdateMilepost(Epm_Milepost model, List<long> ids)
        {
            var oldModel = DataOperateBusiness<Epm_Milepost>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);

            Result<int> result = new Result<int>();
            try
            {
                bool dConfig = DataOperateBusiness<Epm_Milepost>.Get().Count(i => i.Name == model.Name && i.Id != model.Id && i.ParentId == model.ParentId) > 0;
                if (dConfig)
                {
                    throw new Exception("该工程节点名称已经存在");
                }
                //dConfig = DataOperateBusiness<Epm_Milepost>.Get().Count(i => i.Code == model.Code && i.Id != model.Id) > 0;
                //if (dConfig)
                //{
                //    throw new Exception("该里程碑编码已经存在");
                //}
                //修改里程碑表数据
                var rows = DataOperateBusiness<Epm_Milepost>.Get().Update(model);

                //删除里程碑项目资料关联表数据
                var milepostDataList = DataOperateBusiness<Epm_MilepostData>.Get().GetList(t => t.MilepostId == model.Id).ToList();

                foreach (var item in milepostDataList)
                {
                    item.OperateUserId = CurrentUserID.ToLongReq();
                    item.OperateUserName = CurrentUserName;
                    item.OperateTime = DateTime.Now;
                    item.DeleteTime = DateTime.Now;
                }
                rows = DataOperateBusiness<Epm_MilepostData>.Get().DeleteRange(milepostDataList);

                //获取项目资料信息
                var dataConfig = DataOperateBusiness<Epm_DataConfig>.Get().GetList(t => ids.Contains(t.Id)).ToList();

                List<Epm_MilepostData> newMilepostDataList = new List<Epm_MilepostData>();
                foreach (var item in dataConfig)
                {
                    Epm_MilepostData temp = new Epm_MilepostData();
                    temp = base.SetCurrentUser(temp);
                    temp.MilepostId = model.Id;
                    temp.MilepostName = model.Name;
                    temp.DataCode = item.Code;
                    temp.DataConfigId = item.Id;
                    temp.DataName = item.Name;

                    newMilepostDataList.Add(temp);
                }
                //批量增加里程碑项目资料关联表
                rows = DataOperateBusiness<Epm_MilepostData>.Get().AddRange(newMilepostDataList);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Modify.GetText(), "修改工程节点:" + model.Id + ":" + model.Name);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateMilepost");
            }
            return result;
        }

        /// <summary>
        /// 修改里程碑关联资料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateMilepostData(Epm_MilepostData model)
        {
            var oldModel = DataOperateBusiness<Epm_MilepostData>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);

            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_MilepostData>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Modify.GetText(), "修改工程节点关联资料:" + model.Id + ":" + model.DataName);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateMilepostData");
            }
            return result;
        }

        /// <summary>
        /// 删除里程碑
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteMilepostbyIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_Milepost>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                foreach (var model in models)
                {
                    //判断是否存在里程碑下还存在子级，如存在则不能删除
                    var list = DataOperateBusiness<Epm_Milepost>.Get().GetList(i => i.ParentId == model.Id).ToList();
                    if (list.Count > 0)
                    {
                        throw new Exception("父级工程节点中存在子级");
                    }
                    model.OperateUserId = CurrentUserID.ToLongReq();
                    model.OperateUserName = CurrentUserName;
                    model.OperateTime = DateTime.Now;
                    model.DeleteTime = DateTime.Now;
                }
                var rows = DataOperateBusiness<Epm_Milepost>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Delete.GetText(), "删除工程节点:" + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteMilepostbyIds");
            }
            return result;
        }

        /// <summary>
        /// 删除里程碑关联资料
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteMilepostDatabyIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_MilepostData>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                foreach (var model in models)
                {
                    model.OperateUserId = CurrentUserID.ToLongReq();
                    model.OperateUserName = CurrentUserName;
                    model.OperateTime = DateTime.Now;
                    model.DeleteTime = DateTime.Now;
                }
                var rows = DataOperateBusiness<Epm_MilepostData>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Delete.GetText(), "删除工程节点关联资料:" + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteMilepostDatabyIds");
            }
            return result;
        }

        /// <summary>
        /// 获取里程碑列表
        /// </summary>
        /// <returns></returns>
        public Result<List<Epm_Milepost>> GetMilepostList()
        {
            Result<List<Epm_Milepost>> result = new Result<List<Epm_Milepost>>();
            try
            {
                var list = DataOperateBusiness<Epm_Milepost>.Get().GetList().ToList();
                result.Data = list;
                result.AllRowsCount = list.Count();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMilepostList");
            }
            return result;
        }
        /// <summary>
        /// 获取里程碑信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_Milepost> GetMilepostById(long id)
        {
            Result<Epm_Milepost> result = new Result<Epm_Milepost>();
            try
            {
                var rows = DataOperateBusiness<Epm_Milepost>.Get().GetModel(id);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMilepostById");
            }
            return result;
        }

        /// 查询油站信息列表
        /// </summary> 
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_OilStation>> GetOilStationList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_OilStation>> result = new Result<List<Epm_OilStation>>();
            try
            {
                result = DataOperate.QueryListSimple<Epm_OilStation>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetOilStationList");
            }
            return result;
        }
        public Result<List<Base_VideoManage>> GetVideoManageList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Base_VideoManage>> result = new Result<List<Base_VideoManage>>();
            try
            {
                result = DataOperate.QueryListSimple<Base_VideoManage>(contextBase, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetVideoManageList");
            }
            return result;
        }
        /// 查询所有有效项目
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_Project>> GetIndexProject(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_Project>> result = new Result<List<Epm_Project>>();
            try
            {
                result = DataOperate.QueryListSimple<Epm_Project>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetIndexProject");
            }
            return result;
        }

        /// <summary>
        /// 查询油站信息 GetBaseVideoManageById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_OilStation> GetOilStationById(long id)
        {
            Result<Epm_OilStation> result = new Result<Epm_OilStation>();
            try
            {
                var rows = DataOperateBusiness<Epm_OilStation>.Get().GetModel(id);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetOilStationById");
            }
            return result;
        }
        public Result<Base_VideoManage> GetBaseVideoManageById(long id)
        {
            Result<Base_VideoManage> result = new Result<Base_VideoManage>();
            try
            {
                var rows = DataOperateBasic<Base_VideoManage>.Get().GetModel(id);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetBaseVideoManageById");
            }
            return result;
        }
        /// <summary>
        /// 获取里程
        /// </summary>
        /// <returns></returns>
        public Result<List<MilepostView>> GetMilepostViewList(long parentId, int pageIndex, int pageSize)
        {
            List<MilepostView> list = new List<MilepostView>();
            Result<List<MilepostView>> result = new Result<List<MilepostView>>();
            try
            {
                List<Epm_Milepost> parents = DataOperateBusiness<Epm_Milepost>.Get().GetList(t => t.ParentId == 0).ToList();

                List<Epm_Milepost> mileposts = DataOperateBusiness<Epm_Milepost>.Get().GetList(t => ((t.ParentId != 0 && parentId == 0) || (t.ParentId == parentId && parentId != 0))).OrderBy(t => t.Sort).ToList();
                MilepostView milepost = null;
                foreach (var item in mileposts)
                {
                    milepost = new MilepostView();
                    milepost.Id = item.Id.ToString();
                    milepost.Name = item.Name;
                    milepost.ParentName = parents.Where(t => t.Id == item.ParentId).First().Name;
                    milepost.Sort = item.Sort;
                    milepost.State = item.State;
                    list.Add(milepost);
                }

                result.AllRowsCount = list.Count();
                list = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).OrderBy(t => t.ParentName).OrderBy(t => t.Sort).ToList();
                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMilepostViewList");
            }
            return result;
        }

        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <param name="belong"></param>
        /// <param name="pId"></param>
        /// <returns></returns>
        public MilepostView GetMilepostTree(long pId, string name, int state)
        {
            List<Epm_Milepost> result = new List<Epm_Milepost>();

            result = DataOperateBusiness<Epm_Milepost>.Get().GetList().OrderBy(t => t.Sort).ToList();
            MilepostView rootTree = new MilepostView();

            //var first = result.FirstOrDefault(i => i.Id == pId && i.Name.Contains(name) && (state == -1 || i.State == state));

            //rootTree = first.Id.ToString();
            //rootTree.Name = first.Name;
            //rootTree.Code = first.Code;
            //rootTree.Name = first.Name;
            //rootTree.Sort = first.Sort;
            //rootTree.State = first.State;

            //var tree = createTree(first.Id, result);

            //rootTree.children = tree;
            return rootTree;
        }
        /// <summary>
        /// 生成树
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="allList"></param>
        /// <returns></returns>
        private List<MilepostView> createTree(long parentId, List<Epm_Milepost> allList)
        {
            List<MilepostView> list = new List<MilepostView>();
            //var childList = allList.Where(i => i.ParentId == parentId).OrderBy(i => i.Sort).ToList();
            ////有子权限
            //if (childList != null && childList.Any())
            //{
            //    foreach (var item in childList)
            //    {
            //        MilepostView node = new MilepostView();
            //        node.checkboxValue = item.Id.ToString();
            //        node.Code = item.Code;
            //        node.Name = item.Name;
            //        node.Sort = item.Sort;
            //        node.State = item.State;

            //        var iteratorList = createTree(item.Id, allList);
            //        node.children = iteratorList;
            //        list.Add(node);
            //    }
            //}
            return list;
        }
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>返回添加的ID</returns>
        public Result<long> AddTemplate(Epm_Template model, List<Base_Files> fileList = null)
        {
            Result<long> result = new Result<long>();
            try
            {
                var id = DataOperateBusiness<Epm_Template>.Get().Add(model);
                //新增附件
                AddFilesByTable(model, fileList);
                result.Data = id;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminCategory.TemplateManage.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTemplate");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTemplate(Epm_Template model, List<Base_Files> fileList = null)
        {
            Result<int> result = new Result<int>();
            try
            {
                //删除之前的附件
                DeleteFilesByTable(model.GetType().Name, new List<long>() { model.Id });
                //新增附件
                AddFilesByTable(model, fileList);

                var rows = DataOperateBusiness<Epm_Template>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminCategory.TemplateManage.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTemplate");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTemplateByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_Template>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_Template>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminCategory.TemplateManage.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTemplateByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
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
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_Template> GetTemplateModel(long id)
        {
            Result<Epm_Template> result = new Result<Epm_Template>();
            try
            {
                var model = DataOperateBusiness<Epm_Template>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTemplateModel");
            }
            return result;
        }
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>返回ID</returns>
        public Result<long> AddTemplateDetails(Epm_TemplateDetails model)
        {
            Result<long> result = new Result<long>();
            try
            {
                var rows = DataOperateBusiness<Epm_TemplateDetails>.Get().Add(model);
                result.Data = model.Id;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminCategory.TemplateManage.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddTemplateDetails");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateTemplateDetails(Epm_TemplateDetails model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_TemplateDetails>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminCategory.TemplateManage.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateTemplateDetails");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteTemplateDetailsByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_TemplateDetails>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_TemplateDetails>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminCategory.TemplateManage.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteTemplateDetailsByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_TemplateDetails>> GetTemplateDetailsList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_TemplateDetails>> result = new Result<List<Epm_TemplateDetails>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_TemplateDetails>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTemplateDetailsList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_TemplateDetails> GetTemplateDetailsModel(long id)
        {
            Result<Epm_TemplateDetails> result = new Result<Epm_TemplateDetails>();
            try
            {
                var model = DataOperateBusiness<Epm_TemplateDetails>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetTemplateDetailsModel");
            }
            return result;
        }


        /// <summary>
        /// 根据里程碑id获取项目资料里程碑关联信息
        /// </summary>
        /// <param name="milepostId"></param>
        /// <returns></returns>
        public Result<List<Epm_MilepostData>> GetMilepostDataByMilepostId(long milepostId)
        {
            Result<List<Epm_MilepostData>> result = new Result<List<Epm_MilepostData>>();
            try
            {
                var list = DataOperateBusiness<Epm_MilepostData>.Get().GetList(t => t.MilepostId == milepostId).ToList();
                result.Data = list;
                result.AllRowsCount = list.Count();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMilepostDataByMilepostId");
            }
            return result;
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
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBusiness<Epm_DataConfig>.Get().GetModel(id);
                if (type == 1)
                {
                    model.IsRequire = state;
                }
                else
                {
                    int intState = state == true ? 1 : 0;
                    model.State = intState;
                }

                model.OperateUserId = CurrentUserID.ToLongReq();
                model.OperateUserName = CurrentUserName;
                model.OperateTime = DateTime.Now;

                var rows = DataOperateBusiness<Epm_DataConfig>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "ChangeDataConfigState");
            }
            return result;
        }

        /// <summary>
        /// 里程碑启用禁用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeMilepostState(long id, bool state)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBusiness<Epm_Milepost>.Get().GetModel(id);
                int intState = state == true ? 0 : 1;
                model.State = intState;

                model.OperateUserId = CurrentUserID.ToLongReq();
                model.OperateUserName = CurrentUserName;
                model.OperateTime = DateTime.Now;

                var rows = DataOperateBusiness<Epm_Milepost>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "ChangeMilepostState");
            }
            return result;
        }

        /// <summary>
        /// 获取新闻类型列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_NewTarget>> GetNewTargetListWhr(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_NewTarget>> result = new Result<List<Epm_NewTarget>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_NewTarget>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetNewTargetListWhr");
            }
            return result;
        }

        /// <summary>
        /// 获取新闻类型列表
        /// </summary>
        /// <returns></returns>
        public Result<List<Epm_NewTarget>> GetNewTargetList()
        {
            Result<List<Epm_NewTarget>> result = new Result<List<Epm_NewTarget>>();
            try
            {
                var list = DataOperateBusiness<Epm_NewTarget>.Get().GetList().ToList();
                result.Data = list;
                result.AllRowsCount = list.Count();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetNewTargetList");
            }
            return result;
        }

        /// <summary>
        /// 获取新闻分类详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_NewTarget> GetNewTargetById(long id)
        {
            Result<Epm_NewTarget> result = new Result<Epm_NewTarget>();
            try
            {
                var model = DataOperateBusiness<Epm_NewTarget>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetNewTargetById");
            }
            return result;

        }

        /// <summary>
        /// 添加新闻类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddNewTarget(Epm_NewTarget model)
        {
            model = base.SetCurrentUser(model);
            Result<int> result = new Result<int>();
            try
            {
                bool dConfig = DataOperateBusiness<Epm_NewTarget>.Get().Count(i => i.TargetName == model.TargetName) > 0;
                if (dConfig)
                {
                    throw new Exception("该类别名称已经存在");
                }
                dConfig = DataOperateBusiness<Epm_NewTarget>.Get().Count(i => i.TargetCode == model.TargetCode) > 0;
                if (dConfig)
                {
                    throw new Exception("该类别编号已经存在");
                }
                var rows = DataOperateBusiness<Epm_NewTarget>.Get().Add(model);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Add.GetText(), "添加新闻类型:" + model.Id + ":" + model.TargetName);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddNewTarget");
            }
            return result;
        }

        /// <summary>
        /// 修改新闻类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateNewTarget(Epm_NewTarget model)
        {
            var oldModel = DataOperateBusiness<Epm_NewTarget>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);

            Result<int> result = new Result<int>();
            try
            {
                bool dConfig = DataOperateBusiness<Epm_NewTarget>.Get().Count(i => i.TargetName == model.TargetName && i.Id != model.Id) > 0;
                if (dConfig)
                {
                    throw new Exception("该类别名称已经存在");
                }
                dConfig = DataOperateBusiness<Epm_NewTarget>.Get().Count(i => i.TargetCode == model.TargetCode && i.Id != model.Id) > 0;
                if (dConfig)
                {
                    throw new Exception("该类别编号已经存在");
                }

                var rows = DataOperateBusiness<Epm_NewTarget>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Modify.GetText(), "修改新闻类型:" + model.Id + ":" + model.TargetName);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateNewTarget");
            }
            return result;
        }

        /// <summary>
        /// 修改新闻类型状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeNewTargetState(long id, int state)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBusiness<Epm_NewTarget>.Get().GetModel(id);
                model.State = state;

                model.OperateUserId = CurrentUserID.ToLongReq();
                model.OperateUserName = CurrentUserName;
                model.OperateTime = DateTime.Now;

                var rows = DataOperateBusiness<Epm_NewTarget>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "ChangeNewTargetState");
            }
            return result;
        }

        /// <summary>
        /// 删除新闻类型
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteNewTargetByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_NewTarget>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                foreach (var model in models)
                {
                    model.OperateUserId = CurrentUserID.ToLongReq();
                    model.OperateUserName = CurrentUserName;
                    model.OperateTime = DateTime.Now;
                    model.DeleteTime = DateTime.Now;
                }
                var rows = DataOperateBusiness<Epm_NewTarget>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Delete.GetText(), "删除新闻类型:" + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteNewTargetByIds");
            }
            return result;
        }

        /// <summary>
        /// 获取新闻列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_News>> GetNewsList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            qc.PageInfo.OrderAndSortList = "";
            Result<List<Epm_News>> result = new Result<List<Epm_News>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_News>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetNewsList");
            }
            return result;
        }

        /// <summary>
        /// 获取新闻详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_News> GetNewsById(long id)
        {
            Result<Epm_News> result = new Result<Epm_News>();
            try
            {
                var model = DataOperateBusiness<Epm_News>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetNewsById");
            }
            return result;

        }

        /// <summary>
        /// 添加新闻
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList"></param>
        /// <returns></returns>
        public Result<int> AddNews(Epm_News model, List<Base_Files> fileList = null)
        {
            model = base.SetCurrentUser(model);
            Result<int> result = new Result<int>();
            try
            {
                bool dConfig = DataOperateBusiness<Epm_News>.Get().Count(i => i.NewsTitle == model.NewsTitle) > 0;
                if (dConfig)
                {
                    throw new Exception("该新闻标题已经存在");
                }
                var rows = DataOperateBusiness<Epm_News>.Get().Add(model);

                //新增附件
                AddFilesByTable(model, fileList);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Add.GetText(), "添加新闻:" + model.Id + ":" + model.NewsTitle);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddNews");
            }
            return result;
        }

        /// <summary>
        /// 修改新闻
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateNews(Epm_News model, List<Base_Files> fileList = null)
        {
            var oldModel = DataOperateBusiness<Epm_News>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);

            Result<int> result = new Result<int>();
            try
            {
                bool dConfig = DataOperateBusiness<Epm_News>.Get().Count(i => i.NewsTitle == model.NewsTitle && i.Id != model.Id) > 0;
                if (dConfig)
                {
                    throw new Exception("该新闻标题已经存在");
                }

                if (fileList != null)
                {
                    //删除之前的附件
                    DeleteFilesByTable(model.GetType().Name, new List<long>() { model.Id });
                    //新增附件
                    AddFilesByTable(model, fileList);
                }

                var rows = DataOperateBusiness<Epm_News>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Modify.GetText(), "修改新闻:" + model.Id + ":" + model.NewsTitle);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateNews");
            }
            return result;
        }

        /// <summary>
        /// 删除新闻
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteNewsByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_News>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                foreach (var model in models)
                {
                    model.OperateUserId = CurrentUserID.ToLongReq();
                    model.OperateUserName = CurrentUserName;
                    model.OperateTime = DateTime.Now;
                    model.DeleteTime = DateTime.Now;
                }
                var rows = DataOperateBusiness<Epm_News>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Delete.GetText(), "删除新闻:" + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteNewsByIds");
            }
            return result;
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
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBusiness<Epm_News>.Get().GetModel(id);
                if (type == 1)
                {
                    model.IsTop = state;
                }
                else
                {
                    model.IsPublish = state;
                }

                model.OperateUserId = CurrentUserID.ToLongReq();
                model.OperateUserName = CurrentUserName;
                model.OperateTime = DateTime.Now;

                var rows = DataOperateBusiness<Epm_News>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "ChangeNewsState");
            }
            return result;
        }

        #region 广告位管理方法
        /// <summary>
        /// 获取广告位列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_AdTarget>> GetAdTargetListWhr(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_AdTarget>> result = new Result<List<Epm_AdTarget>>();
            try
            {
                result = DataOperate.QueryListSimple<Epm_AdTarget>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetAdTargetListWhr");
            }
            return result;
        }

        /// <summary>
        /// 获取广告位列表
        /// </summary>
        /// <returns></returns>
        public Result<List<Epm_AdTarget>> GetAdTargetList()
        {
            Result<List<Epm_AdTarget>> result = new Result<List<Epm_AdTarget>>();
            try
            {
                var list = DataOperateBusiness<Epm_AdTarget>.Get().GetList().ToList();
                result.Data = list;
                result.AllRowsCount = list.Count();
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetAdTargetList");
            }
            return result;
        }

        /// <summary>
        /// 获取广告位详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_AdTarget> GetAdTargetById(long id)
        {
            Result<Epm_AdTarget> result = new Result<Epm_AdTarget>();
            try
            {
                var model = DataOperateBusiness<Epm_AdTarget>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetAdTargetById");
            }
            return result;

        }

        /// <summary>
        /// 添加广告位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> AddAdTarget(Epm_AdTarget model)
        {
            model = base.SetCurrentUser(model);
            Result<int> result = new Result<int>();
            try
            {
                bool dConfig = DataOperateBusiness<Epm_AdTarget>.Get().Count(i => i.Name == model.Name) > 0;
                if (dConfig)
                {
                    throw new Exception("该广告位名称已经存在");
                }
                dConfig = DataOperateBusiness<Epm_AdTarget>.Get().Count(i => i.TargetNum == model.TargetNum) > 0;
                if (dConfig)
                {
                    throw new Exception("该广告位编码已经存在");
                }
                var rows = DataOperateBusiness<Epm_AdTarget>.Get().Add(model);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Add.GetText(), "添加广告位:" + model.Id + ":" + model.Name);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddAdTarget");
            }
            return result;
        }

        /// <summary>
        /// 修改广告位
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateAdTarget(Epm_AdTarget model)
        {
            var oldModel = DataOperateBusiness<Epm_AdTarget>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);

            Result<int> result = new Result<int>();
            try
            {
                bool dConfig = DataOperateBusiness<Epm_AdTarget>.Get().Count(i => i.Name == model.Name && i.Id != model.Id) > 0;
                if (dConfig)
                {
                    throw new Exception("该广告位名称已经存在");
                }
                dConfig = DataOperateBusiness<Epm_AdTarget>.Get().Count(i => i.TargetNum == model.TargetNum && i.Id != model.Id) > 0;
                if (dConfig)
                {
                    throw new Exception("该广告位编号已经存在");
                }

                var rows = DataOperateBusiness<Epm_AdTarget>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Modify.GetText(), "修改广告位:" + model.Id + ":" + model.Name);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateAdTarget");
            }
            return result;
        }

        /// <summary>
        /// 修改广告位状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeAdTargetState(long id, int state)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBusiness<Epm_AdTarget>.Get().GetModel(id);
                model.State = state;

                model.OperateUserId = CurrentUserID.ToLongReq();
                model.OperateUserName = CurrentUserName;
                model.OperateTime = DateTime.Now;

                var rows = DataOperateBusiness<Epm_AdTarget>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "ChangeAdTargetState");
            }
            return result;
        }

        /// <summary>
        /// 删除广告位
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteAdTargetByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_AdTarget>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                foreach (var model in models)
                {
                    model.OperateUserId = CurrentUserID.ToLongReq();
                    model.OperateUserName = CurrentUserName;
                    model.OperateTime = DateTime.Now;
                    model.DeleteTime = DateTime.Now;
                }
                var rows = DataOperateBusiness<Epm_AdTarget>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Delete.GetText(), "删除广告位:" + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteAdTargetByIds");
            }
            return result;
        }
        #endregion


        /// <summary>
        /// 获取广告投放列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_AdPutRecord>> GetAdPutRecordList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_AdPutRecord>> result = new Result<List<Epm_AdPutRecord>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_AdPutRecord>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetAdPutRecordList");
            }
            return result;
        }

        /// <summary>
        /// 获取广告投放详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result<Epm_AdPutRecord> GetAdPutRecordById(long id)
        {
            Result<Epm_AdPutRecord> result = new Result<Epm_AdPutRecord>();
            try
            {
                var model = DataOperateBusiness<Epm_AdPutRecord>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetAdPutRecordById");
            }
            return result;

        }

        /// <summary>
        /// 添加广告投放
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fileList"></param>
        /// <returns></returns>
        public Result<int> AddAdPutRecord(Epm_AdPutRecord model, List<Base_Files> fileList = null)
        {
            model = base.SetCurrentUser(model);
            Result<int> result = new Result<int>();
            try
            {
                bool dConfig = DataOperateBusiness<Epm_AdPutRecord>.Get().Count(i => i.AdName == model.AdName) > 0;
                if (dConfig)
                {
                    throw new Exception("该广告名称已经存在");
                }
                var rows = DataOperateBusiness<Epm_AdPutRecord>.Get().Add(model);

                //新增附件
                AddFilesByTable(model, fileList);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Add.GetText(), "添加广告投放:" + model.Id + ":" + model.AdName);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddAdPutRecord");
            }
            return result;
        }

        /// <summary>
        /// 修改广告投放
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Result<int> UpdateAdPutRecord(Epm_AdPutRecord model, List<Base_Files> fileList = null)
        {
            var oldModel = DataOperateBusiness<Epm_AdPutRecord>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);

            Result<int> result = new Result<int>();
            try
            {
                bool dConfig = DataOperateBusiness<Epm_AdPutRecord>.Get().Count(i => i.AdName == model.AdName && i.Id != model.Id) > 0;
                if (dConfig)
                {
                    throw new Exception("该广告名称已经存在");
                }

                if (fileList != null)
                {
                    //删除之前的附件
                    DeleteFilesByTable(model.GetType().Name, new List<long>() { model.Id });
                    //新增附件
                    AddFilesByTable(model, fileList);
                }

                var rows = DataOperateBusiness<Epm_AdPutRecord>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Modify.GetText(), "修改广告投放:" + model.Id + ":" + model.AdName);

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateAdPutRecord");
            }
            return result;
        }

        /// <summary>
        /// 删除广告投放
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteAdPutRecordByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_AdPutRecord>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                foreach (var model in models)
                {
                    model.OperateUserId = CurrentUserID.ToLongReq();
                    model.OperateUserName = CurrentUserName;
                    model.OperateTime = DateTime.Now;
                    model.DeleteTime = DateTime.Now;
                }
                var rows = DataOperateBusiness<Epm_AdPutRecord>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Delete.GetText(), "删除广告投放:" + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteAdPutRecordByIds");
            }
            return result;
        }

        /// <summary>
        /// 修改广告投放状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public Result<int> ChangeAdPutRecordState(long id, int state)
        {
            Result<int> result = new Result<int>();
            try
            {
                var model = DataOperateBusiness<Epm_AdPutRecord>.Get().GetModel(id);
                model.State = state;

                model.OperateUserId = CurrentUserID.ToLongReq();
                model.OperateUserName = CurrentUserName;
                model.OperateTime = DateTime.Now;

                var rows = DataOperateBusiness<Epm_AdPutRecord>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;

            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "ChangeAdPutRecordState");
            }
            return result;
        }
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddNotice(NoticeView model)
        {
            Result<int> result = new Result<int>();
            try
            {
                Epm_Notice notice = ViewToEmp(model);
                // using (TransactionScope ts = new TransactionScope())
                var rows = DataOperateBusiness<Epm_Notice>.Get().Add(notice);
                if (!string.IsNullOrEmpty(model.CompanyIds))
                {
                    var companyIds = model.CompanyIds.Split(',').ToLongList();
                    var companys = DataOperateBasic<Base_Company>.Get().GetList(t => companyIds.Contains(t.Id));
                    var noticeComany = new List<Epm_NoticeCompany>();
                    foreach (var item in companys)
                    {
                        var comany = new Epm_NoticeCompany();
                        comany.CompanyId = item.Id;
                        comany.CompanyName = item.Name;
                        comany.NoticeId = notice.Id;
                        noticeComany.Add(comany);
                    }
                    DataOperateBusiness<Epm_NoticeCompany>.Get().AddRange(noticeComany);
                }
                if (!string.IsNullOrEmpty(model.UserIds))
                {
                    var UserIds = model.UserIds.Split(',').ToLongList();
                    var Users = DataOperateBasic<Base_User>.Get().GetList(t => UserIds.Contains(t.Id));
                    var noticeUser = new List<Epm_NoticeUser>();
                    foreach (var item in Users)
                    {
                        var comany = new Epm_NoticeUser();
                        comany.UserId = item.Id;
                        comany.UserName = item.UserName;
                        comany.NoticeId = notice.Id;
                        noticeUser.Add(comany);
                    }
                    DataOperateBusiness<Epm_NoticeUser>.Get().AddRange(noticeUser);
                }
                if (!string.IsNullOrEmpty(model.ProjectIds))
                {
                    var companyIds = model.ProjectIds.Split(',').ToLongList();
                    var companys = DataOperateBusiness<Epm_Project>.Get().GetList(t => companyIds.Contains(t.Id));
                    var noticeComany = new List<Epm_NoticeProject>();
                    foreach (var item in companys)
                    {
                        var comany = new Epm_NoticeProject();
                        comany.ProjectId = item.Id;
                        comany.ProjectName = item.Name;
                        comany.NoticeId = notice.Id;
                        noticeComany.Add(comany);
                    }
                    DataOperateBusiness<Epm_NoticeProject>.Get().AddRange(noticeComany);
                }


                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Add.GetText(), "新增广告管理:" + model.Id + ":" + model.Title);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddNotice");
            }
            return result;
        }

        private Epm_Notice ViewToEmp(NoticeView model)
        {
            Epm_Notice notice = new Epm_Notice();
            notice = base.SetCurrentUser(notice);
            notice.Content = model.Content;
            notice.Title = model.Title;
            notice.Remark = model.Remark;
            notice.SendTime = notice.OperateTime;
            notice.SendUserId = notice.OperateUserId;
            notice.SendUserName = notice.OperateUserName;
            notice.State = model.State;
            notice.WayOfRelease = model.WayOfRelease;

            return notice;
        }

        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteNoticeByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_Notice>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_Notice>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.SystemParameter.GetText(), SystemRight.Delete.GetText(), "删除公告管理:" + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteNoticeByIds");
            }
            return result;
        }


        public Result<List<NoticeView>> GetNoticeViewList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<NoticeView>> result = new Result<List<NoticeView>>();
            try
            {
                //var res = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Notice>(context, qc);

                BuildDataCondition bd = new BuildDataCondition("Epm_Notice", qc);
                string querySQL = bd.BuildSQL();
                string countSQL = bd.BuildQueryCountSQL();
                var res = DataOperateBusiness<Epm_Notice>.Get().Select<Epm_Notice>(querySQL);
                int total = DataOperateBusiness<Epm_Notice>.Get().GetSingle(countSQL).ToString().ToInt32Req();
                List<NoticeView> list = new List<NoticeView>();
                if (res.Count > 0)
                {
                    List<long> ids = res.Select(t => t.Id).ToList();
                    var company = DataOperateBusiness<Epm_NoticeCompany>.Get().GetList(t => ids.Contains((long)t.NoticeId)).ToList();
                    var user = DataOperateBusiness<Epm_NoticeUser>.Get().GetList(t => ids.Contains((long)t.NoticeId)).ToList();
                    var project = DataOperateBusiness<Epm_NoticeProject>.Get().GetList(t => ids.Contains((long)t.NoticeId)).ToList();

                    foreach (var item in res)
                    {
                        NoticeView notice = NoticeEpmToView(item, company, user, project);
                        list.Add(notice);
                    }
                }
                result.AllRowsCount = total;
                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetNoticeList");
            }
            return result;
        }


        private static NoticeView NoticeEpmToView(Epm_Notice item, List<Epm_NoticeCompany> companys, List<Epm_NoticeUser> users, List<Epm_NoticeProject> projects)
        {
            //var company = DataOperateBusiness<Epm_NoticeCompany>.Get().GetList(t => item.Id == t.NoticeId);
            //var user = DataOperateBusiness<Epm_NoticeUser>.Get().GetList(t => item.Id == t.NoticeId);
            //var project = DataOperateBusiness<Epm_NoticeProject>.Get().GetList(t => item.Id == t.NoticeId);
            var company = companys.Where(t => item.Id == t.NoticeId);
            var user = users.Where(t => item.Id == t.NoticeId);
            var project = projects.Where(t => item.Id == t.NoticeId);

            NoticeView notice = new NoticeView();
            notice.CompanyNames = string.Join(",", company.Select(t => t.CompanyName));
            notice.CompanyIds = string.Join(",", company.Select(t => t.CompanyId));
            notice.UserNames = string.Join(",", user.Select(t => t.UserName));
            notice.UserIds = string.Join(",", user.Select(t => t.UserId));
            notice.ProjectIds = string.Join(",", project.Select(t => t.ProjectId));
            notice.ProjectNames = string.Join(",", project.Select(t => t.ProjectName));
            notice.Content = item.Content;
            notice.Id = item.Id;
            notice.Remark = item.Remark;
            notice.SendTime = item.SendTime;
            notice.SendUserId = item.SendUserId;
            notice.SendUserName = item.SendUserName;
            notice.State = item.State;
            notice.SId = item.SId;
            notice.Title = item.Title;
            notice.WayOfRelease = item.WayOfRelease;
            return notice;
        }

        public Result<List<Epm_Project>> GetProjectList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_Project>> result = new Result<List<Epm_Project>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Project>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectList");
            }
            return result;
        }
        /// <summary>
        /// 获取里程碑树形列表
        /// </summary>
        /// <returns></returns>
        public Result<List<MilepostView>> GetTemplateDetailsViewList(long templateId)
        {
            Result<List<MilepostView>> result = new Result<List<MilepostView>>();
            try
            {
                var listAll = DataOperateBusiness<Epm_TemplateDetails>.Get().GetList(t => t.TemplateId == templateId).ToList();
                List<MilepostView> list = new List<MilepostView>();
                //foreach (var item in listAll.Where(t => t.ParentId == 0))
                //{
                //    MilepostView rootTree = new MilepostView();
                //    rootTree.checkboxValue = item.Id.ToString();
                //    rootTree.Name = item.Name;
                //    rootTree.Sort = item.Sort;
                //    rootTree.State = item.State;
                //    rootTree.children = GetTempDetailsTree(item.Id, listAll);
                //    list.Add(rootTree);
                //}

                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMilepostViewList");
            }
            return result;
        }
        /// <summary>
        /// GetConstituteCompanyList
        /// </summary>
        /// <param name="belong"></param>
        /// <param name="pId"></param>
        /// <returns></returns>
        public List<MilepostView> GetTempDetailsTree(long pId, List<Epm_TemplateDetails> listTemp)
        {
            var result = listTemp.Where(t => t.ParentId == pId);
            List<MilepostView> list = new List<MilepostView>();
            //if (result != null && result.Any())
            //{
            //    foreach (var item in result)
            //    {
            //        MilepostView node = new MilepostView();
            //        node.checkboxValue = item.Id.ToString();
            //        node.Name = item.Name;
            //        node.Sort = item.Sort;
            //        node.State = item.State;
            //        var iteratorList = GetTempDetailsTree(item.Id, listTemp);
            //        node.children = iteratorList;
            //        list.Add(node);
            //    }
            //}
            return list; ;
        }


        #region  检查项
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddCheckItem(Epm_CheckItem model)
        {
            Result<int> result = new Result<int>();
            model = base.SetCurrentUser(model);
            model.CrtCompanyId = CurrentCompanyID.ToLongReq();
            model.CrtCompanyName = CurrentCompanyName;
            try
            {
                var rows = DataOperateBusiness<Epm_CheckItem>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.CheckItem.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddCheckItem");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateCheckItem(Epm_CheckItem model)
        {
            Result<int> result = new Result<int>();

            var oldModel = DataOperateBusiness<Epm_CheckItem>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);

            try
            {
                var rows = DataOperateBusiness<Epm_CheckItem>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.CheckItem.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateCheckItem");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteCheckItemByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_CheckItem>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_CheckItem>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.CheckItem.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteCheckItemByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_CheckItem>> GetCheckItemList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_CheckItem>> result = new Result<List<Epm_CheckItem>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_CheckItem>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCheckItemList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_CheckItem> GetCheckItemModel(long id)
        {
            Result<Epm_CheckItem> result = new Result<Epm_CheckItem>();
            try
            {
                var model = DataOperateBusiness<Epm_CheckItem>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetCheckItemModel");
            }
            return result;
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
            Result<int> result = new Result<int>();
            model = base.SetCurrentUser(model);
            try
            {
                bool dConfig = DataOperateBusiness<Epm_Constitute>.Get().Count(i => i.ProjectNatureCode == model.ProjectNatureCode && i.ConstituteKey == model.ConstituteKey && i.IsAProvide == model.IsAProvide) > 0;
                if (dConfig)
                {
                    throw new Exception("该项目性质已经存在该批复构成信息");
                }

                var rows = DataOperateBusiness<Epm_Constitute>.Get().Add(model);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.ReplyConstitute.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddConstitute");
            }
            return result;
        }

        /// <summary>
        /// 修改批复构成
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public Result<int> UpdateConstitute(Epm_Constitute model)
        {
            Result<int> result = new Result<int>();
            var oldModel = DataOperateBusiness<Epm_Constitute>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);
            try
            {
                bool dConfig = DataOperateBusiness<Epm_Constitute>.Get().Count(i => i.ProjectNatureCode == model.ProjectNatureCode && i.ConstituteKey == model.ConstituteKey && i.IsAProvide == model.IsAProvide && i.Id != model.Id) > 0;
                if (dConfig)
                {
                    throw new Exception("该项目性质已经存在该批复构成信息");
                }
                var rows = DataOperateBusiness<Epm_Constitute>.Get().Update(model);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.ReplyConstitute.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateConstitute");
            }
            return result;
        }

        /// <summary>
        /// 删除批复构成表
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteConstituteByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            var rows = 0;
            try
            {
                var models = DataOperateBusiness<Epm_Constitute>.Get().GetList(i => ids.Contains(i.Id)).ToList();

                rows = DataOperateBusiness<Epm_Constitute>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.ReplyConstitute.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteConstituteByIds");
            }
            return result;
        }

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_Constitute>> GetConstituteList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_Constitute>> result = new Result<List<Epm_Constitute>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Constitute>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetConstituteList");
            }
            return result;
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_Constitute> GetConstituteModel(long id)
        {
            Result<Epm_Constitute> result = new Result<Epm_Constitute>();
            try
            {
                result.Data = DataOperateBusiness<Epm_Constitute>.Get().GetModel(id);
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetConstituteModel");
            }
            return result;
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
            Result<int> result = new Result<int>();
            model = base.SetCurrentUser(model);
            try
            {
                bool dConfig = DataOperateBusiness<Epm_WorkMainPoints>.Get().Count(i => i.DicKey == model.DicKey && i.WorkMain == model.WorkMain) > 0;
                if (dConfig)
                {
                    throw new Exception("该工程内容已经存在");
                }

                var rows = DataOperateBusiness<Epm_WorkMainPoints>.Get().Add(model);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.ReplyConstitute.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddWorkMainPoints");
            }
            return result;
        }

        /// <summary>
        /// 修改批复构成
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public Result<int> UpdateWorkMainPoints(Epm_WorkMainPoints model)
        {
            Result<int> result = new Result<int>();
            var oldModel = DataOperateBusiness<Epm_WorkMainPoints>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);
            try
            {
                bool dConfig = DataOperateBusiness<Epm_WorkMainPoints>.Get().Count(i => i.DicKey == model.DicKey && i.WorkMain == model.WorkMain && i.Id != model.Id) > 0;
                if (dConfig)
                {
                    throw new Exception("该工程内容已经存在");
                }
                var rows = DataOperateBusiness<Epm_WorkMainPoints>.Get().Update(model);

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.ReplyConstitute.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateWorkMainPoints");
            }
            return result;
        }

        /// <summary>
        /// 删除批复构成表
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public Result<int> DeleteWorkMainPointsByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            var rows = 0;
            try
            {
                var models = DataOperateBusiness<Epm_WorkMainPoints>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                foreach (var item in models)
                {
                    item.DeleteTime = DateTime.Now;
                }
                rows = DataOperateBusiness<Epm_WorkMainPoints>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.ReplyConstitute.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteWorkMainPointsByIds");
            }
            return result;
        }

        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_WorkMainPoints>> GetWorkMainPointsList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_WorkMainPoints>> result = new Result<List<Epm_WorkMainPoints>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_WorkMainPoints>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetWorkMainPointsList");
            }
            return result;
        }

        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_WorkMainPoints> GetWorkMainPointsModel(long id)
        {
            Result<Epm_WorkMainPoints> result = new Result<Epm_WorkMainPoints>();

            try
            {
                result.Data = DataOperateBusiness<Epm_WorkMainPoints>.Get().GetModel(id);
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetWorkMainPointsModel");
            }
            return result;
        }

        #endregion

        /// <summary>
        /// 项目性质列表
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_ProjectNature>> GetProjectNatureList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_ProjectNature>> result = new Result<List<Epm_ProjectNature>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_ProjectNature>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetConstituteList");
            }
            return result;
        }

        #region 总批复构成与服务商关联

        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddConstituteCompany(ConstituteCompanyView view)
        {
            Result<int> result = new Result<int>();
            Epm_ConstituteCompany model = view.ConstituteCompany;
            model = base.SetCurrentUser(model);
            try
            {
                //判断批复构成是否已经设置服务商
                QueryCondition qc = new QueryCondition();
                qc.PageInfo.isAllowPage = false;
                qc = AddDefault(qc);
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ConstituteKey",
                    ExpValue = model.ConstituteKey,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
                var hasData = DataOperate.QueryListSimple<Epm_ConstituteCompany>(context, qc).Data;
                if (hasData != null && hasData.Count > 0)
                {
                    var has = hasData.Where(p => p.ConstituteKey == model.ConstituteKey).ToList();
                    if (has != null && has.Count > 0)
                    {
                        throw new Exception(model.ConstituteName + "已存在服务商设置");
                    }
                }

                var rows = DataOperateBusiness<Epm_ConstituteCompany>.Get().Add(model);

                #region 添加总批复及构成关联服务商
                List<Epm_ConstituteCompanyDetails> cwmpList = new List<Epm_ConstituteCompanyDetails>();
                if (view.ConstituteCompanyDetails != null)
                {
                    for (int i = 0; i < view.ConstituteCompanyDetails.Count; i++)
                    {
                        Epm_ConstituteCompanyDetails cwmp = new Epm_ConstituteCompanyDetails();
                        cwmp.CreateUserId = CurrentUserID.ToLongReq();
                        cwmp.CreateUserName = CurrentUserName;
                        cwmp.CreateTime = DateTime.Now;
                        cwmp = SetCurrentUser(cwmp);
                        cwmp = SetCreateUser(cwmp);
                        cwmp.ConstituteCompanyId = model.Id;
                        cwmp.CompanyId = view.ConstituteCompanyDetails[i].CompanyId;
                        cwmp.CompanyName = view.ConstituteCompanyDetails[i].CompanyName;
                        cwmp.ConstituteKey = view.ConstituteCompanyDetails[i].ConstituteKey;
                        cwmp.ConstituteName = view.ConstituteCompanyDetails[i].ConstituteName;
                        cwmpList.Add(cwmp);
                    }
                    //批量添加总批复及构成关联服务商
                    DataOperateBusiness<Epm_ConstituteCompanyDetails>.Get().AddRange(cwmpList);
                }
                #endregion

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.ReplyConstitute.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddConstituteCompany");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateConstituteCompany(ConstituteCompanyView view)
        {
            Result<int> result = new Result<int>();
            Epm_ConstituteCompany model = view.ConstituteCompany;

            var oldModel = DataOperateBusiness<Epm_ConstituteCompany>.Get().GetModel(model.Id);
            model = FiterUpdate(oldModel, model);
            try
            {
                //判断批复构成是否已经设置服务商
                QueryCondition qc = new QueryCondition();
                qc.PageInfo.isAllowPage = false;
                qc = AddDefault(qc);
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "ConstituteKey",
                    ExpValue = model.ConstituteKey,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.Equal
                });
                qc.ConditionList.Add(new ConditionExpression()
                {
                    ExpName = "Id",
                    ExpValue = model.Id,
                    ExpLogical = eLogicalOperator.And,
                    ExpOperater = eConditionOperator.NotEqual
                });
                var hasData = DataOperate.QueryListSimple<Epm_ConstituteCompany>(context, qc).Data;
                if (hasData != null && hasData.Count > 0)
                {
                    var has = hasData.Where(p => p.ConstituteKey == model.ConstituteKey).ToList();
                    if (has != null && has.Count > 0)
                    {
                        throw new Exception(model.ConstituteName + "已存在服务商设置");
                    }
                }

                var rows = DataOperateBusiness<Epm_ConstituteCompany>.Get().Update(model);

                #region 添加总批复及构成关联服务商
                List<Epm_ConstituteCompanyDetails> cwmpList = new List<Epm_ConstituteCompanyDetails>();
                if (view.ConstituteCompanyDetails != null)
                {
                    //先删除原始数据
                    var cWorkMainPoints = DataOperateBusiness<Epm_ConstituteCompanyDetails>.Get().GetList(t => t.ConstituteCompanyId == model.Id).ToList();

                    #region  删除总批复表和服务商关联表数据
                    foreach (var item in cWorkMainPoints)
                    {
                        item.OperateUserId = CurrentUserID.ToLongReq();
                        item.OperateUserName = CurrentUserName;
                        item.OperateTime = DateTime.Now;
                        item.DeleteTime = DateTime.Now;
                    }
                    rows = DataOperateBusiness<Epm_ConstituteCompanyDetails>.Get().DeleteRange(cWorkMainPoints);
                    #endregion

                    for (int i = 0; i < view.ConstituteCompanyDetails.Count; i++)
                    {
                        Epm_ConstituteCompanyDetails cwmp = new Epm_ConstituteCompanyDetails();
                        cwmp.CreateUserId = CurrentUserID.ToLongReq();
                        cwmp.CreateUserName = CurrentUserName;
                        cwmp.CreateTime = DateTime.Now;
                        cwmp = SetCurrentUser(cwmp);
                        cwmp = SetCreateUser(cwmp);
                        cwmp.ConstituteCompanyId = model.Id;
                        cwmp.CompanyId = view.ConstituteCompanyDetails[i].CompanyId;
                        cwmp.CompanyName = view.ConstituteCompanyDetails[i].CompanyName;
                        cwmp.ConstituteKey = view.ConstituteCompanyDetails[i].ConstituteKey;
                        cwmp.ConstituteName = view.ConstituteCompanyDetails[i].ConstituteName;
                        cwmpList.Add(cwmp);
                    }

                    //批量添加总批复及构成关联服务商
                    DataOperateBusiness<Epm_ConstituteCompanyDetails>.Get().AddRange(cwmpList);
                }
                #endregion

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.ReplyConstitute.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateConstituteCompany");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteConstituteCompanyByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_ConstituteCompany>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_ConstituteCompany>.Get().DeleteRange(models);

                //先删除原始数据
                var cWorkMainPoints = DataOperateBusiness<Epm_ConstituteCompanyDetails>.Get().GetList(t => ids.Contains(t.ConstituteCompanyId.Value)).ToList();

                #region  删除总批复表和服务商关联表数据
                foreach (var item in cWorkMainPoints)
                {
                    item.OperateUserId = CurrentUserID.ToLongReq();
                    item.OperateUserName = CurrentUserName;
                    item.OperateTime = DateTime.Now;
                    item.DeleteTime = DateTime.Now;
                }
                rows = DataOperateBusiness<Epm_ConstituteCompanyDetails>.Get().DeleteRange(cWorkMainPoints);
                #endregion

                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.ReplyConstitute.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteConstituteCompanyByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_ConstituteCompany>> GetConstituteCompanyList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_ConstituteCompany>> result = new Result<List<Epm_ConstituteCompany>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_ConstituteCompany>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetConstituteCompanyList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<ConstituteCompanyView> GetConstituteCompanyModel(long id)
        {
            Result<ConstituteCompanyView> result = new Result<ConstituteCompanyView>();
            ConstituteCompanyView list = new ConstituteCompanyView();
            try
            {
                list.ConstituteCompany = DataOperateBusiness<Epm_ConstituteCompany>.Get().GetModel(id);
                list.ConstituteCompanyDetails = DataOperateBusiness<Epm_ConstituteCompanyDetails>.Get().GetList(t => t.ConstituteCompanyId == id).ToList();
                result.Data = list;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetConstituteCompanyModel");
            }
            return result;
        }

        /// <summary>
        /// 获取服务商（根据总批复构成获取关联的服务商）
        /// </summary>
        /// <param name="qc"></param>
        /// <returns></returns>
        public Result<List<Epm_ConstituteCompanyDetails>> GetConstituteCompanyDetailsList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_ConstituteCompanyDetails>> result = new Result<List<Epm_ConstituteCompanyDetails>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_ConstituteCompanyDetails>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetConstituteCompanyDetailsList");
            }
            return result;
        }
        #endregion

        public Result<int> AddSendDate(Bp_SendDate model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Bp_SendDate>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddSendDate");
            }
            return result;
        }


        #region 考勤设置

        ///<summary>
        ///添加:项目考勤设置表
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddProjectAttendance(ProjectAttendanceView model)
        {
            Result<int> result = new Result<int>();
            try
            {
                List<Epm_ProjectAttendance> list = new List<Epm_ProjectAttendance>();
                if (model != null)
                {
                    //先删除
                    var delList = DataOperateBusiness<Epm_ProjectAttendance>.Get().GetList(t => t.ProjectId == 0);
                    if (delList.Any())
                    {
                        DataOperateBusiness<Epm_ProjectAttendance>.Get().DeleteRange(delList);
                    }
                    var dicList = DataOperateBasic<Base_TypeDictionary>.Get().GetList(t => t.Type == "PostType" && t.CreateUserName == "admin").ToList();
                    var attendanceList = model.Attendance.Split('|').ToArray();
                    var timeList = model.AttendanceTime.Split('|').ToArray();
                    foreach (var item in attendanceList)
                    {
                        foreach (var temp in timeList)
                        {
                            Epm_ProjectAttendance view = new Epm_ProjectAttendance();
                            view.AttendanceType = item;
                            string dicName = dicList.Where(t => t.No == item).FirstOrDefault().Name;
                            view.AttendanceName = dicName;
                            view.AttendanceTime = temp;
                            view.MarginError = model.MarginError;
                            view.StartTime = (Convert.ToDateTime(temp).AddMinutes(-model.MarginError.Value)).ToShortTimeString();
                            view.EndTime = (Convert.ToDateTime(temp).AddMinutes(model.MarginError.Value)).ToShortTimeString();
                            view.SetDate = DateTime.Now;
                            SetCreateUser(view);
                            list.Add(view);
                        }
                    }
                }
                var rows = DataOperateBusiness<Epm_ProjectAttendance>.Get().AddRange(list);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                //WriteLog(AdminModule.ProjectAttendance.GetText(), SystemRight.Add.GetText(), "新增项目考勤设置表: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddProjectAttendance");
            }
            return result;
        }
        ///<summary>
        ///修改:项目考勤设置表
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateProjectAttendance(Epm_ProjectAttendance model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_ProjectAttendance>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.ProjectAttendance.GetText(), SystemRight.Modify.GetText(), "修改项目考勤设置表: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateProjectAttendance");
            }
            return result;
        }
        ///<summary>
        ///删除:项目考勤设置表
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteProjectAttendanceByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_ProjectAttendance>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_ProjectAttendance>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.ProjectAttendance.GetText(), SystemRight.Delete.GetText(), "批量删除项目考勤设置表: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteProjectAttendanceByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:项目考勤设置表
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_ProjectAttendance>> GetProjectAttendanceList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_ProjectAttendance>> result = new Result<List<Epm_ProjectAttendance>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_ProjectAttendance>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectAttendanceList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:项目考勤设置表
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_ProjectAttendance> GetProjectAttendanceModel(long id)
        {
            Result<Epm_ProjectAttendance> result = new Result<Epm_ProjectAttendance>();
            try
            {
                var model = DataOperateBusiness<Epm_ProjectAttendance>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetProjectAttendanceModel");
            }
            return result;
        }

        /// <summary>
        /// 获取考勤设置信息
        /// </summary>
        /// <returns></returns>
        public Result<AttendanceView> GetAttendanceModel()
        {
            Result<AttendanceView> result = new Result<AttendanceView>();
            try
            {
                AttendanceView model = new AttendanceView();
                var list = DataOperateBusiness<Epm_ProjectAttendance>.Get().GetList(t => t.ProjectId == 0).ToList();

                if (list.Any())
                {
                    model.AttendanceList = list.Select(t => t.AttendanceType).Distinct().ToList();
                    model.MarginError = list.FirstOrDefault().MarginError;
                    var time = list.Select(t => t.AttendanceTime).ToList().Distinct();
                    model.AttendanceTimeList = new List<string>();
                    foreach (var item in time)
                    {
                        model.AttendanceTimeList.Add(item);
                    }
                    model.Num = time.Count();
                }
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetAttendanceModel");
            }
            return result;
        }

        #endregion
    }
}
