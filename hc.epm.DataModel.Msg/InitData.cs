using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.DataModel.Msg
{
    public class InitData : DropCreateDatabaseIfModelChanges<MsgDataContext>
    {
        protected override void Seed(MsgDataContext context)
        {
            base.Seed(context);
        }

        public override void InitializeDatabase(MsgDataContext context)
        {
            base.InitializeDatabase(context);
        }
    }
}
