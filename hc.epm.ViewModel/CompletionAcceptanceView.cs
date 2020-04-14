using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;

namespace hc.epm.ViewModel
{
    public class CompletionAcceptanceView
    {
        public CompletionAcceptanceView()
        {
            CompletionRectifyCompanys = new List<Epm_CompletionRectifyCompany>();
            BaseFiles = new List<Base_Files>();
        }

        /// <summary>
        /// 整改
        /// </summary>
        public Epm_CompletionAcceptance CompletionAcceptance { get; set; }

        /// <summary>
        /// 整改单位
        /// </summary>
        public List<Epm_CompletionRectifyCompany> CompletionRectifyCompanys { get; set; }

        /// <summary>
        /// 相关附件
        /// </summary>
        public List<Base_Files> BaseFiles { get; set; }
        
    }
}
