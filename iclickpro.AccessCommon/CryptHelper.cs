
using System.Security;
using System.Security.Cryptography;
using System.IO;
using System;
using System.Text;
using System.Collections;
using System.Globalization;

namespace iclickpro.AccessCommon
{
    /// <summary>
    /// 数据加密解密帮助类
    /// </summary>
    public  class CryptHelper
    {
         private static string defaultKeyStr = "JERRYHAN581321";

        /// <summary>
        /// 构造方法
        /// </summary>
        private CryptHelper()
        {
        }
        /// <summary>
        /// 使用缺省密码字符串加密
        /// </summary>
        /// <param name="originalStr">明文</param>
        /// <returns>密文</returns>
        public static string Encrypt(string originalStr)
        {
            return Encrypt(originalStr, defaultKeyStr);
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str">元字符</param>
        /// <returns>加密字符串</returns>
        public static string MD5(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] data = md5.ComputeHash(UTF8Encoding.Default.GetBytes(str));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("X2"));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// 使用缺省密码解密
        /// </summary>
        /// <param name="originalStr">密文</param>
        /// <returns>明文</returns>
        public static string Decrypt(string originalStr)
        {
            try
            {
                return Decrypt(originalStr, defaultKeyStr, Encoding.Default);
            }
            catch
            {
                return originalStr;
            }
        }

        /// <summary>
        /// 使用给定密钥解密
        /// </summary>
        /// <param name="original">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>明文</returns>
        public static string Encrypt(string originalStr, string keyStr)
        {
            byte[] buffBytes = Encoding.Default.GetBytes(originalStr);
            byte[] keyBytes = Encoding.Default.GetBytes(keyStr);
            return Convert.ToBase64String(Encrypt(buffBytes, keyBytes));
        }

        /// <summary>
        /// 给密码加密
        /// </summary>
        /// <param name="keyStr"></param>
        /// <returns></returns>
        public static string EncryptPassword(string keyStr)
        {
            return Encrypt(keyStr);
        }

        /// <summary>
        /// 使用缺省密钥解密,返回指定编码方式明文
        /// </summary>
        /// <param name="original">密文</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>明文</returns>
        public static string Decrypt(string originalStr, string keyStr)
        {
            return Decrypt(originalStr, keyStr, Encoding.Default);
        }

        /// <summary>
        /// 使用缺省密钥解密,返回指定编码方式明文
        /// </summary>
        /// <param name="original">密文</param>
        /// <param name="encoding">编码方式</param>
        /// <returns>明文</returns>
        private static string Decrypt(string encrypted, string keyStr, Encoding encoding)
        {
            byte[] buffBytes = Convert.FromBase64String(encrypted);
            byte[] keyBytes = Encoding.Default.GetBytes(keyStr);
            return encoding.GetString(Decrypt(buffBytes, keyBytes));
        }

        /// <summary>
        /// 生成MD摘要
        /// </summary>
        /// <param name="originalStr">数据源</param>
        /// <returns>摘要</returns>
        public static byte[] MakeMD(byte[] originalStr)
        {
            MD5CryptoServiceProvider hashmd = new MD5CryptoServiceProvider();
            byte[] keyhash = hashmd.ComputeHash(originalStr);
            hashmd = null;
            return keyhash;
        }

        /// <summary>
        /// 使用给定密钥加密
        /// </summary>
        /// <param name="original">明文</param>
        /// <param name="key">密钥</param>
        /// <returns>密文</returns>
        public static byte[] Encrypt(byte[] originalStr, byte[] keyStr)
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = MakeMD(keyStr);
            des.Mode = CipherMode.ECB;

            return des.CreateEncryptor().TransformFinalBlock(originalStr, 0, originalStr.Length);
        }

        /// <summary>
        /// 使用给定密钥解密数据
        /// </summary>
        /// <param name="encrypted">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>明文</returns>
        public static byte[] Decrypt(byte[] encrypted, byte[] keyStr)
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = MakeMD(keyStr);
            des.Mode = CipherMode.ECB;

            return des.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length);
        }

        /// <summary>
        /// 使用给定密钥加密
        /// </summary>
        /// <param name="original">原始数据</param>
        /// <param name="key">密钥</param>
        /// <returns>密文</returns>
        public static byte[] Encrypt(byte[] originalStr)
        {
            byte[] keyStr = Encoding.Default.GetBytes(defaultKeyStr);
            return Encrypt(originalStr, keyStr);
        }

        /// <summary>
        /// 使用缺省密钥解密数据
        /// </summary>
        /// <param name="encrypted">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>明文</returns>
        public static byte[] Decrypt(byte[] encrypted)
        {
            byte[] keyStr = Encoding.Default.GetBytes(defaultKeyStr);
            return Decrypt(encrypted, keyStr);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="base64string"></param>
        /// <returns></returns>
        public static string GetTopParamer(string base64string)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(base64string);
                string decode = Encoding.GetEncoding("GBK").GetString(bytes);
                return decode;
            }
            catch (Exception)
            {
                throw;
            }
        }
    } 
}