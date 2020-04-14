using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using hc.epm.DataModel.Business;

namespace hc.epm.ViewModel.AppView
{
    /// <summary>
    /// APP 问题信息实体
    /// </summary>
    public class QuestionModel
    {
        /// <summary>
        /// 问题 ID
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 问题标题
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 回复数
        /// </summary>
        public int answerCount { get; set; }

        /// <summary>
        /// 发布人Id
        /// </summary>
        public long? submitUserId { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime? submitTime { get; set; }

        /// <summary>
        /// 回复人头像
        /// </summary>
        public string headUrl { get; set; }

        /// <summary>
        /// 问题类型
        /// </summary>
        public string type { get; set; }

        public string businessChild { get; set; }

        /// <summary>
        /// 回复人 ID
        /// </summary>
        public long createUserId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int state { get; set; }

        /// <summary>
        /// 发布人
        /// </summary>
        public string submitUserName { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string workContent { get; set; }

        /// <summary>
        /// 业务 ID
        /// </summary>
        public long businessId { get; set; }

        /// <summary>
        /// 业务状态
        /// </summary>
        public int businessState { get; set; }

        /// <summary>
        /// 项目 ID
        /// </summary>
        public long projectId { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string projectName { get; set; }

        /// <summary>
        /// 业务操作
        /// </summary>
        public string action { get; set; }

        public static QuestionModel QuestionToView(Epm_Question model)
        {
            QuestionModel view = new QuestionModel();
            if (model != null)
            {
                view.id = model.Id;
                view.name = model.Title;
                view.submitUserId = model.SubmitUserId;
                view.submitTime = model.SubmitTime;
                view.type = model.BusinessTypeNo ?? "";
                view.businessChild = "";
                view.state = model.State ?? 0;
                view.createUserId = model.SubmitUserId ?? 0;
                view.submitUserName = model.SubmitUserName ?? "";
                view.workContent = model.Description ?? "";
                view.businessId = model.BusinessId ?? 0;
                view.state = model.State ?? 0;
            }
            return view;
        }
    }
}
