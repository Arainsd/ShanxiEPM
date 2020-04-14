using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using hc.epm.DataModel.Business;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 合同详情
    /// </summary>
    public class ContractView
    {
        /// <summary>
        /// 合同 ID
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 合同编码
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 所属项目
        /// </summary>
        public long projectId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string projectName { get; set; }

        /// <summary>
        /// 合同名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 甲方 ID
        /// </summary>
        public long firstPartyId { get; set; }

        /// <summary>
        /// 甲方名称
        /// </summary>
        public string firstPartyName { get; set; }

        /// <summary>
        /// 乙方 ID
        /// </summary>
        public long secondPartyId { get; set; }

        /// <summary>
        /// 乙方名称
        /// </summary>
        public string secondPartyName { get; set; }

        /// <summary>
        /// 工期(单位：天)
        /// </summary>
        public decimal buildDays { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public string startTime { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public string endDate { get; set; }

        /// <summary>
        /// 合同金额(单位：万元)
        /// </summary>
        public decimal amount { get; set; }

        /// <summary>
        /// 签订日期
        /// </summary>
        public string signTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int state { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public string createTime { get; set; }

        /// <summary>
        /// 将实体信息转换成视图
        /// </summary>
        /// <param name="model">合同实体</param>
        /// <returns></returns>
        public static ContractView EntityToView(Epm_Contract model)
        {
            ContractView view = new ContractView();
            if (model != null)
            {
                view.id = model.Id;
                view.projectId = model.ProjectId ?? 0;
                view.projectName = model.ProjectName ?? "";
                view.code = model.Code ?? "";
                view.name = model.Name ?? "";
                view.firstPartyId = model.FirstPartyId ?? 0;
                view.firstPartyName = model.FirstPartyName ?? "";
                view.secondPartyId = model.SecondPartyId ?? 0;
                view.secondPartyName = model.SecondPartyName ?? "";
                view.buildDays = model.BuildDays ?? 0;
                view.startTime = string.Format("{0:yyyy-MM-dd}", model.StartTime);
                view.endDate = string.Format("{0:yyyy-MM-dd}", model.EndTime);
                view.amount = model.Amount ?? 0;
                view.signTime = string.Format("{0:yyyy-MM-dd}", model.SignTime);
                view.state = model.State ?? 0;
                view.remark = model.Remark ?? "";
                view.createTime = string.Format("{0:yyyy-MM-dd}", model.CreateTime);
            }
            return view;
        }
    }
}