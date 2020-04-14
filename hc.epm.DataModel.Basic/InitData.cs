using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.DataModel.Basic
{
    public class InitData : DropCreateDatabaseIfModelChanges<BasicDataContext>
    {
        protected override void Seed(BasicDataContext context)
        {
            //    var users = new List<UserInfo>()
            //    {
            //        new UserInfo(){UserName="test",Password="test",CANumber="2841377068CEFBBBBD20272475BC6EEEA7301953",Role="企业"},
            //        new UserInfo(){UserName="admin",Password="admin",CANumber="",Role="管理员"}
            //    };
            //    var projects = new List<Project>()
            //    {
            //        new Project(){ Name="测试项目",Number="101" }
            //};
            //    users.ForEach(user => context.UserInfo.Add(user));
            //    projects.ForEach(poject => context.Project.Add(poject));
            //    context.SaveChanges();
            base.Seed(context);
        }

        public override void InitializeDatabase(BasicDataContext context)
        {
            base.InitializeDatabase(context);

        }
    }
}
