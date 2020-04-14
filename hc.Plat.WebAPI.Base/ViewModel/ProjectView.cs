using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using hc.epm.DataModel.Business;

namespace hc.Plat.WebAPI.Base.ViewModel
{
    /// <summary>
    /// 项目详情
    /// </summary>
    public class ProjectView
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ProjectView()
        {
            files = new List<FileView>();
        }

        /// <summary>
        /// 项目 ID
        /// </summary>
        public long id { get; set; }

        /// <summary>
        /// 项目编码
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 项目总金额(万元)
        /// </summary>
        public decimal amount { get; set; }

        /// <summary>
        /// 项目主体
        /// </summary>
        public string subjectNo { get; set; }

        /// <summary>
        /// 项目主体名称
        /// </summary>
        public string subjectName { get; set; }

        /// <summary>
        /// 单位 ID
        /// </summary>
        public long companyId { get; set; }

        /// <summary>
        /// 单位名称
        /// </summary>
        public string companyName { get; set; }

        /// <summary>
        /// 建筑类型
        /// </summary>
        public string buildNo { get; set; }

        /// <summary>
        /// 建筑类型名称
        /// </summary>
        public string buildName { get; set; }

        /// <summary>
        /// 所在省编码
        /// </summary>
        public string province { get; set; }

        /// <summary>
        /// 所在市编码
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// 所在区县编码
        /// </summary>
        public string area { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string address { get; set; }

        /// <summary>
        /// 项目开始日期
        /// </summary>
        public string startDate { get; set; }

        /// <summary>
        /// 项目结束日期
        /// </summary>
        public string endDate { get; set; }

        /// <summary>
        /// 项目类型
        /// </summary>
        public string projectTypeNo { get; set; }

        /// <summary>
        /// 项目类型名称
        /// </summary>
        public string projectTypeName { get; set; }

        /// <summary>
        /// 项目简介
        /// </summary>
        public string desc { get; set; }

        /// <summary>
        /// 项目负责人
        /// </summary>
        public string contactUserName { get; set; }

        /// <summary>
        /// 负责人电话
        /// </summary>
        public string contactPhone { get; set; }

        /// <summary>
        /// 项目状态
        /// </summary>
        public int state { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string createTime { get; set; }

        /// <summary>
        /// 相关附件
        /// </summary>
        public List<FileView> files { get; set; }

        /// <summary>
        /// 第三方单位
        /// </summary>
        public List<Epm_ProjectCompany> companys { get; set; }

        ///// <summary>
        ///// 将实体模型转换成 View
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public static ProjectView EntityToView(Epm_Project model)
        //{
        //    ProjectView view = new ProjectView();
        //    if (model != null)
        //    {
        //        view.id = model.Id;
        //        view.code = model.Code ?? "";
        //        view.name = model.Name ?? "";
        //        view.amount = model.Amount ?? 0;
        //        view.subjectNo = model.SubjectNo ?? "";
        //        view.subjectName = model.SubjectName ?? "";
        //        view.companyId = model.CompanyId ?? 0;
        //        view.companyName = model.CompanyName ?? "";
        //        view.buildNo = model.BuildNo ?? "";
        //        view.buildName = model.BuildName ?? "";
        //        view.province = model.Province ?? "";
        //        view.city = model.City ?? "";
        //        view.area = model.Area ?? "";
        //        view.address = model.Address ?? "";
        //        view.startDate = string.Format("{0:yyyy-MM-dd}", model.StartDate);
        //        view.endDate = string.Format("{0:yyyy-MM-dd}", model.EndDate);
        //        view.projectTypeNo = model.ProjectTypeNo ?? "";
        //        view.projectTypeName = model.ProjectTypeName ?? "";
        //        view.desc = model.Description ?? "";
        //        view.contactUserName = model.ContactUserName ?? "";
        //        view.contactPhone = model.ContactPhone ?? "";
        //        view.state = model.State ?? 0;
        //        view.createTime = string.Format("{0:yyyy-MM-dd}", model.CreateTime);
        //    }

        //    return view;
        //}
    }
}