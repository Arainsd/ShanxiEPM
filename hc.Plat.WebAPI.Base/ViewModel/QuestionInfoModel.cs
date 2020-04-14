using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 提交问题模型
    /// </summary>
    public class QuestionInfoModel
    {
        /// <summary>
        /// 所属项目 ID
        /// </summary>
        public long projectId { get; set; }

        /// <summary>
        /// 所属项目名称
        /// </summary>
        public string projectName { get; set; }

        /// <summary>
        /// 问题类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 问题类型名称
        /// </summary>
        public string typeName { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        public string businessType { get; set; }

        /// <summary>
        /// 业务类型名称
        /// </summary>
        public string businessName { get; set; }

        /// <summary>
        /// 业务 ID
        /// </summary>
        public long businessId { get; set; }


        /// <summary>
        /// 问题标题
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 问题描述
        /// </summary>
        public string desc { get; set; }

        /// <summary>
        /// 问题建议
        /// </summary>
        public string proposal { get; set; }

        /// <summary>
        /// 是否重大事故
        /// </summary>
        public bool isAccident { get; set; }

        /// <summary>
        /// 接收单位
        /// </summary>
        public long companyId { get; set; }

        /// <summary>
        /// 接收单位名称
        /// </summary>
        public string companyName { get; set; }

        /// <summary>
        /// 关联模型
        /// </summary>
        public string bim { get; set; }
    }
}