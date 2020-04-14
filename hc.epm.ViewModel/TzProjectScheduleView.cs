using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class TzProjectScheduleView
    {
        /// <summary>
        /// 加油站项目提出
        /// </summary>
        public bool TzProjectProposal { get; set; }

        /// <summary>
        /// 项目性质
        /// </summary>
        public string Nature { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }


        //现场工程方面调研
        public bool TzResearchOfEngineering { get; set; }

        /// <summary>
        /// 初次谈判
        /// </summary>
        public bool TzFirstNegotiation { get; set; }

        /// <summary>
        /// 土地协议出让谈判信息
        /// </summary>
        public bool TzLandNegotiation { get; set; }

        /// <summary>
        /// 评审材料上报
        /// </summary>
        public bool TzFormTalkFile { get; set; }

        /// <summary>
        /// 项目评审
        /// </summary>
        public bool TzProjectReveiews { get; set; }

        /// <summary>
        /// 项目批复请示
        /// </summary>
        public bool TzProjectApprovalInfo { get; set; }

        /// <summary>
        /// 会议决策
        /// </summary>
        public bool MeetingFileReport { get; set; }

        /// <summary>
        /// 设计方案
        /// </summary>
        public bool TzDesignScheme { get; set; }

        /// <summary>
        /// 图纸会审
        /// </summary>
        public bool TzConDrawing { get; set; }

        /// <summary>
        /// 招标管理
        /// </summary>
        public bool TzBidResult { get; set; }
        
        /// <summary>
        /// 物资申请
        /// </summary>
        public bool TzSupplyMaterialApply { get; set; }

        /// <summary>
        /// 开工申请
        /// </summary>
        public bool TzProjectStartApply { get; set; }

        /// <summary>
        /// 施工管理
        /// </summary>
        public bool TzConstruceManage { get; set; }

        /// <summary>
        /// 竣工管理
        /// </summary>
        public bool TzCompletedManage { get; set; }

        /// <summary>
        /// 试运行
        /// </summary>
        public bool TzProjectPolit { get; set; }
    }
}
