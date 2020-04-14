using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class GcGoodsOrdersApplyView
    {
        /// <summary>
        /// 合同名称
        /// </summary>
        public string txt_xmtm { get; set; }
        /// <summary>
        /// erp订单号
        /// </summary>
        public string txt_erp { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string txt_xmmc { get; set; }
        /// <summary>
        /// 发往地址
        /// </summary>
        public string txt_fwdz { get; set; }
        /// <summary>
        /// 选择方式
        /// </summary>
        public string select_xzfs { get; set; }
        /// <summary>
        /// 收件人
        /// </summary>
        public string txt_sjr { get; set; }
        /// <summary>
        /// 收件人电话
        /// </summary>
        public string txt_dh { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string txt_lxr { get; set; }
        /// <summary>
        /// 联系人电话
        /// </summary>
        public string txt_lxrdh { get; set; }
        /// <summary>
        /// 物资种类
        /// </summary>
        public string select_wzzl { get; set; }
        /// <summary>
        /// No.订购单
        /// </summary>
        public string txt_dgd { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string hr_sqr { get; set; }
        /// <summary>
        /// 申请单位
        /// </summary>
        public string sub_sqdw { get; set; }
        /// <summary>
        /// 申请部门
        /// </summary>
        public string dep_sqbm { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public string date_sqsj { get; set; }
        /// <summary>
        /// 主管领导
        /// </summary>
        public string hr_zgld { get; set; }
        /// <summary>
        /// 分管领导
        /// </summary>
        public string hr_fgld { get; set; }
        /// <summary>
        /// 部门负责人
        /// </summary>
        public string hr_bmfzr { get; set; }
        /// <summary>
        /// 处室负责人
        /// </summary>
        public string hr_csfzr { get; set; }
        /// <summary>
        /// 批文号
        /// </summary>
        public string txt { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string txt_yb { get; set; }
        /// <summary>
        /// 当前年
        /// </summary>
        public string date_dqn { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>
        public string txt_gysmc { get; set; }
        /// <summary>
        /// 供应商名称（新）
        /// </summary>
        public string txt_gysmcx { get; set; }
        /// <summary>
        /// 供应商名称（打印）
        /// </summary>
        public string txt_gysmc_dy { get; set; }
        /// <summary>
        /// 物资种类（新）
        /// </summary>
        public string select_wzzlx { get; set; }
        /// <summary>
        /// 项目名称（新）
        /// </summary>
        public string txt_xmmcx { get; set; }
        /// <summary>
        /// 供应商地址
        /// </summary>
        public string txt_gysdz { get; set; }
        /// <summary>
        /// 供应商邮编
        /// </summary>
        public string txt_gysyb { get; set; }
        /// <summary>
        /// 物资种类（new）
        /// </summary>
        public string select_wzzln { get; set; }
        /// <summary>
        /// 主管领导-角色
        /// </summary>
        public string role_zgld { get; set; }
        /// <summary>
        /// 投资处负责人
        /// </summary>
        public string role_tzcfzr { get; set; }
        /// <summary>
        /// 法律事务岗
        /// </summary>
        public string role_flswg { get; set; }
        /// <summary>
        /// 批准日期
        /// </summary>
        public string date_pzrq { get; set; }
        /// <summary>
        /// 合同报审序号
        /// </summary>
        public string txt_htbsxh { get; set; }
        /// <summary>
        /// 隶属
        /// </summary>
        public string sub_dw { get; set; }
        

        public List<GcGoodsOrdersItem> list { get; set; }

        public class GcGoodsOrdersItem
        {
            /// <summary>
            /// 站名（新）
            /// </summary>
            public string dep_zm { get; set; }
            /// <summary>
            /// 站名（作废）
            /// </summary>
            public string dep_jyz { get; set; }
            /// <summary>
            /// 品名
            /// </summary>
            public string txt_pm { get; set; }
            /// <summary>
            /// 规格
            /// </summary>
            public string txt_gg { get; set; }
            /// <summary>
            /// 单价
            /// </summary>
            public string float_dj { get; set; }
            /// <summary>
            /// 金额
            /// </summary>
            public string float_je { get; set; }
            /// <summary>
            /// 到货地址
            /// </summary>
            public string txt_dhdz { get; set; }
            /// <summary>
            /// 备注
            /// </summary>
            public string txts_bz { get; set; }
            /// <summary>
            /// 数量
            /// </summary>
            public string int_mount { get; set; }
            /// <summary>
            /// 站名
            /// </summary>
            public string txt_zm { get; set; }
        }
    }
}
