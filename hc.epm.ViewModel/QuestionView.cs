using System;
using System.Collections.Generic;
using hc.epm.DataModel.Basic;
using hc.epm.DataModel.Business;

namespace hc.epm.ViewModel
{
    public class QuestionView
    {
        public QuestionView()
        {
            Attachs = new List<Base_Files>();
            QuestionTracks = new List<Epm_QuestionTrack>();
            QuestionBims = new List<Epm_QuestionBIM>();
            QuestionUsers = new List<Epm_QuestionUser>();
        }


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
        ///标题
        ///</summary>
        public string Title { get; set; }

        ///<summary>
        ///描述
        ///</summary>
        public string Description { get; set; }

        ///<summary>
        ///建议
        ///</summary>
        public string Proposal { get; set; }

        ///<summary>
        ///是否重大事故，1是，2不是，枚举
        ///</summary>
        public bool IsAccident { get; set; }

        ///<summary>
        ///业务类型Key
        ///</summary>
        public string BusinessTypeNo { get; set; }

        ///<summary>
        ///业务类型Value
        ///</summary>
        public string BusinessTypeName { get; set; }

        ///<summary>
        ///业务类型Key
        ///</summary>
        public long? BusinessId { get; set; }

        ///<summary>
        ///提报人Id
        ///</summary>
        public long SubmitUserId { get; set; }

        ///<summary>
        ///提报人Name
        ///</summary>
        public string SubmitUserName { get; set; }

        ///<summary>
        ///提报时间
        ///</summary>
        public DateTime? SubmitTime { get; set; }

        ///<summary>
        ///提报单位Id
        ///</summary>
        public long SubmitCompanyId { get; set; }

        ///<summary>
        ///提报单位Name
        ///</summary>
        public string SubmitCompanyName { get; set; }

        ///<summary>
        ///接收单位Id
        ///</summary>
        public long? RecCompanyId { get; set; }

        ///<summary>
        ///接收UserId
        ///</summary>
        public long? RecUserId { get; set; }

        ///<summary>
        ///接收UserName
        ///</summary>
        public string RecUserName { get; set; }

        ///<summary>
        ///接收单位Name
        ///</summary>
        public string RecCompanyName { get; set; }

        ///<summary>
        ///状态[1正常、2关闭]枚举
        ///</summary>
        public int State { get; set; }

        ///<summary>
        ///关闭时间
        ///</summary>
        public DateTime? CloseTime { get; set; }

        ///<summary>
        ///备注
        ///</summary>
        public string Remark { get; set; }

        ///<summary>
        ///创建单位Id
        ///</summary>
        public long CrtCompanyId { get; set; }

        ///<summary>
        ///创建单位Name
        ///</summary>
        public string CrtCompanyName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        public string CreateUserName { get; set; }
        /// <summary>
        /// 问题类型名称
        /// </summary>
        public string ProblemTypeName { get; set; }

        /// <summary>
        /// 问题类型编码
        /// </summary>
        public string ProblemTypeNo { get; set; }

        public long CreateUserId { get; set; }

        /// <summary>
        /// 问题沟通记录
        /// </summary>
        public List<Epm_QuestionTrack> QuestionTracks { get; set; }

        /// <summary>
        /// 问题关联模型
        /// </summary>
        public List<Epm_QuestionBIM> QuestionBims { get; set; }

        /// <summary>
        /// 问题协同人员
        /// </summary>
        public List<Epm_QuestionUser> QuestionUsers { get; set; }

        /// <summary>
        /// 问题相关附件
        /// </summary>
        public List<Base_Files> Attachs { get; set; }
    }
    
}
