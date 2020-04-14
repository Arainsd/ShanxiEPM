using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class TzPeopleChgApplyView
    {
        /// <summary>
        /// 附件
        /// </summary>
        public string Fj { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string hr_sqr { get; set; }
        ///申请部门        
        public string dep_sqbm { get; set; }
        /// <summary>
        /// 申请单位
        /// </summary>
        public string sub_sqdw { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        public string date_sqrq { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string txt_xmmc { get; set; }
        /// <summary>
        /// 建设地址
        /// </summary>
        public string txt_jsdz { get; set; }
        /// <summary>
        /// 施工（监理）单位
        /// </summary>
        public string sub_sgdw { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string txt_fzr { get; set; }
        /// <summary>
        /// 分管领导
        /// </summary>
        public string hr_fgld { get; set; }
        /// <summary>
        /// 部门负责人
        /// </summary>
        public string hr_bmfzr { get; set; }
        /// <summary>
        /// 主管领导
        /// </summary>
        public string hr_zgld { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        public string fj { get; set; }

        public List<TzPeopleChgApplyItem> list { get; set; }

        public class TzPeopleChgApplyItem
        {
            /// <summary>
            /// 变更前执业证书名称
            /// </summary>
            public string txt_bgqzyzsmc { get; set; }
            /// <summary>
            /// 变更后执业证书名称
            /// </summary>
            public string txt_bghzyzsmc { get; set; }
            /// <summary>
            /// 变更岗位
            /// </summary>
            public string txt_bggw { get; set; }
            /// <summary>
            /// 变更前人员
            /// </summary>
            public string txt_bgqry { get; set; }
            /// <summary>
            /// 变更后人员
            /// </summary>
            public string txt_bghry { get; set; }
            /// <summary>
            /// 变更前执业证书号
            /// </summary>
            public string txt_bgqzyzsh { get; set; }
            /// <summary>
            /// 变更后执业证书号
            /// </summary>
            public string txt_bghzyzsh { get; set; }
        }
    }
}
