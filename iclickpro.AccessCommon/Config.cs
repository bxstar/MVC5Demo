using System;
using System.Configuration;

namespace iclickpro.AccessCommon
{
    public class Config
    {
        /// <summary>
        /// �������ļ���ȡ����Ϣ
        /// </summary>
        /// <param name="strKey">�ڵ�keyֵ</param>
        /// <returns>���ؽڵ�������Ϣ</returns>
        public static string GetStrConfig(string strKey)
        {
            return ConfigurationManager.AppSettings[strKey].ToString();
        }
    }

}
