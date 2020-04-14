using AutoMapper;
using hc.epm.DataModel.BaseCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.DataModel.Basic
{
    public class DataOperateBasic<T> : DataOperate<T> where T : BaseModel
    {
        public DataOperateBasic()
        {
            Context = GetBasicDataContext();
        }

        /// <summary>
        /// 创建EF上下文对象,已存在就直接取,不存在就创建,保证线程内是唯一。
        /// </summary>
        private static DbContext GetBasicDataContext()
        {
            //DbContext dbContext = CallContext.GetData("BasicDataContext") as DbContext;
            //if (dbContext == null || dbContext.Database.Connection.State == ConnectionState.Closed)
            //{
            //    InitMap();
            //    dbContext = new BasicDataContext();
            //    CallContext.SetData("BasicDataContext", dbContext);
            //}
            return new BasicDataContext();
        }

        public static DataOperate<T> Get(DbContext dbContext = null)
        {
            //TODO:考虑new对象消耗资源问题
            if (dbContext == null)
            {
                dbContext = new BasicDataContext();
            }
            return new DataOperate<T>(dbContext);
        }

        public static void InitMap()
        {
            Mapper.Initialize(cfg =>
            {
                AutoMapper.Mappers.MapperRegistry.Mappers.Add(new AutoMapper.Data.DataReaderMapper { YieldReturnEnabled = true });
                cfg.CreateMap<System.Data.IDataReader, Base_Company>();
            });
        }
    }
}
