using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.IO;
using System.Drawing;

namespace iclickpro.AccessCommon
{
    public class Strings
    {
        /// <summary>
        /// 验证网址是否是标准的网址
        /// </summary>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public static bool CheckStandardURL(string strUrl)
        {
            var checkUrl = new Regex(@"^(http://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?)|((https|http|ftp|rtsp|mms)?://)?(([0-9a-zA-Z_!~*'().&=+$%-]+: )?[0-9a-zA-Z_!~*'().&=+$%-]+@)?(([0-9]{1,3}\.){3}[0-9]{1,3}|([0-9a-zA-Z_!~*'()-]+\.)*([0-9a-zA-Z][0-9a-zA-Z-]{0,61})?[0-9a-zA-Z]\.[a-zA-Z]{2,6})(:[0-9]{1,4})?((/?)|(/[0-9a-zA-Z_!~*'().;?:@&=+$,%#-]+)+/?)$");
            Match url = checkUrl.Match(strUrl);
            if (!url.Success)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 统计中文操作系统下的字节数
        /// </summary>
        public static int CheckByteCount(string eword)
        {
            byte[] bytecount = Encoding.Default.GetBytes(eword);
            return bytecount.Length;
        }

        /// <summary>
        /// 将图片或flash转换成的二进制进行Hash然后MD5返回图片或flash的唯一标识
        /// </summary>
        /// <param name="myData"></param>
        /// <returns></returns>
        public static string MD5FromImage(byte[] myData)
        {
            var sMd5Code = new StringBuilder();
            HashAlgorithm algorithm = MD5.Create();
            byte[] hashbyte = algorithm.ComputeHash(myData, 0, myData.Length - 1);
            foreach (byte t in hashbyte)
            {
                sMd5Code.Append(t);
            }
            string result = Md5Encrypt(sMd5Code.ToString());
            return result;
        }

        /// <summary>
        /// MD5 加密算法
        /// </summary>
        public static string Md5Encrypt(string s)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            var en = new UTF8Encoding();
            byte[] md5Bt = en.GetBytes(s);
            byte[] cryBt = md5.ComputeHash(md5Bt);
            return BitConverter.ToString(cryBt).Replace("-", "");
        }

        /// <summary>
        /// MD5 加密算法
        /// </summary>
        /// <param name="s">需要加密字符串</param>
        /// <param name="md5Key">密钥</param>
        /// <returns></returns>
        public static string Md5Encrypt(string s, string md5Key)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            var en = new UTF8Encoding();

            byte[] md5Bt = en.GetBytes(s + md5Key);
            byte[] cryBt = md5.ComputeHash(md5Bt);
            return BitConverter.ToString(cryBt).Replace("-", "");
        }

