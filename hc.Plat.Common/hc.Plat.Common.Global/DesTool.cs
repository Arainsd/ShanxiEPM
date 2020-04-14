using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace hc.Plat.Common.Global
{

    public class DesTool
    {
        public static string _DESKey = "hcsinrip";

        public static string DesEncrypt(string toEncrypt)
        {
            //定义DES加密服务提供类
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            // des.Mode=CipherMode.ECB;
            //加密字符串转换为byte数组
            byte[] inputByte = System.Text.ASCIIEncoding.UTF8.GetBytes(toEncrypt);
            //加密密匙转化为byte数组
            byte[] key = Encoding.ASCII.GetBytes(_DESKey); //DES密钥(必须8字节)
            byte[] iv = key;// new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
            des.Key = key;
            des.IV = iv;
            des.Mode = CipherMode.CBC;
            //创建其支持存储区为内存的流
            MemoryStream ms = new MemoryStream();
            //定义将数据流链接到加密转换的流
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(des.Key, des.IV), CryptoStreamMode.Write);
            //  CryptoStream cs = new CryptoStream(ms, des.IV, CryptoStreamMode.Write);

            cs.Write(inputByte, 0, inputByte.Length);
            cs.FlushFinalBlock();
            cs.Close();
            byte[] bytesCipher = ms.ToArray();

            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                //向可变字符串追加转换成十六进制数字符串的加密后byte数组。
                ret.AppendFormat("{0:X2}", b);
            }
            return ret.ToString();

        }

        public static string DesDecrypt(string toDecrypt)
        {
            //定义DES加密解密服务提供类
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            //加密密匙转化为byte数组
            byte[] key = Encoding.ASCII.GetBytes(_DESKey);
            des.Key = key;
            des.IV = key;
            //将被解密的字符串每两个字符以十六进制解析为byte类型，组成byte数组
            int length = (toDecrypt.Length / 2);
            byte[] inputByte = new byte[length];
            for (int index = 0; index < length; index++)
            {
                string substring = toDecrypt.Substring(index * 2, 2);
                inputByte[index] = Convert.ToByte(substring, 16);
            }
            //创建其支持存储区为内存的流
            MemoryStream ms = new MemoryStream();
            //定义将数据流链接到加密转换的流
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByte, 0, inputByte.Length);
            cs.FlushFinalBlock();

            return ASCIIEncoding.UTF8.GetString((ms.ToArray()));
        }

        public static string LoadCertUserPass(string FilePath, out string user, out string pass)
        {
            user = "";
            pass = "";
            string FileName = FilePath + "SinriCert.hsc";
            if (!File.Exists(FileName))
            {
                return "约定目录下未找到验证文件SinriCert.hsc！";
            }

            FileStream fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            string text = "";
            try
            {
                StreamReader m_streamReader = new StreamReader(fs);
                try
                {
                    text = m_streamReader.ReadLine();
                }
                finally
                {
                    m_streamReader.Close();
                }
            }
            finally
            {
                fs.Close();
            }
            try
            {
                text = DesTool.DesDecrypt(text);
                string[] userpass = text.Split('@');
                if (userpass.Length == 0)
                {
                    return "验证文件长度不合法！";
                }
                user = userpass[0];
                user = DesTool.DesDecrypt(user);
                pass = userpass[1];
                pass = DesTool.DesDecrypt(pass);
            }
            catch
            {
                return "验证用户名和密码时，未通过检测";
            }
            return "";
        }
    }
}
