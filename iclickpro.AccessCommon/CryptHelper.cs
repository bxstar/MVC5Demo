
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
    /// ���ݼ��ܽ��ܰ�����
    /// </summary>
    public  class CryptHelper
    {
         private static string defaultKeyStr = "JERRYHAN581321";

        /// <summary>
        /// ���췽��
        /// </summary>
        private CryptHelper()
        {
        }
        /// <summary>
        /// ʹ��ȱʡ�����ַ�������
        /// </summary>
        /// <param name="originalStr">����</param>
        /// <returns>����</returns>
        public static string Encrypt(string originalStr)
        {
            return Encrypt(originalStr, defaultKeyStr);
        }

        /// <summary>
        /// MD5����
        /// </summary>
        /// <param name="str">Ԫ�ַ�</param>
        /// <returns>�����ַ���</returns>
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
        /// ʹ��ȱʡ�������
        /// </summary>
        /// <param name="originalStr">����</param>
        /// <returns>����</returns>
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
        /// ʹ�ø�����Կ����
        /// </summary>
        /// <param name="original">����</param>
        /// <param name="key">��Կ</param>
        /// <returns>����</returns>
        public static string Encrypt(string originalStr, string keyStr)
        {
            byte[] buffBytes = Encoding.Default.GetBytes(originalStr);
            byte[] keyBytes = Encoding.Default.GetBytes(keyStr);
            return Convert.ToBase64String(Encrypt(buffBytes, keyBytes));
        }

        /// <summary>
        /// ���������
        /// </summary>
        /// <param name="keyStr"></param>
        /// <returns></returns>
        public static string EncryptPassword(string keyStr)
        {
            return Encrypt(keyStr);
        }

        /// <summary>
        /// ʹ��ȱʡ��Կ����,����ָ�����뷽ʽ����
        /// </summary>
        /// <param name="original">����</param>
        /// <param name="encoding">���뷽ʽ</param>
        /// <returns>����</returns>
        public static string Decrypt(string originalStr, string keyStr)
        {
            return Decrypt(originalStr, keyStr, Encoding.Default);
        }

        /// <summary>
        /// ʹ��ȱʡ��Կ����,����ָ�����뷽ʽ����
        /// </summary>
        /// <param name="original">����</param>
        /// <param name="encoding">���뷽ʽ</param>
        /// <returns>����</returns>
        private static string Decrypt(string encrypted, string keyStr, Encoding encoding)
        {
            byte[] buffBytes = Convert.FromBase64String(encrypted);
            byte[] keyBytes = Encoding.Default.GetBytes(keyStr);
            return encoding.GetString(Decrypt(buffBytes, keyBytes));
        }

        /// <summary>
        /// ����MDժҪ
        /// </summary>
        /// <param name="originalStr">����Դ</param>
        /// <returns>ժҪ</returns>
        public static byte[] MakeMD(byte[] originalStr)
        {
            MD5CryptoServiceProvider hashmd = new MD5CryptoServiceProvider();
            byte[] keyhash = hashmd.ComputeHash(originalStr);
            hashmd = null;
            return keyhash;
        }

        /// <summary>
        /// ʹ�ø�����Կ����
        /// </summary>
        /// <param name="original">����</param>
        /// <param name="key">��Կ</param>
        /// <returns>����</returns>
        public static byte[] Encrypt(byte[] originalStr, byte[] keyStr)
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = MakeMD(keyStr);
            des.Mode = CipherMode.ECB;

            return des.CreateEncryptor().TransformFinalBlock(originalStr, 0, originalStr.Length);
        }

        /// <summary>
        /// ʹ�ø�����Կ��������
        /// </summary>
        /// <param name="encrypted">����</param>
        /// <param name="key">��Կ</param>
        /// <returns>����</returns>
        public static byte[] Decrypt(byte[] encrypted, byte[] keyStr)
        {
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            des.Key = MakeMD(keyStr);
            des.Mode = CipherMode.ECB;

            return des.CreateDecryptor().TransformFinalBlock(encrypted, 0, encrypted.Length);
        }

        /// <summary>
        /// ʹ�ø�����Կ����
        /// </summary>
        /// <param name="original">ԭʼ����</param>
        /// <param name="key">��Կ</param>
        /// <returns>����</returns>
        public static byte[] Encrypt(byte[] originalStr)
        {
            byte[] keyStr = Encoding.Default.GetBytes(defaultKeyStr);
            return Encrypt(originalStr, keyStr);
        }

        /// <summary>
        /// ʹ��ȱʡ��Կ��������
        /// </summary>
        /// <param name="encrypted">����</param>
        /// <param name="key">��Կ</param>
        /// <returns>����</returns>
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