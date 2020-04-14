using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    /// <summary>
    /// 开工申请审批Model
    /// </summary>
    public class TzStartsApplyApprovalView
    {
        /// <summary>
        /// 申请人
        /// </summary>
        public string hr_sqr { get; set; }
        /// <summary>
        /// 所属单位
        /// </summary>
        public string sub_sqdw { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        public string dept_sqbm { get; set; }
        /// <summary>
        /// 申请日期
        /// </summary>
        public string date_sqrq { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string txt_lxdh { get; set; }
        /// <summary>
        /// 建设项目名称
        /// </summary>
        public string txt_jsxmmc { get; set; }
        /// <summary>
        /// 设计规模或能力
        /// </summary>
        public string txt_sjgm { get; set; }
        /// <summary>
        /// 项目建议书批准文号
        /// </summary>
        public string txt_xmjyswh { get; set; }
        /// <summary>
        /// 投资估算-建议（万元）
        /// </summary>
        public string float_gstz_js { get; set; }
        /// <summary>
        /// 可研报告批准文号
        /// </summary>
        public string txt_kybgwh { get; set; }
        /// <summary>
        /// 估算投资-可研（万元）
        /// </summary>
        public string float_gstz_ky { get; set; }
        /// <summary>
        /// 立项批复或初步设计批准文号
        /// </summary>
        public string txt_cbsjwh { get; set; }
        /// <summary>
        /// 批复投资或概算投资（万元）
        /// </summary>
        public string float_cbsjtz { get; set; }
        /// <summary>
        /// 资金来源
        /// </summary>
        public string select_zjly { get; set; }
        /// <summary>
        /// 当年计划投资情况
        /// </summary>
        public string txt_jhtzqk { get; set; }
        /// <summary>
        /// 计划建设工期（开始）
        /// </summary>
        public string date_jsgq_ks { get; set; }
        /// <summary>
        /// 计划建设工期（结束）
        /// </summary>
        public string date_jsgq_js { get; set; }
        /// <summary>
        /// 工程项目概况及主要工程内容和主要工程量
        /// </summary>
        public string txts_gknr { get; set; }
        /// <summary>
        /// 项目管理机构及人员到位情况
        /// </summary>
        public string txts_xmgljg { get; set; }
        /// <summary>
        /// 工程建设总体部署（或施工组织设计）编制情况
        /// </summary>
        public string txts_ztbs { get; set; }
        /// <summary>
        /// 设计单位及图纸落实情况（主要设计单位、施工图交付计划情况）
        /// </summary>
        public string txts_sjdwtz { get; set; }
        /// <summary>
        /// 施工单位落实及施工队伍、机具进场情况（主要施工单位、合同签订情况）
        /// </summary>
        public string txts_sgdwls { get; set; }
        /// <summary>
        /// 监理单位、质量监督部门落实概况
        /// </summary>
        public string txts_jldwls { get; set; }
        /// <summary>
        /// 施工前期准备情况（包括征地、拆迁、四通一平等）
        /// </summary>
        public string txts_sgqqzb { get; set; }
        /// <summary>
        /// 主要设备、材料订货、到货情况
        /// </summary>
        public string txts_yssb { get; set; }
        /// <summary>
        /// 环境影响、劳动安全卫生、消防等审批手续办理情况
        /// </summary>
        public string txts_hjyx { get; set; }
        /// <summary>
        /// 项目管理机构（项目经理部或油库项目组）设立的文件、机构组成和职责分工各一份
        /// </summary>
        public string file_xmgljg { get; set; }
        /// <summary>
        /// 立项批复或项目初步设计批复文件复印件一份
        /// </summary>
        public string file_lxpf { get; set; }
        /// <summary>
        /// 经审批的施工组织设计或工程建设总体部署一份经审批的施工组织设计或工程建设总体部署一份
        /// </summary>
        public string file_sgzzsj { get; set; }
        /// <summary>
        /// 施工进场人员名单及《安全教育合格证》（复印件）
        /// </summary>
        public string file_sgjcry { get; set; }
        /// <summary>
        /// 分公司与供应厂商确定的主要设备材料交付时间表一份
        /// </summary>
        public string file_sbcljf { get; set; }
        /// <summary>
        /// 工程建设项目、还应提供审查通过后的HSE作业指导书、HSE作业计划书和HSE现场检查表
        /// </summary>
        public string file_zyzds { get; set; }
        /// <summary>
        /// 工程形象进度计划表附于此页
        /// </summary>
        public string txts_gcxxjd { get; set; }
        /// <summary>
        /// 工程建设部门负责人
        /// </summary>
        public string hr_bmfzr { get; set; }
        /// <summary>
        /// 分公司工程建主管领导审核
        /// </summary>
        public string hr_fgld { get; set; }
        /// <summary>
        /// 工程建设主管领导
        /// </summary>
        public string hr_zgld { get; set; }
        /// <summary>
        /// 工程形象进度计划表（附件）
        /// </summary>
        public string file_fj { get; set; }
        /// <summary>
        /// 工程形象进度计划表
        /// </summary>
        public string txts_gcxxjdb { get; set; }
        /// <summary>
        /// 投资与建设管理处负责人
        /// </summary>
        public string role_jsry { get; set; }
        /// <summary>
        /// 建设项目名称2
        /// </summary>
        public string txt_jsxmm { get; set; }
        /// <summary>
        /// 签发日期
        /// </summary>
        public string date_qfrq { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string dep_dw { get; set; }
        /// <summary>
        /// 施工单位
        /// </summary>
        public string txt_sgdw { get; set; }
        /// <summary>
        /// 项目经理
        /// </summary>
        public string txt_xmjl { get; set; }
        /// <summary>
        /// 安全考试成绩（施工）
        /// </summary>
        public string txt_aqkscjsg { get; set; }
        /// <summary>
        /// 监理单位
        /// </summary>
        public string txt_jldw { get; set; }
        /// <summary>
        /// 监理工程师
        /// </summary>
        public string txt_jlgcs { get; set; }
        /// <summary>
        /// 安全考试成绩（监理）
        /// </summary>
        public string txt_aqkscjjl { get; set; }
        /// <summary>
        /// 计划建设工期（天）
        /// </summary>
        public string int_jhjsgq { get; set; }
    }
}
