using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hc.Plat.Common.Global
{
    public class GenUniqueCode
    {
        public static long initialSeed = 123456789012;
        public static string weightCoefficient = "35196072";
        public GenUniqueCode()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }
        public static string GenCode(string LastFouth, long uniqueID)
        {
            string mstr = "";
            LastFouth = "0000" + LastFouth;
            LastFouth = LastFouth.Substring(LastFouth.Length - 4);
            long mTmpID = initialSeed + uniqueID;
            string TmpStr = mTmpID.ToString();
            TmpStr = "000000000000" + TmpStr;
            TmpStr = TmpStr.Substring(TmpStr.Length - 12);
            TmpStr = LastFouth + TmpStr;
            string mCheckCodeOne = "";
            string mCheckCodeTow = "";
            string mfirstStr = "";
            string msecondStr = "";
            int i;
            for (i = 0; i < 8; i++)
            {
                mfirstStr += TmpStr.Substring(i * 2, 1);
                msecondStr += TmpStr.Substring(i * 2 + 1, 1);
            }
            mCheckCodeOne = GetCheckCode(mfirstStr);
            mCheckCodeTow = GetCheckCode(msecondStr);
            mfirstStr = strTransformation(mfirstStr, mCheckCodeOne);
            msecondStr = strTransformation(msecondStr, mCheckCodeTow);
            mstr = "";
            for (i = 0; i < 8; i++)
            {
                mstr += mfirstStr.Substring(i, 1) + msecondStr.Substring(i, 1);
            }
            mstr += mCheckCodeOne + mCheckCodeTow;
            return mstr;
        }
        private static string GetCheckCode(string vStr)
        {
            string mcheckCode = "";
            int i;
            int tmpValue = 0;
            for (i = 0; i < 8; i++)
            {
                tmpValue += Convert.ToInt32(weightCoefficient.Substring(i, 1)) * Convert.ToInt32(vStr.Substring(i, 1));

            }
            tmpValue = tmpValue % 10;
            mcheckCode = tmpValue.ToString();
            return mcheckCode;
        }
        private static string strTransformation(string vStr, string vCheckCode)
        {
            string mStr = "";
            int i;
            int tmpValue = 0;
            for (i = 0; i < 8; i++)
            {
                tmpValue = Convert.ToInt32(vCheckCode) + Convert.ToInt32(vStr.Substring(i, 1));
                tmpValue = tmpValue % 10;
                mStr += tmpValue.ToString();
            }
            return mStr;
        }
    }
}