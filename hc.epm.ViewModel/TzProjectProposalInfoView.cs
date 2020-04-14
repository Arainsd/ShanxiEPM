/************************************************************************************
 * Copyright (c) 2019  All Rights Reserved.
 * CLR版本： 4.0.30319.42000
 * 公司名称：陕西华春网络科技股份有限公司
 * 命名空间：hc.epm.ViewModel
 * 文件名：  TzGasStationProjectView
 * 版本号：  V1.0.0.0
 * 创建人：  wmg	
 * 电子邮箱：wmgwugang@huachun.com
 * 创建时间：2019/8/24 11:02:45
 * 描述：
 * 
 * 
 * 
 ************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hc.epm.DataModel.Business;

namespace hc.epm.ViewModel
{
    /// <summary>
    /// 加油站项目前期提出详情视图
    /// </summary>
    public class TzProjectProposalInfoView
    {
        /// <summary>
        /// 加油站项目提出
        /// </summary>
        public Epm_TzProjectProposal TzProjectProposal { get; set; }

        #region 现场考察
        public TzResearchAllView TzResearchAllView { get; set; }
        #endregion

        /// <summary>
        /// 初次谈判
        /// </summary>
        public Epm_TzFirstNegotiation TzFirstNegotiation { get; set; }

        /// <summary>
        /// 土地协议出让谈判信息
        /// </summary>
        public Epm_TzLandNegotiation TzLandNegotiation { get; set; }

        /// <summary>
        /// 评审材料上报
        /// </summary>
        public Epm_TzFormTalkFile TzFormTalkFile { get; set; }

        /// <summary>
        /// 项目评审
        /// </summary>
        public Epm_TzProjectReveiews TzProjectReveiews { get; set; }

        /// <summary>
        /// 项目批复请示
        /// </summary>
        public Epm_TzProjectApprovalInfo TzProjectApprovalInfo { get; set; }

        /// <summary>
        /// 会议决策
        /// </summary>
        public Epm_MeetingFileReport MeetingFileReport { get; set; }

        /// <summary>
        /// 是否有关闭项目按钮（true：有按钮，false：没有按钮）
        /// </summary>
        public bool IsColseed { get; set; }
    }
}
