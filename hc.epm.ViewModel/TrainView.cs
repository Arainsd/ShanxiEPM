using hc.epm.DataModel.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    public class TrainView
    {
        public long Id { get; set; }
        ///<summary>
		///项目表Id
		///</summary>
		public long? ProjectId { get; set; }

        ///<summary>
        ///项目名称
        ///</summary>
        public string ProjectName { get; set; }

        ///<summary>
        ///业主单位Id
        ///</summary>
        public long? CompanyId { get; set; }

        ///<summary>
        ///业主单位Name
        ///</summary>
        public string CompanyName { get; set; }

        ///<summary>
        ///培训类型Key
        ///</summary>
        public string TrainTypeNo { get; set; }

        ///<summary>
        ///培训类型Value
        ///</summary>
        public string TrainTypeName { get; set; }

        ///<summary>
        ///标题
        ///</summary>
        public string Title { get; set; }

        ///<summary>
        ///内容
        ///</summary>
        public string Content { get; set; }

        ///<summary>
        ///培训开始时间
        ///</summary>
        public DateTime? StartTime { get; set; }

        ///<summary>
        ///培训结束时间
        ///</summary>
        public DateTime? EndTime { get; set; }

        ///<summary>
        ///培训单位Id
        ///</summary>
        public long? TrainCompanyId { get; set; }

        ///<summary>
        ///培训单位Name
        ///</summary>
        public string TrainCompanyName { get; set; }

        ///<summary>
        ///培训人员Id
        ///</summary>
        public long? TrainUserId { get; set; }

        ///<summary>
        ///培训人员Name
        ///</summary>
        public string TrainUserName { get; set; }

        ///<summary>
        ///状态[10待处理,25确认通过,30已驳回,40已废弃]枚举
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
        ///涉及单位Id
        ///</summary>
        public string CompanyIds { get; set; }

        ///<summary>
        ///涉及单位Name
        ///</summary>
        public string CompanyNames { get; set; }

        /// <summary>
        /// 创建人 ID
        /// </summary>
        public long CreateUserId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUserName { get; set; }

        public DateTime? CreateTime { get; set; }

        public List<Epm_TrainCompany> TrainCompany { get; set; }
    }
}
