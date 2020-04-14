using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.epm.Common
{
    public class ConstString
    {

        /// <summary>
        /// 业主负责人角色ID
        /// </summary>
        public static readonly string OwnerDirector = ConfigurationManager.AppSettings["OwnerDirector"];
        /// <summary>
        /// 业主审核人角色Id
        /// </summary>
        public static readonly string OwnerAuditor = ConfigurationManager.AppSettings["OwnerAuditor"];

        /// <summary>
        /// 所有缓存key
        /// </summary>
        public const string CACHEKEYALL = "CacheKeyAll";
        /// <summary>
        /// 用户权限json对象session
        /// </summary>
        public const string RIGHTSSESSION = "RightsSession";
        /// <summary>
        /// 阿里云keyid
        /// </summary>
        public static readonly string KeyId = ConfigurationManager.AppSettings["AccessKeyID"];

        /// <summary>
        /// 阿里云KeySecret
        /// </summary>
        public static readonly string KeySecret = ConfigurationManager.AppSettings["AccessKeySecret"];

        /// <summary>
        /// 服务器证书存储位置
        /// </summary>
        public static readonly string ServerCerPath = ConfigurationManager.AppSettings["ServerCerPath"];

        /// <summary>
        /// 服务器证书密码
        /// </summary>
        public static readonly string ServerCerPwd = "1";

        /// <summary>
        /// cookie admin 保存用户名
        /// </summary>
        public const string COOKIEADMINNAME = "adminlogingname";
        /// <summary>
        /// cookie admin 保存密码
        /// </summary>
        public const string COOKIEADMINPWD = "adminlogingpwd";

        ///// <summary>
        ///// cookie 代理 保存用户名
        ///// </summary>
        //public const string COOKIEBANAME = "balogingname";
        ///// <summary>
        ///// cookie 代理 保存密码
        ///// </summary>
        //public const string COOKIEBAPWD = "balogingpwd";
        ///// <summary>
        ///// cookie 招标人 保存用户名
        ///// </summary>
        //public const string COOKIETENNAME = "tenlogingname";
        ///// <summary>
        ///// cookie 招标人 保存密码
        ///// </summary>
        //public const string COOKIETENPWD = "tenlogingpwd";
        ///// <summary>
        ///// cookie 投标人 保存用户名
        ///// </summary>
        //public const string COOKIEBIDDERNAME = "bidderloginname";
        ///// <summary>
        ///// cookie 投标人 保存密码
        ///// </summary>
        //public const string COOKIEBIDDERPWD = "bidderloginpwd";
        ///// <summary>
        ///// cookie 专家 保存用户名
        ///// </summary>
        //public const string COOKIEEXPNAME = "exploginname";
        ///// <summary>
        ///// cookie 专家 保存密码
        ///// </summary>
        //public const string COOKIEEXPPWD = "exploginpwd";

        /// <summary>
        /// 邮件或者短信验证码长度
        /// </summary>
        public const int RANDOMNUMCOUNT = 6;

        /// <summary>
        /// 短信紧急程度
        /// </summary>
        public const string SMSTYPE = "normal";

        /// <summary>
        /// 短信格式化数据类型
        /// </summary>
        public const string SMSFORMAT = "json";

        ///// <summary>
        ///// 交易平台标识码
        ///// </summary>
        //public const string PLATFORMCODE = "ABCDEF12345";

        ///// <summary>
        ///// 招标计划编码前缀
        ///// </summary>
        //public const string PLANCODEPREFIX = "ZBJH";

        ///// <summary>
        /////  开标大厅编号编码前缀
        ///// </summary>
        //public const string OPENROOMCODEPREFIX = "KBDT";

        ///// <summary>
        ///// 标书领购订单编码
        ///// </summary>
        //public const string ORDERNUMERPREFIX = "BSDD";

        ///// <summary>
        ///// 投标人编码前缀
        ///// </summary>
        //public const string BLACKBIDDERPREFIX = "TB";

        ///// <summary>
        ///// 合同编码前缀
        ///// </summary>
        //public const string CONTRACTPREFIX = "HT";
        ///// <summary>
        ///// 开标大厅用户SignalR通讯cookie
        ///// </summary>
        //public const string SIGNALROPUSERCOOKIE = "OPHallUserCookie";
        ///// <summary>
        ///// 评标大厅用户SignalR通讯cookie
        ///// </summary>
        //public const string SIGNALREPUSERCOOKIE = "EPHallUserCookie";


        /// <summary>
        /// cookie 当前项目Id
        /// </summary>
        public const string COOKIEPROJECTID = "ProjecctId";
        /// <summary>
        /// cookie 当前项目Name
        /// </summary>
        public const string COOKIEPROJECTNAME = "ProjecctName";


        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <param name="numCount">随机数数量</param>
        /// <returns></returns>
        public static string CreateRandomNum(int numCount)
        {
            //string allChar = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,O,P,Q,R,S,T,U,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j,k,m,n,o,p,q,s,t,u,w,x,y,z";
            string allChar = "0,1,2,3,4,5,6,7,8,9";
            string[] allCharArray = allChar.Split(',');//拆分成数组
            string randomNum = "";
            int temp = -1;                             //记录上次随机数的数值，尽量避免产生几个相同的随机数
            Random rand = new Random();
            for (int i = 0; i < numCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(9);
                if (temp == t)
                {
                    return CreateRandomNum(numCount);
                }
                temp = t;
                randomNum += allCharArray[t];
            }
            return randomNum;
        }

        public static long TzMaxFileSize()
        {
            string tzmaxfilesize = ConfigurationManager.AppSettings["TZMaxFileSize"];
            if (string.IsNullOrWhiteSpace(tzmaxfilesize))
            {
                tzmaxfilesize = "1024000";
            }

            long temp = 0;
            if (long.TryParse(tzmaxfilesize, out temp))
            {
                return temp;
            }
            else
            {
                throw new Exception("未设置上传至投资系统的附件的最大值！");
            }
        }
    }
}
