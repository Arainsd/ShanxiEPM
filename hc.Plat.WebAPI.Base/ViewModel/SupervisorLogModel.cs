using System;
using System.Collections.Generic;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 监理日志
    /// </summary>
    public class SupervisorLogModel
    {
        public SupervisorLogModel()
        {
            files = new List<FileView>();
        }

        /// <summary>
        /// 所属项目 ID
        /// </summary>
        public long projectId { get; set; }

        /// <summary>
        /// 所属项目名称
        /// </summary>
        public string projectName { get; set; }

        /// <summary>
        /// 监理日志填报日期
        /// </summary>
        public DateTime submitTime { get; set; }

        /// <summary>
        /// 天气类型编号
        /// </summary>
        public string weatherNo { get; set; }

        /// <summary>
        /// 天气类型名称
        /// </summary>
        public string weatherName { get; set; }

        /// <summary>
        /// 风力
        /// </summary>
        public string windPower { get; set; }

        /// <summary>
        /// 温度
        /// </summary>
        public string temperature { get; set; }

        /// <summary>
        /// 所属计划 ID
        /// </summary>
        public long? planId { get; set; }

        /// <summary>
        /// 所属计划名称
        /// </summary>
        public string planName { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 明日计划
        /// </summary>
        public string tomorrowProject { get; set; }

        /// <summary>
        /// 进度
        /// </summary>
        public string schedule { get; set; }

        /// <summary>
        /// 延期天数
        /// </summary>
        public int delayDay { get; set; }

        /// <summary>
        /// 延期原因
        /// </summary>
        public string reason { get; set; }

        /// <summary>
        /// 现场施工单位
        /// </summary>
        public string workCompanies { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public List<FileView> files { get; set; }
    }

    /// <summary>
    /// 监理日志施工单位
    /// </summary>
    public class SupervisorLogCompanyModel
    {
        /// <summary>
        /// 单位 ID
        /// </summary>
        public long companyId { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string companyName { get; set; }

        /// <summary>
        /// 负责人
        /// </summary>
        public string managerName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 工作人数
        /// </summary>
        public int peopleNum { get; set; }

        /// <summary>
        /// 工种
        /// </summary>
        public string workPeopleType { get; set; }
        /// <summary>
        /// 工种姓名
        /// </summary>
        public string workPeopleTypeName { get; set; }
        /// <summary>
        /// 工种ID
        /// </summary>
        public string workPeopleTypeId { get; set; }
        /// <summary>
        /// 项目经理Name
        /// </summary>
        public string PM { get; set; }
        /// <summary>
        /// 项目经理电话
        /// </summary>
        public string PMPhone { get; set; }
        /// <summary>
        /// 现场负责人
        /// </summary>
        public string LinkMan { get; set; }

        /// <summary>
        /// 现场负责人电话
        /// </summary>
        public string LinkPhone { get; set; }
        /// <summary>
        /// 安全负责人
        /// </summary>
        public string SafeMan { get; set; }
        /// <summary>
        /// 安全负责人电话
        /// </summary>
        public string SafePhone { get; set; }
        /// <summary>
        /// 许可
        /// </summary>
        public string Permit { get; set; }
        /// <summary>
        /// 到场
        /// </summary>
        public string BePresent { get; set; }
        /// <summary>
        /// type
        /// </summary>
        public string type { get; set; }

    }
}