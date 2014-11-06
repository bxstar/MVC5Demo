using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using NetServ.Net.Json;
using System.IO.Compression;

namespace iclickpro.AccessCommon
{
    /// <summary>
    /// 基本共通函数类
    /// </summary>
    public class CommonFunction
    {
        /// <summary>
        /// 取得appSetting的值
        /// </summary>
        /// <param name="strKey"></param>
        /// <returns></returns>
        public static string GetAppSetting(string strKey)
        {
            return System.Configuration.ConfigurationManager.AppSettings[strKey];
        }

        /// <summary>
        /// 取得解码后的用户名
        /// </summary>
        /// <param name="userName">编码后的用户名</param>
        /// <returns></returns>
        public static string GetUserName(string userName)
        {
            return userName;
        }

        /// <summary>
        /// 取得登录的用户名
        /// </summary>
        /// <param name="key"></param>
        /// <param name="topParam"></param>
        /// <returns></returns>
        public static string GetUserName(string key, string topParam)
        {
            Hashtable param = GetParam(topParam);
            if (param != null)
            {
                return param[key].ToString();
            }
            return String.Empty;
        }
        /// <summary>
        /// 取得参数
        /// </summary>
        /// <param name="topParam"></param>
        /// <returns></returns>
        private static Hashtable GetParam(string topParam)
        {
            Hashtable param = null;
            if (topParam != null)
            {
                param = new Hashtable();
                string[] strParam = topParam.Split('&');
                if (strParam.Length > 0)
                {
                    foreach (string t in strParam)
                    {
                        string[] keys = t.Split('=');
                        param.Add(keys[0], keys[1]);
                    }
                }

            }
            return param;
        }
        /// <summary>
        /// 计算有多少页
        /// 用于列表分页
        /// </summary>
        /// <param name="totalNum">总的行数</param>
        /// <param name="pageSize">每页显示的行数</param>
        /// <returns>多少页数</returns>
        public static int getPageCount(int totalNum, int pageSize)
        {
            // 每页显示的条数 = 0 的情况
            if (pageSize == 0)
            {
                return 0;
            }

            int nRemainder = totalNum % pageSize;
            if (nRemainder > 0)
            {
                return totalNum / pageSize + 1;
            }
            else
            {
                return totalNum / pageSize;
            }
        }
        /// <summary>
        /// 转成整型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int JsonObjectToInt(IJsonType obj)
        {
            if (obj is JsonNull || obj.ToString() == "") return 0;

            return Convert.ToInt32(obj.ToString().Split('.')[0]);
        }

        /// <summary>
        /// 转成Decimal
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Decimal JsonObjectToDecimal(IJsonType obj)
        {
            if (obj is JsonNull || obj.ToString() == "") return 0;

            return Convert.ToDecimal(obj.ToString());
        }

        /// <summary>
        /// 转成字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string JsonObjectToString(IJsonType obj)
        {
            if (obj == null) return string.Empty;

            return obj.ToString();
        }

        /// <summary>
        /// 分组将列表元素拼接成由分隔符分隔的字符串
        /// </summary>
        /// <param name="lst">列表元素</param>
        /// <param name="splitter">分隔符</param>
        /// <param name="groupSize">分组大小</param>
        /// <returns></returns>
        public static List<string> SplitterGroupList(List<string> lst, char splitter, int groupSize)
        {
            var lstGroupItem = new List<string>();

            if (lst == null)
            {
                lstGroupItem.Add(null);
            }
            else
            {
                for (int i = 0; i < lst.Count; i += groupSize)
                {
                    string strItems = SplitterList(lst.GetRange(i, (lst.Count - i < groupSize) ? lst.Count - i : groupSize), splitter);
                    lstGroupItem.Add(strItems);
                }
            }

            return lstGroupItem;
        }


        /// <summary>
        /// 将列表元素拼接成由分隔符分隔的字符串
        /// </summary>
        /// <param name="lst">列表元素</param>
        /// <param name="splitter">分隔符</param>
        /// <returns></returns>
        public static string SplitterList(List<string> lst, char splitter)
        {
            string strItems = string.Empty;
            if (lst == null) return strItems;

            foreach (string s in lst)
            {
                if (!string.IsNullOrEmpty(strItems))
                {
                    strItems += splitter;
                }
                strItems += s;
            }
            return strItems;
        }

