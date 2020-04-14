using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
   public class UserInfoView
    {
        public EPM_AIUserFace AIUserFace { get; set; }

        /// <summary>
        /// 明细数据
        /// </summary>
        public Base_User User { get; set; }
    }
}
