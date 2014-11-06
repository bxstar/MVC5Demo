using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Xml;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Data.SqlClient;

namespace iclickpro.AccessCommon
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlNameAndParamer
    {
        /// <summary>
        /// SQL文件
        /// </summary>
        private const string ConfigFile = @"SqlMap.xml";

        /// <summary>
        /// 取得Sql
        /// </summary>
        /// <param name="StrName">SQl文の名称</param>
        /// <returns></returns>
        public static string GetSQLValue(string strSqlName)
        {
            var strSqlValue = String.Empty;
            var configFile = Uri.UnescapeDataString(
                Path.GetDirectoryName((new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath) +
                Path.DirectorySeparatorChar + ConfigFile);

            XmlTextReader xmlTextReader = new XmlTextReader(configFile);
            while (!xmlTextReader.EOF)
            {
                if (xmlTextReader.MoveToContent() == XmlNodeType.Element && xmlTextReader.Name == "Name")
                {
                    if (xmlTextReader.ReadElementString() == strSqlName)
                    {
                        strSqlValue = xmlTextReader.ReadElementString().Replace("\r\n", " ");
                        return strSqlValue.Replace('?', '@');
                    }
                }
                else
                {
                    xmlTextReader.Read();
                }
            }
            return strSqlValue.Replace('?','@');
        }

        /// <summary>
        /// 将实体类转换为数据库参数
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns>数据库参数</returns>
        public static SqlParameter[] ConvertSqlParameter(IDictionary<string, object> paramers)
        {
            if (paramers != null)
            {
                var param = new SqlParameter[paramers.Count];
                int i = 0;
                foreach (var property in paramers)
                {
                    param[i] = new SqlParameter("@" + property.Key, property.Value);
                    i++;
                }
                return param;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 将传递的参数转换为数据库参数
        /// </summary>
        /// <param name="paramers">传入的参数</param>
        /// <returns></returns>
        public static List<SqlParameter[]> ConvertSqlParameters(List<IDictionary<string, object>> paramers)
        {
            var SqlParameters = new List<SqlParameter[]>();
            foreach (var param in paramers)
            {
                SqlParameters.Add(ConvertSqlParameter(param));
            }
            return SqlParameters;
        }
    }
}