        /// <summary>
        /// 输入一个日期，取得是星期几
        /// </summary>
        /// <param name="date">输入日期</param>
        /// <returns></returns>
        public static int GetWeekDay(string date)
        {
            int result = 0;
            try
            {
                var weekday = Convert.ToDateTime(date).DayOfWeek.ToString().ToLower(); 
                switch (weekday)
                {
                    case "monday":
                        {
                            result = 1;
                            break;
                        }
                    case "tuesday":
                        {
                            result = 2;
                            break;
                        }
                    case "wednesday":
                        {
                            result = 3;
                            break;
                        }
                    case "thursday":
                        {
                            result = 4;
                            break;
                        }
                    case "friday":
                        {
                            result = 5;
                            break;
                        }
                    case "saturday":
                        {
                            result = 6;
                            break;
                        }
                    default:
                        {
                            result = 7;
                            break;
                        }
                }
            }
            catch
            {
                result = 0;
            }
            return result;
        }

        /// <summary>
        /// 返回两个时间之间的毫秒差
        /// </summary>
        public static double DateDiff(DateTime currentTime, DateTime finishDate)
        {
            TimeSpan ts1 = new TimeSpan(currentTime.Ticks);
            TimeSpan ts2 = new TimeSpan(finishDate.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            return ts.TotalMilliseconds;
        }

        /// <summary>
        /// 返回两个日期之间的天数
        /// </summary>
        public static int GetDays(string startDate, string endDate)
        {
            DateTime dtStartDate = Convert.ToDateTime(startDate);
            TimeSpan ts = Convert.ToDateTime(endDate).Subtract(dtStartDate);
            return ts.Days;
        }

        /// <summary>
        /// 根据开始日期和结束日期，取得有多少周
        /// </summary>
        public static int GetWeekNum(string startDate,string endDate)
        {
            int result = 0;
            try 
            {
                var days = GetDays(startDate, endDate);
                var start = GetWeekDay(startDate);
                if (start != 1)
                {
                    result = getPageCount(days - 8 + start, 7) + 1;
                }
                else
                {
                    result = getPageCount(days, 7);
                }
            }
            catch
            {
                result = 0;
            }
            return result;
        }

        /// <summary>
        /// 取得周的开始日期和结束日期
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetStartAndEndDay(string startDate, string endDate,int weekIndex)
        {
            // 定义返回值
            Dictionary<string, string> result = null;
            // 返回有多少周
            var weekNum = GetWeekNum(startDate, endDate);
            if (weekIndex > weekNum)
            {
                return result;
            }
            result = new Dictionary<string, string>();
            var start = GetWeekDay(startDate);
            var end = GetWeekDay(endDate);
            if (weekIndex == 1)
            {
                result.Add("StartDay", Convert.ToDateTime(startDate).ToString("yyyy/MM/dd"));
                result.Add("EndDay", Convert.ToDateTime(startDate).AddDays(7 - start).ToString("yyyy/MM/dd"));
            }
            else if (1 < weekIndex && weekIndex < weekNum)
            {
                var days = (weekIndex - 1) * 7 + 8 - start;
                result.Add("StartDay", Convert.ToDateTime(startDate).AddDays(days).ToString("yyyy/MM/dd"));
                result.Add("EndDay", Convert.ToDateTime(startDate).AddDays(days + 6).ToString("yyyy/MM/dd"));
            }
            else
            {
                result.Add("StartDay", Convert.ToDateTime(endDate).AddDays(1 - end).ToString("yyyy/MM/dd"));
                result.Add("EndDay", Convert.ToDateTime(endDate).ToString("yyyy/MM/dd"));
            }
            return result;
        }
    
        /// <summary>
        /// 转换为Base64字符串
        /// </summary>
        /// <param name="strParam"></param>
        /// <returns></returns>
        public static string ToBase64String(string strParam)
        {
            // 将字符串转换为Byte数组
            byte[] bytes = Encoding.Default.GetBytes(strParam);
            // 将byte数组转换为Base64
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 将base64转换为元字符串
        /// </summary>
        /// <param name="strParam"></param>
        /// <returns></returns>
        public static string FromBase64String(string strParam)
        {
            Byte[] bytes = Convert.FromBase64String(strParam);
            string strResult = Encoding.Default.GetString(bytes);
            return strResult;
        }

        /// <summary>
        /// MD5 加密算法
        /// </summary>
        public static string Md5Encrypt(string s)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            UTF8Encoding en = new UTF8Encoding();
            byte[] md5Bt = en.GetBytes(s);
            byte[] cryBt = md5.ComputeHash(md5Bt);
            return BitConverter.ToString(cryBt).Replace("-", "");
        }

        #region 验证邮箱验证邮箱
        /**/
        /// <summary>
        /// 验证邮箱
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsEmail(string source)
        {
            if (source != null)
            {
                return Regex.IsMatch(source, @"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$", RegexOptions.IgnoreCase);
            }
            return false;
        }
        /// <summary>
        /// 验证邮箱
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool HasEmail(string source)
        {
            return Regex.IsMatch(source, @"[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})", RegexOptions.IgnoreCase);
        }
        #endregion

        /// <summary>
        /// 验证是否为整数
        /// </summary>
        /// <param name="source"></param>
        /// <returns>true or false</returns>
        public static bool IsInt(string source)
        {
            if (source != null) 
            {
                return Regex.IsMatch(source, @"[0-9]+");
            }
            return false;
        }

        /// <summary>
        /// 从错误信息中解析得到需要延迟多长时间，主要用于处理 This ban will last for ** more seconds
        /// </summary>
        /// <param name="message">错误信息,类似:This ban will last for ** more seconds</param>
        /// <returns>延迟毫秒数</returns>
        public static int GetTimeFromErrorMessage(object message)
        {
            int time = 0;
            string str = (message ?? "").ToString().Trim().ToLower();
            if (str.Contains("this ban will last for"))
            {
                str = str.Replace("this ban will last for ", "").Replace(" more seconds", "");
                try
                {
                    time = Convert.ToInt32(str) * 1000 + 500; //取到延迟时间+500毫秒
                }
                catch
                {
                    time = 1000;//如果出异常则延迟1秒
                }
            }
            return time;
        }

        /// <summary>
        /// 检查是不是权限问题需要重新登录授权了
        /// </summary>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public static bool CheckIsReachSecurityLevel(object ErrMsg)
        {
            string ermsg = (ErrMsg ?? "").ToString().Trim().ToLower();
            if (!string.IsNullOrEmpty(ermsg) && ermsg.Contains("security authorize invalid"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waitingMillisecond"></param>
        /// <returns></returns>
        public static bool CheckReachTheLimit(int waitingMillisecond)
        {
            if (waitingMillisecond > 3000000)
            {//说明超过了限制时间，判断一下是否要等到第二天 
                DateTime currentTime = DateTime.Now;
                DateTime nextDay = currentTime.Date.AddDays(1);
                if (waitingMillisecond > Convert.ToInt32(nextDay.Subtract(currentTime).TotalMilliseconds))
                {//等待时间已经超过当前时间到明天的总的毫秒数减去偏移值的时候，说明已经超限了
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否包含特殊字符
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool IsContainCharacter(string word)
        {
            string strCharacter = @"~`!@#$%^&*()_+={}[]:;'<>?/\|~·！@#￥%……&*（）——+={}|、】【‘；：《》？、。\";
            char[] characters = strCharacter.ToCharArray();
            foreach (char character in characters)
            {
                if (word.Contains(character) || word.Contains('"'))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 转化DataTable为Json字符串
        /// </summary>
        public static string CreateJson(DataTable dt)
        {
            StringBuilder JsonString = new StringBuilder();
            if (null != dt && 0 < dt.Rows.Count)
            {
                string[] ColumnName = new String[dt.Columns.Count];
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    ColumnName[j] = "\"" + dt.Columns[j].ColumnName.ToString().Replace("\"", "\\\"") + "\":\"";
                }
                JsonString.Append("[");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JsonString.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        JsonString.Append(ColumnName[j]);
                        JsonString.Append(dt.Rows[i][j].ToString().Replace("\"", "\\\""));
                        JsonString.Append("\",");
                    }
                    JsonString.Remove(JsonString.Length - 1, 1);
                    JsonString.Append("},");
                }
                JsonString.Remove(JsonString.Length - 1, 1);
                JsonString.Append("]");
                return JsonString.ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 转化DataTable为List，Dictionary后可以序列化Json
        /// </summary>
        public static List<Dictionary<string,object>> CreateListDictionary(DataTable dt)
        {
            List<Dictionary<string, object>> lstDic = new List<Dictionary<string, object>>();
            StringBuilder JsonString = new StringBuilder();
            if (null != dt && 0 < dt.Rows.Count)
            {
                string[] arrColumnName = new String[dt.Columns.Count];
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    arrColumnName[j] = dt.Columns[j].ColumnName.ToString();
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    foreach (string colName in arrColumnName)
                    {
                        dic.Add(colName, dt.Rows[i][colName]);
                    }
                    lstDic.Add(dic);
                }
            }

            return lstDic;
        }

        public static List<List<T>> SplitLst<T>(List<T> lst, int count)
        {
            List<List<T>> groupLstWord = new List<List<T>>();
            List<T> lstWord = new List<T>();
            for (int i = 0; i < lst.Count; i++)
            {
                if (i % count == 0)
                {
                    if (lstWord.Count != 0)
                    {
                        groupLstWord.Add(lstWord);
                    }
                    lstWord = new List<T>();
                    lstWord.Add(lst[i]);
                }
                else
                {
                    lstWord.Add(lst[i]);
                }
            }
            if (lst.Count != 0)
            {
                groupLstWord.Add(lstWord);
            }
            return groupLstWord;
        }

        /// <summary>
        /// 获取最接近的数
        /// </summary>
        public static int GetNearestNum(List<int> lstNum, int targetNum)
        {
            List<int> buffer = new List<int>();    // 存放找到的数
            List<int> tmp = new List<int>();       // 临时缓存
            int min = 0;
            int dis = 0;

            min = Math.Abs(targetNum - lstNum[0]);
            foreach (int num in lstNum)
            {
                dis = Math.Abs(targetNum - num);
                if (dis < min)
                {
                    min = dis;
                    tmp.Clear();
                    tmp.Add((int)num);
                }
                else if (dis == min)
                {
                    tmp.Add((int)num);
                }
            }
            return tmp[0];
        }

        /// <summary>
        /// 压缩字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Compress(string input)
        {
            byte[] inputBytes = Encoding.Default.GetBytes(input);
            byte[] result = Compress(inputBytes);
            return Convert.ToBase64String(result);
        }
        /// <summary>
        /// 解压缩字符串
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Decompress(string input)
        {
            byte[] inputBytes = Convert.FromBase64String(input);
            byte[] depressBytes = Decompress(inputBytes);
            return Encoding.Default.GetString(depressBytes);
        }

        /// <summary>
        /// 压缩字节数组
        /// </summary>
        /// <param name="str"></param>
        public static byte[] Compress(byte[] inputBytes)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                using (GZipStream zipStream = new GZipStream(outStream, CompressionMode.Compress, true))
                {
                    zipStream.Write(inputBytes, 0, inputBytes.Length);
                    zipStream.Close(); //很重要，必须关闭，否则无法正确解压
                    return outStream.ToArray();
                }
            }
        }

        /// <summary>
        /// 解压缩字节数组
        /// </summary>
        /// <param name="str"></param>
        public static byte[] Decompress(byte[] inputBytes)
        {

            using (MemoryStream inputStream = new MemoryStream(inputBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    using (GZipStream zipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                    {
                        zipStream.CopyTo(outStream);
                        zipStream.Close();
                        return outStream.ToArray();
                    }
                }

            }
        }

        /// <summary>
        /// 获取对象的属性值
        /// </summary>
        public static object GetPropertyValue(object obj, string property)
        {
            System.Reflection.PropertyInfo propertyInfo = obj.GetType().GetProperty(property);
            return propertyInfo.GetValue(obj, null);
        }
    }
}