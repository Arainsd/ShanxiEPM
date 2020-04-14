/*
   2015.12.15 优化取关键字的处理；



*/
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Linq;
using hc.Plat.Common.Global;

namespace hc.Plat.Common.Service
{
    /// <summary>
    /// 数据操作类；
    /// 需要开事物的，请在最外层开具事物：new TransactionScope()；  scope.Complete()；
    /// </summary>
    public class DataOperate
    {
        public DataOperate()
        {
        }

        /// <summary>
        /// 获取TEntity的关键字
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="CurrContext"></param>
        /// <returns></returns>
        public static string[] FindKeyFieldList<TEntity>(DbContext CurrContext)
        {
            ObjectContext objectContext = ((IObjectContextAdapter)CurrContext).ObjectContext;
            var entities = objectContext.MetadataWorkspace.GetItems<System.Data.Entity.Core.Metadata.Edm.EntityType>(System.Data.Entity.Core.Metadata.Edm.DataSpace.CSpace).FirstOrDefault(it => it.Name == typeof(TEntity).Name);
            return entities.KeyMembers.Select(o => o.Name).ToArray();
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="CurrContext">数据连接</param>
        /// <param name="objEntity">需要增加的对象</param>
        /// <returns></returns>
        public static object InsertObject(DbContext CurrContext, object objEntity)
        {
            Type t = objEntity.GetType();
            int effect = -1;
            DbSet set = CurrContext.Set(t);
            set.Add(objEntity);
            effect = CurrContext.SaveChanges();
            return objEntity;
        }

        /// <summary>
        /// 批量新增
        /// </summary>
        /// <param name="CurrContext"></param>
        /// <param name="objEntityList"></param>
        /// <returns></returns>
        public static int InsertObjectList(DbContext CurrContext, List<object> objEntityList)
        {
            int effect = 0;

            var et = objEntityList.GetEnumerator();
            if (et.MoveNext())
            {
                Type t = et.Current.GetType();
                DbSet set = CurrContext.Set(t);
                foreach (var o in objEntityList)
                {
                    set.Add(o);
                }
                effect = CurrContext.SaveChanges();
            }
            return effect;
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="CurrContext"></param>
        /// <param name="objEntity"></param>
        /// <returns></returns>
        public static object ModifyObject(DbContext CurrContext, object objEntity)
        {
            int effect = -1;
            DbEntityEntry entry = CurrContext.Entry(objEntity);
            entry.State = EntityState.Modified;
            effect = CurrContext.SaveChanges();
            return objEntity;
        }

        /// <summary>
        /// 批量修改
        /// </summary>
        /// <param name="CurrContext"></param>
        /// <param name="objEntityList"></param>
        /// <returns></returns>
        public static int ModifyObjectList(DbContext CurrContext, List<object> objEntityList)
        {
            int effect = 0;
            var et = objEntityList.GetEnumerator();
            if (et.MoveNext())
            {
                Type t = et.Current.GetType();
                foreach (var o in objEntityList)
                {
                    DbEntityEntry entry = CurrContext.Entry(o);
                    entry.State = EntityState.Modified;
                }
                effect = CurrContext.SaveChanges();
            }
            return effect;
        }

        /// <summary>
        /// 删除:删除只需要主键 
        /// </summary>
        /// <param name="CurrContext"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object DeleteObject(DbContext CurrContext, object obj)
        {
            int effect = -1;
            DbEntityEntry entry = CurrContext.Entry(obj);
            entry.State = EntityState.Deleted;
            effect = CurrContext.SaveChanges();
            return obj;
        }

        /// <summary>
        /// 批量删除::删除只需要主键，这里删除主键为5的行
        /// </summary>
        /// <param name="CurrContext"></param>
        /// <param name="objEntityList"></param>
        /// <returns></returns>
        public static int DeleteObjectList(DbContext CurrContext, List<object> objEntityList)
        {
            int effect = 0;
            var et = objEntityList.GetEnumerator();
            if (et.MoveNext())
            {
                Type t = et.Current.GetType();
                foreach (var o in objEntityList)
                {
                    DbEntityEntry entry = CurrContext.Entry(o); 
                    entry.State = EntityState.Deleted;
                }
                effect = CurrContext.SaveChanges();
            }
            return effect;
        }

        private static string GetOrderESQL(string TableName, QueryCondition condition)
        {
            string ESQL = " ";

           

            if (condition.SortList.Count != 0)
            {
                ESQL = " ORDER BY ";
                bool isfirst = true;
                foreach (SortExpression se in condition.SortList)
                {
                    string fn = se.SortName; 
                    if (fn.IndexOf(TableName + ".") == -1)
                    {
                        fn = TableName + "." + fn;
                    }
                    if (se.SortType == eSortType.Desc)
                    {
                        if (isfirst)
                        {
                            ESQL = ESQL + " " + fn + " DESC ";
                        }
                        else
                        {
                            ESQL = ESQL + " , " + fn + " DESC ";
                        }
                    }
                    else
                    {
                        if (isfirst)
                        {
                            ESQL = ESQL + " " + fn + " ASC ";
                        }
                        else
                        {
                            ESQL = ESQL + " , " + fn + " ASC ";
                        }
                    }
                    isfirst = false;
                }
            }
            return ESQL;
        }

        private static string GetWhereESQL(string TableName, QueryCondition condition)
        {
            string ESQL = "";


            foreach(ConditionExpression ce in condition.ConditionList)
            {
                ESQL = ESQL + "  " + ce.ToESQLString(TableName);
            } 

            if (ESQL != "")
            {
                if (ESQL.ToUpper().Trim().StartsWith("AND"))
                {
                    ESQL = ESQL.Trim().Remove(0, 3);
                }
                if (ESQL.ToUpper().Trim().StartsWith("OR"))
                {
                    ESQL = ESQL.Trim().Remove(0, 2);
                }
                return " WHERE " + ESQL;
            }
            else
            {
                return " ";
            }
        }

        public static Result<List<TEntity>> QueryListSimple<TEntity>(DbContext CurrContext, QueryCondition condition)
        {
            Result<List<TEntity>> result = new Result<List<TEntity>>();
            try
            {
                if (condition.PageInfo != null && condition.PageInfo.OrderAndSortList != "" && condition.SortList.Count == 0)
                {
                    condition.CheckOrderAndSortList();
                }

                ObjectContext objectContext = ((IObjectContextAdapter)CurrContext).ObjectContext;
                string ContextTypeName = CurrContext.GetType().Name; 
                string TableName = typeof(TEntity).Name; 

                bool isAllowPage = false;
                if (condition != null && condition.PageInfo != null && condition.PageInfo.isAllowPage)
                {
                    isAllowPage = true;

                    //如果没有SORT条件；则需要随意增加一个默认按照关键字的条件；

                    if (condition.SortList.Count==0)
                    {

                        string[] KeyFieldList = FindKeyFieldList<TEntity>(CurrContext);

                        if (KeyFieldList.Length != 0)
                        {
                            string _keyName = KeyFieldList[0];
                            condition.SortList.Add(new SortExpression(TableName + "."+ _keyName, eSortType.Desc));
                        } 
                    }
                }



                /*构造ESQL*/
                string ESQL = "SELECT value " + TableName + " FROM " + ContextTypeName + "." + TableName;

                //获取条件；
                ESQL = ESQL + GetWhereESQL(TableName, condition);

                //增加ORDER BY；
                ESQL = ESQL + GetOrderESQL(TableName, condition);

                IQueryable<TEntity> query = objectContext.CreateQuery<TEntity>(ESQL);
                ObjectQuery<TEntity> objectquery = objectContext.CreateQuery<TEntity>(ESQL);
                              

                objectquery.MergeOption = MergeOption.NoTracking;

                if (isAllowPage)
                {
                    query = objectquery.Skip<TEntity>((condition.PageInfo.CurrentPageIndex - 1) *
                            condition.PageInfo.PageRowCount).Take<TEntity>(condition.PageInfo.PageRowCount);
                    //当前页的数据；
                    result.Data = query.ToList<TEntity>();
                    //总行数；此语句已经转化为COUNT来处理；
                    result.AllRowsCount = objectquery.Count();
                }
                else
                {
                    query = objectquery;
                    //当前页的数据；
                    result.Data = query.ToList<TEntity>();
                    //总行数；
                    result.AllRowsCount = result.Data.Count;
                }
                result.Flag = EResultFlag.Success;
            }
            catch(Exception ex)
            {
                result.Exception = new ExceptionEx(ex, "QueryByCondition");
                result.AllRowsCount = -1;
                result.Data = null;
                result.Flag = EResultFlag.Failure;
            }
            return result;
        }
    }
}
