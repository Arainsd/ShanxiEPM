using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
   public class XtCompletionAcceptanceView
    {

        ///<summary>
        ///项目名称
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        ///项目主体No
        ///</summary>
        public string SubjectNo { get; set; }

        ///<summary>
        ///项目主体Name
        ///</summary>
        public string SubjectName { get; set; }

        ///<summary>
        ///项目主体Id
        ///</summary>
        public string ProjectSubjectId { get; set; }

        ///<summary>
        ///项目主体ShortName
        ///</summary>
        public string ProjectSubjectShortName { get; set; }

        ///<summary>
        ///项目主体Name
        ///</summary>
        public string ProjectSubjectName { get; set; }

        ///<summary>
        ///所属分公司Id
        ///</summary>
        public long? CompanyId { get; set; }

        ///<summary>
        ///所属分公司
        ///</summary>
        public string CompanyName { get; set; }

        ///<summary>
        ///建筑类型Key
        ///</summary>
        public string BuildNo { get; set; }

        ///<summary>
        ///建筑类型Value
        ///</summary>
        public string BuildName { get; set; }

        ///<summary>
        ///地址
        ///</summary>
        public string Address { get; set; }

        ///<summary>
        ///项目开始时间
        ///</summary>
        public string StartDate { get; set; }

        ///<summary>
        ///项目结束时间
        ///</summary>
        public string EndDate { get; set; }

        ///<summary>
        ///项目类型No
        ///</summary>
        public string ProjectTypeNo { get; set; }

        ///<summary>
        ///项目类型Name
        ///</summary>
        public string ProjectTypeName { get; set; }

        ///<summary>
        ///项目金额，单位万元
        ///</summary>
        public string Amount { get; set; }

        ///<summary>
        ///简介
        ///</summary>
        public string Description { get; set; }

        ///<summary>
        ///分公司项目负责人Id
        ///</summary>
        public long? ContactUserId { get; set; }

        ///<summary>
        ///分公司项目负责人Name
        ///</summary>
        public string ContactUserName { get; set; }

        ///<summary>
        ///分公司项目负责人电话
        ///</summary>
        public string ContactPhone { get; set; }

        ///<summary>
        ///备注
        ///</summary>
        public string Remark { get; set; }

        ///<summary>
        ///创建单位Id
        ///</summary>
        public string CrtCompanyId { get; set; }

        ///<summary>
        ///创建单位Name
        ///</summary>
        public string CrtCompanyName { get; set; }


        /// <summary>
        /// 项目性质编码
        /// </summary>
        public string ProjectNature { get; set; }

        /// <summary>
        /// 项目性质名称
        /// </summary>
        public string ProjectNatureName { get; set; }


        /// <summary>
        /// 项目经理
        /// </summary>
        public string PMId { get; set; }
        /// <summary>
        /// 项目经理
        /// </summary>
        public string PMName { get; set; }

        /// <summary>
        /// 附件类型
        /// </summary>
        public string Temp_TzAttachs { get; set; }
        // <summary>
        /// 创建人Id 对应表 Base_User 中的 ObjeId
        /// </summary>
        public string hr_sqr { get; set; }
    }
}
