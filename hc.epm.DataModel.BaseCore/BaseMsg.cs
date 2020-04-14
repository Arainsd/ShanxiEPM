using hc.epm.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.DataModel.BaseCore
{
    public class BaseMsg : BaseModel
    {
        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime RecordTime { get; set; } = DateTime.Now;
    }
}
