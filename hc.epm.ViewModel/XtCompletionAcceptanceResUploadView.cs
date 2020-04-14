using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class XtCompletionAcceptanceResUploadView
    {

        ///<summary>
        ///项目名称
        ///</summary>
        public string ProjectName { get; set; }

        ///<summary>
        ///验收内容
        ///</summary>
        public string Content { get; set; }
        /// <summary>
        /// 验收次数
        /// </summary>
        public string Num { get; set; }
        /// <summary>
        /// 验收单位
        /// </summary>
        public string RecCompanyName { get; set; }
        /// <summary>
        /// 验收人员
        /// </summary>
        public string RecUserName { get; set; }
        /// <summary>
        /// 验收时间
        /// </summary>
        public string RecTime { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 验收结果
        /// </summary>
        public string AcceptanceResult { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string State { get; set; }
        ///<summary>
        ///备注
        ///</summary>
        public string Remark { get; set; }
        ///<summary>
        ///创建单位Name
        ///</summary>
        public string CrtCompanyName { get; set; }
        /// <summary>
        /// 整改内容
        /// </summary>
        public string RectifContent { get; set; }
        ///<summary>
        ///项目主体Name
        ///</summary>
        public string SubjectName { get; set; }
        ///<summary>
        ///项目类型Name
        ///</summary>
        public string ProjectTypeName { get; set; }
        ///<summary>
        ///项目开始时间
        ///</summary>
        public string StartDate { get; set; }

        ///<summary>
        ///项目结束时间
        ///</summary>
        public string EndDate { get; set; }
        ///<summary>
        ///分公司项目负责人Name
        ///</summary>
        public string ContactUserName { get; set; }
        ///<summary>
        ///分公司项目负责人电话
        ///</summary>
        public string ContactPhone { get; set; }
        ///<summary>
        ///地址
        ///</summary>
        public string Address { get; set; }
        /// <summary>
        /// 验收类型
        /// </summary>
        public string AcceptanceType { get; set; }
        ///<summary>
        ///简介
        ///</summary>
        public string Description { get; set; }
        /// <summary>
        /// 验收意见说明
        /// </summary>
        public string Acceptcomments { get; set; }
        /// <summary>
        /// 附件类型
        /// </summary>
        public string Temp_TzAttachs { get; set; }
        /// <summary>
        /// 整改单位
        /// </summary>
        public string Rectificationunit { get; set; }
        // <summary>
        /// 创建人Id 对应表 Base_User 中的 ObjeId
        /// </summary>
        public string hr_sqr { get; set; }
    }
}
