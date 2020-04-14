using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace hc.Plat.Common.Global
{
    /// <summary>
    /// aes加密
    /// </summary>
    public class APIAESTool
    {
        const string _AESKey = "hc*API@plat!^h&%";

        /// <summary>  
        /// AES加密  
        /// </summary>  
        /// <param name="str">待加密字符串</param>  
        /// <returns>加密后字符串</returns>  
        public static string AesEncrypt(string toEncrypt)
        {
            try
            {
                //分组加密算法  
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(toEncrypt);//得到需要加密的字节数组   
                //设置密钥及密钥向量  
                aes.Key = Encoding.UTF8.GetBytes(_AESKey);
                //aes.IV = Encoding.UTF8.GetBytes(key);  
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                byte[] cipherBytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        cipherBytes = ms.ToArray();//得到加密后的字节数组  
                        cs.Close();
                        ms.Close();
                    }
                }
                return Convert.ToBase64String(cipherBytes);
            }
            catch (Exception)
            {
                return "";
            }
            //return toEncrypt;
        }

        /// <summary>  
        /// AES解密  
        /// </summary>  
        /// <param name="str">待解密字符串</param>  
        /// <returns>解密后字符串</returns>  
        public static string AesDecrypt(string toDecrypt)
        {
            try
            {
                byte[] cipherText = Convert.FromBase64String(toDecrypt);
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                aes.Key = Encoding.UTF8.GetBytes(_AESKey);
                //aes.IV = Encoding.UTF8.GetBytes(key);  
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;
                byte[] decryptBytes = new byte[cipherText.Length];
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        cs.Read(decryptBytes, 0, decryptBytes.Length);
                        cs.Close();
                        ms.Close();
                    }
                }
                return Encoding.UTF8.GetString(decryptBytes).Replace("\0", "");   //将字符串后尾的'\0'去掉  
            }
            catch (Exception)
            {
                return "";
            }
            //return toDecrypt;
        }
    }
}
