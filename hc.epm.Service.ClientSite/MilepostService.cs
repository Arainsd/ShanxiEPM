using hc.epm.Common;
using hc.epm.DataModel.Business;
using hc.epm.Service.Base;
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
    public partial class ClientSiteService : BaseService, IClientSiteService
    {
        ///<summary>
        ///添加:
        ///</summary>
        /// <param name="model">要添加的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> AddMilepost(Epm_Milepost model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_Milepost>.Get().Add(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.MilepostConfig.GetText(), SystemRight.Add.GetText(), "新增: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "AddMilepost");
            }
            return result;
        }
        ///<summary>
        ///修改:
        ///</summary>
        /// <param name="model">要修改的model</param>
        /// <returns>受影响的行数</returns>
        public Result<int> UpdateMilepost(Epm_Milepost model)
        {
            Result<int> result = new Result<int>();
            try
            {
                var rows = DataOperateBusiness<Epm_Milepost>.Get().Update(model);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.MilepostConfig.GetText(), SystemRight.Modify.GetText(), "修改: " + model.Id);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "UpdateMilepost");
            }
            return result;
        }
        ///<summary>
        ///删除:
        ///</summary>
        /// <param name="ids">要删除的Id集合</param>
        /// <returns>受影响的行数</returns>
        public Result<int> DeleteMilepostByIds(List<long> ids)
        {
            Result<int> result = new Result<int>();
            try
            {
                var models = DataOperateBusiness<Epm_Milepost>.Get().GetList(i => ids.Contains(i.Id)).ToList();
                var rows = DataOperateBusiness<Epm_Milepost>.Get().DeleteRange(models);
                result.Data = rows;
                result.Flag = EResultFlag.Success;
                WriteLog(AdminModule.MilepostConfig.GetText(), SystemRight.Delete.GetText(), "批量删除: " + rows);
            }
            catch (Exception ex)
            {
                result.Data = -1;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "DeleteMilepostByIds");
            }
            return result;
        }
        ///<summary>
        ///获取列表:
        ///</summary>
        /// <param name="qc">查询条件</param>
        /// <returns>符合条件的数据集合</returns>
        public Result<List<Epm_Milepost>> GetMilepostList(QueryCondition qc)
        {
            qc = AddDefault(qc);
            Result<List<Epm_Milepost>> result = new Result<List<Epm_Milepost>>();
            try
            {
                result = hc.Plat.Common.Service.DataOperate.QueryListSimple<Epm_Milepost>(context, qc);
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMilepostList");
            }
            return result;
        }
        ///<summary>
        ///获取详情:
        ///</summary>
        /// <param name="id">数据Id</param>
        /// <returns>数据详情model</returns>
        public Result<Epm_Milepost> GetMilepostModel(long id)
        {
            Result<Epm_Milepost> result = new Result<Epm_Milepost>();
            try
            {
                var model = DataOperateBusiness<Epm_Milepost>.Get().GetModel(id);
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMilepostModel");
            }
            return result;
        }

        /// <summary>
        /// 获取里程碑树形列表
        /// </summary>
        /// <returns></returns>
        public Result<List<MilepostView>> GetMilepostViewList()
        {
            List<MilepostView> list = new List<MilepostView>();
            Result<List<MilepostView>> result = new Result<List<MilepostView>>();
            try
            {
                var temp = DataOperateBusiness<Epm_Milepost>.Get().GetList(t => t.ParentId == 0).OrderBy(t => t.Sort).ToList();
                if (temp.Count > 0)
                {
                    foreach (var item in temp)
                    {
                        MilepostView rootTree = new MilepostView();
                        rootTree = GetMilepostTree(item.Id);
                        list.Add(rootTree);
                    }

                }
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
        public MilepostView GetMilepostTree(long pId)
        {
            List<Epm_Milepost> result = new List<Epm_Milepost>();

            result = DataOperateBusiness<Epm_Milepost>.Get().GetList().OrderBy(t => t.Sort).ToList();
            MilepostView rootTree = new MilepostView();

            //var first = result.FirstOrDefault(i => i.Id == pId);

            //rootTree.checkboxValue = first.Id.ToString();
            //rootTree.Code = first.Code;
            //rootTree.Name = first.Name;
            //rootTree.Sort = first.Sort;
            //rootTree.State = first.State;
            //rootTree.parentId = first.ParentId.ToString();

            //var tree = createTree(first.Id, result);

            //rootTree.children = tree;
            return rootTree; ;
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
            //        node.parentId = item.ParentId.ToString();

            //        var iteratorList = createTree(item.Id, allList);
            //        node.children = iteratorList;
            //        list.Add(node);
            //    }
            //}
            return list;
        }

        /// <summary>
        /// 根绝项目资料ID获取里程碑信息
        /// </summary>
        /// <param name="dataConfigId"></param>
        /// <returns></returns>
        public Result<Epm_MilepostData> GetMDataByDataId(long dataConfigId)
        {
            Result<Epm_MilepostData> result = new Result<Epm_MilepostData>();
            try
            {
                var model = DataOperateBusiness<Epm_MilepostData>.Get().GetList(t => t.DataConfigId == dataConfigId).OrderByDescending(t => t.OperateTime).FirstOrDefault();
                result.Data = model;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(ex, "GetMDataByDataId");
            }
            return result;
        }

    }
}
