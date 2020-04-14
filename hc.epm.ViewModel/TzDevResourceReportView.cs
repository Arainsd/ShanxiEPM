using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class TzDevResourceReportView
    {
        /// <summary>
        /// 申报人
        /// </summary>
        public string hr_sbr { get; set; }
        /// <summary>
        /// 申报日期
        /// </summary>
        public string data_sbrq { get; set; }
        /// <summary>
        /// 申报单位
        /// </summary>
        public string dept_sbdw { get; set; }
        /// <summary>
        /// 部门负责人
        /// </summary>
        public string hr_bfzr { get; set; }
        /// <summary>
        /// 分管领导（废弃）
        /// </summary>
        public string hr_fgld { get; set; }
        /// <summary>
        /// 申报部门
        /// </summary>
        public string dpt_bm { get; set; }
        /// <summary>
        /// 分管领导
        /// </summary>
        public string hr_fglds { get; set; }

        public List<TzDevResourceItem> list { get; set; }

        public class TzDevResourceItem
        {
            /// <summary>
            /// 地市
            /// </summary>
            public string select_ds { get; set; }
            /// <summary>
            /// 区县
            /// </summary>
            public string txt_qx { get; set; }
            /// <summary>
            /// 项目名称
            /// </summary>
            public string txt_xmmc { get; set; }
            /// <summary>
            /// 项目位置
            /// </summary>
            public string txt_xmwz { get; set; }
            /// <summary>
            /// 项目性质
            /// </summary>
            public string select_xmxz { get; set; }
            /// <summary>
            /// 预计总投资额（万元）
            /// </summary>
            public string int_yjztz { get; set; }
            /// <summary>
            /// 可研销售
            /// </summary>
            public string int_kyxs { get; set; }
            /// <summary>
            /// 汽柴比
            /// </summary>
            public string int_qcb { get; set; }
            /// <summary>
            /// 列为意向站时间
            /// </summary>
            public string data_lwyxzsj { get; set; }
            /// <summary>
            /// 计划立项时间
            /// </summary>
            public string data_jhlxsj { get; set; }
            /// <summary>
            /// 业主姓名
            /// </summary>
            public string txt_yzxm { get; set; }
            /// <summary>
            /// 业主电话
            /// </summary>
            public string txt_yzdh { get; set; }
            /// <summary>
            /// 备注
            /// </summary>
            public string txt_bz { get; set; }
        }

    }
}
