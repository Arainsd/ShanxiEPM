using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.DataModel.BaseCore
{
    public class DataOperate<T> : ContextSet where T : BaseModel
    {
        public DataOperate()
        {

        }
        public DataOperate(DbContext context)
        {
            Context = context;
        }
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Add(T model)
        {
            var newEntry = Context.Set<T>().Add(model);
            int result = Context.SaveChanges();
            return result;
        }
        /// <summary>
        /// 批量添加数据
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        public int AddRange(IEnumerable<T> models)
        {
            Context.Set<T>().AddRange(models);
            int result = Context.SaveChanges();
            return result;
        }
        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Delete(T model)
        {
            model.IsDelete = true;
            return Update(model);
        }
        /// <summary>
        /// 根据id删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(long id)
        {
            var model = Context.Set<T>().Single(item => item.Id == id);
            model.IsDelete = true;
            return Update(model);
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="models"></param>
        /// <returns></returns>
        public int DeleteRange<T>(IEnumerable<T> models) where T : BaseModel
        {
            List<T> updateList = new List<T>();
            foreach (var item in models)
            {
                item.IsDelete = true;
                updateList.Add(item);
            }
            return UpdateRange(updateList);
        }
        /// <summary>
        /// 根据条件删除数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int Delete(Expression<Func<T, bool>> predicate)
        {
            var objects = GetList(predicate);
            foreach (var obj in objects)
            {
                Context.Set<T>().Remove(obj);
            }
            return Context.SaveChanges();
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int Update(T model)
        {
            var entry = Context.Entry(model);
            if (entry.State == EntityState.Detached)
            {
                HandleDetached(model);
            }
            Context.Set<T>().Attach(model);
            entry.State = EntityState.Modified;
            return Context.SaveChanges();
        }
        /// <summary>
        /// Attach的实体对象是通过new创建的，而不是通过Entity Framework从数据库中获取的，但实例的主键对应数据在数据库中存在，该实例而不存在于DBContext上下文中，尝试Attach会抛出异常
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool HandleDetached(T entity)
        {
            var objectContext = ((IObjectContextAdapter)Context).ObjectContext;
            var entitySet = objectContext.CreateObjectSet<T>();
            var entityKey = objectContext.CreateEntityKey(entitySet.EntitySet.Name, entity);
            object foundSet;
            bool exists = objectContext.TryGetObjectByKey(entityKey, out foundSet);
            if (exists)
            {
                objectContext.Detach(foundSet); //从上下文中移除
            }
            return exists;
        }

        /// <summary>
        /// 批量更新数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int UpdateRange<T>(IEnumerable<T> models) where T : BaseModel
        {
            try
            {
                foreach (var model in models)
                {
                    var entry = Context.Entry(model);
                    Context.Set<T>().Attach(model);
                    entry.State = EntityState.Modified;
                }
                return Context.SaveChanges();
            }
            catch (OptimisticConcurrencyException ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 根据指定条件获取一条数据
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public T Single(Expression<Func<T, bool>> expression)
        {
            return Context.Set<T>().Where(i => i.IsDelete == false).FirstOrDefault(expression);
        }
        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetModel(long id)
        {
            return Context.Set<T>().Where(i => i.IsDelete == false).SingleOrDefault(model => model.Id == id);
        }
        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetList(bool isDelete = false)
        {
            if (!isDelete)
            {
                return Context.Set<T>().Where(i => i.IsDelete == false);
            }
            return Context.Set<T>();

        }
        /// <summary>
        /// 根据条件取得数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<T> GetList(Expression<Func<T, bool>> predicate, bool isDelete = false)
        {
            if (!isDelete)
            {
                return Context.Set<T>().Where(i => i.IsDelete == false).Where<T>(predicate).OrderByDescending(i => i.Id);
            }
            return Context.Set<T>().Where<T>(predicate).OrderByDescending(i => i.Id);

        }
        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderDesc"></param>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IQueryable<T> GetListByPage(Expression<Func<T, bool>> predicate, Func<T, object> orderDesc, out int totalCount, int pageIndex = 0,
                                               int pageSize = 10)
        {
            var skipCount = pageIndex * pageSize;
            var resetSet = predicate != null
                                ? Context.Set<T>().Where(i => i.IsDelete == false).Where<T>(predicate).OrderByDescending(orderDesc).AsQueryable()
                                : Context.Set<T>().Where(i => i.IsDelete == false).OrderByDescending(orderDesc).AsQueryable();
            resetSet = skipCount == 0 ? resetSet.Take(pageSize) : resetSet.Skip(skipCount).Take(pageSize);
            totalCount = resetSet.Count();
            return resetSet;
        }
        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="orderAsc"></param>
        /// <param name="totalCount"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IQueryable<T> GetListByPageOrderAsc(Expression<Func<T, bool>> predicate, Func<T, object> orderAsc, out int totalCount, int pageIndex = 0,
                                               int pageSize = 10)
        {
            var skipCount = pageIndex * pageSize;
            var resetSet = predicate != null
                                ? Context.Set<T>().Where(i => i.IsDelete == false).Where<T>(predicate).OrderBy(orderAsc).AsQueryable()
                                : Context.Set<T>().Where(i => i.IsDelete == false).OrderBy(orderAsc).AsQueryable();
            resetSet = skipCount == 0 ? resetSet.Take(pageSize) : resetSet.Skip(skipCount).Take(pageSize);
            totalCount = resetSet.Count();
            return resetSet;
        }
        /// <summary>
        /// 是否存在数据
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Contains(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().Where(i => i.IsDelete == false).Count<T>(predicate) > 0;
        }
        /// <summary>
        /// 数量查询
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>().Where(i => i.IsDelete == false).Count<T>(predicate);
        }
        /// <summary>
        /// 统计类直接执行sql语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<T> Select<T>(string sql) where T : class
        {

            using (var connection = (System.Data.SqlClient.SqlConnection)Context.Database.Connection)
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandTimeout = 60;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                            return Mapper.Map<IDataReader, IEnumerable<T>>(reader).ToList();
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }

        }
        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SQLString)
        {
            using (var connection = (System.Data.SqlClient.SqlConnection)Context.Database.Connection)
            {
                try
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandTimeout = 60;
                    command.CommandType = System.Data.CommandType.Text;
                    command.CommandText = SQLString;
                    object obj = command.ExecuteScalar();
                    if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                    {
                        return null;
                    }
                    else
                    {
                        return obj;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public object GetModel(Func<object, object> p)
        {
            throw new NotImplementedException();
        }
    }
}

