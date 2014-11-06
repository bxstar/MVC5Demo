using System;
using System.Configuration;

namespace iclickpro.AccessCommon
{
    public class Config
    {
        /// <summary>
        /// 从配置文件中取得信息
        /// </summary>
        /// <param name="strKey">节点key值</param>
        /// <returns>返回节点配置信息</returns>
        public static string GetStrConfig(string strKey)
        {
            return ConfigurationManager.AppSettings[strKey].ToString();
        }
    }

}
