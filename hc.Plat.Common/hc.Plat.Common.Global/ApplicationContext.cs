using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Runtime.Serialization;

namespace hc.Plat.Common.Global
{
    [Serializable]
    public class ApplicationContext : Dictionary<string, string>
    {
        public const string KeyOfApplicationContext = "__ApplicationContext";
        internal const string ContextHeaderLocalName = "__ApplicationContext";
        internal const string ContextHeaderNamespace = "urn:hcSinri.com";

        private ApplicationContext()
        { }
        public static ApplicationContext Current
        {
            get
            {
                try
                {
                    if (HttpContext.Current != null)
                    {
                        if (HttpContext.Current.Session[KeyOfApplicationContext] == null)
                        {
                            HttpContext.Current.Session[KeyOfApplicationContext] = new ApplicationContext();
                        }
                        return (ApplicationContext)HttpContext.Current.Session[KeyOfApplicationContext];
                    }
                    if (CallContext.GetData(KeyOfApplicationContext) == null)
                    {
                        CallContext.SetData(KeyOfApplicationContext, new ApplicationContext());
                    }
                    return (ApplicationContext)CallContext.GetData(KeyOfApplicationContext);
                }
                catch (Exception)
                {

                    if (CallContext.GetData(KeyOfApplicationContext) == null)
                    {
                        CallContext.SetData(KeyOfApplicationContext, new ApplicationContext());
                    }
                    return (ApplicationContext)CallContext.GetData(KeyOfApplicationContext);
                }

            }
            set
            {
                CallContext.SetData("__ApplicationContext", value);
            }
        }
        /// <summary>
        /// 用户账户Id
        /// </summary>
        public string UserID
        {
            get { return this.GetContextValue("__UserID"); }
            set { this["__UserID"] = value; }
        }
        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIP
        {
            get { return this.GetContextValue("__ClientIP"); }
            set { this["__ClientIP"] = value; }
        }
        /// <summary>
        /// web服务器IP
        /// </summary>
        public string WebIP
        {
            get { return this.GetContextValue("__WebIP"); }
            set { this["__WebIP"] = value; }
        }
        /// <summary>
        /// 应用服务器IP； 
        /// </summary>
        public string ServiceIP
        {
            get
            {
                return hc.Plat.Common.Global.NetTools.GetLocalMachineIP4();
            }
        }

        private string GetContextValue(string key)
        {
            if (this.ContainsKey(key))
            {
                return (string)this[key];
            }
            return string.Empty;
        }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName
        {
            get { return this.GetContextValue("__UserName"); }
            set { this["__UserName"] = value; }
        }
        /// <summary>
        /// 用户角色身份
        /// </summary>
        public string RoleType
        {
            get { return this.GetContextValue("__RoleType"); }
            set { this["__RoleType"] = value; }
        }
        /// <summary>
        /// 用户所属企业
        /// </summary>
        public string CompanyId
        {
            get { return this.GetContextValue("__CompanyId"); }
            set { this["__CompanyId"] = value; }
        }
        /// <summary>
        /// 用户企业名称
        /// </summary>
        public string CompanyName
        {
            get { return this.GetContextValue("__CompanyName"); }
            set { this["__CompanyName"] = value; }
        }
        /// <summary>
        /// 登录用户涉及单位Ids
        /// </summary>
        public string ProjectIds
        {
            get { return this.GetContextValue("__ProjectIds"); }
            set { this["__ProjectIds"] = value; }
        }
    }
}
