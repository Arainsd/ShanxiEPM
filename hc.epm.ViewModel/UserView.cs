using hc.epm.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.ViewModel
{
    /// <summary>
    /// 当前登录用户信息session view
    /// </summary>
    [Serializable]
    public class UserView
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        ///数字证书SN
        /// </summary>
        public string CANumber { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户编码
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 岗位信息
        /// </summary>
        public string PostValue { get; set; }

        /// <summary>
        /// 职称
        /// </summary>
        public string ProfessionalValue { get; set; }

        /// <summary>
        /// 职业资格（职务）
        /// </summary>
        public string ProfessionalQualificationValue { get; set; }

        /// <summary>
        /// 性别 1 男 0 女
        /// </summary>
        public bool Sex { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 角色身份类型,可能有多个，但是每次登陆的时候肯定是一个身份
        /// </summary>
        public RoleType RoleType { get; set; }
        /// <summary>
        /// 所属企业id
        /// </summary>
        public long CompanyId { get; set; }
        /// <summary>
        /// 企业类型
        /// </summary>
        public string CompanyType { get; set; }
        /// <summary>
        /// 企业编码
        /// </summary>
        public string CompanyNo { get; set; }
        /// <summary>
        /// 企业名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 所拥有的权限ids
        /// </summary>
        public List<long> RightIds { get; set; }
        /// <summary>
        /// 所拥有的权限字典
        /// </summary>
        public Dictionary<string, string> Rights { get; set; }
        /// <summary>
        /// 所属于的角色ids
        /// </summary>
        public List<long> RoleIds { get; set; }

        /// <summary>
        /// 所拥有的菜单Ids
        /// </summary>
        public List<long> MenuIds { get; set; }

        /// <summary>
        /// android token
        /// </summary>
        public string AndroidToken { get; set; }
        /// <summary>
        /// android token 失效时间
        /// </summary>
        public DateTime? AndroidTokenTime { get; set; }
        /// <summary>
        /// ios token
        /// </summary>
        public string IosToken { get; set; }
        /// <summary>
        /// ios token 失效时间
        /// </summary>
        public DateTime? IosTokenTime { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 用户负责在建项目
        /// </summary>
        public Dictionary<long, string> Project { get; set; }
        
        /// <summary>
        /// 微信
        /// </summary>
        public string WeChat { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        public string Qq { get; set; }

        /// <summary>
        /// 部门名称Id
        /// </summary>
        public long DepartmentId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }
    }



    /// <summary>
    /// 用户列表展示模型
    /// </summary>
    public class UserListView
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户编码
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 所属企业id
        /// </summary>
        public long CompanyId { get; set; }
        /// <summary>
        /// 企业编码
        /// </summary>
        public string CompanyNo { get; set; }

        /// <summary>
        /// 企业类型
        /// </summary>
        public string CompanyType { get; set; }

        /// <summary>
        /// 企业类型名称
        /// </summary>
        public string CompanyTypeName { get; set; }

        /// <summary>
        /// 企业名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 微信
        /// </summary>
        public string WeChat { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        public string Qq { get; set; }

        public bool IsLock { get; set; }
        /// <summary>
        /// 岗位
        /// </summary>
        public string PostValue { get; set; }
        /// <summary>
        /// 岗位Value
        /// </summary>
        public string Post { get; set; }

        /// <summary>
        /// 职称
        /// </summary>
        public string ProfessionalValue { get; set; }
        /// <summary>
        /// 职称value
        /// </summary>
        public string Professional { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>
        public string ProfessionalQualificationValue { get; set; }
        /// <summary>
        /// 资格Value
        /// </summary>
        public string ProfessionalQualification { get; set; }

        public DateTime? CreateTime { get; set; }
    }
}
