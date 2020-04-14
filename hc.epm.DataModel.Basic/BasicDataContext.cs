using hc.epm.DataModel.Basic;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.DataModel.Basic
{
    public class BasicDataContext : DbContext
    {
        public BasicDataContext() : base("basicConnectionString")
        {

        }
        ///<summary>
        ///用户
        ///</summary>
        public DbSet<Base_User> Base_User { get; set; }
        ///<summary>
        ///状态日志
        ///</summary>
        public DbSet<Base_StatusLog> Base_StatusLog { get; set; }
        ///<summary>
        ///角色权限
        ///</summary>
        public DbSet<Base_RoleRight> Base_RoleRight { get; set; }
        ///<summary>
        ///角色
        ///</summary>
        public DbSet<Base_Role> Base_Role { get; set; }
        ///<summary>
        ///权限
        ///</summary>
        public DbSet<Base_Right> Base_Right { get; set; }
        ///<summary>
        ///电子协议
        ///</summary>
        public DbSet<Base_Protocol> Base_Protocol { get; set; }
        ///<summary>
        ///日志
        ///</summary>
        public DbSet<Base_Log> Base_Log { get; set; }
        ///<summary>
        ///历史密码
        ///</summary>
        public DbSet<Base_HistoryPassword> Base_HistoryPassword { get; set; }
        ///<summary>
        ///附件表
        ///</summary>
        public DbSet<Base_Files> Base_Files { get; set; }
        ///<summary>
        ///字典
        ///</summary>
        public DbSet<Base_Dictionary> Base_Dictionary { get; set; }
        ///<summary>
        ///类型
        ///</summary>
        public DbSet<Base_TypeDictionary> Base_TypeDictionary { get; set; }
        ///<summary>
        ///部门
        ///</summary>
        public DbSet<Base_Dep> Base_Dep { get; set; }
        ///<summary>
        ///系统配置
        ///</summary>
        public DbSet<Base_Config> Base_Config { get; set; }
        ///<summary>
        ///企业
        ///</summary>
        public DbSet<Base_Company> Base_Company { get; set; }
        ///<summary>
        ///用户角色
        ///</summary>
        public DbSet<Base_UserRole> Base_UserRole { get; set; }
        ///<summary>
        ///系统配置
        ///</summary>
        public DbSet<Base_Settings> Base_Settings { get; set; }
        ///<summary>
        ///摄像机设备
        ///</summary>
        public DbSet<Base_VideoManage> Base_VideoManage { get; set; }
        ///<summary>
        ///区域
        ///</summary>
        public DbSet<Base_Region> Base_Region { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}

