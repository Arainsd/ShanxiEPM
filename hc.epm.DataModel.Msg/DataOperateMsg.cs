using hc.epm.DataModel.BaseCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.DataModel.Msg
{
    public class DataOperateMsg<T> : DataOperate<T> where T : BaseModel
    {
        public DataOperateMsg()
        {
            Context = GetMsgDataContext();
        }
       
        /// <summary>
        /// 创建EF上下文对象,已存在就直接取,不存在就创建,保证线程内是唯一。
        /// </summary>
        private static DbContext GetMsgDataContext()
        {
            DbContext dbContext = CallContext.GetData("MsgDataContext") as DbContext;
            if (dbContext == null)
            {
                dbContext = new MsgDataContext();
                CallContext.SetData("MsgDataContext", dbContext);
            }
            return dbContext;
        }

        public static DataOperate<T> Get()
        {
            return new DataOperate<T>(GetMsgDataContext());
        }
    }
}
