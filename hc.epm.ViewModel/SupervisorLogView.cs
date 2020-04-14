using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using hc.epm.DataModel.Basic;
using hc.epm.DataModel.BaseCore;
using hc.epm.DataModel.Business;

namespace hc.epm.ViewModel
{
    public class SupervisorLogView : BaseBusiness
    {
        public SupervisorLogView()
        {
            SupervisorLogCompanys = new List<Epm_SupervisorLogCompany>();

            Attachs = new List<Base_Files>();
            Attachs1 = new List<Base_Files>();
            SenceList = new List<WorkUploadRealSceneView>();
        }

        ///<summary>
        ///项目表Id
        ///</summary>
        public long? ProjectId { get; set; }

        ///<summary>
        ///项目名称
        ///</summary>
        public string ProjectName { get; set; }

        ///<summary>
        ///提交时间
        ///</summary>
        public DateTime? SubmitTime { get; set; }

        ///<summary>
        ///天气类型Key
        ///</summary>
        public string TypeNo { get; set; }

        ///<summary>
        ///天气类型Value
        ///</summary>
        public string TypeName { get; set; }

        ///<summary>
        ///日志内容
        ///</summary>
        public string Content { get; set; }

        ///<summary>
        ///状态
        ///</summary>
        public int? State { get; set; }

        ///<summary>
        ///备注
        ///</summary>
        public string Remark { get; set; }

        ///<summary>
        ///创建单位Id
        ///</summary>
        public long? CrtCompanyId { get; set; }

        ///<summary>
        ///创建单位Name
        ///</summary>
        public string CrtCompanyName { get; set; }

        ///<summary>
        /// 风力
        ///</summary>
        public string WindPower { get; set; }

        ///<summary>
        /// 温度
        ///</summary>
        public string Temperature { get; set; }

        ///<summary>
        /// 所属计划
        ///</summary>
        public string PlanId { get; set; }

        ///<summary>
        /// 计划名称
        ///</summary>
        public string PlanName { get; set; }

        ///<summary>
        /// 明日计划
        ///</summary>
        public string TomorrowProject { get; set; }

        ///<summary>
        /// 进度
        ///</summary>
        public string Schedule { get; set; }

        ///<summary>
        /// 进度延期原因
        ///</summary>
        public string Reason { get; set; }

        public long? WorkId { get; set; }
        public int Type { get; set; }
        /// <summary>
        /// 工种
        /// </summary>
        public int WorkPeopleType { get; set; }
        /// <summary>
        /// 施工单位列表
        /// </summary>
        public List<Epm_SupervisorLogCompany> SupervisorLogCompanys { get; set; }


        public List<Base_Files> Attachs { get; set; }
        public List<Base_Files> Attachs1 { get; set; }

        public List<WorkUploadRealSceneView> SenceList { get; set; }

        public int planState { get; set; }
        public string nextPlanId { get; set; }
        public string nextPlanName { get; set; }

        public List<Epm_ProjectCompany> Epm_ProjectCompany { get; set; }
    }
}