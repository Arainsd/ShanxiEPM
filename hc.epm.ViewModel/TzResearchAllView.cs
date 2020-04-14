using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class TzResearchAllView
    {
        public TzResearchAllView()
        {
            //TzProjectProposal = new Epm_TzProjectProposal();
            //TzResearchOfEngineering = new Epm_TzResearchOfEngineering();
            //TzResearchOfInformation = new Epm_TzResearchOfInformation();
            //TzResearchOfInvestment = new Epm_TzResearchOfInvestment();
            //TzResearchOfLaw = new Epm_TzResearchOfLaw();
            //TzResearchOfManagement = new Epm_TzResearchOfManagement();
            //TzResearchOfSecurity = new Epm_TzResearchOfSecurity();
        }

        /// <summary>
        /// 加油站项目信息表
        /// </summary>
        public Epm_TzProjectProposal TzProjectProposal { get; set; }

        /// <summary>
        /// 现场工程方面调研
        /// </summary>
        public Epm_TzResearchOfEngineering TzResearchOfEngineering { get; set; }

        /// <summary>
        /// 信息方面调研
        /// </summary>
        public Epm_TzResearchOfInformation TzResearchOfInformation { get; set; }

        /// <summary>
        /// 现场投资调研
        /// </summary>
        public Epm_TzResearchOfInvestment TzResearchOfInvestment { get; set; }

        /// <summary>
        /// 现场法律调研
        /// </summary>
        public Epm_TzResearchOfLaw TzResearchOfLaw { get; set; }

        /// <summary>
        /// 经营方面调研
        /// </summary>
        public Epm_TzResearchOfManagement TzResearchOfManagement { get; set; }

        /// <summary>
        /// 安全方面调研
        /// </summary>
        public Epm_TzResearchOfSecurity TzResearchOfSecurity { get; set; }
    }
}
