using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.DataModel.BaseCore
{
    public class ContextSet
    {
        private static DbContext context;

        protected static DbContext Context
        {
            get
            {
                if (context == null)
                {
                    GetDbContext();
                }
                return context;
            }
            set
            {
                context = value;
            }
        }

        public static void GetDbContext(int contextType = 1)
        {
            context = null;
        }
        ///// <summary>
        ///// 创建EF上下文对象,已存在就直接取,不存在就创建,保证线程内是唯一。
        ///// </summary>
        //private static DbContext GetBasicDataContext()
        //{
        //    DbContext dbContext = CallContext.GetData("BasicDataContext") as DbContext;
        //    //if (dbContext == null)
        //    //{
        //    //    dbContext = new BasicDataContext();
        //    //    CallContext.SetData("BasicDataContext", dbContext);
        //    //}
        //    return dbContext;
        //}
        //private static DbContext GetCoreDataContext()
        //{
        //    DbContext dbContext = CallContext.GetData("CoreDataContext") as DbContext;
        //    if (dbContext == null)
        //    {
        //        dbContext = new CoreDataContext();
        //        CallContext.SetData("CoreDataContext", dbContext);
        //    }
        //    return dbContext;
        //}
        //private static DbContext GetMsgDataContext()
        //{
        //    DbContext dbContext = CallContext.GetData("MsgDataContext") as DbContext;
        //    if (dbContext == null)
        //    {
        //        dbContext = new MsgDataContext();
        //        CallContext.SetData("MsgDataContext", dbContext);
        //    }
        //    return dbContext;
        //}

    }
}
