using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using hc.epm.DataModel.Business;
using hc.epm.DataModel.Basic;

namespace hc.epm.ViewModel
{
    public class SpecialAcceptanceView: Epm_SpecialAcceptance
    {
        public SpecialAcceptanceView()
        {
            AcceptanceDetailList = new List<Epm_SpecialAcceptanceDetails>();
            AttachList = new List<Base_Files>();
        }

        /// <summary>
        /// 验收项目明细列表
        /// </summary>
        public List<Epm_SpecialAcceptanceDetails> AcceptanceDetailList { get; set; }

        /// <summary>
        /// 附件列表
        /// </summary>
        public List<Base_Files> AttachList { get; set; }

        /// <summary>
        /// 审核备注
        /// </summary>
        public string AuditRemark { get; set; }
    }

    /// <summary>
    /// 竣工验收验收项模型
    /// </summary>
    public class CompletionAcceptanceItemView
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public bool Result { get; set; }
    }
}
