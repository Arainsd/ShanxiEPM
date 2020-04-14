using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Snowflake.Net;

namespace hc.epm.Common
{
    public class SnowflakeHelper
    {
        private static readonly IdWorker Worker = new IdWorker(1, 1);

        public static long GetID
        {
            get
            {
                return Worker.NextId();
            }
        }
    }
}
