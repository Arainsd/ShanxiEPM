using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;
using System.Collections.Generic;

namespace hc.epm.ViewModel
{
    public class ProjectApprovalView
    {
        public ProjectApprovalView()
        {
            FileList = new List<Base_Files>();
            ProjectPolit = new Epm_TzProjectPolit();
            ProjectAuditRecordList = new List<Epm_ProjectAuditRecord>();
        }

        public Epm_TzProjectPolit ProjectPolit { get; set; }
        public List<Epm_ProjectAuditRecord> ProjectAuditRecordList { get; set; }

        public List<Base_Files> FileList { get; set; }
    }

    /// <summary>
    /// 协同审批流程提交对象
    /// </summary>
    public class ProjectApprovalApplyView
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public string txt_xmmc { get; set; }

        /// <summary>
        /// 申请单位 ID 对应 Base_Company 表中的 ObjeId
        /// </summary>
        public string sub_sqdw { get; set; }

        /// <summary>
        /// 开工日期 yyyy-MM-dd
        /// </summary>
        public string date_kgrq { get; set; }

        /// <summary>
        /// 竣工日期 yyyy-MM-dd
        /// </summary>
        public string date_jgrq { get; set; }

        /// <summary>
        /// 验收日期 yyyy-MM-dd
        /// </summary>
        public string date_ysrq { get; set; }

        /// <summary>
        /// 验收问题整改完成时间 yyyy-MM-dd
        /// </summary>
        public string date_zgrq { get; set; }

        /// <summary>
        /// 计划完成工程决算时间 yyyy-MM-dd
        /// </summary>
        public string date_jssj { get; set; }

        /// <summary>
        /// 计划完成工程审计时间 yyyy-MM-dd
        /// </summary>
        public string date_sjsj { get; set; }

        /// <summary>
        /// 资料是否齐全
        /// </summary>
        public string txts_zlsfqq { get; set; }

        /// <summary>
        /// 分公司验收意见
        /// </summary>
        public string txt_ysyj { get; set; }

        /// <summary>
        /// 发起时间 yyyy-MM-dd
        /// </summary>
        public string date_sqrq { get; set; }

        /// <summary>
        /// 创建人Id 对应表 Base_User 中的 ObjeId
        /// </summary>
        public string hr_sqr { get; set; }

        /// <summary>
        /// 相关附件 销售企业二级单位验收主要事项表
        /// </summary>
        public string file_fj { get; set; }

        /// <summary>
        /// 施工转生产界面交接确认单
        /// </summary>
        public string file_sgdw { get; set; }

        /// <summary>
        /// 工程交接证书
        /// </summary>
        public string file_gcjjzs { get; set; }

    }
}
