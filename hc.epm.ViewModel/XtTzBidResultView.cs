using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class XtTzBidResultView
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 承办部门
        /// </summary>
        public string UndertakeDepartment { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string UndertakeContacts { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string UndertakeTel { get; set; }
        /// <summary>
        /// 批复文件或者纪要
        /// </summary>
        public string Minutes { get; set; }
        /// <summary>
        /// 招标方式
        /// </summary>
        public string BidName { get; set; }
        /// <summary>
        /// 资金预算及依据
        /// </summary>
        public string CapitalBudget { get; set; }
        /// <summary>
        /// 项目概述
        /// </summary>
        public string ProjectSummary { get; set; }
        /// <summary>
        /// 邀请谈判理由
        /// </summary>
        public string InvitationNegotiate { get; set; }
        /// <summary>
        /// 拟邀请潜在谈判人
        /// </summary>
        public string InvitationNegotiator { get; set; }
        /// <summary>
        /// 公示公司一
        /// </summary>
        public string BidderOne { get; set; }
        /// <summary>
        /// 公示价格一
        /// </summary>
        public string QuotationOne { get; set; }
        /// <summary>
        /// 公示备注一
        /// </summary>
        public string RemarkOne { get; set; }
        /// <summary>
        /// 公示公司二
        /// </summary>
        public string BidderTwo { get; set; }
        /// <summary>
        /// 公示价格二
        /// </summary>
        public string QuotationTwo { get; set; }
        /// <summary>
        /// 公示备注二
        /// </summary>
        public string RemarkTwo { get; set; }
        /// <summary>
        /// 公示公司三
        /// </summary>
        public string BidderThree { get; set; }
        /// <summary>
        /// 公示价格三
        /// </summary>
        public string QuotationThree { get; set; }
        /// <summary>
        /// 公示备注三
        /// </summary>
        public string RemarkThree { get; set; }
        /// <summary>
        /// 拟推荐单位
        /// </summary>
        public string RecommendUnit { get; set; }
        /// <summary>
        /// 推荐理由
        /// </summary>
        public string RecommendReason { get; set; }
        // <summary>
        /// 创建人Id 对应表 Base_User 中的 ObjeId
        /// </summary>
        public string hr_sqr { get; set; }
    }
}
