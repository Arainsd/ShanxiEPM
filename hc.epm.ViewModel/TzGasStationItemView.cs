using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class TzProjectProposalView
    {
        public TzProjectProposalView()
        {
            TzProjectProposal = new Epm_TzProjectProposal();
            TzSiteSurvey = new Epm_TzSiteSurvey();
            TzInitialTalk = new Epm_TzInitialTalk();
            TzFormTalkFile = new Epm_TzFormTalkFile();
            TzTalkRecord = new Epm_TzTalkRecord();
            TzLandTalk = new Epm_TzLandTalk();
            TzProjectApproval = new Epm_TzProjectApproval();
            TzSecondTakl = new Epm_TzSecondTakl();
            TzSecondTalkAudit = new Epm_TzSecondTalkAudit();
        }

        /// <summary>
        /// 加油站项目信息表
        /// </summary>
        public Epm_TzProjectProposal TzProjectProposal { get; set; }

        /// <summary>
        /// 现场调研表
        /// </summary>
        public Epm_TzSiteSurvey TzSiteSurvey { get; set; }

        /// <summary>
        /// 初次谈判表
        /// </summary>
        public Epm_TzInitialTalk TzInitialTalk { get; set; }

        /// <summary>
        /// 土地谈判协议
        /// </summary>
        public Epm_TzLandTalk TzLandTalk { get; set; }

        /// <summary>
        /// 组织评审材料
        /// </summary>
        public Epm_TzFormTalkFile TzFormTalkFile { get; set; }

        /// <summary>
        /// 评审会记录
        /// </summary>
        public Epm_TzTalkRecord TzTalkRecord { get; set; }

        /// <summary>
        /// 项目批复请示
        /// </summary>
        public Epm_TzProjectApproval TzProjectApproval { get; set; }

        /// <summary>
        /// N次谈判
        /// </summary>
        public Epm_TzSecondTakl TzSecondTakl { get; set; }

        /// <summary>
        /// N次谈判审核
        /// </summary>
        public Epm_TzSecondTalkAudit TzSecondTalkAudit { get; set; }
    }
}