        /// <summary>DES加密</summary>
        public static string DesEncrypt(string s)
        {
            byte[] byKey = Encoding.UTF8.GetBytes("wondersg");//wondersgroup
            byte[] iv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            var des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(s);
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateEncryptor(byKey, iv), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>DES解密</summary>
        public static string DesDecrypt(string s)
        {
            byte[] byKey = Encoding.UTF8.GetBytes("wondersg");
            byte[] iv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
            var des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Convert.FromBase64String(s);
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateDecryptor(byKey, iv), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            Encoding encoding = new UTF8Encoding();
            return encoding.GetString(ms.ToArray());
        }

        /// <summary>DES加密</summary>
        public static string DesEncrypt2(string s)
        {
            byte[] byKey = Encoding.UTF8.GetBytes("wondersg");//wondersgroup
            byte[] iv = { 0x0, 0x0, 0x0, 0x0, 0x0, 0x2, 0x3, 0x5 };
            var des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(s);
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateEncryptor(byKey, iv), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>DES解密</summary>
        public static string DesDecrypt2(string s)
        {
            byte[] byKey = Encoding.UTF8.GetBytes("wondersg");
            byte[] iv = { 0x0, 0x0, 0x0, 0x0, 0x0, 0x2, 0x3, 0x5 };
            var des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Convert.FromBase64String(s);
            var ms = new MemoryStream();
            var cs = new CryptoStream(ms, des.CreateDecryptor(byKey, iv), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            Encoding encoding = new UTF8Encoding();
            return encoding.GetString(ms.ToArray());
        }

        //// <summary>
        /// 转半角的函数(DBC case)
        /// 
        /// <param name="input">任意字符串</param>
        /// <returns>半角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>
        public static string ToDBC(string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        /// <summary>
        /// 世奇加密
        /// </summary>
        public static string UnknownENC(string s)
        {
            Encoding utf8 = Encoding.UTF8;
            byte[] bytes = utf8.GetBytes(s);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)(255 - bytes[i] - i);
            }
            return Convert.ToBase64String(bytes);
        }

        public static string ChangeToUnicode(string s)
        {
            byte[] buffer = Encoding.Unicode.GetBytes(s);
            return buffer.Aggregate("", (current, b) => current + string.Format("%{0:X}", b));
        }

        /// <summary>
        /// 转全角的函数(SBC case)
        /// </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>全角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>        
        public static string ToSBC(string input)
        {
            //半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }

        public static string GenderRandam(int length)
        {
            var rd = new Random();
            int ret = rd.Next(Convert.ToInt32(Math.Pow(10, length - 1)), Convert.ToInt32(Math.Pow(10, length)) - 1);
            return ret.ToString();
        }

        /// <summary>
        /// 过滤输入的单引号
        /// </summary>
        /// <param name="inputString">要处理的字符串</param>
        /// <returns></returns>
        public static string InputText(string inputString)
        {
            if (inputString.Trim() != "")
            {
                var retVal = new StringBuilder();
                inputString = inputString.Trim();
                if (inputString != String.Empty)
                {
                    foreach (char t in inputString)
                    {
                        switch (t)
                        {
                            case '\'':
                                retVal.Append("''");
                                break;
                                //case '"':
                                //    retVal.Append("&quot;");
                                //    break;
                                //case '<':
                                //    retVal.Append("&lt;");
                                //    break;
                                //case '>':
                                //    retVal.Append("&gt;");
                                //    break;
                            default:
                                retVal.Append(t);
                                break;
                        }
                    }
                    //retVal.Replace("\n", "<br/>");
                    //retVal.Replace(" ", "&nbsp;");
                }

                return retVal.ToString().Trim();
            }
            return "";
        }

        /// <summary>
        /// 过滤输入的单引号
        /// </summary>
        /// <param name="inputString">要处理的字符串</param>
        /// <returns></returns>
        public static string InputTextNotFilterSpace(string inputString)
        {
            var retVal = new StringBuilder();
            if (!string.IsNullOrEmpty(inputString))
            {
                foreach (char t in inputString)
                {
                    switch (t)
                    {
                        case '\'':
                            retVal.Append("''");
                            break;
                            //case '"':
                            //    retVal.Append("&quot;");
                            //    break;
                            //case '<':
                            //    retVal.Append("&lt;");
                            //    break;
                            //case '>':
                            //    retVal.Append("&gt;");
                            //    break;
                        default:
                            retVal.Append(t);
                            break;
                    }
                }
                //retVal.Replace("\n", "<br/>");
                //retVal.Replace(" ", "&nbsp;");
            }
            return retVal.ToString();
        }


        /// <summary>
        /// 过滤输入的单引号
        /// </summary>
        /// <param name="inputString">要处理的字符串</param>
        /// <returns></returns>
        public static string InputTextNew(string inputString)
        {
            if (inputString != "")
            {
                var retVal = new StringBuilder();
                if (!string.IsNullOrEmpty(inputString))
                {
                    foreach (char t in inputString)
                    {
                        switch (t)
                        {
                            case '\'':
                                retVal.Append("''");
                                break;
                            default:
                                retVal.Append(t);
                                break;
                        }
                    }
                }
                return retVal.ToString();
            }
            return "";
        }


        /// <summary>
        /// 剔除html标签
        /// </summary>
        public static string StripHtml(string strHtml)
        {
            var objRegExp = new Regex("<(.|\n)+?>");
            string strOutput = objRegExp.Replace(strHtml, "ÿ");
            strOutput = strOutput.Replace("&nbsp;", " ");
            strOutput = strOutput.Replace("<", "&lt;");
            strOutput = strOutput.Replace(">", "&gt;");
            return strOutput.Replace("\r\n", "");
        }

        /// <summary>
        /// 获取所有的href link
        /// </summary>
        public static string[] GetLink(string strHtml)
        {
            const string regexStr = "(?<=href[\\s\\r]*=[\\s\\r]*[\"\'])([^\"\'\\s\\r>]*)(?<=[\"\'\\s\\r]{0,1})";
            var reg = new Regex(regexStr, RegexOptions.IgnoreCase);
            MatchCollection mc = reg.Matches(strHtml);
            if (mc.Count > 0)
            {
                var strHref = new string[mc.Count];
                int i = 0;
                foreach (Match m in mc)
                {
                    strHref[i] = m.Groups[1].Value;
                    ++i;
                }
                return strHref;
            }
            return null;
        }

        /// <summary>
        /// 获取所有的Src link
        /// </summary>
        public static string[] GetSrcLink(string strHtml)
        {
            const string regexStr = "(?<=src[\\s\\r]*=[\\s\\r]*[\"\'])([^\"\'\\s\\r>]*)(?<=[\"\'\\s\\r]{0,1})";
            var reg = new Regex(regexStr, RegexOptions.IgnoreCase);
            MatchCollection mc = reg.Matches(strHtml);
            if (mc.Count > 0)
            {
                var strHref = new string[mc.Count];
                int i = 0;
                foreach (Match m in mc)
                {
                    strHref[i] = m.Groups[1].Value;
                    ++i;
                }
                return strHref;
            }
            return null;
        }

        /// <summary>
        /// 解析淘宝的json字符串，返回可以解析的json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string AnalysisTaoBaoJson(string json)
        {
            json = json.Substring(json.IndexOf('(') + 1);
            json = json.Substring(0, json.Length - 2);
            return json;
        }

        /// <summary>
        /// 将string转成float类型的值
        /// </summary>
        /// <param name="input">要转换的字符串</param>
        /// <param name="oneOrZero">OneOrZero=true的话返回-1，否则0</param>
        /// <returns></returns>
        public static float GetFloatTypePrice(string input, bool oneOrZero)
        {
            string strTmp = oneOrZero ? "-1" : "0";
            float result = float.Parse(strTmp);
            if (!string.IsNullOrEmpty(input))
            {
                //验证不为null
                input = input.Trim();
                if (!string.IsNullOrEmpty(input))
                {
                    //验证不为空
                    var num = new Regex(@"^[0-9.]*$");
                    Match isNum = num.Match(input);
                    if (isNum.Success)
                    {
                        //如果不是其他的非数值类型的，直接返回0 
                        result = float.Parse(input);
                    }
                }
            }
            return result;
        }

        public static string GetSystemVersionStringAndServerPack()
        {
            string systemVerSion = Environment.OSVersion.VersionString;
            return systemVerSion;
        }

        /// <summary>
        /// 截取字符串
        /// </summary>
        /// <param name="s"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static String BSubstring(string s, int length)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(s);
            int n = 0;  // 表示当前的字节数
            int i = 0;  // 要截取的字节数
            for (; i < bytes.GetLength(0) && n < length; i++)
            {

                // 偶数位置，如0、2、4等，为UCS2编码中两个字节的第一个字节

                if (i % 2 == 0)
                {
                    n++;      // 在UCS2第一个字节时n加1
                }
                else
                {

                    // 当UCS2编码的第二个字节大于0时，该UCS2字符为汉字，一个汉字算两个字节

                    if (bytes[i] > 0)
                    {
                        n++;
                    }
                }

            }

            // 如果i为奇数时，处理成偶数
            if (i % 2 == 1)
            {

                // 该UCS2字符是汉字时，去掉这个截一半的汉字
                if (bytes[i] > 0)

                    i = i - 1;

                // 该UCS2字符是字母或数字，则保留该字符

                else
                    i = i + 1;
            }
            return Encoding.Unicode.GetString(bytes, 0, i);
        }

        /// <summary>
        /// 验证是否是数值
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool CheckNum(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            var num = new Regex(@"^[0-9.]*$");
            return num.Match(input).Success;
        }

        /// <summary>
        /// 验证是否是数值
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool CheckNumWithOutDecimalPoint(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }
            var num = new Regex(@"^[0-9]*$");
            return num.Match(input).Success;
        }

        /// <summary>
        /// 获取自从1970.1.1截止目前的时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeTicks()
        {
            TimeSpan ts = DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (ts.Ticks / 10000).ToString();
        }

        /// <summary>
        /// 根据宝贝Url，获取淘宝的宝贝id，-1表示Url有误获取失败
        /// </summary>
        public static long GetItemId(string linkUrl)
        {
            System.Text.RegularExpressions.Regex pattern = new Regex("[?|&]id=(\\d*)");
            Match m = pattern.Match(linkUrl);
            if (m.Success)
            {
                return Convert.ToInt64(m.Groups[1].Value);
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 将Unicode转成中文的字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ChangeUnicodeToChinese(string str)
        {
            var reg = new Regex(@"(?i)\\u([0-9a-f]{4})");
            string outStr = reg.Replace(str, m1 => ((char) Convert.ToInt32(m1.Groups[1].Value, 16)).ToString());
            return outStr;
        }

        /// <summary>
        /// 验证是否存在注入代码(条件语句）
        /// </summary>
        public static bool HasInjectionData(string inputData)
        {
            if (string.IsNullOrEmpty(inputData))
                return false;

            //里面定义恶意字符集合
            //验证inputData是否包含恶意集合
            if (Regex.IsMatch(inputData.ToLower(), GetInjectionRegexString()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取正则表达式
        /// </summary>
        private static string GetInjectionRegexString()
        {
            //构造SQL的注入关键字符
            string[] strBadChar =
            {
                //"select\\s",
                //"from\\s",
                "insert\\s",
                "delete\\s",
                "update\\s",
                "drop\\s",
                "truncate\\s",
                "exec\\s",
                "count\\(",
                "declare\\s",
                "asc\\(",
                "mid\\(",
                "char\\(",
                "net user",
                "xp_cmdshell",
                "/add\\s",
                "exec master.dbo.xp_cmdshell",
                "net localgroup administrators"
            };

            //构造正则表达式
            string str_Regex = ".*(";
            for (int i = 0; i < strBadChar.Length - 1; i++)
            {
                str_Regex += strBadChar[i] + "|";
            }
            str_Regex += strBadChar[strBadChar.Length - 1] + ").*";

            return str_Regex;
        }

        /// <summary>
        /// 判断是否为数字
        /// </summary>
        public static bool IsShuZhi(string source)
        {
            if (source != null)
            {
                return Regex.IsMatch(source, @"^[0-9.]+$");
            }
            return false;
        }

        /// <summary>
        /// 判断是否是一个英文字母
        /// </summary>
        public static bool IsOneEnglishChar(string source)
        {
            if (source.Length == 1)
            {
                char s = source.ToCharArray()[0];
                return Char.IsLetter(s);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取字符串中非中文字符的个数（包括数字，英文字母，符号）
        /// </summary>
        public static int GetUnChineseCharNum(string str)
        { 
            int result=0;
            foreach (char c in str) //字符c遍历数组中的所有字符; 
            {
                if (c == '.' || c == '(' || c == ')' || c == '{' || c == '}' || c == '[' || c == ']')
                {//符号
                    result++;
                }
                if (char.IsLetter(c))
                {//是否为字母，如字母计数器加1; 
                    result++;
                }
                else if (char.IsDigit(c))
                {//是否为数字 如数字计数器加1; 
                    result++;
                }
            }

            return result;
        }

        /// <summary>
        /// 获取字符串中的中文
        /// </summary>
        public static string GetChineseString(string str)
        {
            string result = null;
            for (int i = 0; i < str.Length; i++)
            {
                Regex rx = new Regex("^[\\u4e00-\\u9fa5]+$");//中文字符unicode范围  
                if (rx.IsMatch(str[i].ToString()))
                {
                    result += str[i].ToString();
                }
            }
            return result; 
        }
    }
}
