using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace hc.Plat.Common.Global
{
    public class CommonFunction
    {
        static public bool IsEmpty(object value)
        {
            if (value == null)
            {
                return true;
            }
            if (value is string)
            {
                if (value.ToString() == "")
                {
                    return true;
                }
            }
            return false;
        }

        static public bool IsEmail(string value)
        {
            if (value == null)
            {
                return false;
            }
            Regex r = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");
            if (r.IsMatch(value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static public bool IsTel(string value)
        {
            if (value == null)
            {
                return false;
            }
            Regex r = new Regex("^(\\d{3,4}-)?\\d{6,8}$");
            if (r.IsMatch(value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 截取指定长度字符串
        /// </summary>
        /// <param name="inputString">要处理的字符串</param>
        /// <param name="length">指定长度</param>
        /// <param name="isShowFix">是否显示省略号</param>
        /// <returns>返回处理后的字符串</returns>
        public static string GetSubstringStr(string inputString, int length, bool isShowFix = false)
        {
            if (string.IsNullOrWhiteSpace(inputString))
                return inputString;
            if (length % 2 == 1)
            {
                isShowFix = true;
                length--;
            }
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            string tempString = "";
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                    tempLen += 2;
                else
                    tempLen += 1;

                try
                {
                    tempString += inputString.Substring(i, 1);
                }
                catch
                {
                    break;
                }

                if (tempLen >= length)
                    break;
            }

            byte[] mybyte = Encoding.Default.GetBytes(inputString);
            if (isShowFix && mybyte.Length > length)
                tempString += "…";
            return tempString;
        }

        /// <summary>
        /// 隐藏字符串中的重要信息
        /// </summary>
        /// <param name="inputString">需要处理的字符串</param>
        /// <param name="startIndex">开始位置</param>
        /// <param name="length">隐藏长度</param>
        /// <param name="isReversion">否是从字符串末尾开始隐藏</param>
        /// <returns></returns>
        public static string ReplaceHideInfoString(string inputString, int startIndex, int length, bool isReversion = false)
        {
            if (string.IsNullOrWhiteSpace(inputString))
                return "";
            int stringLength = inputString.Length;
            if (stringLength < startIndex)
            {
                startIndex = 0;
            }
            if (startIndex + length > stringLength)
            {
                length = stringLength;
            }


            Func<string, string > getReversionString = delegate(string str)
            {
                if (string.IsNullOrWhiteSpace(str))
                    return "";
                StringBuilder sb = new StringBuilder();
                for (int i = str.Length - 1; i >= 0; i--)
                {
                    sb.Append(str[i]);
                }
                return sb.ToString();
            };


            if (isReversion)
            {
                inputString = getReversionString(inputString);
            }

            char[] chars = inputString.ToCharArray();
            for (int i = startIndex; i < startIndex + length; i++)
            {
                chars[i] = '*';
            }
            inputString = new string(chars);

            if (isReversion)
            {
                inputString = getReversionString(inputString);
            }

            return inputString;
        }
    }


}
