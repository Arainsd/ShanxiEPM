using System;
using hc.Plat.Common.Global;
using System.IdentityModel.Selectors;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Configuration;
using System.ServiceModel;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace hc.Plat.Service.Base
{
    /// <summary>
    /// 实现基础服务的处理
    /// </summary>
    public class BaseEntity : IBase
    {

        /// <summary>
        /// test
        /// </summary>
        public virtual Result<object> Test()
        {
            Result<object> result = new Result<object>();
            try
            {
                result.Data = 1;
                result.Flag = EResultFlag.Success;
            }
            catch (Exception e)
            {
                result.Flag = EResultFlag.Failure;
                result.Exception = new ExceptionEx(e, this.GetType().Module + ":" + this.GetType().ToString());
            }
            return result;
        }




    }

    /// <summary>
    /// MyUserNamePasswordValidator
    /// </summary>
    public class MyUserNamePasswordValidator : System.IdentityModel.Selectors.UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            string FilePath = AppDomain.CurrentDomain.BaseDirectory;
            string user = "";
            string pass = "";
            string msg = DesTool.LoadCertUserPass(FilePath, out user, out pass);
            if (msg != "")
            {
                throw new System.ServiceModel.Security.SecurityNegotiationException(msg);
            }
            if (userName != user || password != pass)
            {
                throw new System.ServiceModel.Security.SecurityNegotiationException("验证用户名和密码时，未通过检测");
            }
        }
    }


}
